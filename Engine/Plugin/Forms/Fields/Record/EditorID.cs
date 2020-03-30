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
            
            if( Form.ParentCollection != null )
                Form.ParentCollection.Remove( Form );
            
            base.SetValue( target, value );
            
            if( Form.ParentCollection != null )
                Form.ParentCollection.Add( Form );
            
            Form.SendObjectDataChangedEvent( null );
        }
        
    }
    
}
