/*
 * Flags.cs
 *
 * Cell Flags field.
 *
 */

using System;


namespace Engine.Plugin.Forms.Fields.Cell
{
    
    public class Flags : CachedUIntField
    {
        
        [Flags]
        public enum Flag : uint
        {
            IsInteriorCell                  = 0x00000001,
            HasWater                        = 0x00000002,
            CantTravelFromHere              = 0x00000004,
            NoLODWater                      = 0x00000008,
            PublicArea                      = 0x00000020,
            HandChanged                     = 0x00000040,
            ShowSky                         = 0x00000080,
            UseSkyLighting                  = 0x00000100,
            SunlightShadows                 = 0x00000800,
            DistantLODOnly                  = 0x00001000,
            PlayerFollowersCantTravelHere   = 0x00002000,
            PackInCell                      = 0x00000400    // Undocumented but seems to be
        }
        
        public Flags( Form form ) : base( form, "DATA" ) {}
        
    }
    
}
