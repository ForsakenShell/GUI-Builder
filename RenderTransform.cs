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
        bbWorldspace worldspace;
        bbImportMod importMod;
        List<VolumeParent> renderVolumes;
        
        // Clipper inputs held for reference
        Maths.Vector2i cellNW;
        Maths.Vector2i cellSE;
        
        // View port
        Maths.Vector2f viewCentre;
        Maths.Vector2f trueCentre;
        float scale;
        float invScale;
        float minScale;
        const float maxScale = 1.0f;
        
        #if DEBUG
        
        // Debug render options
        public bool debugRenderBuildVolumes;
        public bool debugRenderBorders;
        
        #endif
        
        // Computed values from inputs
        Maths.Vector2i hmCentre;
        
        // Size of selected region in cells
        Maths.Vector2i cmSize;
        
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
        List<VolumeParent> hlParents = null;
        List<BuildVolume> hlVolumes = null;
        
        VolumeEditor attachedEditor;
        
        #region Constructor and Dispose()
        
        public RenderTransform( PictureBox _pbTarget )
        {
            pbTarget = _pbTarget;
            attachedEditor = null;
            RenderTargetSizeChanged( false );
        }
        
        #region Semi-Public API:  Destructor & IDispose
        
        // Protect against "double-free" errors caused by combinations of explicit disposal[s] and GC disposal
        bool _disposed = false;
        
        ~RenderTransform()
        {
            Dispose( false );
        }
        
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
        
        protected virtual void Dispose( bool disposing )
        {
            if( _disposed ) return;
            
            // Dispose of external references
            attachedEditor = null;
            pbTarget = null;
            worldspace = null;
            importMod = null;
            renderVolumes = null;
            hlParents = null;
            hlVolumes = null;
            
            // Reset all type fields (not strictly necessary)
            cellNW = Maths.Vector2i.Zero;
            cellSE = Maths.Vector2i.Zero;
            scale = 0f;
            invScale = 0f;
            minScale = 0f;
            viewCentre = Maths.Vector2f.Zero;
            trueCentre = Maths.Vector2f.Zero;
            hmCentre = Maths.Vector2i.Zero;
            cmSize = Maths.Vector2i.Zero;
            rectTarget = Rectangle.Empty;
            
            // Reset render states
            _renderLand = false;
            _renderWater = false;
            _renderCellGrid = false;
            _renderBuildVolumes = false;
            _renderBorders = false;
            
            #if DEBUG
            
            // Reset debug render options
            debugRenderBuildVolumes = false;
            debugRenderBorders = false;
            
            #endif
            
            // Dispose of GDI resources
            gfxTarget.Dispose();
            bmpTarget.Dispose(); 
            iaTarget.Dispose();
            bmpTarget = null;
            gfxTarget = null;
            guTarget = (GraphicsUnit)0;
            iaTarget = null;
            
            // This is no longer a valid state
            _disposed = true;
        }
        
        #endregion
        
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
        
        public Maths.Vector2f WorldspaceClipperCentre()
        {
            var nw = new Maths.Vector2f( cellNW.X * bbConstant.WorldMap_Resolution, cellNW.Y * bbConstant.WorldMap_Resolution );
            var s = GetClipperCellSize();
            var ws = s * bbConstant.WorldMap_Resolution;
            return new Maths.Vector2f(
                nw.X + ws.X * 0.5f,
                nw.Y - ws.Y * 0.5f );
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
        }
        
        #endregion
        
        #region Public Accessors
        
        public VolumeEditor                     AttachedEditor
        {
            get
            {
                return attachedEditor;
            }
            set
            {
                attachedEditor = value;
            }
        }
        
        public bbWorldspace                     Worldspace
        {
            get
            {
                return worldspace;
            }
            set
            {
                if( value == null ) return;
                worldspace = value;
                UpdateCellClipper( worldspace.CellNW, worldspace.CellSE, false );
                SetViewCentre( WorldspaceClipperCentre(), false );
            }
        }
        
        public bbImportMod                      ImportMod
        {
            get
            {
                return importMod;
            }
            set
            {
                importMod = value;
            }
        }
        
        public Maths.Vector2i                   CellNW                  { get { return cellNW; } }
        public Maths.Vector2i                   CellSE                  { get { return cellSE; } }
        
        public List<VolumeParent>               RenderVolumes
        {
            get
            {
                return renderVolumes;
            }
            set
            {
                renderVolumes = value;
            }
        }
        
        public List<VolumeParent>               HighlightParents
        {
            get
            {
                return hlParents;
            }
            set
            {
                hlParents = value;
            }
        }
        
        public List<BuildVolume>                HighlightVolumes
        {
            get
            {
                return hlVolumes;
            }
            set
            {
                hlVolumes = value;
            }
        }
        
        #endregion
        
        #region Scale Calculator
        
        public float CalculateScale( Maths.Vector2i cells )
        {
            var scaleNS = (float)rectTarget.Height / ( (float)cells.Y * bbConstant.WorldMap_Resolution );
            var scaleEW = (float)rectTarget.Width / ( (float)cells.X * bbConstant.WorldMap_Resolution );
            var _scale = scaleNS < scaleEW ? scaleNS : scaleEW;
            return _scale;
        }
        
        public static float CalculateScale( Maths.Vector2i cells, Maths.Vector2i worldCells )
        {
            var scaleNS = (float)( (float)cells.Y / (float)worldCells.Y );
            var scaleEW = (float)( (float)cells.X / (float)worldCells.X );
            var _scale = Maths.InverseLerp( bbConstant.MinZoom, bbConstant.MaxZoom, scaleNS > scaleEW ? scaleNS : scaleEW );
            return _scale;
        }
        
        #endregion
        
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
            
            if( attachedEditor != null )
                attachedEditor.DrawEditor();
            
            if( cacheRenderLayers )
            {
                _renderLand = renderLand;
                _renderWater = renderWater;
                _renderCellGrid = renderCellGrid;
                _renderBuildVolumes = renderBuildVolumes;
                _renderBorders = renderBorders;
            }
            
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
            var tp = WorldspaceToScreenspace( x, y );
            gfxTarget.DrawString( text, font, brush, tp.X, tp.Y );
        }
        
        public void DrawCircleWorldTransform( Pen pen, float x, float y, float r )
        {
            var sr = r * scale;
            var tp = WorldspaceToScreenspace( x, y );
            var tr = new Rectangle(
                (int)( tp.X - sr ),
                (int)( tp.Y - sr ),
                (int)( sr * 2f ),
                (int)( sr * 2f ) );
            gfxTarget.DrawEllipse( pen, tr );
        }
        
        public void DrawLineWorldTransform( Pen pen, float x0, float y0, float x1, float y1 )
        {
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
            var hmClipper = WorldspaceToHeightmapClipper( viewCentre.X, viewCentre.Y );
            gfxTarget.DrawImage( worldspace.LandHeight_Bitmap, rectTarget, hmClipper.X, hmClipper.Y, hmClipper.Width, hmClipper.Height, guTarget, iaTarget );
        }
        
        public void DrawWaterMap()
        {
            if( worldspace.WaterHeight_Bitmap == null ) return;
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
                
                var editorEnabled = ( attachedEditor != null )&&( attachedEditor.Enabled );
                
                foreach( var volume in volumeParent.BuildVolumes )
                {
                    var usePen = editorEnabled ? penMid : penLow;
                    if( !editorEnabled )
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
            
            var editorEnabled = ( attachedEditor != null )&&( attachedEditor.Enabled );
            
            int g = 255;
            if(
                ( !editorEnabled )&&
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
            var pen0 = new Pen( Color.FromArgb( 127, 63, 63, 191 ) );
            var pen = new Pen( Color.FromArgb( 127, 91, 91, 0 ) );
            
            for( int y = cellSE.Y; y <= cellNW.Y; y++ )
            {
                if( ( y >= worldspace.CellSE.Y )&&( y <= worldspace.CellNW.Y ) )
                {
                    for( int x = cellNW.X; x <= cellSE.X; x++ )
                    {
                        if( ( x >= worldspace.CellNW.X )&&( x <= worldspace.CellSE.X ) )
                        {
                            var p0 = new Maths.Vector2f(   x       * bbConstant.WorldMap_Resolution    ,   y       * bbConstant.WorldMap_Resolution     );
                            var p1 = new Maths.Vector2f( ( x + 1 ) * bbConstant.WorldMap_Resolution - 1, ( y - 1 ) * bbConstant.WorldMap_Resolution + 1 );
                            if(
                                ( ( x < -1 )&&( x > 0 ) )&&
                                ( ( y < -1 )&&( y > 0 ) )
                            )
                                DrawRectWorldTransform( pen, p0, p1 );
                            else
                            {
                                var e0 = ( y == -1 ) ? pen0 : pen;
                                var e1 = ( x == -1 ) ? pen0 : pen;
                                var e2 = ( y ==  0 ) ? pen0 : pen;
                                var e3 = ( x ==  0 ) ? pen0 : pen;
                                DrawLineWorldTransform( e0, p0.X, p0.Y, p1.X, p0.Y ); // top
                                DrawLineWorldTransform( e1, p1.X, p0.Y, p1.X, p1.Y ); // right
                                DrawLineWorldTransform( e2, p1.X, p1.Y, p0.X, p1.Y ); // bottom
                                DrawLineWorldTransform( e3, p0.X, p1.Y, p0.X, p0.Y ); // left
                            }
                        }
                    }
                }
            }
        }
        
        #endregion
        
    }
}
