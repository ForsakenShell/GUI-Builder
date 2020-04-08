/*
 * SetReferencePrimitiveShape.cs
 *
 * Sets the Primitive Size (Bounds) of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;

using Shape = Engine.Plugin.Forms.Fields.ObjectReference.Primitive.PrimitiveType;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferencePrimitiveShape : ImportOperation
    {
        
        const string                                    DN_Shape = "Reference.Primitive.Shape";

        readonly Shape                                  Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Shape.Translate(), Value.ToString() )
            };
        }
        
        public                                          SetReferencePrimitiveShape( ImportBase parent, Shape value )
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
                refr.Primitive.SetType( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            return Value == refr.Primitive.GetType( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
