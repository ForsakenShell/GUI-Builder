using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XeLib.API;

// TODO:  Change string return functions to use string.Format() instead of CS syntatic sugar so it will work on older CS compilers

namespace XeLib.Internal
{
    public class Helpers
    {
        public static XeLibException GetException( string message )
        {
            return new XeLibException( message, Messages.GetExceptionMessage() );
        }

        public static string ElementContext( uint handle, string path )
        {
            return "{handle}, \"{path}\"";
        }

        public static string ArrayItemContext( uint handle, string path, string subpath, string value )
        {
            return "{handle}: {path}, {subpath}, {value}";
        }

        public static string FlagContext(uint handle, string path, string name)
        {
            return "{handle}, \"{path}\\{name}\"";
        }

        public static string GetExceptionMessageString(int len)
        {
            if (len < 1) return "";
            var bytes = new Byte[len * 2];
            if (!Functions.GetExceptionMessage(bytes, len))
                throw GetException("GetExceptionMessage failed");
            return Encoding.Unicode.GetString(bytes, 0, len * 2);
        }

        public static string GetMessageString(int len)
        {
            if (len < 1) return "";
            var bytes = new Byte[len * 2];
            if (!Functions.GetMessages(bytes, len))
                throw GetException("GetMessages failed");
            return Encoding.Unicode.GetString(bytes, 0, len * 2);
        }

        public static string GetResultString( int len )
        {
            if( len < 1 ) return null;
            var bytes = new Byte[ len * 2 ];
            return Functions.GetResultString( bytes, len )
                ? Encoding.Unicode.GetString( bytes, 0, len *2 )
                : null;
        }

        public static string[] GetResultStringArray( int len )
        {
            return GetResultString( len ).Split( new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries );
        }
        
        public static bool HandleMatchesElementEx( uint uHandle, Type hType )
        {
            var result = true;
            var etHandle = Elements.ElementTypeEx( uHandle );
            var tAttributes = hType.GetCustomAttributes( typeof( HandleMapping ), true ).FirstOrDefault() as HandleMapping;
            if( ( tAttributes != null )&&( tAttributes.AllowedElementTypes != null ) )
                result = tAttributes.AllowedElementTypes.Contains( etHandle );
            //DebugLog.Write( string.Format( "XeLib.Internal.Helpers :: HandleMatchesElementEx() :: uHandle = 0x{0} :: hType = {1} :: etHandle = {2} :: result = {3}", uHandle.ToString( "X8" ), hType.ToString(), etHandle.ToString(), result ) );
            return result;
        }
        
        public static THandle CreateHandle<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            if( uHandle == ElementHandle.BaseXHandleValue ) return null;
            var hType = typeof( THandle );
            var handleTypeOk = HandleMatchesElementEx( uHandle, hType );
            var resHandle = handleTypeOk
                ? (THandle)Activator.CreateInstance( hType, new Object[] { uHandle } )
                : null;
            //DebugLog.WriteLine( string.Format( "XeLib.Internal.Helpers :: CreateHandle<{0}>() :: uHandle = 0x{1} :: handleTypeOk = {2} :: resHandle = {3}", hType.ToString(), uHandle.ToString( "X8" ), handleTypeOk, resHandle.ToString() ) );
            return resHandle;
        }
        
        public static THandle CreateHandle<THandle>( ElementHandle cloneOf ) where THandle : ElementHandle
        {
            if( cloneOf == null ) return null;
            var hType = typeof( THandle );
            var handleTypeOk = HandleMatchesElementEx( cloneOf.XHandle, hType );
            var resHandle = handleTypeOk
                ? (THandle)Activator.CreateInstance( hType, new Object[] { cloneOf } )
                : null;
            //DebugLog.WriteLine( string.Format( "XeLib.Internal.Helpers :: CreateHandle<{0}>() :: cloneOf = 0x{1} :: handleTypeOk = {2} :: resHandle = {3}", hType.ToString(), cloneOf.ToString(), handleTypeOk, resHandle.ToString() ) );
            return resHandle;
        }
        
        public static uint[] GetXHandleArray( int len )
        {
            if( len < 1 ) return null;
            var array = new uint[ len ];
            return Functions.GetResultArray( array, len )
                ? array
                : null;
            
        }
        
        public static THandle[] GetHandleArray<THandle>( int len ) where THandle : ElementHandle
        {
            var array = GetXHandleArray( len );
            return array != null
                ? Array.ConvertAll( array, a => CreateHandle<THandle>( a ) )
                : null;
        }

        public static Dictionary<string, string> ToDictionary(string input)
        {
            var dict =
                input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split(new[] { '=' }, 2))
                    .ToDictionary(sp => sp[0], sp => sp[1]);
            return dict;
        }
    }
}
