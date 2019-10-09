/*
 * CachedVector3iField.cs
 *
 * Common (cached) Vector2i field.
 *
 */

using System;
using XeLib;

using Maths;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedVector2iField : ValueField<Vector2i>
    {
        
        const string                   _X                           = "X";
        const string                   _Y                           = "Y";
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<Vector2i>  cache                       = null;
        
        public                          CachedVector2iField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<Vector2i>( GetRawValue, SetRawValue );
        }
        
        public override Vector2i        GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, Vector2i value )
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
                string.IsNullOrEmpty( format ) ? "({0},{1})" : format,
                v.X, v.Y );
        }
        
        Vector2i                        GetRawValue( ElementHandle handle )
        {
            return !HasValue( handle )
                ? Vector2i.Zero
                : new Vector2i(
                    ReadInt( handle, BuildSubPath( SubPath, _X ) ),
                    ReadInt( handle, BuildSubPath( SubPath, _Y ) ) );
        }
        
        void                            SetRawValue( ElementHandle handle, Vector2i value )
        {
            WriteInt( BuildSubPath( SubPath, _X ), value.X, false );
            WriteInt( BuildSubPath( SubPath, _Y ), value.Y, true );
        }
        
    }
    
}
