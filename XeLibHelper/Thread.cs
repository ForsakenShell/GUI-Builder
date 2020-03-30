/*
 * Thread.cs
 *
 * Static extension functions for XeLib's thread.
 *
 * User: 1000101
 * Date: 03/07/2018
 * Time: 2:31 PM
 * 
 */
using System;

using XeLib.API;

namespace XeLibHelper
{
    /// <summary>
    /// Static extension functions for XeLib Handles when the handle is a reference record.
    /// </summary>
    public static class Thread
    {
        
        #region Syncronization
        
        public static bool Sync( string xThreadTypeMethod, WorkerThreadPool.WorkerThread syncWith = null )
        {
            DebugLog.OpenIndentLevel( xThreadTypeMethod, false );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            // Can't signal XeLib to stop :(
            syncWith = null;
            
            var state = Setup.LoaderState.IsInactive;
            while( ( state != Setup.LoaderState.IsDone )&&( state != Setup.LoaderState.IsError ) )
            {
                // Release The Kraken!
                System.Threading.Thread.Sleep( 100 );
                
                var xm = XeLib.API.Messages.GetMessages();
                XeLib.API.Messages.ClearMessages();
                if( !string.IsNullOrEmpty( xm ) )
                {
                    DebugLog.WriteLine( xm );
                    m.SetCurrentStatusMessage( xm );
                }
                if( ( syncWith != null )&&( syncWith.StopSignal ) )
                    return false;
                
                state = Setup.GetLoaderStatus();
            }

            var s = Messages.GetMessages();
            if( !string.IsNullOrEmpty( s ) )
            {
                DebugLog.WriteLine( string.Format(
                    "XEditLib output:\n{0}"
                    , s ) );
                Messages.ClearMessages();
            }

            m.StopSyncTimer( tStart );
            m.PopStatusMessage();
            
            DebugLog.CloseIndentLevel();
            
            return state == Setup.LoaderState.IsDone;
        }
        
        #endregion
        
    }
}
