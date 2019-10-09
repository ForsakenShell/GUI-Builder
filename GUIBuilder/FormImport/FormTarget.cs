/*
 * FormTarget.cs
 *
 * Base import form target.
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
    
    public class FormTarget : ImportTarget
    {
        
        public Engine.Plugin.Form Form
        {
            get { return Value as Engine.Plugin.Form; }
            set { Value = value; }
        }
        
        public FormTarget( ImportBase parent, Type formType, uint formID, string editorID ) : base( parent, formType, formID, editorID ) {}
        public FormTarget( ImportBase parent, Type formType, Engine.Plugin.Form form ) : base( parent, formType, form ) {}
        public FormTarget( ImportBase parent, Type formType ) : base( parent, formType ) {}
        //public FormTarget( ImportBase parent ) : base( parent ) {}
        
        protected override void ResolveValue()
        {
            if( Value != null ) return;
            if( Association == null ) return;
            if( Engine.Plugin.Constant.ValidFormID( FormID ) )
                Value = GodObject.Plugin.Data.Root.FindEx( Association, formid: FormID );
            else if( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                Value = GodObject.Plugin.Data.Root.FindEx( Association, editorid: EditorID );
        }
        
    }
    
}
