/*
 * SubDivision.cs
 *
 * This groups all related data to sub-divisions.
 *
 */

using System;
using System.Collections.Generic;

using Maths;

using Engine;
using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Extensions;

using XeLib;


namespace AnnexTheCommonwealth
{
    
    /// <summary>
    /// Description of SubDivision.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:SubDivision" )]
    public class SubDivision : Engine.Plugin.PapyrusScript, GUIBuilder.Interface.WorkshopController
    {
        
        // EdgeFlags for sub-division
        Keyword                                             _EdgeFlagKeyword = null;
        List<EdgeFlag>                                      _EdgeFlags = null;
        bool                                                _EdgeFlagsClosedLoop = false;
        
        // Border enablers for sub-division
        List<BorderEnabler>                                 _BorderEnablers = null;
        //List<GUIBuilder.BorderNode>                       _SandboxBorderNodes = null;
        
        // Build volumes for sub-division
        
        List<BuildAreaVolume>                               _BuildAreaVolumes = null;
        //public List<BorderNode>                           BorderNodes = null;
        
        #region Constructor
        
        public                                              SubDivision( ObjectReference reference )
        : base( reference ) {}
        
        public override bool                                PostLoad()
        {
            return base.PostLoad() && INTERNAL_FetchEdgeFlags();
        }
        
        #endregion
        
        #region Linked Refs
        
        #region Internal Edge Flag Management
        
        void                                                INTERNAL_ResetEdgeFlags()
        {
            _EdgeFlags = null;
            _EdgeFlagKeyword = null;
            _EdgeFlagsClosedLoop = false;
        }
        
        void                                                INTERNAL_InsertEdgeFlag( int index, EdgeFlag flag, Keyword keyword )
        {
            if( _EdgeFlags == null )
            {
                _EdgeFlags = new List<EdgeFlag>();
                _EdgeFlagKeyword = keyword;
            }
            _EdgeFlags.Insert( index, flag );
            flag.kywdSubDivision[ keyword.GetFormID( TargetHandle.Master ) ] = this;
        }
        
        bool                                                INTERNAL_AddEdgeFlag( EdgeFlag flag, Keyword keyword )
        {
            if( _EdgeFlags == null )
            {
                _EdgeFlags = new List<EdgeFlag>();
                _EdgeFlagKeyword = keyword;
            }
            _EdgeFlags.Add( flag );
            flag.kywdSubDivision[ keyword.GetFormID( TargetHandle.Master ) ] = this;
            return true;
        }
        
