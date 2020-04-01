/*
 * SDLRenderer_SDLThread_BeginInvoke.cs
 *
 * These delegates and functions handle executing delegates in the SDL thread.  Certain
 * functions cannot be performed outside the thread that SDL is running in, such as drawing
 * and rendering.  If a function like this needs to be performed outside of the draw/event
 * mechanism, you can use SDLRenderer.BeginInvoke() or SDLRenderer.Invoke() to run the code
 * in the SDL thread.
 * 
 * NOTE:  These delegates will not execute immediately, they are handled via an invoke queue
 * in the SDL thread and should not be used for time critical code.
 * 
 * User: 1000101
 * Date: 1/30/2018
 * Time: 11:59 AM
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using SDL2;

namespace SDL2ThinLayer
{
    public partial class SDLRenderer
    {
        
        #region Internal:  Begin/Invoke queue
        
        class Invoke_RendererOnly : IDisposable
        {
            public void_RendererOnly cbInvoke;
            
            // Used by Invoke()
            public SemaphoreSlim sync = null;
            
            public bool IsBlocking
            {
                get
                {
                    return sync != null;
                }
            }
            
            public void Wait()
            {
                // Was this a BeginInvoke?
                if( !IsBlocking ) return;
                
                // Wait for the SDL thread to handle the Invoke()
                sync.Wait();
                sync.Release();
                
            }
            
            public Invoke_RendererOnly( void_RendererOnly del, bool asyncBegin )
            {
                cbInvoke = del;
                
                if( !asyncBegin )
                {
                    // Create a semaphore with 1 slot and 0 available to handle Invoke()
                    // This way we already hold the lone semaphore slot before we push
                    // the event to the queue.  When the event is handled it will release
                    // the semaphore count and allow this thread to resume execution.
                    sync = new SemaphoreSlim( 0, 1 );
                }
            }
            
            // Protect against "double-free" errors caused by combinations of explicit disposal[s] and GC disposal
            bool _disposed = false;
            
            ~Invoke_RendererOnly()
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
                // This is no longer a valid state
                _disposed = true;
                
                cbInvoke = null;
                
                if( sync != null )
                    sync.Dispose();
                sync = null;
                
            }
            
        }
        
        List<Invoke_RendererOnly>   _invokeQueue;
        
        #endregion
        
        #region Public API:  Invoke() and BeginInvoke()
        
        public void Invoke( void_RendererOnly del )
        {
            INTERNAL_SDLThread_InvokeQueue_PrepareInvoke( del, false );
        }
        
        public void BeginInvoke( void_RendererOnly del )
        {
            INTERNAL_SDLThread_InvokeQueue_PrepareInvoke( del, true );
        }
        
        #endregion
        
        #region Internal:  SDLRenderer Thread Begin/Invoke
        
        #region Queue Management
        
        void INTERNAL_SDLThread_InvokeQueue_Add( Invoke_RendererOnly ueInfo )
        {
            _invokeQueue.Add( ueInfo );
        }
        
        Invoke_RendererOnly INTERNAL_SDLThread_InvokeQueue_FetchNext()
        {
            if( _invokeQueue.Count < 1 ) return null;
            var ueInfo = _invokeQueue[ 0 ];
            _invokeQueue.RemoveAt( 0 );
            return ueInfo;
        }
        
        #endregion
        
        #region Add new invoke bucket to queue
        
        void INTERNAL_SDLThread_InvokeQueue_PrepareInvoke( void_RendererOnly del, bool asyncBegin )
        {
            // Invoke will cause a queued event in the SDL thread.  Normal Invoke/BeginInvoke
            // cannot be used as the main loop for the SDL thread is always running.
            // Due to this being handled through a queue system the call can be delayed.
            
            // Begin/Invoke struct
            var ueInfo = new Invoke_RendererOnly( del, asyncBegin );
            
            // Add this event to the queue
            INTERNAL_SDLThread_InvokeQueue_Add( ueInfo );
            
            // Wait on Invoke
            ueInfo.Wait();
            
        }
        
        #endregion
        
        #region Begin/Invoke Delegate Callback
        
        void INTERNAL_SDLThread_InvokeQueue_HandleInvoke( Invoke_RendererOnly ueInfo )
        {
            // Invoke the delegate
            if( ueInfo.cbInvoke != null )
                ueInfo.cbInvoke( this );
            
            if( ueInfo.IsBlocking )
            {
                // Signal the invoking thread that the delegate has been run.
                // The invoking thread will handle releasing the unmanaged resources.
                ueInfo.sync.Release();
            }
            else
            {
                // BeginInvoke() means we need to free the unmanaged resources
                ueInfo.Dispose();
                ueInfo = null;
            }
        }
        
        void INTERNAL_SDLThread_InvokeQueue_Dispatcher()
        {
            #if DEBUG
            if( !IsReady ) return;
            #endif
            
            DebugLog.OpenIndentLevel();
            
            while( _invokeQueue.Count > 0 )
            {
                var ueInfo = INTERNAL_SDLThread_InvokeQueue_FetchNext();
                if( ueInfo != null )
                {
                    DebugLog.WriteLine( "Invoke Delegate :: " + ueInfo.TypeFullName() );
                    INTERNAL_SDLThread_InvokeQueue_HandleInvoke( ueInfo );
                }
            }

            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #endregion
        
    }
}
