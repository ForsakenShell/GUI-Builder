using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class ElementValues
    {
        
        public static string NameEx( uint uHandle )
        {
            int len;
            return ( Functions.Name( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string LongNameEx( uint uHandle )
        {
            int len;
            return ( Functions.LongName( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string DisplayNameEx( uint uHandle )
        {
            int len;
            return ( Functions.DisplayName( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        // *
        public static string PlacementNameEx( uint uHandle )
        {
            if( !Elements.HasElementEx( uHandle, "NAME" ) )
                return null;
            
            var h = Elements.GetLinksToEx<ElementHandle>( uHandle, "NAME" );
            var result = string.Format( "Places {0}", NameEx( h.XHandle ) );
            h.Dispose();
        
            return result;
        }
        //*/
        
        public static string PathEx( uint uHandle )
        {
            int len;
            return ( Functions.Path( uHandle, true, false, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string LongPathEx( uint uHandle )
        {
            int len;
            return ( Functions.Path( uHandle, false, false, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string LocalPathEx( uint uHandle )
        {
            int len;
            return ( Functions.Path( uHandle, false, true, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string SignatureEx( uint uHandle )
        {
            int len;
            return ( Functions.Signature( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string SortKeyEx( uint uHandle )
        {
            int len;
            return ( Functions.SortKey( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        #region Element Values
        
        #region string Values
        
        public static string GetValueEx( uint uHandle, string path )
        {
            int len;
            return ( Functions.GetValue( uHandle, path, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static bool SetValueEx( uint uHandle, string path, string value )
        {
            return Functions.SetValue( uHandle, path, value );
        }
        
        #endregion
        
        #region Integer Values
        
        public static int GetIntValueEx( uint uHandle, string path )
        {
            int resInt;
            Functions.GetIntValue( uHandle, path, out resInt );
            return resInt;
        }
        
        public static bool SetIntValueEx( uint uHandle, string path, int value )
        {
            return Functions.SetIntValue( uHandle, path, value );
        }
        
        public static uint GetUIntValueEx( uint uHandle, string path )
        {
            uint resInt;
            Functions.GetUIntValue( uHandle, path, out resInt );
            return resInt;
        }
        
        public static bool SetUIntValueEx( uint uHandle, string path, uint value)
        {
            return Functions.SetUIntValue( uHandle, path, value );
        }
        
        #endregion
        
        #region Real Number Values
        
        public static double GetDoubleValueEx( uint uHandle, string path )
        {
            double resDouble;
            Functions.GetFloatValue( uHandle, path, out resDouble );
            return resDouble;
        }
        
        public static bool SetDoubleValueEx( uint uHandle, string path, double value )
        {
            return Functions.SetFloatValue( uHandle, path, value );
        }
        
        public static float GetFloatValueEx( uint uHandle, string path )
        {
            double resDouble;
            Functions.GetFloatValue( uHandle, path, out resDouble );
            return (float)resDouble;
        }
        
        public static bool SetFloatValueEx( uint uHandle, string path, float value )
        {
            return Functions.SetFloatValue( uHandle, path, (double)value );
        }
        
        #endregion
        
        #region Flags
        
        public static bool GetFlagEx( uint uHandle, string path, string name )
        {
            bool resBool;
            Functions.GetFlag( uHandle, path, name, out resBool );
            return resBool;
        }
        
        public static bool SetFlagEx( uint uHandle, string path, string name, bool value )
        {
            return Functions.SetFlag( uHandle, path, name, value );
        }
        
        public static string[] GetEnabledFlagsEx( uint uHandle, string path )
        {
            int len;
            return ( Functions.GetEnabledFlags( uHandle, path, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len ).Split( ',' )
                : null;
        }
        
        public static bool SetEnabledFlagsEx( uint uHandle, string path, string[] flags )
        {
            return Functions.SetEnabledFlags( uHandle, path, string.Join( ",", flags ) );
        }
        
        public static string[] GetAllFlagsEx( uint uHandle, string path )
        {
            int len;
            return ( Functions.GetAllFlags( uHandle, path, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len ).Split( ',' )
                : null;
        }
        
        #endregion
        
        #endregion
        
        #region Meta
        
        public static string[] GetEnumOptionEx( uint uHandle, string path )
        {
            int len;
            return ( Functions.GetEnumOptions( uHandle, path, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len ).Split( ',' )
                : null;
        }
        
        #endregion
        
    }
}