        bool                                                INTERNAL_FetchEdgeFlags( bool forceReset = false, EdgeFlag forceStopAt = null )
        {
            if( ( !_EdgeFlags.NullOrEmpty() )&&( !forceReset ) )
                return true;
            
            //DebugLog.OpenIndentLevel( new [] { this.IDString, "forceReset = " + forceReset.ToString() + "\n", "forceStopAt = " + ( forceStopAt == null ? "[null]" : forceStopAt.IDString ) }, true );
            var result = false;
            
            if( forceReset )
                INTERNAL_ResetEdgeFlags();
            
            var lrs = Reference.LinkedRefs;
            if( lrs.GetCount( TargetHandle.WorkingOrLastFullRequired ) < 1 )     // No references is not an error per-say
            {
                result = true;
                goto localReturnResult;
            }
            
            var fid = GetFormID( TargetHandle.Master );
            var flag = (EdgeFlag)null;
            uint fkFID = Engine.Plugin.Constant.FormID_None;
            
            for( int i = 0; i < lrs.GetCount( TargetHandle.WorkingOrLastFullRequired ); i++ )
            {
                var lro = lrs.GetReference( TargetHandle.WorkingOrLastFullRequired, i );
                if( ( lro != null )&&( GodObject.CoreForms.IsSubDivisionEdgeFlag( lro.GetNameFormID( TargetHandle.WorkingOrLastFullRequired ) ) ) )
                {
                    flag = lro.GetScript<EdgeFlag>();
                    if( flag != null )
                    {
                        fkFID = lrs.GetKeywordFormID( TargetHandle.WorkingOrLastFullRequired, i );
                        break;
                    }
                }
            }
            if( flag == null )     // No edge flags references is not an error per-say
            {
                result = true;
                goto localReturnResult;
            }
            
            var keyword = GodObject.CoreForms.GetSubDivisionEdgeFlagKeyword( fkFID );
            if( keyword == null )    // However, an edge flag with a bad keyword IS invalid
            {
                keyword = fkFID != Engine.Plugin.Constant.FormID_None
                    ? GodObject.Plugin.Data.Root.Find<Keyword>( fkFID )
                    : null;
                var ks = keyword != null
                    ? keyword.ToString()
                    : fkFID != Engine.Plugin.Constant.FormID_None
                    ? "Invalid Keyword FormID 0x" + fkFID.ToString( "X8" )
                    : "[null]";
                DebugLog.WriteWarning( "An edge flag is linked with the wrong keyword!\nkeyword = " + ks );
                goto localReturnResult;
            }
            
            var firstFlag = flag;
            INTERNAL_AddEdgeFlag( firstFlag, keyword );
            var prevFlag = flag;
            
            var linkedRef = flag.Reference.LinkedRefs.GetLinkedRef( TargetHandle.WorkingOrLastFullRequired, fkFID );
            while( linkedRef != null )
            {
                bool added = false;
                if( GodObject.CoreForms.IsSubDivisionEdgeFlag( linkedRef.GetNameFormID( TargetHandle.WorkingOrLastFullRequired ) ) )
                {
                    flag = linkedRef.GetScript<EdgeFlag>();
                    if( flag != null )
                    {
                        if( flag == firstFlag )
                        {
                            _EdgeFlagsClosedLoop = true;
                            break;
                        }
                        var flagIndex = _EdgeFlags.FindIndex( f => f == flag );
                        if( flagIndex >= 0 )
                        {
                            DebugLog.WriteWarning( string.Format(
                                "Sub-division has an edge flag that points to the middle of the chain!\n\tSubDivision: {0}\n\tEdgeFlag: {1}\n\tKeyword: {2}",
                                this.IDString,
                                prevFlag.IDString,
                                keyword.IDString ) );
                            
                            // Bad linking, truncate the list to this flags element
                            _EdgeFlags.RemoveRange( flagIndex + 1, _EdgeFlags.Count - flagIndex - 1 );
                            break;
                        }
                        
                        added = INTERNAL_AddEdgeFlag( flag, keyword );
                        
                        if( ( forceStopAt != null )&&( flag == forceStopAt ) )
                            break;
                        
                        prevFlag = flag;
                        linkedRef = linkedRef.LinkedRefs.GetLinkedRef( TargetHandle.WorkingOrLastFullRequired, fkFID );
                    }
                }
                if( !added )
                    break;
            }
            /*
            if( !_EdgeFlagsClosedLoop )
            {
                var efCount = _EdgeFlags.Count;
                DebugLog.Write( string.Format( "\nWarning: Sub-Division EdgeFlags do not form a complete loop.\n\tSub-Division = {0}\n\tFlag count = {1}\n\tFirst = {2}\n\tLast = {3}", this.IDString, efCount, _EdgeFlags[ 0 ].IDString, _EdgeFlags[ efCount - 1 ].IDString ) );
            }
            */
            result = true;
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            //DebugLog.CloseIndentLevel<EdgeFlag>( "EdgeFlags", _EdgeFlags );
            return result;
        }
        
        #endregion
        
        #region Public Edge Flag Access
        
        public void                                         InsertEdgeFlag( int index, EdgeFlag flag, Keyword keyword )
        {
            INTERNAL_FetchEdgeFlags();
            if( ( index < 0 )||( index > GetEdgeFlagCount() ) )
                return;
            INTERNAL_InsertEdgeFlag( index, flag, keyword );
        }
        
