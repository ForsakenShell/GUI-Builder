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

using SDL2ThinLayer;
using SDL2;

namespace Border_Builder
{
    
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class bbMain : Form
    {
        
        #region Static references to the main form (god objects suck, but fuck it)
        
        private static bbMain _bbMain;
        
        public static bbMain Self
        {
            get
            {
                return _bbMain;
            }
        }
        
        #endregion
        
        // Minimum sizes of the main window and render window/panel
        readonly Size SIZE_ZERO = new Size( 0, 0 );
        readonly Size MIN_RENDER_SIZE = new Size( 640, 480 );
        readonly Size MIN_WINDOW_SIZE;
        readonly Size MAIN_FORM_BASE_SIZE;
        
        // Main render transform
        RenderTransform transform;
        
        // Editor
        VolumeEditor editor;
        
        #region Main Start/Stop
        
        public bbMain()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            _bbMain = this;
            
            MIN_WINDOW_SIZE = pnMainForm.Size;
            MAIN_FORM_BASE_SIZE = this.Size - MIN_WINDOW_SIZE;
        }
        
        void MainShutdown()
        {
            DestroyTransform();
        }
        
        void BbMainFormClosing( object sender, FormClosingEventArgs e )
        {
            MainShutdown();
        }
        
        void MenuExitClick( object sender, EventArgs e )
        {
            MainShutdown();
            Application.Exit();
        }
        
        void FMainLoad( object sender, EventArgs e )
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
        
        #region Main Form and Renderer sizes
        
        Size _lastTransformWindowSize = new Size( 0, 0 );
        Size _lastTransformPanelSize = new Size( 0, 0 );
        
        Size TransformLastWindowSize
        {
            get
            {
                return ( _lastTransformWindowSize.Width == 0 )||( _lastTransformWindowSize.Height == 0 )
                    ? MIN_RENDER_SIZE
                    : _lastTransformWindowSize;
            }
            set
            {
                _lastTransformWindowSize = value;
            }
        }
        
        Size TransformLastPanelSize
        {
            get
            {
                return ( _lastTransformPanelSize.Width == 0 )||( _lastTransformPanelSize.Height == 0 )
                    ? MIN_RENDER_SIZE
                    : _lastTransformPanelSize;
            }
            set
            {
                _lastTransformPanelSize = value;
            }
        }
        
        Size MainFormSizeForPanel( Size panelSize )
        {
            var bpm = MAIN_FORM_BASE_SIZE + MIN_WINDOW_SIZE;
            var bpp = MAIN_FORM_BASE_SIZE + panelSize;
            bpp.Width += MIN_WINDOW_SIZE.Width;
            return new Size(
                        (int)Math.Max( bpm.Width , bpp.Width ),
                        (int)Math.Max( bpm.Height, bpp.Height )
                );
        }
        
        Size TransformMinWindowSize
        {
            get
            {
                return MainFormSizeForPanel( MIN_RENDER_SIZE );
            }
        }
        
        Size TransformMaxWindowSize
        {
            get
            {
                return SIZE_ZERO;
            }
        }
        
        Size CurrentMinWindowSize
        {
            get
            {
                if(
                    ( cbWindowedRenderer.Checked )&&
                    ( transform != null )
                )
                {
                    return TransformMinWindowSize;
                }
                return MIN_WINDOW_SIZE;
            }
        }
        
        Size CurrentMaxWindowSize
        {
            get
            {
                if(
                    ( cbWindowedRenderer.Checked )&&
                    ( transform != null )
                )
                {
                    return TransformMaxWindowSize;
                }
                return MIN_WINDOW_SIZE;
            }
        }
        
        #endregion
        
        #region Global Form and Status bar update
        
