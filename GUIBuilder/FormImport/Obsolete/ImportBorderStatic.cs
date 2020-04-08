/*
 * ImportBorderStatic.cs
 *
 * Border Mesh import (STAT).
 *
 * OBSOLETE - Use ImportBase
 */
 /*
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Plugin.Extensions;
using Maths;


namespace GUIBuilder.FormImport
{
    
    public class ImportBorderStatic : ImportBase
    {
        const string            IMPORT_SIGNATURE = "BorderStatic";

        public const uint       F4_BORDER_RECORD_FLAGS = 0;

        public const uint       ATC_BORDER_RECORD_FLAGS =
            (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD;
        
        string                  NewEditorID = null;
        string                  NIFFilePath = null;
        
        Vector3i                MinBounds = Vector3i.MinValue;
        Vector3i                MaxBounds = Vector3i.MinValue;
        
        Engine.Plugin.Forms.Static TargetStatic                 { get { return Target == null ? null : TargetForm as Engine.Plugin.Forms.Static; } }
        
        protected override void         DumpImport()
        {
            return;
            DebugLog.WriteLine( string.Format(
                "\n{0}{1}{2}{3}{4}{5}",
                this.TypeFullName(),
                Target          .NullSafeIDString( "\n\tTarget Form = {0}", "unresolved" ),
                string.Format( "\n\tNewEditorID = \"{0}\"", NewEditorID ),
                string.Format( "\n\tMinBounds = {0}", MinBounds.ToString() ),
                string.Format( "\n\tMaxBounds = {0}", MaxBounds.ToString() ),
                ( string.IsNullOrEmpty( NIFFilePath ) ? null : string.Format( "\n\tMesh = \"{0}\"", NIFFilePath ) )
            ) );
        }
        
        public                          ImportBorderStatic( string[] importData, uint recordFlags )
            : base( IMPORT_SIGNATURE, recordFlags, false, typeof( Engine.Plugin.Forms.Static ), importData )
        { DumpImport(); }
        
        public                          ImportBorderStatic( Engine.Plugin.Forms.Static originalForm, string newEditorID, string nifFilePath, Vector3i minBounds, Vector3i maxBounds, uint recordFlags )
            : base(  IMPORT_SIGNATURE, recordFlags, false, typeof( Engine.Plugin.Forms.Static ), originalForm )
        {
            if( string.IsNullOrEmpty( newEditorID ) )
                throw new Exception( string.Format( "{0} :: cTor() :: newEditorID cannot be null!", this.TypeFullName() ) );
            
            if( string.IsNullOrEmpty( nifFilePath ) )
                throw new Exception( string.Format( "{0} :: cTor() :: nifFilePath cannot be null!", this.TypeFullName() ) );
            
            if( !Target.IsResolved )
                Target.EditorID = newEditorID;
            NewEditorID     = newEditorID;
            NIFFilePath     = nifFilePath;
            MinBounds       = new Vector3i( minBounds );
            MaxBounds       = new Vector3i( maxBounds );
            DumpImport();
        }
        
        protected override string       GetDisplayUpdateFormInfo()
        {
            var tmp = new List<string>();
            var stat = TargetStatic;
            
            var oldEDID = stat.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            if( string.Compare( oldEDID, NewEditorID, StringComparison.InvariantCulture ) != 0 )
                tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            
            if( stat.ObjectBounds.GetMinValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != MinBounds )
                tmp.Add( string.Format( "Min Bounds {0}", MinBounds.ToString() ) );
            
            if( stat.ObjectBounds.GetMaxValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) != MaxBounds )
                tmp.Add( string.Format( "Max Bounds {0}", MaxBounds.ToString() ) );
            
            if( stat.GetModel( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).InsensitiveInvariantMatch( NIFFilePath ) )
                tmp.Add( string.Format( "Model \"{0}\"", NIFFilePath ) );

            if( ( RecordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD ) != 0 )
            {
                var lods = stat.DistantLOD.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                var updateLODs = ( lods.NullOrEmpty() )||( lods.Length < 4 );
                if( ( !lods.NullOrEmpty() ) && ( lods.Length == 4 ) )
                    for( int i = 0; i < 4; i++ )
                        updateLODs |= !lods[ i ].InsensitiveInvariantMatch( NIFFilePath );
                if( updateLODs )
                    tmp.Add( string.Format( "Distant LOD \"{0}\"", NIFFilePath ) );
            }

            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayNewFormInfo()
        {
            var tmp = new List<string>();
            
            tmp.Add( string.Format( "EditorID \"{0}\"", NewEditorID ) );
            tmp.Add( string.Format( "Min Bounds {0}", MinBounds.ToString() ) );
            tmp.Add( string.Format( "Max Bounds {0}", MaxBounds.ToString() ) );
            tmp.Add( string.Format( "Model \"{0}\"", NIFFilePath ) );
            if( ( RecordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD ) != 0 )
                tmp.Add( string.Format( "Distant LOD \"{0}\"", NIFFilePath ) );
            
            return tmp.ConcatDisplayInfo();
        }
        
        protected override string       GetDisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            var tEDID = TargetStatic == null ? null : TargetStatic.GetEditorID( target );
            return string.IsNullOrEmpty( tEDID )
                ? NewEditorID
                : tEDID;
        }
        
        public override int             InjectPriority { get { return 1000; } }
        
        protected override bool         ImportDataMatchesTarget()
        {
            if( !Resolve( false ) ) return false;
            var stat = TargetStatic;
            if( stat == null ) return false;

            if( ( RecordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD ) != 0 )
            {
                var lods = stat.DistantLOD.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                if( ( lods.NullOrEmpty() )||( lods.Length < 4 ) ) return false;
                for( int i = 0; i < 4; i++ )
                    if( !lods[ i ].InsensitiveInvariantMatch( NIFFilePath ) ) return false;
            }

            return
                ( string.Compare( stat.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), NewEditorID, StringComparison.InvariantCulture ) == 0 )&&
                ( stat.GetModel( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).InsensitiveInvariantMatch( NIFFilePath ) )&&
                ( stat.ObjectBounds.GetMinValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == MinBounds )&&
                ( stat.ObjectBounds.GetMaxValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == MaxBounds );
        }
        
        protected override bool         ApplyImport()
        {
            var stat = TargetStatic;
            
            stat.SetEditorID( Engine.Plugin.TargetHandle.Working, NewEditorID );
            stat.SetModel( Engine.Plugin.TargetHandle.Working, NIFFilePath );
            // Insure it's full LOD at all distances
            if( ( RecordFlags & (uint)Engine.Plugin.Forms.Fields.Record.Flags.Common.HasDistantLOD ) != 0 )
                stat.DistantLOD.SetValue( Engine.Plugin.TargetHandle.Working, new []{ NIFFilePath, NIFFilePath, NIFFilePath, NIFFilePath } );
            stat.ObjectBounds.SetMinValue( Engine.Plugin.TargetHandle.Working, MinBounds );
            stat.ObjectBounds.SetMaxValue( Engine.Plugin.TargetHandle.Working, MaxBounds );
            
            return true;
        }
        
    }
    
}
*/