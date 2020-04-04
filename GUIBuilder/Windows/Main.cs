/*
 * Main.cs
 * 
 * Main window for GUI Builder.
 * 
 */

// Uncomment this line to use the native Windows dialog to load a single (the working) plugin instead of the multi-plugin loader
// This also effects the workspace file selector
//#define USE_SINGLE_LOADER

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace GUIBuilder.Windows
{

    /// <summary>
    /// Use Application.Run( new GUIBuilder.Windows.Min() ) to create this Window
    /// Use GodObject.Windows.GetWindow<Main>() to get this Window
    /// </summary>
    public partial class Main : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {

        // Minimum sizes of the main window and render window/panel
        readonly Size SIZE_ZERO = new Size( 0, 0 );
        readonly Size MIN_RENDER_SIZE = new Size( 700, 42 );
        readonly Size MAX_RENDER_SIZE = new Size( 65536, 42 );

        bool onLoadComplete = false;
        public bool OnLoadComplete { get { return onLoadComplete; } }

        const string TIME_FORMAT = @"mm\:ss";

        bool everythingUnloaded = true;

        #region Main Start/Stop
        
        public Main()
        {
            InitializeComponent();

            this.SuspendLayout();

            // Calculate form size based on the current Windows theme
            var fbs = Maths.GenSize.Multiply( SystemInformation.FrameBorderSize, 2 );
            var cs = new Size(
                0,
                SystemInformation.CaptionHeight );
            var rbt = new Size(
                SystemInformation.HorizontalResizeBorderThickness,
                SystemInformation.VerticalResizeBorderThickness );
            var wfs = fbs + cs + rbt;
            this.MinimumSize = MIN_RENDER_SIZE + wfs;
            this.MaximumSize = MAX_RENDER_SIZE + wfs;

            this.Load += new System.EventHandler( this.OnClientLoad );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnClientClosing );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.OnClientClosed );

            this.mbiFileLoadWorkspace.Click += new System.EventHandler( this.OnLoadWorkspaceClick );
            this.mbiFileCreateWorkspace.Click += new System.EventHandler( this.OnCreateWorkspaceClick );
            
            this.mbiFileLoadPlugin.Click += new System.EventHandler( this.OnLoadPluginClick );
            this.mbiFileSavePlugin.Click += new System.EventHandler( this.OnSavePluginClick );
            
            this.mbiFileCloseFiles.Click += new System.EventHandler( this.OnCloseFilesClick );
            this.mbiFileExit.Click += new System.EventHandler( this.OnMenuExitClick );
            
            this.mbiToolsSubDivisionBatch.Click += new System.EventHandler( this.OnSubDivisionBatchWindowClick );
            this.mbiToolsBorderBatch.Click += new System.EventHandler( this.OnBorderBatchWindowClick );
            this.mbiToolsRenderWindow.Click += new System.EventHandler( this.OnRenderWindowClick );
            this.mbiToolsAbout.Click += new System.EventHandler( this.OnAboutWindowClick );
            this.mbiToolsOptions.Click += new System.EventHandler( this.OnOptionsWindowClick );
            this.mbiToolsCustomForms.Click += new System.EventHandler( this.OnCustomFormsWindowClick );
            
            this.ResumeLayout( false );
        }
        
        bool _already_shutdown = false;
        void MainShutdown()
        {
            if( _already_shutdown )
                return;
            _already_shutdown = true;

            GodObject.Windows.SetEnableState( this, false );
            
            DebugLog.OpenIndentLevel();

            var thread = new System.Threading.Thread( THREAD_MainShutdown )
            {
                Name = GenString.FormatMethod( this.GetType().GetMethodBase( "THREAD_MainShutdown" ), null, false, true, true ).ReplaceInvalidFilenameChars()
            };
            thread.Start();
        }

        void THREAD_MainShutdown()
        {
            WorkerThreadPool.StartMethodBase = System.Reflection.MethodInfo.GetCurrentMethod();
            //Console.WriteLine( "GUIBuilder.Windows.Main.MainShutdown()" );

            SetCurrentStatusMessageEx( "MainWindow.Shutdown".Translate() );

            /* Plugin loader is a worker thread which will get stopped below, this way means that we have to wait for the loader
            if( GodObject.Plugin.IsLoading )
            {
                DebugLog.WriteLine( "Waiting for Plugin Loader..." );
                while( GodObject.Plugin.IsLoading )
                    System.Threading.Thread.Sleep(100);
                DebugLog.WriteLine( "Plugin Loader finished" );
            }
            */

            WorkerThreadPool.StopAllWorkers( true, true );
            
            GodObject.Windows.CloseAllChildWindows();

            bool terminateScheduled = false;

            if( GodObject.Plugin.IsLoaded )
            {
                DebugLog.WriteLine( "Unloading plugin" );
                terminateScheduled = GodObject.Plugin.Unload( MainShutdownTerminate );
            }

            if( !terminateScheduled )
                MainShutdownTerminate();

            DebugLog.Close();
        }

        void MainShutdownTerminate()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate () { MainShutdownTerminate(); }, null );
                return;
            }

            ClearStatusBar();
            DisposeOfSyncTimer();

            GodObject.Plugin.Denit();

            everythingUnloaded = true;

            DebugLog.WriteLine( "Application.Exit()" );
            Application.Exit();

            DebugLog.WriteLine( "Complete" );
            DebugLog.CloseIndentLevel();
        }

        void OnClientLoad( object sender, EventArgs e )
        {
            SetEnableState( this, false );

            this.Location       = GodObject.XmlConfig.ReadLocation( this );
            this.Size           = GodObject.XmlConfig.ReadSize( this );

            this.ResizeEnd      += new System.EventHandler( this.IXmlConfiguration_OnFormResizeEnd );
            this.Move           += new System.EventHandler( this.IXmlConfiguration_OnFormMove );

            this.Translate( true );
            
            GodObject.Windows.SetWindow<Main>( this );
            
            ClearStatusBar();
            onLoadComplete = true;
            SetEnableState( this, true );
        }

        void OnClientClosing( object sender, FormClosingEventArgs e )
        {
            e.Cancel = !everythingUnloaded;
            if( e.Cancel )
            {
                if( _already_shutdown )
                {
                    nextShutdownMessageReminder = syncStopwatch.Elapsed.Seconds + 4;
                    SetCurrentStatusMessageEx( "MainWindow.AlreadyShutdown".Translate() );
                }
                else
                    MainShutdown();
            }
        }

        void OnClientClosed( object sender, FormClosedEventArgs e )
        {
            GodObject.Windows.ClearWindow<Main>();
        }

        void OnMenuExitClick( object sender, EventArgs e )
        {
            MainShutdown();
        }

        void OnCloseFilesClick( object sender, EventArgs e )
        {
            //DebugLog.OpenIndentLevel( new [] { this.TypeFullName(), "mbiFileCloseFilesClick()" } );
            GodObject.Windows.SetEnableState( this, false );

            var result = true;

            if( ( GodObject.Plugin.IsLoading ) || ( !GodObject.Plugin.IsLoaded ) )
                goto localReturnResult;

            result = GodObject.Plugin.Unload( INTERNAL_SDL_SyncThread_OnPluginCloseComplete );

        localReturnResult:
            if( !result )
                GodObject.Windows.SetEnableState( this, true );
            //DebugLog.CloseIndentLevel();
        }

        void INTERNAL_SDL_SyncThread_OnPluginCloseComplete()
        {
            everythingUnloaded = true;
            this.Translate();
            GodObject.Windows.SetEnableState( this, true );
        }


        #region GUIBuilder.Windows.IEnableControlForm


        #region Interface

        public event GUIBuilder.Windows.SetEnableStateHandler  OnSetEnableState;

        /// <summary>
        /// Enable or disable this windows main panel.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        public bool SetEnableState( object sender, bool enable )
        {
            if( this.InvokeRequired )
                return (bool)this.Invoke( (Func<bool>)delegate () { return SetEnableState( sender, enable ); }, null );

            bool tryEnable = OnLoadComplete && enable;
            bool enabled = OnSetEnableState != null
                ? OnSetEnableState( sender, tryEnable )
                : tryEnable;

            if( GodObject.Plugin.IsLoaded )
            {
                mbiFileCreateWorkspace.Enabled = ( enabled ) && ( GodObject.Plugin.Workspace == null );
                mbiFileLoadWorkspace.Enabled = false;
                mbiFileLoadPlugin.Enabled = false;
                mbiFileSavePlugin.Enabled = enabled;
                mbiFileCloseFiles.Enabled = enabled;
                mbiToolsBorderBatch.Enabled = enabled;
                mbiToolsSubDivisionBatch.Enabled = ( enabled ) && ( GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth ) );
                mbiToolsRenderWindow.Enabled = enabled;
                mbiToolsCustomForms.Enabled = enabled;
            }
            else
            {
                mbiFileCreateWorkspace.Enabled = false;
                mbiFileLoadWorkspace.Enabled = enabled;
                mbiFileLoadPlugin.Enabled = enabled;
                mbiFileSavePlugin.Enabled = false;
                mbiFileCloseFiles.Enabled = false;
                mbiToolsBorderBatch.Enabled = false;
                mbiToolsSubDivisionBatch.Enabled = false;
                mbiToolsRenderWindow.Enabled = false;
                mbiToolsCustomForms.Enabled = false;
            }

            mbMain.Enabled = enabled;
            
            return enabled;
        }

        #endregion


        #endregion


        #region GodObject.XmlConfig.IXmlConfiguration


        #region Internal

        void IXmlConfiguration_OnFormMove( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteLocation( this );
        }

        void IXmlConfiguration_OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteSize( this );
        }

        #endregion


        #region Interface

        public GodObject.XmlConfig.IXmlConfiguration XmlParent
        {
            get { return null; }
        }

        public string XmlNodeName
        {
            get { return "MainWindow"; }
        }

        #endregion


        #endregion

        #endregion

        #region Global Form and Status bar update

        void ClearStatusBar()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { ClearStatusBar(); }, null );
                return;
            }
            messageHistory = null;
            sbiTimeElapsed.Text = "";
            sbiTimeEstimated.Text = "";
            sbiTimeEstimated.Visible = false;
            sbiCaption.Text = "";
            sbiItemOfItems.Text = "";
        }
        
        #region Message History
        
        List<string> messageHistory = null;
        public void PushStatusMessage()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { PushStatusMessage(); }, null );
                return;
            }
            messageHistory = messageHistory ?? new List<string>();
            messageHistory.Add( sbiCaption.Text );
        }
        public void PopStatusMessage()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { PopStatusMessage(); }, null );
                return;
            }
            if( messageHistory.NullOrEmpty() )
            {
                sbiCaption.Text = "";
                return;
            }
            var index = messageHistory.Count - 1;
            sbiCaption.Text = messageHistory[ index ];
            messageHistory.RemoveAt( index );
        }
        public void SetCurrentStatusMessage( string message, bool logEcho = false )
        {
            if( !string.IsNullOrEmpty( message ) )
            {
                if( logEcho ) DebugLog.WriteLine( message );
                var CRLF = new char[]{ '\n', '\r' };
                var lastCRLR = message.LastIndexOfAny( CRLF );
                while( lastCRLR > -1 )
                {   // Get the last line in the message with actual contents
                    var splitOff = message.Substring( lastCRLR + 1 );
                    if( !string.IsNullOrEmpty( splitOff ) )
                    {
                        message = splitOff;
                        break;
                    }
                    message = message.Substring( 0, lastCRLR - 1 );
                    lastCRLR = message.LastIndexOfAny( CRLF );
                }
                SetCurrentStatusMessageEx( message );
            }
        }

        void SetCurrentStatusMessageEx( string message )
        {
            if( this.InvokeRequired )
            {
                this.BeginInvoke( (Action)delegate () { SetCurrentStatusMessageEx( message ); }, null );
                return;
            }
            sbiCaption.Text = message;
        }
        
        #endregion
        
        #region Item of Items
        
        List<string> itemOfItemsHistory = null;
        public void PushItemOfItems()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { PushItemOfItems(); }, null );
                return;
            }
            if( itemOfItemsHistory.NullOrEmpty() ) itemOfItemsHistory = new List<string>();
            itemOfItemsHistory.Add( sbiItemOfItems.Text );
        }
        public void PopItemOfItems()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { PopItemOfItems(); }, null );
                return;
            }
            sbiTimeEstimated.Visible = false;
            if( itemOfItemsHistory.NullOrEmpty() )
            {
                sbiItemOfItems.Text = "";
                return;
            }
            var index = itemOfItemsHistory.Count - 1;
            sbiItemOfItems.Text = itemOfItemsHistory[ index ];
            itemOfItemsHistory.RemoveAt( index );
        }
        public void SetItemOfItems( int value, int max, long elapsed = -1, bool logEcho = false )
        {
            if( this.InvokeRequired )
            {
                this.BeginInvoke( (Action)delegate() { SetItemOfItems( value, max, elapsed, logEcho ); }, null );
                return;
            }
            var message = string.Format( "{0}/{1}", value, max );
            sbiItemOfItems.Text = message;
            if( ( elapsed > 0 )&&( value > 0 ) )
            {
                var ticksPerItem = elapsed / value;
                var ticksForAll = ticksPerItem * max;
                var et = new TimeSpan( ticksForAll );
                var em = string.Format( "/ {0}", et.ToString( TIME_FORMAT ) );
                sbiTimeEstimated.Text = em;
                sbiTimeEstimated.Visible = true;
                message = message + " " + em;
            }
            if( logEcho ) DebugLog.WriteLine( message );
        }
        
        #endregion
        
        #region Global sync timer
        
        int SyncTimerCounter = 0;
        System.Windows.Forms.Timer syncTimer = null;
        System.Diagnostics.Stopwatch syncStopwatch = null;
        
        public void StartSyncTimer()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { StartSyncTimer(); }, null );
                return;
            }
            SyncTimerCounter++;
            if( syncStopwatch == null )
            {
                syncStopwatch = new System.Diagnostics.Stopwatch();
                syncStopwatch.Start();
            }
            if( syncTimer == null )
            {
                syncTimer = new Timer();
                syncTimer.Interval = 1000;       // 1 update per second is plenty enough
                syncTimer.Tick += OnSyncTimerTick;
                syncTimer.Start();
            }
        }
        
        public long StopSyncTimer( TimeSpan start, string logExtra = null, bool prefixCallerId = false )
        {
            var elapsed = syncStopwatch != null
                ? syncStopwatch.Elapsed.Ticks - start.Ticks
                : 0;
            var callerId = prefixCallerId ? GenString.GetCallerId( 1, null, false, false, true ) : null;
            var tmp = new TimeSpan( elapsed );
            DebugLog.WriteStrings( null, new string[] { callerId, logExtra, string.Format( "Completed in {0}", tmp.ToString() ) }, false, true, false, false, false );
            StopSyncTimerEx();
            return elapsed;
        }
        
        void StopSyncTimerEx()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { StopSyncTimerEx(); }, null );
                return;
            }
            SyncTimerCounter--;
            if( SyncTimerCounter <= 0 )
                DisposeOfSyncTimer();
        }
        
        public TimeSpan SyncTimerElapsed()
        {
            return( syncStopwatch == null )
                ? TimeSpan.Zero
                : syncStopwatch.Elapsed;
        }
        
        void DisposeOfSyncTimer()
        {
            SyncTimerCounter = 0;
            sbiTimeElapsed.Text = "";
            if( syncTimer != null )
            {
                syncTimer.Stop();
                syncTimer.Dispose();
                syncTimer = null;
            }
            if( syncStopwatch != null )
            {
                syncStopwatch.Stop();
                syncStopwatch = null;
            }
        }

        static long nextShutdownMessageReminder = 0;
        void OnSyncTimerTick( object sender, EventArgs e )
        {
            if( ( syncTimer == null )||( syncStopwatch == null ) ) return;
            sbiTimeElapsed.Text = syncStopwatch.Elapsed.ToString( TIME_FORMAT );
            if( _already_shutdown )
            {
                if( syncStopwatch.Elapsed.Seconds > nextShutdownMessageReminder )
                {
                    nextShutdownMessageReminder = syncStopwatch.Elapsed.Seconds + 4;
                    SetCurrentStatusMessageEx( "MainWindow.Shutdown".Translate() );
                }
            }
        }
        
        #endregion
        
        #endregion
        
        #region Save/Load Plugin

        void OnSavePluginClick( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel();
            GodObject.Windows.SetEnableState( this, false );
            
            if( !GodObject.Plugin.IsLoaded )
                goto localReturnResult;
            
            var wf = GodObject.Plugin.Data.Files.Working;
            if( wf == null )
                goto localReturnResult; // Shouldn't happen but just to be sure
            
            var wfn = wf.Filename;
            //var wfn = GodObject.Fallout4DataPath + @"\ATC_test.esp";
            var saveMsg = string.Format( "MainWindow.SavingFile".Translate(), wfn );
            DebugLog.WriteLine( saveMsg );
            SetCurrentStatusMessage( saveMsg );
            
            var result = wf.Save();
            saveMsg = XeLib.API.Messages.GetMessages();
            if( !result )
            {
                saveMsg = string.Format( "Unable to save \"{0}\"\n{1}", wfn, saveMsg );
                DebugLog.WriteError( saveMsg );
                MessageBox.Show( saveMsg, "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            else if( !string.IsNullOrEmpty( saveMsg ) )
                DebugLog.WriteLine( saveMsg );
            
            saveMsg = string.Format( "MainWindow.FileSaved".Translate(), wfn );
            DebugLog.WriteLine( saveMsg );
            SetCurrentStatusMessage( saveMsg );
            
        localReturnResult:
            GodObject.Windows.SetEnableState( this, true );
            DebugLog.CloseIndentLevel();
        }
        
        void OnLoadPluginClick( object sender, EventArgs e )
        {
            //DebugLog.OpenIndentLevel( new [] { this.TypeFullName(), "mbiFileLoadPluginClick()" } );
            GodObject.Windows.SetEnableState( this, false );
            var reEnableGUI = true;
            
            if( ( GodObject.Plugin.IsLoading )||( GodObject.Plugin.IsLoaded ) )
                goto localReturnResult;
            
            GodObject.Windows.CloseAllChildWindows();
            
#if USE_SINGLE_LOADER
            var dlg = new OpenFileDialog();
            dlg.Title = "Select Bethesda file to work with";
            dlg.Filter = "Bethesda Plugin File|*.esp|Bethesda Master File|*.esm|All Files|*.*";
            dlg.InitialDirectory = GodObject.Paths.Fallout4Data;
            dlg.RestoreDirectory = true;
            dlg.DereferenceLinks = true;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            
#else
            var dlg = new PluginSelector();
            
#endif
            
#if USE_SINGLE_LOADER
            if( ( dlg.ShowDialog() == DialogResult.OK )&&( !string.IsNullOrEmpty( dlg.FileName ) ) )
#else
            if( ( dlg.ShowDialog() == DialogResult.OK )&&( !dlg.SelectedPlugins.NullOrEmpty() )&&( !string.IsNullOrEmpty( dlg.WorkingFile ) ) )
#endif
            {
                ClearStatusBar();
                
#if USE_SINGLE_LOADER
                string path;
                var wf = GenFilePath.FilenameFromPathname( dlg.FileName, out path );
                var sp = new List<string>();
                var orwol = true;
                
                foreach( var master in GodObject.Master.Files )
                    if( master.AlwaysSelect )
                        sp.Add( master.Filename );
                if( !sp.Contains( wf ) )
                    sp.Add( wf );
                
#else
                var wf = dlg.WorkingFile;
                var sp = dlg.SelectedPlugins;
                var orwol = dlg.OpenRenderWindowOnLoad;

#endif

                // If the plugin loader returns true, the loader thread will re-enable the GUI
                everythingUnloaded = !GodObject.Plugin.Load( wf, sp, orwol );
                reEnableGUI &= everythingUnloaded;
            }
            
        localReturnResult:
            if( reEnableGUI )
                GodObject.Windows.SetEnableState( this, true );
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Child Tool Windows
        
        void OnBorderBatchWindowClick( object sender, EventArgs e )
        {
            if(
                ( !GodObject.Plugin.IsLoaded )||
                (
                    ( GodObject.Plugin.Data.SubDivisions.Count == 0 )&&
                    ( GodObject.Plugin.Data.Workshops.SyncedGUIList.Count == 0 )
                )
            )
                return;
            
            //GodObject.Plugin.DoSampleReading();
            
            GodObject.Windows.GetWindow<GUIBuilder.Windows.BorderBatch>( true );
        }
        
        void OnSubDivisionBatchWindowClick( object sender, EventArgs e )
        {
            if(
                ( !GodObject.Plugin.IsLoaded )||
                (
                    ( GodObject.Plugin.Data.SubDivisions.Count == 0 )&&
                    ( GodObject.Plugin.Data.Workshops.SyncedGUIList.Count == 0 )
                )
            )
                return;
            
            //GodObject.Plugin.DoSampleReading();
            
            GodObject.Windows.GetWindow<GUIBuilder.Windows.SubDivisionBatch>( true );
        }
        
        void OnRenderWindowClick( object sender, EventArgs e )
        {
            if( !GodObject.Plugin.IsLoaded ) return;
            GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( true );
        }
        
        #endregion
        
        #region Child Config Windows
        
        void OnAboutWindowClick( object sender, EventArgs e )
        {
            GodObject.Windows.GetWindow<GUIBuilder.Windows.About>( true );
        }
        
        void OnOptionsWindowClick( object sender, EventArgs e )
        {
            GodObject.Windows.GetWindow<GUIBuilder.Windows.Options>( true );
        }
        
        void OnCustomFormsWindowClick( object sender, EventArgs e )
        {
            GodObject.Windows.GetWindow<GUIBuilder.Windows.CustomForms>( true );
        }

        #endregion

        #region Load/Create Workspace

        void OnLoadWorkspaceClick( object sender, EventArgs e )
        {
            //DebugLog.OpenIndentLevel( new [] { this.TypeFullName(), "mbiFileLoadWorkspaceClick()" } );

            GodObject.Windows.SetEnableState( this, false );
            var reEnableGUI = true;
            
            if( ( GodObject.Plugin.IsLoading )||( GodObject.Plugin.IsLoaded ) )
                goto localReturnResult;
            
            GodObject.Windows.CloseAllChildWindows();
            
#if USE_SINGLE_LOADER
            var dlg = new OpenFileDialog();
            dlg.Title = "Select GUIBuilder Workspace to load";
            dlg.Filter = "eXtensible Markup Language|*.xml|All Files|*.*";
            dlg.InitialDirectory = GodObject.Paths.Fallout4Data;
            dlg.RestoreDirectory = true;
            dlg.DereferenceLinks = true;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            
#else
            var dlg = new WorkspaceSelector();
            
#endif
            
#if USE_SINGLE_LOADER
            if( ( dlg.ShowDialog() == DialogResult.OK )&&( !string.IsNullOrEmpty( dlg.FileName ) ) )
#else
            if( ( dlg.ShowDialog() == DialogResult.OK )&&( !string.IsNullOrEmpty( dlg.SelectedWorkspace ) ) )
#endif
            {
                ClearStatusBar();
                
#if USE_SINGLE_LOADER
                string path;
                var sws = GenFilePath.FilenameFromPathname( dlg.FileName, out path );
#else
                var sws = dlg.SelectedWorkspace;
#endif

                // If the plugin loader returns true, the loader thread will re-enable the GUI
                everythingUnloaded = !GodObject.Plugin.Load( sws );
                reEnableGUI &= everythingUnloaded;
            }
            
        localReturnResult:
            if( reEnableGUI )
                GodObject.Windows.SetEnableState( this, true );
            //DebugLog.CloseIndentLevel();
        }
        
        void OnCreateWorkspaceClick( object sender, EventArgs e )
        {
            if( ( !GodObject.Plugin.IsLoaded )||( GodObject.Plugin.IsLoading ) )
            {
                MessageBox.Show(
                    "WorkspaceCreateWait.Body".Translate(),
                    "WorkspaceCreateWait.Title".Translate(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation );
                return;
            }
            if( GodObject.Plugin.Workspace != null )
            {
                MessageBox.Show(
                    "WorkspaceCreateAlreadyLoaded.Body".Translate(),
                    "WorkspaceCreateAlreadyLoaded.Title".Translate(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation );
                return;
            }
            mbiFileCreateWorkspace.Enabled = !GodObject.Plugin.CreateWorkspace();
        }

        #endregion

    }
}
