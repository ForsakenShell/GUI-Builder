/*
 * Primitive.cs
 *
 * Object Reference Primitive fields.
 *
 */

using sdColor = System.Drawing.Color;

using Maths;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class Primitive : RawField
    {
        
        const string _XPath         = "XPRM";
        
        const string _BOUNDS        = "Bounds";
        const string _COLOR         = "Color";
        const string _UNKNOWN       = "Unknown";
        const string _TYPE          = "Type";
        
        CachedVector3fField cache_B = null;
        CachedColorField cache_C = null;
        CachedFloatField cache_U = null;
        CachedIntField cache_T = null;
        
        public Primitive( Form form ) : base( form, _XPath )
        {
            cache_B = new CachedVector3fField( form, _XPath, _BOUNDS );
            cache_C = new CachedColorField( form, _XPath, _COLOR );
            cache_U = new CachedFloatField( form, _XPath, _UNKNOWN );
            cache_T = new CachedIntField( form, _XPath, _TYPE );
        }
        
        public Vector3f GetBounds( TargetHandle target )
        {
            return cache_B.GetValue( target );
        }
        public void SetBounds( TargetHandle target, Vector3f value )
        {
            cache_B.SetValue( target, value );
        }
        
        public sdColor GetColor( TargetHandle target )
        {
            return cache_C.GetValue( target );
        }
        public void SetColor( TargetHandle target, sdColor value )
        {
            cache_C.SetValue( target, value );
        }
        
        public float GetUnknown( TargetHandle target )
        {
            return cache_U.GetValue( target );
        }
        public void SetUnknown( TargetHandle target, float value )
        {
            cache_U.SetValue( target, value );
        }
        
        public int GetType( TargetHandle target )
        {
            return cache_T.GetValue( target );
        }
        public void SetType( TargetHandle target, int value )
        {
            cache_T.SetValue( target, value );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "Bounds: {0} :: Color: {1} :: Unknown {2} :: Type: {3}",
                GetBounds( target ).ToString(),
                GetColor( target ).ToString(),
                GetUnknown( target ),
                GetType( target ) );
        }
        
    }
    
}
