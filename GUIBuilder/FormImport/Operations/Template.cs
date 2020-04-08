/*
 * Template.cs
 *
 * Single operation description.
 *
 */
/*
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
using Priority = GUIBuilder.FormImport.ImportBase.Priority;
using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder.FormImport.Operations
{

    public class Template : ImportOperation
    {
        
        const string                                    DN_Key = "TranslationKey";

        readonly $FieldValueType$                       Value;

        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Key.Translate(), Value )
            };
        }

        public                                          Template( ImportBase parent, $FieldValueType$ value )
        : base( parent )
        {
            Value = value;
        }

        public override bool                            Apply()
        {
            #region Casted Target

            var $casted$ = Target.Value as $TargetType$;
            var result = $casted$ != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( $TargetType$ ).FullName() );
            else
            {
                $casted$.$FieldName$.SetValue( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
                
            #endregion

            #region IXHandle Target

            Target.Value.$FieldName$.SetValue( TargetHandle.Working, Value );
            return TargetMatchesImport();
                
            #end
        }

        public override bool                            TargetMatchesImport()
        {
            #region Casted Target

            var $casted$ = Target.Value as $TargetType$;
            if( $casted$ != null ) return false;
            return Value == $casted$.$FieldName$.GetValue( TargetHandle.WorkingOrLastFullRequired );

            #endregion

            #region IXHandle Target
                
            return Value == Target.Value.$FieldName$.GetValue( TargetHandle.WorkingOrLastFullRequired );
                
            #endregion
        }

    }
}

*/