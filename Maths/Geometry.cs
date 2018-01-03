/*
 * Geometry.cs
 *
 * Geometry functions such as line-line intersect, area, containment, etc.
 *
 * User: 1000101
 * Date: 01/12/2017
 * Time: 9:16 AM
 * 
 */
using System;
using System.Collections.Generic;

namespace Border_Builder
{
    
    public static partial class Maths
    {
        /// <summary>
        /// Description of Geometry.
        /// </summary>
        public static class Geometry
        {
            
            // All shapes (triangles, rectangles and polygons):
            //
            // Can be ordered: CW or CCW, NOT RANDOMLY!
            //
            //         Ok                  Ok                 Not Ok
            //  0--------------1    0--------------3     0--------------2
            //  |              |    |              |     |              |
            //  |      CW      |    |     CCW      |     |   Randomly   |
            //  |              |    |              |     |              |
            //  3--------------2    1--------------2     3--------------1
            //
            // Functions default to CCW but can be explicitly told CW.
            
            public enum Orientation
            {
                Random = 0,
                CCW = -1,
                CW = 1
            }
            
            
            public enum CollisionType
            {
                NoCollision = 0,
                SinglePoint = 1,
                OverlappingRegions = 2,
                Collinear = 2,
                VertexMatch = 3
            }
            
            #region Intersection and overlap (AKA Collission)
            
            public static void Slope( float x1, float y1, float x2, float y2, out float A, out float B, out float C )
            {
                double tmpA, tmpB, tmpC;
                Slope( (double)x1, (double)y1, (double)x2, (double)y2, out tmpA, out tmpB, out tmpC );
                A = (float)tmpA;
                B = (float)tmpB;
                C = (float)tmpC;
            }
            
            public static void Slope( double x1, double y1, double x2, double y2, out double A, out double B, out double C )
            {
                A = -( y2 - y1 );
                B = x2 - x1;
                C = -( A * x1 + B * y1 );
            }
            
            public static class Collision
            {
            
                /// <summary>
                /// Determines if two line segments intersect.
                /// </summary>
                /// <param name="l1p1">Line 1, Point 1</param>
                /// <param name="l1p2">Line 1, Point 2</param>
                /// <param name="l2p1">Line 2, Point 1</param>
                /// <param name="l2p2">Line 2, Point 2</param>
                /// <param name="result">Insection point</param>
                /// <param name="threshold">Maximum distance points can be from each other to be considered the same</param>
                /// <returns>CollisionType</returns>
                public static CollisionType LineLineIntersect(
                    Vector2f l1p1, Vector2f l1p2,
                    Vector2f l2p1, Vector2f l2p2,
                    out Vector2f result,
                    float threshold = Maths.FLOAT_EPSILON )
                {
                    result = Vector2f.Zero;
                    float rX, rY;
                    var llResult = LineLineIntersect(
                        l1p1.X, l1p1.Y, l1p2.X, l1p2.Y,
                        l2p1.X, l2p1.Y, l2p2.X, l2p2.Y,
                        out rX, out rY,
                        threshold );
                    if( llResult != CollisionType.NoCollision )
                        result = new Vector2f( rX, rY );
                    return llResult;
                }
                
