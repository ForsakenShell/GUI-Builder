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
            return s.Substring( wi, sl - wi );
        
        if( wi + wl == sl ) // End of it
            return s.Substring( 0, wi );
        
        // Middle of it
        return s.Substring( 0, wi ) + s.Substring( wi + wl, sl - wi - wl );
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
    
    /// <summary>
    /// Count the number of keys that are contained in a string.
    /// </summary>
    /// <param name="l">string to check</param>
    /// <param name="k">keys to check</param>
    /// <param name="c">comparison type</param>
    /// <returns></returns>
    public static int CountKeysInString( this string l, List<string> k, StringComparison c )
    {
        if( k.NullOrEmpty() ) return 0;
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
    /// Count the number of keys that are contained in a string.
    /// </summary>
    /// <param name="l">string to check</param>
    /// <param name="k">keys to check</param>
    /// <param name="c">comparison type</param>
    /// <returns></returns>
    public static int CountKeysInString( this string l, string[] k, StringComparison c )
    {
        if( k.NullOrEmpty() ) return 0;
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
