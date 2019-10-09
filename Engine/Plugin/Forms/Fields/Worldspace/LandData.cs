/*
 * LandData.cs
 *
 * Worldspace Land Data fields.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields.Worldspace
{
    
    public class LandData : RawField
    {
        
        const string                   _XPath                       = "DNAM";
        
        const string                   _DefaultLandHeight           = @"Default Land Height";
        const string                   _DefaultWaterHeight          = @"Default Water Height";
        
        CachedFloatField                cached_LH;
        CachedFloatField                cached_WH;
        
        public                          LandData( Form form ) : base( form, _XPath )
        {
            cached_LH = new CachedFloatField( form, _XPath, _DefaultLandHeight );
            cached_WH = new CachedFloatField( form, _XPath, _DefaultWaterHeight );
        }
        
        public float                    GetDefaultLandHeight( TargetHandle target )                 { return cached_LH.GetValue( target ); }
        public void                     SetDefaultLandHeight( TargetHandle target, float value )    { cached_LH.SetValue( target, value ); }
        
        public float                    GetDefaultWaterHeight( TargetHandle target )                { return cached_WH.GetValue( target ); }
        public void                     SetDefaultWaterHeight( TargetHandle target, float value )   { cached_WH.SetValue( target, value ); }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                string.IsNullOrEmpty( format ) ? "Default Land Height = {0} :: Default Water Height {1}" : format,
                cached_LH.ToString( target ),
                cached_WH.ToString( target ) );
        }
        
    }
    
}
