/*
 * Workspace.cs
 *
 * This class encompasses mod specific settings forms, etc, for GUIBuilder
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace GUIBuilder
{
    /// <summary>
    /// Description of Workspace.
    /// </summary>
    public class Workspace : XmlBase
    {
        
        const string                    XmlRoot                                 = "Workspace";
        
        const string                    XmlNode_Plugins                         = "Plugins";
        const string                    XmlNode_WorkshopWorkbenches             = "WorkshopWorkbenches";
        public const string             XmlNode_WorkshopBorder                  = "WorkshopBorder";
        const string                    XmlNode_WorkshopPreset                  = "WorkshopBorderPreset";
        const string                    XmlNode_SubDivisionPreset               = "SubDivisionBorderPreset";

        const string                    XmlKey_Container                        = "Container";
        const string                    XmlKey_Filename                         = "Filename";
        const string                    XmlKey_FormID                           = "FormID";
        const string                    XmlKey_WorkingFile                      = "WorkingFile";
        const string                    XmlKey_OpenRenderWindowOnLoad           = "OpenRenderWindowOnLoad";

        const string                    XmlKey_Preset_NodeLength                = "NodeLength";
        const string                    XmlKey_Preset_HighPrecisionFloats       = "HighPrecisionFloats";
        const string                    XmlKey_Preset_AngleAllowance            = "AngleAllowance";
        const string                    XmlKey_Preset_SlopeAllowance            = "SlopeAllowance";
        const string                    XmlKey_Preset_GradientHeight            = "GradientHeight";
        const string                    XmlKey_Preset_GroundOffset              = "GroundOffset";
        const string                    XmlKey_Preset_GroundSink                = "GroundSink";
        const string                    XmlKey_Preset_TargetSuffix              = "TargetSuffix";
        const string                    XmlKey_Preset_MeshSubDirectory          = "MeshSubDirectory";
        const string                    XmlKey_Preset_FilePrefix                = "FilePrefix";
        const string                    XmlKey_Preset_FileSuffix                = "FileSuffix";
        const string                    XmlKey_Preset_CreateImportData          = "CreateImportData";

        #region Internal
        
        public override bool            XmlForceCreateFile                      { get{ return true; } }
        
        public override bool            XmlFileMustExist                        { get{ return true; } }
        
        public override string          RootNodeName                            { get{ return XmlRoot; } }
        
        public override string          Pathname
        {
            get
            {
                var wsPath = GodObject.Paths.Workspace;
                return string.IsNullOrEmpty( wsPath ) || string.IsNullOrEmpty( _Name )
                    ? null
                    : string.Format( "{0}{1}.xml", wsPath, _Name );
            }
        }
        
        // Name = filename - extension
        readonly string                 _Name                                   = null;
        public string                   Name                                    { get{ return _Name; } }
        
        public                          Workspace( string name )
        {
            var working = (string)name.Clone();
            var dot = working.LastIndexOf( '.' );
            if( dot > 0 )
                working = working.Substring( 0, dot );
            _Name = working;
        }

        #endregion

        #region Form Identifiers

        public class                    FormIdentifier
        {
            public string               Filename;
            public uint                 FormID;
            
            public FormIdentifier(  string filename = null, uint formID = Engine.Plugin.Constant.FormID_None )
            {
                Filename = filename;
                FormID   = formID;
            }
            
        }

        public bool                     SetFormIdentifier( string xmlNode, string filename, uint formID, bool commit = false )
        {
            if(
                ( string.IsNullOrEmpty( xmlNode ) ) ||
                ( string.IsNullOrEmpty( filename ) ) ||
                ( !Engine.Plugin.Constant.ValidFormID( formID ) )
               ) return false;

            var node = GetNode( xmlNode ) ?? AppendNode( xmlNode );
            return SetFormIdentifier( node, filename, formID, commit );
        }
        
        public bool                     SetFormIdentifier( XmlNode node, string filename, uint formID, bool commit = false )
        {
            if(
                ( node == null )||
                ( string.IsNullOrEmpty( filename ) ) ||
                ( !Engine.Plugin.Constant.ValidFormID( formID ) )
               ) return false;

            var maskedFormID = formID & 0x00FFFFFF;

            WriteValue<string>( node, XmlKey_Filename, filename, false );
            WriteValue<string>( node, XmlKey_FormID, maskedFormID.ToString( "X8" ), false );

            return !commit || Commit();
        }

        public FormIdentifier           GetFormIdentifier( string xmlNode, bool fileMustBeLoaded )
        {
            return string.IsNullOrEmpty( xmlNode )
                ? null
                : GetFormIdentifier( GetNode( xmlNode ), fileMustBeLoaded );
        }

        public FormIdentifier           GetFormIdentifier( XmlNode node, bool fileMustBeLoaded )
        {
            if( node == null )
                return null;

            var sFormID = ReadValue<string>( node, XmlKey_FormID, null );
            if( string.IsNullOrEmpty( sFormID ) )
                return null;

            uint formID = 0;
            if( !UInt32.TryParse( sFormID, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out formID ) )
                return null;

            var filename = ReadValue<string>( node, XmlKey_Filename, null );
            if( string.IsNullOrEmpty( filename ) )
                return null;

            if( fileMustBeLoaded && !GodObject.Plugin.Data.Files.IsLoaded( filename ) )
                return null;

            var result = new FormIdentifier( filename, formID );
            return result;
        }

        public TForm                    GetIdentifierForm<TForm>( string xmlNode ) where TForm : Engine.Plugin.Form
        {
            return GetIdentifierForm( typeof( TForm ), xmlNode ) as TForm;
        }

        public Engine.Plugin.Form       GetIdentifierForm( Type formType, string xmlNode )
        {
            var fi = GetFormIdentifier( xmlNode, true );
            if( fi != null )
            {
                var file = GodObject.Plugin.Data.Files.Find( fi.Filename );
                if( file != null )
                {
                    var LO = file.LoadOrder << 24;
                    var formID = LO | fi.FormID;
                    return GodObject.Plugin.Data.Root.Find( formType, formID, true ) as Engine.Plugin.Form;
                }
            }
            return null;
        }

        #endregion

        public List<string>             PluginNames
        {
            get
            {
                var pluginFilenames = GetNodes( string.Format( "{0}/{1}", XmlNode_Plugins, XmlKey_Filename ) );
                if( pluginFilenames == null )
                    return null;
                var plugins = new List<string>();
                foreach( XmlNode pluginFilename in pluginFilenames )
                {
                    var name = pluginFilename.InnerText;
                    //Console.WriteLine( string.Format( "Plugins/Filename[] = \"{0}\"", name ) );
                    if( !string.IsNullOrEmpty( name ) )
                        plugins.Add( name );
                }
                return plugins;
            }
            set
            {
                if( value.NullOrEmpty() ) return;
                var pluginNode = GetNode( XmlNode_Plugins ) ?? MakeXPath( XmlNode_Plugins );
                if( pluginNode == null ) return;
                foreach( var pluginName in value )
                {
                    var node = FindChildNode( pluginNode, XmlKey_Filename, pluginName ) ?? AppendNode( pluginNode, XmlKey_Filename );
                    if( node != null )
                        node.InnerText = pluginName;
                }
                Commit();
            }
        }
        
        public string                   WorkingFile
        {
            get
            {
                return ReadValue<string>( XmlKey_WorkingFile );
            }
            set
            {
                WriteValue( XmlKey_WorkingFile, value );
            }
        }
        
        public bool                     OpenRenderWindowOnLoad
        {
            get
            {
                return ReadValue<bool>( XmlKey_OpenRenderWindowOnLoad, false );
            }
            set
            {
                WriteValue<bool>( XmlKey_OpenRenderWindowOnLoad, value );
            }
        }

        #region Custom Workshop Container Forms

        public List<FormIdentifier>     WorkshopWorkbenches
        {
            get
            {
                var xpath = string.Format( "{0}/{1}", XmlNode_WorkshopWorkbenches, XmlKey_Container );
                var workshopNodes = GetNodes( xpath );
                if( workshopNodes == null ) return null;
                
                var identifiers = new List<FormIdentifier>();
                foreach( XmlNode workshopNode in workshopNodes )
                {
                    var identifier = GetFormIdentifier( workshopNode, false );
                    if( identifier != null )
                        identifiers.Add( identifier );
                }
                return identifiers;
            }
            set
            {
                if( value.NullOrEmpty() ) return;

                // Delete any existing list of workshops
                RemoveNode( XmlNode_WorkshopWorkbenches );

                // Create a new list
                var workshopNodes = AppendNode( XmlNode_WorkshopWorkbenches );
                if( workshopNodes == null ) return;

                foreach( var identifier in value )
                {
                    var node = AppendNode( workshopNodes, XmlKey_Container );
                    SetFormIdentifier( node, identifier.Filename, identifier.FormID, false );
                }
                Commit();
            }
        }

        public void                     SetWorkshopWorkbenches( List<Engine.Plugin.Forms.Container> containers )
        {
            // Delete any existing list of workshops
            RemoveNode( XmlNode_WorkshopWorkbenches );

            // Write new list to the workspace
            if( !containers.NullOrEmpty() )
            {
                // Create a new list
                var workshopNodes = AppendNode( XmlNode_WorkshopWorkbenches );
                if( workshopNodes == null ) return;

                foreach( var container in containers )
                {
                    var node = AppendNode( workshopNodes, XmlKey_Container );
                    SetFormIdentifier( node, container.MasterHandle.Filename, container.GetFormID( Engine.Plugin.TargetHandle.Master ) & 0x00FFFFFF, false );
                }
            }

            // Finally, commit the changes
            Commit();
        }

        #endregion

        #region Custom Border Presets

        #region Preset Serialization

        void                            SerializeBorderPreset( ref NIFBuilder.Preset holder, NIFBuilder.Preset preset, string xpath, bool commit = true )
        {
            DebugLog.WriteStrings( null, new string[] { "holder = " + holder.ToStringNullSafe(), "preset = " + preset.ToStringNullSafe(), "xpath = " + xpath }, false, true, false, false );
            holder = preset;
            if( preset == null )
                return;
            var presetNode = GetNode( xpath ) ?? AppendNode( xpath );
            if( presetNode != null )
            {
                WriteValue<bool>(   presetNode, XmlKey_Preset_HighPrecisionFloats, preset.HighPrecisionFloats );
                WriteValue<float>(  presetNode, XmlKey_Preset_NodeLength         , preset.NodeLength );
                WriteValue<double>( presetNode, XmlKey_Preset_AngleAllowance     , preset.AngleAllowance );
                WriteValue<double>( presetNode, XmlKey_Preset_SlopeAllowance     , preset.SlopeAllowance );
                WriteValue<float>(  presetNode, XmlKey_Preset_GradientHeight     , preset.GradientHeight );
                WriteValue<float>(  presetNode, XmlKey_Preset_GroundOffset       , preset.GroundOffset );
                WriteValue<float>(  presetNode, XmlKey_Preset_GroundSink         , preset.GroundSink );
                WriteValue<string>( presetNode, XmlKey_Preset_MeshSubDirectory   , preset.MeshSubDirectory );
                WriteValue<string>( presetNode, XmlKey_Preset_FilePrefix         , preset.FilePrefix );
                WriteValue<string>( presetNode, XmlKey_Preset_TargetSuffix       , preset.TargetSuffix );
                WriteValue<string>( presetNode, XmlKey_Preset_FileSuffix         , preset.FileSuffix );
                WriteValue<bool>(   presetNode, XmlKey_Preset_CreateImportData   , preset.CreateImportData );
                if( commit ) Commit();
            }
        }

        bool                            HasBorderPreset( string xpath )
        {
            return GetNode( xpath ) != null;
        }

        NIFBuilder.Preset               GetBorderPreset( string xpath, NIFBuilder.Preset template, NIFBuilder.Preset.Serializer onSerialize )
        {
            //Console.WriteLine( "GUIBuilder.Workspace.GetBorderPreset() :: xpath = " + xpath );
            NIFBuilder.Preset preset = null;
            var presetNode = GetNode( xpath );
            if( presetNode != null )
            {
                preset = new NIFBuilder.Preset( "Custom",
                    ReadValue<bool>(   presetNode, XmlKey_Preset_HighPrecisionFloats, template.HighPrecisionFloats ),
                    ReadValue<float>(  presetNode, XmlKey_Preset_NodeLength         , template.NodeLength ),
                    ReadValue<double>( presetNode, XmlKey_Preset_AngleAllowance     , template.AngleAllowance ),
                    ReadValue<double>( presetNode, XmlKey_Preset_SlopeAllowance     , template.SlopeAllowance ),
                    ReadValue<float>(  presetNode, XmlKey_Preset_GradientHeight     , template.GradientHeight ),
                    ReadValue<float>(  presetNode, XmlKey_Preset_GroundOffset       , template.GroundOffset ),
                    ReadValue<float>(  presetNode, XmlKey_Preset_GroundSink         , template.GroundSink ),
                    ReadValue<string>( presetNode, XmlKey_Preset_MeshSubDirectory   , template.MeshSubDirectory ),
                    ReadValue<string>( presetNode, XmlKey_Preset_FilePrefix         , template.FilePrefix ),
                    ReadValue<string>( presetNode, XmlKey_Preset_TargetSuffix       , template.TargetSuffix ),
                    ReadValue<string>( presetNode, XmlKey_Preset_FileSuffix         , template.FileSuffix ),
                    ReadValue<bool>(   presetNode, XmlKey_Preset_CreateImportData   , template.CreateImportData ) );
            }
            else
                preset = new NIFBuilder.Preset( template );
            if( ( preset != null )&&( onSerialize != null ) )
                preset.onSerialize += onSerialize;
            return preset;
        }

        #endregion

        #region Workshop Border Custom Preset

        private NIFBuilder.Preset       _WorkshopPreset = null;
        public NIFBuilder.Preset        WorkshopPreset
        {
            get
            {
                //Console.WriteLine( "GUIBuilder.Workspace.WorkshopPreset" );
                if( _WorkshopPreset == null )
                    _WorkshopPreset = GetBorderPreset( XmlNode_WorkshopPreset, NIFBuilder.Preset.Workshop.Custom, SerializeWorkshopPreset );
                return _WorkshopPreset;
            }
        }

        public bool                     HasWorkshopPreset       { get { return HasBorderPreset( XmlNode_WorkshopPreset ); } }

        public void                     SerializeWorkshopPreset( NIFBuilder.Preset preset )
        {
            SerializeBorderPreset( ref _WorkshopPreset, preset, XmlNode_WorkshopPreset );
        }

        #endregion

        #region Sub-Division Border Custom Preset

        private NIFBuilder.Preset       _SubDivisionPreset = null;
        public NIFBuilder.Preset        SubDivisionPreset
        {
            get
            {
                //Console.WriteLine( "GUIBuilder.Workspace.SubDivisionPreset" );
                if( _SubDivisionPreset == null )
                    _SubDivisionPreset = GetBorderPreset( XmlNode_SubDivisionPreset, NIFBuilder.Preset.SubDivision.Custom, SerializeSubDivisionPreset );
                return _SubDivisionPreset;
            }
        }

        public bool                     HasSubDivisonPreset     { get { return HasBorderPreset( XmlNode_SubDivisionPreset ); } }

        public void                     SerializeSubDivisionPreset( NIFBuilder.Preset preset )
        {
            SerializeBorderPreset( ref _SubDivisionPreset, preset, XmlNode_SubDivisionPreset );
        }

        #endregion

        #endregion

        /*
        public string                   WorkshopKeywordBorderGenerator          = null;
        public string                   WorkshopKeywordBorderLink               = null;
        
        public string                   WorkshopBorderMarkerTerrainFollowing    = null;
        public string                   WorkshopBorderMarkerForcedZ             = null;
        
        public List<FormIdentifier>     Workshops                               = null;
        public List<FormIdentifier>     SubDivisions                            = null;
        
        public string                   WorkshopBorderPresetName                = null;
        public NIFBuilder.Preset        WorkshopBorderCustom                    = null;
        
        public string                   SubDivisionBorderPresetName             = null;
        public NIFBuilder.Preset        SubDivisionBorderCustom                 = null;
        */

    }
}