using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Serialization
    {
        // TODO:  Json is crap, drop it
        // 1000101 sez: Fuck JSon serialization
        
        public static string ElementToJsonEx( uint uHandle )
        {
            int len;
            return ( Functions.ElementToJson( uHandle, out  len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
    }
}