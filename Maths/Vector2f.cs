/*
 * Vector2f.cs
 * 
 * A struct and associated functions to work with 2d (float) x,y structs.
 * 
 * User: 1000101
 * Date: 01/12/2017
 * Time: 7:53 AM
 * 
 */
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Maths
{
    /// <summary>
    /// Description of Vector2f.
    /// </summary>
    public struct Vector2f : IEquatable<Vector2f>
    {
        
        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        static string minTemplateString = "(0,0)";
        
        // Component values
        public float X, Y;
        
        public Vector2f( float x, float y )
        {
            X = x;
            Y = y;
        }
        
        public Vector2f( Vector2f v )
        {
            X = v.X;
            Y = v.Y;
        }
        
        public Vector2f( Vector3f v )
        {
            X = v.X;
            Y = v.Y;
        }
        
        public Vector2f( HalfVector2f v )
        {
            X = v.X;
            Y = v.Y;
        }
        
        public Vector2f( Vector2i v )
        {
            X = (float)v.X;
            Y = (float)v.Y;
        }
        
        #region Equality (IEquatable)
        
        public override bool Equals( object obj )
        {
            if (obj is Vector2f)
                return Equals((Vector2f)obj); // use Equals method below
            else
                return false;
        }
        
        public bool Equals( Vector2f other )
        {
            // add comparisions for all members here
            //return ( this - other ).Length.ApproximatelyEquals( 0f );
            return
                ( this.X.ApproximatelyEquals( other.X ) )&&
                ( this.Y.ApproximatelyEquals( other.Y ) );
        }
        
        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        
        public static bool operator == ( Vector2f left, Vector2f right )
        {
            return left.Equals( right );
        }
        
        public static bool operator != ( Vector2f left, Vector2f right )
        {
            return !left.Equals(right);
        }
        
        #endregion
        
        #region Math operators and functions
        
        #region Operators
                
        public static Vector2f operator + ( Vector2f left, Vector2f right )
        {
            return new Vector2f( left.X + right.X, left.Y + right.Y );
        }
        
        public static Vector2f operator + ( Vector2f left, Vector2i right )
        {
            return new Vector2f( left.X + (float)right.X, left.Y + (float)right.Y );
        }
        
        public static Vector2f operator - ( Vector2f left, Vector2f right )
        {
            return new Vector2f( left.X - right.X, left.Y - right.Y );
        }
        
        public static Vector2f operator - ( Vector2f left, Vector2i right )
        {
            return new Vector2f( left.X - (float)right.X, left.Y - (float)right.Y );
        }
        
        public static Vector2f operator * ( Vector2f left, Vector2f right )
        {
            return new Vector2f( left.X * right.X, left.Y * right.Y );
        }
        
        public static Vector2f operator * ( Vector2f left, float right )
        {
            return new Vector2f( left.X * right, left.Y * right );
        }
        
        public static Vector2f operator / ( Vector2f left, Vector2f right )
        {
            return new Vector2f( left.X / right.X, left.Y / right.Y );
        }
        
        public static Vector2f operator / ( Vector2f left, float right )
        {
            return new Vector2f( left.X / right, left.Y / right );
        }
        
        public static Vector2f operator / ( Vector2f left, int right )
        {
            return new Vector2f( left.X / (float)right, left.Y / (float)right );
        }
        
        #endregion
        
        #region Special Values
        
        public static Vector2f Zero
        {
            get
            {
                return new Vector2f( 0.0f, 0.0f );
            }
        }
        
        public static Vector2f MinValue
        {
            get
            {
                return new Vector2f( float.MinValue, float.MinValue );
            }
        }
        
        public static Vector2f MaxValue
        {
            get
            {
                return new Vector2f( float.MaxValue, float.MaxValue );
            }
        }
        
        #endregion
        
        #region Functions
        
        public float Length
        {
            get
            {
                return (float)Math.Sqrt( ( X * X ) + ( Y * Y ) );
            }
        }
        
        public float Angle
        {
            get
            {
                return (float)Math.Atan2( (double)Y, (double)X );
            }
        }
        
        public double Area
        {
            get
            {
                return ( (double) X ) * ( (double) Y );
            }
        }
        
        public double SideRatio
        {
            get
            {
                return X / Y;
            }
        }
        
        public void Normalize()
        {
            var length = Length;
            if( length.ApproximatelyEquals( 0f ) )
                return;
            X /= length;
            Y /= length;
        }
        
        public bool IsZero()
        {
            return Length.ApproximatelyEquals( 0f );
        }
        
        public static Vector2f RotateAroundRad( Vector2f v, Vector2f p, double r )
        {
            var c = Math.Cos( r );
            var s = Math.Sin( r );
            var x = (double)v.X - (double)p.X;
            var y = (double)v.Y - (double)p.Y;
            var rX = x * c - y * s;
            var rY = x * s + y * c;
            return new Vector2f( (float)((double)p.X + rX ), (float)((double)p.Y + rY ) );
        }
        
        public static Vector2f RotateAround( Vector2f v, Vector2f p, float a )
        {
            return RotateAroundRad( v, p, (double)( a * Maths.Constant.DEG_TO_RAD ) );
        }
        
        public static List<Vector2f> RotateAround( List<Vector2f> list, Vector2f p, float a )
        {
            if( list.NullOrEmpty() )
                return null;
            double rads = a * Maths.Constant.DEG_TO_RAD;
            var count = list.Count;
            var rotated = new List<Vector2f>();
            for( int i = 0; i < count; i++ )
                rotated.Add( RotateAroundRad( list[ i ], p, rads ) );
            return rotated;
        }
        
        public Vector2f RotateAround( Vector2f p, float a )
        {
            return RotateAroundRad( this, p, (double)( a * Maths.Constant.DEG_TO_RAD ) );
        }
        
        public float DistanceFrom( Vector2f other )
        {
            return DistanceFrom( other.X, other.Y );
        }
        
        public float DistanceFrom( float otherX, float otherY )
        {
            var dX = this.X - otherX;
            var dY = this.Y - otherY;
            return (float)Math.Sqrt( ( dX * dX ) + ( dY * dY ) );
        }
        
        public Vector2f Average( Vector2f other )
        {
            return Average( other.X, other.Y );
        }
        
        public Vector2f Average( float otherX, float otherY )
        {
            var dX = ( (double)this.X - (double)otherX ) * 0.5d;
            var dY = ( (double)this.Y - (double)otherY ) * 0.5d;
            return new Vector2f( (float)((double)this.X + dX ), (float)((double)this.Y + dY) );
        }
        
        public static float Dot( Vector2f left, Vector2f right )
        {
            return (float)(
                ( (double)left.X * (double)right.X ) +
                ( (double)left.Y * (double)right.Y ) );
        }
        
        public static float Cross( Vector2f left, Vector2f right )
        {
            return (float)(
                ( (double)left.X * (double)right.Y ) -
                ( (double)left.Y * (double)right.X ) );
        }
        
        public static Vector2f Min( Vector2f[] array )
        {
            var min = Vector2f.MaxValue;
            if( !array.NullOrEmpty() )
            {
                foreach( var e in array )
                {
                    min.X = Math.Min( min.X, e.X );
                    min.Y = Math.Min( min.Y, e.Y );
                }
            }
            return min;
        }
        
        public static Vector2f Min( Vector2f[][] array )
        {
            var min = Vector2f.MaxValue;
            if( !array.NullOrEmpty() )
            {
                foreach( var e in array )
                {
                    if( !e.NullOrEmpty() )
                    {
                        var tmp = Vector2f.Min( e );
                        min.X = Math.Min( min.X, tmp.X );
                        min.Y = Math.Min( min.Y, tmp.Y );
                    }
                }
            }
            return min;
        }
        
        public static Vector2f Max( Vector2f[] array )
        {
            var max = Vector2f.MinValue;
            if( !array.NullOrEmpty() )
            {
                foreach( var e in array )
                {
                    max.X = Math.Max( max.X, e.X );
                    max.Y = Math.Max( max.Y, e.Y );
                }
            }
            return max;
        }
        
        public static Vector2f Max( Vector2f[][] array )
        {
            var max = Vector2f.MinValue;
            if( !array.NullOrEmpty() )
            {
                foreach( var e in array )
                {
                    if( !e.NullOrEmpty() )
                    {
                        var tmp = Vector2f.Max( e );
                        max.X = Math.Max( max.X, tmp.X );
                        max.Y = Math.Max( max.Y, tmp.Y );
                    }
                }
            }
            return max;
        }
        
        #endregion
        
        #endregion
        
        public new string ToString()
        {
            return string.Format( "{3}{0}{2}{1}{4}", X, Y, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
        }
        
        public static bool TryParse( string fromString, out Vector2f result )
        {
            result = new Vector2f( Zero );
            
            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();
            
            string[] elements = fromString.Split( delimiterChars );
            
            if( elements.Length != 4 )
                throw new ArgumentException();
            
            float x, y;
            x = float.Parse( elements[ 1 ], CultureInfo.InvariantCulture );
            y = float.Parse( elements[ 2 ], CultureInfo.InvariantCulture );
            
            result = new Vector2f( x, y );
            return true;
        }
        
        public void WriteToStream( System.IO.BinaryWriter stream )
        {
            stream.Write( X );
            stream.Write( Y );
        }
        
    }
    
    public static class Vector2fExtensions
    {
        
        #region Find North-West and South-East corners from a set of points
        
        public static Vector2f GetCornerNWFrom( this List<Vector2f> points )
        {
            if( points.NullOrEmpty() )
                return Vector2f.Zero;
            
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            foreach( var p in points )
            {
                minX = Math.Min( minX, p.X );
                maxY = Math.Max( maxY, p.Y );
            }
            return new Vector2f( minX, maxY );
        }
        
        public static Vector2f GetCornerSEFrom( this List<Vector2f> points )
        {
            if( points.NullOrEmpty() )
                return Vector2f.Zero;
            
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            foreach( var p in points )
            {
                maxX = Math.Max( maxX, p.X );
                minY = Math.Min( minY, p.Y );
            }
            return new Vector2f( maxX, minY );
        }
        
        public static Vector2f GetCornerNWFrom( this Vector2f[] points )
        {
            if( points.NullOrEmpty() )
                return Vector2f.Zero;
            
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            foreach( var p in points )
            {
                minX = Math.Min( minX, p.X );
                maxY = Math.Max( maxY, p.Y );
            }
            return new Vector2f( minX, maxY );
        }
        
        public static Vector2f GetCornerSEFrom( this Vector2f[] points )
        {
            if( points.NullOrEmpty() )
                return Vector2f.Zero;
            
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            foreach( var p in points )
            {
                maxX = Math.Max( maxX, p.X );
                minY = Math.Min( minY, p.Y );
            }
            return new Vector2f( maxX, minY );
        }
        
        public static Vector2f GetCornerNWFrom( this Vector2f[][] polygons )
        {
            if( polygons.NullOrEmpty() )
                return Vector2f.Zero;
            
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            foreach( var p in polygons )
            {
                var tmp = GetCornerNWFrom( p );
                minX = Math.Min( minX, tmp.X );
                maxY = Math.Max( maxY, tmp.Y );
            }
            return new Vector2f( minX, maxY );
        }
        
        public static Vector2f GetCornerSEFrom( this Vector2f[][] polygons )
        {
            if( polygons.NullOrEmpty() )
                return Vector2f.Zero;
            
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            foreach( var p in polygons )
            {
                var tmp = GetCornerNWFrom( p );
                maxX = Math.Max( maxX, tmp.X );
                minY = Math.Min( minY, tmp.Y );
            }
            return new Vector2f( maxX, minY );
        }
        
        #endregion
        
    }
}
