/*
 * SDLRenderer_Texture.cs
 *
 * Public and internal methods for abstracting SDL_Textures.
 *
 * For saving and loading from files, see SDLRenderer_Image.cs
 *
 * User: 1000101
 * Date: 1/31/2018
 * Time: 10:27 AM
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
        
        #region Public API:  Create & Destroy SDLRenderer.Texture
        
        public Texture CreateTextureFromSurface( Surface surface )
        {
            return Texture.INTERNAL_Texture_Create( surface );
        }
        
        public void DestroyTexture( ref Texture texture )
        {
            texture.Dispose();
            texture = null;
        }
        
        #endregion
        
        #region Public API:  SDLRenderer.Texture
        
        public class Texture
        {
            
            #region Internal API:  Surface control objects
            
            internal IntPtr _sdlTexture;
            
            SDLRenderer _renderer;
            
            uint _PixelFormat;
            int _bpp;
            int _Width;
            int _Height;
            uint _Rmask;
            uint _Gmask;
            uint _Bmask;
            uint _Amask;
            
            SDL.SDL_BlendMode _blendMode;
            byte _alphaMod;
            Color _colorMod;
            
            #endregion
            
            #region Public API:  The SDL objects associated with this Texture.
            
            public SDLRenderer Renderer
            {
                get
                {
                    return _renderer;
                }
            }
            
            public IntPtr SDLTexture
            {
                get
                {
                    return _sdlTexture;
                }
            }
            
            #endregion
            
            #region Semi-Public API:  Destructor & IDispose
            
            bool _disposed = false;
            
            ~Texture()
            {
                Dispose( false );
            }
            
            public void Dispose()
            {
                Dispose( true );
                GC.SuppressFinalize( this );
            }
            
            protected virtual void Dispose( bool disposing )
            {
                if( _disposed ) return;
                
                if( _sdlTexture != IntPtr.Zero )
                    SDL.SDL_DestroyTexture( _sdlTexture );
                _sdlTexture = IntPtr.Zero;
                _renderer = null;
                
                _disposed = true;
            }
            
            #endregion
            
            #region Internal:  Texture Creation
            
            bool FillOutInfo( Surface surface )
            {
                _PixelFormat = surface.PixelFormat;
                _Width = surface.Width;
                _Height = surface.Height;
                
                // And now it's bits per pixel and channel masks
                if( SDL.SDL_PixelFormatEnumToMasks(
                    _PixelFormat,
                    out _bpp,
                    out _Rmask,
                    out _Gmask,
                    out _Bmask,
                    out _Amask ) == SDL.SDL_bool.SDL_FALSE )
                    return false;
                
                if( SDL.SDL_GetTextureBlendMode( _sdlTexture, out _blendMode ) != 0 )
                    return false;
                
                if( SDL.SDL_GetTextureAlphaMod( _sdlTexture, out _alphaMod ) != 0 )
                    return false;
                
                byte r, g, b;
                if( SDL.SDL_GetTextureColorMod( _sdlTexture, out r, out g, out b ) != 0 )
                    return false;
                _colorMod = Color.FromArgb( r, g, b );
                
                return true;
            }
            
            internal static Texture INTERNAL_Texture_Create( Surface surface )
            {
                // Create Texture instance
                var texture = new Texture();
                
                // Assign the renderer
                texture._renderer = surface.Renderer;
                
                // Create from the surface
                texture._sdlTexture = SDL.SDL_CreateTextureFromSurface( texture.Renderer.Renderer, surface.SDLSurface );
                if( texture._sdlTexture == IntPtr.Zero )
                {
                    texture.Dispose();
                    return null;
                }
                
                // Fetch the Texture formatting information
                if( !texture.FillOutInfo( surface ) )
                {
                    // Someting dun goned wrung
                    texture.Dispose();
                    return null;
                }
                
                // Copy SDL_Surface characteristics to the SDL_Texture
                texture.BlendMode = surface.BlendMode;
                texture.AlphaMod = surface.AlphaMod;
                texture.ColorMod = surface.ColorMod;
                
                return texture;
            }
            
            #endregion
            
            #region Public API:  Texture Properties
            
            /// <summary>
            /// Get/Set the Texture alpha blend mode.
            /// </summary>
            public SDL.SDL_BlendMode BlendMode
            {
                get
                {
                    return _blendMode;
                }
                set
                {
                    _blendMode = value;
                    SDL.SDL_SetTextureBlendMode( _sdlTexture, value );
                }
            }
            
            /// <summary>
            /// Get/Set the whole Texture alpha modulation.
            /// </summary>
            public byte AlphaMod
            {
                get
                {
                    return _alphaMod;
                }
                set
                {
                    _alphaMod = value;
                    SDL.SDL_SetTextureAlphaMod( _sdlTexture, value );
                }
            }
            
            /// <summary>
            /// Get/Set the whole Texture color modulation.
            /// </summary>
            public Color ColorMod
            {
                get
                {
                    return _colorMod;
                }
                set
                {
                    _colorMod = value;
                    SDL.SDL_SetTextureColorMod( _sdlTexture, value.R, value.G, value.B );
                }
            }
            
            /// <summary>
            /// The SDL_PIXELFORMAT of the SDL_Texture.
            /// </summary>
            public uint PixelFormat
            {
                get
                {
                    return _PixelFormat;
                }
            }
            
            /// <summary>
            /// The number of Bits Per Pixel (bpp) of the SDL_Texture.
            /// </summary>
            public int BitsPerPixel
            {
                get
                {
                    return _bpp;
                }
            }
            
            /// <summary>
            /// The width of the SDL_Texture.
            /// </summary>
            public int Width
            {
                get
                {
                    return _Width;
                }
            }
            
            /// <summary>
            /// The Height of the SDL_Texture.
            /// </summary>
            public int Height
            {
                get
                {
                    return _Height;
                }
            }
            
            /// <summary>
            /// The alpha channel mask of the SDL_Texture.
            /// </summary>
            public uint AlphaMask
            {
                get
                {
                    return _Amask;
                }
            }
            
            /// <summary>
            /// The red channel mask of the SDL_Texture.
            /// </summary>
            public uint RedMask
            {
                get
                {
                    return _Rmask;
                }
            }
            
            /// <summary>
            /// The green channel mask of the SDL_Texture.
            /// </summary>
            public uint GreenMask
            {
                get
                {
                    return _Gmask;
                }
            }
            
            /// <summary>
            /// The blue channel mask of the SDL_Texture.
            /// </summary>
            public uint BlueMask
            {
                get
                {
                    return _Bmask;
                }
            }
            
            #endregion
            
        }
        
        #endregion
        
    }
}
