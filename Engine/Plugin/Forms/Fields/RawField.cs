/*
 * RawField.cs
 * 
 * Abstraction layer for plugin form fields, all form fields use RawField or ValueField as their base class.
 *
 */

using System;
using XeLib;


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
        
        //protected ElementHandle         HandleFromTarget( TargetHandle target )
        public ElementHandle            HandleFromTarget( TargetHandle target )
        {
            ElementHandle h = null;
            switch( target )
            {
                case TargetHandle.Master:
                    h = Form.MasterHandle;
                    break;
                    
                case TargetHandle.LastFullRequired:
                    h = Form.LastFullRequiredHandle;
                    break;
                    
                case TargetHandle.Working:
                    h = Form.WorkingFileHandle;
                    break;
                    
                case TargetHandle.WorkingOrLastFullRequired:
                    h =   Form.WorkingFileHandle.IsValid()      ? Form.WorkingFileHandle
                        : Form.LastFullRequiredHandle.IsValid() ? Form.LastFullRequiredHandle
                        : null;
                    break;
                    
                case TargetHandle.LastFullOptional:
                    h = Form.LastFullOptionalHandle;
                    break;
            }
            if( !h.IsValid() )
                throw new ArgumentException( "target is not valid for field" );
            return h;
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
            return HasValue( HandleFromTarget( target ), path );
        }
        
        public virtual bool             HasValue( ElementHandle handle, string path = "" )
        {
            return handle.HasElement( BuildPath( path ) );
        }
        
        //protected bool                  DeleteRootElement( bool createOverride, bool sendObjectDataChangedEvent )
        public bool                     DeleteRootElement( bool createOverride, bool sendObjectDataChangedEvent )
        {
            //DebugLog.Write( string.Format( "{0} :: CreateRootElement :: createOverride = {1}", this.GetType().ToString(), createOverride ) );
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
            //DebugLog.Write( string.Format( "{0} :: CreateRootElement :: createOverride = {1}", this.GetType().ToString(), createOverride ) );
            var h = Form.WorkingFileHandle;
            if( !h.IsValid() )
            {
                if( !createOverride ) return false;
                h = Form.CopyAsOverride();
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
                    "{0} :: AddElement :: Unable to AddElement \"{1}\" to 0x{2} - \"{3}\"",
                    this.GetType().ToString(),
                    elementPath,
                    this.Form.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" ),
                    this.Form.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) ) );
                return false;
            }
            elementHandle.Dispose();
            
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent();
            
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
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent();
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
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent();
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
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent();
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
            if( sendObjectDataChangedEvent ) Form.SendObjectDataChangedEvent();
        }
        
        #endregion
        
        public override string          ToString()                  { return ToString( TargetHandle.Master, null ); }
        
    }
    
}
