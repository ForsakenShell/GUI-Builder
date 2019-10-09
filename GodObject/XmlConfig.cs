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
        
        public interface IXmlConfiguration
        {
            IXmlConfiguration   XmlParent                   { get; }
            string              XmlKey                      { get; }
            
            /* Since C# isn't capable of multiple class inheritance, this is a handy
             * copy-pasta help function for classes implementing this interface
            public string        XmlPath                     { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
            */
        }
        
        #region Internal
        
        public static readonly string Root = "GUIBuilder";
        
        static XmlDocument _Document;
        static XmlNode _RootNode;
        
        static XmlNode RootNode
        {
            get
            {
                if( _Document == null )
                {
                    _Document = new XmlDocument();
                    if( _Document == null )
                        return null;
                    
                    var configFile = GodObject.Paths.GUIBuilderConfigFile;
                    if( !string.IsNullOrEmpty( configFile ) )
                    {
                        _Document.Load( configFile );
                        _RootNode = _Document.SelectSingleNode( Root );
                    }
                    else
                    {
                        _RootNode = _Document.CreateElement( Root );
                        _Document.AppendChild( _RootNode );
                    }
                }
                return _RootNode;
            }
        }
        
        #endregion
        
        #region Nodes
        
        public static bool Commit()
        {
            var xdoc = _Document;
            if( xdoc == null )
                return false;
            var configFile = GodObject.Paths.BorderBuilder + GUIBuilder.Constant.GUIBuilderConfigFile;
            if( string.IsNullOrEmpty( configFile ) )
                return false;
            
            XmlWriterSettings xsettings = new XmlWriterSettings();
            if( xsettings == null )
                return false;
            xsettings.Indent = true;
            
            XmlWriter xwrite = XmlWriter.Create( configFile, xsettings );
            if( xwrite == null )
                return false;
            
            xdoc.WriteTo( xwrite );
            xwrite.Close();
            
            return true;
        }
        
        public static XmlNodeList GetNodes( string xpath )
        {
            var rnode = RootNode;
            return rnode == null ? null : rnode.SelectNodes( xpath );
            
        }
        
        public static XmlNode GetNode( string xpath )
        {
            var rnode = RootNode;
            return rnode == null ? null : rnode.SelectSingleNode( xpath );
            
        }
        
        #endregion
        
        #region Values
        
        public static string ReadNodeValue( XmlNode node, string key, string defaultValue = null )
        {
            if( node == null )
                return defaultValue;
            var knode = node.SelectSingleNode( key );
            return knode == null ? defaultValue : knode.InnerText;
        }
        
        public static bool WriteNodeValue( XmlNode node, string key, string value, bool commit = false )
        {
            if( node == null )
                return false;
            
            var knode = node.SelectSingleNode( key );
            if( knode == null )
            {
                knode = MakeXPath( node, key );
                if( knode == null )
                    return false;
            }
            /*
            DebugLog.Write(
                string.Format(
                    "\nWriteNodeValue()\n\tkey = \"{0}\"\n\tvalue = \"{1}\"\n\tcommit = {2}",
                    key, value, commit ) );
            */
            knode.InnerText = value;
            return !commit || Commit();
        }
        
        public static string ReadStringValue( string xpath, string key, string defaultValue = null )
        {
            /*
            DebugLog.Write(
                string.Format(
                    "\nReadStringValue()\n\tXPath = \"{0}\"\n\tkey = \"{1}\"\n\tdefaultValue = \"{2}\"",
                    xpath, key, defaultValue ) );
            */
            var dnode = GetNode( xpath );
            if( dnode == null )
                return defaultValue;
            return ReadNodeValue( dnode, key, defaultValue );
        }
        
        public static bool WriteStringValue( string xpath, string key, string value, bool commit = false )
        {
            /*
            DebugLog.Write(
                string.Format(
                    "\nWriteStringValue()\n\tXPath = \"{0}\"\n\tkey = \"{1}\"\n\tvalue = \"{2}\"\n\tcommit = {3}",
                    xpath, key, value, commit ) );
            */
            var dnode = GetNode( xpath );
            if( dnode == null )
            {
                dnode = MakeXPath( xpath );
                if( dnode == null )
                    return false;
            }
            return WriteNodeValue( dnode, key, value, commit );
        }
        
        public static bool WriteInt( string xpath, string key, int value, bool commit = false )
        {
            var svalue = value.ToString();
            //DebugLog.Write( xpath + "/" + key + " = " + svalue );
            return WriteStringValue( xpath, key, svalue, commit );
        }
        
        public static int ReadInt( string xpath, string key, int defaultValue )
        {
            var svalue = ReadStringValue( xpath, key );
            if( string.IsNullOrEmpty( svalue ) )
                return defaultValue;
            
            //DebugLog.Write( xpath + "/" + key + " = " + svalue + " ? " + defaultValue );
            int rvalue;
            return !int.TryParse( svalue, out rvalue )
                ? defaultValue
                : rvalue;
        }
        
        public static bool WriteFloat( string xpath, string key, float value, bool commit = false )
        {
            var svalue = value.ToString();
            return WriteStringValue( xpath, key, svalue, commit );
        }
        
        public static float ReadFloat( string xpath, string key, float defaultValue )
        {
            var svalue = ReadStringValue( xpath, key );
            if( string.IsNullOrEmpty( svalue ) )
                return defaultValue;
            
            float rvalue;
            return !float.TryParse( svalue, out rvalue )
                ? defaultValue
                : rvalue;
        }
        
        public static bool WriteSDLPoint( string xpath, string key, SDL2.SDL.SDL_Point value, bool commit = false )
        {
            var svalue = SDL2ThinLayer.Extensions.ToString( value );
            return WriteStringValue( xpath, key, svalue, commit );
        }
        
        public static SDL2.SDL.SDL_Point ReadSDLPoint( string xpath, string key, SDL2.SDL.SDL_Point defaultValue )
        {
            var svalue = ReadStringValue( xpath, key );
            if( string.IsNullOrEmpty( svalue ) )
                return defaultValue;
            
            SDL2.SDL.SDL_Point spoint;
            return SDL2ThinLayer.Extensions.TryParseSDLPoint( svalue, out spoint )
                ? spoint
                : defaultValue;
        }
        
        public static bool WritePoint( string xpath, string key, System.Drawing.Point value, bool commit = false )
        {
            var spoint = SDL2ThinLayer.Extensions.ToSDLPoint( value );
            var svalue = SDL2ThinLayer.Extensions.ToString( spoint );
            return WriteStringValue( xpath, key, svalue, commit );
        }
        
        public static System.Drawing.Point ReadPoint( string xpath, string key, System.Drawing.Point defaultValue )
        {
            var svalue = ReadStringValue( xpath, key );
            if( string.IsNullOrEmpty( svalue ) )
                return defaultValue;
            
            SDL2.SDL.SDL_Point spoint;
            return SDL2ThinLayer.Extensions.TryParseSDLPoint( svalue, out spoint )
                ? SDL2ThinLayer.Extensions.ToPoint( spoint )
                : defaultValue;
        }
        
        public static bool WriteSize( string xpath, string key, System.Drawing.Size value, bool commit = false )
        {
            var spoint = new SDL2.SDL.SDL_Point( value.Width, value.Height );
            var svalue = SDL2ThinLayer.Extensions.ToString( spoint );
            return WriteStringValue( xpath, key, svalue, commit );
        }
        
        public static System.Drawing.Size ReadSize( string xpath, string key, System.Drawing.Size defaultValue )
        {
            var svalue = ReadStringValue( xpath, key );
            if( string.IsNullOrEmpty( svalue ) )
                return defaultValue;
            
            SDL2.SDL.SDL_Point spoint;
            return SDL2ThinLayer.Extensions.TryParseSDLPoint( svalue, out spoint )
                ? new System.Drawing.Size( spoint.x, spoint.y )
                : defaultValue;
        }
        
        #endregion
        
        #region Paths
        
        public static string XmlPathTo( IXmlConfiguration config )
        {
            if( config == null ) return null;
            var xmlKey = config.XmlKey.ReplaceInvalidFilenameChars();
            if( string.IsNullOrEmpty( xmlKey ) ) return null;
            //DebugLog.Write( string.Format( "\nXmlPathTo :: xmlKey = \"{0}\" :: Parent ? {1}", xmlKey, ( config.XmlParent == null ? "false" : "true" ) ) );
            if( config.XmlParent == null ) return xmlKey;
            var pXPath = XmlPathTo( config.XmlParent );
            //var s = ""; //"\n" + System.Environment.StackTrace;
            //DebugLog.Write( string.Format( "\nXmlPathTo :: xmlKey = \"{0}\" :: pXPath = \"{1}\"{2}", xmlKey, pXPath, s ) );
            return string.IsNullOrEmpty( pXPath )
                ? xmlKey
                : string.Format( "{0}/{1}", pXPath, xmlKey );
        }
        
        public static XmlNode AppendNode( XmlNode parent, string key )
        {
            if( ( parent == null )||( string.IsNullOrEmpty( key ) ) )
               return null;
            return parent.AppendChild( _Document.CreateElement( key ) );
        }
        
        public static XmlNode MakeXPath( string xpath )
        {
            return MakeXPath( _RootNode, xpath );
        }
        
        public static XmlNode MakeXPath( XmlNode parent, string xpath )
        {
            if( _Document == null )
                return null;
            // grab the next node name in the xpath; or return parent if empty
            string[] partsOfXPath = xpath.Trim('/').Split('/');
            string nextNodeInXPath = partsOfXPath.First();
            if( string.IsNullOrEmpty( nextNodeInXPath ) )
                return parent;
            
            // get or create the node from the name
            XmlNode node = parent.SelectSingleNode( nextNodeInXPath );
            if( node == null )
                node = AppendNode( parent, nextNodeInXPath );
            
            // rejoin the remainder of the array as an xpath expression and recurse
            string rest = string.Join( "/", partsOfXPath.Skip( 1 ).ToArray() );
            return MakeXPath( node, rest );
        }
        
        #endregion
        
    }
}
