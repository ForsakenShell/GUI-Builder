/*
 * WorldspaceDataPool.cs
 *
 * Worldspace heightmap data.
 * 
 * This is stored separately from the worldspace itself so that the heightmap
 * isn't loaded multiple times for each override of a given worldspace.
 *
 */
//#define HEIGHT_MAP_AT_POS_LERP          // Quad Lerp or;
#define HEIGHT_MAP_AT_POS_INTERSECT     // Triangle Intersect

using System;
using System.Collections.Generic;
using System.IO;

using Maths;

using SDL2;
using SDL2ThinLayer;

using XeLib;
using XeLib.API;

using GUIBuilder;

using Engine.Plugin.Forms;

namespace GodObject
{
    /// <summary>
    /// Description of WorldspaceDataPool.
    /// </summary>
    public static class WorldspaceDataPool
    {
        
        const string LandHeight_Texture_File_Suffix = "_LandHeights.dds";
        const string WaterHeight_Texture_File_Suffix = "_WaterHeights.dds";
        
        // User feedback strings in the status area
        static readonly string txtCreateTexturesNP = "WorldspaceDataPool.CreatingTextures".Translate();
        static readonly string txtCreateTextures = "WorldspaceDataPool.CreatingTexturesP".Translate();
        
        public class PoolEntry : IDisposable
        {
            
            Worldspace _Worldspace = null;
            
            //public string LandHeights_Texture_File = null;
            //public string WaterHeights_Texture_File = null;
            //public string Stats_File = null;
            
            // Stats file data
            //public float MaxHeight = 0.0f;
            //public float MinHeight = 0.0f;
            //public float DeltaHeight = 0.0f;
            
            // Height map data (extracted from DDS)
            // Height map data is now extracted from the CELLs themselves
            //public int HeightMap_Width = 0;
            //public int HeightMap_Height = 0;
            //public float[,] LandHeightMap = null;
            //public float[,] WaterHeightMap = null;
            //public bool[,] CellLoaded = null;
            
            // Surfaces and Textures for rendering
            SDLRenderer.Surface LandHeight_Surface;
            SDLRenderer.Surface WaterHeight_Surface;
            public SDLRenderer.Texture LandHeight_Texture;
            public SDLRenderer.Texture WaterHeight_Texture;
            
            //GUIBuilder.Windows.RenderChild.RenderTransform _textureLoadQueuetransform = null;
            //bool _textureLoadQueued = false;
            bool _texturesReady = false;
            public bool TexturesReady            { get { return _texturesReady; } }
            
            #region Allocation & Disposal
            
            #region Allocation
            
            public PoolEntry( Worldspace worldspace )
            {
                _Worldspace = worldspace;
                /*
                var b = string.Format( @"{0}{1}\{2}\{2}", GodObject.Paths.BorderBuilder, GUIBuilder.Constant.WorldspacePath, _Worldspace.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if(
                    // disable once InvokeAsExtensionMethod
                    ( GenFilePath.TryAssignFile( b + "_LandHeights.dds" , ref LandHeights_Texture_File  ) )&&
                    // disable once InvokeAsExtensionMethod
                    ( GenFilePath.TryAssignFile( b + "_WaterHeights.dds", ref WaterHeights_Texture_File ) )&&
                    // disable once InvokeAsExtensionMethod
                    ( GenFilePath.TryAssignFile( b + "_Stats.txt"       , ref Stats_File                ) )
                )
                {
                    FetchPhysicalDDSInfo( LandHeights_Texture_File );
                    FetchPhysicalStatsInfo();
                }
                */
            }
            
            #endregion
            
            #region Disposal
            
            protected bool Disposed = false;
            
            ~PoolEntry()
            {
                Dispose( true );
            }
            
            public void Dispose()
            {
                Dispose( true );
            }
            
            protected void Dispose( bool disposing )
            {
                if( Disposed )
                    return;
                Disposed = true;
                
                // Dispose of external references
                DestroyTextures();
                DestroySurfaces();
                
                //LandHeightMap = null;
                //WaterHeightMap = null;
            }
            
            public void DestroyTextures()
            {
                _texturesReady = false;
                if( LandHeight_Texture != null )
                {
                    LandHeight_Texture.Dispose();
                    LandHeight_Texture = null;
                }
                if( WaterHeight_Texture != null )
                {
                    WaterHeight_Texture.Dispose();
                    WaterHeight_Texture = null;
                }
            }
            
