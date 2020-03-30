/*
 * LocationRef.cs
 * 
 * LoCation Reference Type form class.
 * 
 */

using System;
using sdColor = System.Drawing.Color;

using XeLib;


namespace Engine.Plugin.Forms
{

    [Attributes.FormAssociation( "LCRT", "LocationRef", true )]
    public class LocationRef : Form
    {

        #region Common Fallout 4 Form fields
        
        Fields.LocationRef.Color _Color;

        #endregion

        #region Allocation & Disposal

        #region Allocation

        //public Location() : base() {}

        public LocationRef( string filename, uint formID ) : base( filename, formID ) { }

        //public Location( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public LocationRef( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) { }

        public override void CreateChildFields()
        {
            _Color = new Fields.LocationRef.Color( this );
        }

        #endregion

        #endregion

        #region Properties

        public sdColor GetColor( TargetHandle target )
        {
            return _Color.GetValue( target );
        }
        public void SetColor( TargetHandle target, sdColor value )
        {
            _Color.SetValue( target, value );
        }


        #endregion

        #region Debugging

        public override void DebugDumpChild( TargetHandle target )
        {
            if( _Color.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tColor: {0}", _Color.ToString( target ) ) );
        }

        #endregion

    }

}
