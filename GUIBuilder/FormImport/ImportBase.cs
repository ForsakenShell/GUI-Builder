/*
 * ImportBase.cs
 *
 * Base class for importing bulk data back into the working file.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

using GUIBuilder.Windows;

using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;
using Engine.Plugin.Extensions;


namespace GUIBuilder.FormImport
{
    
    public class ImportBase : ISyncedGUIObject
    {

        BatchImport                                     _BatchWindow = null;

        Priority                                        _InjectPriority;
        public Priority                                 InjectPriority
        {
            get
            {
                return _InjectPriority;
            }
        }

        string                                          _Signature = typeof( ImportBase ).TypeName();
        public string                                   Signature
        {
            get
            {
                return _Signature;
            }
        }
        
        bool                                            _FailOnApplyIfUnresolved = true;
        
        ImportStates                                    _ImportState = ImportStates.Unparsed;
        public ImportStates                             ImportState
        {
            get
            {
                return _ErrorState
                    ? ImportStates.Error
                    : _ImportState;
            }
        }
        
        bool                                            _ErrorState = false;
        string                                          _ErrorMessage = null;
        protected string                                ErrorMessasge
        {
            get
            {
                return _ErrorMessage;
            }
        }
        
        private ImportTarget                            _Target = null;
        public ImportTarget                             Target
        {
            get
            {
                return _Target;
            }
        }

        private List<ImportOperation>                   _Operations = new List<ImportOperation>();

        public void                                     AddOperation( ImportOperation operation )
        {
            _Operations.Add( operation );
        }

        #region Constructor

        private void                                    INTERNAL_Constructor(
            string signature,
            Priority priority,
            bool failOnApplyIfUnresolved,
            Type type,
            IXHandle target,
            string editorID,
            Cell cell )
        {
            bool resolved = false;
            try
            {
                if( type == null )
                    throw new NullReferenceException( "'type' cannot be null" );
                if( !type.HasInterface<IXHandle>() )
                    throw new NullReferenceException( "'type' must implement Engine.Plugin.Interface.IXHandle" );

                var tType = target?.GetType();
                if( ( target != null ) && ( type != tType ) )
                    throw new NullReferenceException( string.Format( "target is wrong type!\n'type' = {0}\n'target' = {1}", type.FullName(), tType.FullName() ) ); ;
                
                _Signature                  = signature;
                _InjectPriority             = priority;
                _FailOnApplyIfUnresolved    = failOnApplyIfUnresolved;
                _Target                     = type == typeof( ObjectReference )
                                            ? new ObjectReferenceTarget( this, "TargetU".Translate(), target as ObjectReference, cell )
                                            : new ImportTarget( this, "TargetU".Translate(), type, target, editorID );
                resolved                    = _Target.Resolve( failOnApplyIfUnresolved );

                if( ( failOnApplyIfUnresolved ) & ( !resolved ) )
                    throw new Exception( "Unable to resolve import target!" );
            }
            catch( Exception e )
            {   // Catch the inner exception and pass it to the caller
                throw new Exception( string.Format(
                    "{0} :: Exception parsing parameters\nParse Error:\n{1}\nInner Exception:\n{2}",
                    this.TypeFullName(),
                    _ErrorMessage,
                    e.ToString() ) );
            }
            _ImportState = resolved
                ? ImportStates.Resolved
                : ImportStates.Parsed;

        }

        public                                          ImportBase(
            string signature,
            Priority priority,
            bool failOnApplyIfUnresolved,
            Type type,
            IXHandle target,
            string editorID )
        {
            INTERNAL_Constructor( signature, priority, failOnApplyIfUnresolved, type, target, editorID, null );
        }

        public                                          ImportBase(
            string signature,
            Priority priority,
            bool failOnApplyIfUnresolved,
            ObjectReference target,
            string editorID,
            Cell cell )
        {
            INTERNAL_Constructor( signature, priority, failOnApplyIfUnresolved, typeof( ObjectReference ), target, editorID, cell );
        }

        #endregion

        #region Internal Error Monitoring

        protected void                                  ClearErrors()
        {
            _ErrorMessage = null;
            _ErrorState = false;
        }
        public void                                     AddErrorMessage( ErrorTypes type, string message, Exception e = null )
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
                DebugLog.WriteStrings( null, new [] { e.ToString() }, false, true, false, false, false );
            _ErrorState = true;
        }

        #endregion

        public virtual bool                             Resolve( bool errorIfUnresolveable )
        {
            ClearErrors();
            if( ImportState == ImportStates.Resolved ) return true;
            var resolved = _Target.Resolve( errorIfUnresolveable );
            var oCount = _Operations.Count;
            if( ( resolved )&&( oCount > 0 ) )
            {
                for( int i = 0; i < oCount; i++ )
                {
                    var opResult = _Operations[ i ].Resolve( errorIfUnresolveable );
                    if( ( errorIfUnresolveable )&&( !opResult ) )
                    {
                        resolved = false;
                        break;
                    }
                }
            }
            if( resolved ) _ImportState = ImportStates.Resolved;
            return resolved;
        }

        #region ISyncedListViewObject
        
        public Engine.Plugin.File[]                     Files
        {   // This should just be the list of targets which means just the working file...right...?
            get
            {
                var f = GodObject.Plugin.Data.Files.Working;
                return f == null
                    ? null
                    : new []{ f };
            }
        }
        
        public string[]                                 Filenames
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
        
        public uint                                     LoadOrder
        {
            get
            {
                return _Target.Value != null
                    ? _Target.Value.LoadOrder
                    : 0xFF;
            }
        }
        
        public uint                                     GetFormID( TargetHandle target )
        {
            return ( _Target == null )||( _Target.Value == null )
                ? Engine.Plugin.Constant.FormID_Invalid
                : _Target.Value.GetFormID( target );
        }
        public void                                     SetFormID( TargetHandle target, uint value )
        {
            throw new NotImplementedException();
        }
        
        public string                                   GetEditorID( TargetHandle target )
        {
            return ( Target.Value == null )
                ? Target.EditorID
                : Target.Value.GetEditorID( target );
        }
        public void                                     SetEditorID( TargetHandle target, string value )
        {
            throw new NotImplementedException();
        }
        
        public ConflictStatus                           ConflictStatus
        {
            get
            {
                if( !_Target.Value.Resolveable(_Target.FormID, _Target.EditorID ) )
                    return ConflictStatus.NewForm;
                
                var target = _Target.Value;
                if( target == null )
                    return ConflictStatus.NewForm;
                
                return !ImportDataMatchesTarget()
                    ? ConflictStatus.RequiresOverride
                    : ConflictStatus.NoConflict;
            }
        }
        
        public string                                   ExtraInfo
        {
            get
            {
                return _ErrorState
                    ? _ErrorMessage
                    : GetDisplayExtraInfo();
            }
        }
        
        public event EventHandler                       ObjectDataChanged;
        bool                                            _SupressEvents = false;
        
        public bool                                     ObjectDataChangedEventsSupressed
        {
            get
            {
                return _SupressEvents;
            }
        }

        public void                                     SupressObjectDataChangedEvents()
        {
            _SupressEvents = true;
            var targetSync = _Target.Value as ISyncedGUIObject;
            if( targetSync != null )
                targetSync.SupressObjectDataChangedEvents();
        }
        
        public void                                     ResumeObjectDataChangedEvents( bool sendevent )
        {
            //DebugLog.Write( string.Format( "{0} :: ResumeObjectDataChangedEvents() :: sendevent = {1}", this.FullTypeName(), sendevent ) );
            _SupressEvents = false;
            var targetSync = _Target.Value as ISyncedGUIObject;
            if( targetSync != null )
                targetSync.ResumeObjectDataChangedEvents( sendevent );
            if( sendevent ) SendObjectDataChangedEvent( this );
        }
        
        public void                                     SendObjectDataChangedEvent( object sender )
        {
            //DebugLog.Write( string.Format( "{0} :: SendObjectDataChangedEvent()", this.FullTypeName() ) );
            if( _SupressEvents ) return;
            ObjectDataChanged?.Invoke( sender, null );
        }
        
        public bool                                     InitialCheckedOrSelectedState()
        {
            var cs = ConflictStatus;
            return
                ( cs == ConflictStatus.NewForm )||
                ( cs == ConflictStatus.RequiresOverride );
        }
        
        public bool                                     ObjectChecked( bool checkedValue )
        {
            return checkedValue;
        }
        
        #endregion
        
        protected string                                GetDisplayExtraInfo()
        {
            Target.Resolve( false );
            var target = _Target.Value;
            
            if( ( target != null )&&( ImportDataMatchesTarget() ) )
                return "No changes";
            
            var prefix = ( target == null )
                ? "New Form: "
                : "Update Form: ";
            
            var tmp = new List<string>();

            var oCount = _Operations.Count;
            for( int i = 0; i < oCount; i++ )
            {
                var operation = _Operations[ i ];
                if(
                    ( !Target.IsResolved )||
                    ( !operation.Resolve( false ) )||
                    ( !operation.TargetMatchesImport() ) )
                {
                    var infos = operation.OperationalInformation();
                    if( !infos.NullOrEmpty() )
                        foreach( var info in infos )
                            tmp.Add( info );
                }
            }

            var tmp2 = GenIXHandle.ConcatDisplayInfo( tmp );
            return prefix + tmp2;
        }
        
        
        protected virtual bool                          ResolveReferenceForms( bool errorIfUnresolveable )
        {
            return true;
        }
        
        public virtual bool                             ImportDataMatchesTarget()
        {
            if( !_Target.Resolve( false ) ) return false;
            var oCount = _Operations.Count;
            int i;
            for( i = 0; i < oCount; i++ )
            {
                var operation = _Operations[ i ];

                if(
                    ( !operation.Resolve( false ) )||
                    ( !operation.TargetMatchesImport() )
                )   return false;
            }
            return true;
        }

        public bool                                     Apply( BatchImport importWindow )
        {
            DebugLog.OpenIndentLevel( Target.NullSafeIDString() );
            var result = false;

            DumpImport();

            _BatchWindow = importWindow;
            if( _ErrorState )
            {
                AddErrorMessage( ErrorTypes.Import, "Import in error state, cannot Apply()" );
                goto localAbort;
            }

            // Target may not resolve if this is a new form
            _Target.Resolve( false );

            if( _Target.Value == null )
            {
                var message = "Target.CreateNewFormInWorkingFile"; // TODO:  Add translation
                DebugLog.WriteLine( message );
                importWindow.AddImportMessage( message );
                if( !_Target.CreateNewFormInWorkingFile() )
                    goto localAbort;
            }
            else
            {
                var message = "Target.CopyToWorkingFile"; // TODO:  Add translation
                DebugLog.WriteLine( message );
                importWindow.AddImportMessage( message );
                if( !_Target.CopyToWorkingFile() )
                    goto localAbort;
            }
            
            SupressObjectDataChangedEvents();
            
            try
            {
                result = true;  // Assume at this point that everything is ok, that way any skipped operation does not false flag a failure
                DebugLog.OpenIndentLevel( new [] { this.TypeFullName(), "Apply Import Operations" }, false, true, false, false, true, false );
                var oCount = _Operations.Count;
                int i;
                for( i = 0; i < oCount; i++ )
                {
                    var operation = _Operations[ i ];

                    // Resolve this operations resources
                    result = operation.Resolve( true );
                    if( !result )
                    {
                        var message = operation.Signature + ": Unable to resolve additional operation references";
                        importWindow.AddImportMessage( message );
                        DebugLog.WriteLine( message );
                        break;
                    }

                    // Is this operation redundant?
                    if( !operation.TargetMatchesImport() )
                    {
                        // Apply the operation
                        importWindow.AddImportMessage( operation.Signature );
                        DebugLog.WriteLine( operation.Signature );
                        result = operation.Apply();
                    }
                    if( !result )
                    {
                        var message = operation.Signature + ": Unable to apply operation to target";
                        importWindow.AddImportMessage( message );
                        DebugLog.WriteLine( message );
                        break;
                    }
                }
                if( !result )
                {
                    var message = "Import operation failed\n" + _Operations[ i ].TypeFullName();
                    importWindow.AddImportMessage( message );
                    DebugLog.WriteLine( message );
                }
                DebugLog.CloseIndentLevel();
            }
            catch( Exception e )
            {
                result = false;
                AddErrorMessage( ErrorTypes.Import, "An unexpected exception has occured applying import!", e );
            }

            ResumeObjectDataChangedEvents( true );

            if( !result )
                DebugLog.WriteError( "Unable to apply import to the target form!" );
            
        localAbort:
            DebugLog.CloseIndentLevel( "result", result.ToString() );
            return result;
        }
        
        protected virtual void                          DumpImport()
        {
            var oCount = _Operations.Count;
            DebugLog.OpenIndentLevel();
            DebugLog.WriteStrings( null,
                new[] {
                    this            .Signature,
                    Target          .NullSafeIDString( "Target = {0}", "unresolved" ),
                    string          .Format( "Operations = {0}", oCount.ToString() )
                },
                false, true, false, false, false );
            for( int i = 0; i < oCount; i++ )
            {
                var operation = _Operations[ i ];
                DebugLog.OpenIndentLevel( operation.Signature, false );
                var infos = operation.OperationalInformation();
                if( !infos.NullOrEmpty() )
                    foreach( var info in infos )
                        DebugLog.WriteLine( info );
                DebugLog.CloseIndentLevel();
            }
            DebugLog.CloseIndentLevel();
        }

        #region Import List Management
        
        public static void                              AddToList( ref List<ImportBase> list, ImportBase i )
        {
            if( i == null ) return;
            if( list == null ) list = new List<ImportBase>();
            if( !list.NullOrEmpty() )
            {
                var iFID = i.GetFormID( TargetHandle.Master );
                var iFIDValid = iFID.ValidFormID();
                foreach( var li in list )
                {
                    if( li.Signature.InsensitiveInvariantMatch( i.Signature ) )
                    {
                        var liFID = li.GetFormID( TargetHandle.Master );
                        if( ( !iFIDValid )||( !liFID.ValidFormID() ) )
                        {
                            if( li.GetEditorID( TargetHandle.WorkingOrLastFullRequired ).InsensitiveInvariantMatch( i.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) ) )
                            {
                                //DebugLog.Write( string.Format( "\nReject :: \"{0}\" :: \"{1}\"", li.EditorID, i.EditorID ) );
                                return;
                            }
                        }
                        else if( iFID == liFID )
                        {
                            //DebugLog.Write( string.Format( "\nReject :: 0x{0} :: 0x{1}", li.FormID.ToString( "X8" ), i.FormID.ToString( "X8" ) ) );
                            return;
                        }
                    }
                }
            }
            DebugLog.OpenIndentLevel();
            i.DumpImport();
            DebugLog.CloseIndentLevel();
            list.Add( i );
        }
        
        public static void                              AddToList( ref List<ImportBase> list, List<ImportBase> otherList )
        {
            if( otherList.NullOrEmpty() ) return;
            foreach( var oli in otherList )
                AddToList( ref list, oli );
        }
        
        public static bool                              AllImportsMatchState( List<ImportBase> list, ImportStates state )
        {
            return
                ( !list.NullOrEmpty() )&&
                ( !list.Any( item =>( item.ImportState != state ) ) );
        }
        
        public static bool                              AllImportsMatchTarget( List<ImportBase> list )
        {
            return
                ( list.NullOrEmpty() )||
                ( list.All( item =>( item.ImportDataMatchesTarget() ) ) );
        }
        
        #endregion
        
        public override string                          ToString()
        {
            return string.Format( "[Signature = \"{0}\" :: Target = {1}]", Signature, _Target.NullSafeIDString() );
        }
        
        /// <summary>
        /// Open the import dialog and wait for it to exit.
        /// DO NOT CALL THIS FROM THE MAIN UI THREAD!
        /// </summary>
        /// <param name="importForms">Forms to import</param>
        /// <param name="enableControlsOnClose">Enable all forms when the dialog closes</param>
        /// <param name="allImportsMatchTarget">All import targets match import data on close</param>
        /// <returns></returns>
        public static bool                              ShowImportDialog(
            List<ImportBase> importForms,
            bool enableControlsOnClose,
            ref bool allImportsMatchTarget )
        {
            if( importForms.NullOrEmpty() ) return false;
            if( System.Threading.Thread.CurrentThread.ManagedThreadId == 1 )
                throw new Exception( "Cannot show BatchImportWindow from main thread!" );
            
            #region Import window

            var bbiw = GodObject.Windows.GetWindow<GUIBuilder.Windows.BatchImport>();
            bbiw.EnableControlsOnClose = enableControlsOnClose;
            bbiw.ImportForms = importForms;
            bbiw.ShowDialog();
            
            allImportsMatchTarget = bbiw.AllImportsMatchTarget;
            
            #endregion
            
            return true;
        }
        
    }
    
}
