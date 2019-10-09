/*
 * Position.cs
 *
 * Object Reference Position field.
 *
 */


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class Position : CachedVector3fField
    {
        
        public Position( Form form ) : base( form, "DATA", "Position" ) {}
        
    }
    
}
