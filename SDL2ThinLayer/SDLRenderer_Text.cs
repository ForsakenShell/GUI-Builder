/*
 * SDLRenderer_Text.cs
 *
 * Public and internal methods for drawing text.
 *
 * User: 1000101
 * Date: 05/02/2018
 * Time: 9:36 AM
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
        
        public void DrawText( SDL.SDL_Point p, Font font, string text, Color c, int style = SDL_ttf.TTF_STYLE_NORMAL )
        {
            DrawText( p.x, p.y, font, text, c, style );
        }
        
        public void DrawText( int x, int y, Font font, string text, Color c, int style = SDL_ttf.TTF_STYLE_NORMAL )
        {
            if( string.IsNullOrEmpty( text ) ) return;
            
            var oldStyle = font.Style;
            font.Style = style;
            
            Surface surface = font.TextBlended( text, c );
            
            font.Style = oldStyle;
            
            var rect = new SDL.SDL_Rect( x, y, surface.Width, surface.Height );
            Blit( rect, surface );
            
            surface.Dispose();
            surface = null;
        }
        
    }
}
