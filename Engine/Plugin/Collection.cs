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
using Engine.Plugin.Extensions;


namespace Engine.Plugin
{
    
    public class Collection : IDisposable //, ICollection
    {

        // TODO:  GIVE THIS A LOCK AND MAKE IT RE-ENTRY SAFE WITH THE DATA ROOT LOCK!
        // IT CURRENTLY ENTERS THE DATA ROOT THROUGH THE PUBLIC LOCK METHODS WHICH WILL CAUSE RACE CONDITIONS IF CALLED FROM THE DATA ROOT!

        #region Container Data

        object                                  _CollectionLock         = new object();


        ClassAssociation                        _Association            = null;
        
        bool                                    _FullLoadComplete       = false;
        public bool                             FullLoadComplete        { get { return _FullLoadComplete; } }
        bool                                    _FullPostLoadComplete   = false;
        public bool                             FullPostLoadComplete    { get { return _FullPostLoadComplete; } }
        protected IXHandle                      _Ancestor               = null;
        
        protected List<IXHandle>                _AllForms               = null;
        protected Dictionary<uint, IXHandle>    _ByFormID               = null;
        protected Dictionary<string, IXHandle>  _ByEditorID             = null;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        void                            BuildCollection( ClassAssociation association, IXHandle ancestor )
        {
            if( association == null )
                throw new ArgumentException( string.Format( "{0} :: Unable to resolve Association", this.TypeFullName() ) );
            _Association = association;
            if( ancestor != null )
            {
                _Ancestor = ancestor;
                _Ancestor.AddCollection( this );
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
            lock( _CollectionLock )
            {
                if( Disposed )
                    return;
                Disposed = true;

                if( _ByFormID != null )
                    _ByFormID.Clear();
                _ByFormID = null;

                if( _ByEditorID != null )
                    _ByEditorID.Clear();
                _ByEditorID = null;

                if( !_AllForms.NullOrEmpty() )
                {
                    var c = _AllForms.Count - 1;
                    for( int i = c; i >= 0; i-- )
                    {
                        //Console.WriteLine( "Disposing of " + Association.Signature + " index " + i + " of " + c );
                        _AllForms[ i ].Dispose(); // <-- This sometimes triggers an exception when terminating, reason is hard to find as (a) it doesn't always happen and (b) there is no clear trigger for it
                    }
                    _AllForms.Clear();
                }
                _AllForms = null;

                _Association = null;
            }
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
            if( Disposed ) return false;
            lock( _CollectionLock )
                return AddEx( syncObject, false );
        }

        public virtual bool             AddEx( IXHandle syncObject, bool alreadyHoldsRootLock )
        {
            if( Disposed ) return false;

            //DebugLog.OpenIndentLevel( syncObject.IDString, true );

            bool result = false;

            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                goto localAbort;
            }
            if( syncObject == null )
            {
                DebugLog.WriteError( "syncObject is null" );
                goto localAbort;
            }
            if( !_Association.ClassType.IsInstanceOfType( syncObject ) )
            {
                DebugLog.WriteError( string.Format( "Invalid Association.ClassType for \"{0}\"! :: Expected {1} :: Got {2}", syncObject.Signature, syncObject.TypeFullName(), _Association.ClassType.FullName() ) );
                goto localAbort;
            }

            _AllForms = _AllForms ?? new List<IXHandle>();
            _AllForms.AddOnce( syncObject );

            var soFID = syncObject.GetFormID( Engine.Plugin.TargetHandle.Master );
            if( soFID.ValidFormID() )
            {
                _ByFormID = _ByFormID ?? new Dictionary<uint, IXHandle>();
                _ByFormID[ soFID ] = syncObject;
            }

            var soEDID = syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
            if( !string.IsNullOrEmpty( soEDID ) )
            {
                _ByEditorID = _ByEditorID ?? new Dictionary<string, IXHandle>();
                _ByEditorID[ soEDID.ToLower() ] = syncObject;
            }

            if( alreadyHoldsRootLock  )
                result = GodObject.Plugin.Data.Root.AddToMasterTableEx( syncObject );
            else
                result = GodObject.Plugin.Data.Root.AddToMasterTable( syncObject );

            //syncObject.Ancestor = _Ancestor; // Shouldn't need to do this now...?
        localAbort:
            //DebugLog.CloseIndentLevel();
            return result;
        }

        public virtual void             Remove( IXHandle syncObject )
        {
            if( Disposed ) return;
            lock( _CollectionLock )
                RemoveEx( syncObject, false );
        }

        public virtual void             RemoveEx( IXHandle syncObject, bool alreadyHoldsRootLock )
        {
            if( Disposed ) return;
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "!IsValid" );
                return;
            }
            if( syncObject == null )
            {
                DebugLog.WriteError( "syncObject is null" );
                return;
            }
            if( !_Association.ClassType.IsInstanceOfType( syncObject ) )
            {
                DebugLog.WriteError( string.Format( "Invalid Form Type! :: {0}", syncObject.TypeFullName() ) );
                return;
            }
            
