/*
 * ImportEdgeFlagReference.cs
 *
 * Sub-division/Workshop edge flag reference (REFR(STAT)).
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Plugin.Extensions;


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
                this.TypeFullName(),
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
            
            if( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != Engine.Plugin.Constant.FormID_None )
                tmp.Add( "Clear Location Reference" );
            
            if( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) )
                tmp.Add(string.Format("Layer {0}", GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_Controllers.ExtraInfoFor()) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( string.Format( "Layer {0}", GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_Controllers.ToString() ) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            var flagBase = GodObject.CoreForms.GetSubDivisionEdgeFlag( TargetRef.GetName( target ) );
            return flagBase.ExtraInfoFor(format: "Placed instance of {0}", unresolveable: "unresolved");
        }
        
        public override int             InjectPriority { get { return 9000; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var refr = TargetRef;
            if( refr == null ) return false;
            
            return
                ( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == Engine.Plugin.Constant.FormID_None )&&
                ( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) );
        }
        
        protected override bool         ApplyImport()
        {
            var refr = TargetRef;
            
            refr.SetLayer( Engine.Plugin.TargetHandle.Working, GodObject.CoreForms.AnnexTheCommonwealth.Layer.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            // Remove unwanted elements automagically added by the CK/XeLib
            refr.LocationReference.DeleteRootElement( false, false );
            
            return true;
        }
        
    }
    
}
