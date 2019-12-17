/*
 * Translator.cs
 *
 * Translation handler for GUIBuilder.
 *
 * Translations for Controls are taken from the Control.Tag and replace the Control.Text.
 * 
 * Prefixing "ToolTip:" to the ToolStripItem.Tag will instead replace the ToolStripItem.ToolTip with the translation.
 *     NOTE:  This is handled in the translation method for ToolStripItem.
 * 
 * Suffixing the translation key (including Control.Tag) with a colon (":") will add a colon to the end of the translation.
 *     NOTE:  This is handled in the core string.Translate() method that all Translate() methods use.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public static class Translator
{
    
    #region Internal
    
    class TranslationInterface : XmlBase
    {
        
        public override bool            XmlForceCreateFile                      { get{ return false; } }
        
        public override bool            XmlFileMustExist                        { get{ return true; } }
        
        public override string          RootNodeName                            { get{ return "GUIBuilder"; } }
        
        public override string          Pathname                                { get{ return GodObject.Paths.GUIBuilderLanguageFile; } }
        
    }
    
    static readonly string              MissingTranslation                      = "Missing translation";
    static readonly string              ToolTipPrefix                           = "ToolTip:";
    
    static TranslationInterface         _TranslationInterface                   = new TranslationInterface();
    static Dictionary<string,string>    _Translations                           = new Dictionary<string, string>();
    
    #endregion
    
    /// <summary>
    /// Get the language translation from a translation key.
    /// </summary>
    /// <param name="key">Key in the translation file to return the translation of.</param>
    /// <returns></returns>
    public static string Translate( this string key )
    {
        bool colonSuffix = false;
        string wKey = (string)key.Clone();
        string result = null;
        if( wKey.EndsWith( ":", StringComparison.Ordinal ) )
        {
            colonSuffix = true;
            wKey = wKey.Substring( 0, wKey.Length - 1 );
        }
        if( _Translations.TryGetValue( wKey, out result ) )
        {
            if( colonSuffix ) result += ":";
            return result;
        }
        var knode = _TranslationInterface.GetNode( wKey );
        if( knode == null )
        {
            DebugLog.WriteLine( string.Format( "{0} :: \"{1}\"", MissingTranslation, key ) );
            result = MissingTranslation;
        }
        else
            result = knode.InnerText;
        _Translations.Add( wKey, result );
        if( colonSuffix ) result += ":";
        return result;
    }
    
    /// <summary>
    /// Translate a Control.Tag into the language appropriate Control.Text.
    /// </summary>
    /// <param name="control">Control to translate.</param>
    /// <param name="translateChildControls">Recursively translate child Control.Controls, ToolStrip.Items and, TabControl.TabPages?</param>
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
    
    /// <summary>
    /// Translate a ToolStripItem.Tag into the language appropriate ToolStripItem.Text.
    /// </summary>
    /// <param name="item">ToolStripItem to translate.</param>
    /// <param name="translateChildItems">Recursively translate child ToolStripItem.Items?</param>
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
