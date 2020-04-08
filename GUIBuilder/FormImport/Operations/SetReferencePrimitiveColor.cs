/*
 * SetReferencePrimitiveColor.cs
 *
 * Sets the Primitive Size (Bounds) of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;

using Color = System.Drawing.Color;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferencePrimitiveColor : ImportOperation
    {

        const string                                    DN_Color = "Reference.Primitive.Color";

        readonly Color                                  Value;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_Color.Translate(), Value.ToString() )
            };
        }
        
        public                                          SetReferencePrimitiveColor( ImportBase parent, Color value )
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
                refr.Primitive.SetColor( TargetHandle.Working, Value );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            return Value == refr.Primitive.GetColor( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}
