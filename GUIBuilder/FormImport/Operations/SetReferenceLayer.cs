/*
 * SetReferenceLayer.cs
 *
 * Sets the Layer of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferenceLayer : ImportOperation
    {
        
        const string                                    DN_Layer = "Form.LayerU";
        const string                                    DN_Clear = "ClearU";

        readonly ImportTarget                           _Layer;

        public override string[]                        OperationalInformation()
        {
            return new [] {
                (
                    ( _Layer == null )
                    ? string.Format( "{0}: {1}", DN_Layer.Translate(), DN_Clear.Translate()        )
                    : string.Format( "{0}: {1}", _Layer  .DisplayName, _Layer  .NullSafeIDString() )
                )
            };
        }

        public                                          SetReferenceLayer( ImportBase parent )
        : base( parent )
        {
            _Layer = null;
        }

        public                                          SetReferenceLayer( ImportBase parent, Layer value )
        : base( parent )
        {
            _Layer = new ImportTarget( parent, DN_Layer.Translate(), typeof( Layer ), value );
        }

        public                                          SetReferenceLayer( ImportBase parent, string layerEditorID )
        : base( parent )
        {
            _Layer = new ImportTarget( parent, DN_Layer.Translate(), typeof( Layer ), layerEditorID );
        }
        public override bool                            Resolve( bool errorIfUnresolveable )
        {
            _Layer?.Resolve( false );
            return true;
        }

        public override bool                            Apply()
        {
            var refr = Target.Value as ObjectReference;
            var result = refr != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( ObjectReference ).FullName() );
            else
            {
                if( ( _Layer == null )||( _Layer.Value == null ) )
                    refr.SetLayer( TargetHandle.Working, null );
                else
                {
                    refr.SetLayerFormID( TargetHandle.Working, _Layer.FormID );
                    result = TargetMatchesImport();
                }
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            var lFID = refr.GetLayerFormID( TargetHandle.WorkingOrLastFullRequired );
            return lFID == ( ( _Layer != null ) ? _Layer.FormID : 0 );
        }
    }

}