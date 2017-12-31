/*
 * DebugLog.cs
 *
 * Creates and writes to a log file for debugging purposes.
 *
 * User: 1000101
 * Date: 03/12/2017
 * Time: 1:34 PM
 * 
 */
using System;
using System.IO;

namespace Border_Builder
{
    /// <summary>
    /// Description of DebugLog.
    /// </summary>
    public static class DebugLog
    {
        static string       logFile = "Border_Builder.log";
        static FileStream   logStream = null;
        static int          logIndent = 0;
        
        public static bool Opened
        {
            get{ return logStream != null; }
        }
        
        public static void Open()
        {
            // Already open?
            if( logStream != null )
                return;
            
            logStream = File.Open( logFile, FileMode.Create );
            
            DebugLog.Write( string.Format( "Border Builder {0} log opened at {1}\n", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), System.DateTime.Now.ToString() ) );
            
        }
        
        public static void Close()
        {
            // Already closed?
            if( logStream == null )
                return;
            
            DebugLog.Write( string.Format( "\nLog closed at {0}", System.DateTime.Now.ToString() ) );
            
            logStream.Close();
            logStream = null;
        }
        
        public static void Write( string message )
        {
            if( !DebugLog.Opened )
                DebugLog.Open();
            var lines = message.Split( '\n' );
            foreach( var line in lines )
            {
                if( ( line.StartsWith( "}" ) )||( line.EndsWith( "}" ) ) )
                {
                    logIndent--;
                    if( logIndent < 0 )
                        logIndent = 0;
                }
                
                var writeLine = "";
                for( int i = 0; i < logIndent; i++ )
                    writeLine = writeLine + "\t";
                writeLine = writeLine + line + "\n";
                var bytes = System.Text.Encoding.ASCII.GetBytes( writeLine );
                logStream.Write( bytes, 0, bytes.Length );
                
                if( ( line.StartsWith( "{" ) )||( line.EndsWith( "{" ) ) )
                    logIndent++;
            }
        }
        
    }
}
