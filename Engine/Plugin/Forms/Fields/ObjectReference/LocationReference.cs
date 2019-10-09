/*
 * LocationReference.cs
 *
 * Object Reference Location Reference field.
 *
 */

using System;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class LocationReference : CachedUIntField
    {
        
        public LocationReference( Form form ) : base( form, "XLRL" ) {}
        
        public Forms.Location           GetLocation( TargetHandle target )
        {
            var lrID = GetValue( target );
            return !lrID.ValidFormID()
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Location>( lrID, true );
        }
        
        public void                     SetLocation( TargetHandle target, Forms.Location value )
        {
            if( target != TargetHandle.Working )
                throw new NotImplementedException();
            if( value == null )
                DeleteRootElement( true, true );
            else
                SetValue( target, value.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
        }
        
        /* TODO:  Reimplement as Field<> ToString( TargetHandle target ) override
        public override string          ToString()
        {
            if( !HasValue() ) return null;
            var lID = Value;
            if( lID == Constant.FormID_None ) return null;
            var location = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Location>( lID, true );
            if( location == null ) throw new Exception( "Unable to load Location Form 0x" + lID.ToString( "X8" ) );
            return location.ToString();
            //return string.Format(
            //    "\"{0}\" - 0x{1} - \"{2}\"",
            //    location.Signature,
            //    location.FormID.ToString( "X8" ),
            //    location.EditorID
            //);
        }
        */
        
    }
    
}
