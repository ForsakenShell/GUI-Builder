/*
 * CopyRefrToNewCell
 *
 * Test program to move a refr from one cell to another
 *
 * User: 1000101
 * Date: 02/03/2019
 * Time: 11:18 AM
 * 
 */
using System;
using System.Collections.Generic;

using TestCommon;
using XeLib;
using XeLib.API;

namespace CopyRefrToNewCell
{
    class Program
    {
        
        const string testTitle = "Test - Copy Refr To New Cell";
        
        //const string testPlugin = "TestCopyRefrToNewCell.esp";
        const string testPlugin = "AnnexTheCommonwealth.esp";
        const string savePlugin = "TestCopyRefrToNewCell_PostCopy.esp";
        
        const UInt32 sourceRefrFormID = 0x00110C39;
        const UInt32 sourceCellFormID = 0x00018AA2;
        const UInt32 destCellFormID = 0x0000E46D;
        
        static string GamePath;
        static bool dumpErrorOnExit = false;
        
        public static void Main(string[] args)
        {
            #region Initialize XeLib and load plugins
            
            Console.WriteLine( "\n" + testTitle + "...start" );
            
            int pCount = 0;
            Handle[] pHandles = null;
            Handle sourceRefrHandle = null;
            Handle sourceCellHandle = null;
            Handle destCellHandle = null;
            Handle newRefrHandle = null;
            
            
            dumpErrorOnExit = !TestCommon.XeLib.Initialize( out GamePath );
            if( dumpErrorOnExit ) goto LocalAbort;
            
            var plugins = new string[]{ TestCommon.XeLib.ESM_Fallout4, TestCommon.XeLib.ESM_AnnexTheCommonwealth, testPlugin };
            pCount = plugins.Length;
            
            dumpErrorOnExit = !TestCommon.XeLib.Load( plugins );
            if( dumpErrorOnExit ) goto LocalAbort;
            
            //dumpErrorOnExit = !TestCommon.XeLib.BuildReferences();
            //if( dumpErrorOnExit ) goto LocalAbort;
            
            #endregion
            
            #region Get form handles
            
            Console.WriteLine( "\nGet file handles" );
            pHandles = new XeLib.Handle[ pCount ];
            for( int i = 0; i < pCount; i++ )
            {
                var p = plugins[ i ];
                pHandles[ i ] = Files.FileByName( p );
                dumpErrorOnExit = !pHandles[ i ].IsValid();
                if( dumpErrorOnExit ) goto LocalAbort;
            }
            
            var testPluginLOMask = ( (uint)Files.GetFileLoadOrder( pHandles[ 1 ] ) ) << 24;
            
            Console.WriteLine( "\nGet form handles" );
            
            sourceRefrHandle = Records.GetRecord( pHandles[ 2 ], testPluginLOMask | sourceRefrFormID );
            dumpErrorOnExit  = !sourceRefrHandle.IsValid();
            if( dumpErrorOnExit ) goto LocalAbort;
            
            sourceCellHandle = Records.GetRecord( pHandles[ 2 ], sourceCellFormID );
            dumpErrorOnExit  = !sourceCellHandle.IsValid();
            if( dumpErrorOnExit ) goto LocalAbort;
            
            destCellHandle   = Records.GetRecord( pHandles[ 2 ], destCellFormID );
            dumpErrorOnExit  = !destCellHandle.IsValid();
            if( dumpErrorOnExit ) goto LocalAbort;
            
            #endregion
            
            
            
            Console.WriteLine(
                string.Format(
                    "\nTest copy refr 0x{0} from cell 0x{1} to cell 0x{2}\n",
                    sourceRefrHandle.FormID.ToString( "X8" ),
                    sourceCellHandle.FormID.ToString( "X8" ),
                    destCellHandle.FormID.ToString( "X8" )
                ) );
            
            //newRefrHandle    = Elements.CopyElement( sourceRefrHandle, destCellHandle, true );
            newRefrHandle    = TestCommon.XeLib.CopyMoveToCell( sourceRefrHandle, destCellHandle, true );
            dumpErrorOnExit  = !newRefrHandle.IsValid();
            if( dumpErrorOnExit ) goto LocalAbort;
            
            Console.WriteLine( "\nNew refr FormID: 0x" + Records.GetFormId( newRefrHandle ).ToString( "X8" ) );
            
            
            
            #region Save and exit
            
            var saveFilePath = GamePath + "Data\\" + savePlugin;
            Console.WriteLine( "\nSave as " + saveFilePath );
            dumpErrorOnExit = XeLib.API.Files.SaveFile( pHandles[ 1 ], saveFilePath );
            if( dumpErrorOnExit ) goto LocalAbort;
            
            Console.WriteLine( "\nTest complete" );
            
            Handle testNull = null;
            testNull.Dispose();
            
        LocalAbort:
            if( dumpErrorOnExit )
                TestCommon.XeLib.WriteMessages();
            TestCommon.XeLib.TryDispose( ref newRefrHandle, sourceRefrHandle );
            TestCommon.XeLib.TryDispose( ref destCellHandle );
            TestCommon.XeLib.TryDispose( ref sourceCellHandle );
            TestCommon.XeLib.TryDispose( ref sourceRefrHandle );
            for( int i = 0; i < pCount; i++ )
                TestCommon.XeLib.TryDispose( ref pHandles[ i ] );
            TestCommon.XeLib.Denit();
            Console.WriteLine( "\n" + testTitle + "...stop" );
            
            #endregion
        }
        
    }
}