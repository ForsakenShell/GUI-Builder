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
        
        #region Sub-division edge flag segments
        
        public static bool ClearEdgeFlagSegments( List<SubDivision> subdivisions )
        {
            if( subdivisions.NullOrEmpty() )
                return false;
            
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: ClearEdgeFlagSegments()" );
            
            var m = GodObject.Windows.GetMainWindow();
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
        
        public static bool CalculateWorkshopEdgeFlagSegments(
            List<WorkshopScript> workshops,
            Engine.Plugin.Forms.Keyword workshopBorderGeneratorKeyword,
            Engine.Plugin.Forms.Static forcedZ,
            float nodeLength,
            float slopeAllowance,
            bool updateMapUIData )
        {
            DebugLog.OpenIndentLevel( new string[] { "GUIBuilder.SubDivisionBatch", "CalculateWorkshopEdgeFlagSegments()", "keyword = " + workshopBorderGeneratorKeyword.ToStringNullSafe(), "workshops = " + workshops.ToStringNullSafe() } );
            
            if(
                ( workshops.NullOrEmpty() )||
                ( workshopBorderGeneratorKeyword == null )
            )
                return false;
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            foreach( var workshop in workshops )
            {
                m.SetCurrentStatusMessage( string.Format( "SubDivisionBatch.CalculatingBordersFor".Translate(), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                workshop.BuildBorders( workshopBorderGeneratorKeyword, forcedZ, nodeLength, slopeAllowance, updateMapUIData );
            }
            
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatch :: CalculateWorkshopEdgeFlagSegments() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
            return true;
        }
        
        public static bool CalculateSubDivisionEdgeFlagSegments( List<SubDivision> subdivisions, float nodeLength, float slopeAllowance, bool updateMapUIData )
        {
            if(
                subdivisions.NullOrEmpty() ||
                nodeLength <= 0.0f
            )
                return false;
            
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: CalculateSubDivisionEdgeFlagSegments()" );
            
            var result = false;
            
            var m = GodObject.Windows.GetMainWindow();
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
                m.SetCurrentStatusMessage( string.Format( "SubDivisionBatch.CalculatingBordersFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                subdivision.BuildSegmentsFromEdgeFlags( nodeLength, slopeAllowance, updateMapUIData );
            }
            
            result = true;
            
        localReturnResult:
            var tEnd = m.SyncTimerElapsed().Ticks - tStart.Ticks;
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatch :: CalculateSubDivisionEdgeFlagSegments() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel( tEnd, "result", result.ToString() );
            return result;
        }
        
        #endregion
        
        public static void CheckMissingElements( List<AnnexTheCommonwealth.SubDivision> subdivisions, bool checkBorderEnablers, bool checkSandboxVolumes )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: CheckMissingElements()" );
            
            var m = GodObject.Windows.GetMainWindow();
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
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: GenerateMissingBorderEnablers()" );
            
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
            
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatch :: GenerateMissingBorderEnablers() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void GenerateSandboxes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool createMissing, bool ignoreExisting )
        {
            if( ( !createMissing )&&( ignoreExisting ) )
                return; // So, uh...do nothing, der?
            
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: GenerateSandboxes()" );
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
                
                msg = string.Format( "SubDivisionBatch.CalculatingSandboxFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                var osv = subdivision.GetOptimalSandboxVolume(
                    fSandboxCylinderBottom: GodObject.CoreForms.ATC_fSandboxCylinderBottom,
                    fSandboxCylinderTop: GodObject.CoreForms.ATC_fSandboxCylinderTop );
                
                if( osv == null )
                    DebugLog.WriteLine( string.Format( "Unable to calculate sandbox for {0}", subdivision.ToString() ) );
                else
                {
                    DebugLog.WriteLine( string.Format(
                        "Sandbox :: {0} :: Size = {1} -> {4} :: Position = {2} -> {5} :: Z Rotation = {3} -> {6}",
                        subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ),
                        sandbox == null ? "[null]" : sandbox.Reference.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                        sandbox == null ? "[null]" : sandbox.Reference.Primitive.GetBounds( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString(),
                        sandbox == null ? "[null]" : sandbox.Reference.GetRotation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).Z.ToString(),
                        osv.Position.ToString(),
                        osv.Size.ToString(),
                        osv.Rotation.Z.ToString()
                       ) );
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
                            GodObject.CoreForms.ESM_ATC_ACTI_SandboxVolume,
                            w, c,
                            osv.Position,
                            osv.Rotation,
                            osv.Size,
                            subdivision.Reference,
                            GodObject.CoreForms.ESM_ATC_KYWD_LinkedSandboxVolume,
                            GodObject.CoreForms.ESM_ATC_LAYR_SandboxVolumes
                    ) );
                }
                msg = "GUIBuilder.SubDivisionBatch :: GenerateSandboxes() :: {0} :: " + subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                m.StopSyncTimer( msg, tStart.Ticks );
                m.PopStatusMessage();
            }
            
            msg = "GUIBuilder.SubDivisionBatch :: GenerateSandboxes() :: Completed in {0}";
            m.StopSyncTimer( msg, fStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void NormalizeBuildVolumes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool missingOnly )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: NormalizeBuildVolumes()" );
            
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
                    msg = "GUIBuilder.SubDivisionBatch :: NormalizeBuildVolumes() :: {0} :: " + subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    m.StopSyncTimer( msg, tStart.Ticks );
                    m.PopStatusMessage();
                    continue;
                }
                
                msg = string.Format( "SubDivisionBatch.NormalizingBuildVolumesFor".Translate(), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                subdivision.NormalizeBuildVolumes(
                    ref list,
                    -1024.0f, 5120.0f );
                
                msg = "GUIBuilder.SubDivisionBatch :: NormalizeBuildVolumes() :: {0} :: " + subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                m.StopSyncTimer( msg, tStart.Ticks );
                m.PopStatusMessage();
            }
            
            msg = "GUIBuilder.SubDivisionBatch :: NormalizeBuildVolumes() :: Completed in {0}";
            m.StopSyncTimer( msg, fStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
    }
    
}
