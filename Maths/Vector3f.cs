/*
 * Vector3f.cs
 * 
 * A struct and associated functions to work with 3d (float) x,y,z structs.
 * 
 * User: 1000101
 * Date: 01/12/2017
 * Time: 7:00 AM
 * 
 */
using System;

namespace Border_Builder
{
    
    public static partial class Maths
    {
        /// <summary>
        /// Description of Vector3f.
        /// </summary>
        public struct Vector3f : IEquatable<Vector3f>
        {
            
            // String delimiters for ToString() and FromString()
            private const char delimiterSetOpen = '(';
            private const char delimiterSetClose = ')';
            private const char delimiterSetElement= ',';
            private static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
            private const string minTemplateString = "(0,0,0)";
            
            // Component values
            public float X, Y, Z;
            
            public Vector3f( float x, float y, float z )
            {
                X = x;
                Y = y;
                Z = z;
            }
            
            public Vector3f( Vector3f v )
            {
                X = v.X;
                Y = v.Y;
                Z = v.Z;
            }
            
            #region Equality (IEquatable)
            
            public override bool Equals( object obj )
            {
                if (obj is Vector3f)
                    return Equals((Vector3f)obj); // use Equals method below
                else
                    return false;
            }
            
            public bool Equals( Vector3f other )
            {
                // add comparisions for all members here
                return
                    ( this.X.ApproximatelyEquals( other.X ) )&&
                    ( this.Y.ApproximatelyEquals( other.Y ) )&&
                    ( this.Z.ApproximatelyEquals( other.Z ) );
            }
            
            public override int GetHashCode()
            {
                // combine the hash codes of all members here (e.g. with XOR operator ^)
                return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
            }
            
            public static bool operator == ( Vector3f left, Vector3f right )
            {
                return left.Equals( right );
            }
            
            public static bool operator !=( Vector3f left, Vector3f right )
            {
                return !left.Equals(right);
            }
            
            #endregion
            
            #region Math operators and functions
            
            #region Operators
                    
            public static Vector3f operator + ( Vector3f left, Vector3f right )
            {
                return new Vector3f( left.X + right.X, left.Y + right.Y, left.Z + right.Z );
            }
            
            public static Vector3f operator - ( Vector3f left, Vector3f right )
            {
                return new Vector3f( left.X - right.X, left.Y - right.Y, left.Z - right.Z );
            }
            
            public static Vector3f operator * ( Vector3f left, Vector3f right )
            {
                return new Vector3f( left.X * right.X, left.Y * right.Y, left.Z * right.Z );
            }
            
            public static Vector3f operator * ( Vector3f left, float right )
            {
                return new Vector3f( left.X * right, left.Y * right, left.Z * right );
            }
            
            public static Vector3f operator / ( Vector3f left, Vector3f right )
            {
                return new Vector3f( left.X / right.X, left.Y / right.Y, left.Z / right.Z );
            }
            
            public static Vector3f operator / ( Vector3f left, float right )
            {
                return new Vector3f( left.X / right, left.Y / right, left.Z / right );
            }
            
            #endregion
            
            #region Functions
            
            public static Vector3f Zero
            {
                get
                {
                    return new Vector3f( 0, 0, 0 );
                }
            }
            
            public float Length
            {
                get
                {
                    return (float)Math.Sqrt( ( X * X ) + ( Y * Y ) + ( Z * Z ) );
                }
            }
            
            public void Normalize()
            {
                var length = Length;
                if( length.ApproximatelyEquals( 0f ) )
                    return;
                X /= length;
                Y /= length;
                Z /= length;
            }
            
            public bool IsZero()
            {
                return Length.ApproximatelyEquals( 0f );
            }
            
            public static Vector3f Cross( Vector3f left, Vector3f right )
            {
                return new Vector3f(
                    (float)( ( (double)left.Y * (double)right.Z ) - ( (double)left.Z * (double)right.Y ) ),
                    (float)( ( (double)left.Z * (double)right.X ) - ( (double)left.X * (double)right.Z ) ),
                    (float)( ( (double)left.X * (double)right.Y ) - ( (double)left.Y * (double)right.X ) ) );
            }
            
            public static float Dot( Vector3f left, Vector3f right )
            {
                return (float)(
                    ( (double)left.X * (double)right.X ) +
                    ( (double)left.Y * (double)right.Y ) +
                    ( (double)left.Z * (double)right.Z ) );
            }
            
            #endregion
            
            #endregion
            
            public new string ToString()
            {
                return string.Format( "{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
            }
            
            public static bool TryParse( string fromString, out Vector3f result )
            {
                result = Zero;
                
                if( fromString.Length < minTemplateString.Length )
                    throw new ArgumentException();
                
                string[] elements = fromString.Split( delimiterChars );
                
                if( elements.Length != 5 )
                    throw new ArgumentException();
                
                float x, y, z;
                if( !float.TryParse( elements[ 1 ], out x ) )
                    throw new ArgumentException();
                if( !float.TryParse( elements[ 2 ], out y ) )
                    throw new ArgumentException();
                if( !float.TryParse( elements[ 3 ], out z ) )
                    throw new ArgumentException();
                
                result = new Vector3f( x, y, z );
                return true;
            }
            
        }
    }
}