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
        
        const string                    XmlKey_Filename                         = "Filename";
        const string                    XmlKey_FormID                           = "FormID";
        const string                    XmlKey_WorkingFile                      = "WorkingFile";
        const string                    XmlKey_OpenRenderWindowOnLoad           = "OpenRenderWindowOnLoad";
        
        public const string             XmlNode_WorkshopBorder                  = "WorkshopBorder";
        
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
        
        public bool                     SetFormIdentifier( string xmlNode, string filename, uint formID, bool commit = false )
        {
            if(
                ( string.IsNullOrEmpty( xmlNode ) )||
                ( string.IsNullOrEmpty( filename ) )||
                ( !Engine.Plugin.Constant.ValidFormID( formID ) )
               )   return false;
            
            var node = GetNode( xmlNode ) ?? AppendNode( xmlNode );
            if( node == null )
                return false;
            
            var maskedFormID = formID & 0x00FFFFFF;
            
            WriteValue<string>( node, XmlKey_Filename, filename, false );
            WriteValue<string>( node, XmlKey_FormID, maskedFormID.ToString( "X8" ), false );
            
            return !commit || Commit();
        }
        
        public FormIdentifier           GetFormIdentifier( string xmlNode, bool fileMustBeLoaded )
        {
            if( string.IsNullOrEmpty( xmlNode ) )
                return null;

            var node = GetNode( xmlNode );
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
            var fi = GetFormIdentifier( xmlNode, true );
            if( fi!= null )
            {
                var file = GodObject.Plugin.Data.Files.Find( fi.Filename );
                if( file != null )
                {
                    var LO = file.LoadOrder << 24;
                    var formID = LO | fi.FormID;
                    return GodObject.Plugin.Data.Root.Find<TForm>( formID, true );
                }
            }
            return null;
        }

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