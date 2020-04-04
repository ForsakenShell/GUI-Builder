/*
 * HalfVector3f.cs
 * 
 * A struct and associated functions to work with 2d (16-bit float) x,y structs.
 * 
 * User: 1000101
 * Date: 02/02/2020
 * Time: 12:48 PM
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
    public struct HalfVector3f : IEquatable<HalfVector3f>
    {

        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        static string minTemplateString = "(0,0,0)";

        // Component values
        Half _X, _Y, _Z;

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

        public float Z
        {
            get { return HalfHelper.HalfToSingle( _Z ); }
            set { _Z = HalfHelper.SingleToHalf( value ); }
        }
        public HalfVector3f( float x, float y, float z )
        {
            _X = HalfHelper.SingleToHalf( x );
            _Y = HalfHelper.SingleToHalf( y );
            _Z = HalfHelper.SingleToHalf( z );
        }

        public HalfVector3f( HalfVector3f v )
        {
            _X = new Half( v.X );
            _Y = new Half( v.Y );
            _Z = new Half( v.Z );
        }

        public HalfVector3f( Vector3f v )
        {
            _X = HalfHelper.SingleToHalf( v.X );
            _Y = HalfHelper.SingleToHalf( v.Y );
            _Z = HalfHelper.SingleToHalf( v.Z );
        }

        public HalfVector3f( Vector3i v )
        {
            _X = new Half( v.X );
            _Y = new Half( v.Y );
            _Z = new Half( v.Z );
        }

        #region Equality (IEquatable)

        public override bool Equals( object obj )
        {
            if( obj is HalfVector3f )
                return Equals( (HalfVector3f)obj ); // use Equals method below
            else
                return false;
        }

        public bool Equals( HalfVector3f other )
        {
            // add comparisions for all members here
            //return ( this - other ).Length.ApproximatelyEquals( 0f );
            return
                ( this.X.ApproximatelyEquals( other.X ) ) &&
                ( this.Y.ApproximatelyEquals( other.Y ) ) &&
                ( this.Z.ApproximatelyEquals( other.Z ) );
        }

        public override int GetHashCode()
        {
            // combine the hash codes of all members here (e.g. with XOR operator ^)
            return _X.GetHashCode() ^ _Y.GetHashCode() ^ _Z.GetHashCode();
        }

        public static bool operator ==( HalfVector3f left, HalfVector3f right )
        {
            return left.Equals( right );
        }

        public static bool operator !=( HalfVector3f left, HalfVector3f right )
        {
            return !left.Equals( right );
        }

        #endregion

        #region Math operators and functions

        #region Operators

        public static HalfVector3f operator +( HalfVector3f left, HalfVector3f right )
        {
            return new HalfVector3f( left.X + right.X, left.Y + right.Y, left.Z + right.Z );
        }

        public static HalfVector3f operator +( HalfVector3f left, Vector3f right )
        {
            return new HalfVector3f( new Half( left.X + right.X ), new Half( left.Y + right.Y ), new Half( left.Z + right.Z ) );
        }

        public static HalfVector3f operator +( HalfVector3f left, Vector3i right )
        {
            return new HalfVector3f( new Half( left.X + right.X ), new Half( left.Y + right.Y ), new Half( left.Z + right.Z ) );
        }

        public static HalfVector3f operator -( HalfVector3f left, HalfVector3f right )
        {
            return new HalfVector3f( left.X - right.X, left.Y - right.Y, left.Z - right.Z );
        }

        public static HalfVector3f operator -( HalfVector3f left, Vector3f right )
        {
            return new HalfVector3f( new Half( left.X - right.X ), new Half( left.Y - right.Y ), new Half( left.Z - right.Z ) );
        }

        public static HalfVector3f operator -( HalfVector3f left, Vector3i right )
        {
            return new HalfVector3f( new Half( left.X - (float)right.X ), new Half( left.Y - (float)right.Y ), new Half( left.Z - (float)right.Z ) );
        }

        public static HalfVector3f operator *( HalfVector3f left, HalfVector3f right )
        {
            return new HalfVector3f( left.X * right.X, left.Y * right.Y, left.Z * right.Z );
        }

        public static HalfVector3f operator *( HalfVector3f left, Vector3f right )
        {
            return new HalfVector3f( new Half( left.X * right.X ), new Half( left.Y * right.Y ), new Half( left.Z * right.Z ) );
        }

        public static HalfVector3f operator *( HalfVector3f left, float right )
        {
            return new HalfVector3f( new Half( left.X * right ), new Half( left.Y * right ), new Half( left.Z * right ) );
        }

        public static HalfVector3f operator /( HalfVector3f left, HalfVector3f right )
        {
            return new HalfVector3f( left.X / right.X, left.Y / right.Y, left.Z / right.Z );
        }

        public static HalfVector3f operator /( HalfVector3f left, Vector3f right )
        {
            return new HalfVector3f( new Half( left.X / right.X ), new Half( left.Y / right.Y ), new Half( left.Z / right.Z ) );
        }

        public static HalfVector3f operator /( HalfVector3f left, float right )
        {
            return new HalfVector3f( new Half( left.X / right ), new Half( left.Y / right ), new Half( left.Y / right ) );
        }

        public static HalfVector3f operator /( HalfVector3f left, int right )
        {
            return new HalfVector3f( new Half( left.X / (float)right ), new Half( left.Y / (float)right ), new Half( left.Z / (float)right ) );
        }

        #endregion

        #region Special Values

        public static HalfVector3f Zero
        {
            get
            {
                return new HalfVector3f( 0.0f, 0.0f, 0.0f );
            }
        }

        public static HalfVector3f MinValue
        {
            get
            {
                return new HalfVector3f( Half.MinValue, Half.MinValue, Half.MinValue );
            }
        }

        public static HalfVector3f MaxValue
        {
            get
            {
                return new HalfVector3f( Half.MaxValue, Half.MaxValue, Half.MaxValue );
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
                var z = Z;
                return (float)Math.Sqrt( ( x * x ) + ( y * y ) + ( z * z ) );
            }
        }

        public double Volume
        {
            get
            {
                return (double)X * (double)Y * (double)Z;
            }
        }

        public void Normalize()
        {
            var length = Length;
            if( length.ApproximatelyEquals( 0f ) )
                return;
            var x = X;
            var y = Y;
            var z = Z;
            X = x / length;
            Y = y / length;
            Z = y / length;
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
                "{0}{3}{1}{4}{1}{5}{2}",
                delimiterSetOpen,
                delimiterSetElement,
                delimiterSetClose,
                X, Y, Z
            );
        }

        public static bool TryParse( string fromString, out HalfVector3f result )
        {
            result = HalfVector3f.Zero;

            if( fromString.Length < minTemplateString.Length )
                throw new ArgumentException();

            string[] elements = fromString.Split( delimiterChars );

            if( elements.Length != 5 )
                throw new ArgumentException();

            float x, y, z;
            x = float.Parse( elements[ 1 ], CultureInfo.InvariantCulture );
            y = float.Parse( elements[ 2 ], CultureInfo.InvariantCulture );
            z = float.Parse( elements[ 3 ], CultureInfo.InvariantCulture );

            result = new HalfVector3f( x, y, z );
            return true;
        }

        public void WriteToStream( System.IO.BinaryWriter stream )
        {
            _X.WriteToStream( stream );
            _Y.WriteToStream( stream );
            _Z.WriteToStream( stream );
        }

    }
}
