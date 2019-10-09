/*
 * Rotation.cs
 *
 * Object Reference Rotation field.
 *
 */


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class Rotation : CachedVector3fField
    {
        
        public Rotation( Form form ) : base( form, "DATA", "Rotation" ) {}
        
    }
    
}
