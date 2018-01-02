/*
 * bbImportMod.cs
 * 
 * Class to encapsulate the data for creating borders for a specific mod.
 * 
 * User: 1000101
 * Date: 27/11/2017
 * Time: 1:11 PM
 * 
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Border_Builder
{
    /// <summary>
    /// Description of bbImportMod.
    /// </summary>
    public class bbImportMod
    {
        
        private const string BuildVolumeRef_File_Suffix = "_Build_Volumes.txt";
        
        private string _fullPath = null;
        
        public string Name = null;
        
        public string BuildVolumeRef_File = null;
        
        public List<VolumeParent> VolumeParents = null;
        
        #region Constructor
        
        public bbImportMod( string newName, string fullPath )
        {
            // Validate paths and files
            if( string.IsNullOrEmpty( newName ) )
                throw new System.ArgumentException( "newName cannot be null or empty!", fullPath );
            Name = newName;
            if( !bbUtils.TryAssignPath( ref _fullPath, fullPath ) )
                throw new System.ArgumentException( "Invalid path for import mod!", newName );
            // These files are optional but certain functions cannot be performed without them
            bbUtils.TryAssignFile( ref BuildVolumeRef_File, _fullPath + newName + BuildVolumeRef_File_Suffix );
        }
        
        #endregion
        
        #region Preloading
        
        public void Preload()
        {
            
        }
        
        #endregion
        
        #region File loading and parsing
        
        /// <summary>
        /// Parse #Name#_Build_Volumes.txt as exported from xEdit -> Apply Script -> ESM - Export Build Volumes
        /// </summary>
        public void LoadBuildVolumesFile()
        {
            const string cBuildVolumes = "BuildVolumes";
            const string cCount = "Count";
            const string cIndex = "Index";
            const string cPosition = "Position";
            const string cReference = "Reference";
            const string cRotation = "Rotation";
            const string cSize = "Size";
            const string cCell = "Cell";
            
            VolumeParent volumeParent = null;
            
            bool parentOpen = false;
            bool parentCounted = false;
            bool parentIndexing = false;
            int volumeCount = -1;
            int volumeIndex = -1;
            string[] lineWords;
            string[] elementWords;
            
            VolumeParents = new List<VolumeParent>();
            
            var buildVolumeFile = File.ReadLines( BuildVolumeRef_File );
            foreach( string fileLine in buildVolumeFile )
            {
                lineWords = fileLine.ParseImportLine();
                if( !lineWords.NullOrEmpty() )
                {
                    if( !parentOpen )
                    {
                        if( ( lineWords[ 0 ] != cBuildVolumes )||( lineWords.Length != 5 ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        int parentEDID;
                        if( !int.TryParse( lineWords[ 1 ], System.Globalization.NumberStyles.HexNumber, null, out parentEDID ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        int parentWorldspaceEDID;
                        if( !int.TryParse( lineWords[ 4 ], System.Globalization.NumberStyles.HexNumber, null, out parentWorldspaceEDID ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        volumeParent = new VolumeParent(
                            parentEDID,
                            lineWords[ 2 ],
                            lineWords[ 3 ],
                            parentWorldspaceEDID
                        );
                        VolumeParents.Add( volumeParent );
                        // Volume count can now be processed
                        parentOpen = true;
                        parentCounted = false;
                        parentIndexing = false;
                        volumeCount = -1;
                        volumeIndex = -1;
                    }
                    else if( volumeParent == null )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return;
                    }
                    else if( !parentCounted )
                    {
                        if( lineWords.Length != 3 )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Volume count
                        elementWords = lineWords[ 0 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cCount ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !int.TryParse( elementWords[ 1 ], out volumeCount ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Parent position
                        elementWords = lineWords[ 1 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cPosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumeParent.Position ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Parent reference
                        elementWords = lineWords[ 2 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cReference ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cReference ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        volumeParent.Reference = elementWords[ 1 ];
                        // Individual volumes can now be processed
                        parentCounted = true;
                        parentIndexing = true;
                        volumeIndex = 0;
                    }
                    else if( !parentIndexing )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return;
                    }
                    else
                    {
                        if( lineWords.Length != 6 )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Volume index
                        elementWords = lineWords[ 0 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cIndex ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        int currentIndex;
                        if( !int.TryParse( elementWords[ 1 ], out currentIndex ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( currentIndex != volumeIndex )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        Maths.Vector3f volumePosition;
                        Maths.Vector3f volumeSize;
                        Maths.Vector3f volumeRotation;
                        Maths.Vector2i volumeCell;
                        string volumeReference;
                        
                        // Volume position
                        elementWords = lineWords[ 1 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cPosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumePosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume Rotation
                        elementWords = lineWords[ 2 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cRotation ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cRotation ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumeRotation ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cRotation ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume size
                        elementWords = lineWords[ 3 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cSize ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cSize ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumeSize ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cSize ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume cell
                        elementWords = lineWords[ 4 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cCell ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCell ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector2i.TryParse( elementWords[ 1 ], out volumeCell ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cCell ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume reference
                        elementWords = lineWords[ 5 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cReference ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", BuildVolumeRef_File, fileLine, cReference ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        volumeReference = elementWords[ 1 ];
                        
                        BuildVolume buildVolume = new BuildVolume(
                            volumePosition,
                            volumeRotation,
                            volumeSize,
                            volumeCell,
                            volumeReference
                        );
                        
                        volumeParent.BuildVolumes.Add( buildVolume );
                        
                        // Ready for next volume
                        volumeIndex++;
                        
                        if( volumeIndex == volumeCount )
                        {
                            parentOpen = false;
                            parentCounted = false;
                            parentIndexing = false;
                            volumeCount = -1;
                            volumeIndex = -1;
                        }
                    }
                }
            }
        }
        
        #endregion
        
        #region Data enumeration (TODO: Rename to something more appropriate)
        
        public VolumeParent VolumeByFormID( string formID )
        {
            if( !string.IsNullOrEmpty( formID ) )
                foreach( var volumeParent in VolumeParents )
                    if( volumeParent.FormID == formID )
                        return volumeParent;
            
            return null;
        }
        
        #endregion
        
    }
}
