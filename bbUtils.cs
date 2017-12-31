/*
 * bbUtils.cs
 * 
 * Utility functions that don't aren't associated with a specific class.
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:17 PM
 * 
 */
using System;
using System.IO;

namespace Border_Builder
{
    /// <summary>
    /// Description of bbUtils.
    /// </summary>
    public static class bbUtils
    {
        
        #region File and path safe assignment (validates)
        
        public static bool TryAssignPath( ref string target, string newPath )
        {
            if( !Directory.Exists( newPath ) )
                return false;
            if( ( !newPath.EndsWith( @"/" ) )&&( !newPath.EndsWith( @"\" ) ) )
               target = newPath + @"\";
            else
                target = newPath;
            return true;
        }
        
        public static  bool TryAssignFile( ref string target, string newFile)
        {
            if( !File.Exists( newFile ) )
                return false;
            target = newFile;
            return true;
        }
        
        #endregion
        
        #region Import file line splitting and trimming
        
        public static string[] ParseImportLine( this string importLine, char delimiter = ';' )
        {
            string[] words = importLine.Split( delimiter );
            for( int i = 0; i < words.Length; i++ )
                words[ i ] = words[ i ].Trim( ' ' );
            return words;
        }
        
        #endregion
        
    }
}
