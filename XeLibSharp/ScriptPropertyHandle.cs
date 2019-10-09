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
        
        #region Record ID - PropertyName
        
        public int PropertyType
        {
            get
            {
                return ElementValues.GetIntValueEx( XHandle, "Type" );
            }
            set
            {
                ElementValues.SetIntValueEx( XHandle, "Type", value );
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
        
        private string ValueSubPath( int type, int index = 0 )
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
                case 1: return @"Value\Object Union\Object v2\FormID";
                case 2: return "String";
                case 3: return "Int32";
                case 4: return "Float";
                case 5: return "Bool";
                case 6: return @"Value\Struct";
                case 7: return @"Value\Struct";
                case 11: return string.Format( @"Value\Array of Object\Object Union #{0}\Object v2\FormID", index );
                case 12: return string.Format( @"Value\Array of String\Element #{0}", index );
                case 13: return string.Format( @"Value\Array of Int32\Element #{0}", index );
                case 14: return string.Format( @"Value\Array of Float\Element #{0}", index );
                case 15: return string.Format( @"Value\Array of Bool\Element #{0}", index );
                case 16: return string.Format( @"Value\Array of Struct\Struct #{0}", index );
            }
            return null;
        }
        #region string Values
        
        public override string GetValue()
        {
            return ElementValues.GetValueEx( this.XHandle, ValueSubPath( PropertyType ) );
        }
        
        public override bool SetValue( string value )
        {
            return ElementValues.SetValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        #endregion
        
        #region Integer Values
        
        public override int GetIntValue()
        {
            return ElementValues.GetIntValueEx( this.XHandle, ValueSubPath( PropertyType ) );
        }
        
        public override bool SetIntValue( int value )
        {
            return ElementValues.SetIntValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        public override uint GetUIntValue()
        {
            return ElementValues.GetUIntValueEx( this.XHandle, ValueSubPath( PropertyType ) );
        }
        
        public override bool SetUIntValue( uint value )
        {
            return ElementValues.SetUIntValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        #endregion
        
        #region Real Number Values
        
        public override double GetDoubleValue()
        {
            return ElementValues.GetDoubleValueEx( this.XHandle, ValueSubPath( PropertyType ) );
        }
        
        public override bool SetDoubleValue( double value )
        {
            return ElementValues.SetDoubleValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        public override float GetFloatValue()
        {
            return (float)ElementValues.GetFloatValueEx( this.XHandle, ValueSubPath( PropertyType ) );
        }
        
        public override bool SetFloatValue( float value )
        {
            return ElementValues.SetFloatValueEx( this.XHandle, ValueSubPath( PropertyType ), value );
        }
        
        #endregion
        
        #endregion
        
    }
}