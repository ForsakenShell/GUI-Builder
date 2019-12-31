/*
 * BorderBatch.cs
 *
 * Batch functions for borders.
 *
 * Generally used by but not exclusive to the associated window.
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using Maths;
using Fallout4;
using AnnexTheCommonwealth;

namespace GUIBuilder
{
    /// <summary>
    /// Description of BorderBatch.
    /// </summary>
    public static class BorderBatch
    {

        public const string     WSDS_KYWD_BorderGenerator   = "WorkshopBorderGenerator";
        public const string     WSDS_KYWD_BorderLink        = "WorkshopBorderLink";
        public const string     WSDS_STAT_TerrainFollowing  = "TerrainFollowing";
        public const string     WSDS_STAT_ForcedZ           = "ForcedZ";

        #region Create NIFs
        
        public static List<GUIBuilder.FormImport.ImportBase> CreateNIFs(
            string borderSetName,
            List<Fallout4.WorkshopScript> workshops,
            float gradientHeight,
            float groundOffset,
            float groundSink,
            string targetPath,
            string targetSuffix,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string fileSuffix,
            bool createImportData )
        {
            if(
                ( workshops.NullOrEmpty() )||
                ( string.IsNullOrEmpty( targetPath ) )
            )
                return null;
            
            //DebugLog.Write( "GUIBuilder.BorderBatch.CreateNIFs() :: Start (Workshop)" );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<GUIBuilder.FormImport.ImportBase> list = null;
            
            try
            {
                
                foreach( var workshop in workshops )
                {   // Build a workshop border
                    m.SetCurrentStatusMessage( string.Format( "BorderBatch.CreateNIFsFor".Translate(), borderSetName, workshop.GetFormID( Engine.Plugin.TargetHandle.Master ), workshop.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                    var subList = workshop.CreateBorderNIFs(
                        gradientHeight, groundOffset, groundSink,
                        targetPath, targetSuffix,
                        meshSuffix, meshSubPath,
                        filePrefix, fileSuffix,
                        createImportData );
                    if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
            }
            catch( Exception e )
            {
                DebugLog.WriteLine( "GUIBuilder.BorderBatch.CreateNIFs() :: An exception occured while creating NIFs\n\n" + e.ToString() );
            }
            
            m.StopSyncTimer( "GUIBuilder.BorderBatch :: CreateNIFs() :: Completed (" + borderSetName + ") in {0}", tStart.Ticks );
            m.PopStatusMessage();
            return list;
        }
        
        public static List<GUIBuilder.FormImport.ImportBase> CreateNIFs(
            string borderSetName,
            List<AnnexTheCommonwealth.SubDivision> subdivisions,
            float gradientHeight,
            float groundOffset,
            float groundSink,
            string targetPath,
            string targetSuffix,
            string meshSuffix,
            string meshSubPath,
            string filePrefix,
            string fileSuffix,
            bool createImportData )
        {
            if(
                ( subdivisions.NullOrEmpty() )||
                ( string.IsNullOrEmpty( targetPath ) )
            )
                return null;
            
            //DebugLog.Write( "\nGUIBuilder.BorderBatch.CreateNIFs() :: Start (Sub-Division)" );
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            List<FormImport.ImportBase> list = null;
            
            try
            {
                
                foreach( var subdivision in subdivisions )
                {
                    m.SetCurrentStatusMessage( string.Format( "BorderBatch.CreateNIFsFor".Translate(), borderSetName, subdivision.GetFormID( Engine.Plugin.TargetHandle.Master ), subdivision.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                    var subList = subdivision.CreateBorderNIFs(
                        gradientHeight, groundOffset, groundSink,
                        targetPath, targetSuffix,
                        meshSuffix, meshSubPath,
                        filePrefix, fileSuffix,
                        createImportData );
                    if( ( createImportData )&&( !subList.NullOrEmpty() ) )
                    {
                        if( list == null )
                            list = subList;
                        else
                            list.AddAll( subList );
                    }
                }
            }
            catch( Exception e )
            {
                DebugLog.WriteLine( "GUIBuilder.BorderBatch :: CreateNIFs() :: An exception occured while generating NIFBuilder import file\n\n" + e.ToString() );
            }
            
            m.StopSyncTimer( "GUIBuilder.BorderBatch :: CreateNIFs() :: Completed (" + borderSetName + ") in {0}", tStart.Ticks );
            m.PopStatusMessage();
            return list;
        }
        
        #endregion
        
    }
    
}
