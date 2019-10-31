/*
 * ImportBorderReference.cs
 *
 * Border Reference import (REFR(STAT)).
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
    
    public class ImportBorderReference : ImportBase
    {
        const string            IMPORT_SIGNATURE = "BorderRef";
        const uint              TARGET_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.Persistent |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.IsFullLOD |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.LODRespectsEnableState |
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.REFR.NoRespawn;
        
        FormTarget              ftBaseStat = null;
        FormTarget              ftWorldspace = null;
        Vector2i                CellGrid = Vector2i.MinValue;
        FormTarget              ftCell = null;
        Vector3f                Position = Vector3f.MinValue;
        ScriptTarget            stEnableParent = null;
        FormTarget              ftLinkRef = null;
        FormTarget              ftLinkKeyword = null;
        FormTarget              ftLayer = null;
        
        //Engine.Plugin.Forms.ObjectReference TargetRef           { get { return Target         == null ? null : Target.Form           as Engine.Plugin.Forms.ObjectReference; } }
        Engine.Plugin.Forms.Static TargetStatic                 { get { return ftBaseStat     == null ? null : ftBaseStat.Form       as Engine.Plugin.Forms.Static; } }
        Engine.Plugin.Forms.Cell TargetCell                     { get { return ftCell         == null ? null : ftCell.Form           as Engine.Plugin.Forms.Cell; } }
        Engine.Plugin.Forms.Worldspace TargetWorldspace         { get { return ftWorldspace   == null ? null : ftWorldspace.Form     as Engine.Plugin.Forms.Worldspace; } }
        AnnexTheCommonwealth.BorderEnabler TargetEnableParent   { get { return stEnableParent == null ? null : stEnableParent.Script as AnnexTheCommonwealth.BorderEnabler; } }
        Engine.Plugin.Forms.ObjectReference TargetLinkRef       { get { return ftLinkRef      == null ? null : ftLinkRef.Form        as Engine.Plugin.Forms.ObjectReference; } }
        Engine.Plugin.Forms.Keyword TargetLinkKeyword           { get { return ftLinkKeyword  == null ? null : ftLinkKeyword.Form    as Engine.Plugin.Forms.Keyword; } }
        Engine.Plugin.Forms.Layer TargetLayer                   { get { return ftLayer        == null ? null : ftLayer.Form          as Engine.Plugin.Forms.Layer; } }
        
        protected override void         DumpImport()
        {
            return;
            DebugLog.WriteLine( string.Format(
                "\n{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                this.GetType()  .ToString(),
                Target          .DisplayIDInfo( "\n\tTarget Form = {0}", "unresolved" ),
                ftBaseStat      .DisplayIDInfo( "\n\tBaseStat = {0}" ),
                ftWorldspace    .DisplayIDInfo( "\n\tWorldspace = {0}" ),
                ftCell          .DisplayIDInfo( "\n\tCell = {0}" ),
                string          .Format       ( "\n\tCellGrid = {0}", CellGrid.ToString() ),
                string          .Format       ( "\n\tPosition = {0}", Position.ToString() ),
                stEnableParent  .DisplayIDInfo( "\n\tEnableParent = {0}" ),
                ftLinkRef       .DisplayIDInfo( "\n\tLinkRef = {0}" ),
                ftLinkKeyword   .DisplayIDInfo( "\n\tLinkKeyword = {0}" ),
                ftLayer         .DisplayIDInfo( "\n\tLayer = {0}" )
            ) );
        }
        
        public                          ImportBorderReference( Engine.Plugin.Forms.ObjectReference originalForm, uint statFormID, string statEditorID, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell, Vector3f position, AnnexTheCommonwealth.BorderEnabler borderEnabler, Engine.Plugin.Forms.ObjectReference linkRef, Engine.Plugin.Forms.Keyword linkKeyword, Engine.Plugin.Forms.Layer layer )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( Engine.Plugin.Forms.ObjectReference ), originalForm, worldspace, cell )
        {
            ftBaseStat      = new FormTarget( this, typeof( Engine.Plugin.Forms.Static ), statFormID, statEditorID );
            ftWorldspace    = new FormTarget( this, typeof( Engine.Plugin.Forms.Worldspace ), worldspace );
            ftCell          = new FormTarget( this, typeof( Engine.Plugin.Forms.Cell ), cell );
            CellGrid        = new Vector2i(
                cell == null
                ? Vector2i.MinValue
                : cell.CellGrid.GetGrid( Engine.Plugin.TargetHandle.Master ) );
            Position        = new Vector3f( position );
            stEnableParent  = new ScriptTarget( this, typeof( AnnexTheCommonwealth.BorderEnabler ), borderEnabler );
            ftLinkRef       = new FormTarget( this, typeof( Engine.Plugin.Forms.ObjectReference ), linkRef );
            ftLinkKeyword   = new FormTarget( this, typeof( Engine.Plugin.Forms.Keyword ), linkKeyword );
            ftLayer         = new FormTarget( this, typeof( Engine.Plugin.Forms.Layer ), layer );
            DumpImport();
        }
        
        public                          ImportBorderReference( string[] importData )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( Engine.Plugin.Forms.ObjectReference ), importData )
        {
            ftBaseStat        = new FormTarget( this, typeof( Engine.Plugin.Forms.Static ) );
            ftWorldspace      = new FormTarget( this, typeof( Engine.Plugin.Forms.Worldspace ) );
            ftCell            = new FormTarget( this, typeof( Engine.Plugin.Forms.Cell ) );
            stEnableParent    = new ScriptTarget( this, typeof( AnnexTheCommonwealth.BorderEnabler ) );
            ftLinkRef         = new FormTarget( this, typeof( Engine.Plugin.Forms.ObjectReference ) );
            ftLinkKeyword     = new FormTarget( this, typeof( Engine.Plugin.Forms.Keyword ) );
            ftLayer           = new FormTarget( this, typeof( Engine.Plugin.Forms.Layer ) );
            DumpImport();
        }
        
        protected override string       GetDisplayUpdateFormInfo()
        {
            var tmp = new List<string>();
            var refr = TargetRef;
            
            if( refr.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != ftBaseStat.FormID )
                tmp.Add( ftBaseStat.DisplayIDInfo( "Base form {0}" ) );
            
            if( refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != Position )
                tmp.Add( string.Format( "Position {0}", Position.ToString() ) );
            if( !ftCell.Matches( refr.Cell, false ) )
            {
                tmp.Add( string.Format(
                    "Cell {0}{1}",
                    CellGrid.ToString(),
                    ftCell.DisplayIDInfo( " ({0})", "unresolved" )
                ) );
            }
            if( !ftWorldspace.Matches( refr.Worldspace, false ) )
                tmp.Add( ftWorldspace.DisplayIDInfo( "Worldspace {0}", "unresolved" ) );
            
            if( !ftLayer.Matches( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), true ) )
                tmp.Add( ftLayer.DisplayIDInfo( "Layer {0}" ) );
            
            if(
                ( stEnableParent.Resolveable() )&&
                ( !stEnableParent.Matches( refr.EnableParent.GetReferenceID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), false ) )
            )   tmp.Add( ftLayer.DisplayIDInfo( "Enabled Parent {0}" ) );
            
            if(
                ( ftLinkRef.Resolveable() )&&
                ( ftLinkKeyword.Resolveable() )
            )   {
                var lr = refr.LinkedRefs.GetLinkedRef( ftLinkKeyword.FormID );
                if( !ftLinkRef.Matches( lr, false ) )
                    tmp.Add(
                        string.Format(
                            "Linked to {0} using Keyword {1}",
                            ftLinkRef.DisplayIDInfo(),
                            ftLinkKeyword.DisplayIDInfo()
                    ) );
            }
            
            if( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != Engine.Plugin.Constant.FormID_None )
                tmp.Add( "Clear Location Reference" );
            
            return GenIXHandle.ConcatDisplayInfo( tmp );
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( ftBaseStat.DisplayIDInfo( "Placed instance of {0}" ) );
            
            tmp.Add( string.Format( "Position {0}", Position.ToString() ) );
            tmp.Add( string.Format(
                "Cell {0}{1}",
                CellGrid.ToString(),
                ftCell.DisplayIDInfo( " ({0})", "unresolved" )
            ) );
            tmp.Add( ftWorldspace.DisplayIDInfo( "Worldspace {0}", "unresolved" ) );
            
            tmp.Add( ftLayer.DisplayIDInfo( "Layer {0}" ) );
            
            if( stEnableParent.Resolveable() )
                tmp.Add( ftLayer.DisplayIDInfo( "Enabled Parent {0}" ) );
            
            if(
                ( ftLinkRef.Resolveable() )&&
                ( ftLinkKeyword.Resolveable() )
            )   tmp.Add(
                    string.Format(
                        "Linked to {0} using Keyword {1}",
                        ftLinkRef.DisplayIDInfo(),
                        ftLinkKeyword.DisplayIDInfo()
                ) );
            
            return GenIXHandle.ConcatDisplayInfo( tmp );
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            return ftBaseStat.DisplayIDInfo( "Placed instance of {0}" );
        }
        
        public override int             InjectPriority { get { return 10000; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var refr = TargetRef;
            if( refr == null )
                return false;
            
            var lwk = ftLinkRef.Resolveable() && ftLinkKeyword.Resolveable();
            if( ( !lwk )&&( !stEnableParent.Resolveable() ) ) return false;
            
            var lr = lwk
                ? refr.LinkedRefs.GetLinkedRef( ftLinkKeyword.FormID )
                : null;
            
            return
                ( TargetRecordFlagsMatch )&&
                ( ftBaseStat.Matches( refr.GetName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), false ) )&&
                ( ftWorldspace.Matches( refr.Worldspace, false ) )&&
                ( ftCell.Matches( refr.Cell, false ) )&&
                ( refr.GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == Position )&&
                ( refr.LocationReference.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == Engine.Plugin.Constant.FormID_None )&&
                ( ftLayer.Matches( refr.GetLayer( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), true ) )&&
                ( stEnableParent.Matches( refr.EnableParent.GetReferenceID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), !stEnableParent.Resolveable() ) )||
                ( ftLinkRef.Matches( lr, !lwk ) );
        }
        
        protected override bool         ResolveReferenceForms( bool errorIfUnresolveable )
        {
            // Resolve required forms
            ftBaseStat  .Resolve( false );  // BaseStat may not yet be created but part of the whole set
            ftWorldspace.Resolve( errorIfUnresolveable );
            ftCell      .Resolve( errorIfUnresolveable );
            if( ( TargetCell == null )&&( TargetWorldspace != null ) )
                ftCell.Form = Engine.Plugin.Forms.Worldspace.GetCellForRefr( TargetWorldspace, Position, TARGET_RECORD_FLAGS );
            
            // Can't get references of a form that doesn't exist (yet)
            if( ( TargetForm == null )&&( ftBaseStat.Form != null ) )
            {
                // Reuse an existing reference of the static nif (should be only one)
                // or create a new reference in the cell if there are no usages.
                var baseFormReferences = ftBaseStat.Form.References;
                if( !baseFormReferences.NullOrEmpty() )
                {
                    // There may be multiple references to the form in XeLibs reference table, some may be the same actual reference
                    // but in multiple mods, they will all resolve to the same form to GUIBuilder, in any event
                    // we'll take the first instance and throw a warning if there is more than one
                    var border = baseFormReferences.FirstOrDefault( (x) => ( x is Engine.Plugin.Forms.ObjectReference ) );
                    if( baseFormReferences.Count > 1 )
                        DebugLog.WriteLine( string.Format(
                            "\n{0} :: ResolveForm() :: Multiple references of 0x{1} \"{2}\"! :: Using first reference found - 0x{3}",
                            this.GetType().ToString(),
                            ftBaseStat.FormID.ToString( "X8" ),
                            ftBaseStat.EditorID,
                            border.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) ) );
                    SetTarget( border );
                }
            }
            
            // Resolve optional forms
            stEnableParent.Resolve( false );
            ftLinkRef     .Resolve( false );
            ftLinkKeyword .Resolve( false );
            ftLayer       .Resolve( false );
            
            // Minimum forms resolved?
            var minFormsFound =
                //( ftBaseStat.IsResolved )&&
                ( ftWorldspace.IsResolved )&&
                ( ftCell.IsResolved )&&
                (
                    ( stEnableParent.IsResolved )||
                    (
                        ( ftLinkRef.IsResolved )&&
                        ( ftLinkKeyword.IsResolved )
                    )
                );
            
            return minFormsFound;
        }
        
        public override bool            ParseKeyValue( string key, string value )
        {
            switch( key )
            {
                case "STATFormID":
                    ftBaseStat.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "STATEDID":
                    ftBaseStat.EditorID = value;
                    break;
                    
                case "WorldspaceFormID":
                    ftWorldspace.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "WorldspaceEDID":
                    ftWorldspace.EditorID = value;
                    break;
                    
                case "CellGrid":
                    Maths.Vector2i.TryParse( value, out CellGrid );
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
                    
                case "EnableParentFormID":
                    stEnableParent.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "EnableParentEDID":
                    stEnableParent.EditorID = value;
                    break;
                    
                case "LinkRefFormID":
                    ftLinkRef.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "LinkRefEDID":
                    ftLinkRef.EditorID = value;
                    break;
                    
                case "LinkKeywordFormID":
                    ftLinkKeyword.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "LinkKeywordEDID":
                    ftLinkKeyword.EditorID = value;
                    break;
                    
                case "LayerFormID":
                    ftLayer.FormID = uint.Parse( value, System.Globalization.NumberStyles.HexNumber );
                    break;
                case "LayerEDID":
                    ftLayer.EditorID = value;
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
                        ftBaseStat.DisplayIDInfo( unresolveableSuffix: "unresolved" ),
                        ftCell.DisplayIDInfo( unresolveableSuffix: "unresolved" ) ) );
                    return false;
                }
                refr.SetName( Engine.Plugin.TargetHandle.Working, ftBaseStat.FormID );
                SetTarget( refr );
                //TargetForm = refr;
                return true;
            }
            catch( Exception e )
            {
                AddErrorMessage( ErrorTypes.Import, string.Format(
                    "An exception occured when trying to create a new ObjectReference instance of {0} in cell {1}\nInner Exception:\n{2}",
                    ftBaseStat.DisplayIDInfo( unresolveableSuffix: "unresolved" ),
                    ftCell.DisplayIDInfo( unresolveableSuffix: "unresolved" ),
                    e.ToString() ) );
            }
            return false;
        }
        
        protected override bool         ApplyImport()
        {
            var refr = TargetRef;
            
            ApplyRecordFlagsToTarget();
            refr.SetName( Engine.Plugin.TargetHandle.Working, ftBaseStat.FormID );
            refr.SetPosition( Engine.Plugin.TargetHandle.Working, Position );
            if( ftLayer.IsResolved )
                refr.SetLayer( Engine.Plugin.TargetHandle.Working, ftLayer.FormID );
            if( stEnableParent.IsResolved )
            {   // Set the enable parent and update it's references
                refr.EnableParent.SetReferenceID( Engine.Plugin.TargetHandle.Working, stEnableParent.FormID );
                TargetEnableParent.AddBaseFormAsNIFReference( TargetStatic );
                TargetEnableParent.AddPlacedNIFReference( refr );
            }
            if( ( ftLinkRef.IsResolved )&&( ftLinkKeyword.IsResolved ) )
                refr.LinkedRefs.SetLinkedRef( ftLinkRef.FormID, ftLinkKeyword.FormID );
            
            // Remove unwanted elements automagically added by the CK/XeLib
            refr.LocationReference.DeleteRootElement( false, false );
            
            return true;
        }
        
    }
    
}
