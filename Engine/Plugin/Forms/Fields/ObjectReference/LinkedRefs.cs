/*
 * LinkedRefs.cs
 *
 * Object Reference Linked References field.
 *
 */

using System.Collections.Generic;

using XeLib;
using XeLib.API;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class LinkedRefs : RawField
    {
        
        //public static readonly string XPath = "Linked References";
        //static readonly string _Element = "XLKR - Linked Reference";
        static readonly string         _Reference = "Ref";
        static readonly string         _Keyword = "Keyword/Ref";
        
        List<ElementHandle>            _LinkedReferences = null;
        
        public LinkedRefs( Form form ) : base( form, "Linked References" ) {}
        
        #region Internal management
        
        void                            ClearCurrentLinkedRefHandles()
        {
            if( _LinkedReferences.NullOrEmpty() )
                return;
            
            foreach( var lr in _LinkedReferences )
                lr.Dispose();
            
            _LinkedReferences = null;
        }
        
        void                            GetLinkedRefsFromForm()
        {
            if( _LinkedReferences != null )
                return;
            
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetLinkedRefsFromForm()", Form.ToString() } );
            
            _LinkedReferences = new List<ElementHandle>();
            
            if( !HasValue( TargetHandle.WorkingOrLastFullRequired ) )
                return;
                //goto localReturnResult;
            
            var bH = HandleFromTarget( TargetHandle.WorkingOrLastFullRequired );
            
            var handles = bH.GetElements<ElementHandle>( XPath, false, false );
            if( ( handles == null )||( handles.Length == 0 ) )
            {
                var s = Messages.GetMessages();
                var m = "ElementHandle.GetElements() == null";
                if( !string.IsNullOrEmpty( s ) )
                {
                    m += "\n";
                    m += s;
                }
                DebugLog.WriteWarning( this.GetType().ToString(), "GetLinkedRefsFromForm()", m );
                return;
                //goto localReturnResult;
            }
            
            foreach( var handle in handles )
                _LinkedReferences.Add( handle );
            
        //localReturnResult:
            //DebugLog.CloseIndentLevel( _LinkedReferences.Count.ToString() );
        }
        
        public int                      FindKeywordIndex( uint keywordFormID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "FindKeywordIndex()", "keywordFormID = 0x" + keywordFormID.ToString( "X8" ), Form.ToString() } );
            int result = -1;
            
            if( ( keywordFormID == Engine.Plugin.Constant.FormID_None )||( keywordFormID == Engine.Plugin.Constant.FormID_Invalid ) )
                goto localReturnResult;
            
            GetLinkedRefsFromForm();
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            
            result = _LinkedReferences.FindIndex( (h) => ( h.GetUIntValueEx( _Keyword ) == keywordFormID ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        public int                      FindReferenceIndex( uint refID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "FindReferenceIndex()", "refID = 0x" + refID.ToString( "X8" ), Form.ToString() } );
            
            int result = -1;
            
            GetLinkedRefsFromForm();
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            
            result = _LinkedReferences.FindIndex( (h) => ( h.GetUIntValueEx( _Reference ) == refID ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( result.ToString() );
            return result;
        }
        
        #endregion
        
        public int                      Count
        {
            get
            {
                GetLinkedRefsFromForm();
                return _LinkedReferences == null
                    ? 0
                    : _LinkedReferences.Count;
            }
        }
        
        public Forms.ObjectReference    GetLinkedRef( uint keywordFormID )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetLinkedRef()", "keywordFormID = 0x" + keywordFormID.ToString( "X8" ), Form.ToString() } );
            int index = FindKeywordIndex( keywordFormID );
            var result = index < 0
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.ObjectReference>( _LinkedReferences[ index ].GetUIntValueEx( _Reference ) );
            //DebugLog.CloseIndentLevel<Forms.ObjectReference>( result );
            return result;
        }
        
        public void                     SetLinkedRef( uint refID, uint keywordFormID = Engine.Plugin.Constant.FormID_None )
        {
            if( ( refID == Engine.Plugin.Constant.FormID_None )&&( keywordFormID == Engine.Plugin.Constant.FormID_None ) )
                return;
            
            var refIndex = refID == Engine.Plugin.Constant.FormID_None ? -1 : FindReferenceIndex( refID );
            var keyIndex = keywordFormID == Engine.Plugin.Constant.FormID_None ? -1 : FindKeywordIndex( keywordFormID );
            if( ( refIndex >= 0 )&&( refIndex == keyIndex ) )
                return; // Linked ref with this refID using this keyword already exists
            
            if( refID != Engine.Plugin.Constant.FormID_None )
            {
                if( refIndex < 0 )
                {   // No linked ref for this refID, add a new one
                    Add( refID, keywordFormID );
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
        
        public bool                     Add( uint refID, uint keywordFormID = Constant.FormID_None )
        {
            bool isInWorkingFile = Form.IsInWorkingFile(); // Check but don't copy (yet)
            if( !isInWorkingFile )
            {
                if( !CreateRootElement( true, false ) ) // Copy override it now
                    return false;
                ClearCurrentLinkedRefHandles();
                GetLinkedRefsFromForm();
            }
            
            var index = FindReferenceIndex( refID );
            if( index >= 0 ) // Already exists
            {
                if( !isInWorkingFile ) // Copied as override
                    Form.SendObjectDataChangedEvent();
                return false;
            }
            
            // New element in form
            index = AddEx( refID, keywordFormID );
            
            Form.SendObjectDataChangedEvent();
            return index >= 0;
        }
        
        int                             AddEx( uint refID, uint keywordFormID )
        {
            // New element in form
            var index = _LinkedReferences.Count;
            var lrh = Form.WorkingFileHandle.AddArrayItem<ElementHandle>( XPath, "", "" );
            if( !lrh.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddEx()", string.Format(
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
        
        public bool                     Remove( uint refID, uint keywordFormID = Constant.FormID_None )
        {
            bool isInWorkingFile = Form.IsInWorkingFile(); // Check but don't copy (yet)
            if( !isInWorkingFile )
            {
                if( !CreateRootElement( true, false ) ) // Copy override it now
                    return false;
                ClearCurrentLinkedRefHandles();
                GetLinkedRefsFromForm();
            }
            
            var index = FindReferenceIndex( refID );
            if( index < 0 ) // Already doesn't exist
            {
                if( !isInWorkingFile ) // Copied as override
                    Form.SendObjectDataChangedEvent();
                return false;
            }
            
            var result = RemoveEx( index );
            
            Form.SendObjectDataChangedEvent();
            return result;
        }
        
        bool                            RemoveEx( int index )
        {
            var lrHandle = _LinkedReferences[ index ];
            if( !Elements.RemoveElementOrParentEx( lrHandle.XHandle ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "RemoveEx()", string.Format(
                    "Unable to RemoveElementOrParentEx() 0x{0} from 0x{1} - \"{2}\"",
                    lrHandle.ToString(),
                    this.Form.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ),
                    this.Form.GetEditorID( Engine.Plugin.TargetHandle.LastValid ) ) );
                return false;
            }
            _LinkedReferences.RemoveAt( index );
            lrHandle.Dispose();
            return true;
        }
        
        #region Indexed Properties Internals
        
        #region ReferenceID
        
        IndexedProperty<int, uint>     _ReferenceIDIndexer;
        
        uint                            GetReferenceID( int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetReferenceID()", "index = " + index.ToString(), Form.ToString() } );
            uint result = Constant.FormID_Invalid;
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = _LinkedReferences[ index ].GetUIntValueEx( _Reference );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( "0x" + result.ToString( "X8" ) );
            return result;
        }
        void                            SetReferenceID( int index, uint value )
        {
            if( _LinkedReferences.NullOrEmpty() )
                return;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
            _LinkedReferences[ index ].SetUIntValueEx( _Reference, value );
        }
        
        #endregion
        
        #region Object Reference
        
        IndexedProperty<int, Forms.ObjectReference> _ReferenceIndexer;
        
        Forms.ObjectReference           GetReference( int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetReference()", "index = " + index.ToString(), Form.ToString() } );
            Forms.ObjectReference result = null;
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.ObjectReference>( _LinkedReferences[ index ].GetUIntValueEx( _Reference ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<Forms.ObjectReference>( result );
            return result;
        }
        void                            SetReference( int index, Forms.ObjectReference value )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "SetReference()", "index = " + index.ToString(), Form.ToString() } );
            
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
        
        IndexedProperty<int, uint>     _KeywordFormIDIndexer;
        
        uint                            GetKeywordFormID( int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetKeywordFormID()", "index = " + index.ToString(), Form.ToString() } );
            uint result = Constant.FormID_Invalid;
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = _LinkedReferences[ index ].GetUIntValueEx( _Keyword );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel( "0x" + result.ToString( "X8" ) );
            return result;
        }
        void                            SetKeywordFormID( int index, uint value )
        {
            if( _LinkedReferences.NullOrEmpty() )
                return;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                return;
            _LinkedReferences[ index ].SetUIntValueEx( _Keyword, value );
        }
        
        #endregion
        
        #region Keyword
        
        IndexedProperty<int, Forms.Keyword> _KeywordIndexer;
        
        Forms.Keyword                   GetKeyword( int index )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "GetKeyword()", "index = " + index.ToString(), Form.ToString() } );
            Forms.Keyword result = null;
            
            if( _LinkedReferences.NullOrEmpty() )
                goto localReturnResult;
            if( ( index < 0 )||( index >= _LinkedReferences.Count ) )
                goto localReturnResult;
            
            result = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Keyword>( _LinkedReferences[ index ].GetUIntValueEx( _Keyword ) );
            
        localReturnResult:
            //DebugLog.CloseIndentLevel<Forms.Keyword>( result );
            return result;
        }
        void                            SetKeyword( int index, Forms.Keyword value )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "SetKeyword()", "index = " + index.ToString(), Form.ToString() } );
            
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
        
        #region Indexed Properties
        
        public IndexedProperty<int, uint> ReferenceID
        {
            get
            {
                GetLinkedRefsFromForm();
                if( _ReferenceIDIndexer == null )
                    _ReferenceIDIndexer = new IndexedProperty<int, uint>( GetReferenceID, SetReferenceID );
                return _ReferenceIDIndexer;
            }
        }
        
        public IndexedProperty<int, Forms.ObjectReference> Reference
        {
            get
            {
                //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Reference[]", Form.ToString() } );
                GetLinkedRefsFromForm();
                if( _ReferenceIndexer == null )
                    _ReferenceIndexer = new IndexedProperty<int, Forms.ObjectReference>( GetReference, SetReference );
                //DebugLog.CloseIndentLevel();
                return _ReferenceIndexer;
            }
        }
        
        public IndexedProperty<int, uint> KeywordFormID
        {
            get
            {
                GetLinkedRefsFromForm();
                if( _KeywordFormIDIndexer == null )
                    _KeywordFormIDIndexer = new IndexedProperty<int, uint>( GetKeywordFormID, SetKeywordFormID );
                return _KeywordFormIDIndexer;
            }
        }
        
        public IndexedProperty<int, Forms.Keyword> Keyword
        {
            get
            {
                GetLinkedRefsFromForm();
                if( _KeywordIndexer == null )
                    _KeywordIndexer = new IndexedProperty<int, Forms.Keyword>( GetKeyword, SetKeyword );
                return _KeywordIndexer;
            }
        }
        
        #endregion
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            return null;
        }
        
    }
    
}
