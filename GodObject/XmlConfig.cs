/*
 * XmlConfig.cs
 *
 * Configuration information for GUIBuilder windows.
 *
 */
using System;
using System.Linq;
using System.Xml;


namespace GodObject
{
    
    public static class XmlConfig
    {
        
        const int                           CurrentConfigVersion                    = 1;
        const string                        XmlKey_ConfigVersion                    = "ConfigVersion";
        
        // XmlNodes for the program wide options, not the options window
        public const string                 XmlNode_Options                         = "Options";
        public const string                 XmlNode_AlwaysSelectMasters             = "AlwaysSelectMasters";

        // XmlKeys for debug log options
        public const string                 XmlKey_MirrorToConsole                  = "MirrorToConsole";
        public const string                 XmlKey_ZipLogs                          = "ZipLogs";

        // XmlKeys for System.Windows.Forms.Control
        public const string                 XmlKey_Location                         = "Location";
        public const string                 XmlKey_Size                             = "Size";

        // XmlNode and XmlKey for NIF ExportInfo
        public const string                 XmlNode_NIF_ExportInfo                  = "NIFExportInfo";
        public const string                 XmlKey_NIF_ExportInfo                   = "Line_{0}";

        public interface IXmlConfiguration
        {
            
            [System.ComponentModel.Browsable( false )]
            IXmlConfiguration               XmlParent                               { get; }
            
            [System.ComponentModel.Browsable( false )]
            string                          XmlNodeName                             { get; }
        }
        
        #region Internal
        
        class ConfigInterface : XmlBase
        {
            
            public override bool            XmlForceCreateFile                      { get{ return true; } }
            
            public override bool            XmlFileMustExist                        { get{ return false; } }
            
            public override string          RootNodeName                            { get{ return "GUIBuilder"; } }
            
            public override string          Pathname                                { get{ return GodObject.Paths.GUIBuilderConfigFile; } }
            
            public override void            OnLoad()
            {
                // Reset the config info if the config version is less than 1
                var configVer = ReadValue<int>( XmlKey_ConfigVersion, 0 );
                if( configVer < 1 )
                    Reset( true, true );
            }
            
            public override void            OnInit()
            {
                WriteValue<int>( XmlKey_ConfigVersion, CurrentConfigVersion );
            }
        }
        
        static ConfigInterface              _ConfigInterface                        = new ConfigInterface();
        
        #endregion
        
        public static bool WasReset { get { return _ConfigInterface.WasReset; } }
        
        #region Nodes
        
        public static XmlNodeList GetNodes( string[] nodes )
        {
            return _ConfigInterface.GetNodes( XmlPathTo( nodes ) );
        }
        
        public static XmlNodeList GetNodes( string[] nodes, string node )
        {
            return _ConfigInterface.GetNodes( XmlPathTo( nodes, node ) );
        }
        
        public static XmlNodeList GetNodes( string xpath )
        {
            return _ConfigInterface.GetNodes( xpath );
        }
        
        public static XmlNode GetNode( string[] nodes )
        {
            return _ConfigInterface.GetNode( XmlPathTo( nodes ) );
        }
        
        public static XmlNode GetNode( string[] nodes, string node )
        {
            return _ConfigInterface.GetNode( XmlPathTo( nodes, node ) );
        }
        
        public static XmlNode GetNode( string xpath )
        {
            return _ConfigInterface.GetNode( xpath );
        }
        
        public static XmlNode MakeXPath( string[] nodes )
        {
            return _ConfigInterface.MakeXPath( XmlPathTo( nodes ) );
        }
        
        public static XmlNode MakeXPath( string[] nodes, string node )
        {
            return _ConfigInterface.MakeXPath( XmlPathTo( nodes, node ) );
        }
        
        public static XmlNode MakeXPath( string xpath )
        {
            return _ConfigInterface.MakeXPath( xpath );
        }
        
        public static XmlNode AppendNode( XmlNode pnode, string xpath )
        {
            return _ConfigInterface.AppendNode( pnode, xpath );
        }
        
        #endregion
        
        public static bool                  Commit()
        {
            return _ConfigInterface.Commit();
        }

        public static void                  RemoveNode( string xpath )
        {
            _ConfigInterface.RemoveNode( null, xpath );
        }

        #region Values

        public static string                ReadNode( IXmlConfiguration config, string key )
        {
            return _ConfigInterface.ReadNode( XmlPathTo( config ), key );
        }
        
        public static string                ReadNode( XmlNode pnode, string xpath, string key )
        {
            return _ConfigInterface.ReadNode( pnode, xpath, key );
        }
        