                /// <summary>
                /// Determines if two line segments intersect.
                /// </summary>
                /// <param name="l1x1">Line 1, X 1</param>
                /// <param name="l1y1">Line 1, Y 1</param>
                /// <param name="l1x2">Line 1, X 2</param>
                /// <param name="l1y2">Line 1, Y 2</param>
                /// <param name="l2x1">Line 2, X 1</param>
                /// <param name="l2y1">Line 2, Y 1</param>
                /// <param name="l2x2">Line 2, X 2</param>
                /// <param name="l2y2">Line 2, Y 2</param>
                /// <param name="rx">Intersect X</param>
                /// <param name="ry">Interesct Y</param>
                /// <param name="threshold">Maximum distance points can be from each other to be considered the same</param>
                /// <returns>CollisionType</returns>
                public static CollisionType LineLineIntersect(
                    float l1x1, float l1y1, float l1x2, float l1y2,
                    float l2x1, float l2y1, float l2x2, float l2y2,
                    out float rx, out float ry,
                    float threshold = Maths.FLOAT_EPSILON
                   )
                {
                    rx = 0;
                    ry = 0;
                    
                    // Try fast fails first
                    
                    // Zero-length lines
                    if(
                        ( ( l1x1.ApproximatelyEquals( l1x2, threshold ) )&&( l1y1.ApproximatelyEquals( l1y2, threshold ) ) )||
                        ( ( l2x1.ApproximatelyEquals( l2x2, threshold ) )&&( l2y1.ApproximatelyEquals( l2y2, threshold ) ) )
                       )
                    {
                        return CollisionType.NoCollision;
                    }
                    
                    // Shared end-point?
                    if(
                        ( ( l1x1.ApproximatelyEquals( l2x1, threshold ) )&&( l1y1.ApproximatelyEquals( l2y1, threshold ) ) )||
                        ( ( l1x1.ApproximatelyEquals( l2x2, threshold ) )&&( l1y1.ApproximatelyEquals( l2y2, threshold ) ) )
                       ){
                        rx = l1x1;
                        ry = l1y1;
                        return CollisionType.VertexMatch;
                    }
                    if(
                        ( ( l1x2.ApproximatelyEquals( l2x1, threshold ) )&&( l1y2.ApproximatelyEquals( l2y1, threshold ) ) )||
                        ( ( l1x2.ApproximatelyEquals( l2x2, threshold ) )&&( l1y2.ApproximatelyEquals( l2y2, threshold ) ) )
                       ){
                        rx = l1x2;
                        ry = l1y2;
                        return CollisionType.VertexMatch;
                    }
                    
                    // Now time for some maths...
                    
                    // Handle vertical lines having 0 delta X by adding epsilon if line 1 X's are equal
                    /*
                    const float verticalDelta = 0.01f;
                    if( l1x1.ApproximatelyEquals( l1x2, verticalDelta ) )
                    {
                        l1x1 -= verticalDelta;
                        l1x2 += verticalDelta;
                    }
                    if( l2x1.ApproximatelyEquals( l2x2, verticalDelta ) )
                    {
                        l2x1 -= verticalDelta;
                        l2x2 += verticalDelta;
                    }
                    */
                    
                    // Translate lines so l1p1 is on the origin
                    l1x2 -= l1x1; l1y2 -= l1y1;
                    l2x1 -= l1x1; l2y1 -= l1y1;
                    l2x2 -= l1x1; l2y2 -= l1y1;
                    
                    // Line 1 length
                    var l1Length = (float)Math.Sqrt( ( l1x2 * l1x2 ) + ( l1y2 * l1y2 ) );
                    
                    // Rotate lines so line 1 is horizontal along the positive X axis
                    var rCos = l1x2 / l1Length;
                    var rSin = l1y2 / l1Length;
                    var nx1 = l2x1 * rCos + l2y1 * rSin;
                    var ny1 = l2y1 * rCos - l2x1 * rSin;
                    var nx2 = l2x2 * rCos + l2y2 * rSin;
                    var ny2 = l2y2 * rCos - l2x2 * rSin;
                    
                    // Line 2 must cross the X axis
                    if(
                        ( ( ny1 <  0f )&&( ny2 <  0f ) )||
                        ( ( ny1 >= 0f )&&( ny2 >= 0f ) )
                       ) return CollisionType.NoCollision;
                    
                    // Intersection point on line 1
                    var iPos = nx2 + ( nx1 - nx2 ) * ny2 / ( ny2 - ny1 );
                    
                    // Intersection must be somewhere on l1
                    if( ( iPos < 0 )||( iPos > l1Length ) ) return CollisionType.NoCollision;
                    
                    // Translate the intersection back to the original co-ordinate system
                    rx = l1x1 + iPos * rCos;
                    ry = l1y1 + iPos * rSin;
                    
                    // Intersection found
                    if(
                        ( iPos.ApproximatelyEquals( 0f, threshold ) )||
                        ( iPos.ApproximatelyEquals( l1Length, threshold ) )
                    )
                        return CollisionType.VertexMatch;
                    
                    return CollisionType.SinglePoint;
                    
                }
            
                /// <summary>
                /// Determines if an arbitrary point is inside a polygon made of a minimum of three points
                /// </summary>
                /// <param name="p">Point</param>
                /// <param name="points">Points making up the polygon</param>
                /// <param name="order">Order of points that make up the polygon, clockwise (CW) or counter clockwise (CCW)</param>
                /// <returns>true if the point is inside the polygon</returns>
                public static bool PointInPoly(
                    Vector2f p,
                    Vector2f[] points,
                    Orientation order = Orientation.CCW
                   )
                {
                    if( ( order != Orientation.CW )&&( order != Orientation.CCW ) ) return false;
                    
                    var xp = (double)p.X;
                    var yp = (double)p.Y;
                    
                    double A, B, C;
                    int max = points.Length;
                    int next;
                    for( int index = 0; index < max; index++ )
                    {
                        next = ( index + 1 ) % max;
                        
                        var x1 = (double)points[ index ].X;
                        var y1 = (double)points[ index ].Y;
                        var x2 = (double)points[ next ].X;
                        var y2 = (double)points[ next ].Y;
                        
                        // If the point isn't on the vertex, do a slope comparison (points on vertices are automatically "inside")
                        if( ( !x1.ApproximatelyEquals( x2 ) )||( !y1.ApproximatelyEquals( y2 ) ) )
                        {
                            Slope( x1, y1, x2, y2, out A, out B, out C );
                            var D = A * xp + B * yp + C;
                            
                            if(
                                ( ( order == Orientation.CW  )&&( D > 0.0d ) )||
                                ( ( order == Orientation.CCW )&&( D < 0.0d ) )
                            ) return false;
                        }
                    }
                    return true;
                }
                
