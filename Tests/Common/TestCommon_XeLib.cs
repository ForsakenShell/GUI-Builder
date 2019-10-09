/*
 * TestCommon_XeLib.cs
 *
 * Minimum common functions for tests with XeLib
 *
 * User: 1000101
 * Date: 02/03/2019
 * Time: 11:18 AM
 * 
 */
using System;
using System.Collections.Generic;

using XeLib;
using XeLib.API;
using XeLib.Internal;

namespace TestCommon
{
    public static class XeLib
    {
        
        public static string ESM_Fallout4 = "Fallout4.esm";
        public static string ESM_AnnexTheCommonwealth = "AnnexTheCommonwealth.esm";
        public static string ESM_SimSettlements = "SimSettlements.esm";
        
        static bool isLoaded = false;
        
        #region XeLib Messages
        
        public static void ClearMessages()
        {
            string estr = Messages.GetMessages();
        }
        
        public static void WriteMessages()
        {
            var estr = Messages.GetMessages();
            if( string.IsNullOrEmpty( estr ) ) return;
            Console.Write( estr );
        }
        
        #endregion
        
        #region XeLib Init/Denit
        
        public static bool Initialize( out string gamePath )
        {
            Console.WriteLine( "\nInitializing XeLib..." );
            Meta.Initialize();
            
            // Set FO4 environment and get the game path
            Setup.SetGameMode( Setup.GameMode.GmFo4 );
            gamePath = Setup.GetGamePath( Setup.GameMode.GmFo4 );
            var result = !string.IsNullOrEmpty( gamePath );
            
            Console.WriteLine( result ? "XeLib initialized" : "Error initializing XeLib" );
            WriteMessages();
            
            return result;
        }
        
        public static void Denit()
        {
            Console.Write( "\nClosing XeLib..." );
            Meta.Close();
            Console.WriteLine( "closed" );
        }
        
        #endregion
        
        #region [Un]Load plugins
        
        public static bool Load( string[] plugins )
        {
            if( ( plugins == null )||( plugins.Length < 1 ) ) return false;
            if( isLoaded ) return false;
            
            Console.WriteLine( "\nLoading files..." );
            
            var g = new List<string>();
            for( int i = 0; i < plugins.Length; i++ )
            {
                var p = plugins[ i ];
                if( !string.IsNullOrEmpty( p ) )
                {
                    g.Add( p );
                    Console.WriteLine( string.Format( "\t[{0}] = {1}", i.ToString( "X2" ), p ) );
                }
            }
            
            const float timerInterval = 10.0f;
            var nextTimerUpdate = timerInterval;
            var timer = new System.Diagnostics.Stopwatch();
            
            Console.WriteLine( "\nXeLib.LoadPlugins() :: [00:00] :: Start..." );
            timer.Start();
            Setup.LoadPlugins( g );
            
            var state = Setup.LoaderState.IsInactive;
            while( ( state != Setup.LoaderState.IsDone )&&( state != Setup.LoaderState.IsError ) )
            {
                WriteMessages();
                
                // Release The Kraken!
                System.Threading.Thread.Sleep( 0 );
                
                state = Setup.GetLoaderStatus();
                
                if( timer.Elapsed.TotalSeconds >= nextTimerUpdate )
                {
                    Console.WriteLine( string.Format(
                        "XeLib.LoadPlugins() :: [{0}:{1}] :: Still loading...",
                        timer.Elapsed.Minutes.ToString( "d2" ),
                        timer.Elapsed.Seconds.ToString( "d2" )
                        ) );
                    nextTimerUpdate += timerInterval;
                }
            }
            timer.Stop();
            WriteMessages();
            
            var exitMessage = state != Setup.LoaderState.IsError
                ? "XeLib.LoadPlugins() :: [{0}:{1}] :: Finished\n"
                : "XeLib.LoadPlugins() :: [{0}:{1}] :: Load Error!\n";
            Console.WriteLine( string.Format(
                exitMessage,
                timer.Elapsed.Minutes.ToString( "d2" ),
                timer.Elapsed.Seconds.ToString( "d2" )
                ) );
            
            isLoaded = state == Setup.LoaderState.IsDone;
            return isLoaded;
        }
        
        #endregion
        
        #region Build XeLib Cross References
        
        public static bool BuildReferences()
        {
            if( !isLoaded ) return false;
            
            Console.WriteLine( "\nBuilding references for files..." );
            
            const float timerInterval = 10.0f;
            var nextTimerUpdate = timerInterval;
            var timer = new System.Diagnostics.Stopwatch();
            
            Console.WriteLine( "\nXeLib.BuildReferences() :: [00:00] :: Start..." );
            timer.Start();
            Setup.BuildReferencesEx( ElementHandle.BaseXHandleValue, false );
            
            var state = Setup.LoaderState.IsInactive;
            while( ( state != Setup.LoaderState.IsDone )&&( state != Setup.LoaderState.IsError ) )
            {
                WriteMessages();
                
                // Release The Kraken!
                System.Threading.Thread.Sleep( 0 );
                
                state = Setup.GetLoaderStatus();
                
                if( timer.Elapsed.TotalSeconds >= nextTimerUpdate )
                {
                    Console.WriteLine( string.Format(
                        "XeLib.BuildReferences() :: [{0}:{1}] :: Still building...",
                        timer.Elapsed.Minutes.ToString( "d2" ),
                        timer.Elapsed.Seconds.ToString( "d2" )
                        ) );
                    nextTimerUpdate += timerInterval;
                }
            }
            timer.Stop();
            WriteMessages();
            
            var exitMessage = state != Setup.LoaderState.IsError
                ? "XeLib.BuildReferences() :: [{0}:{1}] :: Finished\n"
                : "XeLib.BuildReferences() :: [{0}:{1}] :: Build Error!\n";
            Console.WriteLine( string.Format(
                exitMessage,
                timer.Elapsed.Minutes.ToString( "d2" ),
                timer.Elapsed.Seconds.ToString( "d2" )
                ) );
            
            isLoaded = state == Setup.LoaderState.IsDone;
            return isLoaded;
        }
        
