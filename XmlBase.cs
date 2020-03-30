/*
 * XmlBase.cs
 *
 * Base class for accessing Xml
 *
 */
//#define CONSOLE_LOG

using System;
using System.Linq;
using System.Xml;


public abstract class XmlBase
{
    
    public abstract bool            XmlForceCreateFile                      { get; }
    
    public abstract bool            XmlFileMustExist                        { get; }
    
    public abstract string          RootNodeName                            { get; }
    
    public abstract string          Pathname                                { get; }
    
    public virtual void             OnLoad()                                {}
    public virtual void             OnInit()                                {}
    
    #region Internal
    
    bool                            _WasReset                               = false;
    
    XmlDocument                     _Document                               = null;
    XmlNode                         _RootNode                               = null;
    
    XmlNode                         RootNode
    {
        get
        {
            if( _Document == null )
            {
                var pathname = Pathname;
#if CONSOLE_LOG
                Console.WriteLine( string.Format( "XmlBase.Pathname = \"{0}\"", pathname ) );
#endif
                if( string.IsNullOrEmpty( pathname ) )
                    return null;
                
                string xmlFile = null;
                var fileExists = pathname.TryAssignFile( ref xmlFile, XmlForceCreateFile );
                if( fileExists )
                {
#if CONSOLE_LOG
                    Console.WriteLine( "File found!" );
#endif
                    _Document = new XmlDocument();
                    if( _Document == null )
                        return null;
                    _Document.Load( xmlFile );
                    _RootNode = _Document.SelectSingleNode( RootNodeName );
                    
                    OnLoad();
                }
                else
                {
#if CONSOLE_LOG
                    Console.WriteLine( "File not found!" );
#endif
                    if( ( !XmlForceCreateFile )&&( XmlFileMustExist ) )
                        return null;
                    
                    _Document = new XmlDocument();
                    if( _Document == null )
                        return null;
                    
                    _RootNode = _Document.CreateElement( RootNodeName );
                    _Document.AppendChild( _RootNode );
                }
                
                OnInit();
                
                if( XmlForceCreateFile )
                    Commit();
            }
            return _RootNode;
        }
    }
    
    #endregion
    
    public bool                     WasReset
    {
        get
        {
            var rnode = RootNode; // Get the root node to trigger a possible reset
            return _WasReset;
        }
    }
    
    protected void                  Reset( bool doInit = true, bool commit = false )
    {
        //Console.WriteLine( "RESETTING!" );
        _WasReset = true;
        _RootNode.RemoveAll();
        if( doInit )
            OnInit();
        if( commit )
            Commit();
    }
    
    public bool                     Commit()
    {
        var xdoc = _Document;
        if( xdoc == null )
        {
            //Console.WriteLine( "No document to commit!" );
            return false;
        }
        var xmlFile = Pathname;
        if( string.IsNullOrEmpty( xmlFile ) )
        {
            //Console.WriteLine( "No xml file to save to!" );
            return false;
        }
        
        var xsettings = new XmlWriterSettings();
        if( xsettings == null )
            return false;
        xsettings.Indent = true;
        
        var xwrite = XmlWriter.Create( xmlFile, xsettings );
        if( xwrite == null )
        {
            //Console.WriteLine( "Cannot create writer for \"" + xmlFile + "\"" );
            return false;
        }
        
        xdoc.WriteTo( xwrite );
        xwrite.Close();
        
        return true;
    }
    
    #region Nodes
    
    public XmlNodeList              GetNodes( XmlNode pnode, string xpath )
    {
        if( pnode == null )
            pnode = RootNode;
        return pnode?.SelectNodes( xpath );
    }
    
    public XmlNodeList              GetNodes( string xpath )
    {
        return GetNodes( null, xpath );
    }
    
    public XmlNode                  GetNode( XmlNode pnode, string xpath )
    {
        if( pnode == null )
            pnode = RootNode;
        return pnode?.SelectSingleNode( xpath );
    }
    
    public XmlNode                  GetNode( string xpath )
    {
        return GetNode( null, xpath );
    }

    public void                     RemoveNode( XmlNode pnode, string xpath )
    {
        if( pnode == null )
            pnode = RootNode;
        var child = GetNode( pnode, xpath );
        if( child == null ) return;
        pnode?.RemoveChild( child );
    }

    public void                     RemoveNode( string xpath )
    {
        RemoveNode( null, xpath );
    }


