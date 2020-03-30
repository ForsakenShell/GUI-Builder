/*
 * Formlist.cs
 * 
 * FormLiST form class.
 * 
 * TODO: WRITE ME!
 */

using System;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "FLST", "FormList", true )]
    public class Formlist : Form
    {
        
        #region Formlist fields
        
        //Fields.Layer.Parent _Parent;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Layer() : base() {}
        
        public Formlist( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Layer( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Formlist( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            //_Parent = new Fields.Layer.Parent( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        /*
        public uint Parent
        {
            get
            {
                return _Parent.Value;
            }
            set
            {
                _Parent.Value = value;
            }
        }
        */
        
        #endregion
        
        #region Debugging
        
        /*
        public override void DebugDumpChild()
        {
            if( _Parent.HasValue() )
                DebugLog.WriteLine( string.Format( "\tParent: 0x{0}", _Parent.ToString() ) );
        }
        */
        
        #endregion
        
    }
    
}
