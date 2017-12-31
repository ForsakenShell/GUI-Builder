/*
 * bbConstant.cs
 * 
 * Global constants used by Border Builder.
 * 
 * User: 1000101
 * Date: 25/11/2017
 * Time: 6:17 PM
 * 
 */
using System;

namespace Border_Builder
{
    /// <summary>
    /// Description of bbConstant.
    /// </summary>
    public static class bbConstant
    {
        public const string DataPath = "BorderBuilder";
        public const string BorderConfigPath = DataPath + "\\BorderConfigs";
        public const string WorldspacePath = DataPath + "\\Worldspaces";
        public const string ImportModPath = DataPath + "\\Mods";
        
        public const float MaxZoom = 1f / 16f;
        public const float MinZoom = 1f / 128f;
        public const float HeightMap_Resolution = 32;
        public const float HeightMap_Alignment = 1024;
        public const float WorldMap_Resolution = 4096;
        public const float HeightMap_To_Worldmap = WorldMap_Resolution / HeightMap_Resolution;
        
    }
    
}
