/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 10/07/2018
 * Time: 11:52 AM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

using Maths;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Forms.Fields.Structs
{
    
    public struct LinkedRef : IDisposable
    {
        
        public static LinkedRef Invalid = new LinkedRef( Handle.Invalid );
        
        public static readonly string XPath = "Linked References";
        private static readonly string _Reference = "Ref";
        private static readonly string _Keyword = "Keyword/Ref";
        
        private Handle _Handle;
        
        #region Allocation & Disposal
        
        public LinkedRef( Handle handle )
        {
            _Handle = handle;
            Disposed = false;
        }
        
        public LinkedRef( Handle parentHandle, UInt32 referenceFormID, UInt32 keywordFormID )
        {
            //Console.WriteLine( "struct LinkedRef :: cTor()" );
            var entry = Elements.AddElement( parentHandle, XPath + _Reference );
            _Handle = Elements.GetContainer( entry );
            Disposed = false;
            ReferenceFormID = referenceFormID;
            KeywordFormID = keywordFormID;
        }
        
        #region Semi-Public API:  Destructor & IDispose
        
        // Handle "double-free" by combinations of explicit disposal[s] and GC disposal
        
        private bool Disposed;
        
        public void Dispose()
        {
            Dispose( true );
        }
        
        public void Dispose( bool disposing )
        {
            if( Disposed )
                return;
            
            if( _Handle != Handle.Invalid )
            {
                _Handle.Dispose();
                _Handle = Handle.Invalid;
            }
            
            Disposed = true;
        }
        
        #endregion
        
        #endregion
        
        public bool IsValid()
        {
            return _Handle != Handle.Invalid;
        }
        
        public Forms.ObjectReference Reference
        {
            get
            {
                var rfid = ReferenceFormID;
                if( ( rfid == Constant.FormID_Invalid )||( rfid == Constant.FormID_None ) )
                    return null;
                var rform = GodObject.Plugin.GetFormByID( rfid );
                return rform == null
                    ? null
                    : rform as Forms.ObjectReference;
            }
            set
            {
                UInt32 rfid = Constant.FormID_None;
                if( value != null )
                    rfid = value.FormID;
                ReferenceFormID = rfid;
            }
        }
        
        public Forms.Keyword Keyword
        {
            get
            {
                var rfid = KeywordFormID;
                if( ( rfid == Constant.FormID_Invalid )||( rfid == Constant.FormID_None ) )
                    return null;
                var rform = GodObject.Plugin.GetFormByID( rfid );
                return rform == null
                    ? null
                    : rform as Forms.Keyword;
            }
            set
            {
                UInt32 rfid = Constant.FormID_None;
                if( value != null )
                    rfid = value.FormID;
                KeywordFormID = rfid;
            }
        }
        
        public UInt32 ReferenceFormID
        {
            get
            {
                return _Handle == Handle.Invalid
                    ? Constant.FormID_Invalid
                    : ElementValues.GetUIntValue( _Handle, _Reference );
            }
            set
            {
                if( _Handle == Handle.Invalid )
                    return;
                ElementValues.SetUIntValue( _Handle, _Reference, value );
            }
        }
        
        public UInt32 KeywordFormID
        {
            get
            {
                return _Handle == Handle.Invalid
                    ? Plugin.Constant.FormID_Invalid
                    : ElementValues.GetUIntValue( _Handle, _Keyword );
            }
            set
            {
                if( _Handle == Handle.Invalid )
                    return;
                ElementValues.SetUIntValue( _Handle, _Keyword, value );
            }
        }
        
        public bool Remove()
        {
            if( _Handle == Handle.Invalid )
                return false;
            bool removed = Elements.RemoveElement( _Handle );
            if( removed )
            {
                _Handle = Handle.Invalid;
                Dispose();
            }
            return removed;
        }
    }
    
}
