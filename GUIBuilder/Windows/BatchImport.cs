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
    /// Description of BorderBatchImportWindow.
    /// </summary>
    public partial class BatchImport : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return null; } }
        public string XmlNodeName { get{ return "BatchImportWindow"; } }
        
        bool onLoadComplete = false;
        
        public bool AllImportsMatchTarget = false;
        public bool EnableControlsOnClose = true;
        public List<FormImport.ImportBase> ImportForms = null;

        public BatchImport()
        {
            //DebugLog.Write( string.Format( "\n{0} :: cTor() :: Start", this.GetType().ToString() ) );
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            lvImportForms.CustomAscendingSort = PrioritySortAsc;
            lvImportForms.CustomDescendingSort = PrioritySortDes;
            
            //DebugLog.Write( string.Format( "\n{0} :: cTor() :: Complete", this.GetType().ToString() ) );
        }
        
        #region Windows.Forms Events
        
        void OnFormLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( string.Format( "\n{0} :: OnFormLoad() :: Start", this.GetType().ToString() ) );
            
            // This is a modal window which is created, data added, then displayed
            // At this point we just need to sort the data and populate the form
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            
            this.BringToFront();
            
            /*
            DebugLog.Write( string.Format( "ImportForms = {0}", ImportForms.NullOrEmpty() ? 0 : ImportForms.Count ) );
            if( !ImportForms.NullOrEmpty() )
            {
                foreach( var i in ImportForms )
                    DebugLog.Write( string.Format( "\t{0} : 0x{1} - \"{2}\"", i.Signature, i.FormID.ToString( "X8" ), i.EditorID ) );
            }
            */
            
            m.SetCurrentStatusMessage( "BatchImportWindow.Analyzing".Translate() );
            lvImportForms.SyncObjects = ImportForms;
            //SortImportForms();
            //RepopulateImportListView( false );
            
            m.PopStatusMessage();
            pnMain.Enabled = true;
            onLoadComplete = true;
            
            //DebugLog.Write( string.Format( "\n{0} :: OnFormLoad() :: Complete", this.GetType().ToString() ) );
        }
        
        void OnFormClosed( object sender, FormClosedEventArgs e )
        {
            AllImportsMatchTarget =
                ( ImportForms.NullOrEmpty() )||
                ( FormImport.ImportBase.AllImportsMatchTarget( ImportForms ) );
            //var m = GodObject.Windows.GetMainWindow();
            //m.PopStatusMessage();
            if( EnableControlsOnClose )
                GodObject.Windows.SetEnableState( true );
            GodObject.Windows.SetWindow<BatchImport>( null, false );
        }
        
        void OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteLocation( this );
        }
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( this );
        }

        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate () { SetEnableState( enabled ); }, null );
                return;
            }

            pnMain.Enabled = enabled;
        }

        void btnCloseClick( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.None;
            this.Close();
        }
        
        void btnImportSelectedClick( object sender, EventArgs e )
        {
            var thread = WorkerThreadPool.CreateWorker( THREAD_ImportSelectedListViewItems, null );
            if( thread != null )
                thread.Start();
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
            DebugLog.WriteLine( "SortImportForms()" );
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
                if( !string.IsNullOrEmpty( line ) ) tbImportMessages.AppendText( line + "\r\n" );
            tbImportMessages.Refresh();
            tbImportMessages.ScrollToCaret();
        }

        #endregion

        #region Import the Forms

        void THREAD_ImportSelectedListViewItems()
        {
            SetEnableState( false );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            tbImportMessages.Clear();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            //DebugLog.Write( string.Format( "\n{0} :: ImportSelectedListViewItems() :: Start", this.GetType().ToString() ) );
            
            #region Apply Imports
            
            var msg = "BatchImportWindow.ImportingForms".Translate();
            m.SetCurrentStatusMessage( msg );
            
            var selectedImportForms = lvImportForms.GetSelectedSyncObjects();
            if( !selectedImportForms.NullOrEmpty() )
            {
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( "BatchImportWindow.Sorting".Translate() );
                SortImportForms( selectedImportForms, true );
                foreach( var importForm in selectedImportForms )
                {
                    msg = string.Format( "BatchImportWindow.ImportingForm".Translate(), importForm.Signature, importForm.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), importForm.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                    m.SetCurrentStatusMessage( msg );
                    AddImportMessage( msg );
                    importForm.Apply( this );
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
            
            m.StopSyncTimer(
                "GUIBuilder.BatchImportWindow :: ImportSelectedListViewItems() :: Completed in {0}",
                tStart.Ticks );
            m.PopStatusMessage();
            SetEnableState( true );
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
