/*
 * Options.cs
 *
 * Main GUIBuilder configuration options.
 *
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of Options.
    /// </summary>
    public partial class Options : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return null; } }
        public string XmlNodeName { get{ return "OptionsWindow"; } }
        
        bool onLoadComplete = false;
        
        public Options()
        {
            InitializeComponent();
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            tbSDLVideoRenderWarning.Text = string.Format( "OptionsWindow.SDLHint.Warning".Translate(), GodObject.Windows.SDLVideoDriverSoftware );
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            
            lvAlwaysSelectMasters.SyncObjects = GodObject.Master.Files;
            UpdateCSColors();
            
            cbLanguage.Items.Clear();
            var languages = GodObject.Paths.LanguageOptions;
            foreach( var lang in languages )
                cbLanguage.Items.Add( lang );
            cbLanguage.SelectedIndex = languages.IndexOf( GodObject.Paths.Language );
            
            cbSDLVideoDriver.Items.Clear();
            for( int i = 0; i < GodObject.Windows.SDLVideoDrivers.Length; i++ )
                cbSDLVideoDriver.Items.Add( GodObject.Windows.SDLVideoDrivers[ i ] );
            cbSDLVideoDriver.SelectedIndex = GodObject.Windows.SDLVideoDriverIndex;
            
            cbZipLogFiles.Checked = GodObject.XmlConfig.ReadValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, true );
            
            this.BringToFront();
            onLoadComplete = true;
        }
        
        void OnFormClosed( object sender, FormClosedEventArgs e )
        {
            GodObject.Windows.SetWindow<Options>( null, false );
        }
        
        void OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteLocation( this );
        }
        
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( this );
        }
        
        public void SetEnableState( bool enabled ) {}

        void cbLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            if( !onLoadComplete )
                return;
            GodObject.Paths.Language = cbLanguage.Text;
        }
        
        void cbSDLVideoDriverSelectedIndexChanged( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.Windows.SDLVideoDriverIndex = cbSDLVideoDriver.SelectedIndex;
        }
        
        #region Conflict Status
        
        void UpdateCSColors()
        {
            tbCSInvalid.BackColor = Engine.Plugin.ConflictStatus.Invalid.GetConflictStatusBackColor();
            tbCSUneditable.BackColor = Engine.Plugin.ConflictStatus.Uneditable.GetConflictStatusBackColor();
            tbCSNewForm.BackColor = Engine.Plugin.ConflictStatus.NewForm.GetConflictStatusBackColor();
            tbCSNoConflict.BackColor = Engine.Plugin.ConflictStatus.NoConflict.GetConflictStatusBackColor();
            tbCSOverrideInAncestor.BackColor = Engine.Plugin.ConflictStatus.OverrideInAncestor.GetConflictStatusBackColor();
            tbCSOverrideInWorkingFile.BackColor = Engine.Plugin.ConflictStatus.OverrideInWorkingFile.GetConflictStatusBackColor();
            tbCSOverrideInPostLoad.BackColor = Engine.Plugin.ConflictStatus.OverrideInPostLoad.GetConflictStatusBackColor();
            tbCSRequiresOverride.BackColor = Engine.Plugin.ConflictStatus.RequiresOverride.GetConflictStatusBackColor();
        }
        
        void cbZipLogFilesCheckedChanged( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, cbZipLogFiles.Checked, true );
        }
        
        #endregion
        
    }
}
