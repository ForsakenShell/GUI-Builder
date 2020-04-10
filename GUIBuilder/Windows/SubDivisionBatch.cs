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
    /// Use GodObject.Windows.GetWindow<SubDivisionBatch>() to create this Window
    /// </summary>
    public partial class SubDivisionBatch : WindowBase
    {

        public SubDivisionBatch() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();
            
            this.ClientLoad += new System.EventHandler( this.OnClientLoad );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnClientClosing );

            this.btnCheckMissingElements.Click += new System.EventHandler( this.OnCheckMissingElementsClick );
            this.btnNormalizeBuildVolumes.Click += new System.EventHandler( this.OnNormalizeBuildVolumesClick );
            this.btnOptimizeSandboxVolumes.Click += new System.EventHandler( this.OnOptimizeSandboxVolumesClick );

            this.OnSetEnableState += OnClientSetEnableState;
            this.lvSubDivisions.OnSetSyncObjectsThreadComplete += OnSyncSubDivisionsThreadComplete;

            this.ResumeLayout( false );
        }


        void OnClientLoad( object sender, EventArgs e )
        {
            lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;
            UpdateSubDivisionList();
        }

        void OnClientClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnSubDivisionListChanged;
        }

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        bool OnClientSetEnableState( object sender, bool enable )
        {
            var enabled =
                OnLoadComplete &&
                enable &&
                !lvSubDivisions.IsSyncObjectsThreadRunning;
            return enabled;
        }

        #region Sync'd list monitoring

        void OnSyncSubDivisionsThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> sender )
        {
            SetEnableState( sender, true );
        }

        void UpdateSubDivisionList()
        {
            var subdivisions = GodObject.Plugin.Data.SubDivisions.ToList( false );
            lvSubDivisions.SyncObjects = subdivisions;
        }
        
        void OnSubDivisionListChanged( object sender, EventArgs e )
        {
            UpdateSubDivisionList();
        }
        
        #endregion
        
        void OnCheckMissingElementsClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( sender, false );
            WorkerThreadPool.CreateWorker( THREAD_CheckMissingElements, null ).Start();
        }
        
        void THREAD_CheckMissingElements()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "BatchWindow.Function.CheckingElements".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            GUIBuilder.SubDivisionBatch.CheckMissingElements(
                subdivisions,
                cbElementBorderEnablers.Checked,
                cbElementSandboxVolumes.Checked );

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
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            GUIBuilder.SubDivisionBatch.GenerateSandboxes( ref list, subdivisions, m, false, false );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();

            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            GodObject.Windows.SetEnableState( this, true );
        }
        
        void OnNormalizeBuildVolumesClick( object sender, EventArgs e)
        {
            GodObject.Windows.SetEnableState( sender, false );
            WorkerThreadPool.CreateWorker( THREAD_NormalizeBuildVolumes, null ).Start();
        }
        
        void THREAD_NormalizeBuildVolumes()
        {
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            
            if( subdivisions.NullOrEmpty() )
            {
                GodObject.Windows.SetEnableState( this, true );
                return;
            }
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            GUIBuilder.SubDivisionBatch.NormalizeBuildVolumes(
                ref list,
                Engine.Plugin.TargetHandle.WorkingOrLastFullRequired,
                subdivisions,
                m,
                false );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( this, true );
        }
        
    }
}
