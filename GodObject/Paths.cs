/*
 * Paths.cs
 * 
 * File paths used by GUIBuilder.
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace GodObject
{
    
    public static class Paths
    {
        
        static readonly string          XmlNode_Options     = "Options";
        static readonly string          XmlNode_Paths       = "Paths";
        static readonly string          XmlLanguageKey      = "Language";
        static readonly string          XmlOutputKey        = "Output";
        
        #region Fallout 4 Paths
        
        static string _fallout4 = null;
        public static string Fallout4
        {
            get
            {
                return _fallout4;
            }
            set
            {
                if( string.IsNullOrEmpty( value ) )
                {
                    _fallout4 = string.Empty;
                    return;
                }
                value.TryAssignPath( ref _fallout4 );
            }
        }
        
        static string _fallout4Data = null;
        public static string Fallout4Data
        {
            get
            {
                if( string.IsNullOrEmpty( _fallout4Data ) )
                {
                    var fo4Path = Fallout4;
                    if( string.IsNullOrEmpty( fo4Path ) )
                        return null;
                    var tryPath = fo4Path + Engine.Constant.DataPath;
                    if( !tryPath.TryAssignPath( ref _fallout4Data ) )
                        return null;
                }
                return _fallout4Data;
            }
        }
        
        #endregion
        
        #region GUIBuilder Path
        
        static string _borderBuilder = null;
        public static string BorderBuilder
        {
            get
            {
                if( string.IsNullOrEmpty( _borderBuilder ) )
                {
                    // Try current path, then Fallout 4 path
                    var readPath = System.Environment.CurrentDirectory + "\\";
                    var tryPath = readPath + GUIBuilder.Constant.BorderBuilderPath;
                    if( !tryPath.TryAssignPath( ref _borderBuilder ) )
                    {
                        readPath = Fallout4;
                        if( string.IsNullOrEmpty( readPath ) )
                            return null;
                        tryPath = readPath + GUIBuilder.Constant.BorderBuilderPath;
                        if( !tryPath.TryAssignPath( ref _borderBuilder ) )
                            return null;
                    }
                }
                return _borderBuilder;
            }
            set
            {
                if( string.IsNullOrEmpty( value ) )
                {
                    _borderBuilder = string.Empty;
                    return;
                }
                value.TryAssignPath( ref _borderBuilder );
            }
        }
        
        static string _Language = string.Empty;
        public static string Language
        {
            get
            {
                if( string.IsNullOrEmpty( _Language ) )
                    _Language = XmlConfig.ReadStringValue( XmlNode_Options, XmlLanguageKey, GUIBuilder.Constant.DefaultLanguage );
                return _Language;
            }
            set
            {
                var bbPath = BorderBuilder;
                if( string.IsNullOrEmpty( bbPath ) )
                    return;
                if(
                    ( string.IsNullOrEmpty( value ) )||
                    ( !string.Format( "{0}{1}/{2}/{3}", bbPath, GUIBuilder.Constant.LanguageSubPath, value, GUIBuilder.Constant.LanguageFile ).FileExists() )
                )   value = GUIBuilder.Constant.DefaultLanguage;
                _Language = value;
                XmlConfig.WriteStringValue( XmlNode_Options, XmlLanguageKey, value );
            }
        }
        
        static string _GUIBuilderLanguageFile = string.Empty;
        public static string GUIBuilderLanguageFile
        {
            get
            {
                if( string.IsNullOrEmpty( _GUIBuilderLanguageFile ) )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var language = Language;
                    if( string.IsNullOrEmpty( language ) )
                        return null;
                    var tryFile = string.Format( "{0}{1}/{2}/{3}", bbPath, GUIBuilder.Constant.LanguageSubPath, language, GUIBuilder.Constant.LanguageFile );
                    if( !tryFile.TryAssignFile( ref _GUIBuilderLanguageFile ) )
                        return null;
                }
                return _GUIBuilderLanguageFile;
            }
        }
        
        static List<string> _LanguageOptions = null;
        public static List<string> LanguageOptions
        {
            get
            {
                if( _LanguageOptions.NullOrEmpty() )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var langPath = bbPath + GUIBuilder.Constant.LanguageSubPath;
                    var langPaths = new List<string>( System.IO.Directory.EnumerateDirectories( langPath ) );
                    var languages = new List<string>();
                    foreach( var lang in langPaths )
                    {
                        var testFile = string.Format( "{0}/{1}", lang, GUIBuilder.Constant.LanguageFile );
                        if( testFile.FileExists() )
                            languages.Add( lang.Substring( lang.LastIndexOf( "\\", StringComparison.Ordinal ) + 1 ) );
                    }
                    if( !languages.NullOrEmpty() )
                        _LanguageOptions = languages;
                }
                return _LanguageOptions;
            }
        }
        
        static string _GUIBuilderConfigFile = string.Empty;
        public static string GUIBuilderConfigFile
        {
            get
            {
                if( string.IsNullOrEmpty( _GUIBuilderConfigFile ) )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var tryFile = bbPath + GUIBuilder.Constant.GUIBuilderConfigFile;
                    if( !tryFile.TryAssignFile( ref _GUIBuilderConfigFile ) )
                        return null;
                }
                return _GUIBuilderConfigFile;
            }
        }
        
        static string _NIFBuilderOutput = null;
        public static string NIFBuilderOutput
        {
            get
            {
                if( string.IsNullOrEmpty( _NIFBuilderOutput ) )
                {
                    _NIFBuilderOutput = GodObject.XmlConfig.ReadStringValue( XmlNode_Paths, XmlOutputKey );
                    if( !string.IsNullOrEmpty( _NIFBuilderOutput ) )
                        return _NIFBuilderOutput;
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var tryPath = bbPath + GUIBuilder.Constant.DefaultNIFBuilderOutputPath;
                    if( !tryPath.TryAssignPath( ref _NIFBuilderOutput ) )
                        return null;
                    GodObject.XmlConfig.WriteStringValue( XmlNode_Paths, XmlOutputKey, _NIFBuilderOutput, true );
                }
                return _NIFBuilderOutput;
            }
            set
            {
                _NIFBuilderOutput = value;
                GodObject.XmlConfig.WriteStringValue( XmlNode_Paths, XmlOutputKey, _NIFBuilderOutput, true );
            }
        }
        
        static string _renderTransformImage = null;
        public static string RenderTransformImage
        {
            get
            {
                if( string.IsNullOrEmpty( _renderTransformImage ) )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var tryPath = bbPath + GUIBuilder.Constant.RenderTransformImagePath;
                    if( !tryPath.TryAssignPath( ref _renderTransformImage ) )
                        return null;
                }
                return _renderTransformImage;
            }
        }
        
        static string[] _worldspace = null;
        public static string[] Worldspace
        {
            get
            {
                if( _worldspace == null )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    string readPath = null;
                    var tryPath = bbPath + GUIBuilder.Constant.WorldspacePath;
                    if( !tryPath.TryAssignPath( ref readPath ) )
                        return null;
                    _worldspace = Directory.GetDirectories( readPath );
                }
                return _worldspace;
            }
        }
        
        #endregion
    }
    
}

