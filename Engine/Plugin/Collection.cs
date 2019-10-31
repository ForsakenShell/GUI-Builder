/*
 * Collection.cs
 *
 * Base container class for IDataSync objects.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

using XeLib;
using XeLib.API;

using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;


namespace Engine.Plugin
{
    
    public class Collection : IDisposable, ICollection
    {
        
        #region Container Data
        
        ClassAssociation                _Association            = null;
        
        bool                            _FullLoadComplete       = false;
        public bool                     FullLoadComplete        { get { return _FullLoadComplete; } }
        bool                            _FullPostLoadComplete   = false;
        public bool                     FullPostLoadComplete    { get { return _FullPostLoadComplete; } }
        protected IXHandle              _Ancestor               = null;
        
        protected List<IXHandle>               _AllForms               = null;
        protected Dictionary<uint, IXHandle>    _ByFormID               = null;
        protected Dictionary<string, IXHandle>  _ByEditorID             = null;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        void                            BuildCollection( ClassAssociation association, IXHandle ancestor )
        {
            if( association == null )
                throw new ArgumentException( string.Format( "{0} :: Unable to resolve Association", this.GetType().ToString() ) );
            _Association = association;
            if( ancestor != null )
            {
                _Ancestor = ancestor;
                _Ancestor.AddICollection( this );
            }
        }
        
        public                          Collection( ClassAssociation association, IXHandle ancestor )
        {
            BuildCollection( association, ancestor );
        }
        
        public                          Collection( ClassAssociation association )
        {
            BuildCollection( association, null );
        }
        
        #endregion
        
        #region Disposal
        
        protected bool Disposed = false;
        
                                       ~Collection()
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
            
            if( _ByFormID != null )
                _ByFormID.Clear();
            _ByFormID = null;
            
            if( _ByEditorID != null )
                _ByEditorID.Clear();
            _ByEditorID = null;
            
            if( !_AllForms.NullOrEmpty() )
            {
                foreach( var form in _AllForms )
                    form.Dispose();
                _AllForms.Clear();
            }
            _AllForms = null;
            
            _Association = null;
            
            Disposed = true;
        }
        
        #endregion
        
        #endregion
        
        #region Basic Properties
        
        public ClassAssociation         Association         { get { return _Association; } }
        
        public IXHandle                 Ancestor            { get { return _Ancestor; } }
        public Engine.Plugin.Form       ParentForm          { get { return _Ancestor as Engine.Plugin.Form; } }
        
        #endregion
        
        #region Add/Remove
        
        public virtual bool             Add( IXHandle syncObject )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Add()", "Collection !IsValid" );
                return false;
            }
            if( syncObject == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Add()", "syncObject is null" );
                return false;
            }
            if( !_Association.ClassType.IsInstanceOfType( syncObject ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Add()", string.Format( "Invalid Association.ClassType for \"{0}\"! :: Expected {1} :: Got {2}", syncObject.GetType().ToString(), syncObject.GetType().ToString(), _Association.ClassType.ToString() ) );
                return false;
            }
            
            if( _AllForms == null )
                _AllForms = new List<IXHandle>();
            _AllForms.AddOnce( syncObject );
            
            if( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ValidFormID() )
            {
                _ByFormID = _ByFormID ?? new Dictionary<uint, IXHandle>();
                _ByFormID[ syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ) ] = syncObject;
            }
            
            var soEDID = syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
            if( !string.IsNullOrEmpty( soEDID ) )
            {
                _ByEditorID = _ByEditorID ?? new Dictionary<string, IXHandle>();
                _ByEditorID[ soEDID.ToLower() ] = syncObject;
            }
            
            GodObject.Plugin.Data.Root.AddToMasterTable( syncObject );
            
            //syncObject.Ancestor = _Ancestor; // Shouldn't need to do this now...?
            return true;
        }
        
        public virtual void             Remove( IXHandle syncObject )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Remove()", "!IsValid" );
                return;
            }
            if( syncObject == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Remove()", "syncObject is null" );
                return;
            }
            if( !_Association.ClassType.IsInstanceOfType( syncObject ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Remove()", string.Format( "Invalid Form Type! :: \"{0}\"", syncObject.GetType().ToString() ) );
                return;
            }
            
            if( _AllForms != null )
                _AllForms.Remove( syncObject );
            
            if( ( Engine.Plugin.Constant.ValidFormID( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ) ) )&&( _ByFormID != null ) )
                _ByFormID.Remove( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ) );
            
            var soEDID = syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
            if( ( Engine.Plugin.Constant.ValidEditorID( soEDID ) )&&( _ByEditorID != null ) )
                _ByEditorID.Remove( soEDID.ToLower() );
            
            GodObject.Plugin.Data.Root.RemoveFromMasterTable( syncObject );
            
            //syncObject.Ancestor = null; // Shouldn't need to do this now...?
        }
        
        #endregion
        
        #region Add From Record, Create New
        
        public IXHandle                 AddFromRecord( IXHandle ancestor, ElementHandle handle )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", "Collection !IsValid" );
                return null;
            }
            
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Invalid handle 0x{0}", handle.ToString() ) );
                return null;
            }
            
            var rSig = handle.Signature;
            if( rSig != _Association.Signature )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Invalid record signature!  :: Expected \"{0}\" got \"{1}\"", _Association.Signature, rSig ) );
                return null;
            }
            
            if( ( ancestor == null )&&( !_Association.AllowRootCollection ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Class does not allow root collections! :: {0}",  _Association.ClassType.ToString() ) );
                return null;
            }
            
            var syncObject = Activator.CreateInstance( _Association.ClassType, new Object[] { this, ancestor, handle } ) as IXHandle; //IDataSync;
            if( syncObject == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Unable to create new {0}!", _Association.ClassType.ToString() ) );
                return null;
            }
            
            if( !syncObject.Load() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Form 0x{0} - \"{1}\" :: {2}.Load() returned false!", syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), _Association.ClassType.ToString() ) );
                return null;
            }
            
            var formid = syncObject.GetFormID( Engine.Plugin.TargetHandle.Master );
            foreach( var file in GodObject.Plugin.Data.Files.Loaded )
            {
                if( file.LoadOrder > syncObject.LoadOrder )
                {
                    var hOverride = file.WorkingFileHandle.GetRecord( formid, false );
                    if( hOverride.IsValid() )
                    {
                        //syncObject.UpdateHandles( hOverride ); // Shouldn't need to do this now...?
                        if( !syncObject.Load() )
                        {
                            DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Form 0x{0} - \"{1}\" :: {2}.Load() returned false for override handle!", syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), _Association.ClassType.ToString() ) );
                            return null;
                        }
                    }
                }
            }
            
            if( !syncObject.PostLoad() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "AddFromRecord()", string.Format( "Form 0x{0} - \"{1}\" :: {2}.PostLoad() returned false!", syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), _Association.ClassType.ToString() ) );
                return null;
            }
            
            Add( syncObject );
            
            return syncObject;
        }
        
        public TSync                    CreateNew<TSync>() where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNew<TSync>()", "Collection !IsValid" );
                return null;
            }
            if( typeof( TSync ) != _Association.ClassType )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNew<TSync>()", string.Format( "TSync does not match Form Type :: {0} != {1}", typeof( TSync ).ToString(), _Association.ClassType.ToString() ) );
                return null;
            }
            return CreateNew() as TSync;
        }
        
        public IXHandle                 CreateNew()
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNew()", "Collection !IsValid" );
                return null;
            }
            
            if( Ancestor == null )
                return CreateNewRootForm();
            return ParentForm == null
                ? null
                : CreateNewChildForm();
        }
        
        TSync                           CreateNewChildForm<TSync>() where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm<TSync>()", "Collection !IsValid" );
                return null;
            }
            if( typeof( TSync ) != _Association.ClassType )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm<TSync>()", string.Format( "TSync does not match Form Type :: {0} != {1}", typeof( TSync ).ToString(), _Association.ClassType.ToString() ) );
                return null;
            }
            return CreateNewChildForm() as TSync;
        }
        
        IXHandle                        CreateNewChildForm()
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm()", "Collection !IsValid" );
                return null;
            }
            var pForm = ParentForm;
            if( pForm == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm()", string.Format( "Form Type does not have a parent Form :: {0}", _Association.ClassType.ToString() ) );
                return null;
            }
            
            var wf = GodObject.Plugin.Data.Files.Working;
            var iiwf = pForm.CopyAsOverride(); //pForm.IsInWorkingFile( true );           // Am I in the working file?
            if( !iiwf.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm()", string.Format( "Unable to override parent \"{0}\" Form in {3} :: 0x{1} - \"{2}\"", pForm.Signature, pForm.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), pForm.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), wf.Filename ) );
                return null;
            }
            
            var resHandle = pForm.WorkingFileHandle.AddElement<FormHandle>( _Association.Signature );
            if( !resHandle.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm()", string.Format( "Unable to create new child \"{0}\" Form in parent \"{1}\" Form in {4} :: 0x{2} - \"{3}\"", _Association.Signature, pForm.Signature, pForm.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), pForm.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), wf.Filename ) );
                return null;
            }
            
            // Add the form to the container
            var resObject = AddFromRecord( wf, resHandle );
            if( resObject == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewChildForm()", string.Format( "Unable to add new child \"{0}\" Form to parent \"{1}\" Form in {4} :: 0x{2} - \"{3}\"", _Association.Signature, pForm.Signature, pForm.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ), pForm.GetEditorID( Engine.Plugin.TargetHandle.LastValid ), wf.Filename ) );
                return null;
            }
            
            // Hand the new form back to the caller
            return resObject;
        }
        
        IXHandle                        CreateNewRootForm()
        {
            var wf = GodObject.Plugin.Data.Files.Working;
            if( wf == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewRootForm()", "No working file loaded, unable to create/inject forms!" );
                return null;
            }
            var wfch = wf.WorkingFileHandle.GetElement<ElementHandle>( _Association.Signature );
            if( !wfch.IsValid() )
                wfch = wf.WorkingFileHandle.AddElement<ElementHandle>( _Association.Signature );
            if( !wfch.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewRootForm()", string.Format( "Unable to GetElement or AddElement() for Root Container \"{0}\" in {1}", _Association.Signature, wf.Filename ) );
                return null;
            }
            
            var handle = wfch.AddElement<FormHandle>( _Association.Signature );
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewRootForm()", string.Format( "Unable to AddElement() for a new Form in Root Container \"{0}\" in {1}", _Association.Signature, wf.Filename ) );
                return null;
            }
            
            // Add the form to the container
            var syncObject = AddFromRecord( wf, handle );
            if( syncObject == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "CreateNewRootForm()", string.Format( "Unable to add new root \"{0}\" for Form \"{1}\" in Root Container in {2}", _Association.ClassType.ToString(), _Association.Signature, wf.Filename ) );
                return null;
            }
            
            // Hand the new form back to the caller
            return syncObject;
        }
        
        #endregion
        
        #region Instance Enumeration
        
        public int                      Count
        {
            get
            {
                return _AllForms.NullOrEmpty()
                    ? 0
                    : _AllForms.Count;
            }
        }
        
        IXHandle                        TryLoad( FormHandle record )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "TryLoad()", ( record == null ? "[null]" : record.ToString() ) } );
            
            IXHandle result = null;
            
            if( !record.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", "Invalid record!" );
                goto localReturnResult;
            }
            if( !record.IsMaster )
            {
                var masterRecord = record.GetMasterRecord();
                if( !masterRecord.IsValid() )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", "Cannot get master record!" );
                    goto localReturnResult;
                }
                record = masterRecord;
            }
            var rSig = record.Signature;
            if( rSig != Association.Signature )
            {
                DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", string.Format( "Record Signature expected \"{0}\" got \"{1}\"", Association.Signature, rSig ) );
                goto localReturnResult;
            }
            var file = GodObject.Plugin.Data.Files.Find( record );
            if( file == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", "Unable to find containing file" );
                goto localReturnResult;
            }
            
            // Create a new data sync object (Form) of the appropriate type
            //result = AddFromRecord( file, record );
            var ancestor = _Ancestor;
            if( !ancestor.IsValid() )
                ancestor = file;
            result = AddFromRecord( ancestor, record );
            if( result == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", string.Format( "Unable to create {0} in {1}", _Association.ClassType.ToString(), ancestor.ToString() ) );
                goto localReturnResult;
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 TryLoad( uint formid )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "TryLoad()", "0x" + formid.ToString( "X8" ) } );
            IXHandle result = null;
            var record = Records.FindMasterRecordEx( ElementHandle.BaseXHandleValue, formid, false );
            if( !record.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "TryLoad()", "Unable to get master record!" );
                goto localReturnResult;
            }
            result = TryLoad( record );
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 TryLoad( string editorid )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "TryLoad()", editorid } );
            IXHandle result = null;
            if( ParentForm != null )
            {   // Child form in parent (ie, REFR in a CELL, CELL in a WRLD, etc)
                var handles = ParentForm.Handles;
                foreach( var handle in handles )
                {
                    result = TryLoad( editorid, handle );
                    if( result != null )
                        goto localReturnResult;
                }
            }
            else
            {   // Root form in file (ie, STAT, ACTI, etc)
                foreach( var file in GodObject.Plugin.Data.Files.Loaded )
                {
                    result = TryLoad( editorid, file.MasterHandle );
                    if( result != null )
                        goto localReturnResult;
                }
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        IXHandle                        TryLoad( string editorid, ElementHandle source )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "TryLoad()", editorid, ( source == null ? "[null]" : source.ToString() ) } );
            
            IXHandle result = null;
            
            if( string.IsNullOrEmpty( editorid ) )
                goto localReturnResult;
            
            var records = source.GetRecords( _Association.Signature, false );
            if( records.NullOrEmpty() )
                goto localReturnResult;
            
            var lsEDID = editorid.ToLower();
            for( int i = 0; i < records.Length; i++ )
            {
                var record = records[ i ];
                var recFID = record.EditorID;
                if( !string.IsNullOrEmpty( recFID ) )
                {
                    var lrEDID = recFID.ToLower();
                    if( lrEDID == lsEDID )
                    {
                        result = TryLoad( record );
                        if( ( result == null )||( !result.IsHandleFor( record ) ) )
                           record.Dispose();
                        for( int j = i + 1; j < records.Length; j++ )
                            records[ j ].Dispose(); // <<-- Dispose of the rest of the handles in the array
                        goto localReturnResult;
                    }
                }
                record.Dispose();
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 FindEx( ClassAssociation targetAssociation, FormHandle handle = null, uint formid = 0, string editorid = null, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "FindEx()", ( targetAssociation == null ? "[null]" : targetAssociation.ToString() ), ( handle == null ? "[null]" : handle.ToString() ), "0x" + formid.ToString( "X8" ), editorid, tryLoad.ToString() } );
            
            IXHandle result = null;
            
            bool handleValid = handle.IsValid();
            if( handleValid )
            {
                if( formid == 0 ) formid = handle.FormID;
                if( string.IsNullOrEmpty( editorid ) ) editorid = handle.EditorID;
            }
            
            bool searchByFormID = Engine.Plugin.Constant.ValidFormID( formid );
            bool searchByEditorID = Engine.Plugin.Constant.ValidEditorID( editorid );
            
            // Nothing to search by
            if( ( !searchByFormID )&&( !searchByEditorID ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "FindEx()", "No FormID or EditorID to search for" );
                goto localReturnResult;
            }
            
            bool searchInThis =
                ( targetAssociation == null )||             // Unknown target, search everything (SLOW!)
                ( targetAssociation == Association );       // Target is the same class as this containers contents
            bool searchInChildren = 
                ( targetAssociation != Association )&&      // Also handles equality to null at the same time
                ( Association.HasChildOrGrandchildAssociationOf( targetAssociation ) );
            
            // Expected target is not associated with this container or its children
            if( ( !searchInThis )&&( !searchInChildren ) )
            {
                DebugLog.WriteError( this.GetType().ToString(), "FindEx()", string.Format( "targetAssociation is invalid! :: Expected \"{0}\" or child, got \"{1}\"", Association.Signature, ( targetAssociation == null ? "null" : targetAssociation.Signature ) ) );
                goto localReturnResult;
            }
            
            if( searchInThis )
            {
                if( searchByFormID )
                {
                    if( _ByFormID == null )
                        _ByFormID = new Dictionary<uint, IXHandle>();
                    if( _ByFormID.TryGetValue( formid, out result ) )
                        goto localReturnResult;
                    if( tryLoad )
                    {
                        result = handleValid
                            ? TryLoad( handle )
                            : TryLoad( formid );
                        if( result != null )
                            goto localReturnResult;
                    }
                }
                else if( searchByEditorID )
                {
                    if( _ByEditorID == null )
                        _ByEditorID = new Dictionary<string, IXHandle>();
                    if( _ByEditorID.TryGetValue( editorid.ToLower(), out result ) )
                        goto localReturnResult;
                    if( tryLoad )
                    {
                        result = handleValid
                            ? TryLoad( handle )
                            : TryLoad( editorid );
                        if( result != null )
                            goto localReturnResult;
                    }
                }
            }
            if( searchInChildren )
            {
                var allForms = ToList( tryLoad: tryLoad );
                if( allForms.NullOrEmpty() )
                    goto localReturnResult;
                
                foreach( var cForm in allForms )
                {
                    var cCollections = cForm.ChildCollections;
                    if( !cCollections.NullOrEmpty() )
                    {
                        foreach( var cCollection in cCollections )
                        {
                            if(
                                ( targetAssociation == null )|| // Unkown target, search everything (SLOW)
                                ( cCollection.Association.HasChildOrGrandchildAssociationOf( targetAssociation ) )
                            ){
                                result = cCollection.FindEx( targetAssociation, handle, formid, editorid, tryLoad );
                                if( result != null )
                                    goto localReturnResult;
                            }
                        }
                    }
                }
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( XeLib.FormHandle handle, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Find()", ( handle == null ? "[null]" : handle.ToString() ), tryLoad.ToString() } );
            IXHandle result = null;
            
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find()", "handle !IsValid()" );
                goto localReturnResult;
            }
            
            var association = Reflection.AssociationFrom( handle.Signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find()", "Unable to get Association for collection" );
                goto localReturnResult;
            }
            
            // Get parent container from handle, work through
            // ancestory back to the root container and build
            // the tree as needed to this point.
            //handle.DumpContainerTree();
            
            // Build the tree of forms
            var refTree = handle.GetContainerRecordTree();
            
            // Load the form tree
            var forms = GodObject.Plugin.Data.Root.LoadRecordTree( refTree );
            GodObject.Plugin.Data.Root.DisposeOfHandlesNotUsedByObjects( refTree, forms );
            
            result = forms.NullOrEmpty()
                ? null
                : forms.Last(); // Last form is the one we want
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string signature, uint formid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Find()", signature, "0x" + formid.ToString( "X8" ), tryLoad.ToString() } );
            
            IXHandle result = null;
            
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find()", string.Format( "ClassAssociation[ \"{0}\" ] :: !IsValid()", signature ) );
                goto localReturnResult;
            }
            result = FindEx( association, null, formid, null, tryLoad );
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string signature, string editorid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Find()", signature, editorid, tryLoad.ToString() } );
            
            IXHandle result = null;
            
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find()", string.Format( "ClassAssociation[ \"{0}\" ] :: !IsValid()", signature ) );
                goto localReturnResult;
            }
            result = FindEx( association, null, 0, editorid, tryLoad );
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( uint formid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Find()", "0x" + formid.ToString( "X8" ), tryLoad.ToString() } );
            
            var h = Records.FindMasterRecordEx( ElementHandle.BaseXHandleValue, formid, false );
            var result = Find( h, tryLoad );
            if( ( result == null )||( !result.IsHandleFor( h ) ) )
                h.Dispose();
            
       //localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public TSync                    Find<TSync>( uint formid, bool tryLoad = true ) where TSync : class, IXHandle
        {
            //DebugLog.OpenIndentLevel( string.Format( "{0} :: Find<{1}>() :: 0x{2} :: {3}\n{{", this.GetType().ToString(), typeof( TSync ).ToString(), formid.ToString( "X8" ), tryLoad ) );
            
            TSync result = null;
            
            var type = typeof( TSync );
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find<TSync>()", string.Format( "ClassAssociation[ {0} ] :: !IsValid()", ( type == null ? "null" : type.ToString() ) ) );
                goto localReturnResult;
            }
            result = FindEx( null, null, formid, null, tryLoad ) as TSync;
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<TSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string editorid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "Find()", editorid, tryLoad.ToString() } );
            
            var result = FindEx( null, null, 0, editorid, tryLoad );
            
       //localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public TSync                    Find<TSync>( string editorid, bool tryLoad = true ) where TSync : class, IXHandle
        {
            //DebugLog.OpenIndentLevel( string.Format( "{0} :: Find<{1}>() :: \"{2}\" :: {3}\n{{", this.GetType().ToString(), typeof( TSync ).ToString(), editorid, tryLoad ) );
            
            TSync result = null;
            
            var type = typeof( TSync );
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "Find<TSync>()", string.Format( "ClassAssociation[ {0} ] :: !IsValid()", ( type == null ? "null" : type.ToString() ) ) );
                goto localReturnResult;
            }
            result =  FindEx( null, null, 0, editorid, tryLoad ) as TSync;
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<TSync>( result );
            return result;
        }
        
        public List<IXHandle>           ToList( int loadOrderFilter = -1, bool tryLoad = true )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "ToList()", "Collection !IsValid" );
                return null;
            }
            if( !_FullLoadComplete )
            {
                if( !tryLoad )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "ToList()", "Must call LoadAllForms() first!" );
                    return null;
                }
                if( !LoadAllForms() )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "ToList()", "Unable to LoadAllForms()!" );
                    return null;
                }
            }
            if( !_FullPostLoadComplete )
            {
                if( !tryLoad )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "ToList()", "Must call PostLoad() first!" );
                    return null;
                }
                if( !PostLoad() )
                {
                    DebugLog.WriteError( this.GetType().ToString(), "ToList()", "Unable to PostLoad()!" );
                    return null;
                }
            }
            if( _AllForms.NullOrEmpty() )
            {
                //DebugLog.Write( FormatLogMessage( "ToList()", "_AllForms is null or empty" ) );
                return null;    // Not an error
            }
            
            return ( loadOrderFilter >=0 )&&( loadOrderFilter <= GodObject.Plugin.Data.Files.Working.LoadOrder )
                ? _AllForms.Where( (x) => ( x.LoadOrder == loadOrderFilter ) ).ToList()
                : _AllForms;
        }
        
        public List<TSync>              ToList<TSync>( int loadOrderFilter = -1, bool tryLoad = true ) where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "ToList<TSync>()", "!IsValid" );
                return null;
            }
            if( typeof( TSync ) != _Association.ClassType )
            {
                DebugLog.WriteError( this.GetType().ToString(), "ToList<TSync>()", string.Format( "TSync does not match Form Type :: {0} != {1}", typeof( TSync ).ToString(), _Association.ClassType.ToString() ) );
                return null;
            }
            var syncList = ToList( loadOrderFilter, tryLoad );
            
            return syncList.NullOrEmpty()
                ? null
                : syncList.ConvertAll( (x) => ( x as TSync ) );
        }
        
        #endregion
        
        #region Group Loading
        
        protected bool                  LoadFromEx( Interface.IXHandle source, ElementHandle handle )
        {
            var m = GodObject.Windows.GetMainWindow();
            m.PushItemOfItems();
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "LoadFromEx()", source.ToString(), handle.ToString() } );
            var result = false;
            try
            {
                // Read from the specific handle for the source
                var records = handle.GetRecords( _Association.Signature, true );
                //DebugLog.WriteLine( string.Format( "{0} records found with signature \"{1}\"", ( records.NullOrEmpty() ? 0 : records.Length ), _Association.Signature ) );
                if( records.NullOrEmpty() )
                {
                    result = true;
                    DebugLog.WriteWarning( this.GetType().ToString(), "LoadFromEx()", string.Format( "No records found with signature \"{0}\"\nsource = {1}\nhandle = {2}\n{3}", _Association.Signature, source.ToString(), handle.ToString(), System.Environment.StackTrace ) );
                    goto localReturnResult;
                }
                
                var pForm = ParentForm;
                var max = records.Length;
                for( int i = 0; i < max; i++ )
                {
                    m.SetItemOfItems( i, max );
                    var record = records[ i ];
                    //DebugLog.OpenIndentLevel( new [] { "Loading record", record.ToString() } );
                    
                    bool consumedRecord = false;
                    
                    // Ignore records XeLib returns when it shouldn't
                    var rcHandle = record.GetContainerRecord();
                    if(
                        ( ( pForm == null )&&( rcHandle != null ) )||
                        ( ( pForm != null )&&( !pForm.IsHandleFor( rcHandle ) ) )
                    )   goto localSkipRecord;
                    
                    // Ignore overrides
                    if( record.IsOverride )
                        goto localSkipRecord;
                    
                    var recFID = record.FormID;
                    
                    // Try to find this form in the global data tree.
                    // Things like object references they may be moved to a different cell, or;
                    // A mod changed the form type;
                    // Either way we will need to update the container the form is in.
                    // Do NOT, however try to load it if it isn't in the master table, we're doing that now.
                    var syncObject = GodObject.Plugin.Data.Root.Find( record, false );
                    if( syncObject != null )
                    {
                        // Form already loaded
                        var oldCollection = syncObject.Collection;
                        if( oldCollection != this )
                        {
                            // Form was loaded into a different container
                            if( oldCollection != null) oldCollection.Remove( syncObject );
                            this.Add( syncObject );
                        }
                        // Skip this record
                        goto localSkipRecord;
                    }
                    
                    // Create a new data sync object of the appropriate type
                    syncObject = Activator.CreateInstance( _Association.ClassType, new Object[] { this, source, record } ) as IXHandle;
                    if( syncObject == null )
                    {
                        DebugLog.WriteError( this.GetType().ToString(), "LoadFromEx()", string.Format( "Unable to create {0} for record 0x{1}!", _Association.ClassType.ToString(), record.ToString() ) );
                        goto localSkipRecord;
                    }
                    consumedRecord = true;
                    
                    //DebugLog.WriteLine( "Created new Form Instance :: " + syncObject.ToString() );
                    
                    // Add this data sync object to the list
                    Add( syncObject );
                    
                    // Now sync form from source
                    if( !syncObject.Load() )
                    {
                        DebugLog.WriteError( this.GetType().ToString(), "LoadFromEx()", string.Format( "Load for {0} FormID 0x{1} returned false!", _Association.ClassType.ToString(), syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) ) );
                        goto localReturnResult;
                    }
                    
                localSkipRecord:
                    if( rcHandle != null ) rcHandle.Dispose();
                    if( !consumedRecord )
                        record.Dispose();
                    //DebugLog.CloseIndentLevel();
                }
                
            }
            catch( Exception e )
            {
                DebugLog.WriteError( this.GetType().ToString(), "LoadFromEx()", string.Format( "An exception occured during Load of {0}! :: Exception:\n{1}", this.ToString(), e.ToString() ) );
                goto localReturnResult;
            }
            
            result = true;
            
        localReturnResult:
            var tEnd = m.SyncTimerElapsed().Ticks - tStart.Ticks;
            m.StopSyncTimer( string.Format( "{0} :: LoadFromEx() :: Completed in {1}", this.GetType().ToString(), "{0}" ), tStart.Ticks );
            m.PopItemOfItems();
            DebugLog.CloseIndentLevel( tEnd, "result", result.ToString() );
            return result;
        }
        
        public bool                     LoadAllForms()
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "LoadAllForms()", "Collection !IsValid" );
                return false;
            }
            if( _FullLoadComplete ) return true;
            
            //DebugLog.OpenIndentLevel( new [] { "LoadAllForms()", string.Format( "ParentForm :: {0}", ( ParentForm == null ? "plugins" : ParentForm.ToString() ) ) } );
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingSigFormsFromAncestor".Translate(), _Association.Signature, ( ParentForm == null ? "Plugin.PluginFiles".Translate() : ParentForm.ExtraInfoFor() ) ) );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            var pForm = ParentForm;
            if( pForm == null )
            {
                foreach( var file in GodObject.Plugin.Data.Files.Loaded )
                {
                    //if( !LoadFrom( file ) ) goto localReturnResult;
                    LoadFrom( file ); // If the plugin doesn't contain a root form, this isn't an error
                }
            }
            else
            {
                var handles = pForm.Handles;
                foreach( var handle in handles )
                {
                    if( !LoadFromEx( pForm, handle ) ) goto localReturnResult;
                }
            }
            
            //DebugLog.WriteLine( new [] { this.GetType().ToString(), "LoadAllForms()", string.Format( "{0} \"{1}\" Forms loaded in parent container", ( _AllForms.NullOrEmpty() ? 0 : _AllForms.Count() ), _Association.Signature ) } );
            
            _FullLoadComplete = true;
            
        localReturnResult:
            m.StopSyncTimer( "Engine.Plugin.Container :: LoadAllForms() :: Completed in {0}", tStart.Ticks );
            m.PopStatusMessage();
            //DebugLog.CloseIndentLevel( _FullLoadComplete.ToString() );
            return _FullLoadComplete;
        }
        
        public virtual bool             LoadFrom( Interface.IXHandle source )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "LoadFrom()", "Collection !IsValid" );
                return false;
            }
            if( source == null )
            {
                DebugLog.WriteError( this.GetType().ToString(), "LoadFrom()", "source is null" );
                return false;
            }
            
            return _FullLoadComplete || LoadFromEx( source, source.WorkingFileHandle );
        }
        
        public bool                     PostLoad()
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( this.GetType().ToString(), "PostLoad()", "Collection !IsValid" );
                return false;
            }
            if( !_FullLoadComplete ) return false;
            if( _FullPostLoadComplete ) return true;
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.PushItemOfItems();
            m.SetCurrentStatusMessage( string.Format( "Plugin.PostLoadReferenceOfSig".Translate(), _Association.Signature ) );
            m.StartSyncTimer();
            var tStart = m.SyncTimerElapsed();
            
            if( !_AllForms.NullOrEmpty() )
            {
                var max = _AllForms.Count;
                for( int i = 0; i < max; i++ )
                {
                    m.SetItemOfItems( i, max );
                    var li = _AllForms[ i ];
                    // Now sync form
                    if( !li.PostLoad() )
                    {
                        DebugLog.WriteError( this.GetType().ToString(), "PostLoad()", string.Format( "{0} FormID 0x{1} returned false!", _Association.ClassType.ToString(), li.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" ) ) );
                        goto LocalAbort;
                    }
                }
            }
            
            _FullPostLoadComplete = true;
        LocalAbort:
            m.StopSyncTimer( "Engine.Plugin.Container :: PostLoad() :: Completed in {0}", tStart.Ticks );
            m.PopItemOfItems();
            m.PopStatusMessage();
            return _FullPostLoadComplete;
        }
        
        #endregion
        
    }
    
}
