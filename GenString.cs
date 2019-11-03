/*
 * GenString.cs
 * 
 * Generic functions for strings
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Description of GenString.
/// </summary>
public static class GenString
{
    
    #region String matching
    
    public static bool InsensitiveInvariantMatch( this string a, string b )
    {
        return string.Equals( a, b, StringComparison.InvariantCultureIgnoreCase );
    }
    
    #endregion
    
    #region string splitting and trimming
    
    public static string[] ParseImportLine( this string importLine, char delimiter = ';' )
    {
        const string commentStart = "//";
        string working = importLine.Trim( ' ' );
        working = working.Trim( '\t' );
        if( working.StartsWith( commentStart, StringComparison.InvariantCulture ) ) return null;
        
        string[] words = working.Split( delimiter );
        for( int i = 0; i < words.Length; i++ )
            words[ i ] = words[ i ].Trim( ' ' );
        return words;
    }
    
    public static string StripFrom( this string str, string word, System.StringComparison comparison )
    {
        var sl = str.Length;
        var wl = word.Length;
        if( ( wl == 0 )||( wl > sl ) )
            return str;
        
        var s = str;
        var wi = s.IndexOf( word, comparison );
        if( wi < 0 )
            return str;
        
        if( wi == 0 )   // Starts with it
            return s.Substring( wi, sl - wi );
        
        if( wi + wl == sl ) // End of it
            return s.Substring( 0, wi );
        
        // Middle of it
        return s.Substring( 0, wi ) + s.Substring( wi + wl, sl - wi - wl );
    }
    
    #endregion
    
    #region string Arrays and Lists
    
    public static uint LongestStringSizeInArray( this string[] array )
    {
        uint longest = 0;
        for( int i = 0; i < array.Length; i++ )
        {
            if( !string.IsNullOrEmpty( array[ i ] ) )
            {
                var length = (uint)array[ i ].Length;
                if( length > longest ) longest = length;
            }
        }
        return longest;
    }
    
    public static uint LongestStringSizeInList( this List<string> list )
    {
        uint longest = 0;
        for( int i = 0; i < list.Count; i++ )
        {
            if( !string.IsNullOrEmpty( list[ i ] ) )
            {
                var length = (uint)list[ i ].Length;
                if( length > longest ) longest = length;
            }
        }
        return longest;
    }
    
    /// <summary>
    /// Count the number of keys that are contained in a string.
    /// </summary>
    /// <param name="l">string to check</param>
    /// <param name="k">keys to check</param>
    /// <param name="c">comparison type</param>
    /// <returns></returns>
    public static int CountKeysInString( this string l, List<string> k, StringComparison c )
    {
        if( k == null ) return 0;
        var m = (int)0;
        foreach( var t in k )
        {
            if( string.IsNullOrEmpty( t ) ) continue;
            if( l.IndexOf( t, c ) < 0 ) continue;
            m++;
        }
        return m;
    }
    
    /// <summary>
    /// Count the number of keys that are contained in two strings.
    /// </summary>
    /// <param name="l">first string to check</param>
    /// <param name="r">second string to check</param>
    /// <param name="k">keys to check</param>
    /// <param name="c">comparison type</param>
    /// <returns></returns>
    public static int CountCommonMatchesBetweenStrings( this string l, string r, List<string> k, StringComparison c )
    {
        if( ( l == null )||( r == null )||( k == null ) ) return 0;
        var m = (int)0;
        foreach( var t in k )
        {
            if( string.IsNullOrEmpty( t ) ) continue;
            if( l.IndexOf( t, c ) < 0 ) continue;
            if( r.IndexOf( t, c ) < 0 ) continue;
            m++;
        }
        return m;
    }
    
    #endregion
    
    #region Null-safe object.ToString()
    
    public static string ToStringNullSafe<T>( this T o ) where T : class
    {
        return o == null ? "[null]" : o.ToString();
    }
    
    #endregion
    
}