        public static bool                  WriteNode( XmlNode pnode, string xpath, string key, string value, bool commit = false )
        {
            return _ConfigInterface.WriteNode( pnode, xpath, key, value, commit );
        }
        
        public static T                     ReadValue<T>( XmlNode pnode, string xpath, string key, T defaultValue = default(T) )
        {
            return _ConfigInterface.ReadValue<T>( pnode, xpath, key, defaultValue );
        }
        public static T                     ReadValue<T>( XmlNode pnode, string key, T defaultValue = default(T) )
        {
            return ReadValue<T>( pnode, null, key, defaultValue );
        }
        public static T                     ReadValue<T>( string xpath, string key, T defaultValue = default(T) )
        {
            return ReadValue<T>( null, xpath, key, defaultValue );
        }
        public static T                     ReadValue<T>( string key, T defaultValue = default(T) )
        {
            return ReadValue<T>( null, null, key, defaultValue );
        }
        public static T                     ReadValue<T>( IXmlConfiguration config, string key, T defaultValue = default(T) )
        {
            return ReadValue<T>( null, XmlPathTo( config ), key, defaultValue );
        }
        
        public static bool                  WriteValue<T>( XmlNode pnode, string xpath, string key, T value, bool commit = false )
        {
            return _ConfigInterface.WriteValue<T>( pnode, xpath, key, value, commit );
        }
        public static bool                  WriteValue<T>( XmlNode pnode, string key, T value, bool commit = false )
        {
            return WriteValue<T>( pnode, null, key, value, commit );
        }
        public static bool                  WriteValue<T>( string xpath, string key, T value, bool commit = false )
        {
            return WriteValue<T>( null, xpath, key, value, commit );
        }
        public static bool                  WriteValue<T>( string key, T value, bool commit = false )
        {
            return WriteValue<T>( null, null, key, value, commit );
        }
        public static bool                  WriteValue<T>( IXmlConfiguration config, string key, T value, bool commit = false )
        {
            return WriteValue<T>( null, XmlPathTo( config ), key, value, commit );
        }
        
        public static System.Drawing.Point  ReadLocation<T>( T config ) where T : System.Windows.Forms.Control, IXmlConfiguration
        {
            return ReadValue<System.Drawing.Point>( null, XmlPathTo( config ), XmlKey_Location, config.Location );
        }
        public static System.Drawing.Size   ReadSize<T>( T config ) where T : System.Windows.Forms.Control, IXmlConfiguration
        {
            return ReadValue<System.Drawing.Size>( null, XmlPathTo( config ), XmlKey_Size, config.Size );
        }
        
        public static bool                  WriteLocation<T>( T config, bool commit = true ) where T : System.Windows.Forms.Control, IXmlConfiguration
        {
            return WriteValue<System.Drawing.Point>( null, XmlPathTo( config ), XmlKey_Location, config.Location, commit );
        }
        public static bool                  WriteSize<T>( T config, bool commit = true ) where T : System.Windows.Forms.Control, IXmlConfiguration
        {
            return WriteValue<System.Drawing.Size>( null, XmlPathTo( config ), XmlKey_Size, config.Size, commit );
        }
        
        #endregion
        
        #region Paths
        
        static string                       XmlPathTo( string[] nodes )
        {
            if( nodes.NullOrEmpty() ) return null;
            var xpath = nodes[ 0 ];
            for( int i = 1; i < nodes.Length; i++ )
                xpath += "/" + nodes[ i ];
            return xpath;
        }
        
        static string                       XmlPathTo( string[] nodes, string node )
        {
            if( nodes.NullOrEmpty() ) return null;
            var xpath = nodes[ 0 ];
            for( int i = 1; i < nodes.Length; i++ )
                xpath += "/" + nodes[ i ];
            if( !string.IsNullOrEmpty( node ) )
                xpath += "/" + node;
            return xpath;
        }
        
        static string                       XmlPathTo( IXmlConfiguration config )
        {
            if( config == null ) return null;
            var xpath = config.XmlNodeName;
            if( string.IsNullOrEmpty( xpath ) ) return null;
            var pxpath = XmlPathTo( config.XmlParent );
            return string.IsNullOrEmpty( pxpath )
                ? xpath
                : string.Format( "{0}/{1}", pxpath, xpath );
        }
        
        #endregion
        
        public static IXmlConfiguration     GetXmlParent( System.Windows.Forms.Control control )
        {
            var p = control.Parent;
            while( p != null )
            {
                var x = p as GodObject.XmlConfig.IXmlConfiguration;
                if( x != null )
                    return x;
                p = p.Parent;
            }
            return null;
        }
        
    }
}
