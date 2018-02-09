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

namespace Border_Builder
{
    
    public static partial class Maths
    {
        /// <summary>
        /// Description of Vector2f.
        /// </summary>
        public struct Vector2f : IEquatable<Vector2f>
        {
            
            // String delimiters for ToString() and FromString()
            private const char delimiterSetOpen = '(';
            private const char delimiterSetClose = ')';
            private const char delimiterSetElement= ',';
            private static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
            private static string minTemplateString = "(0,0)";
            
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
            
            public static bool operator !=( Vector2f left, Vector2f right )
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
            
            #region Functions
            
            public static Vector2f Zero
            {
                get
                {
                    return new Vector2f( 0, 0 );
                }
            }
            
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
            
            public static Vector2f RotateAround( Vector2f v, Vector2f p, float a )
            {
                var c = Math.Cos( a * Maths.DEG_TO_RAD );
                var s = Math.Sin( a * Maths.DEG_TO_RAD );
                var x = (double)v.X - (double)p.X;
                var y = (double)v.Y - (double)p.Y;
                var rX = x * c - y * s;
                var rY = x * s + y * c;
                return new Vector2f( (float)((double)p.X + rX ), (float)((double)p.Y + rY ) );
            }
            
            public Vector2f RotateAround( Vector2f p, float a )
            {
                return RotateAround( this, p, a );
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
           
            #endregion
            
            #endregion
            
            public new string ToString()
            {
                return string.Format( "{3}{0}{2}{1}{4}", X, Y, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
            }
            
            public static bool TryParse( string fromString, out Vector2f result )
            {
                result = Zero;
                
                if( fromString.Length < minTemplateString.Length )
                    throw new ArgumentException();
                
                string[] elements = fromString.Split( delimiterChars );
                
                if( elements.Length != 4 )
                    throw new ArgumentException();
                
                float x, y;
                if( !float.TryParse( elements[ 1 ], out x ) )
                    throw new ArgumentException();
                if( !float.TryParse( elements[ 2 ], out y ) )
                    throw new ArgumentException();
                
                result = new Vector2f( x, y );
                return true;
            }
            
        }
    }
}