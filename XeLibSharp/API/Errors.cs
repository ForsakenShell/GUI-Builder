using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Errors
    {
        
        public static bool CheckForErrorsEx( uint uHandle )
        {
            return Functions.CheckForErrors( uHandle );
        }
        
        public static bool GetErrorThreadDone()
        {
            return Functions.GetErrorThreadDone();
        }
        
        // TODO: Json parse result
        // 1000101 sez: Fuck JSon serialization
        public static string GetErrors()
        {
            int len;
            return Functions.GetErrors( out len )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static bool RemoveIdenticalRecordsEx( uint uHandle, bool removeItms = true, bool removeItpos = true )
        {
            return Functions.RemoveIdenticalRecords( uHandle, removeItms, removeItpos );
        }
    }
}