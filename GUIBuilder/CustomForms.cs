/*
 * CustomForms.cs
 * 
 * Holds instance related Forms and other global user objects.
 * 
 * TODO:  Properly separate this into base class types
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Engine.Plugin.Forms;

using GUIBuilder.Windows;


namespace GUIBuilder
{
    public static class CustomForms
    {

        public static void SaveToWorkspace()
        {
            var w = GodObject.Plugin.Workspace;
            if( w == null ) return;

            #region Border Presets

            if( _CustomWorkshopPreset != null )
            {
                _CustomWorkshopPreset.onSerialize -= w.SerializeWorkshopPreset; // Remove then add to make sure it's only handled once
                _CustomWorkshopPreset.onSerialize += w.SerializeWorkshopPreset;
                w.SerializeWorkshopPreset( _CustomWorkshopPreset );
            }

            if( _CustomSubDivisionPreset != null )
            {
                _CustomSubDivisionPreset.onSerialize -= w.SerializeSubDivisionPreset; // Remove then add to make sure it's only handled once
                _CustomSubDivisionPreset.onSerialize += w.SerializeSubDivisionPreset;
                w.SerializeSubDivisionPreset( _CustomSubDivisionPreset );
            }

            #endregion

            #region Border Detection Forms

            if( _WorkshopBorderGeneratorKeyword != null )
                SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator, _WorkshopBorderGeneratorKeyword );

            if( _WorkshopBorderLinkKeyword != null )
                SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink, _WorkshopBorderLinkKeyword );

            if( _WorkshopTerrainFollowingMarker != null )
                SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing, _WorkshopTerrainFollowingMarker );

            if( _WorkshopForcedZMarker != null )
                SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ, _WorkshopForcedZMarker );

            if( _WorkshopBorderWithBottomRef != null )
                SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_LCRT_BorderWithBottom, _WorkshopBorderWithBottomRef );

            #endregion

            #region Workshop Forms

            if( _WorkshopWorkbenches != null )
                w.SetWorkshopWorkbenches( _WorkshopWorkbenches );

            #endregion

        }

        #region Custom EditorID Formats

        public static class EditorIDFormats
        {
            // TODO:  Move this region to a more appropriate parent static class

            const string                    XmlNode_Formats         = "EditorIDFormats";

            public const string             Token_ModPrefix         = "{mod}";
            public const string             Token_FormType          = "{type}";
            public const string             Token_Name              = "{name}";
            public const string             Token_Index             = "{index}";
            public const string             Token_Neighbour         = "{neighbour}";
            public const string             Token_SubIndex          = "{subindex}";

            public const string             Default_ModPrefix       = "MyMod";
        
            public const string             Default_Location        = "{mod}_{name}_Location";
            public const string             Default_EncounterZone   = "{mod}_{name}_EncounterZone";
            public const string             Default_Layer           = "{mod}_{name}";
            public const string             Default_Cells           = "{mod}_{name}_{index}";
            public const string             Default_WorkshopRef     = "{mod}_{name}_WorkshopRef";
            public const string             Default_BorderStatic    = "{mod}_{name}_WorkshopBorder";
            public const string             Default_BuildVolumes    = "{mod}_{name}_BuildAreaVolume_{index}";
            public const string             Default_SandboxVolume   = "{mod}_{name}_SandboxArea";
            public const string             Default_CenterMarker    = "{mod}_{name}_Center";
            
            public const string             ESM_ATC_Mod_Prefix      = "ESM_ATC";
            public static string            ESM_ATC_STAT_Border     = string.Format( "{0}_{1}_{2}_Border_{3}_{4}_{5}", Token_ModPrefix, Token_FormType, Token_Name, Token_Index, Token_Neighbour, Token_SubIndex );
            
            static string                   FromWorkspace( string xmlKey, string defaultValue )
            {
                var ws = GodObject.Plugin.Workspace;
                return ws == null
                    ? defaultValue
                    : ws.ReadValue<string>( XmlNode_Formats, xmlKey, defaultValue );
            }

            static void                     ToWorkspace( string xmlKey, string value, bool commit = false )
            {
                var ws = GodObject.Plugin.Workspace;
                if( ws == null ) return;
                ws.WriteValue<string>( XmlNode_Formats, xmlKey, value, commit );
            }

            public static string            FormatEditorID( string format, string modPrefix, string formType, string name, int index = -1, string neighbour = null, int subindex = -1 )
            {
                if( string.IsNullOrEmpty( format ) ) return null;

                string result = string.Copy( format );

                GenString.ReplaceToken( ref result, Token_ModPrefix , modPrefix );
                GenString.ReplaceToken( ref result, Token_FormType  , formType  );
                GenString.ReplaceToken( ref result, Token_Name      , name      );
                GenString.ReplaceToken( ref result, Token_Index     , ( index    < 0 ? null : index   .ToString( "D2" ) ) );
                GenString.ReplaceToken( ref result, Token_Neighbour , neighbour );
                GenString.ReplaceToken( ref result, Token_SubIndex  , ( subindex < 0 ? null : subindex.ToString( "D2" ) ) );

                return result;
            }

            public static string            ModPrefix
            {
                get { return FromWorkspace( "ModPrefix", Default_ModPrefix ); }
                set { ToWorkspace( "ModPrefix", value, true ); }
            }

            public static string            Location
            {
                get { return FromWorkspace( "Location", Default_Location ); }
                set { ToWorkspace( "Location", value, true ); }
            }
        
            public static string            EncounterZone
            {
                get { return FromWorkspace( "EncounterZone", Default_EncounterZone ); }
                set { ToWorkspace( "EncounterZone", value, true ); }
            }
        
            public static string            Layer
            {
                get { return FromWorkspace( "Layer", Default_Layer ); }
                set { ToWorkspace( "Layer", value, true ); }
            }
        
            public static string            Cells
            {
                get { return FromWorkspace( "Cells", Default_Cells ); }
                set { ToWorkspace( "Cells", value, true ); }
            }
        
            public static string            WorkshopRef
            {
                get { return FromWorkspace( "WorkshopRef", Default_WorkshopRef ); }
                set { ToWorkspace( "WorkshopRef", value, true ); }
            }
        
            public static string            BorderStatic
            {
                get { return FromWorkspace( "BorderStatic", Default_BorderStatic ); }
                set { ToWorkspace( "BorderStatic", value, true ); }
            }
        
            public static string            BuildVolumes
            {
                get { return FromWorkspace( "BuildVolumes", Default_BuildVolumes ); }
                set { ToWorkspace( "BuildVolumes", value, true ); }
            }
        
            public static string            SandboxVolume
            {
                get { return FromWorkspace( "SandboxVolume", Default_SandboxVolume ); }
                set { ToWorkspace( "SandboxVolume", value, true ); }
            }
        
            public static string            CenterMarker
            {
                get { return FromWorkspace( "CenterMarker", Default_CenterMarker ); }
                set { ToWorkspace( "CenterMarker", value, true ); }
            }
        
        }
        #endregion


        #region Custom NIFBuilder.Presets

        static NIFBuilder.Preset   _CustomWorkshopPreset = null;
        public static NIFBuilder.Preset CustomWorkshopPreset
        {
            get
            {
                if( _CustomWorkshopPreset == null )
                    _CustomWorkshopPreset = GodObject.Plugin.Workspace == null
                        ? new NIFBuilder.Preset( NIFBuilder.Preset.Workshop.Custom )
                        : GodObject.Plugin.Workspace.WorkshopPreset;
                return _CustomWorkshopPreset;
            }
        }

        static NIFBuilder.Preset   _CustomSubDivisionPreset = null;
        public static NIFBuilder.Preset CustomSubDivisionPreset
        {
            get
            {
                if( _CustomSubDivisionPreset == null )
                    _CustomSubDivisionPreset = GodObject.Plugin.Workspace == null
                        ? new NIFBuilder.Preset( NIFBuilder.Preset.SubDivision.Custom )
                        : GodObject.Plugin.Workspace.SubDivisionPreset;
                return _CustomSubDivisionPreset;
            }
        }

        #endregion


        #region Custom Base Engine.Plugin.Forms


        #region Generic Form Type Cache Handler

        public class CustomFormCache : IDisposable
        {
            readonly object Lock = new object();

            Type _FormType = null;
            List<Engine.Plugin.Form> _Pool = null;
            List<Engine.Plugin.Form> _LastFiltered = null;
            int _LastLoadOrderFilter = -1;
            bool loadedFromGlobal = false;
            Dictionary<string,Engine.Plugin.Form> _detectedSuffixForms;

            public Type FormType { get{ return _FormType;  } }

            public CustomFormCache( Type formType )
            {
                _FormType = formType;
            }

            #region Disposal

            protected bool                  Disposed = false;

            ~CustomFormCache()
            {
                Dispose( true );
            }

            public void Dispose()
            {
                Dispose( true );
            }

            protected virtual void Dispose( bool disposing )
            {
                lock( Lock )
                {
                    if( Disposed )
                        return;
                    Disposed = true;
                    _Pool = null;
                    _LastFiltered = null;
                    _LastLoadOrderFilter = -1;
                    loadedFromGlobal = false;
                    _detectedSuffixForms = null;
                    _FormType = null;
                }
            }

            #endregion


            public static List<TForm> ToListTForm<TForm>( List<Engine.Plugin.Form> list ) where TForm : Engine.Plugin.Form
            {
                return list.NullOrEmpty()
                    ? null
                    : list.ConvertAll( f => f as TForm ).ToList();
            }


            internal void LoadDetectionForms( ThreadParams parameters, string[] detectionSuffix, bool globalLoadOnAnyFail = true )
            {
                lock( Lock )
                {
                    if( Disposed )
                        return;

                    if( parameters.LogUpdate )
                    {
                        var s = "";
                        if( !detectionSuffix.NullOrEmpty() )
                        {
                            foreach( var suff in detectionSuffix )
                            {
                                if( !string.IsNullOrEmpty( suff ) )
                                {
                                    if( !string.IsNullOrEmpty( s ) ) s += ", ";
                                    s += "\"" + suff + "\"";
                                }
                            }
                            if( !string.IsNullOrEmpty( s ) ) s = "[" + s + "]";
                        }
                        DebugLog.OpenIndentLevel( s );
                    }

                    _Pool = _Pool ?? new List<Engine.Plugin.Form>();
                    bool loadGlobal = detectionSuffix.NullOrEmpty();
                    bool sortForms = false;

                    if( !loadGlobal )
                    {
                        var ws = GodObject.Plugin.Workspace;
                        if( ws == null )
                            loadGlobal = globalLoadOnAnyFail;
                        else
                        {
                            foreach( var suffix in detectionSuffix )
                            {
                                // Check if there is a workspace that defines the control form
                                if( !string.IsNullOrEmpty( suffix ) )
                                {
                                    var wsForm = ws.GetIdentifierForm( _FormType, suffix );
                                    if( wsForm != null )
                                        sortForms |= _Pool.AddOnce( wsForm );
                                    else
                                    {
                                        loadGlobal = globalLoadOnAnyFail;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if( ( loadGlobal ) && ( !loadedFromGlobal ) )
                    {
                        loadedFromGlobal = true;
                        var cForms = GodObject.Plugin.Data.Root.GetCollection( _FormType, true, true, false ).ToList<Engine.Plugin.Form>();
                        sortForms |= _Pool.AddOnce( cForms );
                    }
                    if( sortForms )
                        _Pool.Sort( ( x, y ) => ( x.GetFormID( Engine.Plugin.TargetHandle.Master ) < y.GetFormID( Engine.Plugin.TargetHandle.Master ) ? -1 : 1 ) );

                    if( parameters.LogUpdate )
                    {
                        //DebugLog.WriteIDStrings( "_Pool", _Pool, true, true );
                        DebugLog.CloseIndentLevel();
                    }
                }
            }


            internal List<TForm> FilteredList<TForm>( int loadOrderFilter ) where TForm : Engine.Plugin.Form
            {
                var list = FilteredList( loadOrderFilter );
                return ToListTForm<TForm>( list );
            }

            internal List<Engine.Plugin.Form> FilteredList( int loadOrderFilter )
            {
                lock( Lock )
                {
                    if( _Pool.NullOrEmpty() )
                        return null;

                    List<Engine.Plugin.Form> result = null;

                    if( loadOrderFilter == -1 )
                        result = _Pool;

                    else if( loadOrderFilter == _LastLoadOrderFilter )
                        result = _LastFiltered;

                    else
                    {
                        result = _Pool.Where( ( x ) => ( x.LoadOrder == loadOrderFilter ) ).ToList();
                        result.Sort( ( x, y ) => ( x.GetFormID( Engine.Plugin.TargetHandle.Master ) < y.GetFormID( Engine.Plugin.TargetHandle.Master ) ? -1 : 1 ) );
                        _LastLoadOrderFilter = loadOrderFilter;
                        _LastFiltered = result;
                    }

                    return result;
                }
            }


            public TForm FindDetectionFormBySuffixInPool<TForm>( string detectionSuffix, int loadOrderFilter = -1, int startIndex = 0 ) where TForm : Engine.Plugin.Form
            {
                return FindDetectionFormBySuffixInPool( detectionSuffix, loadOrderFilter, startIndex ) as TForm;
            }

            public Engine.Plugin.Form FindDetectionFormBySuffixInPool( string detectionSuffix, int loadOrderFilter = -1, int startIndex = 0 )
            {
                lock( Lock )
                {
                    var lSuffix = detectionSuffix.ToLower();
                    Engine.Plugin.Form result = null;

                    bool doFilter = ( loadOrderFilter >= 0 )&&( loadOrderFilter < GodObject.Plugin.Data.Files.Loaded.Count );
                    if( !doFilter )
                    {
                        _detectedSuffixForms = _detectedSuffixForms ?? new Dictionary<string, Engine.Plugin.Form>();
                        if( _detectedSuffixForms.TryGetValue( lSuffix, out result ) )
                            return result;
                    }

                    var list = FilteredList( loadOrderFilter );
                    if( list.NullOrEmpty() ) return result;

                    if( startIndex < 0 ) startIndex = 0;
                    if( startIndex >= list.Count ) return result;

                    for( int i = startIndex; i < list.Count; i++ )
                    {
                        var form = list[ i ];
                        var fEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        if( !string.IsNullOrEmpty( fEDID ) )
                        {
                            var lcase = fEDID.ToLower();
                            if( lcase.EndsWith( lSuffix ) )
                            {
                                result = form;
                                break;
                            }
                        }
                    }
                    if( ( result != null ) && ( doFilter ) )
                        _detectedSuffixForms[ lSuffix ] = result;

                    return result;
                }
            }

            public List<TForm> FindDetectionFormsByWildcardsInPool<TForm>( string[] wildcards, int loadOrderFilter = -1 ) where TForm : Engine.Plugin.Form
            {
                var list = FindDetectionFormsByWildcardsInPool( wildcards, loadOrderFilter );
                return ToListTForm<TForm>( list );
            }
            
            public List<Engine.Plugin.Form> FindDetectionFormsByWildcardsInPool( string[] wildcards, int loadOrderFilter = -1 )
            {
                lock( Lock )
                {
                    List<Engine.Plugin.Form> results = null;

                    var list = FilteredList( loadOrderFilter );
                    if( list.NullOrEmpty() ) return results;

                    if( wildcards.NullOrEmpty() ) return list;

                    for( int i = 0; i < list.Count; i++ )
                    {
                        var form = list[ i ];
                        var fEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        if( !string.IsNullOrEmpty( fEDID ) )
                            if( fEDID.CountKeysInString( wildcards, StringComparison.InvariantCultureIgnoreCase ) > 0 )
                                results.AddOnce( form );
                    }

                    return results.NullOrEmpty() ? null : results;
                }
            }

            public List<TForm> FindDetectionFormsByWildcardInPool<TForm>( string wildcard, int loadOrderFilter = -1 ) where TForm : Engine.Plugin.Form
            {
                var list = FindDetectionFormsByWildcardInPool( wildcard, loadOrderFilter );
                var result = ToListTForm<TForm>( list );
                return result;
            }
            
            public List<Engine.Plugin.Form> FindDetectionFormsByWildcardInPool( string wildcard, int loadOrderFilter = -1 )
            {
                lock( Lock )
                {
                    List<Engine.Plugin.Form> results = null;

                    var list = FilteredList( loadOrderFilter );
                    if( list.NullOrEmpty() )
                        goto localAbort;

                    if( string.IsNullOrEmpty( wildcard ) )
                    {
                        results = list;
                        goto localAbort;
                    }

                    results = new List<Engine.Plugin.Form>();
                    for( int i = 0; i < list.Count; i++ )
                    {
                        var form = list[ i ];
                        var fEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        if( !string.IsNullOrEmpty( fEDID ) )
                        {
                            if( fEDID.IndexOf( wildcard, StringComparison.InvariantCultureIgnoreCase ) >= 0 )
                                results.AddOnce( form );
                        }
                    }
                    
                localAbort:
                    return results.NullOrEmpty() ? null : results;
                }
            }

        }

        #endregion


        #region Workshop Form Pools

        #region Pools

        //static CustomFormCache<Keyword> _KeywordPool;
        //static CustomFormCache<Static> _StaticPool;
        //static CustomFormCache<LocationRef> _LocationRefPool;
        //static CustomFormCache<Container> _ContainerPool;

        static object _CacheLock = new object();
        static List<CustomFormCache> _CustomFormCaches;

        internal static CustomFormCache GetCustomFormCache<TForm>() where TForm : Engine.Plugin.Form
        {
            return GetCustomFormCache( typeof( TForm ) );
        }

        internal static CustomFormCache GetCustomFormCache( Type formType )
        {
            lock( _CacheLock )
                return GetCustomFormCacheEx( formType );
        }

        /// <summary>
        /// Must hold _CacheLock before entry!
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        private static CustomFormCache GetCustomFormCacheEx( Type formType )
        {
            var caches = _CustomFormCaches ?? new List<CustomFormCache>();
            if( _CustomFormCaches != caches )
                _CustomFormCaches = caches;

            var result = caches.Find( c => c.FormType == formType );
            if( result == null )
            {
                result = new CustomFormCache( formType );
                caches.Add( result );
            }

            return result;
        }

        /*
        internal static CustomFormCache<Keyword> KeywordPool
        {
            get
            {
                if( _KeywordPool == null )
                    _KeywordPool = new CustomFormCache<Keyword>();
                return _KeywordPool;
            }
        }

        internal static CustomFormCache<Static> StaticPool
        {
            get
            {
                if( _StaticPool == null )
                    _StaticPool = new CustomFormCache<Static>();
                return _StaticPool;
            }
        }

        internal static CustomFormCache<LocationRef> LocationRefPool
        {
            get
            {
                if( _LocationRefPool == null )
                    _LocationRefPool = new CustomFormCache<LocationRef>();
                return _LocationRefPool;
            }
        }

        internal static CustomFormCache<Container> ContainerPool
        {
            get
            {
                if( _ContainerPool == null )
                    _ContainerPool = new CustomFormCache<Container>();
                return _ContainerPool;
            }
        }
        */

        #endregion

        #region Disposal

        public static void Dispose()
        {
            lock( _CacheLock )
            {
                var caches = _CustomFormCaches;
                if( !caches.NullOrEmpty() )
                    foreach( var cache in caches )
                        cache.Dispose();

                _CustomFormCaches = null;
                _WorkshopBorderGeneratorKeyword = null;
                _WorkshopBorderLinkKeyword = null;
                _WorkshopTerrainFollowingMarker = null;
                _WorkshopForcedZMarker = null;
                _WorkshopBorderWithBottomRef = null;
                _WorkshopWorkbenches = null;
            }
        }

        /*
        internal static void DisposeOfCache<TForm>( ref CustomFormCache<TForm> cache ) where TForm : Engine.Plugin.Form
        {
            if( cache != null )
                cache.Dispose();
            cache = null;
        }
        */
        
        #endregion

        #endregion


        #region Workshop Border Node Detection Forms

        static Keyword _WorkshopBorderGeneratorKeyword = null;
        public static Keyword WorkshopBorderGeneratorKeyword
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopBorderGeneratorKeyword == null )
                        _WorkshopBorderGeneratorKeyword = GetDetectionFormFromWorkspaceOrPoolBySuffixEx<Keyword>( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator );
                    return _WorkshopBorderGeneratorKeyword;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopBorderGeneratorKeyword = value;
                    SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator, value );
                }
            }
        }

        static Keyword _WorkshopBorderLinkKeyword = null;
        public static Keyword WorkshopBorderLinkKeyword
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopBorderLinkKeyword == null )
                        _WorkshopBorderLinkKeyword = GetDetectionFormFromWorkspaceOrPoolBySuffixEx<Keyword>( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink );
                    return _WorkshopBorderLinkKeyword;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopBorderLinkKeyword = value;
                    SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink, value );
                }
            }
        }

        static Static _WorkshopTerrainFollowingMarker = null;
        public static Static WorkshopTerrainFollowingMarker
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopTerrainFollowingMarker == null )
                        _WorkshopTerrainFollowingMarker = GetDetectionFormFromWorkspaceOrPoolBySuffix<Static>( GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing );
                    return _WorkshopTerrainFollowingMarker;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopTerrainFollowingMarker = value;
                    SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing, value );
                }
            }
        }

        static Static _WorkshopForcedZMarker = null;
        public static Static WorkshopForcedZMarker
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopForcedZMarker == null )
                        _WorkshopForcedZMarker = GetDetectionFormFromWorkspaceOrPoolBySuffixEx<Static>( GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ );
                    return _WorkshopForcedZMarker;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopForcedZMarker = value;
                    SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ, value );
                }
            }
        }

        static LocationRef _WorkshopBorderWithBottomRef = null;
        public static LocationRef WorkshopBorderWithBottomRef
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopBorderWithBottomRef == null )
                        _WorkshopBorderWithBottomRef = GetDetectionFormFromWorkspaceOrPoolBySuffixEx<LocationRef>( GUIBuilder.WorkshopBatch.WSDS_LCRT_BorderWithBottom );
                    return _WorkshopBorderWithBottomRef;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopBorderWithBottomRef = value;
                    SaveDetectionFormToWorkspaceBySuffix( GUIBuilder.WorkshopBatch.WSDS_LCRT_BorderWithBottom, value );
                }
            }
        }

        #endregion



        #region Workshop Forms

        static List<Container> _WorkshopWorkbenches = null;
        public static List<Container> WorkshopWorkbenches
        {
            get
            {
                lock( _CacheLock )
                {
                    if( _WorkshopWorkbenches == null )
                    {
                        if( GodObject.Plugin.Workspace != null )
                        {
                            var wfids = GodObject.Plugin.Workspace.WorkshopWorkbenches;
                            if( !wfids.NullOrEmpty() )
                            {
                                var loadedPlugins = GodObject.Plugin.Data.Files.Loaded.Select( x => x.Filename ).ToList();
                                var list = new List<Container>();
                                foreach( var wfid in wfids )
                                {
                                    if( Engine.Plugin.Constant.ValidFormID( wfid.FormID ) && wfid.Filename.InsensitiveInvariantMatch( loadedPlugins ) )
                                    {
                                        Engine.Plugin.Forms.Container customWorkshop;
                                        GodObject.CoreForms.TryAddCustomWorkshopWorkbench( wfid.FormID, wfid.Filename, out customWorkshop );
                                        if( customWorkshop != null )
                                            list.Add( customWorkshop );
                                    }
                                }
                                if( !list.NullOrEmpty() )
                                    _WorkshopWorkbenches = list;
                            }
                        }
                    }
                    return _WorkshopWorkbenches;
                }
            }
            set
            {
                lock( _CacheLock )
                {
                    _WorkshopWorkbenches = value;
                    GodObject.Plugin.Workspace?.SetWorkshopWorkbenches( value );
                    GodObject.CoreForms.ClearWorkshopWorkbenches();
                    if( value.NullOrEmpty() ) return;
                    foreach( var form in value )
                    {
                        Engine.Plugin.Forms.Container customWorkshop;
                        GodObject.CoreForms.TryAddCustomWorkshopWorkbench( form.GetFormID( Engine.Plugin.TargetHandle.Master ) & 0x00FFFFFF, form.MasterHandle.Filename, out customWorkshop );
                    }
                }
            }
        }


        public static List<Container> GetFilteredWorkshopWorkbenches( int loadOrderFilter = -1 )
        {
            return GetCustomFormCache<Container>()?.FilteredList<Container>( loadOrderFilter );
        }

        public static List<Container> FindWorkshopWorkbenches( string wildcard, int loadOrderFilter = -1 )
        {
            return GetCustomFormCache<Container>()?.FindDetectionFormsByWildcardInPool<Container>( wildcard, loadOrderFilter );
        }

        #endregion


        #region Detection Forms

        public static TForm FormFromComboBox<TForm>( ComboBox control ) where TForm : Engine.Plugin.Form
        {
            if( control == null ) return null;
            if( control.InvokeRequired )
                throw new MethodAccessException( "Must call GUIBuilder.CustomForms.FormFromComboBox<TForm>() from the UI thread!" );

            if( control.SelectedIndex < 1 ) return null;

            TForm result = null;

            var formIDSubStr = control.Text.Substring( 2, 8 );
            uint formID;
            if( !uint.TryParse( formIDSubStr, System.Globalization.NumberStyles.HexNumber, null, out formID ) ) return null;

            result = GodObject.Plugin.Data.Root.Find<TForm>( formID );

            return result;
        }

        public static bool SaveDetectionFormToWorkspaceBySuffix<TForm>( string detectionSuffix, TForm form ) where TForm : Engine.Plugin.Form
        {
            if( string.IsNullOrEmpty( detectionSuffix ) )
                return false;

            var ws = GodObject.Plugin.Workspace;
            if( ws == null ) return false;

            var result = false;
            if( form == null )
            {
                ws.RemoveNode( detectionSuffix );
                result = true;
            }
            else
                result = ws.SetFormIdentifier( detectionSuffix, form.MasterHandle.Filename, form.GetFormID( Engine.Plugin.TargetHandle.Master ), false );

            // Commit the change
            ws.Commit();

            return result;
        }

        public static TForm GetDetectionFormFromWorkspaceOrPoolBySuffix<TForm>( string detectionSuffix, int loadOrderFilter = -1 ) where TForm : Engine.Plugin.Form
        {
            return GetDetectionFormFromWorkspaceOrPoolBySuffixEx( typeof( TForm ), detectionSuffix, loadOrderFilter ) as TForm;
        }

        public static Engine.Plugin.Form GetDetectionFormFromWorkspaceOrPoolBySuffix( Type formType, string detectionSuffix, int loadOrderFilter = -1 )
        {
            return GetDetectionFormFromWorkspaceOrPoolBySuffixEx( formType, detectionSuffix, loadOrderFilter );
        }

        /// <summary>
        /// Must NOT hold _CacheLock before entry!
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <param name="detectionSuffix"></param>
        /// <param name="loadOrderFilter"></param>
        /// <returns></returns>
        private static TForm GetDetectionFormFromWorkspaceOrPoolBySuffixEx<TForm>( string detectionSuffix, int loadOrderFilter = -1 ) where TForm : Engine.Plugin.Form
        {
            return GetDetectionFormFromWorkspaceOrPoolBySuffixEx( typeof( TForm ), detectionSuffix, loadOrderFilter ) as TForm;
        }

        /// <summary>
        /// Must NOT hold _CacheLock before entry!
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="detectionSuffix"></param>
        /// <param name="loadOrderFilter"></param>
        /// <returns></returns>
        private static Engine.Plugin.Form GetDetectionFormFromWorkspaceOrPoolBySuffixEx( Type formType, string detectionSuffix, int loadOrderFilter = -1 )
        {
            Engine.Plugin.Form result = null;

            if( string.IsNullOrEmpty( detectionSuffix ) ) return result;

            CustomFormCache cache;
            lock( _CacheLock )
                cache = GetCustomFormCache( formType );

            if( cache == null )
                return result;

            // Try from Workspace
            result = GodObject.Plugin.Workspace?.GetIdentifierForm( formType, detectionSuffix );
            if( result != null )
                return result;

            // Look through the [filtered] pool of forms

            // Prefix detection suffix with an underscore
            var editorIDSuffix = detectionSuffix;
            if( editorIDSuffix[ 0 ] != '_' )
                editorIDSuffix = "_" + editorIDSuffix;

            // Look through the pool for the form with the suffix
            result = cache.FindDetectionFormBySuffixInPool( editorIDSuffix, loadOrderFilter );

            // Try save it to the Workspace
            if( result != null )
                SaveDetectionFormToWorkspaceBySuffix( detectionSuffix, result );

            return result;
        }

        public static List<TForm> GetDetectionFormsFromPoolByWildcard<TForm>( string wildcard, int loadOrderFilter = -1 ) where TForm : Engine.Plugin.Form
        {
            return CustomFormCache.ToListTForm<TForm>( GetDetectionFormsFromPoolByWildcard( typeof( TForm ), wildcard, loadOrderFilter ) );
        }

        public static List<Engine.Plugin.Form> GetDetectionFormsFromPoolByWildcard( Type formType, string wildcard, int loadOrderFilter = -1 )
        {
            List<Engine.Plugin.Form> results = null;

            var cache = GetCustomFormCache( formType );
            if( cache == null )
                return results;

            // Look through the [filtered] pool for the form with the wildcard
            results = cache.FindDetectionFormsByWildcardInPool( wildcard, loadOrderFilter );

            return results;
        }

        #endregion



        #endregion


        #region Main Thread Parameters


        internal abstract class ThreadParams
        {

            public readonly IEnableControlForm  Form;
            public readonly int                 LoadOrderFilter;
            public readonly bool                UpdateStatusBar;
            public readonly bool                TimeThread;
            public readonly bool                LogUpdate;

            readonly Action                     ThreadOnStart;

            private WorkerThreadPool.WorkerThread _Thread = null;

            private System.Reflection.MethodBase _THREAD_Start = null;

            Main                                _MainWindow = null;
            public Main MainWindow
            {
                get
                {
                    if( _MainWindow == null )
                        _MainWindow = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                    return _MainWindow;
                }
            }

            public ThreadParams(
                IEnableControlForm form,
                int loadOrderFilter,
                Action threadOnStart,
                WorkerThreadPool.ThreadOnFinish onFinishedCallback,
                bool updateStatusBar,
                bool timeThread,
                bool logUpdate,
                string nameSuffix
            )
            {
                Form = form;
                LoadOrderFilter = loadOrderFilter;
                ThreadOnStart = threadOnStart;
                UpdateStatusBar = updateStatusBar;
                TimeThread = timeThread;
                LogUpdate = logUpdate;

                _THREAD_Start = this.GetType().GetMethodBase( "THREAD_Start" );

                _Thread = WorkerThreadPool.CreateWorker(
                    THREAD_Init,
                    onFinishedCallback,
                    _THREAD_Start,
                    nameSuffix );
            }

            public virtual string LogParams()
            {
                return "Form = " + Form.TypeFullName() +
                    "\nLoadOrderFilter = 0x" + LoadOrderFilter.ToString( "X2" ) +
                    "\nUpdateStatusBar = " + ( UpdateStatusBar ? "true" : "false" ) +
                    "\nTimeThread = " + ( TimeThread ? "true" : "false" ) +
                    "\nLogUpdate = " + ( LogUpdate ? "true" : "false" );
            }

            public bool Start()
            {
                return _Thread == null ? false : _Thread.Start();
            }

            public void THREAD_Init()
            {

                #region Thread Init

                if( LogUpdate )
                {
                    DebugLog.OpenIndentLevel( false );
                    DebugLog.WriteLine( LogParams() );
                    DebugLog.CloseIndentLevel();
                }

                TimeSpan tStart = default;
                if( UpdateStatusBar )
                {
                    MainWindow.PushStatusMessage();
                    if( TimeThread )
                    {
                        MainWindow.StartSyncTimer();
                        tStart = MainWindow.SyncTimerElapsed();
                    }
                }

                #endregion

                // Invoke thread loop initializer
                ThreadOnStart?.Invoke();

                DebugLog.OpenIndentLevel( this.TypeFullName() + ".Start()", false );

                THREAD_Start();
                
                DebugLog.CloseIndentLevel();

                #region Thread Denit

                if( UpdateStatusBar )
                {
                    if( TimeThread )
                        MainWindow.StopSyncTimer( tStart );
                    MainWindow.PopStatusMessage();
                }

                #endregion
            }


            /// <summary>
            /// This is where the actual thread function goes
            /// </summary>
            protected abstract void THREAD_Start();
            
        }


        #region Thread Functions


        // Thread Safe
        static void ComboBox_Items_Add_Filtered_SelectedIndex_Set_BySuffix<TForm>( ThreadParams parameters, ComboBox control, string detectionSuffix ) where TForm : Engine.Plugin.Form
        {
            ComboBox_Items_Add_Filtered_SelectedIndex_Set_BySuffix( parameters, typeof( TForm ), control, detectionSuffix );
        }

        static void ComboBox_Items_Add_Filtered_SelectedIndex_Set_BySuffix( ThreadParams parameters, Type formType, ComboBox control, string detectionSuffix )
        {
            if( control == null ) return;

            var cache = GetCustomFormCache( formType );
            if( cache == null ) return;

            if( parameters.LogUpdate )
                DebugLog.OpenIndentLevel( "detectionSuffix = \"" + detectionSuffix + "\"" );

            THREAD_UI_INVOKE_ComboBox_Items_Clear( parameters, control, true );

            var list = cache.FilteredList( parameters.LoadOrderFilter );

            foreach( var form in list )
                THREAD_UI_INVOKE_ComboBox_Items_Add( parameters, control, form.IDString );

            // Try to find the "default" form
            Get_ComboBox_SelectedIndex_Set_BySuffix( parameters, control, list, "_" + detectionSuffix );

            if( parameters.LogUpdate )
                DebugLog.CloseIndentLevel();
        }


        // Thread Safe
        static TForm Get_ComboBox_SelectedIndex_Set_BySuffix<TForm>( ThreadParams parameters, ComboBox control, List<TForm> forms, string suffix ) where TForm : Engine.Plugin.Form, Engine.Plugin.Interface.IXHandle
        {
            TForm result = null;
            if( parameters.LogUpdate )
                DebugLog.OpenIndentLevel( "suffix = \"" + suffix + "\"" );

            if( control == null ) goto localAbort;
            if( forms.NullOrEmpty() ) goto localAbort;
            if( string.IsNullOrEmpty( suffix ) ) goto localAbort;

            var lSuffix = suffix.ToLower();
            var sIndex = 0; // Default "None"

            // Try to find the "default" form
            for( int i = 0; i < forms.Count; i++ )
            {
                var form = forms[ i ];
                var fEDID = form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                //DebugLog.Write( string.Format( "\t{0} - \"{1}\" - 0x{2} \"{3}\"", i, ( form == null ? "null" : form.Signature ), ( form == null ? 0 : form.FormID ).ToString( "X8" ), fEDID ) );
                if( !string.IsNullOrEmpty( fEDID ) )
                {
                    var lcase = fEDID.ToLower();
                    if( lcase.EndsWith( lSuffix ) )
                    {
                        result = form;
                        sIndex = 1 + i; // Skip "None"
                        break;
                    }
                }
            }
            THREAD_UI_INVOKE_ComboBox_SelectedIndex_Set( parameters, control, sIndex );

        localAbort:
            if( parameters.LogUpdate )
                DebugLog.CloseIndentLevel( "result", result?.IDString );
            return result;
        }


        // If not calling from the UI Thread, UI Thread must not be blocked
        static void SyncedListView_Items_Add_Selected_Set<TForm>( ThreadParams parameters, GUIBuilder.Windows.Controls.SyncedListView<TForm> control, List<TForm> forms, List<TForm> selectedForms ) where TForm : Engine.Plugin.Form
        {
            if( parameters.LogUpdate )
            {
                DebugLog.OpenIndentLevel();
                DebugLog.WriteIDStrings( "forms", forms, false, true );
                DebugLog.WriteIDStrings( "selectedForms", selectedForms, false, true );
            }

            if( forms.NullOrEmpty() )
                control.SyncObjects = null;
            else
            {
                control.SyncObjects = forms;
                control.UpdateListViewCheckSelection( selectedForms, true, true );
            }

            if( parameters.LogUpdate )
                DebugLog.CloseIndentLevel();
        }

        #endregion


        #region UI Cross-Thread Functions

        static void THREAD_UI_INVOKE_ComboBox_Items_Clear( ThreadParams parameters, ComboBox control, bool addNoneItem )
        {
            if( control == null ) return;
            if( parameters.Form.InvokeRequired )
            {
                parameters.Form.Invoke( (Action)delegate () { THREAD_UI_INVOKE_ComboBox_Items_Clear( parameters, control, addNoneItem ); }, null );
                return;
            }

            control.Items.Clear();
            if( addNoneItem )
                control.Items.Add( string.Format( " [{0}] ", "DropdownSelectNone".Translate() ) );
        }

        static void THREAD_UI_INVOKE_ComboBox_Items_Add( ThreadParams parameters, ComboBox control, string text )
        {
            if( control == null ) return;
            if( string.IsNullOrEmpty( text ) ) return;
            if( parameters.Form.InvokeRequired )
            {
                parameters.Form.Invoke( (Action)delegate () { THREAD_UI_INVOKE_ComboBox_Items_Add( parameters, control, text ); }, null );
                return;
            }

            control.Items.Add( text );
        }

        static void THREAD_UI_INVOKE_ComboBox_SelectedIndex_Set( ThreadParams parameters, ComboBox control, int index )
        {
            if( control == null ) return;
            if( parameters.Form.InvokeRequired )
            {
                parameters.Form.Invoke( (Action)delegate () { THREAD_UI_INVOKE_ComboBox_SelectedIndex_Set( parameters, control, index ); }, null );
                return;
            }

            try
            {
                if( ( control.Items == null ) || ( control.Items.Count == 0 ) )
                {
                    DebugLog.WriteWarning( "control.Items == null OR control.Items.Count == 0" );
                    return;
                }
                control.SelectedIndex =
                    ( index < 0 ) || ( index >= control.Items.Count )
                    ? 0
                    : index;
            }
            catch( Exception e )
            {
                DebugLog.WriteException( e, "index = " + index.ToString() );
                //DebugLog.WriteError( "Exception occured on index = " + index + "\n" + e.ToString() );
            }
        }

        #endregion

        #endregion



        #region ComboBox Population (w/Form Detection) Thread Management

        internal class ComboBoxPopulation : ThreadParams
        {

            #region Parameters
            
            public readonly ComboBox[]          ComboBoxes;
            public readonly string[]            Suffixes;
            public readonly Type                FormType;

            #endregion

            #region Constructors

            public ComboBoxPopulation(
                IEnableControlForm form,
                ComboBox[] comboBoxes,
                string[] suffixes,
                Type formType,
                int filter,
                Action threadOnStart,
                WorkerThreadPool.ThreadOnFinish onFinishedCallback,
                bool updateStatusBar,
                bool timeThread,
                bool logUpdate,
                string nameSuffix
            ) : base( form, filter, threadOnStart, onFinishedCallback, updateStatusBar, timeThread, logUpdate, nameSuffix )
            {
                ComboBoxes = comboBoxes;
                Suffixes = suffixes;
                FormType = formType;
            }

            #endregion


            public override string LogParams()
            {
                var s = base.LogParams() + "\nFormType = " + FormType.FullName();
                var c = GetComboBoxCount();
                for( var i = 0; i < c; i++ )
                    s += "\n\"Suffix\" -> ComboBox:: [ " + i + " ] :: \"" + GetSuffix( i ) + "\" -> " + GetComboBox( i ).ToStringNullSafe();
                return s;
            }


            protected override void THREAD_Start()
            {
                var cache = GetCustomFormCache( FormType );
                if( cache != null )
                {
                    cache.LoadDetectionForms( this, Suffixes );

                    var c = GetComboBoxCount();
                    for( int i = 0; i < c; i++ )
                        ComboBox_Items_Add_Filtered_SelectedIndex_Set_BySuffix(
                            this,
                            FormType,
                            GetComboBox( i ),
                            GetSuffix( i ) );
                }
            }


            #region Internal

            int GetComboBoxCount()
            {
                return ComboBoxes.NullOrEmpty()
                    ? 0
                    : ComboBoxes.Length;
            }

            ComboBox GetComboBox( int index )
            {
                if( ComboBoxes.NullOrEmpty() ) return null;
                if( ( index < 0 ) || ( index > ComboBoxes.Length ) ) return null;
                return ComboBoxes[ index ];
            }

            string GetSuffix( int index )
            {
                if( Suffixes.NullOrEmpty() ) return null;
                if( ( index < 0 ) || ( index > Suffixes.Length ) ) return null;
                return Suffixes[ index ];
            }

            #endregion

        }

        #region Thread-safe Start

        public static bool StartComboBoxRepopulationThread(
            IEnableControlForm form,
            ComboBox[] comboBoxes,
            string[] suffixes,
            Type formType,
            int filter,
            Action threadOnStart,
            WorkerThreadPool.ThreadOnFinish onFinishedCallback = null,
            bool updateStatusBar = false,
            bool timeThread = false,
            bool logUpdate = true
        )
        {
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( formType );
            if( association == null ) return false;
            var parameters = new ComboBoxPopulation(
                form,
                comboBoxes,
                suffixes,
                formType,
                filter,
                threadOnStart,
                onFinishedCallback,
                updateStatusBar,
                timeThread,
                logUpdate,
                association.Signature
            );
            if( parameters == null ) return false;
            return parameters.Start();
        }

        #endregion

        #endregion



        #region Workshop Workbench and Control Population Thread Management

        internal class WorkshopContainers : ThreadParams
        {

            #region Parameters
            
            public readonly GUIBuilder.Windows.Controls.SyncedListView<Container> ContainerSyncedListView;
            public readonly List<Container> SelectedContainers;
            public readonly string Wildcard;

            #endregion

            #region Constructors

            public WorkshopContainers(
                IEnableControlForm form,
                GUIBuilder.Windows.Controls.SyncedListView<Container> containerSyncedListView,
                List<Container> selectedContainers,
                string wildcard,
                int filter,
                Action threadOnStart,
                WorkerThreadPool.ThreadOnFinish onFinishedCallback,
                bool updateStatusBar,
                bool timeThread,
                bool logUpdate
            ) : base(
                form, filter,
                threadOnStart, onFinishedCallback,
                updateStatusBar, timeThread, logUpdate,
                Engine.Plugin.Attributes.Reflection.AssociationFrom( typeof( Container ) )?.Signature )
            {
                ContainerSyncedListView = containerSyncedListView;
                SelectedContainers = selectedContainers.Clone();
                Wildcard = wildcard;
            }

            #endregion


            public override string LogParams()
            {
                return base.LogParams() +
                    "\nContainerSyncedListView = " + ContainerSyncedListView.ToStringNullSafe() +
                    "\nSelectedContainers = " + SelectedContainers.NullSafeIDStrings() +
                    "\nWildcard = \"" + Wildcard + "\"";
            }


            protected override void THREAD_Start()
            {
                var cache = GetCustomFormCache<Container>();
                if( cache != null )
                {
                    cache.LoadDetectionForms( this, null );

                    SyncedListView_Items_Add_Selected_Set(
                        this,
                        this.ContainerSyncedListView,
                        cache.FindDetectionFormsByWildcardInPool<Container>( Wildcard, LoadOrderFilter ),
                        this.SelectedContainers );
                }
            }

        }

        #region Thread-safe Start

        public static bool StartSyncedListViewRepopulationThread(
            IEnableControlForm form,
            GUIBuilder.Windows.Controls.SyncedListView<Container> containerSyncedListView,
            List<Container> selectedContainers,
            string wildcard,
            int filter,
            Action threadOnStart,
            WorkerThreadPool.ThreadOnFinish onFinishedCallback = null,
            bool updateStatusBar = false,
            bool timeThread = false,
            bool logUpdate = true
        )
        {
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( typeof( Engine.Plugin.Forms.Container ) );
            if( association == null ) return false;
            var parameters = new WorkshopContainers(
                form,
                containerSyncedListView,
                selectedContainers,
                wildcard,
                filter,
                threadOnStart,
                onFinishedCallback,
                updateStatusBar,
                timeThread,
                logUpdate
            );
            if( parameters == null ) return false;
            return parameters.Start();
        }

        #endregion

        #endregion


    }

}
