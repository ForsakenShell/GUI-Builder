/*
 * Layer.cs
 * 
 * LAYeR form class.
 * 
 */

using System;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "LAYR", "Layer", true )]
    public class Layer : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.Layer.Parent _Parent;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Layer() : base() {}
        
        public Layer( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Layer( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Layer( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _Parent = new Fields.Layer.Parent( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        public uint GetParent( TargetHandle target )
        {
            return _Parent.GetValue( target );
        }
        public void SetParent( TargetHandle target, uint value )
        {
            _Parent.SetValue( target, value );
        }
        
        #endregion
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _Parent.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tParent: ", _Parent.ToString( target ) ) );
        }
        
        #endregion
        
    }
    
}
