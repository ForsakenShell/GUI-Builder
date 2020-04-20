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

using Engine.Plugin;

using EditorIDFormatter = GUIBuilder.CustomForms.EditorIDFormats;


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
            this.OnSetEnableState += OnClientSetEnableState;

            this.btnCheckMissingElements.Click += new System.EventHandler( this.OnCheckMissingElementsClick );
            this.btnNormalizeBuildVolumes.Click += new System.EventHandler( this.OnNormalizeBuildVolumesClick );
            this.btnOptimizeSandboxVolumes.Click += new System.EventHandler( this.OnOptimizeSandboxVolumesClick );

            cbNormalizeBuildVolumesScanTerrain    .CheckedChanged += OnWorkspaceSerializableSettingChange;
            tbNormalizeBuildVolumesGroundSink     .TextChanged    += OnWorkspaceSerializableSettingChange;
            tbNormalizeBuildVolumesTopAbovePeak   .TextChanged    += OnWorkspaceSerializableSettingChange;
            
            cbOptimizeSandboxVolumesCreateNew     .CheckedChanged += OnWorkspaceSerializableSettingChange;
            cbOptimizeSandboxVolumesIgnoreExisting.CheckedChanged += OnWorkspaceSerializableSettingChange;
            cbOptimizeSandboxVolumesScanTerrain   .CheckedChanged += OnWorkspaceSerializableSettingChange;
            tbOptimizeSandboxVolumesCylinderTop   .TextChanged    += OnWorkspaceSerializableSettingChange;
            tbOptimizeSandboxVolumesCylinderBottom.TextChanged    += OnWorkspaceSerializableSettingChange;
            tbOptimizeSandboxVolumesVolumePadding .TextChanged    += OnWorkspaceSerializableSettingChange;
            
            this.lvSubDivisions.OnSetSyncObjectsThreadComplete += OnSyncSubDivisionsThreadComplete;

            this.ResumeLayout( false );
        }

        #region Client Window Events

        void OnClientLoad( object sender, EventArgs e )
        {
            lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;
            UpdateSubDivisionList();
            
            var ws = GodObject.Plugin.Workspace;
            
            SetCheckboxFromWorkspace( cbNormalizeBuildVolumesScanTerrain    , ws, VolumeBatch.XmlKey_BV_ScanTerrain   , true  );
            SetTextboxFromWorkspace ( tbNormalizeBuildVolumesGroundSink     , ws, VolumeBatch.XmlKey_BV_GroundSink    , GodObject.CoreForms.AnnexTheCommonwealth.fBuildVolumeGroundSink   , "F2" );
            SetTextboxFromWorkspace ( tbNormalizeBuildVolumesTopAbovePeak   , ws, VolumeBatch.XmlKey_BV_TopAbovePeak  , GodObject.CoreForms.AnnexTheCommonwealth.fBuildVolumeTopAbovePeak , "F2" );
            
            SetCheckboxFromWorkspace( cbOptimizeSandboxVolumesCreateNew     , ws, VolumeBatch.XmlKey_SV_CreateNew     , true  );
            SetCheckboxFromWorkspace( cbOptimizeSandboxVolumesIgnoreExisting, ws, VolumeBatch.XmlKey_SV_IgnoreExisting, false );
            SetCheckboxFromWorkspace( cbOptimizeSandboxVolumesScanTerrain   , ws, VolumeBatch.XmlKey_SV_ScanTerrain   , true  );
            SetTextboxFromWorkspace ( tbOptimizeSandboxVolumesCylinderTop   , ws, VolumeBatch.XmlKey_SV_CylinderTop   , GodObject.CoreForms.AnnexTheCommonwealth.fSandboxCylinderTop      , "F2" );
            SetTextboxFromWorkspace ( tbOptimizeSandboxVolumesCylinderBottom, ws, VolumeBatch.XmlKey_SV_CylinderBottom, GodObject.CoreForms.AnnexTheCommonwealth.fSandboxCylinderBottom   , "F2" );
            SetTextboxFromWorkspace ( tbOptimizeSandboxVolumesVolumePadding , ws, VolumeBatch.XmlKey_SV_Padding       , GodObject.CoreForms.AnnexTheCommonwealth.fSandboxPadding          , "F2" );
            
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

        #endregion

        #region Workspace Serializable UI Elements

        void SetCheckboxFromWorkspace( CheckBox cb, Workspace ws, string xmlKey, bool defaultValue )
        {
            cb.Checked = ws == null
                ? defaultValue
                : ws.ReadValue<bool>( this.XmlNodeName, xmlKey, defaultValue );
        }

        void SaveCheckboxToWorkspace( CheckBox cb, Workspace ws, string xmlKey )
        {
            if( ws == null ) return;
            ws.WriteValue<bool>( this.XmlNodeName, xmlKey, cb.Checked, false );
        }

        void SetTextboxFromWorkspace( TextBox tb, Workspace ws, string xmlKey, float defaultValue, string valueFormat )
        {
            tb.Text =
                (
                    ws == null
                    ? defaultValue
                    : ws.ReadValue<float>( this.XmlNodeName, xmlKey, defaultValue )
                ).ToString( valueFormat );
        }

        void SaveTextboxToWorkspace( TextBox tb, Workspace ws, string xmlKey )
        {
            if( ws == null ) return;
            ws.WriteValue<float>( this.XmlNodeName, xmlKey, float.Parse( tb.Text ), false );
        }

        void OnWorkspaceSerializableSettingChange( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            var ws = GodObject.Plugin.Workspace;
            if( ws == null ) return;

            SaveCheckboxToWorkspace( cbNormalizeBuildVolumesScanTerrain    , ws, VolumeBatch.XmlKey_BV_ScanTerrain    );
            SaveTextboxToWorkspace ( tbNormalizeBuildVolumesGroundSink     , ws, VolumeBatch.XmlKey_BV_GroundSink     );
            SaveTextboxToWorkspace ( tbNormalizeBuildVolumesTopAbovePeak   , ws, VolumeBatch.XmlKey_BV_TopAbovePeak   );
            
            SaveCheckboxToWorkspace( cbOptimizeSandboxVolumesCreateNew     , ws, VolumeBatch.XmlKey_SV_CreateNew      );
            SaveCheckboxToWorkspace( cbOptimizeSandboxVolumesIgnoreExisting, ws, VolumeBatch.XmlKey_SV_IgnoreExisting );
            SaveCheckboxToWorkspace( cbOptimizeSandboxVolumesScanTerrain   , ws, VolumeBatch.XmlKey_SV_ScanTerrain    );
            SaveTextboxToWorkspace ( tbOptimizeSandboxVolumesCylinderTop   , ws, VolumeBatch.XmlKey_SV_CylinderTop    );
            SaveTextboxToWorkspace ( tbOptimizeSandboxVolumesCylinderBottom, ws, VolumeBatch.XmlKey_SV_CylinderBottom );
            SaveTextboxToWorkspace ( tbOptimizeSandboxVolumesVolumePadding , ws, VolumeBatch.XmlKey_SV_Padding        );

            ws.Commit();
        }

        #endregion

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

        #region Check Missing Elements

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
            
            bool validUserInput = true;

            validUserInput &= float.TryParse( tbOptimizeSandboxVolumesCylinderTop.Text   , out float cylinderTop    );
            validUserInput &= float.TryParse( tbOptimizeSandboxVolumesCylinderBottom.Text, out float cylinderBottom );
            validUserInput &= float.TryParse( tbOptimizeSandboxVolumesVolumePadding.Text , out float volumePadding  );

            if( validUserInput )
                GUIBuilder.SubDivisionBatch.CheckMissingElements(
                    subdivisions,
                    cbElementBorderEnablers.Checked,
                    cbElementSandboxVolumes.Checked,
                    cylinderTop,
                    cylinderBottom,
                    volumePadding
                );

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
                    ref list,
                    TargetHandle.WorkingOrLastFullRequired,
                    subdivisions,
                    createNew,
                    ignoreExisting,
                    scanTerrain,
                    cylinderTop,
                    cylinderBottom,
                    volumePadding,
                    (
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn
                    ),
                    string.Format( "ESM_ATC_REFR_SV_{0}", EditorIDFormatter.Token_Name ),
                    GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_SandboxVolume,
                    GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_SandboxVolume.GetMarkerColor( TargetHandle.Master ),
                    GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSandboxVolume,
                    false, // invertLinkedRefDirection (controller->volume?)
                    PreferedSandboxLayer
                );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();

            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );

            GodObject.Windows.SetEnableState( this, true );
        }

        Engine.Plugin.Forms.Layer PreferedSandboxLayer<TController>(
            ref List<GUIBuilder.FormImport.ImportBase> imports,
            Engine.Plugin.TargetHandle target,
            Engine.Plugin.Forms.ObjectReference sandbox,
            TController controller,
            out string preferedLayerEditorID
        )   where TController : AnnexTheCommonwealth.SubDivision, Interface.WorkshopController
        {
            var preferedLayer =
                sandbox != null
                ?   sandbox.GetLayer( target )
                ??  GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_SandboxVolumes
                :   GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_SandboxVolumes;
            preferedLayerEditorID = preferedLayer.GetEditorID( target );
            return preferedLayer;
        }

        #endregion

        #region Normalize Build Volumes

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
            
            bool validUserInput = true;

            var scanTerrain = cbNormalizeBuildVolumesScanTerrain.Checked;
            validUserInput &= float.TryParse( tbNormalizeBuildVolumesTopAbovePeak.Text   , out float topAbovePeak );
            validUserInput &= float.TryParse( tbNormalizeBuildVolumesGroundSink.Text     , out float groundSink   );

            if( validUserInput )
                GUIBuilder.SubDivisionBatch.NormalizeBuildVolumes(
                    ref list,
                    Engine.Plugin.TargetHandle.WorkingOrLastFullRequired,
                    subdivisions,
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
