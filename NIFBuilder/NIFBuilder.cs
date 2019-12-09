/*
 * CreateNIFs.cs
 *
 * Entry point to create a set of border NIFs from raw data.
 *
 * User: 1000101
 * Date: 02/02/2019
 * Time: 11:21 AM
 * 
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Maths;
using GUIBuilder;

public static partial class NIFBuilder
{
    
    public static class Colours
    {
        public static uint[] InsideBorder     = { 0x00000000, 0xFF00FF00, 0xFF00FF00 };
        public static uint[] OutsideBorder    = { 0x00000000, 0xFF003FFF, 0xFF003FFF };
        
        public static uint[] InsideSandbox    = { 0x00000000, 0xFFFF00FF, 0xFFFF00FF };
        public static uint[] OutsideSandbox   = { 0x00000000, 0xFF007FFF, 0xFF007FFF };
    }
    
    public static List<GUIBuilder.FormImport.ImportBase> CreateNIFs(
            bool createImportData,
            List<BorderNode> allNodes,
            Engine.Plugin.Forms.Worldspace worldspace,
            AnnexTheCommonwealth.BorderEnabler enablerReference,
            Engine.Plugin.Forms.ObjectReference linkRef,
            Engine.Plugin.Forms.Keyword linkKeyword,
            Engine.Plugin.Forms.Layer layer,
            string targetPath,
            string targetSuffix,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string location,
            string fileSuffix,
            string neighbour,
            int borderIndex,
            string forcedNIFPath,
            string forcedNIFFile,
            float volumeCeiling,
            float gradientHeight,
            float groundOffset,
            float groundSink,
            bool splitMeshes,
            bool offsetMesh,
            Vector3f originalMeshPosition,
            uint[] insideColours,
            uint[] outsideColours,
            List<Engine.Plugin.Form> originalForms )
    {
        DebugLog.OpenIndentLevel( new string [] { "NIFBuilder", "CreateNIFs()",
            Mesh.BuildTargetPath( targetPath, targetSuffix ),
            gradientHeight.ToString(), groundOffset.ToString(), groundSink.ToString(),
            location, neighbour,
            createImportData.ToString()
            } );
        
        List<GUIBuilder.FormImport.ImportBase> list = null;
        
        if(
            ( string.IsNullOrEmpty( targetPath ) )||
            ( string.IsNullOrEmpty( location ) )||
            ( allNodes.NullOrEmpty() )||
            ( allNodes.Count < 2 )||
            ( worldspace == null )||
            (
                ( enablerReference == null )&&
                (
                    ( linkRef == null )&&
                    ( linkKeyword == null )
                )
            )||
            (
                ( splitMeshes )&&
                ( !string.IsNullOrEmpty( forcedNIFFile) )
            )
        )   goto localAbort;
        
        // If enablerKeyword is null then the enabler is linked to the border reference as it's enable parent (XESP field of the reference), ie: sub-division borders
        // otherwise, the border reference is linked to the enable parent as a standard linked reference using the keyword, ie: workshop borders
        
        BorderNodeGroup.DumpGroupNodes( allNodes, "Pre-clone nodes:" );
        
        // Clone the list and the nodes so we don't corrupt the original data
        var clonedNodeList = new List<BorderNode>();
        foreach( var node in allNodes )
            clonedNodeList.Add( node.Clone() );
        
        BorderNodeGroup.DumpGroupNodes( clonedNodeList, "Post-clone nodes:" );
        
        List<BorderNodeGroup> nodeGroups = null;
        if( !splitMeshes )
        {
            var meshCentre = offsetMesh
                ? originalMeshPosition
                : BorderNode.Centre( clonedNodeList, true );
            
            var centreCell = Engine.SpaceConversions.WorldspaceToCellGrid( meshCentre.X, meshCentre.Y );
            BorderNode.CentreNodes( clonedNodeList, meshCentre );
            
            nodeGroups = new List<BorderNodeGroup>();
            var nodeGroup = new BorderNodeGroup(
                centreCell,
                clonedNodeList,
                meshSuffix, meshSubPath,
                filePrefix, location,
                fileSuffix, borderIndex,
                neighbour, -1,
                forcedNIFPath, forcedNIFFile
                );
            nodeGroup.Placement = new Vector3f( meshCentre );
            nodeGroups.Add( nodeGroup );
        }
        else
        {
            nodeGroups = BorderNodeGroup.SplitAcrossCells(
                clonedNodeList,
                meshSuffix, meshSubPath,
                filePrefix, location,
                fileSuffix, borderIndex,
                neighbour, 0 );
            foreach( var group in nodeGroups )
                group.CentreAndPlaceNodes();
        }
        
        if( nodeGroups.NullOrEmpty() )
        {
            DebugLog.WriteLine( "No Node Groups!" );
            goto localAbort;
        }
        
        for( int i = 0; i < nodeGroups.Count; i++ )
        {
            var group = nodeGroups[ i ];
            BorderNodeGroup.DumpGroupNodes( group.Nodes, "Group[ " + i + " ] Nodes:" );
            
            if( group.BuildMesh( gradientHeight, groundOffset, groundSink, insideColours, outsideColours ) )
            {
                group.Mesh.Write( targetPath, targetSuffix );
                if( createImportData )
                {
                    var keys = string.IsNullOrEmpty( forcedNIFFile )
                        ? Mesh.MatchKeys( location, neighbour, group.BorderIndex, group.NIFIndex )
                        : Mesh.MatchKeys( group.Mesh.nifFile );
                    
                    // Create an import for the Static Object
                    var orgStat = group.BestStaticFormFromOriginalsFor( originalForms, keys, true );
                    var statFormID = orgStat == null ? Engine.Plugin.Constant.FormID_None : orgStat.GetFormID( Engine.Plugin.TargetHandle.Master );
                    var statEditorID = group.Mesh.nifFile;
                    var minBounds = group.MinBounds; // Nodes are terrain following and their bounds will be
                    var maxBounds = group.MaxBounds; // the elevation differences from the center (placement)
                    minBounds.Z -= (int)groundSink;  // point, so add the appropriate offsets for the mesh
                    maxBounds.Z += (int)( groundOffset + gradientHeight );
                    GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportBorderStatic( orgStat, statEditorID, group.NIFFilePath, minBounds, maxBounds ) );
                    
                    // Create an import for the Object Reference
                    var orgRefr = group.BestObjectReferenceFromOriginalsFor( originalForms, statFormID, true );
                    var cell = worldspace.Cells.GetByGrid( group.Cell );
                    GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportBorderReference( orgRefr, statFormID, statEditorID, worldspace, cell, group.Placement, enablerReference, linkRef, linkKeyword, layer ) );
                }
            }
        }
        
    localAbort:
        DebugLog.CloseIndentLevel( "Imports", list );
        return list;
    }
    
}
