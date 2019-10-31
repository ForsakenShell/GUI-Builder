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
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public static class DebugLog
{
    
    const string LogRootPath = "GUIBuilder.log";
    
    public const string FormatNewLineChar = "\n";
    public const string FormatTabChar = "\t";
    public const string FormatSeparator = " :: ";
    public const string OpenIndentChar = "{";
    public const string CloseIndentChar = "}";
    
    const string DateTimeFormat = "yyyy'_'MM'_'dd_HH'_'mm'_'ss";
    
    static string instanceTime = null;
    
    static string logPath = null;
    
    [ThreadStatic]
    static string _logFile = null;
    
    [ThreadStatic]
    static FileStream logStream = null;
    
    [ThreadStatic]
    static int logIndent = 0;
    
    [ThreadStatic]
    static bool logInitialized = false;
    
    [ThreadStatic]
    static bool logClosed = false;
    
    public static string ThreadID
    {
        get
        {
            return "0x" + Thread.CurrentThread.ManagedThreadId.ToString( "X8" );
        }
    }
    
    public static string ThreadName
    {
        get
        {
            var name = Thread.CurrentThread.Name;
            return string.IsNullOrEmpty( name )
                ? "Thread_" + ThreadID
                : name;
        }
    }
    
    public static string Filename
    {
        get
        {
            if( string.IsNullOrEmpty( _logFile ) )
            {
                string rightNow = GenFilePath.RightNow( DateTimeFormat ).ReplaceInvalidFilenameChars();
                if( string.IsNullOrEmpty( instanceTime ) )
                {
                    instanceTime = rightNow;
                    logPath = string.Format( "{0}\\{1}", LogRootPath, instanceTime );
                    LogRootPath.DeleteFile();   // Force removal of the old global log file
                    logPath.CreatePath();
                }
                var name = ThreadName;
                _logFile = string.Format( "{0}\\{1}_{2}.log", logPath, rightNow, name );
            }
            return _logFile;
        }
    }
    
    public static bool Initialized
    {
        get
        {
            return logInitialized;
        }
    }
    
    public static void Close()
    {
        if( ( logInitialized )&&( !logClosed ) )
        {
            _WriteLines( string.Format( "{2}{1}Log closed at {0}", DateTime.Now.ToString(), FormatNewLineChar, CloseIndentChar ) );
            logStream.Flush();
            logStream.Close();
            logClosed = true;
            logInitialized = false;
            logStream = null;
        }
    }
    
    public static string Open()
    {
        if( ( !logClosed )&&( !logInitialized ) )
        {
            logStream = File.Open( Filename, FileMode.Create, FileAccess.Write, FileShare.Read );
            logInitialized = true;
            _WriteLines( string.Format(
                "GUIBuilder {0} log for thread \"{1}\" with ID {2} opened at {3}{4}{5}",
                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                ThreadName,
                ThreadID,
                DateTime.Now.ToString(),
                FormatNewLineChar, OpenIndentChar
            ) );
            logStream.Flush();
        }
        return Filename;
    }
    
    public static void WriteLine( string message )
    {
        if( logClosed ) return;
        if( string.IsNullOrEmpty( message ) ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( message );
        logStream.Flush();
    }
    
    public static void WriteLine( string[] values )
    {
        if( logClosed ) return;
        if( values.NullOrEmpty() ) return;
        if( !logInitialized ) Open();
        
        var result = _FormatLine( values );
        _WriteLines( result );
        
        logStream.Flush();
    }
    
    public static void WriteError( string namespaceClassName, string functionName, string message )
    {
        if( logClosed ) return;
        if( string.IsNullOrEmpty( namespaceClassName) ) return;
        if( string.IsNullOrEmpty( functionName ) ) return;
        if( string.IsNullOrEmpty( message ) ) return;
        if( !logInitialized ) Open();
        
        _WriteAlertMessage( "ERROR", "======", namespaceClassName, functionName, message, true );
    }
    
    public static void WriteWarning( string namespaceClassName, string functionName, string message )
    {
        if( logClosed ) return;
        if( string.IsNullOrEmpty( message ) ) return;
        if( !logInitialized ) Open();
        
        _WriteAlertMessage( "Warning", "------", namespaceClassName, functionName, message, false );
    }
    
    public static void OpenIndentLevel( string[] values )
    {
        if( logClosed ) return;
        if( values.NullOrEmpty() ) return;
        if( !logInitialized ) Open();
        
        var result = _FormatLine( values );
        _WriteLines( result );
        _WriteLines( OpenIndentChar );
        
        logStream.Flush();
    }
    
    public static void OpenIndentLevel( string message )
    {
        if( logClosed ) return;
        if( string.IsNullOrEmpty( message ) ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( message );
        _WriteLines( OpenIndentChar );
        
        logStream.Flush();
    }
    
    public static void OpenIndentLevel()
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( OpenIndentChar );
        
        logStream.Flush();
    }
    
    public static void WriteList<TList>( string listName, IList<TList> list )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        if( list.NullOrEmpty() )
            _WriteLines( string.Format( "{0} = [null]", listName ) );
        else
        {
            var c = list.Count;
            _WriteLines( string.Format( "{0}: Contains {1} elements{2}{3}", listName, c, FormatNewLineChar, OpenIndentChar ) );
            for( int i = 0; i < c ; i++ )
                _WriteLines( "[ " + i + " ] = " + list[ i ].ToString() );
            _WriteLines( CloseIndentChar );
        }
        
        logStream.Flush();
    }
    
    public static void WriteArray<TArray>( string arrayName, TArray[] array )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        if( array.NullOrEmpty() )
            _WriteLines( string.Format( "{0} = [null]", arrayName ) );
        else
        {
            var c = array.Length;
            _WriteLines( string.Format( "{0}: Contains {1} elements{2}{3}", arrayName, c, FormatNewLineChar, OpenIndentChar ) );
            for( int i = 0; i < c ; i++ )
                _WriteLines( "[ " + i + " ] = " + array[ i ].ToString() );
            _WriteLines( CloseIndentChar );
        }
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel<TList>( string listName, List<TList> list )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        WriteList( listName, list );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel<TArray>( string arrayName, TArray[] array )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        WriteArray( arrayName, array );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel( string resultName, string result )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( string.Format( "{0} = {1}", resultName, result ) );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel<TResult>( string resultName, TResult result ) where TResult : class
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( string.Format( "{0} = {1}", resultName, ( result == null ? "[null]" : result.ToString() ) ) );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel( long elapsed, string resultName, string result )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        var tmp = new TimeSpan( elapsed );
        _WriteLines( string.Format( "Completed in {0}{3}{1} = {2}", tmp.ToString(), resultName, result, FormatNewLineChar ) );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel( long elapsed )
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        var tmp = new TimeSpan( elapsed );
        _WriteLines( string.Format( "Completed in {0}{1}", tmp.ToString(), FormatNewLineChar ) );
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    public static void CloseIndentLevel()
    {
        if( logClosed ) return;
        if( !logInitialized ) Open();
        
        _WriteLines( CloseIndentChar );
        
        logStream.Flush();
    }
    
    static string _FormatLine( string[] values )
    {
        if( values.NullOrEmpty() ) return null;
        
        var result = new StringBuilder();
        for( int i = 0; i < values.Length; i++ )
        {
            if( i > 0 )
                result.Append( FormatSeparator );
            result.Append( values[ i ] );
        }
        
        return result.ToString();
    }
    
    static void _WriteAlertMessage( string alertName, string alertCap, string namespaceClassName, string functionName, string message, bool stackTrace )
    {
        var stack = stackTrace
            ? Environment.StackTrace + FormatNewLineChar
            : null;
        _WriteLines(
            string.Format(
                "{0}[ {1} ]{0}{2}{4} :: {5}{2}{6}{2}{7}{0}",
                alertCap,
                alertName,
                FormatNewLineChar,
                FormatTabChar,
                namespaceClassName,
                functionName,
                message,
                stack ) );
        logStream.Flush();
    }
    
    static void _WriteLines( string messageLines )
    {
        if( !string.IsNullOrEmpty( messageLines ) )
        {
            string[] lines = messageLines.Split( new string [] { FormatNewLineChar }, StringSplitOptions.RemoveEmptyEntries );
            string[] array = lines;
            for( int j = 0; j < array.Length; j++ )
            {
                string line = array[j];
                if( !string.IsNullOrEmpty( line ) )
                {
                    if(
                        ( ( line.Length == 1 )&&( line.StartsWith( CloseIndentChar ) ) )||
                        ( ( line.Length >  1 )&&( line.EndsWith  ( CloseIndentChar ) ) )
                    ){
                        logIndent--;
                        if( logIndent < 0 )
                            logIndent = 0;
                    }
                    
                    string writeLine = "";
                    for( int i = 0; i < logIndent; i++ )
                        writeLine += FormatTabChar;
                    writeLine += line;
                    writeLine += FormatNewLineChar;
                    byte[] bytes = Encoding.ASCII.GetBytes( writeLine );
                    if( !bytes.NullOrEmpty() )
                    {
                        logStream.Write( bytes, 0, bytes.Length );
                        //Console.Write( writeLine );
                    }
                    
                    if(
                        ( ( line.Length == 1 )&&( line.StartsWith( OpenIndentChar ) ) )||
                        ( ( line.Length >  1 )&&( line.EndsWith  ( OpenIndentChar ) ) )
                    ){
                        logIndent++;
                    }
                }
            }
        }
    }
    
}
