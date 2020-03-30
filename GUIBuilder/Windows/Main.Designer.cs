/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
	partial class Main
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;
		System.Windows.Forms.MenuStrip mbMain;
		System.Windows.Forms.ToolStripMenuItem mbiFile;
		System.Windows.Forms.ToolStripMenuItem mbiTools;
		System.Windows.Forms.StatusStrip sbMain;
		System.Windows.Forms.ToolStripStatusLabel sbiCaption;
		System.Windows.Forms.ToolStripSeparator mbiFileSeparator1;
		System.Windows.Forms.ToolStripMenuItem mbiFileExit;
		System.Windows.Forms.ToolStripMenuItem mbiToolsAbout;
		System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		System.Windows.Forms.ToolStripStatusLabel sbiItemOfItems;
		System.Windows.Forms.ToolStripMenuItem mbiFileLoadPlugin;
		System.Windows.Forms.ToolStripSeparator mbiFileSeparator2;
		System.Windows.Forms.ToolStripMenuItem mbiFileCloseFiles;
		System.Windows.Forms.ToolStripStatusLabel sbiTimeElapsed;
		System.Windows.Forms.ToolStripMenuItem mbiToolsRendererWindow;
		System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		System.Windows.Forms.ToolStripMenuItem mbiToolsBorderBatch;
		System.Windows.Forms.ToolStripMenuItem mbiFileSavePlugin;
		System.Windows.Forms.ToolStripMenuItem mbiToolsSubDivisionBatch;
		System.Windows.Forms.ToolStripMenuItem mbiToolsOptions;
		System.Windows.Forms.ToolStripStatusLabel sbiTimeEstimated;
		private System.Windows.Forms.ToolStripMenuItem mbiFileLoadWorkspace;
		private System.Windows.Forms.ToolStripMenuItem mbiFileCreateWorkspace;
		private System.Windows.Forms.ToolStripSeparator mbiFileSeparator3;
		private System.Windows.Forms.ToolStripMenuItem mbiToolsCustomForms;
		
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
            //System.Console.WriteLine( "GUIBuilder.Windows.Main.InitializeComponent()" );

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.sbMain = new System.Windows.Forms.StatusStrip();
            this.sbiTimeElapsed = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbiTimeEstimated = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbiCaption = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbiItemOfItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mbMain = new System.Windows.Forms.MenuStrip();
            this.mbiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileLoadWorkspace = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileCreateWorkspace = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mbiFileLoadPlugin = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileSavePlugin = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mbiFileCloseFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiFileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mbiFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiToolsSubDivisionBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiToolsBorderBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiToolsRendererWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mbiToolsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mbiToolsCustomForms = new System.Windows.Forms.ToolStripMenuItem();
            this.sbMain.SuspendLayout();
            this.mbMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // sbMain
            // 
            this.sbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbiTimeElapsed,
            this.sbiTimeEstimated,
            this.sbiCaption,
            this.sbiItemOfItems,
            this.toolStripStatusLabel1});
            this.sbMain.Location = new System.Drawing.Point(0, 20);
            this.sbMain.Name = "sbMain";
            this.sbMain.Size = new System.Drawing.Size(692, 22);
            this.sbMain.TabIndex = 1;
            this.sbMain.Text = "statusStrip1";
            // 
            // sbiTimeElapsed
            // 
            this.sbiTimeElapsed.AutoSize = false;
            this.sbiTimeElapsed.Name = "sbiTimeElapsed";
            this.sbiTimeElapsed.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.sbiTimeElapsed.Size = new System.Drawing.Size(30, 17);
            this.sbiTimeElapsed.Text = "00:00";
            // 
            // sbiTimeEstimated
            // 
            this.sbiTimeEstimated.AutoSize = false;
            this.sbiTimeEstimated.Name = "sbiTimeEstimated";
            this.sbiTimeEstimated.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.sbiTimeEstimated.Size = new System.Drawing.Size(40, 17);
            this.sbiTimeEstimated.Text = "/ 00:00";
            // 
            // sbiCaption
            // 
            this.sbiCaption.AutoToolTip = true;
            this.sbiCaption.Name = "sbiCaption";
            this.sbiCaption.Size = new System.Drawing.Size(536, 17);
            this.sbiCaption.Spring = true;
            this.sbiCaption.Text = "Operation being performed la la la, this is really long to test, la la la";
            this.sbiCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sbiItemOfItems
            // 
            this.sbiItemOfItems.Name = "sbiItemOfItems";
            this.sbiItemOfItems.Size = new System.Drawing.Size(71, 17);
            this.sbiItemOfItems.Text = "99999/99999";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // mbMain
            // 
            this.mbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiFile,
            this.mbiTools});
            this.mbMain.Location = new System.Drawing.Point(0, 0);
            this.mbMain.Name = "mbMain";
            this.mbMain.Size = new System.Drawing.Size(692, 24);
            this.mbMain.TabIndex = 0;
            this.mbMain.Text = "menuStrip1";
            // 
            // mbiFile
            // 
            this.mbiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiFileLoadWorkspace,
            this.mbiFileCreateWorkspace,
            this.mbiFileSeparator1,
            this.mbiFileLoadPlugin,
            this.mbiFileSavePlugin,
            this.mbiFileSeparator2,
            this.mbiFileCloseFiles,
            this.mbiFileSeparator3,
            this.mbiFileExit});
            this.mbiFile.Name = "mbiFile";
            this.mbiFile.Size = new System.Drawing.Size(35, 20);
            this.mbiFile.Tag = "MainWindow.FileMenu";
            this.mbiFile.Text = "File";
            // 
            // mbiFileLoadWorkspace
            // 
            this.mbiFileLoadWorkspace.Name = "mbiFileLoadWorkspace";
            this.mbiFileLoadWorkspace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.mbiFileLoadWorkspace.Size = new System.Drawing.Size(202, 22);
            this.mbiFileLoadWorkspace.Tag = "MainWindow.FileMenu.LoadWorkspace";
            this.mbiFileLoadWorkspace.Text = "Load Workspace";
            this.mbiFileLoadWorkspace.Click += new System.EventHandler(this.mbiFileLoadWorkspaceClick);
            // 
            // mbiFileCreateWorkspace
            // 
            this.mbiFileCreateWorkspace.Enabled = false;
            this.mbiFileCreateWorkspace.Name = "mbiFileCreateWorkspace";
            this.mbiFileCreateWorkspace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mbiFileCreateWorkspace.Size = new System.Drawing.Size(202, 22);
            this.mbiFileCreateWorkspace.Tag = "MainWindow.FileMenu.CreateWorkspace";
            this.mbiFileCreateWorkspace.Text = "Create Workspace";
            this.mbiFileCreateWorkspace.Click += new System.EventHandler(this.mbiFileCreateWorkspaceClick);
            // 
            // mbiFileSeparator1
            // 
            this.mbiFileSeparator1.Name = "mbiFileSeparator1";
            this.mbiFileSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // mbiFileLoadPlugin
            // 
            this.mbiFileLoadPlugin.Name = "mbiFileLoadPlugin";
            this.mbiFileLoadPlugin.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.mbiFileLoadPlugin.Size = new System.Drawing.Size(202, 22);
            this.mbiFileLoadPlugin.Tag = "MainWindow.FileMenu.LoadPlugin";
            this.mbiFileLoadPlugin.Text = "Load Plugin";
            this.mbiFileLoadPlugin.Click += new System.EventHandler(this.mbiFileLoadPluginClick);
            // 
            // mbiFileSavePlugin
            // 
            this.mbiFileSavePlugin.Enabled = false;
            this.mbiFileSavePlugin.Name = "mbiFileSavePlugin";
            this.mbiFileSavePlugin.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mbiFileSavePlugin.Size = new System.Drawing.Size(202, 22);
            this.mbiFileSavePlugin.Tag = "MainWindow.FileMenu.SavePlugin";
            this.mbiFileSavePlugin.Text = "Save Plugin";
            this.mbiFileSavePlugin.Click += new System.EventHandler(this.mbiFileSavePluginClick);
            // 
            // mbiFileSeparator2
            // 
            this.mbiFileSeparator2.Name = "mbiFileSeparator2";
            this.mbiFileSeparator2.Size = new System.Drawing.Size(199, 6);
            // 
            // mbiFileCloseFiles
            // 
            this.mbiFileCloseFiles.Enabled = false;
            this.mbiFileCloseFiles.Name = "mbiFileCloseFiles";
            this.mbiFileCloseFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mbiFileCloseFiles.Size = new System.Drawing.Size(202, 22);
            this.mbiFileCloseFiles.Tag = "MainWindow.FileMenu.CloseAllFiles";
            this.mbiFileCloseFiles.Text = "Close";
            this.mbiFileCloseFiles.Click += new System.EventHandler(this.mbiFileCloseFilesClick);
            // 
            // mbiFileSeparator3
            // 
            this.mbiFileSeparator3.Name = "mbiFileSeparator3";
            this.mbiFileSeparator3.Size = new System.Drawing.Size(199, 6);
            // 
            // mbiFileExit
            // 
            this.mbiFileExit.Name = "mbiFileExit";
            this.mbiFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.mbiFileExit.Size = new System.Drawing.Size(202, 22);
            this.mbiFileExit.Tag = "MainWindow.FileMenu.Exit";
            this.mbiFileExit.Text = "Exit";
            this.mbiFileExit.Click += new System.EventHandler(this.OnMenuExitClick);
            // 
            // mbiTools
            // 
            this.mbiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiToolsSubDivisionBatch,
            this.mbiToolsBorderBatch,
            this.mbiToolsRendererWindow,
            this.toolStripSeparator1,
            this.mbiToolsAbout,
            this.mbiToolsOptions,
            this.mbiToolsCustomForms});
            this.mbiTools.Name = "mbiTools";
            this.mbiTools.Size = new System.Drawing.Size(44, 20);
            this.mbiTools.Tag = "MainWindow.ToolMenu";
            this.mbiTools.Text = "Tools";
            // 
            // mbiToolsSubDivisionBatch
            // 
            this.mbiToolsSubDivisionBatch.Enabled = false;
            this.mbiToolsSubDivisionBatch.Name = "mbiToolsSubDivisionBatch";
            this.mbiToolsSubDivisionBatch.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsSubDivisionBatch.Tag = "MainWindow.ToolMenu.SubDivisions";
            this.mbiToolsSubDivisionBatch.Text = "SubDivisions";
            this.mbiToolsSubDivisionBatch.Click += new System.EventHandler(this.mbiToolsSubDivisionBatchClick);
            // 
            // mbiToolsBorderBatch
            // 
            this.mbiToolsBorderBatch.Enabled = false;
            this.mbiToolsBorderBatch.Name = "mbiToolsBorderBatch";
            this.mbiToolsBorderBatch.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsBorderBatch.Tag = "MainWindow.ToolMenu.Borders";
            this.mbiToolsBorderBatch.Text = "Borders";
            this.mbiToolsBorderBatch.Click += new System.EventHandler(this.mbiWindowsBorderBatchClick);
            // 
            // mbiToolsRendererWindow
            // 
            this.mbiToolsRendererWindow.Enabled = false;
            this.mbiToolsRendererWindow.Name = "mbiToolsRendererWindow";
            this.mbiToolsRendererWindow.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsRendererWindow.Tag = "MainWindow.ToolMenu.Render";
            this.mbiToolsRendererWindow.Text = "Render";
            this.mbiToolsRendererWindow.Click += new System.EventHandler(this.mbiWindowsRendererClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // mbiToolsAbout
            // 
            this.mbiToolsAbout.Name = "mbiToolsAbout";
            this.mbiToolsAbout.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsAbout.Tag = "MainWindow.ToolMenu.About";
            this.mbiToolsAbout.Text = "About";
            this.mbiToolsAbout.Click += new System.EventHandler(this.mbiWindowsAboutClick);
            // 
            // mbiToolsOptions
            // 
            this.mbiToolsOptions.Name = "mbiToolsOptions";
            this.mbiToolsOptions.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsOptions.Tag = "MainWindow.ToolMenu.Options";
            this.mbiToolsOptions.Text = "Options";
            this.mbiToolsOptions.Click += new System.EventHandler(this.mbiToolsOptionsClick);
            // 
            // mbiToolsCustomForms
            // 
            this.mbiToolsCustomForms.Name = "mbiToolsCustomForms";
            this.mbiToolsCustomForms.Size = new System.Drawing.Size(142, 22);
            this.mbiToolsCustomForms.Tag = "MainWindow.ToolMenu.CustomForms";
            this.mbiToolsCustomForms.Text = "Custom Forms";
            this.mbiToolsCustomForms.Click += new System.EventHandler(this.mbiToolsCustomFormsClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 42);
            this.Controls.Add( this.sbMain );
            this.Controls.Add( this.mbMain );
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mbMain;
            this.MinimumSize = new System.Drawing.Size(700, 38);
            this.Name = "Main";
            this.Tag = "MainWindow.Title";
            this.Text = "Title";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnFormClosing );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.sbMain.ResumeLayout( false );
            this.mbMain.ResumeLayout( false );
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
