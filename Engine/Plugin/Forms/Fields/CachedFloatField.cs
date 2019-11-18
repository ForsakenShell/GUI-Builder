/*
 * CachedFloatField.cs
 *
 * Common (cached) float field.
 *
 */

using System;
using XeLib;
using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedFloatField: ValueField<float>
    {
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<float>     cache                       = null;
        
        public                          CachedFloatField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<float>( GetRawValue, SetRawValue );
        }
        
        public override float           GetValue( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, float value )
        {
            var h = Form.HandleFromTarget( target );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = Form.HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.IsNullOrEmpty( format ) ? v.ToString() : string.Format( format, v );
        }
        
        float                           GetRawValue( ElementHandle handle )
        {
            return ReadFloat( handle, BuildSubPath( SubPath, RootElement ) );
        }
        
        void                            SetRawValue( ElementHandle handle, float value )
        {
            WriteFloat( BuildSubPath( SubPath, RootElement ), value, true );
        }
        
    }
    
}
