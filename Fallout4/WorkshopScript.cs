﻿/*
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

using XeLib;
using XeLib.API;
using XeLibHelper;

namespace Fallout4
{
    
    /// <summary>
    /// Description of Workshop.
    /// </summary>
    [Engine.Plugin.Attributes.ScriptAssociation( /*typeof( WorkshopScript ),*/ "WorkshopScript" )]
    public class WorkshopScript : Engine.Plugin.PapyrusScript
    {
        
        // Border marker nodes for workshops
        Engine.Plugin.Forms.Keyword _WorkshopLinkKeyword = null;
        Engine.Plugin.Forms.Keyword _MarkerNodeLinkKeyword = null;
        List<Engine.Plugin.Forms.ObjectReference> _BorderMarkerNodes = null;
        List<GUIBuilder.BorderNode> _nodes = null;
        
        Engine.Plugin.Forms.ObjectReference _Border = null;
        
        // Build volumes for workshops
        List<Engine.Plugin.Forms.ObjectReference> _BuildVolumes = null;
        
        // Pulled from handle for quick access
        
        public string LocationName = null;
        
        #region Constructor
        
        public WorkshopScript( Engine.Plugin.Forms.ObjectReference reference ) : base( reference )
        {
            LocationName = string.Format( "0x{0} - No location", reference.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ) );
        }
        
        #endregion
        
        #region Public Properties
        
        public string NameFromEditorID
        {
            get
            {
                // TODO:  FIX ME FOR PROPER NAMESPACE PREFIXES AND STRING SCANNING, THIS CODE SUX!
                var foo = Reference.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
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
                        var refrRef = refr.LinkedRefs.GetLinkedRef( wlbae.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
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
                    current.LinkedRefs.Remove( this.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), Engine.Plugin.Constant.FormID_None );
                
                // Link the new border
                if( ( value != null )&&( value != _Border ) )
                {
                    var index = value.LinkedRefs.FindKeywordIndex( wlbae.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                    if( index >= 0 )
                    {   // Redirect the existing link to the new ref
                        value.LinkedRefs.Reference[ index ] = Reference;
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
        
        List<Engine.Plugin.Forms.ObjectReference> BuildVolumes
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
                        var refrRef = refr.LinkedRefs.GetLinkedRef( wlp.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
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
        
        public void BuildBorders( Engine.Plugin.Forms.Keyword keyword, Engine.Plugin.Forms.Static forcedZ, float approximateNodeLength, float slopeAllowance, bool updateMapUIData )
        {
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "BuildBorders()", this.ToString() } );
            
            ClearKeywordAndEdgeMarkers( keyword == null );
            if( ( keyword == null )||( Form == null ) )
                goto localReturnResult;
            var firstNode = GetFirstBorderNode( keyword );
            if( firstNode == null )
                goto localReturnResult;
            var nodeKeyword = GetBorderNodeKeyword( firstNode );
            if( nodeKeyword == null )
                goto localReturnResult;
            
            /*
            DebugLog.Write( string.Format(
                "Fallout4.WorkshopScript.BuildBorders() :: Start :: workshop 0x{0} \"{1}\" :: workshop link keyword 0x{2} \"{3}\" :: node link keyword 0x{4} \"{5}\" :: first node 0x{6} \"{7}\"",
                this.FormID.ToString( "X8" ), this.EditorID,
                keyword.FormID.ToString( "X8" ), keyword.EditorID,
                nodeKeyword.FormID.ToString( "X8" ), nodeKeyword.EditorID,
                firstNode.FormID.ToString( "X8" ), firstNode.EditorID ) );
            */
            
            _WorkshopLinkKeyword = keyword;
            _MarkerNodeLinkKeyword = nodeKeyword;
            _BorderMarkerNodes = GetBorderNodes( firstNode, nodeKeyword );
            _nodes = BorderNode.GenerateBorderNodes( Reference.Worldspace, _BorderMarkerNodes, approximateNodeLength, slopeAllowance, forcedZ );
            
            SendObjectDataChangedEvent();
            /*
            DebugLog.Write( string.Format( "Fallout4.WorkshopScript.BuildBorders() :: Complete :: workshop 0x{0} \"{1}\" :: keyword 0x{2} \"{3}\"", this.FormID.ToString( "X8" ), this.EditorID, keyword.FormID.ToString( "X8" ), keyword.EditorID ) );
            DebugLog.Write( string.Format(
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
        
        Engine.Plugin.Forms.ObjectReference GetFirstBorderNode( Engine.Plugin.Forms.Keyword workshopLinkKeyword )
        {
            if( workshopLinkKeyword == null )
                return null;
            
            var forms = Form.References;
            if( forms.NullOrEmpty() )
                return null;
            
            foreach( var form in forms )
            {
                var refr = form as Engine.Plugin.Forms.ObjectReference;
                if( refr == null )
                    continue;
                
                var lr = refr.LinkedRefs.GetLinkedRef( workshopLinkKeyword.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( ( lr == null )||( lr != Reference ) )
                    continue;
                
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.GetFirstBorderNode() :: 0x{0} \"{1}\"",
                    refr.FormID.ToString( "X8" ),
                    refr.EditorID ) );
                */
                
                return refr;
            }
            return null;
        }
        
        Engine.Plugin.Forms.Keyword GetBorderNodeKeyword( Engine.Plugin.Forms.ObjectReference firstNode )
        {
            if( firstNode == null )
                return null;
            
            var count = firstNode.LinkedRefs.Count;
            if( count < 1 )
                return null;
            
            for( int i = 0; i < count; i++ )
            {
                var refr = firstNode.LinkedRefs.Reference[ i ];
                if( ( refr == null )||( refr == Reference ) )
                    continue;
                
                var kywd = firstNode.LinkedRefs.Keyword[ i ];
                if( kywd == null )
                    continue;
                
                /*
                DebugLog.Write( string.Format(
                    "Fallout4.WorkshopScript.GetBorderNodeKeyword() :: 0x{0} \"{1}\"",
                    kywd.FormID.ToString( "X8" ),
                    kywd.EditorID ) );
                */
                
                return kywd;
            }
            return null;
        }
        
        List<Engine.Plugin.Forms.ObjectReference> GetBorderNodes( Engine.Plugin.Forms.ObjectReference firstNode, Engine.Plugin.Forms.Keyword nodeKeyword )
        {
            if( ( nodeKeyword == null )||( firstNode == null )||( Form == null ) )
                return null;
            
            var list = new List<Engine.Plugin.Forms.ObjectReference>();
            var node = firstNode.LinkedRefs.GetLinkedRef( nodeKeyword.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
            if( node == null )
                return null;
            
            var closedLoop = false;
            list.Add( firstNode );
            list.Add( node );
            while( true )
            {
                node = node.LinkedRefs.GetLinkedRef( nodeKeyword.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                if( node == null )
                    break;
                list.Add( node );
                closedLoop = ( node == firstNode );
                if( closedLoop )
                    break;
            }
            
            if( !closedLoop )
            {
                var efCount = list.Count;
                DebugLog.WriteWarning( this.GetType().ToString(), "GetBorderNodes()", string.Format( "Workshop EdgeFlags do not form a complete loop.\n\tWorkshop = {0}\n\tFlag count = {1}\n\tFirst = {2}\n\tLast = {3}", this.ToString(), efCount, list[ 0 ].ToString(), list[ efCount - 1 ].ToString() ) );
            }
            /*
            DebugLog.Write( string.Format(
                "Fallout4.WorkshopScript.GetBorderNodes() :: Total count: {0}",
                list.Count ) );
            */
            
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
            DebugLog.OpenIndentLevel( string.Format( "{0} :: CreateBorderNIFs() :: workshop 0x{1} - \"{2}\"", this.GetType().ToString(), this.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ), this.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
            
            List<GUIBuilder.FormImport.ImportBase> result = null;
            
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
            
            var keyword = _WorkshopLinkKeyword;
            var worldspace = Reference.Worldspace;
            var workshopFID = this.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
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
                originalForms
                );
            
        localReturnResult:
            DebugLog.CloseIndentLevel();
            return result;
        }
        
        public void ClearKeywordAndEdgeMarkers( bool sendchangedevent )
        {
            _WorkshopLinkKeyword = null;
            _BorderMarkerNodes = null;
            _nodes = null;
            if( sendchangedevent )
                SendObjectDataChangedEvent();
        }
        
    }
    
}