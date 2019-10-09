/*
 * FullName.cs
 *
 * Full name field used by multiple forms.
 *
 */


namespace Engine.Plugin.Forms.Fields.Shared
{
    
    public class FullName : CachedStringField
    {
        
        public FullName( Form form ) : base( form, "FULL" ) {}
        
    }
    
}
