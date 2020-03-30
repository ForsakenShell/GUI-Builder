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
using System.Reflection;

public static class WorkerThreadPool
{
    
    public delegate void ThreadStart();
    public delegate void ThreadStartParams( object obj );
    public delegate void ThreadOnFinish();
    public delegate void ThreadOnFinishParams( object obj );

    #region Global Unique Thread Identities

    [ThreadStatic]
    static MethodBase _StartMethodBase = null;

    public static MethodBase StartMethodBase
    {
        get
        {
            return _StartMethodBase;
        }
        set
        {
            if( _StartMethodBase != null )
                throw new Exception( "StartMethodBase can only be set once per thread" );
            _StartMethodBase = value;
        }
    }

    static public string StartMethodBaseName( bool trimThreadPrefix )
    {
        return GenString.FormatMethod( StartMethodBase, null, false, trimThreadPrefix, true );
    }

    static public string StartMethodBaseNameFriendly( bool trimThreadPrefix )
    {
        return GenString
            .FormatMethod( StartMethodBase, null, false, trimThreadPrefix, true )
            .ReplaceInvalidFilenameChars();
    }

    static public int CurrentThreadID { get { return Thread.CurrentThread.ManagedThreadId; } }

    /// <summary>
    /// The thread.Name field must be set before calling this function.
    /// Returns a unique filename friendly ID string for a thread.
    /// </summary>
    /// <param name="thread"></param>
    /// <returns>Unique string for any given thread</returns>
    public static string FriendlyThreadIdName( Thread thread = null )
    {
        thread = thread ?? Thread.CurrentThread;
        return string.Format(
                "{0}0x{1}_{2}",
                GenString.THREAD_PREFIX,
                thread.ManagedThreadId.ToString( "X8" ),
                thread.Name
            );
    }

    public static void SetName( string name )
    {
        SetName( null, name );
    }

    public static void SetName( Thread thread, string name )
    {
        if( string.IsNullOrEmpty( name ) ) return;
        ( thread ?? Thread.CurrentThread ).Name = name;
    }

    public static string GetName( Thread thread = null )
    {
        return ( thread ?? Thread.CurrentThread ).Name;
    }

    #endregion

    public class WorkerThread : IDisposable
    {

        Thread _thread = null;
        MethodBase _StartMethodBase = null;

        internal object _Obj;
        
        internal bool _StopSignal;
        
        internal ThreadStart _Start;
        internal ThreadStartParams _StartParams;
        internal ThreadOnFinish _OnFinish;
        internal ThreadOnFinishParams _OnFinishParams;

        #region Allocation & Disposal

        #region Allocation

        public WorkerThread( ThreadStart start, ThreadOnFinish onFinish, MethodBase reportStartMethod = null, string nameSuffix = null )
        {
            INTERNAL_cTor( null, start, onFinish, null, null, reportStartMethod, nameSuffix );
        }
        
        public WorkerThread( object obj, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams, MethodBase reportStartMethod = null, string nameSuffix  = null )
        {
            INTERNAL_cTor( obj, null, null, startParams, onFinishParams, reportStartMethod, nameSuffix );
        }
        
