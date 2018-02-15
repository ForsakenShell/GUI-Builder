/*
 * bbWorldspace.cs
 * 
 * Class to encapsulate worldspaces, their heightmaps, etc, etc.
 * 
 * User: 1000101
 * Date: 25/11/2017
 * Time: 6:39 PM
 * 
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


using SDL2ThinLayer;
using SDL2;

namespace Border_Builder
{
    /// <summary>
    /// Description of bbWorldspace.
    /// </summary>
    public class bbWorldspace : IDisposable
    {
        private const string LandHeight_Texture_File_Suffix = "_LandHeights.dds";
        private const string WaterHeight_Texture_File_Suffix = "_WaterHeights.dds";
        private const string CellRange_File_Suffix = "_CellRange.txt";
        private const string Stats_File_Suffix = "_Stats.txt";
        
        private string _fullPath = null;
        
        public string Name = null;
        public string LandHeights_Texture_File = null;
        public string WaterHeights_Texture_File = null;
        public string Stats_File = null;
        public string CellRange_File = null;
        
        // Stats file data
        public string FormID = null;
        public int EditorID = 0;
        public float MaxHeight = 0.0f;
        public float MinHeight = 0.0f;
        
        // Cell Range file data
        public Maths.Vector2i CellNW;
        public Maths.Vector2i CellSE;
        
        // Height map data (extracted from DDS)
        public int HeightMap_Width = 0;
        public int HeightMap_Height = 0;
        public Single[,] LandHeightMap = null;
        public Single[,] WaterHeightMap = null;
        //public Bitmap LandHeight_Bitmap = null;
        //public Bitmap WaterHeight_Bitmap = null;
        SDL2ThinLayer.SDLRenderer.Surface LandHeight_Surface;
        SDL2ThinLayer.SDLRenderer.Surface WaterHeight_Surface;
        public SDL2ThinLayer.SDLRenderer.Texture LandHeight_Texture;
        public SDL2ThinLayer.SDLRenderer.Texture WaterHeight_Texture;
        
        // User feedback strings in the status area
        const string txtCreateTexturesNP = "Creating textures...";
        const string txtCreateTextures = "Creating textures...{0}%";
        
        #region Constructor
        
        public bbWorldspace( string newName, string fullPath )
        {
            // Validate paths and files
            if( string.IsNullOrEmpty( newName ) )
                throw new System.ArgumentException( "newName cannot be null or empty!", fullPath );
            Name = newName;
            if( !bbUtils.TryAssignPath( ref _fullPath, fullPath ) )
                throw new System.ArgumentException( "Invalid path for worldspace!", newName );
            if( !bbUtils.TryAssignFile( ref CellRange_File, _fullPath + newName + CellRange_File_Suffix ) )
                throw new System.ArgumentException( "Unable to find Cell Range file", newName );
            if( !bbUtils.TryAssignFile( ref Stats_File, _fullPath + newName + Stats_File_Suffix ) )
                throw new System.ArgumentException( "Unable to find Stats file", newName );
            // These files are optional but certain functions cannot be performed without them
            bbUtils.TryAssignFile( ref LandHeights_Texture_File, _fullPath + newName + LandHeight_Texture_File_Suffix );
            bbUtils.TryAssignFile( ref WaterHeights_Texture_File, _fullPath + newName + WaterHeight_Texture_File_Suffix );
        }
        
        #endregion
        
        #region Semi-Public API:  Destructor & IDispose
        
        // Protect against "double-free" errors caused by combinations of explicit disposal[s] and GC disposal
        
        bool _disposed = false;
        
        ~bbWorldspace()
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
            DestroyTextures();
            DestroySurfaces();
            
            LandHeightMap = null;
            WaterHeightMap = null;
            
            // This is no longer a valid state
            _disposed = true;
        }
        
        #endregion
        
        #region Data file preloading
        
        public void Preload()
        {
            LoadStatsFile();
            LoadCellRangeFile();
            FetchPhysicalDDSInfo( LandHeights_Texture_File );
        }
        
        #endregion
        
        #region Data file loading and parsing
        
        /// <summary>
        /// Parse #Name#_Stats.txt as exported from FO4CK -> World -> World LOD -> #Name# -> Export Land/Water Height Map(s)
        /// </summary>
        public void LoadStatsFile()
        {
            const string cWorldSpace = "Worldspace: ";
            const string cEditorIDOpen = " [";
            const string cEditorIDClose = "]";
            const string cMaxHeight = "Max height: ";
            const string cMinHeight = "Min height: ";
            var statsFile = File.ReadLines( Stats_File );
            foreach( string fileLine in statsFile )
            {
                if( fileLine.StartsWith( cWorldSpace ) )
                {
                    int openID =  fileLine.IndexOf( cEditorIDOpen );
                    int closeID =  fileLine.IndexOf( cEditorIDClose );
                    FormID = fileLine.Substring( cWorldSpace.Length, openID - cWorldSpace.Length );
                    string tmp = fileLine.Substring( openID + cEditorIDOpen.Length, closeID - openID - cEditorIDOpen.Length );
                    if( !int.TryParse( tmp, System.Globalization.NumberStyles.HexNumber, null, out EditorID ) )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "LoadStatsFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", Stats_File, fileLine, cWorldSpace ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return;
                    }
                }
                if( fileLine.StartsWith( cMaxHeight ) )
                {
                    MaxHeight = float.Parse( fileLine.Substring( cMaxHeight.Length ) );
                }
                if( fileLine.StartsWith( cMinHeight ) )
                {
                    MinHeight = float.Parse( fileLine.Substring( cMinHeight.Length ) );
                }
            }
        }
        
        /// <summary>
        /// Parse #Name#_CellRange.txt as written by user as reported by FO4CK -> World -> Generate Max Height Data For World
        /// </summary>
        public void LoadCellRangeFile()
        {
            const string cCellRangeMinX = "CellRangeMinX=";
            const string cCellRangeMaxX = "CellRangeMaxX=";
            const string cCellRangeMinY = "CellRangeMinY=";
            const string cCellRangeMaxY = "CellRangeMaxY=";
            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;
            var cellRangeFile = File.ReadLines( CellRange_File );
            foreach( string fileLine in cellRangeFile )
            {
                if( fileLine.StartsWith( cCellRangeMinX ) )
                {
                    minX = int.Parse( fileLine.Substring( cCellRangeMinX.Length ) );
                }
                if( fileLine.StartsWith( cCellRangeMaxX ) )
                {
                    maxX = int.Parse( fileLine.Substring( cCellRangeMaxX.Length ) );
                }
                if( fileLine.StartsWith( cCellRangeMinY ) )
                {
                    minY = int.Parse( fileLine.Substring( cCellRangeMinY.Length ) );
                }
                if( fileLine.StartsWith( cCellRangeMaxY ) )
                {
                    maxY = int.Parse( fileLine.Substring( cCellRangeMaxY.Length ) );
                }
            }
            CellNW = new Maths.Vector2i( minX, maxY );
            CellSE = new Maths.Vector2i( maxX, minY );
        }
        
        #endregion
        
        #region Derived Properties from file data
        
        public Maths.Vector2i HeightMapOffset
        {
            get
            {
                var midX = HeightMap_Width  / 2;
                var midY = HeightMap_Height / 2;
                return new Maths.Vector2i( midY, midY );
            }
        }
        
        public Maths.Vector2i HeightMapCellOffset
        {
            get
            {
                var midX = HeightMap_Width  / 2;
                var midY = HeightMap_Height / 2;
                var midXC = (int)( midX / bbConstant.HeightMap_Resolution );
                var midYC = (int)( midY / bbConstant.HeightMap_Resolution );
                return new Maths.Vector2i( -midXC, midYC );
            }
        }
        
        public Maths.Vector2i HeightMapCellSize
        {
            get
            {
                var xC = (int)( HeightMap_Width  / bbConstant.HeightMap_Resolution );
                var yC = (int)( HeightMap_Height / bbConstant.HeightMap_Resolution );
                return new Maths.Vector2i( xC, yC );
            }
        }
        
        #endregion
        
        #region Heightmap[s] DDS loading and Texture creation
        
        public void DestroyTextures()
        {
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
        
        public bool LoadLandHeightMap( RenderTransform transform )
        {
            return LoadHeightMapDDS( LandHeights_Texture_File, out LandHeightMap, transform );
        }
        
        public bool LoadWaterHeightMap( RenderTransform transform )
        {
            return LoadHeightMapDDS( WaterHeights_Texture_File, out WaterHeightMap, transform );
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
        
        unsafe SDLRenderer.Surface CreateSurface( RenderTransform transform, HeightMapColorAt hmColorAt, int statusPad, SDL.SDL_BlendMode blendMode )
        {
            var surface = transform.Renderer.CreateSurface( HeightMap_Width, HeightMap_Height, SDL.SDL_PIXELFORMAT_ARGB8888 );
            if( surface == null )
                throw new Exception( string.Format( "Cannot create a new surface!\n\n{0}", SDL.SDL_GetError() ) );
            
            if( surface.MustLock )
                surface.Lock();
            
            Single cp;
            Single lp = -1f;
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
                transform.MainForm.UpdateStatusProgress( y );
                cp = ((Single)( statusPad + y ) / ((Single)HeightMap_Height * 2.0f)) * 100.0f;
                if( (int)cp > (int)lp )
                {
                    transform.MainForm.UpdateStatusMessage( string.Format( txtCreateTextures, (int)cp ) );
                    lp = cp;
                }
                //System.Threading.Thread.Sleep(0);
            }
            
            if( surface.MustLock )
                surface.Unlock();
            
            surface.BlendMode = blendMode;
            return surface;
        }
        
        bool _textureLoadQueued = false;
        unsafe public void CreateHeightmapTextures( RenderTransform transform )
        {
            Console.WriteLine( "CreateHeightmapTextures()" );
            if( !transform.ReadyForUse() ) return;
            if( _textureLoadQueued ) return;
            _textureLoadQueued = true;
            
            if( LandHeightMap == null )
                LoadLandHeightMap( transform );
            if( WaterHeightMap == null )
                LoadWaterHeightMap( transform );
            
            transform.MainForm.UpdateStatusMessage( txtCreateTexturesNP );
            transform.MainForm.SetStatusProgressMinimum( 0 );
            transform.MainForm.SetStatusProgressMaximum( HeightMap_Height * 2 );
            transform.MainForm.UpdateStatusProgress( 0 );
            
            transform.Renderer.BeginInvoke(
                (r) => {
                    Console.WriteLine( "CreateHeightmapTextures() || transform.Renderer.BeginInvoke()" );
                    if( LandHeight_Texture != null )
                    {
                        LandHeight_Texture.Dispose();
                        LandHeight_Texture = null;
                    }
                    if( LandHeight_Surface == null )
                        LandHeight_Surface = CreateSurface( transform, LandHeightmapColor, 0, SDL.SDL_BlendMode.SDL_BLENDMODE_NONE );
                    LandHeight_Texture = transform.Renderer.CreateTextureFromSurface( LandHeight_Surface );
                    
                    if( WaterHeight_Texture != null )
                    {
                        WaterHeight_Texture.Dispose();
                        WaterHeight_Texture = null;
                    }
                    if( WaterHeight_Surface == null )
                        WaterHeight_Surface = CreateSurface( transform, WaterHeightmapColor, HeightMap_Height, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND );
                    WaterHeight_Texture = transform.Renderer.CreateTextureFromSurface( WaterHeight_Surface );
                    
                    transform.MainForm.UpdateStatusMessage( string.Format( txtCreateTextures, (int)100 ) );
                    _textureLoadQueued = false;
                } );
        }
        
        unsafe bool FetchPhysicalDDSInfo( string ddsFile )
        {
            if( !File.Exists( ddsFile ) )
                return false;
            
            FileStream ddsStream = null;
            try
            {
                ddsStream = File.Open( ddsFile, FileMode.Open );
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
            var ddsHeader = new DDS_Header();
            if( !NoBSFileAccess.Read( ddsStream, 0, &ddsHeader, (uint)sizeof( DDS_Header ) ) )
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
        
        unsafe bool LoadHeightMapDDS( string ddsFile, out Single[,] target, RenderTransform transform  )
        {
            var thingsOk = true;
            target = null;
            
            transform.MainForm.SetStatusProgressMinimum( 0 );
            transform.MainForm.SetStatusProgressMaximum( 0 );
            transform.MainForm.UpdateStatusProgress( 0 );
            transform.MainForm.UpdateStatusMessage( string.Format( "Loading {0}...", ddsFile ) );
            
            if( !File.Exists( ddsFile ) )
                return false;
            
            FileStream ddsStream = null;
            try
            {
                ddsStream = File.Open( ddsFile, FileMode.Open );
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
            
            var ddsHeader = new DDS_Header();
            if( !NoBSFileAccess.Read( ddsStream, 0, &ddsHeader, (uint)sizeof( DDS_Header ) ) )
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
                    transform.MainForm.SetStatusProgressMaximum( HeightMap_Height );
                    
                    transform.MainForm.UpdateStatusMessage( string.Format( "Loading heightmap...", 0, HeightMap_Height ) );
                    target = new Single[HeightMap_Height,HeightMap_Width];
                    
                    Single cp;
                    Single lp = -1f;
                    
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
                        transform.MainForm.UpdateStatusProgress( y );
                        cp = ((Single)y/ (Single)HeightMap_Height) * 100.0f;
                        if( (int)cp > (int)lp )
                        {
                            transform.MainForm.UpdateStatusMessage( string.Format( "Loading heightmap...{0}%", (int)cp ) );
                            lp = cp;
                        }
                        System.Threading.Thread.Sleep(0);
                    }
                    transform.MainForm.UpdateStatusMessage( string.Format( "Loading heightmap...{0}%", (int)100 ) );
                }
            }
            ddsStream.Close();
            return thingsOk;
        }
        
        #endregion
        
    }
}
