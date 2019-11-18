/*
 * ObjectBounds.cs
 *
 * Object Bounds field used by multiple forms.
 *
 */

using System;
using XeLib;

using Maths;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields.Shared
{
    
    public class ObjectBounds : RawField
    {
        
        const string                   _Min_X                       = "X1";
        const string                   _Min_Y                       = "Y1";
        const string                   _Min_Z                       = "Z1";
        const string                   _Max_X                       = "X2";
        const string                   _Max_Y                       = "Y2";
        const string                   _Max_Z                       = "Z2";
        
        ElementHandle                   cached_Min_Handle           = null;
        ElementHandle                   cached_Max_Handle           = null;
        Vector3i                       _value_Min;
        Vector3i                       _value_Max;
        
        public                          ObjectBounds( Form form ) : base( form, "OBND" ) {}
        
        public Vector3i                 GetMinValue( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( ( cached_Min_Handle != null )&&( cached_Min_Handle == h ) ) return _value_Min;
            _value_Min = !HasValue( h )
                ? Vector3i.Zero
                : new Vector3i(
                    ReadInt( h, _Min_X ),
                    ReadInt( h, _Min_Y ),
                    ReadInt( h, _Min_Z ) );
            cached_Min_Handle = h;
            return _value_Min;
        }
        
        public void                     SetMinValue( TargetHandle target, Vector3i value )
        {
            var h = Form.HandleFromTarget( target );
            cached_Min_Handle = h;
            _value_Min = value;
            WriteInt( _Min_X, value.X, false );
            WriteInt( _Min_Y, value.Y, false );
            WriteInt( _Min_Z, value.Z, true  );
        }
        
        public Vector3i                 GetMaxValue( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( ( cached_Max_Handle != null )&&( cached_Max_Handle == h ) ) return _value_Max;
            _value_Max = !HasValue( h )
                ? Vector3i.Zero
                : new Vector3i(
                    ReadInt( h, _Max_X ),
                    ReadInt( h, _Max_Y ),
                    ReadInt( h, _Max_Z ) );
            cached_Max_Handle = h;
            return _value_Max;
        }
        
        public void                     SetMaxValue( TargetHandle target, Vector3i value )
        {
            var h = Form.HandleFromTarget( target );
            cached_Max_Handle = h;
            _value_Max = value;
            WriteInt( _Max_X, value.X, false );
            WriteInt( _Max_Y, value.Y, false );
            WriteInt( _Max_Z, value.Z, true  );
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            return string.Format(
                string.IsNullOrEmpty( format ) ? "{0}-{1}" : format,
                GetMinValue( target ).ToString(),
                GetMaxValue( target ).ToString() );
        }
        
    }
    
}
