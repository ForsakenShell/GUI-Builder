using System;
using System.Linq;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Masters
    {
        
        public static bool CleanMastersEx( uint uHandle )
        {
            return Functions.CleanMasters( uHandle );
        }
        
        public static bool SortMastersEx( uint uHandle )
        {
            return Functions.SortMasters( uHandle );
        }
        
        public static bool AddMasterEx( uint uHandle, string filename )
        {
            return Functions.AddMaster( uHandle, filename );
        }
        
        public static bool AddRequiredMastersEx( uint uHandle, uint uFile, bool asNew = false )
        {
            return Functions.AddRequiredMasters( uHandle, uFile, asNew );
        }
        
        public static FileHandle[] GetMastersEx( uint uHandle )
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Masters", "GetMastersEx()", "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resHandles = ( Functions.GetMasters( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<FileHandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static FileHandle[] GetRequiredByEx( uint uHandle )
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Masters", "GetRequiredByEx()", "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resHandles = ( Functions.GetRequiredBy( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<FileHandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static string[] GetMasterNamesEx( uint uHandle )
        {
            int len;
            return ( Functions.GetMasterNames( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultStringArray( len )
                : null;
        }
        
        /*
        public static void AddAllMastersEx( uint uHandle )
        {
            var filename = ElementValues.NameEx( uHandle );
            var loadedFiles = Setup.GetLoadedFileNames();
            var fileIndex = Array.IndexOf( loadedFiles, filename );
            
            for( var i = 0; i < fileIndex; i++ )
            {
                if( loadedFiles[ i ].EndsWith( ".Hardcoded.dat", StringComparison.InvariantCultureIgnoreCase ) ) continue;
                AddMasterEx( uHandle, loadedFiles[ i ] );
            }
        }
        */
        
        /*
        public static string[] GetAvailableMastersEx( uint uHandle )
        {
            var filename = ElementValues.NameEx( uHandle );
            var allMasters = Setup.GetLoadedFileNames();
            var currentMasters = GetMasterNamesEx( uHandle );
            var index = Array.IndexOf( allMasters, filename );
            return allMasters.Take( index ).Except( currentMasters ).ToArray();
        }
        */
        
    }
}