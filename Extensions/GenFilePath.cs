/*
 * GenFilePath.cs
 * 
 * Generic functions for files and paths
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:17 PM
 * 
 */
using System;
using System.IO;
using System.Linq;


/// <summary>
/// Description of GenString.
/// </summary>
public static class GenFilePath
{
    
    #region File and path safe assignment (validates)
    
    public static string RightNow( string format = null )
    {
        return System.DateTime.Now.ToString( format ).ReplaceInvalidFilenameChars();
    }
    
    public static string ReplaceInvalidFilenameChars( this string filename, bool replaceSpaces = true, bool replaceDots = true, bool replaceColons = true, bool replaceSlashes = true )
    {
        var workingFile = filename;
        if( replaceSpaces       ) workingFile = workingFile.Replace( ' ', '_' );
        if( replaceDots         ) workingFile = workingFile.Replace( '.', '_' );
        if( replaceColons       ) workingFile = workingFile.Replace( ':', '_' );
        if( replaceSlashes      ) workingFile = workingFile.Replace( '/', '_' ).Replace( '\\', '_' );
        return workingFile;
    }
    
    public static string FixPathSlashes( this string path, bool trailingSlash )
    {
        if( string.IsNullOrEmpty( path ) ) return null;
        var working = path.Replace( '/', '\\' );
        if( ( trailingSlash )&&( !working.EndsWith( "\\", StringComparison.InvariantCulture ) ) )
           working += "\\";
        return working;
    }
    
    public static bool PathExists( this string path )
    {
        return Directory.Exists( FixPathSlashes( path, true ) );
    }
    
    public static bool FileExists( this string file )
    {
        return File.Exists( FixPathSlashes( file, false ) );
    }
    
    public static bool TryAssignPath( this string path, ref string target, bool forceCreate = false )
    {
        var working = FixPathSlashes( path, true );
        if( !Directory.Exists( working ) )
        {
            if( !forceCreate ) return false;
            if( !path.CreatePath() ) return false;
        }
        target = working;
        return true;
    }
    
    public static bool TryAssignFile( this string file, ref string target, bool forceCreatePath = false )
    {
        var working = FixPathSlashes( file, false );
        //DebugLog.Write( string.Format( "{0} -> {1}", newFile, working ) );
        if( forceCreatePath )
        {
            string path;
            FilenameFromPathname( file, out path );
            if( ( !string.IsNullOrEmpty( path ) )&&( !Directory.Exists( path ) ) )
                if( !CreatePath( path ) )
                    return false;
        }
        if( !File.Exists( working ) ) return false;
        target = working;
        return true;
    }
    
    public static bool CreatePath( this string path )
    {
        try{
            System.IO.Directory.CreateDirectory( path );
        } catch {
            return false;
        }
        return true;
    }
    
    public static void DeleteFile( this string pathname )
    {
        if( string.IsNullOrEmpty( pathname ) )return;
        if( !File.Exists( pathname ) )return;
        File.Delete( pathname );
    }
    
    public static string FilenameFromPathname( string pathname, string datapath = null, bool allowGhosts = false )
    {
        string path = null;
        return FilenameFromPathname( pathname, out path, datapath, allowGhosts );
    }
    
    public static string FilenameFromPathname( string pathname, out string path, string datapath = null, bool allowGhosts = false )
    {
        var working = pathname.FixPathSlashes( false );
        var wdata = datapath.FixPathSlashes( true );
        path = null;
        
        if( ( !allowGhosts )&&( working.EndsWith( ".ghost", StringComparison.InvariantCulture ) ) ) return string.Empty;
        
        if( ( !string.IsNullOrEmpty( datapath ) )&&( working.StartsWith( wdata, StringComparison.InvariantCulture ) ) )
        {
            var dataPathLength = wdata.Length;
            pathname = working.Substring( dataPathLength );
        }
        var lastSlash = working.LastIndexOf( '\\' );
        path = ( lastSlash < 0 )
            ? null
            : working.Substring( 0, lastSlash );
        //DebugLog.Write( string.Format( "GenFilePath.FilenameFromPathname :: {0} :: {1}", lastSlash, working.Length ) );
        return ( lastSlash < 0 )
            ? pathname
            : working.Substring( lastSlash + 1, working.Length - lastSlash - 1 );
        //return pathname;
    }
    
    #endregion
    
}