        public void                                         AddEdgeFlag( EdgeFlag flag, Keyword keyword )
        {
            INTERNAL_FetchEdgeFlags();
            INTERNAL_AddEdgeFlag( flag, keyword );
        }
        
        public EdgeFlag                                     GetEdgeFlagAt( int index )
        {
            INTERNAL_FetchEdgeFlags();
            return ( _EdgeFlags.NullOrEmpty() )||( index < 0 )||( index >= GetEdgeFlagCount() )
                ? null
                : _EdgeFlags[ index ];
        }
        
        public int                                          GetEdgeFlagCount()
        {
            INTERNAL_FetchEdgeFlags();
            return _EdgeFlags.NullOrEmpty()
                ? 0
                : _EdgeFlags.Count;
        }
        
        /// <summary>
        /// The purpose of this function is to get a thread safe copy of the sub-divisions edge flags.
        /// This returns the sub-divisions edge flags, manipulating this may cause errors with the sub-division itself, use the proper API calls for that.
        /// </summary>
        /// <returns>The internal edge flags list.</returns>
        public List<EdgeFlag>                               EdgeFlags
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { this.IDString, "EdgeFlags" }, true );
                var resFunc = INTERNAL_FetchEdgeFlags();
            //localReturnResult:
                //DebugLog.CloseIndentLevel<EdgeFlag>( _EdgeFlags );
                return _EdgeFlags.NullOrEmpty()
                    ? null
                    : _EdgeFlags;
            }
        }
        
        public List<ObjectReference>                        BorderMarkers
        {
            get
            {
                return EdgeFlags?.ConvertAll( f => f.Reference );
            }
        }
        
        public Keyword                                      EdgeFlagKeyword
        {
            get
            {
                INTERNAL_FetchEdgeFlags();
                return _EdgeFlagKeyword;
            }
            // TODO:  Make this change all the links in an existing set
            set
            {
                _EdgeFlagKeyword = value;
            }
        }
        
        public bool                                         EdgeFlagsClosedLoop
        {
            get
            {
                INTERNAL_FetchEdgeFlags();
                return _EdgeFlagsClosedLoop;
            }
        }
        
        #endregion
        
        #region Public Border Enabler Access
        
        public void                                         AddBorderEnabler( BorderEnabler enabler )
        {
            if( _BorderEnablers == null )
                _BorderEnablers = new List<BorderEnabler>();
            _BorderEnablers.AddOnce( enabler );
        }
        
        public void                                         RemoveBorderEnabler( BorderEnabler enabler )
        {
            if( _BorderEnablers == null ) return;
            _BorderEnablers.Remove( enabler );
        }
        
        public bool                                         HasBorderEnabler( BorderEnabler enabler )
        {
            return
                ( !_BorderEnablers.NullOrEmpty() )&&
                ( _BorderEnablers.Contains( enabler ) );
        }
        
        public List<BorderEnabler>                          BorderEnablers
        {
            get
            {
                return _BorderEnablers.Clone();
            }
        }
        
        #endregion
        
        #region Public Build Volume Access
        
        public List<BuildAreaVolume>                        BuildAreaVolumes
        {
            get
            {
                if( _BuildAreaVolumes == null )
                {
                    var subRefs = Reference.References;
                    if( subRefs.NullOrEmpty() )
                        return null;
                    
                    var volumeActivator = GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_BuildAreaVolume;
                    if( volumeActivator == null )
                        return null;
                    var linkKeyword = GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedBuildAreaVolume;
                    if( linkKeyword == null )
                        return null;
                    
                    var thisFID = Reference.GetFormID( TargetHandle.Master );
                    var volumeFID = volumeActivator.GetFormID( TargetHandle.Master );
                    var keywordFID = linkKeyword.GetFormID( TargetHandle.Master );
                    
                    var list = new List<BuildAreaVolume>();
                    
                    foreach( var subRef in subRefs )
                    {
                        var refr = subRef as ObjectReference;
                        if( ( refr != null )&&( refr.GetNameFormID( TargetHandle.WorkingOrLastFullRequired ) == volumeFID ) )
                        {
                            var li = refr.LinkedRefs.FindKeywordIndex( TargetHandle.WorkingOrLastFullRequired, keywordFID );
                            if( ( li >= 0 )&&( refr.LinkedRefs.GetReferenceID( TargetHandle.WorkingOrLastFullRequired, li ) == thisFID ) )
                            {
                                var volume = refr.GetScript<BuildAreaVolume>();
                                if( volume != null )
                                    list.Add( volume );
                            }
                        }
                    }
                    if( !list.NullOrEmpty() )
                        _BuildAreaVolumes = list;
                }
                return _BuildAreaVolumes;
            }
        }
        
        public List<ObjectReference>                        BuildVolumes
        {
            get
            {
                return BuildAreaVolumes?.ConvertAll( v => v.Reference );
            }
        }
        public float                                        BuildVolumeCeiling
        {
            get
            {
                var volumes = BuildAreaVolumes;
                var volumeCeiling = float.MinValue;
                if( !volumes.NullOrEmpty() )
                {
                    foreach( var volume in volumes )
                    {
                        var vRef = volume.Reference;
                        var vrPos = vRef.GetPosition( TargetHandle.WorkingOrLastFullRequired );
                        var vrBounds = vRef.Primitive.GetBounds( TargetHandle.WorkingOrLastFullRequired );
                        var ceiling = vrPos.Z + vrBounds.Z * 0.5f;
                        if( ceiling > volumeCeiling )
                            volumeCeiling = ceiling;
                    }
                }
                return volumeCeiling;
            }
        }
        
        #endregion
        
        #region Public Sandbox Access
        
        public ObjectReference                              SandboxEdgeEnabler
        {
            get
            {
                return Reference.LinkedRefs.GetLinkedRef( TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSandboxEdge.GetFormID( TargetHandle.Master ) );
            }
        }
        
        public ObjectReference                              SandboxVolume
        {
            get
            {
                return Reference.LinkedRefs.GetLinkedRef(
                    TargetHandle.WorkingOrLastFullRequired,
                    GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSandboxVolume.GetFormID( TargetHandle.Master ) );
            }
        }
        
        #endregion
        
        #endregion
        
        #region PapyrusScript Properties & Enumerations
        
        #region myLocation
        
        const string                                        PS_myLocation = "myLocation";
        
        public uint                                         GetMyLocation( TargetHandle target )
        {
            var result = Engine.Plugin.Constant.FormID_Invalid;
            
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_myLocation );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_myLocation ) );
                goto localAbort;
            }
            
            result = ph.GetUIntValue();
            
        localAbort:
            return result;
        }
        public void                                         SetMyLocation( TargetHandle target, uint value )
        {
            if( target != TargetHandle.Working )
                throw new Exception( string.Format( "AnnexTheCommonwealth.SubDivision :: SetMyLocation() :: Invalid target = {0}", target.ToString() ) );
            if( !this.IsInWorkingFile() )
                if( !this.CopyAsOverride().IsValid() )
                    throw new Exception( "AnnexTheCommonwealth.SubDivision :: SetMyLocation() :: Unable to copy override to working file!" );
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_myLocation );
            if( !ph.IsValid() )
                //ph = this.
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_myLocation ), true );
                return;
            }
            ph.SetUIntValue( value );
        }
        
        #endregion
        
        #region Relationships
        
        const string                                        PS_RelationshipsAnyAll = "RelationshipsAnyAll";
        
        public int                                          GetRelationshipsAnyAll( TargetHandle target )
        {
            var result = 0;
            
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_RelationshipsAnyAll );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                goto localAbort;
            }
            
            result = Math.Min( 0, Math.Max( 1, ph.GetIntValue() ) );
            
        localAbort:
            return result;
        }
        public void                                         SetRelationshipsAnyAll( TargetHandle target, int value )
        {
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_RelationshipsAnyAll );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                return;
            }
            ph.SetIntValue( value );
        }
        
        #endregion
        
        #region Quests
        
        const string                                        PS_QuestsAnyAll = "QuestStagesAnyAll";
        
        public int                                          GetQuestStagesAnyAll( TargetHandle target )
        {
            var result = 0;
            
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_QuestsAnyAll );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                goto localAbort;
            }
            
            result = Math.Min( 0, Math.Max( 1, ph.GetIntValue() ) );
            
        localAbort:
            return result;
        }
        public void                                         SetQuestStagesAnyAll( TargetHandle target, int value )
        {
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_QuestsAnyAll );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                return;
            }
            ph.SetIntValue( value );
        }
        
        #endregion
        
        #region Relationships & Quests
        
        const string                                        PS_RelationshipsAndQuests = "RelationshipsAndQuests";
        
        public int                                          GetRelationshipsAndQuests( TargetHandle target )
        {
            var result = 0;
            
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_RelationshipsAndQuests );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                goto localAbort;
            }
            
            result = Math.Min( 0, Math.Max( 1, ph.GetIntValue() ) );
            
        localAbort:
            return result;
        }
        public void                                         SetRelationshipsAndQuests( TargetHandle target, int value )
        {
            var ph = this.ScriptPropertyHandleFromTarget( target, PS_RelationshipsAndQuests );
            if( !ph.IsValid() )
            {
                DebugLog.WriteLine( string.Format( "ScriptPropertyHandleFromTarget() returned null!  Target = {0} :: Property = \"{1}\"", target.ToString(), PS_RelationshipsAnyAll ), true );
                return;
            }
            ph.SetIntValue( value );
        }
        
        #endregion
        
        #endregion
        
        public string                                       LocationName
        {
            get
            {
                var location = GodObject.Plugin.Data.Root.Find<Location>( GetMyLocation( TargetHandle.WorkingOrLastFullRequired ), true );
                return location?.GetFullName( TargetHandle.WorkingOrLastFullRequired );
            }
        }

        public string                                       QualifiedName
        {
            get
            {
                var lName = LocationName;
                if( !string.IsNullOrEmpty( lName ) )
                    return lName.Replace( " ", "" ).Replace( "-", "" );
                // TODO:  FIX ME FOR PROPER NAMESPACE PREFIXES!
                var foo = GetEditorID( TargetHandle.WorkingOrLastFullRequired );
                var si = foo.IndexOf( "SubDivision", StringComparison.InvariantCultureIgnoreCase );
                return foo.Substring( 3, si - 3 );
            }
        }
        
        #region Bounding box and cells of all build volume
        
        public static List<Vector2f>                        GetBuildVolumeCornerPointsFrom<TVolume>( List<TVolume> volumes ) where TVolume : Volume
        {
            if( volumes.NullOrEmpty() )
                return null;
            
            var list = new List<Vector2f>();
            foreach( var volume in volumes )
            {
                var corners = volume.Reference.GetCorners( TargetHandle.WorkingOrLastFullRequired );
                foreach( var corner in corners )
                {
                    list.Add( new Vector2f( corner ) );
                    DebugLog.WriteLine( string.Format( "Corner :: {0}", corner.ToString() ) );
                }
            }
            
            return list;
        }
        
        public Vector2f                                     CornerNW
        {
            get
            {
                return GetBuildVolumeCornerPointsFrom( BuildAreaVolumes ).GetCornerNWFrom();
            }
        }
        
        public Vector2f                                     CornerSE
        {
            get
            {
                return GetBuildVolumeCornerPointsFrom( BuildAreaVolumes ).GetCornerSEFrom();
            }
        }
        
        public Vector2i CellNW                              { get { return CornerNW.WorldspaceToCellGrid(); } }
        
        public Vector2i CellSE                              { get { return CornerSE.WorldspaceToCellGrid(); } }
        
        #endregion
        
        #region Build Border - Edge Flag Sharing
        
        void                                                BuildSandboxBorder( float approximateNodeLength )
        {
            ClearSandboxBorderNodes( false );
            var enabler = SandboxEdgeEnabler;
            var volume = SandboxVolume;
            if( ( enabler == null )||( volume == null ) )
            {
                SendObjectDataChangedEvent( this );
                return;
            }
            SendObjectDataChangedEvent( this );
        }
        
        void                                                AddNodeToSubCount( uint subFID, Dictionary<uint, List<uint>> subCount, uint flagFID )
        {
            List<uint> sfl;
            if( !subCount.TryGetValue( subFID, out sfl ) )
            {
                sfl = new List<uint>();
                sfl.Add( flagFID );
                subCount[ subFID ] = sfl;
            }
            else
            {
                sfl.Add( flagFID );
            }
        }
        
        public List<GUIBuilder.FormImport.ImportBase>       GenerateMissingBorderEnablersFromEdgeFlags()
        {
            //DebugLog.OpenIndentLevel( this.IDString, true );
            
            List<GUIBuilder.FormImport.ImportBase> result = null;
            
            if( _EdgeFlagKeyword == null )
            {
                //DebugLog.WriteLine( "_EdgeFlagKeyword is NULL!" );
                goto localReturnResult;
            }
            
            if( _EdgeFlags.NullOrEmpty() )
            {
                //DebugLog.WriteLine( "_EdgeFlags is NULL or EMPTY!" );
                goto localReturnResult;
            }
            
            var kfid = _EdgeFlagKeyword.GetFormID( TargetHandle.Master );
            
            _BorderEnablers = _BorderEnablers ?? new List<BorderEnabler>();
            
            var subCount = new Dictionary<uint, List<uint>>();
            
            // Go through the edge flags and find which ones are consecutively shared with other sub-divisions (so no shares on multiple independant flags)
            const uint nullFID = Engine.Plugin.Constant.FormID_None;
            var thisFID = this.GetFormID( TargetHandle.Master );
            var prevFlag = _EdgeFlags[ 0 ];
            for( int i = _EdgeFlags.Count - 1; i >= 0; i-- )
            {
                var flag = _EdgeFlags[ i ];
                var flagFID = flag.GetFormID( TargetHandle.Master );
                if( flag.kywdSubDivision.Count == 1 )
                {
                    // Main border only
                    AddNodeToSubCount( nullFID, subCount, flagFID );
                }
                else
                {
                    if( !flag.HasAnySharedAssociations( prevFlag, thisFID ) )
                    {
                        // Main border too
                        AddNodeToSubCount( nullFID, subCount, prevFlag.GetFormID( TargetHandle.Master ) ); // Previous flag
                        AddNodeToSubCount( nullFID, subCount, flagFID ); // this flag
                    }
                    // And the rest
                    foreach( var ksp in flag.kywdSubDivision )
                        if( ksp.Key != kfid )
                            AddNodeToSubCount( ksp.Value.GetFormID( TargetHandle.Master ), subCount, flagFID );
                }
                prevFlag = flag;
            }
            
            /*
            DebugLog.OpenIndentLevel( "Shared Counts: " );
            foreach( var sc in subCount )
            {
                DebugLog.WriteLine( "Neighbour: 0x" + sc.Key.ToString( "X8" ) );
                DebugLog.OpenIndentLevel( "Flags:" );
                foreach( var f in sc.Value )
                    DebugLog.WriteLine( "0x" + f.ToString( "X8" ) );
                DebugLog.CloseIndentLevel();
            }
            DebugLog.CloseIndentLevel();
            */
            
            // Now go through the lists of shared flags and find out which ones are missing border enablers
            foreach( var sc in subCount )
            {
                //DebugLog.WriteLine( "Trying to find enabler for neighbour 0x" + sc.Key.ToString( "X8" ) );
                if( sc.Value.Count >= 2 )
                {
                    bool isMain = sc.Key == Engine.Plugin.Constant.FormID_None;
                    //DebugLog.OpenIndentLevel();
                    var e = _BorderEnablers.Find( b =>
                        {
                            //DebugLog.WriteLine( "_BorderEnablers.Find() :: ? 0x" + ( b.Neighbour == null ? 0 : b.Neighbour.FormID ).ToString( "X8" ) );
                            if( isMain ) return b.Neighbour == null;
                            if( b.Neighbour == null ) return false;
                            return b.Neighbour.GetFormID( TargetHandle.Master ) == sc.Key;
                        } );
                    //DebugLog.CloseIndentLevel();
                    if( e != null )
                    {
                        //DebugLog.WriteLine( "Found existing enabler by neighbour " + e.IDString );
                        continue;
                    }
                    
                    // Try to find an existing enabler with the proper name (why isn't it linked to the sub-division or detected as such?)
                    SubDivision neighbour = null;
                    if( Engine.Plugin.Constant.ValidFormID( sc.Key ) )
                        neighbour = GodObject.Plugin.Data.SubDivisions.Find( sc.Key );
                    
                    var newEditorID = 
                        string.Format(
                            "{0}{1}Border01{2}",
                            "ESM",
                            this.QualifiedName,
                            ( neighbour == null ? "Main" : neighbour.QualifiedName ) );
                    
                    // Check enablers linked to the sub-division
                    e = _BorderEnablers.Find( (b) => ( b.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) == newEditorID ) );
                    if( e != null )
                    {
                        //DebugLog.WriteLine( "Found existing enabler by EditorID " + e.IDString );
                        continue;
                    }
                    
                    // Check for unlinked enablers
                    e = GodObject.Plugin.Data.BorderEnablers.Find( newEditorID );
                    if( e != null )
                    {
                        //DebugLog.WriteLine( "Found existing enabler in global table " + e.IDString );
                        continue;
                    }
                    
                    // No enabler found, need to create one
                    var refPos = Reference.GetPosition( TargetHandle.WorkingOrLastFullRequired );
                    var neiPos = neighbour.Reference.GetPosition( TargetHandle.WorkingOrLastFullRequired );
                    
                    var newpos = sc.Key == Engine.Plugin.Constant.FormID_None
                        ? new Vector3f(
                            refPos.X,
                            refPos.Y,
                            ControllerPosition.ZPOS_BorderEnabler )
                        : ControllerPosition.CalculateRelativeFrom(
                            refPos,
                            neiPos,
                            ControllerPosition.Controller_XY_Separation,
                            ControllerPosition.ZPOS_BorderEnabler );
                    
                    //DebugLog.WriteLine( "Adding import for " + newEditorID );
                    
                    /*
                    var import = new GUIBuilder.FormImport.ImportBorderEnablerReference(
                         null,
                         newEditorID,
                         Reference.Worldspace,
                         Reference.Worldspace.Cells.Persistent,
                         newpos,
                         this, neighbour );
                    
                    GUIBuilder.FormImport.ImportBase.AddToList( ref result, import );
                    */
                    
                }
            }
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<GUIBuilder.FormImport.ImportBase>( result );
            return result.NullOrEmpty()
                ? null
                : result;
        }
        
        public void                                         ClearAllBorderSegmentsAndNodes( bool sendchangedevent )
        {
            ClearSandboxBorderNodes( false );
            ClearBorderEnablerEdgeFlags( false );
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
        public void                                         ClearSandboxBorderNodes( bool sendchangedevent )
        {
            //_SandboxBorderNodes = null;
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
        public void                                         ClearBorderEnablerEdgeFlags( bool sendchangedevent )
        {
            if( _BorderEnablers.NullOrEmpty() ) return;
            foreach( var enabler in _BorderEnablers )
                enabler.ClearSegmentsAndEdgeFlags( true );
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
        public void                                         BuildSegmentsFromEdgeFlags( float approximateNodeLength, double angleAllowance, double slopeAllowance, bool updateMapUIData )
        {
            DebugLog.OpenIndentLevel( this.IDString );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            if( _BorderEnablers.NullOrEmpty() )
            {
                DebugLog.WriteLine( "No BorderEnablers for SubDivision" );
                goto localReturnResult;
            }
            
            foreach( var enabler in _BorderEnablers )
            {
                m.SetCurrentStatusMessage( string.Format( "BorderBatch.CalculatingBordersFor".Translate(), enabler.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) ) );
                enabler.BuildSegmentsFromSubDivisionEdgeFlags( approximateNodeLength, angleAllowance, slopeAllowance, updateMapUIData );
            }
            
            SendObjectDataChangedEvent( this );
            
        localReturnResult:
            var elapsed = m.StopSyncTimer( tStart, this.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel( elapsed );
        }
        
        public List<GUIBuilder.FormImport.ImportBase>       CreateBorderNIFs(
            float gradientHeight,
            float groundOffset,
            float groundSink,
            string targetPath,
            string targetSuffix,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string fileSuffix,
            bool createImportData,
            bool highPrecisionVertexes )
        {
            //DebugLog.Write( string.Format( "\n{0} :: CreateBorderNIFs() :: sub-division 0x{1} \"{2}\"", this.FullTypeName(), this.FormID.ToString( "X8" ), this.EditorID ) );
            if( _BorderEnablers.NullOrEmpty() )
                return null;
            
            var volumeCeiling = BuildVolumeCeiling;
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            // Fix edge flag record flags
            if( createImportData )
            {
                if( !_EdgeFlags.NullOrEmpty() )
                {
                    foreach( var flag in _EdgeFlags )
                    {
                        var w = flag.Reference.Worldspace;
                        var c = w == null
                            ? flag.Reference.Cell
                            : flag.Reference.Worldspace.Cells.Persistent;
                        //GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportEdgeFlagReference( flag, w, c ) );
                    }
                }
            }
            foreach( var enabler in _BorderEnablers )
            {
                var subList = enabler.CreateBorderNIFs(
                    gradientHeight, groundOffset, groundSink,
                    targetPath, targetSuffix,
                    meshSuffix, meshSubPath,
                    filePrefix, fileSuffix,
                    volumeCeiling,
                    createImportData,
                    highPrecisionVertexes );
                //if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                //    GUIBuilder.FormImport.ImportBase.AddToList( ref list, subList );
            }
            
            return list;
        }
        
        #endregion
        
        #region IMouseOver
        
        public override List<string>                        MouseOverExtra
        {
            get
            {
                if( ( _BorderEnablers == null )&&( _EdgeFlagKeyword == null ) )
                    return null;
                var moel = new List<string>();
                
                if( _BorderEnablers != null )
                {
                    moel.Add( "BorderEnablers:" );
                    foreach( var enabler in _BorderEnablers )
                    {
                        moel.Add( string.Format( "\t{0}", enabler.IDString ) );
                        var emoel = enabler.MouseOverExtra;
                        if( !emoel.NullOrEmpty() )
                            foreach( var emoe in emoel )
                                moel.Add( string.Format( "\t\t{0}", emoe ) );
                    }
                }
                if( _EdgeFlagKeyword != null )
                {
                    moel.Add( "EdgeFlags:" );
                    moel.Add( string.Format( "\tKeyword: {0}", _EdgeFlagKeyword.IDString ) );
                    moel.Add( string.Format( "\tClosed Loop: {0}", _EdgeFlagsClosedLoop ) );
                    foreach( var flag in _EdgeFlags )
                    {
                        moel.Add( string.Format( "\t\t{0}", flag.IDString ) );
                        var fmoel = flag.MouseOverExtra;
                        if( !fmoel.NullOrEmpty() )
                            foreach( var fmoe in fmoel )
                                moel.Add( string.Format( "\t\t\t{0}", fmoe ) );
                    }
                }
                return moel;
            }
        }
        
        #endregion
        
    }
    
}
