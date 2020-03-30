/*
 * SDLRenderer_SDLThread.cs
 *
 * Everything thread related to the class is handled here.  Don't mess with this part of the class
 * unless you fully understand the magic of multi-threading.
 *
 * User: 1000101
 * Date: 28/01/2018
 * Time: 3:00 AM
 * 
 */
using System;
using System.Diagnostics;
using System.Threading;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    public partial class SDLRenderer : IDisposable
    {
        
        #region Internal:  Thread state enum
        
        enum SDLThreadState
        {
            Inactive = 0,
            Starting = 1,
            Running = 2,
            Stopping = 3,
            Paused = 4,
            Error = -1
        }
        
        #endregion
        
        #region Internal:  Thread level state machine variables
        
        Thread _sdlThread;
        SDLThreadState _threadState;
        bool _exitRequested;
        bool _pauseThread;
        bool _windowResetRequired;
        bool _rendererResetRequired;
        
        Stopwatch _threadTimer;
        long _drawTicks;
        long _eventTicks;
        long _baseFrameDelay;
        
        ImageTypes _windowSaveFormat;
        string _windowSaveFilename;
        bool _windowSaveRequested;
        bool _windowSaved;
            
        #endregion
        
        #region Internal:  Performance feedback variables
        
        int _actualFPS;
        int _potentialFPS; // Take this with a grain of salt
        long _averageFrameTime;
        
        #endregion
        
        #region Public API:  SDLRenderer Thread Update frequency
        
        public int DrawsPerSecond
        {
            get
            {
                return _drawsPS;
            }
            set
            {
                if( value <= 0.0d ) return;
                if( value > MAX_UPDATES_PER_SECOND ) return;
                _drawsPS = value;
                INTERNAL_UpdateState_ThreadIntervals();
            }
        }
        
        public int EventsPerSecond
        {
            get
            {
                return _eventsPS;
            }
            set
            {
                if( value <= 0.0d ) return;
                if( value > MAX_UPDATES_PER_SECOND ) return;
                _eventsPS = value;
                INTERNAL_UpdateState_ThreadIntervals();
            }
        }
        
        #endregion
        
        #region Public API:  Performance Feedback Properties
        
        public int ActualFPS
        {
            get
            {
                return _actualFPS;
            }
        }
        
        public int PotentialFPS
        {
            get
            {
                return _potentialFPS;
            }
        }
        
        public long AverageFrameTimeTicks
        {
            get
            {
                return _averageFrameTime;
            }
        }
        
        public long AverageFrameTimeMS
        {
            get
            {
                return ( _averageFrameTime / TimeSpan.TicksPerMillisecond );
            }
        }
        
        #endregion
        
        #region Internal:  Thread State
        
        // These are the only properties/functions that should be called outside of the SDLThread
        
        bool INTERNAL_SDLThread_Active
        {
            get
            {
                return _threadState > SDLThreadState.Inactive;
            }
        }
        
        bool INTERNAL_SDLThread_Starting
        {
            get
            {
                return _threadState == SDLThreadState.Starting;
            }
        }
        
        bool INTERNAL_SDLThread_Running
        {
            get
            {
                return _threadState == SDLThreadState.Running;
            }
        }
        
        bool INTERNAL_SDLThread_Paused
        {
            get
            {
                return _threadState == SDLThreadState.Paused;
            }
        }
        
        bool INTERNAL_SDLThread_Stopping
        {
            get
            {
                return _threadState == SDLThreadState.Stopping;
            }
        }
        
        #endregion
        
        #region Public API:  SDLThread Pause/Resume
        
        /// <summary>
        /// Tells the SDLThread to pause and will not return until it does or it can't.
        ///
        /// DO NOT CALL THIS FROM THE SDLThread!
        /// </summary>
        public bool SDLThread_Pause()
        {
            if( !INTERNAL_SDLThread_Running ) return false;
            
            _pauseThread = true;
            while( !INTERNAL_SDLThread_Paused )
                Thread.Sleep( 0 );
            
            return true;
        }
        
        /// <summary>
        /// Tells the SDLThread to resume and will not return until it does or it can't.
        ///
        /// DO NOT CALL THIS FROM THE SDLThread!
        /// </summary>
        public bool SDLThread_Resume()
        {
            if( !INTERNAL_SDLThread_Paused ) return false;
            
            _pauseThread = false;
            while( !INTERNAL_SDLThread_Running )
                Thread.Sleep( 0 );
            
            return true;
        }
        
        #endregion
        
        #region Internal:  WaitFor[TypeValue]
        
        /// <summary>
        /// Tells the current thread to wait for the SDLThread to set a specific internal variable to a specific value.
        ///
        /// DO NOT CALL THIS FROM THE SDLThread!
        /// </summary>
        void INTERNAL_SDLThread_WaitForBool( ref bool toWaitFor, bool initialValue, bool waitValue, int releaseMS )
        {
            toWaitFor = initialValue;
            while( toWaitFor != waitValue )
                Thread.Sleep( releaseMS );
        }
        
        #endregion
        
        #region Internal:  SDLRenderer Thread Main Methods
        
        // All code here should be running in it's own thread created in INTERNAL_Init_SDLThread()
        // and should never be called outside of the thread itself.
        
        void THREAD_INTERNAL_SDL_Main()
        {
            WorkerThreadPool.StartMethodBase = System.Reflection.MethodInfo.GetCurrentMethod();
            WorkerThreadPool.SetName( WorkerThreadPool.StartMethodBaseNameFriendly( true ) );

            DebugLog.Open();
            
            //Console.WriteLine( "INTERNAL_SDLThread_Main()" );
            
            _threadState = SDLThreadState.Starting;
            
            // Create the SDL_Window
            var windowOK = INTERNAL_SDLThread_InitWindow();
            if( !windowOK )
            {
                INTERNAL_SDLThread_Cleanup( SDLThreadState.Error );
                return;
            }
            
            // Create the SDL_Renderer
            var rendererOK = INTERNAL_SDLThread_InitRenderer();
            if( !rendererOK )
            {
                INTERNAL_SDLThread_Cleanup( SDLThreadState.Error );
                return;
            }
            
            // Translate the state machine into something meaningful to SDL
            INTERNAL_UpdateState_FunctionPointers();
            INTERNAL_UpdateState_CursorVisibility();
            INTERNAL_UpdateState_ThreadIntervals();
            
            // Now do the main thread loop
            INTERNAL_SDLThread_MainLoop();
            
            // Clean up after the thread
            INTERNAL_SDLThread_Cleanup( SDLThreadState.Inactive );
            
            DebugLog.Close();
            
        }
        
        void INTERNAL_SDLThread_MainLoop()
        {
            DebugLog.OpenIndentLevel();
            //Console.WriteCaller();
            
            TimeSpan loopTime = TimeSpan.FromTicks( 0 );
            long loopStartTick = 0;
            long loopDelayTicks = 0;
            long loopEndTick = 0;
            TimeSpan loopSleepTime;
            int loopSleepMS = 0;
            long lastDrawTick = 0;
            long lastEventTick = 0;
            long drawTickDelta = 0;
            long eventTickDelta = 0;
            
            long lastFPSCount = 0;
            long fpsTicks = 0;
            long frameStart = 0;
            long frameEnd = 0;
            long frameTime = 0;
            
            // Thread timing
            _threadTimer = new Stopwatch();
            
            // Mark the thread as running
            _threadState = SDLThreadState.Running;
            _threadTimer.Start();
            
            // Loop until we exit
            //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Enter" );
            while( !_exitRequested )
            {
                //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Top" );
                
                if( _exitRequested )
                    _threadState = SDLThreadState.Stopping;
                
                // Get the loop start timestamp
                if( !_exitRequested )
                {
                    loopStartTick = _threadTimer.Elapsed.Ticks;
                    loopDelayTicks += loopTime.Ticks;
                }
                
                // Pause the thread at the top of the loop
                if( ( !_exitRequested )&&( _pauseThread ) )
                {
                    // Pause the thread until _pauseThread returns to false.  "Pausing" in
                    // this case is sleeping for 1ms in a loop. Ideally we would sleep for
                    // less than 1ms though, but 0 will make us return as soon as we can
                    // and use a bunch of CPU which we don't want.  1ms isn't even the true
                    // amount of time we'll sleep for, but 1 is the lowest positive value
                    // and will translate into the actual platform minimum sleep time
                    // (typically 15ms on Windows).
                    _threadState = SDLThreadState.Paused;
                    
                    //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Pause" );
                    while( _pauseThread )
                        Thread.Sleep( 1 );
                    //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Resume" );
                    
                    _threadState = SDLThreadState.Running;
                }
                
                // Save the last frame drawn to an image file
                if( ( !_exitRequested )&&( _windowSaveRequested ) )
                    INTERNAL_SDLThread_SaveWindowToFile();
                
                // Handle any invokes that have been queued
                if( ( !_exitRequested )&&( !_invokeQueue.NullOrEmpty() ) )
                    INTERNAL_SDLThread_InvokeQueue_Dispatcher();
                
                // Window needs to be recreated, do that before anything else
                if( ( !_exitRequested )&&( _windowResetRequired ) )
                    _exitRequested = !INTERNAL_SDLThread_ResetWindow();
                
                // Renderer needs to be recreated, do that before anything else
                if( ( !_exitRequested )&&( _rendererResetRequired ) )
                    _exitRequested = !INTERNAL_SDLThread_ResetRenderer();
                
                // Render the scene and handle events if we haven't been asked to terminate
                if( !_exitRequested )
                {
                    if( loopDelayTicks >= _baseFrameDelay )
                    {
                        loopDelayTicks -= _baseFrameDelay;
                        drawTickDelta = loopStartTick - lastDrawTick;
                        eventTickDelta = loopStartTick - lastEventTick;
                        
                        if( drawTickDelta >= _drawTicks )
                        {
                            // Time to render the scene
                            lastFPSCount++;
                            drawTickDelta -= _drawTicks;
                            lastDrawTick = loopStartTick;
                            frameStart = _threadTimer.Elapsed.Ticks;
                            INTERNAL_SDLThread_RenderScene();
                            frameEnd = _threadTimer.Elapsed.Ticks;
                            frameTime += ( frameEnd - frameStart );
                        }
                        
                        if( eventTickDelta >= _eventTicks )
                        {
                            // Time to check and handle events
                            eventTickDelta -= _eventTicks;
                            lastEventTick = loopStartTick;
                            INTERNAL_SDLThread_EventDispatcher();
                        }
                    }
                }
                
                // If a client event handler requested an exit, don't worry about timings
                if( !_exitRequested )
                {
                    // Sleep until the next expected update
                    loopSleepTime = TimeSpan.FromTicks( _baseFrameDelay - loopDelayTicks );
                    loopSleepMS = (int)loopSleepTime.TotalMilliseconds;
                    if( loopSleepMS >= 15 )
                        Thread.Sleep( loopSleepTime); //Thread.Sleep( loopSleepMS );
                    
                    // End of loop, get time elapsed for this loop
                    loopEndTick = _threadTimer.Elapsed.Ticks;
                    loopTime = TimeSpan.FromTicks( loopEndTick - loopStartTick );
                    
                    // Performance feedback
                    fpsTicks += loopTime.Ticks;
                    if( ( fpsTicks >= TimeSpan.TicksPerSecond )&&( lastFPSCount > 0 ) )
                    {
                        _actualFPS = (int)( ( lastFPSCount * TimeSpan.TicksPerSecond ) / fpsTicks );
                        _potentialFPS = (int)( ( lastFPSCount * TimeSpan.TicksPerSecond ) / frameTime );
                        _averageFrameTime = frameTime / lastFPSCount;
                        lastFPSCount = 0;
                        fpsTicks = 0;
                        frameTime = 0;
                    }
                }
                //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Bottom" );
            }
            //Console.WriteLine( "INTERNAL_SDLThread_MainLoop() :: Loop_Exit" );
            
            // Mark the thread as no longer running
            _threadTimer.Stop();
            _actualFPS = 0;
            _potentialFPS = 0;
            _averageFrameTime = 0;
            
            DebugLog.CloseIndentLevel();
        }
        
        void INTERNAL_SDLThread_RenderScene()
        {
            //Console.WriteLine( "INTERNAL_SDLThread_RenderScene()" );
            
            #if DEBUG
            if( !IsReady ) return;
            #endif
            
            DelFunc_ClearScene();
            
            if( DrawScene != null )
            {
                //Console.WriteLine( "INTERNAL_SDLThread_RenderScene()" );
                DrawScene( this );
            }
            SDL.SDL_RenderPresent( _sdlRenderer );
        }
        
        #endregion
        
        #region Internal:  SDLThread Save SDL_Window to file (ie: png, etc)
        
        void INTERNAL_SDLThread_SaveWindowToFile()
        {
            _windowSaveRequested = false;
            _rendererResetRequired = true;
            
            var sdlSurface = SDL.SDL_GetWindowSurface( _sdlWindow );
            _windowSaved = INTERNAL_Save_SDLSurface( sdlSurface, _windowSaveFormat, _windowSaveFilename );
            
        }
        
        #endregion
        
        #region Internal:  SDLThread Init/Denit/Reset
        
        void INTERNAL_SDLThread_Cleanup( SDLThreadState newState )
        {
            Console.WriteLine( "INTERNAL_SDLThread_Cleanup()" );
            
            // Dispose of the renderer, window, etc
            INTERNAL_SDLThread_ReleaseWindowAndRenderer();
            
            _sdlThread = null;
            _threadState = newState;
        }
        
        void INTERNAL_SDLThread_ReleaseWindowAndRenderer()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ReleaseWindowAndRenderer()" );
            
            INTERNAL_SDLThread_ReleaseRenderer();
            INTERNAL_SDLThread_ReleaseWindow();
        }
        
        void INTERNAL_SDLThread_ReleaseWindow()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ReleaseWindow()" );
            if( _sdlWindow != IntPtr.Zero )
                SDL.SDL_DestroyWindow( _sdlWindow );
            _sdlWindow = IntPtr.Zero;
        }
        
        void INTERNAL_SDLThread_ReleaseRenderer()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ReleaseRenderer()" );
            if( _sdlRenderer != IntPtr.Zero )
                SDL.SDL_DestroyRenderer( _sdlRenderer );
            _sdlRenderer = IntPtr.Zero;
        }
        
        bool INTERNAL_SDLThread_ResetWindowAndRenderer()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ResetWindowAndRenderer()" );
            
            if( !INTERNAL_SDLThread_ResetWindow() )
                return false;
            if( !INTERNAL_SDLThread_ResetRenderer() )
                return false;
            
            // SDL_Window and SDL_Renderer are ready for use
            return true;
        }
        
        bool INTERNAL_SDLThread_ResetWindow()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ResetWindow()" );
            
            // Free the existing SDL_Window
            INTERNAL_SDLThread_ReleaseWindow();
            
            // Aquire new a SDL_Window and SDL_Renderer
            return INTERNAL_SDLThread_InitWindow();
        }
        
        bool INTERNAL_SDLThread_ResetRenderer()
        {
            Console.WriteLine( "INTERNAL_SDLThread_ResetRenderer()" );
            
            // Get the old state values
            var obm = this.BlendMode;
            var osc = this.ShowCursor;
            
            // Free the existing SDL_Renderer
            INTERNAL_SDLThread_ReleaseRenderer();
            
            // Aquire new a SDL_Renderer
            var ret = INTERNAL_SDLThread_InitRenderer();
            
            // SDL_Renderer created
            if( ret )
            {
                // Set the old state values
                this.BlendMode = obm;
                this.ShowCursor = osc;
                
                // Call any RendererReset handlers
                if( RendererReset != null )
                    RendererReset( this );
            }
            
            return ret;
        }
        
        bool INTERNAL_SDLThread_InitWindow()
        {
            Console.WriteLine( "INTERNAL_SDLThread_InitWindow()" );
            
            // Create the SDL window
            _sdlWindow = SDL.SDL_CreateWindow(
                _InitParams.WindowTitle, /* _windowTitle, */
                SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                _windowSize.Width, _windowSize.Height, /* _InitParams.WindowSize.Width, _InitParams.WindowSize.Height, */
                SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE |
                ( _InitParams.Anchored /* _anchored */ ?
                SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS |
                SDL.SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR
                : (SDL.SDL_WindowFlags)0 )
            );
            if( _sdlWindow == IntPtr.Zero )
                return false;
                //throw new Exception( string.Format( "Unable to create SDL_Window!\n\n{0}", SDL.SDL_GetError() ) );
            
            _sdlWindow_PixelFormat = SDL.SDL_GetWindowPixelFormat( _sdlWindow );
            if( _sdlWindow_PixelFormat == SDL.SDL_PIXELFORMAT_UNKNOWN )
                return false;
                //throw new Exception( string.Format( "Unable to obtain SDL_Window pixel format!\n\n{0}", SDL.SDL_GetError() ) );
            
            if( SDL.SDL_PixelFormatEnumToMasks(
                _sdlWindow_PixelFormat,
                out _sdlWindow_bpp,
                out _sdlWindow_Rmask,
                out _sdlWindow_Gmask,
                out _sdlWindow_Bmask,
                out _sdlWindow_Amask ) == SDL.SDL_bool.SDL_FALSE )
                return false;
                //throw new Exception( string.Format( "Unable to obtain SDL_Window pixel format bitmasks!\n\n{0}", SDL.SDL_GetError() ) );
            
            // Get the Win32 HWND from the SDL window
            var sysWMinfo = new SDL.SDL_SysWMinfo();
            SDL.SDL_GetWindowWMInfo( _sdlWindow, ref sysWMinfo );
            _sdlWindowHandle = sysWMinfo.info.win.window;
            
            if( _InitParams.Anchored ) //( _anchored )
            {
                // Time to anchor the window to the control...
                
                // Tell SDL we don't want a border...
                SDL.SDL_SetWindowBordered( _sdlWindow, SDL.SDL_bool.SDL_FALSE );
                
                // ...Aero doesn't always listen to SDL so force it through the Windows API
                var winStyle = (WinAPI.WindowStyleFlags)WinAPI.GetWindowLongPtr( _sdlWindowHandle, WinAPI.WindowLongIndex.GWL_STYLE );
                winStyle &= ~WinAPI.WindowStyleFlags.WS_BORDER;
                winStyle &= ~WinAPI.WindowStyleFlags.WS_SIZEBOX;
                winStyle &= ~WinAPI.WindowStyleFlags.WS_DLGFRAME;
                WinAPI.SetWindowLongPtr( _sdlWindowHandle, WinAPI.WindowLongIndex.GWL_STYLE, (uint)winStyle );
               
                // Move the SDL window to 0, 0
                WinAPI.SetWindowPos(
                    _sdlWindowHandle,
                    _InitParams.ParentFormHandle, /*_mainFormHandle,*/
                    0, 0,
                    0, 0,
                    WinAPI.WindowSWPFlags.SWP_NOSIZE | WinAPI.WindowSWPFlags.SWP_SHOWWINDOW
                );
                
                // Anchor the SDL_Window to the control
                WinAPI.SetParent( _sdlWindowHandle, _InitParams.TargetControlHandle );// _targetControlHandle );
                
                SDL.SDL_ShowWindow( _sdlWindow );
            }
            else
            {
                // Make the SDL_Window look like a tool window
                var winStyle = (WinAPI.WindowStyleFlags)WinAPI.GetWindowLongPtr( _sdlWindowHandle, WinAPI.WindowLongIndex.GWL_EXSTYLE );
                winStyle |= WinAPI.WindowStyleFlags.WS_EX_TOOLWINDOW;
                WinAPI.SetWindowLongPtr( _sdlWindowHandle, WinAPI.WindowLongIndex.GWL_EXSTYLE, (uint)winStyle );
                
                // ShowWindow to force all the changes and present the SDL_Window
                //WinAPI.ShowWindow( _sdlWindowHandle, WinAPI.ShowWindowFlags.SW_SHOWNORMAL );
                SDL.SDL_ShowWindow( _sdlWindow );
                
            }
            
            _windowResetRequired = false;
            _rendererResetRequired = true;
            return true;
        }
        
        bool INTERNAL_SDLThread_InitRenderer()
        {
            Console.WriteLine( "INTERNAL_SDLThread_InitRenderer()" );
            
            // Create the underlying renderer
            _sdlRenderer = SDL.SDL_CreateRenderer(
                _sdlWindow,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );
            if( _sdlRenderer == IntPtr.Zero )
                //return false;
                throw new Exception( string.Format( "Unable to create SDL_Renderer!\n\n{0}", SDL.SDL_GetError() ) );
            
            if( SDL.SDL_GetRendererInfo( _sdlRenderer, out _sdlRenderInfo ) != 0 )
                return false;
                //throw new Exception( string.Format( "Unable to obtain SDL_RendererInfo!\n\n{0}", SDL.SDL_GetError() ) );
            
            _rendererResetRequired = false;
            return true;
        }
        
        #endregion
        
    }
    
}
