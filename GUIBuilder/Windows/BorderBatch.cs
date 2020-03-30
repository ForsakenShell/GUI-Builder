/*
 * BorderBatch.cs
 *
 * Border batch window.
 *
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using AnnexTheCommonwealth;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of BorderBatch.
    /// </summary>
    public partial class BorderBatch : WindowBase
    {
        
        
        /// <summary>
        /// Use GodObject.Windows.GetWindow<BorderBatch>() to create this Window
        /// </summary>
        public BorderBatch() : base( true )
        {
            InitializeComponent();
            this.OnSetEnableState   += new SetEnableStateHandler( this.OnFormSetEnableState );
        }


        #region GodObject.XmlConfig.IXmlConfiguration


        public override string XmlNodeName { get { return "BorderBatchWindow"; } }


        #endregion

        bool _nodesBuilt = false;
        List<GUIBuilder.FormImport.ImportBase> _importData = null;
        
        Fallout4.WorkshopScript _sampleWorkshop = null;
        SubDivision _sampleSubDivision = null;
        
        ToolTip tbNIFBuilderTargetFolderToolTip;
        ToolTip tbNIFBuilderSubDivisionFilePathSampleToolTip;
        ToolTip tbNIFBuilderWorkshopFilePathSampleToolTip;
        
        List<TabPage> hiddenPages = new List<TabPage>();
        
        #region Window management
        
        void BorderBatch_OnLoad( object sender, EventArgs e )
        {
            tbTargetFolder.Text = GodObject.Paths.NIFBuilderOutput;
            
            tbNIFBuilderTargetFolderToolTip = new ToolTip();
            tbNIFBuilderTargetFolderToolTip.ShowAlways = true;
            tbNIFBuilderTargetFolderToolTip.SetToolTip( tbTargetFolder, tbTargetFolder.Text );
            
            if( !GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth ) )
                EnablePage( tpSubDivisions, false );
            else
            {
                tbNIFBuilderSubDivisionFilePathSampleToolTip = new ToolTip();
                tbNIFBuilderSubDivisionFilePathSampleToolTip.ShowAlways = true;

                int presetIndex = ( ( GodObject.Plugin.Workspace != null ) && ( GodObject.Plugin.Workspace.HasSubDivisonPreset ) ) ? 0 : 1;
                RepopulatePresetComboBoxes( cbSubDivisionPresets, NIFBuilder.Preset.SubDivisionPresets, presetIndex );
                
                UpdateSubDivisionList( false );
                lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
                GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;

                EnablePage( tpSubDivisions, true );
            }

            if( !GodObject.Master.Loaded( GodObject.Master.Fallout4 ) )
                EnablePage( tpWorkshops, false );
            else
            {
                tbNIFBuilderWorkshopFilePathSampleToolTip = new ToolTip();
                tbNIFBuilderWorkshopFilePathSampleToolTip.ShowAlways = true;

                int presetIndex = ( ( GodObject.Plugin.Workspace != null ) && ( GodObject.Plugin.Workspace.HasWorkshopPreset ) ) ? 0 : 1;
                RepopulatePresetComboBoxes( cbWorkshopPresets, NIFBuilder.Preset.WorkshopPresets, presetIndex );

                UpdateWorkshopList( false );
                //lvWorkshops.SyncedEditorFormType = typeof( FormEditor.WorkshopScript );
                GodObject.Plugin.Data.Workshops.SyncedGUIList.ObjectDataChanged += OnWorkshopListChanged;

                EnablePage( tpWorkshops, true );
            }

            UpdateNIFFilePathSampleInternal();
        }

        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnSubDivisionListChanged;
            GodObject.Plugin.Data.Workshops.SyncedGUIList.ObjectDataChanged -= OnWorkshopListChanged;
        }

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enabled">Enable state to set</param>
        void OnFormSetEnableState( bool enabled )
        {
            btnClear.Enabled = _nodesBuilt;
            btnGenNodes.Enabled = !_nodesBuilt;
            btnBuildNIFs.Enabled = _nodesBuilt;
            btnImportNIFs.Enabled = _importData != null;
        }

        void EnablePage( TabPage page, bool enable )
        {
            if( enable )
            {
                if( !tcObjectSelect.TabPages.Contains( page ) )
                    tcObjectSelect.TabPages.Add( page );
                hiddenPages.Remove( page );
            }
            else
            {
                if( !hiddenPages.Contains( page ) )
                    hiddenPages.Add( page );
                tcObjectSelect.TabPages.Remove( page );
            }
        }

        #endregion

        #region Sync'd list monitoring

        void UpdateWorkshopList( bool updateSampleDisplay )
        {
            var workshops = GodObject.Plugin.Data.Workshops.SyncedGUIList.ToList( false );
            if( workshops.NullOrEmpty() )
                _sampleWorkshop = null;
            else
            {
                // Try to find one in the working mod, otherwise use the first one
                _sampleWorkshop = workshops.Find( (x) => ( x.LoadOrder == GodObject.Plugin.Data.Files.Working.LoadOrder ) );
                if( _sampleWorkshop == null )
                    _sampleWorkshop = workshops[ 0 ];
            }
            lvWorkshops.SyncObjects = workshops;
            if( updateSampleDisplay )
                UpdateNIFFilePathSampleInternal();
            
        }
        
        void OnWorkshopListChanged( object sender, EventArgs e )
        {
            UpdateWorkshopList( true );
        }
        
        void UpdateSubDivisionList( bool updateSampleDisplay )
        {
            var subdivisions = GodObject.Plugin.Data.SubDivisions.ToList( false );
            if( subdivisions.NullOrEmpty() )
                _sampleSubDivision = null;
            else
            {
                // Try to find one in the working mod, otherwise use the first one
                _sampleSubDivision = subdivisions.Find( (x) => ( x.LoadOrder == GodObject.Plugin.Data.Files.Working.LoadOrder ) );
                if( _sampleSubDivision == null )
                    _sampleSubDivision = subdivisions[ 0 ];
            }
            lvSubDivisions.SyncObjects = subdivisions;
            if( updateSampleDisplay )
                UpdateNIFFilePathSampleInternal();
        }
        
        void OnSubDivisionListChanged( object sender, EventArgs e )
        {
            UpdateSubDivisionList( true );
        }
        
        #endregion
        
        #region Clear out existing sub-division edge flag segments
        
        void THREAD_ClearEdgeFlagSegments()
        {
            GodObject.Windows.SetEnableState( false );
            
            _nodesBuilt = false;
            _importData = null;
            
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            GUIBuilder.SubDivisionBatch.ClearEdgeFlagNodes( subdivisions );
            
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnClearClick( object sender, EventArgs e )
        {
            WorkerThreadPool.CreateWorker( THREAD_ClearEdgeFlagSegments, null ).Start();
        }
        
        #endregion
        
        #region Calculate edge flag segments
        
        class ThreadParams_CalculateBorderNodes
        {
            public readonly List<Fallout4.WorkshopScript> Workshops;
            public readonly List<AnnexTheCommonwealth.SubDivision> SubDivisions;
            public readonly NIFBuilder.Preset WorkshopPreset;
            public readonly NIFBuilder.Preset SubDivisionPreset;
            public readonly bool UpdateMapUIData;

            public ThreadParams_CalculateBorderNodes(
                List<Fallout4.WorkshopScript> workshops,
                List<AnnexTheCommonwealth.SubDivision> subDivisions,
                NIFBuilder.Preset workshopPreset,
                NIFBuilder.Preset subDivisionPreset,
                bool updateMapUIData
            )
            {
                Workshops = workshops;
                SubDivisions = subDivisions;
                WorkshopPreset = workshopPreset;
                SubDivisionPreset = subDivisionPreset;
                UpdateMapUIData = updateMapUIData;
            }
        }

        void THREAD_CalculateBorderNodes( object obj )
        {
            var parameters = obj as ThreadParams_CalculateBorderNodes;
            _nodesBuilt = false;
            _importData?.Clear();
            var nodesBuilt = false;

            if( ( !parameters.Workshops.NullOrEmpty() )&&( parameters.WorkshopPreset != null ) )
            {
                var kywdWBG = GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword;
                var kywdWBL = GUIBuilder.CustomForms.WorkshopBorderLinkKeyword;
                var statWFZ = GUIBuilder.CustomForms.WorkshopForcedZMarker;
                var lcrtBWB = GUIBuilder.CustomForms.WorkshopBorderWithBottomRef;
                if( kywdWBG != null )
                {
                    if( GUIBuilder.WorkshopBatch.CalculateWorkshopBorderMarkerNodes(
                        parameters.Workshops,
                        kywdWBG,
                        kywdWBL,
                        statWFZ,
                        lcrtBWB,
                        parameters.WorkshopPreset.NodeLength,
                        parameters.WorkshopPreset.AngleAllowance,
                        parameters.WorkshopPreset.SlopeAllowance,
                        parameters.UpdateMapUIData ) )
                    {
                        nodesBuilt = true;
                    }
                }
            }
            
            if( ( !parameters.SubDivisions.NullOrEmpty() )&&( parameters.SubDivisionPreset != null ) )
            {
                if( GUIBuilder.SubDivisionBatch.CalculateSubDivisionEdgeFlagNodes(
                    parameters.SubDivisions,
                    parameters.SubDivisionPreset.NodeLength,
                    parameters.SubDivisionPreset.AngleAllowance,
                    parameters.SubDivisionPreset.SlopeAllowance,
                    parameters.UpdateMapUIData ) )
                {
                    nodesBuilt = true;
                }
            }

            _nodesBuilt = nodesBuilt;
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnGenNodesClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );

            var workshops = lvWorkshops.GetSelectedSyncObjects();
            if( !workshops.NullOrEmpty() )
            {
                if(
                    ( GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword == null )||
                    ( GUIBuilder.CustomForms.WorkshopBorderLinkKeyword      == null )//||
                    //( GUIBuilder.CustomForms.WorkshopTerrainFollowingMarker == null )||
                    //( GUIBuilder.CustomForms.WorkshopForcedZMarker          == null )||
                    //( GUIBuilder.CustomForms.WorkshopBorderWithBottomRef    == null )
                )
                {
                    GodObject.Windows.GetWindow<CustomForms>( true );
                    SetEnableState( true );
                    return;
                }
            }

            var subDivisions = GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth )
                    ? lvSubDivisions.GetSelectedSyncObjects()
                    : null;
            if( ( workshops.NullOrEmpty() )&&( subDivisions.NullOrEmpty() ) )
            {
                SetEnableState( true );
                return;
            }

            var wsPreset = SelectedWorkshopPreset;
            var sdPreset = SelectedSubDivisionPreset;
            object obj = new ThreadParams_CalculateBorderNodes( workshops, subDivisions, wsPreset, sdPreset, false );
            var thread = WorkerThreadPool.CreateWorker( obj, THREAD_CalculateBorderNodes, null );
            if( thread == null )
            {
                SetEnableState( true );
                return;
            }
            thread.Start();
        }

        #endregion

        void UpdateNIFFilePathSampleInternal()
        {
            if( !OnLoadComplete )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "UpdateNIFFilePathSampleInternal()" } );
            
            var target = tbTargetFolder.Text;
            if( ( !string.IsNullOrEmpty( target ) )&&( target[ target.Length - 1 ] != '\\' ) )
                target += @"\";
            
            var wsPreset = FullChildSelectedWorkshopPreset;
            var wsFilePrefix = tbWorkshopNIFFilePrefix.Text;
            var wsName = _sampleWorkshop    != null ? _sampleWorkshop   .NameFromEditorID : "";
            var wsSample = NIFBuilder.Mesh.BuildFilePath(
                tbMeshDirectory.Text,
                ( wsPreset != null ? wsPreset.MeshSubDirectory : tbWorkshopNIFMeshSubDirectory.Text ),
                ( wsPreset != null ? wsPreset.FilePrefix : tbWorkshopNIFFilePrefix.Text ),
                wsName,
                ( wsPreset != null ? wsPreset.FileSuffix : tbWorkshopNIFFileSuffix.Text )
                );
            
            tbWorkshopNIFSampleFilePath.Text = wsSample;
            tbNIFBuilderWorkshopFilePathSampleToolTip.SetToolTip( tbWorkshopNIFSampleFilePath, wsSample );
            
            if( GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth ) )
            {
                var sdPreset = FullChildSelectedSubDivisionPreset;
                var sdFilePrefix = tbSubDivisionNIFFilePrefix.Text;
                var sdName = _sampleSubDivision != null ? _sampleSubDivision.NameFromEditorID : "";
                var sdNeighbourName = "Main";
                var subBorders = _sampleSubDivision != null ? _sampleSubDivision.BorderEnablers : null;
                if( !subBorders.NullOrEmpty() )
                {
                    var border = subBorders[ 0 ];
                    var neighbour = border == null ? null : border.Neighbour;
                    //if( ( border != null )&&( border.Neighbour != null ) )
                    if( neighbour != null )
                        sdNeighbourName = neighbour.NameFromEditorID;
                }
                
                var sdSample = NIFBuilder.Mesh.BuildFilePath(
                    tbMeshDirectory.Text,
                    ( sdPreset != null ? sdPreset.MeshSubDirectory : tbSubDivisionNIFMeshSubDirectory.Text ),
                    ( sdPreset != null ? sdPreset.FilePrefix : tbSubDivisionNIFFilePrefix.Text ),
                    sdName,
                    ( sdPreset != null ? sdPreset.FileSuffix : tbSubDivisionNIFFileSuffix.Text ), 1,
                    sdNeighbourName, 1 );
                
                tbNIFBuilderSubDivisionNIFSampleFilePath.Text = sdSample;
                tbNIFBuilderSubDivisionFilePathSampleToolTip.SetToolTip( tbNIFBuilderSubDivisionNIFSampleFilePath, sdSample );
            }
            
            //DebugLog.CloseIndentLevel();
        }
        
        #region Build NIFs
        
        void THREAD_BuildNIFs()
        {
            GodObject.Windows.SetEnableState( false );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            _importData = null;
            List<GUIBuilder.FormImport.ImportBase> list = null;

            var targetPath = tbTargetFolder.Text;
            var meshSuffix = tbMeshDirectory.Text;
            
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            if( !workshops.NullOrEmpty() )
            {
                DebugLog.OpenIndentLevel( "Workshop Borders", false );
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( "BorderBatchWindow.BuildingWorkshopBorders".Translate() );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var wsPreset = SelectedWorkshopPreset;
                if( wsPreset == null )
                {
                    var createImportData = cbWorkshopNIFCreateImportData.Checked;
                    var subList = GUIBuilder.WorkshopBatch.BuildNIFs(
                        "Custom",
                        workshops,
                        float.Parse( tbWorkshopNIFGradientHeight.Text ),
                        float.Parse( tbWorkshopNIFGroundOffset.Text ),
                        float.Parse( tbWorkshopNIFGroundSink.Text ),
                        targetPath, tbWorkshopNIFTargetSubDirectory.Text,
                        meshSuffix, tbWorkshopNIFMeshSubDirectory.Text,
                        tbWorkshopNIFFilePrefix.Text, tbWorkshopNIFFileSuffix.Text,
                        createImportData );
                    if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
                else
                {
                    var subList = CreatePresetWorkshopNIFs( workshops, wsPreset, targetPath, meshSuffix );
                    if( ( wsPreset.CreateImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }

                m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                DebugLog.CloseIndentLevel();
            }
            
            if( GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth ) )
            {
                var subDivisions = lvSubDivisions.GetSelectedSyncObjects();
                if( !subDivisions.NullOrEmpty() )
                {
                    DebugLog.OpenIndentLevel( "Sub-Division Borders", false );
                    m.PushStatusMessage();
                    m.SetCurrentStatusMessage( "BorderBatchWindow.BuildingSubDivisionBorders".Translate() );
                    m.StartSyncTimer();
                    var tStart = m.SyncTimerElapsed();
                    
                    var sdPreset = SelectedSubDivisionPreset;
                    if( sdPreset == null )
                    {
                        var createImportData = cbSubDivisionNIFCreateImportData.Checked;
                        var subList = GUIBuilder.SubDivisionBatch.BuildNIFs(
                            "Custom",
                            subDivisions,
                            float.Parse( tbSubDivisionNIFGradientHeight.Text ),
                            float.Parse( tbSubDivisionNIFGroundOffset.Text ),
                            float.Parse( tbSubDivisionNIFGroundSink.Text ),
                            targetPath, tbSubDivisionNIFTargetSubDirectory.Text,
                            meshSuffix, tbSubDivisionNIFMeshSubDirectory.Text,
                            tbSubDivisionNIFFilePrefix.Text, tbSubDivisionNIFFileSuffix.Text,
                            cbSubDivisionNIFCreateImportData.Checked );
                        if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                        {
                            if( list == null )
                                list = subList;
                            else
                                list.AddAll( subList );
                        }
                    }
                    else
                    {
                        var subList = CreatePresetSubDivisionNIFs( subDivisions, sdPreset, targetPath, meshSuffix );
                        if( ( sdPreset.CreateImportData )&&( !subList.NullOrEmpty() ) )
                        {
                            if( list == null )
                                list = subList;
                            else
                                list.AddAll( subList );
                        }
                    }

                    m.StopSyncTimer( tStart );
                    m.PopStatusMessage();
                    DebugLog.CloseIndentLevel();
                }
            }
            
            _importData = list;
            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
        }
        
        List<GUIBuilder.FormImport.ImportBase> CreatePresetWorkshopNIFs(
            List<Fallout4.WorkshopScript> workshops,
            NIFBuilder.Preset preset,
            string targetPath,
            string meshSuffix )
        {
            if( workshops.NullOrEmpty() ) return null;
            if( preset == null ) return null;
            List<GUIBuilder.FormImport.ImportBase> list = null;
            if( preset.SetOfPresets )
            {
                foreach( var subSet in preset.SubSets )
                {
                    var subList = CreatePresetWorkshopNIFs( workshops, subSet, targetPath, meshSuffix );
                    if( ( subSet.CreateImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
                return list;
            }
            return GUIBuilder.WorkshopBatch.BuildNIFs(
                preset.Name,
                workshops,
                preset.GradientHeight,
                preset.GroundOffset,
                preset.GroundSink,
                targetPath,
                preset.TargetSuffix,
                meshSuffix,
                preset.MeshSubDirectory,
                preset.FilePrefix, preset.FileSuffix,
                preset.CreateImportData );
        }
        
        List<GUIBuilder.FormImport.ImportBase> CreatePresetSubDivisionNIFs(
            List<SubDivision> subDivisions,
            NIFBuilder.Preset preset,
            string targetPath,
            string meshSuffix )
        {
            if( subDivisions.NullOrEmpty() ) return null;
            if( preset == null ) return null;
            List<GUIBuilder.FormImport.ImportBase> list = null;
            if( preset.SetOfPresets )
            {
                foreach( var subSet in preset.SubSets )
                {
                    var subList = CreatePresetSubDivisionNIFs( subDivisions, subSet, targetPath, meshSuffix );
                    if( ( subSet.CreateImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
                return list;
            }
            return GUIBuilder.SubDivisionBatch.BuildNIFs(
                preset.Name,
                subDivisions,
                preset.GradientHeight,
                preset.GroundOffset,
                preset.GroundSink,
                targetPath,
                preset.TargetSuffix,
                meshSuffix,
                preset.MeshSubDirectory,
                preset.FilePrefix, preset.FileSuffix,
                preset.CreateImportData );
        }
        
        void btnBuildNIFsClick( object sender, EventArgs e )
        {
            if( string.IsNullOrEmpty( tbTargetFolder.Text ) )
            {
                tbNIFBuilderTargetFolderMouseClick( null, null );
                if( string.IsNullOrEmpty( tbTargetFolder.Text ) )
                {
                    DebugLog.WriteLine( "No target folder selected!", true );
                    return;
                }
            }
            WorkerThreadPool.CreateWorker( THREAD_BuildNIFs, null ).Start();
        }
        
        void tbNIFBuilderTargetFolderMouseClick( object sender, MouseEventArgs e )
        {
            
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = GodObject.Paths.NIFBuilderOutput;
            
            if( fbd.ShowDialog() == DialogResult.OK )
            {
                var path = fbd.SelectedPath;
                if( !string.IsNullOrEmpty( path ) )
                {
                    GodObject.Paths.NIFBuilderOutput = path;
                    tbTargetFolder.Text = path;
                    tbNIFBuilderTargetFolderToolTip.SetToolTip( tbTargetFolder, path );
                }
            }
            
            fbd.Dispose();
            fbd = null;
        }
        
        void uiUpdateWorkshopNIFFilePathSample( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            UpdatingPresetUI = true;
            cbWorkshopPresets.SelectedIndex = 0;
            _SelectedWorkshopPreset = 0;
            UpdateNIFFilePathSampleInternal();
            UpdatingPresetUI = false;
        }
        
        void uiUpdateSubDivisionNIFFilePathSample( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            UpdatingPresetUI = true;
            cbSubDivisionPresets.SelectedIndex = 0;
            _SelectedSubDivisionPreset = 0;
            UpdateNIFFilePathSampleInternal();
            UpdatingPresetUI = false;
        }
        
        void tbNIFBuilderNIFFilePathSampleMouseClick( object sender, MouseEventArgs e )
        {
            // Supress user changes to the filepath sample textbox
            tbWorkshopNIFSampleFilePath.Parent.Focus();
        }
        
        #endregion
        
        #region Import NIFs
        
        void btnImportNIFsClick( object sender, EventArgs e )
        {
            if( _importData.NullOrEmpty() ) return;
            var t = WorkerThreadPool.CreateWorker( THREAD_ImportNIFs, null );
            if( t != null )
            {
                GodObject.Windows.SetEnableState( false );
                t.Start();
            }
        }
        
        void THREAD_ImportNIFs()
        {
            if( _importData.NullOrEmpty() )
                return;
            bool tmp = false;
            if( !FormImport.ImportBase.ShowImportDialog( _importData, true, ref tmp ) )
                GodObject.Windows.SetEnableState( true );
        }
        
        #endregion
        
        #region Presets
        
        #region Update Preset UI
        
        static void RepopulatePresetComboBoxes( ComboBox cb, List<NIFBuilder.Preset> presets, int selectedIndex )
        {
            cb.Items.Clear();
            cb.Items.Add( "NIFBuilder.Preset.Custom".Translate() );
            var count = presets == null ? 0 : presets.Count;
            if( count > 0 )
            {
                for( int i = 0; i < count; i++ )
                    cb.Items.Add( presets[ i ].Name );
                cb.SelectedIndex = selectedIndex;
            }
            else
                cb.SelectedIndex = 0;
        }
        
        static void SetPresetUIValue( TextBox tb, string value )
        {
            if( tb == null ) return;
            tb.Text = value;
        }
        
        static void SetPresetUIValue( TextBox tb, float value, bool clear = false )
        {
            if( tb == null ) return;
            tb.Text = clear ? "" : value.ToString( "F4" );
        }
        
        static void SetPresetUIValue( CheckBox cb, bool value, bool intermediate = false )
        {
            if( cb == null ) return;
            cb.ThreeState = intermediate;
            cb.CheckState =
                    !value
                ?   CheckState.Unchecked
                :   intermediate
                ?   CheckState.Indeterminate
                :   CheckState.Checked;
        }
        
        static bool UpdatingPresetUI = false;
        static void UpdatePresetUI(
            ref NIFBuilder.Preset lastPreset,
            NIFBuilder.Preset newPreset,
            int index, ComboBox cbPresets,
            TextBox nodeLength,
            TextBox angleAllowance, TextBox slopeAllowance,
            TextBox targetSuffix, TextBox meshSubDirectory,
            TextBox filePrefix, TextBox fileSuffix,
            TextBox gradientHeight, TextBox groundOffset, TextBox groundSink,
            CheckBox createImportData )
        {
            if( UpdatingPresetUI ) return;
            UpdatingPresetUI = true;

            DebugLog.WriteStrings( null, new string[] { "lastPreset = " + lastPreset.ToStringNullSafe(), "newPreset = " + newPreset.ToStringNullSafe() }, false, true, false, false );

            if( index < 0 )
                index = 0;
            
            // Check is important so we don't trigger the UI callback by setting the same value and cause a stack overflow
            // disable once RedundantCheckBeforeAssignment
            if( cbPresets.SelectedIndex != index )
                cbPresets.SelectedIndex = index;
            
            if( ( lastPreset != null )&&( lastPreset.AllowWriteback ) )
            {
                lastPreset.NodeLength       = float.Parse( nodeLength.Text );
                lastPreset.AngleAllowance   = double.Parse( angleAllowance.Text );
                lastPreset.SlopeAllowance   = double.Parse( slopeAllowance.Text );
                lastPreset.GradientHeight   = float.Parse( gradientHeight.Text );
                lastPreset.GroundOffset     = float.Parse( groundOffset.Text );
                lastPreset.GroundSink       = float.Parse( groundSink.Text );
                lastPreset.TargetSuffix     = targetSuffix.Text;
                lastPreset.MeshSubDirectory = meshSubDirectory.Text;
                lastPreset.FilePrefix       = filePrefix.Text;
                lastPreset.FileSuffix       = fileSuffix.Text;
                lastPreset.CreateImportData = createImportData.Checked;
                lastPreset.Serialize();
            }
            if( newPreset != lastPreset )
            {
                if( newPreset != null )
                {
                    SetPresetUIValue( targetSuffix      , newPreset.TargetSuffix );
                    SetPresetUIValue( meshSubDirectory  , newPreset.MeshSubDirectory );
                    SetPresetUIValue( filePrefix        , newPreset.FilePrefix );
                    SetPresetUIValue( fileSuffix        , newPreset.FileSuffix );
                    SetPresetUIValue( nodeLength        , newPreset.NodeLength );
                    SetPresetUIValue( angleAllowance    , (float)newPreset.AngleAllowance );
                    SetPresetUIValue( slopeAllowance    , (float)newPreset.SlopeAllowance );
                    SetPresetUIValue( gradientHeight    , newPreset.GradientHeight      , newPreset.SetOfPresets );
                    SetPresetUIValue( groundOffset      , newPreset.GroundOffset        , newPreset.SetOfPresets );
                    SetPresetUIValue( groundSink        , newPreset.GroundSink          , newPreset.SetOfPresets );
                    SetPresetUIValue( createImportData  , newPreset.CreateImportData    , newPreset.SetOfPresets );
                }
                lastPreset = newPreset;
            }
            //else if( createImportData != null )
            //    SetPresetUIValue( createImportData  , createImportData.Checked, false );

            UpdatingPresetUI = false;
        }

        NIFBuilder.Preset _LastWorkshopPreset = null;
        void UpdateWorkshopPresetUI( int index )
        {
            var newPreset = SelectedWorkshopPreset;
            UpdatePresetUI(
                ref _LastWorkshopPreset,
                newPreset,
                index,
                cbWorkshopPresets,
                tbWorkshopNodeLength,
                tbWorkshopNodeAngleAllowance,
                tbWorkshopNodeSlopeAllowance,
                tbWorkshopNIFTargetSubDirectory,
                tbWorkshopNIFMeshSubDirectory,
                tbWorkshopNIFFilePrefix,
                tbWorkshopNIFFileSuffix,
                tbWorkshopNIFGradientHeight,
                tbWorkshopNIFGroundOffset,
                tbWorkshopNIFGroundSink,
                cbWorkshopNIFCreateImportData ); ;
            UpdateNIFFilePathSampleInternal();
        }

        NIFBuilder.Preset _LastSubDivisionPreset = null;
        void UpdateSubDivisionPresetUI( int index )
        {
            var newPreset = SelectedSubDivisionPreset;
            UpdatePresetUI(
                ref _LastSubDivisionPreset,
                newPreset,
                index,
                cbSubDivisionPresets,
                tbSubDivisionNodeLength,
                tbSubDivisionNodeAngleAllowance,
                tbSubDivisionNodeSlopeAllowance,
                tbSubDivisionNIFTargetSubDirectory,
                tbSubDivisionNIFMeshSubDirectory,
                tbSubDivisionNIFFilePrefix,
                tbSubDivisionNIFFileSuffix,
                tbSubDivisionNIFGradientHeight,
                tbSubDivisionNIFGroundOffset,
                tbSubDivisionNIFGroundSink,
                cbSubDivisionNIFCreateImportData );
            UpdateNIFFilePathSampleInternal();
        }
        
        #endregion
        
        #region Get Selected Preset
        
        NIFBuilder.Preset GetPreset( NIFBuilder.Preset defaultPreset, List<NIFBuilder.Preset> list, int index, bool fullChild = false )
        {
            //Console.WriteLine( "GetPreset()" );
            if( index < 0 ) return defaultPreset;
            if( list.NullOrEmpty() ) return defaultPreset;
            if( index >= list.Count ) return defaultPreset;
            var preset = list[ index ];
            if( ( fullChild )&&( preset.SetOfPresets ) )
            {
                for( int i = 0; i < preset.SubSets.Count; i++ )
                {
                    var child = GetPreset( defaultPreset, preset.SubSets, i, fullChild );
                    if( ( child != null )&&( child != defaultPreset ) ) return child;
                }
            }
            return preset;
        }
        
        int _SelectedWorkshopPreset = 0;
        int _SelectedSubDivisionPreset = 0;
        
        NIFBuilder.Preset SelectedWorkshopPreset
        {
            get
            {
                //Console.WriteLine( "SelectedWorkshopPreset" );
                return GetPreset(
                    GodObject.Plugin.Workspace?.WorkshopPreset,
                    NIFBuilder.Preset.WorkshopPresets,
                    _SelectedWorkshopPreset - 1,
                    false );
            }
        }
        
        NIFBuilder.Preset FullChildSelectedWorkshopPreset
        {
            get
            {
                //Console.WriteLine( "FullChildSelectedWorkshopPreset" );
                return GetPreset(
                    GodObject.Plugin.Workspace?.WorkshopPreset,
                    NIFBuilder.Preset.WorkshopPresets,
                    _SelectedWorkshopPreset - 1,
                    true );
            }
        }
        
        NIFBuilder.Preset SelectedSubDivisionPreset
        {
            get
            {
                //Console.WriteLine( "SelectedSubDivisionPreset" );
                return GetPreset(
                    GodObject.Plugin.Workspace?.SubDivisionPreset,
                    NIFBuilder.Preset.SubDivisionPresets,
                    _SelectedSubDivisionPreset - 1,
                    false );
            }
        }
        
        NIFBuilder.Preset FullChildSelectedSubDivisionPreset
        {
            get
            {
                //Console.WriteLine( "FullChildSelectedSubDivisionPreset" );
                return GetPreset(
                    GodObject.Plugin.Workspace?.SubDivisionPreset,
                    NIFBuilder.Preset.SubDivisionPresets,
                    _SelectedSubDivisionPreset - 1,
                    true );
            }
        }
        
        #endregion
        
        #region Preset UI User Events
        
        void cbWorkshopPresetsSelectedIndexChanged( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            _SelectedWorkshopPreset = cbWorkshopPresets.SelectedIndex;
            UpdateWorkshopPresetUI( _SelectedWorkshopPreset );
        }
        
        void cbSubDivisionPresetsSelectedIndexChanged( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            _SelectedSubDivisionPreset = cbSubDivisionPresets.SelectedIndex;
            UpdateSubDivisionPresetUI( _SelectedSubDivisionPreset );
        }
        
        void uiWorkshopNIFBuilderChanged( object sender, EventArgs e )
        {
            if( ( !OnLoadComplete )||( UpdatingPresetUI ) )return;
            _SelectedWorkshopPreset = 0;
            UpdateWorkshopPresetUI( _SelectedWorkshopPreset );
        }
        
        void uiSubDivisionNIFBuilderChanged( object sender, EventArgs e )
        {
            if( ( !OnLoadComplete )||( UpdatingPresetUI ) ) return;
            _SelectedSubDivisionPreset = 0;
            UpdateSubDivisionPresetUI( _SelectedSubDivisionPreset );
        }
        
        #endregion
        
        #endregion
        
        #region Tab Control
        
        void tcObjectSelectSelectedIndexChanged( object sender, EventArgs e )
        {
            switch( tcObjectSelect.SelectedIndex )
            {
                case 0:
                    if( lvSubDivisions.Visible )
                        lvSubDivisions.RepopulateListView();
                    break;
                    
                case 1:
                    if( lvWorkshops.Visible )
                        lvWorkshops.RepopulateListView();
                    break;
                    
            }
        }
        
        #endregion
        
    }
    
}
