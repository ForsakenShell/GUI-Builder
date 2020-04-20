/*
 * GenString.cs
 * 
 * Generic functions for strings
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;


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

    public static bool SensitiveInvariantMatch( this string a, string b )
    {
        return string.Equals( a, b, StringComparison.InvariantCulture );
    }

    public static bool InsensitiveInvariantMatch( this string a, string[] b )
    {
        if( ( b != null ) && ( b.Length > 0 ) )
            for( int i = 0; i < b.Length; i++ )
                if( string.Equals( a, b[ i ], StringComparison.InvariantCultureIgnoreCase ) ) return true;
        return false;
    }

    public static bool InsensitiveInvariantMatch( this string a, List<string> b )
    {
        if( !b.NullOrEmpty() )
            for( int i = 0; i < b.Count; i++ )
                if( string.Equals( a, b[ i ], StringComparison.InvariantCultureIgnoreCase ) ) return true;
        return false;
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
            return StripFrom( s.Substring( wl, sl - wl ), word, comparison );
        
        if( wi + wl == sl ) // End of it
            return StripFrom( s.Substring( 0, wi ), word, comparison );
        
        // Middle of it
        return StripFrom( s.Substring( 0, wi ) + s.Substring( wi + wl, sl - wi - wl ), word, comparison );
    }
    
    #endregion
    
    #region string Arrays and Lists
    
    public static uint LongestStringLength( this string[] array )
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
    
    public static uint LongestStringLength( this List<string> list )
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
    

    public static int CountKeyInString( this string left, string key, StringComparison comparison )
    {
        if( string.IsNullOrEmpty( key ) ) return 0;
        var index = left.IndexOf( key, comparison );
        int count = 0;
        while( index >= 0 )
        {
            count++;
            index = left.IndexOf( key, index + 1, comparison );
        }
        return count;
    }

    /// <summary>
    /// Count the number of keys that are contained in a string.
    /// </summary>
    /// <param name="left">string to check</param>
    /// <param name="keys">keys to check</param>
    /// <param name="comparison">comparison type</param>
    /// <returns></returns>
    public static int CountKeysInString( this string left, List<string> keys, StringComparison comparison )
    {
        if( keys.NullOrEmpty() ) return 0;
        var count = (int)0;
        foreach( var key in keys )
        {
            if( string.IsNullOrEmpty( key ) ) continue;
            count += left.CountKeyInString( key, comparison );
        }
        return count;
    }

    /// <summary>
    /// Count the number of times a set of keys is contained in a string.
    /// </summary>
    /// <param name="left">string to check</param>
    /// <param name="keys">keys to check</param>
    /// <param name="comparison">comparison type</param>
    /// <returns></returns>
    public static int CountKeysInString( this string left, string[] keys, StringComparison comparison )
    {
        if( keys.NullOrEmpty() ) return 0;
        var count = (int)0;
        foreach( var key in keys )
        {
            if( string.IsNullOrEmpty( key ) ) continue;
            count += left.CountKeyInString( key, comparison );
        }
        return count;
    }

    /// <summary>
    /// Count the number of times set of keys is contained in both strings.
    /// </summary>
    /// <param name="left">first string to check</param>
    /// <param name="right">second string to check</param>
    /// <param name="keys">keys to check</param>
    /// <param name="comparison">comparison type</param>
    /// <param name="minOrMax">Count using Min (true) or Max (false)</param>
    /// <returns></returns>
    public static int CountCommonMatchesBetweenStrings( this string left, string right, List<string> keys, StringComparison comparison, bool minOrMax = false )
    {
        if( ( string.IsNullOrEmpty( left ) )||( string.IsNullOrEmpty( right ) )||( keys.NullOrEmpty() ) ) return 0;
        //DebugLog.OpenIndentLevel();
        var count = (int)0;
        foreach( var key in keys )
        {
            if( string.IsNullOrEmpty( key ) ) continue;
            var lCount = left .CountKeyInString( key, comparison );
            var rCount = right.CountKeyInString( key, comparison );
            //DebugLog.WriteStrings( null, new [] {
            //    "key: \"" + key + "\"",
            //    "\t" + lCount.ToString() + " times in \"" + left + "\"",
            //    "\t" + rCount.ToString() + " times in \"" + right + "\"",
            //}, false, true, false, false, false  );
            //count += minOrMax ? Math.Min( lCount, rCount ) : Math.Max( lCount, rCount );
            count += lCount;
            count += rCount;
        }
        //DebugLog.CloseIndentLevel( "count", count.ToString() );
        return count;
    }
    
    #endregion
    
    #region Null-safe object.ToString()
    
    public static string ToStringNullSafe<T>( this T o ) where T : class
    {
        return o == null ? "[null]" : o.ToString();
    }
    
    public static string ToStringNullSafe<T>( this List<T> l ) where T : class
    {
        if( l.NullOrEmpty() )
            return "[null]";
        var r = "[ ";
        for( int i = 0; i < l.Count; i++ )
        {
            r += l[ i ].ToStringNullSafe();
            r += ( i < l.Count - 1 ) ? ", " : " ]";
        }
        return r;
    }

    public static string NullSafeIDString<TIXHandle>( this TIXHandle i ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        return i == null ? "[null]" : i.IDString;
    }

    public static string NullSafeIDStrings<TIXHandle>( this List<TIXHandle> l ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        if( l.NullOrEmpty() )
            return "[null]";
        var r = "[ ";
        for( int i = 0; i < l.Count; i++ )
        {
            r += l[ i ].NullSafeIDString();
            r += ( i < l.Count - 1 ) ? ", " : " ]";
        }
        return r;
    }

    #endregion


    /// <summary>
    /// Replace all instances of tokenName with tokenValue in source.
    /// </summary>
    /// <param name="source">Tokenized string</param>
    /// <param name="tokenName">Token to replace in source</param>
    /// <param name="tokenValue">Token replacement value for source</param>
    /// <param name="comparisonType">String comparison type</param>
    public static void ReplaceToken( ref string source, string tokenName, string tokenValue = null, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase )
    {
        if( string.IsNullOrEmpty( tokenName ) ) return;

        while( true )
        {
            if( string.IsNullOrEmpty( source ) ) break;

            var i = source.IndexOf( tokenName, comparisonType );
            if( i < 0 ) break;

            var l = tokenName.Length;
            var pre  = i == 0 ? null : source.Substring( 0, i );
            var post = i + l == source.Length ? null : source.Substring( i + l );

            var result = string.Format(
                "{0}{1}{2}",
                pre,
                tokenValue,
                post
            );

            source = result;
        }
    }

    #region Pretty Type [Full]Name and, Null-safe object and List<T> Type [Full]Name

    public static string UnmangleGenerics( this string name )
    {
        // nomspace+foo`1[narf+bar] -> nomspace.foo`1[narf.bar]
        //Console.WriteLine( ">> UnmangleGenerics() :: name   = \"" + name + "\"" );

        var result = name.Replace( '+', '.' );

        if( !string.IsNullOrEmpty( result ) )
        {
            var genPrefixOffset = result.IndexOf( '`' );
            while( genPrefixOffset > 0 )
            {
                var genOpen = result.IndexOf( '[' );
                var genClose = result.IndexOf( ']' );

                var pre = result.Substring( 0, genPrefixOffset );
                var post = genClose == result.Length - 1 ? null : result.Substring( genClose + 1 );
                var param = result.Substring( genOpen + 1, genClose - genOpen - 1 );

                //           111111111122222222223
                // 0123456789012345678901234567890
                // nomspace.foo`1[narf.bar]
                //
                // genPrefixOffset = 12
                // genOpen = 14
                // genClose = 23
                // pre = "nomspace.foo"
                // post = null
                // param = "narf.bar"
                // result = nomspace.foo<narf.bar>

                // Build unmanged string
                result = string.Format( "{0}<{1}>{2}", pre, param, post );

                // Check for any more generic demangling
                genPrefixOffset = result.IndexOf( '`' );
            }
        }

        //Console.WriteLine( "<< UnmangleGenerics() :: result = \"" + result + "\"" );
        return result;
    }

    public static string Name<T>( this T t ) where T : Type
    {
        if( t == null ) return "[null]";
        var m = t.ToString().UnmangleGenerics();
        var i = m.LastIndexOf( '<' );
        if( i > 0 ) // Generic type, get the dot before the generic type
            i = m.LastIndexOf( '.', i );
        else
            i = m.LastIndexOf( '.' );
        return i > -1 ? m.Substring( i + 1 ) : m;
    }

    public static string FullName<T>( this T t ) where T : Type
    {
        if( t == null ) return "[null]";
        return t.ToString().UnmangleGenerics();
    }

    static string ListTypeNames<T>( this List<T> l, Func<Type,string> nameFunc ) where T : Type
    {
        if( l.NullOrEmpty() )
            return "[null]";
        var r = "[ ";
        for( int i = 0; i < l.Count; i++ )
        {
            r += nameFunc( l[ i ] );
            r += ( i < l.Count - 1 ) ? ", " : " ]";
        }
        return r;
    }

    public static string TypeName<T>( this List<T> l ) where T : Type
    {
        return ListTypeNames( l, Name );
    }

    public static string TypeFullName<T>( this List<T> l ) where T : Type
    {
        return ListTypeNames( l, FullName );
    }

    public static string TypeName( this object o )
    {
        return o.GetType().Name();
    }

    public static string TypeFullName( this object o )
    {
        return o.GetType().FullName();
    }

    #endregion

    #region Pretty Caller and Method Name

    public const string THREAD_PREFIX = "THREAD_";

    public static string GetCallerId( int frameOffset, string[] reportParams, bool includeParams, bool trimThreadPrefix, bool fullType )
    {
        var trace = new System.Diagnostics.StackTrace();
        var frame = trace.GetFrame( 1 + frameOffset );
        var method = frame.GetMethod();
        return FormatMethod( method, reportParams, includeParams, trimThreadPrefix, fullType );
    }

    public static string FormatMethod( System.Reflection.MethodBase method, string[] reportParams, bool includeParams, bool trimThreadPrefix, bool fullType )
    {
        if( method == null ) return null;
        var nType = !fullType ? null : method?.ReflectedType.FullName();

        // Get unmangled method name
        var nMethod = method.Name.UnmangleGenerics();

        //trim off the "THREAD_" prefix
        if( ( trimThreadPrefix ) && ( nMethod.StartsWith( THREAD_PREFIX, StringComparison.InvariantCultureIgnoreCase ) ) )
            nMethod = nMethod.Substring( THREAD_PREFIX.Length );

        var result = string.IsNullOrEmpty( nType ) ? null : nType + ".";
        result += nMethod;
        if(
            ( method.MemberType != System.Reflection.MemberTypes.Property )&&
            ( method.MemberType != System.Reflection.MemberTypes.Field ) )
        {
            result += "(";

            if( !reportParams.NullOrEmpty() )
            {
                var pList = "";
                foreach( var param in reportParams )
                {
                    if( !string.IsNullOrEmpty( pList ) )
                        pList += ", ";
                    else
                        pList += " ";
                    pList += param;
                }
                pList += " ";
                result += pList;
            }
            else if( includeParams )
            {
                var mParams = method.GetParameters();
                if( !mParams.NullOrEmpty() )
                {
                    Array.Sort( mParams, ( x, y ) => ( x.Position < y.Position ? -1 : 1 ) );
                    var pList = "";
                    foreach( var param in mParams )
                    {
                        if( !string.IsNullOrEmpty( pList ) )
                            pList += ", ";
                        else
                            pList += " ";
                        pList += param.ParameterType.FullName() + " " + param.Name.UnmangleGenerics();
                    }
                    pList += " ";
                    result += pList;
                }
            }

            result += ")";
        }
        return result;
    }

    #endregion

}
