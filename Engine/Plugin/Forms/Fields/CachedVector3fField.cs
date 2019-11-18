/*
 * CachedVector3fField.cs
 *
 * Common (cached) Vector3f field.
 *
 */

using System;
using XeLib;
using Engine.Plugin.Extensions;
using Maths;


namespace Engine.Plugin.Forms.Fields
{
    
    public class CachedVector3fField : ValueField<Vector3f>
    {
        
        const string                   _X                           = "X";
        const string                   _Y                           = "Y";
        const string                   _Z                           = "Z";
        
        readonly string                 SubPath                     = null;
        
        CachedGetSetIXHandle<Vector3f>  cache = null;
        
        public                          CachedVector3fField( Form form, string xpath, string subpath = null ) : base( form, xpath )
        {
            SubPath = subpath;
            cache = new CachedGetSetIXHandle<Vector3f>( GetRawValue, SetRawValue );
        }
        
        public override Vector3f        GetValue( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            return cache.GetValue( h );
        }
        
        public override void            SetValue( TargetHandle target, Vector3f value )
        {
            var h = Form.HandleFromTarget( target );
            if( !h.IsValid() )
                throw new ArgumentException( "target is not valid for field" );
            cache.SetValue( h, value );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var h = Form.HandleFromTarget( target );
            var v = GetRawValue( h );
            return string.Format(
                string.IsNullOrEmpty( format ) ? "({0},{1},{2})" : format,
                v.X, v.Y, v.Z );
        }
        
        Vector3f                        GetRawValue( ElementHandle handle )
        {
            return !HasValue( handle )
                ? Vector3f.Zero
                : new Vector3f(
                    ReadFloat( handle, BuildSubPath( SubPath, _X ) ),
                    ReadFloat( handle, BuildSubPath( SubPath, _Y ) ),
                    ReadFloat( handle, BuildSubPath( SubPath, _Z ) ));
        }
        
        void                            SetRawValue( ElementHandle handle, Vector3f value )
        {
            WriteFloat( BuildSubPath( SubPath, _X ), value.X, false );
            WriteFloat( BuildSubPath( SubPath, _Y ), value.Y, false );
            WriteFloat( BuildSubPath( SubPath, _Z ), value.Z, true );
        }
        
    }
    
}
