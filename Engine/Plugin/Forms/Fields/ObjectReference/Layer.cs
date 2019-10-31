/*
 * Layer.cs
 *
 * Object Reference Layer field.
 *
 */

using System;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class Layer : CachedUIntField
    {
        
        public Layer( Form form ) : base( form, "XLYR" ) {}
        
        public Forms.Layer GetLayer( TargetHandle target )
        {
            var lrID = GetValue( target );
            return !lrID.ValidFormID()
                ? null
                : GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( lrID, true );
        }
        public void SetLayer( TargetHandle target, Forms.Layer value )
        {
            if( value == null )
                DeleteRootElement( true, true );
            else
                SetValue( target, value.GetFormID( Engine.Plugin.TargetHandle.Master ) );
        }
        
        public override string ToString( TargetHandle target, string format = null )
        {
            var h = HandleFromTarget( target );
            if( !XeLib.HandleExtensions.IsValid( h ) )
                return null;
            var lID = GetValue( target );
            if( lID == Constant.FormID_None ) return null;
            var layer = GodObject.Plugin.Data.Root.Find<Engine.Plugin.Forms.Layer>( lID, true );
            if( layer == null ) throw new Exception( "Unable to load Layer Form 0x" + lID.ToString( "X8" ) );
            return layer.ToString();
            //return string.Format(
            //    "\"{0}\" - 0x{1} - \"{2}\"",
            //    layer.Signature,
            //    layer.FormID.ToString( "X8" ),
            //    layer.EditorID
            //);
        }
        
    }
    
}
