/*
 * Static.cs
 * 
 * STATic object form class.
 * 
 */

using System;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "STAT", "Static Object", true )]
    public class Static : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Shared.Model _Model;
        Fields.Shared.ObjectBounds _ObjectBounds;
        Fields.Static.DistantLOD _DistantLOD;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Static() : base() {}
        
        public Static( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Static( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Static( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FullName = new Fields.Shared.FullName( this );
            _Model = new Fields.Shared.Model( this );
            _ObjectBounds = new Fields.Shared.ObjectBounds( this );
            _DistantLOD = new Fields.Static.DistantLOD( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        public string GetFullName( TargetHandle target )
        {
            return _FullName.GetValue( target );
        }
        public void SetFullName( TargetHandle target, string value )
        {
            _FullName.SetValue( target, value );
        }
        
        public string GetModel( TargetHandle target )
        {
            return _Model.GetValue( target );
        }
        public void SetModel( TargetHandle target, string value )
        {
            _Model.SetValue( target, value );
        }
        
        public Fields.Shared.ObjectBounds ObjectBounds
        {
            get
            {
                return _ObjectBounds;
            }
        }
        
        public Fields.Static.DistantLOD DistantLOD
        {
            get
            {
                return _DistantLOD;
            }
        }
        
        #endregion
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _FullName.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFull Name: \"{0}\"", _FullName.ToString( target ) ) );
            if( _Model.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tModel: \"{0}\"", _Model.ToString( target ) ) );
            if( _ObjectBounds.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tObject Bounds: {0}", _ObjectBounds.ToString( target ) ) );
            if( _DistantLOD.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tDistant LOD:\n{0}", _DistantLOD.ToString( target ) ) );
        }
        
        #endregion
        
    }
    
}
