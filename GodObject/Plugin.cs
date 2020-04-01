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
        public static void DumpAncestralTree( Engine.Plugin.Interface.IXHandle obj, Engine.Plugin.Interface.IXHandle initialObj = null )
        {
            if( ( initialObj != null )&&( initialObj == obj ) )
            {
                DebugLog.WriteLine( "\tI'm my own grandpa!" );
                return;
            }
            if( initialObj == null )
            {
                DebugLog.WriteCaller( false );
                initialObj = obj;
            }
            DebugLog.WriteLine( string.Format(
                "\t{0} :: {1}",
                obj.TypeFullName(),
                obj.IDString ) );
            if( obj.Ancestor != null )
                DumpAncestralTree( obj.Ancestor, initialObj );
        }
    }

    public static partial class Plugin
    {
        
        #region Internal variables
        
        
        private static GUIBuilder.Workspace _Workspace = null;
        public static GUIBuilder.Workspace Workspace {
            get
            {
                //Console.WriteLine( "GodObject.Plugin.Workspace" );
                return _Workspace;
            }
        }

        
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
        
        #region Load/Create Workspace
        
        public static bool Load( string workspace )
        {
            var result = false;
            if( _Workspace != null ) return result;
            _Workspace = new Workspace( workspace );
            if( _Workspace != null )
            {
                result = Load(
                    Workspace.WorkingFile,
                    Workspace.PluginNames,
                    Workspace.OpenRenderWindowOnLoad );
            }
            return result;
        }
        
        public static bool CreateWorkspace()
        {
            if( string.IsNullOrEmpty( _workingFile ) )
            {
                DebugLog.WriteError( "No working file!" );
                return false;
            }
            if( _Workspace != null ) return false;
            _Workspace = new Workspace( _workingFile );
            if( _Workspace == null )
            {
                DebugLog.WriteError( "Could not create Workspace!" );
                return false;
            }
            _Workspace.PluginNames = _loadPlugins;
            _Workspace.WorkingFile = _workingFile;
            _Workspace.OpenRenderWindowOnLoad = _openRenderWindowOnLoad;
            _Workspace.Commit();
            GUIBuilder.CustomForms.SaveToWorkspace();
            
            return true;
        }
        
        #endregion
        
        #region [Un]Load plugin
        
        public static bool Load( string workingFile, List<string> plugins, bool openRenderWindowOnLoad )
        {
            var result = false;
            
            if( (_isLoaded )||( plugins.NullOrEmpty() )||( string.IsNullOrEmpty( workingFile ) ) )
                goto localAbort;
            
            if( _isLoading )
            {
                result = true;
                goto localAbort;
            }
            
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
            
            result = _isLoading || _isLoaded;
        localAbort:
            if( !result )
                GodObject.Windows.SetEnableState( null, true );
            return result;
        }
        
        public static bool Unload( WorkerThreadPool.ThreadOnFinish onFinished )
        {
            var unloadThread = WorkerThreadPool.CreateWorker( THREAD_UnloadPlugins, onFinished );
            return unloadThread == null
                ? false
                : unloadThread.Start();
        }
        
        static void THREAD_UnloadPlugins()
        {
            INTERNAL_Unload( true, true, true, null, true );
        }

        #endregion
        
        #region Internal functions
        
        static bool SyncWithThread( WorkerThreadPool.ThreadStart start, WorkerThreadPool.WorkerThread syncWith )
        {
            var t = WorkerThreadPool.CreateWorker( start, null );
            if( t == null ) return false;
            
            //t.TimeThread = timeThread;
            t.Start();
            return t.Sync( GenString.FormatMethod( start.Method, null, false, true, true ), syncWith );
        }
        
        public static bool BuildAllPluginReferences( WorkerThreadPool.WorkerThread thread = null )
        {
            Messages.ClearMessages();
            return 
                Setup.BuildReferencesEx( ElementHandle.BaseXHandleValue, false ) &&
                XeLibHelper.Thread.Sync( "XeLib.API.Setup.BuildReferencesEx()", thread );
        }
        
        public static bool BuildReferencesFor( Engine.Plugin.File mod, WorkerThreadPool.WorkerThread thread = null )
        {
            Messages.ClearMessages();
            return
                mod.BuildReferences() &&
                XeLibHelper.Thread.Sync( "Engine.Plugin.File.BuildReferences() :: " + mod.Filename, thread );
        }
        
        static void INTERNAL_Unload( bool dispose, bool stopThread, bool sync, string prefix, bool enableUI )
        {
            DebugLog.OpenIndentLevel( new [] { "dispose = " + dispose.ToString(), "stopThread = " + stopThread.ToString(), "sync = " + sync.ToString(), "prefix = \"" + prefix + "\"" }, true, true, false );
            
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
                // Unload references to custom forms...
                GUIBuilder.CustomForms.Dispose();

                // Unload references to core forms...
                Data.Clear();

                // ...then unload core forms
                CoreForms.Dispose();
                //foreach( var form in CoreForms.Forms )
                //    form.Dispose();
                
            }
            
            if( _Workspace != null )
                _Workspace.Commit();

            _Workspace = null;
            _workingFile = null;
            _isLoading = isLoading;

            if( enableUI )
            {
                GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>( true ).Translate();
                GodObject.Windows.SetEnableState( null, true );
            }

            DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Worker Threads
        
        static void THREAD_LoadCoreForms()
        {
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Plugin.LoadBaseForms".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();

            // Add Workspace custom Core Forms

            var ws = Workspace;
            if( ws != null )
            {
                var workshopIdentifiers = ws.WorkshopWorkbenches;
                if( !workshopIdentifiers.NullOrEmpty() )
                {
                    foreach( var identifier in workshopIdentifiers )
                    {
                        if( Engine.Plugin.Constant.ValidFormID( identifier.FormID ) && identifier.Filename.InsensitiveInvariantMatch( _loadPlugins ) )
                        {
                            Engine.Plugin.Forms.Container customWorkshop;
                            CoreForms.TryAddCustomWorkshopWorkbench( identifier.FormID, identifier.Filename, out customWorkshop );
                        }
                    }
                }
            }

            DebugLog.OpenIndentLevel( "CoreForms", false );
            foreach( var form in CoreForms.Forms )
            {
                //DebugLog.Write( string.Format( "\n{0} :: 0x{1} :: {2}", form.Signature, form._Forced_FormID.ToString( "X8" ), form._Forced_Filename ) );
                if( ( !string.IsNullOrEmpty( form.ForcedFilename ) )&&( Data.Files.IsLoaded( form.ForcedFilename ) ) )
                {
                    var handle = form.MasterHandle;
                    if( !handle.IsValid() )
                        throw new Exception( string.Format( "Unable to get handle for form 0x{0}", form.ForcedFormID.ToString( "X8" ) ) );

                    DebugLog.WriteLine( form.ToStringNullSafe() );

                    //form.DebugDump();
                }
            }
            DebugLog.CloseIndentLevel();
            
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
        }
        
        static void THREAD_LoadCoreFormReferences()
        {
            Data.Load();
            Data.PostLoad();
        }
        
        static void THREAD_PluginLoader()
        {
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            try
            {
                
                #region Create string of plugins to load
                
                DebugLog.OpenIndentLevel( "Selected Plugins", false );
                DebugLog.WriteList( "Filenames", _loadPlugins, false, true );
                DebugLog.WriteLine( "Working file = " + _workingFile );
                DebugLog.CloseIndentLevel();
                
                if( _thread.StopSignal )
                    goto signalStopAbort;
                
                #endregion
                
                #region Load plugins
                
                m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingFile".Translate(), _workingFile ) );
                Messages.ClearMessages();
                Setup.LoadPlugins( _loadPlugins );
                
                if( ( !XeLibHelper.Thread.Sync( "XeLib.API.Setup.LoadPlugins()", null ) )||( _thread.StopSignal ) )
                {
                    if( _thread.StopSignal )
                        goto signalStopAbort;
                    
                    throw new Exception( "XeLib Error" );
                }
                else
                {
                
                #endregion
                
                #region Load data from plugins
                    
                    #region Create list of mods from the loaded plugins
                    
                    DebugLog.OpenIndentLevel( "Get Plugin File Handles", false );
                    
                    Data.Files.Loaded = new List<Engine.Plugin.File>();
                    
                    foreach( var fileHandle in Setup.GetLoadedFiles() )
                    {
                        var newFile = new Engine.Plugin.File( fileHandle );
                        Data.Files.Loaded.Add( newFile );

                        DebugLog.WriteLine( newFile.ToStringNullSafe() );

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
                        goto signalStopAbort;
                    
                    #endregion
                    
                    #region Build references for the plugins
                    
                    foreach( var file in Data.Files.Loaded )
                    {
                        Messages.ClearMessages();
                        
                        if( !file.BuildReferences() )
                            throw new Exception( "Could not start thread fop XeLib.API.Setup.BuildReferencesEx()" );

                        XeLibHelper.Thread.Sync( "XeLib.API.Setup.BuildReferencesEx()", _thread );
                        
                        if( _thread.StopSignal )
                            goto signalStopAbort;
                    }

                    #endregion
                    
                    #region Get the Core Forms needed from the required masters
                    
                    if( !SyncWithThread( THREAD_LoadCoreForms, _thread ) )
                    {
                        if( _thread.StopSignal )
                            goto signalStopAbort;
                        
                        throw new Exception( "Unable to sync plugin data!" );
                    }
                    
                    #endregion
                    
                    #region Load the Core Form References
                    
                    if( !SyncWithThread( THREAD_LoadCoreFormReferences, _thread ) )
                    {
                        if( _thread.StopSignal )
                            goto signalStopAbort;
                        
                        throw new Exception( "Unable to sync plugin data!" );
                    }
                    
                    #endregion
                    
                    _isLoaded = true;
                    
                    if( _openRenderWindowOnLoad )
                        GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( true );
                }

                #endregion

            }
            
            #region Sum Ting Wong
            
            catch( Exception e )
            {
                DebugLog.WriteException( e );
                //DebugLog.WriteError( "An unexpected exception has occured!\n" + e.ToString() );
                
                System.Windows.Forms.MessageBox.Show(
                    e.ToString(),
                    "Exception during load!",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Stop );
                
                INTERNAL_Unload( true, false, false, "PluginLoader()", false );
            }
            
            #endregion
            
        signalStopAbort:
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            GodObject.Windows.SetEnableState( null, true );
            //_workingFile = string.Empty;
            _isLoading = false;
            _thread = null;
        }
        
        #endregion
        
    }
    
}

