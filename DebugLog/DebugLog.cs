/*
 * DebugLog.cs
 *
 * Per-Thread Debug logging
 *
 * User: 1000101
 * Date: 18/04/2019
 * Time: 11:15 AM
 * 
 */
//#define TRACE_INDENT_EVENTS // <-- Only here to find mismatched open/close
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public static class DebugLog
{

    #region Constants

    //const string LogRootPath = "GUIBuilder.log";
    
    public const string FormatNewLineChar = "\n";
    public const string FormatLineFeedChar = "\r";
    public const string FormatTabChar = "\t";
    public const char FormatTabAsChar = '\t';
    public const char FormatConsoleTabReplacement = ' ';
    public const string FormatSeparator = " :: ";
    public const string OpenIndentChar = "{";
    public const string CloseIndentChar = "}";
    
    const string DateTimeFormat = "yyyy'_'MM'_'dd_HH'_'mm'_'ss";

    #endregion

    #region Global Fields

    static string instanceTime = null;
    
    static string _logPath = null;

    #endregion

    #region Thread Local Storage Fields

    [ThreadStatic]
    static string _logFile = null;
    
    [ThreadStatic]
    static FileStream _logStream = null;
    
    [ThreadStatic]
    static int _logIndent = 0;
    
    [ThreadStatic]
    static bool _logInitialized = false;
    
    [ThreadStatic]
    static bool _logClosed = false;

    [ThreadStatic]
    static bool _logToConsole = false;

    #endregion

    #region Global Methods

    public static void ZipLogs( bool deleteFiles )
    {
        if( string.IsNullOrEmpty( _logPath ) ) return; // No logs created, nothing to zip
        //Console.WriteLine( _logPath );
        
        var zipFile = string.Format( "{0}.zip", _logPath );
        //Console.WriteLine( zipFile );
        
        var logFiles = Directory.GetFiles( _logPath, "*.log" );
        
        if( logFiles.Length == 0 ) return; // No logs creates, nothing to zip
        
        using( var zipToOpen = new FileStream( zipFile, FileMode.Create ) )
        {
            using( var archive = new ZipArchive( zipToOpen, ZipArchiveMode.Update ) )
            {
                for( int i = 0; i < logFiles.Length; i++ )
                {
                    var logZipFilename = GenFilePath.FilenameFromPathname( logFiles[ i ] );
                    //Console.WriteLine( string.Format( "{0} -> {1}", logFiles[ i ], logZipFilename ) );
                    archive.CreateEntryFromFile( logFiles[ i ], logZipFilename, CompressionLevel.Optimal );
                    if( deleteFiles )
                        File.Delete( logFiles[ i ] );
                }
            }
        }
        if( deleteFiles )
        {
            Directory.Delete( _logPath );
            _logPath = null;
        }
    }

    #endregion

    #region Thread Local Methods

    #region Log Management

    public static string Filename
    {
        get
        {
            if( string.IsNullOrEmpty( _logFile ) )
            {
                string rightNow = GenFilePath.RightNow( DateTimeFormat ).ReplaceInvalidFilenameChars();
                if( string.IsNullOrEmpty( instanceTime ) )
                {
                    var logRoot = GodObject.Paths.DebugLog;
                    if( string.IsNullOrEmpty( logRoot ) )
                        return null;
                    instanceTime = rightNow;
                    _logPath = string.Format( "{0}\\{1}", logRoot, instanceTime );
                    _logPath.CreatePath();
                }
                var fullThreadId = WorkerThreadPool.FriendlyThreadIdName();
                _logFile = string.Format( "{0}\\{1}_{2}.log", _logPath, fullThreadId, rightNow );
            }
            return _logFile;
        }
    }
    
    public static bool Initialized
    {
        get
        {
            return _logInitialized;
        }
    }
    
    public static void Close()
    {
        if( ( _logInitialized )&&( !_logClosed ) )
        {
            _WriteLine( string.Format( "{2}{1}Log closed at {0}", DateTime.Now.ToString(), FormatNewLineChar, CloseIndentChar ) );
            _logStream.Flush();
            _logStream.Close();
            _logClosed = true;
            _logInitialized = false;
            _logStream = null;
        }
    }
    
    public static bool Open( bool logToConsole = false )
    {
        if( ( !_logClosed )&&( !_logInitialized ) )
        {
            _logToConsole = logToConsole;
            var filename = Filename;
            if( string.IsNullOrEmpty( filename ) ) return false;
            _logStream = File.Open( Filename, FileMode.Create, FileAccess.Write, FileShare.Read );
            _logInitialized = true;
            _WriteLine( string.Format(
                "GUIBuilder {0} log for \"{1}\" opened at {2}{3}{4}{3}{5}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                WorkerThreadPool.FriendlyThreadIdName(),
                DateTime.Now.ToString(),
                FormatNewLineChar,
                WorkerThreadPool.StartMethodBaseName( false ),
                OpenIndentChar
            ) );
            _logStream.Flush();
        }
        return false;
    }

    #endregion

    #region Indent Levels

    #region Open

    public static void OpenIndentLevel( string[] values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = false, bool indentElements = true, bool prefixCallerId = true, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, true, false, true ) : null;

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            WriteStrings( null, values, includeNulls, singleLinePerItem, includeElementCount, includeIndicies, indentElements );
        }
        else
        {
            var vals = _BuildStringsLine( values, includeNulls );
            _WriteLine( _BuildStringsLine( new[] { callerId, vals }, false ) );
        }
        
        _WriteLine( OpenIndentChar );

        _logStream.Flush();
    }

    public static void OpenIndentLevel<TIXHandle>( List<TIXHandle> values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = false, bool indentElements = true, bool prefixCallerId = true, bool includeCallerParams = false, string[] reportParams = null ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, true, false, true ) : null;

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            WriteIDStrings( null, values, includeNulls, singleLinePerItem, includeElementCount, includeIndicies, indentElements );
        }
        else
        {
            var vals = _BuildIDStringsLine( values, includeNulls );
            _WriteLine( _BuildStringsLine( new[] { callerId, vals }, false ) );
        }

        _WriteLine( OpenIndentChar );

        _logStream.Flush();
    }

    public static void OpenIndentLevel( string message, bool prefixCallerId = true, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        string result = _BuildStringsLine( new [] { ( prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null ), message }, false );
        _WriteLine( result );
        _WriteLine( OpenIndentChar );

        _logStream.Flush();
    }

    public static void OpenIndentLevel( bool prefixCallerId = true, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        if( prefixCallerId ) _WriteLine( GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) );
        _WriteLine( OpenIndentChar );

        _logStream.Flush();
    }

    #endregion

    #region Close

    public static void CloseIndentIDStrings<TIXHandle>( string listName, List<TIXHandle> list, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        WriteIDStrings( listName, list, includeNulls, singleLinePerItem, includeElementCount, includeIndicies, indentElements );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentList<TList>( string listName, List<TList> list, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        WriteList( listName, list, includeNulls, singleLinePerItem, includeElementCount, includeIndicies, indentElements );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentArray<TArray>( string arrayName, TArray[] array, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        WriteArray( arrayName, array, includeNulls, singleLinePerItem, includeElementCount, includeIndicies, indentElements );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentLevel( string resultName, string result )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteLine( string.Format( "{0} = {1}", resultName, result ) );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentLevel<TResult>( string resultName, TResult result ) where TResult : class
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteLine( string.Format( "{0} = {1}", resultName, result.ToStringNullSafe() ) );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentLevel( long elapsed, string resultName, string result )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        if( elapsed >= 0 )
        {
            var tmp = new TimeSpan( elapsed );
            _WriteLine( string.Format( "Completed in {0}", tmp.ToString() ) );
        }

        _WriteLine( string.Format( "{0} = {1}", resultName, result ) );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentLevel( long elapsed )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var tmp = new TimeSpan( elapsed );
        _WriteLine( string.Format( "Completed in {0}", tmp.ToString() ) );
        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    public static void CloseIndentLevel()
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteLine( CloseIndentChar );

        _logStream.Flush();
    }

    #endregion

    #endregion

    #region Errors and Warnings

    static void _WriteAlertMessage( string alertName, string alertCap, string message, bool stackTrace )
    {
        _WriteLine(
            string.Format(
                "{0}[ {1} ]{0}",
                alertCap,
                alertName
                ) );
        
        _WriteLine( GenString.GetCallerId( 2, null, false, false, true ) );
        if( !string.IsNullOrEmpty( message ) ) _WriteLine( message );

        if( stackTrace )
        {
            var trace = new System.Diagnostics.StackTrace();
            _WriteLine( trace.ToString() );
            //var frames = trace.GetFrames();
            //_WriteStackTrace( 2, frames );
        }
        
        _WriteLine( alertCap );
        _logStream.Flush();
    }

    // This doesn't work, frame.GetFileLineNumber() always returns 0 and frame.GetFileName() returns null :(
    private static void _WriteStackTrace( int initialFrame, System.Diagnostics.StackFrame[] frames )
    {
        if( frames == null ) return;
        if( initialFrame >= frames.Length ) return;
        for( int i = initialFrame; i < frames.Length; i++ )
        {
            var frame = frames[ i ];
            var method = frame.GetMethod();
            var callerID = GenString.FormatMethod( method, null, false, false, true );
            _WriteLine( string.Format(
                "{0}Line: {1} : {2} : {3}",
                FormatTabChar,
                frame.GetFileLineNumber(),
                callerID,
                frame.GetFileName()
                ) );
        }
    }

    public static void WriteException( Exception e, string message = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteAlertMessage( "EXCEPTION", "======",
            string.Format(
                "{0}{1}{2}",
                message,
                ( message == null ? null : FormatNewLineChar ),
                e.ToString()
            ), true );
    }

    public static void WriteError( string message )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteAlertMessage( "ERROR", "======", message, true );
    }

    public static void WriteWarning( string message )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteAlertMessage( "Warning", "------", message, false );
    }

    #endregion

    #region Single Line Write

    public static void WriteCaller( bool includeParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        _WriteLine( GenString.GetCallerId( 1, reportParams, includeParams, false, true ) );
        _logStream.Flush();
    }

    public static void WriteLine( string message, bool prefixCallerId = false, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        string result = _BuildStringsLine( new [] { ( prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null ), message }, false );
        _WriteLine( result );
        _logStream.Flush();
    }

    #endregion

    #region Multi-Line Write

    public static void WriteStrings( string name, string[] values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true, bool prefixCallerId = false, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null;
        var nameData = _FormatNameElementCount( "Strings", name, ( values.NullOrEmpty() ? 0 : values.Length ), singleLinePerItem, includeElementCount );

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            _WriteLine( nameData );
            if( !values.NullOrEmpty() )
            {
                if( indentElements ) _WriteLine( OpenIndentChar );
                for( int i = 0; i < values.Length; i++ )
                {
                    var isNull = string.IsNullOrEmpty( values[ i ] );
                    if( ( includeNulls ) || ( !isNull ) )
                    {
                        if( includeIndicies )
                            _WriteLine( string.Format( "[ {0} ] = {1}", i, isNull ? "[null]" : values[ i ] ) );
                        else
                            _WriteLine( isNull ? "[null]" : values[ i ] );
                    }
                }
                if( indentElements ) _WriteLine( CloseIndentChar );
            }
        }
        else
        {
            var vals = _BuildStringsLine( values, includeNulls );
            var result = _BuildStringsLine( new [] { callerId, nameData, vals }, false );
            _WriteLine( result );
        }
        
        _logStream.Flush();
    }

    public static void WriteArray<TArray>( string name, TArray[] values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true, bool prefixCallerId = false, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null;
        var nameData = _FormatNameElementCount( "Array", name, ( values.NullOrEmpty() ? 0 : values.Length ), singleLinePerItem, includeElementCount );

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            _WriteLine( nameData );
            if( !values.NullOrEmpty() )
            {
                if( indentElements ) _WriteLine( OpenIndentChar );
                for( int i = 0; i < values.Length; i++ )
                {
                    var isNull = values[ i ] == null;
                    if( ( includeNulls ) || ( !isNull ) )
                    {
                        if( includeIndicies )
                            _WriteLine( string.Format( "[ {0} ] = {1}", i, isNull ? "[null]" : values[ i ].ToString() ) );
                        else
                            _WriteLine( isNull ? "[null]" : values[ i ].ToString() );
                    }
                }
                if( indentElements ) _WriteLine( CloseIndentChar );
            }
        }
        else
        {
            var vals = _BuildArrayLine( values, includeNulls );
            var result = _BuildStringsLine( new [] { callerId, nameData, vals }, false );
            _WriteLine( result );
        }

        _logStream.Flush();
    }

    public static void WriteList<TList>( string name, IList<TList> values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true, bool prefixCallerId = false, bool includeCallerParams = false, string[] reportParams = null )
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null;
        var nameData = _FormatNameElementCount( "List", name, ( values.NullOrEmpty() ? 0 : values.Count ), singleLinePerItem, includeElementCount ); 

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            _WriteLine( nameData );
            if( !values.NullOrEmpty() )
            {
                if( indentElements ) _WriteLine( OpenIndentChar );
                for( int i = 0; i < values.Count; i++ )
                {
                    var isNull = values[ i ] == null;
                    if( ( includeNulls ) || ( !isNull ) )
                    {
                        if( includeIndicies )
                            _WriteLine( string.Format( "[ {0} ] = {1}", i, isNull ? "[null]" : values[ i ].ToString() ) );
                        else
                            _WriteLine( isNull ? "[null]" : values[ i ].ToString() );
                    }
                }
                if( indentElements ) _WriteLine( CloseIndentChar );
            }
        }
        else
        {
            var vals = _BuildListLine( values, includeNulls );
            var result = _BuildStringsLine( new [] { callerId, nameData, vals }, false );
            _WriteLine( result );
        }

        _logStream.Flush();
    }

    public static void WriteIDStrings<TIXHandle>( string name, IList<TIXHandle> values, bool includeNulls = false, bool singleLinePerItem = false, bool includeElementCount = true, bool includeIndicies = true, bool indentElements = true, bool prefixCallerId = false, bool includeCallerParams = false, string[] reportParams = null ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        if( _logClosed ) return;
        if( ( !_logInitialized )&&( !Open() ) ) return;

        var callerId = prefixCallerId ? GenString.GetCallerId( 1, reportParams, includeCallerParams, false, true ) : null;
        var nameData = _FormatNameElementCount( "IDStrings", name, ( values.NullOrEmpty() ? 0 : values.Count ), singleLinePerItem, includeElementCount );

        if( singleLinePerItem )
        {
            _WriteLine( callerId );
            _WriteLine( nameData );
            if( !values.NullOrEmpty() )
            {
                if( indentElements ) _WriteLine( OpenIndentChar );
                for( int i = 0; i < values.Count; i++ )
                {
                    var isNull = values[ i ] == null;
                    if( ( includeNulls ) || ( !isNull ) )
                    {
                        if( includeIndicies )
                            _WriteLine( string.Format( "[ {0} ] = {1}", i, isNull ? "[null]" : values[ i ].IDString ) );
                        else
                            _WriteLine( isNull ? "[null]" : values[ i ].IDString );
                    }
                }
                if( indentElements ) _WriteLine( CloseIndentChar );
            }
        }
        else
        {
            var vals = _BuildIDStringsLine( values, includeNulls );
            var result = _BuildStringsLine( new [] { callerId, nameData, vals }, false );
            _WriteLine( result );
        }

        _logStream.Flush();
    }

    #endregion

    #region Internal Multi->Single Line Builders

    static string _FormatNameElementCount( string type, string name, int count, bool singleLinePerItem, bool includeElementCount )
    {
        var nameIsNull = string.IsNullOrEmpty( name );
        return ( ( nameIsNull && !singleLinePerItem ) || ( !includeElementCount ) )
            ? null
            : string.Format(
                "{0} contains {1} elements",
                (
                    nameIsNull
                    ? type
                    : name
                ),
                count );
    }

    static string _BuildStringsLine( string[] values, bool includeNulls )
    {
        if( values.NullOrEmpty() ) return null;

        var addedTo = false;
        var result = new StringBuilder();
        for( int i = 0; i < values.Length; i++ )
        {
            var isNull = string.IsNullOrEmpty( values[ i ] );
            if( ( includeNulls )||( !isNull ) )
            {
                if( addedTo )
                    result.Append( FormatSeparator );
                if( isNull )
                    result.Append( "[null]" );
                else
                    result.Append( values[ i ] );
                addedTo = true;
            }
        }
        
        return result.ToString();
    }

    static string _BuildArrayLine<TArray>( TArray[] values, bool includeNulls )
    {
        if( values.NullOrEmpty() ) return null;

        var addedTo = false;
        var result = new StringBuilder();
        for( int i = 0; i < values.Length; i++ )
        {
            var isNull = values[ i ] == null;
            if( ( includeNulls ) || ( !isNull ) )
            {
                if( addedTo )
                    result.Append( FormatSeparator );
                if( isNull )
                    result.Append( "[null]" );
                else
                    result.Append( values[ i ].ToString() );
                addedTo = true;
            }
        }

        return result.ToString();
    }

    static string _BuildListLine<TList>( IList<TList> values, bool includeNulls )
    {
        if( values.NullOrEmpty() ) return null;

        var addedTo = false;
        var result = new StringBuilder();
        for( int i = 0; i < values.Count; i++ )
        {
            var isNull = values[ i ] == null;
            if( ( includeNulls ) || ( !isNull ) )
            {
                if( addedTo )
                    result.Append( FormatSeparator );
                if( isNull )
                    result.Append( "[null]" );
                else
                    result.Append( values[ i ].ToString() );
                addedTo = true;
            }
        }

        return result.ToString();
    }

    static string _BuildIDStringsLine<TIXHandle>( IList<TIXHandle> values, bool includeNulls ) where TIXHandle : Engine.Plugin.Interface.IXHandle
    {
        if( values.NullOrEmpty() ) return null;

        var addedTo = false;
        var result = new StringBuilder();
        for( int i = 0; i < values.Count; i++ )
        {
            var isNull = values[ i ] == null;
            if( ( includeNulls ) || ( !isNull ) )
            {
                if( addedTo )
                    result.Append( FormatSeparator );
                if( isNull )
                    result.Append( "[null]" );
                else
                    result.Append( values[ i ].IDString );
                addedTo = true;
            }
        }

        return result.ToString();
    }

    #endregion

    #region Internal Writers

    static void _WriteLine( string line )
    {
        if( string.IsNullOrEmpty( line ) ) return;
        
        if( ( line.Contains( FormatNewLineChar ) )||( line.Contains( FormatLineFeedChar ) ) )
        {
            _WriteConcatenatedLines( line );
            return;
        }

        if(
            ( ( line.Length == 1 ) && ( line.StartsWith( CloseIndentChar ) ) ) ||
            ( ( line.Length > 1 ) && ( line.EndsWith( CloseIndentChar ) ) )
        )
        {
            #if TRACE_INDENT_EVENTS
            _WriteLines( Environment.StackTrace ); // <-- Only here to find mismatched open/close
            #endif
            _logIndent--;
            if( _logIndent < 0 )
                _logIndent = 0;
        }

        string writeLine = "";
        for( int i = 0; i < _logIndent; i++ )
            writeLine += FormatTabChar;
        
        writeLine += line;

        if( _logToConsole )
        {
            var consoleLine = writeLine.Replace( FormatTabAsChar, FormatConsoleTabReplacement );
            Console.WriteLine( consoleLine );
        }

        writeLine += FormatNewLineChar;
        byte[] bytes = Encoding.ASCII.GetBytes( writeLine );
        if( !bytes.NullOrEmpty() )
            _logStream.Write( bytes, 0, bytes.Length );

        if(
            ( ( line.Length == 1 ) && ( line.StartsWith( OpenIndentChar ) ) ) ||
            ( ( line.Length > 1 ) && ( line.EndsWith( OpenIndentChar ) ) )
        )
        {
            #if TRACE_INDENT_EVENTS
            _WriteLines( Environment.StackTrace ); // <-- Only here to find mismatched open/close
            #endif
            _logIndent++;
        }
    }

    static void _WriteConcatenatedLines( string messageLines )
    {
        if( string.IsNullOrEmpty( messageLines ) ) return;

        string[] lines = messageLines.Split( new string [] { FormatNewLineChar, FormatLineFeedChar }, StringSplitOptions.RemoveEmptyEntries );
        _WriteLines( lines );
    }

    static void _WriteLines( string[] lines )
    {
        if( lines.NullOrEmpty() ) return;

        foreach( var line in lines )
            _WriteLine( line );
    }

    #endregion

    #endregion

}
