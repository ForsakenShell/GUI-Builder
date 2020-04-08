/*
 * SetEditorID.cs
 *
 * Sets a new EditorID on a Form.
 *
 */
using Engine.Plugin;


namespace GUIBuilder.FormImport.Operations
{

    public class SetEditorID : ImportOperation
    {

        #region EditorID Templating

        public const string                             Token_Name = "{name}";
        public const string                             Token_Index = "{index}";

        public static string                            FormatEditorID( string format, string name, int index = -1 )
        {
            if( string.IsNullOrEmpty( format ) )
                format = string.Format(
                    "{0}{1}",
                    ( string.IsNullOrEmpty( name ) ? null : Token_Name ),
                    ( index < 0 ? null : Token_Index )
                );

            string result = string.Copy( format );

            GenString.ReplaceToken( ref result, Token_Name, name );
            GenString.ReplaceToken( ref result, Token_Index, ( index < 0 ? null : index.ToString( "D2" ) ) );

            return result;
        }

        #endregion

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
