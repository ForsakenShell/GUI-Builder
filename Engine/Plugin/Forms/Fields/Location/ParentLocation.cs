/*
 * ParentLocation.cs
 *
 * Location Parent Location field.
 *
 */


namespace Engine.Plugin.Forms.Fields.Location
{
    
    public class ParentLocation : CachedUIntField
    {
        
        public ParentLocation( Form form ) : base( form, "PNAM" ) {}
        
        public Forms.Location GetLocation( TargetHandle target )
        {
            var lrID = GetValue( target );
            return !lrID.ValidFormID()
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Location>( lrID, true );
        }
        public void SetLocation( TargetHandle target, Forms.Location value )
        {
            if( value == null )
                DeleteRootElement( true, true );
            else
                SetValue( target, value.GetFormID( Engine.Plugin.TargetHandle.Master ) );
        }
    }
    
}
