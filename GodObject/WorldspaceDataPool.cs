/*
 * WorldspaceDataPool.cs
 *
 * Worldspace heightmap data.
 * 
 * This is stored separately from the worldspace itself so that the heightmap
 * isn't loaded multiple times for each override of a given worldspace.
 *
 * TODO:  Fold this into the Worldspace itself
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
        
        // User feedback strings in the status area
        static readonly string txtCreateTexturesNP = "WorldspaceDataPool.CreatingTextures".Translate();
        static readonly string txtCreateTextures = "WorldspaceDataPool.CreatingTexturesP".Translate();
        
        public class PoolEntry : IDisposable
        {
            
            Worldspace _Worldspace = null;
            
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
            
            #region Heightmap scanning
            
            public float SurfaceHeightAtWorldPos( Engine.Plugin.TargetHandle target, float x, float y )
            {
                // If the land is below the water, return the waters surface
                var lh = LandHeightAtWorldPos( target, x, y );
                var wh = WaterHeightAtWorldPos( target, x, y );
                return lh > wh ? lh : wh;
            }
            
            // Read the land height from the CELLs LAND record (or the WRLD default)
            public float LandHeightAtWorldPos( Engine.Plugin.TargetHandle target, float x, float y )
            {
                //DebugLog.OpenIndentLevel( string.Format( "[in] = ({0},{1})", x, y ) );

                var result = HeightAtWorldPos( target, x, y );
                
                //DebugLog.CloseIndentLevel( "result", result.ToString() );
                return result;
            }

            // Read the water height from the CELL record (or the WRLD default)
            public float WaterHeightAtWorldPos( Engine.Plugin.TargetHandle target, float x, float y )
            {
                // If this worldspace uses a parents water data, then return the parents water height
                if( ( _Worldspace.Parent.GetParentingFlags( target ) & (uint)Engine.Plugin.Forms.Fields.Worldspace.Parent.Flags.UseWaterData ) != 0 )
                {
                    var parentWorldspace = _Worldspace.Parent.GetParentWorldspace( target );
                    if( parentWorldspace == null )
                        throw new Exception( string.Format( "Worldspace {0} uses parent worldspace water data but does not specify a parent worldspace!", _Worldspace.IDString ) );
                    
                    var parentWSDP = GetPoolEntry( parentWorldspace );
                    if( parentWSDP == null )
                        throw new Exception( string.Format( "Could not get WorldspaceDataPool for worldspace {0}!", parentWorldspace.IDString ) );
                    
                    return parentWSDP.WaterHeightAtWorldPos( target, x, y );
                }
                
                var grid = Engine.SpaceConversions.WorldspaceToCellGrid( x, y );
                var cell = _Worldspace.Cells.GetByGrid( grid );
                return cell == null
                    ? _Worldspace.LandData.GetDefaultWaterHeight( target )
                    : cell.WaterHeight.GetValue( target );
            }

            // Get the actual land heightmap for the WRLD grid
            public float[,] HeightMapForWorldspaceGrid( Engine.Plugin.TargetHandle target, Vector2i grid )
            {
                // If this worldspace uses a parents land data, then return the parents heightmap
                if( ( _Worldspace.Parent.GetParentingFlags( target ) & (uint)Engine.Plugin.Forms.Fields.Worldspace.Parent.Flags.UseLandData ) != 0 )
                {
                    var parentWorldspace = _Worldspace.Parent.GetParentWorldspace( target );
                    if( parentWorldspace == null )
                        throw new Exception( string.Format( "Worldspace {0} uses parent worldspace land data but does not specify a parent worldspace!", _Worldspace.IDString ) );
                    
                    var parentWSDP = GetPoolEntry( parentWorldspace );
                    if( parentWSDP == null )
                        throw new Exception( string.Format( "Could not get WorldspaceDataPool for worldspace {0}!", parentWorldspace.IDString ) );
                    
                    return parentWSDP.HeightMapForWorldspaceGrid( target, grid );
                }

                var cell = _Worldspace.Cells.GetByGrid( grid );
                var landscape = cell?.Landscape;
                if( landscape != null )
                    return landscape.Heightmap.GetHeightmap( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                
                // No CELL for the grid or the CELL does not have a LAND record
                // Create a flat heightmap with the default height
                var hms = Engine.Plugin.Forms.Fields.Landscape.Heightmap.HeightmapSize;
                var dlh = _Worldspace.LandData.GetDefaultLandHeight( target );
                var heightmap = new float[ hms, hms ];
                for( var row = 0; row < hms; row++ )
                    for( var col = 0; col < hms; col++ )
                        heightmap[ col, row ] = dlh;
                
                return heightmap;
            }

            public float HeightAtWorldPos( Engine.Plugin.TargetHandle target, float x, float y )
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
                
                const float htw    = Engine.Constant.HeightMap_To_Worldmap;
                const float wth    = Engine.Constant.WorldMap_To_Heightmap;
                const int   hmSize = Engine.Plugin.Forms.Fields.Landscape.Heightmap.HeightmapSize - 1;

                #endregion

                // Get the heightmap for the grid that world pos x,y falls into
                var grid = Engine.SpaceConversions.WorldspaceToCellGrid( x, y );

                Vector2f cellBase = Vector2f.Zero;
                float xToCell = 0.0f;
                float yToCell = 0.0f;
                int x0 = 0;
                int y0 = 0;
                bool regrid = true;
                while( regrid )
                {
                    regrid = false;

                    // Subtract the bottom-left corner of the CELL from the input world pos
                    cellBase = Engine.SpaceConversions.CellGridToWorldspace( grid );
                    xToCell = x - cellBase.X;
                    yToCell = y - cellBase.Y;

                    // Get the four verts for the quad the point is in.
                    x0 = (int)( xToCell * wth );
                    if( x0 == hmSize )
                    {
                        grid.X++;
                        regrid = true;
                        //DebugLog.WriteLine( "regrid on x0" );
                    }
                    y0 = (int)( yToCell * wth );
                    if( y0 == hmSize )
                    {
                        grid.Y++;
                        regrid = true;
                        //DebugLog.WriteLine( "regrid on y0" );
                    }
                }
                var x1 = x0 + 1;
                var y1 = y0 + 1;

                #region Get points on the height map

                /*
                DebugLog.WriteStrings( null, new [] {
                    "grid = " + grid.ToString(),
                    "cellBase = " + cellBase.ToString(),
                    "xToCell = " + xToCell,
                    "yToCell = " + yToCell,
                    "x0 = " + x0,
                    "x1 = " + x1,
                    "y0 = " + y0,
                    "y1 = " + y1,
                    "hmSize = " + hmSize
                }, false, true, false, false, false );
                */

                var heightmap = HeightMapForWorldspaceGrid( target, grid );
                var z00 = heightmap[ x0, y0 ];
                var z10 = heightmap[ x1, y0 ];
                var z01 = heightmap[ x0, y1 ];
                var z11 = heightmap[ x1, y1 ];

                #endregion

                #region Height By Triangle Intersect

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

                Vector3f vResult = Vector3f.Zero;

                // Get the intersection with triangle
                if     ( Geometry.Collision.RayTriangleIntersect( ray, t0, out vResult, Geometry.Orientation.CW ) )
                    result = vResult.Z;
                else if( Geometry.Collision.RayTriangleIntersect( ray, t1, out vResult, Geometry.Orientation.CW ) )
                    result = vResult.Z;
                else
                    DebugLog.WriteError( "Ray did not intersect with either heightmap triangle!\nray = " + ray.ToString() + "\nt0 = " + t0[ 0 ] + " " + t0[ 1 ] + " " + t0[ 2 ] + "\nt1: " + t1[ 0 ] + " " + t1[ 1 ] + " " + t1[ 2 ] );
                
                #endregion

                return result;
            }

            public bool ComputeZHeightsFromVolumes( Engine.Plugin.TargetHandle target, Vector2f[][] volumes, out float minZ, out float maxZ, out float averageZ, out float averageWaterZ, bool useWaterIfHigher = true, bool showScanlineProgress = false )
            {
                //DebugLog.OpenIndentLevel();

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

                double totalWaterZ = 0;
                double totalLandZ = 0;
                int waterPoints = 0;
                int landPoints = 0;
                var wsMin = Vector2f.Min( volumes );
                var wsMax = Vector2f.Max( volumes );
                var gridWSSize = Engine.Constant.WorldMap_Resolution;
                var hmToWorld = Engine.Constant.HeightMap_To_Worldmap;
                var minBounds = Engine.SpaceConversions.WorldspaceToHeightmap( wsMin );
                var maxBounds = Engine.SpaceConversions.WorldspaceToHeightmap( wsMax.X + gridWSSize, wsMax.Y - gridWSSize );
                var max = maxBounds.Y - minBounds.Y;

                /*
                DebugLog.WriteStrings( null, new [] {
                    "minBounds = " + minBounds.ToString(),
                    "maxBounds = " + maxBounds.ToString(),
                }, false, true, false, false, false );
                */

                for( int hy = minBounds.Y; hy <= maxBounds.Y; hy++ )
                {
                    
                    if( showScanlineProgress )
                        m.SetItemOfItems( hy - minBounds.Y, max );
                    
                    for( int hx = minBounds.X; hx <= maxBounds.X; hx++ )
                    {
                        var hmX = hx * hmToWorld;
                        var hmY = hy * hmToWorld;
                        var hmWP = new Vector2f( hmX, hmY );
                        //DebugLog.WriteLine( "hmWP = " + hmWP.ToString() );
                        var hitIndex = Maths.Geometry.Collision.PointInPolys( hmWP, volumes );
                        if( hitIndex < 0 ) continue;
                        
                        var lh = LandHeightAtWorldPos( target, hmX, hmY );
                        var wh = WaterHeightAtWorldPos( target, hmX, hmY );
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
                m.StopSyncTimer( tStart );
                if( showScanlineProgress )
                    m.PopItemOfItems();
                
                //DebugLog.CloseIndentLevel();
                return result;
            }

            #endregion

            #region Heightmap[s] Texture creation

            // TODO:  This needs the min and max height for the worldspace which means scanning the entire world

            #region Get Heightmap Color At Position

            delegate int HeightMapColorAt( Engine.Plugin.TargetHandle target, int x, int y );
            
            uint LandHeightmapColor( Engine.Plugin.TargetHandle target, int x, int y )
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
            
            uint WaterHeightmapColor( Engine.Plugin.TargetHandle target, int x, int y )
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
