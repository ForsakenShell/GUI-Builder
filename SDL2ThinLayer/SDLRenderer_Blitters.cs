/*
 * SDLRenderer_Blitters.cs
 *
 * Public and internal methods for blitting Surfaces and Textures.
 *
 * User: 1000101
 * Date: 01/02/2018
 * Time: 10:50 AM
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
        
        #region Public API:  Surface & Texture Blitters
        
        #region Surface Blitters
        
        /// <summary>
        /// Blits a Surface to the SDL_Window.
        /// 
        /// NOTE:  A Texture will be created from the Surface.  Use Surface.DeleteTexture() after modifying a Surface between blits.
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="surface">The Surface to render</param>
        public void Blit( SDL.SDL_Rect dstRect, Surface surface )
        {
            Blit( dstRect, surface.Texture );
        }
        
        /// <summary>
        /// Blits a Surface to the SDL_Window.
        /// 
        /// NOTE:  A Texture will be created from the Surface.  Use Surface.DeleteTexture() after modifying a Surface between blits.
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="surface">The Surface to render</param>
        /// <param name="srcRect">Region of the Surface to render from.</param>
        public void Blit( SDL.SDL_Rect dstRect, Surface surface, SDL.SDL_Rect srcRect )
        {
            Blit( dstRect, surface.Texture, srcRect );
        }
        
        /// <summary>
        /// Blits a Surface to the SDL_Window.
        /// 
        /// NOTE:  A Texture will be created from the Surface.  Use Surface.DeleteTexture() after modifying a Surface between blits.
        /// NOTE 2:  Rotation is clockwise as per SDL.SDL_RenderCopyEx().
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="surface">The Surface to render</param>
        /// <param name="angle">Angle in degrees to rotate the Surface.</param>
        public void Blit( SDL.SDL_Rect dstRect, Surface surface, double angle )
        {
            Blit( dstRect, surface.Texture, angle );
        }
        
        /// <summary>
        /// Blits a Surface to the SDL_Window.
        /// 
        /// NOTE:  A Texture will be created from the Surface.  Use Surface.DeleteTexture() after modifying a Surface between blits.
        /// NOTE 2:  Rotation is clockwise as per SDL.SDL_RenderCopyEx().
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="surface">The Surface to render</param>
        /// <param name="srcRect">Region of the Surface to render from.</param>
        /// <param name="angle">Angle in degrees to rotate the Surface.</param>
        public void Blit( SDL.SDL_Rect dstRect, Surface surface, SDL.SDL_Rect srcRect, double angle )
        {
            Blit( dstRect, surface.Texture, srcRect, angle );
        }
        
        #endregion
        
        #region Texture Blitters
        
        /// <summary>
        /// Blits a Texture to the SDL_Window.
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="texture">The exture to render</param>
        public void Blit( SDL.SDL_Rect dstRect, Texture texture )
        {
            if( texture == null ) return;
            SDL.SDL_RenderCopy( _sdlRenderer, texture._sdlTexture, IntPtr.Zero, ref dstRect  );
        }
        
        /// <summary>
        /// Blits a Texture to the SDL_Window.
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="texture">The Texture to render</param>
        /// <param name="srcRect">Region of the Texture to render from.</param>
        public void Blit( SDL.SDL_Rect dstRect, Texture texture, SDL.SDL_Rect srcRect )
        {
            if( texture == null ) return;
            SDL.SDL_RenderCopy( _sdlRenderer, texture._sdlTexture, ref srcRect, ref dstRect  );
        }
        
        /// <summary>
        /// Blits a Texture to the SDL_Window.
        /// 
        /// NOTE:  Rotation is clockwise as per SDL.SDL_RenderCopyEx().
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="texture">The Texture to render</param>
        /// <param name="angle">Angle in degrees to rotate the Texture.</param>
        public void Blit( SDL.SDL_Rect dstRect, Texture texture, double angle )
        {
            if( texture == null ) return;
            SDL.SDL_RenderCopyEx( _sdlRenderer, texture._sdlTexture, IntPtr.Zero, ref dstRect, angle, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE );
        }
        
        /// <summary>
        /// Blits a Texture to the SDL_Window.
        /// 
        /// NOTE:  Rotation is clockwise as per SDL.SDL_RenderCopyEx().
        /// </summary>
        /// <param name="dstRect">Position and size on the SDL_Window to render to.</param>
        /// <param name="texture">The Texture to render</param>
        /// <param name="srcRect">Region of the Texture to render from.</param>
        /// <param name="angle">Angle in degrees to rotate the Surface.</param>
        public void Blit( SDL.SDL_Rect dstRect, Texture texture, SDL.SDL_Rect srcRect, double angle )
        {
            if( texture == null ) return;
            SDL.SDL_RenderCopyEx( _sdlRenderer, texture._sdlTexture, ref srcRect, ref dstRect, angle, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE );
        }
        
        #endregion
        
        #endregion
        
    }
}
