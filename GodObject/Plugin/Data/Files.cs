/*
 * Files.cs
 * 
 * Global list of loaded masters and plugins.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Fallout4;
using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;
using XeLibHelper;
using Engine.Plugin.Interface;

namespace GodObject
{
    
    public static partial class Plugin
    {
        
        public static partial class Data
        {
            
            public static class Files
            {
                
                public static Engine.Plugin.File Working = null;
                public static List<Engine.Plugin.File> Loaded = null;
                
                public static void Clear()
                {
                    Working = null;
                    if( Loaded != null )
                        foreach( var file in Loaded )
                            file.Dispose();
                    Loaded = null;
                }
                
                public static bool IsLoaded( string filename )
                {
                    var f = Find( filename );
                    if( f == null )
                        return false;
                    return f.LoadOrder != Engine.Plugin.Constant.LO_Invalid;
                }
                
                public static Engine.Plugin.File Find( string filename )
                {
                    if( ( string.IsNullOrEmpty( filename ) )||( Loaded.NullOrEmpty() ) )
                        return null;
                    
                    foreach( var file in Loaded )
                    {
                        //Console.WriteLine( filename + " ?= " + file.Filename );
                        if( filename.InsensitiveInvariantMatch( file.Filename ) )
                            return file;
                    }
                    return null;
                }
                
                public static Engine.Plugin.File Find( ElementHandle handle )
                {
                    if( ( !handle.IsValid() )||( Loaded.NullOrEmpty() ) )
                        return null;
                    return Find( handle.Filename );
                }
                
            }
            
        }
        
    }
    
}