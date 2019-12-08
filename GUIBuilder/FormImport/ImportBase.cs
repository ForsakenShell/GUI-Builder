/*
 * ImportBase.cs
 *
 * Base class for importing bulk data back into the working file.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Plugin.Extensions;


namespace GUIBuilder.FormImport
{
    
    public abstract class ImportBase : Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        /* TODO:  Implement this cleaner
         * Copy-pasta these into child classes and change as appropriate
         * The first two parameters of the base constructor will then be
         * IMPORT_SIGNATURE, TARGET_RECORD_FLAGS
        const string                    IMPORT_SIGNATURE = "ImportClass";
        const uint                      TARGET_RECORD_FLAGS = Target_Form_Flags;
        */
        
        GUIBuilder.Windows.BatchImport _BatchWindow                 = null;
        
        string                         _Signature                   = null;
        uint                           _RecordFlags                 = 0;
        bool                           _FailOnApplyIfUnresolved     = true;
        
        ImportStates                   _ImportState                 = ImportStates.Unparsed;
        public ImportStates             ImportState
        {
            get
            {
                return _ErrorState
                    ? ImportStates.Error
                    : _ImportState;
            }
        }
        
        bool                           _ErrorState                  = false;
        string                         _ErrorMessage                = null;
        protected string                ErrorMessasge               { get { return _ErrorMessage; } }
        
        private ImportTarget           _Target                      = null;
        
        /*
        public string                   IDString
        {
            get
            {
                var tvID = _Target.Value as Engine.Plugin.Interface.ISyncedGUIObject;
                return tvID != null ? tvID.IDString : string.Format( "0x{0} - \"{1}\"", _Target.Value.FormID.ToString( "X8" ), _Target.Value.EditorID );
            }
        }
        */
        
        /// <summary>
        /// Base import constructor
        /// </summary>
        /// <param name="signature">Import signature</param>
        /// <param name="recordFlags">Target record flags</param>
        /// <param name="failOnApplyIfUnresolved">Throw an exception if the target cannot be resolved</param>
        /// <param name="classType">Target class type, cannot be null</param>
        /// <param name="targetForm">Target form to apply import to (may be null for injection)</param>
        protected                       ImportBase( string signature, uint recordFlags, bool failOnApplyIfUnresolved, Type classType, Engine.Plugin.Form targetForm )
        {
            if( classType == null )
                throw new NullReferenceException( "classType cannot be null" );
            var fType = typeof( Engine.Plugin.Form );
            if( ( fType != classType )&&( !classType.IsSubclassOf( fType ) ) )
                throw new NullReferenceException( "classType must be or derrived from \"Engine.Plugin.Form\"" );
            var tType = targetForm == null ? null : targetForm.GetType();
            if( ( targetForm != null )&&( classType != tType ) )
                throw new NullReferenceException( string.Format( "classType does not match targetForm!  Must be \"{0}\"", tType.ToString() ) );
            bool resolved = false;
            try
            {
                _Signature = signature;
                _RecordFlags = recordFlags;
                _FailOnApplyIfUnresolved = failOnApplyIfUnresolved;
                _Target = classType == typeof( Engine.Plugin.Forms.ObjectReference )
                    ? new ObjectReferenceTarget( this, targetForm, null, null )
                    : new FormTarget( this, classType, targetForm );
                resolved = _Target.Resolve( failOnApplyIfUnresolved );
                if( ( failOnApplyIfUnresolved )&( !resolved ) )
                    throw new Exception( string.Format(
                        "{0} :: cTor :: Unable to resolve import target!\n{1}",
                        this.GetType().ToString(),
                        _ErrorMessage ) );
            }
            catch( Exception e )
            {
                throw new Exception( string.Format(
                    "{0} :: cTor :: Exception parsing parameters\nParse Error:\n{1}\nInner Exception:\n{2}",
                    this.GetType().ToString(),
                    _ErrorMessage,
                    e.ToString() ) );
            }
            _ImportState = resolved
                ? ImportStates.Resolved
                : ImportStates.Parsed;
        }
        
        /// <summary>
        /// Base import constructor
        /// </summary>
        /// <param name="signature">Import signature</param>
        /// <param name="recordFlags">Target record flags</param>
        /// <param name="failOnApplyIfUnresolved">Throw an exception if the target cannot be resolved</param>
        /// <param name="classType">Target class type, cannot be null</param>
        /// <param name="targetScript">Target script to apply import to (may be null for injection)</param>
        protected                       ImportBase( string signature, uint recordFlags, bool failOnApplyIfUnresolved, Type classType, Engine.Plugin.PapyrusScript targetScript )
        {
            if( classType == null )
                throw new NullReferenceException( "classType cannot be null" );
            var fType = typeof( Engine.Plugin.PapyrusScript );
            if( ( fType != classType )&&( !classType.IsSubclassOf( fType ) ) )
                throw new NullReferenceException( "classType must be or derrived from \"Engine.Plugin.PapyrusScript\"" );
            var tType = targetScript == null ? null : targetScript.GetType();
            if( ( targetScript != null )&&( classType != tType ) )
                throw new NullReferenceException( string.Format( "classType does not match targetScript!  Must be \"{0}\", targetScript is \"{1}\"", classType.ToString(), tType.ToString() ) );
            bool resolved = false;
            try
            {
                _Signature = signature;
                _RecordFlags = recordFlags;
                _FailOnApplyIfUnresolved = failOnApplyIfUnresolved;
                _Target = new ScriptTarget( this, classType, targetScript );
                resolved = _Target.Resolve( failOnApplyIfUnresolved );
                if( ( failOnApplyIfUnresolved )&( !resolved ) )
                    throw new Exception( string.Format(
                        "{0} :: cTor :: Unable to resolve import target!\n{1}",
                        this.GetType().ToString(),
                        _ErrorMessage ) );
            }
            catch( Exception e )
            {
                throw new Exception( string.Format(
                    "{0} :: cTor :: Exception parsing parameters\nParse Error:\n{1}\nInner Exception:\n{2}",
                    this.GetType().ToString(),
                    _ErrorMessage,
                    e.ToString() ) );
            }
            _ImportState = resolved
                ? ImportStates.Resolved
                : ImportStates.Parsed;
        }
        
        /// <summary>
        /// Base import constructor
        /// </summary>
        /// <param name="signature">Import signature</param>
        /// <param name="recordFlags">Target record flags</param>
        /// <param name="failOnApplyIfUnresolved">Throw an exception if the target cannot be resolved</param>
        /// <param name="classType">Target class type, cannot be null</param>
        /// <param name="targetForm">Target form to apply import to (may be null for injection)</param>
        protected                       ImportBase( string signature, uint recordFlags, bool failOnApplyIfUnresolved, Type classType, Engine.Plugin.Forms.ObjectReference targetForm, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell )
        {
            if( classType == null )
                throw new NullReferenceException( "classType cannot be null" );
            var fType = typeof( Engine.Plugin.Forms.ObjectReference );
            if( ( fType != classType )&&( !classType.IsSubclassOf( fType ) ) )
                throw new NullReferenceException( "classType must be or derrived from \"Engine.Plugin.Forms.ObjectReference\"" );
            var tType = targetForm == null ? null : targetForm.GetType();
            if( ( targetForm != null )&&( classType != tType ) )
                throw new NullReferenceException( string.Format( "classType does not match targetForm!  Must be \"{0}\"", tType.ToString() ) );
            bool resolved = false;
            try
            {
                _Signature = signature;
                _RecordFlags = recordFlags;
                _FailOnApplyIfUnresolved = failOnApplyIfUnresolved;
                _Target = classType == typeof( Engine.Plugin.Forms.ObjectReference )
                    ? new ObjectReferenceTarget( this, targetForm, worldspace, cell )
                    : new FormTarget( this, classType, targetForm );
                resolved = _Target.Resolve( failOnApplyIfUnresolved );
                if( ( failOnApplyIfUnresolved )&( !resolved ) )
                    throw new Exception( string.Format(
                        "{0} :: cTor :: Unable to resolve import target!\n{1}",
                        this.GetType().ToString(),
                        _ErrorMessage ) );
            }
            catch( Exception e )
            {
                throw new Exception( string.Format(
                    "{0} :: cTor :: Exception parsing parameters\nParse Error:\n{1}\nInner Exception:\n{2}",
                    this.GetType().ToString(),
                    _ErrorMessage,
                    e.ToString() ) );
            }
            _ImportState = resolved
                ? ImportStates.Resolved
                : ImportStates.Parsed;
        }
        
        protected                       ImportBase( string signature, uint recordFlags, bool failOnApplyIfUnresolved, Type classType, string[] importData )
        {
            if( classType == null )
                throw new NullReferenceException( "classType cannot be null" );
            var fType = typeof( Engine.Plugin.Form );
            if( ( fType != classType )&&( !classType.IsSubclassOf( fType ) ) )
                throw new NullReferenceException( "classType must be or derrived from \"Engine.Plugin.Form\"" );
            bool resolved = false;
            try
            {
                _Signature = signature;
                _RecordFlags = recordFlags;
                _FailOnApplyIfUnresolved = failOnApplyIfUnresolved;
                _Target = classType == typeof( Engine.Plugin.Forms.ObjectReference )
                    ? new ObjectReferenceTarget( this )
                    : new FormTarget( this, classType );
                //_Target = new FormTarget( this, classType );
                resolved = ParseImport( importData );
                if( ( failOnApplyIfUnresolved )&( !resolved ) )
                    throw new Exception( string.Format(
                        "{0} :: cTor :: Unable to parse importData\nParse Error:\n{1}",
                        this.GetType().ToString(),
                        _ErrorMessage ) );
            }
            catch( Exception e )
            {
                throw new Exception( string.Format(
                    "{0} :: cTor :: Exception parsing importData\nParse Error:\n{1}\nInner Exception:\n{2}",
                    this.GetType().ToString(),
                    _ErrorMessage,
                    e.ToString() ) );
            }
            _ImportState = resolved
                ? ImportStates.Resolved
                : ImportStates.Parsed;
        }
        
        protected void                  ClearErrors()
        {
            _ErrorMessage = null;
            _ErrorState = false;
        }
        public void                     AddErrorMessage( FormImport.ErrorTypes type, string message, Exception e = null )
        {
            var errorLine = string.Format( "{0} : {1} : {2}", Signature, type.ToString(), string.IsNullOrEmpty( message ) ? "Undefined" : message );
            if( _ErrorMessage == null )
                _ErrorMessage = errorLine;
            else
                _ErrorMessage += errorLine;
            if( _BatchWindow != null )
                _BatchWindow.AddImportMessage( errorLine );
            DebugLog.WriteLine( errorLine );
            if( e != null )
                DebugLog.WriteLine( new [] { e.ToString(), e.StackTrace } );
            _ErrorState = true;
        }
        
        public bool                     ImportIsValid()
        {
            return
                _ErrorState
                // This conditional is wanted as it is cheaper than Resolve()
                // disable once SimplifyConditionalTernaryExpression
                ? false
                : Resolve( false );
        }
        
        public bool                     Resolve( bool errorIfUnresolveable )
        {
            ClearErrors();
            if( ImportState == ImportStates.Resolved ) return true;
            var resolved = _Target.Resolve( errorIfUnresolveable ) && ResolveReferenceForms( errorIfUnresolveable );
            if( resolved ) _ImportState = ImportStates.Resolved;
            return resolved;
        }
        
        #region Target Shortcuts
        
        protected ImportTarget          Target                      { get { return _Target; } }
        protected void                  SetTarget( Engine.Plugin.Interface.IXHandle syncObject )
        {
            _Target.Value = syncObject;
        }
        
        protected Engine.Plugin.Form    TargetForm
        {
            get
            {
                var f = _Target as FormTarget;
                if( f != null )
                    return f.Form;
                var s = _Target as ScriptTarget;
                return ( s != null )&&( s.Script != null )
                    ? s.Script.Form
                    : null;
            }
        }
        protected Engine.Plugin.Forms.ObjectReference TargetRef
        {
            get
            {
                var f = _Target as ObjectReferenceTarget;
                if( f != null )
                    return f.Reference;
                var s = _Target as ScriptTarget;
                return ( s != null )&&( s.Script != null )
                    ? s.Script.Reference
                    : null;
            }
        }
        protected Engine.Plugin.PapyrusScript TargetScript
        {
            get
            {
                var s = _Target as ScriptTarget;
                return ( s != null )
                    ? s.Script
                    : null;
            }
        }
        
        protected uint                  TargetFormID                { get { return _Target.FormID; } }
        protected string                TargetEditorID              { get { return _Target.EditorID; } }
        
        public string                   DisplayIDInfo( string f = "{0}", string unresolveableSuffix = null )
        {
            //return ImportBase.ExtraInfoFor( f, Value, FormID, EditorID, unresolveable, extra );
            return _Target.DisplayIDInfo( f, unresolveableSuffix );
        }
        
        #endregion
        
        #region Override Target[s]
        
        protected bool                  CopyToWorkingFile<T>( T syncObject ) where T : class, Engine.Plugin.Interface.IXHandle
        {
            if( syncObject == null ) return false;
            
            string errorMessage = null;
            try
            {
                if( syncObject.IsInWorkingFile() )
                    return true;
                if( syncObject.CopyAsOverride() != null )
                    return true;
                errorMessage = string.Format(
                    "\n{0} :: CopyToWorkingFile<T>() :: Unable to copy override for {1}!",
                    this.GetType().ToString(),
                    syncObject.ExtraInfoFor( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ), syncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), unresolveable: "unresolved" )
                );
            }
            catch( Exception e )
            {
                errorMessage = string.Format(
                    "\n{0} :: CopyToWorkingFile<T>() :: An exception occured when trying to copy override for {1}\nInner Exception:\n{2}",
                    this.GetType().ToString(),
                    syncObject.ExtraInfoFor( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ), syncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ), unresolveable: "unresolved" ),
                    e.ToString());
            }
            
            DebugLog.WriteLine( errorMessage );
            return false;
        }
        
        protected virtual bool          CreateNewFormInWorkingFile()
        {
            return false;
        }
        
        #endregion
        
        #region Target Record Flags
        
        bool                            TargetRecordFlagsMatch
        {
            get
            {
                return ( TargetForm != null )&&( TargetForm.RecordFlags.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == _RecordFlags );
                //if( TargetForm == null ) return false;
                //var rf = TargetForm.RecordFlags.GetValue( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                //DebugLog.WriteLine( new string[] { this.GetType().ToString(), TargetForm.ExtraInfoFor(), string.Format( "Record Flags = 0x{0} ?= 0x{1}", rf.ToString( "X8" ), _RecordFlags.ToString( "X8" ) ) } );
                //return rf == _RecordFlags;
            }
        }
        
        bool                            ApplyRecordFlagsToTarget()
        {
            var targetForm = TargetForm;
            if( targetForm == null ) return false;
            targetForm.RecordFlags.SetValue( Engine.Plugin.TargetHandle.Working, _RecordFlags );
            return TargetRecordFlagsMatch;
        }
        
        #endregion
        
        #region Parse import file data
        
        public bool                     ParseImport( string[] importData )
        {
            ClearErrors();
            
            if( importData.NullOrEmpty() )
            {
                AddErrorMessage( ErrorTypes.Parse, "Null or empty importData!" );
                return false;
            }
            
            var result = true;  // We hope
            var properFormType = false;
            foreach( var keyValue in importData )
            {
                if( string.IsNullOrEmpty( keyValue ) )
                    continue;
                
                var kv = keyValue.ParseImportLine( ':' );
                if( ( kv == null )||( kv.Length != 2 ) )
                    continue;
                
                if( string.IsNullOrEmpty( kv[ 0 ] ) )
                    continue;
                
                //DebugLog.Write( string.Format( "\"{0}\" :: \"{1}\"", kv[ 0 ], kv[ 1 ] ) );
                
                if( kv[ 0 ] == "Type" )
                {
                    properFormType = kv[ 1 ] == Signature;
                    if( !properFormType )
                    {
                        AddErrorMessage(
                            ErrorTypes.Parse,
                            string.Format(
                                "Invalid \"Type\", expected \"{0}\" got \"{1}\"",
                                Signature,
                                kv[ 1 ] )
                        );
                        result = false;
                    }
                }
                else if( properFormType )
                {
                    switch( kv[ 0 ] )
                    {
                        case "TargetFormID":
                            _Target.FormID = uint.Parse( kv[ 1 ], System.Globalization.NumberStyles.HexNumber );
                            break;
                        case "TargetEditorID":
                            _Target.EditorID = kv[ 1 ];
                            break;
                            
                        case "TargetSignature":
                            var fa = Engine.Plugin.Attributes.Reflection.FormAssociationFrom( kv[ 1 ] );
                            if( fa == null )
                            {
                                AddErrorMessage(
                                    ErrorTypes.Parse,
                                    string.Format(
                                        "Unknown Form Signature :: \"{0}\"",
                                        kv[ 1 ] )
                                );
                                result = false;
                                break;
                            }
                            _Target.Association = fa;
                            break;
                            
                        default:
                            if( !ParseKeyValue( kv[ 0 ], kv[ 1 ] ) )
                            {
                                AddErrorMessage(
                                    ErrorTypes.Parse,
                                    string.Format(
                                        "Unknown keyword and value :: \"{0}\" = \"{1}\"",
                                        kv[ 0 ], kv[ 1 ] )
                                );
                                result = false;
                            }
                            break;
                    }
                }
                else if ( result )
                {   // No error yet and "Type" not yet encountered (should be first keyword:value of the import line)
                    AddErrorMessage(
                        ErrorTypes.Parse,
                        string.Format(
                            "Expected keyword \"Type\" got \"{0}\"",
                            kv[ 0 ] )
                    );
                    result = false;
                }
            }
            // Try to finalize but don't worry if we can't yet;
            // May be trying to reference a base object which
            // hasn't yet been injected or is going to be
            if( result )
                result = Resolve( false );
            return result;
        }
        
        public virtual bool             ParseKeyValue( string key, string value )
        {
            return true;
        }
        
        #endregion
        
        #region ISyncedListViewObject
        
        /*
        public string                   Filename
        {
            get
            {
                return _Target.Value != null
                    ? _Target.Value.Filename
                    : null;
            }
        }
        */
        
        public Engine.Plugin.File[]     Files
        {   // This should just be the list of targets which means just the working file...right...?
            get
            {
                var f = GodObject.Plugin.Data.Files.Working;
                return f == null
                    ? null
                    : new []{ f };
            }
        }
        
        public string[]                 Filenames
        {
            get
            {
                var files = Files;
                if( files.NullOrEmpty() ) return null;
                var filenames = new string[ files.Length ];
                for( int i = 0; i < files.Length; i++ )
                    filenames[ i ] = files[ i ].Filename;
                return filenames;
            }
        }
        
        public string                   Signature                   { get { return _Signature; } }
        
        public uint                     LoadOrder
        {
            get
            {
                return _Target.Value != null
                    ? _Target.Value.LoadOrder
                    : 0xFF;
            }
        }
        
        /*
        public uint                     FormID
        {
            get
            {
                return( _Target == null )||( _Target.Value == null )
                    ? Engine.Plugin.Constant.FormID_Invalid
                    : _Target.Value.FormID;
            }
        }
        */
        
        public uint                     GetFormID( Engine.Plugin.TargetHandle target )
        {
            return ( _Target == null )||( _Target.Value == null )
                ? Engine.Plugin.Constant.FormID_Invalid
                : _Target.Value.GetFormID( target );
        }
        public void                     SetFormID( Engine.Plugin.TargetHandle target, uint value )
        {
            if( ( _Target == null )||( _Target.Value == null ) ) return;
            _Target.Value.SetFormID( target, value );
        }
        
        /*
        public string                   EditorID
        {
            get { return GetDisplayEditorID(); }
            set { throw new NotImplementedException(); }
        }
        */
        
        public string                   GetEditorID( Engine.Plugin.TargetHandle target )
        {
            return GetDisplayEditorID( target );
        }
        public void                     SetEditorID( Engine.Plugin.TargetHandle target, string value )
        {
            throw new NotImplementedException();
        }
        
        public Engine.Plugin.ConflictStatus ConflictStatus
        {
            get
            {
                //DebugLog.Write( "ConflictStatus.get()" );
                if(!_Target.Value.Resolveable(_Target.FormID, _Target.EditorID) )
                    return Engine.Plugin.ConflictStatus.NewForm;
                    //return Engine.Plugin.ConflictStatus.Invalid;
                var targetForm = TargetForm;
                if( targetForm == null )
                    return Engine.Plugin.ConflictStatus.NewForm;
                /*
                var isMaster = targetForm.WorkingFileHandle.IsMaster;
                var wFile = GodObject.Plugin.Data.Files.Working;
                var tfFile = targetForm.Mod;
                var isInWorkingFile =
                    ( tfFile != null   )&&
                    ( wFile  != null   )&&
                    ( wFile  == tfFile );
                */
                //var isInWorkingFile = targetForm.IsInWorkingFile();
                return
                     !TargetRecordFlagsMatch || !ImportDataMatchesTarget()
                        ? Engine.Plugin.ConflictStatus.RequiresOverride
                        : Engine.Plugin.ConflictStatus.NoConflict;
            }
        }
        
        /*
        public bool                     IsModified
        {
            get
            {
                var form = TargetForm;
                return
                    ( form != null )&&
                    ( form.IsModified );
            }
        }
        */
        
        public string                   ExtraInfo
        {
            get
            {
                return _ErrorState
                    ? _ErrorMessage
                    : GetDisplayExtraInfo();
            }
        }
        
        public event EventHandler       ObjectDataChanged;
        bool                           _SupressEvents = false;
        
        public void                     SupressObjectDataChangedEvents()
        {
            _SupressEvents = true;
            if( TargetForm != null )
                TargetForm.SupressObjectDataChangedEvents();
        }
        
        public void                     ResumeObjectDataChangedEvents( bool sendevent )
        {
            //DebugLog.Write( string.Format( "{0} :: ResumeObjectDataChangedEvents() :: sendevent = {1}", this.GetType().ToString(), sendevent ) );
            _SupressEvents = false;
            if( TargetForm != null )
                TargetForm.ResumeObjectDataChangedEvents( sendevent );
            if( sendevent ) SendObjectDataChangedEvent( this );
        }
        
        public void                     SendObjectDataChangedEvent( object sender )
        {
            //DebugLog.Write( string.Format( "{0} :: SendObjectDataChangedEvent()", this.GetType().ToString() ) );
            if( _SupressEvents ) return;
            EventHandler handler = ObjectDataChanged;
            if( handler != null )
                handler( sender, null );
        }
        
        public bool                     InitialCheckedOrSelectedState()
        {
            var cs = ConflictStatus;
            return
                ( cs == Engine.Plugin.ConflictStatus.NewForm )||
                ( cs == Engine.Plugin.ConflictStatus.RequiresOverride );
        }
        
        public bool                     ObjectChecked( bool checkedValue )
        {
            return checkedValue;
        }
        
        #endregion
        
        protected abstract string       GetDisplayUpdateFormInfo();
        protected abstract string       GetDisplayNewFormInfo();
        
        protected string                GetDisplayExtraInfo()
        {
            var targetForm = TargetForm;
            
            if( ( targetForm != null )&&( TargetRecordFlagsMatch )&&( ImportDataMatchesTarget() ) )
                return "No changes";
            
            var prefix = ( targetForm == null )
                ? "New Form: "
                : "Update Form: ";
            
            var tmp = new List<string>();
            if( !TargetRecordFlagsMatch )
            {
                if( _RecordFlags == 0 )
                    tmp.Add( "Clear Record Flags" );
                else
                    tmp.Add( string.Format( "Record Flags 0x{0}", _RecordFlags.ToString( "X8" ) ) );
            }
            
            tmp.Add( ( targetForm == null )
                    ? GetDisplayNewFormInfo()
                    : GetDisplayUpdateFormInfo() );
            
            var tmp2 = GenIXHandle.ConcatDisplayInfo( tmp );
            return prefix + tmp2;
        }
        
        protected abstract string       GetDisplayEditorID( Engine.Plugin.TargetHandle target );
        
        public abstract int             InjectPriority { get; }
        
        protected virtual bool          ResolveReferenceForms( bool errorIfUnresolveable )
        {
            return true;
        }
        
        protected abstract bool         ImportDataMatchesTarget();
        
        public bool                     Apply( GUIBuilder.Windows.BatchImport importWindow )
        {
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.FormImport.ImportBase", "Apply()", this.GetType().ToString(), Target.DisplayIDInfo() } );
            var result = false;
            
            _BatchWindow = importWindow;
            if( _ErrorState )
            {
                AddErrorMessage( ErrorTypes.Import, "Import in error state, cannot Apply()" );
                goto localAbort;
            }
            if(
                ( _FailOnApplyIfUnresolved )&
                ( !Resolve( _FailOnApplyIfUnresolved ) )
            )
            {
                AddErrorMessage( ErrorTypes.Import, "Resolve() errors, cannot Apply()" );
                goto localAbort;
            }
            
            if( TargetForm == null )
            {
                if( !CreateNewFormInWorkingFile() )
                {
                    AddErrorMessage( ErrorTypes.Import, "Unable to create new form in working file" );
                    goto localAbort;
                }
            }
            else if( !CopyToWorkingFile( TargetForm ) )
            {
                AddErrorMessage(
                    ErrorTypes.Import,
                    string.Format(
                        "Unable to create override of {0} in \"{1}\"",
                        TargetForm.ToString(),
                        GodObject.Plugin.Data.Files.Working.Filename
                    )
                );
                goto localAbort;
            }
            
            SupressObjectDataChangedEvents();
            
            try
            {
                DebugLog.OpenIndentLevel( new [] { "GUIBuilder.FormImport.ImportBase", "ApplyImport()", this.GetType().ToString() } );
                result = ApplyImport();
                DebugLog.CloseIndentLevel();
                
                DebugLog.OpenIndentLevel( new [] { "GUIBuilder.FormImport.ImportBase", "ApplyRecordFlagsToTarget()", this.GetType().ToString() } );
                result &= ApplyRecordFlagsToTarget();
                DebugLog.CloseIndentLevel();
            }
            catch( Exception e )
            {
                AddErrorMessage( ErrorTypes.Import, "An unexpected exception has occured applying import!", e );
            }
            if( result )
            {
                var refr = TargetForm as Engine.Plugin.Forms.ObjectReference;
                if( refr != null ) refr.CheckForBackgroundCellChange( false );
            }
            else
                DebugLog.WriteError( this.GetType().ToString(), "Apply()", "Unable to apply import to the target form!" );
            
            ResumeObjectDataChangedEvents( true );
            
        localAbort:
            DebugLog.CloseIndentLevel( "result", result.ToString() );
            return result;
        }
        
        protected abstract void         DumpImport();
        protected abstract bool         ApplyImport();
        
        #region Generic Sub-Class Functions
        
        #region Import List Management
        
        public static void              AddToList( ref List<ImportBase> list, ImportBase i )
        {
            if( i == null ) return;
            if( list == null ) list = new List<ImportBase>();
            if( !list.NullOrEmpty() )
            {
                foreach( var li in list )
                {
                    if( li.Signature.InsensitiveInvariantMatch( i.Signature ) )
                    {
                        if(
                            ( !Engine.Plugin.Constant.ValidFormID( li.GetFormID( Engine.Plugin.TargetHandle.Master ) ) )||
                            ( !Engine.Plugin.Constant.ValidFormID( i .GetFormID( Engine.Plugin.TargetHandle.Master ) ) )
                        )
                        {
                            if( li.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).InsensitiveInvariantMatch( i.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) )
                            {
                                //DebugLog.Write( string.Format( "\nReject :: \"{0}\" :: \"{1}\"", li.EditorID, i.EditorID ) );
                                return;
                            }
                        }
                        else if( li.GetFormID( Engine.Plugin.TargetHandle.Master ) == i.GetFormID( Engine.Plugin.TargetHandle.Master ) )
                        {
                            //DebugLog.Write( string.Format( "\nReject :: 0x{0} :: 0x{1}", li.FormID.ToString( "X8" ), i.FormID.ToString( "X8" ) ) );
                            return;
                        }
                    }
                }
            }
            list.Add( i );
        }
        
        public static void              AddToList( ref List<ImportBase> list, List<ImportBase> otherList )
        {
            if( otherList.NullOrEmpty() ) return;
            foreach( var oli in otherList )
                AddToList( ref list, oli );
        }
        
        public static bool              AllImportsMatchState( List<ImportBase> list, ImportStates state )
        {
            return
                ( !list.NullOrEmpty() )&&
                ( !list.Any( item =>( item.ImportState != state ) ) );
        }
        
        public static bool              AllImportsMatchTarget( List<ImportBase> list )
        {
            return
                ( list.NullOrEmpty() )||
                ( list.All( item =>( item.ImportDataMatchesTarget() ) ) );
        }
        
        #endregion
        
        #endregion
        
        public override string          ToString()
        {
            return string.Format( "[Signature = \"{0}\" :: Target = {1}]", Signature, DisplayIDInfo() );
        }
        
        public static bool              ShowImportDialog( List<ImportBase> importForms, bool enableControlsOnClose, ref bool allImportsMatchTarget )
        {
            if( importForms.NullOrEmpty() ) return false;
            
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.FormImport.ImportBase", "ShowImportDialog()" } );
            
            #region Import window
            
            var bbiw = new GUIBuilder.Windows.BatchImport();
            bbiw.EnableControlsOnClose = enableControlsOnClose;
            bbiw.ImportForms = importForms;
            bbiw.ShowDialog();
            
            allImportsMatchTarget = bbiw.AllImportsMatchTarget;
            
            #endregion
            
            DebugLog.CloseIndentLevel();
            return true;
        }
        
    }
    
}
