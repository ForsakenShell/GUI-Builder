/*
 * SpaceConversions.cs
 * 
 * Global space conversion [extension] functions used by GUIBuilder.
 *
 */
using System;
using System.Drawing;

using Maths;

using SDL2;

namespace GUIBuilder
{
    
    public static class SpaceConversions
    {
        
        #region Some Extension Methods
        
        public static Vector2i Centre( this System.Drawing.Rectangle rectangle )
        {
            return new Vector2i( rectangle.X + rectangle.Width >> 1, rectangle.Y + rectangle.Height >> 1 );
        }
        
        public static Vector2i Centre( this SDL2.SDL.SDL_Rect rectangle )
        {
            return new Vector2i( rectangle.x + rectangle.w >> 1, rectangle.y + rectangle.h >> 1 );
        }
        
        #endregion
        
        #region Unit Type Conversions (Vector2i->SDL.SDL_Point, etc)
        
        public static SDL.SDL_Point ToSDLPoint( this Vector2i v )
        {
            return new SDL.SDL_Point( v.X, v.Y );
        }
        
        public static SDL.SDL_Point ToSDLPoint( this Vector2f v, bool rounded = false )
        {
            return rounded ?
                new SDL.SDL_Point( (int)Math.Round( v.X ), (int)Math.Round( v.Y ) ) :
                new SDL.SDL_Point( ( int )v.X, ( int )v.Y );
        }
        
        public static Size ToSize( this SDL.SDL_Rect rect )
        {
            return new Size( rect.w, rect.h );
        }
        
        public static SDL.SDL_Rect ToSDLRect( this Size size )
        {
            return new SDL.SDL_Rect( 0, 0, size.Width, size.Height );
        }
        
        public static SDL.SDL_Point ToSDLPoint( this Size size )
        {
            return new SDL.SDL_Point( size.Width, size.Height );
        }
        
        public static Vector2i ToVector2i( this Size size )
        {
            return new Vector2i( size.Width, size.Height );
        }
        
        #endregion
        
    }
}