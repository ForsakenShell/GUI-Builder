/*
 * Root.cs
 * 
 * Global tree of forms loaded.
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
using Engine.Plugin;
using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;

namespace GodObject
{
    
    public static partial class Plugin
    {
        
        public static partial class Data
        {
            
            public static partial class Root
            {
                
                #region Enumerate Root Containers
                
                //public static List<BaseForm> BaseForms = null;
                static List<Collection> _Collections = null;
                
                static Dictionary<uint, IXHandle> _ByFormID = null;
                static Dictionary<string, IXHandle> _ByEditorID = null;
                
                public static void      Dispose()
                {
                    if( _Collections != null )
                    {
                        foreach( var bfs in Root._Collections )
                            bfs.Dispose();
                        _Collections.Clear();
                        _Collections = null;
                    }
                    if( _ByFormID != null )
                    {
                        _ByFormID.Clear();
                        _ByFormID = null;
                    }
                    if( _ByEditorID != null )
                    {
                        _ByEditorID.Clear();
                        _ByEditorID = null;
                    }
                }
                
                public static Collection GetCollection<TSync>( bool createIfNeeded, bool doFullLoad )
                    where TSync : class, IXHandle
                {
                    return GetCollection( typeof( TSync ), createIfNeeded, doFullLoad );
                }
                
                public static Collection GetCollection( Type classType, bool createIfNeeded, bool doFullLoad )
                {
                    if( classType == null ) return null;
                    
                    var association = Reflection.AssociationFrom( classType );
                    if( association == null )
                        throw new Exception( string.Format( "GodObject.Plugin.Data.Root :: GetCollection() :: Cannot find Class Association from Type \"{0}\"!", classType.ToString() ) );
                    
                    return PrepCollection( association, createIfNeeded, doFullLoad );
                }
                
                public static Collection GetCollection( string signature, bool createIfNeeded, bool doFullLoad )
                {
                    if( string.IsNullOrEmpty( signature ) ) return null;
                    var association = Reflection.AssociationFrom( signature );
                    if( association == null )
                        throw new Exception( string.Format( "GodObject.Plugin.Data.Root :: GetCollection() :: Cannot find Class Association for Signature \"{0}\"!", signature ) );
                    
                    return PrepCollection( association, createIfNeeded, doFullLoad );
                }
                
                public static Collection GetCollection( ClassAssociation association, bool createIfNeeded, bool doFullLoad )
                {
                    return PrepCollection( association, createIfNeeded, doFullLoad );
                }
                
                static Collection       PrepCollection( ClassAssociation association, bool createIfNeeded, bool doFullLoad )
                {
                    if( !association.IsValid() ) return null;
                    if(
                        ( !association.AllowRootCollection )||
                        ( association.CollectionClassType == null )
                    )   return null;
                    
                    if( _Collections == null )
                        _Collections = new List<Collection>();
                    
                    var collection = _Collections.Find( (x) => ( x.Association == association ) );
                    if( ( collection == null )&&( createIfNeeded ) )
                    {
                        
                        collection = Activator.CreateInstance( association.CollectionClassType, new object[]{ association } ) as Collection;
                        if( collection == null )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "PrepCollection()", string.Format(
                                "Unable to create Collection Type {0} for Class Type {1}",
                                association.CollectionClassType == null ? "null" : association.CollectionClassType.ToString(),
                                association.ClassType == null ? "null" : association.ClassType.ToString()
                            ) );
                            return null;
                        }
                        _Collections.Add( collection );
                    }
                    
                    if( !collection.IsValid() ) return null;
                    
                    if( doFullLoad )
                    {
                        if( !collection.LoadAllForms() )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "PrepCollection()", string.Format( "Unable to LoadAllForms() for the root container of \"{0}\" Forms", collection.Association.Signature ) );
                        }
                        else if( !collection.PostLoad() )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "PrepCollection()", string.Format( "Unable to PostLoad() for the root container of \"{0}\" Forms", collection.Association.Signature ) );
                        }
                    }
                    
                    return collection;
                }
                
                #endregion
                
                #region Enumerate Root Collections
                
                public static void      AddToMasterTable( IXHandle syncObject )
                {
                    if( !syncObject.IsValid() ) return;
                    
                    if( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ValidFormID() )
                    {
                        if( _ByFormID == null )
                            _ByFormID = new Dictionary<uint, IXHandle>();
                        _ByFormID[ syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ) ] = syncObject;
                    }
                    
                    var eid = syncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    if( eid.ValidEditorID() )
                    {
                        if( _ByEditorID == null )
                            _ByEditorID = new Dictionary<string, IXHandle>();
                        var lEID = eid.ToLower();
                        _ByEditorID[ lEID ] = syncObject;
                    }
                }
                
                public static void      RemoveFromMasterTable( IXHandle syncObject )
                {
                    if( !syncObject.IsValid() ) return;
                    
                    if( _ByFormID != null )
                        _ByFormID.Remove( syncObject.GetFormID( Engine.Plugin.TargetHandle.Master ) );
                    
                    var eid = syncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    if( eid.ValidEditorID() )
                    {
                        if( _ByEditorID != null )
                            _ByEditorID.Remove( eid.ToLower() );
                    }
                }
                
                public static IXHandle  FindEx( ClassAssociation classAssociation, FormHandle handle = null, uint formid = 0, string editorid = null, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "FindEx()\n", "classAssociation = " + ( classAssociation == null ? "[null]" : classAssociation.ToString() ) + "\n", "handle = " + ( handle == null ? "[null]" : handle.ToString() ) + "\n", "formid = 0x" + formid.ToString( "X8" ) + "\n", "editorid = \"" + editorid + "\"\n", "tryLoad = " + tryLoad.ToString() } );
                    bool handleValid = handle.IsValid();
                    if( handleValid )
                    {
                        if( formid == 0 ) formid = handle.FormID;
                        if( string.IsNullOrEmpty( editorid ) ) editorid = handle.EditorID;
                    }
                    bool searchByFormID = Engine.Plugin.Constant.ValidFormID( formid );
                    bool searchByEditorID = Engine.Plugin.Constant.ValidEditorID( editorid );
                    
                    IXHandle result = null;
                    
                    if( searchByFormID )
                    {
                        // Fastest search is if Class and FormID are known
                        if( _ByFormID.TryGetValue( formid, out result ) )
                            goto localReturnResult;
                    }
                    else if( searchByEditorID )
                    {
                        // Check the master EditorID dictionary
                        if( ( _ByEditorID != null )&&( _ByEditorID.TryGetValue( editorid.ToLower(), out result ) ) )
                            goto localReturnResult;
                    }
                    else if( !tryLoad )
                        goto localReturnResult;
                    
                    FormHandle record = null;
                    
                    if( searchByFormID )
                    {
                        // Get the record from plugin
                        record = ( handleValid )
                            ? handle.GetMasterRecord()
                            : Records.FindMasterRecordEx( ElementHandle.BaseXHandleValue, formid, false );
                        if( record == null )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "FindEx()", string.Format( "Unable to find formid 0x{0} in loaded files\n}}", formid.ToString( "X8" ) ) );
                            goto localReturnResult;
                        }
                        var recordAssociation = Reflection.AssociationFrom( record.Signature );
                        if( ( classAssociation.IsValid() )&&( recordAssociation.IsValid() )&&( classAssociation != recordAssociation ) )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "FindEx()", string.Format( "Invalid association, code expected \"{0}\", record is \"{1}\"\n}}", classAssociation.Signature, recordAssociation.Signature ) );
                            record.Dispose();
                            goto localReturnResult;
                        }
                        
                        // Build the tree of forms
                        var refTree = record.GetContainerRecordTree();
                        
                        // Load the form tree
                        var forms = LoadRecordTree( refTree );
                        DisposeOfHandlesNotUsedByObjects( refTree, forms );
                        
                        result = forms.NullOrEmpty()
                            ? null
                            : forms.Last(); // Last form is the one we want
                        
                        goto localReturnResult;
                    }
                    
                    #region Unknown target type search, look through all records for the EditorID
                    
                    if( !classAssociation.IsValid() )
                    {
                        if( _Collections.NullOrEmpty() )
                            goto localReturnResult;
                        
                        foreach( var collection in _Collections )
                        {
                            result = collection.FindEx( null, handle, formid, editorid, tryLoad );
                            if( result != null )
                                goto localReturnResult;
                        }
                        
                        return null;
                    }
                    
                    #endregion
                    
                    #region  Known target type search, look through only records of the appropriate type for the EditorID
                    
                    if( classAssociation.AllowRootCollection )
                    {
                        var collection = GetCollection( classAssociation, true, false );
                        if( !collection.IsValid() )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "FindEx()", string.Format( "Unable to GetCollection for Association \"{0}\" :: 0x{1}", classAssociation.Signature, formid.ToString( "X8" ) ) );
                            goto localReturnResult;
                        }
                        result = collection.FindEx( classAssociation, handle, formid, editorid, tryLoad );
                        if( result != null )
                            goto localReturnResult;
                    }
                    
                    var rootAssociations = Reflection.ParentAssociationsOf( classAssociation );
                    if( rootAssociations.NullOrEmpty() )
                        goto localReturnResult;
                    
                    foreach( var rootAssociation in rootAssociations )
                    {
                        var collection = GetCollection( rootAssociation, true, false );
                        if( !collection.IsValid() )
                        {
                            DebugLog.WriteError( "GodObject.Plugin.Data.Root", "FindEx()", string.Format( "Unable to GetCollection for Association \"{0}\" :: 0x{1}", rootAssociation.Signature, formid.ToString( "X8" ) ) );
                            goto localReturnResult;
                        }
                        result = collection.FindEx( classAssociation, handle, formid, editorid, tryLoad );
                        if( result != null )
                            goto localReturnResult;
                    }
                    
                    #endregion
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<IDataSync>( result );
                    return result;
                }
                
                public static List<IXHandle> LoadRecordTree( FormHandle[] refTree )
                {
                    if( refTree.NullOrEmpty() ) return null;
                    
                    //DebugLog.OpenIndentLevel( "GodObject.Plugin.Data.Root :: LoadRecordTree()" );
                    //DebugLog.WriteArray( "refTree", refTree );
                    
                    List<IXHandle> resultList = null;
                    
                    // The first record handle should be a root record, get it's association
                    var rootRecord = refTree[ 0 ];
                    var rootAssociation = Reflection.AssociationFrom( rootRecord.Signature );
                    if( !rootAssociation.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to get Association for root record " + rootRecord.ToString() );
                        goto localReturnResult;
                    }
                    
                    // Prep the root container
                    var rootCollection = PrepCollection( rootAssociation, true, false );
                    if( !rootCollection.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to PrepCollection() for root record " + rootRecord.ToString() );
                        goto localReturnResult;
                    }
                    
                    // Find the root form
                    var rootForm = rootCollection.FindEx( rootAssociation, rootRecord, rootRecord.FormID, rootRecord.EditorID, true );
                    if( !rootForm.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to FindEx() for root record " + rootRecord.ToString() );
                        goto localReturnResult;
                    }
                    
                    // Add the root form to the result
                    resultList = new List<IXHandle>();
                    resultList.Add( rootForm );
                    
                    // Next form starts as the root form
                    var nextForm = rootForm;
                    var count = refTree.Length;
                    if( count > 1 )
                    {
                        // Go through each level of the record tree loading each form
                        for( int i = 1; i < count; i++ )
                        {
                            var lastForm = resultList[ i - 1 ];
                            var nextRecord = refTree[ i ];
                            
                            var nextAssociation = Reflection.AssociationFrom( nextRecord.Signature );
                            if( !nextAssociation.IsValid() )
                            {
                                DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to get Association for child record " + nextRecord.ToString() + "\nIn parent form " + lastForm.ToString() );
                                goto localReturnResult;
                            }
                            var nextCollection = lastForm.CollectionFor( nextAssociation );
                            if( !nextCollection.IsValid() )
                            {
                                DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to get CollectionFor() for child record " + nextRecord.ToString() + "\nIn parent form " + lastForm.ToString() );
                                goto localReturnResult;
                            }
                            
                            nextForm = nextCollection.FindEx( nextAssociation, nextRecord, nextRecord.FormID, nextRecord.EditorID, true );
                            if( !nextForm.IsValid() )
                            {
                                DebugLog.WriteError( "GodObject.Plugin.Data.Root", "LoadRecordTree()", "Unable to FindEx() for child record " + nextRecord.ToString() + "\nIn parent form " + lastForm.ToString() );
                                goto localReturnResult;
                            }
                            
                            // Add the form from this collection
                            resultList.Add( nextForm );
                        }
                    }
                    
                    // Now return the forms from the records
                localReturnResult:
                    //DebugLog.CloseIndentLevel<IXHandle>( "IXHandle tree", resultList );
                    return resultList.NullOrEmpty()
                        ? null
                        : resultList;
                }
                
                public static void      DisposeOfHandlesNotUsedByObjects( FormHandle[] handles, List<IXHandle> syncObjects )
                {
                    if( handles.NullOrEmpty() ) return;
                    //DebugLog.OpenIndentLevel( "GodObject.Plugin.Data.Root.DisposeOfHandlesNotUsedByObjects()" );
                    foreach( var h in handles )
                    {
                        if( syncObjects.NullOrEmpty() )
                        {
                            //DebugLog.WriteLine( new [] { "Dispose", "Empty syncObjectList{", h.ToString(), "}" } );
                            h.Dispose();
                        }
                        else
                        {
                            if( !syncObjects.Any( (f)=> f.IsHandleFor( h ) ) )
                            {
                                //DebugLog.WriteLine( new [] { "Dispose", "Unused handle{", h.ToString(), "}" } );
                                h.Dispose();
                            }
                        }
                    }
                    //DebugLog.CloseIndentLevel();
                }
                
                public static IXHandle  Find( XeLib.FormHandle handle, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find()\n", "handle = " + handle.ToString() + "\n", "tryLoad = " + tryLoad.ToString() } );
                    
                    IXHandle result = null;
                    
                    if( !handle.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find()", "handle !IsValid()" );
                        goto localReturnResult;
                    }
                    
                    // Handle FormID is in the master table?
                    if( _ByFormID != null )
                    {
                        var hFID = handle.FormID;
                        if( ( hFID.ValidFormID() )&&( _ByFormID.TryGetValue( hFID, out result ) ) )
                            goto localReturnResult;
                    }
                    
                    // Handle EditorID is in the master table?
                    if( _ByEditorID != null )
                    {
                        var hEID = handle.EditorID;
                        if( ( hEID.ValidEditorID() )&&( _ByEditorID.TryGetValue( hEID, out result ) ) )
                            goto localReturnResult;
                    }
                    
                    if( !tryLoad ) goto localReturnResult;
                    
                    // Get parent container from handle, work through
                    // ancestory back to the root container and build
                    // the tree as needed to this point.
                    //handle.DumpContainerTree();
                    
                    var association = Reflection.AssociationFrom( handle.Signature );
                    if( !association.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find()", string.Format(
                            "Unable to get Association for Collection from {0}",
                            handle.ToString() ) );
                        goto localReturnResult;
                    }
                    
                    // Build the tree of forms
                    var refTree = handle.GetContainerRecordTree();
                    
                    // Load the form tree
                    var forms = LoadRecordTree( refTree );
                    DisposeOfHandlesNotUsedByObjects( refTree, forms );
                    
                    result = forms.NullOrEmpty()
                        ? null
                        : forms.Last(); // Last form is the one we want
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<IXHandle>( "result", result );
                    return result;
                }
                
                public static IXHandle  Find( uint formID, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find()\n", "formID = 0x" + formID.ToString( "X8" ) + "\n", "tryLoad = " + tryLoad.ToString() } );
                    var result = FindEx( null, null, formID, null, tryLoad );
                //localReturnResult:
                    //DebugLog.CloseIndentLevel<IDataSync>( result );
                    return result;
                }
                
                public static IXHandle  Find( string signature, uint formID, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find()\n", "signature = \"" + signature + "\"\n", "formID = 0x" + formID.ToString( "X8" ) + "\n", "tryLoad = " + tryLoad.ToString() } );
                    
                    IXHandle result = null;
                    
                    var association = Reflection.AssociationFrom( signature );
                    if( !association.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find()", "Unable to get Association from Signature \"" + signature + "\"" );
                        goto localReturnResult;
                    }
                    result = FindEx( association, null, formID, null, tryLoad );
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<IDataSync>( result );
                    return result;
                }
                
                public static TSync     Find<TSync>( uint formID, bool tryLoad = true ) where TSync : class, IXHandle
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find<" + typeof( TSync ).ToString() + ">()\n", "formID = 0x" + formID.ToString( "X8" ) + "\n", "tryLoad = " + tryLoad.ToString() } );
                    
                    TSync result = null;
                    
                    var type = typeof( TSync );
                    var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
                    if( !association.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find<TSync>()", string.Format( "Unable to get Association from {0}", ( type == null ? "null" : type.ToString() ) ) );
                        goto localReturnResult;
                    }
                    result = FindEx( association, null, formID, null, tryLoad ) as TSync;
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<TSync>( result );
                    return result;
                }
                
                public static IXHandle  Find( string editorID, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find()\n", "editorID = \"" + editorID + "\"\n", "tryLoad = " + tryLoad.ToString() } );
                    var result = FindEx( null, null, 0, editorID, tryLoad );
                //localReturnResult:
                    //DebugLog.CloseIndentLevel<IDataSync>( result );
                    return result;
                }
                
                public static IXHandle  Find( string signature, string editorID, bool tryLoad = true )
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find()\n", "signature = \"" + signature + "\"\n", "editorID = \"" + editorID + "\"\n", "tryLoad = " + tryLoad.ToString() } );
                    IXHandle result = null;
                    
                    var association = Reflection.AssociationFrom( signature );
                    if( !association.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find()", string.Format( "Unable to get Association from Signature \"{0}\"", signature ) );
                        goto localReturnResult;
                    }
                    result = FindEx( association, null, 0, editorID, tryLoad );
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<IDataSync>( result );
                    return result;
                }
                
                public static TSync     Find<TSync>( string editorID, bool tryLoad = true ) where TSync : class, IXHandle
                {
                    //DebugLog.OpenIndentLevel( new [] { "GodObject.Plugin.Data.Root", "Find<" + typeof( TSync ).ToString() + ">()\n", "editorID = \"" + editorID + "\"\n", "tryLoad = " + tryLoad.ToString() } );
                    
                    TSync result = null;
                    
                    var type = typeof( TSync );
                    var association = Engine.Plugin.Attributes.Reflection.AssociationFrom( type );
                    if( !association.IsValid() )
                    {
                        DebugLog.WriteError( "GodObject.Plugin.Data.Root", "Find<TSync>()", string.Format( "Unable to get Association from {0}", ( type == null ? "null" : type.ToString() ) ) );
                        goto localReturnResult;
                    }
                    result = FindEx( association, null, 0, editorID, tryLoad ) as TSync;
                    
                localReturnResult:
                    //DebugLog.CloseIndentLevel<TSync>( result );
                    return result;
                }
                
                #endregion
                
            }
            
        }
        
    }
    
}