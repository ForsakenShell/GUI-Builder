/*
 * WorkerThreadPool.cs
 *
 * Managed worker threads ensuring proper shutdown of workers.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public static class WorkerThreadPool
{
    
    public delegate void ThreadStart();
    public delegate void ThreadStartParams( object obj );
    public delegate void ThreadOnFinish();
    public delegate void ThreadOnFinishParams( object obj );
    
    public class WorkerThread : IDisposable
    {
        
        Thread _thread = null;
        
        internal object _Obj;
        
        internal bool _StopSignal;
        
        internal ThreadStart _Start;
        internal ThreadStartParams _StartParams;
        internal ThreadOnFinish _OnFinish;
        internal ThreadOnFinishParams _OnFinishParams;
        
        #region Allocation & Disposal
        
        #region Allocation
        
        public WorkerThread( ThreadStart start, ThreadOnFinish onFinish )
        {
            INTERNAL_cTor( null, start, onFinish, null, null );
        }
        
        public WorkerThread( object obj, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams )
        {
            INTERNAL_cTor( obj, null, null, startParams, onFinishParams );
        }
        
        void INTERNAL_cTor( object obj, ThreadStart start, ThreadOnFinish onFinish, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams )
        {
            _Obj = obj;
            
            _StopSignal = false;
            
            _Start = start;
            _OnFinish = onFinish;
            
            _StartParams = startParams;
            _OnFinishParams = onFinishParams;
            
            _thread = new Thread( INTERNAL_WorkerThread );
            _thread.Name =
                start       != null ? start      .Method.Name :
                startParams != null ? startParams.Method.Name :
                "Thread_ID_0x" + _thread.ManagedThreadId.ToString( "X8" );
                
        }
        
        #endregion
        
        #region Disposal
        
        protected bool Disposed = false;
        
        ~WorkerThread()
        {
            Dispose( true );
        }
        
        public void Dispose()
        {
            Dispose( true );
        }
        
        protected virtual void Dispose( bool disposing )
        {
            if( Disposed )
                return;
            
            if( IsRunning )
                Stop( true );
            
            _Obj = null;
            
            _Start = null;
            _StartParams = null;
            
            _OnFinish = null;
            _OnFinishParams = null;
            
            _thread = null;
            
            WorkerThreadPool.INTERNAL_RemoveWorker( this );
            Disposed = true;
        }
        
        #endregion
        
        #endregion
        
        #region Public API
        
        public bool StopSignal { get { return _StopSignal; } }
        
        public bool IsReady { get { return _thread != null; } }
        public bool IsRunning { get { return IsReady && _thread.IsAlive; } }
        
        public bool Start()
        {
            if( ( !IsReady )||( IsRunning )||( _StopSignal ) )
                return false;
            _thread.Start();
            return IsRunning;
        }
        
        public void Stop( bool sync )
        {
            _StopSignal = true;
            if( sync )
                while( IsRunning )
                    System.Threading.Thread.Sleep( 0 );
        }
        
        public bool Sync( string prefix, WorkerThreadPool.WorkerThread syncWith = null )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Sync()", prefix } );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            while( IsRunning )
            {
                // Release The Kraken!
                System.Threading.Thread.Sleep( 100 );
                
                if( ( syncWith != null )&&( syncWith.StopSignal ) )
                    break;
            }
            
            var syncMsg = prefix + " :: Thread finished in {0}";
            m.StopSyncTimer( syncMsg, tStart.Ticks );
            m.PopStatusMessage();
            
            DebugLog.CloseIndentLevel();
            
            return _StopSignal == false;
        }
        
        #endregion
        
        void INTERNAL_WorkerThread()
        {
            // Start the worker
            
            try
            {
                if( ( _StartParams != null )&&( _Obj != null ) )
                    _StartParams( _Obj );
                else if( _Start != null )
                    _Start();
                
                // OnFinish callback
                if( !_StopSignal )
                {
                    if( ( _OnFinishParams != null )&&( _Obj != null ) )
                        _OnFinishParams( _Obj );
                    else if( _OnFinish != null )
                        _OnFinish();
                }
            }
            catch( Exception e )
            {
                GodObject.Windows.SetEnableState( true );
                DebugLog.WriteError( "WorkerThreadPool", "INTERNAL_WorkerThread()", "An exception has occured while executing the thread\n" + e.ToString() );
            }
            
            if( DebugLog.Initialized )
                DebugLog.Close();
        }
        
    }
    
    static List<WorkerThread> _workers = null;
    
    #region Public API to create and manage workers
    
    public static WorkerThread CreateWorker( ThreadStart start, ThreadOnFinish onFinish )
    {
        if( _workers == null )
            _workers = new List<WorkerThread>();
        if( _workers == null )
            return null;
        var w = new WorkerThread( start, onFinish );
        if( ( w == null )||( !w.IsReady ) )
            return null;
        _workers.Add( w );
        return w;
    }
    
    public static WorkerThread CreateWorker( object obj, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams )
    {
        if( _workers == null )
            _workers = new List<WorkerThread>();
        if( _workers == null )
            return null;
        var w = new WorkerThread( obj, startParams, onFinishParams );
        if( ( w == null )||( !w.IsReady ) )
            return null;
        _workers.Add( w );
        return w;
    }
    
    public static void StopAllWorkers( bool sync, bool disposeOfWorkers )
    {
        DebugLog.WriteLine( "WorkerThreadPool.StopAllWorkers()" );
        
        if( sync )
            DebugLog.WriteLine( "WorkerThreadPool.StopAllWorkers() :: Syncing..." );
        
        INTERNAL_StopActiveWorkers( sync, disposeOfWorkers );
        
        if( sync )
            DebugLog.WriteLine( "WorkerThreadPool.StopAllWorkers() :: ...Stopped" );
    }
    
    public static bool AnyActiveWorkers { get { return INTERNAL_AnyWorkerActive(); } }
    
    #endregion
    
    #region Internal API to manage the pool of workers
    
    static void INTERNAL_RemoveWorker( WorkerThread workerThread )
    {
        if( ( _workers == null )||( workerThread == null ) )
            return;
        _workers.Remove( workerThread );
    }
    
    static bool INTERNAL_AnyWorkerActive()
    {
        if( _workers == null )
            return false;
        for( int i = _workers.Count - 1; i >= 0; i-- )
        {
            var w = _workers[ i ];
            if( w.IsRunning )
                return true;
        }
        return false;
    }
    
    static void INTERNAL_StopActiveWorkers( bool sync, bool disposeOfWorkers )
    {
        if( _workers == null )
            return;
        for( int i = _workers.Count - 1; i >= 0; i-- )
        {
            var w = _workers[ i ];
            if( w.IsRunning )
                w.Stop( sync );
            if( disposeOfWorkers )
                w.Dispose();
        }
    }
    
    #endregion
    
}
