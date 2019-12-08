/*
 * Main.cs
 * 
 * Main window for GUI Builder.
 * 
 */

// Uncomment this line to use the native Windows dialog to load a single (the working) plugin instead of the multi-plugin loader
//#define USE_SINGLE_LOADER

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace GUIBuilder.Windows
{
    
    /// <summary>
    /// Description of Main.
    /// </summary>
    public partial class Main : Form
    {
        
        // Minimum sizes of the main window and render window/panel
        readonly Size SIZE_ZERO = new Size( 0, 0 );
        readonly Size MIN_RENDER_SIZE = new Size( 700, 42 );
        readonly Size MAX_RENDER_SIZE = new Size( 65536, 42 );
        
        const string XmlNode = "MainWindow";
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        bool onLoadComplete = false;
        
        const string TIME_FORMAT = @"mm\:ss";
        
        #region Main Start/Stop
        
        public Main()
        {
            InitializeComponent();
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
        }
        
        bool _already_shutdown = false;
        void MainShutdown()
        {
            if( _already_shutdown )
                return;
            _already_shutdown = true;
            
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "MainShutdown()" } );
            
            ClearStatusBar();
            sbiCaption.Text = "Shutting down...";
            
            if( GodObject.Plugin.IsLoading )
            {
                DebugLog.WriteLine( "Waiting for Plugin Loader..." );
                while( GodObject.Plugin.IsLoading )
                    System.Threading.Thread.Sleep(100);
                DebugLog.WriteLine( "Plugin Loader finished" );
            }
            
            WorkerThreadPool.StopAllWorkers( true, true );
            
            DisposeOfSyncTimer();
            GodObject.Windows.CloseAllChildWindows();
            
            if( GodObject.Plugin.IsLoaded )
            {
                DebugLog.WriteLine( "Unloading plugin" );
                GodObject.Plugin.Unload();
            }
            GodObject.Plugin.Denit();
            
            DebugLog.WriteLine( "Application.Exit()" );
            Application.Exit();
            DebugLog.WriteLine( "Complete" );
            
            DebugLog.CloseIndentLevel();
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            if( !GodObject.Plugin.Initialize() )
            {
                MessageBox.Show( "Unable to find Fallout 4!\n\nMake sure you have the game installed correctly.", "GUIBuilder Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Application.Exit();
                return;
            }
            
            var bbPath = GodObject.Paths.BorderBuilder;
            if( string.IsNullOrEmpty( bbPath ) )
            {
                MessageBox.Show( string.Format( "Unable to find \"{0}\"\n\nMake sure you have the GUIBuilder installed correctly.", Constant.BorderBuilderPath ), "GUIBuilder Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Application.Exit();
                return;
            }
            
            var configFile = GodObject.Paths.GUIBuilderConfigFile;
            //Console.WriteLine( configFile );
            if( ( string.IsNullOrEmpty( configFile ) )||( !System.IO.File.Exists( configFile ) ) )
                GodObject.Windows.GetOptionsWindow( true );
            
            /*
            var configFile = GodObject.GUIBuilderConfigFilePath;
            if( string.IsNullOrEmpty( configFile ) )
            {
                MessageBox.Show( string.Format( "Unable to find \"{0}\\{1}\"\n\nMake sure you have the GUIBuilder installed correctly.", Constant.BorderBuilderPath, Constant.GUIBuilderConfigFile ), "GUIBuilder Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Application.Exit();
                return;
            }
            */
            
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadPoint( XmlNode, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlNode, XmlSize, this.Size );
            
            GodObject.Windows.SetMainWindow( this, false );
            
            ClearStatusBar();
            
            onLoadComplete = true;
            
        }
        
        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "OnFormClosing()" } );
            MainShutdown();
            DebugLog.CloseIndentLevel();
        }
        
        void OnMenuExitClick( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "OnMenuExitClick()" } );
            MainShutdown();
            DebugLog.CloseIndentLevel();
        }
        
        void OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WritePoint( XmlNode, XmlLocation, this.Location, true );
        }
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( XmlNode, XmlSize, this.Size, true );
        }
        
        #endregion
        
        #region Global Form and Status bar update
        
        /// <summary>
        /// Long running functions should disable the main form so the user can't spam inputs.  Don't forget to enable the form again after the long-running function is complete so the user can continue to use the program.
        /// </summary>
        /// <param name="enabled">true to enable the form and it's controls, false to disable the form and it's controls.</param>
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            mbMain.Enabled = enabled;
            if( GodObject.Plugin.IsLoaded )
            {
                mbiToolsBorderBatch.Enabled = enabled;
                mbiToolsSubDivisionBatch.Enabled = ( enabled )&&( GodObject.Master.AnnexTheCommonwealth.Loaded );
                mbiToolsRendererWindow.Enabled = enabled;
            }
        }
        
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
            if( messageHistory.NullOrEmpty() ) messageHistory = new List<string>();
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
            if( this.InvokeRequired )
            {
                this.BeginInvoke( (Action)delegate() { SetCurrentStatusMessage( message, logEcho ); }, null );
                return;
            }
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
                syncTimer.Interval = 250;       // 4 updates per second is plenty enough
                syncTimer.Tick += OnSyncTimerTick;
                syncTimer.Start();
            }
        }
        
        public void StopSyncTimer( string consoleLog = null, long start = 0 )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { StopSyncTimer( consoleLog, start ); }, null );
                return;
            }
            var elapsed = syncStopwatch != null
                ? syncStopwatch.Elapsed.Ticks - start
                : 0;
            if( !string.IsNullOrEmpty( consoleLog ) )
            {
                //if( !consoleLog.Contains( "{0}" ) )
                //    consoleLog += " :: Completed in {0}";
                var tmp = new TimeSpan( elapsed );
                DebugLog.WriteLine( string.Format( consoleLog, tmp.ToString() ) );
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
        
        void OnSyncTimerTick( object sender, EventArgs e )
        {
            if( ( syncTimer == null )||( syncStopwatch == null ) ) return;
            sbiTimeElapsed.Text = syncStopwatch.Elapsed.ToString( TIME_FORMAT );
        }
        
        #endregion
        
        #endregion
        
        #region Load ESM/ESP
        
        void OnMenuCloseFileClick( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "OnMenuCloseFileClick()" } );
            GodObject.Windows.SetEnableState( false );
            
            if( ( GodObject.Plugin.IsLoading )||( !GodObject.Plugin.IsLoaded ) )
                goto localReturnResult;
            
            GodObject.Plugin.Unload();
            
            this.Translate();
            
        localReturnResult:
            GodObject.Windows.SetEnableState( true );
        }
        
        void OnMenuFileSaveClick( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "OnMenuFileSaveClick()" } );
            GodObject.Windows.SetEnableState( false );
            
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
                DebugLog.WriteError( this.GetType().ToString(), "OnMenuFileSaveClick()", saveMsg );
                MessageBox.Show( saveMsg, "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            else if( !string.IsNullOrEmpty( saveMsg ) )
                DebugLog.WriteLine( saveMsg );
            
            saveMsg = string.Format( "MainWindow.FileSaved".Translate(), wfn );
            DebugLog.WriteLine( saveMsg );
            SetCurrentStatusMessage( saveMsg );
            
        localReturnResult:
            GodObject.Windows.SetEnableState( true );
            DebugLog.CloseIndentLevel();
        }
        
        void OnMenuLoadESMESPClick( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "OnMenuLoadESMESPClick()" } );
            GodObject.Windows.SetEnableState( false );
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
                reEnableGUI &= !GodObject.Plugin.Load( wf, sp, orwol );
            }
            
        localReturnResult:
            if( reEnableGUI )
                GodObject.Windows.SetEnableState( true );
            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Child Tool Windows
        
        void mbiWindowsBorderBatchClick( object sender, EventArgs e )
        {
            if(
                ( !GodObject.Plugin.IsLoaded )||
                (
                    ( GodObject.Plugin.Data.SubDivisions.Count == 0 )&&
                    ( GodObject.Plugin.Data.Workshops.Count == 0 )
                )
            )
                return;
            
            //GodObject.Plugin.DoSampleReading();
            
            GodObject.Windows.GetBorderBatchWindow( true );
        }
        
        void mbiToolsSubDivisionBatchClick( object sender, EventArgs e )
        {
            if(
                ( !GodObject.Plugin.IsLoaded )||
                (
                    ( GodObject.Plugin.Data.SubDivisions.Count == 0 )&&
                    ( GodObject.Plugin.Data.Workshops.Count == 0 )
                )
            )
                return;
            
            //GodObject.Plugin.DoSampleReading();
            
            GodObject.Windows.GetSubDivisionBatchWindow( true );
        }
        
        void mbiWindowsRendererClick( object sender, EventArgs e )
        {
            if( !GodObject.Plugin.IsLoaded ) return;
            GodObject.Windows.GetRenderWindow( true );
        }
        
        #endregion
        
        #region Child Config Windows
        
        void mbiWindowsAboutClick( object sender, EventArgs e )
        {
            GodObject.Windows.GetAboutWindow( true );
        }
        
        void mbiToolsOptionsClick( object sender, EventArgs e )
        {
            GodObject.Windows.GetOptionsWindow( true );
        }
        
        #endregion
        
    }
}
