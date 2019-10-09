/*
 * DistantLOD.cs
 *
 * Static Object Distance LOD field.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms.Fields.Static
{
    
    public class DistantLOD : ValueField<string[]> //<Structs.DistantLOD>
    {
        
        //const string _LOD   = @"LOD";
        const string                   _LOD_0                       = @"LOD #0 (Level 0)\Mesh";
        const string                   _LOD_1                       = @"LOD #1 (Level 1)\Mesh";
        const string                   _LOD_2                       = @"LOD #2 (Level 2)\Mesh";
        const string                   _LOD_3                       = @"LOD #3 (Level 3)\Mesh";
        
        ElementHandle                  cached_Handle                = null;
        string[]                       _value                       = null;
        
        public                          DistantLOD( Form form ) : base( form, "MNAM - Distant LOD" ) {}
        
        public override string[]        GetValue( TargetHandle target )
        {
            var h = HandleFromTarget( target );
            if( ( cached_Handle != null )&&( h == cached_Handle ) ) return _value;
            cached_Handle = h;
            _value = !HasValue( h )
                ? null
                : new string[]{
                        ReadString( h, _LOD_0 ),
                        ReadString( h, _LOD_1 ),
                        ReadString( h, _LOD_2 ),
                        ReadString( h, _LOD_3 ) };
            return _value;
        }
        
        public override void            SetValue( TargetHandle target, string[] value )
        {
            if( ( value == null )||( value.Length < 1 ) )
                return;
            
            var h = HandleFromTarget( target );
            
            _value = value;
            cached_Handle = h;
            if( value.Length >= 1 )
                WriteString( _LOD_0, value[ 0 ], false );
            if( value.Length >= 2 )
                WriteString( _LOD_1, value[ 1 ], false );
            if( value.Length >= 3 )
                WriteString( _LOD_2, value[ 2 ], false );
            if( value.Length >= 4 )
                WriteString( _LOD_3, value[ 3 ], false );
            
            Form.SendObjectDataChangedEvent();
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var v = GetValue( target );
            if( v == null )
                return null;
            
            var s = string.Empty;
            for( var i = 0; i <= Math.Min( 3, v.Length ); i++ )
            {
                s += string.Format(
                    string.IsNullOrEmpty( format ) ? "LOD #{0}: \"{1}\"" : format,
                    i, v[ i ] );
                s += "\n";
            }
            return s;
        }
        
    }
    
}
