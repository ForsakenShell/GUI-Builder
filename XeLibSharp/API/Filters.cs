using XeLib.Internal;

namespace XeLib.API
{
    public static class Filters
    {
        
        public static bool FilterRecordEx( uint uHandle )
        {
            return Functions.FilterRecord( uHandle );
        }
        
        public static bool ResetFilter()
        {
            return Functions.ResetFilter();
        }
    }
}