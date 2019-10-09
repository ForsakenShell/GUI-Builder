/*
 * ObjectReference.cs
 * 
 * Object REFRence form class.
 * 
 */

using System;

using Maths;

using XeLib;
using XeLibHelper;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "REFR", "Object Reference", false )]
    public class ObjectReference : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.ObjectReference.Name _Name;
        Fields.ObjectReference.Primitive _Primitive;
        Fields.ObjectReference.Layer _Layer;
        Fields.ObjectReference.EnableParent _EnableParent;
        Fields.ObjectReference.Position _Position;
        Fields.ObjectReference.Rotation _Rotation;
        Fields.ObjectReference.LocationReference _LocationReference;
        
        Fields.ObjectReference.LinkedRefs _LinkedRefs;
        
        #endregion
        
        public override bool IsMouseOver( Maths.Vector2f mouse, float maxDistance )
        {
            var p3d = GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            var p2d = new Maths.Vector2f( p3d.X, p3d.Y );
            return ( p2d - mouse ).Length <= maxDistance;
        }
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public ObjectReference() : base() {}
        
        public ObjectReference( string filename, uint formID ) : base( filename, formID ) {}
        
        //public ObjectReference( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public ObjectReference( Interface.ICollection container, Interface.IXHandle ancestor, FormHandle handle ) : base( container, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _Name = new Fields.ObjectReference.Name( this );
            _Primitive = new Fields.ObjectReference.Primitive( this );
            _Layer = new Fields.ObjectReference.Layer( this );
            _EnableParent = new Fields.ObjectReference.EnableParent( this );
            _Position = new Fields.ObjectReference.Position( this );
            _Rotation = new Fields.ObjectReference.Rotation( this );
            _LinkedRefs = new Fields.ObjectReference.LinkedRefs( this );
            _LocationReference = new Engine.Plugin.Forms.Fields.ObjectReference.LocationReference( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        public uint GetName( TargetHandle target )
        {
            return _Name.GetValue( target );
        }
        public void SetName( TargetHandle target, uint value )
        {
            _Name.SetValue( target, value );
        }
        
        public Fields.ObjectReference.Primitive Primitive
        {
            get
            {
                return _Primitive;
            }
        }
        
        public uint GetLayer( TargetHandle target )
        {
            return _Layer.GetValue( target );
        }
        public void SetLayer( TargetHandle target, uint value )
        {
            _Layer.SetValue( target, value );
        }
        
        public Fields.ObjectReference.EnableParent EnableParent
        {
            get
            {
                return _EnableParent;
            }
        }
        
        public void CheckForBackgroundCellChange( bool sendObjectDataChangedEvent )
        {
            // Changing position and some record flags will trigger XeLib to update the cell container,
            // We need to match those changes in GUIBuilder too
            if( !IsInWorkingFile() ) return;
            
            var oH = WorkingFileHandle as XeLib.FormHandle;
            if( !oH.IsValid() ) return;
            
            var newCell = ( oH.RecordFlags & (uint)Forms.Fields.Record.Flags.Common.Persistent ) != 0
                ? Worldspace.Cells.Persistent
                : Worldspace.Cells.GetByGrid( GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).WorldspaceToCellGrid() );
            var fileCellFormID = oH.GetCellFormID();
            var fileCell = Worldspace.Cells.Find( fileCellFormID ) as Engine.Plugin.Forms.Cell;
            var oldCell = Cell;
            if( ( newCell == oldCell )&&( fileCell == oldCell ) ) return;
            
            // Changed containers
            //DebugLog.Write( string.Format( "\n{0} :: CheckForBackgroundCellChange()\n\tthis     = {1}\n\tnewCell  = {2}\n\toldCell  = {3}\n\tfileCell = {4}\n", this.GetType().ToString(), this.ToString(), newCell.ToString(), oldCell.ToString(), fileCell.ToString() ) );
            
            //DebugLog.Write( "newCell.InInWorkingFile( true )..." );
            if( !newCell.IsInWorkingFile() )
                throw new Exception( "Unable to copy destination cell as override to move object reference to!" );
            
            if( newCell != fileCell )
            {
                //DebugLog.Write( "this.CopyMoveToCell( moveRefrToCell: true )..." );
                try
                {
                    var ncOH = newCell.WorkingFileHandle as XeLib.FormHandle;
                    var newHandle = oH.CopyMoveToCell( ncOH, true );
                    if( !newHandle.IsValid() )
                        throw new Exception( "Unable to move object reference to new cell!" );
                    //DebugLog.Write( "this.UpdateHandle()..." );
                    this.AddNewHandle( newHandle );
                }
                catch( Exception e )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "CheckForBackgroundCellChange()", e.ToString() );
                }
            }
            
            //DebugLog.Write( "oldCell.RemoveForm()..." );
            oldCell.ObjectReferences.Remove( this );
            //DebugLog.Write( "newCell.AddForm()..." );
            newCell.ObjectReferences.Add( this );
            
            //DebugLog.Write( string.Format( "sendObjectDataChangedEvent = {0}\n", sendObjectDataChangedEvent ) );
            if( sendObjectDataChangedEvent )
            {
                oldCell.SendObjectDataChangedEvent();
                newCell.SendObjectDataChangedEvent();
                this.SendObjectDataChangedEvent();
            }
        }
        
        public Vector3f GetPosition( TargetHandle target )
        {
            return _Position.GetValue( target );
        }
        public void SetPosition( TargetHandle target, Vector3f value )
        {
            _Position.SetValue( target, value );
            CheckForBackgroundCellChange( true );
        }
        
        public Vector3f GetRotation( TargetHandle target )
        {
            return _Rotation.GetValue( target );
        }
        public void SetRotation( TargetHandle target, Vector3f value )
        {
            _Rotation.SetValue( target, value );
        }
        
        public Fields.ObjectReference.LinkedRefs LinkedRefs
        {
            get
            {
                //DebugLog.Write( string.Format( "ObjectReference.LinkedRefs :: 0x{0}", FormID.ToString( "X8" ) ) );
                return _LinkedRefs;
            }
        }
        
        public Fields.ObjectReference.LocationReference LocationReference
        {
            get
            {
                //DebugLog.Write( string.Format( "ObjectReference.LocationReference :: 0x{0}", FormID.ToString( "X8" ) ) );
                return _LocationReference;
            }
        }
        
        public Forms.Cell Cell
        {
            get
            {
                return Ancestor as Forms.Cell;
            }
        }
        
        public Forms.Worldspace Worldspace
        {
            get
            {
                return Ancestor.Ancestor as Forms.Worldspace;
            }
        }
        
        #endregion
        
        public bool LinksTo( uint referenceID )
        {
            int count = _LinkedRefs.Count;
            if( count < 1 )
                return false;
            
            for( int i = 0; i < count; i++ )
            {
                var rID = _LinkedRefs.ReferenceID[ i ];
                if( rID == referenceID )
                    return true;
            }
            
            return false;
        }
        
        public ObjectReference NextInChain( uint keywordFormID )
        {
            int count = _LinkedRefs.Count;
            if( count < 1 )
                return null;
            
            for( int i = 0; i < count; i++ )
            {
                var kFID = _LinkedRefs.KeywordFormID[ i ];
                if( kFID == keywordFormID )
                    return _LinkedRefs.Reference[ i ];
            }
            
            return null;
        }
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _Name.HasValue( target ) )
            {
                var b = GodObject.Plugin.Data.Root.Find( GetName( target ) );
                DebugLog.WriteLine( string.Format( "\tName: {0}", b.ToString() ) );
            }
            if( _Layer.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tLayer: {0}", _Layer.ToString() ) );
            var ep = _EnableParent.GetReference( target );
            if( ep != null )
                DebugLog.WriteLine( string.Format( "\tEnableParent:\n\t\t{0}", _EnableParent.ToString( target ) ) );
            if( _LocationReference.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tLocation Reference: {0}", _LocationReference.ToString( target ) ) );
            if( Worldspace != null )
                DebugLog.WriteLine( string.Format( "\tWorldspace: {0}", Worldspace.ToString() ) );
            if( Cell != null )
                DebugLog.WriteLine( string.Format( "\tCell: {0}", Cell.ToString() ) );
            if( _Position.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tPosition: {0}", _Position.ToString( target ) ) );
            if( _Rotation.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tRotation: {0}", _Rotation.ToString( target ) ) );
            var lrCount = _LinkedRefs.Count;
            if( lrCount > 0 )
            {
                DebugLog.WriteLine( "\tLinked Refs:" );
                for( int i = 0; i < lrCount; i++ )
                {
                    var lrRefr = _LinkedRefs.Reference[ i ];
                    var lrKywd = _LinkedRefs.Keyword[ i ];
                    DebugLog.WriteLine( string.Format(
                        lrKywd == null ? "\t\t{0}" : "\t\t{0}, {1}",
                        lrRefr.ToString(),
                        lrKywd.ToString()
                    ) );
                }
            }
        }
        
        #endregion
        
        /*
        public override string ToString( )
        {
            return string.Format(
                "\"{0}\" - 0x{1}{3} - \"{2}\"",
                Signature,
                FormID.ToString( "X8" ),
                EditorID,
                RecordFlags.Persistent ? "[P]" : null
            );
        }
        */
        
    }
    
}
