/*
 * RawField.cs
 * 
 * Abstraction layer for plugin form fields, all form fields use RawField or ValueField as their base class.
 *
 */

using System;
using System.Linq;
using XeLib;

using Engine.Plugin.Extensions;


namespace Engine.Plugin.Forms
{
    
    public abstract class RawField
    {
        // TODO:  Make this more generic for other uses outside of GUIBuilder, for now only worry about our specific needs
        // Specifically, the writers ignoring TargetHandle and always using Form.WorkingFileHandle
        
        #region Required Override Members
        
        public abstract string          ToString( TargetHandle target, string format = null );
        
        #endregion
        
        protected const string          RootElement                 = "";
        
        protected Form                  Form                        = null;
        protected string                XPath                       = null;
        
        protected                       RawField( Form form, string xpath )
        {
            if( form == null )
                throw new ArgumentNullException( "form" );
            Form = form;
            XPath = xpath;
        }
        
        protected static string         BuildSubPath( string subPath, string elementPath )
        {
            return string.IsNullOrEmpty( subPath )
                ? elementPath
                : string.Format( @"{0}\{1}", subPath, elementPath );
        }
        
        protected string                BuildPath( string path )
        {
            return string.IsNullOrEmpty( path )
                ? XPath
                : string.Format( @"{0}\{1}", XPath, path );
        }
        
        /*
        static bool HasElement( ElementHandle handle, string path )
        {
            return handle.IsValid() && handle.HasElement( path );
        }
        */
        
        #region Plugin record queries
        
        // The reader and writer functions will call HasValue() and CreateRootElement() respectively.
        // It is especially important for field writers to call CreateRootElement() as it will make
        // sure that the work being done is on the copy of the record in the working file (either as
        // an override or as the original record).
        
        //protected virtual bool          HasValue( ElementHandle handle, string path = "" )
        public virtual bool             HasValue( TargetHandle target, string path = "" )
        {
            return HasValue( Form.HandleFromTarget( target ), path );
        }
        
        public virtual bool             HasValue( ElementHandle handle, string path = "" )
        {
            return handle.HasElement( BuildPath( path ) );
        }
        
        //protected bool                  DeleteRootElement( bool createOverride, bool sendObjectDataChangedEvent )
        public bool                     DeleteRootElement( bool createOverride, bool sendObjectDataChangedEvent )
        {
            //DebugLog.Write( string.Format( "{0} :: CreateRootElement :: createOverride = {1}", this.FullTypeName(), createOverride ) );
            var h = Form.WorkingFileHandle;
            if( !h.IsValid() )
            {
                if( !createOverride ) return false;
                h = Form.CopyAsOverride();
            }
            if( !Form.IsInWorkingFile() ) return false;
            var path = BuildPath( RootElement );
            return !h.HasElement( path ) || h.RemoveElement( path ); // If it doesn't exist the out-come is the same as deleting it
        }
        
        protected bool                  CreateRootElement( bool createOverride, bool sendObjectDataChangedEvent )
        {
            //DebugLog.Write( string.Format( "{0} :: CreateRootElement :: createOverride = {1}", this.FullTypeName(), createOverride ) );
            var h = Form.WorkingFileHandle;
            if( !h.IsValid() )
            {
                if( !createOverride ) return false;
                h = Form.CopyAsOverride();
                if( !h.IsValid() ) return false;
            }
            var path = BuildPath( RootElement );
            if( h.HasElement( path ) ) return true;
            return( createOverride )&&( AddElement( RootElement, sendObjectDataChangedEvent ) );
        }
        
        protected bool                  AddElement( string path, bool sendObjectDataChangedEvent )
        {
            var elementPath = BuildPath( path );
            var elementHandle = Form.WorkingFileHandle.AddElement<ElementHandle>( elementPath );
            if( !elementHandle.IsValid() )
            {
                DebugLog.WriteLine( string.Format(
                    "Unable to AddElement \"{0}\" to {1}",
                    elementPath,
                    this.Form.IDString ),
                    true );
                return false;
            }
            elementHandle.Dispose();
            
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
            
            return true;
        }
        
        protected float                 ReadFloat( ElementHandle handle, string path, float defaultValue = 0.0f )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetFloatValueEx( elementPath );
        }
        protected void                  WriteFloat( string path, float value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetFloatValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }

        protected sbyte                 ReadSByte( ElementHandle handle, string path, sbyte defaultValue = 0 )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetSByteValueEx( elementPath );
        }
        protected void                  WriteSByte( string path, sbyte value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetSByteValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }

        protected byte                  ReadUByte( ElementHandle handle, string path, byte defaultValue = 0 )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetUByteValueEx( elementPath );
        }
        protected void                  WriteUByte( string path, byte value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetUByteValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }

        protected int                   ReadInt( ElementHandle handle, string path, int defaultValue = 0 )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetIntValueEx( elementPath );
        }
        protected void                  WriteInt( string path, int value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetIntValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }
        
        protected uint                  ReadUInt( ElementHandle handle, string path, uint defaultValue = 0 )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetUIntValueEx( elementPath );
        }
        protected void                  WriteUInt( string path, uint value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetUIntValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }
        
        protected string                ReadString( ElementHandle handle, string path, string defaultValue = null )
        {
            var elementPath = BuildPath( path );
            return !handle.HasElement( elementPath )
                ? defaultValue
                : handle.GetValueEx( elementPath );
        }
        protected void                  WriteString( string path, string value, bool sendObjectDataChangedEvent )
        {
            if( !CreateRootElement( true, false ) ) return;
            Form.WorkingFileHandle.SetValueEx( BuildPath( path ), value );
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent( null );
        }
        
        #endregion
        
        public override string          ToString()                  { return ToString( TargetHandle.Master, null ); }
        
    }
    
}
