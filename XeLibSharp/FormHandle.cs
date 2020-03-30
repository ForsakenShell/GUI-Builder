using System;
using XeLib.API;
using XeLib.Internal;

namespace XeLib
{
    
    [HandleMapping( new[] { Elements.ElementTypes.EtMainRecord } )]
    public class FormHandle : ElementHandle
    {
        
        public FormHandle( uint uHandle ) : base( uHandle ) {}
        
        public override string ToStringExtra()
        {
            return Disposed
                ? null
                : string.Format( "Signature = \"{0}\" :: FormID = 0x{1} :: EditorID = \"{2}\"", this.Signature, this.FormID.ToString( "X8" ), this.EditorID );
        }
        
        #region Record Flags
        
        const string            RecordFlags_Path        = @"Record Header\Record Flags";
        const uint              PartialRecord_Value     = 0x00004000;
        const uint              Persistent_Value        = 0x00000400;
        
        public uint RecordFlags
        {
            get
            {
                return Elements.HasElementEx( this.XHandle, RecordFlags_Path )
                    ? ElementValues.GetUIntValueEx( this.XHandle, RecordFlags_Path )
                    : 0;
            }
            set
            {
                if( !Elements.HasElementEx( this.XHandle, RecordFlags_Path ) ) return;
                ElementValues.SetUIntValueEx( this.XHandle, RecordFlags_Path, value );
            }
        }
        
        public string[] GetRecordFlags()
        {
            return RecordValues.GetAllFlagsEx( this.XHandle );
        }
        
        public bool GetRecordFlag( string flag )
        {
            return RecordValues.GetRecordFlagEx( this.XHandle, flag );
        }
        
        public void SetRecordFlag( string flag, bool value )
        {
            RecordValues.SetRecordFlagEx( this.XHandle, flag, value );
        }
        
        public bool IsPartialRecord
        {
            get
            {
                return ( RecordFlags & PartialRecord_Value ) != 0;
            }
        }
        
        public bool IsPersistentRecord
        {
            get
            {
                return ( RecordFlags & Persistent_Value ) != 0;
            }
            set
            {
                RecordFlags = ( RecordFlags & ~Persistent_Value ) | ( value ? Persistent_Value : 0 );
            }
        }
        
        #endregion
        
        #region Record ID - FormID, EditorID
        
        public uint FormID
        {
            get
            {
                return Records.GetFormIDEx( this.XHandle );
            }
            set
            {
                Records.SetFormIDEx( this.XHandle, value );
            }
        }
        
        public string EditorID
        {
            get
            {
                return RecordValues.GetEditorIDEx( this.XHandle );
            }
            set
            {
                RecordValues.SetEditorIDEx( this.XHandle, value );
            }
        }
        
        #endregion
        
        #region Ancestry - [Grand]Parents
        
        public FileHandle[] GetMasters()
        {
            return Masters.GetRequiredByEx( this.XHandle );
        }
        
        public string[] GetMasterNames()
        {
            return Masters.GetMasterNamesEx( this.XHandle );
        }
        
        #region Record overrides and references
        
        public FormHandle[] GetOverrides()
        {
            int len;
            return !Functions.GetOverrides( XHandle, out len )
                ? null
                : Helpers.GetHandleArray<FormHandle>( len );
        }
        
        public FormHandle[] GetReferencedBy()
        {
            return Records.GetReferencedByEx( this.XHandle );
        }
        
        public ScriptHandle GetScript( string scriptName )
        {
            var scriptsElements = GetElements<ElementHandle>( @"VMAD\Scripts" );
            if( scriptsElements.NullOrEmpty() ) return null;
            ScriptHandle result = null;
            foreach( var scriptsElement in scriptsElements )
            {
                var nameElement = scriptsElement.GetElement<XeLib.ElementHandle>( "scriptName" );
                if( nameElement.IsValid() )
                {
                    var elementValue = nameElement.GetValue();
                    if( elementValue.InsensitiveInvariantMatch( scriptName ) )
                        result = new ScriptHandle( scriptsElement.XHandle );
                    nameElement.Dispose();
                }
                if( ( result == null )||( result.XHandle != scriptsElement.XHandle ) )
                    scriptsElement.Dispose();
            }
            return result;
        }
        
        #endregion
        
        #endregion
        
        #region Handle Aliasing
        
        public FormHandle GetMasterRecord()
        {
            if( this.IsMaster ) return this;
            uint resHandle;
            return !Functions.GetMasterRecord( XHandle, out resHandle )
                ? null
                : Helpers.CreateHandle<FormHandle>( resHandle );
        }
        
        #endregion
        
        #region Debugging
        
        public void DumpContainerTree()
        {
            DebugLog.WriteLine( string.Format(
                "IXHandle.IDString".Translate(),
                FormID.ToString( "X8" ),
                EditorID ),
                true );
            uint uContainer = XHandle;
            while( XeLib.Internal.Functions.GetContainer( uContainer, out uContainer ) )
            {
                var et = Elements.ElementTypeEx( uContainer );
                uint fid = 0;
                string eid = null;
                fid = Records.GetFormIDEx( uContainer );
                eid = RecordValues.GetEditorIDEx( uContainer );
                string id = null;
                if( ( fid != 0xFFFFFFFF )||( !string.IsNullOrEmpty( id ) ) )
                    id = " :: " + string.Format(
                        "IXHandle.IDString".Translate(),
                        fid.ToString( "X8" ),
                        eid );
                DebugLog.WriteLine( string.Format(
                    "\tContainer :: {0}{1}",
                    et.ToString(),
                    id ) );
            }
        }
        
        #endregion
        
    }
}