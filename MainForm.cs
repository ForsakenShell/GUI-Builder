/*
 * MainForm.cs
 * 
 * Main form for Border Builder.
 * 
 * User: 1000101
 * Date: 24/11/2017
 * Time: 10:55 PM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Border_Builder
{
    
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class fMain : Form
    {
        
        #region Static references to the main form (god objects suck, but fuck it)
        
        private static fMain    _fMain;
        
        public static fMain Self
        {
            get{
                return _fMain;
            }
        }
        
        #endregion
        
        
        // Main render transform
        RenderTransform transform;
        
        #region Main Start/Stop
        
        public fMain()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            _fMain = this;
        }
        
        void MenuExitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        void FMainLoad(object sender, EventArgs e)
        {
            // DEBUG Code (since my dev path isn't the game install path and I'm too lazy to enumerate it atm)
            var machineName = Environment.MachineName;
            if( machineName == "MASTER" )
                bbGlobal.GlobalPath = "C:\\Games\\Steam\\steamapps\\common\\Fallout 4";
            else if( machineName == "ERICS-LAPTOP" )
                bbGlobal.GlobalPath = "C:\\Utils\\dev\\Projects\\Border Builder";
            
            #if DEBUG
            cbRenderBuildVolumes.ThreeState = true;
            cbRenderBorders.ThreeState = true;
            #endif
            
            ResetAllControls();
            PreloadImportMods();
            PreloadWorldspaces();
        }
        
        #endregion
        
        #region Global Form and Status bar update
        
        /// <summary>
        /// Long running functions should disable the main form so the user can't spam inputs.  Don't forget to enable the form again after the long-running function is complete so the user can continue to use the program.
        /// </summary>
        /// <param name="enabled">true to enable the form and it's controls, false to disable the form and it's controls.</param>
        public void SetEnableState( bool enabled )
        {
            this.Enabled = enabled;
        }
        
        public void UpdateStatusMessage( string message )
        {
            sbiCaption.Text = message;
            sbMain.Invalidate();
            sbMain.Refresh();
        }
        
        public void UpdateStatusProgress( int value )
        {
            sbiProgress.Value = value;
        }
        
        public void SetStatusProgressMinimum( int min )
        {
            sbiProgress.Minimum = min;
        }
        
        public void SetStatusProgressMaximum( int max )
        {
            sbiProgress.Maximum = max;
        }
        
        #endregion
        
        #region Grouped form control reset/update
        
        void ResetRenderWindowControls( bool fullReset = false )
        {
            var worldspace = SelectedWorldspace();
            var selectedImport = SelectedImportMod();
            btnCellWindowRedraw.Enabled = ( worldspace != null );
            
            if( !fullReset )
                return;
            
            // Worldspace and import mod selected?
            if( ( worldspace == null )||( selectedImport == null ) )
                return;
            
            lbVolumeParents.Items.Clear();
            gbRenderSelectedOnly.Enabled = false;
            
            // Set appropriate maskable parents
            if( selectedImport.VolumeParents.Count > 0 )
            {
                foreach( var volumeParent in selectedImport.VolumeParents )
                   if( worldspace.EditorID == volumeParent.WorldspaceEDID )
                        lbVolumeParents.Items.Add( volumeParent.FormID );
            }
            
            gbRenderSelectedOnly.Enabled = true;
            
            // Allow editing so long as welding hasn't been done
            gbEditOptions.Enabled = !selectedImport.Welded;
            if( !gbEditOptions.Enabled )
                cbEditModeEnable.Checked = false;
            
        }
        
        void ResetWorldspaceControls( bool fullReset = false )
        {
            // Worldspace controls
            btnCellWindowRedraw.Enabled = false;
            btnLoadWorldspaceHeightTextures.Enabled = false;
            cbRenderWorldspaceHeightTextures.Checked = false;
            tbWorldspaceFormIDEditorID.Clear();
            tbWorldspaceGridBottomX.Clear();
            tbWorldspaceGridBottomY.Clear();
            tbWorldspaceGridTopX.Clear();
            tbWorldspaceGridTopY.Clear();
            tbWorldspaceHeightmapTexture.Clear();
            tbWorldspaceWaterHeightsTexture.Clear();
            tbWorldspaceMapHeightMax.Clear();
            tbWorldspaceMapHeightMin.Clear();
            if( !fullReset )
                return;
            cbWorldspace.Text = "";
            cbWorldspace.Items.Clear();
        }
        
        void ResetImportModControls( bool fullReset = false )
        {
            // Import mod controls
            btnLoadImportModBuildVolumes.Enabled = false;
            btnWeldImportVolumeVerts.Enabled = false;
            btnBuildImportModVolumeBorders.Enabled = false;
            if( !fullReset )
                return;
            tbWeldThreshold.Text = "64.0000";
            cbImportMod.Text = "";
            cbImportMod.Items.Clear();
        }
        
        void ResetAllControls()
        {
            ResetWorldspaceControls( true );
            ResetImportModControls( true );
            pnRenderWindow.Enabled = false;
            pnRenderWindow.Visible = false;
            sbiCaption.Text = "";
            sbiMouseToCellGrid.Text = "";
            sbiMouseToWorldspace.Text = "";
        }
        
        #endregion
        
        #region Preload on startup/demand
        
        bool PreloadImportMods( bool forceReload = false )
        {
            
            if( ( forceReload )||( bbGlobal.ImportMods == null ) )
                bbGlobal.ImportMods = new List<bbImportMod>();
            
            foreach( string selectedImportPath in bbGlobal.ImportModPaths )
            {
                string tmpStr = selectedImportPath;
                int tmp = selectedImportPath.LastIndexOf( @"\" );
                if( tmp < 1 )
                    tmp = selectedImportPath.LastIndexOf( @"/" );
                if( tmp > 1 )
                    tmpStr = selectedImportPath.Substring( tmp + 1 );
                
                var selectedImport = new bbImportMod( tmpStr, selectedImportPath );
                selectedImport.Preload();
                bbGlobal.ImportMods.Add( selectedImport );
                cbImportMod.Items.Add( tmpStr );
            }
            
            bbGlobal.ImportMods.Sort( ( x, y ) =>
                               {
                                   return string.Compare( x.Name, y.Name );
                               } );
            return true;
        }
        
        bool PreloadWorldspaces( bool forceReload = false )
        {
            
            if( ( forceReload )||( bbGlobal.Worldspaces == null ) )
                bbGlobal.Worldspaces = new List<bbWorldspace>();
            
            foreach( string worldspacePath in bbGlobal.WorldspacePaths )
            {
                string tmpStr = worldspacePath;
                int tmp = worldspacePath.LastIndexOf( @"\" );
                if( tmp < 1 )
                    tmp = worldspacePath.LastIndexOf( @"/" );
                if( tmp > 1 )
                    tmpStr = worldspacePath.Substring( tmp + 1 );
                
                var worldspace = new bbWorldspace( tmpStr, worldspacePath );
                worldspace.Preload();
                bbGlobal.Worldspaces.Add( worldspace );
                cbWorldspace.Items.Add( tmpStr );
            }
            bbGlobal.Worldspaces.Sort( ( x, y ) =>
                               {
                                   return string.Compare( x.Name, y.Name );
                               } );
            return true;
        }
        
        #endregion
        
        #region Object enumeration (bbWorldspace, bbImportMod, etc)
        
        bbWorldspace SelectedWorldspace()
        {
            int index = cbWorldspace.SelectedIndex;
            return index >= 0 ? bbGlobal.Worldspaces[ index ] : null;
        }
        
        bbImportMod SelectedImportMod()
        {
            int index = cbImportMod.SelectedIndex;
            return index >= 0 ? bbGlobal.ImportMods[ index ] : null;
        }
        
        List<VolumeParent> SelectedVolumeParents()
        {
            var importMod = SelectedImportMod();
            if( importMod == null ) return null;
            
            var list = new List<VolumeParent>();
            foreach( var item in lbVolumeParents.SelectedItems )
            {
                list.Add( importMod.VolumeByFormID( (string)item ) );
            }
            return list;
        }
        
        #endregion
        
        #region Worldspace form controls
        
        void LoadWorldspaceTextures( bbWorldspace worldspace, bool renderBitmaps )
        {
            if( worldspace == null )
                return;
            
            bool loadOk = 
                ( worldspace.LoadLandHeightMap( this ) )&&
                ( worldspace.LoadWaterHeightMap( this ) );
            
            if( loadOk )
            {
                if( renderBitmaps )
                    worldspace.RenderHeightMap( this );
                ResetRenderWindowControls();
            }
            else
            {
                btnLoadWorldspaceHeightTextures.Enabled = true;
            }
        }
        
        void CbWorldspaceSelectedIndexChanged(object sender, EventArgs e)
        {
            ResetWorldspaceControls();
            var worldspace = SelectedWorldspace();
            if( worldspace == null )
                return;
            tbWorldspaceFormIDEditorID.Text = worldspace.FormID + " [" + worldspace.EditorID.ToString( "X" ) + "]";
            tbWorldspaceMapHeightMax.Text = worldspace.MaxHeight.ToString( "n6" );
            tbWorldspaceMapHeightMin.Text = worldspace.MinHeight.ToString( "n6" );
            tbWorldspaceGridTopX.Text = worldspace.CellNW.X.ToString();
            tbWorldspaceGridTopY.Text = worldspace.CellNW.Y.ToString();
            tbWorldspaceGridBottomX.Text = worldspace.CellSE.X.ToString();
            tbWorldspaceGridBottomY.Text = worldspace.CellSE.Y.ToString();
            tbWorldspaceHeightmapTexture.Text = worldspace.LandHeights_Texture_File;
            tbWorldspaceWaterHeightsTexture.Text = worldspace.WaterHeights_Texture_File;
            btnLoadWorldspaceHeightTextures.Enabled = ( !string.IsNullOrEmpty( worldspace.LandHeights_Texture_File ) )&&( !string.IsNullOrEmpty( worldspace.WaterHeights_Texture_File ) );
            
            ResetRenderWindowControls( true );
        }
        
        void BtnLoadWorldspaceHeightTexturesClick(object sender, EventArgs e)
        {
            var worldspace = SelectedWorldspace();
            if( ( worldspace == null )||( string.IsNullOrEmpty( worldspace.LandHeights_Texture_File ) )||( string.IsNullOrEmpty( worldspace.WaterHeights_Texture_File ) ) )
               return;
            
            btnLoadWorldspaceHeightTextures.Enabled = false;
            SetEnableState( false );
            
            LoadWorldspaceTextures( worldspace, cbRenderWorldspaceHeightTextures.Checked );
            
            SetEnableState( true );
        }
        
        #endregion
        
        #region Import mod form controls
        
        void CbImportModSelectedIndexChanged(object sender, EventArgs e)
        {
            ResetImportModControls();
            var index = cbImportMod.SelectedIndex;
            var selectedImport = bbGlobal.ImportMods[ index ];
            btnLoadImportModBuildVolumes.Enabled = ( !string.IsNullOrEmpty( selectedImport.BuildVolumeRef_File ) );
            btnBuildImportModVolumeBorders.Enabled = ( selectedImport.VolumeParents != null );
            btnWeldImportVolumeVerts.Enabled = ( selectedImport.VolumeParents != null );
        }
        
        void BtnLoadImportModBuildVolumesClick(object sender, EventArgs e)
        {
            var selectedImport = SelectedImportMod();
            if( selectedImport == null ) return;
            SetEnableState( false );
            selectedImport.LoadBuildVolumesFile();
            btnBuildImportModVolumeBorders.Enabled = ( selectedImport.VolumeParents != null );
            btnWeldImportVolumeVerts.Enabled = ( selectedImport.VolumeParents != null );
            ResetRenderWindowControls( true );
            SetEnableState( true );
        }
        
        void TbWeldThresholdKeyPress( object sender, KeyPressEventArgs e )
        {
            if( ( !char.IsControl( e.KeyChar ) )&&( !char.IsDigit( e.KeyChar ) )&&( e.KeyChar != '.' ) )
            {
                e.Handled = true;
            }
        }
        
        void BtnWeldImportVolumeVertsClick( object sender, EventArgs e )
        {
            var selectedImport = SelectedImportMod();
            if( selectedImport == null )
                return;
            SetEnableState( false );
            
            // Weld the corners across the world!
            selectedImport.WeldVerticies( float.Parse( tbWeldThreshold.Text ), cbWeldAllTogether.Checked );
            
            // Welding prevents editor mode
            gbEditOptions.Enabled = false;
            
            SetEnableState( true );
        }
        
        void BuildSelectedModBorders( bbImportMod selectedImport )
        {
            if( selectedImport == null )
                return;
            SetEnableState( false );
            
            //var volumeParent = selectedImport.VolumeParents[ 0 ];
            foreach( var volumeParent in selectedImport.VolumeParents )
            {
                //if( volumeParent.EditorID == 0x01059EDC )
                {
                var worldspace = bbGlobal.WorldspaceFromEditorID( volumeParent.WorldspaceEDID );
                if( worldspace != null )
                {
                    
                    if( worldspace.LandHeightMap == null )
                        worldspace.LoadLandHeightMap( this );
                        
                    if( worldspace.WaterHeightMap == null )
                        worldspace.LoadWaterHeightMap( this );
                    
                    UpdateStatusMessage( string.Format( "Building borders for {0}...", volumeParent.FormID ) );
                    
                    volumeParent.BuildBorders();
                }
                }
            }
            SetEnableState( true );
        }
        
        void BtnBuildImportModVolumeBordersClick( object sender, EventArgs e )
        {
            BuildSelectedModBorders( SelectedImportMod() );
        }
        void VolumeBordersToolStripMenuItemClick( object sender, EventArgs e )
        {
            BuildSelectedModBorders( SelectedImportMod() );
        }
        
        #endregion
        
        void CbEditModeEnableCheckedChanged( object sender, EventArgs e )
        {
            if( cbEditModeEnable.Checked )
            {
                if( transform == null )
                {
                    cbEditModeEnable.Checked = false;
                    return;
                }
                transform.EnableEditorMode( this, sbiEditorSelectionMode, tbEMHotKeys );
            }
            else if( transform != null )
            {
                transform.DisableEditorMode();
            }
        }
        
        #region Rendering and controls
        
        #region Rendering
        
        public int RenderWindowWidth
        {
            get
            {
                return pbRenderWindow.Width;
            }
        }
        
        public int RenderWindowHeight
        {
            get
            {
                return pbRenderWindow.Height;
            }
        }
        
        /*
        public Maths.Vector2i RenderWindowCellNW
        {
            get
            {
                return pbRenderWindow.Width;
            }
        }
        */
        
        #endregion
        
        void GetRenderOptions( out bool renderNonPlayable, out bool renderLand, out bool renderWater, out bool renderCellGrid, out bool renderBuildVolumes, out bool renderBorders, out bool renderSelectedOnly, out bool exportPNG )
        {
            var selectedImport = SelectedImportMod();
            var selectedVolumes = SelectedVolumeParents();
            
            renderNonPlayable   = cbRenderOverRegion.Checked;
            renderLand          = cbRenderLandHeight.Checked;
            renderWater         = cbRenderWaterHeight.Checked;
            renderCellGrid      = cbRenderCellGrid.Checked;
            renderBuildVolumes  = ( selectedImport != null )&&( selectedImport.VolumeParents != null )&&( cbRenderBuildVolumes.Checked );
            renderBorders       = ( selectedImport != null )&&( selectedImport.VolumeParents != null )&&( cbRenderBorders.Checked );
            renderSelectedOnly  = ( !selectedVolumes.NullOrEmpty() )&&( cbRenderSelectedOnly.Checked );
            exportPNG           = cbExportPNG.Checked;
            
        }
        
        void BtnCellWindowRedrawClick(object sender, EventArgs e)
        {
            var selectedWorldspace = SelectedWorldspace();
            if( selectedWorldspace == null )
                return;
            
            SetEnableState( false );
            
            var selectedImport = SelectedImportMod();
            var selectedVolumes = SelectedVolumeParents();
            
            bool renderNonPlayable, renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, renderSelectedOnly, exportPNG;
            GetRenderOptions( out renderNonPlayable, out renderLand, out renderWater, out renderCellGrid, out renderBuildVolumes, out renderBorders, out renderSelectedOnly, out exportPNG );
            
            if( !renderSelectedOnly ) selectedVolumes = null;
            
            if( ( selectedWorldspace.LandHeight_Bitmap == null )||( selectedWorldspace.WaterHeight_Bitmap == null ) )
                selectedWorldspace.RenderHeightMap( this );
            
            Maths.Vector2i cellNW;
            Maths.Vector2i cellSE;
            //var scale = 0f;
            
            if( renderSelectedOnly )
            {
                cellNW = new Maths.Vector2i( int.MaxValue, int.MinValue );
                cellSE = new Maths.Vector2i( int.MinValue, int.MaxValue );
                foreach( var volume in selectedVolumes )
                {
                    var volNW = volume.CellNW;
                    var volSE = volume.CellSE;
                    if( volNW.X < cellNW.X ) cellNW.X = volNW.X;
                    if( volNW.Y > cellNW.Y ) cellNW.Y = volNW.Y;
                    if( volSE.X > cellSE.X ) cellSE.X = volSE.X;
                    if( volSE.Y < cellSE.Y ) cellSE.Y = volSE.Y;
                }
                //var size = new Maths.Vector2i( cellSE.X - cellNW.X + 1, cellNW.Y - cellSE.Y + 1 );
                //var world = new Maths.Vector2i( worldspace.CellSE.X - worldspace.CellNW.X + 1, worldspace.CellNW.Y - worldspace.CellSE.Y + 1 );
                //scale = RenderTransform.CalculateScale( size, world ); // bbConstant.MaxZoom;
            }
            else
            {
                if( renderNonPlayable )
                {
                    var hmCSize = selectedWorldspace.HeightMapCellSize;
                    cellNW = selectedWorldspace.HeightMapCellOffset;
                    cellSE = new Maths.Vector2i(
                        cellNW.X + ( hmCSize.X - 1 ),
                        cellNW.Y - ( hmCSize.Y - 1 ) );
                }
                else
                {
                    cellNW = selectedWorldspace.CellNW;
                    cellSE = selectedWorldspace.CellSE;
                }
            }
            
            if( transform == null )
            {
                UpdateStatusMessage( "Creating render transform..." );
                transform = new RenderTransform( pbRenderWindow );
                
                // Reconfigure editor mode for the new transform
                if( cbEditModeEnable.Checked )
                    transform.EnableEditorMode( this, sbiEditorSelectionMode, tbEMHotKeys );
                
            }
            
            UpdateStatusMessage( "Updating render transform..." );
            
            // Update data references
            transform.worldspace = selectedWorldspace;
            transform.importMod = selectedImport;
            transform.renderVolumes = selectedVolumes;
            
            // Update physical transform
            transform.UpdateCellClipper( cellNW, cellSE, false );
            
            //transform.SetScale( 0.0125f / 2.0f, false );
            //transform.SetViewCentre( Maths.Vector2f.Zero, false );
            
            //var nw = new Maths.Vector2f( cellNW.X * bbConstant.WorldMap_Resolution, cellNW.Y * bbConstant.WorldMap_Resolution );
            //var se = new Maths.Vector2f( cellSE.X * bbConstant.WorldMap_Resolution, cellSE.Y * bbConstant.WorldMap_Resolution );
            //transform.SetScale( transform.CalculateScale( transform.GetClipperCellSize() ), false );
            //transform.SetViewCentre( ( nw + se ) / 2f, false );
            
            var nw = new Maths.Vector2f( cellNW.X * bbConstant.WorldMap_Resolution, cellNW.Y * bbConstant.WorldMap_Resolution );
            var s = transform.GetClipperCellSize();
            var ws = s * bbConstant.WorldMap_Resolution;
            var vc = new Maths.Vector2f(
                nw.X + ws.X * 0.5f,
                nw.Y - ws.Y * 0.5f );
            transform.SetScale( transform.CalculateScale( s ), false );
            transform.SetViewCentre( vc, false );
            
            transform.RecomputeSceneClipper();
            
            #if DEBUG
            
            transform.debugRenderBuildVolumes = cbRenderBuildVolumes.CheckState == CheckState.Indeterminate;
            transform.debugRenderBorders = cbRenderBorders.CheckState == CheckState.Indeterminate;
            
            #endif
            
            UpdateStatusMessage( "Rendering final bitmap..." );
            transform.RenderScene( renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, false, true );
            
            if( exportPNG )
                transform.RenderToPNG();
            
            UpdateStatusMessage( "Refreshing..." );
            pbRenderWindow.Refresh();
            pnRenderWindow.Enabled = true;
            pnRenderWindow.Visible = true;
            pnRenderWindow.Tag = selectedWorldspace;
            UpdateStatusMessage( string.Format( "Rendered map: ({0},{1})-({2},{3})", transform.CellNW.X, transform.CellNW.Y, transform.CellSE.X, transform.CellSE.Y ) );
            
            SetEnableState( true );
        }
        
        /*
        void UpdateSelectedOnlyVolume()
        {
            var selectedImport = SelectedImportMod();
            if( selectedImport == null )
            {
                cbRenderJumpTo.Tag = null;
                return;
            }
            
            var volumeParent = selectedImport.VolumeByFormID( cbRenderJumpTo.Text );
            cbRenderSelectedOnly.Checked = true;
            btnRenderJumpTo.Enabled = ( volumeParent != null );
            cbRenderJumpTo.Tag = volumeParent;
            if( volumeParent != null )
                UpdateStatusMessage( string.Format( "{0}, {1}-{2}", volumeParent.FormID, volumeParent.CellNW.ToString(), volumeParent.CellSE.ToString() ) );
        }
        
        void CbRenderSelectedOnlyCheckedChanged( object sender, EventArgs e )
        {
            if( cbRenderSelectedOnly.Checked == false )
                return;
            UpdateSelectedOnlyVolume();
        }
        
        void CbRenderJumpToSelectedIndexChanged( object sender, EventArgs e )
        {
            if( cbRenderJumpTo.SelectedIndex == -1 )
            {
                cbRenderSelectedOnly.Checked = false;
                cbRenderJumpTo.Tag = null;
                return;
            }
            UpdateSelectedOnlyVolume();
        }
        
        void BtnRenderJumpToClick( object sender, EventArgs e )
        {
            UpdateSelectedOnlyVolume();
            var worldspace = SelectedWorldspace();
            var selectedImport = SelectedImportMod();
            var locationFormID = cbRenderJumpTo.Text;
            if( ( worldspace == null )||( selectedImport == null )||( string.IsNullOrEmpty( locationFormID ) ) )
                return;
            
            SetEnableState( false );
            
            RenderCellWindow();
            
            var volumeParent = selectedImport.VolumeByFormID( locationFormID );
            if( volumeParent != null )
            {
                var wWidth  = ( worldspace.HeightMap_Width  - pnRenderWindow.Width  ) / 2f;
                var wHeight = ( worldspace.HeightMap_Height - pnRenderWindow.Height ) / 2f;
                var lX = volumeParent.Position.X * RenderScale;
                var lY = volumeParent.Position.Y * RenderScale;
                //pnRenderWindow.HorizontalScroll.Value = (int)( wWidth  + lX );
                //pnRenderWindow.VerticalScroll.Value   = (int)( wHeight - lY );
            }
            SetEnableState( true );
            
        }
        */
        
        void PbRenderWindowMouseUp( object sender, MouseEventArgs e )
        {
            if( transform == null ) return;
            if( transform.MouseSelectionMode ) return;
            
            var mouseWorldPos = transform.ScreenspaceToWorldspace( e.X, e.Y );
            
            List<VolumeParent> parents = null;
            List<BuildVolume> volumes = null;
            if( !transform.ImportMod.TryGetVolumesFromPos( mouseWorldPos, out parents, out volumes, transform.renderVolumes ) )
                return;
            
            foreach( var parent in parents )
            {
                if( lbVolumeParents.SelectedItems.Contains( parent.FormID ) )
                    lbVolumeParents.SelectedItems.Remove( parent.FormID );
                else
                    lbVolumeParents.SelectedItems.Add( parent.FormID );
            }
        }
        
        void PbRenderWindowMouseMove( object sender, MouseEventArgs e )
        {
            if( transform == null ) return;
            
            sbiMouseToCellGrid.Text = transform.ScreenspaceToCellGrid( e.X, e.Y ).ToString();
            var mouseWorldPos = transform.ScreenspaceToWorldspace( e.X, e.Y );
            sbiMouseToWorldspace.Text = mouseWorldPos.ToString();
            
            List<VolumeParent> parents = null;
            List<BuildVolume> volumes = null;
            string status = "";
            if(
                ( transform.ImportMod != null )&&
                ( transform.ImportMod.TryGetVolumesFromPos( mouseWorldPos, out parents, out volumes, transform.renderVolumes ) )
            ){
                status = parents[ 0 ].FormID;
                for( int i = 1; i < parents.Count; i++ )
                    status = status + ", " + parents[ i ].FormID;
            }
            
            UpdateStatusMessage( status );
            
            var rthlParents = transform.HighlightParents;
            var rthlVolumes = transform.HighlightVolumes;
            
            bool volumesChanged = false;
            
            if( parents != null )
            {
                if(
                    ( rthlParents == null )||
                    (
                        ( !rthlParents.Contains( parents ) )||
                        ( !parents.Contains( rthlParents ) )
                    )
                )   volumesChanged = true;
            }
            else if( rthlParents != null )
                volumesChanged = true;
            
            if( volumes != null )
            {
                if(
                    ( rthlVolumes == null )||
                    (
                        ( !rthlVolumes.Contains( volumes ) )||
                        ( !volumes.Contains( rthlVolumes ) )
                    )
                )   volumesChanged = true;
            }
            else if( rthlVolumes != null )
                volumesChanged = true;
            
            if( !volumesChanged ) return;
            
            transform.hlParents = parents;
            transform.hlVolumes = volumes;
            
            // The editor will handle redrawing the scene
            if( transform.MouseSelectionMode ) return;
            
            // The editor won't handle redrawing the scene
            bool renderNonPlayable, renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, renderSelectedOnly, exportPNG;
            GetRenderOptions( out renderNonPlayable, out renderLand, out renderWater, out renderCellGrid, out renderBuildVolumes, out renderBorders, out renderSelectedOnly, out exportPNG );
            
            renderLand = false;
            renderWater = false;
            transform.RenderScene( renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, false, false );
        }
        
        void PbRenderWindowSizeChanged( object sender, EventArgs e )
        {
            if( transform == null ) return;
            
            transform.RenderTargetSizeChanged();
        }
        
        void PbRenderWindowPreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
        {
            if( transform == null ) return;
            
            var viewCentre = transform.GetViewCentre();
            var invScale = transform.GetInvScale();
            
            var code = e.KeyCode;
            if( code == Keys.Left )     viewCentre.X -= invScale;
            if( code == Keys.Right )    viewCentre.X += invScale;
            if( code == Keys.Up )       viewCentre.Y += invScale;
            if( code == Keys.Down )     viewCentre.Y -= invScale;
            
            transform.SetViewCentre( viewCentre );
        }
        
        #endregion
        
        #region Show about form
        
        void MbiHelpAboutClick( object sender, EventArgs e )
        {
            var about = new HelpAboutForm();
            about.fMain = this;
            about.Show();
        }
        
        #endregion
        
    }
}
