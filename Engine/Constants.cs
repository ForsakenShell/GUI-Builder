/*
 * Constants.cs
 * 
 * Global constants used by the Fallout 4 Engine.
 * 
 */

namespace Engine
{
    
    public static class Constant
    {
        public const string DataPath = "Data";
        
        public const float HeightMap_Resolution = 32;
        //public const float HeightMap_Alignment = 1024;
        public const float WorldMap_Resolution = 4096;
        public const float HeightMap_To_Worldmap = WorldMap_Resolution / HeightMap_Resolution;
        public const float WorldMap_To_Heightmap = HeightMap_Resolution / WorldMap_Resolution;
        
        public const float DefaultWaterHeight = float.MaxValue;
        
    }
    
}
