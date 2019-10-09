/*
 * ObjectReferenceTarget.cs
 *
 * Base object reference import class.
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
    
    public class ObjectReferenceTarget : FormTarget
    {
        
        public readonly Engine.Plugin.Forms.Worldspace Worldspace;
        public readonly Engine.Plugin.Forms.Cell Cell;
        
        public Engine.Plugin.Forms.ObjectReference Reference
        {
            get { return Value as Engine.Plugin.Forms.ObjectReference; }
            set { Value = value; }
        }
        
        public ObjectReferenceTarget( ImportBase parent, uint formID, string editorID, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell )
            : base( parent, typeof( Engine.Plugin.Forms.ObjectReference ), formID, editorID )
        {
            Worldspace = worldspace;
            Cell = cell;
        }
        public ObjectReferenceTarget( ImportBase parent, Engine.Plugin.Form form, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell )
            : base( parent, typeof( Engine.Plugin.Forms.ObjectReference ), form )
        {
            Worldspace = worldspace;
            Cell = cell;
        }
        public ObjectReferenceTarget( ImportBase parent, Engine.Plugin.Forms.Worldspace worldspace, Engine.Plugin.Forms.Cell cell )
            : base( parent, typeof( Engine.Plugin.Forms.ObjectReference ) )
        {
            Worldspace = worldspace;
            Cell = cell;
        }
        public ObjectReferenceTarget( ImportBase parent )
            : base( parent, typeof( Engine.Plugin.Forms.ObjectReference ) ) {}
        
        protected override void ResolveValue()
        {
            if( Value != null ) return;
            if( Association == null ) return;
            if( Cell != null )
            {
                if( Engine.Plugin.Constant.ValidFormID( FormID ) )
                    Value = Cell.ObjectReferences.FindEx( Association, formid: FormID );
                else if( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                    Value = Cell.ObjectReferences.FindEx( Association, editorid: EditorID );
                if( Value != null ) return;
            }
            if( Worldspace != null )
            {
                if( Engine.Plugin.Constant.ValidFormID( FormID ) )
                    Value = Worldspace.Cells.FindEx( Association, formid: FormID );
                else if( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                    Value = Worldspace.Cells.FindEx( Association, editorid: EditorID );
                if( Value != null ) return;
            }
            if( Engine.Plugin.Constant.ValidFormID( FormID ) )
                Value = GodObject.Plugin.Data.Root.FindEx( Association, formid: FormID );
            else if( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                Value = GodObject.Plugin.Data.Root.FindEx( Association, editorid: EditorID );
        }
        
    }
    
}
