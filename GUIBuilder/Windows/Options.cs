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
        /// Use Application.Run( new GUIBuilder.Windows.Min() ) to create this Window or,
        /// Use GodObject.Windows.GetWindow<Options>() to create this Window
        /// </summary>

        private System.Windows.Forms.TextBox[] tbNIFExportInfo;

        public Options() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();

            this.ClientLoad += new System.EventHandler( this.OnClientLoad );
            this.OnSetEnableState += OnClientSetEnableState;

            this.cbLanguage.SelectedIndexChanged += new System.EventHandler( this.OnLanguageChanged );
            this.cbSDLVideoDriver.SelectedIndexChanged += new System.EventHandler( this.OnSDLVideoDriverChanged );
            this.cbZipLogFiles.CheckedChanged += new System.EventHandler( this.OnZipLogsChanged );
            this.cbLogMainToConsole.CheckedChanged += new System.EventHandler( this.OnMirrorMainLogToConsoleChanged );
            this.btnNIFExportInfoReset.Click += new System.EventHandler( this.OnNIFExportInfoResetButtonClick );

            this.tbNIFExportInfo_3.TextChanged += new System.EventHandler( this.OnNIFExportInfoChanged );
            this.tbNIFExportInfo_2.TextChanged += new System.EventHandler( this.OnNIFExportInfoChanged );
            this.tbNIFExportInfo_1.TextChanged += new System.EventHandler( this.OnNIFExportInfoChanged );
            this.tbNIFExportInfo_0.TextChanged += new System.EventHandler( this.OnNIFExportInfoChanged );

            tbNIFExportInfo = new System.Windows.Forms.TextBox[]{
                tbNIFExportInfo_0,
                tbNIFExportInfo_1,
                tbNIFExportInfo_2,
                tbNIFExportInfo_3
            };

            this.lvAlwaysSelectMasters.OnSetSyncObjectsThreadComplete += OnSyncAlwaysSelectMastersThreadComplete;

            this.ResumeLayout( false );
        }
        
        
        #region Client Window Events

        void OnClientLoad( object sender, EventArgs e )
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

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        bool OnClientSetEnableState( object sender, bool enable )
        {
            var enabled =
                enable &&
                !lvAlwaysSelectMasters.IsSyncObjectsThreadRunning;
            return enabled;
        }

        #endregion

        #region Sync'd list monitoring

        void OnSyncAlwaysSelectMastersThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<GodObject.Master.File> sender )
        {
            SetEnableState( sender, true );
        }

        #endregion

        #region Language

        void OnLanguageChanged(object sender, EventArgs e)
        {
            if( !OnLoadComplete ) return;
            GodObject.Paths.Language = cbLanguage.Text;
        }

        #endregion

        #region SDL Hints

        void OnSDLVideoDriverChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.Windows.SDLVideoDriverIndex = cbSDLVideoDriver.SelectedIndex;
        }

        #endregion

        #region Log Files

        void OnMirrorMainLogToConsoleChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_MirrorToConsole, cbLogMainToConsole.Checked, true );
        }
        
        void OnZipLogsChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, cbZipLogFiles.Checked, true );
        }

        #endregion

        #region NIF ExportInfo

        bool blockExportInfoUI = false;

        void OnNIFExportInfoChanged( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            if( blockExportInfoUI ) return;

            var exportInfo = new string[ 4 ];
            for( int i = 0; i < 4; i++ )
                exportInfo[ i ] = tbNIFExportInfo[ i ].Text;

            NIFBuilder.ExportInfo = exportInfo;
        }
        
        void OnNIFExportInfoResetButtonClick( object sender, EventArgs e )
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
