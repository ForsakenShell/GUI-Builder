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
using System.Globalization;

namespace Maths
{
    /// <summary>
    /// Description of Vector3f.
    /// </summary>
    public struct Vector3f : IEquatable<Vector3f>
    {
        
        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        const string minTemplateString = "(0,0,0)";
        
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
        
        public Vector3f( Vector2f v )
        {
            X = v.X;
            Y = v.Y;
            Z = 0.0f;
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
        
        public static bool operator != ( Vector3f left, Vector3f right )
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
        
        #region Special Values
        
        public static Vector3f Zero
        {
            get
            {
                return new Vector3f( 0.0f, 0.0f, 0.0f );
            }
        }

        public static Vector3f Up
        {
            get
            {
                return new Vector3f( 0.0f, 0.0f, 1.0f );
            }
        }

        public static Vector3f Down
        {
            get
            {
                return new Vector3f( 0.0f, 0.0f, -1.0f );
            }
        }

        public static Vector3f MinValue
        {
            get
            {
                return new Vector3f( float.MinValue, float.MinValue, float.MinValue );
            }
        }
        
        public static Vector3f MaxValue
        {
            get
            {
                return new Vector3f( float.MaxValue, float.MaxValue, float.MaxValue );
            }
        }
        
        #endregion
        
        #region Functions
        
        public float Length
        {
            get
            {
                return (float)Math.Sqrt( ( X * X ) + ( Y * Y ) + ( Z * Z ) );
            }
        }
        
        public float Length2D
        {
            get
            {
                return (float)Math.Sqrt( ( X * X ) + ( Y * Y ) );
            }
        }
        
        public void Normalize()
        {
            this = Vector3f.Normal( this );
        }

        public static Vector3f Normal( Vector3f v )
        {
            if( v == null )
                return Vector3f.Zero;
            var length = v.Length;
            return length.ApproximatelyEquals( 0.0f )
                ? Vector3f.Zero
                : new Vector3f(
                    v.X / length,
                    v.Y / length,
                    v.Z / length );
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
        
        public override string ToString()
        {
            return string.Format( "{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
        }
        
        public static bool TryParse( string fromString, out Vector3f result )
        {
            result = new Vector3f( Zero );
            
            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();
            
            string[] elements = fromString.Split( delimiterChars );
            
            if( elements.Length != 5 )
                throw new ArgumentException();
            
            float x, y, z;
            x = float.Parse( elements[ 1 ], CultureInfo.InvariantCulture );
            y = float.Parse( elements[ 2 ], CultureInfo.InvariantCulture );
            z = float.Parse( elements[ 3 ], CultureInfo.InvariantCulture );
            
            result = new Vector3f( x, y, z );
            return true;
        }
        
        public void WriteToStream( System.IO.BinaryWriter stream )
        {
            stream.Write( X );
            stream.Write( Y );
            stream.Write( Z );
        }
        
    }
}
