/*
 * HalfVector2f.cs
 * 
 * A struct and associated functions to work with 2d (16-bit float) x,y structs.
 * 
 * User: 1000101
 * Date: 02/02/2019
 * Time: 4:18 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Maths
{
    /// <summary>
    /// Description of HalfVector2f.
    /// </summary>
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct HalfVector2f : IEquatable<HalfVector2f>
    {

        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        static string minTemplateString = "(0,0)";

        // Component values
        Half _X, _Y;

        public float X
        {
            get { return HalfHelper.HalfToSingle( _X ); }
            set { _X = HalfHelper.SingleToHalf( value ); }
        }

        public float Y
        {
            get { return HalfHelper.HalfToSingle( _Y ); }
            set { _Y = HalfHelper.SingleToHalf( value ); }
        }

        public HalfVector2f( float x, float y )
        {
            _X = HalfHelper.SingleToHalf( x );
            _Y = HalfHelper.SingleToHalf( y );
        }

        public HalfVector2f( HalfVector2f v )
        {
            _X = new Half( v.X );
            _Y = new Half( v.Y );
        }

        public HalfVector2f( Vector2f v )
        {
            _X = HalfHelper.SingleToHalf( v.X );
            _Y = HalfHelper.SingleToHalf( v.Y );
        }

        public HalfVector2f( Vector2i v )
        {
            _X = new Half( v.X );
            _Y = new Half( v.Y );
        }

        #region Equality (IEquatable)

        public override bool Equals( object obj )
        {
            if( obj is HalfVector2f )
                return Equals( (HalfVector2f)obj ); // use Equals method below
            else
                return false;
        }

        public bool Equals( HalfVector2f other )
        {
            // add comparisions for all members here
            //return ( this - other ).Length.ApproximatelyEquals( 0f );
            return
                ( this.X.ApproximatelyEquals( other.X ) ) &&
                ( this.Y.ApproximatelyEquals( other.Y ) );
        }

        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return _X.GetHashCode() ^ _Y.GetHashCode();
        }

        public static bool operator ==( HalfVector2f left, HalfVector2f right )
        {
            return left.Equals( right );
        }

        public static bool operator !=( HalfVector2f left, HalfVector2f right )
        {
            return !left.Equals( right );
        }

        #endregion

        #region Math operators and functions

        #region Operators

        public static HalfVector2f operator +( HalfVector2f left, HalfVector2f right )
        {
            return new HalfVector2f( left.X + right.X, left.Y + right.Y );
        }

        public static HalfVector2f operator +( HalfVector2f left, Vector2f right )
        {
            return new HalfVector2f( new Half( left.X + right.X ), new Half( left.Y + right.Y ) );
        }

        public static HalfVector2f operator +( HalfVector2f left, Vector2i right )
        {
            return new HalfVector2f( new Half( left.X + right.X ), new Half( left.Y + right.Y ) );
        }

        public static HalfVector2f operator -( HalfVector2f left, HalfVector2f right )
        {
            return new HalfVector2f( left.X - right.X, left.Y - right.Y );
        }

        public static HalfVector2f operator -( HalfVector2f left, Vector2f right )
        {
            return new HalfVector2f( new Half( left.X - right.X ), new Half( left.Y - right.Y ) );
        }

        public static HalfVector2f operator -( HalfVector2f left, Vector2i right )
        {
            return new HalfVector2f( new Half( left.X - (float)right.X ), new Half( left.Y - (float)right.Y ) );
        }

        public static HalfVector2f operator *( HalfVector2f left, HalfVector2f right )
        {
            return new HalfVector2f( left.X * right.X, left.Y * right.Y );
        }

        public static HalfVector2f operator *( HalfVector2f left, Vector2f right )
        {
            return new HalfVector2f( new Half( left.X * right.X ), new Half( left.Y * right.Y ) );
        }

        public static HalfVector2f operator *( HalfVector2f left, float right )
        {
            return new HalfVector2f( new Half( left.X * right ), new Half( left.Y * right ) );
        }

        public static HalfVector2f operator /( HalfVector2f left, HalfVector2f right )
        {
            return new HalfVector2f( left.X / right.X, left.Y / right.Y );
        }

        public static HalfVector2f operator /( HalfVector2f left, Vector2f right )
        {
            return new HalfVector2f( new Half( left.X / right.X ), new Half( left.Y / right.Y ) );
        }

        public static HalfVector2f operator /( HalfVector2f left, float right )
        {
            return new HalfVector2f( new Half( left.X / right ), new Half( left.Y / right ) );
        }

        public static HalfVector2f operator /( HalfVector2f left, int right )
        {
            return new HalfVector2f( new Half( left.X / (float)right ), new Half( left.Y / (float)right ) );
        }

        #endregion

        #region Special Values

        public static HalfVector2f Zero
        {
            get
            {
                return new HalfVector2f( 0.0f, 0.0f );
            }
        }

        public static HalfVector2f MinValue
        {
            get
            {
                return new HalfVector2f( Half.MinValue, Half.MinValue );
            }
        }

        public static HalfVector2f MaxValue
        {
            get
            {
                return new HalfVector2f( Half.MaxValue, Half.MaxValue );
            }
        }

        #endregion

        #region Functions

        public float Length
        {
            get
            {
                var x = X;
                var y = Y;
                return (float)Math.Sqrt( ( x * x ) + ( y * y ) );
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
                return (double)X * (double)Y;
            }
        }

        public double SideRatio
        {
            get
            {
                return (double)X / (double)Y;
            }
        }

        public void Normalize()
        {
            var length = Length;
            if( length.ApproximatelyEquals( 0f ) )
                return;
            var x = X;
            var y = Y;
            X = x / length;
            Y = y / length;
        }

        public bool IsZero()
        {
            return Length.ApproximatelyEquals( 0f );
        }

        #endregion

        #endregion

        public override string ToString()
        {
            return string.Format(
                "{0}{3}{1}{4}{2}",
                delimiterSetOpen, 
                delimiterSetElement,
                delimiterSetClose,
                X, Y
            );
        }

        public static bool TryParse( string fromString, out HalfVector2f result )
        {
            result = HalfVector2f.Zero;

            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();

            string[] elements = fromString.Split( delimiterChars );

            if( elements.Length != 4 )
                throw new ArgumentException();

            float x, y;
            x = float.Parse( elements[ 1 ], CultureInfo.InvariantCulture );
            y = float.Parse( elements[ 2 ], CultureInfo.InvariantCulture );

            result = new HalfVector2f( x, y );
            return true;
        }

        public void WriteToStream( System.IO.BinaryWriter stream )
        {
            _X.WriteToStream( stream );
            _Y.WriteToStream( stream );
        }

    }
}
