/*
 * WorkshopBatch.cs
 *
 * Batch functions for workshops.
 *
 * Generally used by but not exclusive to the associated window.
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using Maths;
using Fallout4;
using AnnexTheCommonwealth;

namespace GUIBuilder
{
    /// <summary>
    /// Description of WorkshopBatch.
    /// </summary>
    public static class WorkshopBatch
    {

        public const string     WSDS_KYWD_BorderGenerator   = "WorkshopBorderGenerator";
        public const string     WSDS_KYWD_BorderLink        = "WorkshopBorderLink";
        public const string     WSDS_STAT_TerrainFollowing  = "TerrainFollowing";
        public const string     WSDS_STAT_ForcedZ           = "ForcedZ";
        public const string     WSDS_LCRT_BorderWithBottom  = "BorderWithBottom";

        public const string     WSDS_CONT_WorkshopWorkbench = "WorkshopWorkbench";

        #region Workshop Border Marker -> Border Nodes

        public static bool CalculateWorkshopBorderMarkerNodes(
            List<WorkshopScript> workshops,
            Engine.Plugin.Forms.Keyword workshopBorderGeneratorKeyword,
            Engine.Plugin.Forms.Keyword workshopBorderLinkKeyword,
            Engine.Plugin.Forms.Static forcedZ,
            Engine.Plugin.Forms.LocationRef borderWithBottomRef,
            float nodeLength,
            double angleAllowance,
            double slopeAllowance,
            bool updateMapUIData )
        {
            bool result = false;

            DebugLog.OpenIndentLevel();

            if( workshopBorderGeneratorKeyword == null )
            {
                DebugLog.WriteError( string.Format( "Keyword:  {0} = null!", GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator ) );
                goto localAbort;
            }
            if( workshopBorderLinkKeyword == null )
            {
                DebugLog.WriteError( string.Format( "Keyword:  {0} = null!", GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink ) );
                goto localAbort;
            }

            if( workshops.NullOrEmpty() )
            {
                DebugLog.WriteLine( "No workshops selected" );
                goto localAbort;
            }

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            foreach( var workshop in workshops )
            {
                m.SetCurrentStatusMessage( string.Format( "BorderBatch.CalculatingBordersFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                result |= GenerateBorderNodesFromMarkers( workshop, nodeLength, angleAllowance, slopeAllowance, updateMapUIData );
            }

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
        localAbort:
            DebugLog.CloseIndentLevel();
            return result;
        }

        public static bool GenerateBorderNodesFromMarkers(
            WorkshopScript workshop,
            float approximateNodeLength,
            double angleAllowance,
            double slopeAllowance,
            bool updateMapUIData )
        {
            bool result = false;

            DebugLog.OpenIndentLevel( workshop?.IDString );

            if( workshop == null )
                goto localAbort;

            workshop.ClearBorderMarkersAndNodes( false );

            var borderMarkers = workshop.GetBorderMarkers();
            var nodes = BorderNode.GenerateBorderNodes( workshop.Reference.Worldspace, borderMarkers, approximateNodeLength, angleAllowance, slopeAllowance, GUIBuilder.CustomForms.WorkshopForcedZMarker, GUIBuilder.CustomForms.WorkshopBorderWithBottomRef );

            result = !nodes.NullOrEmpty();
            if( result )
                workshop.BorderNodes = nodes;

            workshop.SendObjectDataChangedEvent( workshop );

        localAbort:
            DebugLog.CloseIndentLevel();
            return result;
        }

        #endregion

        #region Workshop Border Nodes -> Border NIF

        public static List<GUIBuilder.FormImport.ImportBase> CreateNIF(
            WorkshopScript workshop,
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
            DebugLog.OpenIndentLevel( workshop?.IDString );

            List<GUIBuilder.FormImport.ImportBase> result = null;

            if( workshop == null )
                goto localAbort;

            //DebugLog.WriteList( "_nodes", _nodes );

            var borderNodes = workshop.BorderNodes;
            if( borderNodes.NullOrEmpty() )
                goto localAbort;

            var volumes = workshop.BuildVolumes;
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

            //var keyword = GUIBuilder.CustomForms.WorkshopBorderGeneratorKeyword;
            var worldspace = workshop.Reference.Worldspace;
            //var workshopFID = this.GetFormID( Engine.Plugin.TargetHandle.Master );
            var workshopName = workshop.NameFromEditorID;
            var border = workshop.BorderReference;
            if( ( createImportData ) && ( border != null ) )
                originalForms.Add( border );
            var offsetMesh = ( border != null )&&( ( !border.IsInWorkingFile() )||( !workshop.IsInWorkingFile() ) );
            string forcedNIFFile = null;
            string forcedNIFPath = null;
            var stat = workshop.BorderStatic;
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
                borderNodes,
                worldspace,
                null,
                workshop.Reference,
                GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedBuildAreaEdge,
                GodObject.CoreForms.Fallout4.Layer.WorkshopBorderArt,
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
                true,
                NIFBuilder.ExportInfo,
                highPrecisionVertexes
                );

        localAbort:
            DebugLog.CloseIndentLevel();
            return result;
        }

        public static List<GUIBuilder.FormImport.ImportBase> BuildNIFs(
            string borderSetName,
            List<Fallout4.WorkshopScript> workshops,
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
            if(
                ( workshops.NullOrEmpty() ) ||
                ( string.IsNullOrEmpty( targetPath ) )
            )
                return null;

            DebugLog.OpenIndentLevel();

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();

            List<GUIBuilder.FormImport.ImportBase> list = null;

            try
            {

                foreach( var workshop in workshops )
                {   // Build a workshop border
                    m.SetCurrentStatusMessage( string.Format( "BorderBatch.CreateNIFsFor".Translate(), borderSetName, workshop.IDString ) );
                    var subList = WorkshopBatch.CreateNIF(
                        workshop,
                        gradientHeight, groundOffset, groundSink,
                        targetPath, targetSuffix,
                        meshSuffix, meshSubPath,
                        filePrefix, fileSuffix,
                        createImportData,
                        highPrecisionVertexes );
                    if( ( createImportData ) && ( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
            }
            catch( Exception e )
            {
                DebugLog.WriteException( e );
            }

            m.StopSyncTimer( tStart, borderSetName );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();

            return list;
        }

        #endregion

        #region Generate and Optimize Workshop Elements

        public static void CheckMissingElements( List<Fallout4.WorkshopScript> workshops, bool checkBorderEnablers, bool checkSandboxVolumes )
        {
            /*
            DebugLog.OpenIndentLevel();

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( string.Format( "ControllerBatch.CheckingElements".Translate(), "Controller.WorkshopL".Translate() ) );

            List<GUIBuilder.FormImport.ImportBase> list = null;

            if( checkBorderEnablers )
                GenerateMissingBorderEnablers( ref list, workshops, m );
            if( checkSandboxVolumes )
                GenerateSandboxes( ref list, workshops, m, true, true );

            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, true, ref allImportsMatchTarget );

            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
            */
        }

        public static void GenerateSandboxes( ref List<FormImport.ImportBase> list, List<Fallout4.WorkshopScript> workshops, GUIBuilder.Windows.Main m, bool createMissing, bool ignoreExisting )
        {
            if( ( !createMissing ) && ( ignoreExisting ) )
                return; // So, uh...do nothing, der?

            DebugLog.OpenIndentLevel();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "ControllerBatch.CalculatingSandboxes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();

            foreach( var workshop in workshops )
            {
                m.PushStatusMessage();
                msg = string.Format( "ControllerBatch.CheckingSandboxFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );

                var borderMarkers = workshop.GetBorderMarkers();
                var sandbox = workshop.SandboxVolume;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tWorkshop = {0}", workshop, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                if(
                    ( ( sandbox != null ) && ( ignoreExisting ) ) ||
                    ( ( sandbox == null ) && ( !createMissing ) ) ||
                    ( borderMarkers.NullOrEmpty() )
                )
                {
                    m.PopStatusMessage();
                    continue;
                }

                DebugLog.OpenIndentLevel( workshop.IDString, false );

                msg = string.Format( "ControllerBatch.CalculatingSandboxFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();

                var hintZ = 0.0f;
                var buildVolumes = workshop.BuildVolumes;
                if( !buildVolumes.NullOrEmpty() )
                {
                    foreach( var volume in buildVolumes )
                        hintZ += volume.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z;
                    hintZ /= buildVolumes.Count;
                }
                else if( sandbox != null )
                    hintZ = sandbox.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z;
                else
                    hintZ = workshop.Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z;

                // Use edge flag reference points instead of build volumes so we can work with less points that are accurate enough
                var points = new List<Vector2f>();
                foreach( var marker in borderMarkers )
                    points.Add( new Vector2f( marker.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );

                var hull = Maths.Geometry.ConvexHull.MakeConvexHull( points );

                var osv = VolumeBatch.CalculateOptimalSandboxVolume(
                    hull,
                    workshop.Reference.Worldspace,
                    false,
                    GodObject.CoreForms.Fallout4.fSandboxCylinderBottom,
                    GodObject.CoreForms.Fallout4.fSandboxCylinderTop,
                    128.0f, 128.0f,
                    hintZ
                );

                if( osv == null )
                    DebugLog.WriteLine( string.Format( "Unable to calculate sandbox for {0}", workshop.ToString() ) );
                else
                {
                    DebugLog.WriteStrings( null, new[] {
                        string.Format(
                            "Position = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                            osv.Size.ToString() ),
                        string.Format(
                            "Size = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                            osv.Position.ToString() ),
                        string.Format(
                            "Z Rotation = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z.ToString(),
                            osv.Rotation.Z.ToString() )
                        }, false, true, false, false );
                    var w = workshop.Reference.Worldspace;
                    var c = w == null
                        ? workshop.Reference.Cell
                        : workshop.Reference.Worldspace.Cells.Persistent;
                    var sandboxBase = sandbox == null
                        ? GodObject.CoreForms.Fallout4.Activator.DefaultDummy
                        : sandbox.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    var color = sandbox == null
                        ? System.Drawing.Color.FromArgb( 255, 0, 0 )
                        : sandbox.Primitive.GetColor( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    FormImport.ImportBase.AddToList(
                        ref list,
                        new FormImport.ImportSandboxReference(
                            sandbox,
                            string.Format(
                                "{0}{1}",
                                workshop.NameFromEditorID,
                                "SandboxArea" ),
                            sandboxBase,
                            w, c,
                            osv.Position,
                            osv.Rotation,
                            osv.Size,
                            color,
                            workshop.Reference,
                            GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkSandbox,
                            workshop.Reference.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                            FormImport.ImportSandboxReference.FO4_TARGET_RECORD_FLAGS
                    ) );
                }
                var elapsed = m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                DebugLog.CloseIndentLevel( elapsed );
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        public static void NormalizeBuildVolumes( ref List<FormImport.ImportBase> list, List<Fallout4.WorkshopScript> workshops, GUIBuilder.Windows.Main m, bool missingOnly )
        {
            DebugLog.OpenIndentLevel();

            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "ControllerBatch.CheckingBuildVolumes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();

            foreach( var workshop in workshops )
            {
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                msg = string.Format( "ControllerBatch.CheckingBuildVolumesFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );

                var volumes = workshop.BuildVolumes;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                //if( ( volumes.NullOrEmpty() )&&( missingOnly ) )
                if( volumes.NullOrEmpty() )
                {
                    m.StopSyncTimer( tStart, workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                    m.PopStatusMessage();
                    continue;
                }
                var borderMarkers = workshop.GetBorderMarkers();
                if( borderMarkers.NullOrEmpty() )
                {
                    m.StopSyncTimer( tStart, workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                    m.PopStatusMessage();
                    continue;
                }

                msg = string.Format( "ControllerBatch.NormalizingBuildVolumesFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );

                // Use edge flag reference points instead of build volumes so we can work with less points that are accurate enough
                var points = new List<Vector2f>();
                foreach( var marker in borderMarkers )
                    points.Add( new Vector2f( marker.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );

                var hull = Maths.Geometry.ConvexHull.MakeConvexHull( points );

                var color = System.Drawing.Color.FromArgb( 128, 0, 255 );

                VolumeBatch.NormalizeBuildVolumes(
                    ref list,
                    workshop.NameFromEditorID,   // FIX ME!
                    hull,
                    volumes,
                    workshop.Reference.Worldspace,
                    workshop.Reference,
                    GodObject.CoreForms.Fallout4.Keyword.WorkshopLinkedPrimitive,
                    GodObject.CoreForms.Fallout4.Activator.DefaultDummy,
                    color,
                    GUIBuilder.FormImport.ImportBuildVolumeReference.FO4_TARGET_RECORD_FLAGS,
                    -512.0f, 512.0f
                ); ;

                m.StopSyncTimer( tStart, workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.PopStatusMessage();
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        #endregion

    }

}
