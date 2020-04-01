/*
 * SDLRenderer.cs
 *
 * This is the constructor, destructor and actual IDisposable interface.
 *
 * User: 1000101
 * Date: 19/12/2017
 * Time: 9:16 AM
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading;

using System.Drawing;
using System.Windows.Forms;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    
    public partial class SDLRenderer : IDisposable
    {
        
        #region Public API:  Control constants
        
        public const int MAX_UPDATES_PER_SECOND = 240; // 240 FPS, do we need faster than that???
        public const int DEFAULT_DRAWS_PER_SECOND = 60;
        public const int DEFAULT_EVENTS_PER_SECOND = 120;
        
        #endregion
        
        #region Public API:  SDLRenderer Constructors
        
        public class InitParams
        {
            
            #region Constructor Params
            
            public readonly Form                ParentForm                  = null;
            public readonly Control             TargetControl               = null;
            
            public readonly int                 WindowWidth                 = -1;
            public readonly int                 WindowHeight                = -1;
            
            public readonly string              WindowTitle                 = null;
            public readonly void_RendererOnly   WindowClosed                = null;
            public readonly int                 TargetDrawPerSecond         = -1;
            public readonly int                 TargetEventsPerSecond       = -1;
            public readonly bool                FastRender                  = false;
            public readonly bool                ShowCursorOverControl       = false;
            
            // In order to be valid, values must be the same length as hints (as they map 1:1)
            public readonly List<string>        SDLHints                    = null;
            public readonly List<string>        SDLHintValues               = null;
            
            #endregion
            
            #region Filled by constructor
            
            public readonly Control             ParentControl               = null;
            public readonly IntPtr              ParentFormHandle            = IntPtr.Zero;
            public readonly IntPtr              TargetControlHandle         = IntPtr.Zero;
            public readonly IntPtr              ParentControlHandle         = IntPtr.Zero;
            
            public readonly bool                Anchored                    = false;
            public readonly Size                WindowSize                  = new Size( -1, -1 );
            
            #endregion
            
            public InitParams(
                Form                parentForm,
                Control             targetControl,
                int                 drawsPerSecond          = DEFAULT_DRAWS_PER_SECOND,
                int                 eventsPerSecond         = DEFAULT_EVENTS_PER_SECOND,
                bool                fastRender              = true,
                bool                showCursorOverControl   = true,
                List<string>        sdlHints                = null,
                List<string>        sdlHintValues           = null
            ){
                if( parentForm == null )
                    throw new ArgumentException( "parentForm cannot be null!" );
                if( targetControl == null )
                    throw new ArgumentException( "targetControl cannot be null!" );
                if( drawsPerSecond < 1 )
                    throw new ArgumentException( "drawsPerSecond must be greater than 0!" );
                if( eventsPerSecond < 1 )
                    throw new ArgumentException( "eventsPerSecond must be greater than 0!" );
                int sdlHL  = sdlHints.NullOrEmpty()      ? 0 : sdlHints.Count;
                int sdlHVL = sdlHintValues.NullOrEmpty() ? 0 : sdlHintValues.Count;
                if( sdlHL != sdlHVL )
                    throw new ArgumentException( "sdlHints and sdlHintValues must be the same length (or both null)!" );
                
                ParentForm                  = parentForm;
                TargetControl               = targetControl;
                WindowWidth                 = targetControl.Size.Width;
                WindowHeight                = targetControl.Size.Height;
                //WindowTitle                 = windowTitle;
                //WindowClosed                = windowClosed;
                TargetDrawPerSecond         = drawsPerSecond;
                TargetEventsPerSecond       = eventsPerSecond;
                FastRender                  = fastRender;
                ShowCursorOverControl       = showCursorOverControl;
                if( sdlHL > 0 )
                {
                    SDLHints                = sdlHints.Clone();
                    SDLHintValues           = sdlHintValues.Clone();
                }
                
                ParentControl               = TargetControl;
                ParentFormHandle            = ParentForm.Handle;
                TargetControlHandle         = TargetControl.Handle;
                ParentControlHandle         = ParentControl.Handle;
                
                Anchored                    = true;
                WindowSize                  = new Size( WindowWidth, WindowHeight );
            }
            
            public InitParams(
                Form                parentForm,
                int                 windowWidth,
                int                 windowHeight,
                string              windowTitle,
                void_RendererOnly   windowClosed,
                int                 drawsPerSecond          = DEFAULT_DRAWS_PER_SECOND,
                int                 eventsPerSecond         = DEFAULT_EVENTS_PER_SECOND,
                bool                fastRender              = true,
                bool                showCursorOverControl   = true,
                List<string>        sdlHints                = null,
                List<string>        sdlHintValues           = null
            ){
                if( parentForm == null )
                    throw new ArgumentException( "parentForm cannot be null!" );
                if( windowClosed == null ) // windowClosed must be set for an unanchored window
                    throw new ArgumentException( "windowClosed cannot be null!" );
                if( string.IsNullOrEmpty( windowTitle ) )
                    throw new ArgumentException( "windowTitle cannot be null!" );
                if( windowWidth < 1 )
                    throw new ArgumentException( "windowWidth must be greater than 0!" );
                if( windowHeight < 1 )
                    throw new ArgumentException( "windowHeight must be greater than 0!" );
                if( drawsPerSecond < 1 )
                    throw new ArgumentException( "drawsPerSecond must be greater than 0!" );
                if( eventsPerSecond < 1 )
                    throw new ArgumentException( "eventsPerSecond must be greater than 0!" );
                int sdlHL  = sdlHints.NullOrEmpty()      ? 0 : sdlHints.Count;
                int sdlHVL = sdlHintValues.NullOrEmpty() ? 0 : sdlHintValues.Count;
                if( sdlHL != sdlHVL )
                    throw new ArgumentException( "sdlHints and sdlHintValues must be the same length (or both null)!" );
                
                ParentForm                  = parentForm;
                //TargetControl               = null;
                WindowWidth                 = windowWidth;
                WindowHeight                = windowHeight;
                WindowTitle                 = windowTitle;
                WindowClosed                = windowClosed;
                TargetDrawPerSecond         = drawsPerSecond;
                TargetEventsPerSecond       = eventsPerSecond;
                FastRender                  = fastRender;
                ShowCursorOverControl       = showCursorOverControl;
                if( sdlHL > 0 )
                {
                    SDLHints                = sdlHints.Clone();
                    SDLHintValues           = sdlHintValues.Clone();
                }
                
                ParentControl               = ParentForm;
                ParentFormHandle            = ParentForm.Handle;
                //TargetControlHandle         = IntPtr.Zero;
                ParentControlHandle         = ParentControl.Handle;
                
                //Anchored                    = false;
                WindowSize                  = new Size( WindowWidth, WindowHeight );
            }
            
        }
        
        public SDLRenderer( InitParams initParams )
        {
            _InitParams = initParams;
            INTERNAL_Init_Main();
        }
        
        #endregion
        
        #region Semi-Public API:  Destructor & IDispose
        
        // Protect against "double-free" errors caused by combinations of explicit disposal[s] and GC disposal
        bool _disposed = false;
        
        ~SDLRenderer()
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

            DebugLog.OpenIndentLevel( "disposing = " + disposing.ToString() );
            
            // Empty the invoke queue, anything that hasn't been
            // processed at this point will never be processed.
            if( _invokeQueue != null )
            {
                _invokeQueue.Clear();
                _invokeQueue = null;
            }
            
            // Destroy the SDL_Window
            DestroyWindow();
            
            // Shutdown SDL itself
            DebugLog.WriteLine( "SDL2.SDL.SDL_Quit()" );
            if( _sdlInitialized )
                SDL.SDL_Quit();
            
            // No longer a valid SDL state
            _sdlInitialized = false;
            
            _InitParams = null;
            
            // This is no longer a valid state
            _disposed = true;
            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Public API:  DestroyWindow()
        
        public void DestroyWindow()
        {
            if( !INTERNAL_SDLThread_Active ) return;

            DebugLog.OpenIndentLevel();
            
            // Disable all scenes
            //DrawScene = null;
            
            // Signal the event thread to stop
            _exitRequested = true;
            
            // And wait for it to stop
            DebugLog.WriteLine( "Waiting for SDLRenderer_Thread to quit..." );
            while( INTERNAL_SDLThread_Active )
                Thread.Sleep( 0 );

            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
    }
}
