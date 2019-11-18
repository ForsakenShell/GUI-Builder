/*
 * CachedIntField.cs
 *
 * Common (cached) int field.
 *
 */

using System;
using XeLib;
using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedIntField: ValueField<int>
    {
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<int>       cache                       = null;
        
        public                          CachedIntField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<int>( GetRawValue, SetRawValue );
        }
        
        public override int             GetValue( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, int value )
        {
            var h = Form.HandleFromTarget( target );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = Form.HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.IsNullOrEmpty( format ) ? "0x" + v.ToString( "X8" ) : string.Format( format, v );
        }
        
        int                             GetRawValue( ElementHandle handle )
        {
            return ReadInt( handle, BuildSubPath( SubPath, RootElement ) );
        }
        
        void                            SetRawValue( ElementHandle handle, int value )
        {
            WriteInt( BuildSubPath( SubPath, RootElement ), value, true );
        }
        
    }
    
}
