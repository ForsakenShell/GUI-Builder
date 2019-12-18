/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class BorderBatch
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.Panel pnWindow;
        System.Windows.Forms.Label lbSubDivisionNodeLength;
        System.Windows.Forms.Button btnGenNodes;
        System.Windows.Forms.TextBox tbSubDivisionNodeLength;
        System.Windows.Forms.Button btnClear;
        System.Windows.Forms.Button btnBuildNIFs;
        
        System.Windows.Forms.TextBox tbSubDivisionGroundOffset;
        System.Windows.Forms.TextBox tbSubDivisionGradientHeight;
        System.Windows.Forms.Label lblSubDivisionGradientHeight;
        System.Windows.Forms.Label lblSubDivisionGroundSink;
        System.Windows.Forms.Label lblSubDivisionGroundOffset;
        System.Windows.Forms.GroupBox gbTargetFolder;
        System.Windows.Forms.TextBox tbTargetFolder;
        System.Windows.Forms.TextBox tbSubDivisionGroundSink;
        System.Windows.Forms.GroupBox gbBorderFunctions;
        System.Windows.Forms.Button btnImportNIFs;
        System.Windows.Forms.TextBox tbWorkshopFilePrefix;
        System.Windows.Forms.Label lblWorkshopFilePrefix;
        System.Windows.Forms.TextBox tbWorkshopSampleFilePath;
        System.Windows.Forms.TextBox tbWorkshopMeshSubDirectory;
        System.Windows.Forms.CheckBox cbSubDivisionCreateImportData;
        GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> lvSubDivisions;
        System.Windows.Forms.TabControl tcObjectSelect;
        System.Windows.Forms.TabPage tpWorkshops;
        GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript> lvWorkshops;
        System.Windows.Forms.TabPage tpSubDivisions;
        System.Windows.Forms.ComboBox cbWorkshopKeywordBorderGenerator;
        System.Windows.Forms.Label lblWorkshopMarkerLink;
        System.Windows.Forms.CheckBox cbRestrictWorkshopBorderKeywords;
        System.Windows.Forms.Label lbSubDivisionSlopeAllowance;
        System.Windows.Forms.TextBox tbSubDivisionSlopeAllowance;
        System.Windows.Forms.GroupBox gbSubDivisionNodeAndNIFGeneration;
        System.Windows.Forms.TextBox tbSubDivisionMeshSubDirectory;
        System.Windows.Forms.TextBox tbNIFBuilderSubDivisionSampleFilePath;
        System.Windows.Forms.GroupBox gbWorkshopNodeAndNIFGeneration;
        System.Windows.Forms.TextBox tbMeshDirectory;
        System.Windows.Forms.Label lblMeshDirectory;
        System.Windows.Forms.TextBox tbSubDivisionFilePrefix;
        System.Windows.Forms.Label lblSubDivisionFilePrefix;
        System.Windows.Forms.Label lblSubDivisionMeshSubDirectory;
        System.Windows.Forms.Label lblWorkshopMeshSubDirectory;
        System.Windows.Forms.ComboBox cbSubDivisionPresets;
        System.Windows.Forms.Label lblSubDivisionPresets;
        System.Windows.Forms.TextBox tbWorkshopGroundSink;
        System.Windows.Forms.TextBox tbWorkshopGroundOffset;
        System.Windows.Forms.TextBox tbWorkshopGradientHeight;
        System.Windows.Forms.Label lblWorkshopGradientHeight;
        System.Windows.Forms.Label lblWorkshopGroundSink;
        System.Windows.Forms.Label lblWorkshopGroundOffset;
        System.Windows.Forms.Label lblWorkshopSlopeAllowance;
        System.Windows.Forms.TextBox tbWorkshopSlopeAllowance;
        System.Windows.Forms.Label lblWorkshopNodeLength;
        System.Windows.Forms.TextBox tbWorkshopNodeLength;
        System.Windows.Forms.ComboBox cbWorkshopPresets;
        System.Windows.Forms.Label lblWorkshopPresets;
        System.Windows.Forms.TextBox tbSubDivisionFileSuffix;
        System.Windows.Forms.Label lblSubDivisionFileSuffix;
        System.Windows.Forms.TextBox tbSubDivisionTargetSuffix;
        System.Windows.Forms.Label lblSubDivisionTargetSuffix;
        System.Windows.Forms.TextBox tbWorkshopFileSuffix;
        System.Windows.Forms.Label lblWorkshopFileSuffix;
        System.Windows.Forms.TextBox tbWorkshopTargetSuffix;
        System.Windows.Forms.Label lblWorkshopTargetSuffix;
        System.Windows.Forms.ComboBox cbWorkshopBorderMarkerForcedZ;
        System.Windows.Forms.Label lblWorkshopForcedZStatic;
        System.Windows.Forms.GroupBox gbWorkshopNodeDetection;
        System.Windows.Forms.CheckBox cbWorkshopCreateImportData;
        private System.Windows.Forms.Label lbWorkshopBorderGenerator;
        private System.Windows.Forms.ComboBox cbWorkshopKeywordBorderLink;
        private System.Windows.Forms.ComboBox cbWorkshopBorderMarkerTerrainFollowing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionKeywords;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionStaticMarkers;
        
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
            this.pnWindow = new System.Windows.Forms.Panel();
            this.gbTargetFolder = new System.Windows.Forms.GroupBox();
            this.tbMeshDirectory = new System.Windows.Forms.TextBox();
            this.tbTargetFolder = new System.Windows.Forms.TextBox();
            this.lblMeshDirectory = new System.Windows.Forms.Label();
            this.tcObjectSelect = new System.Windows.Forms.TabControl();
            this.tpSubDivisions = new System.Windows.Forms.TabPage();
            this.gbSubDivisionNodeAndNIFGeneration = new System.Windows.Forms.GroupBox();
            this.tbSubDivisionGroundSink = new System.Windows.Forms.TextBox();
            this.tbSubDivisionFileSuffix = new System.Windows.Forms.TextBox();
            this.cbSubDivisionCreateImportData = new System.Windows.Forms.CheckBox();
            this.lbSubDivisionSlopeAllowance = new System.Windows.Forms.Label();
            this.lblSubDivisionFileSuffix = new System.Windows.Forms.Label();
            this.tbSubDivisionGroundOffset = new System.Windows.Forms.TextBox();
            this.tbSubDivisionTargetSuffix = new System.Windows.Forms.TextBox();
            this.tbSubDivisionGradientHeight = new System.Windows.Forms.TextBox();
            this.lblSubDivisionGradientHeight = new System.Windows.Forms.Label();
            this.lblSubDivisionTargetSuffix = new System.Windows.Forms.Label();
            this.tbSubDivisionSlopeAllowance = new System.Windows.Forms.TextBox();
            this.cbSubDivisionPresets = new System.Windows.Forms.ComboBox();
            this.lblSubDivisionGroundSink = new System.Windows.Forms.Label();
            this.tbSubDivisionMeshSubDirectory = new System.Windows.Forms.TextBox();
            this.lblSubDivisionGroundOffset = new System.Windows.Forms.Label();
            this.lblSubDivisionPresets = new System.Windows.Forms.Label();
            this.tbNIFBuilderSubDivisionSampleFilePath = new System.Windows.Forms.TextBox();
            this.lbSubDivisionNodeLength = new System.Windows.Forms.Label();
            this.lblSubDivisionMeshSubDirectory = new System.Windows.Forms.Label();
            this.tbSubDivisionNodeLength = new System.Windows.Forms.TextBox();
            this.tbSubDivisionFilePrefix = new System.Windows.Forms.TextBox();
            this.lblSubDivisionFilePrefix = new System.Windows.Forms.Label();
            this.lvSubDivisions = new GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision>();
            this.tpWorkshops = new System.Windows.Forms.TabPage();
            this.gbWorkshopNodeDetection = new System.Windows.Forms.GroupBox();
            this.gbWorkshopNodeDetectionStaticMarkers = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblWorkshopForcedZStatic = new System.Windows.Forms.Label();
            this.cbWorkshopBorderMarkerTerrainFollowing = new System.Windows.Forms.ComboBox();
            this.cbWorkshopBorderMarkerForcedZ = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNodeDetectionKeywords = new System.Windows.Forms.GroupBox();
            this.lbWorkshopBorderGenerator = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderGenerator = new System.Windows.Forms.ComboBox();
            this.lblWorkshopMarkerLink = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderLink = new System.Windows.Forms.ComboBox();
            this.cbRestrictWorkshopBorderKeywords = new System.Windows.Forms.CheckBox();
            this.gbWorkshopNodeAndNIFGeneration = new System.Windows.Forms.GroupBox();
            this.cbWorkshopCreateImportData = new System.Windows.Forms.CheckBox();
            this.tbWorkshopGroundSink = new System.Windows.Forms.TextBox();
            this.tbWorkshopFileSuffix = new System.Windows.Forms.TextBox();
            this.tbWorkshopGroundOffset = new System.Windows.Forms.TextBox();
            this.lblWorkshopFileSuffix = new System.Windows.Forms.Label();
            this.tbWorkshopGradientHeight = new System.Windows.Forms.TextBox();
            this.tbWorkshopFilePrefix = new System.Windows.Forms.TextBox();
            this.lblWorkshopGradientHeight = new System.Windows.Forms.Label();
            this.tbWorkshopMeshSubDirectory = new System.Windows.Forms.TextBox();
            this.lblWorkshopGroundSink = new System.Windows.Forms.Label();
            this.tbWorkshopTargetSuffix = new System.Windows.Forms.TextBox();
            this.lblWorkshopGroundOffset = new System.Windows.Forms.Label();
            this.lblWorkshopTargetSuffix = new System.Windows.Forms.Label();
            this.lblWorkshopSlopeAllowance = new System.Windows.Forms.Label();
            this.cbWorkshopPresets = new System.Windows.Forms.ComboBox();
            this.tbWorkshopSlopeAllowance = new System.Windows.Forms.TextBox();
            this.lblWorkshopPresets = new System.Windows.Forms.Label();
            this.lblWorkshopNodeLength = new System.Windows.Forms.Label();
            this.tbWorkshopNodeLength = new System.Windows.Forms.TextBox();
            this.tbWorkshopSampleFilePath = new System.Windows.Forms.TextBox();
            this.lblWorkshopMeshSubDirectory = new System.Windows.Forms.Label();
            this.lblWorkshopFilePrefix = new System.Windows.Forms.Label();
            this.lvWorkshops = new GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript>();
            this.gbBorderFunctions = new System.Windows.Forms.GroupBox();
            this.btnImportNIFs = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGenNodes = new System.Windows.Forms.Button();
            this.btnBuildNIFs = new System.Windows.Forms.Button();
            this.pnWindow.SuspendLayout();
            this.gbTargetFolder.SuspendLayout();
            this.tcObjectSelect.SuspendLayout();
            this.tpSubDivisions.SuspendLayout();
            this.gbSubDivisionNodeAndNIFGeneration.SuspendLayout();
            this.tpWorkshops.SuspendLayout();
            this.gbWorkshopNodeDetection.SuspendLayout();
            this.gbWorkshopNodeDetectionStaticMarkers.SuspendLayout();
            this.gbWorkshopNodeDetectionKeywords.SuspendLayout();
            this.gbWorkshopNodeAndNIFGeneration.SuspendLayout();
            this.gbBorderFunctions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnWindow
            // 
            this.pnWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnWindow.Controls.Add(this.gbTargetFolder);
            this.pnWindow.Controls.Add(this.tcObjectSelect);
            this.pnWindow.Controls.Add(this.gbBorderFunctions);
            this.pnWindow.Location = new System.Drawing.Point(0, 0);
            this.pnWindow.Name = "pnWindow";
            this.pnWindow.Size = new System.Drawing.Size(672, 640);
            this.pnWindow.TabIndex = 0;
            // 
            // gbTargetFolder
            // 
            this.gbTargetFolder.Controls.Add(this.tbMeshDirectory);
            this.gbTargetFolder.Controls.Add(this.tbTargetFolder);
            this.gbTargetFolder.Controls.Add(this.lblMeshDirectory);
            this.gbTargetFolder.Location = new System.Drawing.Point(339, 6);
            this.gbTargetFolder.Name = "gbTargetFolder";
            this.gbTargetFolder.Size = new System.Drawing.Size(330, 68);
            this.gbTargetFolder.TabIndex = 5;
            this.gbTargetFolder.TabStop = false;
            this.gbTargetFolder.Tag = "BorderBatchWindow.TargetDirectory";
            this.gbTargetFolder.Text = "Target Folder";
            // 
            // tbMeshDirectory
            // 
            this.tbMeshDirectory.Location = new System.Drawing.Point(60, 42);
            this.tbMeshDirectory.Name = "tbMeshDirectory";
            this.tbMeshDirectory.Size = new System.Drawing.Size(264, 20);
            this.tbMeshDirectory.TabIndex = 9;
            this.tbMeshDirectory.TextChanged += new System.EventHandler(this.uiUpdateWorkshopNIFFilePathSample);
            // 
            // tbTargetFolder
            // 
            this.tbTargetFolder.Location = new System.Drawing.Point(6, 19);
            this.tbTargetFolder.Name = "tbTargetFolder";
            this.tbTargetFolder.ReadOnly = true;
            this.tbTargetFolder.Size = new System.Drawing.Size(318, 20);
            this.tbTargetFolder.TabIndex = 0;
            this.tbTargetFolder.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbNIFBuilderTargetFolderMouseClick);
            // 
            // lblMeshDirectory
            // 
            this.lblMeshDirectory.Location = new System.Drawing.Point(6, 45);
            this.lblMeshDirectory.Name = "lblMeshDirectory";
            this.lblMeshDirectory.Size = new System.Drawing.Size(56, 17);
            this.lblMeshDirectory.TabIndex = 8;
            this.lblMeshDirectory.Text = "Meshes\\";
            // 
            // tcObjectSelect
            // 
            this.tcObjectSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcObjectSelect.Controls.Add(this.tpSubDivisions);
            this.tcObjectSelect.Controls.Add(this.tpWorkshops);
            this.tcObjectSelect.Location = new System.Drawing.Point(0, 80);
            this.tcObjectSelect.Name = "tcObjectSelect";
            this.tcObjectSelect.SelectedIndex = 0;
            this.tcObjectSelect.Size = new System.Drawing.Size(672, 560);
            this.tcObjectSelect.TabIndex = 0;
            this.tcObjectSelect.SelectedIndexChanged += new System.EventHandler(this.tcObjectSelectSelectedIndexChanged);
            // 
            // tpSubDivisions
            // 
            this.tpSubDivisions.Controls.Add(this.gbSubDivisionNodeAndNIFGeneration);
            this.tpSubDivisions.Controls.Add(this.lvSubDivisions);
            this.tpSubDivisions.Location = new System.Drawing.Point(4, 22);
            this.tpSubDivisions.Name = "tpSubDivisions";
            this.tpSubDivisions.Padding = new System.Windows.Forms.Padding(3);
            this.tpSubDivisions.Size = new System.Drawing.Size(664, 534);
            this.tpSubDivisions.TabIndex = 1;
            this.tpSubDivisions.Tag = "BorderBatchWindow.Tab.SubDivisions";
            this.tpSubDivisions.Text = "Sub-Divisions";
            this.tpSubDivisions.UseVisualStyleBackColor = true;
            // 
            // gbSubDivisionNodeAndNIFGeneration
            // 
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionGroundSink);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionFileSuffix);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.cbSubDivisionCreateImportData);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lbSubDivisionSlopeAllowance);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionFileSuffix);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionGroundOffset);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionTargetSuffix);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionGradientHeight);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionGradientHeight);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionTargetSuffix);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionSlopeAllowance);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.cbSubDivisionPresets);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionGroundSink);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionMeshSubDirectory);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionGroundOffset);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionPresets);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbNIFBuilderSubDivisionSampleFilePath);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lbSubDivisionNodeLength);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionMeshSubDirectory);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionNodeLength);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.tbSubDivisionFilePrefix);
            this.gbSubDivisionNodeAndNIFGeneration.Controls.Add(this.lblSubDivisionFilePrefix);
            this.gbSubDivisionNodeAndNIFGeneration.Location = new System.Drawing.Point(0, 0);
            this.gbSubDivisionNodeAndNIFGeneration.Name = "gbSubDivisionNodeAndNIFGeneration";
            this.gbSubDivisionNodeAndNIFGeneration.Size = new System.Drawing.Size(330, 251);
            this.gbSubDivisionNodeAndNIFGeneration.TabIndex = 11;
            this.gbSubDivisionNodeAndNIFGeneration.TabStop = false;
            this.gbSubDivisionNodeAndNIFGeneration.Tag = "BorderBatchWindow.NodesAndNIFs";
            this.gbSubDivisionNodeAndNIFGeneration.Text = "Node and NIF Generation";
            // 
            // tbSubDivisionGroundSink
            // 
            this.tbSubDivisionGroundSink.Location = new System.Drawing.Point(261, 204);
            this.tbSubDivisionGroundSink.Name = "tbSubDivisionGroundSink";
            this.tbSubDivisionGroundSink.Size = new System.Drawing.Size(63, 20);
            this.tbSubDivisionGroundSink.TabIndex = 8;
            this.tbSubDivisionGroundSink.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSubDivisionGroundSink.TextChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // tbSubDivisionFileSuffix
            // 
            this.tbSubDivisionFileSuffix.Location = new System.Drawing.Point(103, 112);
            this.tbSubDivisionFileSuffix.Name = "tbSubDivisionFileSuffix";
            this.tbSubDivisionFileSuffix.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionFileSuffix.TabIndex = 28;
            this.tbSubDivisionFileSuffix.TextChanged += new System.EventHandler(this.uiUpdateSubDivisionNIFFilePathSample);
            // 
            // cbSubDivisionCreateImportData
            // 
            this.cbSubDivisionCreateImportData.AutoEllipsis = true;
            this.cbSubDivisionCreateImportData.Location = new System.Drawing.Point(6, 225);
            this.cbSubDivisionCreateImportData.Name = "cbSubDivisionCreateImportData";
            this.cbSubDivisionCreateImportData.Size = new System.Drawing.Size(317, 21);
            this.cbSubDivisionCreateImportData.TabIndex = 5;
            this.cbSubDivisionCreateImportData.Tag = "BorderBatchWindow.NodesAndNIFs.CreateImports";
            this.cbSubDivisionCreateImportData.Text = "Create Import Data";
            this.cbSubDivisionCreateImportData.UseVisualStyleBackColor = true;
            this.cbSubDivisionCreateImportData.CheckedChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // lbSubDivisionSlopeAllowance
            // 
            this.lbSubDivisionSlopeAllowance.Location = new System.Drawing.Point(6, 184);
            this.lbSubDivisionSlopeAllowance.Name = "lbSubDivisionSlopeAllowance";
            this.lbSubDivisionSlopeAllowance.Size = new System.Drawing.Size(90, 17);
            this.lbSubDivisionSlopeAllowance.TabIndex = 6;
            this.lbSubDivisionSlopeAllowance.Tag = "BorderBatchWindow.NodesAndNIFs.SlopeAllowance:";
            this.lbSubDivisionSlopeAllowance.Text = "Slope Allowance:";
            // 
            // lblSubDivisionFileSuffix
            // 
            this.lblSubDivisionFileSuffix.Location = new System.Drawing.Point(6, 115);
            this.lblSubDivisionFileSuffix.Name = "lblSubDivisionFileSuffix";
            this.lblSubDivisionFileSuffix.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionFileSuffix.TabIndex = 27;
            this.lblSubDivisionFileSuffix.Tag = "BorderBatchWindow.NodesAndNIFs.FileSuffix:";
            this.lblSubDivisionFileSuffix.Text = "File Suffix:";
            // 
            // tbSubDivisionGroundOffset
            // 
            this.tbSubDivisionGroundOffset.Location = new System.Drawing.Point(261, 181);
            this.tbSubDivisionGroundOffset.Name = "tbSubDivisionGroundOffset";
            this.tbSubDivisionGroundOffset.Size = new System.Drawing.Size(63, 20);
            this.tbSubDivisionGroundOffset.TabIndex = 7;
            this.tbSubDivisionGroundOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSubDivisionGroundOffset.TextChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // tbSubDivisionTargetSuffix
            // 
            this.tbSubDivisionTargetSuffix.Location = new System.Drawing.Point(103, 43);
            this.tbSubDivisionTargetSuffix.Name = "tbSubDivisionTargetSuffix";
            this.tbSubDivisionTargetSuffix.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionTargetSuffix.TabIndex = 22;
            this.tbSubDivisionTargetSuffix.TextChanged += new System.EventHandler(this.uiUpdateSubDivisionNIFFilePathSample);
            // 
            // tbSubDivisionGradientHeight
            // 
            this.tbSubDivisionGradientHeight.Location = new System.Drawing.Point(261, 158);
            this.tbSubDivisionGradientHeight.Name = "tbSubDivisionGradientHeight";
            this.tbSubDivisionGradientHeight.Size = new System.Drawing.Size(63, 20);
            this.tbSubDivisionGradientHeight.TabIndex = 6;
            this.tbSubDivisionGradientHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSubDivisionGradientHeight.TextChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // lblSubDivisionGradientHeight
            // 
            this.lblSubDivisionGradientHeight.Location = new System.Drawing.Point(170, 161);
            this.lblSubDivisionGradientHeight.Name = "lblSubDivisionGradientHeight";
            this.lblSubDivisionGradientHeight.Size = new System.Drawing.Size(90, 16);
            this.lblSubDivisionGradientHeight.TabIndex = 2;
            this.lblSubDivisionGradientHeight.Tag = "BorderBatchWindow.NodesAndNIFs.GradientHeight:";
            this.lblSubDivisionGradientHeight.Text = "Gradient Height:";
            // 
            // lblSubDivisionTargetSuffix
            // 
            this.lblSubDivisionTargetSuffix.Location = new System.Drawing.Point(6, 46);
            this.lblSubDivisionTargetSuffix.Name = "lblSubDivisionTargetSuffix";
            this.lblSubDivisionTargetSuffix.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionTargetSuffix.TabIndex = 23;
            this.lblSubDivisionTargetSuffix.Tag = "BorderBatchWindow.NodesAndNIFs.TargetSubDirectory:";
            this.lblSubDivisionTargetSuffix.Text = "Target Sub-Folder:";
            // 
            // tbSubDivisionSlopeAllowance
            // 
            this.tbSubDivisionSlopeAllowance.Location = new System.Drawing.Point(97, 181);
            this.tbSubDivisionSlopeAllowance.Name = "tbSubDivisionSlopeAllowance";
            this.tbSubDivisionSlopeAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbSubDivisionSlopeAllowance.Size = new System.Drawing.Size(63, 20);
            this.tbSubDivisionSlopeAllowance.TabIndex = 7;
            this.tbSubDivisionSlopeAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSubDivisionSlopeAllowance.TextChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // cbSubDivisionPresets
            // 
            this.cbSubDivisionPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubDivisionPresets.Location = new System.Drawing.Point(75, 19);
            this.cbSubDivisionPresets.Name = "cbSubDivisionPresets";
            this.cbSubDivisionPresets.Size = new System.Drawing.Size(249, 21);
            this.cbSubDivisionPresets.TabIndex = 16;
            this.cbSubDivisionPresets.SelectedIndexChanged += new System.EventHandler(this.cbSubDivisionPresetsSelectedIndexChanged);
            // 
            // lblSubDivisionGroundSink
            // 
            this.lblSubDivisionGroundSink.Location = new System.Drawing.Point(170, 207);
            this.lblSubDivisionGroundSink.Name = "lblSubDivisionGroundSink";
            this.lblSubDivisionGroundSink.Size = new System.Drawing.Size(90, 16);
            this.lblSubDivisionGroundSink.TabIndex = 1;
            this.lblSubDivisionGroundSink.Tag = "BorderBatchWindow.NodesAndNIFs.GroundSink:";
            this.lblSubDivisionGroundSink.Text = "Ground Sink:";
            // 
            // tbSubDivisionMeshSubDirectory
            // 
            this.tbSubDivisionMeshSubDirectory.Location = new System.Drawing.Point(103, 66);
            this.tbSubDivisionMeshSubDirectory.Name = "tbSubDivisionMeshSubDirectory";
            this.tbSubDivisionMeshSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionMeshSubDirectory.TabIndex = 5;
            this.tbSubDivisionMeshSubDirectory.TextChanged += new System.EventHandler(this.uiUpdateSubDivisionNIFFilePathSample);
            // 
            // lblSubDivisionGroundOffset
            // 
            this.lblSubDivisionGroundOffset.Location = new System.Drawing.Point(170, 184);
            this.lblSubDivisionGroundOffset.Name = "lblSubDivisionGroundOffset";
            this.lblSubDivisionGroundOffset.Size = new System.Drawing.Size(90, 16);
            this.lblSubDivisionGroundOffset.TabIndex = 3;
            this.lblSubDivisionGroundOffset.Tag = "BorderBatchWindow.NodesAndNIFs.GroundOffset:";
            this.lblSubDivisionGroundOffset.Text = "Ground Offset:";
            // 
            // lblSubDivisionPresets
            // 
            this.lblSubDivisionPresets.Location = new System.Drawing.Point(6, 22);
            this.lblSubDivisionPresets.Name = "lblSubDivisionPresets";
            this.lblSubDivisionPresets.Size = new System.Drawing.Size(66, 16);
            this.lblSubDivisionPresets.TabIndex = 15;
            this.lblSubDivisionPresets.Tag = "BorderBatchWindow.NodesAndNIFs.Preset:";
            this.lblSubDivisionPresets.Text = "Preset:";
            // 
            // tbNIFBuilderSubDivisionSampleFilePath
            // 
            this.tbNIFBuilderSubDivisionSampleFilePath.BackColor = System.Drawing.SystemColors.Control;
            this.tbNIFBuilderSubDivisionSampleFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNIFBuilderSubDivisionSampleFilePath.Location = new System.Drawing.Point(6, 135);
            this.tbNIFBuilderSubDivisionSampleFilePath.Name = "tbNIFBuilderSubDivisionSampleFilePath";
            this.tbNIFBuilderSubDivisionSampleFilePath.Size = new System.Drawing.Size(318, 20);
            this.tbNIFBuilderSubDivisionSampleFilePath.TabIndex = 7;
            this.tbNIFBuilderSubDivisionSampleFilePath.Text = "pickles";
            // 
            // lbSubDivisionNodeLength
            // 
            this.lbSubDivisionNodeLength.Location = new System.Drawing.Point(6, 161);
            this.lbSubDivisionNodeLength.Name = "lbSubDivisionNodeLength";
            this.lbSubDivisionNodeLength.Size = new System.Drawing.Size(90, 16);
            this.lbSubDivisionNodeLength.TabIndex = 0;
            this.lbSubDivisionNodeLength.Tag = "BorderBatchWindow.NodesAndNIFs.NodeLength:";
            this.lbSubDivisionNodeLength.Text = "Node Length:";
            // 
            // lblSubDivisionMeshSubDirectory
            // 
            this.lblSubDivisionMeshSubDirectory.Location = new System.Drawing.Point(6, 69);
            this.lblSubDivisionMeshSubDirectory.Name = "lblSubDivisionMeshSubDirectory";
            this.lblSubDivisionMeshSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionMeshSubDirectory.TabIndex = 10;
            this.lblSubDivisionMeshSubDirectory.Tag = "BorderBatchWindow.NodesAndNIFs.MeshSubDirectory:";
            this.lblSubDivisionMeshSubDirectory.Text = "Mesh Sub-Folder:";
            // 
            // tbSubDivisionNodeLength
            // 
            this.tbSubDivisionNodeLength.Location = new System.Drawing.Point(97, 158);
            this.tbSubDivisionNodeLength.Name = "tbSubDivisionNodeLength";
            this.tbSubDivisionNodeLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbSubDivisionNodeLength.Size = new System.Drawing.Size(63, 20);
            this.tbSubDivisionNodeLength.TabIndex = 4;
            this.tbSubDivisionNodeLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSubDivisionNodeLength.TextChanged += new System.EventHandler(this.uiSubDivisionNIFBuilderChanged);
            // 
            // tbSubDivisionFilePrefix
            // 
            this.tbSubDivisionFilePrefix.Location = new System.Drawing.Point(103, 89);
            this.tbSubDivisionFilePrefix.Name = "tbSubDivisionFilePrefix";
            this.tbSubDivisionFilePrefix.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionFilePrefix.TabIndex = 9;
            this.tbSubDivisionFilePrefix.TextChanged += new System.EventHandler(this.uiUpdateSubDivisionNIFFilePathSample);
            // 
            // lblSubDivisionFilePrefix
            // 
            this.lblSubDivisionFilePrefix.Location = new System.Drawing.Point(6, 92);
            this.lblSubDivisionFilePrefix.Name = "lblSubDivisionFilePrefix";
            this.lblSubDivisionFilePrefix.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionFilePrefix.TabIndex = 8;
            this.lblSubDivisionFilePrefix.Tag = "BorderBatchWindow.NodesAndNIFs.FilePrefix:";
            this.lblSubDivisionFilePrefix.Text = "File Prefix:";
            // 
            // lvSubDivisions
            // 
            this.lvSubDivisions.AllowHidingItems = true;
            this.lvSubDivisions.AllowOverrideColumnSorting = true;
            this.lvSubDivisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSubDivisions.CheckBoxes = true;
            this.lvSubDivisions.EditorIDColumn = true;
            this.lvSubDivisions.ExtraInfoColumn = false;
            this.lvSubDivisions.FilenameColumn = false;
            this.lvSubDivisions.FormIDColumn = true;
            this.lvSubDivisions.LoadOrderColumn = false;
            this.lvSubDivisions.Location = new System.Drawing.Point(336, 0);
            this.lvSubDivisions.MultiSelect = true;
            this.lvSubDivisions.Name = "lvSubDivisions";
            this.lvSubDivisions.Size = new System.Drawing.Size(328, 534);
            this.lvSubDivisions.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvSubDivisions.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvSubDivisions.SyncedEditorFormType = null;
            this.lvSubDivisions.SyncObjects = null;
            this.lvSubDivisions.TabIndex = 10;
            this.lvSubDivisions.TypeColumn = false;
            // 
            // tpWorkshops
            // 
            this.tpWorkshops.Controls.Add(this.gbWorkshopNodeDetection);
            this.tpWorkshops.Controls.Add(this.gbWorkshopNodeAndNIFGeneration);
            this.tpWorkshops.Controls.Add(this.lvWorkshops);
            this.tpWorkshops.Location = new System.Drawing.Point(4, 22);
            this.tpWorkshops.Name = "tpWorkshops";
            this.tpWorkshops.Padding = new System.Windows.Forms.Padding(3);
            this.tpWorkshops.Size = new System.Drawing.Size(664, 534);
            this.tpWorkshops.TabIndex = 0;
            this.tpWorkshops.Tag = "BorderBatchWindow.Tab.Workshops";
            this.tpWorkshops.Text = "Workshops";
            this.tpWorkshops.UseVisualStyleBackColor = true;
            // 
            // gbWorkshopNodeDetection
            // 
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionStaticMarkers);
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionKeywords);
            this.gbWorkshopNodeDetection.Controls.Add(this.cbRestrictWorkshopBorderKeywords);
            this.gbWorkshopNodeDetection.Location = new System.Drawing.Point(0, 257);
            this.gbWorkshopNodeDetection.Name = "gbWorkshopNodeDetection";
            this.gbWorkshopNodeDetection.Size = new System.Drawing.Size(330, 278);
            this.gbWorkshopNodeDetection.TabIndex = 17;
            this.gbWorkshopNodeDetection.TabStop = false;
            this.gbWorkshopNodeDetection.Tag = "BorderBatchWindow.NodeDetection";
            this.gbWorkshopNodeDetection.Text = "Node Detection";
            // 
            // gbWorkshopNodeDetectionStaticMarkers
            // 
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.label1);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.lblWorkshopForcedZStatic);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerTerrainFollowing);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerForcedZ);
            this.gbWorkshopNodeDetectionStaticMarkers.Location = new System.Drawing.Point(6, 167);
            this.gbWorkshopNodeDetectionStaticMarkers.Name = "gbWorkshopNodeDetectionStaticMarkers";
            this.gbWorkshopNodeDetectionStaticMarkers.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionStaticMarkers.TabIndex = 22;
            this.gbWorkshopNodeDetectionStaticMarkers.TabStop = false;
            this.gbWorkshopNodeDetectionStaticMarkers.Tag = "BorderBatchWindow.NodeDetection.StaticMarkers:";
            this.gbWorkshopNodeDetectionStaticMarkers.Text = "StaticMarkers:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 17);
            this.label1.TabIndex = 19;
            this.label1.Tag = "BorderBatchWindow.NodeDetection.BorderMarkerTerrainFollowing:";
            this.label1.Text = "BorderMarkerTerrainFollowing:";
            // 
            // lblWorkshopForcedZStatic
            // 
            this.lblWorkshopForcedZStatic.Location = new System.Drawing.Point(6, 60);
            this.lblWorkshopForcedZStatic.Name = "lblWorkshopForcedZStatic";
            this.lblWorkshopForcedZStatic.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopForcedZStatic.TabIndex = 15;
            this.lblWorkshopForcedZStatic.Tag = "BorderBatchWindow.NodeDetection.BorderMarkerForcedZ:";
            this.lblWorkshopForcedZStatic.Text = "BorderMarkerForcedZ:";
            // 
            // cbWorkshopBorderMarkerTerrainFollowing
            // 
            this.cbWorkshopBorderMarkerTerrainFollowing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBorderMarkerTerrainFollowing.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopBorderMarkerTerrainFollowing.Name = "cbWorkshopBorderMarkerTerrainFollowing";
            this.cbWorkshopBorderMarkerTerrainFollowing.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBorderMarkerTerrainFollowing.TabIndex = 20;
            this.cbWorkshopBorderMarkerTerrainFollowing.SelectedIndexChanged += new System.EventHandler(this.cbWorkshopBorderMarkerTerrainFollowingSelectedIndexChanged);
            // 
            // cbWorkshopBorderMarkerForcedZ
            // 
            this.cbWorkshopBorderMarkerForcedZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBorderMarkerForcedZ.Location = new System.Drawing.Point(6, 80);
            this.cbWorkshopBorderMarkerForcedZ.Name = "cbWorkshopBorderMarkerForcedZ";
            this.cbWorkshopBorderMarkerForcedZ.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBorderMarkerForcedZ.TabIndex = 16;
            this.cbWorkshopBorderMarkerForcedZ.SelectedIndexChanged += new System.EventHandler(this.cbWorkshopBorderMarkerForcedZSelectedIndexChanged);
            // 
            // gbWorkshopNodeDetectionKeywords
            // 
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lbWorkshopBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lblWorkshopMarkerLink);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderLink);
            this.gbWorkshopNodeDetectionKeywords.Location = new System.Drawing.Point(6, 55);
            this.gbWorkshopNodeDetectionKeywords.Name = "gbWorkshopNodeDetectionKeywords";
            this.gbWorkshopNodeDetectionKeywords.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionKeywords.TabIndex = 21;
            this.gbWorkshopNodeDetectionKeywords.TabStop = false;
            this.gbWorkshopNodeDetectionKeywords.Tag = "BorderBatchWindow.NodeDetection.Keywords:";
            this.gbWorkshopNodeDetectionKeywords.Text = "Keywords:";
            // 
            // lbWorkshopBorderGenerator
            // 
            this.lbWorkshopBorderGenerator.Location = new System.Drawing.Point(6, 16);
            this.lbWorkshopBorderGenerator.Name = "lbWorkshopBorderGenerator";
            this.lbWorkshopBorderGenerator.Size = new System.Drawing.Size(306, 17);
            this.lbWorkshopBorderGenerator.TabIndex = 17;
            this.lbWorkshopBorderGenerator.Tag = "BorderBatchWindow.NodeDetection.WorkshopBorderGenerator:";
            this.lbWorkshopBorderGenerator.Text = "WorkshopBorderGenerator:";
            // 
            // cbWorkshopKeywordBorderGenerator
            // 
            this.cbWorkshopKeywordBorderGenerator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopKeywordBorderGenerator.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopKeywordBorderGenerator.Name = "cbWorkshopKeywordBorderGenerator";
            this.cbWorkshopKeywordBorderGenerator.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopKeywordBorderGenerator.TabIndex = 13;
            this.cbWorkshopKeywordBorderGenerator.SelectedIndexChanged += new System.EventHandler(this.cbWorkshopKeywordBorderGeneratorSelectedIndexChanged);
            // 
            // lblWorkshopMarkerLink
            // 
            this.lblWorkshopMarkerLink.Location = new System.Drawing.Point(6, 60);
            this.lblWorkshopMarkerLink.Name = "lblWorkshopMarkerLink";
            this.lblWorkshopMarkerLink.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopMarkerLink.TabIndex = 12;
            this.lblWorkshopMarkerLink.Tag = "BorderBatchWindow.NodeDetection.WorkshopMarkerLink:";
            this.lblWorkshopMarkerLink.Text = "WorkshopMarkerLink:";
            // 
            // cbWorkshopKeywordBorderLink
            // 
            this.cbWorkshopKeywordBorderLink.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopKeywordBorderLink.Location = new System.Drawing.Point(6, 80);
            this.cbWorkshopKeywordBorderLink.Name = "cbWorkshopKeywordBorderLink";
            this.cbWorkshopKeywordBorderLink.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopKeywordBorderLink.TabIndex = 18;
            this.cbWorkshopKeywordBorderLink.SelectedIndexChanged += new System.EventHandler(this.cbWorkshopKeywordBorderLinkSelectedIndexChanged);
            // 
            // cbRestrictWorkshopBorderKeywords
            // 
            this.cbRestrictWorkshopBorderKeywords.Checked = true;
            this.cbRestrictWorkshopBorderKeywords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRestrictWorkshopBorderKeywords.Location = new System.Drawing.Point(6, 19);
            this.cbRestrictWorkshopBorderKeywords.Name = "cbRestrictWorkshopBorderKeywords";
            this.cbRestrictWorkshopBorderKeywords.Size = new System.Drawing.Size(318, 30);
            this.cbRestrictWorkshopBorderKeywords.TabIndex = 14;
            this.cbRestrictWorkshopBorderKeywords.Text = "Restrict to:\r\n{0}";
            this.cbRestrictWorkshopBorderKeywords.UseVisualStyleBackColor = true;
            this.cbRestrictWorkshopBorderKeywords.CheckStateChanged += new System.EventHandler(this.cbRestrictWorkshopBorderKeywordsChanged);
            // 
            // gbWorkshopNodeAndNIFGeneration
            // 
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.cbWorkshopCreateImportData);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopGroundSink);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopFileSuffix);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopGroundOffset);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopFileSuffix);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopGradientHeight);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopFilePrefix);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopGradientHeight);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopMeshSubDirectory);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopGroundSink);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopTargetSuffix);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopGroundOffset);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopTargetSuffix);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopSlopeAllowance);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.cbWorkshopPresets);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopSlopeAllowance);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopPresets);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopNodeLength);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopNodeLength);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.tbWorkshopSampleFilePath);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopMeshSubDirectory);
            this.gbWorkshopNodeAndNIFGeneration.Controls.Add(this.lblWorkshopFilePrefix);
            this.gbWorkshopNodeAndNIFGeneration.Location = new System.Drawing.Point(0, 0);
            this.gbWorkshopNodeAndNIFGeneration.Name = "gbWorkshopNodeAndNIFGeneration";
            this.gbWorkshopNodeAndNIFGeneration.Size = new System.Drawing.Size(330, 251);
            this.gbWorkshopNodeAndNIFGeneration.TabIndex = 10;
            this.gbWorkshopNodeAndNIFGeneration.TabStop = false;
            this.gbWorkshopNodeAndNIFGeneration.Tag = "BorderBatchWindow.NodesAndNIFs";
            this.gbWorkshopNodeAndNIFGeneration.Text = "Node and NIF Generation";
            // 
            // cbWorkshopCreateImportData
            // 
            this.cbWorkshopCreateImportData.AutoEllipsis = true;
            this.cbWorkshopCreateImportData.Location = new System.Drawing.Point(6, 225);
            this.cbWorkshopCreateImportData.Name = "cbWorkshopCreateImportData";
            this.cbWorkshopCreateImportData.Size = new System.Drawing.Size(317, 21);
            this.cbWorkshopCreateImportData.TabIndex = 18;
            this.cbWorkshopCreateImportData.Text = "Create Import Data";
            this.cbWorkshopCreateImportData.UseVisualStyleBackColor = true;
            this.cbWorkshopCreateImportData.CheckedChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // tbWorkshopGroundSink
            // 
            this.tbWorkshopGroundSink.Location = new System.Drawing.Point(261, 204);
            this.tbWorkshopGroundSink.Name = "tbWorkshopGroundSink";
            this.tbWorkshopGroundSink.Size = new System.Drawing.Size(63, 20);
            this.tbWorkshopGroundSink.TabIndex = 24;
            this.tbWorkshopGroundSink.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkshopGroundSink.TextChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // tbWorkshopFileSuffix
            // 
            this.tbWorkshopFileSuffix.Location = new System.Drawing.Point(103, 112);
            this.tbWorkshopFileSuffix.Name = "tbWorkshopFileSuffix";
            this.tbWorkshopFileSuffix.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopFileSuffix.TabIndex = 26;
            this.tbWorkshopFileSuffix.TextChanged += new System.EventHandler(this.uiUpdateWorkshopNIFFilePathSample);
            // 
            // tbWorkshopGroundOffset
            // 
            this.tbWorkshopGroundOffset.Location = new System.Drawing.Point(261, 181);
            this.tbWorkshopGroundOffset.Name = "tbWorkshopGroundOffset";
            this.tbWorkshopGroundOffset.Size = new System.Drawing.Size(63, 20);
            this.tbWorkshopGroundOffset.TabIndex = 23;
            this.tbWorkshopGroundOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkshopGroundOffset.TextChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // lblWorkshopFileSuffix
            // 
            this.lblWorkshopFileSuffix.Location = new System.Drawing.Point(6, 115);
            this.lblWorkshopFileSuffix.Name = "lblWorkshopFileSuffix";
            this.lblWorkshopFileSuffix.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopFileSuffix.TabIndex = 25;
            this.lblWorkshopFileSuffix.Tag = "BorderBatchWindow.NodesAndNIFs.FileSuffix:";
            this.lblWorkshopFileSuffix.Text = "File Suffix:";
            // 
            // tbWorkshopGradientHeight
            // 
            this.tbWorkshopGradientHeight.Location = new System.Drawing.Point(261, 158);
            this.tbWorkshopGradientHeight.Name = "tbWorkshopGradientHeight";
            this.tbWorkshopGradientHeight.Size = new System.Drawing.Size(63, 20);
            this.tbWorkshopGradientHeight.TabIndex = 22;
            this.tbWorkshopGradientHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkshopGradientHeight.TextChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // tbWorkshopFilePrefix
            // 
            this.tbWorkshopFilePrefix.Location = new System.Drawing.Point(103, 89);
            this.tbWorkshopFilePrefix.Name = "tbWorkshopFilePrefix";
            this.tbWorkshopFilePrefix.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopFilePrefix.TabIndex = 1;
            this.tbWorkshopFilePrefix.TextChanged += new System.EventHandler(this.uiUpdateWorkshopNIFFilePathSample);
            // 
            // lblWorkshopGradientHeight
            // 
            this.lblWorkshopGradientHeight.Location = new System.Drawing.Point(170, 161);
            this.lblWorkshopGradientHeight.Name = "lblWorkshopGradientHeight";
            this.lblWorkshopGradientHeight.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopGradientHeight.TabIndex = 20;
            this.lblWorkshopGradientHeight.Tag = "BorderBatchWindow.NodesAndNIFs.GradientHeight:";
            this.lblWorkshopGradientHeight.Text = "Gradient Height:";
            // 
            // tbWorkshopMeshSubDirectory
            // 
            this.tbWorkshopMeshSubDirectory.Location = new System.Drawing.Point(103, 66);
            this.tbWorkshopMeshSubDirectory.Name = "tbWorkshopMeshSubDirectory";
            this.tbWorkshopMeshSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopMeshSubDirectory.TabIndex = 2;
            this.tbWorkshopMeshSubDirectory.TextChanged += new System.EventHandler(this.uiUpdateWorkshopNIFFilePathSample);
            // 
            // lblWorkshopGroundSink
            // 
            this.lblWorkshopGroundSink.Location = new System.Drawing.Point(170, 207);
            this.lblWorkshopGroundSink.Name = "lblWorkshopGroundSink";
            this.lblWorkshopGroundSink.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopGroundSink.TabIndex = 19;
            this.lblWorkshopGroundSink.Tag = "BorderBatchWindow.NodesAndNIFs.GroundSink:";
            this.lblWorkshopGroundSink.Text = "Ground Sink:";
            // 
            // tbWorkshopTargetSuffix
            // 
            this.tbWorkshopTargetSuffix.Location = new System.Drawing.Point(103, 43);
            this.tbWorkshopTargetSuffix.Name = "tbWorkshopTargetSuffix";
            this.tbWorkshopTargetSuffix.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopTargetSuffix.TabIndex = 20;
            this.tbWorkshopTargetSuffix.TextChanged += new System.EventHandler(this.uiUpdateWorkshopNIFFilePathSample);
            // 
            // lblWorkshopGroundOffset
            // 
            this.lblWorkshopGroundOffset.Location = new System.Drawing.Point(170, 184);
            this.lblWorkshopGroundOffset.Name = "lblWorkshopGroundOffset";
            this.lblWorkshopGroundOffset.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopGroundOffset.TabIndex = 21;
            this.lblWorkshopGroundOffset.Tag = "BorderBatchWindow.NodesAndNIFs.GroundOffset:";
            this.lblWorkshopGroundOffset.Text = "Ground Offset:";
            // 
            // lblWorkshopTargetSuffix
            // 
            this.lblWorkshopTargetSuffix.Location = new System.Drawing.Point(6, 46);
            this.lblWorkshopTargetSuffix.Name = "lblWorkshopTargetSuffix";
            this.lblWorkshopTargetSuffix.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopTargetSuffix.TabIndex = 21;
            this.lblWorkshopTargetSuffix.Tag = "BorderBatchWindow.NodesAndNIFs.TargetSubDirectory:";
            this.lblWorkshopTargetSuffix.Text = "Target Sub-Folder:";
            // 
            // lblWorkshopSlopeAllowance
            // 
            this.lblWorkshopSlopeAllowance.Location = new System.Drawing.Point(6, 184);
            this.lblWorkshopSlopeAllowance.Name = "lblWorkshopSlopeAllowance";
            this.lblWorkshopSlopeAllowance.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopSlopeAllowance.TabIndex = 17;
            this.lblWorkshopSlopeAllowance.Tag = "BorderBatchWindow.NodesAndNIFs.SlopeAllowance:";
            this.lblWorkshopSlopeAllowance.Text = "Slope Allowance:";
            // 
            // cbWorkshopPresets
            // 
            this.cbWorkshopPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopPresets.Location = new System.Drawing.Point(75, 19);
            this.cbWorkshopPresets.Name = "cbWorkshopPresets";
            this.cbWorkshopPresets.Size = new System.Drawing.Size(249, 21);
            this.cbWorkshopPresets.TabIndex = 14;
            this.cbWorkshopPresets.SelectedIndexChanged += new System.EventHandler(this.cbWorkshopPresetsSelectedIndexChanged);
            // 
            // tbWorkshopSlopeAllowance
            // 
            this.tbWorkshopSlopeAllowance.Location = new System.Drawing.Point(97, 181);
            this.tbWorkshopSlopeAllowance.Name = "tbWorkshopSlopeAllowance";
            this.tbWorkshopSlopeAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbWorkshopSlopeAllowance.Size = new System.Drawing.Size(63, 20);
            this.tbWorkshopSlopeAllowance.TabIndex = 18;
            this.tbWorkshopSlopeAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkshopSlopeAllowance.TextChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // lblWorkshopPresets
            // 
            this.lblWorkshopPresets.Location = new System.Drawing.Point(6, 22);
            this.lblWorkshopPresets.Name = "lblWorkshopPresets";
            this.lblWorkshopPresets.Size = new System.Drawing.Size(66, 16);
            this.lblWorkshopPresets.TabIndex = 10;
            this.lblWorkshopPresets.Tag = "BorderBatchWindow.NodesAndNIFs.Preset:";
            this.lblWorkshopPresets.Text = "Preset:";
            // 
            // lblWorkshopNodeLength
            // 
            this.lblWorkshopNodeLength.Location = new System.Drawing.Point(6, 161);
            this.lblWorkshopNodeLength.Name = "lblWorkshopNodeLength";
            this.lblWorkshopNodeLength.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopNodeLength.TabIndex = 15;
            this.lblWorkshopNodeLength.Tag = "BorderBatchWindow.NodesAndNIFs.NodeLength:";
            this.lblWorkshopNodeLength.Text = "Node Length:";
            // 
            // tbWorkshopNodeLength
            // 
            this.tbWorkshopNodeLength.Location = new System.Drawing.Point(97, 158);
            this.tbWorkshopNodeLength.Name = "tbWorkshopNodeLength";
            this.tbWorkshopNodeLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbWorkshopNodeLength.Size = new System.Drawing.Size(63, 20);
            this.tbWorkshopNodeLength.TabIndex = 16;
            this.tbWorkshopNodeLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbWorkshopNodeLength.TextChanged += new System.EventHandler(this.uiWorkshopNIFBuilderChanged);
            // 
            // tbWorkshopSampleFilePath
            // 
            this.tbWorkshopSampleFilePath.BackColor = System.Drawing.SystemColors.Control;
            this.tbWorkshopSampleFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbWorkshopSampleFilePath.Location = new System.Drawing.Point(6, 135);
            this.tbWorkshopSampleFilePath.Name = "tbWorkshopSampleFilePath";
            this.tbWorkshopSampleFilePath.Size = new System.Drawing.Size(318, 20);
            this.tbWorkshopSampleFilePath.TabIndex = 4;
            this.tbWorkshopSampleFilePath.Text = "pickles";
            this.tbWorkshopSampleFilePath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbNIFBuilderNIFFilePathSampleMouseClick);
            // 
            // lblWorkshopMeshSubDirectory
            // 
            this.lblWorkshopMeshSubDirectory.Location = new System.Drawing.Point(6, 69);
            this.lblWorkshopMeshSubDirectory.Name = "lblWorkshopMeshSubDirectory";
            this.lblWorkshopMeshSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopMeshSubDirectory.TabIndex = 5;
            this.lblWorkshopMeshSubDirectory.Tag = "BorderBatchWindow.NodesAndNIFs.MeshSubDirectory:";
            this.lblWorkshopMeshSubDirectory.Text = "Mesh Sub-Folder:";
            // 
            // lblWorkshopFilePrefix
            // 
            this.lblWorkshopFilePrefix.Location = new System.Drawing.Point(6, 92);
            this.lblWorkshopFilePrefix.Name = "lblWorkshopFilePrefix";
            this.lblWorkshopFilePrefix.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopFilePrefix.TabIndex = 0;
            this.lblWorkshopFilePrefix.Tag = "BorderBatchWindow.NodesAndNIFs.FilePrefix:";
            this.lblWorkshopFilePrefix.Text = "File Prefix:";
            // 
            // lvWorkshops
            // 
            this.lvWorkshops.AllowHidingItems = true;
            this.lvWorkshops.AllowOverrideColumnSorting = true;
            this.lvWorkshops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWorkshops.CheckBoxes = true;
            this.lvWorkshops.EditorIDColumn = true;
            this.lvWorkshops.ExtraInfoColumn = false;
            this.lvWorkshops.FilenameColumn = false;
            this.lvWorkshops.FormIDColumn = true;
            this.lvWorkshops.LoadOrderColumn = false;
            this.lvWorkshops.Location = new System.Drawing.Point(336, 0);
            this.lvWorkshops.MultiSelect = true;
            this.lvWorkshops.Name = "lvWorkshops";
            this.lvWorkshops.Size = new System.Drawing.Size(328, 534);
            this.lvWorkshops.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvWorkshops.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvWorkshops.SyncedEditorFormType = null;
            this.lvWorkshops.SyncObjects = null;
            this.lvWorkshops.TabIndex = 11;
            this.lvWorkshops.TypeColumn = false;
            // 
            // gbBorderFunctions
            // 
            this.gbBorderFunctions.Controls.Add(this.btnImportNIFs);
            this.gbBorderFunctions.Controls.Add(this.btnClear);
            this.gbBorderFunctions.Controls.Add(this.btnGenNodes);
            this.gbBorderFunctions.Controls.Add(this.btnBuildNIFs);
            this.gbBorderFunctions.Location = new System.Drawing.Point(3, 6);
            this.gbBorderFunctions.Name = "gbBorderFunctions";
            this.gbBorderFunctions.Size = new System.Drawing.Size(330, 68);
            this.gbBorderFunctions.TabIndex = 9;
            this.gbBorderFunctions.TabStop = false;
            this.gbBorderFunctions.Tag = "BorderBatchWindow.Functions";
            this.gbBorderFunctions.Text = "Functions";
            // 
            // btnImportNIFs
            // 
            this.btnImportNIFs.Location = new System.Drawing.Point(249, 15);
            this.btnImportNIFs.Name = "btnImportNIFs";
            this.btnImportNIFs.Size = new System.Drawing.Size(75, 23);
            this.btnImportNIFs.TabIndex = 3;
            this.btnImportNIFs.Tag = "BorderBatchWindow.Function.ImportNIFs";
            this.btnImportNIFs.Text = "Import NIFs";
            this.btnImportNIFs.UseVisualStyleBackColor = true;
            this.btnImportNIFs.Click += new System.EventHandler(this.btnImportNIFsClick);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(6, 15);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 0;
            this.btnClear.Tag = "BorderBatchWindow.Function.ClearNodes";
            this.btnClear.Text = "Clear Nodes";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClearClick);
            // 
            // btnGenNodes
            // 
            this.btnGenNodes.Location = new System.Drawing.Point(87, 15);
            this.btnGenNodes.Name = "btnGenNodes";
            this.btnGenNodes.Size = new System.Drawing.Size(75, 23);
            this.btnGenNodes.TabIndex = 1;
            this.btnGenNodes.Tag = "BorderBatchWindow.Function.GenerateNodes";
            this.btnGenNodes.Text = "Gen. Nodes";
            this.btnGenNodes.UseVisualStyleBackColor = true;
            this.btnGenNodes.Click += new System.EventHandler(this.btnGenNodesClick);
            // 
            // btnBuildNIFs
            // 
            this.btnBuildNIFs.Location = new System.Drawing.Point(168, 15);
            this.btnBuildNIFs.Name = "btnBuildNIFs";
            this.btnBuildNIFs.Size = new System.Drawing.Size(75, 23);
            this.btnBuildNIFs.TabIndex = 2;
            this.btnBuildNIFs.Tag = "BorderBatchWindow.Function.BuildNIFs";
            this.btnBuildNIFs.Text = "Build NIFs";
            this.btnBuildNIFs.UseVisualStyleBackColor = true;
            this.btnBuildNIFs.Click += new System.EventHandler(this.btnBuildNIFsClick);
            // 
            // BorderBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 641);
            this.Controls.Add(this.pnWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(680, 665);
            this.Name = "BorderBatch";
            this.ShowInTaskbar = false;
            this.Tag = "BorderBatchWindow.Title";
            this.Text = "title";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.pnWindow.ResumeLayout(false);
            this.gbTargetFolder.ResumeLayout(false);
            this.gbTargetFolder.PerformLayout();
            this.tcObjectSelect.ResumeLayout(false);
            this.tpSubDivisions.ResumeLayout(false);
            this.gbSubDivisionNodeAndNIFGeneration.ResumeLayout(false);
            this.gbSubDivisionNodeAndNIFGeneration.PerformLayout();
            this.tpWorkshops.ResumeLayout(false);
            this.gbWorkshopNodeDetection.ResumeLayout(false);
            this.gbWorkshopNodeDetectionStaticMarkers.ResumeLayout(false);
            this.gbWorkshopNodeDetectionKeywords.ResumeLayout(false);
            this.gbWorkshopNodeAndNIFGeneration.ResumeLayout(false);
            this.gbWorkshopNodeAndNIFGeneration.PerformLayout();
            this.gbBorderFunctions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
