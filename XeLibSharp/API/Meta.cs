using System;
using System.Collections.Generic;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Meta
    {
        
        public enum SortBy : byte
        {
            None = 0,
            FormId = 1,
            EditorId = 2,
            Name = 3
        }
        
        public static void Initialize()
        {
            Functions.InitXEdit();
        }
        
        public static void Close()
        {
            try
            {
                Functions.CloseXEdit();
            }
            catch( Exception e )
            {
                throw new Exception( "XeLib.Close() threw an error, why we're wrapping it I'm not sure.\nInner Exception:\n" + e.ToString() );
            }
        }
        
        public static string GetGlobal( string key )
        {
            int len;
            return ( Functions.GetGlobal( key, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string GetGlobals()
        {
            int len;
            return ( Functions.GetGlobals( out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static bool SetSortMode( SortBy mode, bool reverse )
        {
            return Functions.SetSortMode( Convert.ToByte( mode ), reverse );
        }
        
        public static bool ReleaseEx( uint uHandle )
        {
            return Functions.Release( uHandle );
        }
        
        public static bool ReleaseNodesEx( uint uHandle )
        {
            return Functions.ReleaseNodes( uHandle );
        }
        
        public static bool SwitchEx( uint uOne, uint uTwo )
        {
            return Functions.Switch( uOne, uTwo );
        }
        
        public static uint[] GetDuplicateXHandlesEx( uint uHandle )
        {
            int len;
            return ( Functions.GetDuplicateHandles( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetXHandleArray( len )
                : null;
        }
        
        public static THandle[] GetDuplicateHandlesEx<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Meta", "GetDuplicateHandlesEx<" + typeof( THandle ).ToString() + ">()", "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resHandles = ( Functions.GetDuplicateHandles( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<THandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static bool ResetStore()
        {
            return Functions.ResetStore();
        }
        
        public static string SignatureFromName( string name )
        {
            int len;
            return ( Functions.SignatureFromName( name, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static string NameFromSignature( string sig )
        {
            int len;
            return ( Functions.NameFromSignature( sig, out len ) )&&( len > 0 )
                ? Helpers.GetResultString( len )
                : null;
        }
        
        public static Dictionary<string, string> GetSignatureNameMap()
        {
            int len;
            return ( Functions.GetSignatureNameMap( out len ) )&&( len > 0 )
                ? Helpers.ToDictionary( Helpers.GetResultString( len ) )
                : null;
        }
        
    }
}