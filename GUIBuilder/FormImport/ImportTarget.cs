/*
 * ImportTarget.cs
 *
 * Base import target class.
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using Maths;
using Fallout4;
using AnnexTheCommonwealth;

namespace GUIBuilder.FormImport
{
    
    public abstract class ImportTarget
    {
        
        protected ImportBase Parent;
        
        Engine.Plugin.Interface.IXHandle _Value = null;
        public Engine.Plugin.Interface.IXHandle Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                SyncImportReferenceInfoWithResolvedTarget();
            }
        }
        
        public Engine.Plugin.Attributes.ClassAssociation Association = null;
        public uint FormID = Engine.Plugin.Constant.FormID_None;
        public string EditorID = null;
        
        public bool IsResolved { get { return Value != null; } }
        
        public uint DisplayFormID( Engine.Plugin.TargetHandle target )
        {
            return
                _Value == null
                ? FormID
                : _Value.GetFormID( target );
        }
        
        public string DisplayEditorID( Engine.Plugin.TargetHandle target )
        {
            return
                _Value == null
                ? EditorID
                : _Value.GetEditorID( target );
        }
        
        protected ImportTarget( ImportBase parent, Type classType, uint formID, string editorID )
        {
            Association = Engine.Plugin.Attributes.Reflection.AssociationFrom( classType );
            if( Association == null )
                throw new Exception( string.Format( "{0} :: cTor() :: Cannot resolve Association from classType {1}", this.GetType().ToString(), ( classType == null ? "null" : classType.ToString() ) ) );
            Parent = parent;
            FormID = formID;
            EditorID = editorID;
        }
        
        protected ImportTarget( ImportBase parent, Type classType, Engine.Plugin.Interface.IXHandle target = null)
        {
            Association = Engine.Plugin.Attributes.Reflection.AssociationFrom( classType );
            if( Association == null )
                throw new Exception( string.Format( "{0} :: cTor() :: Cannot resolve Association from classType {1}", this.GetType().ToString(), ( classType == null ? "null" : classType.ToString() ) ) );
            Parent = parent;
            Value = target;
        }
        
        protected ImportTarget( ImportBase parent, Type classType )
        {
            Association = Engine.Plugin.Attributes.Reflection.AssociationFrom( classType );
            if( Association == null )
                throw new Exception( string.Format( "{0} :: cTor() :: Cannot resolve Association from classType {1}", this.GetType().ToString(), ( classType == null ? "null" : classType.ToString() ) ) );
            Parent = parent;
        }
        
        public bool Matches<T>( T other, bool allowClearing ) where T : class, Engine.Plugin.Interface.IXHandle
        {
            if(
                ( !allowClearing )&&
                (
                    ( other == null )||
                    ( !Resolveable() )
                )
            )   return false;
            var otherFormID     = ( other != null ) ? other.GetFormID( Engine.Plugin.TargetHandle.Master ) : Engine.Plugin.Constant.FormID_None;
            var otherEditorID   = ( other != null ) ? other.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) : null;
            var fm = Matches( otherFormID, allowClearing );
            var em = Matches( otherEditorID );
            var fe = FormID == Engine.Plugin.Constant.FormID_None;
            var ee = string.IsNullOrEmpty( EditorID );
            var result = allowClearing && fe && ee;
            if( !fe &&  ee ) result = fm;
            if(  fe && !ee ) result = em;
            if( !fe && !ee ) result = fm && em;
            //DebugLog.WriteLine( new [] { this.GetType().ToString(), "Matches<T>()", "allowClearing = " + allowClearing.ToString(), "FormID ? " + fm.ToString(), "EditorID ? " + em.ToString(), "result = " + result.ToString() } );
            return result;
        }
        
        public bool Matches( uint otherFormID, bool allowClearing )
        {
            return (
                (
                    ( allowClearing )||
                    (
                        ( FormID != Engine.Plugin.Constant.FormID_None )&&
                        ( otherFormID != Engine.Plugin.Constant.FormID_None )
                    )
                )&&
                ( Engine.Plugin.Constant.ValidFormID( FormID      ) )&&
                ( Engine.Plugin.Constant.ValidFormID( otherFormID ) )&&
                ( FormID == otherFormID )
            );
        }
        
        public bool Matches( string otherEditorID )
        {
            return (
                ( Engine.Plugin.Constant.ValidEditorID( EditorID      ) )&&
                ( Engine.Plugin.Constant.ValidEditorID( otherEditorID ) )&&
                ( EditorID.InsensitiveInvariantMatch( otherEditorID ) )
            );
        }
        
        public bool Resolveable()
        {
            return
                ( _Value != null )||
                (
                    ( Association != null )&&
                    (
                        ( Engine.Plugin.Constant.ValidFormID  ( FormID   ) )||
                        ( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                    )
                );
        }
        
        protected abstract void ResolveValue();
        
        public bool Resolve( bool errorIfUnresolveable = true )
        {
            if( _Value == null )
                ResolveValue();
            if( ( errorIfUnresolveable)&&( _Value == null ) )
                Parent.AddErrorMessage( FormImport.ErrorTypes.Resolve, this.DisplayIDInfo( "Cannot resolve {0}" ) );
            return ( _Value != null );
        }
        
        public string DisplayIDInfo( string format = null, string unresolveableSuffix = null )
        {
            if( string.IsNullOrEmpty( format ) ) format = "{0}";
            //return ImportBase.ExtraInfoFor( f, Value, FormID, EditorID, unresolveable, extra );
            var tmp = !Resolveable()
                ? string.IsNullOrEmpty( unresolveableSuffix )
                    ? string.Format(
                        "0x{0} - \"{1}\"",
                        FormID.ToString( "X8" ),
                        EditorID )
                    : string.Format(
                        "0x{0} - \"{1}\" {2}",
                        FormID.ToString( "X8" ),
                        EditorID,
                        unresolveableSuffix )
                : string.Format(
                    "0x{0} - \"{1}\"",
                    DisplayFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ),
                    DisplayEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
            return string.IsNullOrEmpty( format )
                    ? tmp
                    : string.Format( format, tmp );
        }
        
        protected void SyncImportReferenceInfoWithResolvedTarget()
        {
            if( _Value != null )
            {
                FormID = Value.GetFormID( Engine.Plugin.TargetHandle.Master );
                EditorID = Value.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            }
        }
        
    }
    
}
