/*
 * WorkshopBatchWindow.cs
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 30/03/2020
 * Time: 7:34 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{

    /// <summary>
    /// Use GodObject.Windows.GetWindow<WorkshopBatch>() to create this Window
    /// </summary>
    public partial class WorkshopBatch : WindowBase
    {

        public WorkshopBatch() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();

            this.ClientLoad += new System.EventHandler( this.OnClientLoad );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnClientClosing );
            this.OnSetEnableState += OnClientSetEnableState;

            this.btnCheckMissingElements.Click += new System.EventHandler( this.OnCheckMissingElementsClick );
            this.btnNormalizeBuildVolumes.Click += new System.EventHandler( this.OnNormalizeBuildVolumesClick );
            this.btnOptimizeSandboxVolumes.Click += new System.EventHandler( this.OnOptimizeSandboxVolumesClick );

            this.lvWorkshops.OnSetSyncObjectsThreadComplete += OnSyncWorkshopsThreadComplete;

            this.ResumeLayout( false );
        }


        void OnClientLoad( object sender, EventArgs e )
        {
            //lvWorkshops.SyncedEditorFormType = typeof( FormEditor.Workshop );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnWorkshopListChanged;
            UpdateWorkshopList();
        }

        void OnClientClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnWorkshopListChanged;
        }

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        bool OnClientSetEnableState( object sender, bool enable )
        {
            var enabled =
                enable &&
                !lvWorkshops.IsSyncObjectsThreadRunning;
            return enabled;
        }

        #region Sync'd list monitoring

        void OnSyncWorkshopsThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript> sender )
        {
            SetEnableState( sender, true );
        }

        void UpdateWorkshopList()
        {
            var workshops = GodObject.Plugin.Data.Workshops.SyncedGUIList.ToList( false );
            lvWorkshops.SyncObjects = workshops;
        }
        
        void OnWorkshopListChanged( object sender, EventArgs e )
        {
            UpdateWorkshopList();
        }
        
        #endregion
        
        void OnCheckMissingElementsClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( sender, false );
            WorkerThreadPool.CreateWorker( THREAD_CheckMissingElements, null ).Start();
        }
        
        void THREAD_CheckMissingElements()
        {
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            
            if( workshops.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "BatchWindow.Function.CheckingElements".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            /* TODO: Write me!
            GUIBuilder.WorkshopBatch.CheckMissingElements(
                workshops,
                cbElementBorderMarkers.Checked,
                cbElementSandboxVolumes.Checked );
            */

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( this, true );
        }
        
        void OnOptimizeSandboxVolumesClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( sender, false );
            WorkerThreadPool.CreateWorker( THREAD_OptimizeSandboxVolumes, null ).Start();
        }
        
        void THREAD_OptimizeSandboxVolumes()
        {
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            
            if( workshops.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;

            // TODO: Write me!
            //GUIBuilder.WorkshopBatch.GenerateSandboxes( ref list, workshops, m, false, false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( this, true );
        }
        
        void OnNormalizeBuildVolumesClick(object sender, EventArgs e)
        {
            GodObject.Windows.SetEnableState( this, false );
            WorkerThreadPool.CreateWorker( THREAD_NormalizeBuildVolumes, null ).Start();
        }
        
        void THREAD_NormalizeBuildVolumes()
        {
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            
            if( workshops.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            // TODO: Write me!
            //GUIBuilder.WorkshopBatch.NormalizeBuildVolumes( ref list, workshops, m, false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( this, true );
        }
        
    }
}
