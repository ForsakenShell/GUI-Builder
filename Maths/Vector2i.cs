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
    public struct Vector2i : IEquatable<Vector2i>
    {
        
        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        static string minTemplateString = "(0,0)";
        
        // Component values
        public int X, Y;
        
        public Vector2i( int x, int y )
        {
            X = x;
            Y = y;
        }
        
        public Vector2i( Vector2i other )
        {
            X = other.X;
            Y = other.Y;
        }
        
        #region Equality (IEquatable)
        
        public override bool Equals( object obj )
        {
            if (obj is Vector2i)
                return Equals((Vector2i)obj); // use Equals method below
            else
                return false;
        }
        
        public bool Equals( Vector2i other )
        {
            // add comparisions for all members here
            return
                ( this.X == other.X )&&
                ( this.Y == other.Y );
        }
        
        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        
        public static bool operator == ( Vector2i left, Vector2i right )
        {
            return left.Equals( right );
        }
        
        public static bool operator != ( Vector2i left, Vector2i right )
        {
            return !left.Equals(right);
        }
        
        #endregion
        
        #region Math operators and functions
        
        #region Operators
                
        public static Vector2i operator + ( Vector2i left, Vector2i right )
        {
            return new Vector2i( left.X + right.X, left.Y + right.Y );
        }
        
        public static Vector2i operator - ( Vector2i left, Vector2i right )
        {
            return new Vector2i( left.X - right.X, left.Y - right.Y );
        }
        
        public static Vector2i operator * ( Vector2i left, Vector2i right )
        {
            return new Vector2i( left.X * right.X, left.Y * right.Y );
        }
        
        public static Vector2i operator * ( Vector2i left, int right )
        {
            return new Vector2i( left.X * right, left.Y * right );
        }
        
        public static Vector2f operator * ( Vector2i left, float right )
        {
            return new Vector2f( (float)left.X * right, (float)left.Y * right );
        }
        
        public static Vector2i operator / ( Vector2i left, Vector2i right )
        {
            return new Vector2i( left.X / right.X, left.Y / right.Y );
        }
        
        public static Vector2i operator / ( Vector2i left, int right )
        {
            return new Vector2i( left.X / right, left.Y / right );
        }
        
        #endregion
        
        #region Special Values
        
        public static Vector2i Zero
        {
            get
            {
                return new Vector2i( 0, 0 );
            }
        }
        
        public static Vector2i MinValue
        {
            get
            {
                return new Vector2i( int.MinValue, int.MinValue );
            }
        }
        
        public static Vector2i MaxValue
        {
            get
            {
                return new Vector2i( int.MaxValue, int.MaxValue );
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
        
        public bool IsZero()
        {
            return ( X == 0 )&&( Y == 0 );
        }
        
        #endregion
        
        #endregion
        
        public override string ToString()
        {
            return string.Format( "{3}{0}{2}{1}{4}", X, Y, delimiterSetElement, delimiterSetOpen, delimiterSetClose );
        }
        
        public static bool TryParse( string fromString, out Vector2i result )
        {
            result = new Vector2i( Zero );
            
            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();
            
            string[] elements = fromString.Split( delimiterChars );
            
            if( elements.Length != 4 )
                throw new ArgumentException();
            
            int x, y;
            if( !int.TryParse( elements[ 1 ], out x ) )
                throw new ArgumentException();
            if( !int.TryParse( elements[ 2 ], out y ) )
                throw new ArgumentException();
            
            result = new Vector2i( x, y );
            return true;
        }
        
        public void WriteToStream( System.IO.BinaryWriter stream )
        {
            stream.Write( X );
            stream.Write( Y );
        }
        
    }
}
