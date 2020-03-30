/*
 * Location.cs
 * 
 * LoCaTioN form class.
 * 
 */

using System;

using XeLib;


namespace Engine.Plugin.Forms
{
    
    [Attributes.FormAssociation( "LCTN", "Location", true )]
    public class Location : Form
    {
        
        #region Common Fallout 4 Form fields
        
        Fields.Shared.FullName _FullName;
        Fields.Location.ParentLocation _ParentLocation;
        Fields.Location.WorldLocationRadius _WorldLocationRadius;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        //public Location() : base() {}
        
        public Location( string filename, uint formID ) : base( filename, formID ) {}
        
        //public Location( Plugin.File mod, Interface.IDataSync ancestor, Handle handle ) : base( mod, ancestor, handle ) {}
        public Location( Collection parentCollection, Interface.IXHandle ancestor, FormHandle handle ) : base( parentCollection, ancestor, handle ) {}
        
        public override void CreateChildFields()
        {
            _FullName = new Fields.Shared.FullName( this );
            _ParentLocation = new Fields.Location.ParentLocation( this );
            _WorldLocationRadius = new Fields.Location.WorldLocationRadius( this );
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
        
        public uint GetParentLocation( TargetHandle target )
        {
            return _ParentLocation.GetValue( target );
        }
        public void SetParentLocation( TargetHandle target, uint value )
        {
            _ParentLocation.SetValue( target, value );
        }
        
        public float GetWorldLocationRadius( TargetHandle target )
        {
            return _WorldLocationRadius.GetValue( target );
        }
        public void SetWorldLocationRadius( TargetHandle target, float value )
        {
            _WorldLocationRadius.SetValue( target, value );
        }
        
        #endregion
        
        #region Debugging
        
        public override void DebugDumpChild( TargetHandle target )
        {
            if( _FullName.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tFull Name: \"{0}\"", _FullName.ToString( target ) ) );
            if( _ParentLocation.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tParent Location: {0}", _ParentLocation.ToString( target ) ) );
            if( _WorldLocationRadius.HasValue( target ) )
                DebugLog.WriteLine( string.Format( "\tWorld Location Radius: {0}", _WorldLocationRadius.ToString( target ) ) );
        }
        
        #endregion
        
    }
    
}
