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

using SetEditorID = GUIBuilder.FormImport.Operations.SetEditorID;

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

        #region Client Window Events

        void OnClientLoad( object sender, EventArgs e )
        {
            //lvWorkshops.SyncedEditorFormType = typeof( FormEditor.Workshop );
            GodObject.Plugin.Data.Workshops.SyncedGUIList.ObjectDataChanged += OnWorkshopListChanged;
            UpdateWorkshopList();
            
            // TODO:  Load/Save these to the Workspace
            cbNormalizeBuildVolumesScanTerrain.Checked      = true;
            tbNormalizeBuildVolumesGroundSink.Text          = GodObject.CoreForms.Fallout4.fBuildVolumeGroundSink       .ToString( "F2" );
            tbNormalizeBuildVolumesTopAbovePeak.Text        = GodObject.CoreForms.Fallout4.fBuildVolumeTopAbovePeak     .ToString( "F2" );
            
            cbOptimizeSandboxVolumesCreateNew.Checked       = true;
            cbOptimizeSandboxVolumesIgnoreExisting.Checked  = true;
            cbOptimizeSandboxVolumesScanTerrain.Checked     = true;
            tbOptimizeSandboxVolumesCylinderTop.Text        = GodObject.CoreForms.Fallout4.fSandboxCylinderTop          .ToString( "F2" );
            tbOptimizeSandboxVolumesCylinderBottom.Text     = GodObject.CoreForms.Fallout4.fSandboxCylinderBottom       .ToString( "F2" );
            tbOptimizeSandboxVolumesVolumePadding.Text      = GodObject.CoreForms.Fallout4.fSandboxPadding              .ToString( "F2" );
        }

        void OnClientClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.Workshops.SyncedGUIList.ObjectDataChanged -= OnWorkshopListChanged;
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

        #endregion

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

        #region Check Missing Elements

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

        #endregion

        #region Optimize Sandbox Volumes

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
            
            List<GUIBuilder.FormImport.ImportBase> imports = null;

            bool validUserInput = true;

            var createNew       = cbOptimizeSandboxVolumesCreateNew.Checked;;
            var ignoreExisting  = cbOptimizeSandboxVolumesIgnoreExisting.Checked;
            var scanTerrain     = cbOptimizeSandboxVolumesScanTerrain.Checked;
            validUserInput      &= float.TryParse( tbOptimizeSandboxVolumesCylinderTop.Text   , out float cylinderTop    );
            validUserInput      &= float.TryParse( tbOptimizeSandboxVolumesCylinderBottom.Text, out float cylinderBottom );
            validUserInput      &= float.TryParse( tbOptimizeSandboxVolumesVolumePadding.Text , out float volumePadding  );

            if( validUserInput )
                GUIBuilder.VolumeBatch.GenerateSandboxes(
                    m,
                    ref imports,
                    Engine.Plugin.TargetHandle.WorkingOrLastFullRequired,
                    workshops,
                    createNew,
                    ignoreExisting,
                    scanTerrain,
                    cylinderTop,
                    cylinderBottom,
                    volumePadding,
                    (uint)0,
                    string.Format( "{0}WorkshopSandboxArea", SetEditorID.Token_Name ),
                    GodObject.CoreForms.Fallout4.Activator.DefaultDummy,
                    System.Drawing.Color.FromArgb( 255, 0, 0 ),
                    GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkSandbox,
                    true, // invertLinkedRefDirection (controller->volume?)
                    PreferedSandboxLayer
                );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();

            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( imports, false, ref allImportsMatchTarget );

            GodObject.Windows.SetEnableState( this, true );
        }

        Engine.Plugin.Forms.Layer PreferedSandboxLayer<TController>(
            ref List<GUIBuilder.FormImport.ImportBase> imports,
            Engine.Plugin.TargetHandle target,
            Engine.Plugin.Forms.ObjectReference sandbox,
            TController controller,
            out string preferedLayerEditorID
        )   where TController : Fallout4.WorkshopScript, Interface.WorkshopController
        {
            return VolumeBatch.GetRecommendedLayer(
                ref imports,
                (
                    sandbox == null
                    ? null
                    : new List<Engine.Plugin.Forms.ObjectReference>(){ sandbox }
                ),
                controller.Reference.GetLayer( target ),
                string.Format( "{0}Workshop", SetEditorID.Token_Name ),
                controller.QualifiedName,
                -1,
                out preferedLayerEditorID
            );
        }

        #endregion

        #region Normalize Build Volumes

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
            
            bool validUserInput = true;

            var scanTerrain = cbNormalizeBuildVolumesScanTerrain.Checked;
            validUserInput &= float.TryParse( tbNormalizeBuildVolumesTopAbovePeak.Text   , out float topAbovePeak );
            validUserInput &= float.TryParse( tbNormalizeBuildVolumesGroundSink.Text     , out float groundSink   );

            if( validUserInput )
                GUIBuilder.WorkshopBatch.NormalizeBuildVolumes(
                    ref list,
                    Engine.Plugin.TargetHandle.WorkingOrLastFullRequired,
                    workshops,
                    m,
                    false,
                    scanTerrain,
                    topAbovePeak,
                    groundSink
                );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( this, true );
        }
        
       #endregion

    }
}
