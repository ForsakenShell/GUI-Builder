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
            bool createImportData )
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
                true,
                NIFBuilder.ExportInfo
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
            bool createImportData )
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
                        createImportData );
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

    }

}
