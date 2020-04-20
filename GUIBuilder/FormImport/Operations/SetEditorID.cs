/*
 * SetEditorID.cs
 *
 * Sets a new EditorID on a Form.
 *
 */
using Engine.Plugin;

using EditorIDFormatter = GUIBuilder.CustomForms.EditorIDFormats;


namespace GUIBuilder.FormImport.Operations
{

    public class SetEditorID : ImportOperation
    {

        const string                                    DN_EditorID = "Form.EditorID";

        readonly string                                 Value;
        
        public override string[]                        OperationalInformation()
        {
            return new[] {
                string.Format( "{0}: \"{1}\"", DN_EditorID.Translate(), Value )
            };
        }
        
        public                                          SetEditorID( ImportBase parent, string value )
        : base( parent )
        {
            Value = value;
            Target.SetEditorID( Value );
        }

        public                                          SetEditorID( ImportBase parent, string format, string name, int index = -1 )
        : base( parent )
        {
            Value = EditorIDFormatter.FormatEditorID( format, EditorIDFormatter.ModPrefix, Target.Association.Signature, name, index );
            Target.SetEditorID( Value );
        }

        public                                          SetEditorID( ImportBase parent )
        : base( parent )
        {
            Value = Target.EditorID;
        }

        public override bool                            Resolve( bool errorIfUnresolveable )
        {
            Target.SetEditorID( Value );
            return true;
        }

        public override bool                            Apply()
        {
            Target.Value.SetEditorID( TargetHandle.Working, Value );
            return TargetMatchesImport();
        }

        public override bool                            TargetMatchesImport()
        {
            return Value.SensitiveInvariantMatch( Target.Value.GetEditorID( TargetHandle.WorkingOrLastFullRequired ) );
        }
    }

}
