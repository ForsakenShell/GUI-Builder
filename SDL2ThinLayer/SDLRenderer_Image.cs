/*
 * SDLRenderer_Image.cs
 *
 * Handles saving the SDLRenderer window and Surfaces to image files.
 *
 * User: 1000101
 * Date: 04/02/2018
 * Time: 1:06 AM
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
        
        #region Internal:  Save SDL_Surface
        
        bool INTERNAL_Save_SDLSurface( IntPtr sdlSurface, ImageTypes fileType, string filename )
        {
            if( string.IsNullOrEmpty( filename ) ) return false;
            if( sdlSurface == IntPtr.Zero ) return false;
            
            bool ret = false;
            var mustLock = SDL.SDL_MUSTLOCK( sdlSurface );
            
            if( mustLock )
                SDL.SDL_LockSurface( sdlSurface );
            
            
            switch( fileType )
            {
                case ImageTypes.PNG :
                {
                    // NOTE: THIS IS BROKEN IN SDL2!  NEED TO GET A REPLACEMENT FUNCTION FOR IT!
                    ret = SDL_image.IMG_SavePNG( sdlSurface, filename ) == 0;
                    break;
                }
                default:
                {
                    break;
                }
            }
            
            if( mustLock )
                SDL.SDL_UnlockSurface( sdlSurface );
            
            return ret;
        }
        
        #endregion
        
        #region Public API:  Save/Load SDLRenderer and Surfaces
        
        /// <summary>
        /// Save the SDL_Window contents to a file.
        /// 
        /// WARNING:  DO NOT CALL THIS FROM THE SDLThread!  This will force the recreation of the SDL_Window and SDL_Renderer as per SDL_GetWindowSurface.
        /// 
        /// NOTE: Due to the way SDL2 works, you will need to manually recreate Surfaces and Textures after calling this.
        /// </summary>
        public bool SaveSurface( ImageTypes fileType, string filename )
        {
            _windowSaveFormat = fileType;
            _windowSaveFilename = filename;
            _windowSaved = false;
            
            INTERNAL_SDLThread_WaitForBool( ref _windowSaveRequested, true, false, 0 );
            
            return _windowSaved;
        }
        
        public bool SaveSurface( Surface surface, ImageTypes fileType, string filename )
        {
            return INTERNAL_Save_SDLSurface( surface.SDLSurface, fileType, filename );
        }
        
        public Surface LoadSurface( string filename )
        {
            if( string.IsNullOrEmpty( filename ) ) return null;
            
            var sdlSurface = SDL_image.IMG_Load( filename );
            
            return sdlSurface == IntPtr.Zero ? null : Surface.INTERNAL_Surface_Wrap( this, sdlSurface );
        }
        
        #endregion
        
    }
}
