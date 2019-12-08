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
		System.Windows.Forms.ToolStripMenuItem mbiExit;
		System.Windows.Forms.ToolStripMenuItem mbiToolsAbout;
		System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		System.Windows.Forms.ToolStripStatusLabel sbiItemOfItems;
		System.Windows.Forms.ToolStripMenuItem mbiLoadESMESP;
		System.Windows.Forms.ToolStripSeparator mbiFileSeparator2;
		System.Windows.Forms.ToolStripMenuItem mbiCloseFiles;
		System.Windows.Forms.ToolStripStatusLabel sbiTimeElapsed;
		System.Windows.Forms.ToolStripMenuItem mbiToolsRendererWindow;
		System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		System.Windows.Forms.ToolStripMenuItem mbiToolsBorderBatch;
		System.Windows.Forms.ToolStripMenuItem mbiFileSave;
		System.Windows.Forms.ToolStripMenuItem mbiToolsSubDivisionBatch;
		System.Windows.Forms.ToolStripMenuItem mbiToolsOptions;
		System.Windows.Forms.ToolStripStatusLabel sbiTimeEstimated;
		
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
		    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
		    this.mbMain = new System.Windows.Forms.MenuStrip();
		    this.mbiFile = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiLoadESMESP = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiFileSave = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		    this.mbiCloseFiles = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		    this.mbiExit = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiTools = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiToolsSubDivisionBatch = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiToolsBorderBatch = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiToolsRendererWindow = new System.Windows.Forms.ToolStripMenuItem();
		    this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		    this.mbiToolsAbout = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
		    this.sbMain = new System.Windows.Forms.StatusStrip();
		    this.sbiTimeElapsed = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiTimeEstimated = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiCaption = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiItemOfItems = new System.Windows.Forms.ToolStripStatusLabel();
		    this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
		    this.mbMain.SuspendLayout();
		    this.sbMain.SuspendLayout();
		    this.SuspendLayout();
		    // 
		    // mbMain
		    // 
		    this.mbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiFile,
            this.mbiTools});
		    this.mbMain.Location = new System.Drawing.Point(0, 0);
		    this.mbMain.Name = "mbMain";
		    this.mbMain.Size = new System.Drawing.Size(684, 24);
		    this.mbMain.TabIndex = 0;
		    this.mbMain.Text = "menuStrip1";
		    // 
		    // mbiFile
		    // 
		    this.mbiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiLoadESMESP,
            this.mbiFileSave,
            this.mbiFileSeparator1,
            this.mbiCloseFiles,
            this.mbiFileSeparator2,
            this.mbiExit});
		    this.mbiFile.Name = "mbiFile";
		    this.mbiFile.Size = new System.Drawing.Size(37, 20);
		    this.mbiFile.Tag = "MainWindow.FileMenu";
		    this.mbiFile.Text = "File";
		    // 
		    // mbiLoadESMESP
		    // 
		    this.mbiLoadESMESP.Name = "mbiLoadESMESP";
		    this.mbiLoadESMESP.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
		    this.mbiLoadESMESP.Size = new System.Drawing.Size(145, 22);
		    this.mbiLoadESMESP.Tag = "MainWindow.FileMenu.Load";
		    this.mbiLoadESMESP.Text = "Load";
		    this.mbiLoadESMESP.Click += new System.EventHandler(this.OnMenuLoadESMESPClick);
		    // 
		    // mbiFileSave
		    // 
		    this.mbiFileSave.Name = "mbiFileSave";
		    this.mbiFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
		    this.mbiFileSave.Size = new System.Drawing.Size(145, 22);
		    this.mbiFileSave.Tag = "MainWindow.FileMenu.Save";
		    this.mbiFileSave.Text = "Save";
		    this.mbiFileSave.Click += new System.EventHandler(this.OnMenuFileSaveClick);
		    // 
		    // mbiFileSeparator1
		    // 
		    this.mbiFileSeparator1.Name = "mbiFileSeparator1";
		    this.mbiFileSeparator1.Size = new System.Drawing.Size(142, 6);
		    // 
		    // mbiCloseFiles
		    // 
		    this.mbiCloseFiles.Name = "mbiCloseFiles";
		    this.mbiCloseFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
		    this.mbiCloseFiles.Size = new System.Drawing.Size(145, 22);
		    this.mbiCloseFiles.Tag = "MainWindow.FileMenu.Close";
		    this.mbiCloseFiles.Text = "Close";
		    this.mbiCloseFiles.Click += new System.EventHandler(this.OnMenuCloseFileClick);
		    // 
		    // mbiFileSeparator2
		    // 
		    this.mbiFileSeparator2.Name = "mbiFileSeparator2";
		    this.mbiFileSeparator2.Size = new System.Drawing.Size(142, 6);
		    // 
		    // mbiExit
		    // 
		    this.mbiExit.Name = "mbiExit";
		    this.mbiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
		    this.mbiExit.Size = new System.Drawing.Size(145, 22);
		    this.mbiExit.Tag = "MainWindow.FileMenu.Exit";
		    this.mbiExit.Text = "Exit";
		    this.mbiExit.Click += new System.EventHandler(this.OnMenuExitClick);
		    // 
		    // mbiTools
		    // 
		    this.mbiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiToolsSubDivisionBatch,
            this.mbiToolsBorderBatch,
            this.mbiToolsRendererWindow,
            this.toolStripSeparator1,
            this.mbiToolsAbout,
            this.mbiToolsOptions});
		    this.mbiTools.Name = "mbiTools";
		    this.mbiTools.Size = new System.Drawing.Size(48, 20);
		    this.mbiTools.Tag = "MainWindow.ToolMenu";
		    this.mbiTools.Text = "Tools";
		    // 
		    // mbiToolsSubDivisionBatch
		    // 
		    this.mbiToolsSubDivisionBatch.Enabled = false;
		    this.mbiToolsSubDivisionBatch.Name = "mbiToolsSubDivisionBatch";
		    this.mbiToolsSubDivisionBatch.Size = new System.Drawing.Size(141, 22);
		    this.mbiToolsSubDivisionBatch.Tag = "MainWindow.ToolMenu.SubDivisions";
		    this.mbiToolsSubDivisionBatch.Text = "SubDivisions";
		    this.mbiToolsSubDivisionBatch.Click += new System.EventHandler(this.mbiToolsSubDivisionBatchClick);
		    // 
		    // mbiToolsBorderBatch
		    // 
		    this.mbiToolsBorderBatch.Enabled = false;
		    this.mbiToolsBorderBatch.Name = "mbiToolsBorderBatch";
		    this.mbiToolsBorderBatch.Size = new System.Drawing.Size(141, 22);
		    this.mbiToolsBorderBatch.Tag = "MainWindow.ToolMenu.Borders";
		    this.mbiToolsBorderBatch.Text = "Borders";
		    this.mbiToolsBorderBatch.Click += new System.EventHandler(this.mbiWindowsBorderBatchClick);
		    // 
		    // mbiToolsRendererWindow
		    // 
		    this.mbiToolsRendererWindow.Enabled = false;
		    this.mbiToolsRendererWindow.Name = "mbiToolsRendererWindow";
		    this.mbiToolsRendererWindow.Size = new System.Drawing.Size(141, 22);
		    this.mbiToolsRendererWindow.Tag = "MainWindow.ToolMenu.Render";
		    this.mbiToolsRendererWindow.Text = "Render";
		    this.mbiToolsRendererWindow.Click += new System.EventHandler(this.mbiWindowsRendererClick);
		    // 
		    // toolStripSeparator1
		    // 
		    this.toolStripSeparator1.Name = "toolStripSeparator1";
		    this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
		    // 
		    // mbiToolsAbout
		    // 
		    this.mbiToolsAbout.Name = "mbiToolsAbout";
		    this.mbiToolsAbout.Size = new System.Drawing.Size(141, 22);
		    this.mbiToolsAbout.Tag = "MainWindow.ToolMenu.About";
		    this.mbiToolsAbout.Text = "About";
		    this.mbiToolsAbout.Click += new System.EventHandler(this.mbiWindowsAboutClick);
		    // 
		    // mbiToolsOptions
		    // 
		    this.mbiToolsOptions.Name = "mbiToolsOptions";
		    this.mbiToolsOptions.Size = new System.Drawing.Size(141, 22);
		    this.mbiToolsOptions.Tag = "MainWindow.ToolMenu.Options";
		    this.mbiToolsOptions.Text = "Options";
		    this.mbiToolsOptions.Click += new System.EventHandler(this.mbiToolsOptionsClick);
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
		    this.sbMain.Size = new System.Drawing.Size(684, 22);
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
		    this.sbiCaption.Size = new System.Drawing.Size(527, 17);
		    this.sbiCaption.Spring = true;
		    this.sbiCaption.Text = "Operation being performed la la la, this is really long to test, la la la";
		    this.sbiCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		    // 
		    // sbiItemOfItems
		    // 
		    this.sbiItemOfItems.Name = "sbiItemOfItems";
		    this.sbiItemOfItems.Size = new System.Drawing.Size(72, 17);
		    this.sbiItemOfItems.Text = "99999/99999";
		    // 
		    // toolStripStatusLabel1
		    // 
		    this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
		    this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
		    // 
		    // Main
		    // 
		    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.ClientSize = new System.Drawing.Size(684, 42);
		    this.Controls.Add(this.sbMain);
		    this.Controls.Add(this.mbMain);
		    this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
		    this.MainMenuStrip = this.mbMain;
		    this.MinimumSize = new System.Drawing.Size(700, 38);
		    this.Name = "Main";
		    this.Tag = "MainWindow.Title";
		    this.Text = "Title";
		    this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
		    this.Load += new System.EventHandler(this.OnFormLoad);
		    this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
		    this.Move += new System.EventHandler(this.OnFormMove);
		    this.mbMain.ResumeLayout(false);
		    this.mbMain.PerformLayout();
		    this.sbMain.ResumeLayout(false);
		    this.sbMain.PerformLayout();
		    this.ResumeLayout(false);
		    this.PerformLayout();

		}
	}
}
