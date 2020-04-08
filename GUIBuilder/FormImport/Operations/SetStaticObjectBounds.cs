/*
 * SetStaticObjectBounds.cs
 *
 * Sets the Object Bounds of a StaticObject
 *
 */
using Maths;

using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetStaticObjectBounds : ImportOperation
    {
        
        const string                                    DN_MinBounds = "Static.MinBounds";
        const string                                    DN_MaxBounds = "Static.MaxBounds";

        readonly Vector3i                               Min;
        readonly Vector3i                               Max;
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", DN_MinBounds.Translate(), Min.ToString() ),
                string.Format( "{0}: {1}", DN_MaxBounds.Translate(), Max.ToString() )
            };
        }
        
        public                                          SetStaticObjectBounds( ImportBase parent, Vector3i min, Vector3i max )
        : base( parent )
        {
            Min = min;
            Max = max;
        }

        public override bool                            Apply()
        {
            var stat = Target.Value as Static;
            var result = stat != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( Static ).FullName() );
            else
            {
                stat.ObjectBounds.SetMinValue( TargetHandle.Working, Min );
                stat.ObjectBounds.SetMaxValue( TargetHandle.Working, Max );
                result = TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var stat = Target.Value as Static;
            if( stat == null ) return false;
            return
                ( Min == stat.ObjectBounds.GetMinValue( TargetHandle.WorkingOrLastFullRequired ) )&&
                ( Max == stat.ObjectBounds.GetMaxValue( TargetHandle.WorkingOrLastFullRequired ) );
        }
    }

}
