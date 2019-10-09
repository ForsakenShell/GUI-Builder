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
    
    public struct LinkedRefs
    {
        
        private Handle _Parent;
        private List<Structs.LinkedRef> _LinkedReferences;
        //private Handle[] _Handles;
        
        #region Allocation & Disposal
        
        public LinkedRefs( Handle parent )
        {
            //Console.WriteLine( "struct LinkedRefs :: cTor()" );
            _Parent = parent;
            _LinkedReferences = new List<Structs.LinkedRef>();
            FetchLinkedRefsFromParent();
        }
        
        #endregion
        
        private void FetchLinkedRefsFromParent()
        {
            if( _LinkedReferences != null )
                return;
            //Console.WriteLine( "struct LinkedRefs :: FetchLinkedRefsFromParent()" );
            var handles = Elements.GetElements( _Parent, Structs.LinkedRef.XPath, false, false );
            if( handles.NullOrEmpty() )
                return;
            _LinkedReferences = new List<Structs.LinkedRef>();
            foreach( var handle in handles )
            {
                _LinkedReferences.Add(
                    new Structs.LinkedRef( handle ) );
                //Meta.Release( Handle );
            }
        }
        
        public int Count
        {
            get
            {
                return _LinkedReferences.Count;
            }
        }
        
        public LinkedRef this[ int index ]
        {
            get
            {
                return _LinkedReferences[ index ];
            }
            set
            {
                _LinkedReferences[ index ] = value;
            }
        }
        
        public int Find( UInt32 reference = Constant.FormID_None, UInt32 keyword = Constant.FormID_None )
        {
            if( ( reference == Constant.FormID_None )&&( keyword == Constant.FormID_None ) ) // Both can't be none
                return -1;
            
            if( ( reference == Constant.FormID_None )&&( keyword != Constant.FormID_None ) ) // May be true (unknown reference, get by keyword)
                for( int index = 0; index < _LinkedReferences.Count; index++ )
                    if( keyword == _LinkedReferences[ index ].KeywordFormID )
                        return index;
            
            if( ( reference != Constant.FormID_None )&&( keyword == Constant.FormID_None ) ) // May also be true (known reference, no keyword)
                for( int index = 0; index < _LinkedReferences.Count; index++ )
                    if( ( reference == _LinkedReferences[ index ].ReferenceFormID )&&( _LinkedReferences[ index ].KeywordFormID == Constant.FormID_None ) )
                        return index;
            
            if( ( reference != Constant.FormID_None )&&( keyword != Constant.FormID_None ) ) // Both may be true (known reference and keyword)
                for( int index = 0; index < _LinkedReferences.Count; index++ )
                    if( ( reference == _LinkedReferences[ index ].ReferenceFormID )&&( keyword == _LinkedReferences[ index ].KeywordFormID ) )
                        return index;
            
            return -1;                                                     // No matching linked ref found
        }
        
        public Structs.LinkedRef GetLinkedRef( UInt32 reference = Constant.FormID_None, UInt32 keyword = Constant.FormID_None )
        {
            int index = Find( reference, keyword );
            return index < 0
                ? LinkedRef.Invalid
                : _LinkedReferences[ index ];
        }
        
        public Forms.ObjectReference GetLinkedRef( UInt32 keyword = Constant.FormID_None )
        {
            int index = Find( Constant.FormID_None, keyword );
            return index < 0
                ? null
                : _LinkedReferences[ index ].Reference;
        }
        
        public bool AddLinkedRef( UInt32 reference = Constant.FormID_None, UInt32 keyword = Constant.FormID_None )
        {
            if( Find( reference, keyword ) >= 0 )
                return false;
            
            var lr = new Structs.LinkedRef( _Parent, reference, keyword );
            _LinkedReferences.Add( lr );
            
            return true;
        }
        
        public bool RemoveLinkedRef( UInt32 reference = Constant.FormID_None, UInt32 keyword = Constant.FormID_None )
        {
            int index = Find( reference, keyword );
            if( index < 0 )
                return false;
            
            bool removed = _LinkedReferences[ index ].Remove();
            if( removed )
                _LinkedReferences.RemoveAt( index );
            
            return true;
        }
        
    }
    
}
