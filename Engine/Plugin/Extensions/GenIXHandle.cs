﻿/*
 * GenIXHandle.cs
 * 
 * Generic functions for IXHandle
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:17 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;


namespace Engine.Plugin.Extensions
{
    /// <summary>
    /// Description of GenIXHandle.
    /// </summary>
    public static class GenIXHandle
    {
        
        #region IXHandle Display Info
        
        public static bool              Resolveable<T>( this T target, uint formID, string editorID ) where T : class, Engine.Plugin.Interface.IXHandle
        {
            return
                ( target != null )||
                ( formID.ValidFormID() )||
                ( editorID.ValidEditorID() );
        }
        
        public static string            ConcatDisplayInfo( this List<string> items )
        {
            if( items.NullOrEmpty() ) return null;
            string result = null;
            foreach( var item in items )
            {
                if( !string.IsNullOrEmpty( item ) )
                {
                    if( string.IsNullOrEmpty( result ) )
                        result = item;
                    else
                    {
                        result += "; ";
                        result += item;
                    }
                }
            }
            return result;
        }
        
        public static string            ExtraInfoFor<T>( this T target, uint formID = Engine.Plugin.Constant.FormID_Invalid, string editorID = null, string format = null, string unresolveable = null, string extra = null, bool includeSignature = false, bool includeFilename = false ) where T : class, Engine.Plugin.Interface.IXHandle
        {
            if( string.IsNullOrEmpty( format ) ) format = "{0}";
            if( !Resolveable( target, formID, editorID ) ) return string.Format( format, unresolveable );
            string fileNames = null;
            if( includeFilename )
            {
                var th = target.Handles;
                for( int i = 0; i < th.Count; i++ )
                {
                    if( fileNames != null ) fileNames += "; ";
                    fileNames += th[ i ].Filename;
                }
                fileNames = string.Format( "[{0}]", fileNames );
            }
            return string.Format(
                format,
                string.Format(
                    "{0}{1}{2}",
                    ( includeSignature ? string.Format( "\"{0}\" ", target.Signature ) : null ),
                    (
                        target == null
                        ? string.Format( "IXHandle.IDString".Translate(), formID, editorID )
                        : target.IDString
                    ),
                    ( includeFilename  ? string.Format( " in {0}", fileNames ) : null )
                ),
                extra
            );
        }

        public static void              DumpAncestralTree( Engine.Plugin.Interface.IXHandle obj, Engine.Plugin.Interface.IXHandle initialObj = null )
        {
            if( ( initialObj != null ) && ( initialObj == obj ) )
            {
                DebugLog.WriteLine( "\tI'm my own grandpa!" );
                return;
            }
            if( initialObj == null )
            {
                DebugLog.WriteCaller( false );
                initialObj = obj;
            }
            DebugLog.WriteLine( string.Format(
                "\t{0} :: {1}",
                obj.TypeFullName(),
                obj.IDString ) );
            if( obj.Ancestor != null )
                DumpAncestralTree( obj.Ancestor, initialObj );
        }

        #endregion

    }

}