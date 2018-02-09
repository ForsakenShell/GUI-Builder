/*
 * Created by SharpDevelop.
 * User: Eric Cowles
 * Date: 24/11/2017
 * Time: 10:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Border_Builder
{
	partial class fMain
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.MenuStrip mbMain;
		private System.Windows.Forms.ToolStripMenuItem mbiFile;
		private System.Windows.Forms.ToolStripMenuItem mbiBuild;
		private System.Windows.Forms.ToolStripMenuItem mbiHelp;
		private System.Windows.Forms.StatusStrip sbMain;
		private System.Windows.Forms.ToolStripProgressBar sbiProgress;
		private System.Windows.Forms.ToolStripStatusLabel sbiCaption;
		private System.Windows.Forms.ToolStripMenuItem mbiImportWorldspace;
		private System.Windows.Forms.ToolStripSeparator mbiFileSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mbiExit;
		private System.Windows.Forms.GroupBox gbWorldspace;
		private System.Windows.Forms.TextBox tbWorldspaceFormIDEditorID;
		private System.Windows.Forms.Label lWorldspaceFormID;
		private System.Windows.Forms.ComboBox cbWorldspace;
		private System.Windows.Forms.GroupBox gbWorldspaceGridRange;
		private System.Windows.Forms.TextBox tbWorldspaceGridBottomX;
		private System.Windows.Forms.TextBox tbWorldspaceGridBottomY;
		private System.Windows.Forms.Label lWorldspaceGridBRComma;
		private System.Windows.Forms.TextBox tbWorldspaceGridTopY;
		private System.Windows.Forms.TextBox tbWorldspaceGridTopX;
		private System.Windows.Forms.Label lWorldspaceGridTLComma;
		private System.Windows.Forms.GroupBox gbWorldspaceGridRangeIndicator;
		private System.Windows.Forms.GroupBox gbWorldspaceTextures;
		private System.Windows.Forms.TextBox tbWorldspaceWaterHeightsTexture;
		private System.Windows.Forms.TextBox tbWorldspaceHeightmapTexture;
		private System.Windows.Forms.GroupBox gbWorldspaceMapHeightRange;
		private System.Windows.Forms.Label lWorldspaceMapHeightMax;
		private System.Windows.Forms.Label lWorldspaceMapHeightMin;
		private System.Windows.Forms.TextBox tbWorldspaceMapHeightMax;
		private System.Windows.Forms.TextBox tbWorldspaceMapHeightMin;
		private System.Windows.Forms.Button btnLoadWorldspaceHeightTextures;
		private System.Windows.Forms.GroupBox gbSourceMod;
		private System.Windows.Forms.ComboBox cbImportMod;
		private System.Windows.Forms.Button btnLoadImportModBuildVolumes;
		private System.Windows.Forms.PictureBox pbRenderWindow;
		private System.Windows.Forms.Panel pnRenderWindow;
		private System.Windows.Forms.Button btnCellWindowRedraw;
		private System.Windows.Forms.GroupBox gbRenderOptions;
		private System.Windows.Forms.CheckBox cbRenderCellGrid;
		private System.Windows.Forms.CheckBox cbRenderWaterHeight;
		private System.Windows.Forms.CheckBox cbRenderLandHeight;
		private System.Windows.Forms.CheckBox cbRenderBuildVolumes;
		private System.Windows.Forms.CheckBox cbRenderBorders;
		private System.Windows.Forms.Button btnBuildImportModVolumeBorders;
		private System.Windows.Forms.ToolStripMenuItem mbiImportMod;
		private System.Windows.Forms.ToolStripMenuItem mbiBuildVolumeBorders;
		private System.Windows.Forms.ToolStripMenuItem mbiHelpAbout;
		private System.Windows.Forms.GroupBox gbRenderSelectedOnly;
		private System.Windows.Forms.CheckBox cbRenderSelectedOnly;
		private System.Windows.Forms.CheckBox cbRenderWorldspaceHeightTextures;
		private System.Windows.Forms.Button btnWeldImportVolumeVerts;
		private System.Windows.Forms.GroupBox gbWeldThreshold;
		private System.Windows.Forms.TextBox tbWeldThreshold;
		private System.Windows.Forms.CheckBox cbExportPNG;
		private System.Windows.Forms.CheckBox cbRenderOverRegion;
		private System.Windows.Forms.ListBox lbVolumeParents;
		private System.Windows.Forms.ToolStripStatusLabel sbiMouseToCellGrid;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel sbiMouseToWorldspace;
		private System.Windows.Forms.GroupBox gbEditOptions;
		private System.Windows.Forms.TextBox tbEMHotKeys;
		private System.Windows.Forms.CheckBox cbEditModeEnable;
		private System.Windows.Forms.ToolStripStatusLabel sbiEditorSelectionMode;
		private System.Windows.Forms.CheckBox cbWeldAllTogether;
		private System.Windows.Forms.Label lblWeldThreshold;
		
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
		private void InitializeComponent()
		{
		    this.mbMain = new System.Windows.Forms.MenuStrip();
		    this.mbiFile = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiImportWorldspace = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiImportMod = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		    this.mbiExit = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiBuild = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiBuildVolumeBorders = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiHelp = new System.Windows.Forms.ToolStripMenuItem();
		    this.mbiHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
		    this.sbMain = new System.Windows.Forms.StatusStrip();
		    this.sbiProgress = new System.Windows.Forms.ToolStripProgressBar();
		    this.sbiCaption = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiEditorSelectionMode = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiMouseToCellGrid = new System.Windows.Forms.ToolStripStatusLabel();
		    this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
		    this.sbiMouseToWorldspace = new System.Windows.Forms.ToolStripStatusLabel();
		    this.gbWorldspace = new System.Windows.Forms.GroupBox();
		    this.cbRenderWorldspaceHeightTextures = new System.Windows.Forms.CheckBox();
		    this.btnLoadWorldspaceHeightTextures = new System.Windows.Forms.Button();
		    this.gbWorldspaceMapHeightRange = new System.Windows.Forms.GroupBox();
		    this.lWorldspaceMapHeightMax = new System.Windows.Forms.Label();
		    this.lWorldspaceMapHeightMin = new System.Windows.Forms.Label();
		    this.tbWorldspaceMapHeightMax = new System.Windows.Forms.TextBox();
		    this.tbWorldspaceMapHeightMin = new System.Windows.Forms.TextBox();
		    this.gbWorldspaceTextures = new System.Windows.Forms.GroupBox();
		    this.tbWorldspaceWaterHeightsTexture = new System.Windows.Forms.TextBox();
		    this.tbWorldspaceHeightmapTexture = new System.Windows.Forms.TextBox();
		    this.tbWorldspaceFormIDEditorID = new System.Windows.Forms.TextBox();
		    this.lWorldspaceFormID = new System.Windows.Forms.Label();
		    this.cbWorldspace = new System.Windows.Forms.ComboBox();
		    this.gbWorldspaceGridRange = new System.Windows.Forms.GroupBox();
		    this.tbWorldspaceGridBottomX = new System.Windows.Forms.TextBox();
		    this.tbWorldspaceGridBottomY = new System.Windows.Forms.TextBox();
		    this.lWorldspaceGridBRComma = new System.Windows.Forms.Label();
		    this.tbWorldspaceGridTopY = new System.Windows.Forms.TextBox();
		    this.tbWorldspaceGridTopX = new System.Windows.Forms.TextBox();
		    this.lWorldspaceGridTLComma = new System.Windows.Forms.Label();
		    this.gbWorldspaceGridRangeIndicator = new System.Windows.Forms.GroupBox();
		    this.gbSourceMod = new System.Windows.Forms.GroupBox();
		    this.gbWeldThreshold = new System.Windows.Forms.GroupBox();
		    this.cbWeldAllTogether = new System.Windows.Forms.CheckBox();
		    this.lblWeldThreshold = new System.Windows.Forms.Label();
		    this.tbWeldThreshold = new System.Windows.Forms.TextBox();
		    this.btnWeldImportVolumeVerts = new System.Windows.Forms.Button();
		    this.btnBuildImportModVolumeBorders = new System.Windows.Forms.Button();
		    this.btnLoadImportModBuildVolumes = new System.Windows.Forms.Button();
		    this.cbImportMod = new System.Windows.Forms.ComboBox();
		    this.pbRenderWindow = new System.Windows.Forms.PictureBox();
		    this.pnRenderWindow = new System.Windows.Forms.Panel();
		    this.btnCellWindowRedraw = new System.Windows.Forms.Button();
		    this.gbRenderOptions = new System.Windows.Forms.GroupBox();
		    this.cbRenderOverRegion = new System.Windows.Forms.CheckBox();
		    this.cbExportPNG = new System.Windows.Forms.CheckBox();
		    this.gbRenderSelectedOnly = new System.Windows.Forms.GroupBox();
		    this.lbVolumeParents = new System.Windows.Forms.ListBox();
		    this.cbRenderSelectedOnly = new System.Windows.Forms.CheckBox();
		    this.cbRenderBorders = new System.Windows.Forms.CheckBox();
		    this.cbRenderBuildVolumes = new System.Windows.Forms.CheckBox();
		    this.cbRenderCellGrid = new System.Windows.Forms.CheckBox();
		    this.cbRenderWaterHeight = new System.Windows.Forms.CheckBox();
		    this.cbRenderLandHeight = new System.Windows.Forms.CheckBox();
		    this.gbEditOptions = new System.Windows.Forms.GroupBox();
		    this.cbEditModeEnable = new System.Windows.Forms.CheckBox();
		    this.tbEMHotKeys = new System.Windows.Forms.TextBox();
		    this.mbMain.SuspendLayout();
		    this.sbMain.SuspendLayout();
		    this.gbWorldspace.SuspendLayout();
		    this.gbWorldspaceMapHeightRange.SuspendLayout();
		    this.gbWorldspaceTextures.SuspendLayout();
		    this.gbWorldspaceGridRange.SuspendLayout();
		    this.gbSourceMod.SuspendLayout();
		    this.gbWeldThreshold.SuspendLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.pbRenderWindow)).BeginInit();
		    this.pnRenderWindow.SuspendLayout();
		    this.gbRenderOptions.SuspendLayout();
		    this.gbRenderSelectedOnly.SuspendLayout();
		    this.gbEditOptions.SuspendLayout();
		    this.SuspendLayout();
		    // 
		    // mbMain
		    // 
		    this.mbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiFile,
            this.mbiBuild,
            this.mbiHelp});
		    this.mbMain.Location = new System.Drawing.Point(0, 0);
		    this.mbMain.Name = "mbMain";
		    this.mbMain.Size = new System.Drawing.Size(754, 24);
		    this.mbMain.TabIndex = 0;
		    this.mbMain.Text = "menuStrip1";
		    // 
		    // mbiFile
		    // 
		    this.mbiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiImportWorldspace,
            this.mbiImportMod,
            this.mbiFileSeparator1,
            this.mbiExit});
		    this.mbiFile.Name = "mbiFile";
		    this.mbiFile.Size = new System.Drawing.Size(37, 20);
		    this.mbiFile.Text = "File";
		    // 
		    // mbiImportWorldspace
		    // 
		    this.mbiImportWorldspace.Name = "mbiImportWorldspace";
		    this.mbiImportWorldspace.ShortcutKeyDisplayString = "Ctrl+W";
		    this.mbiImportWorldspace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
		    this.mbiImportWorldspace.Size = new System.Drawing.Size(220, 22);
		    this.mbiImportWorldspace.Text = "Import Worldspace";
		    // 
		    // mbiImportMod
		    // 
		    this.mbiImportMod.Name = "mbiImportMod";
		    this.mbiImportMod.ShortcutKeyDisplayString = "Ctrl+M";
		    this.mbiImportMod.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
		    this.mbiImportMod.Size = new System.Drawing.Size(220, 22);
		    this.mbiImportMod.Text = "Import Mod";
		    // 
		    // mbiFileSeparator1
		    // 
		    this.mbiFileSeparator1.Name = "mbiFileSeparator1";
		    this.mbiFileSeparator1.Size = new System.Drawing.Size(217, 6);
		    // 
		    // mbiExit
		    // 
		    this.mbiExit.Name = "mbiExit";
		    this.mbiExit.Size = new System.Drawing.Size(220, 22);
		    this.mbiExit.Text = "Exit";
		    this.mbiExit.Click += new System.EventHandler(this.MenuExitClick);
		    // 
		    // mbiBuild
		    // 
		    this.mbiBuild.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiBuildVolumeBorders});
		    this.mbiBuild.Name = "mbiBuild";
		    this.mbiBuild.Size = new System.Drawing.Size(46, 20);
		    this.mbiBuild.Text = "Build";
		    // 
		    // mbiBuildVolumeBorders
		    // 
		    this.mbiBuildVolumeBorders.Name = "mbiBuildVolumeBorders";
		    this.mbiBuildVolumeBorders.ShortcutKeyDisplayString = "Ctrl+B";
		    this.mbiBuildVolumeBorders.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
		    this.mbiBuildVolumeBorders.Size = new System.Drawing.Size(199, 22);
		    this.mbiBuildVolumeBorders.Text = "Volume Borders";
		    this.mbiBuildVolumeBorders.Click += new System.EventHandler(this.VolumeBordersToolStripMenuItemClick);
		    // 
		    // mbiHelp
		    // 
		    this.mbiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbiHelpAbout});
		    this.mbiHelp.Name = "mbiHelp";
		    this.mbiHelp.Size = new System.Drawing.Size(44, 20);
		    this.mbiHelp.Text = "Help";
		    // 
		    // mbiHelpAbout
		    // 
		    this.mbiHelpAbout.Name = "mbiHelpAbout";
		    this.mbiHelpAbout.Size = new System.Drawing.Size(107, 22);
		    this.mbiHelpAbout.Text = "About";
		    this.mbiHelpAbout.Click += new System.EventHandler(this.MbiHelpAboutClick);
		    // 
		    // sbMain
		    // 
		    this.sbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbiProgress,
            this.sbiCaption,
            this.sbiEditorSelectionMode,
            this.sbiMouseToCellGrid,
            this.toolStripStatusLabel1,
            this.sbiMouseToWorldspace});
		    this.sbMain.Location = new System.Drawing.Point(0, 647);
		    this.sbMain.Name = "sbMain";
		    this.sbMain.Size = new System.Drawing.Size(754, 22);
		    this.sbMain.TabIndex = 1;
		    this.sbMain.Text = "statusStrip1";
		    // 
		    // sbiProgress
		    // 
		    this.sbiProgress.Name = "sbiProgress";
		    this.sbiProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		    this.sbiProgress.Size = new System.Drawing.Size(100, 16);
		    // 
		    // sbiCaption
		    // 
		    this.sbiCaption.AutoToolTip = true;
		    this.sbiCaption.Name = "sbiCaption";
		    this.sbiCaption.Size = new System.Drawing.Size(413, 17);
		    this.sbiCaption.Spring = true;
		    this.sbiCaption.Text = "Operation being performed la la la, this is really long to test, la la la";
		    this.sbiCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		    // 
		    // sbiEditorSelectionMode
		    // 
		    this.sbiEditorSelectionMode.AutoSize = false;
		    this.sbiEditorSelectionMode.Name = "sbiEditorSelectionMode";
		    this.sbiEditorSelectionMode.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		    this.sbiEditorSelectionMode.Size = new System.Drawing.Size(48, 17);
		    // 
		    // sbiMouseToCellGrid
		    // 
		    this.sbiMouseToCellGrid.AutoSize = false;
		    this.sbiMouseToCellGrid.Name = "sbiMouseToCellGrid";
		    this.sbiMouseToCellGrid.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		    this.sbiMouseToCellGrid.Size = new System.Drawing.Size(64, 17);
		    this.sbiMouseToCellGrid.Text = "-99,-99";
		    // 
		    // toolStripStatusLabel1
		    // 
		    this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
		    this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
		    // 
		    // sbiMouseToWorldspace
		    // 
		    this.sbiMouseToWorldspace.AutoSize = false;
		    this.sbiMouseToWorldspace.Name = "sbiMouseToWorldspace";
		    this.sbiMouseToWorldspace.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		    this.sbiMouseToWorldspace.Size = new System.Drawing.Size(112, 17);
		    this.sbiMouseToWorldspace.Text = "(-123456,-123456)";
		    // 
		    // gbWorldspace
		    // 
		    this.gbWorldspace.Controls.Add(this.cbRenderWorldspaceHeightTextures);
		    this.gbWorldspace.Controls.Add(this.btnLoadWorldspaceHeightTextures);
		    this.gbWorldspace.Controls.Add(this.gbWorldspaceMapHeightRange);
		    this.gbWorldspace.Controls.Add(this.gbWorldspaceTextures);
		    this.gbWorldspace.Controls.Add(this.tbWorldspaceFormIDEditorID);
		    this.gbWorldspace.Controls.Add(this.lWorldspaceFormID);
		    this.gbWorldspace.Controls.Add(this.cbWorldspace);
		    this.gbWorldspace.Controls.Add(this.gbWorldspaceGridRange);
		    this.gbWorldspace.Location = new System.Drawing.Point(6, 140);
		    this.gbWorldspace.Name = "gbWorldspace";
		    this.gbWorldspace.Size = new System.Drawing.Size(287, 243);
		    this.gbWorldspace.TabIndex = 2;
		    this.gbWorldspace.TabStop = false;
		    this.gbWorldspace.Text = "Worldspace:        ";
		    // 
		    // cbRenderWorldspaceHeightTextures
		    // 
		    this.cbRenderWorldspaceHeightTextures.Location = new System.Drawing.Point(138, 213);
		    this.cbRenderWorldspaceHeightTextures.Name = "cbRenderWorldspaceHeightTextures";
		    this.cbRenderWorldspaceHeightTextures.Size = new System.Drawing.Size(130, 24);
		    this.cbRenderWorldspaceHeightTextures.TabIndex = 7;
		    this.cbRenderWorldspaceHeightTextures.Text = "Create bitmaps";
		    this.cbRenderWorldspaceHeightTextures.UseVisualStyleBackColor = true;
		    // 
		    // btnLoadWorldspaceHeightTextures
		    // 
		    this.btnLoadWorldspaceHeightTextures.Location = new System.Drawing.Point(6, 214);
		    this.btnLoadWorldspaceHeightTextures.Name = "btnLoadWorldspaceHeightTextures";
		    this.btnLoadWorldspaceHeightTextures.Size = new System.Drawing.Size(117, 23);
		    this.btnLoadWorldspaceHeightTextures.TabIndex = 6;
		    this.btnLoadWorldspaceHeightTextures.Text = "Load Textures";
		    this.btnLoadWorldspaceHeightTextures.UseVisualStyleBackColor = true;
		    this.btnLoadWorldspaceHeightTextures.Click += new System.EventHandler(this.BtnLoadWorldspaceHeightTexturesClick);
		    // 
		    // gbWorldspaceMapHeightRange
		    // 
		    this.gbWorldspaceMapHeightRange.Controls.Add(this.lWorldspaceMapHeightMax);
		    this.gbWorldspaceMapHeightRange.Controls.Add(this.lWorldspaceMapHeightMin);
		    this.gbWorldspaceMapHeightRange.Controls.Add(this.tbWorldspaceMapHeightMax);
		    this.gbWorldspaceMapHeightRange.Controls.Add(this.tbWorldspaceMapHeightMin);
		    this.gbWorldspaceMapHeightRange.Location = new System.Drawing.Point(129, 56);
		    this.gbWorldspaceMapHeightRange.Name = "gbWorldspaceMapHeightRange";
		    this.gbWorldspaceMapHeightRange.Size = new System.Drawing.Size(148, 73);
		    this.gbWorldspaceMapHeightRange.TabIndex = 5;
		    this.gbWorldspaceMapHeightRange.TabStop = false;
		    this.gbWorldspaceMapHeightRange.Text = "Map Height";
		    // 
		    // lWorldspaceMapHeightMax
		    // 
		    this.lWorldspaceMapHeightMax.Location = new System.Drawing.Point(9, 48);
		    this.lWorldspaceMapHeightMax.Name = "lWorldspaceMapHeightMax";
		    this.lWorldspaceMapHeightMax.Size = new System.Drawing.Size(36, 23);
		    this.lWorldspaceMapHeightMax.TabIndex = 3;
		    this.lWorldspaceMapHeightMax.Text = "Max:";
		    // 
		    // lWorldspaceMapHeightMin
		    // 
		    this.lWorldspaceMapHeightMin.Location = new System.Drawing.Point(9, 22);
		    this.lWorldspaceMapHeightMin.Name = "lWorldspaceMapHeightMin";
		    this.lWorldspaceMapHeightMin.Size = new System.Drawing.Size(36, 23);
		    this.lWorldspaceMapHeightMin.TabIndex = 2;
		    this.lWorldspaceMapHeightMin.Text = "Min:";
		    // 
		    // tbWorldspaceMapHeightMax
		    // 
		    this.tbWorldspaceMapHeightMax.Enabled = false;
		    this.tbWorldspaceMapHeightMax.Location = new System.Drawing.Point(51, 45);
		    this.tbWorldspaceMapHeightMax.Name = "tbWorldspaceMapHeightMax";
		    this.tbWorldspaceMapHeightMax.ReadOnly = true;
		    this.tbWorldspaceMapHeightMax.Size = new System.Drawing.Size(88, 20);
		    this.tbWorldspaceMapHeightMax.TabIndex = 1;
		    this.tbWorldspaceMapHeightMax.Text = "44872.000000";
		    this.tbWorldspaceMapHeightMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
		    // 
		    // tbWorldspaceMapHeightMin
		    // 
		    this.tbWorldspaceMapHeightMin.Enabled = false;
		    this.tbWorldspaceMapHeightMin.Location = new System.Drawing.Point(51, 19);
		    this.tbWorldspaceMapHeightMin.Name = "tbWorldspaceMapHeightMin";
		    this.tbWorldspaceMapHeightMin.ReadOnly = true;
		    this.tbWorldspaceMapHeightMin.Size = new System.Drawing.Size(88, 20);
		    this.tbWorldspaceMapHeightMin.TabIndex = 0;
		    this.tbWorldspaceMapHeightMin.Text = "-8320.000000";
		    this.tbWorldspaceMapHeightMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
		    // 
		    // gbWorldspaceTextures
		    // 
		    this.gbWorldspaceTextures.Controls.Add(this.tbWorldspaceWaterHeightsTexture);
		    this.gbWorldspaceTextures.Controls.Add(this.tbWorldspaceHeightmapTexture);
		    this.gbWorldspaceTextures.Location = new System.Drawing.Point(6, 135);
		    this.gbWorldspaceTextures.Name = "gbWorldspaceTextures";
		    this.gbWorldspaceTextures.Size = new System.Drawing.Size(271, 73);
		    this.gbWorldspaceTextures.TabIndex = 4;
		    this.gbWorldspaceTextures.TabStop = false;
		    this.gbWorldspaceTextures.Text = "Textures";
		    // 
		    // tbWorldspaceWaterHeightsTexture
		    // 
		    this.tbWorldspaceWaterHeightsTexture.Enabled = false;
		    this.tbWorldspaceWaterHeightsTexture.Location = new System.Drawing.Point(6, 45);
		    this.tbWorldspaceWaterHeightsTexture.Name = "tbWorldspaceWaterHeightsTexture";
		    this.tbWorldspaceWaterHeightsTexture.ReadOnly = true;
		    this.tbWorldspaceWaterHeightsTexture.Size = new System.Drawing.Size(256, 20);
		    this.tbWorldspaceWaterHeightsTexture.TabIndex = 1;
		    this.tbWorldspaceWaterHeightsTexture.Text = "Commonwealth_WaterHeights.dds";
		    // 
		    // tbWorldspaceHeightmapTexture
		    // 
		    this.tbWorldspaceHeightmapTexture.Enabled = false;
		    this.tbWorldspaceHeightmapTexture.Location = new System.Drawing.Point(6, 19);
		    this.tbWorldspaceHeightmapTexture.Name = "tbWorldspaceHeightmapTexture";
		    this.tbWorldspaceHeightmapTexture.ReadOnly = true;
		    this.tbWorldspaceHeightmapTexture.Size = new System.Drawing.Size(256, 20);
		    this.tbWorldspaceHeightmapTexture.TabIndex = 0;
		    this.tbWorldspaceHeightmapTexture.Text = "Commonwealth_LandHeights.dds";
		    // 
		    // tbWorldspaceFormIDEditorID
		    // 
		    this.tbWorldspaceFormIDEditorID.Enabled = false;
		    this.tbWorldspaceFormIDEditorID.Location = new System.Drawing.Point(97, 27);
		    this.tbWorldspaceFormIDEditorID.Name = "tbWorldspaceFormIDEditorID";
		    this.tbWorldspaceFormIDEditorID.ReadOnly = true;
		    this.tbWorldspaceFormIDEditorID.Size = new System.Drawing.Size(180, 20);
		    this.tbWorldspaceFormIDEditorID.TabIndex = 3;
		    this.tbWorldspaceFormIDEditorID.Text = "Commonwealth [3C]";
		    // 
		    // lWorldspaceFormID
		    // 
		    this.lWorldspaceFormID.Location = new System.Drawing.Point(6, 30);
		    this.lWorldspaceFormID.Name = "lWorldspaceFormID";
		    this.lWorldspaceFormID.Size = new System.Drawing.Size(99, 23);
		    this.lWorldspaceFormID.TabIndex = 2;
		    this.lWorldspaceFormID.Text = "FormID [EditorID]:";
		    // 
		    // cbWorldspace
		    // 
		    this.cbWorldspace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		    this.cbWorldspace.FormattingEnabled = true;
		    this.cbWorldspace.Location = new System.Drawing.Point(76, 0);
		    this.cbWorldspace.Name = "cbWorldspace";
		    this.cbWorldspace.Size = new System.Drawing.Size(201, 21);
		    this.cbWorldspace.TabIndex = 1;
		    this.cbWorldspace.SelectedIndexChanged += new System.EventHandler(this.CbWorldspaceSelectedIndexChanged);
		    // 
		    // gbWorldspaceGridRange
		    // 
		    this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridBottomX);
		    this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridBottomY);
		    this.gbWorldspaceGridRange.Controls.Add(this.lWorldspaceGridBRComma);
		    this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridTopY);
		    this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridTopX);
		    this.gbWorldspaceGridRange.Controls.Add(this.lWorldspaceGridTLComma);
		    this.gbWorldspaceGridRange.Controls.Add(this.gbWorldspaceGridRangeIndicator);
		    this.gbWorldspaceGridRange.Location = new System.Drawing.Point(6, 56);
		    this.gbWorldspaceGridRange.Name = "gbWorldspaceGridRange";
		    this.gbWorldspaceGridRange.Size = new System.Drawing.Size(117, 73);
		    this.gbWorldspaceGridRange.TabIndex = 0;
		    this.gbWorldspaceGridRange.TabStop = false;
		    this.gbWorldspaceGridRange.Text = "Grid Range";
		    // 
		    // tbWorldspaceGridBottomX
		    // 
		    this.tbWorldspaceGridBottomX.Enabled = false;
		    this.tbWorldspaceGridBottomX.Location = new System.Drawing.Point(42, 45);
		    this.tbWorldspaceGridBottomX.Name = "tbWorldspaceGridBottomX";
		    this.tbWorldspaceGridBottomX.ReadOnly = true;
		    this.tbWorldspaceGridBottomX.Size = new System.Drawing.Size(30, 20);
		    this.tbWorldspaceGridBottomX.TabIndex = 6;
		    this.tbWorldspaceGridBottomX.Text = "38";
		    this.tbWorldspaceGridBottomX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		    // 
		    // tbWorldspaceGridBottomY
		    // 
		    this.tbWorldspaceGridBottomY.Enabled = false;
		    this.tbWorldspaceGridBottomY.Location = new System.Drawing.Point(79, 45);
		    this.tbWorldspaceGridBottomY.Name = "tbWorldspaceGridBottomY";
		    this.tbWorldspaceGridBottomY.ReadOnly = true;
		    this.tbWorldspaceGridBottomY.Size = new System.Drawing.Size(30, 20);
		    this.tbWorldspaceGridBottomY.TabIndex = 4;
		    this.tbWorldspaceGridBottomY.Text = "-46";
		    this.tbWorldspaceGridBottomY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		    // 
		    // lWorldspaceGridBRComma
		    // 
		    this.lWorldspaceGridBRComma.Location = new System.Drawing.Point(71, 48);
		    this.lWorldspaceGridBRComma.Name = "lWorldspaceGridBRComma";
		    this.lWorldspaceGridBRComma.Size = new System.Drawing.Size(10, 23);
		    this.lWorldspaceGridBRComma.TabIndex = 5;
		    this.lWorldspaceGridBRComma.Text = ",";
		    // 
		    // tbWorldspaceGridTopY
		    // 
		    this.tbWorldspaceGridTopY.Enabled = false;
		    this.tbWorldspaceGridTopY.Location = new System.Drawing.Point(43, 19);
		    this.tbWorldspaceGridTopY.Name = "tbWorldspaceGridTopY";
		    this.tbWorldspaceGridTopY.ReadOnly = true;
		    this.tbWorldspaceGridTopY.Size = new System.Drawing.Size(30, 20);
		    this.tbWorldspaceGridTopY.TabIndex = 1;
		    this.tbWorldspaceGridTopY.Text = "36";
		    this.tbWorldspaceGridTopY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		    // 
		    // tbWorldspaceGridTopX
		    // 
		    this.tbWorldspaceGridTopX.Enabled = false;
		    this.tbWorldspaceGridTopX.Location = new System.Drawing.Point(6, 19);
		    this.tbWorldspaceGridTopX.Name = "tbWorldspaceGridTopX";
		    this.tbWorldspaceGridTopX.ReadOnly = true;
		    this.tbWorldspaceGridTopX.Size = new System.Drawing.Size(30, 20);
		    this.tbWorldspaceGridTopX.TabIndex = 0;
		    this.tbWorldspaceGridTopX.Text = "-42";
		    this.tbWorldspaceGridTopX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		    // 
		    // lWorldspaceGridTLComma
		    // 
		    this.lWorldspaceGridTLComma.Location = new System.Drawing.Point(35, 22);
		    this.lWorldspaceGridTLComma.Name = "lWorldspaceGridTLComma";
		    this.lWorldspaceGridTLComma.Size = new System.Drawing.Size(10, 23);
		    this.lWorldspaceGridTLComma.TabIndex = 2;
		    this.lWorldspaceGridTLComma.Text = ",";
		    // 
		    // gbWorldspaceGridRangeIndicator
		    // 
		    this.gbWorldspaceGridRangeIndicator.Location = new System.Drawing.Point(18, 22);
		    this.gbWorldspaceGridRangeIndicator.Name = "gbWorldspaceGridRangeIndicator";
		    this.gbWorldspaceGridRangeIndicator.Size = new System.Drawing.Size(81, 34);
		    this.gbWorldspaceGridRangeIndicator.TabIndex = 3;
		    this.gbWorldspaceGridRangeIndicator.TabStop = false;
		    // 
		    // gbSourceMod
		    // 
		    this.gbSourceMod.Controls.Add(this.gbWeldThreshold);
		    this.gbSourceMod.Controls.Add(this.btnBuildImportModVolumeBorders);
		    this.gbSourceMod.Controls.Add(this.btnLoadImportModBuildVolumes);
		    this.gbSourceMod.Controls.Add(this.cbImportMod);
		    this.gbSourceMod.Location = new System.Drawing.Point(6, 27);
		    this.gbSourceMod.Name = "gbSourceMod";
		    this.gbSourceMod.Size = new System.Drawing.Size(287, 107);
		    this.gbSourceMod.TabIndex = 3;
		    this.gbSourceMod.TabStop = false;
		    this.gbSourceMod.Text = "Source Mod:        ";
		    // 
		    // gbWeldThreshold
		    // 
		    this.gbWeldThreshold.Controls.Add(this.cbWeldAllTogether);
		    this.gbWeldThreshold.Controls.Add(this.lblWeldThreshold);
		    this.gbWeldThreshold.Controls.Add(this.tbWeldThreshold);
		    this.gbWeldThreshold.Controls.Add(this.btnWeldImportVolumeVerts);
		    this.gbWeldThreshold.Location = new System.Drawing.Point(6, 27);
		    this.gbWeldThreshold.Name = "gbWeldThreshold";
		    this.gbWeldThreshold.Size = new System.Drawing.Size(182, 74);
		    this.gbWeldThreshold.TabIndex = 10;
		    this.gbWeldThreshold.TabStop = false;
		    this.gbWeldThreshold.Text = "Welding";
		    // 
		    // cbWeldAllTogether
		    // 
		    this.cbWeldAllTogether.Location = new System.Drawing.Point(8, 48);
		    this.cbWeldAllTogether.Name = "cbWeldAllTogether";
		    this.cbWeldAllTogether.Size = new System.Drawing.Size(75, 18);
		    this.cbWeldAllTogether.TabIndex = 17;
		    this.cbWeldAllTogether.Text = "Globally";
		    this.cbWeldAllTogether.UseVisualStyleBackColor = true;
		    // 
		    // lblWeldThreshold
		    // 
		    this.lblWeldThreshold.Location = new System.Drawing.Point(6, 22);
		    this.lblWeldThreshold.Name = "lblWeldThreshold";
		    this.lblWeldThreshold.Size = new System.Drawing.Size(77, 17);
		    this.lblWeldThreshold.TabIndex = 10;
		    this.lblWeldThreshold.Text = "Threshold:";
		    // 
		    // tbWeldThreshold
		    // 
		    this.tbWeldThreshold.Location = new System.Drawing.Point(89, 19);
		    this.tbWeldThreshold.Name = "tbWeldThreshold";
		    this.tbWeldThreshold.Size = new System.Drawing.Size(87, 20);
		    this.tbWeldThreshold.TabIndex = 0;
		    this.tbWeldThreshold.Text = "64.0000";
		    this.tbWeldThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		    this.tbWeldThreshold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbWeldThresholdKeyPress);
		    // 
		    // btnWeldImportVolumeVerts
		    // 
		    this.btnWeldImportVolumeVerts.Enabled = false;
		    this.btnWeldImportVolumeVerts.Location = new System.Drawing.Point(89, 45);
		    this.btnWeldImportVolumeVerts.Name = "btnWeldImportVolumeVerts";
		    this.btnWeldImportVolumeVerts.Size = new System.Drawing.Size(87, 23);
		    this.btnWeldImportVolumeVerts.TabIndex = 9;
		    this.btnWeldImportVolumeVerts.Text = "Weld Vertices";
		    this.btnWeldImportVolumeVerts.UseVisualStyleBackColor = true;
		    this.btnWeldImportVolumeVerts.Click += new System.EventHandler(this.BtnWeldImportVolumeVertsClick);
		    // 
		    // btnBuildImportModVolumeBorders
		    // 
		    this.btnBuildImportModVolumeBorders.Enabled = false;
		    this.btnBuildImportModVolumeBorders.Location = new System.Drawing.Point(194, 72);
		    this.btnBuildImportModVolumeBorders.Name = "btnBuildImportModVolumeBorders";
		    this.btnBuildImportModVolumeBorders.Size = new System.Drawing.Size(87, 23);
		    this.btnBuildImportModVolumeBorders.TabIndex = 8;
		    this.btnBuildImportModVolumeBorders.Text = "Build Borders";
		    this.btnBuildImportModVolumeBorders.UseVisualStyleBackColor = true;
		    this.btnBuildImportModVolumeBorders.Click += new System.EventHandler(this.BtnBuildImportModVolumeBordersClick);
		    // 
		    // btnLoadImportModBuildVolumes
		    // 
		    this.btnLoadImportModBuildVolumes.Enabled = false;
		    this.btnLoadImportModBuildVolumes.Location = new System.Drawing.Point(194, 27);
		    this.btnLoadImportModBuildVolumes.Name = "btnLoadImportModBuildVolumes";
		    this.btnLoadImportModBuildVolumes.Size = new System.Drawing.Size(87, 23);
		    this.btnLoadImportModBuildVolumes.TabIndex = 7;
		    this.btnLoadImportModBuildVolumes.Text = "Load Volumes";
		    this.btnLoadImportModBuildVolumes.UseVisualStyleBackColor = true;
		    this.btnLoadImportModBuildVolumes.Click += new System.EventHandler(this.BtnLoadImportModBuildVolumesClick);
		    // 
		    // cbImportMod
		    // 
		    this.cbImportMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		    this.cbImportMod.FormattingEnabled = true;
		    this.cbImportMod.Location = new System.Drawing.Point(76, 0);
		    this.cbImportMod.Name = "cbImportMod";
		    this.cbImportMod.Size = new System.Drawing.Size(201, 21);
		    this.cbImportMod.TabIndex = 2;
		    this.cbImportMod.SelectedIndexChanged += new System.EventHandler(this.CbImportModSelectedIndexChanged);
		    // 
		    // pbRenderWindow
		    // 
		    this.pbRenderWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		    this.pbRenderWindow.BackColor = System.Drawing.Color.Black;
		    this.pbRenderWindow.Cursor = System.Windows.Forms.Cursors.Cross;
		    this.pbRenderWindow.Location = new System.Drawing.Point(0, 0);
		    this.pbRenderWindow.Name = "pbRenderWindow";
		    this.pbRenderWindow.Size = new System.Drawing.Size(449, 614);
		    this.pbRenderWindow.TabIndex = 4;
		    this.pbRenderWindow.TabStop = false;
		    this.pbRenderWindow.SizeChanged += new System.EventHandler(this.PbRenderWindowSizeChanged);
		    this.pbRenderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PbRenderWindowMouseMove);
		    this.pbRenderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PbRenderWindowMouseUp);
		    // 
		    // pnRenderWindow
		    // 
		    this.pnRenderWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		    this.pnRenderWindow.Controls.Add(this.pbRenderWindow);
		    this.pnRenderWindow.Location = new System.Drawing.Point(299, 27);
		    this.pnRenderWindow.Name = "pnRenderWindow";
		    this.pnRenderWindow.Size = new System.Drawing.Size(449, 614);
		    this.pnRenderWindow.TabIndex = 5;
		    // 
		    // btnCellWindowRedraw
		    // 
		    this.btnCellWindowRedraw.Location = new System.Drawing.Point(129, 136);
		    this.btnCellWindowRedraw.Name = "btnCellWindowRedraw";
		    this.btnCellWindowRedraw.Size = new System.Drawing.Size(152, 23);
		    this.btnCellWindowRedraw.TabIndex = 7;
		    this.btnCellWindowRedraw.Text = "Redraw Map";
		    this.btnCellWindowRedraw.UseVisualStyleBackColor = true;
		    this.btnCellWindowRedraw.Click += new System.EventHandler(this.BtnCellWindowRedrawClick);
		    // 
		    // gbRenderOptions
		    // 
		    this.gbRenderOptions.Controls.Add(this.cbRenderOverRegion);
		    this.gbRenderOptions.Controls.Add(this.cbExportPNG);
		    this.gbRenderOptions.Controls.Add(this.btnCellWindowRedraw);
		    this.gbRenderOptions.Controls.Add(this.gbRenderSelectedOnly);
		    this.gbRenderOptions.Controls.Add(this.cbRenderBorders);
		    this.gbRenderOptions.Controls.Add(this.cbRenderBuildVolumes);
		    this.gbRenderOptions.Controls.Add(this.cbRenderCellGrid);
		    this.gbRenderOptions.Controls.Add(this.cbRenderWaterHeight);
		    this.gbRenderOptions.Controls.Add(this.cbRenderLandHeight);
		    this.gbRenderOptions.Location = new System.Drawing.Point(6, 389);
		    this.gbRenderOptions.Name = "gbRenderOptions";
		    this.gbRenderOptions.Size = new System.Drawing.Size(287, 165);
		    this.gbRenderOptions.TabIndex = 6;
		    this.gbRenderOptions.TabStop = false;
		    this.gbRenderOptions.Text = "Render Options";
		    // 
		    // cbRenderOverRegion
		    // 
		    this.cbRenderOverRegion.Location = new System.Drawing.Point(6, 19);
		    this.cbRenderOverRegion.Name = "cbRenderOverRegion";
		    this.cbRenderOverRegion.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderOverRegion.TabIndex = 16;
		    this.cbRenderOverRegion.Text = "Non-Playable";
		    this.cbRenderOverRegion.UseVisualStyleBackColor = true;
		    // 
		    // cbExportPNG
		    // 
		    this.cbExportPNG.Location = new System.Drawing.Point(6, 139);
		    this.cbExportPNG.Name = "cbExportPNG";
		    this.cbExportPNG.Size = new System.Drawing.Size(109, 18);
		    this.cbExportPNG.TabIndex = 15;
		    this.cbExportPNG.Text = "Export PNG";
		    this.cbExportPNG.UseVisualStyleBackColor = true;
		    // 
		    // gbRenderSelectedOnly
		    // 
		    this.gbRenderSelectedOnly.Controls.Add(this.lbVolumeParents);
		    this.gbRenderSelectedOnly.Controls.Add(this.cbRenderSelectedOnly);
		    this.gbRenderSelectedOnly.Enabled = false;
		    this.gbRenderSelectedOnly.Location = new System.Drawing.Point(129, 9);
		    this.gbRenderSelectedOnly.Name = "gbRenderSelectedOnly";
		    this.gbRenderSelectedOnly.Size = new System.Drawing.Size(152, 121);
		    this.gbRenderSelectedOnly.TabIndex = 13;
		    this.gbRenderSelectedOnly.TabStop = false;
		    this.gbRenderSelectedOnly.Text = "Volume Groups";
		    // 
		    // lbVolumeParents
		    // 
		    this.lbVolumeParents.FormattingEnabled = true;
		    this.lbVolumeParents.Location = new System.Drawing.Point(9, 19);
		    this.lbVolumeParents.Name = "lbVolumeParents";
		    this.lbVolumeParents.ScrollAlwaysVisible = true;
		    this.lbVolumeParents.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
		    this.lbVolumeParents.Size = new System.Drawing.Size(130, 69);
		    this.lbVolumeParents.TabIndex = 16;
		    // 
		    // cbRenderSelectedOnly
		    // 
		    this.cbRenderSelectedOnly.Location = new System.Drawing.Point(9, 97);
		    this.cbRenderSelectedOnly.Name = "cbRenderSelectedOnly";
		    this.cbRenderSelectedOnly.Size = new System.Drawing.Size(103, 18);
		    this.cbRenderSelectedOnly.TabIndex = 14;
		    this.cbRenderSelectedOnly.Text = "Selected Only";
		    this.cbRenderSelectedOnly.UseVisualStyleBackColor = true;
		    // 
		    // cbRenderBorders
		    // 
		    this.cbRenderBorders.Checked = true;
		    this.cbRenderBorders.CheckState = System.Windows.Forms.CheckState.Checked;
		    this.cbRenderBorders.Location = new System.Drawing.Point(6, 115);
		    this.cbRenderBorders.Name = "cbRenderBorders";
		    this.cbRenderBorders.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderBorders.TabIndex = 12;
		    this.cbRenderBorders.Text = "Volume Borders";
		    this.cbRenderBorders.UseVisualStyleBackColor = true;
		    // 
		    // cbRenderBuildVolumes
		    // 
		    this.cbRenderBuildVolumes.Checked = true;
		    this.cbRenderBuildVolumes.CheckState = System.Windows.Forms.CheckState.Checked;
		    this.cbRenderBuildVolumes.Location = new System.Drawing.Point(6, 97);
		    this.cbRenderBuildVolumes.Name = "cbRenderBuildVolumes";
		    this.cbRenderBuildVolumes.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderBuildVolumes.TabIndex = 11;
		    this.cbRenderBuildVolumes.Text = "Build Volumes";
		    this.cbRenderBuildVolumes.UseVisualStyleBackColor = true;
		    // 
		    // cbRenderCellGrid
		    // 
		    this.cbRenderCellGrid.Checked = true;
		    this.cbRenderCellGrid.CheckState = System.Windows.Forms.CheckState.Checked;
		    this.cbRenderCellGrid.Location = new System.Drawing.Point(6, 79);
		    this.cbRenderCellGrid.Name = "cbRenderCellGrid";
		    this.cbRenderCellGrid.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderCellGrid.TabIndex = 10;
		    this.cbRenderCellGrid.Text = "Cell Grid";
		    this.cbRenderCellGrid.UseVisualStyleBackColor = true;
		    // 
		    // cbRenderWaterHeight
		    // 
		    this.cbRenderWaterHeight.Checked = true;
		    this.cbRenderWaterHeight.CheckState = System.Windows.Forms.CheckState.Checked;
		    this.cbRenderWaterHeight.Location = new System.Drawing.Point(6, 61);
		    this.cbRenderWaterHeight.Name = "cbRenderWaterHeight";
		    this.cbRenderWaterHeight.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderWaterHeight.TabIndex = 9;
		    this.cbRenderWaterHeight.Text = "Water Height";
		    this.cbRenderWaterHeight.UseVisualStyleBackColor = true;
		    // 
		    // cbRenderLandHeight
		    // 
		    this.cbRenderLandHeight.Checked = true;
		    this.cbRenderLandHeight.CheckState = System.Windows.Forms.CheckState.Checked;
		    this.cbRenderLandHeight.Location = new System.Drawing.Point(6, 43);
		    this.cbRenderLandHeight.Name = "cbRenderLandHeight";
		    this.cbRenderLandHeight.Size = new System.Drawing.Size(109, 18);
		    this.cbRenderLandHeight.TabIndex = 8;
		    this.cbRenderLandHeight.Text = "Land Height";
		    this.cbRenderLandHeight.UseVisualStyleBackColor = true;
		    // 
		    // gbEditOptions
		    // 
		    this.gbEditOptions.Controls.Add(this.cbEditModeEnable);
		    this.gbEditOptions.Controls.Add(this.tbEMHotKeys);
		    this.gbEditOptions.Location = new System.Drawing.Point(6, 560);
		    this.gbEditOptions.Name = "gbEditOptions";
		    this.gbEditOptions.Size = new System.Drawing.Size(287, 74);
		    this.gbEditOptions.TabIndex = 7;
		    this.gbEditOptions.TabStop = false;
		    // 
		    // cbEditModeEnable
		    // 
		    this.cbEditModeEnable.Location = new System.Drawing.Point(6, 0);
		    this.cbEditModeEnable.Name = "cbEditModeEnable";
		    this.cbEditModeEnable.Size = new System.Drawing.Size(81, 18);
		    this.cbEditModeEnable.TabIndex = 0;
		    this.cbEditModeEnable.Text = "Edit Mode:";
		    this.cbEditModeEnable.UseVisualStyleBackColor = true;
		    this.cbEditModeEnable.CheckedChanged += new System.EventHandler(this.CbEditModeEnableCheckedChanged);
		    // 
		    // tbEMHotKeys
		    // 
		    this.tbEMHotKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		    this.tbEMHotKeys.Location = new System.Drawing.Point(6, 19);
		    this.tbEMHotKeys.Multiline = true;
		    this.tbEMHotKeys.Name = "tbEMHotKeys";
		    this.tbEMHotKeys.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		    this.tbEMHotKeys.Size = new System.Drawing.Size(168, 49);
		    this.tbEMHotKeys.TabIndex = 1;
		    this.tbEMHotKeys.Text = "I am a potato";
		    // 
		    // fMain
		    // 
		    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.ClientSize = new System.Drawing.Size(754, 669);
		    this.Controls.Add(this.gbEditOptions);
		    this.Controls.Add(this.gbWorldspace);
		    this.Controls.Add(this.gbRenderOptions);
		    this.Controls.Add(this.pnRenderWindow);
		    this.Controls.Add(this.gbSourceMod);
		    this.Controls.Add(this.sbMain);
		    this.Controls.Add(this.mbMain);
		    this.MainMenuStrip = this.mbMain;
		    this.MinimumSize = new System.Drawing.Size(770, 700);
		    this.Name = "fMain";
		    this.Text = "Border Builder";
		    this.Load += new System.EventHandler(this.FMainLoad);
		    this.mbMain.ResumeLayout(false);
		    this.mbMain.PerformLayout();
		    this.sbMain.ResumeLayout(false);
		    this.sbMain.PerformLayout();
		    this.gbWorldspace.ResumeLayout(false);
		    this.gbWorldspace.PerformLayout();
		    this.gbWorldspaceMapHeightRange.ResumeLayout(false);
		    this.gbWorldspaceMapHeightRange.PerformLayout();
		    this.gbWorldspaceTextures.ResumeLayout(false);
		    this.gbWorldspaceTextures.PerformLayout();
		    this.gbWorldspaceGridRange.ResumeLayout(false);
		    this.gbWorldspaceGridRange.PerformLayout();
		    this.gbSourceMod.ResumeLayout(false);
		    this.gbWeldThreshold.ResumeLayout(false);
		    this.gbWeldThreshold.PerformLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.pbRenderWindow)).EndInit();
		    this.pnRenderWindow.ResumeLayout(false);
		    this.gbRenderOptions.ResumeLayout(false);
		    this.gbRenderSelectedOnly.ResumeLayout(false);
		    this.gbEditOptions.ResumeLayout(false);
		    this.gbEditOptions.PerformLayout();
		    this.ResumeLayout(false);
		    this.PerformLayout();

		}
	}
}
