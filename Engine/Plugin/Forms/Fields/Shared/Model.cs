/*
 * Model.cs
 *
 * Model field used by multiple forms.
 *
 */


namespace Engine.Plugin.Forms.Fields.Shared
{
    
    public class Model : CachedStringField
    {
        
        public Model( Form form ) : base( form, "Model", "MODL" ) {}
        
    }
    
}
