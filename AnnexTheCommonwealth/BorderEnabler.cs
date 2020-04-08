/*
 * BorderEnabler.cs
 *
 * A border for a sub-division.
 *
 */

using System.Collections.Generic;

using Maths;


namespace AnnexTheCommonwealth
{
    /// <summary>
    /// Description of BorderEnabler.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "ESM:ATC:BorderEnabler" )]
    public class BorderEnabler : Engine.Plugin.PapyrusScript
    {
        
        #region Meta
        
        struct nifBucket<T>
        {
            
            public T nif;
            public bool usedByImport;
            public bool addedByImport;
            
            public bool ExistsOnLoad { get { return !addedByImport; } }
            
            public nifBucket( T n, bool used, bool added )
            {
                nif = n;
                usedByImport = used;
                addedByImport = added;
            }
            
        }
        
        Dictionary<uint,nifBucket<Engine.Plugin.Forms.Static>> _NIFs = null;
        Dictionary<uint,nifBucket<Engine.Plugin.Forms.ObjectReference>> _placedNIFs = null;
        
        List<GUIBuilder.BorderSegment> _segments = null;
        
        #endregion
        
        #region Allocation
        
        public BorderEnabler( Engine.Plugin.Forms.ObjectReference reference ) : base( reference ) {}
        
        #endregion
        
        #region Post/Load
        
        public override bool PostLoad()
        {
            if( !base.PostLoad() ) return false;
            var subdivision = SubDivision;
            if( subdivision != null )
                subdivision.AddBorderEnabler( this );
            
            return true;
        }
        
        #endregion
        
        #region Properties
        
