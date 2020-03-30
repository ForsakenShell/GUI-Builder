using System;
using System.Collections.Generic;
using XeLib.API;
using XeLib.Internal;

namespace XeLib
{
    
    [HandleMapping( new[] { Elements.ElementTypes.EtFile } )]
    public class FileHandle : ElementHandle
    {
        
        public FileHandle( uint uHandle ) : base( uHandle ) {}
        
        #region File Management
        
        public bool NukeFile()
        {
            return Files.NukeFileEx( this.XHandle );
        }
        
        public bool RenameFile( string newFilename )
        {
            return Files.RenameFileEx( this.XHandle, newFilename );
        }
        
        public bool SaveFile( string filePath = "" )
        {
            return Files.SaveFileEx( this.XHandle, filePath );
        }
        
        #endregion
        
        #region File Checksums
        
        public string MD5Hash()
        {
            return Files.MD5HashEx( this.XHandle );
        }
        
        public string CRCHash()
        {
            return Files.CRCHashEx( this.XHandle );
        }
        
        #endregion
        
        #region Record Meta
        
        public int GetRecordCount()
        {
            return Files.GetRecordCountEx( this.XHandle );
        }
        
        public int GetOverrideRecordCount()
        {
            return Files.GetOverrideRecordCountEx( this.XHandle );
        }
        
        public uint NextObjectID
        {
            get
            {
                return FileValues.GetNextObjectIDEx( this.XHandle );
            }
            set
            {
                FileValues.SetNextObjectIDEx( this.XHandle, value );
            }
        }
        
        #endregion
        
        #region File Meta
        
        public string Author
        {
            get { return FileValues.GetFileAuthorEx( this.XHandle ); }
            set { FileValues.SetFileAuthorEx( this.XHandle, value ); }
        }
        
        public string Description
        {
            get { return FileValues.GetFileDescriptionEx( this.XHandle ); }
            set { FileValues.SetFileDescriptionEx( this.XHandle, value ); }
        }
        
        public override uint LoadOrder      { get { return Files.GetFileLoadOrderEx( this.XHandle ); } }
        
        public override string Signature    { get { return IsESM ? "Plugin.Master".Translate() + " (ESM)" : IsESL ? "PLugin.LightPlugin".Translate() + " (ESL)" : "Plugin.Plugin".Translate() + " (ESP)"; } }
        
        public bool IsESM
        {
            get { return FileValues.GetIsEsmEx( this.XHandle ); }
            set { FileValues.SetIsEsmEx( this.XHandle, value ); }
        }
        
        public bool IsESL
        {
            get { return FileValues.GetIsEslEx( this.XHandle ); }
            set { FileValues.SetIsEslEx( this.XHandle, value ); }
        }
        
        public bool IsESP
        {
            get { return FileValues.GetIsEspEx( this.XHandle ); }
        }
        
        public ElementHandle GetFileHeader()
        {
            return Files.GetFileHeaderEx<ElementHandle>( this.XHandle );
        }
        
        public bool BuildReferences( bool sync = true )
        {
            return Setup.BuildReferencesEx( this.XHandle, sync );
        }
        
        #endregion
        
        #region Record Sorting
        
        public bool SortEditorIDs( string signature )
        {
            return Files.SortEditorIDsEx( this.XHandle, signature );
        }
        
        public bool SortNames( string signature )
        {
            return Files.SortNamesEx( this.XHandle, signature );
        }
        
        #endregion
        
        #region Masters
        
        public bool CleanMasters()
        {
            return Masters.CleanMastersEx( this.XHandle );
        }
        
        public bool SortMasters()
        {
            return Masters.SortMastersEx( this.XHandle );
        }
        
        public bool AddMaster( string filename )
        {
            return Masters.AddMasterEx( this.XHandle, filename );
        }
        
        public bool AddRequiredMasters( ElementHandle handle, bool asNew = false )
        {
            return Masters.AddRequiredMastersEx( handle.XHandle, this.XHandle, asNew );
        }
        
        public FileHandle[] GetMasters()
        {
            return Masters.GetMastersEx( this.XHandle );
        }

        /// <summary>
        /// Returns the index of the specified file in the plugin specific dependencies.
        /// </summary>
        /// <param name="filename">Filename of master file</param>
        /// <returns>-1 on error, > -1 as an index with the plugin itself always being the last index</returns>
        public int IndexInMasters( string filename )
        {
            if( string.IsNullOrEmpty( filename ) ) return -1;

            var masters = Masters.GetMastersEx( this.XHandle );
            var masterCount = masters.NullOrEmpty() ? 0 : masters.Length;
            if( masterCount == 0 ) return -1;

            int result = -1;

            if( this.Filename.InsensitiveInvariantMatch( filename ) )
                result = masterCount;

            else
            {
                for( int i = 0; i < masterCount; i++ )
                {
                    if( ( masters[ i ].IsValid() ) && ( masters[ i ].Filename.InsensitiveInvariantMatch( filename ) ) )
                    {
                        result = i;
                        break;
                    }
                }
            }

            for( int i = 0; i < masterCount; i++ )
            {
                DebugLog.WriteLine( "Disposing of :: " + masters[ i ].ToStringNullSafe() );
                masters[ i ].Dispose();
            }

            return result;
        }

        public static List<string> GetMastersOf( string filename )
        {
            List<string> masters = null;
            var fileHandle = XeLib.API.Setup.LoadPluginHeaderEx<XeLib.FileHandle>( filename );
            if( fileHandle.IsValid() )
            {
                var fileHeader = fileHandle.GetElement<XeLib.ElementHandle>( "File Header" );
                if( fileHeader.IsValid() )
                {
                    var mfs = fileHeader.GetElement<XeLib.ElementHandle>( "Master Files" );
                    if( mfs.IsValid() )
                    {
                        var mfl = mfs.GetElements<XeLib.ElementHandle>();
                        if( !mfl.NullOrEmpty() )
                        {
                            masters = new List<string>();
                            for( int j = 0; j < mfl.Length; j++ )
                            {
                                masters.Add( mfl[ j ].GetValueEx( "MAST" ) );
                                mfl[ j ].Dispose();
                            }
                        }
                        mfs.Dispose();
                    }
                    fileHeader.Dispose();
                }
                //fileHandle.Dispose(); // <-- These seem to be shared internally by XeLib???  MOAR RESEARCH!
            }
            return masters;
        }
        
        #endregion
        
    }
}
