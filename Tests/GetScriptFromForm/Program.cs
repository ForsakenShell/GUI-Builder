/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
using System;
using System.Collections.Generic;

using TestCommon;
using XeLib;
using XeLib.API;

namespace GetScriptFromForm
{
    class Program
    {
        const string testTitle = "Test - Get script from form";
        
        const string testPlugin = "AnnexTheCommonwealth.esm";
        
        // Test ESM_ATC_ACTI_SubDivision
        //const UInt32 testFormID = 0x00037B66;
        //const string testScriptName = "ESM:ATC:SubDivision";
        
        // Test ESMSanctuarySubDivision
        const UInt32 testFormID = 0x0003BFB9;
        const string testScriptName = "ESM:ATC:SubDivision";
        
        const string testScriptProperty = "myLocation";
        
        static string GamePath;
        static bool dumpErrorOnExit = false;
        
        public static void Main(string[] args)
        {
            #region Initialize XeLib and load plugins
            
            Console.WriteLine( "\n" + testTitle + "...start" );
            
            int pCount = 0;
            FileHandle testFileHandle = null;
            FormHandle testFormHandle = null;
            ScriptHandle testScriptHandle = null;
            ScriptPropertyHandle testScriptPropertyHandle = null;
            
            dumpErrorOnExit = !TestCommon.XeLib.Initialize( out GamePath );
            if( dumpErrorOnExit ) goto LocalAbort;
            
            var plugins = new string[]{ TestCommon.XeLib.ESM_Fallout4, testPlugin };
            pCount = plugins.Length;
            
            dumpErrorOnExit = !TestCommon.XeLib.Load( plugins );
            if( dumpErrorOnExit ) goto LocalAbort;
            
            #endregion
            
            #region Get form handles
            
            Console.WriteLine( "\nGet file handle" );
            testFileHandle = Files.FileByName( testPlugin );
            dumpErrorOnExit = !testFileHandle.IsValid();
            if( dumpErrorOnExit )
            {
                Console.WriteLine( "\tFailed to get handle for " + testPlugin );
                goto LocalAbort;
            }
            
            Console.WriteLine( "\tSignature: \"" + testFileHandle.Signature + "\"" );
            
            Console.WriteLine( "\nGet form handle" );
            var testPluginLOMask = testFileHandle.LoadOrder << 24;
            var loadFormID = testPluginLOMask | testFormID;
            testFormHandle = testFileHandle.GetMasterRecord( loadFormID  );
            dumpErrorOnExit  = !testFormHandle.IsValid();
            if( dumpErrorOnExit )
            {
                Console.WriteLine( "\tFailed to get handle for 0x" + loadFormID.ToString( "X8" ) );
                goto LocalAbort;
            }
            
            Console.WriteLine( "\nGet script handle" );
            testScriptHandle = testFormHandle.GetScript( testScriptName );
            dumpErrorOnExit  = !testScriptHandle.IsValid();
            if( dumpErrorOnExit )
            {
                Console.WriteLine( "\tFailed to get handle for " + testScriptName );
                goto LocalAbort;
            }
            
            Console.WriteLine( "\nGet script property handle" );
            testScriptPropertyHandle = testScriptHandle.GetProperty( testScriptProperty );
            dumpErrorOnExit  = !testScriptPropertyHandle.IsValid();
            if( dumpErrorOnExit )
            {
                Console.WriteLine( "\tFailed to get handle for " + testScriptProperty );
                goto LocalAbort;
            }
            
            #endregion
            
            
            Console.WriteLine( "\nFile:           " + testFileHandle.ToString() );
            Console.WriteLine(   "Form:           " + testFormHandle.ToString() );
            Console.WriteLine(   "Script:         " + testScriptHandle.ToString() );
            Console.WriteLine(   "ScriptProperty: " + testScriptPropertyHandle.ToString() );
            Console.WriteLine(   "ScriptProperty.Value = 0x" + testScriptPropertyHandle.GetUIntValue().ToString( "X8" ) );
            
            testScriptPropertyHandle.DebugDumpChildElements( true );
            
            Console.WriteLine( "\nTest complete" );
            
        LocalAbort:
            if( dumpErrorOnExit )
                TestCommon.XeLib.WriteMessages();
            TestCommon.XeLib.TryDispose( ref testScriptPropertyHandle );
            TestCommon.XeLib.TryDispose( ref testScriptHandle );
            TestCommon.XeLib.TryDispose( ref testFormHandle );
            TestCommon.XeLib.TryDispose( ref testFileHandle );
            TestCommon.XeLib.Denit();
            Console.WriteLine( "\n" + testTitle + "...stop" );
            
        }
        
    }
}