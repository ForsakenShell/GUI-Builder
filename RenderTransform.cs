/*
 * RenderTransform
 *
 * Handles all the specifics of rendering and scaling for the selected worldspace, import mod [and, parent volume]
 * respecting clipping and scaling.
 *
 * User: 1000101
 * Date: 19/12/2017
 * Time: 9:16 AM
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
    
    public class RenderTransform : IDisposable
    {
        
        public enum Heightmap
        {
            Land,
            Water
        }
        
        // Source data pointers
        public bbWorldspace worldspace;
        public bbImportMod importMod;
        public List<VolumeParent> renderVolumes;
        
        // Clipper inputs held for reference
        Maths.Vector2i cellNW;
        Maths.Vector2i cellSE;
        Maths.Vector2f viewCentre;
        Maths.Vector2f trueCentre;
        //Rectangle wmViewPort;
        //Rectangle wmClipper;
        //Rectangle hmClipper;
        float scale;
        float invScale;
        float minScale;
        const float maxScale = 1.0f;
        
        // Editor mode controls
        
        enum EditorSelectionMode
        {
            None = 0,
            Vertex,
            Edge
        }
        
        bool editorMode;
        bool oldFormKeyPreview; // We're setting KeyPreview to true, don't reset it to false erroneously when editor mode is toggled off 
        bool editorMouseOver;
        EditorSelectionMode editorSelectionMode;
        
        Form editorForm;
        ToolStripStatusLabel editorSelectionModeStatus;
        
        #if DEBUG
        
        // Debug render options
        public bool debugRenderBuildVolumes;
        public bool debugRenderBorders;
        
        #endif
        
        // Computed values from inputs
        Maths.Vector2i hmCentre;
        
        // Size of selected region in cells
        Maths.Vector2i cmSize;
        
        // Heightmap offset and size
        //Maths.Vector2i hmOffset;
        //Maths.Vector2i hmSize;
        
        // Worldmap offset and size
        //Maths.Vector2f wmOffset;
        //Maths.Vector2f wmSize;
        
        //Maths.Vector2f hmTransform;
        //Maths.Vector2f wmTransform;
        
        // Render target
        Bitmap bmpTarget;
        Graphics gfxTarget;
        GraphicsUnit guTarget;
        ImageAttributes iaTarget;
        Rectangle rectTarget;
        PictureBox pbTarget;
        
        // Last rendered layers
        bool _renderLand;
        bool _renderWater;
        bool _renderCellGrid;
        bool _renderBuildVolumes;
        bool _renderBorders;
        
        // High light parents and volumes
        public List<VolumeParent> hlParents = null;
        public List<BuildVolume> hlVolumes = null;
        
        #region Constructor and Dispose()
        
        public RenderTransform( PictureBox _pbTarget )
        {
            pbTarget = _pbTarget;
            editorMode = false;
            editorMouseOver = false;
            RenderTargetSizeChanged( false );
        }
        
        public void Dispose()
        {
            DisableEditorMode();
            worldspace = null;
            importMod = null;
            renderVolumes = null;
            cellNW = Maths.Vector2i.Zero;
            cellSE = Maths.Vector2i.Zero;
            scale = 0f;
            viewCentre = Maths.Vector2f.Zero;
            trueCentre = Maths.Vector2f.Zero;
            //wmViewPort = Rectangle.Empty;
            //wmClipper = Rectangle.Empty;
            //hmClipper = Rectangle.Empty;
            hmCentre = Maths.Vector2i.Zero;
            cmSize = Maths.Vector2i.Zero;
            //hmOffset = Maths.Vector2i.Zero;
            //hmSize = Maths.Vector2i.Zero;
            //wmOffset = Maths.Vector2f.Zero;
            //wmSize = Maths.Vector2f.Zero;
            //hmTransform = Maths.Vector2f.Zero;
            //wmTransform = Maths.Vector2f.Zero;
            gfxTarget.Dispose();
            bmpTarget.Dispose(); 
            iaTarget.Dispose();
            bmpTarget = null;
            gfxTarget = null;
            guTarget = (GraphicsUnit)0;
            iaTarget = null;
            rectTarget = Rectangle.Empty;
        }
        
        #endregion
        
        #region Render State Manipulation
        
        public void UpdateSceneMetricsAndRedraw()
        {
            RecomputeSceneClipper();
            ReRenderCurrentScene();
        }
        
        public void RenderTargetSizeChanged( bool updateScene = true )
        {
            if(
                ( pbTarget.Width < 1 )||
                ( pbTarget.Height < 1 )
            )   return;
            
            var rect = new Rectangle(
                0,
                0,
                pbTarget.Width,
                pbTarget.Height );
            
            if( rect == rectTarget )
                return;
            
            rectTarget = rect;
            trueCentre = new Maths.Vector2f( rectTarget.Centre() );
            
            bool doGC = false;
            if( gfxTarget != null )
            {
                gfxTarget.Dispose();
                gfxTarget = null;
                doGC = true;
            }
            if( bmpTarget != null )
            {
                bmpTarget.Dispose(); 
                bmpTarget = null;
                doGC = true;
            }
            if( iaTarget != null )
            {
                iaTarget.Dispose();
                iaTarget = null;
            }
            
            if( doGC )
                System.GC.Collect( System.GC.MaxGeneration );
            
            bmpTarget = new Bitmap( rectTarget.Width, rectTarget.Height );
            gfxTarget = Graphics.FromImage( bmpTarget );
            
            guTarget = GraphicsUnit.Pixel;
            iaTarget = new ImageAttributes();
            iaTarget.SetWrapMode( System.Drawing.Drawing2D.WrapMode.Tile );
            
            if( updateScene )
                UpdateSceneMetricsAndRedraw();
            
        }
        
        public void UpdateCellClipper( Maths.Vector2i _cellNW, Maths.Vector2i _cellSE, bool updateScene = true )
        {
            bool mustRebuildBuffers = ( bmpTarget == null )||( gfxTarget == null )||( iaTarget == null );
            
            cellNW = _cellNW;
            cellSE = _cellSE;
            hmCentre = worldspace.HeightMapOffset;
                
            // Compute
            cmSize = bbSpaceConversions.SizeOfCellRange( cellNW, cellSE );
            
            if( updateScene )
                UpdateSceneMetricsAndRedraw();
        }
        
        public float GetScale() { return scale; }
        public void SetScale( float value, bool updateScene = true )
        {
            scale = value;
            invScale = 1.0f / scale;
            if( updateScene )
                UpdateSceneMetricsAndRedraw();
        }
        public float GetInvScale() { return invScale; }
        public void SetInvScale( float value, bool updateScene = true )
        {
            scale = 1.0f / value;
            if( updateScene )
                UpdateSceneMetricsAndRedraw();
        }
        
        public Maths.Vector2f GetViewCentre() { return viewCentre; }
        public void SetViewCentre( Maths.Vector2f value, bool updateScene = true )
        {
            viewCentre = value;
            if( updateScene )
                UpdateSceneMetricsAndRedraw();
        }
        
        public Maths.Vector2i GetClipperCellSize() { return cmSize; }
        
        public void RecomputeSceneClipper()
        {
            minScale = CalculateScale( cmSize );
            scale = Math.Min( maxScale, Math.Max( minScale, scale ) );
            invScale = 1.0f / scale;
            
            /*
            // Turn cellNW, cellSE into the worldmap clipper
            var wmClipper = new Rectangle(
                (int)( cellNW.X * bbConstant.WorldMap_Resolution ),
                (int)( cellNW.Y * bbConstant.WorldMap_Resolution ),
                (int)( cmSize.X * bbConstant.WorldMap_Resolution ),
                (int)( cmSize.Y * bbConstant.WorldMap_Resolution )
            );
            
            // Render target size
            var tWidth = rectTarget.Width;
            var tHeight = rectTarget.Height;
            
            // Compute the visible area at the current zoom level
            var tvWidth   = tWidth   * invScale;
            var tvHeight  = tHeight  * invScale;
            var htvWidth  = tvWidth  * 0.5f;
            var htvHeight = tvHeight * 0.5f;
            
            // Make sure the viewCentre is inside the worldmap clipper
            viewCentre.X = Math.Min( wmClipper.Right  - htvWidth,
                           Math.Max( wmClipper.Left   + htvWidth,
                                     viewCentre.X ) );
            
            viewCentre.Y = Math.Min( wmClipper.Top    + htvHeight,
                           Math.Max( wmClipper.Bottom - htvHeight,
                                     viewCentre.Y ) );
            */
            
            // Compute the visible area of the worldmap
            /*
            wmOffset = new Maths.Vector2f(
                viewCentre.X - htvWidth,
                viewCentre.Y + htvHeight );
            wmSize = new Maths.Vector2f(
                tvWidth,
                tvHeight );
            */
            
            // Compute the visible area of the heightmap
            /*
            hmOffset = new Maths.Vector2i(
                hmCentre.X + (int)( wmOffset.X * bbConstant.WorldMap_To_Heightmap ),
                hmCentre.Y - (int)( wmOffset.Y * bbConstant.WorldMap_To_Heightmap ) );
            hmSize = new Maths.Vector2i(
                (int)( wmSize.X * bbConstant.WorldMap_To_Heightmap ),
                (int)( wmSize.Y * bbConstant.WorldMap_To_Heightmap ) );
            */
            
            /*
            hmTransform = new Maths.Vector2f(
                -cellNW.X * bbConstant.WorldMap_Resolution * scale,
                -cellSE.Y * bbConstant.WorldMap_Resolution * scale );
            
            wmTransform = new Maths.Vector2f(
                -wmOffset.X * scale,
                 wmOffset.Y * scale );
            */
            
        }
        
        #endregion
        
        #region Editor Mode
        
        #region Editor Mode State (Enable/Disable)
        
        public void DisableEditorMode()
        {
            if( !editorMode )
                return;
            
            mouseSelectionMode = false;
            editorSelectedVertices = null;
            editorMode = false;
            
            editorSelectionModeStatus.Text = EditorSelectionModeLabel;
            
            editorForm.KeyPreview = oldFormKeyPreview;
            editorForm.KeyPress -= this.editorMode_KeyPress;
            pbTarget.MouseDown -= this.editorMode_MouseDown;
            pbTarget.MouseUp -= this.editorMode_MouseUp;
            pbTarget.MouseMove -= this.editorMode_MouseMove;
            pbTarget.MouseEnter -= this.editorMode_MouseEnter;
            pbTarget.MouseLeave -= this.editorMode_MouseLeave;
            
            editorForm = null;
            editorSelectionModeStatus = null;
        }
        
        public void EnableEditorMode( Form _editorForm, ToolStripStatusLabel _editorSelectionModeStatus, TextBox _editorHotkeyDescriptions )
        {
            if( ( _editorForm == null )||( _editorSelectionModeStatus == null )||( _editorHotkeyDescriptions == null ) )
            {
                return;
            }
            if( editorMode )
            {
                DisableEditorMode();
            }
            
            editorForm = _editorForm;
            editorSelectionModeStatus = _editorSelectionModeStatus;
            
            oldFormKeyPreview = editorForm.KeyPreview;
            editorForm.KeyPreview = true;
            editorForm.KeyPress += this.editorMode_KeyPress;
            pbTarget.MouseDown += this.editorMode_MouseDown;
            pbTarget.MouseUp += this.editorMode_MouseUp;
            pbTarget.MouseMove += this.editorMode_MouseMove;
            pbTarget.MouseEnter += this.editorMode_MouseEnter;
            pbTarget.MouseLeave += this.editorMode_MouseLeave;
            
            editorMode = true;
            mouseSelectionMode = false;
            editorSelectedVertices = null;
            editorSelectionModeStatus.Text = EditorSelectionModeLabel;
            
            _editorHotkeyDescriptions.Clear();
            foreach( var hk in EditorHotkeys )
                _editorHotkeyDescriptions.AppendText( hk.FullDescription + "\n" );
        }
        
        #endregion
        
        #region Hotkey Dispatcher, Delegate and, Struct
        
        public delegate bool HotkeyDelegate( object sender, KeyPressEventArgs e );
        
        public struct EditorHotkey
        {
            char[] _validKeys;
            string _showKey;
            string _description;
            
            HotkeyDelegate _callback;
            
            public EditorHotkey( char[] validKeys, string showKey, string description, HotkeyDelegate callback )
            {
                this._validKeys = validKeys;
                this._showKey = showKey;
                this._description = description;
                this._callback = callback;
            }
            
            public string FullDescription
            {
                get
                {
                    return string.Format( "{0} - {1}", _showKey, _description );
                }
            }
            
            public bool ValidKeyPressed( char test )
            {
                foreach( var key in _validKeys )
                    if( test == key ) return true;
                return false;
            }
            
            public bool TryHandle( object sender, KeyPressEventArgs e )
            {
                return ValidKeyPressed( e.KeyChar ) && _callback( sender, e );
            }
        }
        
        public bool HotkeyDispatcher( object sender, KeyPressEventArgs e )
        {
            foreach( var hk in EditorHotkeys )
                if( hk.TryHandle( sender, e ) ) return true;
            return false;
        }
        
        #endregion
        
        #region Selection Mode
        
        string EditorSelectionModeLabel
        {
            get
            {
                if( editorMode )
                {
                    switch( editorSelectionMode )
                    {
                        case EditorSelectionMode.None :
                            return "None";
                        case EditorSelectionMode.Vertex :
                            return "Vertex";
                        case EditorSelectionMode.Edge :
                            return "Edge";
                    }
                }
                return "";
            }
        }
        
        void EditorSelectionModeToggle( EditorSelectionMode mode )
        {
            editorSelectionMode = editorSelectionMode != mode ? mode : EditorSelectionMode.None;
            if( editorSelectionModeStatus != null )
                editorSelectionModeStatus.Text = EditorSelectionModeLabel;
        }
        
        #endregion
        
        #region Hotkey Selection Modes
        
        bool mouseSelectionMode;
        Maths.Vector2f mouseSelectionPoint;
        List<WeldPoint> editorSelectedVertices;
        
        public List<WeldPoint> EditorSelectedVertices { get { return editorSelectedVertices; } }
        public bool MouseSelectionMode { get { return mouseSelectionMode; } }
        
        public void SelectVerticiesNear( Maths.Vector2f position, bool unweldAsNeeded = false )
        {
            var corners = WeldPoint.FindWeldableCorners( worldspace, renderVolumes, position, 64f, true, null );
            WeldPoint.FilterWeldPoints( corners, true, false, true, unweldAsNeeded, null );
            editorSelectedVertices = corners;
        }
        
        public WeldPoint ClosestAnchoredCornerNear( Maths.Vector2f position, bool includeAlreadySelectedCorners = false )
        {
            // Find all the corners near the position
            var corners = WeldPoint.FindWeldableCorners( worldspace, renderVolumes, position, 64f, true, null );
            WeldPoint.FilterWeldPoints( corners, false, true, true, false, !includeAlreadySelectedCorners ? editorSelectedVertices : null );
            
            // No corners found
            if( corners.NullOrEmpty() ) return null;
            
            // Now search through the remaining anchored corners and return the closest
            int closest = 0;
            float dist = ( position - corners[ closest ].Position ).Length;
            for( int i = 1; i < corners.Count; i++ )
            {
                float temp = ( position - corners[ i ].Position ).Length;
                if( temp < dist )
                {
                    dist = temp;
                    closest = i;
                }
            }
            
            return corners[ closest ];
        }
        
        void MoveSelectedVerticiesToMouse( MouseEventArgs e, bool anchorCorners )
        {
            if( editorSelectedVertices.NullOrEmpty() )
                return;
            
            // Update the mouse selection point
            mouseSelectionPoint = ScreenspaceToWorldspace( e.X, e.Y );
            
            // Find the cloest anchored vertex that isn't in the selected group
            var closestAnchored = ClosestAnchoredCornerNear( mouseSelectionPoint, false );
            
            // Found it, snap the selection point to the anchor point
            if( closestAnchored != null )
                mouseSelectionPoint = closestAnchored.Position;
            
            // Now move the selected corners to the mouse/closest unselected anchored corner
            WeldPoint.WeldCornersTo( mouseSelectionPoint, editorSelectedVertices, true, anchorCorners );
        }
        
        bool ToggleVertexSelection( object sender, KeyPressEventArgs e )
        {
            // Only allow toggle if not actively selecting anything
            if( mouseSelectionMode ) return false;
            
            EditorSelectionModeToggle( EditorSelectionMode.Vertex );
            return true;
        }
        
        bool ToggleEdgeSelection( object sender, KeyPressEventArgs e )
        {
            // Only allow toggle if not actively selecting anything
            if( mouseSelectionMode ) return false;
            
            EditorSelectionModeToggle( EditorSelectionMode.Edge );
            return true;
        }
        
        #endregion
        
        #region EditorHotkeys Property
        
        List<EditorHotkey> _editorHotkeys;
        public List<EditorHotkey> EditorHotkeys
        {
            get
            {
                if( _editorHotkeys == null )
                {
                    var hk = new List<EditorHotkey>();
                    hk.Add( new EditorHotkey( new char[] { 'V', 'v' }, "V", "Toggle vertex selection", ToggleVertexSelection ) );
                    hk.Add( new EditorHotkey( new char[] { 'E', 'e' }, "E", "Toggle edge selection", ToggleEdgeSelection ) );
                    _editorHotkeys = hk;
                }
                return _editorHotkeys;
            }
        }
        
        #endregion
        
        #region Editor Mode pbTarget and editorForm events
        
        void editorMode_KeyPress( object sender, KeyPressEventArgs e )
        {
            if( !editorMouseOver )
                return;
            e.Handled = HotkeyDispatcher( sender, e );
        }
        
        void editorMode_MouseDown( object sender, MouseEventArgs e )
        {
            if( editorSelectionMode == EditorSelectionMode.None ) return;
            
            mouseSelectionMode = true;
            
            if( editorSelectionMode == EditorSelectionMode.Vertex )
            {
                mouseSelectionPoint = ScreenspaceToWorldspace( e.X, e.Y );
                SelectVerticiesNear( mouseSelectionPoint, true );
            }
            
            ReRenderCurrentScene();
        }
        
        void editorMode_MouseUp( object sender, MouseEventArgs e )
        {
            if( !mouseSelectionMode ) return;
            
            if( editorSelectionMode == EditorSelectionMode.Vertex )
            {
                // Set and anchor the corner[s] when done moving them
                MoveSelectedVerticiesToMouse( e, true );
            }
            
            mouseSelectionMode = false;
            mouseSelectionPoint = Maths.Vector2f.Zero;
            editorSelectedVertices = null;
        }
        
        void editorMode_MouseMove( object sender, MouseEventArgs e )
        {
            if( editorSelectionMode == EditorSelectionMode.Vertex )
            {
                if( mouseSelectionMode )
                {
                    // Set but don't anchor the corner[s] while moving them
                    MoveSelectedVerticiesToMouse( e, false );
                }
                else
                {
                    // User isn't actively editing but we need to update the "selection circle"
                    mouseSelectionPoint = ScreenspaceToWorldspace( e.X, e.Y );
                }
            }
            
            ReRenderCurrentScene();
        }
        
        void editorMode_MouseEnter( object sender, EventArgs e )
        {
            editorMouseOver = true;
        }
        
        void editorMode_MouseLeave( object sender, EventArgs e )
        {
            editorMouseOver = false;
        }
        
        #endregion
        
        #endregion
        
        #region Public Accessors
        
        public bbWorldspace                     Worldspace              { get { return worldspace; } }
        public bbImportMod                      ImportMod               { get { return importMod; } }
        
        public Maths.Vector2i                   CellNW                  { get { return cellNW; } }
        public Maths.Vector2i                   CellSE                  { get { return cellSE; } }
        
        public List<VolumeParent>               HighlightParents        { get { return hlParents; } }
        public List<BuildVolume>                HighlightVolumes        { get { return hlVolumes; } }
        
        #endregion
        
        public float CalculateScale( Maths.Vector2i cells )
        {
            var scaleNS = (float)rectTarget.Height / ( (float)cells.Y * bbConstant.WorldMap_Resolution );
            var scaleEW = (float)rectTarget.Width / ( (float)cells.X * bbConstant.WorldMap_Resolution );
            //var scale = Maths.Lerp( minScale, maxScale, scaleNS > scaleEW ? scaleNS : scaleEW );
            var scale = scaleNS < scaleEW ? scaleNS : scaleEW;
            return scale;
        }
        
        public static float CalculateScale( Maths.Vector2i cells, Maths.Vector2i worldCells )
        {
            var scaleNS = (float)( (float)cells.Y / (float)worldCells.Y );
            var scaleEW = (float)( (float)cells.X / (float)worldCells.X );
            var scale = Maths.InverseLerp( bbConstant.MinZoom, bbConstant.MaxZoom, scaleNS > scaleEW ? scaleNS : scaleEW );
            return scale;
        }
        
        #region Screenspace (Window) transforms
        
        #region Screenspace <-> Worldspace
        
        public Maths.Vector2f ScreenspaceToWorldspace( float x, float y )
        {
            var r = new Maths.Vector2f(
                 x - trueCentre.X,
                -y + trueCentre.Y );
            r *= invScale;
            r += viewCentre;
            return r;
        }
        
        public Maths.Vector2f WorldspaceToScreenspace( float x, float y )
        {
            var r = new Maths.Vector2f( x, y ) - viewCentre;
            r *= scale;
            r.X +=       trueCentre.X;
            r.Y = -r.Y + trueCentre.Y;
            return r;
        }
        
        public Maths.Vector2f ScreenspaceToWorldspace( Maths.Vector2f v )
        {
            return ScreenspaceToWorldspace( v.X, v.Y );
        }
        
        public Maths.Vector2f WorldspaceToScreenspace( Maths.Vector2f v )
        {
            return WorldspaceToScreenspace( v.X, v.Y );
        }
        
        public Rectangle WorldspaceToHeightmapClipper( float x, float y )
        {
            var ss = new Maths.Vector2f( rectTarget.Width, rectTarget.Height ) * invScale;
            var hss = ss * 0.5f;
            var nw = bbSpaceConversions.WorldspaceToHeightmap( x - hss.X, y + hss.Y, hmCentre );
            var s = ss.WorldspaceToCellspace();
            return new Rectangle(
                nw.X, nw.Y,
                (int)s.X, (int)s.Y
            );
        }
        
        #endregion
        
        #region Screenspace <-> Cellspace
        
        public Maths.Vector2f ScreenspaceToCellspace( float x, float y )
        {
            return ScreenspaceToWorldspace( x, y ).WorldspaceToCellspace();
        }
        
        public Maths.Vector2f CellspaceToScreenspace( float x, float y )
        {
            return WorldspaceToScreenspace( bbSpaceConversions.CellspaceToWorldspace( x, y ) );
        }
        
        public Maths.Vector2f ScreenspaceToCellspace( Maths.Vector2f v )
        {
            return ScreenspaceToWorldspace( v ).WorldspaceToCellspace();
        }
        
        public Maths.Vector2f CellspaceToScreenspace( Maths.Vector2f v )
        {
            return WorldspaceToScreenspace( v.CellspaceToWorldspace() );
        }
        
        #endregion
        
        #region Screenspace <-> CellGrid
        
        public Maths.Vector2i ScreenspaceToCellGrid( float x, float y )
        {
            return ScreenspaceToWorldspace( x, y ).WorldspaceToCellGrid();
        }
        
        public Maths.Vector2f CellGridToScreenspace( int x, int y )
        {
            return WorldspaceToScreenspace( bbSpaceConversions.CellGridToWorldspace( x, y ) );
        }
        
        public Maths.Vector2i ScreenspaceToCellGrid( Maths.Vector2f v )
        {
            return ScreenspaceToWorldspace( v ).WorldspaceToCellGrid();
        }
        
        public Maths.Vector2f CellGridToScreenspace( Maths.Vector2i v )
        {
            return WorldspaceToScreenspace( v.CellGridToWorldspace() );
        }
        
        #endregion
        
        #endregion
        
        #region Render Scene
        
        public void ReRenderCurrentScene()
        {
            RenderScene( _renderLand, _renderWater, _renderCellGrid, _renderBuildVolumes, _renderBorders, true, false );
        }
        
        public void RenderScene( bool renderLand, bool renderWater, bool renderCellGrid, bool renderBuildVolumes, bool renderBorders, bool fastRender, bool cacheRenderLayers )
        {
            if( fastRender )
            {
                gfxTarget.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                gfxTarget.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                gfxTarget.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                gfxTarget.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            }
            else
            {
                gfxTarget.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfxTarget.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                gfxTarget.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gfxTarget.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            }
            
            if( renderLand )
                DrawLandMap();
            
            if( renderWater )
                DrawWaterMap();
            
            if( renderCellGrid )
                DrawCellGrid();
            
            if( renderBuildVolumes )
                DrawBuildVolumes();
            
            if( renderBorders )
                DrawParentBorders();
            
            if( editorMode )
            {
                switch( editorSelectionMode )
                {
                    case EditorSelectionMode.None :
                        break;
                        
                    case EditorSelectionMode.Vertex :
                        // Draw the "selection circle"
                        var pen = new Pen( Color.White );
                        DrawCircleWorldTransform( pen, mouseSelectionPoint.X, mouseSelectionPoint.Y, 1024f );
                            
                        break;
                        
                    case EditorSelectionMode.Edge :
                        break;
                }
            }
            
            if( cacheRenderLayers )
            {
                _renderLand = renderLand;
                _renderWater = renderWater;
                _renderCellGrid = renderCellGrid;
                _renderBuildVolumes = renderBuildVolumes;
                _renderBorders = renderBorders;
            }
            
            pbTarget.Size = new Size( rectTarget.Width, rectTarget.Height );
            pbTarget.Image = bmpTarget;
        }
        
        public void RenderToPNG()
        {
            // Export the [whole] map to a PNG
            if(
                ( !renderVolumes.NullOrEmpty() )&&
                ( renderVolumes.Count > 1 )
               ) return;
            var filename = renderVolumes.NullOrEmpty() ? worldspace.FormID : renderVolumes[ 0 ].FormID + ".png";
            bmpTarget.Save( filename, System.Drawing.Imaging.ImageFormat.Png );
        }
        
        #endregion
        
        #region Drawing Primitives (cellspace (heightmap) to transform target)
        
        public void DrawLineCellTransform( Pen pen, float x0, float y0, float x1, float y1 )
        {
            //var r0 = hmTransform + new Maths.Vector2f( x0 * bbConstant.HeightMap_To_Worldmap * scale, y0 * bbConstant.HeightMap_To_Worldmap * scale );
            //var r1 = hmTransform + new Maths.Vector2f( x1 * bbConstant.HeightMap_To_Worldmap * scale, y1 * bbConstant.HeightMap_To_Worldmap * scale );
            var tp0 = CellspaceToScreenspace( x0, y0 );
            var tp1 = CellspaceToScreenspace( x1, y1 );
            gfxTarget.DrawLine( pen, tp0.X, tp0.Y, tp1.X, tp1.Y );
        }
        
        public void DrawRectCellTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineCellTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineCellTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineCellTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineCellTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        #endregion
        
        #region Drawing Primitives (worldspace to transform target)
        
        public void DrawTextWorldTransform( string text, float x, float y, Pen pen, float size = 12f, Font font = null, Brush brush = null )
        {
            if( font == null )
                font = new Font( FontFamily.GenericSerif, size, FontStyle.Regular, GraphicsUnit.Pixel );
            if( brush == null )
                brush = new SolidBrush( pen.Color );
            //var rx = wmTransform.X + x * scale;
            //var ry = wmTransform.Y - y * scale;
            //gfxTarget.DrawString( text, font, brush, rx, ry );
            var tp = WorldspaceToScreenspace( x, y );
            gfxTarget.DrawString( text, font, brush, tp.X, tp.Y );
        }
        
        public void DrawCircleWorldTransform( Pen pen, float x, float y, float r )
        {
            var sr = r * scale;
            var hr = sr / 2f;
            //var sX = wmTransform.X + x * scale;
            //var sY = wmTransform.Y - y * scale;
            var tp = WorldspaceToScreenspace( x, y );
            var tr = new Rectangle(
                (int)( tp.X - hr ),
                (int)( tp.Y - hr ),
                (int)( sr ),
                (int)( sr ) );
            gfxTarget.DrawEllipse( pen, tr );
        }
        
        public void DrawLineWorldTransform( Pen pen, float x0, float y0, float x1, float y1 )
        {
            //var rx0 = wmTransform.X + x0 * scale;
            //var ry0 = wmTransform.Y - y0 * scale;
            //var rx1 = wmTransform.X + x1 * scale;
            //var ry1 = wmTransform.Y - y1 * scale;
            //gfxTarget.DrawLine( pen, rx0, ry0, rx1, ry1 );
            var tp0 = WorldspaceToScreenspace( x0, y0 );
            var tp1 = WorldspaceToScreenspace( x1, y1 );
            gfxTarget.DrawLine( pen, tp0.X, tp0.Y, tp1.X, tp1.Y );
        }
        
        public void DrawLineWorldTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p1.Y );
        }
        
        public void DrawLineWorldTransform( Pen pen, Maths.Vector3f p0, Maths.Vector3f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p1.Y );
        }
        
        public void DrawRectWorldTransform( Pen pen, Maths.Vector2f p0, Maths.Vector2f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineWorldTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineWorldTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineWorldTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        public void DrawRectWorldTransform( Pen pen, Maths.Vector3f p0, Maths.Vector3f p1 )
        {
            DrawLineWorldTransform( pen, p0.X, p0.Y, p1.X, p0.Y );
            DrawLineWorldTransform( pen, p1.X, p0.Y, p1.X, p1.Y );
            DrawLineWorldTransform( pen, p1.X, p1.Y, p0.X, p1.Y );
            DrawLineWorldTransform( pen, p0.X, p1.Y, p0.X, p0.Y );
        }
        
        public void DrawPolyWorldTransform( Pen pen, Maths.Vector2f[] p )
        {
            var last = p.Length - 1;
            for( int index = 0; index < p.Length - 1; index++ )
                DrawLineWorldTransform( pen, p[ index ], p[ index + 1 ] );
            DrawLineWorldTransform( pen, p[ last ], p[ 0 ] );
        }
        
        #endregion
        
        #region Heightmap Drawing
        
        public void DrawLandMap()
        {
            if( worldspace.LandHeight_Bitmap == null ) return;
            //gfxTarget.DrawImage( worldspace.LandHeight_Bitmap, rectTarget, hmOffset.X, hmOffset.Y, hmSize.X, hmSize.Y, guTarget, iaTarget );
            var hmClipper = WorldspaceToHeightmapClipper( viewCentre.X, viewCentre.Y );
            gfxTarget.DrawImage( worldspace.LandHeight_Bitmap, rectTarget, hmClipper.X, hmClipper.Y, hmClipper.Width, hmClipper.Height, guTarget, iaTarget );
        }
        
        public void DrawWaterMap()
        {
            if( worldspace.WaterHeight_Bitmap == null ) return;
            //gfxTarget.DrawImage( worldspace.WaterHeight_Bitmap, rectTarget, hmOffset.X, hmOffset.Y, hmSize.X, hmSize.Y, guTarget, iaTarget );
            var hmClipper = WorldspaceToHeightmapClipper( viewCentre.X, viewCentre.Y );
            gfxTarget.DrawImage( worldspace.WaterHeight_Bitmap, rectTarget, hmClipper.X, hmClipper.Y, hmClipper.Width, hmClipper.Height, guTarget, iaTarget );
        }
        
        #endregion
        
        #region Build Volumes Drawing
        
        void DrawBuildVolume( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( worldspace.EditorID != volumeParent.WorldspaceEDID )
                return;
            
            #if DEBUG
            if(
                ( !debugRenderBuildVolumes )||
                ( volumeParent.BorderSegments.NullOrEmpty() )
            )
            #endif
            {
                int rb = 223;
                var penHigh = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                rb -= 48;
                var penMid = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                rb -= 48;
                var penLow = new Pen( Color.FromArgb( 255, rb, 0, rb ) );
                
                foreach( var volume in volumeParent.BuildVolumes )
                {
                    var usePen = editorMode ? penMid : penLow;
                    if( !editorMode )
                    {
                        if( hlParents.NullOrEmpty() )
                        {
                            usePen = penHigh;
                        }
                        else if(
                            ( !hlVolumes.NullOrEmpty() )&&
                            ( hlVolumes.Contains( volume ) )
                        )
                        {
                            usePen = penHigh;
                        }
                        else if( hlParents.Contains( volumeParent ) )
                        {
                            usePen = penMid;
                        }
                    }
                
                    DrawPolyWorldTransform( usePen, volume.Corners );
                }
            }
            #if DEBUG
            else
            {
                int r = 255;
                int g = 0;
                int b = 127;
                
                for( var i = 0; i < volumeParent.BorderSegments.Count; i++ )
                {
                    var segment = volumeParent.BorderSegments[ i ];
                    
                    var pen = new Pen( Color.FromArgb( 255, r, g, b ) );
                    
                    r -= 32;
                    g += 32;
                    b += 64;
                    if( r <   0 ) r += 255;
                    if( g > 255 ) g -= 255;
                    if( b > 255 ) b -= 255;
                    
                    var niP = ( segment.P0 + segment.P1 ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, pen, 10f );
                    DrawLineWorldTransform( pen, segment.P0, segment.P1 );
                }
            }
            #endif
        }
        
        public void DrawBuildVolumes()
        {
            if( renderVolumes.NullOrEmpty() )
            {
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawBuildVolume( volumeParent );
            }
            else
            {
                if( hlParents.NullOrEmpty() )
                    foreach( var volumeParent in renderVolumes )
                        DrawBuildVolume( volumeParent );
                else
                {
                    foreach( var volumeParent in renderVolumes )
                        if( !hlParents.Contains( volumeParent ) )
                            DrawBuildVolume( volumeParent );
                    
                    foreach( var volumeParent in hlParents )
                        DrawBuildVolume( volumeParent );
                }
            }
        }
        
        #endregion
        
        #region Parent Border Drawing
        
        void DrawParentBorder( VolumeParent volumeParent )
        {
            // Only show build volumes for the appropriate worldspace
            if( ( worldspace.EditorID != volumeParent.WorldspaceEDID )||( volumeParent.BorderNodes == null ) )
                return;
            
            int g = 255;
            if(
                ( !editorMode )&&
                ( !hlParents.NullOrEmpty() )&&
                ( !hlParents.Contains( volumeParent ) )
            )   g >>= 1;
            
            var pen = new Pen( Color.FromArgb( 255, 0, g, 0 ) );
            
            for( var i = 0; i < volumeParent.BorderNodes.Count; i++ )
            {
                var node = volumeParent.BorderNodes[ i ];
                
                /*
                if(
                    ( !hlVolumes.NullOrEmpty() )&&
                    ( hlVolumes.Contains( node.Volume ) )
                )   g >>= 1;
                */
                
                DrawLineWorldTransform( pen, node.A, node.B );
                
                #if DEBUG
                if( debugRenderBorders )
                {
                    var niP = ( node.A + node.B ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, pen );
                }
                #endif
            }
            
        }
        
        public void DrawParentBorders()
        {
            if( renderVolumes.NullOrEmpty() )
            {
                foreach( var volumeParent in importMod.VolumeParents )
                    DrawParentBorder( volumeParent );
            }
            else
            {
                if( hlParents.NullOrEmpty() )
                    foreach( var volumeParent in renderVolumes )
                        DrawParentBorder( volumeParent );
                else
                {
                    foreach( var volumeParent in renderVolumes )
                        if( !hlParents.Contains( volumeParent ) )
                            DrawParentBorder( volumeParent );
                    
                    foreach( var volumeParent in hlParents )
                        DrawParentBorder( volumeParent );
                }
            }
        }
        
        #endregion
        
        #region Cell Grid Drawing
        
        public void DrawCellGrid()
        {
            var pen0 = new Pen( Color.FromArgb( 127, 255, 255, 0 ) );
            var pen = new Pen( Color.FromArgb( 127, 91, 91, 0 ) );
            
            for( int y = cellSE.Y; y <= cellNW.Y; y++ )
            {
                if( ( y >= worldspace.CellSE.Y )&&( y <= worldspace.CellNW.Y ) )
                {
                    for( int x = cellNW.X; x <= cellSE.X; x++ )
                    {
                        if( ( x >= worldspace.CellNW.X )&&( x <= worldspace.CellSE.X ) )
                        {
                            var p = ( x == 0 )||( y == 0 ) ? pen0 : pen;
                            var p0 = new Maths.Vector2f(   x       * bbConstant.WorldMap_Resolution    ,   y       * bbConstant.WorldMap_Resolution     );
                            var p1 = new Maths.Vector2f( ( x + 1 ) * bbConstant.WorldMap_Resolution - 1, ( y - 1 ) * bbConstant.WorldMap_Resolution + 1 );
                            DrawRectWorldTransform( p, p0, p1 );
                        }
                    }
                }
            }
        }
        
        #endregion
        
    }
}
