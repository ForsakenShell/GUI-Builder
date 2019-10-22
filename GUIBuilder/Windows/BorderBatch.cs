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
    public partial class BorderBatch : Form
    {
        
        const string            XmlNode         = "BorderBatchWindow";
        const string            XmlLocation     = "Location";
        const string            XmlSize         = "Size";
        bool                    onLoadComplete  = false;
        
        bool _nodesBuilt = false;
        string _reImportFile = null;
        List<GUIBuilder.FormImport.ImportBase> _importData = null;
        
        Fallout4.WorkshopScript _sampleWorkshop = null;
        SubDivision _sampleSubDivision = null;
        
        List<Engine.Plugin.Forms.Keyword> _WorkshopBorderKeywordPool = null;
        List<Engine.Plugin.Forms.Static>  _WorkshopBorderForcedZMarkerPool = null;
        
        ToolTip tbNIFBuilderTargetFolderToolTip;
        ToolTip tbNIFBuilderSubDivisionFilePathSampleToolTip;
        ToolTip tbNIFBuilderWorkshopFilePathSampleToolTip;
        
        bool ReImportFileValid
        {
            get
            {
                return !string.IsNullOrEmpty( _reImportFile ) && System.IO.File.Exists( _reImportFile );
            }
        }
        
        #region Window management
        
        public BorderBatch()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadPoint( XmlNode, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlNode, XmlSize, this.Size );
            
            tbTargetFolder.Text = GodObject.Paths.DefaultNIFBuilderOutput;
            
            tbNIFBuilderSubDivisionFilePathSampleToolTip = new ToolTip();
            tbNIFBuilderSubDivisionFilePathSampleToolTip.ShowAlways = true;
            
            tbNIFBuilderWorkshopFilePathSampleToolTip = new ToolTip();
            tbNIFBuilderWorkshopFilePathSampleToolTip.ShowAlways = true;
            
            tbNIFBuilderTargetFolderToolTip = new ToolTip();
            tbNIFBuilderTargetFolderToolTip.ShowAlways = true;
            tbNIFBuilderTargetFolderToolTip.SetToolTip( tbTargetFolder, tbTargetFolder.Text );
            
            RepopulatePresetComboBoxes( cbWorkshopPresets   , NIFBuilder.Preset.WorkshopPresets   );
            RepopulatePresetComboBoxes( cbSubDivisionPresets, NIFBuilder.Preset.SubDivisionPresets );
            
            cbRestrictWorkshopBorderKeywords.Text = string.Format( "{0}\n{1}", "BorderBatchWindow.NodeDetection.Restrict".Translate(), GodObject.Plugin.Data.Files.Working.Filename );
            
            lvSubDivisions.SyncedEditorFormType = typeof( FormEditor.SubDivision );
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged += OnSubDivisionListChanged;
            UpdateSubDivisionList( false );
            
            //lvWorkshops.SyncedEditorFormType = typeof( FormEditor.WorkshopScript );
            GodObject.Plugin.Data.Workshops.ObjectDataChanged += OnWorkshopListChanged;
            UpdateWorkshopList( false );
            
            UpdateNIFFilePathSampleInternal();
            
            onLoadComplete = true;
            SetEnableState( true );
        }
        
        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Plugin.Data.SubDivisions.ObjectDataChanged -= OnSubDivisionListChanged;
            GodObject.Plugin.Data.Workshops.ObjectDataChanged -= OnWorkshopListChanged;
            GodObject.Windows.SetBorderBatchWindow( null, false );
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
        
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            
            btnClear.Enabled = _nodesBuilt;
            btnGenNodes.Enabled = !_nodesBuilt;
            btnBuildNIFs.Enabled = _nodesBuilt;
            btnImportNIFs.Enabled = _importData != null;// ReImportFileValid; // Use file selector for this
            //pnNIFBuilder.Enabled = !cbNIFBuilderFullATCSet.Checked;
            
            pnWindow.Enabled = enabled;
        }
        
        #endregion
        
        #region Repopulate Workshop Node Detection Forms
        
        void cbRestrictWorkshopBorderKeywordsChanged( object sender, EventArgs e )
        {
            RepopulateWorkshopNodeDetectionForms();
        }
        
        void RepopulateWorkshopNodeDetectionForms()
        {
            var thread = WorkerThreadPool.CreateWorker( Thread_RepopulateWorkshopNodeDetectionForms, null );
            if( thread == null ) return;
            thread.Start();
        }
        
        void ResetWorkshopNodeDetectionFormControl( ComboBox control )
        {
            if( control == null ) return;
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { ResetWorkshopNodeDetectionFormControl( control ); }, null );
                return;
            }
            
            control.Items.Clear();
            control.Items.Add( " [NONE] " );
        }
        
        void AddWorkshopNodeDetectionFormControlItem( ComboBox control, string text )
        {
            if( control == null ) return;
            if( string.IsNullOrEmpty( text ) ) return;
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { AddWorkshopNodeDetectionFormControlItem( control, text ); }, null );
                return;
            }
            
            control.Items.Add( text );
        }
        
        void FindAndSetWorkshopNodeDetectionFormControlDefault<TForm>( ComboBox control, List<TForm> forms, string suffix ) where TForm : Engine.Plugin.Form
        {
            if( control == null ) return;
            if( forms.NullOrEmpty() ) return;
            if( string.IsNullOrEmpty( suffix ) ) return;
            
            var lSuffix = suffix.ToLower();
            var sIndex = 0; // Default "None"
            
            // Try to find the "default" form
            for( int i = 0; i < forms.Count; i++ )
            {
                var form = forms[ i ];
                var fEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                //DebugLog.Write( string.Format( "\t{0} - \"{1}\" - 0x{2} \"{3}\"", i, ( form == null ? "null" : form.Signature ), ( form == null ? 0 : form.FormID ).ToString( "X8" ), fEDID ) );
                if( !string.IsNullOrEmpty( fEDID ) )
                {
                    var lcase = fEDID.ToLower();
                    if( lcase.EndsWith( lSuffix ) )
                    {
                        sIndex = 1 + i; // Skip "None"
                        break;
                    }
                }
            }
            SetWorkshopNodeDetectionFormControlDefault( control, sIndex );
        }
        
        void SetWorkshopNodeDetectionFormControlDefault( ComboBox control, int index )
        {
            if( control == null ) return;
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetWorkshopNodeDetectionFormControlDefault( control, index ); }, null );
                return;
            }
            
            control.SelectedIndex =
                index < 0 ? 0
                : index >= control.Items.Count ? 0
                : index;
        }
        
        void RepopulateWorkshopNodeDetectionControl<TForm>( string message, ComboBox control, int filter, string detectionSuffix, out List<TForm> list ) where TForm : Engine.Plugin.Form
        {
            list = null;
            if( control == null ) return;
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( message );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            ResetWorkshopNodeDetectionFormControl( control );
            
            var cForms = GodObject.Plugin.Data.Root.GetCollection<TForm>( true, true );
            if( cForms != null )
            {
                
                list = cForms.ToList<TForm>( filter );
                if( !list.NullOrEmpty() )
                {
                    list.Sort( (x, y)=>( x.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) < y.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ? -1 : 1 ) );
                    
                    var c = list.Count();
                    foreach( var form in list )
                        AddWorkshopNodeDetectionFormControlItem( control, string.Format( "0x{0} - \"{1}\"", form.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ), form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                    
                    // Try to find the "default" form
                    FindAndSetWorkshopNodeDetectionFormControlDefault( control, list, detectionSuffix );
                }
            }
            
            m.StopSyncTimer( message, tStart.Ticks );
            m.PopStatusMessage();
        }
        
        void Thread_RepopulateWorkshopNodeDetectionForms()
        {
            SetEnableState( false );
            
            var restricted = cbRestrictWorkshopBorderKeywords.Checked;
            var filter = restricted ? (int)GodObject.Plugin.Data.Files.Working.LoadOrder : -1;
            
            #region Keywords
            
            RepopulateWorkshopNodeDetectionControl<Engine.Plugin.Forms.Keyword>(
                "BorderBatchWindow.SearchingForKeyword".Translate(),
                cbWorkshopBorderKeyword,
                filter,
                "_WorkshopBorderGenerator",
                out _WorkshopBorderKeywordPool );
            
            #endregion
            
            #region Statics
            
            RepopulateWorkshopNodeDetectionControl<Engine.Plugin.Forms.Static>(
                "BorderBatchWindow.SearchingForForcedZStatic".Translate(),
                cbWorkshopForcedZStatic,
                filter,
                "_ForcedZ",
                out _WorkshopBorderForcedZMarkerPool );
            
            #endregion
            
            SetEnableState( true );
        }
        
        #endregion
        
        #region Sync'd list monitoring
        
        void UpdateWorkshopList( bool updateSampleDisplay )
        {
            var workshops = GodObject.Plugin.Data.Workshops.ToList();
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
            var subdivisions = GodObject.Plugin.Data.SubDivisions.ToList();
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
        
        void ClearEdgeFlagSegments()
        {
            GodObject.Windows.SetEnableState( false );
            
            _nodesBuilt = false;
            _reImportFile = null;
            _importData = null;
            
            var subdivisions = lvSubDivisions.GetSelectedSyncObjects();
            GUIBuilder.SubDivisionBatch.ClearEdgeFlagSegments( subdivisions );
            
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnClearClick( object sender, EventArgs e )
        {
            WorkerThreadPool.CreateWorker( ClearEdgeFlagSegments, null ).Start();
        }
        
        #endregion
        
        #region Calculate sub-division edge flag segments
        
        void THREAD_CalculateBorderNodesFromEdgeFlags()
        {
            GodObject.Windows.SetEnableState( false );
            
            DebugLog.OpenIndentLevel();
            
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            if( !workshops.NullOrEmpty() )
            {
                var kwSelected = cbWorkshopBorderKeyword.SelectedIndex - 1;
                if( kwSelected >= 0 )
                {
                    var keyword = _WorkshopBorderKeywordPool[ kwSelected ];
                    var fzSelected = cbWorkshopForcedZStatic.SelectedIndex - 1;
                    var forcedZ = fzSelected >= 0 ? _WorkshopBorderForcedZMarkerPool[ fzSelected ] : null;
                    var wsPreset = SelectedWorkshopPreset;
                    var nodeLength = ( wsPreset == null )
                        ? float.Parse( tbWorkshopNodeLength.Text )
                        : wsPreset.NodeLength;
                    var slopeAllowance = ( wsPreset == null )
                        ? float.Parse( tbWorkshopSlopeAllowance.Text )
                        : wsPreset.SlopeAllowance;
                    if( GUIBuilder.SubDivisionBatch.CalculateWorkshopEdgeFlagSegments(
                        workshops,
                        keyword,
                        forcedZ,
                        nodeLength,
                        slopeAllowance,
                        false ) )
                    {
                        _nodesBuilt = true;
                        _reImportFile = null;
                        _importData = null;
                    }
                }
            }
            
            var subDivisions = lvSubDivisions.GetSelectedSyncObjects();
            if( !subDivisions.NullOrEmpty() )
            {
                var sdPreset = SelectedSubDivisionPreset;
                var nodeLength = ( sdPreset == null )
                    ? float.Parse( tbSubDivisionNodeLength.Text )
                    : sdPreset.NodeLength;
                var slopeAllowance = ( sdPreset == null )
                    ? float.Parse( tbSubDivisionSlopeAllowance.Text )
                    : sdPreset.SlopeAllowance;
                var createImportData = ( sdPreset == null )
                    ? cbSubDivisionCreateImportData.Checked
                    : sdPreset.CreateImportData;
                if( GUIBuilder.SubDivisionBatch.CalculateSubDivisionEdgeFlagSegments(
                    subDivisions,
                    nodeLength,
                    slopeAllowance,
                    createImportData ) )
                {
                    _nodesBuilt = true;
                    _reImportFile = null;
                    _importData = null;
                }
            }
            
            DebugLog.CloseIndentLevel();
            GodObject.Windows.SetEnableState( true );
        }
        
        void btnGenNodesClick( object sender, EventArgs e )
        {
            WorkerThreadPool.CreateWorker( THREAD_CalculateBorderNodesFromEdgeFlags, null ).Start();
        }
        
        #endregion
        
        void UpdateNIFFilePathSampleInternal()
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "UpdateNIFFilePathSampleInternal()" } );
            
            var target = tbTargetFolder.Text;
            if( ( !string.IsNullOrEmpty( target ) )&&( target[ target.Length - 1 ] != '\\' ) )
                target += @"\";
            
            var wsPreset = FullChildSelectedWorkshopPreset;
            var wsFilePrefix = tbWorkshopFilePrefix.Text;
            var wsName = _sampleWorkshop    != null ? _sampleWorkshop   .NameFromEditorID : "";
            var wsSample = NIFBuilder.Mesh.BuildFilePath(
                tbMeshDirectory.Text,
                ( wsPreset != null ? wsPreset.MeshSubDirectory : tbWorkshopMeshSubDirectory.Text ),
                ( wsPreset != null ? wsPreset.FilePrefix : tbWorkshopFilePrefix.Text ),
                wsName,
                ( wsPreset != null ? wsPreset.FileSuffix : tbWorkshopFileSuffix.Text )
                );
            
            tbWorkshopSampleFilePath.Text = wsSample;
            tbNIFBuilderWorkshopFilePathSampleToolTip.SetToolTip( tbWorkshopSampleFilePath, wsSample );
            
            var sdPreset = FullChildSelectedSubDivisionPreset;
            var sdFilePrefix = tbSubDivisionFilePrefix.Text;
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
                ( sdPreset != null ? sdPreset.MeshSubDirectory : tbSubDivisionMeshSubDirectory.Text ),
                ( sdPreset != null ? sdPreset.FilePrefix : tbSubDivisionFilePrefix.Text ),
                sdName,
                ( sdPreset != null ? sdPreset.FileSuffix : tbSubDivisionFileSuffix.Text ), 1,
                sdNeighbourName, 1 );
            
            tbNIFBuilderSubDivisionSampleFilePath.Text = sdSample;
            tbNIFBuilderSubDivisionFilePathSampleToolTip.SetToolTip( tbNIFBuilderSubDivisionSampleFilePath, sdSample );
            
            DebugLog.CloseIndentLevel();
        }
        
        #region Build NIFs
        
        void CreateNIFs()
        {
            GodObject.Windows.SetEnableState( false );
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            _reImportFile = null;
            _importData = null;
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            var targetPath = tbTargetFolder.Text;
            var meshSuffix = tbMeshDirectory.Text;
            
            var workshops = lvWorkshops.GetSelectedSyncObjects();
            if( !workshops.NullOrEmpty() )
            {
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( "BorderBatchWindow.BuildingWorkshopBorders".Translate() );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var wsPreset = SelectedWorkshopPreset;
                if( wsPreset == null )
                {
                    var createImportData = cbWorkshopCreateImportData.Checked;
                    var subList = GUIBuilder.BorderBatch.CreateNIFs(
                        "Custom",
                        workshops,
                        float.Parse( tbWorkshopGradientHeight.Text ),
                        float.Parse( tbWorkshopGroundOffset.Text ),
                        float.Parse( tbWorkshopGroundSink.Text ),
                        targetPath, tbWorkshopTargetSuffix.Text,
                        meshSuffix, tbWorkshopMeshSubDirectory.Text,
                        tbWorkshopFilePrefix.Text, tbWorkshopFileSuffix.Text,
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
                
                m.StopSyncTimer( "GUIBuilder.BorderBatchWindow :: CreateNIFs() :: Workshop Borders :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
            }
            
            var subDivisions = lvSubDivisions.GetSelectedSyncObjects();
            if( !subDivisions.NullOrEmpty() )
            {
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( "BorderBatchWindow.BuildingSubDivisionBorders".Translate() );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var sdPreset = SelectedSubDivisionPreset;
                if( sdPreset == null )
                {
                    var createImportData = cbSubDivisionCreateImportData.Checked;
                    var subList = GUIBuilder.BorderBatch.CreateNIFs(
                        "Custom",
                        subDivisions,
                        float.Parse( tbSubDivisionGradientHeight.Text ),
                        float.Parse( tbSubDivisionGroundOffset.Text ),
                        float.Parse( tbSubDivisionGroundSink.Text ),
                        targetPath, tbSubDivisionTargetSuffix.Text,
                        meshSuffix, tbSubDivisionMeshSubDirectory.Text,
                        tbSubDivisionFilePrefix.Text, tbSubDivisionFileSuffix.Text,
                        cbSubDivisionCreateImportData.Checked );
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
                
                m.StopSyncTimer( "GUIBuilder.BorderBatchWindow :: CreateNIFs() :: Sub-Division Borders :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
            }
            
            _importData = list;
            m.StopSyncTimer( "GUIBuilder.BorderBatchWindow :: CreateNIFs() Completed in {0}", fStart.Ticks );
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
            return GUIBuilder.BorderBatch.CreateNIFs(
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
            return GUIBuilder.BorderBatch.CreateNIFs(
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
                return;
            WorkerThreadPool.CreateWorker( CreateNIFs, null ).Start();
        }
        
        void tbNIFBuilderTargetFolderMouseClick( object sender, MouseEventArgs e )
        {
            
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = GodObject.Paths.DefaultNIFBuilderOutput;
            
            if( fbd.ShowDialog() != DialogResult.OK )
            {
                var path = fbd.SelectedPath;
                if( !string.IsNullOrEmpty( path ) )
                {
                    GodObject.Paths.DefaultNIFBuilderOutput = path;
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
            UpdateNIFFilePathSampleInternal();
            UpdatingPresetUI = false;
        }
        
        void uiUpdateSubDivisionNIFFilePathSample( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            UpdatingPresetUI = true;
            cbSubDivisionPresets.SelectedIndex = 0;
            UpdateNIFFilePathSampleInternal();
            UpdatingPresetUI = false;
        }
        
        void tbNIFBuilderNIFFilePathSampleMouseClick( object sender, MouseEventArgs e )
        {
            // Supress user changes to the filepath sample textbox
            tbWorkshopSampleFilePath.Parent.Focus();
        }
        
        #endregion
        
        #region Import NIFs
        
        void btnImportNIFsClick( object sender, EventArgs e )
        {
            GodObject.Windows.SetEnableState( false );
            var reEnableGUI = true;
            
            if( !_importData.NullOrEmpty() )
            {
                reEnableGUI = false;
                var t = WorkerThreadPool.CreateWorker( ImportNIFs, null ).Start();
            }
            else if( !string.IsNullOrEmpty( _reImportFile ) )
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "BorderBatchWindow.ImportNIFsTitle".Translate();
                dlg.Filter = string.Format( "{0}|NIFBuilder_BorderNIFs_*.txt|{1}|*.*", "BorderBatchWindow.ImportExportFilter".Translate(), "BorderBatchWindow.ImportAllFilter".Translate() );
                dlg.InitialDirectory = GodObject.Paths.DefaultNIFBuilderOutput;
                dlg.FileName = _reImportFile;
                dlg.RestoreDirectory = true;
                dlg.DereferenceLinks = true;
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.Multiselect = false;
                if( dlg.ShowDialog() == DialogResult.OK )
                {
                    var fileToLoad = dlg.FileName; // Allow import files from anywhere
                    if( !string.IsNullOrEmpty( fileToLoad ) )
                    {
                        DebugLog.WriteLine( "User selected: " + fileToLoad );
                        // eg:
                        //User selected: C:\Games\Steam\steamapps\common\Fallout 4\BorderBuilder\Output\NIFBuilder_BorderNIFs_AnnexTheCommonwealth_esp_29_09_2018_9_00_00_AM.txt
                        
                        // If the plugin loader returns true, the loader thread will re-enable the GUI
                        //reEnableGUI &= !GodObject.Plugin.Load( fileToLoad );
                        
                        _reImportFile = fileToLoad;
                        if( !ReImportFileValid )
                        {
                            _reImportFile = null;
                        }
                        else
                        {
                            reEnableGUI = false;
                            WorkerThreadPool.CreateWorker( ImportNIFs, null ).Start();
                        }
                        
                    }
                }
                dlg.Dispose();
            }
            
            if( reEnableGUI )
                GodObject.Windows.SetEnableState( true );
        }
        
        void ImportNIFs()
        {
            if( !_importData.NullOrEmpty() )
            {
                bool tmp = false;
                if( !FormImport.ImportBase.ShowImportDialog( _importData, true, ref tmp ) )
                    GodObject.Windows.SetEnableState( true );
            }
            else if( !string.IsNullOrEmpty( _reImportFile ) )
            {
                if( !GUIBuilder.BorderBatch.ImportNIFs( _reImportFile, true ) )
                    GodObject.Windows.SetEnableState( true );
            }
        }
        
        #endregion
        
        #region Presets
        
        #region Update Preset UI
        
        static void RepopulatePresetComboBoxes( ComboBox cb, List<NIFBuilder.Preset> presets )
        {
            cb.Items.Clear();
            cb.Items.Add( "BorderBatchWindow.PresetCustom".Translate() );
            var count = presets == null ? 0 : presets.Count;
            if( count > 0 )
            {
                for( int i = 0; i < count; i++ )
                    cb.Items.Add( presets[ i ].Name );
                cb.SelectedIndex = 1;
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
            List<NIFBuilder.Preset> presets, int index, ComboBox cbPresets,
            TextBox targetSuffix, TextBox meshSubDirectory,
            TextBox filePrefix, TextBox fileSuffix,
            TextBox nodeLength, TextBox slopeAllowance,
            TextBox gradientHeight, TextBox groundOffset, TextBox groundSink,
            CheckBox createImportData )
        {
            if( UpdatingPresetUI )
                return;
            
            UpdatingPresetUI = true;
            if( index < 0 )
                index = 0;
            
            // Check is important so we don't trigger the UI callback by setting the same value and cause a stack overflow
            // disable once RedundantCheckBeforeAssignment
            if( cbPresets.SelectedIndex != index )
                cbPresets.SelectedIndex = index;
            
            if( index > 0 )
            {
                var preset = presets[ index - 1 ];
                SetPresetUIValue( targetSuffix      , preset.TargetSuffix     );
                SetPresetUIValue( meshSubDirectory  , preset.MeshSubDirectory );
                SetPresetUIValue( filePrefix        , preset.FilePrefix       );
                SetPresetUIValue( fileSuffix        , preset.FileSuffix       );
                SetPresetUIValue( nodeLength        , preset.NodeLength       );
                SetPresetUIValue( slopeAllowance    , preset.SlopeAllowance   );
                SetPresetUIValue( gradientHeight    , preset.GradientHeight  , preset.SetOfPresets );
                SetPresetUIValue( groundOffset      , preset.GroundOffset    , preset.SetOfPresets );
                SetPresetUIValue( groundSink        , preset.GroundSink      , preset.SetOfPresets );
                SetPresetUIValue( createImportData  , preset.CreateImportData, preset.SetOfPresets );
            }
            else if( createImportData != null )
                SetPresetUIValue( createImportData  , createImportData.Checked, false );
            
            UpdatingPresetUI = false;
        }
        
        void UpdateWorkshopPresetUI( int index )
        {
            UpdatePresetUI(
                NIFBuilder.Preset.WorkshopPresets,
                index,
                cbWorkshopPresets,
                tbWorkshopTargetSuffix,
                tbWorkshopMeshSubDirectory,
                tbWorkshopFilePrefix,
                tbWorkshopFileSuffix,
                tbWorkshopNodeLength,
                tbWorkshopSlopeAllowance,
                tbWorkshopGradientHeight,
                tbWorkshopGroundOffset,
                tbWorkshopGroundSink,
                cbWorkshopCreateImportData );
            UpdateNIFFilePathSampleInternal();
        }
        
        void UpdateSubDivisionPresetUI( int index )
        {
            UpdatePresetUI(
                NIFBuilder.Preset.SubDivisionPresets,
                index,
                cbSubDivisionPresets,
                tbSubDivisionTargetSuffix,
                tbSubDivisionMeshSubDirectory,
                tbSubDivisionFilePrefix,
                tbSubDivisionFileSuffix,
                tbSubDivisionNodeLength,
                tbSubDivisionSlopeAllowance,
                tbSubDivisionGradientHeight,
                tbSubDivisionGroundOffset,
                tbSubDivisionGroundSink,
                cbSubDivisionCreateImportData );
            UpdateNIFFilePathSampleInternal();
        }
        
        #endregion
        
        #region Get Selected Preset
        
        NIFBuilder.Preset GetPreset( List<NIFBuilder.Preset> list, int index, bool fullChild = false )
        {
            if( index < 0 ) return null;
            if( list.NullOrEmpty() ) return null;
            if( index >= list.Count ) return null;
            var preset = list[ index ];
            if( ( fullChild )&&( preset.SetOfPresets ) )
            {
                for( int i = 0; i < preset.SubSets.Count; i++ )
                {
                    var child = GetPreset( preset.SubSets, i, fullChild );
                    if( child != null ) return child;
                }
            }
            return preset;
        }
        
        int _SelectedWorkshopPreset = -1;
        int _SelectedSubDivisionPreset = -1;
        
        NIFBuilder.Preset SelectedWorkshopPreset
        {
            get
            {
                if( _SelectedWorkshopPreset < 0 ) return null;
                return GetPreset(
                    NIFBuilder.Preset.WorkshopPresets,
                    _SelectedWorkshopPreset,
                    false );
            }
        }
        
        NIFBuilder.Preset FullChildSelectedWorkshopPreset
        {
            get
            {
                if( _SelectedWorkshopPreset < 0 ) return null;
                return GetPreset(
                    NIFBuilder.Preset.WorkshopPresets,
                    _SelectedWorkshopPreset - 1,
                    true );
            }
        }
        
        NIFBuilder.Preset SelectedSubDivisionPreset
        {
            get
            {
                if( _SelectedSubDivisionPreset < 0 ) return null;
                return GetPreset(
                    NIFBuilder.Preset.SubDivisionPresets,
                    _SelectedSubDivisionPreset - 1,
                    false );
            }
        }
        
        NIFBuilder.Preset FullChildSelectedSubDivisionPreset
        {
            get
            {
                if( _SelectedSubDivisionPreset < 0 ) return null;
                return GetPreset(
                    NIFBuilder.Preset.SubDivisionPresets,
                    _SelectedSubDivisionPreset,
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
            if( UpdatingPresetUI ) return;
            UpdateWorkshopPresetUI( 0 );
        }
        
        void uiSubDivisionNIFBuilderChanged( object sender, EventArgs e )
        {
            if( UpdatingPresetUI ) return;
            UpdateSubDivisionPresetUI( 0 );
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
                    if( ( _WorkshopBorderKeywordPool.NullOrEmpty() )||( _WorkshopBorderForcedZMarkerPool.NullOrEmpty() ) )
                        RepopulateWorkshopNodeDetectionForms();
                    if( lvWorkshops.Visible )
                        lvWorkshops.RepopulateListView();
                    break;
                    
            }
        }
        
        #endregion
        
    }
    
}