                /// <summary>
                /// Determines if any of a set of points is inside a polygon made of a minimum of three points
                /// </summary>
                /// <param name="p">Set of points to test</param>
                /// <param name="points">Points making up the polygon</param>
                /// <param name="order">Order of points that make up the polygon, clockwise (CW) or counter clockwise (CCW)</param>
                /// <returns>-1 if no points are inside the polygon or the count of the points detected and pin filled with the inside points</returns>
                public static int PointsInPoly(
                    Vector2f[] p,
                    Vector2f[] points,
                    out Vector2f[] pin,
                    Orientation order = Orientation.CCW
                   )
                {
                    pin = null;
                    
                    if( ( order != Orientation.CW )&&( order != Orientation.CCW ) ) return -1;
                    
                    int pMax = p.Length;
                    if( pMax < 1 ) return -1;
                    
                    var tmpList = new List<Vector2f>();
                    
                    // Do the heavy lifting once by computing the slopes of the entire polygon first
                    int max = points.Length;
                    var A = new double[ max ];
                    var B = new double[ max ];
                    var C = new double[ max ];
                    
                    int next;
                    for( int index = 0; index < max; index++ )
                    {
                        next = ( index + 1 ) % max;
                        
                        var x1 = (double)points[ index ].X;
                        var y1 = (double)points[ index ].Y;
                        var x2 = (double)points[ next ].X;
                        var y2 = (double)points[ next ].Y;
                        
                        Slope( x1, y1, x2, y2, out A[ index ], out B[ index ], out C[ index ] );
                    }
                    
                    for( int pIndex = 0; pIndex < pMax; pIndex++ )
                    {
                        var xp = (double)p[ pIndex ].X;
                        var yp = (double)p[ pIndex ].Y;
                        var addPoint = true;
                        
                        for( int index = 0; index < max; index++ )
                        {
                            // If the point isn't on the vertex, do a slope comparison (points on vertices are automatically "inside")
                            if( ( !xp.ApproximatelyEquals( points[ index ].X ) )||( !yp.ApproximatelyEquals( points[ index ].Y ) ) )
                            {
                                var D = A[ pIndex ] * xp + B[ pIndex ] * yp + C[ pIndex ];
                                
                                if(
                                    ( ( order == Orientation.CW  )&&( D > 0.0d ) )||
                                    ( ( order == Orientation.CCW )&&( D < 0.0d ) )
                                   ){
                                    addPoint = false;
                                    break;
                                }
                            }
                        }
                        if( addPoint ) tmpList.Add( p[ pIndex ] );
                    }
                    
                    if( tmpList.Count == 0 )
                        return -1;
                    
                    pin = tmpList.ToArray();
                    return pin.Length;
                }
                
            }
            
            
            #endregion
            
            
            #region Area's of polygons
            
            public static class Area
            {
                
                public static float Triangle( Vector2f[] p )
                {
                    return Triangle(
                        p[0].X, p[0].Y,
                        p[1].X, p[1].Y,
                        p[2].X, p[2].Y );
                }
                
                public static float Triangle(
                    Vector2f p0,
                    Vector2f p1,
                    Vector2f p2
                   )
                {
                    return Triangle(
                        p0.X, p0.Y,
                        p1.X, p1.Y,
                        p2.X, p2.Y );
                }
                
                public static float Triangle(
                    float x0, float y0,
                    float x1, float y1,
                    float x2, float y2
                   )
                {
                    /*
                          1 | x0 y0 1 |
                      A = - | x1 y1 1 |
                          2 | x2 y2 1 |
                    */
                    //float rval = ( ( x0 * ( y1 - y2 ) ) + ( x1 * ( y2 - y0 ) ) + ( x2 * ( y0 - y1 ) ) ) * 0.5f;
                    float rval = ( x2 * y1 - x1 * y2 )-( x2 * y0 - x0 * y2 )-( x1 * y0 - x0 * y1 );
                    return Math.Abs( rval );
                }
                
                public static float Rect( Vector2f[] rect )
                {
                    return
                        Area.Triangle( rect[ 0 ], rect[ 1 ], rect[ 2 ] ) +
                        Area.Triangle( rect[ 0 ], rect[ 2 ], rect[ 3 ] );
                }
                
                public static float Polygon( Vector2f[] points )
                {
                    float cX = 0f;
                    float cY = 0f;
                    var last = points.Length;
                    
                    foreach( var point in points )
                    {
                        cX += point.X;
                        cY += point.Y;
                    }
                    cX /= (float)( last );
                    cY /= (float)( last );
                    
                    float area = 0f;
                    last--;
                    for( int index = 0; index < last; index++ )
                        area += Triangle(
                            cX, cY,
                            points[ index ].X, points[ index ].Y,
                            points[ index + 1 ].X, points[ index + 1 ].Y );
                    
                    area += Triangle(
                        cX, cY,
                        points[ last ].X, points[ last ].Y,
                        points[ 0 ].X, points[ 0 ].Y );
                    
                    return area;
                }
            }
            
            #endregion
            
            
        }
    }
}