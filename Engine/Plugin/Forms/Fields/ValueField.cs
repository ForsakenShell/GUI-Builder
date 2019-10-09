/*
 * Field.cs
 * 
 * Abstraction layer for plugin form fields, all form fields use RawField or ValueField as their base class.
 *
 */

using System;
using XeLib;


namespace Engine.Plugin.Forms
{
    
    public abstract class ValueField<TValue> : RawField
    {
        
        #region Required Override Members
        
        public virtual TValue           GetValue( TargetHandle target )
        { throw new NotImplementedException(); }
        
        public virtual void             SetValue( TargetHandle target, TValue value )
        { throw new NotImplementedException(); }
        
        #endregion
        
        protected                       ValueField( Form form, string xpath ) : base( form, xpath ) {}
        
    }
    
}
