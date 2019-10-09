/*
 * MapData.cs
 *
 * Worldspace Map Data fields.
 *
 */

using Maths;


namespace Engine.Plugin.Forms.Fields.Worldspace
{
    
    public class MapData : RawField //<Structs.MapData>
    {
        
        const string _XPath         = "MNAM";
        
        const string _CellNW        = @"Cell Coordinates\NW Cell";
        const string _CellSE        = @"Cell Coordinates\SE Cell";
        
        
        CachedVector2iField cached_NW;
        CachedVector2iField cached_SE;
        
        public MapData( Form form ) : base( form, "MNAM" )
        {
            cached_NW = new CachedVector2iField( form, _XPath, _CellNW );
            cached_SE = new CachedVector2iField( form, _XPath, _CellSE );
        }
        
        /*
        public bool HasValue()
        {
            return cached_NW.HasValue();
        }
        */
        
        public Vector2i GetCellNW( TargetHandle target )
        {
            return cached_NW.GetValue( target );
        }
        public void SetCellNW( TargetHandle target, Vector2i value )
        {
            cached_NW.SetValue( target, value );
        }
        
        public Vector2i GetCellSE( TargetHandle target )
        {
            return cached_SE.GetValue( target );
        }
        public void SetCellSE( TargetHandle target, Vector2i value )
        {
            cached_SE.SetValue( target, value );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "{0}-{1}",
                GetCellNW( target ).ToString(),
                GetCellSE( target ).ToString() );
        }
        
    }
    
}
