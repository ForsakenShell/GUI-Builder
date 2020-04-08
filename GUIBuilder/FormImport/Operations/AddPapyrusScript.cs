/*
 * AddPapyrusScript.cs
 *
 * Single operation description.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Maths;

using Fallout4;
using AnnexTheCommonwealth;

using Engine.Plugin;
using Engine.Plugin.Forms;
using Engine.Plugin.Interface;
using Engine.Plugin.Attributes;
using Engine.Plugin.Extensions;

using GUIBuilder.FormImport;

using Color = System.Drawing.Color;
using SetEditorID = GUIBuilder.FormImport.Operations.SetEditorID;
using Operations = GUIBuilder.FormImport.Operations;
using Priority = GUIBuilder.FormImport.Priority;
using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder.FormImport.Operations
{

    public class AddPapyrusScript : ImportOperation
    {
        
        const string                                    DN_PapyrusScript = "Reference.ScriptU";

        readonly Type                                   Value;

        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_PapyrusScript.Translate(), Value )
            };
        }

        public                                          AddPapyrusScript( ImportBase parent, Type value )
        : base( parent )
        {
            Value = value;
        }

        public override bool                            Apply()
        {
            var form = Target.Value as Form;
            var result = form != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( Form ).FullName() );
            else
            {
                result = Value != typeof( AnnexTheCommonwealth.BuildAreaVolume );
                if( !result )
                    Parent.AddErrorMessage( ErrorTypes.Import, "Cannot 'attach' Script to Form, must be AnnexTheCommonwealth.BuildAreaVolume\nTODO:  This properly" );
                else
                {
                    // TODO:  Make this import properly add the script to the VMAD table, not this nonsense
                    var refr = form as ObjectReference;
                    if( refr == null )
                    {
                        result = false;
                        Parent.AddErrorMessage( ErrorTypes.Import, "Cannot 'attach' Script to Form, must be Target must be an ObjectReference\nTODO:  This properly" );
                    }
                    else
                    {
                        var script = new AnnexTheCommonwealth.BuildAreaVolume( refr );
                        result = script.PostLoad();
                        if( result )
                            GodObject.Plugin.Data.BuildVolumes.Add( script );
                    }
                }
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {   // TODO:  This properly!
            if( Value != typeof( AnnexTheCommonwealth.BuildAreaVolume ) ) return false;

            var form = Target.Value as Form;
            if( form == null ) return false;

            var refr = form as ObjectReference;
            if( refr == null ) return false;

            var script = GodObject.Plugin.Data.BuildVolumes.Find( Target.FormID );
            return script != null;
        }

    }
}
