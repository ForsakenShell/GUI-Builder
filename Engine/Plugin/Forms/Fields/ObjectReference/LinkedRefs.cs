/*
 * LinkedRefs.cs
 *
 * Object Reference Linked References field.
 *
 */

using System.Collections.Generic;

using XeLib;
using XeLib.API;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class LinkedRefs : RawField
    {
        
        //public static readonly string XPath = "Linked References";
        //static readonly string _Element = "XLKR - Linked Reference";
        static readonly string         _Reference = "Ref";
        static readonly string         _Keyword = "Keyword/Ref";
        
        ElementHandle                  _LastHandle = null;
        List<ElementHandle>            _LinkedReferences = null;
        
        public LinkedRefs( Form form ) : base( form, "Linked References" ) {}
        
        #region Internal management
        
        void                            ClearCurrentLinkedRefHandles()
        {
            _LastHandle = null;
            
            if( _LinkedReferences.NullOrEmpty() )
                return;
            
            foreach( var lr in _LinkedReferences )
                lr.Dispose();
            
            _LinkedReferences = null;
        }
        
        void                            GetLinkedRefsFromForm( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( _LastHandle == h ) return;
            
            ClearCurrentLinkedRefHandles();
            
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetLinkedRefsFromForm()", Form.ToString() } );
            
            _LastHandle = h;
            
            if( !HasValue( target ) )
                return;
                //goto localReturnResult;
            
            _LinkedReferences = new List<ElementHandle>();
            
            var handles = h.GetElements<ElementHandle>( XPath, false, false );
            if( ( handles == null )||( handles.Length == 0 ) )
            {
                var s = Messages.GetMessages();
                var m = "ElementHandle.GetElements() == null";
                if( !string.IsNullOrEmpty( s ) )
                {
                    m += "\n";
                    m += s;
                }
                DebugLog.WriteWarning( m );
                return;
                //goto localReturnResult;
            }
            
            foreach( var handle in handles )
                _LinkedReferences.Add( handle );
            
        //localReturnResult:
            //DebugLog.CloseIndentLevel( _LinkedReferences.Count.ToString() );
        }
        
        #endregion
        
        public int                      FindKeywordIndex( TargetHandle target, uint keywordFormID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "FindKeywordIndex()", "keywordFormID = 0x" + keywordFormID.ToString( "X8" ), Form.ToString() } );
            int result = -1;
            
            if( ( keywordFormID == Engine.Plugin.Constant.FormID_None )||( keywordFormID == Engine.Plugin.Constant.FormID_Invalid ) )
                goto localReturnResult;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            
            result = _LinkedReferences.FindIndex( (h) => ( h.GetUIntValueEx( _Keyword ) == keywordFormID ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        public int                      FindReferenceIndex( TargetHandle target, uint refID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "FindReferenceIndex()", "refID = 0x" + refID.ToString( "X8" ), Form.ToString() } );
            
            int result = -1;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            
            result = _LinkedReferences.FindIndex( (h) => ( h.GetUIntValueEx( _Reference ) == refID ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        public int                      GetCount( TargetHandle target )
        {
            GetLinkedRefsFromForm( target );
            
            return _LinkedReferences.NullOrEmpty()
                ? 0
                : _LinkedReferences.Count;
        }
        
        public Forms.ObjectReference    GetLinkedRef( TargetHandle target, uint keywordFormID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetLinkedRef()", "keywordFormID = 0x" + keywordFormID.ToString( "X8" ), Form.ToString() } );
            GetLinkedRefsFromForm( target );
            
            int index = FindKeywordIndex( target, keywordFormID );
            var result = index < 0
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.ObjectReference>( _LinkedReferences[ index ].GetUIntValueEx( _Reference ) );
            //DebugLog.CloseIndentLevel<Forms.ObjectReference>( result );
            return result;
        }
        
        public void                     SetLinkedRef( TargetHandle target, uint refID, uint keywordFormID = Engine.Plugin.Constant.FormID_None )
        {
            if( ( refID == Engine.Plugin.Constant.FormID_None )&&( keywordFormID == Engine.Plugin.Constant.FormID_None ) )
                return;
            
            GetLinkedRefsFromForm( target );
            
            var refIndex = refID == Engine.Plugin.Constant.FormID_None ? -1 : FindReferenceIndex( target, refID );
            var keyIndex = keywordFormID == Engine.Plugin.Constant.FormID_None ? -1 : FindKeywordIndex( target, keywordFormID );
            if( ( refIndex >= 0 )&&( refIndex == keyIndex ) )
                return; // Linked ref with this refID using this keyword already exists
            
            if( refID != Engine.Plugin.Constant.FormID_None )
            {
                if( refIndex < 0 )
                {   // No linked ref for this refID, add a new one
                    Add( target, refID, keywordFormID );
                }
                else
                {   // Update the existing linked refs keyword
                    _LinkedReferences[ refIndex ].SetUIntValueEx( _Keyword, keywordFormID );
                }
                if( ( keyIndex >= 0 )&&( keyIndex != refIndex ) )
                {   // Remove the old linked ref using this keyword as Fallout 4 only allows one linked ref per keyword (or many with none)
                    RemoveEx( keyIndex );
                }
            }
            else if( keyIndex >= 0 )
            {   // Clear by keyword
                RemoveEx( keyIndex );
            }
        }
        
        public bool                     Add( TargetHandle target, uint refID, uint keywordFormID = Constant.FormID_None )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: Add() :: Target must be TargetHandle.Working!" );
            
            bool isInWorkingFile = Form.IsInWorkingFile(); // Check but don't copy (yet)
            if( ( !isInWorkingFile )&&( !CreateRootElement( true, false ) ) ) // Copy override it now
                return false;
            
            GetLinkedRefsFromForm( target );
            
            var index = FindReferenceIndex( target, refID );
            if( index >= 0 ) // Already exists
                return false;
            
            // New element in form
            index = AddEx( refID, keywordFormID );
            
            Form.SendObjectDataChangedEvent( null );
            return index >= 0;
        }
        
        int                             AddEx( uint refID, uint keywordFormID )
        {
            // New element in form
            if( _LinkedReferences == null )
                _LinkedReferences = new List<ElementHandle>();
            var index = _LinkedReferences.Count;
            var lrh = Form.WorkingFileHandle.AddArrayItem<ElementHandle>( XPath, "", "" );
            if( !lrh.IsValid() )
            {
                DebugLog.WriteError( string.Format(
                    "Unable to AddArrayItem() \"{0}\" to {1}",
                    XPath,
                    this.Form.ToString() ) );
                return -1;
            }
            
            // Save the handle to the new element
            _LinkedReferences.Add( lrh );
            
            // Assign the elements values
            _LinkedReferences[ index ].SetUIntValueEx( _Reference, refID );
            _LinkedReferences[ index ].SetUIntValueEx( _Keyword, keywordFormID );
            
            return index;
        }
        
        public bool                     Remove( TargetHandle target, uint refID, uint keywordFormID = Constant.FormID_None )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: Remove() :: Target must be TargetHandle.Working!" );
            
            bool isInWorkingFile = Form.IsInWorkingFile(); // Check but don't copy (yet)
            if( ( !isInWorkingFile )&&( !CreateRootElement( true, false ) ) ) // Copy override it now
            
            GetLinkedRefsFromForm( target );
            
            var index = FindReferenceIndex( target, refID );
            if( index < 0 ) // Already doesn't exist
                return false;
            
            var result = RemoveEx( index );
            
            Form.SendObjectDataChangedEvent( null );
            return result;
        }
        
        bool                            RemoveEx( int index )
        {
            var lrHandle = _LinkedReferences[ index ];
            if( !Elements.RemoveElementOrParentEx( lrHandle.XHandle ) )
            {
                DebugLog.WriteError( string.Format(
                    "Unable to RemoveElementOrParentEx() 0x{0} from {1}",
                    lrHandle.ToString(),
                    this.Form.IDString ) );
                return false;
            }
            _LinkedReferences.RemoveAt( index );
            lrHandle.Dispose();
            return true;
        }
        
        #region Indexed Properties Internals
        
        #region ReferenceID
        
        public uint                     GetReferenceID( TargetHandle target, int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetReferenceID()", "index = " + index.ToString(), Form.ToString() } );
            uint result = Constant.FormID_Invalid;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = _LinkedReferences[ index ].GetUIntValueEx( _Reference );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( "0x" + result.ToString( "X8" ) );
            return result;
        }
        public void                     SetReferenceID( TargetHandle target, int index, uint value )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: SetReferenceID() :: Target must be TargetHandle.Working!" );
            GetLinkedRefsFromForm( target );
            if( _LinkedReferences.NullOrEmpty() )
                return;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
            _LinkedReferences[ index ].SetUIntValueEx( _Reference, value );
        }
        
        #endregion
        
        #region Object Reference
        
        public Forms.ObjectReference    GetReference( TargetHandle target, int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetReference()", "index = " + index.ToString(), Form.ToString() } );
            Forms.ObjectReference result = null;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.ObjectReference>( _LinkedReferences[ index ].GetUIntValueEx( _Reference ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<Forms.ObjectReference>( result );
            return result;
        }
        public void                     SetReference( TargetHandle target, int index, Forms.ObjectReference value )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: SetReference() :: Target must be TargetHandle.Working!" );
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "SetReference()", "index = " + index.ToString(), Form.ToString() } );
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                return;
                //goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
                //goto localReturnResult;
            
            var refID = value == null
                ? Constant.FormID_None
                : value.GetFormID( Engine.Plugin.TargetHandle.Master );
            
            _LinkedReferences[ index ].SetUIntValueEx( _Reference, refID );
            
        //localReturnResult:
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #region Keyword FormID
        
        public uint                     GetKeywordFormID( TargetHandle target, int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetKeywordFormID()", "index = " + index.ToString(), Form.ToString() } );
            uint result = Constant.FormID_Invalid;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = _LinkedReferences[ index ].GetUIntValueEx( _Keyword );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( "0x" + result.ToString( "X8" ) );
            return result;
        }
        public void                     SetKeywordFormID( TargetHandle target, int index, uint value )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: SetKeywordFormID() :: Target must be TargetHandle.Working!" );
            GetLinkedRefsFromForm( target );
            if( _LinkedReferences.NullOrEmpty() )
                return;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
            _LinkedReferences[ index ].SetUIntValueEx( _Keyword, value );
        }
        
        #endregion
        
        #region Keyword
        
        public Forms.Keyword            GetKeyword( TargetHandle target, int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "GetKeyword()", "index = " + index.ToString(), Form.ToString() } );
            Forms.Keyword result = null;
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Keyword>( _LinkedReferences[ index ].GetUIntValueEx( _Keyword ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<Forms.Keyword>( result );
            return result;
        }
        public void                     SetKeyword( TargetHandle target, int index, Forms.Keyword value )
        {
            if( target != TargetHandle.Working ) throw new System.ArgumentException( "Engine.Plugin.Forms.Fields.ObjectReference.LinkedRefs :: SetKeyword() :: Target must be TargetHandle.Working!" );
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "SetKeyword()", "index = " + index.ToString(), Form.ToString() } );
            
            GetLinkedRefsFromForm( target );
            
            if( _LinkedReferences.NullOrEmpty() )
                return;
                //goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
                //goto localReturnResult;
            
            var keywordFormID = value == null
                ? Constant.FormID_None
                : value.GetFormID( Engine.Plugin.TargetHandle.Master );
            
            _LinkedReferences[ index ].SetUIntValueEx( _Keyword, keywordFormID );
            
        //localReturnResult:
            //DebugLog.CloseIndentLevel();
        }
        
        #endregion
        
        #endregion
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            return null;
        }
        
    }
    
}
