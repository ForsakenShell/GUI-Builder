/*
 * Container.cs
 * 
 * CONTainer form class.
 * 
 */

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "CONT", "Container", true )]
    public class Container : Form
    {
        
        #region Container Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Shared.Model _Model;
        Fields.Shared.ObjectBounds _ObjectBounds;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Container() : base() {}
        
        public Container( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Container( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Container( Interface.ICollection container, Interface.IXHandle ancestor, FormHandle handle ) : base( container, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FullName = new Fields.Shared.FullName( this );
            _Model = new Fields.Shared.Model( this );
            _ObjectBounds = new Fields.Shared.ObjectBounds( this );
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
        }
        
        #endregion
        
    }
    
}