            if( _AllForms != null )
                _AllForms.Remove( syncObject );

            var soFID = syncObject.GetFormID( Engine.Plugin.TargetHandle.Master );
            if( ( soFID.ValidFormID() )&&( _ByFormID != null ) )
                _ByFormID.Remove( soFID );
            
            var soEDID = syncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
            if( ( Engine.Plugin.Constant.ValidEditorID( soEDID ) )&&( _ByEditorID != null ) )
                _ByEditorID.Remove( soEDID.ToLower() );
            
            if( alreadyHoldsRootLock )
                GodObject.Plugin.Data.Root.RemoveFromMasterTableEx( syncObject );
            else
                GodObject.Plugin.Data.Root.RemoveFromMasterTable( syncObject );
            
            //syncObject.Ancestor = null; // Shouldn't need to do this now...?
        }

        bool                            TryFindInFormIDDictionaryEx( out IXHandle result, FormHandle handle, uint formid, bool tryLoad, bool alreadyHoldsRootLock )
        {
            result = null;
            _ByFormID = _ByFormID ?? new Dictionary<uint, IXHandle>();
            if( ( !_ByFormID.TryGetValue( formid, out result ) ) && ( tryLoad ) )
            {
                result = handle.IsValid()
                    ? TryLoadEx( handle, alreadyHoldsRootLock )
                    : TryLoadEx( formid, alreadyHoldsRootLock );
            }
            return result.IsValid();
        }

        bool                            TryFindInEditorIDDictionaryEx( out IXHandle result, FormHandle handle, string editorid, bool tryLoad, bool alreadyHoldsRootLock )
        {
            result = null;
            _ByEditorID = _ByEditorID ?? new Dictionary<string, IXHandle>();
            if( ( !_ByEditorID.TryGetValue( editorid, out result ) ) && ( tryLoad ) )
            {
                result = handle.IsValid()
                    ? TryLoadEx( handle, alreadyHoldsRootLock )
                    : TryLoadEx( editorid, alreadyHoldsRootLock );
            }
            return result.IsValid();
        }

        #endregion

        #region Add From Record, Create New

        public IXHandle                 AddFromRecord( IXHandle ancestor, ElementHandle handle )
        {
            lock( _CollectionLock )
                return AddFromRecordEx( ancestor, handle, false );
        }

        public IXHandle                 AddFromRecordEx( IXHandle ancestor, ElementHandle handle, bool alreadyHoldsRootLock )
        {
            //DebugLog.OpenIndentLevel( new [] { ancestor.NullSafeIDString(), handle.ToStringNullSafe() }, true );

            IXHandle result = null;
            
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                goto localAbort;
            }
            
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( string.Format( "Invalid handle 0x{0}", handle.ToString() ) );
                goto localAbort;
            }
            
            var rSig = handle.Signature;
            if( rSig != _Association.Signature )
            {
                DebugLog.WriteError( string.Format( "Invalid record signature!  :: Expected \"{0}\" got \"{1}\"", _Association.Signature, rSig ) );
                goto localAbort;
            }
            
            if( ( ancestor == null )&&( !_Association.AllowRootCollection ) )
            {
                DebugLog.WriteError( string.Format( "Class does not allow root collections! :: {0}",  _Association.ClassType.ToString() ) );
                goto localAbort;
            }
            
            result = Activator.CreateInstance( _Association.ClassType, new Object[] { this, ancestor, handle } ) as IXHandle; //IDataSync;
            if( result == null )
            {
                DebugLog.WriteError( string.Format( "Unable to create new {0}!", _Association.ClassType.ToString() ) );
                goto localAbort;
            }
            
            if( !result.Load() )
            {
                DebugLog.WriteError( string.Format( "Form {0} :: {1}.Load() returned false!", result.IDString, _Association.ClassType.FullName() ) );
                goto localAbort;
            }
            
            /* // Shouldn't need to do this now...?
            var formid = result.GetFormID( Engine.Plugin.TargetHandle.Master );
            foreach( var file in GodObject.Plugin.Data.Files.Loaded )
            {
                if( file.LoadOrder > result.LoadOrder )
                {
                    var hOverride = file.WorkingFileHandle.GetRecord( formid, false );
                    if( hOverride.IsValid() )
                    {
                        //syncObject.UpdateHandles( hOverride );
                        if( !result.Load() )
                        {
                            DebugLog.WriteError( this.FullTypeName(), "AddFromRecord()", string.Format( "Form {0} :: {1}.Load() returned false for override handle!", result.IDString, _Association.ClassType.FullName() ) );
                            goto localAbort;
                        }
                    }
                }
            }
            */
            
            if( !result.PostLoad() )
            {
                DebugLog.WriteError( string.Format( "Form {0} :: {1}.PostLoad() returned false!", result.IDString, _Association.ClassType.FullName() ) );
                goto localAbort;
            }
            
            AddEx( result, alreadyHoldsRootLock );
            
        localAbort:
            //DebugLog.CloseIndentLevel( "result", result );
            return result;
        }
        
        public TSync                    CreateNew<TSync>() where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return null;
            }
            if( typeof( TSync ) != _Association.ClassType )
            {
                DebugLog.WriteError( string.Format( "TSync does not match Form Type :: {0} != {1}", typeof( TSync ).ToString(), _Association.ClassType.ToString() ) );
                return null;
            }
            return CreateNew() as TSync;
        }
        
        public IXHandle                 CreateNew()
        {
            lock( _CollectionLock )
                return CreateNewEx( false );
        }

        IXHandle                        CreateNewEx( bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return null;
            }
            
            if( Ancestor == null )
                return CreateNewRootFormEx( alreadyHoldsRootLock );
            return ParentForm == null
                ? null
                : CreateNewChildFormEx( alreadyHoldsRootLock );
        }
        
        TSync                           CreateNewChildFormEx<TSync>( bool alreadyHoldsRootLock ) where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return null;
            }
            if( typeof( TSync ) != _Association.ClassType )
            {
                DebugLog.WriteError( string.Format( "TSync does not match Form Type :: {0} != {1}", typeof( TSync ).ToString(), _Association.ClassType.ToString() ) );
                return null;
            }
            return CreateNewChildFormEx( alreadyHoldsRootLock ) as TSync;
        }
        
        IXHandle                        CreateNewChildFormEx( bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return null;
            }
            var pForm = ParentForm;
            if( pForm == null )
            {
                DebugLog.WriteError( string.Format( "Form Type does not have a parent Form :: {0}", _Association.ClassType.ToString() ) );
                return null;
            }
            
            var wf = GodObject.Plugin.Data.Files.Working;
            var iiwf = pForm.CopyAsOverride(); //pForm.IsInWorkingFile( true );           // Am I in the working file?
            if( !iiwf.IsValid() )
            {
                DebugLog.WriteError( string.Format( "Unable to override parent \"{0}\" Form in {1} :: {2}", pForm.Signature, wf.Filename, pForm.IDString ) );
                return null;
            }
            
            var resHandle = pForm.WorkingFileHandle.AddElement<FormHandle>( _Association.Signature );
            if( !resHandle.IsValid() )
            {
                DebugLog.WriteError( string.Format( "Unable to create new child \"{0}\" Form in parent \"{1}\" Form in {2} :: {3}", _Association.Signature, pForm.Signature, wf.Filename, pForm.IDString ) );
                return null;
            }
            
            // Add the form to the container
            var resObject = AddFromRecordEx( pForm, resHandle, alreadyHoldsRootLock );
            if( resObject == null )
            {
                DebugLog.WriteError( string.Format( "Unable to add new child \"{0}\" Form to parent \"{1}\" Form in {2} :: {3}", _Association.Signature, pForm.Signature, wf.Filename, pForm.IDString ) );
                return null;
            }
            
            // Hand the new form back to the caller
            return resObject;
        }
        
        IXHandle                        CreateNewRootFormEx( bool alreadyHoldsRootLock )
        {
            var wf = GodObject.Plugin.Data.Files.Working;
            if( wf == null )
            {
                DebugLog.WriteError( "No working file loaded, unable to create/inject forms!" );
                return null;
            }
            var wfch = wf.WorkingFileHandle.GetElement<ElementHandle>( _Association.Signature );
            if( !wfch.IsValid() )
                wfch = wf.WorkingFileHandle.AddElement<ElementHandle>( _Association.Signature );
            if( !wfch.IsValid() )
            {
                DebugLog.WriteError( string.Format( "Unable to GetElement or AddElement() for Root Container \"{0}\" in {1}", _Association.Signature, wf.Filename ) );
                return null;
            }
            
            var handle = wfch.AddElement<FormHandle>( _Association.Signature );
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( string.Format( "Unable to AddElement() for a new Form in Root Container \"{0}\" in {1}", _Association.Signature, wf.Filename ) );
                return null;
            }
            
            // Add the form to the container
            var syncObject = AddFromRecordEx( wf, handle, alreadyHoldsRootLock );
            if( syncObject == null )
            {
                DebugLog.WriteError( string.Format( "Unable to add new root \"{0}\" for Form \"{1}\" in Root Container in {2}", _Association.ClassType.ToString(), _Association.Signature, wf.Filename ) );
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
                lock( _CollectionLock )
                    return _AllForms.NullOrEmpty()
                        ? 0
                        : _AllForms.Count;
            }
        }

        public IXHandle                 TryLoad( uint formid )
        {
            lock( _CollectionLock )
                return TryLoadEx( formid, false );
        }

        public IXHandle                 TryLoadEx( uint formid, bool alreadyHoldsRootLock )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "TryLoad()", "0x" + formid.ToString( "X8" ) } );
            IXHandle result = null;
            var record = Records.FindMasterRecordEx( ElementHandle.BaseXHandleValue, formid, false );
            if( !record.IsValid() )
            {
                DebugLog.WriteError( "Unable to get master record!" );
                goto localReturnResult;
            }
            result = TryLoadEx( record, alreadyHoldsRootLock );
        localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }

        public IXHandle                 TryLoad( FormHandle record )
        {
            lock( _CollectionLock )
                return TryLoadEx( record, false );
        }

        public IXHandle                 TryLoadEx( FormHandle record, bool alreadyHoldsRootLock )
        {
            //DebugLog.OpenIndentLevel( record.ToStringNullSafe(), true );

            IXHandle result = null;
            
            if( !record.IsValid() )
            {
                DebugLog.WriteError( "Invalid record!" );
                goto localReturnResult;
            }
            if( !record.IsMaster )
            {
                var masterRecord = record.GetMasterRecord();
                if( !masterRecord.IsValid() )
                {
                    DebugLog.WriteError( "Cannot get master record!" );
                    goto localReturnResult;
                }
                record = masterRecord;
            }
            var rSig = record.Signature;
            if( rSig != Association.Signature )
            {
                DebugLog.WriteError( string.Format( "Record Signature expected \"{0}\" got \"{1}\"", Association.Signature, rSig ) );
                goto localReturnResult;
            }
            var file = GodObject.Plugin.Data.Files.Find( record );
            if( file == null )
            {
                DebugLog.WriteError( "Unable to find containing file" );
                goto localReturnResult;
            }
            
            // Create a new data sync object (Form) of the appropriate type
            //result = AddFromRecord( file, record );
            var ancestor = _Ancestor;
            if( !ancestor.IsValid() )
                ancestor = file;
            result = AddFromRecordEx( ancestor, record, alreadyHoldsRootLock );
            if( result == null )
            {
                DebugLog.WriteError( string.Format( "Unable to create {0} in {1}", _Association.ClassType.ToString(), ancestor.ToString() ) );
                goto localReturnResult;
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IXHandle>( "result", result );
            return result;
        }

        public IXHandle                 TryLoad( string editorid )
        {
            lock( _CollectionLock )
                return TryLoadEx( editorid, false );
        }

        public IXHandle                 TryLoadEx( string editorid, bool alreadyHoldsRootLock )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "TryLoad()", editorid } );
            IXHandle result = null;
            if( ParentForm != null )
            {   // Child form in parent (ie, REFR in a CELL, CELL in a WRLD, etc)
                var handles = ParentForm.Handles;
                foreach( var handle in handles )
                {
                    result = TryLoadEx( editorid, handle, alreadyHoldsRootLock );
                    if( result != null )
                        goto localReturnResult;
                }
            }
            else
            {   // Root form in file (ie, STAT, ACTI, etc)
                foreach( var file in GodObject.Plugin.Data.Files.Loaded )
                {
                    result = TryLoadEx( editorid, file.MasterHandle, alreadyHoldsRootLock );
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
            lock( _CollectionLock )
                return TryLoadEx( editorid, source, false );
        }

        IXHandle                        TryLoadEx( string editorid, ElementHandle source, bool alreadyHoldsRootLock )
        {
            //DebugLog.OpenIndentLevel( new [] { editorid, source.ToStringNullSafe() }, true );

            IXHandle result = null;
            
            if( string.IsNullOrEmpty( editorid ) )
                goto localReturnResult;
            
            var records = source.GetRecords( _Association.Signature, false );
            if( records.NullOrEmpty() )
                goto localReturnResult;
            
            for( int i = 0; i < records.Length; i++ )
            {
                var record = records[ i ];
                var recEDID = record.EditorID;
                if( ( !string.IsNullOrEmpty( recEDID ) )&&( recEDID.InsensitiveInvariantMatch( editorid ) ) )
                {
                    result = TryLoadEx( record, alreadyHoldsRootLock );
                    if( ( result == null )||( !result.IsHandleFor( record ) ) )
                        record.Dispose();
                    for( int j = i + 1; j < records.Length; j++ )
                        records[ j ].Dispose(); // <<-- Dispose of the rest of the handles in the array
                    goto localReturnResult;
                }
                record.Dispose();
            }
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }

        public IXHandle                 FindEx( ClassAssociation targetAssociation, FormHandle handle = null, uint formid = 0, string editorid = null, bool tryLoad = true, bool alreadyHoldsRootLock = false )
        {
            return FindExEx( targetAssociation, handle, formid, editorid, tryLoad, alreadyHoldsRootLock, false );
        }

        public IXHandle                 FindExEx( ClassAssociation targetAssociation, FormHandle handle = null, uint formid = 0, string editorid = null, bool tryLoad = true, bool alreadyHoldsRootLock = false, bool alreadyHoldsCollectionLock = false )
        {
            //DebugLog.OpenIndentLevel( new [] { targetAssociation.ToStringNullSafe(), handle.ToStringNullSafe(), string.Format( "IXHandle.IDString".Translate(), formid.ToString( "X8" ), editorid ), tryLoad.ToString() }, true );

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
                DebugLog.WriteError( "No FormID or EditorID to search for" );
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
                DebugLog.WriteError( string.Format( "targetAssociation is invalid! :: Expected \"{0}\" or child, got \"{1}\"", Association.Signature, ( targetAssociation == null ? "null" : targetAssociation.Signature ) ) );
                goto localReturnResult;
            }
            
            if( searchInThis )
            {
                if( alreadyHoldsCollectionLock )
                {
                    if( ( searchByFormID   )&&( TryFindInFormIDDictionaryEx(   out result, handle, formid  , tryLoad, alreadyHoldsRootLock ) ) )
                        goto localReturnResult;
                    if( ( searchByEditorID )&&( TryFindInEditorIDDictionaryEx( out result, handle, editorid, tryLoad, alreadyHoldsRootLock ) ) )
                        goto localReturnResult;
                }
                else
                {
                    lock( _CollectionLock )
                    {
                        if( ( searchByFormID   )&&( TryFindInFormIDDictionaryEx(   out result, handle, formid  , tryLoad, alreadyHoldsRootLock ) ) )
                            goto localReturnResult;
                        if( ( searchByEditorID )&&( TryFindInEditorIDDictionaryEx( out result, handle, editorid, tryLoad, alreadyHoldsRootLock ) ) )
                            goto localReturnResult;
                    }
                }
            }
            if( searchInChildren )
            {
                List<IXHandle> allForms = null;
                if( alreadyHoldsCollectionLock )
                    allForms = ToListExEx( -1, tryLoad, true, alreadyHoldsRootLock );
                else
                    allForms = ToListEx( tryLoad: tryLoad, alreadyHoldsRootLock: alreadyHoldsRootLock );
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
                                // Make sure we use the locking mechanism of the child collection
                                result = cCollection.FindEx( targetAssociation, handle, formid, editorid, tryLoad, alreadyHoldsRootLock );
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
            lock( _CollectionLock )
                return FindExEx( handle, tryLoad, false );
        }

        public IXHandle                 FindEx( XeLib.FormHandle handle, bool tryLoad = true, bool alreadyHoldsRootLock = false )
        {
            lock( _CollectionLock )
                return FindExEx( handle, tryLoad, alreadyHoldsRootLock );
        }

        public IXHandle                 FindExEx( XeLib.FormHandle handle, bool tryLoad = true, bool alreadyHoldsRootLock = false )
        {
            //DebugLog.OpenIndentLevel( new [] { handle.ToStringNullSafe(), tryLoad.ToString() }, true );
            IXHandle result = null;
            
            if( !handle.IsValid() )
            {
                DebugLog.WriteError( "handle !IsValid()" );
                goto localReturnResult;
            }
            
            var association = Reflection.AssociationFrom( handle.Signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( "Unable to get Association for collection" );
                goto localReturnResult;
            }
            
            // Get parent container from handle, work through
            // ancestory back to the root container and build
            // the tree as needed to this point.
            //handle.DumpContainerTree();
            
            // Build the tree of forms
            var refTree = handle.GetContainerRecordTree();

            List<IXHandle> forms = null;
            // Load the form tree
            if( alreadyHoldsRootLock )
                forms = GodObject.Plugin.Data.Root.LoadRecordTreeEx( refTree, true, this );
            else
                forms = GodObject.Plugin.Data.Root.LoadRecordTree( refTree, true );
            
            result = forms.NullOrEmpty()
                ? null
                : forms.Last(); // Last form is the one we want
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string signature, uint formid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Find()", signature, "0x" + formid.ToString( "X8" ), tryLoad.ToString() } );
            
            IXHandle result = null;
            
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( string.Format( "ClassAssociation[ \"{0}\" ] :: !IsValid()", signature ) );
                goto localReturnResult;
            }
            result = FindEx( association, null, formid, null, tryLoad );
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string signature, string editorid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Find()", signature, editorid, tryLoad.ToString() } );
            
            IXHandle result = null;
            
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( signature );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( string.Format( "ClassAssociation[ \"{0}\" ] :: !IsValid()", signature ) );
                goto localReturnResult;
            }
            result = FindEx( association, null, 0, editorid, tryLoad );
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public IXHandle                 Find( uint formid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Find()", "0x" + formid.ToString( "X8" ), tryLoad.ToString() } );
            
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
            //DebugLog.OpenIndentLevel( string.Format( "{0} :: Find<{1}>() :: 0x{2} :: {3}\n{{", this.FullTypeName(), typeof( TSync ).ToString(), formid.ToString( "X8" ), tryLoad ) );
            
            TSync result = null;
            
            var type = typeof( TSync );
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( string.Format( "ClassAssociation[ {0} ] :: !IsValid()", ( type == null ? "null" : type.ToString() ) ) );
                goto localReturnResult;
            }
            result = FindEx( null, null, formid, null, tryLoad ) as TSync;
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<TSync>( result );
            return result;
        }
        
        public IXHandle                 Find( string editorid, bool tryLoad = true )
        {
            //DebugLog.OpenIndentLevel( new [] { this.FullTypeName(), "Find()", editorid, tryLoad.ToString() } );
            
            var result = FindEx( null, null, 0, editorid, tryLoad );
            
       //localReturnResult:
            //DebugLog.CloseIndentLevel<IDataSync>( result );
            return result;
        }
        
        public TSync                    Find<TSync>( string editorid, bool tryLoad = true ) where TSync : class, IXHandle
        {
            //DebugLog.OpenIndentLevel( string.Format( "{0} :: Find<{1}>() :: \"{2}\" :: {3}\n{{", this.FullTypeName(), typeof( TSync ).ToString(), editorid, tryLoad ) );
            
            TSync result = null;
            
            var type = typeof( TSync );
            var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
            if( !association.IsValid() )
            {
                DebugLog.WriteError( string.Format( "ClassAssociation[ {0} ] :: !IsValid()", ( type == null ? "null" : type.ToString() ) ) );
                goto localReturnResult;
            }
            result =  FindEx( null, null, 0, editorid, tryLoad ) as TSync;
            
       localReturnResult:
            //DebugLog.CloseIndentLevel<TSync>( result );
            return result;
        }

        public List<IXHandle>           ToList( int loadOrderFilter = -1, bool tryLoad = true )
        {
            lock( _CollectionLock )
                return ToListExEx( loadOrderFilter, tryLoad, true, false );
        }
        
        public List<IXHandle>           ToListEx( int loadOrderFilter = -1, bool tryLoad = true, bool alreadyHoldsRootLock = false )
        {
            lock( _CollectionLock )
                return ToListExEx( loadOrderFilter, tryLoad, true, alreadyHoldsRootLock );
        }

        public List<IXHandle>           ToListExEx( int loadOrderFilter, bool tryLoad, bool updateUIOnLoad, bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return null;
            }
            if( !_FullLoadComplete )
            {
                if( !tryLoad )
                {
                    DebugLog.WriteError( "Must call LoadAllForms() first!" );
                    return null;
                }
                if( !LoadAllFormsEx( updateUIOnLoad, alreadyHoldsRootLock  ) )
                {
                    DebugLog.WriteError( "Unable to LoadAllFormsEx()!" );
                    return null;
                }
            }
            if( !_FullPostLoadComplete )
            {
                if( !tryLoad )
                {
                    DebugLog.WriteError( "Must call PostLoad() first!" );
                    return null;
                }
                if( !PostLoadEx( updateUIOnLoad, alreadyHoldsRootLock ) )
                {
                    DebugLog.WriteError( "Unable to PostLoadEx()!" );
                    return null;
                }
            }
            if( _AllForms.NullOrEmpty() )
            {
                //DebugLog.Write( FormatLogMessage( "ToListExEx()", "_AllForms is null or empty" ) );
                return null;    // Not an error
            }
            
            return ( loadOrderFilter >=0 )&&( loadOrderFilter <= GodObject.Plugin.Data.Files.Working.LoadOrder )
                ? _AllForms.Where( (x) => ( x.LoadOrder == loadOrderFilter ) ).ToList()
                : _AllForms.Clone();
        }

        public List<TSync>              ToList<TSync>( int loadOrderFilter = -1, bool tryLoad = true ) where TSync : class, IXHandle
        {
            lock( _CollectionLock )
                return ToListEx<TSync>( loadOrderFilter, tryLoad, false );
        }

        public List<TSync>              ToListEx<TSync>( int loadOrderFilter = -1, bool tryLoad = true, bool alreadyHoldsRootLock = false ) where TSync : class, IXHandle
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "!IsValid" );
                return null;
            }
            if( !_Association.ClassType.IsClassOrSubClassOf( typeof( TSync ) )  )
            {
                DebugLog.WriteError( string.Format( "TSync is not a valid cast from Form Type :: Collection Form Type = {0} :: TSync = {1}", _Association.ClassType.ToString(), typeof( TSync ).ToString() ) );
                return null;
            }
            var syncList = ToListExEx( loadOrderFilter, tryLoad, true, alreadyHoldsRootLock );
            
            return syncList.NullOrEmpty()
                ? null
                : syncList.ConvertAll( (x) => ( x as TSync ) );
        }

        #endregion

        #region Group Loading

        protected bool                  LoadFromEx( Interface.IXHandle source, ElementHandle handle, bool updateUI )
        {
            lock( _CollectionLock )
                return LoadFromExEx( source, handle, updateUI, false );
        }

        bool                            LoadFromExEx( Interface.IXHandle source, ElementHandle handle, bool updateUI, bool alreadyHoldsRootLock )
        {
            TimeSpan tStart = default;
            GUIBuilder.Windows.Main m = null;
            if( updateUI )
            {
                m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushItemOfItems();
                m.StartSyncTimer();
                tStart = m.SyncTimerElapsed();
            }
            
            //DebugLog.OpenIndentLevel( new [] { "source = " + ( source == null ? "[null]" : source.IDString ), "handle = " + handle.ToStringNullSafe(), "alreadyHoldsRootLock = " + alreadyHoldsRootLock.ToString() }, true, true, false );
            var result = false;
            try
            {
                // Read from the specific handle for the source
                var records = handle.GetRecords( _Association.Signature, true );
                //DebugLog.WriteLine( string.Format( "{0} records found with signature \"{1}\"", ( records.NullOrEmpty() ? 0 : records.Length ), _Association.Signature ) );
                if( records.NullOrEmpty() )
                {
                    result = true;
                    //DebugLog.WriteWarning( this.FullTypeName(), "LoadFromExEx()", string.Format( "No records found with signature \"{0}\"\nsource = {1}\nhandle = {2}\n{3}", _Association.Signature, source.ToString(), handle.ToString(), System.Environment.StackTrace ) );
                    goto localReturnResult;
                }
                
                var pForm = ParentForm;
                var max = records.Length;
                for( int i = 0; i < max; i++ )
                {
                    if( updateUI )
                        m.SetItemOfItems( i, max );

                    var record = records[ i ];
                    //DebugLog.OpenIndentLevel( new [] { "Loading record", record.ToString() } );
                    
                    bool consumedRecord = false;
                    
                    // Ignore records XeLib returns that it shouldn't
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
                    // A Form was loaded "early" (CoreForms and CustomForms), or;
                    // A mod changed the form type;
                    // Either way we will need to update the container the form is in.
                    IXHandle syncObject = null;
                    if( alreadyHoldsRootLock )
                        syncObject = GodObject.Plugin.Data.Root.FindEx( record, false );
                    else
                        syncObject = GodObject.Plugin.Data.Root.Find( record, false );
                    if( syncObject != null )
                    {
                        //var s = record.Signature + " 0x" + recFID.ToString( "X8" ) + " already found in GodObject.Plugin.Data.Root\n" + record.ToString();
                        //DebugLog.WriteWarning( s );

                        // Form already loaded
                        var oldCollection = syncObject.ParentCollection;
                        
                        // Form was loaded into a different container
                        if( ( oldCollection != this )&&( oldCollection != null ) )
                            oldCollection.Remove( syncObject );

                        // Make sure it's in this container
                        this.Add( syncObject );
                        
                        // Skip this record
                        goto localSkipRecord;
                    }
                    
                    // Create a new data sync object of the appropriate type
                    syncObject = Activator.CreateInstance( _Association.ClassType, new Object[] { this, source, record } ) as IXHandle;
                    if( syncObject == null )
                    {
                        DebugLog.WriteError( string.Format( "Unable to create {0} for record 0x{1}!", _Association.ClassType.ToString(), record.ToString() ) );
                        goto localSkipRecord;
                    }
                    consumedRecord = true;
                    
                    //DebugLog.WriteLine( "Created new Form Instance :: " + syncObject.ToString() );
                    
                    // Add this data sync object to the list
                    if( !AddEx( syncObject, alreadyHoldsRootLock ) )
                    {
                        DebugLog.WriteError( string.Format( "Unable to add {0} for record 0x{1} to dictionaries!", _Association.ClassType.ToString(), record.ToString() ) );
                        goto localSkipRecord;
                    }

                    // Now sync form from source
                    if( !syncObject.Load() )
                    {
                        DebugLog.WriteError( string.Format( "Load for {0} Form {1} returned false!", _Association.ClassType.ToString(), syncObject.IDString ) );
                        goto localReturnResult;
                    }
                    
                localSkipRecord:
                    if( rcHandle != null ) rcHandle.Dispose();
                    if( !consumedRecord  ) record  .Dispose();
                    //DebugLog.CloseIndentLevel();
                }
                
            }
            catch( Exception e )
            {
                DebugLog.WriteException( e );
                //DebugLog.WriteError( string.Format( "An exception occured during Load of {0}! :: Exception:\n{1}", this.ToString(), e.ToString() ) );
                goto localReturnResult;
            }
            
            result = true;

        localReturnResult:
            long elapsed = -1;
            if( updateUI )
            {
                elapsed = m.StopSyncTimer( tStart );
                m.PopItemOfItems();
            }
            //DebugLog.CloseIndentLevel( "result", result.ToString() );
            return result;
        }

        public bool                     LoadAllForms( bool updateUI )
        {
            lock( _CollectionLock )
                return LoadAllFormsEx( updateUI, false );
        }

        public bool                     LoadAllFormsEx( bool updateUI, bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return false;
            }
            if( _FullLoadComplete ) return true;

            //DebugLog.OpenIndentLevel( new [] { "LoadAllFormsEx()", string.Format( "ParentForm :: {0}", ( ParentForm == null ? "plugins" : ParentForm.ToString() ) ) } );

            TimeSpan tStart = default;
            GUIBuilder.Windows.Main m = null;

            if( updateUI )
            {
                m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingSigFormsFromAncestor".Translate(), _Association.Signature, ( ParentForm == null ? "Plugin.PluginFiles".Translate() : ParentForm.ExtraInfoFor() ) ) );
                m.StartSyncTimer();
                tStart = m.SyncTimerElapsed();
            }
            
            var pForm = ParentForm;
            if( pForm == null )
            {
                foreach( var file in GodObject.Plugin.Data.Files.Loaded )
                {
                    //if( !LoadFrom( file ) ) goto localReturnResult;
                    LoadFromEx( file, updateUI, alreadyHoldsRootLock ); // If the plugin doesn't contain a root form, this isn't an error
                }
            }
            else
            {
                var handles = pForm.Handles;
                foreach( var handle in handles )
                {
                    if( !LoadFromExEx( pForm, handle, updateUI, alreadyHoldsRootLock ) ) goto localReturnResult;
                }
            }
            
            //DebugLog.WriteLine( new [] { this.FullTypeName(), "LoadAllForms()", string.Format( "{0} \"{1}\" Forms loaded in parent container", ( _AllForms.NullOrEmpty() ? 0 : _AllForms.Count() ), _Association.Signature ) } );
            
            _FullLoadComplete = true;

        localReturnResult:
            long elapsed = -1;
            if( updateUI )
            {
                elapsed = m.StopSyncTimer( tStart );
                m.PopStatusMessage();
            }
            //DebugLog.CloseIndentLevel( _FullLoadComplete.ToString() );
            return _FullLoadComplete;
        }

        public virtual bool             LoadFrom( Interface.IXHandle source, bool updateUI )
        {
            lock( _CollectionLock )
                return LoadFromEx( source, updateUI, false );
        }

        bool                            LoadFromEx( Interface.IXHandle source, bool updateUI, bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return false;
            }
            if( source == null )
            {
                DebugLog.WriteError( "source is null" );
                return false;
            }
            
            return _FullLoadComplete || LoadFromExEx( source, source.MasterHandle, updateUI, alreadyHoldsRootLock );
        }

        public bool                     PostLoad( bool updateUI )
        {
            lock( _CollectionLock )
                return PostLoadEx( updateUI, false );
        }

        public bool                     PostLoadEx( bool updateUI, bool alreadyHoldsRootLock )
        {
            if( !this.IsValid() )
            {
                DebugLog.WriteError( "Collection !IsValid" );
                return false;
            }
            if( !_FullLoadComplete ) return false;
            if( _FullPostLoadComplete ) return true;

            TimeSpan tStart = default;
            GUIBuilder.Windows.Main m = null;

            if( updateUI )
            {
                m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                m.PushStatusMessage();
                m.PushItemOfItems();
                m.SetCurrentStatusMessage( string.Format( "Plugin.PostLoadReferenceOfSig".Translate(), _Association.Signature ) );
                m.StartSyncTimer();
                tStart = m.SyncTimerElapsed();
            }
            
            if( !_AllForms.NullOrEmpty() )
            {
                var max = _AllForms.Count;
                for( int i = 0; i < max; i++ )
                {
                    if( updateUI )
                        m.SetItemOfItems( i, max );
                    var li = _AllForms[ i ];
                    // Now sync form
                    if( !li.PostLoad() )
                    {
                        DebugLog.WriteError( string.Format( "{0} Form {1} returned false!", _Association.ClassType.ToString(), li.IDString ) );
                        goto LocalAbort;
                    }
                }
            }
            
            _FullPostLoadComplete = true;
        LocalAbort:
            long elapsed = -1;
            if( updateUI )
            {
                elapsed = m.StopSyncTimer( tStart );
                m.PopItemOfItems();
                m.PopStatusMessage();
            }
            return _FullPostLoadComplete;
        }
        
        #endregion
        
    }
    
}
