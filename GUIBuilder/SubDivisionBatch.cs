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
            var elapsed = m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel( elapsed, "result", result.ToString() );
            return result;
        }
        
        #endregion
        
        public static void CheckMissingElements( List<AnnexTheCommonwealth.SubDivision> subdivisions, bool checkBorderEnablers, bool checkSandboxVolumes )
        {
            DebugLog.OpenIndentLevel();
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "SubDivisionBatch.CheckingElements".Translate() );
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            if( checkBorderEnablers )
                GenerateMissingBorderEnablers( ref list, subdivisions, m );
            if( checkSandboxVolumes )
                GenerateSandboxes( ref list, subdivisions, m, true, true );
            
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
                FormImport.ImportBase.AddToList( ref list, subdivision.GenerateMissingBorderEnablersFromEdgeFlags() );
            }
            
            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void GenerateSandboxes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool createMissing, bool ignoreExisting )
        {
            if( ( !createMissing )&&( ignoreExisting ) )
                return; // So, uh...do nothing, der?
            
            DebugLog.OpenIndentLevel();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "SubDivisionBatch.CalculatingSandboxes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                msg = string.Format( "SubDivisionBatch.CheckingSandboxFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                var sandbox = subdivision.SandboxVolume;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                if(
                    ( ( sandbox != null )&&( ignoreExisting ) )||
                    ( ( sandbox == null )&&( !createMissing ) )
                ) {
                    m.PopStatusMessage();
                    continue;
                }

                DebugLog.OpenIndentLevel( subdivision.IDString, false );

                msg = string.Format( "SubDivisionBatch.CalculatingSandboxFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var osv = subdivision.GetOptimalSandboxVolume(
                    fSandboxCylinderBottom: GodObject.CoreForms.AnnexTheCommonwealth.fSandboxCylinderBottom,
                    fSandboxCylinderTop: GodObject.CoreForms.AnnexTheCommonwealth.fSandboxCylinderTop );
                
                if( osv == null )
                    DebugLog.WriteLine( string.Format( "Unable to calculate sandbox for {0}", subdivision.ToString() ) );
                else
                {
                    DebugLog.WriteStrings( null, new[] {
                        string.Format(
                            "Size = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                            osv.Size.ToString() ),
                        string.Format(
                            "Position = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Reference.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                            osv.Position.ToString() ),
                        string.Format(
                            "Z Rotation = {0} -> {1}",
                            sandbox == null ? "[null]" : sandbox.Reference.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z.ToString(),
                            osv.Rotation.Z.ToString() )
                        }, false, true, false, false );
                    var w = subdivision.Reference.Worldspace;
                    var c = w == null
                        ? subdivision.Reference.Cell
                        : subdivision.Reference.Worldspace.Cells.Persistent;
                    FormImport.ImportBase.AddToList(
                        ref list,
                        new FormImport.ImportSandboxReference(
                            sandbox,
                            string.Format(
                                "{0}{1}{2}",
                                "ESM",
                                subdivision.NameFromEditorID,
                                "SandboxArea" ),
                            GodObject.CoreForms.AnnexTheCommonwealth.Activator.ESM_ATC_ACTI_SandboxVolume,
                            w, c,
                            osv.Position,
                            osv.Rotation,
                            osv.Size,
                            subdivision.Reference,
                            GodObject.CoreForms.AnnexTheCommonwealth.Keyword.ESM_ATC_KYWD_LinkedSandboxVolume,
                            GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_SandboxVolumes
                    ) );
                }
                var elapsed =  m.StopSyncTimer( tStart );
                m.PopStatusMessage();
                DebugLog.CloseIndentLevel( elapsed );
            }
            
            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void NormalizeBuildVolumes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool missingOnly )
        {
            DebugLog.OpenIndentLevel();
            
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "SubDivisionBatch.CheckingBuildVolumes".Translate() );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                msg = string.Format( "SubDivisionBatch.CheckingBuildVolumesFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                var volumes = subdivision.BuildVolumes;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                //if( ( volumes.NullOrEmpty() )&&( missingOnly ) )
                if( volumes.NullOrEmpty() )
                {
                    m.StopSyncTimer( tStart, subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                    m.PopStatusMessage();
                    continue;
                }
                
                msg = string.Format( "SubDivisionBatch.NormalizingBuildVolumesFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                subdivision.NormalizeBuildVolumes(
                    ref list,
                    -1024.0f, 5120.0f );

                m.StopSyncTimer( tStart, subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.PopStatusMessage();
            }

            m.StopSyncTimer( fStart );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }

        #region Sub-Division Border Nodes -> Border NIF

        public static List<GUIBuilder.FormImport.ImportBase> BuildNIFs(
            string borderSetName,
            List<AnnexTheCommonwealth.SubDivision> subdivisions,
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

    }

}
