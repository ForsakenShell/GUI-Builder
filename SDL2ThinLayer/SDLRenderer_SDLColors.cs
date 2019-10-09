/*
 * SDLRenderer_SDLColors.cs
 *
 * Static predefined SDL_Colors.
 *
 * User: 1000101
 * Date: 05/02/2018
 * Time: 9:29 AM
 * 
 */
using System;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    public partial class SDLRenderer : IDisposable
    {
        
        #region Predefined SDL_Colors
        
        static SDL.SDL_Color _sdlWhite = Color.White.ToSDLColor();
        public static SDL.SDL_Color SDLWhite { get { return _sdlWhite; } }
        
        static SDL.SDL_Color _sdlBlack = Color.Black.ToSDLColor();
        public static SDL.SDL_Color SDLBlack { get { return _sdlBlack; } }
        
        static SDL.SDL_Color _sdlAliceBlue = Color.AliceBlue.ToSDLColor();
        public static SDL.SDL_Color SDLAliceBlue { get { return _sdlAliceBlue; } }
        
        #endregion
        
    }
}
