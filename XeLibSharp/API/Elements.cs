using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Elements
    {
        
        public enum DefTypes : byte
        {
            DtRecord = 0,
            DtSubRecord,
            DtSubRecordArray,
            DtSubRecordStruct,
            DtSubRecordUnion,
            DtString,
            DtLString,
            DtLenString,
            DtByteArray,
            DtInteger,
            DtIntegerFormater,
            DtIntegerFormaterUnion,
            DtFlag,
            DtFloat,
            DtArray,
            DtStruct,
            DtUnion,
            DtEmpty,
            DtStructChapter,
            DtUnknown = 255
        }
        
        public enum ElementTypes : byte
        {
            EtFile = 0,
            EtMainRecord,
            EtGroupRecord,
            EtSubRecord,
            EtSubRecordStruct,
            EtSubRecordArray,
            EtSubRecordUnion,
            EtArray,
            EtStruct,
            EtValue,
            EtFlag,
            EtStringListTerminator,
            EtUnion,
            EtStructChapter,
            EtUnknown = 255
        }
        
        public enum SmashTypes : byte
        {
            StUnknown = 0,
            StRecord,
            StString,
            StInteger,
            StFlag,
            StFloat,
            StStruct,
            StUnsortedArray,
            StUnsortedStructArray,
            StSortedArray,
            StSortedStructArray,
            StByteArray,
            StUnion
        }
        
        public enum ValueTypes : byte
        {
            VtUnknown = 0,
            VtBytes,
            VtNumber,
            VtString,
            VtText,
            VtReference,
            VtFlags,
            VtEnum,
            VtColor,
            VtArray,
            VtStruct,
            VtNoValue = 255
        }
        
        public static bool HasElementEx( uint uHandle, string path = "" )
        {
            bool resBool;
            return Functions.HasElement( uHandle, path, out resBool ) && resBool;
        }
        
        public static THandle GetElementEx<THandle>( uint uHandle, string path ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetElement( uHandle, path, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle AddElementEx<THandle>( uint uHandle, string path ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.AddElement( uHandle, path, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle AddElementValueEx<THandle>( uint uHandle, string path, string value ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.AddElementValue( uHandle, path, value, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static bool RemoveElementEx( uint uHandle, string path = "" )
        {
            return Functions.RemoveElement( uHandle, path );
        }
        
        public static bool RemoveElementOrParentEx( uint uHandle )
        {
            return Functions.RemoveElementOrParent( uHandle );
        }
        
        public static bool SetElementEx( uint src, uint dest )
        {
            return Functions.SetElement( src, dest );
        }
        
        public static THandle[] GetElementsEx<THandle>( uint uHandle, string path = "", bool sort = false, bool filter = false ) where THandle : ElementHandle
        {
            //DebugLog.OpenIndentLevel( new [] { "XeLib.API.Elements", "GetElementsEx<" + typeof( THandle ).ToString() + ">()", "uHandle = 0x" + uHandle.ToString( "X8" ), "path = \"" + path + "\"", "sort = " + sort, "filter = " + filter } );
            int len;
            var resHandles = ( Functions.GetElements( uHandle, path, sort, filter, out len ) )&&( len > 0 )
                ? Helpers.GetHandleArray<THandle>( len )
                : null;
            //DebugLog.CloseIndentLevel();
            return resHandles;
        }
        
        public static string[] GetDefNamesEx( uint uHandle )
        {
            int len;
            return Functions.GetDefNames( uHandle, out len )
                ? Helpers.GetResultStringArray( len )
                : null;
        }
        
        public static string[] GetAddListEx( uint uHandle )
        {
            int len;
            return Functions.GetAddList( uHandle, out len )
                ? Helpers.GetResultStringArray( len )
                : null;
        }
        
        public static THandle GetLinksToEx<THandle>( uint uHandle, string path ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetLinksTo( uHandle, path, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static bool SetLinksToEx( uint uBase, string path, uint uReference )
        {
            return Functions.SetLinksTo( uBase, path, uReference );
        }
        
        public static THandle GetContainerEx<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetContainer( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle GetElementFileEx<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetElementFile( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle GetElementGroup<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetElementGroup( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle GetElementRecordEx<THandle>( uint uHandle ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetElementRecord( uHandle, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static int ElementCountEx( uint uHandle )
        {
            int resInt;
            Functions.ElementCount( uHandle, out resInt );
            return resInt;
        }
        
        public static bool ElementEqualsEx( uint uOne, uint uTwo )
        {
            bool resBool;
            return Functions.ElementEquals( uOne, uTwo, out resBool ) && resBool;
        }
        
        public static bool ElementMatchesEx( uint uHandle, string path, string value )
        {
            bool resBool;
            return Functions.ElementMatches( uHandle, path, value, out resBool ) && resBool;
        }
        
        public static bool HasArrayItemEx( uint uHandle, string path, string subpath, string value )
        {
            bool resBool;
            return Functions.HasArrayItem( uHandle, path, subpath, value, out resBool) && resBool;
        }
        
        public static THandle GetArrayItemEx<THandle>( uint uHandle, string path, string subpath, string value ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.GetArrayItem( uHandle, path, subpath, value, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle AddArrayItemEx<THandle>( uint uHandle, string path, string subpath, string value ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.AddArrayItem( uHandle, path, subpath, value, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static bool RemoveArrayItemEx( uint uHandle, string path, string subpath, string value )
        {
            return Functions.RemoveArrayItem( uHandle, path, subpath, value );
        }
        
        public static bool MoveArrayItemEx( uint uHandle, int index )
        {
            return Functions.MoveArrayItem( uHandle, index );
        }
        
        public static THandle CopyElementEx<THandle>( uint src, uint dst, bool asNew = false ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.CopyElement( src, dst, asNew, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle FindNextElementEx<THandle>( uint uHandle, string search, bool byPath, bool byValue ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.FindNextElement( uHandle, search, byPath, byValue, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static THandle FindPreviousElementEx<THandle>( uint uHandle, string search, bool byPath, bool byValue ) where THandle : ElementHandle
        {
            uint resHandle;
            return ( Functions.FindPreviousElement( uHandle, search, byPath, byValue, out resHandle ) )&&( resHandle != ElementHandle.BaseXHandleValue )
                ? Helpers.CreateHandle<THandle>( resHandle )
                : null;
        }
        
        public static bool GetSignatureAllowedEx( uint uHandle, string signature )
        {
            bool resBool;
            return Functions.GetSignatureAllowed( uHandle, signature, out resBool ) && resBool;
        }
        
        public static string[] GetAllowedSignaturesEx( uint uHandle )
        {
            int len;
            return ( Functions.GetAllowedSignatures( uHandle, out len ) )&&( len > 0 )
                ? Helpers.GetResultStringArray( len )
                : null;
        }
        
        public static bool GetIsModifiedEx( uint uHandle )
        {
            bool resBool;
            return Functions.GetIsModified( uHandle, out resBool ) && resBool;
        }
        
        public static bool GetIsEditableEx( uint uHandle )
        {
            bool resBool;
            return Functions.GetIsEditable( uHandle, out resBool ) && resBool;
        }
        
        public static bool SetIsEditableEx( uint uHandle, bool isEditable )
        {
            return Functions.SetIsEditable( uHandle, isEditable );
        }
        
        public static bool GetIsRemoveableEx( uint uHandle )
        {
            bool resBool;
            return Functions.GetIsRemoveable( uHandle, out resBool ) && resBool;
        }
        
        public static bool GetCanAddEx( uint uHandle )
        {
            bool resBool;
            return Functions.GetCanAdd( uHandle, out resBool ) && resBool;
        }
        
        public static ElementTypes ElementTypeEx( uint uHandle )
        {
            byte resByte;
            return Functions.ElementType( uHandle, out resByte )
                ? (ElementTypes)resByte
                : ElementTypes.EtUnknown;
        }
        
        public static DefTypes DefTypeEx( uint uHandle )
        {
            byte resByte;
            return Functions.DefType( uHandle, out resByte )
                ? (DefTypes)resByte
                : DefTypes.DtUnknown;
        }
        
        public static SmashTypes SmashTypeEx( uint uHandle )
        {
            byte resByte;
            return Functions.SmashType( uHandle, out resByte )
                ? (SmashTypes)resByte
                : SmashTypes.StUnknown;
        }

        public static ValueTypes ValueTypeEx( uint uHandle )
        {
            byte resByte;
            return !Functions.ValueType( uHandle, out resByte )
                ? (ValueTypes)resByte
                : ValueTypes.VtUnknown;
        }

        public static bool IsSortedEx( uint uHandle )
        {
            bool resBool;
            return Functions.IsSorted( uHandle, out resBool ) && resBool;
        }

        public static bool IsFlagsEx( uint uHandle )
        {
            return ValueTypeEx( uHandle ) == ValueTypes.VtFlags;
        }
        
    }
}