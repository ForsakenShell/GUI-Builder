/*
 * SDLRenderer_Surface.cs
 *
 * Public and internal methods for abstracting SDL_Surfaces.
 *
 * For saving and loading from files, see SDLRenderer_Image.cs
 *
 * User: 1000101
 * Date: 1/31/2018
 * Time: 10:24 AM
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
        
        #region Public API:  Create & Destroy SDLRenderer.Surface
        
        public Surface CreateSurface( int width, int height )
        {
            return Surface.INTERNAL_Surface_Create( this, width, height, this.BitsPerPixel, this.PixelFormat );
        }
        
        public Surface CreateSurface( int width, int height, uint pixelFormat )
        {
            DebugLog.WriteCaller();
            return Surface.INTERNAL_Surface_Create( this, width, height, this.BitsPerPixel, pixelFormat );
        }
        
        public Surface CreateSurface( int width, int height, int bpp, uint pixelFormat )
        {
            return Surface.INTERNAL_Surface_Create( this, width, height, bpp, pixelFormat );
        }
        
        public void DestroySurface( ref Surface surface )
        {
            surface.Dispose();
            surface = null;
        }
        
        #endregion
        
        #region Public API:  SDLRenderer.Surface
        
        public class Surface : IDisposable
        {
            
            #region Internal API:  Surface control objects
            
            internal IntPtr _sdlSurface;
            unsafe internal SDL.SDL_Surface* _sdlSurfacePtr;
            SDLRenderer _sdlRenderer;
            Texture _texture;
            
            uint _PixelFormat;
            int _bpp;
            int _Width;
            int _Height;
            uint _Rmask;
            uint _Gmask;
            uint _Bmask;
            uint _Amask;
            
            bool _mustLock;
            
            int _pitch;
            unsafe void* _pixels;
            
            SDL.SDL_BlendMode _blendMode;
            byte _alphaMod;
            Color _colorMod;
            
            #endregion
            
            #region Public API:  Access to the cached Texture for the Surface.
            
            public Texture Texture
            {
                get
                {
                    if( _texture == null )
                        _texture = Texture.INTERNAL_Texture_Create( this );
                    return _texture;
                }
            }
            
            public void DeleteTexture()
            {
                if( _texture == null ) return;
                _texture.Dispose();
                _texture = null;
            }
            
            #endregion
            
            #region Public API:  The SDL objects associated with this Surface.
            
            public SDLRenderer Renderer
            {
                get
                {
                    return _sdlRenderer;
                }
            }
            
            public IntPtr SDLSurface
            {
                get
                {
                    return _sdlSurface;
                }
            }
            
            unsafe public SDL.SDL_Surface* SDLSurfacePtr
            {
                get
                {
                    return _sdlSurfacePtr;
                }
            }
            
            #endregion
            
            #region Semi-Public API:  Destructor & IDispose
            
            bool _disposed = false;
            
            ~Surface()
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
                
                DeleteTexture();
                
                if( _sdlSurface != IntPtr.Zero )
                    SDL.SDL_FreeSurface( _sdlSurface );
                _sdlSurface = IntPtr.Zero;
                unsafe
                {
                    _sdlSurfacePtr = null;
                }
                _sdlRenderer = null;
                
                _disposed = true;
            }
            
            #endregion
            
            #region Internal:  Surface Creation
            
            bool FillOutInfo()
            {
                unsafe
                {
                    // Get the SDL_Surface*
                    _sdlSurfacePtr = (SDL.SDL_Surface*)( _sdlSurface.ToPointer() );
                    
                    // Fetch the SDL_Surface pixel format
                    var fmtPtr = (uint*)( _sdlSurfacePtr->format.ToPointer() );
                    _PixelFormat = *fmtPtr;
                    
                    // Get the size of the SDL_Surface
                    _Width = _sdlSurfacePtr->w;
                    _Height = _sdlSurfacePtr->h;
                    _pitch = _sdlSurfacePtr->pitch;
                    _pixels = _sdlSurfacePtr->pixels.ToPointer();
                }
                
                // And now it's bits per pixel and channel masks
                if( SDL.SDL_PixelFormatEnumToMasks(
                    _PixelFormat,
                    out _bpp,
                    out _Rmask,
                    out _Gmask,
                    out _Bmask,
                    out _Amask ) == SDL.SDL_bool.SDL_FALSE )
                    return false;
                
                if( SDL.SDL_GetSurfaceBlendMode( _sdlSurface, out _blendMode ) != 0 )
                    return false;
                
                if( SDL.SDL_GetSurfaceAlphaMod( _sdlSurface, out _alphaMod ) != 0 )
                    return false;
                
                byte r, g, b;
                if( SDL.SDL_GetSurfaceColorMod( _sdlSurface, out r, out g, out b ) != 0 )
                    return false;
                _colorMod = Color.FromArgb( r, g, b );
                
                _mustLock = SDL.SDL_MUSTLOCK( _sdlSurface );
                
                return true;
            }
            
            internal static Surface INTERNAL_Surface_Create( SDLRenderer renderer, int width, int height, int bpp, uint pixelFormat )
            {
                // Create Surface instance
                var surface = new Surface();
                
                // Assign the renderer
                surface._sdlRenderer = renderer;
                
                // Create from the renderer
                surface._sdlSurface = SDL.SDL_CreateRGBSurfaceWithFormat( 0, width, height, bpp, pixelFormat );
                if( surface._sdlSurface == IntPtr.Zero )
                {
                    surface.Dispose();
                    return null;
                }
                
                // Fetch the Surface formatting information
                if( !surface.FillOutInfo() )
                {
                    // Someting dun goned wrung
                    surface.Dispose();
                    return null;
                }
                
                return surface;
            }
            
            internal static Surface INTERNAL_Surface_Wrap( SDLRenderer renderer, IntPtr sdlSurface )
            {
                // Create Surface instance
                var surface = new Surface();
                
                // Assign the renderer
                surface._sdlRenderer = renderer;
                
                // Assign the SDL_Surface
                surface._sdlSurface = sdlSurface;
                
                // Fetch the Surface formatting information
                if( !surface.FillOutInfo() )
                {
                    // Someting dun goned wrung
                    surface.Dispose();
                    return null;
                }
                
                return surface;
            }
            
            #endregion
            
            #region Public API:  Surface Locking
            
            public bool MustLock { get { return _mustLock; } }
            
            public bool Lock()
            {
                return SDL.SDL_LockSurface( _sdlSurface ) == 0;
            }
            
            public void Unlock()
            {
                SDL.SDL_UnlockSurface( _sdlSurface );
            }
            
            #endregion
            
            #region Public API:  Surface Properties
            
            /// <summary>
            /// Get/Set the Surface alpha blend mode.
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
                    SDL.SDL_SetSurfaceBlendMode( _sdlSurface, value );
                    if( _texture != null )
                        _texture.BlendMode = value;
                }
            }
            
            /// <summary>
            /// Get/Set the whole Surface alpha modulation.
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
                    SDL.SDL_SetSurfaceAlphaMod( _sdlSurface, value );
                    if( _texture != null )
                        _texture.AlphaMod = value;
                }
            }
            
            /// <summary>
            /// Get/Set the whole Surface color modulation.
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
                    SDL.SDL_SetSurfaceColorMod( _sdlSurface, value.R, value.G, value.B );
                    if( _texture != null )
                        _texture.ColorMod = value;
                }
            }
            
            /// <summary>
            /// The SDL_PIXELFORMAT of the SDL_Surface.
            /// </summary>
            public uint PixelFormat
            {
                get
                {
                    return _PixelFormat;
                }
            }
            
            /// <summary>
            /// The number of Bits Per Pixel (bpp) of the SDL_Surface.
            /// </summary>
            public int BitsPerPixel
            {
                get
                {
                    return _bpp;
                }
            }
            
            /// <summary>
            /// The width of the SDL_Surface.
            /// </summary>
            public int Width
            {
                get
                {
                    return _Width;
                }
            }
            
            /// <summary>
            /// The Height of the SDL_Surface.
            /// </summary>
            public int Height
            {
                get
                {
                    return _Height;
                }
            }
            
            /// <summary>
            /// The Pitch of the pixel buffer.
            /// </summary>
            public int Pitch
            {
                get
                {
                    return _pitch;
                }
            }
            
            /// <summary>
            /// A void* pointer to the raw pixel buffer.
            /// </summary>
            unsafe public void* Pixels
            {
                get
                {
                    return _pixels;
                }
            }
            
            /// <summary>
            /// The alpha channel mask of the SDL_Surface.
            /// </summary>
            public uint AlphaMask
            {
                get
                {
                    return _Amask;
                }
            }
            
            /// <summary>
            /// The red channel mask of the SDL_Surface.
            /// </summary>
            public uint RedMask
            {
                get
                {
                    return _Rmask;
                }
            }
            
            /// <summary>
            /// The green channel mask of the SDL_Surface.
            /// </summary>
            public uint GreenMask
            {
                get
                {
                    return _Gmask;
                }
            }
            
            /// <summary>
            /// The blue channel mask of the SDL_Surface.
            /// </summary>
            public uint BlueMask
            {
                get
                {
                    return _Bmask;
                }
            }
            
            #endregion
            
            #region Public API:  Surface Methods
            
            public uint CompatColor( Color c )
            {
                unsafe
                {
                    return SDL.SDL_MapRGBA( _sdlSurfacePtr->format, c.R, c.G, c.B, c.A );
                }
            }
            
            #endregion
            
            
        }
        
        #endregion
        
    }
}
