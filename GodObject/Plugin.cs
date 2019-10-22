/*
 * Plugin.cs
 * 
 * Plugin loader and initialization.
 * 
 */
using System;
using System.Collections.Generic;

using GUIBuilder;

using XeLib;
using XeLib.API;


namespace GodObject
{
    
    public static class Debug
    {
        public static void DumpAncestralTree( Engine.Plugin.Interface.IXHandle o, Engine.Plugin.Interface.IXHandle s = null )
        {
            if( ( s != null )&&( s == o ) )
            {
                DebugLog.WriteLine( "\tI'm my own grandpa!" );
                return;
            }
            if( s == null )
            {
                DebugLog.WriteLine( "\nDumpAncestralTree" );
                s = o;
            }
            DebugLog.WriteLine( string.Format(
                "\t{0} :: 0x{1} - \"{2}\"",
                o.GetType().ToString(),
                o.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ),
                o.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
            if( o.Ancestor != null )
                DumpAncestralTree( o.Ancestor, s );
        }
    }
    
    public static partial class Plugin
    {
        
        #region Internal variables
        
        static List<string> _loadPlugins = null;
        static string _workingFile = null;
        static bool _openRenderWindowOnLoad = false;
        
        //static HandleGroup _hgroup = null;
        static bool _isLoading = false;
        static bool _isLoaded = false;
        
        static WorkerThreadPool.WorkerThread _thread = null;
        
        #endregion
        
        #region Init/Denit
        
        public static bool Initialize()
        {
            Meta.Initialize();
            
            // Set FO4 environment and get the game path
            Setup.SetGameMode( Setup.GameMode.GmFo4 );
            GodObject.Paths.Fallout4 = Setup.GetGamePath( Setup.GameMode.GmFo4 );
            
            return !string.IsNullOrEmpty( GodObject.Paths.Fallout4Data );
        }
        
        public static void Denit()
        {
            //DebugLog.Write( "\nbbGlobal.Plugin.Denit()" );
            Meta.Close();
        }
        
        #endregion
        
        #region State Properties
        
        public static bool IsLoading
        {
            get
            {
                return _isLoading;
            }
        }
        
        public static bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }
        
        #endregion
        
        #region [Un]Load plugin
        
        public static bool Load( string workingFile, List<string> plugins, bool openRenderWindowOnLoad )
        {
            var isLoading = _isLoading;
            
            if( ( plugins.NullOrEmpty() )||( string.IsNullOrEmpty( workingFile ) ) )
                return false;
            
            if( ( _isLoaded )&&( !_isLoading ) )
            {
                GodObject.Windows.SetEnableState( true );
                return false;
            }
            if( _isLoading )
                return false;
            
            //_plugin = plugin;
            _workingFile = workingFile;
            _loadPlugins = plugins.Clone();
            _openRenderWindowOnLoad = openRenderWindowOnLoad;
            _isLoaded = false;
            
            _thread = WorkerThreadPool.CreateWorker(
                THREAD_PluginLoader,
                null
            );
            
            _isLoading = ( _thread != null );
            if( _isLoading )
                _thread.Start();
            
            return _isLoading;
        }
        
        public static void Unload()
        {
            INTERNAL_Unload( true, true, true, null );
        }
        
        #endregion
        
        #region Internal functions
        
        static bool SyncWithThread( WorkerThreadPool.ThreadStart start, WorkerThreadPool.WorkerThread syncWith, string prefix )
        {
            var t = WorkerThreadPool.CreateWorker( start, null );
            if( t == null ) return false;
            
            //t.TimeThread = timeThread;
            t.Start();
            return t.Sync( prefix, syncWith );
        }
        
        public static bool BuildAllPluginReferences( WorkerThreadPool.WorkerThread thread = null )
        {
            Messages.ClearMessages();
            return 
                Setup.BuildReferencesEx( ElementHandle.BaseXHandleValue, false ) &&
                XeLibHelper.Thread.Sync( "GodObject.Plugin :: BuildAllPluginReferences()", thread );
        }
        
        public static bool BuildReferencesFor( Engine.Plugin.File mod, WorkerThreadPool.WorkerThread thread = null )
        {
            Messages.ClearMessages();
            return
                mod.BuildReferences() &&
                XeLibHelper.Thread.Sync( "GodObject.Plugin :: BuildReferencesFor()", thread );
        }
        
        static void INTERNAL_Unload( bool dispose, bool stopThread, bool sync, string prefix )
        {
            DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin", "INTERNAL_Unload()", "dispose = " + dispose.ToString(), "stopThread = " + stopThread.ToString(), "sync = " + sync.ToString(), "prefix = \"" + prefix + "\"" } );
            
            var isLoading = _isLoading;
            _isLoading = true;
            _isLoaded = false;
            
            GodObject.Windows.CloseAllChildWindows();
            
            if( ( _thread != null )&&( stopThread ) )
            {
                _thread.Stop( sync );
                _thread = null;
            }
            
            if( dispose )
            {
                // Unload references to core forms...
                Data.Clear();
                
                // ...then unload core forms
                foreach( var form in CoreForms.Forms )
                    form.Dispose();
                
            }
            
            _isLoading = isLoading;
            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Worker Threads
        
