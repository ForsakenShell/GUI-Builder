/*
 * Vector2i.cs
 * 
 * A struct and associated functions to work with 2d (int) x,y structs.
 * 
 * User: 1000101
 * Date: 01/12/2017
 * Time: 7:53 AM
 * 
 */
using System;

namespace Maths
{
    /// <summary>
    /// Description of Vector2i.
    /// </summary>
    public struct Vector3i : IEquatable<Vector3i>
    {
        
        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        static string minTemplateString = "(0,0,0)";
        
        // Component values
        public int X, Y, Z;
        
        public Vector3i( int x, int y, int z )
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vector3i( float x, float y, float z )
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }
        
        public Vector3i( Vector3i other )
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }
        
        #region Equality (IEquatable)
        
        public override bool Equals( object obj )
        {
            if (obj is Vector3i)
                return Equals((Vector3i)obj); // use Equals method below
            else
                return false;
        }
        
        public bool Equals( Vector3i other )
        {
            // add comparisions for all members here
            return
                ( this.X == other.X )&&
                ( this.Y == other.Y )&&
                ( this.Z == other.Z );
        }
        
        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
        
        public static bool operator == ( Vector3i left, Vector3i right )
        {
            return left.Equals( right );
        }
        
        public static bool operator != ( Vector3i left, Vector3i right )
        {
            return !left.Equals(right);
        }
        
        #endregion
        
        #region Math operators and functions
        
        #region Operators
                
        public static Vector3i operator + ( Vector3i left, Vector3i right )
        {
            return new Vector3i( left.X + right.X, left.Y + right.Y, left.Z + right.Z );
        }
        
        public static Vector3i operator - ( Vector3i left, Vector3i right )
        {
            return new Vector3i( left.X - right.X, left.Y - right.Y, left.Z - right.Z );
        }
        
        public static Vector3i operator * ( Vector3i left, Vector3i right )
        {
            return new Vector3i( left.X * right.X, left.Y * right.Y, left.Z * right.Z );
        }
        
        public static Vector3i operator * ( Vector3i left, int right )
        {
            return new Vector3i( left.X * right, left.Y * right, left.Z * right );
        }
        
        public static Vector3f operator * ( Vector3i left, float right )
        {
            return new Vector3f( (float)left.X * right, (float)left.Y * right, (float)left.Z * right );
        }
        
        public static Vector3i operator / ( Vector3i left, Vector3i right )
        {
            return new Vector3i( left.X / right.X, left.Y / right.Y, left.Z / right.Z );
        }
        
        public static Vector3i operator / ( Vector3i left, int right )
        {
            return new Vector3i( left.X / right, left.Y / right, left.Z / right );
        }
        
        #endregion
        
        #region Special Values
        
        public static Vector3i Zero
        {
            get
            {
                return new Vector3i( 0, 0, 0 );
            }
        }
        
        public static Vector3i MinValue
        {
            get
            {
                return new Vector3i( int.MinValue, int.MinValue, int.MinValue );
            }
        }
        
        public static Vector3i MaxValue
        {
            get
            {
                return new Vector3i( int.MaxValue, int.MaxValue, int.MaxValue );
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
        
        public bool IsZero()
        {
            return ( X == 0 )&&( Y == 0 )&&( Z == 0 );
        }
        
        #endregion
        
        #endregion
        
        public new string ToString()
        {
            return string.Format( "{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
        }
        
        public static bool TryParse( string fromString, out Vector3i result )
        {
            result = new Vector3i( Zero );
            
            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();
            
            string[] elements = fromString.Split( delimiterChars );
            
            if( elements.Length != 5 )
                throw new ArgumentException();
            
            int x, y, z;
            if( !int.TryParse( elements[ 1 ], out x ) )
                throw new ArgumentException();
            if( !int.TryParse( elements[ 2 ], out y ) )
                throw new ArgumentException();
            if( !int.TryParse( elements[ 3 ], out z ) )
                throw new ArgumentException();
            
            result = new Vector3i( x, y, z );
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
