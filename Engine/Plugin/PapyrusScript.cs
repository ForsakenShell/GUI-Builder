/*
 * PapyrusScript.cs
 *
 * Abstraction layer for plugin papyrus scripts, all papyrus scripts use this as their base class.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

using XeLib;

using Engine.Plugin.Attributes;
using Engine.Plugin.Interface;

namespace Engine.Plugin
{
    
    /// <summary>
    /// Description of PapyrusScript.
    /// </summary>
    public abstract class PapyrusScript : IXHandle, ISyncedGUIObject, GUIBuilder.IMouseOver
    {
        
        #region Meta data
        
        IXHandle                        _Ancestor       = null; // Ancestral form (eg, a REFR, ACTI, etc)
        
        List<ElementHandle>             _Handles        = null;
        int                             _LastFullRequiredHandleIndex = -1;
        int                             _LastFullOptionalHandleIndex = -1;
        int                             _WorkingFileHandleIndex = -1;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        protected                       PapyrusScript( Form form )
        {
            _Ancestor = form;
            form.AttachScript( this );
        }
        
        #endregion
        
        #region Disposal
        
        protected bool                  Disposed = false;
        
                                        ~PapyrusScript()
        {
            Dispose( true );
        }
        
        public void                     Dispose()
        {
            Dispose( true );
        }
        
        protected virtual void          Dispose( bool disposing )
        {
            if( Disposed )
                return;
            
            if( !_Handles.NullOrEmpty() )
            {
                ElementHandle.ReleaseHandles( _Handles );
               _Handles.Clear();
               _Handles = null;
            }
            _LastFullRequiredHandleIndex = -1;
            _WorkingFileHandleIndex = -1;
            
            _Ancestor       = null;
            
            Disposed = true;
        }
        
        #endregion
        
        #endregion
        
        #region IXHandle
        
        public override int             GetHashCode()               { return GetFormID( Engine.Plugin.TargetHandle.Master ).GetHashCode() ^ Signature.GetHashCode(); }
        
        public override string          ToString()                  { return string.Format( "{0} :: {1}", Signature, IDString ); }

        public string                   IDString                    { get { return Ancestor.IDString; } }

        #region Required Properties

        public string                   Signature                   { get { return Association.Signature; } }
        
        public IXHandle                 Ancestor                    { get { return _Ancestor; } }
        
        public File[]                   Files                       { get { return _Ancestor.Files; } }
        
        public string[]                 Filenames                   { get { return _Ancestor.Filenames; } }

        public string                   GetFilename( TargetHandle target )
        {
            return _Ancestor.GetFilename( target );
        }

        public uint                     LoadOrder                   { get { return _Ancestor.LoadOrder; } }
        
        /*
        public uint                     FormID                      { get { return _Ancestor.FormID; } }
        
        public string                   EditorID
        {
            get{ return Form.EditorID; }
            set{ Form.EditorID = value; }
        }
        */
        
        public uint                     GetFormID( TargetHandle target )
        {
            return _Ancestor.GetFormID( target );
        }
        public void                     SetFormID( TargetHandle target, uint value )
        {
            _Ancestor.SetFormID( target, value );
        }
        
        public string                   GetEditorID( TargetHandle target )
        {
            return _Ancestor.GetEditorID( target );
        }
        public void                     SetEditorID( TargetHandle target, string value )
        {
            _Ancestor.SetEditorID( target, value );
        }
        
        //public Engine.Plugin.Forms.Fields.Record.FormID FormID      { get { return _Ancestor.FormID; } }
        
        //public Engine.Plugin.Forms.Fields.Record.EditorID EditorID  { get { return _Ancestor.EditorID; } }
        
        public virtual ConflictStatus   ConflictStatus              { get { return _Ancestor.ConflictStatus; } }
        
        #endregion
        
        #region Un/Loading
        
        public virtual bool             Load()                      { return true; }
        
        public virtual bool             PostLoad()                  { return true; }
        
        // For Dispose() see the Allocation/Deallocation region above
        
        #endregion
        
        #region XeLib Handles
        
        #region Files
        
        public ElementHandle            CopyAsOverride()
        {
            // Form.CopyAsOverride() will handle updating the ancestral tree and adding the new script handle (will call AddNewHandle())
            var aoh = _Ancestor.CopyAsOverride();
            return !aoh.IsValid() ? null : WorkingFileHandle;
        }
        
        public virtual bool             IsInWorkingFile()       { return IsInFile( GodObject.Plugin.Data.Files.Working ); }
        
        public bool                     IsInFile( Plugin.File file ) { return HandleFor( file ) != null; }
        
        public bool                     IsModifiedIn( Plugin.File file )
        {
            var h = HandleFor( file );
            return h.IsValid() && h.IsModified;
        }
        
        public ElementHandle            HandleFor( Plugin.File file )
        {
            return !GetHandles()
                ? null
                : _Handles.Find( h => h.Filename.InsensitiveInvariantMatch( file.Filename ) );
        }
        
        #endregion
        
        #region Raw Handles
        
        public bool                     IsHandleFor( ElementHandle handle ) { return handle.IsValid() && GetHandles() && _Handles.Any( handle.DuplicateOf ); }
        
        public bool                     AddNewHandle( ElementHandle newHandle )
        {
            if( !newHandle.IsValid() )
            {
                DebugLog.WriteError( "newHandle is invalid!" );
                return false;
            }
            if( ( newHandle as ScriptHandle ) == null )
            {
                DebugLog.WriteError( "newHandle is not a ScriptHandle!" );
                return false;
            }
            
            var handles = _Handles ?? new List<ElementHandle>();
            
            var insertAt = HandleExtensions.ExistingHandleIndex( _Handles, newHandle );
            if( insertAt >= 0 )
            {
                if( newHandle != _Handles[ insertAt ] )
                {
                    _Handles[ insertAt ].Dispose();
                    _Handles[ insertAt ] = newHandle;
                }
                return true;
            }
            insertAt = HandleExtensions.InsertHandleIndex( _Handles, newHandle );
            if( insertAt < 0 )
            {
                DebugLog.WriteWarning( string.Format( "Unable to get load order insertion index for newHandle {0}", newHandle.ToString() ) );
                return false;
            }
            
            if( insertAt == _Handles.Count )
                _Handles.Add( newHandle );
            else
                _Handles.Insert( insertAt, newHandle );
            
            RecalcHandleIndexes();
            
            return true;
        }
        
        public ElementHandle            MasterHandle                { get { return HandleByIndex( 0 ); } }
        public ElementHandle            WorkingFileHandle           { get { return HandleByIndex( _WorkingFileHandleIndex ); } }
        
        public ElementHandle            LastFullRequiredHandle      { get { return HandleByIndex( _LastFullRequiredHandleIndex ); } }
        public ElementHandle            LastFullOptionalHandle      { get { return HandleByIndex( _LastFullOptionalHandleIndex ); } }
        public ElementHandle            LastHandleBeforeWorkingFile { get { return HandleByIndex( _WorkingFileHandleIndex <= 0 ? 0 :_WorkingFileHandleIndex - 1 ); } }
        
        // Return a cloned list so the caller cannot directly manipulate it.  Adding/Removing handles should be done through the proper API calls.
        public List<ElementHandle>      Handles                     { get { return !GetHandles() ? null : _Handles.Clone(); } }
        
        #endregion
        
        #region Private handle manipulation
        
        bool                            GetHandles()
        {
            if( !_Handles.NullOrEmpty() ) return true;
            
            var formElementHandles = _Ancestor.Handles;
            _Handles = new List<ElementHandle>();
            
            foreach( var feh in formElementHandles )
            {
                var fh = feh as FormHandle;
                if( fh != null )
                {
                    var sh = fh.GetScript( Signature );
                    if( sh.IsValid() )
                        _Handles.Add( sh );
                }
            }
            
            RecalcHandleIndexes();
            
            return true;
        }
        
        void                            RecalcHandleIndexes()
        {
            _LastFullRequiredHandleIndex = -1;
            _LastFullOptionalHandleIndex = -1;
            _WorkingFileHandleIndex      = -1;
            var wLO = GodObject.Plugin.Data.Files.Working.LoadOrder;
            
            if( !_Handles.NullOrEmpty() )
            {
                var c = _Handles.Count();
                for( int i = 0; i < c; i++ )
                {
                    var hOverride = _Handles[ i ];
                    var hLO = hOverride.FileHandle.LoadOrder;
                    if( hLO == wLO )
                        _WorkingFileHandleIndex = i;
                    if( hLO < wLO )
                        _LastFullRequiredHandleIndex = i;
                    if( hLO > wLO )
                        _LastFullOptionalHandleIndex = i;
                }
            }
        }
        
        ElementHandle                   HandleByIndex( int index )
        {
            return !GetHandles() || index < 0 || index >= _Handles.Count
                ? null
                : _Handles[ index ];
        }
        
        #endregion
        
        #endregion
        
        #region Collections
        
        // Scripts don't have collections
        
        public virtual Collection       ParentCollection
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        
        public void                     AddCollection( Collection collection )
        {   throw new NotImplementedException(); }
        
        public Collection               CollectionFor( string signature )
        {   throw new NotImplementedException(); }
        public Collection               CollectionFor<TSync>() where TSync : class, IXHandle
        {   throw new NotImplementedException(); }
        public Collection               CollectionFor( ClassAssociation association )
        {   throw new NotImplementedException(); }
        
        public List<Collection>         ChildCollections
        { get { throw new NotImplementedException(); } }
        
        #endregion
        
        public static TScript           CreateScript<TScript>( Form form )
            where TScript : PapyrusScript
        {
            return (TScript)Activator.CreateInstance( typeof( TScript ), new Object[] { form } );
        }
        
        #endregion
        
        #region ISyncedListViewObject
        
        public virtual string           ExtraInfo                   { get { return Form.ExtraInfo; } }
        
        //public string                   Filename                    { get { return Form.Filename; } }
        
        public virtual bool             ObjectChecked( bool checkedValue )
        {
            return checkedValue;
        }
        
        public event EventHandler       ObjectDataChanged;
        
        bool                            _SupressObjectDataChangedEvent = false;

        public bool ObjectDataChangedEventsSupressed { get { return _SupressObjectDataChangedEvent; } }

        public void                     SupressObjectDataChangedEvents()
        {
            _SupressObjectDataChangedEvent = true;
        }
        public void                     ResumeObjectDataChangedEvents( bool sendevent )
        {
            _SupressObjectDataChangedEvent = false;
            if( sendevent )
                SendObjectDataChangedEvent( this );
        }
        
        public void                     SendObjectDataChangedEvent( object sender )
        {
            if( _SupressObjectDataChangedEvent )
                return;
            EventHandler handler = ObjectDataChanged;
            if( handler != null ) handler( sender, null );
            if( sender != Form )
                Form.SendObjectDataChangedEvent( this );
        }
        
        public virtual bool             InitialCheckedOrSelectedState()
        {
            return false;
        }
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Used to indicate that this script does not actually exist
        /// and is a placeholder for GUIBuilder working data.
        /// </summary>
        public virtual bool             EditorScript                { get { return false; } }
        
        public Form                     Form                        { get { return _Ancestor as Form; } }
        
        public Forms.ObjectReference    Reference                   { get { return Form as Forms.ObjectReference; } }
        
        ClassAssociation                _Association = null;
        /// <summary>
        /// Class Attributes for this instanced Form.
        /// </summary>
        public ClassAssociation         Association
        {
            get
            {
                if( _Association == null )
                    _Association = Reflection.AssociationFrom( this.GetType() );
                return _Association;
            }
        }
        
        #endregion
        
        //public abstract string        ScriptDisplayName
        //{
        //    get;
        //}
        
        #region IMouseOver
        
        public bool                     IsMouseOver( Maths.Vector2f mouse, float maxDistance )
        {
            return
                ( Reference != null )&&
                ( Reference.IsMouseOver( mouse, maxDistance ) );
        }
        
        public List<string>             MouseOverInfo
        {
            get
            {
                var mo = new List<string>();
                mo.Add( string.Format( "Script: \"{0}\"", Signature ) );
                var moel = MouseOverExtra;
                if( !moel.NullOrEmpty() )
                    foreach( var moe in moel )
                        mo.Add( string.Format( "\t{0}", moe ) );
                return mo;
            }
        }
        
        public virtual List<string>     MouseOverExtra              { get { return null; } }
        
        #endregion
        
    }
    
}
