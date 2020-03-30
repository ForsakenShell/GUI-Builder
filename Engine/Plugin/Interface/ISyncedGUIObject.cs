/*
 * ISyncedGUIObject.cs
 *
 * Syncronization interface for objects displayed in the UI.
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
    
    public interface ISyncedGUIObject
    {
        
        //string                          IDString                    { get; }
        
        Plugin.File[]                   Files                       { get; }
        string[]                        Filenames                   { get; }
        //bool                            IsModifiedIn( Plugin.File file );
        
        string                          Signature                   { get; }
        
        uint                            LoadOrder                   { get; }
        
        //uint                            FormID                      { get; }
        //string                          EditorID                    { get; set; }
        
        //Engine.Plugin.Forms.Fields.Record.FormID FormID             { get; }
        //Engine.Plugin.Forms.Fields.Record.EditorID EditorID         { get; }
        
        uint                            GetFormID( Engine.Plugin.TargetHandle target );
        void                            SetFormID( Engine.Plugin.TargetHandle target, uint value );
        
        string                          GetEditorID( Engine.Plugin.TargetHandle target );
        void                            SetEditorID( Engine.Plugin.TargetHandle target, string value );
        
        ConflictStatus                  ConflictStatus              { get; }
        
        string                          ExtraInfo                   { get; }
        
        event EventHandler              ObjectDataChanged;

        bool                            ObjectDataChangedEventsSupressed { get; }

        void                            SupressObjectDataChangedEvents();
        void                            ResumeObjectDataChangedEvents( bool sendevent );
        void                            SendObjectDataChangedEvent( object sender );
        
        bool                            InitialCheckedOrSelectedState();
        
        bool                            ObjectChecked( bool checkedValue );

        int                             GetHashCode();

    }

}
