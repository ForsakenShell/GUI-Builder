/*
 * SetReferencePosition.cs
 *
 * Sets the position of an ObjectReference
 *
 */
using Maths;

using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferencePosition : ImportOperation
    {
        
        const string                                    DN_Position = "Reference.Position";

        readonly Vector3f                               Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Position.Translate(), Value.ToString() )
            };
        }
        
        public                                          SetReferencePosition( ImportBase parent, Vector3f value )
        : base( parent )
        {
            Value = value;
        }

        public override bool                            Apply()
        {
            var refr = Target.Value as ObjectReference;
            var result = refr != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( ObjectReference ).FullName() );
            else
            {
                refr.SetPosition( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            return Value == refr.GetPosition( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
