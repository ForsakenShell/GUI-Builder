/*
 * WorldspaceHeightmap.cs
 *
 * Worldspace heightmap data.
 * 
 * This is stored separately from the worldspace itself so that the heightmap
 * isn't loaded multiple times for each override of a given worldspace.
 *
 */
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
        const string txtCreateTexturesNP = "Creating textures...";
        const string txtCreateTextures = "Creating textures...{0}%";
        
        public class PoolEntry : IDisposable
        {
            
            Worldspace _Worldspace = null;
            
            public string LandHeights_Texture_File = null;
            public string WaterHeights_Texture_File = null;
            public string Stats_File = null;
            
            // Stats file data
            public float MaxHeight = 0.0f;
            public float MinHeight = 0.0f;
            
            // Height map data (extracted from DDS)
            public int HeightMap_Width = 0;
            public int HeightMap_Height = 0;
            public Single[,] LandHeightMap = null;
            public Single[,] WaterHeightMap = null;
            
            // Surfaces and Textures for rendering
            SDLRenderer.Surface LandHeight_Surface;
            SDLRenderer.Surface WaterHeight_Surface;
            public SDLRenderer.Texture LandHeight_Texture;
            public SDLRenderer.Texture WaterHeight_Texture;
            
            GUIBuilder.Windows.RenderChild.RenderTransform _textureLoadQueuetransform = null;
            bool _textureLoadQueued = false;
            bool _texturesReady = false;
            public bool TexturesReady            { get { return _texturesReady; } }
            
            #region Allocation & Disposal
            
            #region Allocation
            
            public PoolEntry( Worldspace worldspace )
            {
                _Worldspace = worldspace;
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
                
                LandHeightMap = null;
                WaterHeightMap = null;
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
            
            public Vector2i HeightMapOffset
            {
                get
                {
                    var midX = HeightMap_Width  / 2;
                    var midY = HeightMap_Height / 2;
                    return new Vector2i( midY, midY );
                }
            }
            
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
            
            public Vector2i HeightMapCellSize
            {
                get
                {
                    var xC = (int)( HeightMap_Width  / Engine.Constant.HeightMap_Resolution );
                    var yC = (int)( HeightMap_Height / Engine.Constant.HeightMap_Resolution );
                    return new Vector2i( xC, yC );
                }
            }
            
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
                return HeightAtWorldPos( x, y, LandHeightMap );
            }
            public float LandHeightAtPos( int x, int y )
            {
                return HeightAtPos( x, y, LandHeightMap );
            }
            
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
            public float WaterHeightAtPos( int x, int y )
            {
                var grid = Engine.SpaceConversions.HeightmapToCellGrid( x, y, HeightMapOffset );
                var cell = _Worldspace.Cells.GetByGrid( grid );
                var value = cell == null
                    ? _Worldspace.LandData.GetDefaultWaterHeight( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired )
                    : cell.WaterHeight.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                return value;
            }
            
            public float HeightAtPos( int x, int y, Single[,] heightMap )
            {
                if( ( x < 0 )||( y < 0 )||( x >= HeightMap_Width )||( y >= HeightMap_Height ) ) return float.MinValue;
                return MinHeight + ( heightMap[ y, x ] * ( MaxHeight - MinHeight ) );
            }
            
            public float HeightAtWorldPos( float x, float y, Single[,] heightMap )
            {
                // Does some slow lerps and averages to interpolate the height
                // of the ground at a given world point, this is not a perfect value
                // and can be off by a small amount due to the way the land tris
                // are generated.  The closer the position is to the actual ground
                // vertex the more accurate it is.
                
                // TODO:  Look at how the game/CK generates tris from
                // the heightmap data to get a more accurate result.
                
                // z01      z11
                //  +--------+
                //  |        |   Where:
                //  |  P     |   P = world position x, y
                //  |        |   zXY = heightmap point
                //  +--------+
                // z00      z10
                
                #region Get points on the height map
                
                // Short-hand
                var hmo = HeightMapOffset;
                var map = heightMap;
                
                // Get the four verts for the quad the point is in.
                var x0 = (int)( x * Engine.Constant.WorldMap_To_Heightmap ); if( x < 0 ) x0 -= 1;
                var y0 = (int)( y * Engine.Constant.WorldMap_To_Heightmap ); if( y < 0 ) y0 -= 1;
                var x1 = x0 + 1;
                var y1 = y0 + 1;
                var z00 = map[ hmo.Y - y0, hmo.X + x0 ];
                var z01 = map[ hmo.Y - y0, hmo.X + x1 ];
                var z10 = map[ hmo.Y - y1, hmo.X + x0 ];
                var z11 = map[ hmo.Y - y1, hmo.X + x1 ];
                
                #endregion
                
                #region Four line lerp - Lerp outside edges then average the results
                
                // Get x,y delta and norms from point to the reference vert (0,0)
                var xd = x - ( (float)x0 * Engine.Constant.HeightMap_To_Worldmap );
                var yd = y - ( (float)y0 * Engine.Constant.HeightMap_To_Worldmap );
                var xn = xd * Engine.Constant.WorldMap_To_Heightmap;
                var yn = yd * Engine.Constant.WorldMap_To_Heightmap;
                
                // Lerp along the edges
                var lz00z01 = Maths.Lerps.Lerp( z00, z01, xn );
                var lz00z10 = Maths.Lerps.Lerp( z00, z10, yn );
                var lz10z11 = Maths.Lerps.Lerp( z10, z11, xn );
                var lz01z11 = Maths.Lerps.Lerp( z01, z11, yn );
                
                // Average the results
                var az = ( lz00z01 + lz00z10 + lz10z11 + lz01z11 ) * 0.25f;
                var result = MinHeight + ( az * ( MaxHeight - MinHeight ) );
                
                #endregion
                
                return result;
            }
            
            public bool ComputeZHeightsFromVolumes( Vector2f[][] volumes, out float minZ, out float maxZ, out float averageZ, out float averageWaterZ, bool useWaterIfHigher = true, bool showScanlineProgress = false )
            {
                minZ = float.MaxValue;
                maxZ = float.MinValue;
                averageWaterZ = float.MinValue;
                averageZ = 0f;
                if( volumes.NullOrEmpty() )
                {
                    DebugLog.WriteError( "GodObject.WorldspaceDataPool.PoolEntry", "ComputeZHeightsFromVolumes()", "volumes is null or empty" );
                    return false;
                }
                if( !LoadLandHeightMap() )
                {
                    DebugLog.WriteError( "GodObject.WorldspaceDataPool.PoolEntry", "ComputeZHeightsFromVolumes()", "Cannot load land heightmap" );
                    return false;
                }
                
                double totalWaterZ = 0;
                double totalLandZ = 0;
                int waterPoints = 0;
                int landPoints = 0;
                var wsMin = Vector2f.Min( volumes );
                var wsMax = Vector2f.Max( volumes );
                var minBounding = Engine.SpaceConversions.WorldspaceToHeightmap( wsMin, HeightMapOffset );
                var maxBounding = Engine.SpaceConversions.WorldspaceToHeightmap( wsMax, HeightMapOffset );
                
                var m = GodObject.Windows.GetMainWindow();
                var max = (int)0;
                if( showScanlineProgress )
                {
                    m.PushItemOfItems();
                    max = minBounding.Y - maxBounding.Y;
                }
                
                for( int hy = maxBounding.Y; hy <= minBounding.Y; hy++ )
                {
                    
                    if( showScanlineProgress )
                        m.SetItemOfItems( hy - maxBounding.Y, max );
                    
                    for( int hx = minBounding.X; hx <= maxBounding.X; hx++ )
                    {
                        var hmWP = Engine.SpaceConversions.HeightmapToWorldspace( hx, hy, HeightMapOffset );
                        var hitIndex = Maths.Geometry.Collision.PointInPolys( hmWP, volumes );
                        if( hitIndex < 0 ) continue;
                        var lh = LandHeightAtPos( hx, hy );
                        var wh = WaterHeightAtPos( hx, hy );
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
                
                if( showScanlineProgress )
                    m.PopItemOfItems();
                
                if( landPoints < 1 )    // No land points means nothing was found inside the volumes
                {
                    DebugLog.WriteError( "GodObject.WorldspaceDataPool.PoolEntry", "ComputeZHeightsFromVolumes()", "No points on heightmap found in volume[s]" );
                    return false;       // This should never happen unless the heightmap bounding or heightmap <-> worldspace conversions failed
                }
                
                averageZ = (float)( totalLandZ / (double)landPoints );
                averageWaterZ = waterPoints > 0
                    ? (float)( totalWaterZ / (double)waterPoints )
                    : float.MinValue;
                
                return true;
            }
            
            #endregion
            
            #region Heightmap[s] DDS loading and Texture creation
            
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
            
            delegate int HeightMapColorAt( int x, int y );
            
            int LandHeightmapColor( int x, int y )
            {
                var lh = LandHeightMap[ y, x ];
                var f = 255 * lh;
                var i = (byte)Math.Round( f );
                unchecked
                {
                    return (int)0xFF000000 | ( i << 16 ) | ( i << 8 ) | i;
                }
            }
            
            int WaterHeightmapColor( int x, int y )
            {
                var wh = WaterHeightMap[ y, x ];
                var lh = LandHeightMap[ y, x ];
                return wh > lh ? 0x7F00007F : 0;
            }
            
            unsafe SDLRenderer.Surface CreateSurface( SDLRenderer renderer, HeightMapColorAt hmColorAt, SDL.SDL_BlendMode blendMode )
            {
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
            }
            
            void THREAD_LoadAndCreateHeightmapTextures( SDLRenderer renderer )
            {
                //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "THREAD_LoadAndCreateHeightmapTextures()" } );
                
                var m = GodObject.Windows.GetMainWindow();
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                LoadHeightMapData();
                
                m.SetCurrentStatusMessage( "Creating texture from land heightmap..." );
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
                
                m.SetCurrentStatusMessage( "Creating texture from water heightmap..." );
                
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
                _texturesReady = ( LandHeight_Texture != null )&&( WaterHeight_Texture != null );
                
                m.StopSyncTimer( "WorldspaceDataPool.PoolEntry :: THREAD_LoadAndCreateHeightmapTextures() :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
                GodObject.Windows.SetEnableState( true );   // Temp fix until the async bug with this function is resolved
                //DebugLog.CloseIndentLevel();
            }
            
            public void CreateHeightmapTextures( GUIBuilder.Windows.RenderChild.RenderTransform transform )
            {
                DebugLog.WriteLine( new [] { this.GetType().ToString(), "CreateHeightmapTextures()" } );
                
                if( !transform.ReadyForUse() )
                    return;
                if( _textureLoadQueued )
                    return;
                
                _textureLoadQueued = true;
                GodObject.Windows.SetEnableState( false );   // Temp fix until the async bug with this function is resolved
                
                _texturesReady = false;
                _textureLoadQueuetransform = transform;
                
                WorkerThreadPool.CreateWorker( THREAD_CreateHeightmapTextures, null ).Start();
            }
            
            unsafe void THREAD_CreateHeightmapTextures()
            {
                _textureLoadQueuetransform.Renderer.BeginInvoke( THREAD_LoadAndCreateHeightmapTextures );
                _textureLoadQueuetransform = null;
            }
            
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
                catch ( Exception e)
                {
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
            
            unsafe bool LoadHeightMapDDS( string ddsFile, ref Single[,] target )
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
                    System.Windows.Forms.MessageBox.Show( string.Format( "File.Open()\n{1}\nUnable to access \"{0}\"!", ddsFile, e.ToString() ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                    return false;
                }
                
                var m = GodObject.Windows.GetMainWindow();
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( string.Format( "Loading {0}...", ddsFile ) );
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
                        
                        target = new Single[ HeightMap_Height, HeightMap_Width ];
                        
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
                
                m.StopSyncTimer( "WorldspaceDataPool.PoolEntry :: LoadHeightMapDDS() :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
                return thingsOk;
            }
            
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
            _Pool.Add( worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), entry );
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
            if( !_Pool.ContainsKey( worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                CreateEntryFor( worldspace );
            return _Pool[ worldspace.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ];
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
