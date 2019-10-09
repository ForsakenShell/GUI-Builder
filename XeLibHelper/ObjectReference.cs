/*
 * ObjectReference.cs
 *
 * Static extension functions for XeLib Handles when the handle is an ObjectReference record (REFR).
 *
 * User: 1000101
 * Date: 03/07/2018
 * Time: 2:31 PM
 * 
 */
using System;
using System.Globalization;

//using Maths;

using XeLib;
using XeLib.API;
using XeLib.Internal;

namespace XeLibHelper
{
    
    /// <summary>
    /// Static extension functions for XeLib Handles when the handle is a reference record.
    /// </summary>
    public static class ObjectReference
    {
        
        /*
        
        #region Position
        
        public static Vector3f GetPosition( this Handle record )
        {
            var position = Elements.GetElement( record, @"DATA\Position" );
            return new Vector3f(
                (float)ElementValues.GetFloatValue( position, "X" ),
                (float)ElementValues.GetFloatValue( position, "Y" ),
                (float)ElementValues.GetFloatValue( position, "Z" )
               );
        }
        
        public static void SetPosition( this Handle record, Vector3f p )
        {
            var position = Elements.GetElement( record, @"DATA\Position" );
            ElementValues.SetFloatValue( position, "X", p.X );
            ElementValues.SetFloatValue( position, "Y", p.Y );
            ElementValues.SetFloatValue( position, "Z", p.Z );
        }
        
        #endregion
        
        #region Rotation
        
        public static Vector3f GetRotation( this Handle record )
        {
            var rotation = Elements.GetElement( record, @"DATA\Rotation" );
            return new Vector3f(
                (float)ElementValues.GetFloatValue( rotation, "X" ),
                (float)ElementValues.GetFloatValue( rotation, "Y" ),
                (float)ElementValues.GetFloatValue( rotation, "Z" )
               );
        }
        
        public static void SetRotation( this Handle record, Vector3f r )
        {
            var rotation = Elements.GetElement( record, @"DATA\Rotation" );
            ElementValues.SetFloatValue( rotation, "X", r.X );
            ElementValues.SetFloatValue( rotation, "Y", r.Y );
            ElementValues.SetFloatValue( rotation, "Z", r.Z );
        }
        
        #endregion
        
        #region (Primitive) Bounds
        
        public static Vector3f GetReferencePrimitiveBounds( this Handle record )
        {
            var bounds = Elements.GetElement( record, @"XPRM\Bounds" );
            return new Vector3f(
                (float)ElementValues.GetFloatValue( bounds, "X" ),
                (float)ElementValues.GetFloatValue( bounds, "Y" ),
                (float)ElementValues.GetFloatValue( bounds, "Z" )
               );
        }
        
        public static void SetReferencePrimitiveBounds( this Handle record, Vector3f b )
        {
            var bounds = Elements.GetElement( record, @"XPRM\Bounds" );
            ElementValues.SetFloatValue( bounds, "X", b.X );
            ElementValues.SetFloatValue( bounds, "Y", b.Y );
            ElementValues.SetFloatValue( bounds, "Z", b.Z );
        }
        
        #endregion
        
        */
        
        #region Cell
        
        /*
        public static Handle GetCellHandle( this Handle record )
        {
            return record.FindParentBySignature( "CELL" );
        }
        */
        
        public static uint GetCellFormID( this ElementHandle record )
        {
            return ElementValues.GetUIntValueEx( record.XHandle, "Cell" );
        }
        
        public static void SetCell( this FormHandle record, uint cellFormID )
        {
            record.SetUIntValueEx( "Cell", cellFormID );
        }
        
        public static void SetCell( this FormHandle record, FormHandle cell )
        {
            SetCell( record, cell.FormID );
        }
        
        #endregion
        
        #region Worldspace
        
        public static FormHandle GetWorldspaceHandle( this ElementHandle record )
        {
            return record.FindParentBySignature<FormHandle>( "WRLD" );
        }
        
        public static uint GetWorldspaceFormID( this ElementHandle record )
        {
            var wH = record.FindParentBySignature<FormHandle>( "WRLD" );
            if( !wH.IsValid() )
                return Engine.Plugin.Constant.FormID_Invalid;
            var result = wH.FormID;
            wH.Dispose();
            return result;
        }
        
        #endregion
        
        public static FormHandle CopyMoveToCell( this FormHandle source, FormHandle destination, bool moveRefrToCell = false )
        {
            if( ( !source.IsValid() )||( !destination.IsValid() ) ) return null;
            if( destination.Signature != "CELL" ) return null;
            var sSignature = source.Signature;
            if( sSignature != "REFR" ) return null;
            
            FormHandle result = null;
            
            if( moveRefrToCell )
            {   // Override [and move to new CELL]
                var sfHandle = source.FileHandle;
                var dfHandle = destination.FileHandle;
                var inSameFile = sfHandle.DuplicateOf( dfHandle );
                DebugLog.WriteLine( "OVERRIDE REFR - InSameFile ? " + inSameFile );
                result = inSameFile
                    ? source
                    : Elements.CopyElementEx<FormHandle>( source.XHandle, dfHandle.XHandle, false );
                if( !result.IsValid() ) return null;
                result.SetCell( destination );
            }
            else
            {   // New record in destination CELL
                DebugLog.WriteLine( "NEW REFR" );
                result = destination.AddElement<FormHandle>( sSignature );
                if( !result.IsValid() ) return null;
            }
            
            // Appropriate persistence flag for new record
            const string PERSISTENT = "Persistent";
            var dPersistent = destination.GetRecordFlag( PERSISTENT );
            DebugLog.WriteLine( "Persistence: " + dPersistent );
            DebugLog.WriteLine( "Current Flags: " + result.RecordFlags.ToString( "X8" ) );
            result.SetRecordFlag( PERSISTENT, dPersistent );
            DebugLog.WriteLine( "New Flags: " + result.RecordFlags.ToString( "X8" ) );
            
            if( result != source )
            {
                var sElements = Elements.GetElementsEx<ElementHandle>( source.XHandle );
                if( ( sElements != null )&&( sElements.Length > 0 ) )
                {
                    foreach( var eHandle in sElements )
                    {
                        //var eType = eHandle.ElementType;
                        ElementHandle nHandle = null;
                        var eLPath = eHandle.LocalPath;
                        if(
                            ( eLPath != "Record Header" )&&
                            ( eLPath != "Cell" )&&
                            ( eHandle.Signature != "EDID" )
                        )
                        {
                            /*
                            DebugLog.Write(
                                string.Format(
                                    "Copy Element\n\tType = {0}\n\tSignature = \"{1}\"\n\tPath = \"{2}\"",
                                    eHandle.ElementType.ToString(),
                                    eHandle.Signature,
                                    eHandle.LocalPath
                                   ) );
                            */
                            nHandle = Elements.CopyElementEx<ElementHandle>( eHandle.XHandle, result.XHandle, true );
                            if( !nHandle.IsValid() )
                            {
                                DebugLog.WriteError( "XeLibHelper.ObjectReference", "CopyMoveToCell()", "Unable to copy source element to new record!" );
                            }
                            nHandle.Dispose();
                        }
                        eHandle.Dispose();
                    }
                }
            }
            
            return result;
        }
        
    }
}
