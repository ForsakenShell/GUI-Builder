/*
 * ImportLayer.cs
 *
 * Layer import (LAYR).
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Plugin.Extensions;


namespace GUIBuilder.FormImport
{
    
    public class ImportLayer : ImportBase
    {
        const string            IMPORT_SIGNATURE = "Layer";
        const uint              TARGET_RECORD_FLAGS = 0;
        
        string                  NewEditorID = null;
        
        Engine.Plugin.Forms.Layer TargetLayer { get { return Target == null ? null : TargetForm as Engine.Plugin.Forms.Layer; } }
        
        protected override void         DumpImport()
        {
            //return;
            DebugLog.WriteLine( string.Format(
                "\n{0}{1}{2}{3}{4}{5}",
                this.GetType()  .ToString(),
                Target          .DisplayIDInfo( "\n\tTarget Form = {0}", "unresolved" ),
                string.Format( "\n\tNewEditorID = \"{0}\"", NewEditorID )
            ) );
        }
        
        public                          ImportLayer( string[] importData )
            : base( IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( Engine.Plugin.Forms.Layer ), importData )
        { DumpImport(); }
        
        public                          ImportLayer( Engine.Plugin.Forms.Layer originalForm, string newEditorID )
            : base(  IMPORT_SIGNATURE, TARGET_RECORD_FLAGS, false, typeof( Engine.Plugin.Forms.Layer ), originalForm )
        {
            if( string.IsNullOrEmpty( newEditorID ) )
                throw new Exception( string.Format( "{0} :: cTor() :: newEditorID cannot be null!", this.GetType().ToString() ) );
            
            if( !Target.IsResolved )
                Target.EditorID = newEditorID;
            NewEditorID     = newEditorID;
            DumpImport();
        }
        
        protected override string       GetDisplayUpdateFormInfo()
        {
            var tmp = new List<string>();
            var layer = TargetLayer;
            
            var oldEDID = layer.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            if( string.Compare( oldEDID, NewEditorID, StringComparison.InvariantCulture ) != 0 )
                tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            var tEDID = TargetLayer == null ? null : TargetLayer.GetEditorID( target );
            return string.IsNullOrEmpty( tEDID )
                ? NewEditorID
                : tEDID;
        }
        
        public override int             InjectPriority { get { return 500; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var layer = TargetLayer;
            if( layer == null )
                return false;
            
            return
                ( string.Compare( layer.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), NewEditorID, StringComparison.InvariantCulture ) == 0 );
        }
        
        public override bool            ParseKeyValue( string key, string value )
        {
            switch( key )
            {
                case "EditorID":
                    NewEditorID = value;
                    break;
                default:
                    return false;
            }
            return true;
        }
        
        protected override bool         CreateNewFormInWorkingFile()
        {
            try
            {
                var cLayers = GodObject.Plugin.Data.Root.GetCollection<Engine.Plugin.Forms.Layer>( true, false );
                if( cLayers == null )
                {
                    AddErrorMessage( ErrorTypes.Import, "Unable to get root container for Layers" );
                    return false;
                }
                var layer = cLayers.CreateNew<Engine.Plugin.Forms.Layer>();
                if( layer == null )
                {
                    AddErrorMessage( ErrorTypes.Import, string.Format(
                        "Unable to create a new Layer in \"{0}\"",
                        GodObject.Plugin.Data.Files.Working.Filename ) );
                    return false;
                }
                SetTarget( layer );
                //TargetForm = layer;
                return true;
            }
            catch( Exception e )
            {
                AddErrorMessage( ErrorTypes.Import, string.Format(
                    "An exception occured when trying to create a new Layer in \"{0}\"!\nInner exception:\n{1}",
                    GodObject.Plugin.Data.Files.Working.Filename,
                    e.ToString() ) );
            }
            return false;
        }
        
        protected override bool         ApplyImport()
        {
            var layer = TargetLayer;
            
            layer.SetEditorID( Engine.Plugin.TargetHandle.Working, NewEditorID );
            
            return true;
        }
        
    }
    
}
