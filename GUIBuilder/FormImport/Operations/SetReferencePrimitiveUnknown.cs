/*
 * SetReferencePrimitiveUnknown.cs
 *
 * Sets the Primitive Size (Bounds) of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferencePrimitiveUnknown : ImportOperation
    {
        
        const string                                    DN_Unknown = "Reference.Primitive.Unknown";

        readonly float                                  Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Unknown.Translate(), Value.ToString() )
            };
        }
        
        public                                          SetReferencePrimitiveUnknown( ImportBase parent, float value )
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
                refr.Primitive.SetUnknown( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            return Value == refr.Primitive.GetUnknown( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
