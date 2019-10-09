/*
 * Paths.cs
 * 
 * File paths used by GUIBuilder.
 * 
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using Maths;

using Engine;
using GUIBuilder;

using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;

namespace GodObject
{
    
    public static class Paths
    {
        
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
                    var readPath = Fallout4;
                    if( string.IsNullOrEmpty( readPath ) )
                        readPath = System.Environment.CurrentDirectory;
                    var tryPath = readPath + GUIBuilder.Constant.BorderBuilderPath;
                    if( !tryPath.TryAssignPath( ref _borderBuilder ) )
                        return null;
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
        
        static string _defaultNIFBuilderOutput = null;
        public static string DefaultNIFBuilderOutput
        {
            get
            {
                if( string.IsNullOrEmpty( _defaultNIFBuilderOutput ) )
                {
                    var bbPath = BorderBuilder;
                    if( string.IsNullOrEmpty( bbPath ) )
                        return null;
                    var tryPath = bbPath + GUIBuilder.Constant.DefaultNIFBuilderOutputPath;
                    if( !tryPath.TryAssignPath( ref _defaultNIFBuilderOutput ) )
                        return null;
                }
                return _defaultNIFBuilderOutput;
            }
            set
            {
                _defaultNIFBuilderOutput = value;
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

