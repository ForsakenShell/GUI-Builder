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

using Maths;
using GUIBuilder;

using SetEditorID = GUIBuilder.FormImport.Operations.SetEditorID;
using Operations = GUIBuilder.FormImport.Operations;
using Priority = GUIBuilder.FormImport.Priority;



public static partial class NIFBuilder
{
    
    public static class Colours
    {
        public static uint[] InsideBorder     = { 0x00000000, 0xFF00FF00, 0xFF00FF00 };
        public static uint[] OutsideBorder    = { 0x00000000, 0xFF003FFF, 0xFF003FFF };
        
        public static uint[] InsideSandbox    = { 0x00000000, 0xFFFF00FF, 0xFFFF00FF };
        public static uint[] OutsideSandbox   = { 0x00000000, 0xFF007FFF, 0xFF007FFF };
    }

    public const uint       F4_BORDER_STATIC_RECORD_FLAGS = 0;

    public const uint       ATC_BORDER_STATIC_RECORD_FLAGS =
        (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD;

    public const uint       F4_BORDER_REFERENCE_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled;

    public const uint       ATC_BORDER_REFERENCE_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.IsFullLOD |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.LODRespectsEnableState |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.VisibleWhenDistant;

    static string[] DefaultExportInfo = new string[ 4 ]{
        string.Format(
            "{0} v{1} {2}",
            "AboutWindow.ComicTitle".Translate(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            "AboutWindow.Author".Translate()
        ),
        "AboutWindow.AuthorLink".Translate(),
        "NIFBuilder",
        "AboutWindow.License".Translate()
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
            string[] exportInfo,
            bool highPrecisionVertexes )
    {
        DebugLog.OpenIndentLevel(
            new string [] {
                "Target Path = \"" + Mesh.BuildTargetPath( targetPath, targetSuffix ) + "\"",
                "gradientHeight = " + gradientHeight.ToString(),
                "groundOffset = " + groundOffset.ToString(),
                "groundSink = " + groundSink.ToString(),
                "location = \"" + location + "\"",
                "neighbour = \"" + neighbour + "\"",
                "createImportData = " + createImportData.ToString(),
                "highPrecisionVertexes = " + highPrecisionVertexes.ToString()
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
            
            if( group.BuildMesh( gradientHeight, groundOffset, groundSink, insideColours, outsideColours, highPrecisionVertexes ) )
            {
                group.Mesh.Write( targetPath, targetSuffix, exportInfo );
                if( createImportData )
                {
                    var keys = string.IsNullOrEmpty( forcedNIFFile )
                        ? Mesh.MatchKeys( location, neighbour, group.BorderIndex, group.NIFIndex )
                        : Mesh.MatchKeys( group.Mesh.nifFile );

                    // Create an import for the Static Object
                    
                    uint recordFlags = workshopBorder
                        ? F4_BORDER_STATIC_RECORD_FLAGS
                        : ATC_BORDER_STATIC_RECORD_FLAGS;

                    var orgStat = group.BestStaticFormFromOriginalsFor( originalForms, keys, true );
                    var statFormID = orgStat == null ? Engine.Plugin.Constant.FormID_Invalid : orgStat.GetFormID( Engine.Plugin.TargetHandle.Master );
                    var statEditorID = group.Mesh.nifFile;
                    var minBounds = group.MinBounds; // Nodes are terrain following and their bounds will be
                    var maxBounds = group.MaxBounds; // the elevation differences from the center (placement)
                    minBounds.Z -= (int)groundSink;  // point, so add the appropriate offsets for the mesh
                    maxBounds.Z += (int)( groundOffset + gradientHeight );

                    CreateBorderStaticImport( ref list,
                        workshopBorder,
                        statEditorID,
                        orgStat,
                        group.NIFFilePath,
                        minBounds,
                        maxBounds );

                    //GUIBuilder.FormImport.ImportBase.AddToList( ref list, new GUIBuilder.FormImport.ImportBorderStatic( orgStat, statEditorID, group.NIFFilePath, minBounds, maxBounds, recordFlags ) );

                    // Create an import for the Object Reference
                    var orgRefr = group.BestObjectReferenceFromOriginalsFor( originalForms, statFormID, true );

                    CreateBorderReferenceImport( ref list,
                        workshopBorder,
                        orgStat,
                        statEditorID,
                        orgRefr,
                        (
                            ( worldspace == null )
                            ? orgRefr?.Cell
                            : worldspace.Cells.GetByGrid( group.Cell )
                        ),
                        group.Placement,
                        layer,
                        (
                            ( layer == null )
                            ? null
                            : layer.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired )
                        ),
                        enablerReference,
                        linkRef, linkKeyword );
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

    static void CreateBorderStaticImport( ref List<GUIBuilder.FormImport.ImportBase> list,
        bool workshopBorder,
        string statEditorID,
        Engine.Plugin.Forms.Static orgStat,
        string NIFFilePath,
        Vector3i minBounds,
        Vector3i maxBounds
        )
    {
        uint recordFlags = workshopBorder
                        ? F4_BORDER_STATIC_RECORD_FLAGS
                        : ATC_BORDER_STATIC_RECORD_FLAGS;

        var import = new GUIBuilder.FormImport.ImportBase(
                        "Border Static",
                        Priority.Form_Static,
                        false,
                        typeof( Engine.Plugin.Forms.Static ),
                        orgStat,
                        statEditorID );

        import.AddOperation( new Operations.SetRecordFlags( import, recordFlags ) );

        import.AddOperation( new Operations.SetEditorID( import, statEditorID ) );

        import.AddOperation( new Operations.SetStaticObjectModels( import,
            NIFFilePath,
            (
                ( ( recordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD ) == 0 )
                ? null
                : new string[] { NIFFilePath, NIFFilePath, NIFFilePath, NIFFilePath }
            ) ) );

        import.AddOperation( new Operations.SetStaticObjectBounds( import,
            minBounds,
            maxBounds
            ) );
        
        GUIBuilder.FormImport.ImportBase.AddToList(
            ref list,
            import );
    }

    static void CreateBorderReferenceImport( ref List<GUIBuilder.FormImport.ImportBase> list,
        bool workshopBorder,
        Engine.Plugin.Forms.Static baseStat,
        string statEditorID,
        Engine.Plugin.Forms.ObjectReference orgRefr,
        Engine.Plugin.Forms.Cell cell,
        Vector3f position,
        Engine.Plugin.Forms.Layer layer,
        string layerEditorID,
        AnnexTheCommonwealth.BorderEnabler enablerReference,
        Engine.Plugin.Forms.ObjectReference linkedRef,
        Engine.Plugin.Forms.Keyword linkKeyword
        )
    {
        var recordFlags = workshopBorder
            ? F4_BORDER_REFERENCE_RECORD_FLAGS
            : ATC_BORDER_REFERENCE_RECORD_FLAGS;

        var import = new GUIBuilder.FormImport.ImportBase(
            "Border Reference",
            Priority.Ref_Border,
            false,
            orgRefr,
            statEditorID,
            cell );

        import.AddOperation( new Operations.SetRecordFlags( import, recordFlags ) );
        
        if( baseStat != null )
            import.AddOperation( new Operations.SetReferenceBaseForm( import, baseStat ) );
        else if( !string.IsNullOrEmpty( statEditorID ) )
            import.AddOperation( new Operations.SetReferenceBaseForm( import, statEditorID ) );
        
        import.AddOperation( new Operations.SetReferencePosition( import, position ) );
        
        if( layer != null )
            import.AddOperation( new Operations.SetReferenceLayer( import, layer ) );
        else if( !string.IsNullOrEmpty( layerEditorID ) )
            import.AddOperation( new Operations.SetReferenceLayer( import, layerEditorID ) );
        
        if( enablerReference != null )
        {
            import.AddOperation( new Operations.SetReferenceEnableParent( import,
                enablerReference?.Reference,
                0x00000000,
                EnableParentChanged
            ) );
        }
        
        if( ( linkedRef != null ) && ( linkKeyword != null ) )
        {
            import.AddOperation( new Operations.SetReferenceLinkedRef( import,
                linkedRef, linkKeyword
            ) );
        }
        
        import.AddOperation( new Operations.SetReferenceLocationReference( import ) );

        GUIBuilder.FormImport.ImportBase.AddToList(
            ref list,
            import );

    }

    static bool EnableParentChanged( Engine.Plugin.Forms.ObjectReference reference, bool linked )
    {   // TODO:  Put this somewhere more appropriate, it will likely be used by other imports
        
        var borderEnabler = reference.GetScript<AnnexTheCommonwealth.BorderEnabler>();
        if( borderEnabler == null ) return true;
        
        var baseStatic = reference.GetName<Engine.Plugin.Forms.Static>( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
        if( linked )
        {
            borderEnabler.AddBaseFormAsNIFReference( baseStatic );
            borderEnabler.AddPlacedNIFReference( reference );
        }
        else
        {
            borderEnabler.RemoveBaseFormAsNIFReference( baseStatic );
            borderEnabler.RemovePlacedNIFReference( reference );
        }
        
        borderEnabler.SendObjectDataChangedEvent( borderEnabler );

        return true;
    }

}
