/*
 * ImportMod.cs
 * 
 * Class to encapsulate the data for creating borders for a specific mod.
 * 
 * THIS HAS BEEN LONG OBSOLETED!
 * 
 * The only reason this file remains is some code or logic may yet be pilfered from it.
 * 
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Maths;
using Engine;

using GUIBuilder;
using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;

/*
namespace DEPRECATED
{
    /// <summary>
    /// Description of bbImportMod.
    /// </summary>
    public class ImportMod
    {
        
        // Mirrored flat across backers
        //public string Name = null;
        
        List<SubDivision> _SubDivisions = null;
        List<EdgeFlag> _EdgeFlags = null;
        
        // Backer: xEdit Import File
        //const string BuildVolumeRef_File_Suffix = "_BuildVolumes.txt";
        //const string EdgeFlagRef_File_Suffix = "_EdgeFlags.txt";
        
        //string _fullPath = null;
        //string _BuildVolumeRef_File = null;
        //string _EdgeFlagRef_File = null;
        
        // Backer: Bethesda Plugin File
        //Handle _handle = Handle.Invalid;
        Engine.Plugin.File _Plugin = null;
        
        #region Constructor
        
        public ImportMod( string newName, string fullPath )
        {
            // Validate paths and files
            if( string.IsNullOrEmpty( newName ) )
                throw new System.ArgumentException( "newName cannot be null or empty!", fullPath );
            
            if( !Utils.TryAssignPath( ref _fullPath, fullPath ) )
                throw new System.ArgumentException( "Invalid path for import mod!", newName );
            // These files are optional but certain functions cannot be performed without them
            Utils.TryAssignFile( ref _BuildVolumeRef_File, _fullPath + newName + BuildVolumeRef_File_Suffix );
            Utils.TryAssignFile( ref _EdgeFlagRef_File , _fullPath + newName + EdgeFlagRef_File_Suffix  );
            
            Name = newName;
        }
       
        public ImportMod( Engine.Plugin.File plugin )
        {
            _Plugin = plugin;
        }
        
        #endregion
        
        public string Name
        {
            get
            {
                return _Plugin.Filename;
            }
        }
        
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
            
            if( ( _dataBacker != BackerType.xEditImportFile )||( string.IsNullOrEmpty( _BuildVolumeRef_File ) )||( !File.Exists( _BuildVolumeRef_File ) ) )
                return;
            
            SubDivision subdivision = null;
            
            bool parentOpen = false;
            bool parentCounted = false;
            bool parentIndexing = false;
            int volumeCount = -1;
            int volumeIndex = -1;
            string[] lineWords;
            string[] elementWords;
            
            _welded = false;
            SubDivisions = new List<SubDivision>();
            
            var volumeFile = File.ReadLines( _BuildVolumeRef_File );
            foreach( string fileLine in volumeFile )
            {
                lineWords = fileLine.ParseImportLine();
                if( !lineWords.NullOrEmpty() )
                {
                    if( !parentOpen )
                    {
                        if( ( lineWords[ 0 ] != cBuildVolumes )||( lineWords.Length != 5 ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        uint subDivisionFormID;
                        if( !uint.TryParse( lineWords[ 1 ], System.Globalization.NumberStyles.HexNumber, null, out subDivisionFormID ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        uint worldspaceFormID;
                        if( !uint.TryParse( lineWords[ 4 ], System.Globalization.NumberStyles.HexNumber, null, out worldspaceFormID ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        subdivision = new SubDivision(
                            subDivisionFormID,
                            lineWords[ 2 ],
                            Maths.Vector3f.Zero,
                            worldspaceFormID
                        );
                        SubDivisions.Add( subdivision );
                        // Volume count can now be processed
                        parentOpen = true;
                        parentCounted = false;
                        parentIndexing = false;
                        volumeCount = -1;
                        volumeIndex = -1;
                    }
                    else if( subdivision == null )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cBuildVolumes ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return;
                    }
                    else if( !parentCounted )
                    {
                        if( lineWords.Length != 3 )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Volume count
                        elementWords = lineWords[ 0 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cCount ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !int.TryParse( elementWords[ 1 ], out volumeCount ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // SubDivision position
                        elementWords = lineWords[ 1 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cPosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out subdivision.Position ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // SubDivision FormID (again)
                        //elementWords = lineWords[ 2 ].ParseImportLine( ':' );
                        //if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cReference ) )
                        //{
                        //    System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cReference ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        //    return;
                        //}
                        //subdivision.Reference = elementWords[ 1 ];
                        // Individual volumes can now be processed
                        parentCounted = true;
                        parentIndexing = true;
                        volumeIndex = 0;
                    }
                    else if( !parentIndexing )
                    {
                        System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCount ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                        return;
                    }
                    else
                    {
                        if( lineWords.Length != 6 )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        // Volume index
                        elementWords = lineWords[ 0 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cIndex ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        int currentIndex;
                        if( !int.TryParse( elementWords[ 1 ], out currentIndex ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( currentIndex != volumeIndex )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cIndex ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        Maths.Vector3f volumePosition;
                        Maths.Vector3f volumeSize;
                        Maths.Vector3f volumeRotation;
                        Maths.Vector2i volumeCell;
                        uint volumeFormID;
                        
                        // Volume position
                        elementWords = lineWords[ 1 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cPosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumePosition ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cPosition ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume Rotation
                        elementWords = lineWords[ 2 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cRotation ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cRotation ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumeRotation ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cRotation ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume size
                        elementWords = lineWords[ 3 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cSize ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cSize ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector3f.TryParse( elementWords[ 1 ], out volumeSize ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cSize ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume cell
                        elementWords = lineWords[ 4 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cCell ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCell ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !Maths.Vector2i.TryParse( elementWords[ 1 ], out volumeCell ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cCell ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        // Volume reference
                        elementWords = lineWords[ 5 ].ParseImportLine( ':' );
                        if( ( elementWords.Length != 2 )||( elementWords[ 0 ] != cReference ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cReference ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        if( !uint.TryParse( elementWords[ 1 ], System.Globalization.NumberStyles.HexNumber, null, out volumeFormID ) )
                        {
                            System.Windows.Forms.MessageBox.Show( string.Format( "LoadBuildVolumesFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _BuildVolumeRef_File, fileLine, cReference ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                            return;
                        }
                        
                        var volume = new BuildVolume(
                            volumeFormID,
                            volumePosition,
                            volumeRotation,
                            volumeSize,
                            volumeCell
                        );
                        
                        subdivision.BuildVolumes.Add( volume );
                        
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
            
            SubDivisions.Sort();
        }
        
        /// <summary>
        /// Parse #Name#_Helper_Flags.txt as exported from xEdit -> Apply Script -> ESM - Export Build Volumes
        /// </summary>
        public void LoadEdgeFlagsFile()
        {
            const string cEdgeFlag = "EdgeFlag";
            const string cPosition = "Position";
            const string cFormID = "FormID";
            const string cWorldspaceFormID = "WorldspaceFormID";
            
            if( ( _dataBacker != BackerType.xEditImportFile )||( string.IsNullOrEmpty( _EdgeFlagRef_File ) )||( !File.Exists( _EdgeFlagRef_File ) ) )
                return;
            
            int flagIndex = -1; // Unused
            
            string[] lineWords;
            string[] elementWords;
            
            EdgeFlag newFlag;
            
            EdgeFlags = new List<EdgeFlag>();
            
            int validElementsCount;
            var flagFile = File.ReadLines( _EdgeFlagRef_File );
            foreach( string fileLine in flagFile )
            {
                lineWords = fileLine.ParseImportLine();
                if( ( !lineWords.NullOrEmpty() )&&( lineWords.Length == 4 ) )
                {
                    validElementsCount = 0;
                    newFlag = new EdgeFlag();
                    newFlag.position = Maths.Vector2f.Zero;
                    newFlag.formID = 0;
                    newFlag.worldspaceFormID = 0;
                    
                    foreach( string element in lineWords )
                    {
                        elementWords = element.ParseImportLine( ':' );
                        switch( elementWords[ 0 ] )
                        {
                            case cEdgeFlag :
                                flagIndex = int.Parse( elementWords[ 1 ] );
                                validElementsCount += 1;
                                break;
                                
                            case cPosition :
                                Maths.Vector2f.TryParse( elementWords[ 1 ], out newFlag.position );
                                validElementsCount += 1;
                                break;
                                
                            case cFormID :
                                if( !uint.TryParse( elementWords[ 1 ], System.Globalization.NumberStyles.HexNumber, null, out newFlag.formID ) )
                                {
                                    System.Windows.Forms.MessageBox.Show( string.Format( "LoadHelperFlagsFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _EdgeFlagRef_File, fileLine, cEdgeFlag ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                                    return;
                                }
                                validElementsCount += 1;
                                break;
                                
                            case cWorldspaceFormID :
                                if( !uint.TryParse( elementWords[ 1 ], System.Globalization.NumberStyles.HexNumber, null, out newFlag.worldspaceFormID ) )
                                {
                                    System.Windows.Forms.MessageBox.Show( string.Format( "LoadHelperFlagsFile()\nBad file format!\nExpected '{2}'\n'{1}'\nUnable to import from \"{0}\"!", _EdgeFlagRef_File, fileLine, cEdgeFlag ), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
                                    return;
                                }
                                validElementsCount += 1;
                                break;
                        }
                    }
                    if( validElementsCount == 4 )
                        EdgeFlags.Add( newFlag );
                }
            }
            //System.Windows.Forms.MessageBox.Show( string.Format( "LoadHelperFlagsFile()\nFound '{0}' valid helper flags.", HelperFlags.Count ), "Info", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information );
        }
        
        #endregion
        
        #region Data enumeration (TODO: Rename to something more appropriate)
        
        public List<SubDivision> SubDivisions
        {
            get
            {
                if( _SubDivisions == null )
                {
                }
                return _SubDivisions;
            }
        }
        
        public List<EdgeFlag> EdgeFlags
        {
            get
            {
                return _EdgeFlags;
            }
        }
        
        public SubDivision SubDivisionByEditorID( string editorID )
        {
            if( ( !SubDivisions.NullOrEmpty() )&&( !string.IsNullOrEmpty( editorID ) ) )
                foreach( var subdivision in SubDivisions )
                    if( subdivision.EditorID == editorID )
                        return subdivision;
            
            return null;
        }
        
        public SubDivision SubDivisionByFormID( uint formID )
        {
            if( ( !SubDivisions.NullOrEmpty() )&&( formID != Engine.Plugin.Constant.FormID_Invalid ) )
                foreach( var subdivision in SubDivisions )
                    if( subdivision.FormID == formID )
                        return subdivision;
            
            return null;
        }
        
        public List<EdgeFlag> EdgeFlagsInWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
        {
            if(
                ( worldspace == null )||
                ( EdgeFlags.NullOrEmpty() )
            )
                return null;
            
            var rFlags = new List<EdgeFlag>();
            foreach( var flag in EdgeFlags )
                if( flag.Reference.Worldspace.FormID == worldspace.FormID )
                    rFlags.Add( flag );
            //return rFlags.NullOrEmpty() ? null :  rFlags;
            return rFlags;
        }
        
        #endregion
        
        #region Corner Welding
        
        bool _welded = false;
        
        public bool Welded
        {
            get
            {
                return _welded;
            }
        }
        
        public void WeldVerticies( float threshold, bool weldToOtherParents, bool forceSquare )
        {
            if( _welded )
                return;
            
            var fmain = GodObject.Windows.GetMainWindow();
            fmain.PushStatusMessage();
            
            for( int index = 0; index < SubDivisions.Count; index++ )
            {
                var parent = _SubDivisions[ index ];
                var pvolumes = parent.BuildVolumes;
                if( pvolumes.NullOrEmpty() )
                    continue;
                
                fmain.SetCurrentStatusMessage( string.Format( "Welding verticies for {0}...", parent.FormID ) );
                
                for( int index2 = 0; index2 < pvolumes.Count; index2++ )
                {
                    var volume = pvolumes[ index2 ];
                    WeldPoint.WeldVolumeVerticies( SubDivisions, parent, volume, threshold, weldToOtherParents, forceSquare );
                }
            }
            
            _welded = true;
            fmain.PopStatusMessage();
        }
        
        #endregion
        
        public bool TryGetVolumesFromPos( Maths.Vector2f pos, out List<SubDivision> outParents, out List<Volume> outVolumes, List<SubDivision> fromGroup = null )
        {
            outParents = null;
            outVolumes = null;
            
            //DebugLog.Write( "\nbbImportMod.TryGetVolumesFromPos()" );
            
            fromGroup = fromGroup ?? _SubDivisions;
            if( fromGroup.NullOrEmpty() ) return false;
            
            var parents = new List<SubDivision>();
            var volumes = new List<Volume>();
            
            foreach( var parent in fromGroup )
            {
                var pvolumes = parent.BuildVolumes;
                if( pvolumes.NullOrEmpty() )
                    continue;
                
                foreach( var volume in pvolumes )
                {
                    if( Maths.Geometry.Collision.PointInPoly( pos, volume.Corners ) )
                    {
                        if( !parents.Contains( parent ) )
                            parents.Add( parent );
                        volumes.Add( volume );
                    }
                }
            }
            if( volumes.NullOrEmpty() )
                return false;
            
            outParents = parents;
            outVolumes = volumes;
            return true;
        }
        
    }
}
*/
