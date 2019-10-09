/*
 * SDLRenderer_SDLThread_EventDispatcher.cs
 *
 * The event dispatcher for the SDLThread has been separated from the rest of the threading code
 * just to keep the file from getting too bulky.  Like that section, don't mess with this unless
 * you know what's what with threads.
 * 
 * These delegates and functions handle callback delegates for the SDL_Window event queue.  Remember
 * that client callbacks will be run in the SDLThread and not the main thread.  All resources should
 * be guarded as needed for a multi-threaded environment.
 * 
 * User: 1000101
 * Date: 28/01/2018
 * Time: 3:24 AM
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
        
        #region Public API:  Client Delegate Prototypes
        
        public delegate void void_RendererOnly( SDLRenderer renderer );
        public delegate void void_RendererAndEvent( SDLRenderer renderer, SDL.SDL_Event e );
        
        #endregion
        
        #region Public API:  Client Callbacks
        
        // Actually used in INTERNAL_SDLThread_RenderScene() (see SDLRenderer_SDLThread.cs)
        public void_RendererOnly DrawScene;
        
        // SDL_Events the client can handle, these will be called in the SDLRenderer
        // thread and the client should handle it's own mechanisms for data protection.
        public void_RendererAndEvent KeyDown;
        public void_RendererAndEvent KeyUp;
        public void_RendererAndEvent MouseButtonDown;
        public void_RendererAndEvent MouseButtonUp;
        public void_RendererAndEvent MouseMove;
        public void_RendererAndEvent MouseWheel;
        public void_RendererAndEvent MouseEnter;
        public void_RendererAndEvent MouseExit;
        public void_RendererAndEvent WindowResized;
        public void_RendererAndEvent WindowSizeChanged;
        
        // The RendererReset event can happen at any time and executes in the SDLThread.  This event occurs
        // whenever the SDL_Window and/or SDL_Render are "reset" in any way.  Resources that are linked to
        // the SDLRenderer (SDLRenderer.Texture, etc) should be recreated on this event.  The event itself
        // occurs after the SDL_Window and/or SDL_Render are recreated (as applicable).
        public void_RendererOnly RendererReset;
        
        // For a stand-alone SDL_Window, we need an event handler for it to signal back that the user closed it.
        // Client code cannot explicitly [un]subscribe to this, the handler must be passed to the constructor.
        //void_RendererOnly WindowClosed;
        // This is now stored in InitParams
        
        #endregion
        
        #region Internal:  SDLRender Thread Event Dispatcher
        
        void INTERNAL_SDLThread_EventDispatcher()
        {
            #if DEBUG
            if( !IsReady ) return;
            #endif
            
            SDL.SDL_Event sdlEvent;
            
            // TODO:  Add other relevant events!
            
            while( ( SDL.SDL_PollEvent( out sdlEvent ) != 0 )&&( !_exitRequested ) )
            {
                //Console.WriteLine( string.Format( "INTERNAL_SDLThread_EventDispatcher : sdlEvent.type = 0x{0}", sdlEvent.type.ToString( "X" ) ) );
                switch( sdlEvent.type )
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                    {
                        // Nothing else matters after a quit event, just return
                        _exitRequested = true;
                        return;
                    }
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                    {
                        // Call user KeyDown handler
                        if( KeyDown != null )
                            KeyDown( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_KEYUP:
                    {
                        // Call user KeyUp handler
                        if( KeyUp != null )
                            KeyUp( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    {
                        // Call user MouseButtonDown handler
                        if( MouseButtonDown != null )
                            MouseButtonDown( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    {
                        // Call user MouseButtonUp handler
                        if( MouseButtonUp != null )
                            MouseButtonUp( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    {
                        // Call user MouseMove handler
                        if( MouseMove != null )
                            MouseMove( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                    {
                        // Call user MouseWheel handler
                        if( MouseWheel != null )
                            MouseWheel( this, sdlEvent );
                        break;
                    }
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    {
                        switch( sdlEvent.window.windowEvent )
                        {
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                            {
                                // Signal this thread to terminate the main loop
                                _exitRequested = true;
                                
                                // Asyncronously signal the main thread that this thread is terminating
                                if( _InitParams.WindowClosed != null )
                                    _InitParams.ParentForm.BeginInvoke( _InitParams.WindowClosed, new [] { this } );
                                
                                // Nothing else matters after a window close event, just return
                                return;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                            {
                                // Call user WindowSizeChanged handler
                                if( WindowSizeChanged != null )
                                    WindowSizeChanged( this, sdlEvent );
                                
                                // Update the internal window size
                                _windowSize = new Size( sdlEvent.window.data1, sdlEvent.window.data2 );
                                if( _InitParams.Anchored )
                                    _windowResetRequired = true;  // This will auto-trigger a renderer reset
                                else
                                    _rendererResetRequired = true;
                                //SDL.SDL_RenderPresent( _sdlRenderer );
                                break;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                            {
                                // Call user WindowResized handler
                                if( WindowResized != null )
                                    WindowResized( this, sdlEvent );
                                break;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                            {
                                // Window has been brought to the front
                                SDL.SDL_RenderPresent( _sdlRenderer );
                                break;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                            {
                                // Call user MouseEnter handler
                                if( MouseEnter != null )
                                    MouseEnter( this, sdlEvent );
                                break;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                            {
                                // Call user MouseExit handler
                                if( MouseExit != null )
                                    MouseExit( this, sdlEvent );
                                break;
                            }
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                            {
                                // Bring the containing control to the front
                                BringControlToFront(
                                    _InitParams.ParentControl != null
                                    ? RootParentControl( _InitParams.ParentControl )
                                    : _InitParams.ParentForm );
                                break;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            
        }
        
        void BringControlToFront( System.Windows.Forms.Control control )
        {
            // disable once ConvertToLambdaExpression
            control.BeginInvoke( (Action)delegate() { control.BringToFront(); }, null );
        }
        
        System.Windows.Forms.Control RootParentControl( System.Windows.Forms.Control control )
        {
            return control == null
                ? null
                : control.Parent != null
                    ? RootParentControl( control.Parent )
                    : control;
        }
        
        #endregion
        
    }
}
