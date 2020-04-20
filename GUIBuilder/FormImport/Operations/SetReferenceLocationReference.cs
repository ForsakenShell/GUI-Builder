/*
 * SetReferenceLocationReference.cs
 *
 * Sets the Location Reference of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferenceLocationReference : ImportOperation
    {
        
        const string                                    DN_Location = "Reference.LocationRefU";
        const string                                    DN_Clear = "ClearU";

        readonly ImportTarget                           _Location;

        bool                                            ClearValue
        {
            get
            {
                return ( _Location == null )||( !_Location.FormID.ValidFormID() );
            }
        }
        
        public override string[]                        OperationalInformation()
        {
            return new [] {
                (
                    ClearValue
                    ? string.Format( "{0}: {1}", DN_Location.Translate(), DN_Clear .Translate()        )
                    : string.Format( "{0}: {1}", _Location  .DisplayName, _Location.NullSafeIDString() )
                )
            };
        }

        public                                          SetReferenceLocationReference( ImportBase parent )
        : base( parent )
        {
            _Location = null;
        }

        public                                          SetReferenceLocationReference( ImportBase parent, Location value )
        : base( parent )
        {
            _Location = new ImportTarget( parent, DN_Location.Translate(), typeof( Location ), value );
        }

        public                                          SetReferenceLocationReference( ImportBase parent, string locationEditorID )
        : base( parent )
        {
            _Location = new ImportTarget( parent, DN_Location.Translate(), typeof( Location ), locationEditorID );
        }
        public override bool                            Resolve( bool errorIfUnresolveable )
        {
            _Location?.Resolve( false );
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
                if( ClearValue )
                    result = refr.LocationReference.DeleteRootElement( false, false );
                else
                {
                    refr.LocationReference.SetValue( TargetHandle.Working, _Location.FormID );
                    result = TargetMatchesImport();
                }
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            
            if( ClearValue )
                return !refr.LocationReference.HasValue( TargetHandle.WorkingOrLastFullRequired );

            return _Location.FormID == refr.LocationReference.GetValue( TargetHandle.WorkingOrLastFullRequired );
        }
    }

}