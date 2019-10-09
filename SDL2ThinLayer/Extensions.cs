/*
 * Extensions.cs
 *
 * Useful extensions to structs and classes.
 *
 * User: 1000101
 * Date: 01/02/2018
 * Time: 12:31 PM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    public static class Extensions
    {
        
        // string delimiters for ToString() and FromString()
        const char delimiterSetOpen = '(';
        const char delimiterSetClose = ')';
        const char delimiterSetElement= ',';
        static char[] delimiterChars = { delimiterSetOpen, delimiterSetClose, delimiterSetElement };
        const string minSDLPointTemplateString = "(0,0)";
        
        #region SDL_Point Extensions
        
        #region Equality
        
        /// <summary>
        /// Equality of two SDL_Points
        /// </summary>
        public static bool Equals( this SDL.SDL_Point left, SDL.SDL_Point right )
        {
            return( left.x == right.x )&&( left.y == right.y );
        }
        
        /// <summary>
        /// An an SDL_Point (0,0)?
        /// </summary>
        public static bool IsZero( this SDL.SDL_Point left )
        {
            return( left.x == 0 )&&( left.y == 0 );
        }
        
        #endregion
        
        #region Maths
        
        /// <summary>
        /// Add two SDL_Points
        /// </summary>
        public static SDL.SDL_Point Add( this SDL.SDL_Point left, SDL.SDL_Point right )
        {
            return new SDL.SDL_Point(
                left.x + right.x,
                left.y + right.y );
        }
        
        /// <summary>
        /// Subtract two SDL_Points
        /// </summary>
        public static SDL.SDL_Point Sub( this SDL.SDL_Point left, SDL.SDL_Point right )
        {
            return new SDL.SDL_Point(
                left.x - right.x,
                left.y - right.y );
        }
        
        /// <summary>
        /// Multiply two SDL_Points
        /// </summary>
        public static SDL.SDL_Point Mul( this SDL.SDL_Point left, SDL.SDL_Point right )
        {
            return new SDL.SDL_Point(
                left.x * right.x,
                left.y * right.y );
        }
        
        /// <summary>
        /// Multiply an SDL_Point by a scalar
        /// </summary>
        public static SDL.SDL_Point Mul( this SDL.SDL_Point left, int right )
        {
            return new SDL.SDL_Point(
                left.x * right,
                left.y * right );
        }
        
        /// <summary>
        /// Divide two SDL_Points
        /// </summary>
        public static SDL.SDL_Point Div( this SDL.SDL_Point left, SDL.SDL_Point right )
        {
            return new SDL.SDL_Point(
                left.x / right.x,
                left.y / right.y );
        }
        
        /// <summary>
        /// Divide an SDL_Point by a scalar
        /// </summary>
        public static SDL.SDL_Point Div( this SDL.SDL_Point left, int right )
        {
            return new SDL.SDL_Point(
                left.x / right,
                left.y / right );
        }
        
        /// <summary>
        /// Length of an SDL_Point (distance from (0,0))
        /// </summary>
        public static int Length( this SDL.SDL_Point left )
        {
            return (int)Math.Sqrt( ( left.x * left.x ) + ( left.y * left.y ) );
        }
        
        /// <summary>
        /// Length of an SDL_Point (distance from (0,0))
        /// </summary>
        public static float LengthF( this SDL.SDL_Point left )
        {
            return (float)Math.Sqrt( ( left.x * left.x ) + ( left.y * left.y ) );
        }
        
        #endregion
        
        #region Conversions
        
        /// <summary>
        /// Returns a string representation of an SDL_Point
        /// </summary>
        public static string ToString( this SDL.SDL_Point left, string format = null )
        {
            if( string.IsNullOrEmpty( format ) )
                format = "({0},{1})";
            return string.Format( format, left.x, left.y );
        }
        
        /// <summary>
        /// Convert an SDL_Point to a System.Drawing.Point
        /// </summary>
        public static Point ToPoint( this SDL.SDL_Point left )
        {
            return new Point( left.x, left.y );
        }
        
        /// <summary>
        /// Convert a System.Drawing.Point to an SDL_Point
        /// </summary>
        public static SDL.SDL_Point ToSDLPoint( this Point left )
        {
            return new SDL.SDL_Point(
                left.X,
                left.Y );
        }
        
        /// <summary>
        /// Convert a string to an SDL_Point
        /// </summary>
        public static bool TryParseSDLPoint( string fromString, out SDL.SDL_Point result )
        {
            result = new SDL.SDL_Point( 0, 0 );
            
            if( fromString.Length < minSDLPointTemplateString.Length )
                throw new ArgumentException();
            
            string[] elements = fromString.Split( delimiterChars );
            
            if( elements.Length != 4 )
                throw new ArgumentException();
            
            int x, y;
            if( !int.TryParse( elements[ 1 ], out x ) )
                throw new ArgumentException();
            if( !int.TryParse( elements[ 2 ], out y ) )
                throw new ArgumentException();
            
            result.x = x;
            result.y = y;
            return true;
        }
        
        #endregion
        
        #endregion
        
        #region SDL_Rect Extensions
        
        #region Equality
        
        /// <summary>
        /// Equality of two SDL_Rects
        /// </summary>
        public static bool Equals( this SDL.SDL_Rect left, SDL.SDL_Rect right )
        {
            return( left.x == right.x )&&( left.y == right.y )&&( left.w == right.w )&&( left.h == right.h );
        }
        
        /// <summary>
        /// An an SDL_Rect (0,0,0,0)?
        /// </summary>
        public static bool IsZero( this SDL.SDL_Rect left )
        {
            return( left.x == 0 )&&( left.y == 0 )&&( left.w == 0 )&&( left.y == 0 );
        }
        
        #endregion
        
        #region Conversions
        
        /// <summary>
        /// Returns a string representation of an SDL_Rect
        /// </summary>
        public static string ToString( this SDL.SDL_Rect left, string format = null )
        {
            if( string.IsNullOrEmpty( format ) )
                format = "({0},{1})-({2},{3})";
            return string.Format( format, left.x, left.y, left.w, left.h );
        }
        
        /// <summary>
        /// Convert an SDL_Rect to a System.Drawing.Rectangle
        /// </summary>
        public static Rectangle ToRectangle( this SDL.SDL_Rect left )
        {
            return new Rectangle( left.x, left.y, left.w, left.h );
        }
        
        /// <summary>
        /// Convert a System.Drawing.Rectangle to an SDL_Rect
        /// </summary>
        public static SDL.SDL_Rect ToSDLRect( this Rectangle left )
        {
            return new SDL.SDL_Rect(
                left.X,
                left.Y,
                left.Width,
                left.Height );
        }
        
        #endregion
        
        #endregion
        
        #region SDL_Color Extensions
        
        #region Equality
        
        /// <summary>
        /// Equality of two SDL_Colors
        /// </summary>
        public static bool Equals( this SDL.SDL_Color left, SDL.SDL_Color right )
        {
            return( left.r == right.r )&&( left.g == right.g )&&( left.b == right.b )&&( left.a == right.a );
        }
        
        /// <summary>
        /// An an SDL_Color (0,0,0,0)?
        /// </summary>
        public static bool IsZero( this SDL.SDL_Color left )
        {
            return( left.r == 0 )&&( left.g == 0 )&&( left.b == 0 )&&( left.a == 0 );
        }
        
        #endregion
        
        #region Conversions
        
        /// <summary>
        /// Returns a string representation of an SDL_Color
        /// </summary>
        public static string ToString( this SDL.SDL_Color left, string format = null )
        {
            if( string.IsNullOrEmpty( format ) )
                format = "({0},{1},{2},{3})";
            return string.Format( format, left.a, left.r, left.g, left.b );
        }
        
        /// <summary>
        /// Convert an SDL_Color to a System.Drawing.Color
        /// </summary>
        public static Color ToColor( this SDL.SDL_Color left )
        {
            return Color.FromArgb( left.a, left.r, left.g, left.b );
        }
        
        /// <summary>
        /// Convert a System.Drawing.Color to an SDL_Color
        /// </summary>
        public static SDL.SDL_Color ToSDLColor( this Color left )
        {
            return new SDL.SDL_Color(
                left.R,
                left.G,
                left.B,
                left.A );
        }
        
        #endregion
        
        #endregion
        
        #region List Extensions
        
        /// <summary>
        /// Is the list null or contain 0 elements?
        /// </summary>
        /// <param name="list">List to test for null or empty</param>
        /// <returns></returns>
        public static bool NullOrEmpty<T>( this IList<T> list )
        {
            if( list == null ) return true;
            return list.Count == 0;
        }
        
        #endregion
        
    }
}
