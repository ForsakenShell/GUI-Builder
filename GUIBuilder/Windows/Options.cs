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
    public partial class Options : WindowBase
    {

        /// <summary>
        /// Use GodObject.Windows.GetWindow<Options>() to create this Window
        /// </summary>

        private System.Windows.Forms.TextBox[] tbNIFExportInfo;

        public Options() : base( true )
        {
            InitializeComponent();
            this.ClientLoad += new System.EventHandler( this.Options_OnLoad );
            tbNIFExportInfo = new System.Windows.Forms.TextBox[]{
                tbNIFExportInfo_0,
                tbNIFExportInfo_1,
                tbNIFExportInfo_2,
                tbNIFExportInfo_3
            };
        }
        
        
        #region GodObject.XmlConfig.IXmlConfiguration
        
        
        public override string XmlNodeName { get { return "OptionsWindow"; } }


        #endregion

        #region Options OnLoad

        void Options_OnLoad( object sender, EventArgs e )
        {
            tbSDLVideoRenderWarning.Text = string.Format( "OptionsWindow.SDLHint.Warning".Translate(), GodObject.Windows.SDLVideoDriverSoftware );
            
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
            
            cbLogMainToConsole.Checked = GodObject.XmlConfig.ReadValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_MirrorToConsole, false );
            cbZipLogFiles.Checked = GodObject.XmlConfig.ReadValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, true );

            RepopulateExportInfoTextBoxes();

            this.BringToFront();
        }

        #endregion

        #region Language

        void cbLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            if( !OnLoadComplete ) return;
            GodObject.Paths.Language = cbLanguage.Text;
        }

        #endregion

        #region SDL Hints

        void cbSDLVideoDriverSelectedIndexChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.Windows.SDLVideoDriverIndex = cbSDLVideoDriver.SelectedIndex;
        }

        #endregion

        #region Log Files

        void cbLogMainToConsoleCheckedChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_MirrorToConsole, cbLogMainToConsole.Checked, true );
        }
        
        void cbZipLogFilesCheckedChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, cbZipLogFiles.Checked, true );
        }

        #endregion

        #region NIF ExportInfo

        bool blockExportInfoUI = false;

        void tbNIFExportInfoTextChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            if( blockExportInfoUI ) return;

            var exportInfo = new string[ 4 ];
            for( int i = 0; i < 4; i++ )
                exportInfo[ i ] = tbNIFExportInfo[ i ].Text;

            NIFBuilder.ExportInfo = exportInfo;
        }
        
        void btnNIFExportInfoResetClick( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            if( blockExportInfoUI ) return;
            blockExportInfoUI = true;

            NIFBuilder.ExportInfo = null;
            RepopulateExportInfoTextBoxes();

            blockExportInfoUI = false;
        }

        void RepopulateExportInfoTextBoxes()
        {
            var exportInfo = NIFBuilder.ExportInfo;
            for( int i = 0; i < 4; i++ )
                tbNIFExportInfo[ i ].Text = exportInfo[ i ];
        }

        #endregion

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
        
        #endregion
        
    }
}