            public void DestroySurfaces()
            {
                if( LandHeight_Surface != null )
                {
                    LandHeight_Surface.Dispose();
                    LandHeight_Surface = null;
                }
                if( WaterHeight_Surface != null )
                {
                    WaterHeight_Surface.Dispose();
                    WaterHeight_Surface = null;
                }
            }
            
            #endregion
            
            #endregion
            
            #region Derived Properties from file data
            
            /*
            public Vector2i HeightMapOffset
            {
                get
                {
                    var midX = HeightMap_Width  / 2;
                    var midY = HeightMap_Height / 2;
                    return new Vector2i( midY, midY );
                }
            }
            */

            /*
            public Vector2i HeightMapCellOffset
            {
                get
                {
                    var midX = HeightMap_Width  / 2;
                    var midY = HeightMap_Height / 2;
                    var midXC = (int)( midX / Engine.Constant.HeightMap_Resolution );
                    var midYC = (int)( midY / Engine.Constant.HeightMap_Resolution );
                    return new Vector2i( -midXC, midYC );
                }
            }
            */

            /*
            public Vector2i HeightMapCellSize
            {
                get
                {
                    var xC = (int)( HeightMap_Width  / Engine.Constant.HeightMap_Resolution );
                    var yC = (int)( HeightMap_Height / Engine.Constant.HeightMap_Resolution );
                    return new Vector2i( xC, yC );
                }
            }
            */

            #endregion
            
            #region Heightmap scanning
            
            public float SurfaceHeightAtWorldPos( float x, float y )
            {
                // If the land is below the water, return the waters surface
                var lh = LandHeightAtWorldPos( x, y );
                var wh = WaterHeightAtWorldPos( x, y );
                return lh > wh ? lh : wh;
            }
            
            public float LandHeightAtWorldPos( float x, float y )
            {
                return HeightAtWorldPos( x, y );//, LandHeightMap );
            }
            /*
            public float LandHeightAtPos( int x, int y )
            {
                return HeightAtPos( x, y, LandHeightMap );
            }
            */

            // Read the water height from the CELL record (or the WRLD default) instead of the exported water texture
            // as the exported water texture isn't accurate enough for analysis but it's fine for rendering purposes.
            public float WaterHeightAtWorldPos( float x, float y )
            {
                var grid = Engine.SpaceConversions.WorldspaceToCellGrid( x, y );
                var cell = _Worldspace.Cells.GetByGrid( grid );
                var value = cell == null
                    ? _Worldspace.LandData.GetDefaultWaterHeight( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired )
                    : cell.WaterHeight.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                return value;
            }

            /*
            public float WaterHeightAtPos( int x, int y )
            {
                var grid = Engine.SpaceConversions.HeightmapToCellGrid( x, y, HeightMapOffset );
                var cell = _Worldspace.Cells.GetByGrid( grid );
                var value = cell == null
                    ? _Worldspace.LandData.GetDefaultWaterHeight( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired )
                    : cell.WaterHeight.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                return value;
            }
            */

            /*
            public float HeightAtPos( int x, int y, float[,] heightMap )
            {
                if( ( x < 0 )||( y < 0 )||( x >= HeightMap_Width )||( y >= HeightMap_Height ) ) return float.MinValue;
                return MinHeight + ( heightMap[ y, x ] * DeltaHeight );
            }
            */

