/*
 * SetReferenceBaseForm.cs
 *
 * Sets the Base Form (Name) of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferenceBaseForm : ImportOperation
    {
        
        const string                                    DN_BaseForm = "BaseFormU";
        
        readonly ImportTarget                           _Form;

        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", _Form.DisplayName, _Form.NullSafeIDString() )
            };
        }

        public                                          SetReferenceBaseForm( ImportBase parent, Form value )
        : base( parent )
        {
            _Form = new ImportTarget( parent, DN_BaseForm.Translate(), typeof( Form ), value );
        }

        public                                          SetReferenceBaseForm( ImportBase parent, string editorID )
        : base( parent )
        {
            _Form = new ImportTarget( parent, DN_BaseForm.Translate(), typeof( Form ), editorID );
        }
        public override bool                            Resolve( bool errorIfUnresolveable )
        {
            return _Form.Resolve( errorIfUnresolveable );
        }

        public override bool                            Apply()
        {
            var refr = Target.Value as ObjectReference;
            var result = refr != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( ObjectReference ).FullName() );
            else
            {
                refr.SetNameFormID( TargetHandle.Working, _Form.FormID );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            if( !_Form.Resolve( false ) ) return false;
            return _Form.FormID == refr.GetNameFormID( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}