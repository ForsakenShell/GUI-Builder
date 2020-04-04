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

namespace Maths
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
            EndPoint = 2,
            VertexMatch = 3,
            Collinear = 4,
            OverlappingRegions = 5
        }


        #region Intersection and overlap (AKA Collision)

        public static float Angle( float x1, float y1, float x2, float y2 )
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return ( 360.0f + (float)( Math.Atan2( dy, dx ) * Maths.Constant.RAD_TO_DEG ) ) % 360.0f;
        }

        public static float Angle( Vector2f p1, Vector2f p2 )
        {
            return Angle( p1.X, p1.Y, p2.X, p2.Y );
        }

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
                float threshold = Constant.FLOAT_EPSILON )
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
                float threshold = Constant.FLOAT_EPSILON )
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
                    //DebugLog.Write( "Geometry.LineLineIntersect :: Zero-Length line, l1p1 ~= l1p2 or l2p1 ~= l2p2" );
                    return CollisionType.NoCollision;
                }
                
                // Shared end-point?
                if(
                    ( ( l1x1.ApproximatelyEquals( l2x1, threshold ) )&&( l1y1.ApproximatelyEquals( l2y1, threshold ) ) )||
                    ( ( l1x1.ApproximatelyEquals( l2x2, threshold ) )&&( l1y1.ApproximatelyEquals( l2y2, threshold ) ) )
                   ){
                    rx = l1x1;
                    ry = l1y1;
                    //DebugLog.Write( "Geometry.LineLineIntersect :: l1p1 end-point match with l2" );
                    return CollisionType.VertexMatch;
                }
                if(
                    ( ( l1x2.ApproximatelyEquals( l2x1, threshold ) )&&( l1y2.ApproximatelyEquals( l2y1, threshold ) ) )||
                    ( ( l1x2.ApproximatelyEquals( l2x2, threshold ) )&&( l1y2.ApproximatelyEquals( l2y2, threshold ) ) )
                   ){
                    rx = l1x2;
                    ry = l1y2;
                    //DebugLog.Write( "Geometry.LineLineIntersect :: l1p2 end-point match with l2" );
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
                //var t1x1 = 0.0f;
                //var t1y1 = 0.0f;
                var t1x2 = l1x2 - l1x1;
                var t1y2 = l1y2 - l1y1;
                var t2x1 = l2x1 - l1x1;
                var t2y1 = l2y1 - l1y1;
                var t2x2 = l2x2 - l1x1;
                var t2y2 = l2y2 - l1y1;
                
                // Translated Line 1 length
                var l1Length = (float)Math.Sqrt( ( t1x2 * t1x2 ) + ( t1y2 * t1y2 ) );
                
                // Rotate translated lines so line 1 is horizontal along the positive X axis
                var rCos = t1x2 / l1Length;
                var rSin = t1y2 / l1Length;
                var nx1 = t2x1 * rCos + t2y1 * rSin;
                var ny1 = t2y1 * rCos - t2x1 * rSin;
                var nx2 = t2x2 * rCos + t2y2 * rSin;
                var ny2 = t2y2 * rCos - t2x2 * rSin;
                
                // Translated Line 2 must cross the X axis
                if(
                    ( ( ny1 <  0f )&&( ny2 <  0f ) )||
                    ( ( ny1 >= 0f )&&( ny2 >= 0f ) )
                   )
                {
                    /*
                    DebugLog.Write( string.Format(
                        "Geometry.LineLineIntersect :: l2 does not cross l1 :: (({0},{1})-({2},{3})) x (({4},{5})-({6},{7}))",
                        l1x1, l1y1, l1x2, l1y2,
                        l2x1, l2y1, l2x2, l2y2 ) );
                    */
                    return CollisionType.NoCollision;
                }
                
                // Intersection point on line 1
                var iPos = nx2 + ( nx1 - nx2 ) * ny2 / ( ny2 - ny1 );
                
                // Intersection must be somewhere on l1
                if( ( iPos < 0 )||( iPos > l1Length ) )
                {
                    //DebugLog.Write( "Geometry.LineLineIntersect :: Collision beyond end of p1" );
                    return CollisionType.NoCollision;
                }
                
                // Translate the intersection back to the original co-ordinate system
                rx = l1x1 + iPos * rCos;
                ry = l1y1 + iPos * rSin;
                
                // Intersection found
                /* This should be handled already above when checking for end-point matches
                if(
                    ( iPos.ApproximatelyEquals( 0f, threshold ) )||
                    ( iPos.ApproximatelyEquals( l1Length, threshold ) )
                )
                    return CollisionType.EndPoint;
                */
                
                /*
                DebugLog.Write( string.Format(
                    "Geometry.LineLineIntersect :: Collision at ({8},{9}) = (({0},{1})-({2},{3})) x (({4},{5})-({6},{7}))",
                    l1x1, l1y1, l1x2, l1y2,
                    l2x1, l2y1, l2x2, l2y2,
                    rx, ry
                    ) );
                */
                return CollisionType.SinglePoint;
                
            }

            /// <summary>
            /// Determines if a ray intersects with a triangle
            /// </summary>
            /// <param name="ray">The ray to test</param>
            /// <param name="triangle">The triangle to test</param>
            /// <param name="result">The point on the triangle should the ray intersect it</param>
            /// <param name="order">The vertex order of the triangle</param>
            /// <returns>true if the ray intersects the triangle, false otherwise</returns>
            public static bool RayTriangleIntersect(
                Ray ray,
                Vector3f[] triangle,
                out Vector3f result,
                Orientation order = Orientation.CW )
            {
                result = Vector3f.Zero;
                var bResult = false;

                if( ( ray.Origin == null )||( ray.Direction == null ) ) goto localReturnResult;
                if( ( triangle == null )||( triangle.Length != 3 ) ) goto localReturnResult;
                if( ( order != Orientation.CCW )&&( order != Orientation.CW ) ) goto localReturnResult;

                var tri     = new Vector3f[ 3 ]{
                    triangle[ ( order == Orientation.CCW ) ? 2 : 0 ],
                    triangle[ 1 ],
                    triangle[ ( order == Orientation.CCW ) ? 0 : 2 ]
                };

                var u       = tri[ 1 ] - tri[ 0 ];
                var v       = tri[ 2 ] - tri[ 0 ];
                var n       =  Vector3f.Cross( u, v );
                if( n.IsZero() ) goto localReturnResult;

                var w0      = ray.Origin - tri[ 0 ];
                var a       = -Vector3f.Dot( n, w0 );
                var b       =  Vector3f.Dot( n, ray.Direction );
                if( b.ApproximatelyEquals( 0.0f ) ) goto localReturnResult;

                var r       = a / b;
                if( r < 0.0f ) goto localReturnResult;

                var I       = ray.Origin + ray.Direction * r;

                var uu      = Vector3f.Dot( u, u );
                var uv      = Vector3f.Dot( u, v );
                var vv      = Vector3f.Dot( v, v );
                var w       = I - triangle[ 0 ];
                var wu      = Vector3f.Dot( w, u );
                var wv      = Vector3f.Dot( w, v );
                var D       = uv * uv - uu * vv;

                var s       = ( uv * wv - vv * wu ) / D;
                if( ( s < 0.0f )||(   s       > 1.0f ) ) goto localReturnResult;

                var t       = ( uv * wu - uu * wv ) / D;
                if( ( t < 0.0f )||( ( s + t ) > 1.0f ) ) goto localReturnResult;

                result  = I;
                bResult = true;

            localReturnResult:
                return bResult;
            }

            /// <summary>
            /// Determines if an arbitrary point is inside a polygon made of a minimum of three points
            /// Note:  Only works with convex polygons.
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
                var maxPoints = points == null ? 0 : points.Length;
                if( maxPoints < 1 ) return false;
                
                var xp = (double)p.X;
                var yp = (double)p.Y;
                
                //int intersects = 0;
                //float rx, ry;
                double A, B, C;
                int next;
                for( int index = 0; index < maxPoints; index++ )
                {
                    next = ( index + 1 ) % maxPoints;
                    
                    var x1 = (double)points[ index ].X;
                    var y1 = (double)points[ index ].Y;
                    var x2 = (double)points[ next  ].X;
                    var y2 = (double)points[ next  ].Y;
                    
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
                    
                    /*
                    // Count how many polygon edges a line from the point extended along the x +axis infinity crosses
                    if( LineLineIntersect(
                        p.X, p.Y, float.MaxValue, p.Y,
                        points[ index ].X, points[ index ].Y,
                        points[ next  ].X, points[ next  ].Y,
                        out rx, out ry
                    ) == CollisionType.SinglePoint )
                        intersects++;
                   */
                }
                return true;
                // An odd number of intersects means the point is inside the polygon
                //return ( ( intersects & 1 ) != 0 );
            }
            
            /// <summary>
            /// Determines if an arbitrary point is inside a group of polygons.
            /// Internally calls PointInPoly for each polygon.
            /// Note:  Only works with convex polygons.
            /// </summary>
            /// <param name="p">Point</param>
            /// <param name="polys">Polygons making up the group</param>
            /// <param name="order">Order of points that make up the polygon, clockwise (CW) or counter clockwise (CCW)</param>
            /// <returns>true if the point is inside the polygon</param>
            /// <returns>Polygon array index of the first polygon the point is found in or -1 for all other conditions (error, no poly found, etc).</returns>
            public static int PointInPolys(
                Vector2f p,
                Vector2f[][] polys,
                Orientation order = Orientation.CCW )
            {
                if( ( order != Orientation.CW )&&( order != Orientation.CCW ) ) return -1;
                var maxPolys = polys == null ? 0 : polys.Length;
                if( maxPolys < 1 ) return -1;
                
                for( int i = 0; i < maxPolys; i++ )
                    if( PointInPoly( p, polys[ i ], order ) ) return i;
                
                return -1;
            }
            
            /// <summary>
            /// Determines if any of a set of points is inside a polygon made of a minimum of three points
            /// Note:  Only works with convex polygons.
            /// </summary>
            /// <param name="p">Set of points to test</param>
            /// <param name="points">Points making up the polygon</param>
            /// <param name="pin">Points in polygon (return)</param>
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
        

        #region Convex Hulls
        
        public static class ConvexHull
        {
            
            public class OptimalBoundingBox
            {
                #region Size
                Vector3f _Size = Vector3f.Zero;
                public Vector3f Size
                {
                    get{ return _Size; }
                    set{ _Size = new Vector3f( value ); }
                }
                public Vector2f Size2D
                {
                    get{ return new Vector2f( _Size ); }
                    set{ _Size = new Vector3f( value.X, value.Y, Height ); }
                }
                public float Height
                {
                    get{ return _Size.Z; }
                    set{ _Size.Z = value; }
                }
                #endregion
                
                #region Position
                Vector3f _Position = Vector3f.Zero;
                public Vector3f Position
                {
                    get{ return _Position; }
                    set{ _Position = new Vector3f( value ); }
                }
                public Vector2f Position2D
                {
                    get{ return new Vector2f( _Position ); }
                    set{ _Position = new Vector3f( value.X, value.Y, Z ); }
                }
                public float Z
                {
                    get{ return _Position.Z; }
                    set{ _Position.Z = value; }
                }
                #endregion
                
                #region Rotation
                Vector3f _Rotation = Vector3f.Zero;
                public Vector3f Rotation
                {
                    get{ return _Rotation; }
                    set{ _Rotation = new Vector3f( value ); }
                }
                public float ZRotation
                {
                    get{ return _Rotation.Z; }
                    set{ _Rotation.Z = value; }
                }
                #endregion
                
                public OptimalBoundingBox( Vector2f size, Vector2f position, float rotation )
                {
                    _Size = new Vector3f( size );
                    _Position = new Vector3f( position );
                    ZRotation = rotation;
                    //Dump( "cTor()" );
                }
                
                public Vector2f[] Corners
                {
                    get
                    {
                        var p2D = Position2D;
                        var hSize = _Size * 0.5f;
                        return new Vector2f[]
                        {
                            Vector2f.RotateAround( new Vector2f( _Position.X - hSize.X, _Position.Y - hSize.Y ), p2D, - ZRotation ),
                            Vector2f.RotateAround( new Vector2f( _Position.X + hSize.X, _Position.Y - hSize.Y ), p2D, - ZRotation ),
                            Vector2f.RotateAround( new Vector2f( _Position.X + hSize.X, _Position.Y + hSize.Y ), p2D, - ZRotation ),
                            Vector2f.RotateAround( new Vector2f( _Position.X - hSize.X, _Position.Y + hSize.Y ), p2D, - ZRotation )
                        };
                    }
                }
                
                public override string ToString()
                {
                    return string.Format(
                        "[Size = {0} :: Position = {1} :: Rotation = {2}]",
                        _Size.ToString(),
                        _Position.ToString(),
                        _Rotation.ToString() );
                }
                
            }
            
            // This section was lifted from two tutorials which can be found here:
            // http://csharphelper.com/blog/2014/07/find-the-convex-hull-of-a-set-of-points-in-c/
            // http://csharphelper.com/blog/2014/07/find-minimal-bounding-rectangle-polygon-c/
            
            public static OptimalBoundingBox MinBoundingBox( List<Vector2f> hull )
            {
                //DebugLog.OpenIndentLevel();
                //DebugLog.WriteList<Vector2f>( "hull", hull, false, true );
                
                var numPoints = hull.Count;
                //var hull = hull.ToArray();
                
                float minx = hull[ 0 ].X;
                float maxx = minx;
                float miny = hull[ 0 ].Y;
                float maxy = miny;
                int minxi = 0, maxxi = 0, minyi = 0, maxyi = 0;
                
                for( int i = 1; i < numPoints; i++ )
                {
                    if( minx > hull[ i ].X )
                    {
                        minx = hull[ i ].X;
                        minxi = i;
                    }
                    if( maxx < hull[ i ].X )
                    {
                        maxx = hull[ i ].X;
                        maxxi = i;
                    }
                    if( miny > hull[ i ].Y )
                    {
                        miny = hull[ i ].Y;
                        minyi = i;
                    }
                    if( maxy < hull[ i ].Y )
                    {
                        maxy = hull[ i ].Y;
                        maxyi = i;
                    }
                }
                
                var controlPoint = new [] { minxi, maxyi, maxxi, minyi };
                int currentControlPoint = -1;
                
                // Set the "best" rect to a valid worst possibilty
                float bestArea, bestS1, bestS2;
                Vector2f[] bestRect;
                ComputeBoundingRectangle( out bestRect, out bestArea, out bestS1, out bestS2,
                    hull[ controlPoint[ 0 ] ].X, hull[ controlPoint[ 0 ] ].Y,  0,  1,
                    hull[ controlPoint[ 1 ] ].X, hull[ controlPoint[ 1 ] ].Y,  1,  0,
                    hull[ controlPoint[ 2 ] ].X, hull[ controlPoint[ 2 ] ].Y,  0, -1,
                    hull[ controlPoint[ 3 ] ].X, hull[ controlPoint[ 3 ] ].Y, -1,  0 );
                
                for( int i = 0; i < numPoints; i++ )
                {
                    // Increment the current control point
                    if( currentControlPoint >= 0 )
                        controlPoint[ currentControlPoint ] = ( controlPoint[ currentControlPoint ] + 1 ) % numPoints;
                    
                    // Find the next point on an edge
                    float xmindx, xmindy, ymindx, ymindy;
                    float xmaxdx, xmaxdy, ymaxdx, ymaxdy;
                    FindDxDy( out xmindx, out xmindy, controlPoint[ 0 ], hull, numPoints );
                    FindDxDy( out ymaxdx, out ymaxdy, controlPoint[ 1 ], hull, numPoints );
                    FindDxDy( out xmaxdx, out xmaxdy, controlPoint[ 2 ], hull, numPoints );
                    FindDxDy( out ymindx, out ymindy, controlPoint[ 3 ], hull, numPoints );
                    
                    // Switch so we can look for the smallest opposite/adjacent ratio.
                    float xminopp =  xmindx;
                    float xminadj =  xmindy;
                    float ymaxopp = -ymaxdy;
                    float ymaxadj =  ymaxdx;
                    float xmaxopp = -xmaxdx;
                    float xmaxadj = -xmaxdy;
                    float yminopp =  ymindy;
                    float yminadj = -ymindx;
                    
                    // Pick initial values that will make every point an improvement.
                    float bestopp = 1.0f;
                    float bestadj = 0.0f;
                    int best_control_point = -1;
                    
                    if( ( xminopp >= 0.0 )&&( xminadj >= 0.0 ) )
                    {
                        if( xminopp * bestadj < bestopp * xminadj )
                        {
                            bestopp = xminopp;
                            bestadj = xminadj;
                            best_control_point = 0;
                        }
                    }
                    if( ( ymaxopp >= 0.0 )&&( ymaxadj >= 0.0 ) )
                    {
                        if( ymaxopp * bestadj < bestopp * ymaxadj )
                        {
                            bestopp = ymaxopp;
                            bestadj = ymaxadj;
                            best_control_point = 1;
                        }
                    }
                    if( ( xmaxopp >= 0.0 )&&( xmaxadj >= 0.0 ) )
                    {
                        if( xmaxopp * bestadj < bestopp * xmaxadj )
                        {
                            bestopp = xmaxopp;
                            bestadj = xmaxadj;
                            best_control_point = 2;
                        }
                    }
                    if( ( yminopp >= 0.0 )&&( yminadj >= 0.0 ) )
                    {
                        if( yminopp * bestadj < bestopp * yminadj )
                        {
                            bestopp = yminopp;
                            bestadj = yminadj;
                            best_control_point = 3;
                        }
                    }
                    
                    // No usable edge found
                    if( best_control_point < 0 )
                        break;
                    
                    // Use the new best control point.
                    currentControlPoint = best_control_point;
                    
                    // See which point has the current edge.
                    int i1 = controlPoint[ currentControlPoint ];
                    int i2 = ( i1 + 1 ) % numPoints;
                    float dx = hull[ i2 ].X - hull[ i1 ].X;
                    float dy = hull[ i2 ].Y - hull[ i1 ].Y;
                    
                    // Make dx and dy work for the first line.
                    float temp;
                    switch( currentControlPoint )
                    {
                        case 0:  // null to do.
                            break;
                        case 1:  // dx = -dy, dy = dx
                            temp = dx;
                            dx = -dy;
                            dy = temp;
                            break;
                        case 2:  // dx = -dx, dy = -dy
                            dx = -dx;
                            dy = -dy;
                            break;
                        case 3:  // dx = dy, dy = -dx
                            temp = dx;
                            dx = dy;
                            dy = -temp;
                            break;
                    }
                    
                    // Calculate the bounding rectangle
                    var rect = (Vector2f[]) null;
                    float area, s1, s2;
                    ComputeBoundingRectangle( out rect, out area, out s1, out s2,
                        hull[ controlPoint[ 0 ] ].X, hull[ controlPoint[ 0 ] ].Y,  dx,  dy,
                        hull[ controlPoint[ 1 ] ].X, hull[ controlPoint[ 1 ] ].Y,  dy, -dx,
                        hull[ controlPoint[ 2 ] ].X, hull[ controlPoint[ 2 ] ].Y, -dx, -dy,
                        hull[ controlPoint[ 3 ] ].X, hull[ controlPoint[ 3 ] ].Y, -dy,  dx );
                    
                    if( area < bestArea )
                    {
                        bestArea = area;
                        bestRect = rect;
                        bestS1 = s1;
                        bestS2 = s2;
                    }
                    
                }
                
                OptimalBoundingBox result = null;
                if( !bestRect.NullOrEmpty() )
                {
                    var size = new Vector2f( bestS1, bestS2 );
                    
                    // Can probably get awway with opposite corners, but we'll use all four
                    var pos = new Vector2f(
                        ( bestRect[ 0 ].X + bestRect[ 1 ].X + bestRect[ 2 ].X + bestRect[ 3 ].X ) * 0.25f,
                        ( bestRect[ 0 ].Y + bestRect[ 1 ].Y + bestRect[ 2 ].Y + bestRect[ 3 ].Y ) * 0.25f );
                    
                    result = new OptimalBoundingBox( size, pos, AngleValue( bestRect[ 1 ].X, bestRect[ 1 ].Y, bestRect[ 2 ].X, bestRect[ 2 ].Y ) );
                }
                
                //DebugLog.CloseIndentLevel( "result", result.ToStringNullSafe() );
                return result;
            }
            
            static void ComputeBoundingRectangle( out Vector2f[] rect, out float area, out float s1, out float s2, 
                float px0, float py0, float dx0, float dy0,
                float px1, float py1, float dx1, float dy1,
                float px2, float py2, float dx2, float dy2,
                float px3, float py3, float dx3, float dy3 )
            {
                // Find the points of intersection.
                rect = new Vector2f[ 4 ];
                FindIntersection( px0, py0, px0 + dx0, py0 + dy0, px1, py1, px1 + dx1, py1 + dy1, ref rect[ 0 ] );
                FindIntersection( px1, py1, px1 + dx1, py1 + dy1, px2, py2, px2 + dx2, py2 + dy2, ref rect[ 1 ] );
                FindIntersection( px2, py2, px2 + dx2, py2 + dy2, px3, py3, px3 + dx3, py3 + dy3, ref rect[ 2 ] );
                FindIntersection( px3, py3, px3 + dx3, py3 + dy3, px0, py0, px0 + dx0, py0 + dy0, ref rect[ 3 ] );
                
                // Get the side lengths of the bounding rectangle.
                float vx0 = rect[ 0 ].X - rect[ 1 ].X;
                float vy0 = rect[ 0 ].Y - rect[ 1 ].Y;
                s2 = (float)Math.Sqrt( vx0 * vx0 + vy0 * vy0 );
                
                float vx1 = rect[ 1 ].X - rect[ 2 ].X;
                float vy1 = rect[ 1 ].Y - rect[ 2 ].Y;
                s1 = (float)Math.Sqrt( vx1 * vx1 + vy1 * vy1 );
                
                // Get the area of the bounding rectangle.
                area = s1 * s2;
            }
            
            public static List<Vector2f> MakeConvexHull( List<Vector2f> points )
            {
                //DebugLog.OpenIndentLevel();
                //DebugLog.WriteList( "points", points, false, true );
                
                // Cull points inside an inner bounding box
                var culled = CullPoints( points );
                
                // Find the remaining point with the smallest Y value.
                // if (there's a tie, take the one with the smaller X value.
                var best_pt = culled[ 0 ];
                foreach( var pt in culled )
                    if( ( pt.Y > best_pt.Y )||
                      ( ( pt.Y.ApproximatelyEquals( best_pt.Y ) )&&( pt.X < best_pt.X ) ) )
                        best_pt = pt;
                
                // Move this point to the convex hull.
                var hull = new List<Vector2f>();
                hull.Add( best_pt );
                culled.Remove( best_pt );
                
                // Start wrapping up the other points.
                var sweep_angle = 0.0f;
                while( true )
                {
                    // Find the point with smallest AngleValue
                    // from the last point.
                    var X = hull[ hull.Count - 1 ].X;
                    var Y = hull[ hull.Count - 1 ].Y;
                    best_pt = culled[ 0 ];
                    var best_angle = 360.0f;
                    
                    // Search the rest of the points.
                    foreach( var pt in culled )
                    {
                        var test_angle = AngleValue( X, Y, pt.X, pt.Y );
                        if( ( test_angle >= sweep_angle )&&
                            ( test_angle <  best_angle ) )
                        {
                            best_angle = test_angle;
                            best_pt = pt;
                        }
                    }
                    
                    // See if the first point is better.
                    // If so, we are done.
                    var first_angle = AngleValue( X, Y, hull[ 0 ].X, hull[ 0 ].Y );
                    if( ( first_angle >= sweep_angle )&&
                        ( first_angle <= best_angle  ) )
                        break; // The first point is better. We're done.
                    
                    // Add the best point to the convex hull and remove
                    // it from future tests of the culled points
                    hull.Add( best_pt );
                    culled.Remove( best_pt );
                    
                    // If all of the points are on the hull, we're done.
                    if( culled.Count == 0 ) break;
                    
                    // Best angle is now the sweep angle for the next test.
                    sweep_angle = best_angle;
                    
                }
                
                //DebugLog.WriteList( "hull", hull, false, true );
                //DebugLog.CloseIndentLevel();
                return hull;
            }
            
            static List<Vector2f> CullPoints( List<Vector2f> points )
            {
                //var result = new List<Vector2f>();
                //foreach( var pt in points )
                //    result.Add( new Vector2f( pt ) );
                
                // Find the culling box
                var box = GetMinMaxBox( points );
                
                // Cull the points inside the box
                var result = new List<Vector2f>();
                foreach( var pt in points )
                {
                    if( ( pt.X <= box.Left   )||
                        ( pt.X >= box.Right  )||
                        ( pt.Y <= box.Top    )||
                        ( pt.Y >= box.Bottom ) )
                        result.Add( pt ); // Point is outside the inner culling box, add it
                }
                
                return result;
            }
            
            static System.Drawing.RectangleF GetMinMaxBox( List<Vector2f> points )
            {
                // Get the MinMax quadrilateral
                Vector2f ul = Vector2f.Zero, ur = ul, ll = ul, lr = ul;
                GetMinMaxCorners( points, ref ul, ref ur, ref ll, ref lr );
                
                // Get the coordinates of a box that lies inside this quadrilateral.
                float xmin = Math.Max( ul.X, ll.X );
                float ymin = Math.Max( ul.Y, ur.Y );
                float xmax = Math.Min( ur.X, lr.X );
                float ymax = Math.Min( ll.Y, lr.Y );
                
                return new System.Drawing.RectangleF( xmin, ymin, xmax - xmin, ymax - ymin );
            }
            
            static void GetMinMaxCorners( List<Vector2f> points, ref Vector2f ul, ref Vector2f ur, ref Vector2f ll, ref Vector2f lr )
            {
                // Start with the first point as the solution.
                ul = points[ 0 ];
                ur = ul;
                ll = ul;
                lr = ul;
                
                // Search through the points
                foreach( var pt in points )
                {
                    if( -pt.X - pt.Y > -ul.X - ul.Y ) ul = pt;
                    if(  pt.X - pt.Y >  ur.X - ur.Y ) ur = pt;
                    if( -pt.X + pt.Y > -ll.X + ll.Y ) ll = pt;
                    if(  pt.X + pt.Y >  lr.X + lr.Y ) lr = pt;
                }
            }
            
            public static float AngleValue( float x1, float y1, float x2, float y2 )
            {
                float t;
                
                var dx = x2 - x1;
                var dy = y1 - y2;
                
                var ax = Math.Abs( dx );
                var ay = Math.Abs( dy );
                
                if( ( ax + ay ).ApproximatelyEquals( 0.0f ) )
                    t = 360.0f / 9.0f;
                else
                    t = dy / ( ax + ay );
                
                if( dx < 0.0f )
                    t = 2.0f - t;
                else if( dy < 0.0f )
                    t = 4.0f + t;
                
                return t * 9.0f;
            }
            
            // Find the slope of the edge from point i to point i+1.
            static void FindDxDy( out float dx, out float dy, int i, List<Vector2f> points, int numPoints )
            {
                var i2 = ( i + 1 ) % numPoints;
                dx = points[ i2 ].X - points[ i ].X;
                dy = points[ i2 ].Y - points[ i ].Y;
            }
            
            static bool FindIntersection( float X1, float Y1, float X2, float Y2, float A1, float B1, float A2, float B2, ref Vector2f intersection )
            {
                float dx = X2 - X1;
                float dy = Y2 - Y1;
                float da = A2 - A1;
                float db = B2 - B1;
                float s, t;
                
                // If the segments are parallel, return False.
                //if( Math.Abs( da * dy - db * dx ).ApproximatelyEquals( 0.0f ) ) return false;
                if( Math.Abs( da * dy - db * dx ) < 0.001 ) return false;
                
                // Find the point of intersection.
                s = ( dx * ( B1 - Y1 ) + dy * ( X1 - A1 ) ) / ( da * dy - db * dx );
                t = ( da * ( Y1 - B1 ) + db * ( A1 - X1 ) ) / ( db * dx - da * dy );
                intersection = new Vector2f( X1 + t * dx, Y1 + t * dy );
                return true;
            }
            
        }
        
        #endregion
        
    }
    
}
