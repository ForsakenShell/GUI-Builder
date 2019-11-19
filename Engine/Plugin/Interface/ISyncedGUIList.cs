/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
using System;
using System.Collections.Generic;

using XeLib;
using XeLib.API;

namespace Engine.Plugin.Interface
{
    
    public interface ISyncedGUIList<TList>
    {
        
        event EventHandler  ObjectDataChanged;
        
        void                SupressObjectDataChangedEvents();
        void                ResumeObjectDataChangedEvents( bool sendevent );
        void                SendObjectDataChangedEvent();
        
        int                 Count { get; }
        
        void                Add( TList item );
        bool                Remove( TList item );
        
        List<TList>         ToList( bool includePackInReferences );
        
        TList               Find( uint formid );
        TList               Find( string editorid );
        List<TList>         FindAllInWorldspace( string editorid );
        List<TList>         FindAllInWorldspace( Engine.Plugin.Forms.Worldspace worldspace );
        List<TList>         FindAllInWorldspace( uint formid );
        
    }
    
}
