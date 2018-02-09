/*
 * bbSpaceConversions.cs
 * 
 * Global space conversion [extension] functions used by Border Builder.
 *
 * User: 1000101
 * Date: 09/02/2018
 * Time: 7:51 AM
 * 
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Border_Builder
{
    public static class bbSpaceConversions
    {
        
        #region Heightmap Conversions
        
        public static Maths.Vector2i HeightmapToCellGrid( int x, int y )
        {
            return new Maths.Vector2i(
                (  (int)((float)x / bbConstant.HeightMap_Resolution ) ),
                ( -(int)((float)y / bbConstant.HeightMap_Resolution ) )
           );
        }
        
        public static Maths.Vector2i HeightmapToCellGrid( this Maths.Vector2i v )
        {
            return HeightmapToCellGrid( v.X, v.Y );
        }
        
        #endregion
        
        #region Worldspace Conversions
        
        public static Maths.Vector2f WorldspaceToCellspace( float x, float y )
        {
            return new Maths.Vector2f(
                x * bbConstant.WorldMap_To_Heightmap,
                y * bbConstant.WorldMap_To_Heightmap
           );
        }
        
        public static Maths.Vector2f WorldspaceToCellspace( this Maths.Vector2f v )
        {
            return WorldspaceToCellspace( v.X, v.Y );
        }
        
        public static Maths.Vector2i WorldspaceToCellGrid( float x, float y )
        {
            return new Maths.Vector2i(
                ( (int)( x / bbConstant.WorldMap_Resolution ) ),
                ( (int)( y / bbConstant.WorldMap_Resolution ) )
           );
        }
        
        public static Maths.Vector2i WorldspaceToCellGrid( this Maths.Vector2f v )
        {
            return WorldspaceToCellGrid( v.X, v.Y );
        }
        
        public static Maths.Vector2i WorldspaceToHeightmap( float x, float y, int hmcX, int hmcY )
        {
            return WorldspaceToCellspace( x, y ).CellspaceToHeightmap( hmcX, hmcY );
        }
        
        public static Maths.Vector2i WorldspaceToHeightmap( float x, float y, Maths.Vector2i hmC )
        {
            return WorldspaceToCellspace( x, y ).CellspaceToHeightmap( hmC );
        }
        
        public static Maths.Vector2i WorldspaceToHeightmap( this Maths.Vector2f v, Maths.Vector2i hmC )
        {
            return v.WorldspaceToCellspace().CellspaceToHeightmap( hmC );
        }
        
        public static Maths.Vector2i WorldspaceToHeightmap( this Maths.Vector2f v, int hmcX, int hmcY )
        {
            return v.WorldspaceToCellspace().CellspaceToHeightmap( hmcX, hmcY );
        }
        
        #endregion
        
        #region CellGrid Conversions
        
        public static Maths.Vector2f CellGridToWorldspace( int x, int y )
        {
            return new Maths.Vector2f(
                x * bbConstant.WorldMap_Resolution,
                y * bbConstant.WorldMap_Resolution
            );
        }
        
        public static Maths.Vector2f CellGridToWorldspace( this Maths.Vector2i v )
        {
            return CellGridToWorldspace( v.X, v.Y );
        }
        
        public static Maths.Vector2i CellGridToHeightmap( int x, int y, int hmcX, int hmcY )
        {
            return CellspaceToHeightmap(
                (float)x * bbConstant.HeightMap_Resolution,
                (float)y * bbConstant.HeightMap_Resolution,
                hmcX, hmcY
            );
        }
        
        public static Maths.Vector2i CellGridToHeightmap( int x, int y, Maths.Vector2i hmC )
        {
            return CellGridToHeightmap( x, y, hmC.X, hmC.Y );
        }
        
        public static Maths.Vector2i CellGridToHeightmap( this Maths.Vector2i v, Maths.Vector2i hmC )
        {
            return CellGridToHeightmap( v.X, v.Y, hmC.X, hmC.Y );
        }
        
        public static Maths.Vector2i CellGridToHeightmap( this Maths.Vector2i v, int hmcX, int hmcY )
        {
            return CellGridToHeightmap( v.X, v.Y, hmcX, hmcY );
        }
        
        #endregion
        
        #region Cellspace Conversions
        
        public static Maths.Vector2f CellspaceToWorldspace( float x, float y )
        {
            return new Maths.Vector2f(
                x * bbConstant.HeightMap_To_Worldmap,
                y * bbConstant.HeightMap_To_Worldmap
            );
        }
        
        public static Maths.Vector2f CellspaceToWorldspace( this Maths.Vector2f v )
        {
            return CellspaceToWorldspace( v.X, v.Y );
        }
        
        public static Maths.Vector2i CellspaceToHeightmap( float x, float y, int hmcX, int hmcY )
        {
            return new Maths.Vector2i(
                hmcX + (int)x,
                hmcY - (int)y
            );
        }
        
        public static Maths.Vector2i CellspaceToHeightmap( float x, float y, Maths.Vector2i hmC )
        {
            return CellspaceToHeightmap( x, y, hmC.X, hmC.Y );
        }
        
        public static Maths.Vector2i CellspaceToHeightmap( this Maths.Vector2f v, Maths.Vector2i hmC )
        {
            return CellspaceToHeightmap( v.X, v.Y, hmC.X, hmC.Y );
        }
        
        public static Maths.Vector2i CellspaceToHeightmap( this Maths.Vector2f v, int hmcX, int hmcY )
        {
            return CellspaceToHeightmap( v.X, v.Y, hmcX, hmcY );
        }
        
        #endregion
        
        public static Maths.Vector2i SizeOfCellRange( Maths.Vector2i nw, Maths.Vector2i se )
        {
            return new Maths.Vector2i(
                Math.Abs( se.X - nw.X ) + 1,
                Math.Abs( nw.Y - se.Y ) + 1 );
        }
        
    }
}
