/*
 * ICollection.cs
 *
 * Interface for IDataSync collections
 *
 */
using System;
using System.Collections.Generic;

using XeLib;
using XeLib.API;

using Engine.Plugin.Attributes;

namespace Engine.Plugin.Interface
{
    
    public interface ICollection
    {
        
        #region Required Properties
        
        ClassAssociation                Association                 { get; }
        
        //bool                            IsValid                     { get; }
        
        IXHandle                        Ancestor                    { get; }
        Engine.Plugin.Form              ParentForm                  { get; }
        
        #endregion
        
        #region Loading
        
        bool                            LoadAllForms();
        bool                            LoadFrom( IXHandle source );
        bool                            PostLoad();
        
        #endregion
        
        #region Form Enumeration
        
        int                             Count                       { get; }
        
        List<IXHandle>                  ToList( int loadOrderFilter = -1, bool tryLoad = true );
        List<TSync>                     ToList<TSync>( int loadOrderFilter = -1, bool tryLoad = true ) where TSync : class, IXHandle;
        
        IXHandle                        CreateNew();
        TSync                           CreateNew<TSync>() where TSync : class, IXHandle;
        
        bool                            Add( IXHandle syncObject );
        void                            Remove( IXHandle syncObject );
        
        IXHandle                        FindEx( ClassAssociation targetAssociation, FormHandle handle = null, uint formid = 0, string editorid = null, bool tryLoad = true );
        
        IXHandle                        Find( XeLib.FormHandle handle, bool tryLoad = true );
        
        IXHandle                        Find( string signature, uint formid, bool tryLoad = true );
        IXHandle                        Find( string signature, string editorid, bool tryLoad = true );
        
        IXHandle                        Find( uint formid, bool tryLoad = true );
        IXHandle                        Find( string editorid, bool tryLoad = true );
        
        TSync                           Find<TSync>( uint formid, bool tryLoad = true ) where TSync : class, IXHandle;
        TSync                           Find<TSync>( string editorid, bool tryLoad = true ) where TSync : class, IXHandle;
        
        #endregion
        
    }
    
    public static partial class Extensions
    {
        public static bool              IsValid( this ICollection collection )
        {
            return
                ( collection != null )&&
                ( collection.Association != null )&&
                ( !string.IsNullOrEmpty( collection.Association.Signature ) )&&
                ( collection.Association.ClassType != null );
        }
        
    }
    
}
