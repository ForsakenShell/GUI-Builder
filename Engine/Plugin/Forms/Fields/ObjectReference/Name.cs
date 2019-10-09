/*
 * Name.cs
 *
 * Object Reference Name field (what the REFR is an instance of).
 *
 */


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class Name : CachedUIntField
    {
        
        public Name( Form form ) : base( form, "NAME" ) {}
        
    }
    
}