        public SubDivision SubDivision
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "SubDivision" } );
                
                var kfid = GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master );
                var reference = Reference;
                
                var sref = reference.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kfid );
                var result = sref != null
                    ? sref.GetScript<SubDivision>()
                    : null;
                
                //DebugLog.CloseIndentLevel<SubDivision>( result );
                return result;
            }
            set
            {
                var kfid = GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master );
                _segments = null;
                
                var sIdx = Reference.LinkedRefs.FindKeywordIndex( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kfid );
                if( sIdx < 0 )
                {
                    // Add new linked ref
                    Reference.LinkedRefs.Add( Engine.Plugin.TargetHandle.Working, value.GetFormID( Engine.Plugin.TargetHandle.Master ), kfid );
                }
                else
                {
                    // Replace linked ref
                    Reference.LinkedRefs.SetReferenceID( Engine.Plugin.TargetHandle.Working, sIdx, value.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                SendObjectDataChangedEvent( this );
            }
        }
        
        public Engine.Plugin.Forms.Keyword Keyword
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Keyword" } );
                
                var subdivision = SubDivision;
                var result = subdivision != null
                    ? SubDivision.EdgeFlagKeyword
                    : null;
                
                //DebugLog.CloseIndentLevel<Engine.Plugin.Forms.Keyword>( result );
                return result;
            }
            set
            {
                var subdivision = SubDivision;
                if( subdivision == null )
                    return;
                
                subdivision.EdgeFlagKeyword = value;
                _segments = null;
                SendObjectDataChangedEvent( this );
            }
        }
        
        public SubDivision Neighbour
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Neighbour" } );
                
                var kfid = GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSubDivision.GetFormID( Engine.Plugin.TargetHandle.Master );
                var reference = Reference;
                
                var nref = reference.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kfid );
                var result = nref != null
                    ? nref.GetScript<SubDivision>()
                    : null;
                
                //DebugLog.CloseIndentLevel<SubDivision>( result );
                return result;
            }
            set
            {
                var kfid = GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSubDivision.GetFormID( Engine.Plugin.TargetHandle.Master );
                _segments = null;
                
                var nIdx = Reference.LinkedRefs.FindKeywordIndex( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, kfid );
                if( nIdx < 0 )
                {
                    // Add new linked ref
                    Reference.LinkedRefs.Add( Engine.Plugin.TargetHandle.Working, value.GetFormID( Engine.Plugin.TargetHandle.Master ), kfid );
                }
                else
                {
                    // Replace linked ref
                    Reference.LinkedRefs.SetReferenceID( Engine.Plugin.TargetHandle.Working, nIdx, value.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                }
                SendObjectDataChangedEvent( this );
            }
        }
        
        public List<GUIBuilder.BorderSegment> Segments { get { return _segments; } }
        
        public List<Engine.Plugin.Form> NIFs
        {
            get
            {
                if( _NIFs == null )
                    GetBorderNIFs();
                if( _NIFs == null )
                    return null;
                var nifs = new List<Engine.Plugin.Form>();
                foreach( var nkv in _NIFs )
                    nifs.Add( nkv.Value.nif );
                return nifs;
            }
        }
        
        public List<Engine.Plugin.Forms.ObjectReference> PlacedNIFs
        {
            get
            {
                if( _placedNIFs == null )
                    GetBorderNIFs();
                if( _placedNIFs == null )
                    return null;
                var nifRefs = new List<Engine.Plugin.Forms.ObjectReference>();
                foreach( var pkv in _placedNIFs )
                    nifRefs.Add( pkv.Value.nif );
                return nifRefs;
            }
        }
        
        #endregion
        
        #region Generate Border NIFs
        
        public List<GUIBuilder.FormImport.ImportBase> CreateBorderNIFs(
            float gradientHeight,
            float groundOffset,
            float groundSink,
            string targetPath,
            string targetSuffix,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string fileSuffix,
            float volumeCeiling,
            bool createImportData,
            bool highPrecisionVertexes )
        {
            //DebugLog.Write( string.Format( "\n{0} :: CreateBorderNIFs() :: enabler 0x{1} \"{2}\"", this.FullTypeName(), this.FormID.ToString( "X8" ), this.EditorID ) );
            if( _segments.NullOrEmpty() )
                return null;
            
            var subdivision = SubDivision;
            if( subdivision == null )
                return null;
            
            var keyword = Keyword;
            if( keyword == null )
                return null;
            
            var originalForms = createImportData ? new List<Engine.Plugin.Form>() : null;
            
            var neighbour = Neighbour;
            var worldspace = Reference.Worldspace;
            //var subFID = subdivision.GetFormID( Engine.Plugin.TargetHandle.Master );
            var subName = subdivision.QualifiedName;
            var nsubName = neighbour == null
                ? "Main"
                : neighbour.QualifiedName;
            
            if( ( !string.IsNullOrEmpty( meshSubPath ) )&&( meshSubPath[ meshSubPath.Length - 1 ] != '\\' ) )
                meshSubPath += @"\";
            var subSubPath = string.Format( "{0}{1}", meshSubPath, subName );
            
            if( createImportData )
            {
                var borderRefs = Reference.References;
                if( !borderRefs.NullOrEmpty() )
                {
                    foreach( var form in borderRefs )
                    {
                        var refr = form as Engine.Plugin.Forms.ObjectReference;
                        if( refr == null ) continue;
                        originalForms.Add( refr );
                        var stat = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Static>( refr.GetNameFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                        if( stat != null )
                            originalForms.Add( stat );
                    }
                }
            }
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            for( int si = 0; si < _segments.Count; si++ )
            {
                var seg = _segments[ si ];
                
                var subList = NIFBuilder.CreateNIFs(
                    createImportData,
                    seg.Nodes,
                    worldspace,
                    this,
                    null, null,
                    GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_BorderMeshes,
                    targetPath,
                    targetSuffix,
                    meshSuffix,
                    subSubPath,
                    filePrefix,
                    subName,
                    fileSuffix,
                    nsubName,
                    ( si + 1 ),
                    "",
                    "",
                    volumeCeiling,
                    gradientHeight,
                    groundOffset,
                    groundSink,
                    true,
                    false,
                    Vector3f.Zero,
                    NIFBuilder.Colours.InsideBorder,
                    NIFBuilder.Colours.OutsideBorder,
                    originalForms,
                    false,
                    NIFBuilder.ExportInfo,
                    highPrecisionVertexes
                    );
                
                if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                    GUIBuilder.FormImport.ImportBase.AddToList( ref list, subList );
            }
            
            return list;
        }
        
        #endregion
        
        #region Border segments
        
        public void ClearSegmentsAndEdgeFlags( bool sendchangedevent )
        {
            _segments = null;
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
        public void BuildSegmentsFromSubDivisionEdgeFlags( float approximateNodeLength, double angleAllowance, double slopeAllowance, bool updateMapUIData )
        {
            DebugLog.OpenIndentLevel( new [] { this.IDString, "approximateNodeLength = " + approximateNodeLength, "angleAllowance = " + angleAllowance, "slopeAllowance = " + slopeAllowance, "updateMapUIData = " + updateMapUIData }, false, true, false );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            var subdivision = SubDivision;
            var keyword = Keyword;
            if( ( subdivision == null )||( keyword == null ) )
                goto localReturnResult;
            
            _segments = null;
           
            var subFlags = subdivision.EdgeFlags.Clone();
            if( subFlags.NullOrEmpty() )
                goto localReturnResult;
            
            var subFID = subdivision.GetFormID( Engine.Plugin.TargetHandle.Master );
            
            var closedLoop = subdivision.EdgeFlagsClosedLoop;
            
            var bseg = GenerateSegment( subFlags, subFID, closedLoop );
            while( bseg != null )
            {
                if( bseg.Count >= 2 )
                {
                    if( _segments == null )
                        _segments = new List<GUIBuilder.BorderSegment>();
                    var segment = new GUIBuilder.BorderSegment( this, bseg );
                    segment.GenerateBorderNodes( approximateNodeLength, angleAllowance, slopeAllowance );
                    _segments.Add( segment );
                }
                
                var sfc = subFlags.Count;
                var bsc = bseg.Count;
                var rcount = sfc - bsc;
                if( rcount < 2 )
                    break;
                
                var index = subFlags.FindIndex( e => e == bseg[ bsc - 1 ] );
                var rflags = new List<EdgeFlag>();
                while( rcount > 0 )
                {
                    rcount -= 1;
                    index += 1;
                    if( index >=sfc )
                    {
                        if( closedLoop )
                            index = 0;
                        else
                            break;
                    }
                    rflags.Add( subFlags[ index ] );
                }
                if( rflags.NullOrEmpty() ) break;
                subFlags = rflags;
                
                bseg = GenerateSegment( subFlags, subFID, false );
            }
            
            if( updateMapUIData )
            {
                // TODO: Nuke the old map UI data and write new values
            }
            
            SendObjectDataChangedEvent(this);
            
        localReturnResult:
            var elapsed = m.StopSyncTimer( tStart, this.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel( elapsed );
        }
        
        List<EdgeFlag> GenerateSegment( List<EdgeFlag> subFlags, uint subFID, bool closedLoop )
        {
            //DebugLog.OpenIndentLevel( new [] { this.IDString, "subFlags.Count = " + subFlags.Count.ToString(), "subFID = 0x" + subFID.ToString( "X8" ), "closedLoop = " + closedLoop.ToString() }, true );
            
            List<EdgeFlag> resultList = null;
            
            var neighbour = Neighbour;
            var subFlagCount = subFlags.Count;
            var flag = (EdgeFlag)null;
            var prevFlag = subFlags[ 0 ];
            var firstIndex = -1;
            // Work backwards, this will handle if the chain is split over the end
            // (read: the start of the chain is in the middle or at the end of the list)
            for( int i = subFlagCount - 1; i >= 0; i-- )
            {
                var f = subFlags[ i ];
                if( neighbour != null )
                {   // Shared border
                    var result = f.AssociatedWithSubDivision( neighbour );
                    if( result )
                    {
                        flag = f;
                        firstIndex = i;
                    }
                    else if( ( !result )&&( flag != null ) )
                        break;
                }
                else
                {   // Unshared (main) border
                    var fsswp = f.HasAnySharedAssociations( prevFlag, subFID );
                    if(
                        ( f.kywdSubDivision.Count == 1 )||
                        ( !fsswp )
                    ) {
                        flag = f;
                        firstIndex = i;
                        if( i == 0 )
                            break;
                    }
                    else if( flag != null ) // Found the start, stop looking
                        break;
                }
                prevFlag = f;
            }
            
            // No match?
            if( flag == null )
                goto localReturnResult;
            
            // Now follow the chain
            resultList = new List<EdgeFlag>();
            var index = firstIndex;
            var firstFlag = flag;
            resultList.Add( flag );
            while( true )
            {
                index += 1;
                if( index >= subFlagCount )
                {
                    if( closedLoop )
                        index = 0;
                    else
                        break;
                }
                prevFlag = flag;
                flag = subFlags[ index ];
                if( neighbour != null )
                {
                    if( flag.AssociatedWithSubDivision( neighbour ) )
                        resultList.Add( flag );
                    else
                        break;
                }
                else
                {
                    var fsswp = flag.HasAnySharedAssociations( prevFlag, subFID );
                    if(
                        ( flag.kywdSubDivision.Count == 1 )||
                        ( !fsswp )
                    )   resultList.Add( flag );
                    else // Found the end, stop
                        break;
                }
                if( index == firstIndex )
                    break;
            }
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<EdgeFlag>( resultList );
            return resultList.NullOrEmpty()
                ? null
                : resultList;
        }
        
        #endregion
        
        #region [Placed] Border NIFs
        
        void GetBorderNIFs()
        {
            var refRefs = Reference.References;
            if( refRefs.NullOrEmpty() )
                return;
            
            var stats = new Dictionary<uint,nifBucket<Engine.Plugin.Forms.Static>>();
            var refrs = new Dictionary<uint,nifBucket<Engine.Plugin.Forms.ObjectReference>>();

            var beFID = this.GetFormID( Engine.Plugin.TargetHandle.Master );

            foreach( var refRef in refRefs )
            {
                var refr = refRef as Engine.Plugin.Forms.ObjectReference;
                if( refr != null )
                {
                    if( refr.EnableParent.GetReferenceID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == beFID )
                    {
                        
                        nifBucket<Engine.Plugin.Forms.ObjectReference> refrBucket;
                        var refrFID = refr.GetFormID( Engine.Plugin.TargetHandle.Master );
                        if( !refrs.TryGetValue( refrFID, out refrBucket ) )
                        {
                            refrBucket = new nifBucket<Engine.Plugin.Forms.ObjectReference>( refr, false, false );
                            refrs[ refrFID ] = refrBucket;
                        }
                        
                        var statFID = refr.GetNameFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        
                        nifBucket<Engine.Plugin.Forms.Static> statBucket;
                        if( !stats.TryGetValue( statFID, out statBucket ) )
                        {
                            var stat = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Static>( statFID );
                            if( stat != null )
                            {
                                statBucket = new nifBucket<Engine.Plugin.Forms.Static>( stat, false, false );
                                stats[ statFID ] = statBucket;
                            }
                        }
                    }
                }
            }
            
            _NIFs = stats;
            _placedNIFs = refrs;
        }
        
        public bool StaticFormIsNIFUsedByImport( Engine.Plugin.Forms.Static baseform )
        {
            if( ( baseform == null )||( _NIFs == null ) )
                return false;
            
            nifBucket<Engine.Plugin.Forms.Static> statBucket;
            if( !_NIFs.TryGetValue( baseform.GetFormID( Engine.Plugin.TargetHandle.Master ), out statBucket ) )
                return false;
            
            statBucket.usedByImport = true;
            return true;
        }
        
        public bool AddBaseFormAsNIFReference( Engine.Plugin.Forms.Static baseform )
        {
            if( baseform == null )
                return false;
            
            if( _NIFs == null )
                _NIFs = new Dictionary<uint, nifBucket<Engine.Plugin.Forms.Static>>();

            var baseFID = baseform.GetFormID( Engine.Plugin.TargetHandle.Master );

            nifBucket<Engine.Plugin.Forms.Static> statBucket;
            if( _NIFs.TryGetValue( baseFID, out statBucket ) )
                return true;
            
            statBucket = new nifBucket<Engine.Plugin.Forms.Static>( baseform, true, true );
            _NIFs[ baseFID ] = statBucket;
            
            return true;
        }

        public bool RemoveBaseFormAsNIFReference( Engine.Plugin.Forms.Static baseform )
        {
            if( baseform == null )
                return false;

            if( _NIFs == null )
                return true;

            var baseFID = baseform.GetFormID( Engine.Plugin.TargetHandle.Master );

            return _NIFs.Remove( baseFID );
        }

        public bool FlagPlacedNIFAsUsedByImport( Engine.Plugin.Forms.ObjectReference refr )
        {
            if( ( refr == null )||( _placedNIFs == null ) )
                return false;
            
            nifBucket<Engine.Plugin.Forms.ObjectReference> refrBucket;
            if( !_placedNIFs.TryGetValue( refr.GetFormID( Engine.Plugin.TargetHandle.Master ), out refrBucket ) )
                return false;
            
            refrBucket.usedByImport = true;
            return true;
        }
        
        public bool AddPlacedNIFReference( Engine.Plugin.Forms.ObjectReference refr )
        {
            if( refr == null )
                return false;
            
            if( _placedNIFs == null )
                _placedNIFs = new Dictionary<uint, nifBucket<Engine.Plugin.Forms.ObjectReference>>();

            var refrFID = refr.GetFormID( Engine.Plugin.TargetHandle.Master );

            nifBucket<Engine.Plugin.Forms.ObjectReference> refrBucket;
            if( _placedNIFs.TryGetValue( refrFID, out refrBucket ) )
                return true;
            
            refrBucket = new nifBucket<Engine.Plugin.Forms.ObjectReference>( refr, true, true );
            _placedNIFs[ refrFID ] = refrBucket;
            
            return true;
        }

        public bool RemovePlacedNIFReference( Engine.Plugin.Forms.ObjectReference refr )
        {
            if( refr == null )
                return false;

            if( _placedNIFs == null )
                return true;

            var refrFID = refr.GetFormID( Engine.Plugin.TargetHandle.Master );

            return _placedNIFs.Remove( refrFID );
        }

        #endregion

        #region IMouseOver

        public override List<string> MouseOverExtra
        {
            get
            {
                var subdivision = SubDivision;
                var neighbour = Neighbour;
                var keyword = Keyword;
                var segs = Segments;
                if( ( segs == null )&&( subdivision == null )&&( neighbour == null ) )
                    return null;
                var moel = new List<string>();
                
                if( subdivision != null )
                {
                    moel.Add(  string.Format( "Sub-Division: {0}", subdivision.IDString ) );
                    if( keyword != null )
                        moel.Add( string.Format( "Keyword: {0}", keyword.IDString ) );
                }
                if( neighbour != null )
                {
                    moel.Add( string.Format( "Neighbour: {0}", neighbour.IDString ) );
                }
                if( segs != null )
                {
                    moel.Add( "Segments:" );
                    moel.Add( string.Format( "\tCount: {0}", segs.Count ) );
                    for( int si = 0; si < segs.Count; si++ )
                    {
                        var seg = segs[ si ];
                        moel.Add( string.Format( "\tSegment: {0}", si ) );
                        moel.Add( "\t\tEdge Flags:" );
                        moel.Add( string.Format( "\t\t\tCount: {0}", seg.Flags.Count ) );
                        for( int i = 0; i < seg.Flags.Count; i++ )
                        {
                            var flag = seg.Flags[ i ];
                            moel.Add( string.Format( "\t\t\t\t{0} - {1}", i, flag.Reference.IDString ) );
                            var flmoel = flag.MouseOverExtra;
                            if( !flmoel.NullOrEmpty() )
                                foreach( var flmoe in flmoel )
                                    moel.Add( string.Format( "\t\t\t\t\t{0}", flmoe ) );
                        }
                    }
                }
                var pNifs = PlacedNIFs;
                if( !pNifs.NullOrEmpty() )
                {
                    moel.Add(
                        string.Format(
                            "Placed Meshes: {0}",
                            pNifs.Count ) );
                    // disable once ForCanBeConvertedToForeach
                    for( int i = 0; i < pNifs.Count; i++ )
                    {
                        var pn = pNifs[ i ];
                        var sn = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Static>( pn.GetNameFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                        moel.Add(
                            string.Format(
                                "\tMesh: 0x{0} ({1}) at {2}",
                                pn.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ),
                                ( sn == null ? "[null]" : sn.IDString ),
                                pn.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString() ) );
                        
                    }
                }
                return moel;
            }
        }
        
        #endregion
        
    }
}
