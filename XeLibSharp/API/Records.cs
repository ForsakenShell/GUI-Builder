using System;
using System.Collections.Generic;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Records
    {
        public enum ConflictAll : byte
        {
            None = 0,
            CaUnknown = 1,
            CaOnlyOne,
            CaNoConflict,
            CaConflictBenign,
            CaOverride,
            CaConflict,
            CaConflictCritical
        }
        
        public enum ConflictThis : byte
        {
            None = 0,
            CtUnknown = 1,
            CtIgnored,
            CtNotDefined,
            CtIdenticalToMaster,
            CtOnlyOne,
            CtHiddenByModGroup,
            CtMaster,
            CtConflictBenign,
            CtOverride,
            CtIdenticalToMasterWinsConflict,
            CtConflictWins,
            CtConflictLoses
        }
        
        public static uint GetFormIDEx( uint uHandle, bool native = false, bool local = false )
        {
            uint id;
            if( !Functions.GetFormID( uHandle, out id, native ) )
                return 0xFFFFFFFF;
            return local ? id & 0x00FFFFFF : id;
        }
        
        public static bool SetFormIDEx( uint uHandle, uint newFormId, bool native = false, bool fixReferences = true )
        {
            return Functions.SetFormID( uHandle, newFormId, native, fixReferences );
        }
        
        //static Dictionary<int,RecordHandle> _FormIDDictionary = new Dictionary<int, RecordHandle>();
        
        public static FormHandle GetRecordEx( uint uHandle, uint formId, bool searchMasters = false )
        {
            uint resXHandle;
            var resHandle = ( Functions.GetRecord( uHandle, formId, searchMasters, out resXHandle ) )&&( resXHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resXHandle )
                : null;
            return resHandle;
        }
        
        public static FormHandle[] GetRecordsEx( uint uHandle, string search, bool includeOverrides = false )
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Records", "GetRecordsEx()", "uHandle = 0x" + uHandle.ToString( "X8" ), "search = \"" + search + "\"", "includeOverrides = " + includeOverrides } );
            int len;
            var resHandles = ( Functions.GetRecords( uHandle, search, includeOverrides, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<FormHandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static FormHandle[] GetOverridesEx( uint uHandle )
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Records", "GetOverridesEx()", "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resHandles = ( Functions.GetOverrides( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<FormHandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static FormHandle GetMasterRecordEx( uint uHandle )
        {
            uint resHandle;
            return ( Functions.GetMasterRecord( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resHandle )
                : null;
        }
        
        public static FormHandle FindMasterRecordEx( uint uHandle, uint uFormID, bool searchMasters = false )
        {
            if( uFormID == 0 ) return null;
            
            uint recordHandle;
            var resFunc = Functions.GetRecord( uHandle, uFormID, searchMasters, out recordHandle );
            if( ( !resFunc )||( recordHandle == ElementHandle.BaseXHandleValue ) )
                return null;
            
            if( IsMasterEx( recordHandle ) )
                return Helpers.CreateHandle<FormHandle>( recordHandle );
            
            uint masterHandle;
            resFunc = Functions.GetMasterRecord( recordHandle, out masterHandle );
            Functions.Release( recordHandle );
            
            return ( resFunc )&&( masterHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( masterHandle )
                : null;
        }
        
        public static FormHandle GetContainerRecordEx( uint uHandle )
        {
            uint pHandle = uHandle;
            uint pContainer;
            FormHandle resHandle = null;
            while( Functions.GetContainer( pHandle, out pContainer ) )
            {
                var peType = Elements.ElementTypeEx( pContainer );
                if( peType == Elements.ElementTypes.EtFile )
                    break;
                if( peType == Elements.ElementTypes.EtMainRecord )
                {
                    resHandle = Helpers.CreateHandle<FormHandle>( pContainer );
                    break;
                }
                if( pHandle != uHandle )
                    Functions.Release( pHandle );
                pHandle = pContainer;
            }
            if( pHandle != uHandle )
                Internal.Functions.Release( pHandle );
            return resHandle;
        }
        
        public static FormHandle GetRootContainerRecordEx( uint uHandle )
        {
            uint pHandle = uHandle;
            uint pContainer;
            uint rContainer = 0xFFFFFFFF;
            while( Functions.GetContainer( pHandle, out pContainer ) )
            {
                var peType = Elements.ElementTypeEx( pContainer );
                if( peType == Elements.ElementTypes.EtFile )
                    break;
                if( peType == Elements.ElementTypes.EtMainRecord )
                {
                    if( rContainer != 0xFFFFFFFF )
                        Functions.Release( rContainer );
                    rContainer = pContainer;
                }
                if( ( pHandle != uHandle )&&( pHandle != rContainer ) )
                    Functions.Release( pHandle );
                pHandle = pContainer;
            }
            if( ( pHandle != uHandle )&&( pHandle != rContainer ) )
                Functions.Release( pHandle );
            return rContainer != 0xFFFFFFFF
                ? Helpers.CreateHandle<FormHandle>( rContainer )
                : null;
        }
        
        public static FormHandle GetPreviousOverrideEx( uint uHandle, uint uFile )
        {
            uint resHandle;
            return ( Functions.GetPreviousOverride( uHandle, uFile, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resHandle )
                : null;
        }
        
        public static FormHandle GetWinningOverrideEx( uint uHandle )
        {
            uint resHandle;
            return ( Functions.GetWinningOverride( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resHandle )
                : null;
        }
        
        public static FormHandle FindNextRecordEx( uint uHandle, string search, bool byEdId, bool byName )
        {
            uint resHandle;
            return ( Functions.FindNextRecord( uHandle, search, byEdId, byName, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resHandle )
                : null;
        }
        
        public static FormHandle FindPreviousRecordEx( uint uHandle, string search, bool byEdId, bool byName )
        {
            uint resHandle;
            return ( Functions.FindPreviousRecord( uHandle, search, byEdId, byName, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<FormHandle>( resHandle )
                : null;
        }
        
        public static string[] FindValidReferencesEx( uint uHandle, string signature, string search, int limitTo )
        {
            int len;
            return ( Functions.FindValidReferences( uHandle, signature, search, limitTo, out len ) )&&( len > 0 )
                ? Helpers.GetResultStringArray( len )
                : null;
        }
        
        public static FormHandle[] GetReferencedByEx( uint uHandle )
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Records", "GetReferencedByEx()", "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resHandles = ( Functions.GetReferencedBy( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<FormHandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static bool ExchangeReferencesEx( uint uHandle, uint oldFormId, uint newFormId )
        {
            return Functions.ExchangeReferences( uHandle, oldFormId, newFormId );
        }
        
        public static bool IsMasterEx( uint uHandle )
        {
            bool resBool;
            return ( Functions.IsMaster( uHandle, out resBool ) )&&( resBool );
        }
        
        public static bool IsInjectedEx( uint uHandle )
        {
            bool resBool;
            return ( Functions.IsInjected( uHandle, out resBool ) )&&( resBool );
        }
        
        public static bool IsOverrideEx( uint uHandle )
        {
            bool resBool;
            return ( Functions.IsOverride( uHandle, out resBool ) )&&( resBool );
        }
        
        public static bool IsWinningOverrideEx( uint uHandle )
        {
            bool resBool;
            return ( Functions.IsWinningOverride( uHandle, out resBool ) )&&( resBool );
        }
        
        public static NodeHandle GetNodesEx( uint uHandle )
        {
            uint resHandle;
            return ( Functions.GetNodes( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? new NodeHandle( resHandle )
                : null;
        }
        
        public static ConflictData GetConflictDataEx( uint uNodes, uint uHandle )
        {
            byte resOne, resTwo;
            return Functions.GetConflictData( uNodes, uHandle, out resOne, out resTwo )
                ? new ConflictData( resOne, resTwo )
                : new ConflictData( ConflictAll.None, ConflictThis.None );
        }
        
        public static ConflictData GetRecordConflictDataEx( uint uHandle )
        {
            var nodes = GetNodesEx( uHandle );
            var result = GetConflictDataEx( nodes.XHandle, uHandle );
            nodes.Dispose();
            return result;
        }
        
        public static THandle[] GetNodeElementsEx<THandle>( uint uNodes, uint uHandle ) where THandle : ElementHandle
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Records", "GetNodeElementsEx<" + typeof( THandle ).ToString() + ">()", "uNodes = 0x" + uNodes.ToString( "X8" ), "uHandle = 0x" + uHandle.ToString( "X8" ) } );
            int len;
            var resNodes = ( Functions.GetNodeElements( uNodes, uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<THandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resNodes;
        }
        
        public struct ConflictData
        {
            
            public readonly ConflictAll CAll;
            public readonly ConflictThis CThis;
            
            public ConflictData( ConflictAll cAll, ConflictThis cThis )
            {
                CAll = cAll;
                CThis = cThis;
            }
            
            public ConflictData( Byte cAll, Byte cThis )
            {
                CAll = (ConflictAll)cAll;
                CThis = (ConflictThis)cThis;
            }
        }
        
    }
}