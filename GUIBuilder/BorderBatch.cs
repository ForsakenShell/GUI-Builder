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

        #region Import NIFs

        public static bool ImportNIFs( string guiBuilderImportFile, bool enableControlsOnClose )
        {
            if( string.IsNullOrEmpty( guiBuilderImportFile ) )
                return false;
            
            //DebugLog.Write( "\nGUIBuilder.BorderBatch.ImportNIFs() :: Start :: From File" );
            
            var result = false;
            
            var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
            m.PushStatusMessage();
            var tStart = m.SyncTimerElapsed();
            
            var importLines = File.ReadLines( guiBuilderImportFile ).ToList();
            var count = importLines.Count;
            if( count < 1 ) goto LocalAbort;
            
            List<FormImport.ImportBase> importForms = null;
            
            #region Parse import file
            
            m.SetCurrentStatusMessage( "BorderBatch.ParsingImportFile".Translate() );
            
            for( int index = 0; index < count; index++ )
            {
                var importLine = importLines[ index ];
                
                var lineWords = importLine.ParseImportLine();
                if( !lineWords.NullOrEmpty() )
                {
                    
                    // First keyword:value should be the type
                    var kv = lineWords[ 0 ].ParseImportLine( ':' );
                    
                    if( kv[ 0 ] == "Type" )
                    {
                        if( kv[ 1 ] == "STAT" )
                            FormImport.ImportBase.AddToList( ref importForms, new FormImport.ImportBorderStatic( lineWords ) );
                        
                        else if( kv[ 1 ] == "REFR" )
                            FormImport.ImportBase.AddToList( ref importForms, new FormImport.ImportBorderReference( lineWords ) );
                        
                        else
                        {
                            DebugLog.WriteLine( string.Format(
                                "GUIBuilder.BorderBatch.ImportNIFs() :: Unexpected value for \"Type\" got \"{0}\"",
                                kv[ 1 ] ) );
                            goto LocalAbort;
                        }
                    }
                    else
                    {
                        DebugLog.WriteLine( string.Format(
                            "GUIBuilder.BorderBatch.ImportNIFs() :: Unexpected keyword, expected \"Type\" got \"{0}\"",
                            kv[ 0 ] ) );
                        goto LocalAbort;
                    }
                }
                
            }
            
            #endregion
            
            //DebugLog.Write( "GUIBuilder.BorderBatch.ImportNIFs() :: Complete :: From File" );
            bool tmp = false;
            result = FormImport.ImportBase.ShowImportDialog( importForms, enableControlsOnClose, ref tmp );
            
        LocalAbort:
            m.StopSyncTimer( "GUIBuilder.BorderBatch :: ImportNIFs() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            return result;
        }
        
        #endregion
        
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
