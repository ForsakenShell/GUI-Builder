/*
 * BatchImport.cs
 *
 * Batch import window.
 *
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using AnnexTheCommonwealth;

namespace GUIBuilder.Windows
{

    /// <summary>
    /// Use GodObject.Windows.GetWindow<BatchImport>() to create this Window
    /// </summary>
    public partial class BatchImport : WindowBase
    {

        public BatchImport() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();

            this.ClientLoad += new System.EventHandler( this.OnClientLoad );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnClientClosing );

            this.lvImportForms.OnSetSyncObjectsThreadComplete += OnSyncImportThreadComplete;

            this.scImports.SplitterMoved += new System.Windows.Forms.SplitterEventHandler( this.OnImportsSplitterMoved );
            this.btnImportSelected.Click += new System.EventHandler( this.OnImportButtonClick );
            this.btnClose.Click += new System.EventHandler( this.OnCloseButtonClick );

            lvImportForms.CustomAscendingSort = PrioritySortAsc;
            lvImportForms.CustomDescendingSort = PrioritySortDes;

            this.ResumeLayout( false );
        }


        const string XmlKey_SplitterOffset = "SplitterOffset";


        public bool AllImportsMatchTarget = false;
        public bool EnableControlsOnClose = true;
        public List<FormImport.ImportBase> ImportForms = null;



        #region Window management


        void OnClientLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( string.Format( "\n{0} :: OnFormLoad() :: Start", this.FullTypeName() ) );

            scImports.SplitterDistance = GodObject.XmlConfig.ReadValue<int>( null, XmlNodeName, XmlKey_SplitterOffset, scImports.SplitterDistance );

            // This is a modal window which is created, data added, then displayed
            // At this point we just need to sort the data and populate the form

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            
            this.BringToFront();
            
            /*
            DebugLog.WriteLine( string.Format( "ImportForms = {0}", ImportForms.NullOrEmpty() ? 0 : ImportForms.Count ) );
            if( !ImportForms.NullOrEmpty() )
            {
                foreach( var i in ImportForms )
                    DebugLog.WriteLine( string.Format( "\t{0} : 0x{1} - \"{2}\"", i.Signature, i.FormID.ToString( "X8" ), i.EditorID ) );
            }
            */
            
            m.SetCurrentStatusMessage( "BatchImportWindow.Analyzing".Translate() );
            lvImportForms.SyncObjects = ImportForms;
            //SortImportForms();
            //RepopulateImportListView( false );
            
            m.PopStatusMessage();
            
            //DebugLog.Write( string.Format( "\n{0} :: OnFormLoad() :: Complete", this.FullTypeName() ) );
        }

        void OnClientClosing( object sender, FormClosingEventArgs e )
        {
            AllImportsMatchTarget =
                ( ImportForms.NullOrEmpty() )||
                ( FormImport.ImportBase.AllImportsMatchTarget( ImportForms ) );
            //var m = GodObject.Windows.GetMainWindow();
            //m.PopStatusMessage();
            if( EnableControlsOnClose )
                GodObject.Windows.SetEnableState( sender, true );
        }
        
        
        void OnCloseButtonClick( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.None;
            this.Close();
        }
        
        void OnImportButtonClick( object sender, EventArgs e )
        {
            var thread = WorkerThreadPool.CreateWorker( THREAD_ImportSelectedListViewItems, null );
            if( thread != null )
                thread.Start();
        }
        
        void OnImportsSplitterMoved( object sender, SplitterEventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteValue<int>( XmlNodeName, XmlKey_SplitterOffset, e.SplitY, true );
        }

        void OnSyncImportThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase> sender )
        {
            SetEnableState( sender, true );
        }

        #endregion

        #region Import Forms <-> Import List (ListViewItem)

        public static int PrioritySortImportAsc( FormImport.ImportBase x, FormImport.ImportBase y )
        {
            return
                x.InjectPriority > y.InjectPriority
                ? 1
                : x.InjectPriority < y.InjectPriority
                ? -1
                : string.Compare( x.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), y.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), StringComparison.InvariantCultureIgnoreCase );
        }
        
        public static int PrioritySortImportDes( FormImport.ImportBase x, FormImport.ImportBase y )
        {
            return
                x.InjectPriority > y.InjectPriority
                ? -1
                : x.InjectPriority < y.InjectPriority
                ? 1
                : string.Compare( x.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), y.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), StringComparison.InvariantCultureIgnoreCase );
        }
        
        public static int PrioritySortAsc(
           GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase>.SyncItem x,
           GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase>.SyncItem y )
        {
            return PrioritySortImportAsc( x.GetSyncObject(), y.GetSyncObject() );
        }
        
        public static int PrioritySortDes(
           GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase>.SyncItem x,
           GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase>.SyncItem y )
        {
            return PrioritySortImportDes( x.GetSyncObject(), y.GetSyncObject() );
        }
        
        void SortImportForms( List<FormImport.ImportBase> list, bool ascending )
        {
            DebugLog.WriteCaller( false );
            if( ascending )
                list.Sort( PrioritySortImportAsc );
            else
                list.Sort( PrioritySortImportDes );
        }
        
        public void AddImportMessage( string message )
        {
            if( string.IsNullOrEmpty( message ) ) return;
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { AddImportMessage( message ); }, null );
                return;
            }
            var lines = message.Split( '\n' );
            foreach( var line in lines )
                if( !string.IsNullOrEmpty( line ) )
                    tbImportMessages.AppendText( line + "\r\n" );
            tbImportMessages.Refresh();
            tbImportMessages.ScrollToCaret();
        }

        #endregion

        #region Import the Forms

        void THREAD_ImportSelectedListViewItems()
        {
            SetEnableState( this, false );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            tbImportMessages.Clear();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            //DebugLog.Write( string.Format( "\n{0} :: ImportSelectedListViewItems() :: Start", this.FullTypeName() ) );
            
            #region Apply Imports
            
            var msg = "BatchImportWindow.ImportingForms".Translate();
            m.SetCurrentStatusMessage( msg );
            
            var selectedImports = lvImportForms.GetSelectedSyncObjects();
            if( !selectedImports.NullOrEmpty() )
            {
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( "BatchImportWindow.Sorting".Translate() );
                SortImportForms( selectedImports, true );
                foreach( var import in selectedImports )
                {
                    if( !import.ImportDataMatchesTarget() )
                    {
                        msg = string.Format( "BatchImportWindow.ImportingForm".Translate(), import.Signature, string.Format( "IXHandle.IDString".Translate(), import.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), import.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                        m.SetCurrentStatusMessage( msg );
                        AddImportMessage( msg );
                        import.Apply( this );
                    }
                }
                m.PopStatusMessage();
            }
            
            #endregion
            
            #region Rebuild references for the plugins
            
            //DebugLog.Write( "\nUpdating reference information for plugins...\n" );
            
            foreach( var mod in GodObject.Plugin.Data.Files.Loaded )
            {
                msg = string.Format( "BatchImportWindow.UpdatingReferences".Translate(), mod.Filename );
                //m.SetCurrentStatusMessage( msg );
                AddImportMessage( msg );
                
                if( !GodObject.Plugin.BuildReferencesFor( mod, null ) )
                    AddImportMessage( string.Format( "BatchImportWindow.UnableToUpdateReferencesFor".Translate(), mod.Filename ) );
            }
            
            #endregion
            
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            SetEnableState( this, true );
        }

        #endregion

        #region Override (Ignore) Close Button

        // Supress the close button on the plugin selector, close with the load/cancel buttons.
        // https://stackoverflow.com/questions/13247629/disabling-a-windows-form-closing-button
        const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams mdiCp = base.CreateParams;
                mdiCp.ClassStyle = mdiCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return mdiCp;
            }
        }
        
        #endregion

    }
}
