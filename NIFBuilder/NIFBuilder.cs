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

    static string[] DefaultExportInfo = new string[ 4 ]{
        string.Format( "GUIBuilder {0} by 1000101", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() ),
        "https://www.nexusmods.com/users/106891",
        "NIFBuilder",
        "STAHP LOOKING AT ME!"
    };

    static string[] _ExportInfo;
    public static string[] ExportInfo
    {
        get
        {
            if( _ExportInfo == null )
            {
                _ExportInfo = new string[ 4 ];
                for( int i = 0; i < 4; i++ )
                    _ExportInfo[ i ] = GodObject.XmlConfig.ReadValue<String>(
                        GodObject.XmlConfig.XmlNode_NIF_ExportInfo,
                        string.Format( GodObject.XmlConfig.XmlKey_NIF_ExportInfo, i ),
                        DefaultExportInfo[ i ] );
            }
            return _ExportInfo;
        }
        set
        {
            _ExportInfo = null;
            if( value.NullOrEmpty() )
                GodObject.XmlConfig.RemoveNode( GodObject.XmlConfig.XmlNode_NIF_ExportInfo );
            else
                for( int i = 0; i < 4; i++ )
                    GodObject.XmlConfig.WriteValue<String>(
                        GodObject.XmlConfig.XmlNode_NIF_ExportInfo,
                        string.Format( GodObject.XmlConfig.XmlKey_NIF_ExportInfo, i ),
                        value[ i ],
                        false );
            GodObject.XmlConfig.Commit();
        }
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
            List<Engine.Plugin.Form> originalForms,
            bool workshopBorder,
            string[] exportInfo )
    {
        DebugLog.OpenIndentLevel(
            new string [] {
                "Target Path = \"" + Mesh.BuildTargetPath( targetPath, targetSuffix ) + "\"",
                "gradientHeight = " + gradientHeight.ToString(),
                "groundOffset = " + groundOffset.ToString(),
                "groundSink = " + groundSink.ToString(),
                "location = \"" + location + "\"",
                "neighbour = \"" + neighbour + "\"",
                "createImportData = " + createImportData.ToString()
            },
            true, true, false );
        
        List<GUIBuilder.FormImport.ImportBase> list = null;

        if( string.IsNullOrEmpty( targetPath ) )
        {
            DebugLog.WriteLine( "No targetPath!" );
            goto localAbort;
        }

        if( string.IsNullOrEmpty( location ) )
        {
            DebugLog.WriteLine( "No location!" );
            goto localAbort;
        }

        if( allNodes.NullOrEmpty() )
        {
            DebugLog.WriteLine( "No nodes!" );
            goto localAbort;
        }

        if( allNodes.Count < 2 )
        {
            BorderNodeGroup.DumpGroupNodes( allNodes, "Not enough nodes!" );
            goto localAbort;
        }

        if( worldspace == null )
        {
            DebugLog.WriteLine( "No worldspace!  (Do we even need one?)" );
            goto localAbort;
        }

        if(
            ( splitMeshes ) &&
            ( !string.IsNullOrEmpty( forcedNIFFile ) )
        ){
            DebugLog.WriteLine( "Cannot split meshes with a forcedNIFFile!" );
            goto localAbort;
        }

        if(
            ( enablerReference == null ) &&
            (
                ( linkRef == null ) &&
                ( linkKeyword == null )
            )
        ){
            DebugLog.WriteLine( "No enablerReference or, linkRef and linkKeyword!" );
            goto localAbort;
        }
        
        // If enablerKeyword is null then the enabler is linked to the border reference as it's enable parent (XESP field of the reference), ie: sub-division borders
        // otherwise, the border reference is linked to the enable parent as a standard linked reference using the keyword, ie: workshop borders
        
        //BorderNodeGroup.DumpGroupNodes( allNodes, "Pre-clone nodes:" );
        
        // Clone the list and the nodes so we don't corrupt the original data
        var clonedNodeList = new List<BorderNode>();
        foreach( var node in allNodes )
            clonedNodeList.Add( node.Clone() );
        
        //BorderNodeGroup.DumpGroupNodes( clonedNodeList, "Post-clone nodes:" );
        
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
                group.Mesh.Write( targetPath, targetSuffix, exportInfo );
                if( createImportData )
                {
                    var keys = string.IsNullOrEmpty( forcedNIFFile )
                        ? Mesh.MatchKeys( location, neighbour, group.BorderIndex, group.NIFIndex )
                        : Mesh.MatchKeys( group.Mesh.nifFile );

                    // Create an import for the Static Object
                    
                    uint recordFlags = workshopBorder
                        ? GUIBuilder.FormImport.ImportBorderStatic.F4_BORDER_RECORD_FLAGS
                        : GUIBuilder.FormImport.ImportBorderStatic.ATC_BORDER_RECORD_FLAGS;

                    var orgStat = group.BestStaticFormFromOriginalsFor( originalForms, keys, true );
                    var statFormID = orgStat == null ? Engine.Plugin.Constant.FormID_None : orgStat.GetFormID( Engine.Plugin.TargetHandle.Master );
                    var statEditorID = group.Mesh.nifFile;
                    var minBounds = group.MinBounds; // Nodes are terrain following and their bounds will be
                    var maxBounds = group.MaxBounds; // the elevation differences from the center (placement)
                    minBounds.Z -= (int)groundSink;  // point, so add the appropriate offsets for the mesh
                    maxBounds.Z += (int)( groundOffset + gradientHeight );
                    
                    GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportBorderStatic( orgStat, statEditorID, group.NIFFilePath, minBounds, maxBounds, recordFlags ) );

                    // Create an import for the Object Reference
                    recordFlags = workshopBorder
                        ? GUIBuilder.FormImport.ImportBorderReference.F4_BORDER_RECORD_FLAGS
                        : GUIBuilder.FormImport.ImportBorderReference.ATC_BORDER_RECORD_FLAGS;
                    
                        var orgRefr = group.BestObjectReferenceFromOriginalsFor( originalForms, statFormID, true );
                    var cell = worldspace.Cells.GetByGrid( group.Cell );
                    GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportBorderReference( orgRefr, statFormID, statEditorID, worldspace, cell, group.Placement, enablerReference, linkRef, linkKeyword, layer, recordFlags ) );
                }
            }
            else
            {
                DebugLog.WriteError( "Could not build NIF for node group " + i );
            }
        }
        
    localAbort:
        DebugLog.CloseIndentList( "Imports", list, false, true, true );
        return list;
    }
    
}
