/*
 * Formlist.cs
 * 
 * FormLiST form class.
 * 
 * TODO: WRITE ME!
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "FLST", "Formlist", true )]
    public class Formlist : Form
    {
        
        #region Formlist fields
        
        Fields.Formlist.FormIDs _FormIDs;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Layer() : base() {}
        
        public Formlist( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Layer( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Formlist( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FormIDs = new Fields.Formlist.FormIDs( this );
        }
        
        #endregion
        
        #endregion
        
        #region Properties
        
        #endregion
        
        #region Debugging
        
        public  void        DebugDumpFormlist( TargetHandle target )
        {
            if( !_FormIDs.HasValue( target ) ) return;

            var formIDs = _FormIDs.GetFormIDs( target );
            var pretty = formIDs.ConvertAll<string>( (x) => ( "0x" + x.ToString( "X8" ) ) );

            DebugLog.OpenIndentLevel( "target = " + target.ToString() );
            DebugLog.WriteList( "FormIDs", pretty, false, true );
            DebugLog.CloseIndentLevel();

        }
        
        #endregion
        
    }
    
}
