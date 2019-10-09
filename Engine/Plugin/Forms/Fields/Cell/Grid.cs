/*
 * Grid.cs
 *
 * Cell Grid field.
 *
 */

using Maths;


namespace Engine.Plugin.Forms.Fields.Cell
{
    
    public class CellGrid : RawField
    {
        
        const string _XPath         = "XCLC";
        
        const string _ForceHideLand = "Force Hide Land";
        
        CachedVector2iField cache_G = null;
        CachedUIntField cache_Force = null;
        
        public CellGrid( Form form ) : base( form, _XPath )
        {
            cache_G = new CachedVector2iField( form, _XPath );
            cache_Force = new CachedUIntField( form, _XPath, _ForceHideLand );
        }
        
        /*
        public bool HasValue( TargetHandle target )
        {
            var h = cache_G.HandleFromTarget( target );
            if( !XeLib.HandleExtensions.IsValid( h ) )
                throw new System.ArgumentException( "target is not valid for field" );
            return cache_G.HasValue( h );
        }
        */
        
        public Vector2i GetGrid( TargetHandle target )
        {
            return cache_G.GetValue( target );
        }
        public void SetGrid( TargetHandle target, Vector2i value )
        {
            cache_G.SetValue( target, value );
        }
        
        public uint GetForceHideLand( TargetHandle target )
        {
            return cache_Force.GetValue( target );
        }
        public void SetForceHideLand( TargetHandle target, uint value )
        {
            cache_Force.SetValue( target, value );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "Grid: {0} :: ForceHideLand: 0x{1}",
                GetGrid( target ).ToString(), GetForceHideLand( target ).ToString( "X8" ) );
        }
        
    }
    
}
