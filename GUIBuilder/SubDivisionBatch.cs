﻿/*
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
                m.SetCurrentStatusMessage( string.Format( "Clear borders for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                subdivision.ClearBorderEnablerEdgeFlags( true );
            }
            
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
            return true;
        }
        
        public static bool CalculateWorkshopEdgeFlagSegments(
            List<WorkshopScript> workshops,
            Engine.Plugin.Forms.Keyword keyword,
            Engine.Plugin.Forms.Static forcedZ,
            float nodeLength,
            float slopeAllowance,
            bool updateMapUIData )
        {
            if(
                ( workshops.NullOrEmpty() )||
                ( keyword == null )
            )
                return false;
            
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: CalculateWorkshopEdgeFlagSegments()" );
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            foreach( var workshop in workshops )
            {
                m.SetCurrentStatusMessage( string.Format( "Calculating borders for {0}...", workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                workshop.BuildBorders( keyword, forcedZ, nodeLength, slopeAllowance, updateMapUIData );
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
                        var msg = "Must automatically generate or manually create missing border enablers before calculating border segments from edge flags.";
                        var retry = MessageBox.Show(
                            msg,
                            "Cannot generate border segments",
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
                m.SetCurrentStatusMessage( string.Format( "Calculating borders for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                subdivision.BuildSegmentsFromEdgeFlags( nodeLength, slopeAllowance, updateMapUIData );
            }
            
            result = true;
            
        localReturnResult:
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatch :: CalculateSubDivisionEdgeFlagSegments() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
            return result;
        }
        
        #endregion
        
        public static void CheckMissingElements( List<AnnexTheCommonwealth.SubDivision> subdivisions, bool checkBorderEnablers, bool checkSandboxVolumes )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: CheckMissingElements()" );
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Checking for missing sub-division elements..." );
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            if( checkBorderEnablers )
                GenerateMissingBorderEnablers( ref list, subdivisions, m );
            if( checkSandboxVolumes )
                GenerateSandboxes( ref list, subdivisions, m, true );
            
            bool allImportsMatchTarget = false;
            FormImport.ImportBase.ShowImportDialog( list, true, ref allImportsMatchTarget );
            
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void GenerateMissingBorderEnablers( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: GenerateMissingBorderEnablers()" );
            
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Checking for missing border enablers..." );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            string msg;
            foreach( var subdivision in subdivisions )
            {
                msg = string.Format( "Checking border enablers for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                FormImport.ImportBase.AddToList( ref list, subdivision.GenerateMissingBorderEnablersFromEdgeFlags() );
            }
            
            m.StopSyncTimer( "GUIBuilder.SubDivisionBatch :: GenerateMissingBorderEnablers() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
        public static void GenerateSandboxes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool missingOnly )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: GenerateSandboxes()" );
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Calculating sandbox volumes..." );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                msg = string.Format( "Checking sandbox for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                var sandbox = subdivision.SandboxVolume;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                if( ( sandbox != null )&&( missingOnly ) )
                {
                    m.PopStatusMessage();
                    continue;
                }
                
                msg = string.Format( "Calculating sandbox for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
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
        
        public static void GenerateBuildVolumes( ref List<FormImport.ImportBase> list, List<AnnexTheCommonwealth.SubDivision> subdivisions, GUIBuilder.Windows.Main m, bool missingOnly )
        {
            DebugLog.OpenIndentLevel( "GUIBuilder.SubDivisionBatch :: GenerateBuildVolumes()" );
            
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "Calculating build volumes..." );
            string msg;
            m.StartSyncTimer();
            var fStart = m.SyncTimerElapsed();
            
            foreach( var subdivision in subdivisions )
            {
                m.PushStatusMessage();
                msg = string.Format( "Checking build volumes for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                
                var volumes = subdivision.BuildVolumes;
                /*DebugLog.Write( string.Format(
                    "Sandbox for:{0}{1}",
                    ImportBase.ExtraInfoFor( "\n\tSub-Division = {0}", subdivision, unresolveable: "unresolved" ),
                    ImportBase.ExtraInfoFor( "\n\tSandbox = {0}", sandbox, unresolveable: "unresolved" ) ) ); */
                //if( ( volumes.NullOrEmpty() )&&( missingOnly ) )
                if( volumes.NullOrEmpty() )
                {
                    m.PopStatusMessage();
                    continue;
                }
                
                msg = string.Format( "Calculating build volumes for {0}...", subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                m.SetCurrentStatusMessage( msg );
                m.StartSyncTimer();
                var tStart = m.SyncTimerElapsed();
                
                subdivision.OptimizeBuildVolumes(
                    ref list,
                    -1024.0f, 5120.0f );
                
                msg = "GUIBuilder.SubDivisionBatch :: GenerateBuildVolumes() :: {0} :: " + subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                m.StopSyncTimer( msg, tStart.Ticks );
                m.PopStatusMessage();
            }
            
            msg = "GUIBuilder.SubDivisionBatch :: GenerateBuildVolumes() :: Completed in {0}";
            m.StopSyncTimer( msg, fStart.Ticks );
            m.PopStatusMessage();
            DebugLog.CloseIndentLevel();
        }
        
    }
    
}