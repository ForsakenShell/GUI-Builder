/*
 * ImportBorderEnablerReference.cs
 *
 * Border Enabler import (REFR(ACTI)).
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Plugin.Extensions;
using Maths;
using AnnexTheCommonwealth;


namespace GUIBuilder.FormImport
{
    
    public class ImportBorderEnablerReference : ImportBase
    {
        const string            IMPORT_SIGNATURE = "BorderEnablerRef";
        const uint              TARGET_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.InitiallyDisabled |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn;
        
        
        string                  NewEditorID = null;
        
        FormTarget              ftWorldspace = null;
        FormTarget              ftCell = null;
        Vector3f                Position = Vector3f.MinValue;
        ScriptTarget            stSubDivision = null;
        ScriptTarget            stNeighbour = null;
        
        //Engine.Plugin.Forms.ObjectReference TargetRef           { get { return Target         == null ? null : Target.Form           as Engine.Plugin.Forms.ObjectReference; } }
        Engine.Plugin.Forms.Cell TargetCell                     { get { return ftCell         == null ? null : ftCell.Form           as Engine.Plugin.Forms.Cell; } }
        Engine.Plugin.Forms.Worldspace TargetWorldspace         { get { return ftWorldspace   == null ? null : ftWorldspace.Form     as Engine.Plugin.Forms.Worldspace; } }
        AnnexTheCommonwealth.SubDivision TargetSubDivision      { get { return stSubDivision  == null ? null : stSubDivision.Script  as AnnexTheCommonwealth.SubDivision; } }
        AnnexTheCommonwealth.SubDivision TargetNeighbour        { get { return stNeighbour    == null ? null : stNeighbour.Script    as AnnexTheCommonwealth.SubDivision; } }
        
        protected override void         DumpImport()
        {
            return;
            var s1 = Target          .DisplayIDInfo( "\n\tTarget Form = {0}", "unresolved" );
            var s2 = ( string.IsNullOrEmpty( NewEditorID ) ? null : string.Format( "\n\tNewEditorID = \"{0}\"", NewEditorID ) );
            var s3 = ftWorldspace    .DisplayIDInfo( "\n\tWorldspace = {0}" );
            var s4 = ftCell          .DisplayIDInfo( "\n\tCell = {0}" );
            var s5 = string          .Format       ( "\n\tPosition = {0}", Position.ToString() );
            var s6 = stSubDivision   .DisplayIDInfo( "\n\tSubDivision = {0}" );
            var s7 = stNeighbour     .DisplayIDInfo( "\n\tNeighbour = {0}" );
            DebugLog.WriteLine( string.Format(
                "\n{0}{1}{2}{3}{4}{5}{6}{7}",
                this.TypeFullName(),
                s1,
                s2,
                s3,
                s4,
                s5,
                s6,
                s7
            ) );
        }
        
        public                          ImportBorderEnablerReference( AnnexTheCommonwealth.BorderEnabler originalScript, string newEditorID, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell, Vector3f position, AnnexTheCommonwealth.SubDivision subdivision, AnnexTheCommonwealth.SubDivision neighbour )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( AnnexTheCommonwealth.BorderEnabler ), originalScript )
        {
            if( string.IsNullOrEmpty( newEditorID ) )
                throw new Exception( string.Format( "{0} :: cTor() :: newEditorID cannot be null!", this.TypeFullName() ) );
            
            if( !Target.IsResolved )
                Target.EditorID = newEditorID;
            NewEditorID     = newEditorID;
            ftWorldspace    = new FormTarget( "Worldspace", this, typeof( Engine.Plugin.Forms.Worldspace ), worldspace );
            ftCell          = new FormTarget( "Cell", this, typeof( Engine.Plugin.Forms.Cell ), cell );
            Position        = new Vector3f( position );
            stSubDivision   = new ScriptTarget( "SubDivision", this, typeof( AnnexTheCommonwealth.SubDivision ), subdivision );
            stNeighbour     = new ScriptTarget( "Neighbour", this, typeof( AnnexTheCommonwealth.SubDivision ), neighbour );
            
            DumpImport();
        }
        
        public                          ImportBorderEnablerReference( string[] importData )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( AnnexTheCommonwealth.BorderEnabler ), importData )
        {
            ftWorldspace      = new FormTarget( "Worldspace", this, typeof( Engine.Plugin.Forms.Worldspace ) );
            ftCell            = new FormTarget( "Cell", this, typeof( Engine.Plugin.Forms.Cell ) );
            stSubDivision     = new ScriptTarget( "SubDivision", this, typeof( AnnexTheCommonwealth.SubDivision ) );
            stNeighbour       = new ScriptTarget( "Neighbour", this, typeof( AnnexTheCommonwealth.SubDivision ) );
            DumpImport();
        }
        
        protected override string       GetDisplayUpdateFormInfo()
        {
            var tmp = new List<string>();
            var refr = TargetRef;
            
            var oldEDID = refr.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            if( string.Compare( oldEDID, NewEditorID, StringComparison.InvariantCulture ) != 0 )
                tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            
            if( refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != Position )
                tmp.Add( string.Format( "Position {0}", Position.ToString() ) );
            if( !ftCell.Matches( refr.Cell, false ) )
                tmp.Add( ftCell.DisplayIDInfo( "Cell {0}", "unresolved" ) );
            if( !ftWorldspace.Matches( refr.Worldspace, false ) )
                tmp.Add( ftWorldspace.DisplayIDInfo( "Worldspace {0}", "unresolved" ) );
            
            if( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) )
                tmp.Add( string.Format( "Layer {0}", GodObject.CoreForms.ESM_ATC_LAYR_Controllers.ExtraInfoFor() ) );
            
            var ownSub = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            if( !stSubDivision.Matches( ownSub, false ) )
                tmp.Add( stSubDivision.DisplayIDInfo( "Linked to Sub-Division {0}" ) );
            
            var naySub = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.ESM_ATC_KYWD_LinkedSubDivision.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            if( !stNeighbour.Matches( naySub, true ) )
                tmp.Add( stNeighbour.DisplayIDInfo( "Linked to neighbour {0}" ) );
            
            if( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != Engine.Plugin.Constant.FormID_None )
                tmp.Add( "Clear Location Reference" );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( string.Format( "Placed instance of {0}", GodObject.CoreForms.ESM_ATC_ACTI_BorderEnabler.ExtraInfoFor() ) );
            
            tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            
            tmp.Add( string.Format( "Position {0}", Position.ToString() ) );
            tmp.Add( ftCell.DisplayIDInfo( "Cell {0}", "unresolved" ) );
            tmp.Add( ftWorldspace.DisplayIDInfo( "Worldspace {0}", "unresolved" ) );
            
            tmp.Add( string.Format( "Layer {0}", GodObject.CoreForms.ESM_ATC_LAYR_Controllers.ExtraInfoFor() ) );
            
            tmp.Add( stSubDivision.DisplayIDInfo( "Linked to Sub-Division {0}" ) );
            
            if( stNeighbour.Resolveable() )
                tmp.Add( stNeighbour.DisplayIDInfo( "Linked to neighbour {0}" ) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            var tf = TargetForm;
            return tf == null
                ? NewEditorID
                : tf.GetEditorID( target );
        }
        
        public override int             InjectPriority { get { return 5000; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var targetScript = TargetScript;
            if( targetScript == null )
                return false;
            
            var targetRef = targetScript.Reference;
            
            var ownSub = targetRef.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            var naySub = targetRef.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.ESM_ATC_KYWD_LinkedSubDivision.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            var result = 
                ( string.Compare( targetRef.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), NewEditorID, StringComparison.InvariantCulture ) == 0 )&&
                ( ftWorldspace.Matches( targetRef.Worldspace, true ) )&&
                ( ftCell.Matches( targetRef.Cell, false ) )&&
                ( targetRef.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == Position )&&
                ( targetRef.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == Engine.Plugin.Constant.FormID_None )&&
                ( targetRef.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) )&&
                ( stSubDivision.Matches( ownSub, false ) )&&
                ( stNeighbour.Matches( naySub, !stNeighbour.Resolveable() ) );
            return result;
        }
        
        protected override bool         ResolveReferenceForms( bool errorIfUnresolveable )
        {
            // Resolve required forms
            ftWorldspace.Resolve( errorIfUnresolveable );
            ftCell      .Resolve( errorIfUnresolveable );
            if( ( TargetCell == null )&&( TargetWorldspace != null ) )
                ftCell.Form = Engine.Plugin.Forms.Worldspace.GetCellForRefr( TargetWorldspace, Position, TARGET_RECORD_FLAGS );
            stSubDivision .Resolve( errorIfUnresolveable);
            
            // Resolve optional forms
            stNeighbour   .Resolve( false );
            
            // Minimum forms resolved?
            var minFormsFound =
                ( ftWorldspace.IsResolved )&&
                ( ftCell.IsResolved )&&
                ( stSubDivision.IsResolved );
            
            return minFormsFound;
        }
        
        public override bool            ParseKeyValue( string key, string value )
        {
            switch( key )
            {
                case "WorldspaceFormID":
                    ftWorldspace.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "WorldspaceEDID":
                    ftWorldspace.EditorID = value;
                    break;
                    
                case "CellFormID":
                    ftCell.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "CellEDID":
                    ftCell.EditorID = value;
                    break;
                    
                case "Position":
                    Maths.Vector3f.TryParse( value, out Position );
                    break;
                    
                case "SubDivisionFormID":
                    stSubDivision.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "SubDivisionEDID":
                    stSubDivision.EditorID = value;
                    break;
                    
                case "NeighbourFormID":
                    stNeighbour.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "NeighbourEDID":
                    stNeighbour.EditorID = value;
                    break;
                    
                default:
                    return false;
            }
            return true;
        }
        
        protected override bool         CreateNewFormInWorkingFile()
        {
            var cell = TargetCell;
            if( cell == null )
            {
                AddErrorMessage( ErrorTypes.Import, "Target Cell is unresolved" );
                return false;
            }
            try
            {
                var refr = cell.ObjectReferences.CreateNew<Engine.Plugin.Forms.ObjectReference>();
                if( refr == null )
                {
                    AddErrorMessage( ErrorTypes.Import, string.Format(
                        "Unable to create a new ObjectReference instance of {0} in cell {1}",
                        GodObject.CoreForms.ESM_ATC_ACTI_BorderEnabler.ToString(),
                        ftCell.DisplayIDInfo( unresolveableSuffix: "unresolved" ) ) );
                    return false;
                }
                refr.SetName( Engine.Plugin.TargetHandle.Working, GodObject.CoreForms.ESM_ATC_ACTI_BorderEnabler.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                var newenablerscript = new BorderEnabler( refr );
                if( newenablerscript == null )
                {
                    AddErrorMessage( ErrorTypes.Import, string.Format(
                        "Unable to create a new Script Object on new instance of {0} in cell {1}",
                        GodObject.CoreForms.ESM_ATC_ACTI_BorderEnabler.ToString(),
                        ftCell.DisplayIDInfo( unresolveableSuffix: "unresolved" ) ) );
                    return false;
                }
                newenablerscript.PostLoad();
                GodObject.Plugin.Data.BorderEnablers.Add( newenablerscript );
                SetTarget( newenablerscript );
                return true;
            }
            catch( Exception e )
            {
                AddErrorMessage( ErrorTypes.Import, string.Format(
                    "An exception occured when trying to create a new ObjectReference instance of {0} in cell {1}\nInner Exception:\n{2}",
                    GodObject.CoreForms.ESM_ATC_ACTI_BorderEnabler.ToString(),
                    ftCell.DisplayIDInfo( unresolveableSuffix: "unresolved" ),
                    e.ToString() ) );
            }
            return false;
        }
        
        protected override bool         ApplyImport()
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "ApplyImport()" } );
            var result = false;
            
            var enabler = TargetScript as AnnexTheCommonwealth.BorderEnabler;
            if( enabler == null )
            {
                DebugLog.WriteError( "TargetScript is not Script \"BorderEnabler\"" );
                goto localReturnResult;
            }
            var refr = enabler.Reference;
            
            refr.SetEditorID( Engine.Plugin.TargetHandle.Working, NewEditorID );
            refr.SetPosition( Engine.Plugin.TargetHandle.Working, Position );
            refr.SetLayer( Engine.Plugin.TargetHandle.Working, GodObject.CoreForms.ESM_ATC_LAYR_Controllers.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            var oldSubdivisionRefr = refr.LinkedRefs.GetLinkedRef( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired, GodObject.CoreForms.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            refr.LinkedRefs.SetLinkedRef( Engine.Plugin.TargetHandle.Working, stSubDivision.FormID, GodObject.CoreForms.ESM_ATC_KYWD_LinkedBorder.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            var nayFID = stNeighbour.IsResolved ? stNeighbour.FormID : Engine.Plugin.Constant.FormID_None;
            refr.LinkedRefs.SetLinkedRef( Engine.Plugin.TargetHandle.Working, nayFID, GodObject.CoreForms.ESM_ATC_KYWD_LinkedSubDivision.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            // Remove invalid elements automagically added by the CK/XeLib
            refr.LocationReference.DeleteRootElement( false, false );
            
            // Inform old linked refs of changes as needed
            var subdivision = stSubDivision.Script as AnnexTheCommonwealth.SubDivision;
            if( ( oldSubdivisionRefr != null )&&( subdivision.GetFormID( Engine.Plugin.TargetHandle.Master ) != oldSubdivisionRefr.GetFormID( Engine.Plugin.TargetHandle.Master ) ) )
            {
                var oldSubdivision = oldSubdivisionRefr.GetScript<AnnexTheCommonwealth.SubDivision>();
                if( oldSubdivision != null )
                {
                    oldSubdivision.RemoveBorderEnabler( enabler );
                    oldSubdivision.SendObjectDataChangedEvent( this );
                }
            }
            if( !subdivision.HasBorderEnabler( enabler ) )
            {
                subdivision.AddBorderEnabler( enabler );
                subdivision.SendObjectDataChangedEvent( this );
            }
            
            result = true;
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
    }
    
}
