/*
 * SetReferenceLinkedRef.cs
 *
 * Sets or clears a Linked Reference of an ObjectReference
 *
 */
using Engine.Plugin;
using Engine.Plugin.Forms;


namespace GUIBuilder.FormImport.Operations
{

    public class SetReferenceLinkedRef : ImportOperation
    {
        
        const string                                    DN_LR_Reference = "LinkedRef.ReferenceU";
        const string                                    DN_LR_Keyword   = "LinkedRef.KeywordU";

        public delegate bool                            LinkedRefChangedEvent( ObjectReference reference, bool linked );

        LinkedRefChangedEvent                           OnLinkedRefChanged = null;

        readonly ImportTarget                           _Reference;
        readonly ImportTarget                           _Keyword;
        readonly bool                                   _InvertLinkDirection;

        public override string[]                        OperationalInformation()
        {
            return new [] {
                string.Format( "{0}: {1}", _Reference.DisplayName, _Reference.NullSafeIDString() ),
                string.Format( "{0}: {1}", _Keyword  .DisplayName, _Keyword  .NullSafeIDString() )
            };
        }

        public                                          SetReferenceLinkedRef( ImportBase parent, ObjectReference reference, Keyword keyword, bool invertLinkDirection = false, LinkedRefChangedEvent onLinkedRefChanged = null )
        : base( parent )
        {
            _Reference  = new ImportTarget( parent, DN_LR_Reference.Translate(), typeof( ObjectReference ), reference );
            _Keyword    = new ImportTarget( parent, DN_LR_Keyword  .Translate(), typeof( Keyword         ), keyword   );
            _InvertLinkDirection = invertLinkDirection;
            if( onLinkedRefChanged != null )
                OnLinkedRefChanged += onLinkedRefChanged;
        }

        public override bool                            Apply()
        {
            var refr = Target.Value as ObjectReference;
            var result = refr != null;
            if( !result )
                Parent.AddErrorMessage( ErrorTypes.Import, "ImportTarget did not resolve to " + typeof( ObjectReference ).FullName() );
            else
            {
                ObjectReference oldRefr = null;

                if( !_InvertLinkDirection )
                {
                    if( OnLinkedRefChanged != null )
                        oldRefr = refr.LinkedRefs.GetLinkedRef( TargetHandle.WorkingOrLastFullRequired, _Keyword.FormID );

                    refr.LinkedRefs.SetLinkedRef( TargetHandle.Working, _Reference.FormID, _Keyword.FormID );

                    if( OnLinkedRefChanged != null )
                    {
                        if( oldRefr != null )
                            result &= OnLinkedRefChanged.Invoke( oldRefr, false );
                        result &= OnLinkedRefChanged.Invoke( refr, true );
                    }
                }
                else
                {
                    oldRefr = LinkSource( _Keyword.Value as Keyword );
                    if( oldRefr != null )
                    {
                        result = ( oldRefr.CopyAsOverride() != null );
                        if( !result )
                        {
                            Parent.AddErrorMessage( ErrorTypes.Import, "Unable to copy old link parent to working file " + oldRefr.IDString );
                            return false;
                        }
                        oldRefr.LinkedRefs.Remove( TargetHandle.Working, Target.FormID );
                    }

                    var newRefr = _Reference.Value as ObjectReference;
                    result = ( newRefr.CopyAsOverride() != null );
                    if( !result )
                    {
                        Parent.AddErrorMessage( ErrorTypes.Import, "Unable to copy new link parent to working file " + newRefr.IDString );
                        return false;
                    }

                    newRefr.LinkedRefs.SetLinkedRef( TargetHandle.Working, Target.FormID, _Keyword.FormID );
                    
                    if( OnLinkedRefChanged != null )
                    {
                        result &= OnLinkedRefChanged.Invoke( oldRefr, false );
                        result &= OnLinkedRefChanged.Invoke( newRefr, true  );
                    }
                }
            }
            return result && TargetMatchesImport();
        }

        public override bool                            TargetMatchesImport()
        {
            var refr = Target.Value as ObjectReference;
            if( refr == null ) return false;

            return _InvertLinkDirection
                ? _Reference.Value == LinkSource( _Keyword.Value as Keyword )
                : _Reference.Value == refr.LinkedRefs.GetLinkedRef( TargetHandle.WorkingOrLastFullRequired, _Keyword.FormID );
        }

        ObjectReference                                 LinkSource( Keyword linkKeyword )
        {
            var refr = Target.Value as ObjectReference;
            if( ( refr == null )||( linkKeyword == null ) )
                return null;
            
            var refrRefs = refr.References;
            if( refrRefs.NullOrEmpty() ) return null;
            
            var keywordFormID = linkKeyword.GetFormID( TargetHandle.Master );
            
            foreach( var form in refrRefs )
            {
                var refrRef = form as ObjectReference;
                if( refrRef == null ) continue;
                
                var linked = refrRef.LinkedRefs;
                var linkCount = linked.GetCount( TargetHandle.WorkingOrLastFullRequired );
                if( linkCount < 1 ) continue;
                for( int i = 0; i < linkCount; i++ )
                    if( linked.GetKeywordFormID( TargetHandle.WorkingOrLastFullRequired, i ) == keywordFormID )
                        return refrRef;
            }
            
            return null;
        }

    }
}
