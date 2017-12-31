/*
 * bbGlobal.cs
 * 
 * Global functions and enumated data for use by Border Builder.
 * 
 * User: 1000101
 * Date: 25/11/2017
 * Time: 6:17 PM
 * 
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Border_Builder
{
    /// <summary>
    /// Description of bbGlobal.
    /// </summary>
    public static class bbGlobal
    {
        
        private static string _globalPath = null;
        public static string GlobalPath
        {
            get
            {
                return string.IsNullOrEmpty( _globalPath ) ? "" : _globalPath;
            }
            set
            {
                if( string.IsNullOrEmpty( value ) )
                {
                    _globalPath = null;
                    return;
                }
                _globalPath = value;
                if( !_globalPath.EndsWith( @"/"  ) )
                    _globalPath += "\\";
            }
        }
        
        private static string[] _borderConfigPaths = null;
        public static string[] BorderConfigPaths
        {
            get
            {
                if( _borderConfigPaths == null )
                {
                    _borderConfigPaths = Directory.GetDirectories( GlobalPath  + bbConstant.BorderConfigPath );
                }
                return _borderConfigPaths;
            }
        }
        
        private static string[] _worldspacePaths = null;
        public static string[] WorldspacePaths
        {
            get
            {
                if( _worldspacePaths == null )
                {
                    _worldspacePaths = Directory.GetDirectories( GlobalPath  + bbConstant.WorldspacePath );
                }
                return _worldspacePaths;
            }
        }
        
        private static string[] _importModPaths = null;
        public static string[] ImportModPaths
        {
            get
            {
                if( _importModPaths == null )
                {
                    _importModPaths = Directory.GetDirectories( GlobalPath  + bbConstant.ImportModPath );
                }
                return _importModPaths;
            }
        }
        
        public static List<bbWorldspace> Worldspaces = null;
        
        public static List<bbImportMod> ImportMods = null;
        
        public static bbWorldspace WorldspaceFromEditorID( int editorID )
        {
            foreach( var worldspace in Worldspaces )
                if( worldspace.EditorID == editorID )
                    return worldspace;
            return null;
        }
        
        public static Maths.Vector2i HeightMapToCellGrid( int x, int y )
        {
            return new Maths.Vector2i(
                (  (int)((float)x / bbConstant.HeightMap_Resolution ) ),
                ( -(int)((float)y / bbConstant.HeightMap_Resolution ) )
           );
        }
        
        public static Maths.Vector2i WorldSpaceToCellGrid( float x, float y )
        {
            return new Maths.Vector2i(
                ( (int)( x / bbConstant.WorldMap_Resolution ) ),
                ( (int)( y / bbConstant.WorldMap_Resolution ) )
           );
        }
        
        public static Maths.Vector2i WorldSpaceToCellGrid( Maths.Vector2f v )
        {
            return WorldSpaceToCellGrid( v.X, v.Y );
        }
        
        /*
        public static float Clamp( float value, float lower, float upper )
        {
            return value < lower ? lower :
                value > upper ? upper :
                value;
        }
        
        public static byte Clamp( byte value, byte lower, byte upper )
        {
            return value < lower ? lower :
                value > upper ? upper :
                value;
        }
        
        public static int Clamp( int value, int lower, int upper )
        {
            return value < lower ? lower :
                value > upper ? upper :
                value;
        }
        */
        
    }
    
}