            public float HeightAtWorldPos( float x, float y )//, float[,] heightMap )
            {
                // z01      z11
                //  +--------+
                //  |        |   Where:
                //  |  P     |   P = world position x, y
                //  |        |   zXY = heightmap point
                //  +--------+
                // z00      z10

                var result = 0.0f;

                #region Short-hand
                
                //var hmo = HeightMapOffset;
                //var map = heightMap;
                const float htw = Engine.Constant.HeightMap_To_Worldmap;
                const float wth = Engine.Constant.WorldMap_To_Heightmap;
                
                #endregion

                var grid = Engine.SpaceConversions.WorldspaceToCellGrid( x, y );

                var cell = _Worldspace.Cells.GetByGrid( grid );
                if( cell == null )
                    throw new Exception( this.TypeFullName() + " : Worldspace.Cells.GetByGrid() returned null!" );
                
                var landscape = cell.Landscape;
                if( landscape == null )
                    throw new Exception( this.TypeFullName() + " : Cell.Landscape returned null!" );
                
                var heightmap = landscape.Heightmap.GetHeightmap( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                if( heightmap == null )
                    throw new Exception( this.TypeFullName() + " : Landscape.GetHeightMap() returned null!" );

                // Subtract the bottom-left corner of the CELL from the input world pos
                var cellBase = Engine.SpaceConversions.CellGridToWorldspace( grid );
                var xToCell = x - cellBase.X;
                var yToCell = y - cellBase.Y;

                /*
                DebugLog.WriteStrings( null, new [] {
                    "-----------------------------------------",
                    "[in] = (" + x + "," + y + ")",
                    "Cell.Grid = " + cell.CellGrid.GetGrid( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                    "[in]ToCell = (" + xToCell + "," + yToCell + ")",
                    "cellBase = " + cellBase.ToString(),
                }, false, true, false, false, false );
                */

                #region Get points on the height map

                // Get the four verts for the quad the point is in.
                var x0 = (int)( xToCell * wth );// if( x < 0 ) x0--;
                var y0 = (int)( yToCell * wth );// if( y < 0 ) y0--;
                var x1 = x0 + 1;
                var y1 = y0 + 1;

                /*
                DebugLog.WriteStrings( null, new [] {
                    "[hm]: x0 = " + x0 + " : y0 = " + y0,
                    "[hm]: x1 = " + x1 + " : y1 = " + y1
                }, false, true, false, false, false );
                */

                /*
                var z00 = map[ hmo.Y - y0, hmo.X + x0 ] * DeltaHeight + MinHeight;
                var z10 = map[ hmo.Y - y0, hmo.X + x1 ] * DeltaHeight + MinHeight;
                var z01 = map[ hmo.Y - y1, hmo.X + x0 ] * DeltaHeight + MinHeight;
                var z11 = map[ hmo.Y - y1, hmo.X + x1 ] * DeltaHeight + MinHeight;
                */
                var z00 = heightmap[ x0, y0 ];
                var z10 = heightmap[ x1, y0 ];
                var z01 = heightmap[ x0, y1 ];
                var z11 = heightmap[ x1, y1 ];

                /*
                DebugLog.WriteStrings( null, new [] {
                    "[hm]: z00 = " + z00 + " : z10 = " + z10,
                    "[hm]: z01 = " + z01 + " : z11 = " + z11
                }, false, true, false, false, false );
                */

                #endregion

                #region Height By Lerping
                #if HEIGHT_MAP_AT_POS_LERP
                
                // Does some slow lerps and averages to interpolate the height
                // of the ground at a given world point, this is not a perfect value
                // and can be off by a small amount due to the way the land tris
                // are generated.  The closer the position is to the actual ground
                // vertex the more accurate it is.

                // Get x,y delta and norms from point to the reference vert (0,0)
                var xd = x - ( (float)x0 * htw )
                var yd = y - ( (float)y0 * htw );
                var xn = xd * wth;
                var yn = yd * wth;
                
                // Lerp along the edges
                var lz00z01 = Maths.Lerps.Lerp( z00, z01, xn );
                var lz00z10 = Maths.Lerps.Lerp( z00, z10, yn );
                var lz10z11 = Maths.Lerps.Lerp( z10, z11, xn );
                var lz01z11 = Maths.Lerps.Lerp( z01, z11, yn );
                
                // Average the results
                result = ( lz00z01 + lz00z10 + lz10z11 + lz01z11 ) * 0.25f;
                
                #endif
                #endregion

                #region Height By Triangle Intersect
                #if HEIGHT_MAP_AT_POS_INTERSECT

                // Get the highest vertex from the heightmap and set the ray slightly above that
                var zr = Math.Max( Math.Max( Math.Max( z00, z01 ), z10 ), z11 ) + 16.0f;

                // Down ray
                Ray ray = new Ray( new Vector3f( xToCell, yToCell, zr ), Vector3f.Down );

                // Pick the proper triangle layout (See "Notes\Heightmap Triangle Layout.[txt/png]")
                var xO = x0 & 1; // X is odd?
                var yO = y0 & 1; // Y is odd?

                var tx0 = x0 * htw;
                var tx1 = x1 * htw;
                var ty0 = y0 * htw;
                var ty1 = y1 * htw;

                Vector3f[] t0, t1;

                if( xO == yO )          //if( (  xO &&  yO )||( !xO && !yO ) )
                {
                    t0 = new Vector3f[ 3 ] { new Vector3f( tx0, ty0, z00 ), new Vector3f( tx0, ty1, z01 ), new Vector3f( tx1, ty1, z11 ) };
                    t1 = new Vector3f[ 3 ] { new Vector3f( tx0, ty0, z00 ), new Vector3f( tx1, ty1, z11 ), new Vector3f( tx1, ty0, z10 ) };
                }
                else //if( xO != yO )   //if( ( !xO &&  yO )||(  xO && !yO ) )
                {
                    t0 = new Vector3f[ 3 ] { new Vector3f( tx0, ty0, z00 ), new Vector3f( tx0, ty1, z01 ), new Vector3f( tx1, ty0, z10 ) };
                    t1 = new Vector3f[ 3 ] { new Vector3f( tx0, ty1, z01 ), new Vector3f( tx1, ty1, z11 ), new Vector3f( tx1, ty0, z10 ) };
                }

                //DebugLog.WriteLine( "t0: " + t0[ 0 ] + " " + t0[ 1 ] + " " + t0[ 2 ] );
                //DebugLog.WriteLine( "t1: " + t1[ 0 ] + " " + t1[ 1 ] + " " + t1[ 2 ] );
                //DebugLog.WriteLine( "ray: " + ray.ToString() );

                Vector3f vResult = Vector3f.Zero;

                // Get the intersection with triangle
                if     ( Geometry.Collision.RayTriangleIntersect( ray, t0, out vResult, Geometry.Orientation.CW ) )
                    result = vResult.Z;
                else if( Geometry.Collision.RayTriangleIntersect( ray, t1, out vResult, Geometry.Orientation.CW ) )
                    result = vResult.Z;
                else
                    DebugLog.WriteError( "Ray did not intersect with either heightmap triangle!\nray = " + ray.ToString() + "\nt0 = " + t0[ 0 ] + " " + t0[ 1 ] + " " + t0[ 2 ] + "\nt1: " + t1[ 0 ] + " " + t1[ 1 ] + " " + t1[ 2 ] );
                
                #endif
                #endregion

                //DebugLog.WriteLine( "result = " + result );
                return result;
            }

            public bool ComputeZHeightsFromVolumes( Vector2f[][] volumes, out float minZ, out float maxZ, out float averageZ, out float averageWaterZ, bool useWaterIfHigher = true, bool showScanlineProgress = false )
            {
                //DebugLog.OpenIndentLevel( new [] { "GodObject.WorldspaceDataPool.PoolEntry", "ComputeZHeightsFromVolumes()" } );
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                if( showScanlineProgress )
                    m.PushItemOfItems();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var result = true; // Unless it isn't
                
                minZ = float.MaxValue;
                maxZ = float.MinValue;
                averageWaterZ = float.MinValue;
                averageZ = 0f;
                if( volumes.NullOrEmpty() )
                {
                    DebugLog.WriteError( "volumes is null or empty" );
                    result = false;
                    goto localAbort;
                }
                /*
                if( !LoadLandHeightMap() )
                {
                    DebugLog.WriteError( "Cannot load land heightmap" );
                    result = false;
                    goto localAbort;
                }
                */

                double totalWaterZ = 0;
                double totalLandZ = 0;
                int waterPoints = 0;
                int landPoints = 0;
                var wsMin = Vector2f.Min( volumes );
                var wsMax = Vector2f.Max( volumes );
                var gridWSSize = Engine.Constant.WorldMap_Resolution;
                var minGrid = Engine.SpaceConversions.WorldspaceToCellGrid( wsMin );
                var maxGrid = Engine.SpaceConversions.WorldspaceToCellGrid( wsMax.X + gridWSSize, wsMax.Y - gridWSSize );
                //var minBounding = Engine.SpaceConversions.WorldspaceToHeightmap( wsMin, HeightMapOffset );
                //var maxBounding = Engine.SpaceConversions.WorldspaceToHeightmap( wsMax, HeightMapOffset );
                //var max = minBounding.Y - maxBounding.Y;
                var max = minGrid.Y - maxGrid.Y;

                //for( int hy = maxBounding.Y; hy <= minBounding.Y; hy++ )
                for( int hy = maxGrid.Y; hy <= minGrid.Y; hy++ )
                {
                    
                    if( showScanlineProgress )
                        m.SetItemOfItems( hy - maxGrid.Y, max );
                        //m.SetItemOfItems( hy - maxBounding.Y, max );
                    
                    //for( int hx = minBounding.X; hx <= maxBounding.X; hx++ )
                    for( int hx = minGrid.X; hx <= maxGrid.X; hx++ )
                    {
                        //var hmWP = Engine.SpaceConversions.HeightmapToWorldspace( hx, hy, HeightMapOffset );
                        var hmWP = new Vector2f( hx * gridWSSize, hy * gridWSSize );
                        var hitIndex = Maths.Geometry.Collision.PointInPolys( hmWP, volumes );
                        if( hitIndex < 0 ) continue;
                        //var lh = LandHeightAtPos( hx, hy );
                        //var wh = WaterHeightAtPos( hx, hy );
                        var lh = LandHeightAtWorldPos( hx, hy );
                        var wh = WaterHeightAtWorldPos( hx, hy );
                        if( wh > float.MinValue )
                        {
                            totalWaterZ += (double)wh;
                            waterPoints++;
                        }
                        var th = !useWaterIfHigher  // Ignore water?
                            ? lh                    // Use land
                            : lh > wh               // Is land above water?
                                ? lh                // Use the land
                                : wh;               // Use water
                        if( lh < minZ ) minZ = lh;  // Always use land for lowest
                        if( th > maxZ ) maxZ = th;  // Use land/water for highest
                        totalLandZ += (double)th;
                        landPoints++;
                    }
                }
                
                if( landPoints < 1 )    // No land points means nothing was found inside the volumes
                {
                    DebugLog.WriteError( "No points on heightmap found in volume[s]" );
                    result = false;       // This should never happen unless the heightmap bounding or heightmap <-> worldspace conversions failed
                    goto localAbort;
                }
                
                averageZ = (float)( totalLandZ / (double)landPoints );
                averageWaterZ = waterPoints > 0
                    ? (float)( totalWaterZ / (double)waterPoints )
                    : float.MinValue;
                
                
            localAbort:
                var tEnd = m.SyncTimerElapsed().Ticks - tStart.Ticks;
                if( showScanlineProgress )
                    m.PopItemOfItems();
                //DebugLog.CloseIndentLevel( tEnd, "result", result.ToString() );
                return result;
            }

            #endregion

            #region Heightmap[s] DDS loading and Texture creation

            #region Load Heightmaps
            /*

            public bool LoadHeightMapData()
            {
                return
                    ( LoadLandHeightMap() )&&
                    ( LoadWaterHeightMap() );
            }
            
            public bool LoadLandHeightMap()
            {
                return
                    ( LandHeightMap != null )||
                    ( LoadHeightMapDDS( LandHeights_Texture_File, ref LandHeightMap ) );
            }
            
            public bool LoadWaterHeightMap()
            {
                return
                    ( WaterHeightMap != null )||
                    ( LoadHeightMapDDS( WaterHeights_Texture_File, ref WaterHeightMap ) );
            }

            */
            #endregion

            #region Get Heightmap Color At Position

            delegate int HeightMapColorAt( int x, int y );
            
            uint LandHeightmapColor( int x, int y )
            {
                return 0xFFFF00FF;
                /*
                var lh = LandHeightMap[ y, x ];
                var f = 255 * lh;
                var i = (byte)Math.Round( f );
                unchecked
                {
                    return (int)0xFF000000 | ( i << 16 ) | ( i << 8 ) | i;
                }
                */
            }
            
            uint WaterHeightmapColor( int x, int y )
            {
                return 0x7FFF00FF;
                /*
                var wh = WaterHeightMap[ y, x ];
                var lh = LandHeightMap[ y, x ];
                return wh > lh ? 0x7F00007F : 0;
                */
            }

            #endregion

            #region Create Surfaces and Textures

            unsafe SDLRenderer.Surface CreateSurface( SDLRenderer renderer, HeightMapColorAt hmColorAt, SDL.SDL_BlendMode blendMode )
            {
                return null;
                /*
                var surface = renderer.CreateSurface( HeightMap_Width, HeightMap_Height, SDL.SDL_PIXELFORMAT_ARGB8888 );
                
                if( surface == null )
                    throw new Exception( string.Format( "Cannot create a new surface!\n\n{0}", SDL.SDL_GetError() ) );
                
                if( surface.MustLock )
                    surface.Lock();
                
                var pixels = (int*)surface.Pixels;
                var pitch = surface.Pitch;
                
                for( int y = 0; y < HeightMap_Height; y++ )
                {
                    var scanline = pixels + ( ( pitch * y ) / sizeof( int ) );
                    {
                        for( int x = 0; x < HeightMap_Width; x++ )
                        {
                            unchecked
                            {
                                scanline[ x ] = hmColorAt( x, y );
                            }
                        }
                    }
                }
                
                if( surface.MustLock )
                    surface.Unlock();
                
                surface.BlendMode = blendMode;
                
                return surface;
                */
            }
            
            void THREAD_SDL_INVOKE_LoadAndCreateHeightmapTextures( SDLRenderer renderer )
            {
                /*
                DebugLog.OpenIndentLevel();
                
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();

                //LoadHeightMapData();

                //m.SetCurrentStatusMessage( string.Format( "WorldspaceDataPool.CreatingTextureFrom".Translate(), LandHeights_Texture_File ) );
                if( LandHeight_Texture != null )
                {
                    LandHeight_Texture.Dispose();
                    LandHeight_Texture = null;
                }
                if( LandHeight_Surface == null )
                {
                    LandHeight_Surface = CreateSurface( renderer, LandHeightmapColor, SDL.SDL_BlendMode.SDL_BLENDMODE_NONE );
                }
                LandHeight_Texture = renderer.CreateTextureFromSurface( LandHeight_Surface );
                
                //m.SetCurrentStatusMessage( string.Format( "WorldspaceDataPool.CreatingTextureFrom".Translate(), WaterHeights_Texture_File ) );
                
                if( WaterHeight_Texture != null )
                {
                    WaterHeight_Texture.Dispose();
                    WaterHeight_Texture = null;
                }
                if( WaterHeight_Surface == null )
                {
                    WaterHeight_Surface = CreateSurface( renderer, WaterHeightmapColor, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND );
                }
                WaterHeight_Texture = renderer.CreateTextureFromSurface( WaterHeight_Surface );
                
                _textureLoadQueued = false;
                _textureLoadQueuetransform = null;
                _texturesReady = ( LandHeight_Texture != null )&&( WaterHeight_Texture != null );

                m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                GodObject.Windows.SetEnableState( renderer, true );
                DebugLog.CloseIndentLevel();
                */
            }
            
            public void CreateHeightmapTextures( GUIBuilder.Windows.RenderChild.RenderTransform transform )
            {
                /*
                DebugLog.OpenIndentLevel();

                if( ( transform != null )&&
                    ( transform.ReadyForUse() ) &&
                    ( !_textureLoadQueued ) )
                {

                    _textureLoadQueued = true;
                    GodObject.Windows.SetEnableState( transform, false );

                    _texturesReady = false;
                    _textureLoadQueuetransform = transform;

                    //THREAD_SDL_INVOKE_LoadAndCreateHeightmapTextures( transform.Renderer );
                    _textureLoadQueuetransform.Renderer.BeginInvoke( THREAD_SDL_INVOKE_LoadAndCreateHeightmapTextures );

                }

                DebugLog.CloseIndentLevel();
                */
            }

            #endregion

            #region DDS File Handling
            /*

            bool FetchPhysicalStatsInfo()
            {
                var statsLines = System.IO.File.ReadAllLines( Stats_File );
                if( ( statsLines == null )||( statsLines.Length < 1 ) )
                    return false;
                int c = statsLines.Length;
                int i = 0;
                while( i < c )
                {
                    var parsed = statsLines[ i ].ParseImportLine( ':' );
                    if( ( parsed != null )&&( parsed.Length == 2 ) )
                    {
                        if( parsed[ 0 ].InsensitiveInvariantMatch( "max height" ) )
                            MaxHeight = float.Parse( parsed[ 1 ] );
                        if( parsed[ 0 ].InsensitiveInvariantMatch( "min height" ) )
                            MinHeight = float.Parse( parsed[ 1 ] );
                    }
                    i++;
                }
                DeltaHeight = MaxHeight - MinHeight;
                return true;
            }
            
            unsafe bool FetchPhysicalDDSInfo( string ddsFile )
            {
                if( !System.IO.File.Exists( ddsFile ) )
                    return false;
                
                FileStream ddsStream = null;
                try
                {
                    ddsStream = System.IO.File.Open( ddsFile, FileMode.Open );
                    if( ddsStream == null )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "File.Open()\nNull\nUnable to access \"{0}\"!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return false;
                    }
                }
                catch ( Exception e )
                {
                    DebugLog.WriteException( e );
                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Open()\n{1}\nUnable to access \"{0}\"!", ddsFile, e.ToString() ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                    return false;
                }
                
                var thingsOk = true;
                var ddsHeader = new DirectX.DDS_Header();
                if( !NoBSFileAccess.Read( ddsStream, 0, &ddsHeader, (uint)sizeof( DirectX.DDS_Header ) ) )
                {
                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Read()\nDDS_HEADER\nUnable to read from \"{0}\"!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                    thingsOk  = false;
                }
                else
                {
                    
                    if( !ddsHeader.FileIDOk )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "\"{0}\" is not a valid DDS file!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        thingsOk  = false;
                    }
                    else
                    {
                        HeightMap_Width = (int)ddsHeader.dwWidth;
                        HeightMap_Height = (int)ddsHeader.dwHeight;
                    }
                }
                
                ddsStream.Close();
                return thingsOk;
            }
            
            unsafe bool LoadHeightMapDDS( string ddsFile, ref float[,] target )
            {
                if( ( target != null )||( !System.IO.File.Exists( ddsFile ) ) )
                    return false;
                
                var thingsOk = true;
                target = null;
                
                FileStream ddsStream = null;
                try
                {
                    ddsStream = System.IO.File.Open( ddsFile, FileMode.Open );
                    if( ddsStream == null )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "File.Open()\nNull\nUnable to access \"{0}\"!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return false;
                    }
                }
                catch ( Exception e)
                {
                    DebugLog.WriteException( e );
                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Open()\n{1}\nUnable to access \"{0}\"!", ddsFile, e.ToString() ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                    return false;
                }
                
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( string.Format( "WorldspaceDataPool.Loading".Translate(), ddsFile ) );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var ddsHeader = new DirectX.DDS_Header();
                if( !NoBSFileAccess.Read( ddsStream, 0, &ddsHeader, (uint)sizeof( DirectX.DDS_Header ) ) )
                {
                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Read()\nDDS_HEADER\nUnable to read from \"{0}\"!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                    thingsOk  = false;
                }
                else
                {
                    
                    if( !ddsHeader.FileIDOk )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "\"{0}\" is not a valid DDS file!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        thingsOk  = false;
                    }
                    else
                    {
                        HeightMap_Width = (int)ddsHeader.dwWidth;
                        HeightMap_Height = (int)ddsHeader.dwHeight;
                        
                        target = new float[ HeightMap_Height, HeightMap_Width ];
                        
                        for( int y = 0; y < HeightMap_Height; y++ )
                        {
                            fixed( float* p = &target[ y, 0 ] )
                            {
                                if( !NoBSFileAccess.Read( ddsStream, p, ddsHeader.dwPitchOrLinearSize ) )
                                {
                                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Read()\nScanline\nUnable to read from \"{0}\"!", ddsFile ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                                    thingsOk  = false;
                                }
                            }
                        }
                    }
                }
                ddsStream.Close();
                
                m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                return thingsOk;
            }

            */
            #endregion

            #endregion

        }

