/*
 * FormID.cs
 *
 * FormID field used by all forms.
 *
 */

using System;
using XeLib;
using XeLib.API;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms.Fields.Record
{
    
    public class FormID : ValueField<uint>
    {
        
        public                          FormID( Form form ) : base( form, null ) {}
        
        //protected override bool         HasValue( ElementHandle handle, string path = "" )
        public override bool            HasValue( ElementHandle handle, string path = "" )
        {
            return handle is FormHandle;
        }
        
        public override uint            GetValue( TargetHandle target )
        {
            var fh = Form.HandleFromTarget( target ) as FormHandle;
            if( !fh.IsValid() )
                throw new ArgumentException( "target is not valid for field" );
            return fh.FormID;
        }
        
        public override void            SetValue( TargetHandle target, uint value )
        {
            throw new System.NotImplementedException( "GUIBuilder does not currently support setting the FormID of existing forms!" );
            
            /*
            var fh = HandleFromTarget( target ) as FormHandle;
            if( !fh.IsValid() )
                throw new ArgumentException( "target is not valid for field" );
            
            if( Form.Collection != null )
                Form.Collection.Remove( Form );
                
            fh.FormID = value;
            
                if( Form.Collection != null )
                    Form.Collection.Add( Form );
                
                Form.SendObjectDataChangedEvent( null );
            */
        }
        
        public override string          ToString( TargetHandle target, string format = null )
        {
            var v = GetValue( target );
            return string.Format(
                !string.IsNullOrEmpty( format ) ? format : "0x{0}",
                v.ToString( "X8" ) );
        }
        
    }
    
}
