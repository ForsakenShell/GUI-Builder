/*
 * Translator.cs
 *
 * Translation handler for GUIBuilder.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public static class Translator
{
    
    #region Internal
    
    private static readonly string Root = "GUIBuilder";
    private static readonly string MissingTranslation = "Missing translation";
    private static readonly string ToolTipPrefix = "ToolTip:";
    
    static Dictionary<string,string> _Translations = null;
    
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
                
                var translateFile = GodObject.Paths.GUIBuilderLanguageFile;
                if( ( string.IsNullOrEmpty( translateFile ) )||( !System.IO.File.Exists( translateFile ) ) )
                    return null;
                
                _Document.Load( translateFile );
                _RootNode = _Document.SelectSingleNode( Root );
            }
            return _RootNode;
        }
    }
    
    #endregion
    
    public static string Translate( this string key )
    {
        if( RootNode == null ) return MissingTranslation;
        if( _Translations == null )
            _Translations = new Dictionary<string, string>();
        string result = null;
        if( _Translations.TryGetValue( key, out result ) )
            return result;
        var knode = RootNode.SelectSingleNode( key );
        if( knode == null )
        {
            DebugLog.WriteLine( string.Format( "{0} :: \"{1}\"", MissingTranslation, key ) );
            result = MissingTranslation;
        }
        else
            result = knode.InnerText;
        _Translations.Add( key, result );
        return result;
    }
    
    public static void Translate( this System.Windows.Forms.Control control, bool translateChildControls = false )
    {
        if( control == null ) return;
        
        string key = (string)control.Tag;
        if( !string.IsNullOrEmpty( key ) )
            control.Text = Translate( key );
        
        if( !translateChildControls ) return;
        
        for( int i = 0; i < control.Controls.Count; i++ )
            control.Controls[ i ].Translate( true );
        
        var toolStrip = control as System.Windows.Forms.ToolStrip;
        if( toolStrip != null )
            for( int i = 0; i < toolStrip.Items.Count; i++ )
                toolStrip.Items[ i ].Translate( true );
        
        var tabControl = control as System.Windows.Forms.TabControl;
        if( tabControl != null )
            for( int i = 0; i < tabControl.TabPages.Count; i++ )
                tabControl.TabPages[ i ].Translate( true );
    }
    
    public static void Translate( this System.Windows.Forms.ToolStripItem item, bool translateChildItems = false )
    {
        if( item == null ) return;
        
        string key = (string)item.Tag;
        if( !string.IsNullOrEmpty( key ) )
        {
            var tipText = key.StartsWith( ToolTipPrefix, StringComparison.InvariantCultureIgnoreCase );
            if( tipText )
                item.ToolTipText = Translate( key.Substring( ToolTipPrefix.Length ) );
            else
                item.Text = Translate( key );
        }
        
        if( !translateChildItems ) return;
        
        var dropItem = item as System.Windows.Forms.ToolStripDropDownItem;
        if( dropItem != null )
            for( int i = 0; i < dropItem.DropDownItems.Count; i++ )
                dropItem.DropDownItems[ i ].Translate( true );
    }
    
}