        static void THREAD_SyncGodObjects()
        {
            DebugLog.OpenIndentLevel();
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Plugin.LoadBaseForms".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            foreach( var form in CoreForms.Forms )
            {
                //DebugLog.Write( string.Format( "\n{0} :: 0x{1} :: {2}", form.Signature, form._Forced_FormID.ToString( "X8" ), form._Forced_Filename ) );
                if( Data.Files.IsLoaded( form.MasterHandle.Filename ) )
                {
                    var handle = form.MasterHandle;
                    if( !handle.IsValid() )
                        throw new Exception( string.Format( "Unable to get handle for form 0x{0}", form.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ) ) );
                    
                    //form.DebugDump();
                }
            }
            
            m.StopSyncTimer(
                "GodObject.Plugin :: SyncGodObjects() :: Completed in {0}",
                tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        static void THREAD_SyncGodObjectReferences()
        {
            DebugLog.OpenIndentLevel();
            Data.Load();
            Data.PostLoad();
            DebugLog.CloseIndentLevel();
        }
        
        static void THREAD_PluginLoader()
        {
            DebugLog.OpenIndentLevel();
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            try
            {
                
                #region Create string of plugins to load
                
                DebugLog.OpenIndentLevel( "Selected Plugins" );
                DebugLog.WriteList( "Filenames", _loadPlugins );
                DebugLog.CloseIndentLevel();
                DebugLog.WriteLine( new [] { "Working file", _workingFile } );
                
                if( _thread.StopSignal )
                    throw new Exception( "SignalStop" );
                
                #endregion
                
                #region Load plugins
                
                m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingFile".Translate(), _workingFile ) );
                Messages.ClearMessages();
                Setup.LoadPlugins( _loadPlugins );
                
                if( ( !XeLibHelper.Thread.Sync( "XeLib.API.Setup :: LoadPlugins()", null ) )||( _thread.StopSignal ) )
                {
                    if( _thread.StopSignal )
                        throw new Exception( "SignalStop" );
                    
                    throw new Exception( "XeLib Error" );
                }
                else
                {
                
                #endregion
                
                #region Load data from plugins
                    
                    #region Create list of mods from the loaded plugins
                    
                    DebugLog.OpenIndentLevel( "GetPluginHandles()" );
                    
                    Data.Files.Loaded = new List<Engine.Plugin.File>();
                    
                    foreach( var fileHandle in Setup.GetLoadedFiles() )
                    {
                        var newFile = new Engine.Plugin.File( fileHandle );
                        Data.Files.Loaded.Add( newFile );
                        
                        // Populate master references for specific functionality
                        var filename = fileHandle.Filename;
                        if( filename.InsensitiveInvariantMatch( Master.Filename.Fallout4 ) )
                            Master.Fallout4.Mod = newFile;
                        if( filename.InsensitiveInvariantMatch( Master.Filename.AnnexTheCommonwealth ) )
                            Master.AnnexTheCommonwealth.Mod = newFile;
                        if( filename.InsensitiveInvariantMatch( Master.Filename.SimSettlements ) )
                            Master.SimSettlements.Mod = newFile;
                        // Populate the working file handle
                        if( filename.InsensitiveInvariantMatch( _workingFile ) )
                            Data.Files.Working = newFile;
                    }
                    
                    DebugLog.CloseIndentLevel();
                    
                    if( _thread.StopSignal )
                        throw new Exception( "SignalStop" );
                    
                    #endregion
                    
                    #region Build references for the plugins
                    
                    if( !BuildAllPluginReferences( _thread ) )
                        throw new Exception( "Unable to BuildAllPluginReferences()" );
                    if( _thread.StopSignal )
                        throw new Exception( "SignalStop" );
                    
                    #endregion
                    
                    #region Get the God Objects needed from the required masters
                    
                    if( !SyncWithThread( THREAD_SyncGodObjects, _thread, "GodObject.Plugin :: SyncGodObjects()" ) )
                    {
                        if( _thread.StopSignal )
                            throw new Exception( "SignalStop" );
                        
                        throw new Exception( "Unable to sync plugin data!" );
                    }
                    
                    #endregion
                    
                    #region Sync data in plugins
                    
                    if( !SyncWithThread( THREAD_SyncGodObjectReferences, _thread, "GodObject.Plugin :: SyncGodObjectReferences()" ) )
                    {
                        if( _thread.StopSignal )
                            throw new Exception( "SignalStop" );
                        
                        throw new Exception( "Unable to sync plugin data!" );
                    }
                    
                    #endregion
                    
                    _isLoaded = true;
                    
                    if( _openRenderWindowOnLoad )
                        GodObject.Windows.GetRenderWindow( true );
                }
                #endregion
            }
            
            #region Sum Ting Wong
            
            catch( Exception e )
            {
                DebugLog.WriteLine( "THREAD_PluginLoader() :: An exception has occured!" );
                DebugLog.WriteLine( "THREAD_PluginLoader() :: Exception Data ::\n" + e.ToString() );
                
                System.Windows.Forms.MessageBox.Show(
                    e.ToString(),
                    "Exception during load!",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Stop );
                
                INTERNAL_Unload( true, false, false, "THREAD_PluginLoader()" );
            }
            
            #endregion
            
            m.StopSyncTimer( "GodObject.Plugin :: PluginLoader() :: Thread finished in {0}", tStart.Ticks );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( true );
            _workingFile = string.Empty;
            _isLoading = false;
            
            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
    }
    
}

