/*
 * WaterHeight.cs
 *
 * Cell WaterHeight field.
 *
 */

using System;
using XeLib;

namespace Engine.Plugin.Forms.Fields.Cell
{
    
    public class WaterHeight : CachedFloatField
    {
        
        public                          WaterHeight( Form form ) : base( form, "XCLW" ) {}
        
        public override float           GetValue( TargetHandle target )
        {
            if( ( ( (Engine.Plugin.Forms.Cell)Form ).GetFlags( target ) & (uint)Flags.Flag.HasWater ) == 0 ) return Engine.Constant.DefaultWaterHeight;
            var h = HandleFromTarget( target );
            if( !HasValue( h ) ) return Engine.Constant.DefaultWaterHeight;
            var value = base.GetValue( target );
            return value < Engine.Constant.DefaultWaterHeight
                ? value
                : ( Form.Ancestor != null )&&( Form.Ancestor.Signature == "WRLD" )
                    ? ( (Engine.Plugin.Forms.Worldspace)Form.Ancestor ).LandData.GetDefaultWaterHeight( target )
                    : Engine.Constant.DefaultWaterHeight;
        }
        
        public override void            SetValue( TargetHandle target, float value )
        {
            if( ( ( (Engine.Plugin.Forms.Cell)Form ).GetFlags( target ) & (uint)Flags.Flag.HasWater ) == 0 ) return;
            base.SetValue( target, value );
        }
        
    }
    
}
