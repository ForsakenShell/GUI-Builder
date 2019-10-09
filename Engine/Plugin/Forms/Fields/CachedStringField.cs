/*
 * CachedStringField.cs
 *
 * Common (cached) string field.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedStringField: ValueField<string>
    {
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<string>    cache                       = null;
        
        public                          CachedStringField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<string>( GetRawValue, SetRawValue );
        }
        
        public override string          GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, string value )
        {
            var h = HandleFromTarget( target );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.IsNullOrEmpty( format ) ? v : string.Format( format, v );
        }
        
        string                          GetRawValue( ElementHandle handle )
        {
            return ReadString( handle, BuildSubPath( SubPath, RootElement ) );
        }
        
        void                            SetRawValue( ElementHandle handle, string value )
        {
            WriteString( BuildSubPath( SubPath, RootElement ), value, true );
        }
        
    }
    
}