    public XmlNode                  FindChildNode( XmlNode pnode, string xpath, string value )
    {
        if( pnode == null )
            pnode = RootNode;
        var cnodes = pnode?.SelectNodes( xpath );
        if( cnodes == null ) return null;
        foreach( XmlNode cnode in cnodes )
        {
            var it = cnode.InnerText;
            if( it == value )
                return cnode;
        }
        return null;
    }
    
    public XmlNode                  FindChildNode( string xpath, string value )
    {
        return FindChildNode( null, xpath, value );
    }
    
    #endregion
    
    #region Paths
    
    public XmlNode                  AppendNode( XmlNode pnode, string key )
    {
        if( pnode == null )
            pnode = RootNode;
        return ( pnode == null )||( string.IsNullOrEmpty( key ) )
            ? null
            : pnode.AppendChild( _Document.CreateElement( key ) );
    }
    
    public XmlNode                  AppendNode( string key )
    {
        return AppendNode( null, key );
    }
    
    public XmlNode                  MakeXPath( XmlNode pnode, string xpath )
    {
        if( _Document == null )
            return null;
        // grab the next node name in the xpath; or return parent if empty
        var partsOfXPath = xpath.Trim('/').Split('/');
        var nextNodeInXPath = partsOfXPath.First();
        if( string.IsNullOrEmpty( nextNodeInXPath ) )
            return pnode;
        
        // get or create the node from the name
        XmlNode node = pnode.SelectSingleNode( nextNodeInXPath ) ?? AppendNode( pnode, nextNodeInXPath );
        
        // rejoin the remainder of the array as an xpath expression and recurse
        var rest = string.Join( "/", partsOfXPath.Skip( 1 ).ToArray() );
        return MakeXPath( node, rest );
    }
    
    public XmlNode                  MakeXPath( string xpath )
    {
        return MakeXPath( RootNode, xpath );
    }
    
    #endregion
    
    #region Raw Nodes
    
    public string                   ReadNode( XmlNode pnode, string xpath, string key )
    {
        if( pnode == null )
            pnode = RootNode;
        var xnode = string.IsNullOrEmpty( xpath )
            ? pnode
            : GetNode( pnode, xpath );
        if( xnode == null )
            return null;
        var knode = xnode.SelectSingleNode( key );
        return knode == null ? null : knode.InnerText;
    }
    public string                   ReadNode( string xpath, string key )
    {
        return ReadNode( null, xpath, key );
    }
    
    
    public bool                     WriteNode( XmlNode pnode, string xpath, string key, string value, bool commit = false )
    {
        if( pnode == null )
            pnode = RootNode;
        
        var xnode = string.IsNullOrEmpty( xpath )
            ? pnode
            : GetNode( pnode, xpath ) ?? MakeXPath( pnode, xpath );
        if( xnode == null )
            return false;
        
        var knode = xnode.SelectSingleNode( key ) ?? MakeXPath( xnode, key );
        if( knode == null )
            return false;
        
        knode.InnerText = value;
        
        return !commit || Commit();
    }
    public bool                     WriteNode( string xpath, string key, string value, bool commit = false )
    {
        return WriteNode( null, xpath, key, value, commit );
    }
    
    #endregion
    
    #region Generic Values
    
    public T                        ReadValue<T>( XmlNode pnode, string xpath, string key, T defaultValue = default(T) )
    {
        T result = default(T);
        return ReadNode( pnode, xpath, key ).TryCast( out result )
            ? result
            : defaultValue;
    }
    public T                        ReadValue<T>( XmlNode pnode, string key, T defaultValue = default(T) )
    {
        return ReadValue<T>( pnode, null, key, defaultValue );
    }
    public T                        ReadValue<T>( string xpath, string key, T defaultValue = default(T) )
    {
        return ReadValue<T>( null, xpath, key, defaultValue );
    }
    public T                        ReadValue<T>( string key, T defaultValue = default(T) )
    {
        return ReadValue<T>( null, null, key, defaultValue );
    }
    
    public bool                     WriteValue<T>( XmlNode pnode, string xpath, string key, T value, bool commit = false )
    {
        string s = null;
        return value.TryCast( out s ) && WriteNode( pnode, xpath, key, s, commit );
    }
    public bool                     WriteValue<T>( XmlNode pnode, string key, T value, bool commit = false )
    {
        return WriteValue<T>( pnode, null, key, value, commit );
    }
    public bool                     WriteValue<T>( string xpath, string key, T value, bool commit = false )
    {
        return WriteValue<T>( null, xpath, key, value, commit );
    }
    public bool                     WriteValue<T>( string key, T value, bool commit = false )
    {
        return WriteValue<T>( null, null, key, value, commit );
    }
    
    #endregion
    
}