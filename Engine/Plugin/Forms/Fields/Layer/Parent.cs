/*
 * Parent.cs
 *
 * Layer Parent field.
 *
 */


namespace Engine.Plugin.Forms.Fields.Layer
{
    
    public class Parent : CachedUIntField
    {
        
        public Parent( Form form ) : base( form, "PNAM" ) {}
        
        public Forms.Layer GetLayer( TargetHandle target )
        {
            var lrID = GetValue( target );
            return !lrID.ValidFormID()
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( lrID, true );
        }
        public void SetLayer( TargetHandle target, Forms.Layer value )
        {
            if( value == null )
                DeleteRootElement( true, true );
            else
                SetValue( target, value.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
        }
        
    }
    
}
