/*
 * ImportEdgeFlagReference.cs
 *
 * Sub-division/Workshop edge flag reference (REFR(STAT)).
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

namespace GUIBuilder.FormImport
{
    
    public class ImportEdgeFlagReference : ImportBase
    {
        const string            IMPORT_SIGNATURE = "EdgeFlagRef";
        const uint              TARGET_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn;
        
        //Engine.Plugin.Forms.ObjectReference TargetRef           { get { return Target         == null ? null : Target.Form           as Engine.Plugin.Forms.ObjectReference; } }
        
        protected override void         DumpImport()
        {
            return;
            DebugLog.WriteLine( string.Format(
                "\n{0}{1}",
                this.GetType()  .ToString(),
                Target          .DisplayIDInfo( "\n\tTarget Form = {0}", "unresolved" )
            ) );
        }
        
        public                          ImportEdgeFlagReference( AnnexTheCommonwealth.EdgeFlag originalScript, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, true, typeof( AnnexTheCommonwealth.EdgeFlag ), originalScript )
        {
            DumpImport();
        }
        
        public                          ImportEdgeFlagReference( string[] importData )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, true, typeof( AnnexTheCommonwealth.EdgeFlag ), importData )
        {
            DumpImport();
        }
        
        protected override string       GetDisplayUpdateFormInfo()
        {
            var tmp = new List<string>();
            var refr = TargetRef;
            
            if( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.Working ) != Engine.Plugin.Constant.FormID_None )
                tmp.Add( "Clear Location Reference" );
            
            if( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) )
                tmp.Add( string.Format( "Layer {0}", GodObject.CoreForms.ESM_ATC_LAYR_Controllers.ToString() ) );
            
            return GenIDataSync.ConcatDisplayInfo( tmp );
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( string.Format( "Layer {0}", GodObject.CoreForms.ESM_ATC_LAYR_Controllers.ToString() ) );
            
            return GenIDataSync.ConcatDisplayInfo( tmp );
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            var flagBase = GodObject.CoreForms.SubDivisionEdgeFlag( TargetRef.GetName( target ) );
            return GenIDataSync.ExtraInfoFor( flagBase, format: "Placed instance of {0}", unresolveable: "unresolved" );
        }
        
        public override int             InjectPriority { get { return 9000; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var refr = TargetRef;
            if( refr == null ) return false;
            
            return
                ( TargetRecordFlagsMatch )&&
                ( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.Working ) == Engine.Plugin.Constant.FormID_None )&&
                ( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
        }
        
        protected override bool         ApplyImport()
        {
            var refr = TargetRef;
            
            ApplyRecordFlagsToTarget();
            refr.SetLayer( Engine.Plugin.TargetHandle.Working, GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
            
            // Remove unwanted elements automagically added by the CK/XeLib
            refr.LocationReference.DeleteRootElement( false, false );
            
            return true;
        }
        
    }
    
}
