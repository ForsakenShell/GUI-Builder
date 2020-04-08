/*
 * SetReferencePrimitiveBounds.cs
 *
 * Sets the Primitive Size (Bounds) of an ObjectReference
 *
 */
using Maths;

using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferencePrimitiveBounds : ImportOperation
    {
        
        const string                                    DN_Size = "Reference.Primitive.Size";

        readonly Vector3f                               Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Size.Translate(), Value.ToString() )
            };
        }
        
        public                                          SetReferencePrimitiveBounds( ImportBase parent, Vector3f value )
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
                refr.Primitive.SetBounds( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            return Value == refr.Primitive.GetBounds( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
