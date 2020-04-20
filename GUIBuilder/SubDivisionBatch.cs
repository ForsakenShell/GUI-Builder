/*
 * SubDivisionBatch.cs
 *
 * Batch functions for sub-divisions.
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

using EditorIDFormatter = GUIBuilder.CustomForms.EditorIDFormats;
using Operations = GUIBuilder.FormImport.Operations;
using Priority = GUIBuilder.FormImport.Priority;
using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder
{
    /// <summary>
    /// Description of SubDivisionBatch.
    /// </summary>
    public static class SubDivisionBatch
    {
        
        #region Sub-Division Edge Flags -> Border Nodes
        
        public static bool ClearEdgeFlagNodes( List<SubDivision> subdivisions )
        {
            if( subdivisions.NullOrEmpty() )
                return false;
            
            DebugLog.OpenIndentLevel();
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            
            foreach( var subdivision in subdivisions )
            {
                m.SetCurrentStatusMessage( string.Format( "SubDivisionBatch.ClearingBordersFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                subdivision.ClearBorderEnablerEdgeFlags( true );
            }
            
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
            return true;
        }
        
        public static bool CalculateSubDivisionEdgeFlagNodes(
            List<SubDivision> subdivisions,
            float nodeLength,
            double angleAllowance,
            double slopeAllowance,
            bool updateMapUIData )
        {
            if(
                subdivisions.NullOrEmpty() ||
                nodeLength <= 0.0f
            )
                return false;
            
            DebugLog.OpenIndentLevel();
            
            var result = false;
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            GenerateMissingBorderEnablers( ref list, subdivisions, m );
            
            if( !list.NullOrEmpty() )
            {
                bool allImportsMatchTarget = false;
                while( !allImportsMatchTarget )
                {
                    GodObject.Plugin.Data.BorderEnablers.SupressObjectDataChangedEvents();
                    FormImport.ImportBase.ShowImportDialog( list, false, ref allImportsMatchTarget );
                    GodObject.Plugin.Data.BorderEnablers.ResumeObjectDataChangedEvents( true );
                    if( !allImportsMatchTarget )
                    {
                        var msg = "SubDivisionBatch.WarnMissingEnablers.Body".Translate();
                        var retry = MessageBox.Show(
                            msg,
                            "SubDivisionBatch.WarnMissingEnablers.Title".Translate(),
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Exclamation );
                        if( retry == DialogResult.Cancel )
                        {
                            DebugLog.WriteLine( "Abort on missing border enablers" );
                            goto localReturnResult;
                        }
                    }
                }
            }
            
            foreach( var subdivision in subdivisions )
            {
                m.SetCurrentStatusMessage( string.Format( "BorderBatch.CalculatingBordersFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                subdivision.BuildSegmentsFromEdgeFlags( nodeLength, angleAllowance, slopeAllowance, updateMapUIData );
            }
            
            result = true;
            
        localReturnResult:
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel( "result", result.ToString() );
            return result;
        }

        #endregion

        #region Sub-Division Border Nodes -> Border NIF

        public static List<GUIBuilder.FormImport.ImportBase> BuildNIFs(
            string borderSetName,
            List<AnnexTheCommonwealth.SubDivision> subdivisions,
            float gradientHeight,
            float groundOffset,
            float groundSink,
            string targetPath,
            string targetSubPath,
            string meshSubPath,
            bool createImportData,
            bool highPrecisionVertexes )
        {
            if(
                ( subdivisions.NullOrEmpty() ) ||
                ( string.IsNullOrEmpty( targetPath ) )
            )
                return null;

            DebugLog.OpenIndentLevel();

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();

            List<FormImport.ImportBase> list = null;

            try
            {

                foreach( var subdivision in subdivisions )
                {
                    m.SetCurrentStatusMessage( string.Format( "BorderBatch.CreateNIFsFor".Translate(), borderSetName, subdivision.IDString ) );
                    var subList = subdivision.CreateBorderNIFs(
                        gradientHeight, groundOffset, groundSink,
                        targetPath,
                        targetSubPath,
                        meshSubPath,
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

            //DebugLog.WriteList( "list", list, true, true );

            m.StopSyncTimer( tStart, borderSetName );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();

            return list;
        }

        #endregion

        #region Generate and Optimize Sub-Divison Elements

        public static void CheckMissingElements(
            List<AnnexTheCommonwealth.SubDivision> subdivisions,
            bool checkBorderEnablers,
            bool checkSandboxVolumes,
            float cylinderTop,
            float cylinderBottom,
            float volumePadding
            )
        {
            DebugLog.OpenIndentLevel();

            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( string.Format( "ControllerBatch.CheckingElements".Translate(), "Controller.SubDivisionL".Translate() ) );

            List <GUIBuilder.FormImport.ImportBase> list = null;

            if( checkBorderEnablers )
                GenerateMissingBorderEnablers( ref list, subdivisions, m );
            /*
            if( checkSandboxVolumes )
                GenerateSandboxes(
                    ref list,
                    Engine.Plugin.TargetHandle.WorkingOrLastFullRequired,
                    subdivisions,
                    m,
                    true, true,
                    cylinderTop,
                    cylinderBottom,
                    volumePadding
                );
            */

            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, true, ref allImportsMatchTarget );

            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        public static void GenerateMissingBorderEnablers( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m )
        {
            DebugLog.OpenIndentLevel();

            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "SubDivisionBatch.CheckingMissingBorderEnablers".Translate() );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();

            string msg;
            foreach( var subdivision in subdivisions )
            {
                msg = string.Format( "SubDivisionBatch.CheckingBorderEnablersFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                var subList = subdivision.GenerateMissingBorderEnablersFromEdgeFlags();
                if( list == null )
                    list = subList;
                else
                    list.AddAll( subList );
            }

            //DebugLog.WriteList( "list", list, true, true );

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        /*
        public static void GenerateSandboxes(
            ref List<FormImport.ImportBase> list,
            Engine.Plugin.TargetHandle target,
            List<SubDivision> subdivisions,
            Windows.Main m,
            bool createMissing,
            bool ignoreExisting,
            bool scanTerrain,
            float cylinderTop,
            float cylinderBottom,
            float volumePadding
            )
        {
            if( ( !createMissing ) && ( ignoreExisting ) )
                return; // So, uh...do nothing, der?

            DebugLog.OpenIndentLevel();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "ControllerBatch.CalculatingSandboxes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();

            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                msg = string.Format( "ControllerBatch.CheckingSandboxFor".Translate(), subdivision.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );

                var edgeFlags = subdivision.EdgeFlags;
                var sandbox = subdivision.SandboxVolume;
                if(
                    ( ( sandbox != null ) && ( ignoreExisting ) ) ||
                    ( ( sandbox == null ) && ( !createMissing ) ) ||
                    ( edgeFlags.NullOrEmpty() )
                )
                {
                    m.PopStatusMessage();
                    continue;
                }

                DebugLog.OpenIndentLevel( subdivision.IDString, false );

                msg = string.Format( "ControllerBatch.CalculatingSandboxFor".Translate(), subdivision.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();

                var hintZ = 0.0f;
                var buildVolumes = subdivision.BuildAreaVolumes;
                if( !buildVolumes.NullOrEmpty() )
                {
                    foreach( var volume in buildVolumes )
                        hintZ += volume.Reference.GetPosition( target ).Z;
                    hintZ /= buildVolumes.Count;
                }
                else if( sandbox != null )
                    hintZ = sandbox.GetPosition( target ).Z;
                else
                    hintZ = subdivision.Reference.GetPosition( target ).Z;

                // Use edge flag reference points instead of build volumes so we can work with less points that are accurate enough
                var points = new List<Vector2f>();
                foreach( var flag in edgeFlags )
                    points.Add( new Vector2f( flag.Reference.GetPosition( target ) ) );

                var hull = Maths.Geometry.ConvexHull.MakeConvexHull( points );

                var osv = VolumeBatch.CalculateOptimalSandboxVolume(
                    target,
                    hull,
                    subdivision.Reference.Worldspace,
                    scanTerrain,
                    cylinderBottom,
                    cylinderTop,
                    volumePadding,
                    hintZ
                );

                if( osv == null )
                    DebugLog.WriteLine( string.Format( "Unable to calculate sandbox for {0}", subdivision.IDString ) );
                else
                {
                    DebugLog.WriteStrings( null, new[] {
                        string.Format(
                            "Position = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetPosition( target ).ToString(),
                            osv.Size.ToString() ),
                        string.Format(
                            "Size = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Primitive.GetBounds( target ).ToString(),
                            osv.Position.ToString() ),
                        string.Format(
                            "Z Rotation = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.GetRotation( target ).Z.ToString(),
                            osv.Rotation.Z.ToString() )
                        }, false, true, false, false );

                    #region Find layer for sandbox

                    var preferedLayer =
                        sandbox != null
                        ?   sandbox.GetLayer( target )
                        ??  GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_SandboxVolumes
                        :   GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_SandboxVolumes;
                    string useLayerEditorID = preferedLayer.GetEditorID( target );

                    #endregion

                    var recordFlags =
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn;
                    var sandboxEditorID = string.Format( "ESM_ATC_REFR_SV_{0}", subdivision.QualifiedName );
                    var worldspace = subdivision.Reference.Worldspace;
                    var cell = ( worldspace == null )
                        ? subdivision.Reference.Cell
                        : worldspace.Cells.Persistent;
                    var sandboxBase = GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_SandboxVolume;
                    var color = sandboxBase.GetMarkerColor( target );
                    
                    VolumeBatch.CreateVolumeRefImport( ref list,
                        "Sandbox Volume",
                        Priority.Ref_SandboxVolume,
                        sandbox,
                        sandboxEditorID,
                        sandboxBase,
                        sandboxBase.GetEditorID( target ),
                        cell,
                        osv.Position,
                        osv.Rotation,
                        osv.Size,
                        color,
                        subdivision.Reference,
                        GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSandboxVolume,
                        true,
                        preferedLayer,
                        useLayerEditorID,
                        recordFlags,
                        null );
                }
                var elapsed = m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                DebugLog.CloseIndentLevel( elapsed );
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        */

        public static void NormalizeBuildVolumes(
            ref List<FormImport.ImportBase> list,
            Engine.Plugin.TargetHandle target,
            List<AnnexTheCommonwealth.SubDivision> subdivisions,
            GUIBuilder.Windows.Main m,
            bool missingOnly,
            bool scanTerrain,
            float topAbovePeak,
            float groundSink
            )
        {
            DebugLog.OpenIndentLevel();

            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "ControllerBatch.CheckingBuildVolumes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();

            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                msg = string.Format( "ControllerBatch.CheckingBuildVolumesFor".Translate(), subdivision.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );

                var volumes = subdivision.BuildAreaVolumes;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                //if( ( volumes.NullOrEmpty() )&&( missingOnly ) )
                if( volumes.NullOrEmpty() )
                {
                    m.StopSyncTimer( tStart, subdivision.GetEditorID( target ) );
                    m.PopStatusMessage();
                    continue;
                }
                var edgeFlags = subdivision.EdgeFlags;
                if( edgeFlags.NullOrEmpty() )
                {
                    m.StopSyncTimer( tStart, subdivision.GetEditorID( target ) );
                    m.PopStatusMessage();
                    continue;
                }

                msg = string.Format( "ControllerBatch.NormalizingBuildVolumesFor".Translate(), subdivision.GetEditorID( target ) );
                m.SetCurrentStatusMessage( msg );

                // Use edge flag reference points instead of build volumes so we can work with less points that are accurate enough
                var points = new List<Vector2f>();
                foreach( var flag in edgeFlags )
                    points.Add( new Vector2f( flag.Reference.GetPosition( target ) ) );

                var hull = Maths.Geometry.ConvexHull.MakeConvexHull( points );

                VolumeBatch.NormalizeBuildVolumes(
                    ref list,
                    target,
                    subdivision.Reference,
                    subdivision.QualifiedName,
                    string.Format( "ESM_ATC_LAYR_{0}", EditorIDFormatter.Token_Name ),
                    string.Format( "ESM_ATC_REFR_BV_{0}_{1}", EditorIDFormatter.Token_Name, EditorIDFormatter.Token_Index ),
                    hull,
                    volumes.ConvertAll<Engine.Plugin.Forms.ObjectReference>( v => v.Reference ),
                    subdivision.Reference.Worldspace,
                    scanTerrain,
                    subdivision.Reference,
                    GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedBuildAreaVolume,
                    new Engine.Plugin.Forms.Activator[ 1 ] { GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_BuildAreaVolume },
                    0,
                    GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_BuildAreaVolume.GetMarkerColor( target ),
                    (
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled |
                        (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn
                    ),
                    groundSink,
                    topAbovePeak,
                    typeof( AnnexTheCommonwealth.BuildAreaVolume )
                );

                m.StopSyncTimer( tStart, subdivision.GetEditorID( target ) );
                m.PopStatusMessage();
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        #endregion

    }

}
