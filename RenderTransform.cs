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


using SDL2ThinLayer;
using SDL2;

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
        SDL2ThinLayer.SDLRenderer sdlRenderer;
        bbMain mainForm;
        Control sdlTarget;
        SDL.SDL_Rect rectTarget;
        
        // Last rendered layers and controls
        bool _renderLand;
        bool _renderWater;
        bool _renderCellGrid;
        bool _renderBuildVolumes;
        bool _renderBorders;
        bool _renderFast;
        
        // High light parents and volumes
        List<VolumeParent> hlParents = null;
        List<BuildVolume> hlVolumes = null;
        
        VolumeEditor attachedEditor;
        
        SDLRenderer.void_RendererOnly WindowClosed;
        
        #region Constructor and Dispose()
        
        public RenderTransform( bbMain _form, Control _target )
        {
            mainForm = _form;
            sdlTarget = _target;
            attachedEditor = null;
            WindowClosed = null;
            RecreateSDLRenderer( true );
        }
        
        public RenderTransform( bbMain _form, Size size, SDLRenderer.void_RendererOnly windowClosed )
        {
            mainForm = _form;
            sdlTarget = null;
            attachedEditor = null;
            WindowClosed = windowClosed;
            rectTarget = size.ToSDLRect();
            RecreateSDLRenderer( true );
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
            if( sdlRenderer != null )
                sdlRenderer.Dispose();
            sdlRenderer = null;
            attachedEditor = null;
            //pbTarget = null;
            mainForm = null;
            sdlTarget = null;
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
            //rectTarget = Rectangle.Empty;
            
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
            
            /*
            // Dispose of GDI resources
            gfxTarget.Dispose();
            bmpTarget.Dispose(); 
            iaTarget.Dispose();
            bmpTarget = null;
            gfxTarget = null;
            guTarget = (GraphicsUnit)0;
            iaTarget = null;
            */
            
            // This is no longer a valid state
            _disposed = true;
        }
        
        #endregion
        
        #endregion
        
        #region SDL_Window and SDL_Renderer [re]size/reset events
        
        void SDL_RendererReset( SDLRenderer renderer )
        {
            Console.WriteLine( "SDL_RendererReset()" );
            ReloadWorldspaceTextures( true );
        }
        
        void SDL_WindowResized( SDLRenderer renderer, SDL.SDL_Event e )
        {
            Console.WriteLine( "SDL_WindowResized()" );
            
            // Tell the transform renderer that an external event resized the window
            rectTarget = sdlRenderer.WindowSize.ToSDLRect();
            UpdateSceneMetrics();
        }
        
        bool _sdl_WindowSize_Queued = false;
        void SDL_SetWindowSize( SDLRenderer renderer )
        {
            Console.WriteLine( "SDL_SetWindowSize()" );
            
            // Tell the sdl renderer that an internal event wants to resize the window
            rectTarget = sdlTarget.Size.ToSDLRect();
            sdlRenderer.WindowSize = sdlTarget.Size;
            UpdateSceneMetrics();
            
            _sdl_WindowSize_Queued = false;
        }
        
        public void InvokeSetSDLWindowSize()
        {
            Console.WriteLine( "InvokeSetSDLWindowSize()" );
            
            // Not attached, SDL_Window will handle this
            if( sdlTarget == null ) return;
            
            // Don't requeue if we haven't handled the last one
            if( _sdl_WindowSize_Queued ) return;
            
            _sdl_WindowSize_Queued = true;
            
            // Invoke a window size update in the renderer thread
            sdlRenderer.BeginInvoke( SDL_SetWindowSize );
        }
        
        public void RecreateSDLRenderer( bool forceRebuild )
        {
            Console.WriteLine( "RecreateSDLRenderer()" );
            
            SDL.SDL_Rect newRect = rectTarget;
            
            if( sdlTarget != null )
            {
                if(
                    ( sdlTarget.Width < 1 )||
                    ( sdlTarget.Height < 1 )
                )   return;
                
                var rect = sdlTarget.Size.ToSDLRect();
                
                if(
                    ( rect.Equals( rectTarget ) )&&
                    ( !forceRebuild )
                )   return;
                
                newRect = rect;
            }
            else
            {
                if( sdlRenderer != null )
                    newRect = sdlRenderer.WindowSize.ToSDLRect();
            }
            
            if(
                ( sdlRenderer != null )&&
                ( !forceRebuild )
           )   return;
            
            // Build a whole new renderer
            bool doGC = false;
            if( sdlRenderer != null )
            {
                sdlRenderer.Dispose();
                sdlRenderer = null;
                doGC = true;
            }
            
            if( doGC )
                System.GC.Collect( System.GC.MaxGeneration );
            
            rectTarget = newRect;
            
            sdlRenderer =
                sdlTarget != null
                ? new SDLRenderer( mainForm, sdlTarget )
                : new SDLRenderer( mainForm, rectTarget.w, rectTarget.h, "da window", WindowClosed );
            if( sdlRenderer == null )
                throw new Exception( string.Format( "Error resizing target:\n{0}", SDL.SDL_GetError() ) );
            
            sdlRenderer.BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
            sdlRenderer.DrawScene += SDL_DrawScene;
            if( sdlTarget == null )
                sdlRenderer.WindowResized += SDL_WindowResized;
            sdlRenderer.RendererReset += SDL_RendererReset;
            
            UpdateSceneMetrics();
        }
        
        #endregion
        
        #region Render State Manipulation
        
        public void UpdateSceneMetrics()
        {
            // TODO:  Don't much with trueCentre unless out of bounds!
            trueCentre = new Maths.Vector2f( rectTarget.Centre() );
            minScale = CalculateScale( cmSize );
            scale = Math.Min( maxScale, Math.Max( minScale, scale ) );
            invScale = 1.0f / scale;
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
            //bool mustRebuildBuffers = ( bmpTarget == null )||( gfxTarget == null )||( iaTarget == null );
            
            cellNW = _cellNW;
            cellSE = _cellSE;
            hmCentre = worldspace.HeightMapOffset;
                
            // Compute
            cmSize = bbSpaceConversions.SizeOfCellRange( cellNW, cellSE );
            
            if( updateScene )
                UpdateSceneMetrics();
        }
        
        public float GetScale() { return scale; }
        public void SetScale( float value, bool updateScene = true )
        {
            scale = value;
            invScale = 1.0f / scale;
            if( updateScene )
                UpdateSceneMetrics();
        }
        public float GetInvScale() { return invScale; }
        public void SetInvScale( float value, bool updateScene = true )
        {
            scale = 1.0f / value;
            if( updateScene )
                UpdateSceneMetrics();
        }
        
        public Maths.Vector2f GetViewCentre() { return viewCentre; }
        public void SetViewCentre( Maths.Vector2f value, bool updateScene = true )
        {
            viewCentre = value;
            if( updateScene )
                UpdateSceneMetrics();
        }
        
        public Maths.Vector2i GetClipperCellSize() { return cmSize; }
        
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
        
        public bbMain                           MainForm { get { return mainForm; } }
        public Control                          TargetControl { get { return sdlTarget; } }
        public SDLRenderer                      Renderer { get { return sdlRenderer; } }
        
        public bbWorldspace                     Worldspace
        {
            get
            {
                return worldspace;
            }
            set
            {
                if( value == null ) return;
                if( worldspace != null )
                    worldspace.DestroySurfaces();
                worldspace = value;
                UpdateCellClipper( worldspace.CellNW, worldspace.CellSE, false );
                SetViewCentre( WorldspaceClipperCentre(), false );
                ReloadWorldspaceTextures( false );
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
            var scaleNS = (float)rectTarget.h / ( (float)cells.Y * bbConstant.WorldMap_Resolution );
            var scaleEW = (float)rectTarget.w / ( (float)cells.X * bbConstant.WorldMap_Resolution );
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
            var ss = new Maths.Vector2f( rectTarget.w, rectTarget.h ) * invScale;
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
        
        void ReloadWorldspaceTextures( bool forceReload )
        {
            Console.WriteLine( "ReloadWorldspaceTextures()" );
            if( worldspace == null ) return;
            if(
                ( sdlRenderer == null )||
                ( !sdlRenderer.IsReady )
            ) return;
            if(
                ( forceReload )||
                (
                    ( _renderLand )&&
                    ( worldspace.LandHeight_Texture == null )
                )||
                (
                    ( _renderWater )&&
                    ( worldspace.WaterHeight_Texture == null )
                )
            ) {
                worldspace.CreateHeightmapTextures( this );
            }
        }
        
        #region Render Scene
        
        public bool                             RenderLand
        {
            get     { return _renderLand; }
            set
            {
                _renderLand = value;
                ReloadWorldspaceTextures( false );
            }
        }
        
        public bool                             RenderWater
        {
            get     { return _renderWater; }
            set
            {
                _renderWater = value;
                ReloadWorldspaceTextures( false );
            }
        }
        
        public bool                             RenderCellGrid
        {
            get     { return _renderCellGrid; }
            set     { _renderCellGrid = value; }
        }
        
        public bool                             RenderBuildVolumes
        {
            get     { return _renderBuildVolumes; }
            set     { _renderBuildVolumes = value; }
        }
        
        public bool                             RenderBorders
        {
            get     { return _renderBorders; }
            set     { _renderBorders = value; }
        }
        
        public bool                             RenderFast
        {
            get     { return _renderFast; }
            set
            {
                _renderFast = value;
                sdlRenderer.PreserveUserState = !value;
            }
        }
        
        public void UpdateScene( bool renderLand, bool renderWater, bool renderCellGrid, bool renderBuildVolumes, bool renderBorders, bool fastRender )
        {
            _renderLand = renderLand;
            _renderWater = renderWater;
            _renderCellGrid = renderCellGrid;
            _renderBuildVolumes = renderBuildVolumes;
            _renderBorders = renderBorders;
            _renderFast = fastRender;
            sdlRenderer.PreserveUserState = !fastRender;
            ReloadWorldspaceTextures( false );
        }
        
        void SDL_DrawScene( SDLRenderer renderer )
        {
            if( renderer != sdlRenderer ) return;
            
            if( _renderLand )
                DrawLandMap();
            
            if( _renderWater )
                DrawWaterMap();
            
            if( _renderCellGrid )
                DrawCellGrid();
            
            if( _renderBuildVolumes )
                DrawBuildVolumes();
            
            if( _renderBorders )
                DrawParentBorders();
            
            if( attachedEditor != null )
                attachedEditor.DrawEditor();
            
        }
        
        public void RenderToPNG()
        {
            // Export the [whole] map to a PNG
            if(
                ( !renderVolumes.NullOrEmpty() )&&
                ( renderVolumes.Count > 1 )
               ) return;
            var filename = renderVolumes.NullOrEmpty() ? worldspace.FormID : renderVolumes[ 0 ].FormID + ".png";
            //bmpTarget.Save( filename, System.Drawing.Imaging.ImageFormat.Png );
            // TODO:  Implement SDLRenderer PNG Save
        }
        
        #endregion
        
        #region Drawing Primitives (cellspace (heightmap) to transform target)
        
        public void DrawLineCellTransform( float x0, float y0, float x1, float y1, Color c )
        {
            var tp0 = CellspaceToScreenspace( x0, y0 ).ToSDLPoint( !_renderFast );
            var tp1 = CellspaceToScreenspace( x1, y1 ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawLine( tp0, tp1, c );
        }
        
        public void DrawRectCellTransform( Maths.Vector2f p0, Maths.Vector2f p1, Color c )
        {
            DrawLineCellTransform( p0.X, p0.Y, p1.X, p0.Y, c );
            DrawLineCellTransform( p1.X, p0.Y, p1.X, p1.Y, c );
            DrawLineCellTransform( p1.X, p1.Y, p0.X, p1.Y, c );
            DrawLineCellTransform( p0.X, p1.Y, p0.X, p0.Y, c );
        }
        
        #endregion
        
        #region Drawing Primitives (worldspace to transform target)
        
        public void DrawTextWorldTransform( string text, float x, float y, Color c, SDLRenderer.Font font, int style = 0 )
        {
           var tp = WorldspaceToScreenspace( x, y ).ToSDLPoint( !_renderFast );
           sdlRenderer.DrawText( tp, font, text, c, style );
        }
        
        public void DrawCircleWorldTransform( float x, float y, float r, Color c )
        {
            var sr = _renderFast ? (int)( r * scale ) : (int)Math.Round( r * scale );
            var tp = WorldspaceToScreenspace( x, y ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawCircle( tp, sr, c );
        }
        
        public void DrawLineWorldTransform( float x0, float y0, float x1, float y1, Color c )
        {
            var tp0 = WorldspaceToScreenspace( x0, y0 ).ToSDLPoint( !_renderFast );
            var tp1 = WorldspaceToScreenspace( x1, y1 ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawLine( tp0, tp1, c );
        }
        
        public void DrawLineWorldTransform( Maths.Vector2f p0, Maths.Vector2f p1, Color c )
        {
            DrawLineWorldTransform( p0.X, p0.Y, p1.X, p1.Y, c );
        }
        
        public void DrawLineWorldTransform( Maths.Vector3f p0, Maths.Vector3f p1, Color c )
        {
            DrawLineWorldTransform( p0.X, p0.Y, p1.X, p1.Y, c );
        }
        
        public void DrawRectWorldTransform( Maths.Vector2f p0, Maths.Vector2f p1, Color c )
        {
            DrawLineWorldTransform( p0.X, p0.Y, p1.X, p0.Y, c );
            DrawLineWorldTransform( p1.X, p0.Y, p1.X, p1.Y, c );
            DrawLineWorldTransform( p1.X, p1.Y, p0.X, p1.Y, c );
            DrawLineWorldTransform( p0.X, p1.Y, p0.X, p0.Y, c );
        }
        
        public void DrawRectWorldTransform( Maths.Vector3f p0, Maths.Vector3f p1, Color c )
        {
            DrawLineWorldTransform( p0.X, p0.Y, p1.X, p0.Y, c );
            DrawLineWorldTransform( p1.X, p0.Y, p1.X, p1.Y, c );
            DrawLineWorldTransform( p1.X, p1.Y, p0.X, p1.Y, c );
            DrawLineWorldTransform( p0.X, p1.Y, p0.X, p0.Y, c );
        }
        
        public void DrawPolyWorldTransform( Maths.Vector2f[] p, Color c )
        {
            var last = p.Length - 1;
            for( int index = 0; index < p.Length - 1; index++ )
                DrawLineWorldTransform( p[ index ], p[ index + 1 ], c );
            DrawLineWorldTransform( p[ last ], p[ 0 ], c );
        }
        
        #endregion
        
        #region Heightmap Drawing
        
        public void DrawLandMap()
        {
            if( worldspace.LandHeight_Texture == null ) return;
            var hmClipper = WorldspaceToHeightmapClipper( viewCentre.X, viewCentre.Y );
            var rect = hmClipper.ToSDLRect();
            sdlRenderer.Blit( rectTarget, worldspace.LandHeight_Texture, rect );
        }
        
        public void DrawWaterMap()
        {
            if( worldspace.WaterHeight_Texture == null ) return;
            var hmClipper = WorldspaceToHeightmapClipper( viewCentre.X, viewCentre.Y );
            var rect = hmClipper.ToSDLRect();
            sdlRenderer.Blit( rectTarget, worldspace.WaterHeight_Texture, rect );
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
                var cHigh = Color.FromArgb( 255, rb, 0, rb );
                rb -= 48;
                var cMid = Color.FromArgb( 255, rb, 0, rb );
                rb -= 48;
                var cLow = Color.FromArgb( 255, rb, 0, rb );
                
                var editorEnabled = ( attachedEditor != null )&&( attachedEditor.Enabled );
                
                foreach( var volume in volumeParent.BuildVolumes )
                {
                    var useColor = editorEnabled ? cMid : cLow;
                    if( !editorEnabled )
                    {
                        if( hlParents.NullOrEmpty() )
                        {
                            useColor = cHigh;
                        }
                        else if(
                            ( !hlVolumes.NullOrEmpty() )&&
                            ( hlVolumes.Contains( volume ) )
                        )
                        {
                            useColor = cHigh;
                        }
                        else if( hlParents.Contains( volumeParent ) )
                        {
                            useColor = cMid;
                        }
                    }
                    
                    DrawPolyWorldTransform( volume.Corners, useColor );
                }
            }
            #if DEBUG
            else
            {
                int r = 255;
                int g = 0;
                int b = 127;
                SDLRenderer.Font font = null; // TODO: FILL THIS!
                
                for( var i = 0; i < volumeParent.BorderSegments.Count; i++ )
                {
                    var segment = volumeParent.BorderSegments[ i ];
                    
                    var c = Color.FromArgb( 255, r, g, b );
                    
                    r -= 32;
                    g += 32;
                    b += 64;
                    if( r <   0 ) r += 255;
                    if( g > 255 ) g -= 255;
                    if( b > 255 ) b -= 255;
                    
                    var niP = ( segment.P0 + segment.P1 ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, c, font );
                    DrawLineWorldTransform( segment.P0, segment.P1, c );
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
            
            var c = Color.FromArgb( 255, 0, g, 0 );
            
            #if DEBUG
            SDLRenderer.Font font = null; // TODO: FILL THIS!
            #endif
            
            for( var i = 0; i < volumeParent.BorderNodes.Count; i++ )
            {
                var node = volumeParent.BorderNodes[ i ];
                
                /*
                if(
                    ( !hlVolumes.NullOrEmpty() )&&
                    ( hlVolumes.Contains( node.Volume ) )
                )   g >>= 1;
                */
                
                DrawLineWorldTransform( node.A, node.B, c );
                
                #if DEBUG
                if( debugRenderBorders )
                {
                    var niP = ( node.A + node.B ) * 0.5f;
                    DrawTextWorldTransform( i.ToString(), niP.X, niP.Y, c, font );
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
            var c0 = Color.FromArgb( 127, 63, 63, 191 );
            var c = Color.FromArgb( 127, 91, 91, 0 );
            
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
                            {
                                DrawRectWorldTransform( p0, p1, c );
                            }
                            else
                            {
                                var e0 = ( y == -1 ) ? c0 : c;
                                var e1 = ( x == -1 ) ? c0 : c;
                                var e2 = ( y ==  0 ) ? c0 : c;
                                var e3 = ( x ==  0 ) ? c0 : c;
                                DrawLineWorldTransform( p0.X, p0.Y, p1.X, p0.Y, e0 ); // top
                                DrawLineWorldTransform( p1.X, p0.Y, p1.X, p1.Y, e1 ); // right
                                DrawLineWorldTransform( p1.X, p1.Y, p0.X, p1.Y, e2 ); // bottom
                                DrawLineWorldTransform( p0.X, p1.Y, p0.X, p0.Y, e3 ); // left
                            }
                        }
                    }
                }
            }
        }
        
        #endregion
        
    }
}
