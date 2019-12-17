/*
 * Constants.cs
 * 
 * Global constants used by GUI Builder but not specific to masters and plugins.
 * 
 */

using System;

namespace GUIBuilder
{
    /// <summary>
    /// Description of Constant.
    /// </summary>
    public static class Constant
    {
        public const string BorderBuilderPath = "BorderBuilder";
        public const string BorderConfigPath = "BorderConfigs";
        public const string WorldspacePath = "Worldspaces";
        public const string WorkspacePath = "Workspaces";
        public const string ImportModPath = "Mods";
        public const string RenderTransformImagePath = "RenderTransform";
        public const string DefaultNIFBuilderOutputPath = "Output";
        
        public const string GUIBuilderConfigFile = "GUIBuilder_Options.xml";
        
        public static readonly string DefaultLanguage = "English (UK)";
        public static readonly string LanguageSubPath = "Lang";
        public static readonly string LanguageFile = "Translate.xml";
        
        public const float MaxPictureBoxSize = 16384f;
        public const float MaxZoom = Engine.Constant.WorldMap_Resolution / MaxPictureBoxSize;
        public const float MinZoom = Engine.Constant.HeightMap_Resolution / MaxPictureBoxSize;
        
    }
    
}
