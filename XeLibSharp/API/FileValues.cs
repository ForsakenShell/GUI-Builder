using System;

namespace XeLib.API
{
    public static class FileValues
    {
        
        const string            FileHeaderFlags_Path        = @"File Header\Record Header\Record Flags";
        const string            FileHeaderAuthor_Path       = @"File Header\CNAM";
        const string            FileHeaderDescription_Path  = @"File Header\SNAM";
        const string            NextObjectID_Path           = @"File Header\HEDR\Next Object ID";
        
        public static uint GetNextObjectIDEx( uint uHandle )
        {
            return ElementValues.GetUIntValueEx( uHandle, NextObjectID_Path );
        }
        
        public static bool SetNextObjectIDEx( uint uHandle, uint nextObjectID )
        {
            return ElementValues.SetUIntValueEx( uHandle, NextObjectID_Path, nextObjectID );
        }
        
        public static string GetFileNameEx( uint uHandle )
        {
            return ElementValues.NameEx( uHandle );
        }
        
        public static string GetFileAuthorEx( uint uHandle )
        {
            return ElementValues.GetValueEx( uHandle, FileHeaderAuthor_Path );
        }
        
        public static void SetFileAuthorEx( uint uHandle, string author )
        {
            ElementValues.SetValueEx( uHandle, FileHeaderAuthor_Path, author );
        }
        
        public static string GetFileDescriptionEx( uint uHandle )
        {
            return ElementValues.GetValueEx( uHandle, FileHeaderDescription_Path );
        }
        
        public static void SetFileDescriptionEx( uint uHandle, string description )
        {
            var hasDescription = Elements.HasElementEx( uHandle, FileHeaderDescription_Path );
            if( string.IsNullOrEmpty( description ) )
            {
                if( hasDescription )
                    Elements.RemoveElementEx( uHandle, FileHeaderDescription_Path );
                return;
            }
            if( !hasDescription )
            {
                var hDescription = Elements.AddElementEx<ElementHandle>( uHandle, FileHeaderDescription_Path );
                if( hDescription.IsValid() )
                    hDescription.Dispose();
            }
            ElementValues.SetValueEx( uHandle, FileHeaderDescription_Path, description );
        }
        
        public static bool GetIsEsmEx( uint uHandle )
        {
            return ElementValues.GetFlagEx( uHandle, FileHeaderFlags_Path, "ESM" );
        }
        
        public static void SetIsEsmEx( uint uHandle, bool value )
        {
            ElementValues.SetFlagEx( uHandle, FileHeaderFlags_Path, "ESM", value );
        }
        
        public static bool GetIsEslEx( uint uHandle )
        {
            return ElementValues.GetFlagEx( uHandle, FileHeaderFlags_Path, "ESL" );
        }
        
        public static void SetIsEslEx( uint uHandle, bool value )
        {
            ElementValues.SetFlagEx( uHandle, FileHeaderFlags_Path, "ESL", value );
        }
        
        public static bool GetIsEspEx( uint uHandle )
        {   // No actual flag for ESP, so if it's NOT an ESM and it's NOT an ESL then it's an ESP
            return !GetIsEsmEx( uHandle ) && !GetIsEslEx( uHandle );
        }
        
    }
}