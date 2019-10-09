/*
 * EditorID.cs
 *
 * EditorID field used by all forms.
 *
 */

using System;


namespace Engine.Plugin.Forms.Fields.Record
{
    
    public class EditorID : CachedStringField
    {
        
        public                          EditorID( Form form ) : base( form, "EDID" ) {}
        
        public override void            SetValue( TargetHandle target, string value )
        {
            var cEDID = GetValue( target );
            if( cEDID == value ) return;
            
            if( Form.Collection != null )
                Form.Collection.Remove( Form );
            
            base.SetValue( target, value );
            
            if( Form.Collection != null )
                Form.Collection.Add( Form );
            
            Form.SendObjectDataChangedEvent();
        }
        
    }
    
}
