/*
 * SDLRenderer_Font.cs
 *
 * Public and internal methods for abstracting SDL_Fonts.
 *
 * User: 1000101
 * Date: 05/02/2018
 * Time: 7:39 AM
 * 
 */
using System;
using System.Collections.Generic;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    public partial class SDLRenderer : IDisposable
    {
        
        #region Public API:  Create & Destroy SDLRenderer.Font
        
        public Font CreateFont( int ptSize, string filename )
        {
            return Font.INTERNAL_Font_Create( this, ptSize, filename );
        }
        
        public void DestroyFont( ref Font font )
        {
            font.Dispose();
            font = null;
        }
        
        #endregion
        
        
        #region Public API:  SDLRenderer.Font
        
        public class Font : IDisposable
        {
            
            public class GlyphMetrics
            {
                public int MinX;
                public int MaxX;
                public int MinY;
                public int MaxY;
                public int Advance;
                
                public GlyphMetrics( int minX, int maxX, int minY, int maxY, int advance )
                {
                    MinX = minX;
                    MaxX = maxX;
                    MinY = minY;
                    MaxY = maxY;
                    Advance = advance;
                }
            }
            
            public class FontMetrics
            {
                public int Height;
                public int Ascent;
                public int Descent;
                public int LineSkip;
                
                public FontMetrics( int height, int ascent, int descent, int lineSkip )
                {
                    Height = height;
                    Ascent = ascent;
                    Descent = descent;
                    LineSkip = lineSkip;
                }
            }
            
            #region Internal:  Font control objects
            
            internal IntPtr _sdlFont;
            SDLRenderer _sdlRenderer;
            
            int _ptSize;
            
            FontMetrics _fontMetrics;
            
            int _style;
            int _outline;
            int _hinting;
            bool _kerning;
            
            #endregion
            
            #region Public API:  The SDL objects associated with this Font.
            
            public SDLRenderer Renderer
            {
                get
                {
                    return _sdlRenderer;
                }
            }
            
            #endregion
            
            #region Semi-Public API:  Destructor & IDispose
            
            bool _disposed = false;
            
            ~Font()
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
                
                if( _sdlFont != IntPtr.Zero )
                    SDL2.SDL_ttf.TTF_CloseFont( _sdlFont );
                _sdlFont = IntPtr.Zero;
                _sdlRenderer = null;
                
                _disposed = true;
            }
            
            #endregion
            
            #region Internal:  Font Creation
            
            bool FillOutInfo()
            {
                _fontMetrics = new FontMetrics(
                    SDL_ttf.TTF_FontHeight( _sdlFont ),
                    SDL_ttf.TTF_FontAscent( _sdlFont ),
                    SDL_ttf.TTF_FontDescent( _sdlFont ),
                    SDL_ttf.TTF_FontDescent( _sdlFont ) );
                
                _style   = SDL_ttf.TTF_GetFontStyle( _sdlFont );
                _outline = SDL_ttf.TTF_GetFontOutline( _sdlFont );
                _hinting = SDL_ttf.TTF_GetFontHinting( _sdlFont );
                _kerning = SDL_ttf.TTF_GetFontKerning( _sdlFont ) != 0;
                
                return true;
            }
            
            internal static Font INTERNAL_Font_Create( SDLRenderer renderer, int ptSize, string filename )
            {
                // Create Font instance
                var font = new Font();
                
                // Assign the renderer
                font._sdlRenderer = renderer;
                
                // Create from the renderer
                font._ptSize = ptSize;
                font._sdlFont = SDL_ttf.TTF_OpenFont( filename, ptSize );
                if( font._sdlFont == IntPtr.Zero )
                {
                    font.Dispose();
                    return null;
                }
                
                // Fetch the Surface formatting information
                if( !font.FillOutInfo() )
                {
                    // Someting dun goned wrung
                    font.Dispose();
                    return null;
                }
                
                return font;
            }
            
            #endregion
            
            #region Public API:  Font Properties
            
            /// <summary>
            /// Get the Font metrics.
            /// </summary>
            public FontMetrics Metrics
            {
                get
                {
                    return _fontMetrics;
                }
            }
            
            #region Style
            
            /// <summary>
            /// Get/Set the Font style.  See SDL2.SDL_ttf.TTF_STYLE
            /// </summary>
            public int Style
            {
                get
                {
                    return _style;
                }
                set
                {
                    _style = value;
                    SDL_ttf.TTF_SetFontStyle( _sdlFont, value );
                }
            }
            
            /// <summary>
            /// Get/Set the Font bold style.
            /// </summary>
            public bool Bold
            {
                get
                {
                    return ( _style & SDL_ttf.TTF_STYLE_BOLD ) != 0;
                }
                set
                {
                    if( value )
                        _style |= SDL_ttf.TTF_STYLE_BOLD;
                    else
                        _style &= ~SDL_ttf.TTF_STYLE_BOLD;
                    SDL_ttf.TTF_SetFontStyle( _sdlFont, _style );
                }
            }
            
            /// <summary>
            /// Get/Set the Font italic style.
            /// </summary>
            public bool Italic
            {
                get
                {
                    return ( _style & SDL_ttf.TTF_STYLE_ITALIC ) != 0;
                }
                set
                {
                    if( value )
                        _style |= SDL_ttf.TTF_STYLE_ITALIC;
                    else
                        _style &= ~SDL_ttf.TTF_STYLE_ITALIC;
                    SDL_ttf.TTF_SetFontStyle( _sdlFont, _style );
                }
            }
            
            /// <summary>
            /// Get/Set the Font underline style.
            /// </summary>
            public bool Underline
            {
                get
                {
                    return ( _style & SDL_ttf.TTF_STYLE_UNDERLINE ) != 0;
                }
                set
                {
                    if( value )
                        _style |= SDL_ttf.TTF_STYLE_UNDERLINE;
                    else
                        _style &= ~SDL_ttf.TTF_STYLE_UNDERLINE;
                    SDL_ttf.TTF_SetFontStyle( _sdlFont, _style );
                }
            }
            
            /// <summary>
            /// Get/Set the Font strikethrough style.
            /// </summary>
            public bool StrikeThrough
            {
                get
                {
                    return ( _style & SDL_ttf.TTF_STYLE_STRIKETHROUGH ) != 0;
                }
                set
                {
                    if( value )
                        _style |= SDL_ttf.TTF_STYLE_STRIKETHROUGH;
                    else
                        _style &= ~SDL_ttf.TTF_STYLE_STRIKETHROUGH;
                    SDL_ttf.TTF_SetFontStyle( _sdlFont, _style );
                }
            }
            
            #endregion
            
            /// <summary>
            /// Get/Set the Font outline width in pixels.
            /// </summary>
            public int Outline
            {
                get
                {
                    return _outline;
                }
                set
                {
                    _outline = value;
                    SDL_ttf.TTF_SetFontOutline( _sdlFont, value );
                }
            }
            
            /// <summary>
            /// Get/Set the Font Hinting.  See SDL2.SDL_ttf.TTF_HINTING
            /// </summary>
            public int Hinting
            {
                get
                {
                    return _hinting;
                }
                set
                {
                    _hinting = value;
                    SDL_ttf.TTF_SetFontHinting( _sdlFont, value );
                }
            }
            
            /// <summary>
            /// Get/Set the Font Kerning.
            /// </summary>
            public bool Kerning
            {
                get
                {
                    return _kerning;
                }
                set
                {
                    _kerning = value;
                    SDL_ttf.TTF_SetFontKerning( _sdlFont, value ? 1 : 0 );
                }
            }
            
            #endregion
            
            #region Public API:  Font Methods
            
            public Size TextSize( string text )
            {
                int w, h;
                return SDL_ttf.TTF_SizeText( _sdlFont, text, out w, out h ) == 0 ? new Size( w, h ) : Size.Empty;
            }
            
            public GlyphMetrics Metric( UInt16 ch )
            {
                int minX, minY, maxX, maxY, advance;
                return SDL_ttf.TTF_GlyphMetrics( _sdlFont, ch, out minX, out maxX, out minY, out maxY, out advance ) == 0 ? new GlyphMetrics( minX, maxX, minY, maxY, advance ) : null;
            }
            
            public bool HasGlyph( UInt16 ch )
            {
                return SDL_ttf.TTF_GlyphIsProvided( _sdlFont, ch ) > 0;
            }
            
            public Surface TextSolid( string text, Color c )
            {
                if( string.IsNullOrEmpty( text ) ) return null;
                var sdlSurface = SDL_ttf.TTF_RenderText_Solid( _sdlFont, text, c.ToSDLColor() );
                return sdlSurface == IntPtr.Zero ? null : Surface.INTERNAL_Surface_Wrap( _sdlRenderer, sdlSurface );
            }
            
            public Surface TextShaded( string text, Color fg, Color bg )
            {
                if( string.IsNullOrEmpty( text ) ) return null;
                var sdlSurface = SDL_ttf.TTF_RenderText_Shaded( _sdlFont, text, fg.ToSDLColor(), bg.ToSDLColor() );
                return sdlSurface == IntPtr.Zero ? null : Surface.INTERNAL_Surface_Wrap( _sdlRenderer, sdlSurface );
            }
            
            public Surface TextBlended( string text, Color c )
            {
                if( string.IsNullOrEmpty( text ) ) return null;
                var sdlSurface = SDL_ttf.TTF_RenderText_Blended( _sdlFont, text, c.ToSDLColor() );
                return sdlSurface == IntPtr.Zero ? null : Surface.INTERNAL_Surface_Wrap( _sdlRenderer, sdlSurface );
            }
            
            #endregion
            
        }
        
        #endregion
    }
}