        void INTERNAL_cTor( object obj, ThreadStart start, ThreadOnFinish onFinish, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams, MethodBase reportStartMethod, string nameSuffix )
        {
            _Obj = obj;
            
            _StopSignal = false;

            _Start = start;
            _OnFinish = onFinish;

            _StartParams = startParams;
            _OnFinishParams = onFinishParams;

            _StartMethodBase = reportStartMethod != null
                ? reportStartMethod
                : _StartParams != null
                        ? _StartParams.Method
                        : _Start?.Method;

            var _nameSuffix = nameSuffix == null ? null : "_" + nameSuffix;
            var _nameFriendly =
                GenString.FormatMethod(
                    _StartMethodBase,
                    null, false, true, true
                ).ReplaceInvalidFilenameChars();

            var _name =  string.Format(
                "{0}{1}",
                _nameFriendly,
                _nameSuffix );

            _thread = new Thread( InvokeWorker )
            {
                Name = _name
            };

            /*
                !string.IsNullOrEmpty( threadName )
                ? threadName
                :
                    start       != null ? start      .Method.Name :
                    startParams != null ? startParams.Method.Name :
                    "Thread_ID_0x" + _thread.ManagedThreadId.ToString( "X8" );
            */
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
            Disposed = true;

            if( IsRunning )
                Stop( true );
            
            _Obj = null;
            
            _Start = null;
            _StartParams = null;
            
            _OnFinish = null;
            _OnFinishParams = null;
            
            _thread = null;
            
            WorkerThreadPool.INTERNAL_RemoveWorker( this );
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
            DebugLog.WriteLine( FriendlyThreadIdName( _thread ), true );
            _thread.Start();
            return IsRunning;
        }
        
        public void Stop( bool sync )
        {
            DebugLog.WriteLine( FriendlyThreadIdName( _thread ), true );
            _StopSignal = true;
            if( sync )
                while( IsRunning )
                    System.Threading.Thread.Sleep( 0 );
        }
        
        public bool Sync( string wThreadTypeMethod, WorkerThreadPool.WorkerThread syncWith = null )
        {
            DebugLog.OpenIndentLevel( wThreadTypeMethod, false );

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
            
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            
            DebugLog.CloseIndentLevel();
            
            return _StopSignal == false;
        }
        
        #endregion
        
        void InvokeWorker()
        {
            // Set the thread static global
            StartMethodBase = _StartMethodBase;

            // Start the worker
            try
            {
                if( _StartParams != null )
                    _StartParams( _Obj );
                else _Start?.Invoke();
            }
            catch( Exception e )
            {
                GodObject.Windows.SetEnableState( true );
                DebugLog.WriteException( e );
            }

            // OnFinish callback
            try
            {
                if( _OnFinishParams != null )
                    _OnFinishParams( _Obj );
                else _OnFinish?.Invoke();
            }
            catch( Exception e )
            {
                GodObject.Windows.SetEnableState( true );
                DebugLog.WriteError( "An exception has occured while executing the thread OnFinish handler\n" + e.ToString() );
            }

            if( DebugLog.Initialized )
                DebugLog.Close();
        }
        
    }
    
    static List<WorkerThread> _workers = null;
    
    #region Public API to create and manage workers
    
    public static WorkerThread CreateWorker( ThreadStart start, ThreadOnFinish onFinish, MethodBase reportStartMethod = null, string nameSuffix = null )
    {
        if( _workers == null )
            _workers = new List<WorkerThread>();
        if( _workers == null )
            return null;
        var w = new WorkerThread( start, onFinish, reportStartMethod, nameSuffix );
        if( ( w == null )||( !w.IsReady ) )
            return null;
        _workers.Add( w );
        return w;
    }
    
    public static WorkerThread CreateWorker( object obj, ThreadStartParams startParams, ThreadOnFinishParams onFinishParams, MethodBase reportStartMethod = null, string nameSuffix = null )
    {
        if( _workers == null )
            _workers = new List<WorkerThread>();
        if( _workers == null )
            return null;
        var w = new WorkerThread( obj, startParams, onFinishParams, reportStartMethod, nameSuffix );
        if( ( w == null )||( !w.IsReady ) )
            return null;
        _workers.Add( w );
        return w;
    }
    
    public static void StopAllWorkers( bool sync, bool disposeOfWorkers )
    {
        DebugLog.OpenIndentLevel();
        
        if( sync )
            DebugLog.WriteLine( "Syncing..." );
        
        INTERNAL_StopActiveWorkers( sync, disposeOfWorkers );
        
        if( sync )
            DebugLog.WriteLine( "...Stopped" );

        DebugLog.CloseIndentLevel();
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
