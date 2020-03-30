/*
 * LocationRefTypes.cs
 *
 * Location Reference Types attached to reference.
 *
 */

using System.Collections.Generic;

using XeLib;
using XeLib.API;

using Engine.Plugin.Extensions;


using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Engine.Plugin.Forms.Fields.ObjectReference
{
    
    public class LocationRefTypes : RawField
    {

        static readonly string         _Reference = "Ref #{0}";

        ElementHandle                  _LastHandle = null;
        List<uint>                     _LocationRefs = null;
        
        public LocationRefTypes( Form form ) : base( form, "XLRT" ) { }

        void ClearCurrentLocationRefs()
        {
            _LastHandle = null;
            _LocationRefs = null;
        }

        void GetLocationRefsFromForm( TargetHandle target )
        {
            var h = Form.HandleFromTarget( target );
            if( _LastHandle == h ) return;

            ClearCurrentLocationRefs();

            _LastHandle = h;

            if( !HasValue( target ) )
                return;

            var lrtE = h.GetElement<ElementHandle>( XPath );
            if( !lrtE.IsValid() )
                return;
            XeLib.Internal.Functions.ElementCount( lrtE.XHandle, out int count );
            lrtE.Dispose();
            if( count < 1 ) return;

            var list = new List<uint>();
            for( int i = 0; i < count; i++ )
                list.Add( ReadUInt( h, string.Format( _Reference, i ) ) );
            
            _LocationRefs = list;
        }

        public int GetCount( TargetHandle target )
        {
            GetLocationRefsFromForm( target );
            return _LocationRefs.NullOrEmpty()
                ? 0
                : _LocationRefs.Count;
        }

        public uint GetLocationReference( TargetHandle target, int index )
        {
            GetLocationRefsFromForm( target );
            if( ( index < 0 ) || ( index >= _LocationRefs.Count ) )
                return Engine.Plugin.Constant.FormID_None;
            return _LocationRefs[ index ];
        }

        public void SetLocationReference( TargetHandle target, int index, uint value )
        {
            if( target != TargetHandle.Working )
                return;
            GetLocationRefsFromForm( target );
            if( ( index < 0 ) || ( index >= _LocationRefs.Count ) )
                return;
            WriteUInt( string.Format( _Reference, index ), value, true );
        }

        public int AddLocationReference( TargetHandle target, uint value )
        {
            // New element in form
            if( target != TargetHandle.Working ) return -1;
            GetLocationRefsFromForm( target );
            var index = _LocationRefs.Count;

            if( !AddElement( string.Format( _Reference, index ), false ) ) return -1;
            
            WriteUInt( string.Format( _Reference, index ), value, true );

            // Save the new value
            _LocationRefs.Add( value );

            return index;
        }

        public bool HasLocationRef( TargetHandle target, Engine.Plugin.Forms.LocationRef refType )
        {
            if( refType == null ) return false;
            return HasLocationRef( target, refType.GetFormID( TargetHandle.Master ) );
        }

        public bool HasLocationRef( TargetHandle target, uint formID )
        {
            if( !Engine.Plugin.Constant.ValidFormID( formID ) ) return false;
            GetLocationRefsFromForm( target );
            
            return _LocationRefs.NullOrEmpty()
                ? false
                : _LocationRefs.Contains( formID );
        }

        public override string ToString( TargetHandle target, string format = null )
        {
            GetLocationRefsFromForm( target );
            if( _LocationRefs.NullOrEmpty() ) return null;
            var s = "";
            foreach( var lrtFID in _LocationRefs )
            {
                if( !string.IsNullOrEmpty( s ) ) s += ", ";
                s += "0x" + lrtFID.ToString( "X8" );
            }
            if( string.IsNullOrEmpty( s ) ) return null;
            return "[" + s + "]";
        }

    }

}
