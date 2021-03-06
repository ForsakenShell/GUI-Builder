﻿/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class Options
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbAlwaysSelectMasters;
        GUIBuilder.Windows.Controls.SyncedListView<GodObject.Master.File> lvAlwaysSelectMasters;
        System.Windows.Forms.GroupBox gbConflictStatus;
        System.Windows.Forms.TextBox tbCSInvalid;
        System.Windows.Forms.TextBox tbCSNoConflict;
        System.Windows.Forms.TextBox tbCSOverrideInWorkingFile;
        System.Windows.Forms.TextBox tbCSOverrideInAncestor;
        System.Windows.Forms.TextBox tbCSNewForm;
        System.Windows.Forms.TextBox tbCSRequiresOverride;
        System.Windows.Forms.TextBox tbCSUneditable;
        private System.Windows.Forms.GroupBox gbSDLHints;
        private System.Windows.Forms.ComboBox cbSDLVideoDriver;
        private System.Windows.Forms.Label lblSDLVideoDriver;
        private System.Windows.Forms.TextBox tbSDLVideoRenderWarning;
        private System.Windows.Forms.GroupBox gbLanguage;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.TextBox tbLanguageRestart;
        private System.Windows.Forms.TextBox tbCSOverrideInPostLoad;
        private System.Windows.Forms.CheckBox cbZipLogFiles;
        private System.Windows.Forms.CheckBox cbLogMainToConsole;
        private System.Windows.Forms.GroupBox gbLogs;
        private System.Windows.Forms.GroupBox gbNIFExportInfo;
        private System.Windows.Forms.TextBox tbNIFExportInfo_3;
        private System.Windows.Forms.TextBox tbNIFExportInfo_2;
        private System.Windows.Forms.TextBox tbNIFExportInfo_1;
        private System.Windows.Forms.TextBox tbNIFExportInfo_0;
        private System.Windows.Forms.Button btnNIFExportInfoReset;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.gbAlwaysSelectMasters = new System.Windows.Forms.GroupBox();
            this.lvAlwaysSelectMasters = new GUIBuilder.Windows.Controls.SyncedListView<GodObject.Master.File>();
            this.tbCSInvalid = new System.Windows.Forms.TextBox();
            this.tbCSNewForm = new System.Windows.Forms.TextBox();
            this.tbCSOverrideInAncestor = new System.Windows.Forms.TextBox();
            this.tbCSOverrideInWorkingFile = new System.Windows.Forms.TextBox();
            this.tbCSNoConflict = new System.Windows.Forms.TextBox();
            this.tbCSRequiresOverride = new System.Windows.Forms.TextBox();
            this.tbCSUneditable = new System.Windows.Forms.TextBox();
            this.tbCSOverrideInPostLoad = new System.Windows.Forms.TextBox();
            this.gbConflictStatus = new System.Windows.Forms.GroupBox();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.tbLanguageRestart = new System.Windows.Forms.TextBox();
            this.gbLanguage = new System.Windows.Forms.GroupBox();
            this.lblSDLVideoDriver = new System.Windows.Forms.Label();
            this.cbSDLVideoDriver = new System.Windows.Forms.ComboBox();
            this.tbSDLVideoRenderWarning = new System.Windows.Forms.TextBox();
            this.gbSDLHints = new System.Windows.Forms.GroupBox();
            this.cbZipLogFiles = new System.Windows.Forms.CheckBox();
            this.cbLogMainToConsole = new System.Windows.Forms.CheckBox();
            this.gbLogs = new System.Windows.Forms.GroupBox();
            this.gbNIFExportInfo = new System.Windows.Forms.GroupBox();
            this.btnNIFExportInfoReset = new System.Windows.Forms.Button();
            this.tbNIFExportInfo_3 = new System.Windows.Forms.TextBox();
            this.tbNIFExportInfo_2 = new System.Windows.Forms.TextBox();
            this.tbNIFExportInfo_1 = new System.Windows.Forms.TextBox();
            this.tbNIFExportInfo_0 = new System.Windows.Forms.TextBox();
            this.WindowPanel.SuspendLayout();
            this.gbAlwaysSelectMasters.SuspendLayout();
            this.gbConflictStatus.SuspendLayout();
            this.gbLanguage.SuspendLayout();
            this.gbSDLHints.SuspendLayout();
            this.gbLogs.SuspendLayout();
            this.gbNIFExportInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.gbNIFExportInfo);
            this.WindowPanel.Controls.Add(this.gbLogs);
            this.WindowPanel.Controls.Add(this.gbAlwaysSelectMasters);
            this.WindowPanel.Controls.Add(this.gbConflictStatus);
            this.WindowPanel.Controls.Add(this.gbLanguage);
            this.WindowPanel.Controls.Add(this.gbSDLHints);
            this.WindowPanel.Size = new System.Drawing.Size(627, 431);
            // 
            // gbAlwaysSelectMasters
            // 
            this.gbAlwaysSelectMasters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAlwaysSelectMasters.Controls.Add(this.lvAlwaysSelectMasters);
            this.gbAlwaysSelectMasters.Location = new System.Drawing.Point(3, 3);
            this.gbAlwaysSelectMasters.Name = "gbAlwaysSelectMasters";
            this.gbAlwaysSelectMasters.Size = new System.Drawing.Size(309, 311);
            this.gbAlwaysSelectMasters.TabIndex = 0;
            this.gbAlwaysSelectMasters.TabStop = false;
            this.gbAlwaysSelectMasters.Tag = "OptionsWindow.Masters";
            this.gbAlwaysSelectMasters.Text = "Masters";
            // 
            // lvAlwaysSelectMasters
            // 
            this.lvAlwaysSelectMasters.AllowHidingItems = false;
            this.lvAlwaysSelectMasters.AllowOverrideColumnSorting = false;
            this.lvAlwaysSelectMasters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAlwaysSelectMasters.CheckBoxes = true;
            this.lvAlwaysSelectMasters.EditorIDColumn = false;
            this.lvAlwaysSelectMasters.ExtraInfoColumn = false;
            this.lvAlwaysSelectMasters.FilenameColumn = true;
            this.lvAlwaysSelectMasters.FormIDColumn = false;
            this.lvAlwaysSelectMasters.LoadOrderColumn = true;
            this.lvAlwaysSelectMasters.Location = new System.Drawing.Point(6, 19);
            this.lvAlwaysSelectMasters.MultiSelect = false;
            this.lvAlwaysSelectMasters.Name = "lvAlwaysSelectMasters";
            this.lvAlwaysSelectMasters.Size = new System.Drawing.Size(297, 281);
            this.lvAlwaysSelectMasters.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.LoadOrder;
            this.lvAlwaysSelectMasters.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvAlwaysSelectMasters.SyncedEditorFormType = null;
            this.lvAlwaysSelectMasters.SyncObjects = null;
            this.lvAlwaysSelectMasters.TabIndex = 13;
            this.lvAlwaysSelectMasters.TypeColumn = true;
            // 
            // tbCSInvalid
            // 
            this.tbCSInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSInvalid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSInvalid.Location = new System.Drawing.Point(9, 19);
            this.tbCSInvalid.Name = "tbCSInvalid";
            this.tbCSInvalid.ReadOnly = true;
            this.tbCSInvalid.Size = new System.Drawing.Size(291, 13);
            this.tbCSInvalid.TabIndex = 0;
            this.tbCSInvalid.Tag = "Conflict.Error";
            this.tbCSInvalid.Text = "Error State";
            // 
            // tbCSNewForm
            // 
            this.tbCSNewForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSNewForm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSNewForm.Location = new System.Drawing.Point(9, 45);
            this.tbCSNewForm.Name = "tbCSNewForm";
            this.tbCSNewForm.ReadOnly = true;
            this.tbCSNewForm.Size = new System.Drawing.Size(291, 13);
            this.tbCSNewForm.TabIndex = 2;
            this.tbCSNewForm.Tag = "Conflict.NewForm";
            this.tbCSNewForm.Text = "New Form";
            // 
            // tbCSOverrideInAncestor
            // 
            this.tbCSOverrideInAncestor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSOverrideInAncestor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSOverrideInAncestor.Location = new System.Drawing.Point(9, 71);
            this.tbCSOverrideInAncestor.Name = "tbCSOverrideInAncestor";
            this.tbCSOverrideInAncestor.ReadOnly = true;
            this.tbCSOverrideInAncestor.Size = new System.Drawing.Size(291, 13);
            this.tbCSOverrideInAncestor.TabIndex = 4;
            this.tbCSOverrideInAncestor.Tag = "Conflict.OverrideInAncestor";
            this.tbCSOverrideInAncestor.Text = "Override in Ancestor";
            // 
            // tbCSOverrideInWorkingFile
            // 
            this.tbCSOverrideInWorkingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSOverrideInWorkingFile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSOverrideInWorkingFile.Location = new System.Drawing.Point(9, 84);
            this.tbCSOverrideInWorkingFile.Name = "tbCSOverrideInWorkingFile";
            this.tbCSOverrideInWorkingFile.ReadOnly = true;
            this.tbCSOverrideInWorkingFile.Size = new System.Drawing.Size(291, 13);
            this.tbCSOverrideInWorkingFile.TabIndex = 5;
            this.tbCSOverrideInWorkingFile.Tag = "Conflict.OverrideInWorkingFile";
            this.tbCSOverrideInWorkingFile.Text = "Override in Working File";
            // 
            // tbCSNoConflict
            // 
            this.tbCSNoConflict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSNoConflict.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSNoConflict.Location = new System.Drawing.Point(9, 58);
            this.tbCSNoConflict.Name = "tbCSNoConflict";
            this.tbCSNoConflict.ReadOnly = true;
            this.tbCSNoConflict.Size = new System.Drawing.Size(291, 13);
            this.tbCSNoConflict.TabIndex = 3;
            this.tbCSNoConflict.Tag = "Conflict.None";
            this.tbCSNoConflict.Text = "No Conflict";
            // 
            // tbCSRequiresOverride
            // 
            this.tbCSRequiresOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSRequiresOverride.BackColor = System.Drawing.SystemColors.Control;
            this.tbCSRequiresOverride.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSRequiresOverride.Location = new System.Drawing.Point(9, 110);
            this.tbCSRequiresOverride.Name = "tbCSRequiresOverride";
            this.tbCSRequiresOverride.ReadOnly = true;
            this.tbCSRequiresOverride.Size = new System.Drawing.Size(291, 13);
            this.tbCSRequiresOverride.TabIndex = 6;
            this.tbCSRequiresOverride.Tag = "Conflict.OverrideRequired";
            this.tbCSRequiresOverride.Text = "Requires Override";
            // 
            // tbCSUneditable
            // 
            this.tbCSUneditable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSUneditable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSUneditable.Location = new System.Drawing.Point(9, 32);
            this.tbCSUneditable.Name = "tbCSUneditable";
            this.tbCSUneditable.ReadOnly = true;
            this.tbCSUneditable.Size = new System.Drawing.Size(291, 13);
            this.tbCSUneditable.TabIndex = 1;
            this.tbCSUneditable.Tag = "Conflict.Uneditable";
            this.tbCSUneditable.Text = "Uneditable";
            // 
            // tbCSOverrideInPostLoad
            // 
            this.tbCSOverrideInPostLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSOverrideInPostLoad.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSOverrideInPostLoad.Location = new System.Drawing.Point(9, 97);
            this.tbCSOverrideInPostLoad.Name = "tbCSOverrideInPostLoad";
            this.tbCSOverrideInPostLoad.ReadOnly = true;
            this.tbCSOverrideInPostLoad.Size = new System.Drawing.Size(291, 13);
            this.tbCSOverrideInPostLoad.TabIndex = 7;
            this.tbCSOverrideInPostLoad.Tag = "Conflict.OverrideInPostLoad";
            this.tbCSOverrideInPostLoad.Text = "Override in Post Load File";
            // 
            // gbConflictStatus
            // 
            this.gbConflictStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbConflictStatus.Controls.Add(this.tbCSOverrideInPostLoad);
            this.gbConflictStatus.Controls.Add(this.tbCSUneditable);
            this.gbConflictStatus.Controls.Add(this.tbCSRequiresOverride);
            this.gbConflictStatus.Controls.Add(this.tbCSNoConflict);
            this.gbConflictStatus.Controls.Add(this.tbCSOverrideInWorkingFile);
            this.gbConflictStatus.Controls.Add(this.tbCSOverrideInAncestor);
            this.gbConflictStatus.Controls.Add(this.tbCSNewForm);
            this.gbConflictStatus.Controls.Add(this.tbCSInvalid);
            this.gbConflictStatus.Location = new System.Drawing.Point(318, 3);
            this.gbConflictStatus.Name = "gbConflictStatus";
            this.gbConflictStatus.Size = new System.Drawing.Size(309, 129);
            this.gbConflictStatus.TabIndex = 1;
            this.gbConflictStatus.TabStop = false;
            this.gbConflictStatus.Tag = "OptionsWindow.ConflictStatus";
            this.gbConflictStatus.Text = "Conflicts";
            // 
            // cbLanguage
            // 
            this.cbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Location = new System.Drawing.Point(6, 63);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(297, 21);
            this.cbLanguage.TabIndex = 2;
            // 
            // tbLanguageRestart
            // 
            this.tbLanguageRestart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLanguageRestart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbLanguageRestart.Location = new System.Drawing.Point(6, 19);
            this.tbLanguageRestart.Multiline = true;
            this.tbLanguageRestart.Name = "tbLanguageRestart";
            this.tbLanguageRestart.ReadOnly = true;
            this.tbLanguageRestart.Size = new System.Drawing.Size(297, 38);
            this.tbLanguageRestart.TabIndex = 4;
            this.tbLanguageRestart.TabStop = false;
            this.tbLanguageRestart.Tag = "OptionsWindow.LanguageRestart";
            this.tbLanguageRestart.Text = "Your father smells of elderberries!";
            // 
            // gbLanguage
            // 
            this.gbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLanguage.Controls.Add(this.tbLanguageRestart);
            this.gbLanguage.Controls.Add(this.cbLanguage);
            this.gbLanguage.Location = new System.Drawing.Point(318, 265);
            this.gbLanguage.Name = "gbLanguage";
            this.gbLanguage.Size = new System.Drawing.Size(309, 90);
            this.gbLanguage.TabIndex = 3;
            this.gbLanguage.TabStop = false;
            this.gbLanguage.Tag = "OptionsWindow.Language";
            this.gbLanguage.Text = "Lang it all anyway!";
            // 
            // lblSDLVideoDriver
            // 
            this.lblSDLVideoDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSDLVideoDriver.Location = new System.Drawing.Point(9, 94);
            this.lblSDLVideoDriver.Name = "lblSDLVideoDriver";
            this.lblSDLVideoDriver.Size = new System.Drawing.Size(100, 21);
            this.lblSDLVideoDriver.TabIndex = 0;
            this.lblSDLVideoDriver.Tag = "OptionsWindow.SDLHint.VideoDriver:";
            this.lblSDLVideoDriver.Text = "Driver";
            // 
            // cbSDLVideoDriver
            // 
            this.cbSDLVideoDriver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSDLVideoDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSDLVideoDriver.FormattingEnabled = true;
            this.cbSDLVideoDriver.Location = new System.Drawing.Point(110, 91);
            this.cbSDLVideoDriver.Name = "cbSDLVideoDriver";
            this.cbSDLVideoDriver.Size = new System.Drawing.Size(193, 21);
            this.cbSDLVideoDriver.TabIndex = 1;
            // 
            // tbSDLVideoRenderWarning
            // 
            this.tbSDLVideoRenderWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSDLVideoRenderWarning.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSDLVideoRenderWarning.Location = new System.Drawing.Point(6, 19);
            this.tbSDLVideoRenderWarning.Multiline = true;
            this.tbSDLVideoRenderWarning.Name = "tbSDLVideoRenderWarning";
            this.tbSDLVideoRenderWarning.ReadOnly = true;
            this.tbSDLVideoRenderWarning.Size = new System.Drawing.Size(297, 66);
            this.tbSDLVideoRenderWarning.TabIndex = 3;
            this.tbSDLVideoRenderWarning.TabStop = false;
            this.tbSDLVideoRenderWarning.Tag = "";
            this.tbSDLVideoRenderWarning.Text = "Your mother wears amry boots!";
            // 
            // gbSDLHints
            // 
            this.gbSDLHints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSDLHints.Controls.Add(this.tbSDLVideoRenderWarning);
            this.gbSDLHints.Controls.Add(this.cbSDLVideoDriver);
            this.gbSDLHints.Controls.Add(this.lblSDLVideoDriver);
            this.gbSDLHints.Location = new System.Drawing.Point(318, 138);
            this.gbSDLHints.Name = "gbSDLHints";
            this.gbSDLHints.Size = new System.Drawing.Size(309, 121);
            this.gbSDLHints.TabIndex = 2;
            this.gbSDLHints.TabStop = false;
            this.gbSDLHints.Tag = "OptionsWindow.SDLHint";
            this.gbSDLHints.Text = "SDL";
            // 
            // cbZipLogFiles
            // 
            this.cbZipLogFiles.Checked = true;
            this.cbZipLogFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbZipLogFiles.Location = new System.Drawing.Point(6, 42);
            this.cbZipLogFiles.Name = "cbZipLogFiles";
            this.cbZipLogFiles.Size = new System.Drawing.Size(297, 23);
            this.cbZipLogFiles.TabIndex = 4;
            this.cbZipLogFiles.Tag = "OptionsWindow.ZipLogFiles";
            this.cbZipLogFiles.Text = "Zippity-do-da";
            this.cbZipLogFiles.UseVisualStyleBackColor = true;
            // 
            // cbLogMainToConsole
            // 
            this.cbLogMainToConsole.Location = new System.Drawing.Point(6, 19);
            this.cbLogMainToConsole.Name = "cbLogMainToConsole";
            this.cbLogMainToConsole.Size = new System.Drawing.Size(297, 23);
            this.cbLogMainToConsole.TabIndex = 5;
            this.cbLogMainToConsole.Tag = "OptionsWindow.LogMainToConsole";
            this.cbLogMainToConsole.Text = "Console me";
            this.cbLogMainToConsole.UseVisualStyleBackColor = true;
            // 
            // gbLogs
            // 
            this.gbLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLogs.Controls.Add(this.cbLogMainToConsole);
            this.gbLogs.Controls.Add(this.cbZipLogFiles);
            this.gbLogs.Location = new System.Drawing.Point(318, 361);
            this.gbLogs.Name = "gbLogs";
            this.gbLogs.Size = new System.Drawing.Size(309, 70);
            this.gbLogs.TabIndex = 6;
            this.gbLogs.TabStop = false;
            this.gbLogs.Tag = "OptionsWindow.Logs";
            this.gbLogs.Text = "Logs";
            // 
            // gbNIFExportInfo
            // 
            this.gbNIFExportInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbNIFExportInfo.Controls.Add(this.btnNIFExportInfoReset);
            this.gbNIFExportInfo.Controls.Add(this.tbNIFExportInfo_3);
            this.gbNIFExportInfo.Controls.Add(this.tbNIFExportInfo_2);
            this.gbNIFExportInfo.Controls.Add(this.tbNIFExportInfo_1);
            this.gbNIFExportInfo.Controls.Add(this.tbNIFExportInfo_0);
            this.gbNIFExportInfo.Location = new System.Drawing.Point(3, 320);
            this.gbNIFExportInfo.Name = "gbNIFExportInfo";
            this.gbNIFExportInfo.Size = new System.Drawing.Size(309, 111);
            this.gbNIFExportInfo.TabIndex = 7;
            this.gbNIFExportInfo.TabStop = false;
            this.gbNIFExportInfo.Tag = "OptionsWindow.NIFExportInfo";
            this.gbNIFExportInfo.Text = "sNIF ei strings";
            // 
            // btnNIFExportInfoReset
            // 
            this.btnNIFExportInfoReset.Image = ((System.Drawing.Image)(resources.GetObject("btnNIFExportInfoReset.Image")));
            this.btnNIFExportInfoReset.Location = new System.Drawing.Point(285, 0);
            this.btnNIFExportInfoReset.Name = "btnNIFExportInfoReset";
            this.btnNIFExportInfoReset.Size = new System.Drawing.Size(18, 18);
            this.btnNIFExportInfoReset.TabIndex = 4;
            this.btnNIFExportInfoReset.UseVisualStyleBackColor = true;
            // 
            // tbNIFExportInfo_3
            // 
            this.tbNIFExportInfo_3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNIFExportInfo_3.Location = new System.Drawing.Point(6, 85);
            this.tbNIFExportInfo_3.MaxLength = 254;
            this.tbNIFExportInfo_3.Name = "tbNIFExportInfo_3";
            this.tbNIFExportInfo_3.Size = new System.Drawing.Size(297, 20);
            this.tbNIFExportInfo_3.TabIndex = 3;
            // 
            // tbNIFExportInfo_2
            // 
            this.tbNIFExportInfo_2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNIFExportInfo_2.Location = new System.Drawing.Point(6, 63);
            this.tbNIFExportInfo_2.MaxLength = 254;
            this.tbNIFExportInfo_2.Name = "tbNIFExportInfo_2";
            this.tbNIFExportInfo_2.Size = new System.Drawing.Size(297, 20);
            this.tbNIFExportInfo_2.TabIndex = 2;
            // 
            // tbNIFExportInfo_1
            // 
            this.tbNIFExportInfo_1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNIFExportInfo_1.Location = new System.Drawing.Point(6, 41);
            this.tbNIFExportInfo_1.MaxLength = 254;
            this.tbNIFExportInfo_1.Name = "tbNIFExportInfo_1";
            this.tbNIFExportInfo_1.Size = new System.Drawing.Size(297, 20);
            this.tbNIFExportInfo_1.TabIndex = 1;
            // 
            // tbNIFExportInfo_0
            // 
            this.tbNIFExportInfo_0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNIFExportInfo_0.Location = new System.Drawing.Point(6, 19);
            this.tbNIFExportInfo_0.MaxLength = 254;
            this.tbNIFExportInfo_0.Name = "tbNIFExportInfo_0";
            this.tbNIFExportInfo_0.Size = new System.Drawing.Size(297, 20);
            this.tbNIFExportInfo_0.TabIndex = 0;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 431);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(635, 455);
            this.Name = "Options";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "OptionsWindow.Title";
            this.Text = "Title";
            this.TopMost = true;
            this.WindowPanel.ResumeLayout(false);
            this.gbAlwaysSelectMasters.ResumeLayout(false);
            this.gbConflictStatus.ResumeLayout(false);
            this.gbConflictStatus.PerformLayout();
            this.gbLanguage.ResumeLayout(false);
            this.gbLanguage.PerformLayout();
            this.gbSDLHints.ResumeLayout(false);
            this.gbSDLHints.PerformLayout();
            this.gbLogs.ResumeLayout(false);
            this.gbNIFExportInfo.ResumeLayout(false);
            this.gbNIFExportInfo.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
