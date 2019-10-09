/*
 * SubDivisionBatchWindow.cs
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 23/11/2018
 * Time: 1:51 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    
    /// <summary>
    /// Description of SubDivisionBatch.
    /// </summary>
    public partial class SubDivisionBatch : Form
    {
        
        const string XmlNode = "SubDivisionBatchWindow";
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        bool onLoadComplete = false;
        
        public SubDivisionBatch()
        {
            InitializeComponent();
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            this.Location = GodObject.XmlConfig.ReadPoint( XmlNode, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlNode, XmlSize, this.Size );
            
            lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;
            UpdateSubDivisionList();
            
            onLoadComplete = true;
            SetEnableState( true );
        }
        
        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnSubDivisionListChanged;
            GodObject.Windows.SetSubDivisionBatchWindow( null, false );
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
        
        #region Sync'd list monitoring
        
        void UpdateSubDivisionList()
        {
            var subdivisions = GodObject.Plugin.Data.SubDivisions.ToList();
            lvSubDivisions.SyncObjects = subdivisions;
        }
        
        void OnSubDivisionListChanged( object sender, EventArgs e )
        {
            UpdateSubDivisionList();
        }
        
        #endregion
        
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            
            pnWindow.Enabled = enabled;
        }
        
        void btnCheckMissingElementsClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );
            WorkerThreadPool.CreateWorker( THREAD_CheckMissingElements, null ).Start();
        }
        
        void THREAD_CheckMissingElements()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( true );
                return;
            }
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Checking for missing sub-division elements..." );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            GUIBuilder.SubDivisionBatch.CheckMissingElements(
                subdivisions,
                cbElementBorderEnablers.Checked,
                cbElementSandboxVolumes.Checked );
            
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatchWindow :: CheckMissingElements() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnOptimizeClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );
            WorkerThreadPool.CreateWorker( THREAD_OptimizeVolumes, null ).Start();
        }
        
        void THREAD_OptimizeVolumes()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( true );
                return;
            }
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            if( cbElementSandboxVolumes.Checked )
                GUIBuilder.SubDivisionBatch.GenerateSandboxes( ref list, subdivisions, m, false );
            
            if( cbElementBuildVolumes.Checked )
                GUIBuilder.SubDivisionBatch.GenerateBuildVolumes( ref list, subdivisions, m, false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );
            
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatchWindow :: OptimizeVolumes() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
    }
}