        static Dictionary<uint,PoolEntry> __Pool = null;
        static Dictionary<uint,PoolEntry> _Pool
        {
            get
            {
                if( __Pool == null )
                     __Pool = new Dictionary<uint, PoolEntry>();
                return __Pool;
            }
        }
        
        static void CreateEntryFor( Worldspace worldspace )
        {
            var entry = new PoolEntry( worldspace );
            _Pool.Add( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ), entry );
        }
        
        public static void DrainPool()
        {
            if( __Pool == null )
                return;
            DestroyWorldspaceTextures( true );
            __Pool.Clear();
            __Pool = null;
        }
        
        public static PoolEntry GetPoolEntry( Worldspace worldspace )
        {
            var wFID = worldspace.GetFormID( Engine.Plugin.TargetHandle.Master );
            if( !_Pool.ContainsKey( wFID ) )
                CreateEntryFor( worldspace );
            return _Pool[ wFID ];
        }
        
        public static void DestroyWorldspaceTextures( bool destroySurfaces )
        {
            if( ( _Pool == null )||( _Pool.Count < 1 ) )
                return;
            foreach( var p in _Pool )
            {
                p.Value.DestroyTextures();
                if( destroySurfaces)
                    p.Value.DestroySurfaces();
            }
        }
        
    }
    
}
