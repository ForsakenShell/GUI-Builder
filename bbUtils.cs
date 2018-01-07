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
using System.Collections.Generic;

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
            const string commentStart = "//";
            string working = importLine.Trim( ' ' );
            working = working.Trim( '\t' );
            if( working.StartsWith( commentStart ) ) return null;
            
            string[] words = working.Split( delimiter );
            for( int i = 0; i < words.Length; i++ )
                words[ i ] = words[ i ].Trim( ' ' );
            return words;
        }
        
        #endregion
        
        #region List Helpers
        
        /// <summary>
        /// Is the list null or contain 0 elements?
        /// </summary>
        /// <param name="list">List to test for null or empty</param>
        /// <returns></returns>
        public static bool NullOrEmpty<T>( this IList<T> list )
        {
            if( list == null ) return true;
            return list.Count == 0;
        }
        
        /// <summary>
        /// Does the list contain all elements of the other list?
        /// </summary>
        /// <param name="list">The list to check the contents of</param>
        /// <param name="other">The other list with the items to check</param>
        /// <returns></returns>
        public static bool Contains<T>( this IList<T> list, IList<T> other )
        {
            if( list.NullOrEmpty() ) return false;
            if( other.NullOrEmpty() ) return false;
            foreach( var item in other )
                if( !list.Contains( item ) ) return false;
            return true;
        }
        
        #endregion
        
    }
}
