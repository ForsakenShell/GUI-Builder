/*
 * EnableParent.cs
 *
 * Object Reference Enable Parent fields.
 *
 */

using System;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class EnableParent : RawField
    {
        
        const string _XPath         = "XESP";
        
        const string _Reference     = @"Reference";
        const string _Flags         = @"Flags";
        
        CachedUIntField cache_R = null;
        CachedUIntField cache_F = null;
        
        public EnableParent( Form form ) : base( form, _XPath )
        {
            cache_R = new CachedUIntField( form, _XPath, _Reference );
            cache_F = new CachedUIntField( form, _XPath, _Flags );
        }
        
        public uint GetReferenceID( TargetHandle target )
        {
            return cache_R.GetValue( target );
        }
        public void SetReferenceID( TargetHandle target, uint value )
        {
            cache_R.SetValue( target, value );
        }
        
        public Forms.ObjectReference GetReference( TargetHandle target )
        {
            var rID = GetReferenceID( target );
            return !Engine.Plugin.Constant.ValidFormID( rID )
                ? null
                : GodObject.Plugin.Data.Root.Find( rID ) as Forms.ObjectReference;
        }
        public void SetReference( TargetHandle target, Forms.ObjectReference value )
        {
            if( value == null )
                cache_R.DeleteRootElement( true, true );
            else
                SetReferenceID( target, value.GetFormID( Engine.Plugin.TargetHandle.Master ) );
        }
        
        public uint GetFlags( TargetHandle target )
        {
            return cache_F.GetValue( target );
        }
        public void SetFlags( TargetHandle target, uint value )
        {
            cache_F.SetValue( target, value );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "Reference: {0} :: Flags: 0x{1}",
                GetReference( target ).ToString(),
                GetFlags( target ).ToString( "X8" ) );
        }
        
    }
    
}
