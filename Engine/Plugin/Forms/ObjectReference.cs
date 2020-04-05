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
        Fields.ObjectReference.LocationRefTypes _locationRefTypes;

        
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
        public ObjectReference( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _Name = new Fields.ObjectReference.Name( this );
            _Primitive = new Fields.ObjectReference.Primitive( this );
            _Layer = new Fields.ObjectReference.Layer( this );
            _EnableParent = new Fields.ObjectReference.EnableParent( this );
            _Position = new Fields.ObjectReference.Position( this );
            _Rotation = new Fields.ObjectReference.Rotation( this );
            _LinkedRefs = new Fields.ObjectReference.LinkedRefs( this );
            _LocationReference = new Fields.ObjectReference.LocationReference( this );
            _locationRefTypes = new Fields.ObjectReference.LocationRefTypes( this );
        }

        #endregion

        #endregion

        #region Properties

        public Engine.Plugin.Form GetName( TargetHandle target )
        {
            var nFID = _Name.GetValue( target );
            if( !nFID.ValidFormID() ) return null;
            return GodObject.Plugin.Data.Root.Find( nFID, true ) as Engine.Plugin.Form;
        }
        
        public uint GetNameFormID( TargetHandle target )
        {
            return _Name.GetValue( target );
        }
        public void SetNameFormID( TargetHandle target, uint value )
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
        
        public Engine.Plugin.Forms.Layer GetLayer( TargetHandle target )
        {
            var lFID = GetLayerFormID( target );
            if( !lFID.ValidFormID() ) return null;
            return GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( lFID, true );
        }

        public uint GetLayerFormID( TargetHandle target )
        {
            return _Layer.GetValue( target );
        }
        public void SetLayerFormID( TargetHandle target, uint value )
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
        
        bool UpdateContainerCellHandle( Cell cell )
        {
            DebugLog.OpenIndentLevel();
            
            var result = false;
            
            // Technically the same as cell.IsInWorkingFile() but this is clearer as to our intent
            if( cell.HandleFor( GodObject.Plugin.Data.Files.Working ).IsValid() )
            {
                result = true;
                goto localAbort;
            }
            
            var cmH = cell.MasterHandle as FormHandle;
            var coHs = cmH.GetOverrides();
            if( !coHs.NullOrEmpty() )
            {
                foreach( var coH in coHs )
                {
                    if( coH.Filename.InsensitiveInvariantMatch( GodObject.Plugin.Data.Files.Working.Filename ) )
                    {
                        DebugLog.WriteStrings( null, new [] { "cell = " + cell.ToStringNullSafe(), "handle = " + coH.ToStringNullSafe() }, false, true, false, false );
                        cell.AddNewHandle( coH );
                        result = true;
                    }
                }
            }
            if( !result )
                result = cell.CopyAsOverride().IsValid();
            
        localAbort:
            DebugLog.CloseIndentLevel();
            return result;
        }
        
        public void CheckForBackgroundCellChange( bool sendObjectDataChangedEvent )
        {
            if( ObjectDataChangedEventsSupressed )
                return;

            DebugLog.OpenIndentLevel( this.IDString );
            
            // Changing position and some record flags will trigger XeLib to update the cell container,
            // We need to match those changes in GUIBuilder too
            if( !IsInWorkingFile() ) goto localAbort;
            
            var oH = WorkingFileHandle as XeLib.FormHandle;
            if( !oH.IsValid() ) goto localAbort;
            
            var newCell = oH.IsPersistentRecord
                ? Worldspace.Cells.Persistent
                : Worldspace.Cells.GetByGrid( GetPosition( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).WorldspaceToCellGrid() );
            var fileCellFormID = oH.GetCellFormID();
            var fileCell = Worldspace.Cells.Find( fileCellFormID ) as Engine.Plugin.Forms.Cell;
            var oldCell = Cell;
            if( ( newCell == oldCell )&&( fileCell == oldCell ) ) goto localAbort;
            
            if( !UpdateContainerCellHandle( newCell ) )
            {
                DebugLog.WriteError( "Unable to copy new CELL to working file!" );
                goto localAbort;
            }
            
            /*
            try
            {
                //var ncOH = newCell.WorkingFileHandle as XeLib.FormHandle;
                //oH.SetCell( ncOH );
                //var newHandle = oH.CopyMoveToCell( ncOH, true );
                //if( !newHandle.IsValid() )
                //    DebugLog.WriteError( this.FullTypeName(), "CheckForBackgroundCellChange()", "Unable to move object reference to new cell!" );
                //else
                //    this.AddNewHandle( newHandle );
            }
            catch( Exception e )
            {
                DebugLog.WriteException( e );
                //DebugLog.WriteError( this.FullTypeName(), "CheckForBackgroundCellChange()", e.ToString() );
            }
            */
            
            DebugLog.WriteStrings( null, new [] { "Cell.ObjectReferences.Remove()", oldCell.IDString }, false, false, false, false, false );
            oldCell.ObjectReferences.Remove( this );
            DebugLog.WriteStrings( null, new [] { "Cell.ObjectReferences.Add()", newCell.IDString }, false, false, false, false, false );
            newCell.ObjectReferences.Add( this );
            
            DebugLog.WriteLine( string.Format( "sendObjectDataChangedEvent = {0}", sendObjectDataChangedEvent.ToString() ) );
            if( sendObjectDataChangedEvent )
            {
                oldCell.SendObjectDataChangedEvent( null );
                newCell.SendObjectDataChangedEvent( null );
                this.SendObjectDataChangedEvent( null );
            }
            
        localAbort:
            DebugLog.CloseIndentLevel();
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
        
        public Fields.ObjectReference.LocationRefTypes LocationRefTypes
        {
            get
            {
                //DebugLog.Write( string.Format( "ObjectReference.LocationRefTypes :: 0x{0}", FormID.ToString( "X8" ) ) );
                return _locationRefTypes;
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
        
        public bool LinksTo( Engine.Plugin.TargetHandle target, uint referenceID )
        {
            int count = _LinkedRefs.GetCount( target );
            if( count < 1 )
                return false;
            
            for( int i = 0; i < count; i++ )
            {
                var rID = _LinkedRefs.GetReferenceID( target, i );
                if( rID == referenceID )
                    return true;
            }
            
            return false;
        }
        
        public ObjectReference NextInChain( Engine.Plugin.TargetHandle target, uint keywordFormID )
        {
            int count = _LinkedRefs.GetCount( target );
            if( count < 1 )
                return null;
            
            for( int i = 0; i < count; i++ )
            {
                var kFID = _LinkedRefs.GetKeywordFormID( target, i );
                if( kFID == keywordFormID )
                    return _LinkedRefs.GetReference( target, i );
            }
            
            return null;
        }

        #region Primitive Volume Corners

        public Vector2f[] GetCorners( Engine.Plugin.TargetHandle target )
        {
            if( _Primitive.GetType( target ) != Fields.ObjectReference.Primitive.PrimitiveType.Box ) return null;
            var p = _Position.GetValue( target );
            var r = _Rotation.GetValue( target );
            var b = _Primitive.GetBounds( target );
            return Vector2fExtensions.CalculateCornerPositions( p, r, b );
        }
        
        #endregion

        #region Debugging

        public override void DebugDumpChild( TargetHandle target )
        {
            if( _Name.HasValue( target ) )
            {
                var b = GodObject.Plugin.Data.Root.Find( GetNameFormID( target ) );
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
            var lrCount = _LinkedRefs.GetCount( target );
            if( lrCount > 0 )
            {
                DebugLog.WriteLine( "\tLinked Refs:" );
                for( int i = 0; i < lrCount; i++ )
                {
                    var lrRefr = _LinkedRefs.GetReference( target, i );
                    var lrKywd = _LinkedRefs.GetKeyword( target, i );
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