        /// <summary>
        /// Long running functions should disable the main form so the user can't spam inputs.  Don't forget to enable the form again after the long-running function is complete so the user can continue to use the program.
        /// </summary>
        /// <param name="enabled">true to enable the form and it's controls, false to disable the form and it's controls.</param>
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            pnMainForm.Enabled = enabled;
        }
        
        public void UpdateStatusMessage( string message )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { UpdateStatusMessage( message ); }, null );
                return;
            }
            sbiCaption.Text = message;
        }
        
        public void UpdateStatusProgress( int value )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { UpdateStatusProgress( value ); }, null );
                return;
            }
            sbiProgress.Value = value;
        }
        
        public void SetStatusProgressMinimum( int min )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetStatusProgressMinimum( min ); }, null );
                return;
            }
            sbiProgress.Minimum = min;
        }
        
        public void SetStatusProgressMaximum( int max )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetStatusProgressMaximum( max ); }, null );
                return;
            }
            sbiProgress.Maximum = max;
        }
        
        public void UpdateMouseCellGrid( string message )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { UpdateMouseCellGrid( message ); }, null );
                return;
            }
            sbiMouseToCellGrid.Text = message;
        }
        
        public void UpdateMouseWorldPos( string message )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { UpdateMouseWorldPos( message ); }, null );
                return;
            }
            sbiMouseToWorldspace.Text = message;
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
            this.MinimumSize = MainFormSizeForPanel( SIZE_ZERO );
            this.MaximumSize = MainFormSizeForPanel( SIZE_ZERO );
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
            if( worldspace == null ) return;
            if( !transform.ReadyForUse() ) return;
            
            bool loadOk = 
                ( worldspace.LoadLandHeightMap( transform ) )&&
                ( worldspace.LoadWaterHeightMap( transform ) );
            
            if( loadOk )
            {
                if( renderBitmaps )
                    worldspace.CreateHeightmapTextures( transform );
                ResetRenderWindowControls();
            }
        }
        
        void CbWorldspaceSelectedIndexChanged( object sender, EventArgs e )
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
            
            ResetRenderWindowControls( true );
            
            TryUpdateRenderWindow();
        }
        
        #endregion
        
        #region Import mod form controls
        
        void CbImportModSelectedIndexChanged( object sender, EventArgs e )
        {
            ResetImportModControls();
            var index = cbImportMod.SelectedIndex;
            var selectedImport = bbGlobal.ImportMods[ index ];
            btnLoadImportModBuildVolumes.Enabled = ( !string.IsNullOrEmpty( selectedImport.BuildVolumeRef_File ) );
            btnBuildImportModVolumeBorders.Enabled = ( selectedImport.VolumeParents != null );
            btnWeldImportVolumeVerts.Enabled = ( selectedImport.VolumeParents != null );
        }
        
        void BtnLoadImportModBuildVolumesClick( object sender, EventArgs e )
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
            if( selectedImport == null ) return;
            if( !transform.ReadyForUse() ) return;
            
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
                        worldspace.LoadLandHeightMap( transform );
                        
                    if( worldspace.WaterHeightMap == null )
                        worldspace.LoadWaterHeightMap( transform );
                    
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
        
        #region Editor Enable/Disable
        
        void CbEditModeEnableCheckedChanged( object sender, EventArgs e )
        {
            if( transform == null )
            {
                cbEditModeEnable.Checked = false;
                return;
            }
            if( cbEditModeEnable.Checked )
            {
                if( editor == null )
                    editor = new VolumeEditor( transform, sbiEditorSelectionMode, tbEMHotKeys );
                editor.EnableEditorMode();
            }
            else if( editor != null )
            {
                editor.Dispose();
                editor = null;
            }
        }
        
        #endregion
        
        #region Rendering and controls
        
        void GetRenderOptions( out bool renderNonPlayable, out bool renderLand, out bool renderWater, out bool renderCellGrid, out bool renderBuildVolumes, out bool renderBorders, out bool renderSelectedOnly, out bool exportPNG )
        {
            var selectedImport  = SelectedImportMod();
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
        
        void BtnCellWindowRedrawClick( object sender, EventArgs e )
        {
            if( transform == null )
            {
                SetEnableState( false );
                
                if( !CreateTransform() )
                {
                    SetEnableState( true );
                    return;
                }
                UpdateRenderWindow();
                
                SetEnableState( true );
            }
            else
                DestroyTransform();
        }
        
        void TryUpdateRenderWindow()
        {
            if( transform != null )
            {
                SetEnableState( false );
                UpdateRenderWindow();
                SetEnableState( true );
            }
        }
        
        // This is called syncronously in the renderer thread
        void ResetForNewTransform( SDLRenderer renderer )
        {
            Console.WriteLine( "ResetForNewTransform()" );
            
            DestroyWorldspaceTextures( false );
        }
        
        void DestroyWorldspaceTextures( bool destroySurfaces )
        {
            foreach( var worldspace in bbGlobal.Worldspaces )
            {
                worldspace.DestroyTextures();
                if( destroySurfaces)
                    worldspace.DestroySurfaces();
            }
        }
        
        void DestroyTransform()
        {
            SetEnableState( false );
            
            DestroyWorldspaceTextures( true );
            
            if( editor != null )
                editor.Dispose();
            editor = null;
            
            if( transform != null )
            {
                if( cbWindowedRenderer.Checked )
                    TransformLastWindowSize = transform.Renderer.WindowSize;
                else
                    TransformLastPanelSize = pnRenderWindow.Size;
                transform.Dispose();
            }
            transform = null;
            
            btnCellWindowRedraw.Text = "Show Map";
            cbWindowedRenderer.Enabled = true;
            cbEditModeEnable.Enabled = false;
            this.MinimumSize = MainFormSizeForPanel( SIZE_ZERO );
            this.MaximumSize = MainFormSizeForPanel( SIZE_ZERO );
            
            SetEnableState( true );
        }
        
        bool CreateTransform()
        {
            // Dispose of the old editor (this should never happen though...)
            cbEditModeEnable.Enabled = false;
            if( editor != null )
            {
                editor.Dispose();
                editor = null;
            }
            if( transform != null )
            {
                if( transform.Renderer != null )
                {
                    transform.Renderer.Invoke( ResetForNewTransform );
                }
                transform.Dispose();
                transform = null;
            }
            
            UpdateStatusMessage( "Creating render transform..." );
            if( cbWindowedRenderer.Checked )
            {
                this.MinimumSize = MainFormSizeForPanel( SIZE_ZERO );
                this.MaximumSize = MainFormSizeForPanel( SIZE_ZERO );
                transform = new RenderTransform( this, TransformLastWindowSize, RenderWindow_Closed );
            }
            else
            {
                this.MinimumSize = TransformMinWindowSize;
                this.MaximumSize = TransformMaxWindowSize;
                this.Size = MainFormSizeForPanel( TransformLastPanelSize );
                transform = new RenderTransform( this, pnRenderWindow );
            }
            if( transform == null ) return false;
            
            // Set renderer event handlers
            transform.Renderer.MouseButtonUp += RenderWindow_MouseUp;
            transform.Renderer.MouseMove += RenderWindow_MouseMove;
            transform.Renderer.MouseWheel += RenderWindow_MouseWheel;
            transform.Renderer.KeyDown += RenderWindow_KeyDown;
            
            btnCellWindowRedraw.Text = "Hide Map";
            cbWindowedRenderer.Enabled = false;
            cbEditModeEnable.Enabled = true;
            return true;
        }
        
        bool UpdateRenderWindow()
        {
            var selectedWorldspace = SelectedWorldspace();
            if( selectedWorldspace == null )
                return false;
            
            bool renderNonPlayable, renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, renderSelectedOnly, exportPNG;
            GetRenderOptions( out renderNonPlayable, out renderLand, out renderWater, out renderCellGrid, out renderBuildVolumes, out renderBorders, out renderSelectedOnly, out exportPNG );
            
            var selectedImport = SelectedImportMod();
            var selectedVolumes = renderSelectedOnly ? SelectedVolumeParents() : null;
            
            // Get cell range from [whole] map/selected volumes
            Maths.Vector2i cellNW;
            Maths.Vector2i cellSE;
            
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
            
            UpdateStatusMessage( "Updating render transform..." );
            
            // Update data references
            transform.Worldspace = selectedWorldspace;
            transform.ImportMod = selectedImport;
            transform.RenderVolumes = selectedVolumes;
            
            transform.UpdateScene( renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, true );
            
            #if DEBUG
            
            transform.debugRenderBuildVolumes = cbRenderBuildVolumes.CheckState == CheckState.Indeterminate;
            transform.debugRenderBorders = cbRenderBorders.CheckState == CheckState.Indeterminate;
            
            #endif
            
            // Update physical transform (don't recompute until all the initial conditions are set)
            transform.UpdateCellClipper( cellNW, cellSE, false );
            transform.SetScale( transform.CalculateScale( transform.GetClipperCellSize() ), false );
            transform.SetViewCentre( transform.WorldspaceClipperCentre(), false );
            // Recompute!
            transform.UpdateSceneMetrics();
            
            pnRenderWindow.Visible = true;
            
            // Re-enable editor mode
            if(
                ( editor == null )&&
                ( cbEditModeEnable.Checked )
            ) {
                editor = new VolumeEditor( transform, sbiEditorSelectionMode, tbEMHotKeys );
                editor.EnableEditorMode();
            }
            
            return true;
        }
       
        #endregion
        
        #region Main form controls changed event
        
        void RenderStateControlChanged( object sender, EventArgs e )
        {
            TryUpdateRenderWindow();
        }
        
        void ToggleSelectedParents( List<VolumeParent> parents )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { ToggleSelectedParents( parents ); }, null );
                return;
            }
            foreach( var parent in parents )
            {
                if( lbVolumeParents.SelectedItems.Contains( parent.FormID ) )
                    lbVolumeParents.SelectedItems.Remove( parent.FormID );
                else
                    lbVolumeParents.SelectedItems.Add( parent.FormID );
            }
        }
        
        #endregion
        
        #region Target Panel events
        
        bool _setSizeScheduled = false;
        
        // User resized the main form, trigger a delayed size update of the SDL_Window
        
        void PnRenderWindowResize( object sender, EventArgs e )
        {
            if(
                ( transform == null )||
                ( !transform.Renderer.Anchored )
            ) return;
            if( !_setSizeScheduled )
            {
                _setSizeScheduled = true;
                var timer = new System.Timers.Timer();
                timer.Interval = 2500; // 2.5s
                timer.AutoReset = false;
                timer.Elapsed += ReloadTimerTimeout;
                timer.Start();
            }
        }
        
        void ReloadTimerTimeout( object sender, EventArgs e )
        {
            _setSizeScheduled = false;
            if(
                ( transform == null )||
                ( !transform.Renderer.Anchored )
            ) return;
            transform.InvokeSetSDLWindowSize();
        }
        
        #endregion
        
        #region Render event handlers (called async in renderer thread)
        
        void RenderWindow_Closed( SDLRenderer renderer )
        {
            Console.WriteLine( "MainForm.RenderWindow_Closed()" );
            DestroyTransform();
        }
        
        void RenderWindow_MouseUp( SDLRenderer renderer, SDL.SDL_Event e )
        {
            // Editor supercedes us
            if(
                ( editor != null )&&
                ( editor.MouseSelectionMode )
            ) return;
            
            var mouseWorldPos = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
            
            List<VolumeParent> parents = null;
            List<BuildVolume> volumes = null;
            if( !transform.ImportMod.TryGetVolumesFromPos( mouseWorldPos, out parents, out volumes, transform.RenderVolumes ) )
                return;
            
            ToggleSelectedParents( parents );
        }
        
        void RenderWindow_MouseMove( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var mouseWorldPos = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
            
            List<VolumeParent> parents = null;
            List<BuildVolume> volumes = null;
            string status = "";
            if(
                ( transform.ImportMod != null )&&
                ( transform.ImportMod.TryGetVolumesFromPos( mouseWorldPos, out parents, out volumes, transform.RenderVolumes ) )
            ){
                status = parents[ 0 ].FormID;
                for( int i = 1; i < parents.Count; i++ )
                    status = status + ", " + parents[ i ].FormID;
            }
            
            UpdateMouseCellGrid( mouseWorldPos.WorldspaceToCellGrid().ToString() );
            UpdateMouseWorldPos( mouseWorldPos.ToString() );
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
            
            transform.HighlightParents = parents;
            transform.HighlightVolumes = volumes;
        }
        
        void RenderWindow_MouseWheel( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var scale = transform.GetScale();
            scale += (float)e.wheel.y * 0.0125f;
            transform.SetScale( scale );
        }
        
        void RenderWindow_KeyDown( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var kms = SDL.SDL_GetModState();
            var viewCentre = transform.GetViewCentre();
            var invScale = transform.GetInvScale() * ( ( kms & SDL.SDL_Keymod.KMOD_SHIFT ) != 0 ? 10f : 1f );
            
            var code = e.key.keysym.sym;
            if( code == SDL.SDL_Keycode.SDLK_LEFT )     viewCentre.X -= invScale;
            if( code == SDL.SDL_Keycode.SDLK_RIGHT )    viewCentre.X += invScale;
            if( code == SDL.SDL_Keycode.SDLK_UP )       viewCentre.Y += invScale;
            if( code == SDL.SDL_Keycode.SDLK_DOWN )     viewCentre.Y -= invScale;
            
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