        #endregion
        
        public static void TryDispose<THandle>( ref THandle handle, THandle possibleCloneOf = null ) where THandle : ElementHandle
        {
            if( !handle.IsValid() ) return;
            if(
                ( !possibleCloneOf.IsValid() )||
                ( !handle.DuplicateOf( possibleCloneOf ) )
            )   handle.Dispose();
            handle = null;
        }
        
        // TODO:  The following functions should be in the appropriate test, not the common test functions!
        
        /*
        public static ElementHandle CopyMoveToCell( ElementHandle source, ElementHandle destination, bool moveRefrToCell = false )
        {
            if( ( !source.IsValid() )||( !destination.IsValid() ) ) return null;
            if( destination.Signature != "CELL" ) return null;
            var sSignature = source.Signature;
            if( sSignature != "REFR" ) return null;
            
            ElementHandle newRecord = Handle.Invalid;
            
            if( moveRefrToCell )
            {   // Override [and move to new CELL]
                var sfHandle = source.FileHandle;
                var dfHandle = destination.FileHandle;
                var inSameFile = sfHandle.CloneOf( dfHandle );
                Console.WriteLine( "OVERRIDE REFR - InSameFile ? " + inSameFile );
                newRecord = inSameFile
                    ? source
                    : Elements.CopyElement( source, dfHandle, false );
                if( !newRecord.IsValid() ) return Handle.Invalid;
                newRecord.SetCell( destination );
            }
            else
            {   // New record in destination CELL
                Console.WriteLine( "NEW REFR" );
                newRecord = Elements.AddElement( destination, sSignature );
                if( !newRecord.IsValid() ) return Handle.Invalid;
            }
            
            // Appropriate persistence flag for new record
            const string PERSISTENT = "Persistent";
            var dPersistent = destination.GetRecordFlag( PERSISTENT );
            Console.WriteLine( "Persistence: " + dPersistent );
            Console.WriteLine( "Current Flags: " + newRecord.RecordFlags.ToString( "X8" ) );
            newRecord.SetRecordFlag( PERSISTENT, dPersistent );
            Console.WriteLine( "New Flags: " + newRecord.RecordFlags.ToString( "X8" ) );
            
            if( newRecord != source )
            {
                var sElements = Elements.GetElements( source );
                if( ( sElements != null )&&( sElements.Length > 0 ) )
                {
                    foreach( var eHandle in sElements )
                    {
                        //var eType = eHandle.ElementType;
                        Handle nHandle = Handle.Invalid;
                        var eLPath = eHandle.LocalPath;
                        if(
                            ( eLPath != "Record Header" )&&
                            ( eLPath != "Cell" )&&
                            ( eHandle.Signature != "EDID" )
                        )
                        {
                            nHandle = Elements.CopyElement( eHandle, newRecord, true );
                            if( !nHandle.IsValid() )
                            {
                                Console.WriteLine( "XeLibHelper.ObjectReference.CopyMoveToCell() :: Unable to copy source element to new record!" );
                            }
                            nHandle.Dispose();
                        }
                        eHandle.Dispose();
                    }
                }
            }
            
            return newRecord;
        }
        */
        
        #region Cell
        
        /*
        public static UInt32 GetCellFormID( this Handle record )
        {
            return ElementValues.GetUIntValue( record, "Cell" );
        }
        
        public static void SetCell( this Handle record, UInt32 cellFormID )
        {
            Console.WriteLine( string.Format( "XeLibHelper.ObjectReference.SetCell() :: Pre-set  :: Refr = 0x{0} :: currentCell = 0x{1} :: newCell = 0x{2}", record.FormID.ToString( "X8" ), record.GetCellFormID().ToString( "X8" ), cellFormID.ToString( "X8" ) ) );
            ElementValues.SetUIntValue( record, "Cell", cellFormID );
            Console.WriteLine( string.Format( "XeLibHelper.ObjectReference.SetCell() :: Post-set :: Refr = 0x{0} :: currentCell = 0x{1} :: newCell = 0x{2}", record.FormID.ToString( "X8" ), record.GetCellFormID().ToString( "X8" ), cellFormID.ToString( "X8" ) ) );
        }
        
        public static void SetCell( this Handle record, Handle cell )
        {
            SetCell( record, cell.FormID );
        }
        */
        
        #endregion
        
    }
}