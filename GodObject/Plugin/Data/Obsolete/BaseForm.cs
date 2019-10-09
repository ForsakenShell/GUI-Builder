/*
 * BaseForm.cs
 * 
 * OBSOLETE - ONLY HERE FOR REFERENCE WHILE WRITING THE REPLACEMENT GENERIC CODE
 * 
 * Global flattened data.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Fallout4;
using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;
using XeLibHelper;
using Engine.Plugin.Interface;

#if OLD_CODE
namespace GodObject
{
    
    public static partial class Plugin
    {
        
        public static partial class Data
        {
            
            public static partial class BaseFormData
            {
                
                #region Base Forms Container Class
                
                //public class BaseForms<TForm> : IDisposable
                //    where TForm : Engine.Plugin.Form
                public class BaseForm : IDisposable, IContainer
                {
                    
                    Engine.Plugin.Attributes.FormAttributes _FormAttributes = null;
                    
                    List<IDataSync> _AllForms = null;
                    Dictionary<uint,IDataSync> _ByFormID = null;
                    Dictionary<string,IDataSync> _ByEditorID = null;
                    
                    #region Allocation
                    
                    public BaseForm( Engine.Plugin.Attributes.FormAttributes formAttributes )
                    {
                        _FormAttributes = formAttributes;
                    }
                    
                    #endregion
                    
                    #region Disposal
                    
                    protected bool Disposed = false;
                    
                    ~BaseForm()
                    {
                        Dispose( true );
                    }
                    
                    public void Dispose()
                    {
                        Dispose( true );
                    }
                    
                    protected virtual void Dispose( bool disposing )
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
                        
                        _FormAttributes = null;
                        
                        Disposed = true;
                    }
                    
                    #endregion
                    
                    public bool IsValid
                    {
                        get
                        {
                            return
                                ( _FormAttributes != null )&&
                                ( !string.IsNullOrEmpty( _FormAttributes.Signature ) )&&
                                ( _FormAttributes.FormType != null );
                        }
                    }
                    
                    public Type ObjectType { get { return IsValid ? _FormAttributes.FormType : null; } }
                    public string Signature { get { return IsValid ? _FormAttributes.Signature : null; } }
                    
                    public IDataSync Ancestor { get { return null; } }
                    public Engine.Plugin.Form ParentForm { get { return null; } }
                    
                    #region Form Management
                    
                    public bool LoadAllForms()
                    {
                        return false;
                    }
                    
                    public bool LoadFrom( IDataSync source )
                    {
                        return false;
                        /*
                        if( source == null )
                            return false;
                        
                        if( _Container == null )
                            _Container = new Engine.Plugin.Containers.Statics( null );
                        
                        return _Container.LoadFrom( source );
                        */
                    }
                    
                    public bool PostLoad()
                    {
                        return false;
                    }
                    
                    //public TForm _AddFromRecord<TForm>( Engine.Plugin.File mod, Handle handle )
                    //    where TForm : class, IDataSync
                    public IDataSync _AddFromRecord( Engine.Plugin.File mod, Handle handle )
                    {
                        if( !IsValid )
                        {
                            DebugLog.Write( string.Format(
                                "{0} :: _AddFromRecord() :: Container FormAttributes are incorrect!", this.GetType().ToString() ) );
                            return null;
                        }
                        
                        if( !handle.IsValid() )
                        {
                            DebugLog.Write( string.Format(
                                "{0} :: _AddFromRecord() :: Invalid handle! :: 0x{1}", this.GetType().ToString(), handle.ToString() ) );
                            return null;
                        }
                        
                        var rSig = handle.Signature;
                        if( rSig != _FormAttributes.Signature )
                        {
                            DebugLog.Write( string.Format(
                                "{0} :: _AddFromRecord() :: Invalid signature! :: Expected \"{1}\" got \"{2}\"!", this.GetType().ToString(), _FormAttributes.Signature, rSig ) );
                            return null;
                        }
                        
                        var syncObject = Activator.CreateInstance( _FormAttributes.FormType, new Object[] { this, mod, handle } ) as IDataSync;
                        if( syncObject == null )
                        {
                            DebugLog.Write( string.Format(
                                "{0} :: _AddFromRecord() ::  Unable to create new {1}!", this.GetType().ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        
                        //Setup.BuildReferences( handle, false ); // Will crash XEditLib
                        
                        if( !syncObject.Load() )
                        {
                            DebugLog.Write( string.Format(
                                "{0} :: _AddFromRecord() :: Form 0x{1} - \"{2}\" :: {3}.Load() returned false!",
                                this.GetType().ToString(),
                                syncObject.FormID.ToString( "X8" ), syncObject.EditorID,
                                _FormAttributes.FormType.ToString()
                            ) );
                            return null;
                        }
                        
                        // TODO:  Get override records and apply them to the sync object
                        
                        Add( syncObject );
                        
                        /*
                        //const uint testFID = 0x010A70F1;
                        const uint testFID = 0x01006AD9;
                        if( form.FormID == testFID )
                        {
                            DebugLog.Write( string.Format(
                                "\n{0} :: _AddFromRecord()",
                                this.GetType().ToString() ) );
                            form.DebugDump();
                        }
                        */
                        
                        return syncObject;
                    }
                    
                    public TSync CreateNew<TSync>() where TSync : class, IDataSync
                    {
                        if( !IsValid )
                        {
                            DebugLog.Write( string.Format( "{0} :: CreateNew<TSync>() :: Container FormAttributes are incorrect!", this.GetType().ToString() ) );
                            return null;
                        }
                        if( typeof( TSync ) != _FormAttributes.FormType )
                        {
                            DebugLog.Write( string.Format( "{0} :: CreateNew<TSync>() :: TSync does not match Form Type :: {1} != {2}", this.GetType().ToString(), typeof( TSync ).ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        return CreateNew() as TSync;
                    }
                    
                    public IDataSync CreateNew()
                    {
                        if( !IsValid )
                        {
                            DebugLog.Write( string.Format( "{0} :: CreateNew() :: Container FormAttributes are incorrect!", this.GetType().ToString() ) );
                            return null;
                        }
                        
                        if( Ancestor == null )
                            return CreateNewRootForm();
                        return null;
                        /*return ParentForm == null
                            ? null
                            : CreateNewChildForm();*/
                    }
                    
                    IDataSync CreateNewRootForm()
                    {
                        var wf = Files.Working;
                        if( wf == null )
                        {
                            DebugLog.Write( string.Format( "\n{0} :: CreateNewRootForm() :: No working file loaded, unable to create/inject forms!", this.GetType().ToString() ) );
                            return null;
                        }
                        var wfch = Elements.GetElement( wf.OverrideHandle, _FormAttributes.Signature );
                        if( !wfch.IsValid() )
                            wfch = Elements.AddElement( wf.OverrideHandle, _FormAttributes.Signature );
                        if( !wfch.IsValid() )
                        {
                            DebugLog.Write( string.Format(
                                "\n{0} :: CreateNewRootForm()\n\t***** Unable to GetElement or AddElement() for form container! *****\n\tSignature = \"{1}\"\n\tWorkingFileName = \"{2}\"",
                                this.GetType().ToString(),
                                _FormAttributes.Signature,
                                wf.Filename ) );
                            return null;
                        }
                        
                        var handle = Elements.AddElement( wfch, _FormAttributes.Signature );
                        if( !handle.IsValid() )
                        {
                            DebugLog.Write( string.Format(
                                "\n{0} :: CreateNewRootForm()\n\t***** Unable to AddElement() for new form! *****\n\tSignature = \"{1}\"\n\tWorkingFileName = \"{2}\"",
                                this.GetType().ToString(),
                                _FormAttributes.Signature,
                                wf.Filename ) );
                            return null;
                        }
                        
                        //var form = (TForm)Activator.CreateInstance( typeof( TForm ), new Object[] { wf, wf, handle } );
                        //var syncObject = Engine.Plugin.Form.CreateForm<TResult>( this, wf, handle );
                        var syncObject = Activator.CreateInstance( _FormAttributes.FormType, new Object[] { this, wf, handle } ) as IDataSync;
                        if( syncObject == null )
                        {
                            DebugLog.Write( string.Format( "\n{0} :: CreateNewRootForm() :: Unable to create new {1}!", this.GetType().ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        
                        Add( syncObject );
                        
                        // Hand the new form back to the caller
                        return syncObject;
                    }
                    
                    public bool Add( IDataSync syncObject )
                    {
                        if( !IsValid ) return false;
                        if( syncObject == null ) return false;
                        if( !_FormAttributes.FormType.IsInstanceOfType( syncObject ) )
                        {
                            DebugLog.Write( string.Format(
                                "\n{0} :: AddForm() :: Invalid Form Type! :: \"{1}\"", this.GetType().ToString(), syncObject.GetType().ToString() ) );
                            var st = new System.Diagnostics.StackTrace();
                            DebugLog.Write( "Stack Trace:\n" + st.ToString() );
                            return false;
                        }
                        
                        if( _AllForms == null )
                            _AllForms = new List<IDataSync>();
                        _AllForms.AddOnce( syncObject );
                        
                        if( Engine.Plugin.Constant.ValidFormID( syncObject.FormID ) )
                        {
                            _ByFormID = _ByFormID ?? new Dictionary<uint, IDataSync>();
                            _ByFormID[ syncObject.FormID ] = syncObject;
                        }
                        
                        var soEDID = syncObject.EditorID;
                        if( Engine.Plugin.Constant.ValidEditorID( soEDID ) )
                        {
                            _ByEditorID = _ByEditorID ?? new Dictionary<string, IDataSync>();
                            _ByEditorID[ soEDID.ToLower() ] = syncObject;
                        }
                        
                        return true;
                    }
                    
                    public void Remove( IDataSync syncObject )
                    {
                        if( !IsValid ) return;
                        if( syncObject == null ) return;
                        if( !_FormAttributes.FormType.IsInstanceOfType( syncObject ) ) return;
                        
                        if( _AllForms != null )
                            _AllForms.Remove( syncObject );
                        
                        if( ( Engine.Plugin.Constant.ValidFormID( syncObject.FormID ) )&&( _ByFormID != null ) )
                            _ByFormID.Remove( syncObject.FormID );
                        
                        var soEDID = syncObject.EditorID;
                        if( ( Engine.Plugin.Constant.ValidEditorID( soEDID ) )&&( _ByEditorID != null ) )
                            _ByEditorID.Remove( soEDID.ToLower() );
                    }
                    
                    #endregion
                    
                    #region Instance Enumeration
                    
                    public int Count
                    {
                        get
                        {
                            return _AllForms.NullOrEmpty()
                                ? 0
                                : _AllForms.Count;
                        }
                    }
                    
                    public List<IDataSync>  ToList( int loadOrderFilter = -1 )
                    {
                        if( !IsValid ) return null;
                        if( _AllForms.NullOrEmpty() ) return null;
                        
                        var result = ( loadOrderFilter >=0 )&&( loadOrderFilter <= GodObject.Plugin.Data.Files.Working.LoadOrder )
                            ? _AllForms.Where( (x) => ( x.LoadOrder == loadOrderFilter ) ).ToList()
                            : _AllForms;
                        
                        return result.NullOrEmpty()
                            ? null
                            : result;
                    }
                    
                    public List<TSync>      ToList<TSync>( int loadOrderFilter = -1 ) where TSync : class, IDataSync
                    {
                        if( !IsValid ) return null;
                        if( typeof( TSync ) != _FormAttributes.FormType )
                        {
                            DebugLog.Write( string.Format( "{0} :: ToList<TSync>() :: TSync does not match Form Type :: {1} != {2}", this.GetType().ToString(), typeof( TSync ).ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        if( _AllForms.NullOrEmpty() ) return null;
                        
                        var items = ( loadOrderFilter >=0 )&&( loadOrderFilter <= GodObject.Plugin.Data.Files.Working.LoadOrder )
                            ? _AllForms.Where( (x) => ( x.LoadOrder == loadOrderFilter ) )
                            : _AllForms;
                        
                        return ( items == null )||( !items.Any() )
                            ? null
                            : items.ToList().ConvertAll( (x) => ( x as TSync ) );
                    }
                    
                    public IDataSync        Find( uint formid, bool tryLoad = true )
                    {
                        if( !IsValid ) return null;
                        if( _AllForms.NullOrEmpty() ) return null;
                        if( !Engine.Plugin.Constant.ValidFormID( formid ) )
                            return null;
                        
                        if( _ByFormID == null )
                            _ByFormID = new Dictionary<uint, IDataSync>();
                        
                        IDataSync result = null;
                        if( !_ByFormID.TryGetValue( formid, out result ) )
                        {
                            var record = Handle.FindRecord( formid );
                            if( !record.IsValid() )
                            {
                                DebugLog.Write( string.Format( "{0} :: Find() :: Unable to get record for FormID 0x{1}!", this.GetType().ToString(), formid.ToString( "X8" ) ) );
                                return null;
                            }
                            var hMaster = Handle.FindMasterRecord( ref record );
                            if( !hMaster.IsValid() )
                            {
                                DebugLog.Write( string.Format( "{0} :: Find() :: Unable to get master record for FormID 0x{1}!", this.GetType().ToString(), formid.ToString( "X8" ) ) );
                                return null;
                            }
                            
                            var file = Files.Find( hMaster );
                            if( file == null )
                            {
                                DebugLog.Write( string.Format( "{0} :: GetByFormID() :: Unable to get file for FormID 0x{1} from record 0x{2}!", this.GetType().ToString(), formid.ToString( "X8" ), record.ToString() ) );
                                return null;
                            }
                            
                            // Create a new data sync object of the appropriate type
                            result = _AddFromRecord( file, hMaster );
                            if( result == null )
                            {
                                DebugLog.Write( string.Format( "{0} :: GetByFormID() :: Unable to create {1} for FormID 0x{2} from record 0x{2}!", this.GetType().ToString(), _FormAttributes.FormType.ToString(), formid.ToString( "X8" ), record.ToString() ) );
                                return null;
                            }
                            
                        }
                        
                        return result;
                    }
                    
                    public TSync            Find<TSync>( uint formid, bool tryLoad = true ) where TSync : class, IDataSync
                    {
                        if( !IsValid ) return null;
                        if( typeof( TSync ) != _FormAttributes.FormType )
                        {
                            DebugLog.Write( string.Format( "{0} :: Find<TSync>() :: TSync does not match Form Type :: {1} != {2}", this.GetType().ToString(), typeof( TSync ).ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        var syncObject = Find( formid );
                        return syncObject as TSync;
                    }
                    
                    public IDataSync        Find( string editorid, bool tryLoad = true )
                    {
                        if( !IsValid ) return null;
                        if( _AllForms.NullOrEmpty() ) return null;
                        if( ( _ByEditorID == null )||( string.IsNullOrEmpty( editorid ) ) )
                            return null;
                        
                        IDataSync result = null;
                        _ByEditorID.TryGetValue( editorid.ToLower(), out result );
                        
                        // TODO:  Scan files for editor id - SLOW, implement a sig + edid version
                        
                        return result;
                    }
                    
                    public TSync            Find<TSync>( string editorid, bool tryLoad = true ) where TSync : class, IDataSync
                    {
                        if( !IsValid ) return null;
                        if( typeof( TSync ) != _FormAttributes.FormType )
                        {
                            DebugLog.Write( string.Format( "{0} :: Find<TSync>() :: TSync does not match Form Type :: {1} != {2}", this.GetType().ToString(), typeof( TSync ).ToString(), _FormAttributes.FormType.ToString() ) );
                            return null;
                        }
                        var syncObject = Find( editorid );
                        return syncObject as TSync;
                    }
                    
                    #endregion
                    
                }
                
                #endregion
                
                #region Old Code
                #if OLD_CODE
                
                public static int CountForms<TForm>()
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( false );
                    return bfs == null
                        ? 0
                        : bfs.Count;
                }
                
                public static TForm AddFromRecord<TForm>( Engine.Plugin.File mod, Handle handle )
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( true );
                    return bfs == null
                        ? null
                        : bfs.AddFromRecord( mod, handle ) as TForm;
                }
                
                public static TForm CreateNew<TForm>()
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( true );
                    return bfs == null
                        ? null
                        : bfs.CreateNew<TForm>();
                }
                
                public static void Add<TForm>( TForm form )
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( true );
                    if( bfs == null )
                        return;
                    bfs.Add( form );
                }
                
                public static List<TForm> ToList<TForm>( int loadOrderFilter = -1 )
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( false );
                    return bfs == null
                        ? null
                        : bfs.ToList( loadOrderFilter ).ConvertAll( (x) => ( x as TForm ) );
                }
                
                public static TForm Find<TForm>( uint formid )
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( true );
                    return bfs == null
                        ? null
                        : bfs.Find( formid ) as TForm;
                }
                
                public static TForm Find<TForm>( string editorid )
                    where TForm : Engine.Plugin.Form
                {
                    var bfs = GetOrCreateBaseFormContainerByType<TForm>( false );
                    return bfs == null
                        ? null
                        : bfs.Find( editorid ) as TForm;
                }
                
                public static Engine.Plugin.Form Find( uint formid )
                {
                    if( BaseForms.NullOrEmpty() )
                        return null;
                    foreach( var bfs in BaseForms )
                    {
                        var result = bfs.Find( formid ) as Engine.Plugin.Form;
                        if( result != null )
                            return result;
                    }
                    return null;
                }
                
                public static Engine.Plugin.Form Find( string editorid )
                {
                    if( BaseForms.NullOrEmpty() )
                        return null;
                    foreach( var bfs in BaseForms )
                    {
                        var result = bfs.Find( editorid ) as Engine.Plugin.Form;
                        if( result != null )
                            return result;
                    }
                    return null;
                }
                
                #endif
                #endregion
                
            }
            
        }
        
    }
    
}
#endif
