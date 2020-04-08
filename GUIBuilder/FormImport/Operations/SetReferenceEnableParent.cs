/*
 * SetReferenceEnableParent.cs
 *
 * Sets or clears the Enable Parent of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferenceEnableParent : ImportOperation
    {
        
        const string                                    DN_EP_Reference = "EnableParent.ReferenceU";
        const string                                    DN_EP_Flags = "FlagsU";
        const string                                    DN_Clear = "ClearU";

        public delegate bool                            EnableParentChangedEvent( ObjectReference reference, bool linked );

        EnableParentChangedEvent                        OnEnableParentChanged = null;

        readonly ImportTarget                           _Reference;
        readonly uint                                   _Flags;
        
        public override string[]                        OperationalInformation()
        {
            if( _Reference == null ) return new [] {
                string.Format( "{0}: {1}"  , DN_EP_Reference.Translate(), DN_Clear.Translate() ) };
            return new [] {
                string.Format( "{0}: {1}"  , _Reference .DisplayName    , _Reference.NullSafeIDString() ),
                string.Format( "{0}: 0x{1}", DN_EP_Flags.Translate()    , _Flags    .ToString( "X8" )   )
            };
        }

        public                                          SetReferenceEnableParent( ImportBase parent, ObjectReference reference, uint flags, EnableParentChangedEvent onEnableParentChanged = null )
        : base( parent )
        {
            _Reference  = new ImportTarget( parent, DN_EP_Reference.Translate(), typeof( ObjectReference ), reference );
            _Flags      = flags;
            if( onEnableParentChanged != null )
                OnEnableParentChanged += onEnableParentChanged;
        }

        public                                          SetReferenceEnableParent( ImportBase parent, EnableParentChangedEvent onEnableParentChanged = null )
        : base( parent )
        {
            if( onEnableParentChanged != null )
                OnEnableParentChanged += onEnableParentChanged;
        }

        public override bool                            Apply()
        {
            var refr = Target.Value as ObjectReference;
            var result = refr != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( ObjectReference ).FullName() );
            else
            {
                ObjectReference newRefr = _Reference?.Value as ObjectReference;
                ObjectReference oldRefr = null;

                if( OnEnableParentChanged != null )
                    oldRefr = refr.EnableParent.GetReference( TargetHandle.WorkingOrLastFullRequired );

                if( newRefr == null )
                    refr.EnableParent.DeleteRootElement( false, false );
                else
                {
                    refr.EnableParent.SetReference( TargetHandle.Working, newRefr );
                    refr.EnableParent.SetFlags( TargetHandle.Working, _Flags );
                }

                if( ( oldRefr != null )&&( OnEnableParentChanged != null ) )
                    result &= OnEnableParentChanged.Invoke( oldRefr, false );

                if( ( newRefr != null )&&( OnEnableParentChanged != null ) )
                    result &= OnEnableParentChanged.Invoke( newRefr, true );

                result &= TargetMatchesImport();
            }
            return result;
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;
            
            ObjectReference newRefr = _Reference?.Value as ObjectReference;
            var eRef = refr.EnableParent.GetReference( TargetHandle.WorkingOrLastFullRequired );
            if( newRefr != eRef ) return false;
            
            return _Flags == refr.EnableParent.GetFlags( TargetHandle.WorkingOrLastFullRequired );
        }

    }
}
