/*
 * RawProperty.cs
 * 
 * Abstraction layer for plugin script properties, all script properties use RawProperty as their base class.
 *
 */

using System;
using System.Linq;
using XeLib;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.PapyrusScripts
{
    
    public abstract class RawProperty
    {
        
        // TODO:  Make this more generic for other uses outside of GUIBuilder, for now only worry about our specific needs
        // Specifically, the writers ignoring TargetHandle and always using TargetHandle.Working
        
        #region Required Override Members
        
        public virtual string          ToString( TargetHandle target, string format = null )
        {
            var ph = GetProperty( target, false );
            var phv = ph.IsValid();
            var pt = !phv ? ScriptPropertyHandle.PropertyTypes.None : ph.PropertyType;
            return string.Format(
                "{0} = {1}",
                PropertyName,
                !phv
                ? "[null]"
                : pt == ScriptPropertyHandle.PropertyTypes.Object
                ? string.Format( "0x{0}", ph.GetUIntValue( 0 ).ToString( "X8" ) )
                : pt == ScriptPropertyHandle.PropertyTypes.String
                ? ph.GetValue( 0 )
                : pt == ScriptPropertyHandle.PropertyTypes.Int32
                ? ph.GetIntValue( 0 ).ToString()
                : pt == ScriptPropertyHandle.PropertyTypes.Float
                ? ph.GetFloatValue( 0 ).ToString()
                : pt == ScriptPropertyHandle.PropertyTypes.Bool
                ? ph.GetBoolValue( 0 ).ToString()
                : pt.ToString()
               );
        }
        
        #endregion
        
        protected readonly string       PropertyName                = "";
        
        protected readonly PapyrusScript Script                     = null;
        
        protected                       RawProperty( PapyrusScript script, string propertyName )
        {
            if( script == null )
                throw new ArgumentNullException( "script" );
            Script = script;
            PropertyName = propertyName;
        }
        
        ScriptPropertyHandle            GetProperty( TargetHandle target, bool createOverride )
        {
            var sh = Script.HandleFromTarget( target ) as ScriptHandle;
            if( !sh.IsValid() )
            {
                if( ( !createOverride )||( target != TargetHandle.Working ) ) return null;
                sh = Script.CopyAsOverride() as ScriptHandle;
                if( !sh.IsValid() ) return null;
            }
            var ph = sh.GetProperty( PropertyName );
            if( !ph.IsValid() )
            {
                if( ( !createOverride )||( target != TargetHandle.Working ) ) return null;
                ph = sh.AddProperty( ScriptPropertyHandle.PropertyTypes.Float, PropertyName );
            }
            return ph;
        }
        
        #region Plugin script queries
        
        public bool                     DeleteProperty( bool createOverride, bool sendObjectDataChangedEvent )
        {
            //DebugLog.Write( string.Format( "{0} :: DeleteProperty :: createOverride = {1}", this.FullTypeName(), createOverride ) );
            if( !Script.IsInWorkingFile() )
            {
                if( !createOverride ) return false;
                if( !Script.CopyAsOverride().IsValid() ) return false;
            }
            var sh = Script.HandleFromTarget( TargetHandle.Working ) as ScriptHandle;
            var result = sh.IsValid() && sh.RemoveProperty( PropertyName );
            if( result && sendObjectDataChangedEvent )
                Script.SendObjectDataChangedEvent( null );
            return result;
        }
        
        protected float                 ReadFloat( TargetHandle target, int index, float defaultValue = 0.0f )
        {
            var result = defaultValue;
            var ph = GetProperty( target, false );
            if( ph.IsValid() )
            {
                result = ph.GetFloatValue( index );
                ph.Dispose();
            }
            return result;
        }
        protected void                  WriteFloat( TargetHandle target, int index, float value, bool sendObjectDataChangedEvent )
        {
            var ph = GetProperty( target, true );
            if( !ph.IsValid() ) return;
            ph.SetFloatValue( index, value );
            ph.Dispose();
            if( sendObjectDataChangedEvent ) Script.SendObjectDataChangedEvent( null );
        }
        
        protected int                   ReadInt( TargetHandle target, int index, int defaultValue = 0 )
        {
            var result = defaultValue;
            var ph = GetProperty( target, false );
            if( ph.IsValid() )
            {
                result = ph.GetIntValue( index );
                ph.Dispose();
            }
            return result;
        }
        protected void                  WriteInt( TargetHandle target, int index, int value, bool sendObjectDataChangedEvent )
        {
            var ph = GetProperty( target, true );
            if( !ph.IsValid() ) return;
            ph.SetIntValue( index, value );
            ph.Dispose();
            if( sendObjectDataChangedEvent ) Script.SendObjectDataChangedEvent( null );
        }
        
        protected uint                  ReadUInt( TargetHandle target, int index, uint defaultValue = 0 )
        {
            var result = defaultValue;
            var ph = GetProperty( target, false );
            if( ph.IsValid() )
            {
                result = ph.GetUIntValue( index );
                ph.Dispose();
            }
            return result;
        }
        protected void                  WriteUInt( TargetHandle target, int index, uint value, bool sendObjectDataChangedEvent )
        {
            var ph = GetProperty( target, true );
            if( !ph.IsValid() ) return;
            ph.SetUIntValue( index, value );
            ph.Dispose();
            if( sendObjectDataChangedEvent ) Script.SendObjectDataChangedEvent( null );
        }
        
        protected bool                  ReadBool( TargetHandle target, int index, bool defaultValue = false )
        {
            var result = defaultValue;
            var ph = GetProperty( target, false );
            if( ph.IsValid() )
            {
                result = ph.GetBoolValue( index );
                ph.Dispose();
            }
            return result;
        }
        protected void                  WriteBool( TargetHandle target, int index, bool value, bool sendObjectDataChangedEvent )
        {
            var ph = GetProperty( target, true );
            if( !ph.IsValid() ) return;
            ph.SetBoolValue( index, value );
            ph.Dispose();
            if( sendObjectDataChangedEvent ) Script.SendObjectDataChangedEvent( null );
        }
        
        protected string                ReadString( TargetHandle target, int index, string defaultValue = null )
        {
            var result = defaultValue;
            var ph = GetProperty( target, false );
            if( ph.IsValid() )
            {
                result = ph.GetValue( index );
                ph.Dispose();
            }
            return result;
        }
        protected void                  WriteString( TargetHandle target, int index, string value, bool sendObjectDataChangedEvent )
        {
            var ph = GetProperty( target, true );
            if( !ph.IsValid() ) return;
            ph.SetValue( index, value );
            ph.Dispose();
            if( sendObjectDataChangedEvent ) Script.SendObjectDataChangedEvent( null );
        }
        
        #endregion
        
        public override string          ToString()                  { return ToString( TargetHandle.Master, null ); }
        
    }
    
}
