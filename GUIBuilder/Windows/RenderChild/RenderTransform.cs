/*
 * RenderTransform.cs
 *
 * Handles all the specifics of rendering the selected worldspace and object within.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Maths;
using Engine;

using Fallout4;
using AnnexTheCommonwealth;

using SDL2ThinLayer;
using SDL2;

namespace GUIBuilder.Windows.RenderChild
{
    
    public class RenderTransform : IDisposable
    {
        
        public enum Heightmap
        {
            Land,
            Water
        }
        
        struct EdgeLine
        {
            public Maths.Vector2f p0, p1;
            public Color c;
            public SubDivision SubDivision;
            
            public EdgeLine( SubDivision subdivision, Maths.Vector3f P0, Maths.Vector3f P1, Color C )
            {
                SubDivision = subdivision;
                p0.X = P0.X;
                p0.Y = P0.Y;
                p1.X = P1.X;
                p1.Y = P1.Y;
                c = C;
            }
        }
        
        // Thread protection so updating can't happen while rendering and rendering can't happen while updating
        int _sceneUpdate = 0;
        int _sceneRender = 0;
        
        // Source data pointers
        Engine.Plugin.Forms.Worldspace _worldspace;
        GodObject.WorldspaceDataPool.PoolEntry _poolEntry;
        //ImportMod _importMod;
        List<WorkshopScript> _workshops;
        List<Settlement> _settlements;
        List<SubDivision> _subdivisions;
        
        List<EdgeFlag> _associatedEdgeFlags;
        List<EdgeFlag> _unassociatedEdgeFlags;
        List<EdgeLine> _edgeLines;
        
        // Clipper inputs held for reference
        Maths.Vector2i _cellNW;
        Maths.Vector2i _cellSE;
        
        // View port
        Maths.Vector2f _viewCentre;
        Maths.Vector2f _trueCentre;
        float _scale;
        float _invScale;
        float _minScale;
        const float _maxScale = 1.0f;
        
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
        Control sdlTarget;
        SDL.SDL_Rect rectTarget;
        Maths.Vector2i _mousePos;
        
        // Last rendered layers and controls
        bool _renderFast;
        bool _renderCellGrid;
        bool _renderLand;
        bool _renderWater;
        unsafe bool _renderWorkshops;
        unsafe bool _renderSettlements;
        unsafe bool _renderSubDivisions;
        unsafe bool _renderEdgeFlags;
        unsafe bool _renderEdgeFlagLinks;
        bool _renderBuildVolumes;
        bool _renderSandboxVolumes;
        bool _renderBorders;
        
        float _minRenderScaledSettlementObjects = 4.0f;
        
        // High light parents and volumes
        List<SubDivision> hlSubDivisions = null;
        List<Volume> hlVolumes = null;
        
        //VolumeEditor attachedEditor;
        
        SDLRenderer.void_RendererOnly WindowClosed;
        
        Vector2i            _textBoxPadding = new Vector2i( 2, 2 );
        
        SDLRenderer.Texture _st_Red_X = null;
        SDLRenderer.Texture _st_Settlement = null;
        SDLRenderer.Texture _st_SubDivision = null;
        SDLRenderer.Texture _st_Workshop = null;
        SDLRenderer.Texture _st_BorderEnabler = null;
        SDLRenderer.Texture _st_EdgeFlag = null;
        SDLRenderer.Texture _st_UnassociatedEdgeFlag = null;
        SDLRenderer.Font    _sf_Tahoma = null;
        
        #region Constructor and Dispose()
        
        public RenderTransform( bool startSceneUpdate, SDLRenderer.InitParams initParams )
        {
            CreateTransform( startSceneUpdate, initParams ); //target, Size.Empty, null );
        }
        
        //public RenderTransform( bool startSceneUpdate, Size size, SDLRenderer.void_RendererOnly windowClosed )
        //{
        //    CreateTransform( startSceneUpdate, null, size, windowClosed );
        //}
        
        void CreateTransform( bool startSceneUpdate, SDLRenderer.InitParams initParams ) //Control target, Size size, SDLRenderer.void_RendererOnly windowClosed )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.CreateTransform :: startSceneUpdate = " + startSceneUpdate + "\n" );
            _sceneUpdate = 1; // Lock the transform while we create it
            _sceneRender = 0;
            sdlTarget = initParams.TargetControl;// target;
            //attachedEditor = null;
            WindowClosed = null;
            WindowClosed = initParams.WindowClosed;
            rectTarget = initParams.WindowSize.ToSDLRect();
            RecreateSDLRenderer( true, initParams );
            if( !startSceneUpdate )
                _sceneUpdate = 0; // Unlock the transform now that it's created
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
            
            // Destroy all the surfaces and textures
            GodObject.WorldspaceDataPool.DrainPool();
            
            // Dispose of external references
            if( sdlRenderer != null )
                sdlRenderer.Dispose();
            sdlRenderer = null;
            //attachedEditor = null;
            //pbTarget = null;
            sdlTarget = null;
            _worldspace = null;
            _poolEntry = null;
            //_importMod = null;
            _workshops = null;
            _settlements = null;
            _subdivisions = null;
            _associatedEdgeFlags = null;
            _unassociatedEdgeFlags = null;
            hlSubDivisions = null;
            hlVolumes = null;
            
            // Reset all type fields (not strictly necessary)
            _cellNW = Maths.Vector2i.Zero;
            _cellSE = Maths.Vector2i.Zero;
            _scale = 0f;
            _invScale = 0f;
            _minScale = 0f;
            _viewCentre = Maths.Vector2f.Zero;
            _trueCentre = Maths.Vector2f.Zero;
            hmCentre = Maths.Vector2i.Zero;
            cmSize = Maths.Vector2i.Zero;
            //rectTarget = Rectangle.Empty;
            
            // Reset render states
            _renderFast = false;
            _renderCellGrid = false;
            _renderLand = false;
            _renderWater = false;
            _renderWorkshops = false;
            _renderSettlements = false;
            _renderSubDivisions = false;
            _renderEdgeFlags = false;
            _renderEdgeFlagLinks = false;
            _renderBuildVolumes = false;
            _renderSandboxVolumes = false;
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
        
        #region Rendering and state manipulation thread syncronization
        // Read:  Prevent scene updates and rendering from happening at the same time
        
        public void SyncSceneUpdate( bool start, int trace = 0 )
        {
            //trace = 2;
            if( start )
            {
                while( _sceneRender > 0 )
                    System.Threading.Thread.Sleep( 0 );
                _sceneUpdate += 1;
            }
            else
                _sceneUpdate -= 1;
            if( trace > 0 )
                DebugLog.WriteLine( "GUIBuilder.RenderTransform :: SyncSceneUpdate() :: start = " + start + " :: _sceneUpdate = " + _sceneUpdate + " :: _sceneRender = " + _sceneRender + "\n" + ( trace < 2 ? "" : Environment.StackTrace + "\n" ) );
        }
        
        public void SyncRenderScene( bool start, int trace = 0 )
        {
            //trace = 2;
            if( start )
            {
                while( _sceneUpdate > 0 )
                    System.Threading.Thread.Sleep( 0 );
                _sceneRender += 1;
            }
            else
                _sceneRender -= 1;
            if( trace > 0 )
                DebugLog.WriteLine( "GUIBuilder.RenderTransform :: SyncRenderScene() :: start = " + start + " :: _sceneUpdate = " + _sceneUpdate + " :: _sceneRender = " + _sceneRender + "\n" + ( trace < 2 ? "" : Environment.StackTrace + "\n" ) );
        }
        
        #endregion
        
        #region SDL_Window and SDL_Renderer [re]size/reset events
        
        void SDL_RendererReset( SDLRenderer renderer )
        {
            //DebugLog.Write( "SDL_RendererReset()" );
            ReloadObjectTextures( true );
            ReloadWorldspaceTextures( true );
        }
        
        void SDL_WindowResized( SDLRenderer renderer, SDL.SDL_Event e )
        {
            //DebugLog.Write( "SDL_WindowResized()" );
            
            // Tell the transform renderer that an external event resized the window
            rectTarget = sdlRenderer.WindowSize.ToSDLRect();
            UpdateSceneMetrics();
        }
        
        bool _sdl_WindowSize_Queued = false;
        void SDL_SetWindowSize( SDLRenderer renderer )
        {
            //DebugLog.Write( "SDL_SetWindowSize()" );
            
            SyncSceneUpdate( true );
            
            // Tell the sdl renderer that an internal event wants to resize the window
            rectTarget = sdlTarget.Size.ToSDLRect();
            sdlRenderer.WindowSize = sdlTarget.Size;
            UpdateSceneMetrics();
            
            SyncSceneUpdate( false );
            
            _sdl_WindowSize_Queued = false;
        }
        
        public void InvokeSetSDLWindowSize()
        {
            //DebugLog.Write( "InvokeSetSDLWindowSize()" );
            
            // Not attached, SDL_Window will handle this
            if( sdlTarget == null ) return;
            
            // Don't requeue if we haven't handled the last one
            if( _sdl_WindowSize_Queued ) return;
            
            _sdl_WindowSize_Queued = true;
            
            // Invoke a window size update in the renderer thread
            sdlRenderer.BeginInvoke( SDL_SetWindowSize );
        }
        
        void RecreateSDLRenderer( bool forceRebuild, SDLRenderer.InitParams initParams )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.RecreateSDLRenderer()" );
            //SyncSceneUpdate( true );
            
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
            
            sdlRenderer = new SDLRenderer( initParams );
                //sdlTarget != null
                //? new SDLRenderer( GodObject.Windows.GetRenderWindow(), sdlTarget )
                //: new SDLRenderer( GodObject.Windows.GetMainWindow(), rectTarget.w, rectTarget.h, "da window", WindowClosed );
            if( sdlRenderer == null )
                throw new Exception( string.Format( "Error resizing target:\n{0}", SDL.SDL_GetError() ) );
            
            //sdlRenderer.ShowCursor = false;
            sdlRenderer.BlendMode = SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND;
            sdlRenderer.DrawScene += SDL_DrawScene;
            if( sdlTarget == null )
                sdlRenderer.WindowResized += SDL_WindowResized;
            sdlRenderer.RendererReset += SDL_RendererReset;
            
            //ReloadObjectTextures( false );
            UpdateSceneMetrics();
            
            //SyncSceneUpdate( false );
        }
        
        #endregion
        
        #region Render State Manipulation
        
        void UpdateSceneMetrics()
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.UpdateSceneMetrics()" );
            SyncSceneUpdate( true );
            
            // TODO:  Don't muck with trueCentre unless out of bounds!
            _trueCentre = new Maths.Vector2f( rectTarget.Centre() );
            _minScale = CalculateScale( cmSize );
            _scale = Math.Min( _maxScale, Math.Max( _minScale, _scale ) );
            _invScale = 1.0f / _scale;
            
            SyncSceneUpdate( false );
        }
        
        public Maths.Vector2f WorldspaceClipperCentre()
        {
            var nw = new Maths.Vector2f( _cellNW.X * Engine.Constant.WorldMap_Resolution, _cellNW.Y * Engine.Constant.WorldMap_Resolution );
            var s = GetClipperCellSize();
            var ws = s * Engine.Constant.WorldMap_Resolution;
            return new Maths.Vector2f(
                nw.X + ws.X * 0.5f,
                nw.Y - ws.Y * 0.5f );
        }
        
        public void UpdateCellClipper( Maths.Vector2i cellNW, Maths.Vector2i cellSE, bool updateScene = true )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.UpdateCellClipper()" );
            SyncSceneUpdate( true );
            
            _cellNW = cellNW;
            _cellSE = cellSE;
            hmCentre = _poolEntry != null
                ? _poolEntry.HeightMapOffset
                : Maths.Vector2i.Zero;
            _viewCentre.X = Math.Max( Math.Min( _viewCentre.X, _cellSE.X ), _cellNW.X );
            _viewCentre.Y = Math.Max( Math.Min( _viewCentre.Y, _cellNW.Y ), _cellSE.Y );
            
            // Compute
            cmSize = Engine.SpaceConversions.SizeOfCellRange( _cellNW, _cellSE );
            
            if( updateScene )
                UpdateSceneMetrics();
            
            SyncSceneUpdate( false );
        }
        
        public float GetScale() { return _scale; }
        public void SetScale( float value, bool updateScene = true )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.SetScale()" );
            SyncSceneUpdate( true );
            _scale = value;
            _invScale = 1.0f / _scale;
            if( updateScene )
                UpdateSceneMetrics();
            SyncSceneUpdate( false );
        }
        public float GetInvScale() { return _invScale; }
        public void SetInvScale( float value, bool updateScene = true )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.SetInvScale()" );
            SyncSceneUpdate( true );
            _scale = 1.0f / value;
            if( updateScene )
                UpdateSceneMetrics();
            SyncSceneUpdate( false );
        }
        
        public Maths.Vector2f GetViewCentre() { return _viewCentre; }
        public void SetViewCentre( Maths.Vector2f value, bool updateScene = true )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.SetViewCentre()" );
            SyncSceneUpdate( true );
            _viewCentre = value;
            if( updateScene )
                UpdateSceneMetrics();
            SyncSceneUpdate( false );
        }
        
        public void SetMousePos( Maths.Vector2i mouse )
        {
            _mousePos = mouse;
            UpdateMouseOver();
            //{
                /*
                DebugLog.Write( "\n\n---------------------\nMouse Over Changed!\n" );
                foreach( var moi in _mouseOverInfo )
                    DebugLog.Write( moi );
                DebugLog.Write( "\n---------------------\n\n" );
                */
            //}
        }
        
        public Maths.Vector2i GetClipperCellSize() { return cmSize; }
        
        /*
        public VolumeEditor                     AttachedEditor
        {
            get
            {
                return attachedEditor;
            }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.Workshops.Set() :: value = " + value );
                SyncSceneUpdate( true );
                attachedEditor = value;
                SyncSceneUpdate( false );
            }
        }
        */
        
        //public bbMain                           MainForm { get { return mainForm; } }
        public Control                          TargetControl { get { return sdlTarget; } }
        public SDLRenderer                      Renderer { get { return sdlRenderer; } }
        
        public Engine.Plugin.Forms.Worldspace    Worldspace
        {
            get
            {
                return _worldspace;
            }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.Worldspace.Set() :: value = " + value );
                if( value == _worldspace ) return;
                
                SyncSceneUpdate( true );
                
                if( value == null )
                {
                    _worldspace = null;
                    _poolEntry = null;
                    UpdateCellClipper(
                        new Vector2i( -2,  2 ),
                        new Vector2i(  2, -2 ),
                        false );
                }
                else
                {
                //    worldspace.DestroySurfaces();
                    _worldspace = value;
                    _poolEntry = _worldspace.PoolEntry;
                    var mapData = _worldspace.MapData;
                    UpdateCellClipper(
                        mapData.GetCellNW( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                        mapData.GetCellSE( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                        false );
                }
                SetViewCentre( WorldspaceClipperCentre(), false );
                ReloadWorldspaceTextures( false );
                
                SyncSceneUpdate( false );
            }
        }
        
        /*
        public ImportMod                        ImportMod
        {
            get
            {
                return _importMod;
            }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.ImportMod.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _importMod = value;
                SyncSceneUpdate( false );
            }
        }
        */
        
        public Maths.Vector2i                   CellNW                  { get { return _cellNW; } }
        public Maths.Vector2i                   CellSE                  { get { return _cellSE; } }
        
        public List<WorkshopScript>             Workshops
        {
            get { return _workshops; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.Workshops.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _workshops = value;
                SyncSceneUpdate( false );
            }
        }
        
        public List<Settlement>                 Settlements
        {
            get { return _settlements; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.Settlements.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _settlements = value;
                SyncSceneUpdate( false );
            }
        }
        
        public List<SubDivision>                SubDivisions
        {
            get { return _subdivisions; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.SubDivisions.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _subdivisions = value;
                _associatedEdgeFlags = null;
                _edgeLines = null;
                SyncSceneUpdate( false );
            }
        }
        
        public List<EdgeFlag>                   AssociatedEdgeFlags
        {
            get { return _associatedEdgeFlags; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.AssociatedEdgeFlags.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _associatedEdgeFlags = value;
                SyncSceneUpdate( false );
            }
        }
        
        public List<EdgeFlag>                   UnassociatedEdgeFlags
        {
            get { return _unassociatedEdgeFlags; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.UnassociatedEdgeFlags.Set() :: value = " + value );
                SyncSceneUpdate( true );
                _unassociatedEdgeFlags = value;
                SyncSceneUpdate( false );
            }
        }
        
        public List<SubDivision>                HighlightSubDivisions
        {
            get { return hlSubDivisions; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.HighlightSubDivisions.Set() :: value = " + value );
                SyncSceneUpdate( true );
                hlSubDivisions = value;
                SyncSceneUpdate( false );
            }
        }
        
        public List<Volume>                     HighlightVolumes
        {
            get { return hlVolumes; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.HighlightVolumes.Set() :: value = " + value );
                SyncSceneUpdate( true );
                hlVolumes = value;
                SyncSceneUpdate( false );
            }
        }
        
        #endregion
        
        #region Scale Calculator
        
        public float CalculateScale( Maths.Vector2i cells )
        {
            var scaleNS = (float)rectTarget.h / ( (float)cells.Y * Engine.Constant.WorldMap_Resolution );
            var scaleEW = (float)rectTarget.w / ( (float)cells.X * Engine.Constant.WorldMap_Resolution );
            var scale = scaleNS < scaleEW ? scaleNS : scaleEW;
            return scale;
        }
        
        public static float CalculateScale( Maths.Vector2i cells, Maths.Vector2i worldCells )
        {
            var scaleNS = (float)( (float)cells.Y / (float)worldCells.Y );
            var scaleEW = (float)( (float)cells.X / (float)worldCells.X );
            var scale = Lerps.InverseLerp( Constant.MinZoom, Constant.MaxZoom, scaleNS > scaleEW ? scaleNS : scaleEW );
            return scale;
        }
        
        #endregion
        
        #region Screenspace (Window) transforms
        
        #region Screenspace <-> Worldspace
        
        public Maths.Vector2f ScreenspaceToWorldspace( float x, float y )
        {
            Maths.Vector2f r = new Maths.Vector2f( x - _trueCentre.X, -y + _trueCentre.Y );
            r *= _invScale;
            r += _viewCentre;
            return r;
        }
        
        Maths.Vector2f WorldspaceToScreenspace( float x, float y )
        {
            x -= _viewCentre.X;
            y -= _viewCentre.Y;
            x *= _scale;
            y *= _scale;
            x += _trueCentre.X;
            y = -y + _trueCentre.Y;
            return new Vector2f( x, y );
            /*
            Maths.Vector2f r = new Maths.Vector2f( x, y ) - this._viewCentre;
            r *= _scale;
            r.X +=       _trueCentre.X;
            r.Y = -r.Y + _trueCentre.Y;
            return r;
            */
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
            var ss = new Maths.Vector2f( rectTarget.w, rectTarget.h ) * _invScale;
            var hss = ss * 0.5f;
            var nw = Engine.SpaceConversions.WorldspaceToHeightmap( x - hss.X, y + hss.Y, hmCentre );
            var s = new Vector2i(
                (int)( ss.X * Engine.Constant.WorldMap_To_Heightmap ),
                (int)( ss.Y * Engine.Constant.WorldMap_To_Heightmap ) );
            return new Rectangle(
                nw.X, nw.Y,
                s.X, s.Y
            );
        }
        
        #endregion
        
        #region Screenspace <-> CellGrid
        
        public Maths.Vector2i ScreenspaceToCellGrid( float x, float y )
        {
            return ScreenspaceToWorldspace( x, y ).WorldspaceToCellGrid();
        }
        
        public Maths.Vector2f CellGridToScreenspace( int x, int y )
        {
            return WorldspaceToScreenspace( Engine.SpaceConversions.CellGridToWorldspace( x, y ) );
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
        
        void ReloadObjectTexture( bool forceReload, string path, ref SDLRenderer.Texture sdltex, Color c )
        {
            if( ( !forceReload )&&( sdltex != null ) )
                return;
            
            if( sdltex != null )
            {
                sdltex.Dispose();
                sdltex = null;
            }
            var surf = sdlRenderer.LoadSurface( path );
            if( surf != null )
            {
                sdltex = sdlRenderer.CreateTextureFromSurface( surf );
                if( sdltex != null )
                    sdltex.ColorMod = c;
            }
            surf.Dispose();
            surf = null;
        }
        
        void ReloadObjectTextures( bool forceReload )
        {
            //DebugLog.Write( "GUIBuilder.RenderTransform.ReloadObjectTextures()" );
            if(
                ( sdlRenderer == null )||
                ( !sdlRenderer.IsReady )
            ) return;
            var path_x128 = GodObject.Paths.RenderTransformImage + @"\X_128.png";
            var path_c128 = GodObject.Paths.RenderTransformImage + @"\Circle_128.png";
            var path_ttf  = GodObject.Paths.RenderTransformImage + @"\tahoma.ttf";
            ReloadObjectTexture( forceReload, path_x128, ref _st_Red_X                  , Color.FromArgb( 255, 255,   0,   0 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_Settlement             , Color.FromArgb( 255, 255, 240,   0 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_SubDivision            , Color.FromArgb( 255, 102,   0, 255 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_BorderEnabler          , Color.FromArgb( 255,   6, 255,   0 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_Workshop               , Color.FromArgb( 255, 255,  32,  32 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_EdgeFlag               , Color.FromArgb( 255, 181,  68,   0 ) );
            ReloadObjectTexture( forceReload, path_c128, ref _st_UnassociatedEdgeFlag   , Color.FromArgb( 255,  91,  64,   0 ) );
            _sf_Tahoma    = sdlRenderer.CreateFont( 10, path_ttf );
        }
        
        void ReloadWorldspaceTextures( bool forceReload )
        {
            //DebugLog.Write( string.Format( "\n----->\n{0} :: ReloadWorldspaceTextures() :: Start :: forceReload = {1}\n{2}", this.GetType().ToString(), forceReload, System.Environment.StackTrace ) );
            if( _poolEntry == null )
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.ReloadWorldspaceTextures() :: Worldspace PoolEntry is null!\n<-----" );
                return;
            }
            if(
                ( sdlRenderer == null )||
                ( !sdlRenderer.IsReady )
            )
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.ReloadWorldspaceTextures() :: SDLRenderer is null or not ready!\n<-----" );
                return;
            }
            if(
                ( forceReload )||
                (
                    ( _renderLand )&&
                    ( !_poolEntry.TexturesReady )
                )
            ) {
                _poolEntry.CreateHeightmapTextures( this );
            }
            //DebugLog.Write( string.Format( "\n{0} :: ReloadWorldspaceTextures() :: Complete :: forceReload = {1}\n{2}\n<-----", this.GetType().ToString(), forceReload, System.Environment.StackTrace ) );
        }
        
        #region Render Scene
        
        #region Render Controls
        
        public bool                             RenderFast
        {
            get { return _renderFast; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderFast :: value = " + value );
                SyncSceneUpdate( true );
                _renderFast = value;
                sdlRenderer.PreserveUserState = !value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderCellGrid
        {
            get { return _renderCellGrid; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderCellGrid :: value = " + value );
                SyncSceneUpdate( true );
                _renderCellGrid = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderLand
        {
            get { return _renderLand; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderLand :: value = " + value );
                SyncSceneUpdate( true );
                _renderLand = value;
                ReloadWorldspaceTextures( false );
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderWater
        {
            get { return _renderWater; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderWater :: value = " + value );
                SyncSceneUpdate( true );
                _renderWater = value;
                ReloadWorldspaceTextures( false );
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderWorkshops
        {
            get { return _renderWorkshops; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderWorkshops :: value = " + value );
                SyncSceneUpdate( true );
                _renderWorkshops = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderSettlements
        {
            get { return _renderSettlements; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderSettlements :: value = " + value );
                SyncSceneUpdate( true );
                _renderSettlements = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderSubDivisions
        {
            get { return _renderSubDivisions; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderSubDivisions :: value = " + value );
                SyncSceneUpdate( true );
                _renderSubDivisions = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderEdgeFlags
        {
            get { return _renderEdgeFlags; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderEdgeFlags :: value = " + value );
                SyncSceneUpdate( true );
                _renderEdgeFlags = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderEdgeFlagLinks
        {
            get { return _renderEdgeFlagLinks; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderEdgeFlags :: value = " + value );
                SyncSceneUpdate( true );
                _renderEdgeFlagLinks = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderBuildVolumes
        {
            get { return _renderBuildVolumes; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderBuildVolumes :: value = " + value );
                SyncSceneUpdate( true );
                _renderBuildVolumes = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderSandboxVolumes
        {
            get { return _renderSandboxVolumes; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderSandboxVolumes :: value = " + value );
                SyncSceneUpdate( true );
                _renderSandboxVolumes = value;
                SyncSceneUpdate( false );
            }
        }
        
        public bool                             RenderBorders
        {
            get { return _renderBorders; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.RenderBorders :: value = " + value );
                SyncSceneUpdate( true );
                _renderBorders = value;
                SyncSceneUpdate( false );
            }
        }
        
        public float                            MinRenderScaledSettlementObjects
        {
            get { return _minRenderScaledSettlementObjects; }
            set
            {
                //DebugLog.Write( "GUIBuilder.RenderTransform.MinRenderScaledSettlementObjects :: value = " + value );
                SyncSceneUpdate( true );
                _minRenderScaledSettlementObjects = value;
                SyncSceneUpdate( false );
            }
        }
        
        #endregion
        
        public void UpdateScene( //bool renderLand, bool renderWater, bool renderCellGrid, bool renderBuildVolumes, bool renderBorders, bool renderHelperFlags, bool fastRender )
            bool fastRender,
            bool renderCellGrid,
            bool renderLand,
            bool renderWater,
            bool renderWorkshops,
            bool renderSettlements,
            bool renderSubdivisions,
            bool renderEdgeFlags,
            bool renderEdgeFlagLinks,
            bool renderBuildVolumes,
            bool renderSandboxVolumes,
            bool renderBorders )
        {
            /*
            DebugLog.Write(
                string.Format(
                    "\n{0} :: UpdateScene()\n\tfastRender = {1}\n\trenderCellGrid = {2}\n\trenderLand = {3}\n\trenderWater = {4}\n\trenderWorkshops = {5}\n\trenderSettlements = {6}\n\trenderSubDivisions = {7}\n\trenderEdgeFlags = {8}\n\trenderEdgeFlagLinks = {9}\n\trenderBuildVolumes = {10}\n\trenderSandboxVolumes = {11}\n\trenderBorders = {12}",
                    this.GetType().ToString(),
                    fastRender, renderCellGrid,
                    renderLand, renderWater,
                    renderWorkshops, renderSettlements, renderSubdivisions,
                    renderEdgeFlags, renderEdgeFlagLinks,
                    renderBuildVolumes, renderSandboxVolumes,
                    renderBorders ) );
            */
            
            SyncSceneUpdate( true );
            
            _renderFast = fastRender;
            _renderCellGrid = renderCellGrid;
            _renderLand = renderLand;
            _renderWater = renderWater;
            _renderWorkshops = renderWorkshops;
            _renderSettlements = renderSettlements;
            _renderSubDivisions = renderSubdivisions;
            _renderEdgeFlags = renderEdgeFlags;
            _renderEdgeFlagLinks = renderEdgeFlagLinks;
            _renderBuildVolumes = renderBuildVolumes;
            _renderSandboxVolumes = renderSandboxVolumes;
            _renderBorders = renderBorders;
            sdlRenderer.PreserveUserState = !fastRender;
            ReloadWorldspaceTextures( false );
            
            SyncSceneUpdate( false );
        }
        
        void SDL_DrawScene( SDLRenderer renderer )
        {
            if( renderer != sdlRenderer ) return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "SDL_DrawScene()" } );
            
            SyncRenderScene( true );
            
            ReloadObjectTextures( false );
            
            if( ( _poolEntry != null )&&( _poolEntry.TexturesReady ) )
            {
                
                if( _renderLand )
                    DrawLandMap();
                
                if( _renderWater )
                    DrawWaterMap();
            }
            
            if( _renderCellGrid )
                DrawCellGrid();
            
            if( _renderBuildVolumes )
                DrawBuildVolumes();
            
            if( _renderSandboxVolumes )
                DrawSandboxVolumes();
            
            if( _renderEdgeFlagLinks )
                DrawEdgeFlagLinks();
            
            if( _renderBorders )
                DrawBorders();
            
            //RenderRenderObjects();
            
            if( _renderEdgeFlags )
                DrawEdgeFlags();
            
            if( _renderWorkshops )
                DrawWorkshops();
            
            if( _renderSubDivisions )
                DrawSubDivisions();
            
            if( _renderSettlements )
                DrawSettlements();
            
            //if( attachedEditor != null )
            //    attachedEditor.DrawEditor();
            
            DrawMouseOver();
            DrawMouse();
            
            SyncRenderScene( false );
            
            //DebugLog.CloseIndentLevel();
        }
        
        public void RenderToPNG()
        {
            // Export the [whole] map to a PNG
            if(
                ( !_subdivisions.NullOrEmpty() )&&
                ( _subdivisions.Count > 1 )
               ) return;
            var filename = _subdivisions.NullOrEmpty() ? _worldspace.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) : _subdivisions[ 0 ].GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) + ".png";
            //bmpTarget.Save( filename, System.Drawing.Imaging.ImageFormat.Png );
            // TODO:  Implement SDLRenderer PNG Save
        }
        
        #endregion
        
        // Note:  Certain functions will try/catch and supress exceptions so the render
        // window can be the loser in the case of resource contention. ie:  If objects
        // are being generated and the render window tries to draw them at the same time.
        // This should allow the important thread (the one doing real work) to continue
        // while the renderer will just skip the object for the frame due to the exception.
        
        #region Mouse/Over Rendering
        
        List<IMouseOver> _mouseOverObjects = null;
        List<string> _mouseOverInfo = null;
        
        static void AddMouseOverFrom<T>( ref List<IMouseOver> result, List<T> mouseOverObjects, Maths.Vector2f mouse, float maxDistance ) where T : Engine.Plugin.PapyrusScript
        {
            if( mouseOverObjects.NullOrEmpty() )
                return;
            var moos = new List<IMouseOver>();
            foreach( var moo in mouseOverObjects )
            {
                var refr = moo.Reference;
                if( refr.IsMouseOver( mouse, maxDistance ) )
                    moos.AddOnce( refr );
            }
            if( moos.Count == 0 )
                return;
            if( result == null )
                result = new List<IMouseOver>();
            result.AddOnce( moos );
        }
        
        public List<IMouseOver> GetMouseOverObjects()
        {
            return _mouseOverObjects;
        }
        
        bool UpdateMouseOver()
        {
            var newMoos = (List<IMouseOver>)null;
            float minDistance = _minRenderScaledSettlementObjects / _scale;
            float maxDistance = Math.Max( 64.0f, minDistance );
            var mwpos = ScreenspaceToWorldspace( (float)_mousePos.X, (float)_mousePos.Y );
            if( _renderSettlements )
                AddMouseOverFrom( ref newMoos, _settlements, mwpos, maxDistance );
            if( _renderSubDivisions )
                AddMouseOverFrom( ref newMoos, _subdivisions, mwpos, maxDistance );
            if( _renderWorkshops )
                AddMouseOverFrom( ref newMoos, _workshops, mwpos, maxDistance );
            if( _renderBorders )
                foreach( var s in _subdivisions )
                    AddMouseOverFrom( ref newMoos, s.BorderEnablers, mwpos, maxDistance );
            if( _renderEdgeFlags )
            {
                AddMouseOverFrom( ref newMoos, _associatedEdgeFlags, mwpos, maxDistance );
                AddMouseOverFrom( ref newMoos, _unassociatedEdgeFlags, mwpos, maxDistance );
            }
            
            var hasMoos = !_mouseOverObjects.NullOrEmpty();
            if( newMoos.NullOrEmpty() )
            {
                _mouseOverObjects = null;
                _mouseOverInfo = null;
                return hasMoos;
            }
            
            if(
                ( hasMoos )&&
                ( _mouseOverObjects.Count == newMoos.Count )&&
                ( _mouseOverObjects.ContainsAllElementsOf( newMoos ) )
            )
                return false;
            
            var moil = new List<string>();
            foreach( var newMoo in newMoos )
                moil.AddAll( newMoo.MouseOverInfo );
            
            _mouseOverInfo = moil;
            return true;
        }
        
        void DrawMouseOver()
        {
            if( _mouseOverInfo.NullOrEmpty() )
                return;
            
            DrawMouseOverInfo();
        }
        
        void DrawMouse()
        {
            var c = Color.FromArgb( 255, 255, 255, 255 );
            sdlRenderer.DrawCircle( _mousePos.X, _mousePos.Y, 8, c );
            sdlRenderer.DrawLine( _mousePos.X - 12, _mousePos.Y     , _mousePos.X + 12, _mousePos.Y     , c );
            sdlRenderer.DrawLine( _mousePos.X     , _mousePos.Y - 12, _mousePos.X     , _mousePos.Y + 12, c );
        }
        
        public void DumpMouseOverInfo()
        {
            if( _mouseOverInfo.NullOrEmpty() ) return;
            
            var s = _mouseOverInfo.Clone();
            s.Insert( 0, "\n--------8<--------8<--------8<--------8<--------\n" );// Only people who ever used uncut spool printers will get this
            
            System.IO.File.AppendAllLines( "MouseOverInfo.txt", s );
        }
        
        #endregion
        
        #region Drawing Primitives (worldspace to transform target)
        
        public void DrawTextWorldTransform( string text, float x, float y, Color c, SDLRenderer.Font font, int style = 0 )
        {
           var tp = WorldspaceToScreenspace( x, y ).ToSDLPoint( !_renderFast );
           sdlRenderer.DrawText( tp, font, text, c, style );
        }
        
        /*
        public void DrawCircleWorldTransform( float x, float y, float r, Color c, float minScaledR = 0.0f )
        {
            if( minScaledR < 0.0f )
                minScaledR = 0.0f;
            if( minScaledR > r )
                minScaledR = r;
            r -= minScaledR;
            var sr = _renderFast ? (int)( r * _scale ) : (int)Math.Round( r * _scale );
            sr += (int)minScaledR;
            var tp = WorldspaceToScreenspace( x, y ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawCircle( tp, sr, c );
        }
        
        public void DrawFilledCircleWorldTransform( float x, float y, float r, Color c, float minScaledR = 0.0f )
        {
            if( minScaledR < 0.0f )
                minScaledR = 0.0f;
            if( minScaledR > r )
                minScaledR = r;
            r -= minScaledR;
            var sr = _renderFast ? (int)( r * _scale ) : (int)Math.Round( r * _scale );
            sr += (int)minScaledR;
            var tp = WorldspaceToScreenspace( x, y ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawFilledCircle( tp, sr, c );
        }
        */
        
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
        
        public void DrawFilledRectWorldTransform( Maths.Vector2f p0, Maths.Vector2f p1, Color c )
        {
            var tp0 = WorldspaceToScreenspace( p0 ).ToSDLPoint( !_renderFast );
            var tp1 = WorldspaceToScreenspace( p1 ).ToSDLPoint( !_renderFast );
            sdlRenderer.DrawFilledRect( tp0.x, tp0.y, tp1.x, tp1.y, c );
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
        
        static SDL.SDL_Rect _dstRect;
        public void DrawTextureWorldTransform( float x, float y, SDLRenderer.Texture sdltex )
        {
            var tp = WorldspaceToScreenspace( x, y );//.ToSDLPoint( !_renderFast );
            var ts = (float)sdltex.Width;
            ts -= _minRenderScaledSettlementObjects;
            var st = _renderFast ? (int)( ts * _scale ) : (int)Math.Round( ts * _scale );
            st += (int)_minRenderScaledSettlementObjects;
            var hst = st >> 1;
            _dstRect.x = (int)( tp.X - hst );
            _dstRect.y = (int)( tp.Y - hst );
            _dstRect.w = st;
            _dstRect.h = st;
            sdlRenderer.Blit( _dstRect, sdltex );
        }
        
        #endregion
        
        void DrawMouseOverInfo()
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawMouseOverInfo()" } );
            
            var mp = new Vector2i( _mousePos );
            var moi = new List<string>();
            foreach( var moil in _mouseOverInfo )
                moi.Add( moil.Replace( "\t", "   " ) );
            
            var lineCount = moi.Count;
            int tallest = 0;
            var stringSize = moi.SizeOfLines( _sf_Tahoma, out tallest ) + ( _textBoxPadding * 2 );
            var brc = mp + stringSize;
            
            /* mp = 250, 100
             * stringSize = 400, 500
             * brc = 650, 600
             * ws = 600, 400
             * oobbr = -50, -200
            */
            var oobbr = sdlRenderer.WindowSize.ToVector2i() - brc;
            
            if( oobbr.X <= 0 )
            {
                oobbr.X -= 1;
                mp.X += oobbr.X;
                brc.X += oobbr.X;
            }
            if( oobbr.Y <= 0 )
            {
                oobbr.Y -= 1;
                mp.Y += oobbr.Y;
                brc.Y += oobbr.Y;
            }
            
            if( mp.X < 0 )
            {
                brc.X -= mp.X;
                mp.X = 0;
            }
            if( mp.Y < 0 )
            {
                brc.Y -= mp.Y;
                mp.Y = 0;
            }
            
            sdlRenderer.DrawFilledRect( mp.X, mp.Y, brc.X, brc.Y, Color.Gray     );
            sdlRenderer.DrawRect      ( mp.X, mp.Y, brc.X, brc.Y, Color.DarkGray );
            
            mp += _textBoxPadding;
            for( int i = 0; i < lineCount; i++ )
            {
                //DebugLog.WriteLine( moi[ i ] );
                sdlRenderer.DrawText( mp.X, mp.Y, _sf_Tahoma, moi[ i ], Color.White );
                mp.Y += tallest;
            }
            
            //DebugLog.CloseIndentLevel();
        }
        
        #region Heightmap Drawing
        
        void DrawLandMap()
        {
            if( _poolEntry.LandHeight_Texture == null ) return;
            //DebugLog.WriteLine( new [] { this.GetType().ToString(), "DrawLandMap()" } );
            var hmClipper = WorldspaceToHeightmapClipper( _viewCentre.X, _viewCentre.Y );
            var rect = hmClipper.ToSDLRect();
            sdlRenderer.Blit( rectTarget, _poolEntry.LandHeight_Texture, rect );
        }
        
        void DrawWaterMap()
        {
            if( _poolEntry.WaterHeight_Texture == null ) return;
            //DebugLog.WriteLine( new [] { this.GetType().ToString(), "DrawWaterMap()" } );
            var hmClipper = WorldspaceToHeightmapClipper( _viewCentre.X, _viewCentre.Y );
            var rect = hmClipper.ToSDLRect();
            sdlRenderer.Blit( rectTarget, _poolEntry.WaterHeight_Texture, rect );
        }
        
        #endregion
        
        #region Build Volumes Drawing
        
        void DrawBuildVolume( SubDivision subdivision )
        {
            // Only show build volumes for the appropriate worldspace
            if( ( _worldspace == null )||( subdivision == null ) )
                return;
            //if( _worldspace.FormID != subdivision.Reference.Worldspace.FormID )
            //    return;
            
            var volumes = subdivision.BuildVolumes;
            if( volumes.NullOrEmpty() )
                return;
            
            var wsFID = _worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            try
            {
                
                #if DEBUG
                if(
                    ( !debugRenderBuildVolumes )//||
                    //( subdivision.BorderSegments.NullOrEmpty() )
                )
                #endif
                {
                    int rb = 223;
                    var cHigh = Color.FromArgb( 255, rb, 0, rb );
                    rb -= 48;
                    var cMid = Color.FromArgb( 255, rb, 0, rb );
                    rb -= 48;
                    var cLow = Color.FromArgb( 255, rb, 0, rb );
                    
                    var editorEnabled = false;//( attachedEditor != null )&&( attachedEditor.Enabled );
                    
                    foreach( var volume in volumes )
                    {
                        if( wsFID != volume.Reference.Worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) )
                            continue;
                        
                        var useColor = editorEnabled ? cMid : cLow;
                        if( !editorEnabled )
                        {
                            if( hlSubDivisions.NullOrEmpty() )
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
                            else if( hlSubDivisions.Contains( subdivision ) )
                            {
                                useColor = cMid;
                            }
                        }
                        
                        DrawPolyWorldTransform( volume.Corners, useColor );
                    }
                }
                #if DEBUG
                /*
                else
                {
                    int r = 255;
                    int g = 0;
                    int b = 127;
                    SDLRenderer.Font font = null; // TODO: FILL THIS!
                    
                    for( var i = 0; i < subdivision.BorderSegments.Count; i++ )
                    {
                        var segment = subdivision.BorderSegments[ i ];
                        
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
                */
                #endif
            }
            // disable once EmptyGeneralCatchClause
            catch {}
        }
        
        void DrawBuildVolumes()
        {
            if( ( _worldspace == null )||( _subdivisions.NullOrEmpty() ) )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawBuildVolumes()" } );
            
            if( hlSubDivisions.NullOrEmpty() )
                foreach( var subdivision in _subdivisions )
                    DrawBuildVolume( subdivision );
            else
            {
                foreach( var subdivision in _subdivisions )
                    if( !hlSubDivisions.Contains( subdivision ) )
                        DrawBuildVolume( subdivision );
                
                foreach( var subdivision in hlSubDivisions )
                    DrawBuildVolume( subdivision );
            }
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Sandbox Volumes Drawing
        
        void DrawSandboxVolume( SubDivision subdivision )
        {
            // Only show sandbox volumes for the appropriate worldspace
            if( ( _worldspace == null )||( subdivision == null ) )
                return;
            //if( _worldspace.FormID != subdivision.Reference.Worldspace.FormID )
            //    return;
            
            try
            {
                
                var volume = subdivision.SandboxVolume;
                if( volume == null )
                    return;
                
                var wsFID = _worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                if( wsFID != volume.Reference.Worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) )
                    return;
                
                var c = Color.FromArgb( 255, 204, 76, 51 );
                
                DrawPolyWorldTransform( volume.Corners, c );
            }
            // disable once EmptyGeneralCatchClause
            catch {}
        }
        
        void DrawSandboxVolumes()
        {
            if( ( _worldspace == null )||( _subdivisions.NullOrEmpty() ) )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawSandboxVolumes()" } );
            
            foreach( var subdivision in _subdivisions )
                DrawSandboxVolume( subdivision );
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Edge Flags Drawing
        
        void RebuildAssociatedEdgeFlags()
        {
            if(
                ( _subdivisions.NullOrEmpty() )||
                ( !_associatedEdgeFlags.NullOrEmpty() )||
                ( !_edgeLines.NullOrEmpty() )
               ) return;
            
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "RebuildAssociatedEdgeFlags()" } );
            
            try
            {
                _associatedEdgeFlags = new List<EdgeFlag>();
                _edgeLines = new List<EdgeLine>();
                foreach( var s in _subdivisions )
                {
                    var flags = s.EdgeFlags;
                    if( !flags.NullOrEmpty() )
                    {
                        var sefk = s.EdgeFlagKeyword;
                        foreach( var flag in flags )
                        {
                            _associatedEdgeFlags.AddOnce( flag );
                            var refr = flag.Reference;
                            var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                            var lrfs = refr.LinkedRefs;
                            var count = lrfs.Count;
                            for( int i = 0; i < count; i++ )
                            {
                                var lkFID = lrfs.KeywordFormID[ i ];
                                if( GodObject.CoreForms.IsSubDivisionEdgeFlagKeyword( lkFID ) )
                                {
                                    var lref = lrfs.Reference[ i ];
                                    var lef = lref.GetScript<EdgeFlag>();
                                    if( ( lef != null )&&( lef.AssociatedWithSubDivision( s ) ) )
                                    {
                                        var el = new EdgeLine(
                                            s,
                                            pos,
                                            lref.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                                            sefk.GetColor( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                                        _edgeLines.Add( el );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                _associatedEdgeFlags = null;
                _edgeLines = null;
            }
            DebugLog.CloseIndentLevel();
        }
        
        void DrawEdgeFlags()
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawEdgeFlags()" } );
            
            RebuildAssociatedEdgeFlags();
            
            //DebugLog.Write( string.Format( "DrawEdgeFlags() - AssociatedEdgeFlags.Length = {0}", _associatededgeFlags.Count ) );
            if( !_associatedEdgeFlags.NullOrEmpty() )
            {
                try
                {
                    foreach( var flag in _associatedEdgeFlags )
                    {
                        var refr = flag.Reference;
                        var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        DrawTextureWorldTransform( pos.X, pos.Y, _st_EdgeFlag );
                    }
                }
                // disable once EmptyGeneralCatchClause
                catch {}
            }
            
            //DebugLog.Write( string.Format( "DrawEdgeFlags() - UnassociatedEdgeFlags.Length = {0}", _unassociatededgeFlags.Count ) );
            if( !_unassociatedEdgeFlags.NullOrEmpty() )
            {
                try
                {
                    foreach( var flag in _unassociatedEdgeFlags )
                    {
                        var refr = flag.Reference;
                        var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        DrawTextureWorldTransform( pos.X, pos.Y, _st_UnassociatedEdgeFlag );
                        DrawTextureWorldTransform( pos.X, pos.Y, _st_Red_X );
                    }
                }
                // disable once EmptyGeneralCatchClause
                catch {}
            }
            
            //DebugLog.CloseIndentLevel();
        }
        
        void DrawEdgeFlagLinks()
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawEdgeFlagLinks()" } );
            
            RebuildAssociatedEdgeFlags();
            
            //DebugLog.Write( string.Format( "DrawEdgeFlagLinks() - EdgeLines.Length = {0}", _edgeLines.Count ) );
            if( _edgeLines.NullOrEmpty() )
                return;
                //goto localReturnResult;
            
            foreach( var el in _edgeLines )
                DrawLineWorldTransform( el.p0, el.p1, el.c );
            
        //localReturnResult:
        //    DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Workshops Drawing
        
        void DrawWorkshops()
        {
            if( _workshops.NullOrEmpty() )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawWorkshops()" } );
            
            try
            {
                var c = _workshops.Count;
                for( int i = 0; i < c; i++ )
                {
                    var workshop = _workshops[ i ];
                    var refr = workshop.Reference;
                    var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    DrawTextureWorldTransform( pos.X, pos.Y, _st_Workshop );
                }
            }
            // disable once EmptyGeneralCatchClause
            catch {}
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Sub-Division Drawing
        
        void DrawSubDivisions()
        {
            if( _subdivisions.NullOrEmpty() )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawSubDivisions()" } );
            
            try
            {
                foreach( var subdivision in _subdivisions )
                {
                    var refr = subdivision.Reference;
                    var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    DrawTextureWorldTransform( pos.X, pos.Y, _st_SubDivision );
                }
            }
            // disable once EmptyGeneralCatchClause
            catch {}
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Settlement Drawing
        
        void DrawSettlements()
        {
            if( _settlements.NullOrEmpty() )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawSettlements()" } );
            
            try
            {
                foreach( var settlement in _settlements )
                {
                    var refr = settlement.Reference;
                    var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    DrawTextureWorldTransform( pos.X, pos.Y, _st_Settlement );
                }
            }
            // disable once EmptyGeneralCatchClause
            catch {}
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Border Drawing
        
        /*
        void DrawBorder( SubDivision subdivision )
        {
            // Only show build volumes for the appropriate worldspace
            if( ( _worldspace == null )||( subdivision == null ) )
                return;
            
            if( ( _worldspace.FormID != subdivision.Reference.Worldspace.FormID )||( subdivision.BorderNodes == null ) )
                return;
            
            var editorEnabled = ( attachedEditor != null )&&( attachedEditor.Enabled );
            
            int g = 255;
            if(
                ( !editorEnabled )&&
                ( !hlSubDivisions.NullOrEmpty() )&&
                ( !hlSubDivisions.Contains( subdivision ) )
            )   g >>= 1;
            
            var c = Color.FromArgb( 255, 0, g, 0 );
            
            #if DEBUG
            SDLRenderer.Font font = null; // TODO: FILL THIS!
            #endif
            
            for( var i = 0; i < subdivision.BorderNodes.Count; i++ )
            {
                var node = subdivision.BorderNodes[ i ];
                
                // *
                if(
                    ( !hlVolumes.NullOrEmpty() )&&
                    ( hlVolumes.Contains( node.Volume ) )
                )   g >>= 1;
                //* /
                
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
        */
        
        void DrawBorders()
        {
            if( ( _worldspace == null )||( _subdivisions.NullOrEmpty() ) )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawBorders()" } );
            
            var wsFID = _worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            var c = Color.FromArgb( 255, 0, 255, 0 );
            
            try
            {
                foreach( var subdivision in _subdivisions )
                {
                    var enablers = subdivision.BorderEnablers;
                    if( enablers.NullOrEmpty() )
                        continue;
                    
                    foreach( var enabler in enablers )
                    {
                        var refr = enabler.Reference;
                        if( refr.Worldspace == null )
                        {
                            DebugLog.WriteError( this.GetType().ToString(), "DrawBorders()", string.Format( "Worldspace == null\nCell = {0}\nBorderEnabler = {1}", refr.Cell.ToString(), enabler.ToString() ) );
                            continue;
                        }
                        
                        if( wsFID != refr.Worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) )
                            continue;
                        
                        var pos = refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        DrawTextureWorldTransform( pos.X, pos.Y, _st_BorderEnabler );
                        
                        var segs = enabler.Segments;
                        if( segs.NullOrEmpty() )
                            continue;
                        
                        for( int si = 0; si < segs.Count ; si++ )
                        {
                            var seg = segs[ si ];
                            var flags = seg.Flags;
                            for( int i = 0; i < flags.Count - 1; i++ )
                            {
                                var p0 = flags[ i     ].Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                                var p1 = flags[ i + 1 ].Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                                DrawLineWorldTransform( p0.X, p0.Y, p1.X, p1.Y, c );
                            }
                        }
                    }
                }
            }
            // disable once EmptyGeneralCatchClause
            catch {}
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Cell Grid Drawing
        
        public void DrawCellGrid()
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "DrawCellGrid()", "NW = "+ _cellNW.ToString(), "SE = " + _cellSE.ToString() } );
            
            var c0 = Color.FromArgb( 127, 63, 63, 191 );
            var c = Color.FromArgb( 127, 91, 91, 0 );
            
            var minX =   _cellNW.X       * Engine.Constant.WorldMap_Resolution;
            var maxX = ( _cellSE.X + 1 ) * Engine.Constant.WorldMap_Resolution;
            var minY =   _cellSE.Y       * Engine.Constant.WorldMap_Resolution;
            var maxY = ( _cellNW.Y + 1 ) * Engine.Constant.WorldMap_Resolution;
            
            for( int y = _cellSE.Y; y <= _cellNW.Y + 1; y++ )
            {
                var uc = y == 0 ? c0 : c;
                var yp = y * Engine.Constant.WorldMap_Resolution;
                DrawLineWorldTransform( minX, yp, maxX, yp, uc );
            }
            for( int x = _cellNW.X; x <= _cellSE.X + 1; x++ )
            {
                var uc = x == 0 ? c0 : c;
                var xp = x * Engine.Constant.WorldMap_Resolution;
                DrawLineWorldTransform( xp, minY, xp, maxY, uc );
            }
            
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
    }
}
