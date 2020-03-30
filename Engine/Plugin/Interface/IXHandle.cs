/*
 * IXHandle.cs
 *
 * Interface for XeLib backed classes.
 *
 */
using System;
using System.Collections.Generic;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Interface
{
    
    // Notes:
    
    // Signature:
    // For forms this will be the actual form signature - "STAT", "WRLD", "CELL", etc
    // For scripts this will be the fully qualified scriptname - "WorkshopScript", "ESM:ATC:SubDivision", etc
    // For files, this will be the filename
    
    /// <summary>
    /// Description of IXHandle.
    /// </summary>
    public interface IXHandle //IDataSync
    {
        
        int                             GetHashCode();
        string                          ToString();

        string                          IDString                    { get; }

        #region Required Properties
        
        string                          Signature                   { get; }
        
        IXHandle                        Ancestor                    { get; }
        
        Plugin.File[]                   Files                       { get; }
        string[]                        Filenames                   { get; }
        string                          GetFilename( Engine.Plugin.TargetHandle target );

        uint                            LoadOrder                   { get; }
        
        //uint                            FormID                      { get; }
        //string                          EditorID                    { get; }
        
        uint                            GetFormID( Engine.Plugin.TargetHandle target );
        void                            SetFormID( Engine.Plugin.TargetHandle target, uint value );
        
        string                          GetEditorID( Engine.Plugin.TargetHandle target );
        void                            SetEditorID( Engine.Plugin.TargetHandle target, string value );
        
        //Engine.Plugin.Forms.Fields.Record.FormID FormID             { get; }
        //Engine.Plugin.Forms.Fields.Record.EditorID EditorID         { get; }
        
        ConflictStatus                  ConflictStatus              { get; }
        
        #endregion
        
        #region Un/Loading
        
        bool                            Load();
        bool                            PostLoad();
        
        void                            Dispose();
        
        #endregion
        
        #region XeLib Handles
        
        #region Files
        
        ElementHandle                   CopyAsOverride();
        
        bool                            IsInWorkingFile();
        bool                            IsInFile( Plugin.File file );
        
        bool                            IsModifiedIn( Plugin.File file );
        
        ElementHandle                   HandleFor( Plugin.File file );
        
        #endregion
        
        #region Raw Handles
        
        bool                            IsHandleFor( ElementHandle handle );
        
        bool                            AddNewHandle( ElementHandle newHandle );
        
        ElementHandle                   MasterHandle                { get; }
        ElementHandle                   WorkingFileHandle           { get; }
        
        ElementHandle                   LastFullRequiredHandle      { get; }
        ElementHandle                   LastFullOptionalHandle      { get; }
        ElementHandle                   LastHandleBeforeWorkingFile { get; }
        
        List<ElementHandle>             Handles                     { get; }
        
        #endregion
        
        #endregion
        
        #region Parent/Child collection[s]
        
        #region Parent container collection
        
        Collection                      ParentCollection            { get; set; }
        
        #endregion
        
        #region Child collections
        
        void                            AddCollection( Collection collection );
        
        Collection                      CollectionFor( string signature );
        Collection                      CollectionFor<TSync>() where TSync : class, IXHandle;
        Collection                      CollectionFor( Attributes.ClassAssociation association );
        
        List<Collection>                ChildCollections           { get; }
        
        #endregion
        
        #endregion
        
    }
    
    public static partial class Extensions
    {
        
        public static bool              IsValid( this IXHandle o )
        {
            return
                ( o != null )&&
                ( !o.Handles.NullOrEmpty() );
                //( !string.IsNullOrEmpty( o.Signature ) )&&
                //( o.WorkingFileHandle.IsValid() );
        }
        
    }
    
}
