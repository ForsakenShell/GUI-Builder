/*
 * Form.cs
 * 
 * Abstraction layer for plugin forms, all forms use this as their base class.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

using XeLib;
using XeLib.API;

using Engine.Plugin.Attributes;
using Engine.Plugin.Interface;


namespace Engine.Plugin
{
    
    public abstract class Form : IDisposable, IXHandle, ISyncedGUIObject, GUIBuilder.IMouseOver //, IDataSync
    {
        
        #region Common Fallout 4 Form fields
        
        Forms.Fields.Record.FormID      _FormID;
        Forms.Fields.Record.EditorID    _EditorID;
        Forms.Fields.Record.Flags       _RecordFlags;
        
        #endregion
        
        #region Meta data
        
        IXHandle                        _Ancestor                   = null; // Ancestral form (eg, a CELL for a REFR form)
        Collection                      _ParentCollection           = null;
        List<ElementHandle>             _Handles                    = null;
        int                             _LastFullRequiredHandleIndex = -1;
        int                             _LastFullOptionalHandleIndex = -1;
        int                             _WorkingFileHandleIndex     = -1;
        
        List<PapyrusScript>             _Scripts                    = null;
        
        List<Collection>                _ChildCollections           = null;
        
        string                          _Forced_Filename            = null;
        uint                            _Forced_FormID              = Constant.FormID_Invalid;
        
        #endregion
        
        #region Allocation & Disposal
        
        #region Allocation
        
        protected                       Form( string filename, uint formID )
        {
            if( Association == null )
                throw new Exception( string.Format( "Cannot get Association for {0}", this.GetType() ) );
            if( ( string.IsNullOrEmpty( Signature ) )||( Signature.Length != 4 ) )
                throw new ArgumentException( string.Format( "{1} :: Invalid Signature :: \"{0}\"", Signature, this.TypeFullName() ) );
            _Forced_Filename = filename;
            _Forced_FormID = formID & 0x00FFFFFF;
            CreateFields();
        }
        
        //protected Form( ICollection collection, IDataSync ancestor, RecordHandle handle )
        protected                       Form( Collection parentCollection, IXHandle ancestor, FormHandle handle )
        {
            bool throwError = false;
            string errorString = "";
            if( Association == null )
            {
                throwError = true;
                errorString += string.Format( "\tCannot get Association for {0}\n", this.GetType() );
            }
            if( !ancestor.IsValid() )
            {
                throwError = true;
                errorString += "\tAncestor must be valid\n";
            }
            else
            {
                if( ( ancestor as File ) == null )
                {
                    var aAssociation = Reflection.AssociationFrom( ancestor.Signature );
                    var aCanHaveMe = aAssociation.HasChildOrGrandchildAssociationOf( Association, false );
                    if( !aCanHaveMe )
                    {
                        throwError = true;
                        errorString += "\tAncestor does not have an association for this Form type\n";
                    }
                }
                else if( !Association.AllowRootCollection )
                {
                    throwError = true;
                    errorString += "\tForm type cannot be in a root (file) level container\n";
                }
            }
            if( ( string.IsNullOrEmpty( Signature ) )||( Signature.Length != 4 ) )
            {
                throwError = true;
                errorString += "\tInvalid Signature\n";
            }
            if( parentCollection == null )
            {
                throwError = true;
                errorString += "\tparentCollection cannot be null\n";
            }
            if( !handle.IsValid() )
            {
                throwError = true;
                errorString += "\tInvalid Handle\n";
            }
            var rSig = handle.Signature;
            if( rSig != Signature )
            {
                throwError = true;
                errorString += "\tRecord signature does not match Form signature\n";
            }
            var isMaster = handle.IsMaster;
            if( !isMaster )
            {
                throwError = true;
                errorString += "\tHandle must be the master record\n";
            }
            if( throwError )
            {
                var file = GodObject.Plugin.Data.Files.Find( handle );
                uint rFID = Constant.FormID_Invalid;
                string rPath = string.Empty;
                try
                {
                    rFID = handle.FormID;
                }
                catch {}
                try
                {
                    rPath = handle.LongPath;
                }
                catch {}
                errorString= string.Format(
                    "{0}\tHandle = 0x{1}\n\tPath = \"{7}\"\n\tHandle File = \"{2}\"\n\tForm Signature = \"{3}\"\n\tRecord Signature = \"{4}\"\n\tFormID = 0x{5}\n\tIsMaster = {6}\n\tAncestor = {8}",
                    errorString,
                    handle.ToString(),
                    ( file == null ? "Unresolvable" : file.Filename ),
                    Signature, rSig,
                    rFID.ToString( "X8" ),
                    isMaster,
                    rPath,
                    ancestor.ToStringNullSafe()
                   );
                DebugLog.WriteError( errorString );
                throw new ArgumentNullException( this.TypeFullName(), errorString );
            }
            _ParentCollection = parentCollection;
            _Ancestor = ancestor;
            CreateFields();
            GetHandles( handle );
        }
        
        void                            CreateFields()
        {
            _FormID = new Forms.Fields.Record.FormID( this );
            _EditorID = new Forms.Fields.Record.EditorID( this );
            _RecordFlags = new Forms.Fields.Record.Flags( this );
            
            // Create child containers (if any) for this form
            
            var association = Association;
            //Attributes.AssociationExtensions.Dump( associations, "Creating form for:" );
            if( association.HasChildCollections )
            {
                _ChildCollections = new List<Collection>();
                foreach( var childContainerType in association.ChildTypes )
                {
                    var childAssociation = Plugin.Attributes.Reflection.AssociationFrom( childContainerType );
                    if( childAssociation == null )
                        throw new Exception( string.Format( "Unable to get Association for child Collection Form Type {0}", ( childContainerType == null ? "null" : childContainerType.ToString() ) ) );
                    //Attributes.AssociationExtensions.Dump( childAttributes, "Creating child containers for:" );
                    var childContainer = Activator.CreateInstance( childAssociation.CollectionClassType, new object[]{ childAssociation, this } ) as Collection;
                    if( childContainer == null )
                        throw new Exception( string.Format( "Unable to create Child Container {0} for Form Type {1}", ( childAssociation.CollectionClassType == null ? "null" : childAssociation.CollectionClassType.ToString() ), ( childContainerType == null ? "null" : childContainerType.ToString() ) ) );
                    _ChildCollections.Add( childContainer );
                }
            }
            
            CreateChildFields();
        }
        
        public virtual void             CreateChildFields() {}
        
        #endregion
        
        #region Disposal
        
        protected bool                  Disposed = false;
        
                                        ~Form()
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
            Disposed = true;

            if( !_ChildCollections.NullOrEmpty() )
                foreach( var collection in _ChildCollections )
                    collection.Dispose();
            _ChildCollections = null;

            if( _ParentCollection != null )
                _ParentCollection.Remove( this );
            _ParentCollection = null;

            if( !_Handles.NullOrEmpty() )
            {
                ElementHandle.ReleaseHandles( _Handles );
               _Handles.Clear();
               _Handles = null;
            }
            _LastFullRequiredHandleIndex = -1;
            _WorkingFileHandleIndex = -1;
            
            _Ancestor       = null;

            _References     = null;

        }
        
        #endregion
        
        #endregion
        
        #region IXHandle
        
        public override int             GetHashCode()
        {
            return GetFormID( Engine.Plugin.TargetHandle.Master ).GetHashCode() ^ Signature.GetHashCode();
        }

        public string                   IDString                    { get { return string.Format( "IXHandle.IDString".Translate(), GetFormID( TargetHandle.Master ).ToString( "X8" ), GetEditorID( TargetHandle.LastValid ) ); } }

        public override string          ToString()
        {
            if( this == null )
                return "[null]";
            if( Disposed )
                return "[disposed]";
            var or = WorkingFileHandle;
            string strExtra = or.IsValid() ? or.ToStringExtra() : null;
            string strScripts = null;
            if( !_Scripts.NullOrEmpty() )
            {
                strScripts = "[";
                for( int i = 0; i < _Scripts.Count; i++ )
                {
                    var strS = _Scripts[ i ].ToStringNullSafe();
                    if( i > 0 )
                        strScripts += ", ";
                    strScripts += strS;
                }
                strScripts += "]";
            }
            string strHandles = null;
            if( !_Handles.NullOrEmpty() )
            {
                strHandles = "[";
                for( int i = 0; i < _Handles.Count; i++ )
                {
                    var strH = _Handles[ i ].ToStringNullSafe();
                    if( i > 0 )
                        strHandles += ", ";
                    strHandles += strH;
                }
                strHandles += "]";
            }
            var str = string.Format(
                "[{0}typeof( Form ) = {1}{2}{3}]",
                ( strExtra == null ? null : string.Format( "{0} :: ", strExtra ) ),
                this.TypeFullName(),
                ( strScripts == null ? null : string.Format( " :: scripts = {0}", strScripts ) ),
                ( strHandles == null ? null : string.Format( " :: handles = {0}", strHandles ) )
            );
            return str;
        }
        
        #region Required Properties
        
        public string                   Signature
        {
            get
            {
                var association = Association;
                return association == null ? null : association.Signature;
            }
        }
        
        public virtual IXHandle         Ancestor
        {
            get { return _Ancestor; }
            set { _Ancestor = value; }
        }
        
        public File[]                   Files
        {
            get
            {
                var fCount = _Handles.Count;
                var files = new File[ fCount ];
                for( int i = 0; i < fCount; i++ )
                    files[ i ] = GodObject.Plugin.Data.Files.Find( _Handles[ i ] );
                return files;
            }
        }

        public uint                     ForcedFormID            { get { return _Forced_FormID; } }

        public string                   ForcedFilename          { get { return _Forced_Filename; } }
        
        public string[]                 Filenames
        {
            get
            {
                var fCount = _Handles.Count;
                var filenames = new string[ fCount ];
                for( int i = 0; i < fCount; i++ )
                    filenames[ i ] = _Handles[ i ].Filename;
                return filenames;
            }
        }

        public string                   GetFilename( TargetHandle target )
        {
            var fh = Engine.Plugin.Extensions.TargetHandleExtensions.HandleFromTarget( this, target ) as FormHandle;
            if( !fh.IsValid() )
                throw new ArgumentException( "Cannot get Filename for Form" );
            return fh.Filename;
        }

        public uint                     LoadOrder               { get { return GetFormID( Engine.Plugin.TargetHandle.Master ) >> 24; } }
        
        /*
        public uint                     FormID                  { get { return _FormID.GetValue( TargetHandle.Master ); } }
        
        public string                   EditorID
        {
            get { return _EditorID.GetValue( TargetHandle.Working ); }
            set { _EditorID.SetValue( TargetHandle.Working, value ); }
        }
        */
        
        public uint                     GetFormID( TargetHandle target )
        {
            return _FormID.GetValue( target );
        }
        public void                     SetFormID( TargetHandle target, uint value )
        {
            _FormID.SetValue( target, value );
        }
        
        public string                   GetEditorID( TargetHandle target )
        {
            return _EditorID.GetValue( target );
        }
        public void                     SetEditorID( TargetHandle target, string value )
        {
            _EditorID.SetValue( target, value );
        }
        
        public Forms.Fields.Record.Flags RecordFlags                 { get { return _RecordFlags; } }
        
        //public Engine.Plugin.Forms.Fields.Record.FormID FormID  { get { return _FormID; } }
        
        //public Engine.Plugin.Forms.Fields.Record.EditorID EditorID  { get { return _EditorID; } }
        
        //public Engine.Plugin.Forms.Fields.Record.Flags RecordFlags { get { return _RecordFlags; } }
        
        public virtual ConflictStatus   ConflictStatus
        {
            get
            {
                if( _Handles.NullOrEmpty() ) return Engine.Plugin.ConflictStatus.Invalid;
                /*
                var oh = WorkingFileHandle;
                if( !oh.IsValid() )
                    return Engine.Plugin.ConflictStatus.Invalid;
                var isMaster = oh.IsMaster;
                var isInWorkingFile = Mod == GodObject.Plugin.Data.Files.Working;
                */
                var isInWorkingFile = _WorkingFileHandleIndex > -1;
                return
                    isInWorkingFile
                        ? WorkingFileHandle.IsMaster
                            ? Engine.Plugin.ConflictStatus.NewForm
                            : _WorkingFileHandleIndex == _Handles.Count - 1
                                ? Engine.Plugin.ConflictStatus.OverrideInWorkingFile
                                : Engine.Plugin.ConflictStatus.OverrideInPostLoad
                        : _Handles.Count == 1
                            ? Engine.Plugin.ConflictStatus.NoConflict
                            : _Handles.LastOrDefault().LoadOrder > GodObject.Plugin.Data.Files.Working.LoadOrder
                                ? Engine.Plugin.ConflictStatus.OverrideInPostLoad
                                : Engine.Plugin.ConflictStatus.OverrideInAncestor;
                /*
                    isMaster
                        ? isInWorkingFile
                            ? Engine.Plugin.ConflictStatus.NewForm
                            : Engine.Plugin.ConflictStatus.NoConflict
                        : isInWorkingFile
                            ? Engine.Plugin.ConflictStatus.OverrideInWorkingFile
                            : Engine.Plugin.ConflictStatus.OverrideInAncestor;
                */
            }
        }
        
        #endregion
        
        #region Un/Loading
        
        public virtual bool             Load()
        {
            //DebugLog.OpenIndentLevel( this.ToStringNullSafe(), true );
            //DebugDump( TargetHandle.Master );
            //DebugLog.CloseIndentLevel();
            return true;
        }
        
        public virtual bool             PostLoad()
        {
            //DebugLog.OpenIndentLevel( this.ToStringNullSafe(), true );
            //DebugDump( TargetHandle.Master );
            //DebugLog.CloseIndentLevel();
            return true;
        }
        
        // For Dispose() see the Allocation/Deallocation region above
        
        #endregion
        
        #region XeLib Handles
        
        #region Files
        
        public ElementHandle            CopyAsOverride()        { return CopyAsOverride( LastHandleBeforeWorkingFile ); }
        
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
            //DebugLog.OpenIndentLevel( new string[] { newHandle.ToStringNullSafe(), System.Environment.StackTrace }, true );
            
            bool result = false;
            
            if( !newHandle.IsValid() )
            {
                DebugLog.WriteLine( "newHandle is invalid!", true );
                goto localAbort;
            }
            if( ( newHandle as FormHandle ) == null )
            {
                DebugLog.WriteLine( "newHandle is not a FormHandle!", true );
                goto localAbort;
            }
            
            var insertAt = HandleExtensions.ExistingHandleIndex( _Handles, newHandle );
            if( insertAt >= 0 )
            {
                if( newHandle != _Handles[ insertAt ] )
                {
                    _Handles[ insertAt ].Dispose();
                    _Handles[ insertAt ] = newHandle;
                }
                goto localAbort;
            }
            insertAt = HandleExtensions.InsertHandleIndex( _Handles, newHandle );
            if( insertAt < 0 )
            {
                DebugLog.WriteWarning( string.Format( "Unable to get load order insertion index for newHandle {0}", newHandle.ToString() ) );
                goto localAbort;
            }
            
            if( insertAt == _Handles.Count )
                _Handles.Add( newHandle );
            else
                _Handles.Insert( insertAt, newHandle );
            
            RecalcHandleIndexes();
            result = true;
            
        localAbort:
            //DebugLog.CloseIndentLevel( "_Handles", _Handles );
            return result;
        }
        
        public ElementHandle            MasterHandle                { get { return HandleByIndex( 0 ); } }
        public ElementHandle            WorkingFileHandle           { get { return HandleByIndex( _WorkingFileHandleIndex ); } }
        
        public ElementHandle            LastFullRequiredHandle      { get { return HandleByIndex( _LastFullRequiredHandleIndex ); } }
        public ElementHandle            LastFullOptionalHandle      { get { return HandleByIndex( _LastFullOptionalHandleIndex ); } }
        public ElementHandle            LastHandleBeforeWorkingFile { get { return HandleByIndex( _WorkingFileHandleIndex <= 0 ? 0 :_WorkingFileHandleIndex - 1 ); } }
        
        // Return a cloned list so the caller cannot directly manipulate it.  Adding/Removing handles should be done through the proper API calls.
        public List<ElementHandle>      Handles                     { get { return !GetHandles() ? null : _Handles.Clone(); } }
        
        #region Private handle manipulation
        
        ElementHandle                   CopyAsOverride( ElementHandle fromHandle )
        {
            if( !fromHandle.IsValid() )
            {
                DebugLog.WriteLine( "fromHandle is null!", true );
                return null;
            }
            if( IsInWorkingFile() ) return WorkingFileHandle;
            
            // XeLib is weird in that we copy the top level form which will trigger copying all ancestors.
            // After the inherent ancestry copy is done, we need to go through our data structures to update them.
            // The ancestory XeLibfrom is using seems to be from the long path (which is a huge string).
            
            var workingFile = GodObject.Plugin.Data.Files.Working;
            
            /*
            DebugLog.Write( string.Format(
                "\n{0} :: CopyAsOverride() :: 0x{1} - \"{2}\" :: CopyElement() :: OverrideHandle = 0x{3} :: dstContainer = 0x{4}",
                this.FullTypeName(),
                this.FormID.ToString( "X8" ),
                this.EditorID.ToString(),
                OverrideHandle.ToString(),
                workingFile.OverrideHandle.ToString()
                ) );
            */
            
            // Copy the current top-level into the working file (should be the last handle before the working file)
            var resHandle = fromHandle.CopyElement<FormHandle>( workingFile.WorkingFileHandle, false );
            if( !resHandle.IsValid() )
            {
                DebugLog.WriteError( string.Format(
                    "Unable to copy override!\n\t{0}\n\tWorkingFile = \"{1}\"",
                    fromHandle.ToString(),
                    workingFile.Filename ) );
                //DebugDump();
                return null;
            }
            
            // Update the handles
            if( !AddNewHandle( resHandle ) )
            {
                DebugLog.WriteError( string.Format( "Unable to add new handle for override to Form!\n{0}", fromHandle.ToString() ) );
                resHandle.Dispose();
                return null;
            }
            
            // Now make sure my ancestors are updated
            /* Shouldn't need to actually do this since the tree won't change when creating an override
            var a = Ancestor;
            while( ( a != null )&&( ( a as File ) == null ) )
            {
                if( !a.IsInWorkingFile() )
                {
                    var h = workingFile.MasterHandle.GetRecord( GetFormID( Engine.Plugin.TargetHandle.Master ), false );
                    if( !a.AddNewHandle( h ) )
                    {
                        DebugLog.WriteLine( new [] { this.FullTypeName(), "CopyAsOverride", "Unable to add new handle for override to Form to ancestor...why are we doing this?" } );
                        return null;
                    }
                }
                a = a.Ancestor;
                // *
                DebugLog.Write( string.Format(
                    "\n{0} :: CopyAsOverride() :: 0x{1} - \"{2}\" :: Ancestor.IsInWorkingFile() :: 0x{3} - \"{4}\"",
                    this.FullTypeName(),
                    this.FormID.ToString( "X8" ),
                    this.EditorID.ToString(),
                    Ancestor.FormID.ToString( "X8" ),
                    Ancestor.EditorID ) );
                // * /
                //var aiiwf = Ancestor.IsInWorkingFile( true );
                //if( !aiiwf )
                //    return false;
            }
            */
            // Now make sure any attached scripts are updated
            if( !_Scripts.NullOrEmpty() )
            {
                foreach( var s in _Scripts )
                {
                    var soh = resHandle.GetScript( s.Signature );
                    if( soh.IsValid() )
                        s.AddNewHandle( soh );
                }
            }
            
            /*
            DebugLog.Write( string.Format(
                "\n{0} :: CopyAsOverride() :: 0x{1} - \"{2}\" :: Complete",
                this.FullTypeName(),
                this.FormID.ToString( "X8" ),
                this.EditorID.ToString() ) );
            */
            
            return resHandle;
        }
        
        bool                            GetHandles( FormHandle hSource = null )
        {
            if( !_Handles.NullOrEmpty() ) return true;
            
            var validForcedFormID = Engine.Plugin.Constant.ValidFormID( _Forced_FormID );
            var validForcedFilename = !string.IsNullOrEmpty( _Forced_Filename );
            var forcedFileIsLoaded = validForcedFilename ? GodObject.Plugin.Data.Files.IsLoaded( _Forced_Filename ) : false;

            if( ( Disposed )&&( ( !validForcedFormID )||( !validForcedFilename )||( !forcedFileIsLoaded ) ) )
                return false;

            //DebugLog.OpenIndentLevel( hSource.ToStringNullSafe(), true );

            bool result = false;
            
            if( hSource.IsValid() )
            {
                if( !hSource.IsMaster )
                    hSource = hSource.GetMasterRecord();
            }
            else
            {
                if( !validForcedFormID )
                {
                    DebugLog.WriteError( "FormID is invalid" );
                    goto localAbort;
                }
                if( !validForcedFilename )
                {
                    DebugLog.WriteError( string.Format( "Filename is invalid for 0x{0}", _Forced_FormID.ToString( "X8" ) ) );
                    goto localAbort;
                }
                if( !forcedFileIsLoaded )
                    return false;   // Not an error, user didn't select this core dependancy
                var m = GodObject.Plugin.Data.Files.Find( _Forced_Filename );
                if( ( m == null )||( m.LoadOrder == Constant.LO_Invalid ) )
                {
                    DebugLog.WriteError( string.Format( "Unable to get file {0} for Form 0x{1}", _Forced_Filename, _Forced_FormID.ToString( "X8" ) ) );
                    goto localAbort;
                }
                var fID = _Forced_FormID | m.GetFormID( Engine.Plugin.TargetHandle.Master );
                hSource = m.MasterHandle.GetMasterRecord( fID, false );
                if( !hSource.IsValid() )
                {
                    DebugLog.WriteError( string.Format( "Unable to GetMasterRecord from file {0} for Form 0x{0}", _Forced_Filename, _Forced_FormID.ToString( "X8" ) ) );
                    goto localAbort;
                }
            }
            
            var rSig = hSource.Signature;
            if( rSig != Signature )
            {
                hSource.Dispose();
                DebugLog.WriteError( string.Format( "Record has invalid signature for 0x{0} :: Expected \"{1}\" got \"{2}\"", _Forced_FormID.ToString( "X8" ), Signature, rSig ) );
                goto localAbort;
            }
            
            _Handles = new List<ElementHandle>();
            _Handles.Add( hSource );
            
            var hOverrides = hSource.GetOverrides();
            if( !hOverrides.NullOrEmpty() )
                foreach( var hOverride in hOverrides )
                    if( ( !_Handles.Contains( hOverride ) )&&( !_Handles.Any( h => h.DuplicateOf( hOverride ) ) ) )
                        _Handles.Add( hOverride );

            RecalcHandleIndexes();

            // No collection means this is a CoreForm or an "early load" CustomForm
            if( _ParentCollection == null )
                _ParentCollection = GodObject.Plugin.Data.Root.GetCollection( this.GetType(), true, false, false );
            
            // Make sure the parent collection actually contains this Form
            if( _ParentCollection != null )
                _ParentCollection.Add( this );

            result = true;
            Disposed = false;

        localAbort:
            //DebugLog.CloseIndentLevel( "_Handles", _Handles );
            return result;
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
                    var hOverride = _Handles[ i ] as FormHandle;
                    var hLO = hOverride.FileHandle.LoadOrder;
                    if( hLO == wLO )
                        _WorkingFileHandleIndex = i;
                    if( !hOverride.IsPartialRecord )
                    {
                        if( hLO < wLO )
                            _LastFullRequiredHandleIndex = i;
                        if( hLO > wLO )
                            _LastFullOptionalHandleIndex = i;
                    }
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
        
        #endregion
        
        #region Parent/Child collection[s]
        
        #region Parent container collection
        
        public Collection               ParentCollection
        {
            get { return _ParentCollection; }
            set
            {
                if( _ParentCollection == value ) return;
                if( _ParentCollection != null ) _ParentCollection.Remove( this );
                _ParentCollection = value;
                if( _ParentCollection != null ) _ParentCollection.Add( this );
            }
        }
        
        #endregion
        
        #region Child collections
        
        public void                     AddCollection( Collection container )
        {
            if( _ChildCollections == null ) _ChildCollections = new List<Collection>();
            _ChildCollections.Add( container );
        }
        
        public Collection               CollectionFor( string signature )
        {
            return _ChildCollections.NullOrEmpty() ? null : _ChildCollections.Find( c => c.Association.Signature == signature );
        }
        
        public Collection               CollectionFor<TSync>() where TSync : class, IXHandle
        {
            return _ChildCollections.NullOrEmpty() ? null : _ChildCollections.Find( c => c.Association.ClassType == typeof( TSync ) );
        }
        
        public Collection               CollectionFor( ClassAssociation association )
        {
            return !Plugin.Attributes.AssociationExtensions.IsValid( association ) || _ChildCollections.NullOrEmpty() ? null : _ChildCollections.Find( c => c.Association.ClassType == association.ClassType );
        }
        
        // Don't return list, clone it so the caller cannot directly manipulate it.  Adding/Removing collections should be done through the proper API calls.
        public List<Collection>         ChildCollections            { get { return _ChildCollections.Clone(); } }
        
        #endregion
        
        #endregion
        
        #endregion
        
        #region ISyncedGUIObject
        
        public event EventHandler       ObjectDataChanged;
        
        public virtual string           ExtraInfo                   { get { return Signature; } }
        
        bool _SupressObjectDataChangedEvent = false;

        public bool                     ObjectDataChangedEventsSupressed { get { return _SupressObjectDataChangedEvent; } }

        public void                     SupressObjectDataChangedEvents() { _SupressObjectDataChangedEvent = true; }
        
        public void                     ResumeObjectDataChangedEvents( bool sendevent )
        {

            _SupressObjectDataChangedEvent = false;
            if( sendevent ) SendObjectDataChangedEvent( this );
        }
        
        public void                     SendObjectDataChangedEvent( object sender )
        {
            if( _SupressObjectDataChangedEvent ) return;
            ObjectDataChanged?.Invoke( sender, null );
            if( !_Scripts.NullOrEmpty() )
                foreach( var script in _Scripts )
                    if( sender != script )
                        script.SendObjectDataChangedEvent( this );
        }
        
        public virtual bool             InitialCheckedOrSelectedState() { return false; }
        
        public virtual bool             ObjectChecked( bool checkedValue ) { return checkedValue; }
        
        #endregion
        
        #region Form Specific Functions
        
        public static TForm             CreateForm<TForm>( Collection parentCollection, IXHandle ancestor, ElementHandle handle ) where TForm : class, IXHandle
        {
            if( typeof( TForm ) != parentCollection.Association.ClassType )
            {
                DebugLog.WriteError( string.Format( "TForm does not match Collection Form Type :: {0} != {1}", typeof( TForm ).ToString(), parentCollection.Association.ClassType.ToString() ) );
                return null;
            }
            return CreateForm( parentCollection, ancestor, handle ) as TForm;
        }
        
        public static IXHandle          CreateForm( Collection parentCollection, IXHandle ancestor, ElementHandle handle )
        {
            return Activator.CreateInstance( parentCollection.Association.ClassType, new Object[] { parentCollection, ancestor, handle } ) as IXHandle;
        }

        List<Form>                      _References = null;
        public List<Form>               References
        {
            get
            {
                if( _References.NullOrEmpty() )
                {
                    var m = GodObject.Windows.GetWindow<GUIBuilder.Windows.Main>();
                    m.PushStatusMessage();
                    m.PushItemOfItems();
                    m.SetCurrentStatusMessage( string.Format( "Plugin.LoadingReferencesOf".Translate(), IDString ) );

                    //DebugLog.OpenIndentLevel( this.IDString );

                    List<Form> resultList = null;

                    var mh = MasterHandle as FormHandle;
                    if( !mh.IsValid() ) goto localReturnResult;

                    var refs = mh.GetReferencedBy();
                    if( refs.NullOrEmpty() ) goto localReturnResult;

                    resultList = new List<Form>();
                    var max = refs.Length;
                    for( int i = 0; i < max; i++ )
                    {
                        m.SetItemOfItems( i, max );
                        var refHandle = refs[ i ];
                        #region Debug dump
                        //var rFID = refHandle.FormID;
                        //var rSig = refHandle.Signature;
                        //DebugLog.WriteLine( "[ " + i + " ] = " + rSig + " 0x" + rFID.ToString( "X8" ) );
                        #endregion
                        // Add the form to the return list
                        var rForm = GodObject.Plugin.Data.Root.Find( refHandle ) as Form;
                        if( ( rForm != null ) && ( resultList.IndexOf( rForm ) < 0 ) ) resultList.Add( rForm );
                    }

                localReturnResult:
                    //DebugLog.CloseIndentList( "Forms", resultList, false, true );
                    m.PopItemOfItems();
                    m.PopStatusMessage();
                    _References = resultList.NullOrEmpty()
                        ? null
                        : resultList;
                }
                return _References;
            }
        }
        
        public void                     AttachScript( PapyrusScript script )
        {
            if( _Scripts == null )
                _Scripts = new List<PapyrusScript>();
            if( _Scripts.Exists( s => s == script ) ) return;
            _Scripts.Add( script );
        }
        
        public TScript                  GetScript<TScript>() where TScript : PapyrusScript
        {
            if( _Scripts.NullOrEmpty() ) return null;
            foreach( var script in _Scripts )
            {
                var ps = script as TScript;
                if( ps != null )
                    return ps;
            }
            return null;
        }
        
        public PapyrusScript            GetScript( string scriptSignature )
        {
            return _Scripts.NullOrEmpty() ? null : _Scripts.Find( s => s.Signature == scriptSignature );
        }
        
        #endregion
        
        #region Debugging
        
        public void                     DebugDump( TargetHandle target )
        {
            DebugLog.WriteLine( string.Format( "Base: {0}", this.GetType().FullName ) );
            DebugLog.WriteLine( string.Format( "\tHandles: {0}", _Handles.Count ) );
            DebugLog.WriteLine( string.Format( "\t\tLastFullRequiredHandleIndex: {0}", _LastFullRequiredHandleIndex ) );
            DebugLog.WriteLine( string.Format( "\t\tLastFullOptionalHandleIndex: {0}", _LastFullOptionalHandleIndex ) );
            DebugLog.WriteLine( string.Format( "\t\tWorkingFileHandleIndex: {0}", _WorkingFileHandleIndex ) );
            if( !_Handles.NullOrEmpty() )
                for( int i = 0; i < _Handles.Count; i++ )
                    DebugLog.WriteLine( string.Format( "\t\t[ {0} ] :: {1}", i, _Handles[ i ].ToString() ) );
            DebugLog.WriteLine( string.Format( "\tFormID: {0}", _FormID.ToString( target ) ) );
            DebugLog.WriteLine( string.Format( "\tEditorID: \"{0}\"", _EditorID.ToString( target ) ) );
            var allFlags = RecordFlags.AllFlags( target );
            if( !allFlags.NullOrEmpty() )
            {
                DebugLog.WriteLine( string.Format( "\tFlags: ", RecordFlags.ToString( target ) ) );
                foreach( var f in allFlags )
                    if( ( !string.IsNullOrEmpty( f ) )&&( !f.StartsWith( "unknown", StringComparison.InvariantCultureIgnoreCase ) ) ) DebugLog.WriteLine( string.Format( "\t\t{0} = {1}", f, RecordFlags.GetFlag( target, f ) ) );
            }
            DebugDumpChild( target );
        }
        
        public virtual void             DebugDumpChild( TargetHandle target ) {}
        
        #endregion
        
        #region IMouseOver
        
        public virtual bool             IsMouseOver( Maths.Vector2f mouse, float maxDistance )
        {
            return false;
        }
        
        public List<string>             MouseOverInfo
        {
            get
            {
                var mo = new List<string>();
                mo.Add( string.Format( "{0}: {1}", Signature, IDString ) );
                var moel = MouseOverExtra;
                if( !moel.NullOrEmpty() )
                    foreach( var moe in moel )
                        mo.Add( string.Format( "\t{0}", moe ) );
                var sc = ( _Scripts == null ) ? 0 : _Scripts.Count;
                if( sc > 0 )
                {
                    for( int i = 0; i < sc; i++ )
                    {
                        var smol = _Scripts[ i ].MouseOverInfo;
                        if( !smol.NullOrEmpty() )
                            foreach( var smo in smol )
                                mo.Add( string.Format( "\t{0}", smo ) );
                    }
                }
                return mo;
            }
        }
        
        public virtual List<string>     MouseOverExtra
        {
            get
            {
                return null;
            }
        }
        
        #endregion
        
        #region Class Association
        
        ClassAssociation                _Association = null;
        /// <summary>
        /// Class Attributes for this instanced Form.
        /// </summary>
        public  ClassAssociation        Association
        {
            get
            {
                if( _Association == null )
                    _Association = Reflection.AssociationFrom( this.GetType() );
                return _Association;
            }
        }
        
        #endregion
        
    }
    
}
