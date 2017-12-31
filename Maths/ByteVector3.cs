/*
 * ByteVector3.cs
 *
 * A struct and associated functions to work with (byte) x,y,z structs.
 *
 * User: 1000101
 * Date: 01/12/2017
 * Time: 8:01 AM
 * 
 */
using System;
using System.Runtime.InteropServices;

namespace Border_Builder
{
    
    public static partial class Maths
    {
        /// <summary>
        /// Description of ByteVector3.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct ByteVector3 : IEquatable<ByteVector3>
        {
            
            // String delimiters for ToString() and FromString()
            private const char delimiterSetOpen = '(';
            private const char delimiterSetClose = ')';
            private const char delimiterSetElement= ',';
            private static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
            private static string minTemplateString = "(0,0,0)";
            
            // Component values
            private byte _X, _Y, _Z;
            
            public ByteVector3( float x, float y, float z )
            {
                _X = ComponentFromFloat( x );
                _Y = ComponentFromFloat( y );
                _Z = ComponentFromFloat( z );
            }
            
            #region Equality (IEquatable)
            
            public override bool Equals( object obj )
            {
                if (obj is ByteVector3)
                    return Equals((ByteVector3)obj); // use Equals method below
                else
                    return false;
            }
            
            public bool Equals( ByteVector3 other )
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
            
            public static bool operator == ( ByteVector3 left, ByteVector3 right )
            {
                return left.Equals( right );
            }
            
            public static bool operator !=( ByteVector3 left, ByteVector3 right )
            {
                return !left.Equals(right);
            }
            
            #endregion
            
            #region Math operators and functions
            
            #region Operators
                    
            public static ByteVector3 operator + ( ByteVector3 left, ByteVector3 right )
            {
                return new ByteVector3( left.X + right.X, left.Y + right.Y, left.Z + right.Z );
            }
            
            public static ByteVector3 operator - ( ByteVector3 left, ByteVector3 right )
            {
                return new ByteVector3( left.X - right.X, left.Y - right.Y, left.Z - right.Z );
            }
            
            public static ByteVector3 operator * ( ByteVector3 left, ByteVector3 right )
            {
                return new ByteVector3( left.X * right.X, left.Y * right.Y, left.Z * right.Z );
            }
            
            public static ByteVector3 operator * ( ByteVector3 left, float right )
            {
                return new ByteVector3( left.X * right, left.Y * right, left.Z * right );
            }
            
            public static ByteVector3 operator / ( ByteVector3 left, ByteVector3 right )
            {
                return new ByteVector3( left.X / right.X, left.Y / right.Y, left.Z / right.Z );
            }
            
            public static ByteVector3 operator / ( ByteVector3 left, float right )
            {
                return new ByteVector3( left.X / right, left.Y / right, left.Z / right );
            }
            
            #endregion
            
            #region Component properties
            
            public float X
            {
                get
                {
                    return ComponentToFloat( _X );
                }
                set
                {
                    _X = ComponentFromFloat( value );
                }
            }
            
            public float Y
            {
                get
                {
                    return ComponentToFloat( _Y );
                }
                set
                {
                    _Y = ComponentFromFloat( value );
                }
            }
            
            public float Z
            {
                get
                {
                    return ComponentToFloat( _Z );
                }
                set
                {
                    _Z = ComponentFromFloat( value );
                }
            }
            
            #endregion
            
            #region Functions
            
            public static byte ComponentFromFloat( float value )
            {
                return (byte)( ( ( value.Clamp( -1f, 1f ) + 1f ) /2f ) * 255f );
            }
            
            public static float ComponentToFloat( byte value )
            {
                return ( ( ( ( (float)value ) / 255f ) * 2f ) - 1f );
            }
            
            public static ByteVector3 Zero
            {
                get
                {
                    return new ByteVector3( 0, 0, 0 );
                }
            }
            
            public float Magnitude()
            {
                return (float)Math.Sqrt( ( X * X ) + ( Y * Y ) + ( Z * Z ) );
            }
            
            public void Normalize()
            {
                var mag = Magnitude();
                if( mag.ApproximatelyEquals( 0f ) )
                    return;
                X /= mag;
                Y /= mag;
                Z /= mag;
            }
            
            public bool IsZero()
            {
                return Magnitude().ApproximatelyEquals( 0f );
            }
            
            public static ByteVector3 Cross( ByteVector3 left, ByteVector3 right )
            {
                return new ByteVector3(
                    ( left.Y * right.Z ) - ( left.Z * right.Y ),
                    ( left.Z * right.X ) - ( left.X * right.Z ),
                    ( left.X * right.Y ) - ( left.Y * right.X ) );
            }
            
            public static float Dot( ByteVector3 left, ByteVector3 right )
            {
                return (float)(
                    ( left.X * right.X ) +
                    ( left.Y * right.Y ) +
                    ( left.Z * right.Z ) );
            }
            
            #endregion
            
            #endregion
            
            public new string ToString()
            {
                return string.Format( "{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
            }
            
            public static bool TryParse( string fromString, out ByteVector3 result )
            {
                result = Zero;
                
                if( fromString.Length < minTemplateString.Length )
                    throw new ArgumentException();
                
                string[] elements = fromString.Split( delimiterChars );
                
                if( elements.Length != 3 )
                    throw new ArgumentException();
                
                float x, y, z;
                if( !float.TryParse( elements[ 0 ], out x ) )
                    throw new ArgumentException();
                if( !float.TryParse( elements[ 1 ], out y ) )
                    throw new ArgumentException();
                if( !float.TryParse( elements[ 2 ], out z ) )
                    throw new ArgumentException();
                
                result = new ByteVector3( x, y, z );
                return true;
            }
            
        }
    }
}