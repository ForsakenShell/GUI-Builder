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

namespace Border_Builder
{
    /// <summary>
    /// Description of bbWorldspace.
    /// </summary>
    public class bbWorldspace
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
        //public int CellRangeMinX = 0;
        //public int CellRangeMaxX = 0;
        //public int CellRangeMinY = 0;
        //public int CellRangeMaxY = 0;
        
        // Height map data (extracted from DDS)
        public int HeightMap_Width = 0;
        public int HeightMap_Height = 0;
        public Single[,] LandHeightMap = null;
        public Single[,] WaterHeightMap = null;
        public Bitmap LandHeight_Bitmap = null;
        public Bitmap WaterHeight_Bitmap = null;
        
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
        
        #region Preloading
        
        public void Preload()
        {
            LoadStatsFile();
            LoadCellRangeFile();
        }
        
        #endregion
        
        #region File loading and parsing
        
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
            /*
CellRangeMinX=-42
CellRangeMaxX=38
CellRangeMinY=-46
CellRangeMaxY=36
            */
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
        
        public bool LoadLandHeightMap( fMain fmain )
        {
            return LoadHeightMapDDS( LandHeights_Texture_File, out LandHeightMap, fmain );
        }
        
        public bool LoadWaterHeightMap( fMain fmain )
        {
            return LoadHeightMapDDS( WaterHeights_Texture_File, out WaterHeightMap, fmain );
        }
        
        unsafe public void RenderHeightMap( fMain fmain )
        {
            
            if( LandHeightMap == null )
                LoadLandHeightMap( fmain );
            if( WaterHeightMap == null )
                LoadWaterHeightMap( fmain );
            
            fmain.UpdateStatusMessage( "Creating bitmaps..." );
            fmain.SetStatusProgressMinimum( 0 );
            fmain.SetStatusProgressMaximum( HeightMap_Height * 2 );
            fmain.UpdateStatusProgress( 0 );
            
            Single cp;
            Single lp = -1f;
            LandHeight_Bitmap = new Bitmap( HeightMap_Width, HeightMap_Height );
            for( int y = 0; y < HeightMap_Height; y++ )
            {
                var pixels = LandHeight_Bitmap.LockBits( new Rectangle( 0, y, HeightMap_Width, 1 ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
                var scanline = (int*)pixels.Scan0.ToPointer();
                {
                    for( int x = 0; x < HeightMap_Width; x++ )
                    {
                        var lh = LandHeightMap[ y, x ];
                        var f = 255 * lh;
                        var i = (byte)Math.Round( f );
                        unchecked
                        {
                            scanline[ x ] = (int)0xFF000000 | ( i << 16 ) | ( i << 8 ) | i;
                        }
                    }
                }
                LandHeight_Bitmap.UnlockBits( pixels );
                fmain.UpdateStatusProgress( y );
                cp = ((Single)y / ((Single)HeightMap_Height * 2.0f)) * 100.0f;
                if( (int)cp > (int)lp )
                {
                    fmain.UpdateStatusMessage( string.Format( "Creating bitmaps...{0}%", (int)cp ) );
                    lp = cp;
                }
                System.Threading.Thread.Sleep(0);
            }
            
            WaterHeight_Bitmap = new Bitmap( HeightMap_Width, HeightMap_Height );
            for( int y = 0; y < HeightMap_Height; y++ )
            {
                var pixels = WaterHeight_Bitmap.LockBits( new Rectangle( 0, y, HeightMap_Width, 1 ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
                var scanline = (int*)pixels.Scan0.ToPointer();
                {
                    for( int x = 0; x < HeightMap_Width; x++ )
                    {
                        var wh = WaterHeightMap[ y, x ];
                        var lh = LandHeightMap[ y, x ];
                        scanline[ x ] = wh > lh ? 0x7F00007F : 0;
                    }
                }
                WaterHeight_Bitmap.UnlockBits( pixels );
                fmain.UpdateStatusProgress( HeightMap_Height + y );
                cp = (((Single)HeightMap_Height + (Single)y )/ ((Single)HeightMap_Height * 2.0f)) * 100.0f;
                if( (int)cp > (int)lp )
                {
                    fmain.UpdateStatusMessage( string.Format( "Creating bitmaps...{0}%", (int)cp ) );
                    lp = cp;
                }
                System.Threading.Thread.Sleep(0);
            }
            fmain.UpdateStatusMessage( string.Format( "Creating bitmaps...{0}%", (int)100 ) );
        }
        
        unsafe bool LoadHeightMapDDS( string ddsFile, out Single[,] target, fMain fmain  )
        {
            var thingsOk = true;
            target = null;
            
            fmain.SetStatusProgressMinimum( 0 );
            fmain.SetStatusProgressMaximum( 0 );
            fmain.UpdateStatusProgress( 0 );
            fmain.UpdateStatusMessage( string.Format( "Loading {0}...", ddsFile ) );
            
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
                    fmain.SetStatusProgressMaximum( HeightMap_Height );
                    
                    fmain.UpdateStatusMessage( string.Format( "Loading heightmap...", 0, HeightMap_Height ) );
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
                        fmain.UpdateStatusProgress( y );
                        cp = ((Single)y/ (Single)HeightMap_Height) * 100.0f;
                        if( (int)cp > (int)lp )
                        {
                            fmain.UpdateStatusMessage( string.Format( "Loading heightmap...{0}%", (int)cp ) );
                            lp = cp;
                        }
                        System.Threading.Thread.Sleep(0);
                    }
                    fmain.UpdateStatusMessage( string.Format( "Loading heightmap...{0}%", (int)100 ) );
                }
            }
            ddsStream.Close();
            return thingsOk;
        }
    }
}
