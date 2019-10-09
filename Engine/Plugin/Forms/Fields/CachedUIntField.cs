/*
 * CachedUIntField.cs
 *
 * Common (cached) uint field.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedUIntField: ValueField<uint>
    {
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<uint>      cache                       = null;
        
        public                          CachedUIntField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<uint>( GetRawValue, SetRawValue );
        }
        
        public override uint            GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, uint value )
        {
            var h = HandleFromTarget( target );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.IsNullOrEmpty( format ) ? "0x" + v.ToString( "X8" ) : string.Format( format, v );
        }
        
        uint                            GetRawValue( ElementHandle handle )
        {
            return ReadUInt( handle, BuildSubPath( SubPath, RootElement ) );
        }
        
        void                            SetRawValue( ElementHandle handle, uint value )
        {
            WriteUInt( BuildSubPath( SubPath, RootElement ), value, true );
        }
        
    }
    
}
