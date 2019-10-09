/*
 * WorldBounds.cs
 *
 * Worldspace Bounds fields.
 *
 */

using Maths;


namespace Engine.Plugin.Forms.Fields.Worldspace
{
    
    public class WorldBounds : RawField //<Structs.WorldBounds>
    {
        
        const string _XPath         = "Object Bounds";
        
        const string _Min           = @"NAM0";
        const string _Max           = @"NAM9";
        
        CachedVector2iField cached_Min;
        CachedVector2iField cached_Max;
        
        public WorldBounds( Form form ) : base( form, @"Object Bounds" )
        {
            cached_Min = new CachedVector2iField( form, _XPath, _Min );
            cached_Max = new CachedVector2iField( form, _XPath, _Max );
        }
        
        /*
        public bool HasValue()
        {
            return cached_Min.HasValue();
        }
        */
        
        public Vector2i GetMin( TargetHandle target )
        {
            return cached_Min.GetValue( target );
        }
        public void SetMin( TargetHandle target, Vector2i value )
        {
            cached_Min.SetValue( target, value );
        }
        
        public Vector2i GetMax( TargetHandle target )
        {
            return cached_Max.GetValue( target );
        }
        public void SetMax( TargetHandle target, Vector2i value )
        {
            cached_Max.SetValue( target, value );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "{0}-{1}",
                GetMin( target ).ToString(),
                GetMax( target ).ToString() );
        }
        
    }
    
}
