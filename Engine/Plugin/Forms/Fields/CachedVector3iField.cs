/*
 * CachedVector3iField.cs
 *
 * Common (cached) Vector3i field.
 *
 */

using System;
using XeLib;
using Engine.Plugin.Extensions;
using Maths;


namespace Engine.Plugin.Forms.Field
{
    
    public class CachedVector3iField : ValueField<Vector3i>
    {
        
        const string                   _X                           = "X";
        const string                   _Y                           = "Y";
        const string                   _Z                           = "Z";
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<Vector3i>  cache = null;
        
        public                          CachedVector3iField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<Vector3i>( GetRawValue, SetRawValue );
        }
        
        public override Vector3i        GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, Vector3i value )
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
                v.X, v.Y, v.Z );
        }
        
        Vector3i                        GetRawValue( ElementHandle handle )
        {
            return !HasValue( handle )
                ? Vector3i.Zero
                : new Vector3i(
                    ReadInt( handle, BuildSubPath( SubPath, _X ) ),
                    ReadInt( handle, BuildSubPath( SubPath, _Y ) ),
                    ReadInt( handle, BuildSubPath( SubPath, _Z ) ));
        }
        
        void                            SetRawValue( ElementHandle handle, Vector3i value )
        {
            WriteInt( BuildSubPath( SubPath, _X ), value.X, false );
            WriteInt( BuildSubPath( SubPath, _Y ), value.Y, false );
            WriteInt( BuildSubPath( SubPath, _Z ), value.Z, true );
        }
        
    }
    
}
