using System;
using XeLib.API;
using XeLib.Internal;

namespace XeLib
{
    
    [HandleMapping( new[] { Elements.ElementTypes.EtStruct } )]
    public class ScriptPropertyHandle : ElementHandle
    {
        
        public ScriptPropertyHandle( uint uHandle ) : base( uHandle ) {}
        
        public override string ToStringExtra()
        {
            return Disposed
                ? null
                : string.Format( "Property = \"{0}\"", this.PropertyName );
        }
        
        public enum PropertyTypes
        {
            None = 0,
            Object = 1,
            String = 2,
            Int32 = 3,
            Float = 4,
            Bool = 5,
            Variable = 6,
            Struct = 7,
            ObjectArray = 11,
            StringArray = 12,
            Int32Array = 13,
            FloatArray = 14,
            BoolArray = 15,
            VariableArray = 16,
            StructArray = 17
        }
        
        #region Record ID - PropertyName
        
        public PropertyTypes PropertyType
        {
            get
            {
                return (PropertyTypes)ElementValues.GetIntValueEx( XHandle, "Type" );
            }
            set
            {
                ElementValues.SetIntValueEx( XHandle, "Type", (int)value );
            }
        }
        
        public string PropertyName
        {
            get
            {
                return ElementValues.GetValueEx( XHandle, "propertyName" );
            }
            set
            {
                ElementValues.SetValueEx( XHandle, "propertyName", value );
            }
        }
        
        #endregion
        
        public override string Signature { get { return null; } }
        
        #region Element Values
        
        protected string ValueSubPath( PropertyTypes type, int index = 0 )
        {
            /* Type are as follows:
             * Value    Type        Path
             * 1        Object      "Value\Object Union\Object v2\FormID" (Type = Int32)
             *                      "Value\Object Union\Object v2\Alias" (Type = Int32)
             * 2        String      "String"
             * 3        Int32       "Int32"
             * 4        Float       "Float"
             * 5        Bool        "Bool" (False = 0, True = 1)
             * 6        Variable    "Value\Struct"
             * 7        Struct      "Value\Struct"
             * Arrays: 'X' is the array index
             * 11       Object[]    "Value\Array of Object\Object Union #X\Object v2\FormID"
             *                      "Value\Array of Object\Object Union #X\Object v2\Alias"
             * 12       String[]    "Value\Array of String\Element #X"
             * 13       Int32[]     "Value\Array of Int32\Element #X"
             * 14       Float[]     "Value\Array of Float\Element #X"
             * 15       Bool[]      "Value\Array of Bool\Element #X"
             * 16       Variable[]  null
             * 17       Struct[]    "Value\Array of Struct\Struct #X"
             * 
             * Structs are formatted the same way as the property itself except with "memberName" as the identifier
            */
            switch( type )
            {
                case PropertyTypes.Object:      return @"Value\Object Union\Object v2\FormID";
                case PropertyTypes.String:      return "String";
                case PropertyTypes.Int32:       return "Int32";
                case PropertyTypes.Float:       return "Float";
                case PropertyTypes.Bool:        return "Bool";
                case PropertyTypes.Variable:    return @"Value\Struct";
                case PropertyTypes.Struct:      return @"Value\Struct";
                case PropertyTypes.ObjectArray: return string.Format( @"Value\Array of Object\Object Union #{0}\Object v2\FormID", index );
                case PropertyTypes.StringArray: return string.Format( @"Value\Array of String\Element #{0}", index );
                case PropertyTypes.Int32Array:  return string.Format( @"Value\Array of Int32\Element #{0}", index );
                case PropertyTypes.FloatArray:  return string.Format( @"Value\Array of Float\Element #{0}", index );
                case PropertyTypes.BoolArray:   return string.Format( @"Value\Array of Bool\Element #{0}", index );
                case PropertyTypes.StructArray: return string.Format( @"Value\Array of Struct\Struct #{0}", index );
            }
            return null;
        }
        
        #region String Values
        
        public override string GetValue()
        {
            return GetValue( 0 );
        }
        
        public override bool SetValue( string value )
        {
            return SetValue( 0, value );
        }
        
        public virtual string GetValue( int index )
        {
            return ElementValues.GetValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetValue( int index, string value )
        {
            return ElementValues.SetValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        #endregion
        
        #region Bool Values
        
        public override bool GetBoolValue()
        {
            return GetBoolValue( 0 );
        }
        
        public override bool SetBoolValue( bool value )
        {
            return SetBoolValue( 0, value );
        }
        
        public virtual bool GetBoolValue( int index )
        {
            return ElementValues.GetBoolValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetBoolValue( int index, bool value )
        {
            return ElementValues.SetBoolValueEx( this.XHandle, ValueSubPath( PropertyType, index ), value );
        }
        
        #endregion
        
        #region Integer Values
        
        public override int GetIntValue()
        {
            return GetIntValue( 0 );
        }
        
        public override bool SetIntValue( int value )
        {
            return SetIntValue( 0, value );
        }
        
        public override uint GetUIntValue()
        {
            return GetUIntValue( 0 );
        }
        
        public override bool SetUIntValue( uint value )
        {
            return SetUIntValue( 0, value );
        }
        
        public virtual int GetIntValue( int index )
        {
            return ElementValues.GetIntValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetIntValue( int index, int value )
        {
            return ElementValues.SetIntValueEx( this.XHandle, ValueSubPath( PropertyType, index ), value );
        }
        
        public virtual uint GetUIntValue( int index )
        {
            return ElementValues.GetUIntValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetUIntValue( int index, uint value )
        {
            return ElementValues.SetUIntValueEx( this.XHandle, ValueSubPath( PropertyType, index ), value );
        }
        
        #endregion
        
        #region Real Number Values
        
        public override double GetDoubleValue()
        {
            return GetDoubleValue( 0 );
        }
        
        public override bool SetDoubleValue( double value )
        {
            return SetDoubleValue( 0, value );
        }
        
        public override float GetFloatValue()
        {
            return GetFloatValue( 0 );
        }
        
        public override bool SetFloatValue( float value )
        {
            return SetFloatValue( 0, value );
        }
        
        public virtual double GetDoubleValue( int index )
        {
            return ElementValues.GetDoubleValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetDoubleValue( int index, double value )
        {
            return ElementValues.SetDoubleValueEx( this.XHandle, ValueSubPath( PropertyType, index ), value );
        }
        
        public virtual float GetFloatValue( int index )
        {
            return (float)ElementValues.GetFloatValueEx( this.XHandle, ValueSubPath( PropertyType, index ) );
        }
        
        public virtual bool SetFloatValue( int index, float value )
        {
            return ElementValues.SetFloatValueEx( this.XHandle, ValueSubPath( PropertyType, index ), value );
        }
        
        #endregion
        
        #endregion
        
    }
}