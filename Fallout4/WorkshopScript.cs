/*
 * WorkshopScript.cs
 *
 * This groups all related data to workshops.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using GUIBuilder;

using Maths;

using Engine;
using Engine.Plugin;

using XeLib;
using XeLib.API;
using XeLibHelper;

namespace Fallout4
{
    
    /// <summary>
    /// Description of Workshop.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( "WorkshopScript" )]
    public class WorkshopScript : Engine.Plugin.PapyrusScript
    {
        
        // Border marker nodes for workshops
        Engine.Plugin.Forms.Keyword _BorderGeneratorKeyword = null;
        Engine.Plugin.Forms.Keyword _BorderMarkerLinkKeyword = null;
        Engine.Plugin.Forms.ObjectReference _FirstBorderMarker = null;
        List<Engine.Plugin.Forms.ObjectReference> _BorderMarkers = null;
        List<GUIBuilder.BorderNode> _nodes = null;
        
        Engine.Plugin.Forms.ObjectReference _Border = null;
        
        // Build volumes for workshops
        List<Engine.Plugin.Forms.ObjectReference> _BuildVolumes = null;
        
        // Pulled from handle for quick access
        
        public string LocationName = null;
        
        #region Constructor
        
        public WorkshopScript( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            LocationName = string.Format( "0x{0} - No location", reference.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) );
        }

        #endregion

        #region Public Properties
        
        public override ConflictStatus ConflictStatus
        {
            get
            {
                var wsKeyword = GodObject.Plugin.Workspace?.GetIdentifierForm<Engine.Plugin.Forms.Keyword>( GUIBuilder.BorderBatch.WSDS_KYWD_BorderGenerator );
                if( ( _BorderGeneratorKeyword == null )||( ( wsKeyword != null )&&( wsKeyword != _BorderGeneratorKeyword ) ) )
                    _BorderGeneratorKeyword = wsKeyword;

                return
                    ( _BorderGeneratorKeyword != null )&&( GetFirstBorderMarker( _BorderGeneratorKeyword ) != null )
                    ? ConflictStatus.RequiresOverride
                    : base.ConflictStatus;
            }
        }

        public string NameFromEditorID
        {
            get
            {
                // TODO:  FIX ME FOR PROPER NAMESPACE PREFIXES AND STRING SCANNING, THIS CODE SUX!
                var foo = Reference.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
                foo = foo.StripFrom( "WorkshopREF", StringComparison.InvariantCultureIgnoreCase );
                foo = foo.StripFrom( "Workshop", StringComparison.InvariantCultureIgnoreCase );
                foo = foo.StripFrom( "Reference", StringComparison.InvariantCultureIgnoreCase );
                return foo;
            }
        }
        
        Engine.Plugin.Forms.ObjectReference BorderReference
        {
            get
            {
                if( _Border == null )
                {
                    var forms = Form.References;
                    if( forms.NullOrEmpty() )
                        return null;
                    
                    var wlbae = GodObject.CoreForms.WorkshopLinkedBuildAreaEdge;
                    if( wlbae == null )
                        return null;
                    
                    foreach( var form in forms )
                    {
                        var refr = form as Engine.Plugin.Forms.ObjectReference;
                        if( refr == null )
                            continue;
                        var refrRef = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlbae.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                        if( ( refrRef != null )&&( refrRef == Reference ) )
                        {
                            _Border = refr;
                            break;
                        }
                    }
                }
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BorderReference :: 0x{0} \"{1}\"",
                    ( _Border == null ? 0 : _Border.FormID ).ToString( "X8" ),
                    _Border == null ? "" : _Border.EditorID ) );
                */
                return _Border;
            }
            set
            {
                var wlbae = GodObject.CoreForms.WorkshopLinkedBuildAreaEdge;
                if( wlbae == null )
                    return;
                
                // Unlink the current border
                var current = BorderReference;
                if( ( current != null )&&( current != value ) )
                    current.LinkedRefs.Remove( Engine.Plugin.TargetHandle.Working, this.GetFormID( Engine.Plugin.TargetHandle.Master ), Engine.Plugin.Constant.FormID_None );
                
                // Link the new border
                if( ( value != null )&&( value != _Border ) )
                {
                    var index = value.LinkedRefs.FindKeywordIndex( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlbae.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    if( index >= 0 )
                    {   // Redirect the existing link to the new ref
                        value.LinkedRefs.SetReference( Engine.Plugin.TargetHandle.Working, index, Reference );
                    }
                }
                
                _Border = value;
            }
        }
        
        Engine.Plugin.Forms.Static BorderStatic
        {
            get
            {
                var border = BorderReference;
                if( border == null ) return null;
                //var cStatics = GodObject.Plugin.Data.Root.GetContainer<Engine.Plugin.Forms.Static>( true, false );
                //if( cStatics == null ) return null;
                //var stat = cStatics.Find<Engine.Plugin.Forms.Static>( border.Name );
                var stat = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Static>( border.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                //var stat = border == null
                //    ? null
                //    : GodObject.Plugin.Data.Statics.Find( border.Name );
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BorderStatic :: border.Name = 0x{2} :: 0x{0} \"{1}\"",
                    ( stat == null ? 0 : stat.FormID ).ToString( "X8" ),
                    ( stat == null ? "" : stat.EditorID ),
                    ( border == null ? 0 : border.Name ).ToString( "X8" ) ) );
                */
                return stat;
            }
        }
        
        public Engine.Plugin.Forms.ObjectReference SandboxVolume
        {
            get
            {
                return Reference.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.WorkshopLinkSandbox.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            }
        }
        
        public List<Engine.Plugin.Forms.ObjectReference> BuildVolumes
        {
            get
            {
                if( _BuildVolumes == null )
                {
                    var forms = Form.References;
                    if( forms.NullOrEmpty() )
                        return null;
                    
                    var wlp = GodObject.CoreForms.WorkshopLinkedPrimitive;
                    if( wlp == null )
                        return null;
                    
                    var list = new List<Engine.Plugin.Forms.ObjectReference>();
                    
                    foreach( var form in forms )
                    {
                        var refr = form as Engine.Plugin.Forms.ObjectReference;
                        if( refr == null )
                            continue;
                        var refrRef = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, wlp.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                        if( ( refrRef != null )&&( refrRef == Reference ) )
                        {
                            list.Add( refr );
                        }
                    }
                    if( !list.NullOrEmpty() )
                        _BuildVolumes = list;
                }
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.BuildVolumes :: count = {0}",
                    ( _BuildVolumes.NullOrEmpty() ? 0 : _BuildVolumes.Count ) ) );
                */
                return _BuildVolumes;
            }
        }
        
        #endregion
        
        public void BuildBorders( Engine.Plugin.Forms.Keyword borderGeneratorKeyword, Engine.Plugin.Forms.Static forcedZ, float approximateNodeLength, double angleAllowance, double slopeAllowance, bool updateMapUIData )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "BuildBorders()", this.ToString() } );
            
            ClearKeywordAndEdgeMarkers( borderGeneratorKeyword == null );
            if( ( borderGeneratorKeyword == null )||( Form == null ) )
                goto localReturnResult;
            var firstBorderNode = GetFirstBorderMarker( borderGeneratorKeyword );
            if( firstBorderNode == null )
            {
                DebugLog.WriteLine( "GetFirstBorderNode() returned null! :: BorderGeneratorKeyword = " + borderGeneratorKeyword.ToStringNullSafe() );
                goto localReturnResult;
            }
            var borderMarkerLinkKeyword = GetBorderMarkerLinkKeyword( firstBorderNode );
            if( borderMarkerLinkKeyword == null )
            {
                DebugLog.WriteLine( "GetBorderNodeKeyword() returned null! :: BorderGeneratorKeyword = " + borderGeneratorKeyword.ToStringNullSafe() );
                goto localReturnResult;
            }
            
            /*
            DebugLog.WriteLine( string.Format(
                "Fallout4.WorkshopScript.BuildBorders() :: Start :: workshop 0x{0} \"{1}\" :: workshop link keyword 0x{2} \"{3}\" :: node link keyword 0x{4} \"{5}\" :: first node 0x{6} \"{7}\"",
                this.FormID.ToString( "X8" ), this.EditorID,
                keyword.FormID.ToString( "X8" ), keyword.EditorID,
                nodeKeyword.FormID.ToString( "X8" ), nodeKeyword.EditorID,
                firstNode.FormID.ToString( "X8" ), firstNode.EditorID ) );
            */
            //_BorderGeneratorKeyword = borderGeneratorKeyword;
            //_BorderMarkerLinkKeyword = borderMarkerLinkKeyword;
            _BorderMarkers = GetBorderMarkers( firstBorderNode, borderMarkerLinkKeyword );
            _nodes = BorderNode.GenerateBorderNodes( Reference.Worldspace, _BorderMarkers, approximateNodeLength, angleAllowance, slopeAllowance, forcedZ );
            
            SendObjectDataChangedEvent( this );
            /*
            DebugLog.WriteLine( string.Format( "Fallout4.WorkshopScript.BuildBorders() :: Complete :: workshop 0x{0} \"{1}\" :: keyword 0x{2} \"{3}\"", this.FormID.ToString( "X8" ), this.EditorID, keyword.FormID.ToString( "X8" ), keyword.EditorID ) );
            DebugLog.WriteLine( string.Format(
                "Fallout4.WorkshopScript.BuildBorders() :: Complete :: workshop 0x{0} \"{1}\" :: workshop link keyword 0x{2} \"{3}\" :: node link keyword 0x{4} \"{5}\" :: first node 0x{6} \"{7}\" :: node count = {8}",
                this.FormID.ToString( "X8" ), this.EditorID,
                keyword.FormID.ToString( "X8" ), keyword.EditorID,
                nodeKeyword.FormID.ToString( "X8" ), nodeKeyword.EditorID,
                firstNode.FormID.ToString( "X8" ), firstNode.EditorID,
                _BorderMarkerNodes.NullOrEmpty() ? 0 : _BorderMarkerNodes.Count ) );
            */
        localReturnResult:
           DebugLog.CloseIndentLevel();
        }
        
        Engine.Plugin.Forms.ObjectReference GetFirstBorderMarker( Engine.Plugin.Forms.Keyword workshopBorderGeneratorKeyword )
        {
            var wsKeyword = workshopBorderGeneratorKeyword ?? GodObject.Plugin.Workspace?.GetIdentifierForm<Engine.Plugin.Forms.Keyword>( GUIBuilder.BorderBatch.WSDS_KYWD_BorderGenerator );
            if( ( _BorderGeneratorKeyword == null ) || ( ( wsKeyword != null ) && ( wsKeyword != _BorderGeneratorKeyword ) ) )
            {
                _BorderGeneratorKeyword = wsKeyword;
                _FirstBorderMarker = null;
            }

            if( _BorderGeneratorKeyword == null )
                return null;

            if( _FirstBorderMarker != null )
                return _FirstBorderMarker;

            var forms = Form.References;
            if( forms.NullOrEmpty() )
            {
                DebugLog.WriteLine( new string[] { "Workshop has no other forms referencing it" } );
                return null;
            }
            
            foreach( var form in forms )
            {
                var refr = form as Engine.Plugin.Forms.ObjectReference;
                if( refr == null )
                    continue;
                
                var lr = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, workshopBorderGeneratorKeyword.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                if( lr == null )
                    continue;
                if( lr != Reference )
                {
                    DebugLog.WriteWarning( this.GetType().ToString(), "GetFirstBorderMarker()", string.Format( "Linked Ref does not match workshop\nworkshop = {0}\nlinked ref = {1}\nreference = {2}", Reference.ToStringNullSafe(), lr.ToStringNullSafe(), refr.ToStringNullSafe() ) );
                    continue;
                }

                /*
                DebugLog.WriteLine( new string[] {
                    this.GetType().ToString(),
                    "GetFirstBorderNode()",
                    refr.ToStringNullSafe() } );
                */

                _FirstBorderMarker = refr;
                break;
            }
            return _FirstBorderMarker;
        }
        
        Engine.Plugin.Forms.Keyword GetBorderMarkerLinkKeyword( Engine.Plugin.Forms.ObjectReference firstBorderMarker )
        {
            if( firstBorderMarker == null )
                return null;

            var wsKeyword = GodObject.Plugin.Workspace?.GetIdentifierForm<Engine.Plugin.Forms.Keyword>( GUIBuilder.BorderBatch.WSDS_KYWD_BorderLink );
            if( ( _BorderMarkerLinkKeyword == null ) || ( ( wsKeyword != null ) && ( wsKeyword != _BorderGeneratorKeyword ) ) )
                _BorderMarkerLinkKeyword = wsKeyword;

            if( _BorderMarkerLinkKeyword != null )
                return _BorderMarkerLinkKeyword;

            var count = firstBorderMarker.LinkedRefs.GetCount( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            if( count < 1 )
                return null;
            
            for( int i = 0; i < count; i++ )
            {
                var refr = firstBorderMarker.LinkedRefs.GetReference( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, i );
                if( ( refr == null )||( refr == Reference ) )
                    continue;
                
                var kywd = firstBorderMarker.LinkedRefs.GetKeyword( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, i );
                if( kywd == null )
                    continue;

                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.GetBorderMarkerLinkKeyword() :: 0x{0} \"{1}\"",
                    kywd.FormID.ToString( "X8" ),
                    kywd.EditorID ) );
                */
                // *
                DebugLog.WriteLine( new string[] {
                    this.GetType().ToString(),
                    "GetBorderMarkerLinkKeyword()",
                    kywd.ToStringNullSafe() } );
                // * /


                _BorderMarkerLinkKeyword = kywd;
                break;
            }
            return _BorderMarkerLinkKeyword;
        }
        
        List<Engine.Plugin.Forms.ObjectReference> GetBorderMarkers( Engine.Plugin.Forms.ObjectReference firstMarker, Engine.Plugin.Forms.Keyword linkKeyword )
        {
            DebugLog.OpenIndentLevel( new string[] { this.GetType().ToString(), "GetBorderMarkers()", "workshop = " + this.ToStringNullSafe(), "linkKeyword = " + linkKeyword.ToStringNullSafe(), "firstMarker = " + firstMarker.ToStringNullSafe() } );
            
            var list = (List<Engine.Plugin.Forms.ObjectReference>)null;
            
            if( ( linkKeyword == null )||( firstMarker == null )||( Form == null ) )
                goto localAbort;
            
            var node = firstMarker.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, linkKeyword.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            if( node == null )
                goto localAbort;
            
            list = new List<Engine.Plugin.Forms.ObjectReference>();
            
            var closedLoop = false;
            list.Add( firstMarker );
            list.Add( node );
            while( true )
            {
                node = node.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, linkKeyword.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                if( node == null )
                    break;
                list.Add( node );
                closedLoop = ( node == firstMarker );
                if( closedLoop )
                    break;
            }
            
            if( !closedLoop )
            {
                var efCount = list.Count;
                DebugLog.WriteWarning( this.GetType().ToString(), "GetBorderMarkers()", string.Format( "Workshop border markers do not form a complete loop.\n\tWorkshop = {0}\n\tFlag count = {1}\n\tFirst = {2}\n\tLast = {3}", this.ToString(), efCount, list[ 0 ].ToString(), list[ efCount - 1 ].ToString() ) );
            }
            /*
            DebugLog.Write( string.Format(
                "Fallout4.WorkshopScript.GetBorderMarkers() :: Total count: {0}",
                list.Count ) );
            */

        localAbort:
            DebugLog.CloseIndentLevel( "nodes", list );
            return list;
        }
        
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
            bool createImportData )
        {
            DebugLog.OpenIndentLevel( string.Format( "{0} :: CreateBorderNIFs() :: workshop 0x{1} - \"{2}\"", this.GetType().ToString(), this.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), this.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
            
            List<GUIBuilder.FormImport.ImportBase> result = null;
            
            //DebugLog.WriteList( "_nodes", _nodes );
            
            if( _nodes.NullOrEmpty() )
                goto localReturnResult;
            
            var volumes = BuildVolumes;
            var volumeCeiling = float.MinValue;
            if( !volumes.NullOrEmpty() )
            {
                foreach( var volume in volumes )
                {
                    var vPos = volume.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    var vBounds = volume.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    var ceiling = vPos.Z + vBounds.Z * 0.5f;
                    if( ceiling > volumeCeiling )
                        volumeCeiling = ceiling;
                }
            }
            
            var originalForms = createImportData ? new List<Engine.Plugin.Form>() : null;
            
            var keyword = _BorderGeneratorKeyword;
            var worldspace = Reference.Worldspace;
            var workshopFID = this.GetFormID( Engine.Plugin.TargetHandle.Master );
            var workshopName = this.NameFromEditorID;
            var border = BorderReference;
            if( ( createImportData )&&( border != null ) )
                originalForms.Add( border );
            var offsetMesh = ( border != null )&&( ( !border.IsInWorkingFile() )||( !Form.IsInWorkingFile() ) );
            string forcedNIFFile = null;
            string forcedNIFPath = null;
            var stat = BorderStatic;
            if( stat != null )
            {
                if( createImportData )
                    originalForms.Add( stat );
                if( offsetMesh )
                {
                    var statFilePath = stat.GetModel( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    forcedNIFFile = GenFilePath.FilenameFromPathname( statFilePath, out forcedNIFPath );
                }
            }
            
            result = NIFBuilder.CreateNIFs(
                createImportData,
                _nodes,
                worldspace,
                null,
                Reference,
                GodObject.CoreForms.WorkshopLinkedBuildAreaEdge,
                GodObject.CoreForms.WorkshopBorderArt,
                targetPath,
                targetSuffix,
                meshSuffix,
                meshSubPath,
                filePrefix,
                workshopName,
                fileSuffix,
                "", 1,
                forcedNIFPath,
                forcedNIFFile,
                volumeCeiling,
                gradientHeight,
                groundOffset,
                groundSink,
                false,
                offsetMesh,
                offsetMesh
                    ? border.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired )
                    : Vector3f.Zero,
                NIFBuilder.Colours.InsideBorder,
                NIFBuilder.Colours.OutsideBorder,
                originalForms,
                true
                );
            
        localReturnResult:
            DebugLog.CloseIndentLevel();
            return result;
        }
        
        public void ClearKeywordAndEdgeMarkers( bool sendchangedevent )
        {
            _BorderGeneratorKeyword = null;
            _BorderMarkers = null;
            _nodes = null;
            if( sendchangedevent )
                SendObjectDataChangedEvent( this );
        }
        
    }
    
}
