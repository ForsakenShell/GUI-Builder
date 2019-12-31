/*
 * Data.cs
 * 
 * TODO: Clean this code and make it more generic.
 * 
 * Global lists of papyrus scripted objects.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Fallout4;
using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;
using XeLibHelper;
using Engine.Plugin.Interface;

namespace GodObject
{
    
    public static partial class Plugin
    {
        
        public static partial class Data
        {

            #region Global CoreForm Management
            
            public static void Clear()
            {
                Workshops.Dispose();
                
                if( _Settlements != null )
                {
                    _Settlements.Dispose();
                    _Settlements = null;
                }
                
                if( _SubDivisions != null )
                {
                    _SubDivisions.Dispose();
                    _SubDivisions = null;
                }
                
                if( _BorderEnablers != null )
                {
                    _BorderEnablers.Dispose();
                    _BorderEnablers = null;
                }
                
                if( _BuildVolumes != null )
                {
                    _BuildVolumes.Dispose();
                    _BuildVolumes = null;
                }
                
                if( _SandboxVolumes != null )
                {
                    _SandboxVolumes.Dispose();
                    _SandboxVolumes = null;
                }
                
                EdgeFlags.Dispose();
                
                Root.Dispose();
                /*
                if( Root._Containers != null )
                {
                    foreach( var bf in Root._Containers )
                        bf.Dispose();
                    Root._Containers.Clear();
                    Root._Containers = null;
                }
                */
                
                /*
                Worldspaces.Clear();
                Statics.Clear();
                Locations.Clear();
                Keywords.Clear();
                */
               
                Files.Clear();
            }
            
            public static void Load()
            {
                DebugLog.OpenIndentLevel( "GodObject.Plugin.Data :: Load()" );
                
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                // Find all references of god objects
                Workshops.Load();
                Settlements.Load();
                SubDivisions.Load();
                BuildVolumes.Load();
                SandboxVolumes.Load();
                EdgeFlags.Load();
                BorderEnablers.Load();
                
                m.StopSyncTimer( "Godbject.Plugin.Data :: Load() :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
                
                DebugLog.CloseIndentLevel();
            }
            
            public static void PostLoad()
            {
                DebugLog.OpenIndentLevel( "GodObject.Plugin.Data :: PostLoad()" );
                
                var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                // Post load all god object references
                Workshops.PostLoad();
                Settlements.PostLoad();
                SubDivisions.PostLoad();
                BuildVolumes.PostLoad();
                SandboxVolumes.PostLoad();
                EdgeFlags.PostLoad();
                BorderEnablers.PostLoad();
                
                m.StopSyncTimer( "Godbject.Plugin.Data :: PostLoad() :: Completed in {0}", tStart.Ticks );
                m.PopStatusMessage();
                
                DebugLog.CloseIndentLevel();
            }

            #region Old Code
#if OLD_CODE
            
            #region Worldspaces
            
            public static class Worldspaces
            {
                
                static Engine.Plugin.Containers.Worldspaces _Container = null;
                
                public static void Clear()
                {
                    if( _Container != null )
                        _Container.Dispose();
                    _Container = null;
                }
                
                public static bool LoadFrom( IDataSync source )
                {
                    if( source == null )
                        return false;
                    
                    if( _Container == null )
                        _Container = new Engine.Plugin.Containers.Worldspaces( null );
                    
                    return _Container.LoadFrom( source );
                }
                
                public static bool PostLoad()
                {
                    return
                        ( _Container == null )||
                        ( _Container.PostLoad() );
                }
                
                public static int Count
                {
                    get
                    {
                        return _Container == null
                            ? 0
                            : _Container.Count;
                    }
                }
                
                public static List<Engine.Plugin.Forms.Worldspace> ToList( int loadOrderFilter = -1 )
                {
                    return _Container == null
                        ? null
                        : _Container.ToList( loadOrderFilter ).ConvertAll( (x) => ( x as Engine.Plugin.Forms.Worldspace ) );
                }
                
                public static Engine.Plugin.Forms.Worldspace Find( uint formid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( formid ) as Engine.Plugin.Forms.Worldspace;
                }
                
                public static Engine.Plugin.Forms.Worldspace Find( string editorid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( editorid ) as Engine.Plugin.Forms.Worldspace;
                }
                
            }
            
            #endregion
            
            #region Locations
            
            public static class Locations
            {
                
                static Engine.Plugin.Containers.Locations _Container = null;
                
                public static void Clear()
                {
                    if( _Container != null )
                        _Container.Dispose();
                    _Container = null;
                }
                
                public static bool LoadFrom( IDataSync source )
                {
                    if( source == null )
                        return false;
                    
                    if( _Container == null )
                        _Container = new Engine.Plugin.Containers.Locations( null );
                    
                    return _Container.LoadFrom( source );
                }
                
                public static bool PostLoad()
                {
                    return
                        ( _Container == null )||
                        ( _Container.PostLoad() );
                }
                
                public static int Count
                {
                    get
                    {
                        return _Container == null
                            ? 0
                            : _Container.Count;
                    }
                }
                
                public static List<Engine.Plugin.Forms.Location> ToList( int loadOrderFilter = -1 )
                {
                    return _Container == null
                        ? null
                        : _Container.ToList( loadOrderFilter ).ConvertAll( (x) => ( x as Engine.Plugin.Forms.Location ) );
                }
                
                public static Engine.Plugin.Forms.Location Find( uint formid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( formid ) as Engine.Plugin.Forms.Location;
                }
                
                public static Engine.Plugin.Forms.Location Find( string editorid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( editorid ) as Engine.Plugin.Forms.Location;
                }
                
            }
            
            #endregion
            
            #region Keywords
            
            public static class Keywords
            {
                
                static Engine.Plugin.Containers.Keywords _Container = null;
                
                public static void Clear()
                {
                    if( _Container != null )
                        _Container.Dispose();
                    _Container = null;
                }
                
                public static bool LoadFrom( IDataSync source )
                {
                    if( source == null )
                        return false;
                    
                    if( _Container == null )
                        _Container = new Engine.Plugin.Containers.Keywords( null );
                    
                    return _Container.LoadFrom( source );
                }
                
                public static bool PostLoad()
                {
                    return
                        ( _Container == null )||
                        ( _Container.PostLoad() );
                }
                
                public static int Count
                {
                    get
                    {
                        return _Container == null
                            ? 0
                            : _Container.Count;
                    }
                }
                
                public static List<Engine.Plugin.Forms.Keyword> ToList( int loadOrderFilter = -1 )
                {
                    var list = _Container == null
                        ? null
                        : _Container.ToList( loadOrderFilter );
                    return list.NullOrEmpty()
                        ? null
                        : list.ConvertAll( (x) => ( x as Engine.Plugin.Forms.Keyword ) );
                }
                
                public static Engine.Plugin.Forms.Keyword Find( uint formid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( formid ) as Engine.Plugin.Forms.Keyword;
                }
                
                public static Engine.Plugin.Forms.Keyword Find( string editorid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( editorid ) as Engine.Plugin.Forms.Keyword;
                }
                
            }
            
            #endregion
            
            #region Statics
            
            public static class Statics
            {
                
                static Engine.Plugin.Containers.Statics _Container = null;
                
                public static void Clear()
                {
                    if( _Container != null )
                        _Container.Dispose();
                    _Container = null;
                }
                
                public static bool LoadFrom( IDataSync source )
                {
                    if( source == null )
                        return false;
                    
                    if( _Container == null )
                        _Container = new Engine.Plugin.Containers.Statics( null );
                    
                    return _Container.LoadFrom( source );
                }
                
                public static bool PostLoad()
                {
                    return
                        ( _Container == null )||
                        ( _Container.PostLoad() );
                }
                
                public static int Count
                {
                    get
                    {
                        return _Container == null
                            ? 0
                            : _Container.Count;
                    }
                }
                
                public static List<Engine.Plugin.Forms.Static> ToList( int loadOrderFilter = -1 )
                {
                    return _Container == null
                        ? null
                        : _Container.ToList( loadOrderFilter ).ConvertAll( (x) => ( x as Engine.Plugin.Forms.Static ) );
                }
                
                public static Engine.Plugin.Forms.Static Find( uint formid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( formid ) as Engine.Plugin.Forms.Static;
                }
                
                public static Engine.Plugin.Forms.Static Find( string editorid )
                {
                    return _Container == null
                        ? null
                        : _Container.Find( editorid ) as Engine.Plugin.Forms.Static;
                }
                
                public static Engine.Plugin.Forms.Static CreateNew()
                {
                    return _Container == null
                        ? null
                        : _Container.CreateNew<Engine.Plugin.Forms.Static>();
                }
                
            }
            
            #endregion
            
#endif
            #endregion

            #endregion

            #region Scripted Objects

            #region Scripted Objects Container Class

            public class ScriptedObjects<TScript> : IDisposable, ISyncedGUIList<TScript>
                where TScript : Engine.Plugin.PapyrusScript
            {
                
                bool                        _IsValid = false;
                Engine.Plugin.Form          _ScriptForm = null;
                Dictionary<uint,TScript>    _ScriptForms = null;
                
                #region Allocation
                
                public ScriptedObjects( Engine.Plugin.Form baseform )
                {
                    if( baseform == null )
                        throw new ArgumentNullException( "baseform" );
                    _ScriptForm = baseform;
                    _IsValid = _ScriptForm.IsValid();
                }
                
                #endregion
                
                #region Disposal
                
                protected bool Disposed = false;
                
                ~ScriptedObjects()
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
                    
                    if( _IsValid )
                    {
                        _IsValid = false;
                        
                        if( ( _ScriptForms != null )&&( _ScriptForms.Count > 0 ) )
                            foreach( var kv in _ScriptForms )
                                kv.Value.Dispose();
                    }
                    
                    _ScriptForms = null;
                    _ScriptForm = null;
                    
                    Disposed = true;
                }
                
                #endregion
                
                public bool IsValid         { get { return _IsValid; } }
                
                public bool Load()
                {
                    if( !_IsValid )
                        return false;
                    
                    DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.ScriptedObjects", "Load()", _ScriptForm.GetEditorID( Engine.Plugin.TargetHandle.Master ), _ScriptForm.ToString() } );
                    
                    var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                    m.PushStatusMessage();
                    m.PushItemOfItems();
                    //m.SetCurrentStatusMessage( string.Format( "Finding all references of \"{0}\"...", _ScriptForm.EditorID ) );
                    m.StartSyncTimer();
                    var tStart = m.SyncTimerElapsed();
                    
                    var result = false;
                    
                    var soh = _ScriptForm.MasterHandle;
                    if( !soh.IsValid() )
                    {
                        DebugLog.WriteError( this.GetType().ToString(), "Load()", "_ScriptForm.MasterHandle is invalid!" );
                        goto localReturnResult;
                    }
                    
                    var iforms = _ScriptForm.References;
                    if( iforms.NullOrEmpty() )
                    {
                        DebugLog.WriteError( this.GetType().ToString(), "Load()", "_ScriptForm.References returned NULL!" );
                        goto localReturnResult;
                    }
                    
                    //DebugLog.WriteList<Engine.Plugin.Form>( _ScriptForm.ToString(), iforms );
                    
                    uint bfFID = _ScriptForm.GetFormID( Engine.Plugin.TargetHandle.Master );
                    string bfEID = _ScriptForm.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    
                    _ScriptForms = new Dictionary<uint,TScript>();
                    
                    m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingReferencesOf".Translate(), bfFID.ToString( "X8" ), bfEID ) );
                    var max = iforms.Count;
                    for( int index = 0; index < max; index++ )
                    {
                        m.SetItemOfItems( index, max );
                        var iform = iforms[ index ];
                        var refr = iform as Engine.Plugin.Forms.ObjectReference;
                        //DebugLog.WriteLine( string.Format( "[ {0} ] :: Load() :: {1}\n{{", index, refr.ToStringNullSafe() ) );
                        if( refr != null )
                        {
                            var rFID = refr.GetFormID( Engine.Plugin.TargetHandle.Master );
                            TScript s;
                            if( !_ScriptForms.TryGetValue( rFID, out s ) )
                            {
                                var script = Engine.Plugin.PapyrusScript.CreateScript<TScript>( refr );
                                if( script != null )
                                {
                                    _ScriptForms[ rFID ] = script;
                                    if( !script.Load() )
                                    {
                                        DebugLog.WriteError( this.GetType().ToString(), "Load()", "_ScriptForm.Reference[ " + index + " ].Load() returned false!" );
                                        goto localReturnResult;
                                    }
                                }
                            }
                        }
                        //DebugLog.WriteLine( "}" );
                    }
                    
                    result = true;
                    
                localReturnResult:
                    var tEnd = m.SyncTimerElapsed().Ticks - tStart.Ticks;
                    m.StopSyncTimer( string.Format( "{0} :: Load() :: Completed in {1}", this.GetType().ToString(), "{0}" ), tStart.Ticks );
                    m.PopItemOfItems();
                    m.PopStatusMessage();
                    
                    DebugLog.CloseIndentLevel( tEnd, "result", result.ToString() );
                    return result;
                }
                
                public bool PostLoad()
                {
                    if( !_IsValid )
                        return false;
                    
                    DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "PostLoad()", _ScriptForm.ToString() } );
                    
                    var result = true;
                    
                    var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                    m.PushStatusMessage();
                    m.PushItemOfItems();
                    m.SetCurrentStatusMessage( string.Format( "Plugin.PostLoadReferencesOf".Translate(), _ScriptForm.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), _ScriptForm.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                    m.StartSyncTimer();
                    var tStart = m.SyncTimerElapsed();
                    
                    if( ( _ScriptForms != null )&&( _ScriptForms.Count > 0 ) )
                    {
                        var iforms = _ScriptForms.Values.ToList();
                        //DebugLog.WriteList<TScript>( _ScriptForm.ToString(), iforms );
                        var max = iforms.Count;
                        for( int index = 0; index < max; index++ )
                        {
                            m.SetItemOfItems( index, max );
                            var v = iforms[ index ];
                            //DebugLog.WriteLine( string.Format( "[ {0} ] :: PostLoad() :: {1}\n{{", index, v.ToString() ) );
                            result &= v.PostLoad();
                            //DebugLog.WriteLine( "}" );
                            if( !result )
                                break;
                        }
                    }
                    
                //localReturnResult:
                    var tEnd = m.SyncTimerElapsed().Ticks - tStart.Ticks;
                    m.StopSyncTimer( string.Format( "{0} :: PostLoad() :: Completed in {1}", this.GetType().ToString(), "{0}" ), tStart.Ticks );
                    m.PopItemOfItems();
                    m.PopStatusMessage();
                    
                    DebugLog.CloseIndentLevel( tEnd, "result", result.ToString() );
                    return result;
                }
                
                #region ISyncedGUIList
                
                #region Syncronization
                
                public event EventHandler  ObjectDataChanged;
                
                bool _SupressObjectDataChangedEvent = false;
                public void SupressObjectDataChangedEvents()
                {
                    if( !_IsValid )
                        return;
                    _SupressObjectDataChangedEvent = true;
                }
                public void ResumeObjectDataChangedEvents( bool sendevent )
                {
                    if( !_IsValid )
                        return;
                    
                    _SupressObjectDataChangedEvent = false;
                    if( sendevent )
                        SendObjectDataChangedEvent();
                }
                
                public void SendObjectDataChangedEvent()
                {
                    if( !_IsValid )
                        return;
                    
                    if( _SupressObjectDataChangedEvent )
                        return;
                    EventHandler handler = ObjectDataChanged;
                    if( handler != null )
                        handler( this, null );
                }
                
                #endregion
                
                #region Enumeration
                
                public int Count
                {
                    get
                    {
                        if( !_IsValid )
                            return 0;
                        
                        return _ScriptForms == null
                            ? 0
                            : _ScriptForms.Count;
                    }
                }
                
                public void Add( TScript item )
                {
                    if( !_IsValid )
                        return;
                    
                    if( _ScriptForms == null )
                        _ScriptForms = new Dictionary<uint,TScript>();
                    _ScriptForms[ item.GetFormID( Engine.Plugin.TargetHandle.Master ) ] = item;
                    SendObjectDataChangedEvent();
                }
                
                public bool Remove( TScript item )
                {
                    if( ( !_IsValid )||( _ScriptForms != null ) ) return false;
                    var result =_ScriptForms.Remove( item.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    if( result )
                        SendObjectDataChangedEvent();
                    return result;
                }
                
                public List<TScript> ToList( bool includePackInReferences )
                {
                    if( ( !_IsValid )||( _ScriptForms == null )||( _ScriptForms.Count == 0 ) )
                        return null;
                    return _ScriptForms.Values.Where( x => includePackInReferences || !x.Reference.Cell.GetIsPackInCell( Engine.Plugin.TargetHandle.Master ) ).ToList();
                    //return _ScriptForms.Values.ToList();
                }
                
                public TScript Find( uint formid )
                {
                    if( ( !_IsValid )||( _ScriptForms == null )||( !Engine.Plugin.Constant.ValidFormID( formid ) ) )
                        return null;
                    
                    TScript s = null;
                    _ScriptForms.TryGetValue( formid, out s );
                    
                    return s;
                }
                
                public TScript Find( string editorid )
                {
                    if( ( !_IsValid )||( _ScriptForms == null )||( string.IsNullOrEmpty( editorid ) ) )
                        return null;
                    
                    foreach( var kv in _ScriptForms )
                        if( kv.Value.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == editorid )
                            return kv.Value;
                    
                    return null;
                }
                
                public List<TScript> FindAllInWorldspace( string editorid )
                {
                    if( !_IsValid )
                        return null;
                    //var cWorldspaces = GodObject.Plugin.Data.Root.GetContainer<Engine.Plugin.Forms.Worldspace>( true, false );
                    //if( cWorldspaces == null )
                    //{
                    //    DebugLog.Write( string.Format( "{0} :: FindAllInWorldspace() :: Unable to get root container for Worldspaces!", this.GetType().ToString() ) );
                    //    return null;
                    //}
                    //var worldspace = cWorldspaces.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                    var worldspace = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                
                public List<TScript> FindAllInWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
                {
                    if( !_IsValid )
                        return null;
                    //DebugLog.WriteLine( string.Format( "{0} :: FindAllInWorldspace() :: worldspace ? {1}", this.GetType().ToString(), worldspace.ToStringNullSafe() ) );
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                
                //bool doOnceInFindAllInWorldspace = false;
                public List<TScript> FindAllInWorldspace( uint formid )
                {
                    //DebugLog.WriteLine( string.Format( "{0} :: FindAllInWorldspace() :: worldspace ? 0x{1}", this.GetType().ToString(), formid.ToString( "X8" ) ) );
                    if( ( !_IsValid )||( _ScriptForms == null )||( !Engine.Plugin.Constant.ValidFormID( formid ) ) )
                        return null;
                    
                    var list = new List<TScript>();
                    
                    foreach( var kv in _ScriptForms )
                    {
                        /*
                        if( !doOnceInFindAllInWorldspace )
                        {
                            var refr = kv.Value.Reference;
                            var cell = refr == null ? null : refr.Cell;
                            var wrld = cell == null ? null : cell.Worldspace;
                            DebugLog.WriteLine( new [] { kv.Value.ToString(), refr.ToStringNullSafe(), cell.ToStringNullSafe(), wrld.ToStringNullSafe() } );
                        }
                        */
                        if( ( kv.Value.Reference.Worldspace != null )&&( kv.Value.Reference.Worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) == formid ) )
                            list.Add( kv.Value );
                    }
                    //doOnceInFindAllInWorldspace = true;
                    
                    return list.Count == 0
                        ? null
                        : list;
                }
                
                #endregion
                
                #endregion
                
            }

            #endregion

            #region Scripted Object: Workshops

            public static class Workshops
            {

                #region Global BaseForm Agnosticator (BaseForm ScriptedObjects<WorkshopScript>() Handler)

                static List<ScriptedObjects<WorkshopScript>> _Forms = null;

                static WorkshopObjects                       _SyncedGUIList = null;

                #region Disposal

                private static bool Disposed = false;

                public static void Dispose()
                {
                    Dispose( true );
                }

                private static void Dispose( bool disposing )
                {
                    if( Disposed )
                        return;
                    Disposed = true;

                    if( _SyncedGUIList != null )
                    {
                        _SyncedGUIList.Dispose();
                        _SyncedGUIList = null;
                    }

                    if( _Forms != null )
                    {
                        foreach( var forms in _Forms )
                            if( forms != null )
                                forms.Dispose();
                        _Forms = null;
                    }
                }

                #endregion

                #region Internal ScriptedObjects<WorkshopScript>

                internal static List<ScriptedObjects<WorkshopScript>> Forms
                {
                    get
                    {
                        if( _Forms == null )
                        {
                            _Forms = new List<ScriptedObjects<WorkshopScript>>();
                            foreach( var workbench in CoreForms.WorkshopWorkbenches )
                                _Forms.Add( new ScriptedObjects<WorkshopScript>( workbench ) );
                        }
                        return _Forms;
                    }
                }

                #endregion

                #region Public Global Form Agnosticator (BaseForm WorkshopScript Abstractor)

                public static WorkshopObjects SyncedGUIList
                {
                    get
                    {
                        if( _SyncedGUIList == null )
                            _SyncedGUIList = new WorkshopObjects();
                        return _SyncedGUIList;
                    }
                }

                #endregion

                #region Public Global Load/Unload

                public static bool Load()
                {
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                        foreach( var form in forms )
                            if( !form.Load() ) return false;
                    return true;
                }

                public static bool PostLoad()
                {
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                        foreach( var form in forms )
                            if( !form.PostLoad() ) return false;
                    return true;
                }

                #endregion

                #region Public Global Form Agnostic Search

                public static WorkshopScript Find( uint formid )
                {
                    WorkshopScript result = null;
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                    {
                        foreach( var form in forms )
                        {
                            result = form.Find( formid );
                            if( result != null )
                                return result;
                        }
                    }
                    return null;
                }

                public static WorkshopScript Find( string editorid )
                {
                    WorkshopScript result = null;
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                    {
                        foreach( var form in forms )
                        {
                            result = form.Find( editorid );
                            if( result != null )
                                return result;
                        }
                    }
                    return null;
                }

                public static List<WorkshopScript> FindAllInWorldspace( string editorid )
                {
                    var worldspace = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }

                public static List<WorkshopScript> FindAllInWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
                {
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }

                public static List<WorkshopScript> FindAllInWorldspace( uint formid )
                {
                    var forms = Forms;
                    if( ( forms.NullOrEmpty() ) || ( !Engine.Plugin.Constant.ValidFormID( formid ) ) )
                        return null;

                    var list = new List<WorkshopScript>();

                    foreach( var form in forms )
                    {
                        var workshopRefs = form.FindAllInWorldspace( formid );
                        if( !workshopRefs.NullOrEmpty() )
                            list.AddAll( workshopRefs );
                    }

                    return list.Count == 0
                        ? null
                        : list;
                }

                #endregion

                #endregion

                #region Global Form Reference Agnosticator (BaseForm ScriptedObjects<WorkshopScript>() -> ISyncedGUIList<WorkshopScript> Handler)

                public class WorkshopObjects : IDisposable, ISyncedGUIList<WorkshopScript>
                {

                    #region Allocation

                    public WorkshopObjects()
                    {
                    }

                    #endregion

                    #region Disposal

                    protected bool Disposed = false;

                    ~WorkshopObjects()
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

                        GodObject.Plugin.Data.Workshops.Dispose();

                    }

                    #endregion

                    public bool IsValid { get { return Files.IsLoaded( Master.Filename.Fallout4 ); } }

                    /*
                    public bool Load()
                    {
                        if( !IsValid ) return false;
                        return GodObject.Plugin.Data.Workshops.Load();
                    }

                    public bool PostLoad()
                    {
                        if( !IsValid ) return false;
                        return GodObject.Plugin.Data.Workshops.Load();
                    }
                    */

                    #region ISyncedGUIList

                    #region Syncronization

                    public event EventHandler  ObjectDataChanged;

                    bool _SupressObjectDataChangedEvent = false;
                    public void SupressObjectDataChangedEvents()
                    {
                        _SupressObjectDataChangedEvent = true;
                    }
                    public void ResumeObjectDataChangedEvents( bool sendevent )
                    {
                        _SupressObjectDataChangedEvent = false;
                        if( sendevent )
                            SendObjectDataChangedEvent();
                    }

                    public void SendObjectDataChangedEvent()
                    {
                        if( _SupressObjectDataChangedEvent )
                            return;
                        EventHandler handler = ObjectDataChanged;
                        if( handler != null )
                            handler( this, null );
                    }

                    #endregion

                    #region Enumeration

                    public int Count
                    {
                        get
                        {
                            var forms = Forms;
                            if( forms.NullOrEmpty() )
                                return 0;

                            int count = 0;
                            foreach( var form in forms )
                                count += form.Count;
                            return count;
                        }
                    }

                    public void Add( WorkshopScript item )
                    {   // TODO:  Write me
                        throw new NotImplementedException();
                    }

                    public bool Remove( WorkshopScript item )
                    {   // TODO:  Write me
                        throw new NotImplementedException();
                    }

                    public List<WorkshopScript> ToList( bool includePackInReferences )
                    {
                        var forms = Forms;
                        if( forms.NullOrEmpty() )
                            return null;
                        var list = new List<WorkshopScript>();
                        foreach( var form in forms )
                            list.AddAll( form.ToList( includePackInReferences ) );
                        return list;
                    }

                    public WorkshopScript Find( uint formid )
                    {
                        var forms = Forms;
                        if( forms.NullOrEmpty() )
                            return null;
                        WorkshopScript result = null;
                        foreach( var form in forms )
                        {
                            result = form.Find( formid );
                            if( result != null ) break;
                        }
                        return result;
                    }

                    public WorkshopScript Find( string editorid )
                    {
                        var forms = Forms;
                        if( forms.NullOrEmpty() )
                            return null;
                        WorkshopScript result = null;
                        foreach( var form in forms )
                        {
                            result = form.Find( editorid );
                            if( result != null ) break;
                        }
                        return result;
                    }

                    public List<WorkshopScript> FindAllInWorldspace( string editorid )
                    {
                        var worldspace = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                        return worldspace == null
                            ? null
                            : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    }

                    public List<WorkshopScript> FindAllInWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
                    {
                        return worldspace == null
                            ? null
                            : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    }

                    public List<WorkshopScript> FindAllInWorldspace( uint formid )
                    {
                        if( !Engine.Plugin.Constant.ValidFormID( formid ) )
                            return null;

                        var forms = Forms;
                        if( forms.NullOrEmpty() )
                            return null;

                        var list = new List<WorkshopScript>();
                        foreach( var form in forms )
                            list.AddAll( form.FindAllInWorldspace( formid ) );

                        return list.Count == 0
                            ? null
                            : list;
                    }

                    #endregion

                    #endregion

                }

                #endregion

            }

            #endregion

            #region Scripted Object: Settlements

            static ScriptedObjects<Settlement> _Settlements = null;
            public static ScriptedObjects<Settlement> Settlements
            {
                get
                {
                    if( _Settlements == null )
                        _Settlements = new ScriptedObjects<Settlement>( CoreForms.ESM_ATC_ACTI_Settlement );
                    return _Settlements;
                }
            }
            
            #endregion
            
            #region Scripted Object: Sub-Divisions
            
            static ScriptedObjects<SubDivision> _SubDivisions = null;
            public static ScriptedObjects<SubDivision> SubDivisions
            {
                get
                {
                    if( _SubDivisions == null )
                        _SubDivisions = new ScriptedObjects<SubDivision>( CoreForms.ESM_ATC_ACTI_SubDivision );
                    return _SubDivisions;
                }
            }
            
            #endregion
            
            #region Scripted Object: Build Volumes
            
            static ScriptedObjects<BuildAreaVolume> _BuildVolumes = null;
            public static ScriptedObjects<BuildAreaVolume> BuildVolumes
            {
                get
                {
                    if( _BuildVolumes == null )
                        _BuildVolumes = new ScriptedObjects<BuildAreaVolume>( CoreForms.ESM_ATC_ACTI_BuildAreaVolume );
                    return _BuildVolumes;
                }
            }
            
            #endregion
            
            #region Scripted Object: Sandbox Volumes
            
            static ScriptedObjects<Volume> _SandboxVolumes = null;
            public static ScriptedObjects<Volume> SandboxVolumes
            {
                get
                {
                    if( _SandboxVolumes == null )
                        _SandboxVolumes = new ScriptedObjects<Volume>( CoreForms.ESM_ATC_ACTI_SandboxVolume );
                    return _SandboxVolumes;
                }
            }
            
            #endregion
            
            #region Scripted Object: Border Enablers
            
            static ScriptedObjects<BorderEnabler> _BorderEnablers = null;
            public static ScriptedObjects<BorderEnabler> BorderEnablers
            {
                get
                {
                    if( _BorderEnablers == null )
                        _BorderEnablers = new ScriptedObjects<BorderEnabler>( CoreForms.ESM_ATC_ACTI_BorderEnabler );
                    return _BorderEnablers;
                }
            }
            
            #endregion
            
            #region Scripted Object: Sub-Division Edge Flags
            
            public static class EdgeFlags
            {
                
                static List<ScriptedObjects<EdgeFlag>> _Forms = null;
                public static List<ScriptedObjects<EdgeFlag>> Forms
                {
                    get
                    {
                        if( _Forms == null )
                        {
                            _Forms = new List<ScriptedObjects<EdgeFlag>>();
                            _Forms.Add( new ScriptedObjects<EdgeFlag>( CoreForms.ESM_ATC_STAT_SubDivisionEdgeFlag ) );
                            _Forms.Add( new ScriptedObjects<EdgeFlag>( CoreForms.ESM_ATC_STAT_SubDivisionEdgeFlag_ForcedZ ) );
                        }
                        return _Forms;
                    }
                }
                
                public static bool Load()
                {
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                        foreach( var form in forms )
                            if( !form.Load() ) return false;
                    return true;
                }
                
                public static bool PostLoad()
                {
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                        foreach( var form in forms )
                            if( !form.PostLoad() ) return false;
                    return true;
                }
                
                public static void Dispose()
                {
                    if( _Forms != null )
                    {
                        foreach( var form in _Forms )
                            if( form != null )
                                form.Dispose();
                        _Forms = null;
                    }
                }
                
                public static EdgeFlag Find( uint formid )
                {
                    EdgeFlag result = null;
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                    {
                        foreach( var form in forms )
                        {
                            result = form.Find( formid );
                            if( result != null )
                                return result;
                        }
                    }
                    return null;
                }
                
                public static EdgeFlag Find( string editorid )
                {
                    EdgeFlag result = null;
                    var forms = Forms;
                    if( !forms.NullOrEmpty() )
                    {
                        foreach( var form in forms )
                        {
                            result = form.Find( editorid );
                            if( result != null )
                                return result;
                        }
                    }
                    return null;
                }
                
                public enum Association
                {
                    Associated = 0,
                    Unassociated = 1
                }
                
                public static List<EdgeFlag> ByAssociation( List<EdgeFlag> flags, Association association )
                {
                    if( flags.NullOrEmpty() )
                        return null;
                    
                    var list = new List<EdgeFlag>();
                    foreach( var flag in flags )
                        if(
                            (
                                ( association == Association.Associated )&&
                                ( flag.kywdSubDivision.Count > 0 )
                            )||
                            (
                                ( association == Association.Unassociated )&&
                                ( flag.kywdSubDivision.Count < 1 )
                            )
                        )
                            list.Add( flag );
                    
                    return list.Count == 0
                        ? null
                        : list;
                }
                
                public static List<EdgeFlag> FindAllInWorldspace( string editorid )
                {
                    //var cWorldspaces = GodObject.Plugin.Data.Root.GetContainer<Engine.Plugin.Forms.Worldspace>( true, false );
                    //if( cWorldspaces == null )
                    //{
                    //    DebugLog.Write( "GodObject.Plugin.Data.EdgeFlags :: FindAllInWorldspace() :: Unable to get root container for Worldspaces!" );
                    //    return null;
                    //}
                    //var worldspace = cWorldspaces.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                    var worldspace = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Worldspace>( editorid );
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                
                public static List<EdgeFlag> FindAllInWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
                {
                    return worldspace == null
                        ? null
                        : FindAllInWorldspace( worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                
                public static List<EdgeFlag> FindAllInWorldspace( uint formid )
                {
                    var forms = Forms;
                    if( ( forms.NullOrEmpty() )||( !Engine.Plugin.Constant.ValidFormID( formid ) ) )
                        return null;
                    
                    var list = new List<EdgeFlag>();
                    
                    foreach( var form in forms )
                    {
                        var edgeRefs = form.FindAllInWorldspace( formid );
                        if( !edgeRefs.NullOrEmpty() )
                            list.AddAll( edgeRefs );
                    }
                    
                    return list.Count == 0
                        ? null
                        : list;
                }
                
            }
            
            #endregion
            
            #endregion
            
            #region Global enumeration of forms
            
            public static Engine.Plugin.PapyrusScript GetScriptByFormID( uint formid )
            {
                Engine.Plugin.PapyrusScript result = null;
                
                result = Workshops.Find( formid );
                if( result != null )
                    return result;
                
                result = Settlements.Find( formid );
                if( result != null )
                    return result;
                
                result = SubDivisions.Find( formid );
                if( result != null )
                    return result;
                
                result = BorderEnablers.Find( formid );
                if( result != null )
                    return result;
                
                result = BuildVolumes.Find( formid );
                if( result != null )
                    return result;
                
                result = SandboxVolumes.Find( formid );
                if( result != null )
                    return result;
                
                result = EdgeFlags.Find( formid );
                if( result != null )
                    return result;
                
                return null;
            }
            
            public static Engine.Plugin.PapyrusScript GetScriptByEditorID( string editorid )
            {
                Engine.Plugin.PapyrusScript result = null;
                
                result = Workshops.Find( editorid );
                if( result != null )
                    return result;
                
                result = Settlements.Find( editorid );
                if( result != null )
                    return result;
                
                result = SubDivisions.Find( editorid );
                if( result != null )
                    return result;
                
                result = BorderEnablers.Find( editorid );
                if( result != null )
                    return result;
                
                result = BuildVolumes.Find( editorid );
                if( result != null )
                    return result;
                
                result = SandboxVolumes.Find( editorid );
                if( result != null )
                    return result;
                
                result = EdgeFlags.Find( editorid );
                if( result != null )
                    return result;
                
                return null;
            }
            
            #region Old Code
            #if OLD_CODE
            
            public static Engine.Plugin.Form FindByFormID( uint formid )
            {
                /*
                DebugLog.Write( string.Format(
                    "\n{0} :: FindByFormID() :: 0x{1}",
                    "GodObject.Plugin.Data",
                    formid ) );
                */
                
                // Try god objects first
                foreach( var gob in GodObject.CoreForms.Forms )
                    if( gob.FormID == formid )
                        return gob;
                
                /*
                if( formid == lookfid )
                    DebugLog.Write( string.Format(
                        "\nGodObject.Plugin.Data.FindByFormID() :: 0x{0} :: {1}",
                        formid,
                        "Base Forms" ) );
                */
                
                var result = (Engine.Plugin.Form)null;
                
                // Look through locations
                if( Locations.Count > 0 )
                {
                    result = Locations.Find( formid );
                    if( result != null )
                        return result;
                }
                
                // Look through keywords
                if( Keywords.Count > 0 )
                {
                    result = Keywords.Find( formid );
                    if( result != null )
                        return result;
                }
                
                // Look through Static Objects
                if( Statics.Count > 0 )
                {
                    result = Statics.Find( formid );
                    if( result != null )
                        return result;
                }
                
                // Look through base forms
                result = BaseFormData.Find( formid );
                if( result != null )
                    return result;
                
                // Look through worldspaces, cells and object references
                if( Worldspaces.Count == 0 )
                    return null;
                
                /*
                if( formid == lookfid )
                    DebugLog.Write( string.Format(
                        "\nGodObject.Plugin.Data.FindByFormID() :: 0x{0} :: {1}",
                        formid,
                        "Script References" ) );
                */
                
                var script = GetScriptByFormID( formid );
                if( script != null )
                    return script.Reference;
                
                /*
                if( formid == lookfid )
                    DebugLog.Write( string.Format(
                        "\nGodObject.Plugin.Data.FindByFormID() :: 0x{0} :: {1}",
                        formid,
                        "Worldspaces" ) );
                */
                
                result = Worldspaces.Find( formid );
                if( result != null )
                    return result;
                
                /*
                if( formid == lookfid )
                    DebugLog.Write( string.Format(
                        "\nGodObject.Plugin.Data.FindByFormID() :: 0x{0} :: {1}",
                        formid,
                        "Cells and Object References" ) );
                */
                
                var wl = Worldspaces.ToList();
                var wc = wl.Count;
                for( int wi = 0; wi < wc; wi++ )
                {
                    // Is it a cell in a worldspace?
                    var w = wl[ wi ];
                    result = w.Cells.Find( formid ) as Engine.Plugin.Form;
                    if( result != null )
                        return result;
                    
                    var wcl = w.Cells.ToList();
                    var wcc = wcl.Count;
                    for( int wci = 0; wci < wcc; wci++ )
                    {
                        // Is it an object reference in a cell?
                        var c = wcl[ wci ] as Engine.Plugin.Forms.Cell;
                        result = c.ObjectReferences.Find( formid ) as Engine.Plugin.Form;
                        if( result != null )
                            return result;
                    }
                }
                
                /*
                if( formid == lookfid )
                    DebugLog.Write( string.Format(
                        "\nGodObject.Plugin.Data.FindByFormID() :: 0x{0} :: {1}",
                        formid,
                        "Nothing Found" ) );
                */
                
                return null;
            }
            
            public static Engine.Plugin.Form FindByEditorID( string editorid )
            {
                // Try god objects first
                foreach( var gob in GodObject.CoreForms.Forms )
                    if( gob.EditorID == editorid )
                        return gob;
                
                var result = (Engine.Plugin.Form)null;
                
                // Look through locations
                if( Locations.Count > 0 )
                {
                    result = Locations.Find( editorid );
                    if( result != null )
                        return result;
                }
                
                // Look through keywords
                if( Keywords.Count > 0 )
                {
                    result = Keywords.Find( editorid );
                    if( result != null )
                        return result;
                }
                
                // Look through Static Objects
                if( Statics.Count > 0 )
                {
                    result = Statics.Find( editorid );
                    if( result != null )
                        return result;
                }
                
                // Look through base forms
                result = BaseFormData.Find( editorid );
                if( result != null )
                    return result;
                
                // Look through worldspaces, cells and object references
                if( Worldspaces.Count == 0 )
                    return null;
                
                var script = GetScriptByEditorID( editorid );
                if( script != null )
                    return script.Reference;
                
                result = Worldspaces.Find( editorid );
                if( result != null )
                    return result;
                
                var wl = Worldspaces.ToList();
                var wc = wl.Count;
                for( int wi = 0; wi < wc; wi++ )
                {
                    // Is it a cell in a worldspace?
                    var w = wl[ wi ];
                    result = w.Cells.Find( editorid ) as Engine.Plugin.Form;
                    if( result != null )
                        return result;
                    
                    var wcl = w.Cells.ToList();
                    var wcc = wcl.Count;
                    for( int wci = 0; wci < wcc; wci++ )
                    {
                        // Is it an object reference in a cell?
                        var c = wcl[ wci ] as Engine.Plugin.Forms.Cell;
                        result = c.ObjectReferences.Find( editorid ) as Engine.Plugin.Form;
                        if( result != null )
                            return result;
                    }
                }
                
                return null;
            }
            
            #endif
            #endregion
            
            #endregion
            
        }
        
    }
    
}