/*
 * SetRecordFlags.cs
 *
 * Set new Record Flags on a Form.
 *
 */
using Engine.Plugin;


namespace GUIBuilder.FormImport.Operations
{

    public class SetRecordFlags : ImportOperation
    {
        
        const string                                    DN_RecordFlags = "Form.RecordFlags";
        
        readonly uint                                   Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: 0x{1}", DN_RecordFlags.Translate(), Value.ToString( "X8" ) )
            };
        }
        
        public                                          SetRecordFlags( ImportBase parent, uint value )
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
                form.RecordFlags.SetValue( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var form = Target.Value as Form;
            if( form == null ) return false;
            return Value == form.RecordFlags.GetValue( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
