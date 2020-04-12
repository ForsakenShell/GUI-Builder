/*
 * SpaceConversions.cs
 * 
 * Global space conversion [extension] functions used by Border Builder.
 *
 */

using System;

using Maths;

namespace Engine
{
    
    public static class SpaceConversions
    {
        
        /*
         Heightmap starts in top-left corner at (0,0) and extends down to the right at (+x, +y) (row major array)
         Worldspace extends from the top-left at (-x,+y) to the bottom right at (+x,-y) (standard euler coords)
         
         W<->HM
            +X maps to +X
            +Y maps to -Y
         
        */
        
        #region Heightmap Conversions
        
        public static Vector2i HeightmapToCellGrid( int x, int y, int hmcX, int hmcY )
        {
            var tx = (float)( x - hmcX );
            var ty = (float)( hmcY - y );
            var gx = (int)( tx / Constant.HeightMap_Resolution );
            var gy = (int)( ty / Constant.HeightMap_Resolution );
            if( x < hmcX ) gx--;
            if( y > hmcY ) gy--;
            return new Vector2i( gx, gy );
        }
        
        public static Vector2f HeightmapToWorldspace( int x, int y, int hmcX, int hmcY )
        {
            return new Vector2f(
                ( ((float)( x - hmcX ) ) * Constant.HeightMap_To_Worldmap ),
                ( ((float)( hmcY - y ) ) * Constant.HeightMap_To_Worldmap )
           );
        }
        
        public static Vector2i HeightmapToCellGrid( int x, int y, Vector2i hmC )
        {
            return HeightmapToCellGrid( x, y, hmC.X, hmC.Y );
        }
        public static Vector2i HeightmapToCellGrid( this Vector2i v, int hmcX, int hmcY )
        {
            return HeightmapToCellGrid( v.X, v.Y, hmcX, hmcY );
        }
        public static Vector2i HeightmapToCellGrid( this Vector2i v, Vector2i hmC )
        {
            return HeightmapToCellGrid( v.X, v.Y, hmC.X, hmC.Y );
        }
        
        public static Vector2f HeightmapToWorldspace( int x, int y, Vector2i hmC )
        {
            return HeightmapToWorldspace( x, y, hmC.X, hmC.Y );
        }
        public static Vector2f HeightmapToWorldspace( this Vector2i v, int hmcX, int hmcY )
        {
            return HeightmapToWorldspace( v.X, v.Y, hmcX, hmcY );
        }
        public static Vector2f HeightmapToWorldspace( this Vector2i v, Vector2i hmC )
        {
            return HeightmapToWorldspace( v.X, v.Y, hmC.X, hmC.Y );
        }
        
        #endregion
        
        #region Worldspace Conversions
        
        public static Vector2i WorldspaceToCellGrid( float x, float y )
        {
            var cx = (int)( x / Constant.WorldMap_Resolution );
            var cy = (int)( y / Constant.WorldMap_Resolution );
            if( x < 0.0f ) cx--;
            if( y < 0.0f ) cy--;
            return new Vector2i( cx, cy );
        }
        
        public static Vector2i WorldspaceToHeightmap( float x, float y )
        {
            var result = new Vector2i(
                (int)( x * Constant.WorldMap_To_Heightmap ),
                (int)( y * Constant.WorldMap_To_Heightmap )
            );
            return result;
        }
        public static Vector2i WorldspaceToHeightmap( this Vector2f v )
        {
            return  WorldspaceToHeightmap( v.X, v.Y );
        }
        
        public static Vector2i WorldspaceToCellGrid( this Vector2f v )
        {
            return WorldspaceToCellGrid( v.X, v.Y );
        }
        public static Vector2i WorldspaceToCellGrid( this Vector3f v )
        {
            return WorldspaceToCellGrid( v.X, v.Y );
        }
        
        /*
        public static Vector2i WorldspaceToHeightmap( float x, float y, Vector2i hmC )
        {
            return WorldspaceToHeightmap( x, y, hmC.X, hmC.Y );
        }
        public static Vector2i WorldspaceToHeightmap( this Vector2f v, int hmcX, int hmcY )
        {
            return WorldspaceToHeightmap( v.X, v.Y, hmcX, hmcY );
        }
        public static Vector2i WorldspaceToHeightmap( this Vector3f v, int hmcX, int hmcY )
        {
            return WorldspaceToHeightmap( v.X, v.Y, hmcX, hmcY );
        }
        public static Vector2i WorldspaceToHeightmap( this Vector3f v, Vector2i hmC )
        {
            return  WorldspaceToHeightmap( v.X, v.Y, hmC.X, hmC.Y );
        }
        */

        #endregion
        
        #region CellGrid Conversions
        
        public static Vector2i CellGridToHeightmap( int x, int y, int hmcX, int hmcY )
        {
            return new Vector2i(
                hmcX + (int)( x * Constant.HeightMap_Resolution ),
                hmcY - (int)( y * Constant.HeightMap_Resolution ) );
        }
        
        public static Vector2f CellGridToWorldspace( int x, int y )
        {
            return new Vector2f(
                x * Constant.WorldMap_Resolution,
                y * Constant.WorldMap_Resolution );
        }
        
        public static Vector2i CellGridToHeightmap( int x, int y, Vector2i hmC )
        {
            return CellGridToHeightmap( x, y, hmC.X, hmC.Y );
        }
        public static Vector2i CellGridToHeightmap( this Vector2i v, int hmcX, int hmcY )
        {
            return CellGridToHeightmap( v.X, v.Y, hmcX, hmcY );
        }
        public static Vector2i CellGridToHeightmap( this Vector2i v, Vector2i hmC )
        {
            return CellGridToHeightmap( v.X, v.Y, hmC.X, hmC.Y );
        }
        
        public static Vector2f CellGridToWorldspace( this Vector2i v )
        {
            return CellGridToWorldspace( v.X, v.Y );
        }
        
        #endregion
        
        #region Unit Type Calculations (SizeOfCellRange, etc)
        
        public static Vector2i SizeOfCellRange( Vector2i nw, Vector2i se )
        {
            return new Vector2i(
                Math.Abs( se.X - nw.X ) + 1,
                Math.Abs( nw.Y - se.Y ) + 1 );
        }
        
        #endregion
        
    }
}