/*
 * ScriptTarget.cs
 *
 * Base papyrus script import class.
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
    
    public class ScriptTarget : ImportTarget
    {
        
        public Engine.Plugin.PapyrusScript Script
        {
            get { return Value as Engine.Plugin.PapyrusScript; }
            set { Value = value; }
        }
        
        public ScriptTarget( string name, ImportBase parent, Type scriptType, uint formID, string editorID ) : base( name, parent, scriptType, formID, editorID ) {}
        public ScriptTarget( string name, ImportBase parent, Type scriptType, Engine.Plugin.PapyrusScript script ) : base( name, parent, scriptType, script ) {}
        public ScriptTarget( string name, ImportBase parent, Type scriptType ) : base( name, parent, scriptType ) {}
        //public ScriptTarget( ImportBase parent ) : base( parent ) {}
        
        protected override void ResolveValue()
        {
            if( Value != null ) return;
            if( Engine.Plugin.Constant.ValidFormID( FormID ) )
                Value = GodObject.Plugin.Data.GetScriptByFormID( FormID );
            else if( Engine.Plugin.Constant.ValidEditorID( EditorID ) )
                Value = GodObject.Plugin.Data.GetScriptByEditorID( EditorID );
        }
        
    }
    
}
