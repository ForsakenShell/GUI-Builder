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
        
        
        float RenderScale;
        
        
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
            
            cbRenderJumpTo.Items.Clear();
            gbRenderJumpTo.Enabled = false;
            
            // Set appropriate jump points
            if( selectedImport.VolumeParents.Count > 0 )
                foreach( var volumeParent in selectedImport.VolumeParents )
                   if( worldspace.EditorID == volumeParent.WorldspaceEDID )
                        cbRenderJumpTo.Items.Add( volumeParent.FormID );
            
            gbRenderJumpTo.Enabled = true;
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
        
        VolumeParent SelectedVolumeParent()
        {
            return (VolumeParent)cbRenderJumpTo.Tag;
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
            
            foreach( var volumeParent in selectedImport.VolumeParents )
            {
                UpdateStatusMessage( string.Format( "Welding verticies for {0}...", volumeParent.FormID ) );
                volumeParent.WeldVerticies( float.Parse( tbWeldThreshold.Text ) );
            }
            
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
                //if( volumeParent.EditorID == 0x01098B98 )
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
        
        #region Cell window rendering and controls
        
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
        
        void RenderCellWindow()
        {
            
            var worldspace = SelectedWorldspace();
            if( worldspace == null )
                return;
            
            var selectedImport = SelectedImportMod();
            var selectedVolume = SelectedVolumeParent();
            
            bool renderNonPlayable   = cbRenderOverRegion.Checked;
            bool renderLand          = cbRenderLandHeight.Checked;
            bool renderWater         = cbRenderWaterHeight.Checked;
            bool renderCellGrid      = cbRenderCellGrid.Checked;
            bool renderBuildVolumes  = ( selectedImport != null )&&( selectedImport.VolumeParents != null )&&( cbRenderBuildVolumes.Checked );
            bool renderBorders       = ( selectedImport != null )&&( selectedImport.VolumeParents != null )&&( cbRenderBorders.Checked );
            bool renderSelectedOnly  = ( selectedVolume != null )&&( cbRenderSelectedOnly.Checked );
            bool exportPNG           = cbExportPNG.Checked;
            
            if( !renderSelectedOnly ) selectedVolume = null;
            
            if( ( worldspace.LandHeight_Bitmap == null )||( worldspace.WaterHeight_Bitmap == null ) )
                worldspace.RenderHeightMap( this );
            
            Maths.Vector2i cellNW = worldspace.CellNW;
            Maths.Vector2i cellSE = worldspace.CellSE;
            RenderScale = bbConstant.MinZoom;
            
            if( renderSelectedOnly )
            {
                cellNW = selectedVolume.CellNW;
                cellSE = selectedVolume.CellSE;
                RenderScale = bbConstant.MaxZoom;
            }
            else if( renderNonPlayable )
            {
                var hmCSize = worldspace.HeightMapCellSize;
                cellNW = worldspace.HeightMapCellOffset;
                cellSE = new Maths.Vector2i(
                    cellNW.X + ( hmCSize.X - 1 ),
                    cellNW.Y - ( hmCSize.Y - 1 ) );
            }
            
            UpdateStatusMessage( "[Re]creating render transform..." );
            var rt = new RenderTransform( worldspace, selectedImport, selectedVolume, cellNW, cellSE, RenderScale );
            
            #if DEBUG
            
            rt.debugRenderBuildVolumes = cbRenderBuildVolumes.CheckState == CheckState.Indeterminate;
            rt.debugRenderBorders = cbRenderBorders.CheckState == CheckState.Indeterminate;
            
            #endif
            
            if( renderLand )
                rt.DrawLandMap();
            
            if( renderWater )
                rt.DrawWaterMap();
            
            if( renderCellGrid )
                rt.DrawCellGrid();
            
            if( renderBuildVolumes )
                rt.DrawBuildVolumes();
            
            if( renderBorders )
                rt.DrawParentBorders();
            
            if( exportPNG )
                rt.RenderToPNG();
            
            UpdateStatusMessage( "Rendering final bitmap..." );
            rt.RenderToPictureBox( pbRenderWindow );
            
            // Release references and garbage collect them
            UpdateStatusMessage( "Collecting garbage..." );
            rt.Dispose();
            rt = null;
            System.GC.Collect( System.GC.MaxGeneration );
            
            UpdateStatusMessage( "Refreshing..." );
            pbRenderWindow.Refresh();
            pnRenderWindow.Enabled = true;
            pnRenderWindow.Visible = true;
            pnRenderWindow.Tag = worldspace;
            UpdateStatusMessage( string.Format( "Height map: {0} x {1}", worldspace.HeightMap_Width, worldspace.HeightMap_Height ) );
        }
        
        #endregion
        
        void BtnCellWindowRedrawClick(object sender, EventArgs e)
        {
            SetEnableState( false );
            RenderCellWindow();
            SetEnableState( true );
        }
        
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
        
        void PbRenderWindowMouseMove( object sender, MouseEventArgs e )
        {
            var worldspace = SelectedWorldspace();
            var volumeParent = SelectedVolumeParent();
            var prefix = volumeParent == null ? "" : string.Format( "{0}, {1}-{2}; ", volumeParent.FormID, volumeParent.CellNW.ToString(), volumeParent.CellSE.ToString() );
            UpdateStatusMessage( string.Format( "{0}Cell: {1}", prefix, ( worldspace.HeightMapCellOffset + bbGlobal.HeightMapToCellGrid( e.X, e.Y ) ).ToString() ) );
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
