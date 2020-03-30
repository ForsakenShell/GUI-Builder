/*
 * TargetHandle.cs
 * 
 * Generic functions for TargetHandle
 * 
 * User: 1000101
 * Date: 05/11/2019
 * Time: 3:19 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

using XeLib;
using Engine.Plugin.Interface;


namespace Engine.Plugin.Extensions
{
    /// <summary>
    /// Description of TargetHandleExtensions.
    /// </summary>
    public static class TargetHandleExtensions
    {
        
        public static ElementHandle     HandleFromTarget<T>( this T source, TargetHandle target ) where T : class, IXHandle
        {
            if( !source.IsValid() )
                throw new ArgumentException( string.Format( "Source is null or does not have any XHandles associated with it! :: Source = {0}", source.TypeFullName() ) );
            ElementHandle h = null;
            switch( target )
            {
                case TargetHandle.None:
                    break;
                    
                case TargetHandle.Master:
                    h = source.MasterHandle;
                    break;
                    
                case TargetHandle.LastFullRequired:
                    h = source.LastFullRequiredHandle;
                    break;
                    
                case TargetHandle.Working:
                    h = source.WorkingFileHandle;
                    break;
                    
                case TargetHandle.WorkingOrLastFullRequired:
                    h =   source.WorkingFileHandle.IsValid()      ? source.WorkingFileHandle
                        : source.LastFullRequiredHandle.IsValid() ? source.LastFullRequiredHandle
                        : null;
                    break;
                    
                case TargetHandle.LastFullOptional:
                    h = source.LastFullOptionalHandle;
                    break;
                    
                case TargetHandle.LastValid:
                    h = source.Handles.Last();
                    break;
                    
            }
            if( !h.IsValid() )
                throw new ArgumentException( string.Format( "Source did not yield a valid Handle for Target :: Source = {0} :: Target = {1}", source.TypeFullName(), target.ToString() ) );
            return h;
        }
        
        public static ScriptPropertyHandle ScriptPropertyHandleFromTarget<T>( this T source, TargetHandle target, string propertyName ) where T : PapyrusScript
        {
            var sh = source.HandleFromTarget( target ) as ScriptHandle;
            return sh.GetProperty( propertyName );
        }
        
    }
    
}