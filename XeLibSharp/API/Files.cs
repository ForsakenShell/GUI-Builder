using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Files
    {
        
        #region Global Enumeration
        
        public static FileHandle AddFile( string filename )
        {
            uint resHandle;
            return ( Functions.AddFile( filename, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FileHandle>( resHandle )
                : null;
        }
        
        public static FileHandle FileByIndex( int index )
        {
            uint resHandle;
            return ( Functions.FileByIndex( index, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FileHandle>( resHandle )
                : null;
        }
        
        public static FileHandle FileByLoadOrder( int loadOrder )
        {
            uint resHandle;
            return ( Functions.FileByLoadOrder( loadOrder, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FileHandle>( resHandle )
                : null;
        }
        
        public static FileHandle FileByName( string filename )
        {
            uint resHandle;
            return ( Functions.FileByName( filename, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FileHandle>( resHandle )
                : null;
        }
        
        public static FileHandle FileByAuthor( string author )
        {
            uint resHandle;
            return ( Functions.FileByAuthor( author, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FileHandle>( resHandle )
                : null;
        }
        
        #endregion
        
        #region File Management
        
        public static bool NukeFileEx( uint uHandle )
        {
            return Functions.NukeFile( uHandle );
        }
        
        public static bool RenameFileEx( uint uHandle, string newFilename )
        {
            return Functions.RenameFile( uHandle, newFilename );
        }
        
        public static bool SaveFileEx( uint uHandle, string filePath = "" )
        {
            return Functions.SaveFile( uHandle, filePath );
        }
        
        #endregion
        
        #region File Checksums
        
        public static string MD5HashEx( uint uHandle )
        {
            int len;
            return ( Functions.MD5Hash( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string CRCHashEx( uint uHandle )
        {
            int len;
            return ( Functions.CRCHash( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        #endregion
        
        #region File Meta
        
        public static int GetRecordCountEx( uint uHandle )
        {
            int resInt;
            Functions.GetRecordCount( uHandle, out resInt );
            return resInt;
        }
        
        public static int GetOverrideRecordCountEx( uint uHandle )
        {
            int resInt;
            Functions.GetOverrideRecountCount( uHandle, out resInt );
            return resInt;
        }
        
        public static uint GetFileLoadOrderEx( uint uHandle )
        {
            uint resInt;
            Functions.GetFileLoadOrder( uHandle, out resInt );
            return resInt;
        }
        
        public static THandle GetFileHeaderEx<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            return Elements.GetElementEx<THandle>( uHandle, "File Header" );
        }
        
        #endregion
        
        #region Record Sorting
        
        public static bool SortEditorIDsEx( uint uHandle, string signature )
        {
            return Functions.SortEditorIDs( uHandle, signature );
        }
        
        public static bool SortNamesEx( uint uHandle, string signature )
        {
            return  Functions.SortNames( uHandle, signature );
        }
        
        #endregion
        
    }
}