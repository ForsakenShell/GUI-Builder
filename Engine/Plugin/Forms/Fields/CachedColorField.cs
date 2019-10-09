/*
 * CachedColorField.cs
 *
 * Common (cached) color field.
 *
 */

using System;
using sdColor = System.Drawing.Color;
using XeLib;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedColorField : ValueField<sdColor>
    {
        
        const string                   _Red                         = "Red";
        const string                   _Green                       = "Green";
        const string                   _Blue                        = "Blue";
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<sdColor>   cache                       = null;
        
        public                          CachedColorField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<sdColor>( GetRawValue, SetRawValue );
        }
        
        public override sdColor         GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, sdColor value )
        {
            var h = HandleFromTarget( target );
            if( !h.IsValid() )
                throw new ArgumentException( "target is not valid for field" );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.Format(
                string.IsNullOrEmpty( format ) ? "({0},{1},{2})" : format,
                v.R, v.G, v.B );
        }
        
        sdColor                         GetRawValue( ElementHandle handle )
        {
            return !HasValue( handle, SubPath )
                ? sdColor.FromArgb( 0 )
                : sdColor.FromArgb( 255,
                    ReadInt( handle, BuildSubPath( SubPath, _Red   ) ),
                    ReadInt( handle, BuildSubPath( SubPath, _Green ) ),
                    ReadInt( handle, BuildSubPath( SubPath, _Blue  ) ) );
        }
        
        void                            SetRawValue( ElementHandle handle, sdColor value )
        {
            WriteInt( BuildSubPath( SubPath, _Red   ), value.R, false );
            WriteInt( BuildSubPath( SubPath, _Green ), value.G, false );
            WriteInt( BuildSubPath( SubPath, _Blue  ), value.B, true  );
        }
        
    }
    
}
