/*
 * SDLRenderer_Primitives.cs
 *
 * Public and internal methods for drawing primitives.
 *
 * User: 1000101
 * Date: 28/01/2018
 * Time: 3:06 AM
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
        
        #region Internal:  Rendering Delegate Prototypes
        
        delegate void INTERNAL_Delegate_ClearScene();
        
        delegate void INTERNAL_Delegate_DrawPoint( int x, int y, Color c );
        delegate void INTERNAL_Delegate_DrawPoints( SDL.SDL_Point[] points, int count, Color c );
        
        delegate void INTERNAL_Delegate_DrawLine( int x1, int y1, int x2, int y2, Color c );
        delegate void INTERNAL_Delegate_DrawLines( SDL.SDL_Point[] points, int count, Color c );
        
        delegate void INTERNAL_Delegate_DrawRect( SDL.SDL_Rect rect, Color c );
        delegate void INTERNAL_Delegate_DrawRects( SDL.SDL_Rect[] rects, int count, Color c );
        delegate void INTERNAL_Delegate_DrawFilledRect( SDL.SDL_Rect rect, Color c );
        delegate void INTERNAL_Delegate_DrawFilledRects( SDL.SDL_Rect[] rects, int count, Color c );
        
        delegate void INTERNAL_Delegate_DrawCircle( int x, int y, int r, Color c );
        
        #endregion
        
        #region Internal:  Rendering Function Pointers...I mean Method Delegates...
        
        INTERNAL_Delegate_ClearScene DelFunc_ClearScene;
        
        INTERNAL_Delegate_DrawPoint DelFunc_DrawPoint;
        INTERNAL_Delegate_DrawPoints DelFunc_DrawPoints;
        
        INTERNAL_Delegate_DrawLine DelFunc_DrawLine;
        INTERNAL_Delegate_DrawLines DelFunc_DrawLines;
        
        INTERNAL_Delegate_DrawRect DelFunc_DrawRect;
        INTERNAL_Delegate_DrawRects DelFunc_DrawRects;
        INTERNAL_Delegate_DrawRect DelFunc_DrawFilledRect;
        INTERNAL_Delegate_DrawRects DelFunc_DrawFilledRects;
        
        INTERNAL_Delegate_DrawCircle DelFunc_DrawCircle;
        INTERNAL_Delegate_DrawCircle DelFunc_DrawFilledCircle;
        
        #endregion
        
        #region Public API:  Rendering Primitives
        
        #region Points
        
        public void DrawPoint( SDL.SDL_Point p, Color c )
        {
            DelFunc_DrawPoint( p.x, p.y, c );
        }
        
        public void DrawPoint( int x, int y, Color c )
        {
            DelFunc_DrawPoint( x, y, c );
        }
        
        public void DrawPoints( SDL.SDL_Point[] points, int count, Color c )
        {
            DelFunc_DrawPoints( points, count, c );
        }
        
        #endregion
        
        #region Lines
        
        public void DrawLine( SDL.SDL_Point p1, SDL.SDL_Point p2, Color c )
        {
            DelFunc_DrawLine( p1.x, p1.y, p2.x, p2.y, c );
        }
        
        public void DrawLine( int x1, int y1, int x2, int y2, Color c )
        {
            DelFunc_DrawLine( x1, y1, x2, y2, c );
        }
        
        public void DrawLines( SDL.SDL_Point[] points, int count, Color c )
        {
            DelFunc_DrawLines( points, count, c );
        }
        
        #endregion
        
        #region Rects
        
        public void DrawRect( int x1, int y1, int x2, int y2, Color c )
        {
            var rect = new SDL.SDL_Rect( x1, y1, x2 - x1, y2 - y1 );
            DelFunc_DrawRect( rect, c );
        }
        
        public void DrawRect( SDL.SDL_Rect rect, Color c )
        {
            DelFunc_DrawRect( rect, c );
        }
        
        public void DrawRects( SDL.SDL_Rect[] rects, int count, Color c )
        {
            DelFunc_DrawRects( rects, count, c );
        }
        
        public void DrawFilledRect( int x1, int y1, int x2, int y2, Color c )
        {
            var rect = new SDL.SDL_Rect( x1, y1, x2 - x1, y2 - y1 );
            DelFunc_DrawFilledRect( rect, c );
        }
        
        public void DrawFilledRect( SDL.SDL_Rect rect, Color c )
        {
            DelFunc_DrawFilledRect( rect, c );
        }
        
        public void DrawFilledRects( SDL.SDL_Rect[] rects, int count, Color c )
        {
            DelFunc_DrawFilledRects( rects, count, c );
        }
        
        #endregion
        
        #region Circles
        
        public void DrawCircle( int x, int y, int r, Color c )
        {
            DelFunc_DrawCircle( x, y, r, c );
        }
        
        public void DrawCircle( SDL.SDL_Point p, int r, Color c )
        {
            DelFunc_DrawCircle( p.x, p.y, r, c );
        }
        
        public void DrawFilledCircle( int x, int y, int r, Color c )
        {
            DelFunc_DrawFilledCircle( x, y, r, c );
        }
        
        public void DrawFilledCircle( SDL.SDL_Point p, int r, Color c )
        {
            DelFunc_DrawFilledCircle( p.x, p.y, r, c );
        }
        
        #endregion
        
        #endregion
        
        #region Internal:  Rendering Primitives
        
        // These all come in two flavours:
        // A fast version and a version that preserves the SDL state machine
        // (Which at this point is basically the render draw color)
        
        #region ClearScene
        
        void INTERNAL_DelFunc_ClearScene_Fast()
        {
            INTERNAL_RenderColor = _clearColor;
            SDL.SDL_RenderClear( _sdlRenderer );
        }
        
        void INTERNAL_DelFunc_ClearScene()
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_ClearScene_Fast();
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region Points
        
        #region DrawPoint
        
        void INTERNAL_DelFunc_DrawPoint_Fast( int x, int y, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawPoint( _sdlRenderer, x, y );
        }
        
        void INTERNAL_DelFunc_DrawPoint( int x, int y, Color c)
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawPoint_Fast( x, y, c);
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawPoints
        
        void INTERNAL_DelFunc_DrawPoints_Fast( SDL.SDL_Point[] points, int count, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawPoints( _sdlRenderer, points, count );
        }
        
        void INTERNAL_DelFunc_DrawPoints( SDL.SDL_Point[] points, int count, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawPoints_Fast( points, count, c);
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #endregion
        
        #region Lines
        
        #region DrawLine
        
        void INTERNAL_DelFunc_DrawLine_Fast( int x1, int y1, int x2, int y2, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawLine( _sdlRenderer, x1, y1, x2, y2 );
        }
        
        void INTERNAL_DelFunc_DrawLine( int x1, int y1, int x2, int y2, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawLine_Fast( x1, y1, x2, y2, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawLines
        
        void INTERNAL_DelFunc_DrawLines_Fast( SDL.SDL_Point[] points, int count, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawLines( _sdlRenderer, points, count );
        }
        
        void INTERNAL_DelFunc_DrawLines( SDL.SDL_Point[] points, int count, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawLines_Fast( points, count, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #endregion
        
        #region Rects
        
        #region DrawRect
        
        void INTERNAL_DelFunc_DrawRect_Fast( SDL.SDL_Rect rect, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawRect( _sdlRenderer, ref rect );
        }
        
        void INTERNAL_DelFunc_DrawRect( SDL.SDL_Rect rect, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawRect_Fast( rect, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawRects
        
        void INTERNAL_DelFunc_DrawRects_Fast( SDL.SDL_Rect[] rects, int count, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderDrawRects( _sdlRenderer, rects, count );
        }
        
        void INTERNAL_DelFunc_DrawRects( SDL.SDL_Rect[] rects, int count, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawRects_Fast( rects, count, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawFilledRect
        
        void INTERNAL_DelFunc_DrawFilledRect_Fast( SDL.SDL_Rect rect, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderFillRect( _sdlRenderer, ref rect );
        }
        
        void INTERNAL_DelFunc_DrawFilledRect( SDL.SDL_Rect rect, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawFilledRect_Fast( rect, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawFilledRects
        
        void INTERNAL_DelFunc_DrawFilledRects_Fast( SDL.SDL_Rect[] rects, int count, Color c )
        {
            INTERNAL_RenderColor = c;
            SDL.SDL_RenderFillRects( _sdlRenderer, rects, count );
        }
        
        void INTERNAL_DelFunc_DrawFilledRects( SDL.SDL_Rect[] rects, int count, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawFilledRects_Fast( rects, count, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #endregion
        
        #region Circles
        
        interface INTERNAL_CircleAlgo_UserData
        {
            void Add( int x, int y, int offsetX, int offsetY );
        }
        
        static void INTERNAL_CircleAlgo_Compute( INTERNAL_CircleAlgo_UserData userData, int x, int y, int r )
        {
            // Compute a circle by using the midpoint circle algorithm
            // This algo only computes the first 45 degrees of a circle and mirrors all other points
            
            // Calculcate the points using circle midpoint
            int offsetX = r - 1;
            int offsetY = 0;
            int deltaX = 1;
            int deltaY = 1;
            int r2 = r << 1;
            int correction = deltaX - r2;
            
            while( offsetX >= offsetY )
            {
                // Add octet to the working set
                userData.Add( x, y, offsetX, offsetY );
                
                if( correction <= 0 )
                {
                    offsetY++;
                    correction += deltaY;
                    deltaY += 2;
                }
                
                if( correction > 0 )
                {
                    offsetX--;
                    deltaX += 2;
                    correction += deltaX - r2;
                }
            }
        }
        
        #region DrawCircle
        
        class INTERNAL_CircleAlgo_EdgeData : INTERNAL_CircleAlgo_UserData
        {
            public int Count;
            public SDL.SDL_Point[] Points;
            
            public INTERNAL_CircleAlgo_EdgeData( int expected )
            {
                Count = 0;
                Points = new SDL.SDL_Point[ expected ];
            }
            
            public void Add( int x, int y, int offsetX, int offsetY )
            {
                var ymx = y - offsetX;
                var ymy = y - offsetY;
                var ypy = y + offsetY;
                var ypx = y + offsetX;
                
                var xmx = x - offsetX;
                var xmy = x - offsetY;
                var xpy = x + offsetY;
                var xpx = x + offsetX;
                
                Points[ Count + 0 ] = new SDL.SDL_Point( xmx, ypy );
                Points[ Count + 1 ] = new SDL.SDL_Point( xmx, ymy );
                
                Points[ Count + 2 ] = new SDL.SDL_Point( xmy, ypx );
                Points[ Count + 3 ] = new SDL.SDL_Point( xmy, ymx );
                
                Points[ Count + 4 ] = new SDL.SDL_Point( xpx, ypy );
                Points[ Count + 5 ] = new SDL.SDL_Point( xpx, ymy );
                
                Points[ Count + 6 ] = new SDL.SDL_Point( xpy, ypx );
                Points[ Count + 7 ] = new SDL.SDL_Point( xpy, ymx );
                
                Count += 8;
            }
        }
        
        void INTERNAL_DelFunc_DrawCircle_Fast( int x, int y, int r, Color c )
        {
            // Expected number of points on circumfrence
            // Use standard 2*PI*r and then round up to the next '8'
            var expected = ( 1 + ( (int)Math.Round( 2d * Math.PI * (double)r ) >> 3 ) ) << 3;
            
            // Setup edge data for this circle
            var ed = new INTERNAL_CircleAlgo_EdgeData( expected );
            
            // Compute the edge data
            INTERNAL_CircleAlgo_Compute( ed, x, y, r );
            
            // Render the edges
            INTERNAL_DelFunc_DrawPoints_Fast( ed.Points, ed.Count, c );
        }
        
        void INTERNAL_DelFunc_DrawCircle( int x, int y, int r, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawCircle_Fast( x, y, r, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #region DrawFilledCircle
        
        class INTERNAL_CircleAlgo_FillData : INTERNAL_CircleAlgo_UserData
        {
            int _topY;
            int _bottomY;
            bool[] _set;
            int[] _x1;
            int[] _x2;
            
            public INTERNAL_CircleAlgo_FillData( int expected, int topY )
            {
                _topY = topY;
                _bottomY = _topY + expected - 1;
                _set = new bool[ expected ];
                _x1 = new int[ expected ];
                _x2 = new int[ expected ];
            }
            
            public void Set( int scanline, int x1, int x2 )
            {
                if( scanline < _topY ) return;
                if( scanline > _bottomY ) return;
                scanline -= _topY;
                if( !_set[ scanline ] )
                {
                    _x1[ scanline ] = x1;
                    _x2[ scanline ] = x2;
                    _set[ scanline ] = true;
                    return;
                }
                if( x1 < _x1[ scanline ] ) _x1[ scanline ] = x1;
                if( x2 > _x2[ scanline ] ) _x2[ scanline ] = x2;
            }
            
            public bool Get( int scanline, out int x1, out int x2 )
            {
                if(
                    ( scanline < _topY )||
                    ( scanline > _bottomY )
                )
                {
                    x1 = int.MinValue;
                    x2 = int.MinValue;
                    return false;
                }
                scanline -= _topY;
                if( !_set[ scanline ] )
                {
                    x1 = int.MinValue;
                    x2 = int.MinValue;
                    return false;
                }
                x1 = _x1[ scanline ];
                x2 = _x2[ scanline ];
                return true;
            }
            
            public void Add( int x, int y, int offsetX, int offsetY )
            {
                var ymx = y - offsetX;
                var ymy = y - offsetY;
                var ypy = y + offsetY;
                var ypx = y + offsetX;
                
                var xmx = x - offsetX;
                var xmy = x - offsetY;
                var xpy = x + offsetY;
                var xpx = x + offsetX;
                
                Set( ymy, xmx, xpx );
                Set( ymx, xmy, xpy );
                Set( ypy, xmx, xpx );
                Set( ypx, xmy, xpy );
            }
        }
        
        void INTERNAL_DelFunc_DrawFilledCircle_Fast( int x, int y, int r, Color c )
        {
            // Expected number of scanlines of the circle
            var expected = 1 + r * 2;
            
            // Setup fill data for this circle
            var fd = new INTERNAL_CircleAlgo_FillData( expected, y - r );
            
            // Compute the edge data
            INTERNAL_CircleAlgo_Compute( fd, x, y, r );
            
            // Render the scanlines
            INTERNAL_RenderColor = c;
            int x1, x2;
            for( int scanline = y - r; scanline <= y + r; scanline++ )
            {
                if( fd.Get( scanline, out x1, out x2 ) )
                    SDL.SDL_RenderDrawLine( _sdlRenderer,
                                           x1, scanline,
                                           x2, scanline );
            }
            
        }
        
        void INTERNAL_DelFunc_DrawFilledCircle( int x, int y, int r, Color c )
        {
            var oldColor = INTERNAL_RenderColor;
            INTERNAL_DelFunc_DrawFilledCircle_Fast( x, y, r, c );
            INTERNAL_RenderColor = oldColor;
        }
        
        #endregion
        
        #endregion
        
        #endregion
        
    }
    
}
