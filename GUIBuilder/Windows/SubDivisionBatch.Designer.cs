/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class SubDivisionBatch
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbSubDivisionFunctions;
        System.Windows.Forms.Button btnOptimizeSandboxVolumes;
        System.Windows.Forms.Button btnCheckMissingElements;
        GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> lvSubDivisions;
        System.Windows.Forms.Button btnNormalizeBuildVolumes;
        System.Windows.Forms.GroupBox gbElements;
        System.Windows.Forms.CheckBox cbElementBorderEnablers;
        System.Windows.Forms.CheckBox cbElementSandboxVolumes;
        private System.Windows.Forms.GroupBox gbNormalizeBuildVolumes;
        private System.Windows.Forms.GroupBox gbNormalizeBuildVolumesScanTerrain;
        private System.Windows.Forms.TextBox tbNormalizeBuildVolumesTopAbovePeak;
        private System.Windows.Forms.CheckBox cbNormalizeBuildVolumesScanTerrain;
        private System.Windows.Forms.TextBox tbNormalizeBuildVolumesGroundSink;
        private System.Windows.Forms.Label lblNormalizeBuildVolumesTopAbovePeak;
        private System.Windows.Forms.Label lblNormalizeBuildVolumesGroundSink;
        private System.Windows.Forms.GroupBox gbOptimizeSandboxVolumes;
        private System.Windows.Forms.TextBox tbOptimizeSandboxVolumesCylinderTop;
        private System.Windows.Forms.TextBox tbOptimizeSandboxVolumesCylinderBottom;
        private System.Windows.Forms.Label lblOptimizeSandboxVolumesCylinderTop;
        private System.Windows.Forms.Label lblOptimizeSandboxVolumesCylinderBottom;
        private System.Windows.Forms.TextBox tbOptimizeSandboxVolumesVolumePadding;
        private System.Windows.Forms.Label lblOptimizeSandboxVolumesVolumePadding;
        private System.Windows.Forms.CheckBox cbOptimizeSandboxVolumesIgnoreExisting;
        private System.Windows.Forms.CheckBox cbOptimizeSandboxVolumesCreateNew;
        private System.Windows.Forms.CheckBox cbOptimizeSandboxVolumesScanTerrain;
        
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
            this.lvSubDivisions = new GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision>();
            this.gbSubDivisionFunctions = new System.Windows.Forms.GroupBox();
            this.gbOptimizeSandboxVolumes = new System.Windows.Forms.GroupBox();
            this.cbOptimizeSandboxVolumesIgnoreExisting = new System.Windows.Forms.CheckBox();
            this.cbOptimizeSandboxVolumesCreateNew = new System.Windows.Forms.CheckBox();
            this.cbOptimizeSandboxVolumesScanTerrain = new System.Windows.Forms.CheckBox();
            this.tbOptimizeSandboxVolumesVolumePadding = new System.Windows.Forms.TextBox();
            this.lblOptimizeSandboxVolumesVolumePadding = new System.Windows.Forms.Label();
            this.tbOptimizeSandboxVolumesCylinderTop = new System.Windows.Forms.TextBox();
            this.tbOptimizeSandboxVolumesCylinderBottom = new System.Windows.Forms.TextBox();
            this.lblOptimizeSandboxVolumesCylinderTop = new System.Windows.Forms.Label();
            this.lblOptimizeSandboxVolumesCylinderBottom = new System.Windows.Forms.Label();
            this.btnOptimizeSandboxVolumes = new System.Windows.Forms.Button();
            this.gbNormalizeBuildVolumes = new System.Windows.Forms.GroupBox();
            this.gbNormalizeBuildVolumesScanTerrain = new System.Windows.Forms.GroupBox();
            this.tbNormalizeBuildVolumesTopAbovePeak = new System.Windows.Forms.TextBox();
            this.cbNormalizeBuildVolumesScanTerrain = new System.Windows.Forms.CheckBox();
            this.tbNormalizeBuildVolumesGroundSink = new System.Windows.Forms.TextBox();
            this.lblNormalizeBuildVolumesTopAbovePeak = new System.Windows.Forms.Label();
            this.lblNormalizeBuildVolumesGroundSink = new System.Windows.Forms.Label();
            this.btnNormalizeBuildVolumes = new System.Windows.Forms.Button();
            this.gbElements = new System.Windows.Forms.GroupBox();
            this.btnCheckMissingElements = new System.Windows.Forms.Button();
            this.cbElementSandboxVolumes = new System.Windows.Forms.CheckBox();
            this.cbElementBorderEnablers = new System.Windows.Forms.CheckBox();
            this.WindowPanel.SuspendLayout();
            this.gbSubDivisionFunctions.SuspendLayout();
            this.gbOptimizeSandboxVolumes.SuspendLayout();
            this.gbNormalizeBuildVolumes.SuspendLayout();
            this.gbNormalizeBuildVolumesScanTerrain.SuspendLayout();
            this.gbElements.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.lvSubDivisions);
            this.WindowPanel.Controls.Add(this.gbSubDivisionFunctions);
            this.WindowPanel.Size = new System.Drawing.Size(514, 404);
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
            this.lvSubDivisions.Location = new System.Drawing.Point(189, 3);
            this.lvSubDivisions.MultiSelect = true;
            this.lvSubDivisions.Name = "lvSubDivisions";
            this.lvSubDivisions.Size = new System.Drawing.Size(323, 399);
            this.lvSubDivisions.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvSubDivisions.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvSubDivisions.SyncedEditorFormType = null;
            this.lvSubDivisions.SyncObjects = null;
            this.lvSubDivisions.TabIndex = 11;
            this.lvSubDivisions.TypeColumn = false;
            // 
            // gbSubDivisionFunctions
            // 
            this.gbSubDivisionFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbSubDivisionFunctions.Controls.Add(this.gbOptimizeSandboxVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.gbNormalizeBuildVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.gbElements);
            this.gbSubDivisionFunctions.Location = new System.Drawing.Point(0, 0);
            this.gbSubDivisionFunctions.Name = "gbSubDivisionFunctions";
            this.gbSubDivisionFunctions.Size = new System.Drawing.Size(180, 402);
            this.gbSubDivisionFunctions.TabIndex = 12;
            this.gbSubDivisionFunctions.TabStop = false;
            // 
            // gbOptimizeSandboxVolumes
            // 
            this.gbOptimizeSandboxVolumes.Controls.Add(this.cbOptimizeSandboxVolumesIgnoreExisting);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.cbOptimizeSandboxVolumesCreateNew);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.cbOptimizeSandboxVolumesScanTerrain);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.tbOptimizeSandboxVolumesVolumePadding);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.lblOptimizeSandboxVolumesVolumePadding);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.tbOptimizeSandboxVolumesCylinderTop);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.tbOptimizeSandboxVolumesCylinderBottom);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.lblOptimizeSandboxVolumesCylinderTop);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.lblOptimizeSandboxVolumesCylinderBottom);
            this.gbOptimizeSandboxVolumes.Controls.Add(this.btnOptimizeSandboxVolumes);
            this.gbOptimizeSandboxVolumes.Location = new System.Drawing.Point(6, 200);
            this.gbOptimizeSandboxVolumes.Name = "gbOptimizeSandboxVolumes";
            this.gbOptimizeSandboxVolumes.Size = new System.Drawing.Size(168, 153);
            this.gbOptimizeSandboxVolumes.TabIndex = 15;
            this.gbOptimizeSandboxVolumes.TabStop = false;
            // 
            // cbOptimizeSandboxVolumesIgnoreExisting
            // 
            this.cbOptimizeSandboxVolumesIgnoreExisting.Checked = true;
            this.cbOptimizeSandboxVolumesIgnoreExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOptimizeSandboxVolumesIgnoreExisting.Location = new System.Drawing.Point(12, 48);
            this.cbOptimizeSandboxVolumesIgnoreExisting.Name = "cbOptimizeSandboxVolumesIgnoreExisting";
            this.cbOptimizeSandboxVolumesIgnoreExisting.Size = new System.Drawing.Size(141, 17);
            this.cbOptimizeSandboxVolumesIgnoreExisting.TabIndex = 23;
            this.cbOptimizeSandboxVolumesIgnoreExisting.Tag = "BatchWindow.Function.OptimizeSandboxVolumes.IgnoreExisting";
            this.cbOptimizeSandboxVolumesIgnoreExisting.Text = "Ignore Existing";
            this.cbOptimizeSandboxVolumesIgnoreExisting.UseVisualStyleBackColor = true;
            // 
            // cbOptimizeSandboxVolumesCreateNew
            // 
            this.cbOptimizeSandboxVolumesCreateNew.Checked = true;
            this.cbOptimizeSandboxVolumesCreateNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOptimizeSandboxVolumesCreateNew.Location = new System.Drawing.Point(12, 29);
            this.cbOptimizeSandboxVolumesCreateNew.Name = "cbOptimizeSandboxVolumesCreateNew";
            this.cbOptimizeSandboxVolumesCreateNew.Size = new System.Drawing.Size(141, 17);
            this.cbOptimizeSandboxVolumesCreateNew.TabIndex = 22;
            this.cbOptimizeSandboxVolumesCreateNew.Tag = "BatchWindow.Function.OptimizeSandboxVolumes.CreateNew";
            this.cbOptimizeSandboxVolumesCreateNew.Text = "Create New";
            this.cbOptimizeSandboxVolumesCreateNew.UseVisualStyleBackColor = true;
            // 
            // cbOptimizeSandboxVolumesScanTerrain
            // 
            this.cbOptimizeSandboxVolumesScanTerrain.Checked = true;
            this.cbOptimizeSandboxVolumesScanTerrain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOptimizeSandboxVolumesScanTerrain.Location = new System.Drawing.Point(12, 67);
            this.cbOptimizeSandboxVolumesScanTerrain.Name = "cbOptimizeSandboxVolumesScanTerrain";
            this.cbOptimizeSandboxVolumesScanTerrain.Size = new System.Drawing.Size(141, 17);
            this.cbOptimizeSandboxVolumesScanTerrain.TabIndex = 21;
            this.cbOptimizeSandboxVolumesScanTerrain.Tag = "BatchWindow.Function.Volumes.ScanTerrain";
            this.cbOptimizeSandboxVolumesScanTerrain.Text = "Scan Terrain";
            this.cbOptimizeSandboxVolumesScanTerrain.UseVisualStyleBackColor = true;
            // 
            // tbOptimizeSandboxVolumesVolumePadding
            // 
            this.tbOptimizeSandboxVolumesVolumePadding.Location = new System.Drawing.Point(96, 128);
            this.tbOptimizeSandboxVolumesVolumePadding.Name = "tbOptimizeSandboxVolumesVolumePadding";
            this.tbOptimizeSandboxVolumesVolumePadding.Size = new System.Drawing.Size(60, 20);
            this.tbOptimizeSandboxVolumesVolumePadding.TabIndex = 18;
            this.tbOptimizeSandboxVolumesVolumePadding.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblOptimizeSandboxVolumesVolumePadding
            // 
            this.lblOptimizeSandboxVolumesVolumePadding.Location = new System.Drawing.Point(12, 131);
            this.lblOptimizeSandboxVolumesVolumePadding.Name = "lblOptimizeSandboxVolumesVolumePadding";
            this.lblOptimizeSandboxVolumesVolumePadding.Size = new System.Drawing.Size(95, 17);
            this.lblOptimizeSandboxVolumesVolumePadding.TabIndex = 20;
            this.lblOptimizeSandboxVolumesVolumePadding.Tag = "BatchWindow.Function.OptimizeSandboxVolumes.VolumePadding";
            this.lblOptimizeSandboxVolumesVolumePadding.Text = "Volume Padding";
            // 
            // tbOptimizeSandboxVolumesCylinderTop
            // 
            this.tbOptimizeSandboxVolumesCylinderTop.Location = new System.Drawing.Point(96, 86);
            this.tbOptimizeSandboxVolumesCylinderTop.Name = "tbOptimizeSandboxVolumesCylinderTop";
            this.tbOptimizeSandboxVolumesCylinderTop.Size = new System.Drawing.Size(60, 20);
            this.tbOptimizeSandboxVolumesCylinderTop.TabIndex = 9;
            this.tbOptimizeSandboxVolumesCylinderTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbOptimizeSandboxVolumesCylinderBottom
            // 
            this.tbOptimizeSandboxVolumesCylinderBottom.Location = new System.Drawing.Point(96, 107);
            this.tbOptimizeSandboxVolumesCylinderBottom.Name = "tbOptimizeSandboxVolumesCylinderBottom";
            this.tbOptimizeSandboxVolumesCylinderBottom.Size = new System.Drawing.Size(60, 20);
            this.tbOptimizeSandboxVolumesCylinderBottom.TabIndex = 10;
            this.tbOptimizeSandboxVolumesCylinderBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblOptimizeSandboxVolumesCylinderTop
            // 
            this.lblOptimizeSandboxVolumesCylinderTop.Location = new System.Drawing.Point(12, 89);
            this.lblOptimizeSandboxVolumesCylinderTop.Name = "lblOptimizeSandboxVolumesCylinderTop";
            this.lblOptimizeSandboxVolumesCylinderTop.Size = new System.Drawing.Size(95, 17);
            this.lblOptimizeSandboxVolumesCylinderTop.TabIndex = 11;
            this.lblOptimizeSandboxVolumesCylinderTop.Tag = "BatchWindow.Function.OptimizeSandboxVolumes.CylinderTop";
            this.lblOptimizeSandboxVolumesCylinderTop.Text = "Cylinder Top";
            // 
            // lblOptimizeSandboxVolumesCylinderBottom
            // 
            this.lblOptimizeSandboxVolumesCylinderBottom.Location = new System.Drawing.Point(12, 110);
            this.lblOptimizeSandboxVolumesCylinderBottom.Name = "lblOptimizeSandboxVolumesCylinderBottom";
            this.lblOptimizeSandboxVolumesCylinderBottom.Size = new System.Drawing.Size(95, 17);
            this.lblOptimizeSandboxVolumesCylinderBottom.TabIndex = 12;
            this.lblOptimizeSandboxVolumesCylinderBottom.Tag = "BatchWindow.Function.OptimizeSandboxVolumes.CylinderBottom";
            this.lblOptimizeSandboxVolumesCylinderBottom.Text = "Cylinder Bottom";
            // 
            // btnOptimizeSandboxVolumes
            // 
            this.btnOptimizeSandboxVolumes.Location = new System.Drawing.Point(6, 0);
            this.btnOptimizeSandboxVolumes.Name = "btnOptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnOptimizeSandboxVolumes.TabIndex = 0;
            this.btnOptimizeSandboxVolumes.Tag = "BatchWindow.Function.OptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Text = "Optimize Sandboxes";
            this.btnOptimizeSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // gbNormalizeBuildVolumes
            // 
            this.gbNormalizeBuildVolumes.Controls.Add(this.gbNormalizeBuildVolumesScanTerrain);
            this.gbNormalizeBuildVolumes.Controls.Add(this.btnNormalizeBuildVolumes);
            this.gbNormalizeBuildVolumes.Location = new System.Drawing.Point(6, 90);
            this.gbNormalizeBuildVolumes.Name = "gbNormalizeBuildVolumes";
            this.gbNormalizeBuildVolumes.Size = new System.Drawing.Size(168, 104);
            this.gbNormalizeBuildVolumes.TabIndex = 14;
            this.gbNormalizeBuildVolumes.TabStop = false;
            // 
            // gbNormalizeBuildVolumesScanTerrain
            // 
            this.gbNormalizeBuildVolumesScanTerrain.Controls.Add(this.tbNormalizeBuildVolumesTopAbovePeak);
            this.gbNormalizeBuildVolumesScanTerrain.Controls.Add(this.cbNormalizeBuildVolumesScanTerrain);
            this.gbNormalizeBuildVolumesScanTerrain.Controls.Add(this.tbNormalizeBuildVolumesGroundSink);
            this.gbNormalizeBuildVolumesScanTerrain.Controls.Add(this.lblNormalizeBuildVolumesTopAbovePeak);
            this.gbNormalizeBuildVolumesScanTerrain.Controls.Add(this.lblNormalizeBuildVolumesGroundSink);
            this.gbNormalizeBuildVolumesScanTerrain.Location = new System.Drawing.Point(6, 29);
            this.gbNormalizeBuildVolumesScanTerrain.Name = "gbNormalizeBuildVolumesScanTerrain";
            this.gbNormalizeBuildVolumesScanTerrain.Size = new System.Drawing.Size(156, 70);
            this.gbNormalizeBuildVolumesScanTerrain.TabIndex = 7;
            this.gbNormalizeBuildVolumesScanTerrain.TabStop = false;
            // 
            // tbNormalizeBuildVolumesTopAbovePeak
            // 
            this.tbNormalizeBuildVolumesTopAbovePeak.Location = new System.Drawing.Point(90, 23);
            this.tbNormalizeBuildVolumesTopAbovePeak.Name = "tbNormalizeBuildVolumesTopAbovePeak";
            this.tbNormalizeBuildVolumesTopAbovePeak.Size = new System.Drawing.Size(60, 20);
            this.tbNormalizeBuildVolumesTopAbovePeak.TabIndex = 4;
            this.tbNormalizeBuildVolumesTopAbovePeak.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbNormalizeBuildVolumesScanTerrain
            // 
            this.cbNormalizeBuildVolumesScanTerrain.Checked = true;
            this.cbNormalizeBuildVolumesScanTerrain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNormalizeBuildVolumesScanTerrain.Location = new System.Drawing.Point(6, 0);
            this.cbNormalizeBuildVolumesScanTerrain.Name = "cbNormalizeBuildVolumesScanTerrain";
            this.cbNormalizeBuildVolumesScanTerrain.Size = new System.Drawing.Size(141, 17);
            this.cbNormalizeBuildVolumesScanTerrain.TabIndex = 6;
            this.cbNormalizeBuildVolumesScanTerrain.Tag = "BatchWindow.Function.Volumes.ScanTerrain";
            this.cbNormalizeBuildVolumesScanTerrain.Text = "Scan Terrain";
            this.cbNormalizeBuildVolumesScanTerrain.UseVisualStyleBackColor = true;
            // 
            // tbNormalizeBuildVolumesGroundSink
            // 
            this.tbNormalizeBuildVolumesGroundSink.Location = new System.Drawing.Point(90, 44);
            this.tbNormalizeBuildVolumesGroundSink.Name = "tbNormalizeBuildVolumesGroundSink";
            this.tbNormalizeBuildVolumesGroundSink.Size = new System.Drawing.Size(60, 20);
            this.tbNormalizeBuildVolumesGroundSink.TabIndex = 5;
            this.tbNormalizeBuildVolumesGroundSink.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNormalizeBuildVolumesTopAbovePeak
            // 
            this.lblNormalizeBuildVolumesTopAbovePeak.Location = new System.Drawing.Point(6, 26);
            this.lblNormalizeBuildVolumesTopAbovePeak.Name = "lblNormalizeBuildVolumesTopAbovePeak";
            this.lblNormalizeBuildVolumesTopAbovePeak.Size = new System.Drawing.Size(95, 17);
            this.lblNormalizeBuildVolumesTopAbovePeak.TabIndex = 7;
            this.lblNormalizeBuildVolumesTopAbovePeak.Tag = "BatchWindow.Function.NormalizeBuildVolumes.TopAbovePeak";
            this.lblNormalizeBuildVolumesTopAbovePeak.Text = "Top above peak";
            // 
            // lblNormalizeBuildVolumesGroundSink
            // 
            this.lblNormalizeBuildVolumesGroundSink.Location = new System.Drawing.Point(6, 47);
            this.lblNormalizeBuildVolumesGroundSink.Name = "lblNormalizeBuildVolumesGroundSink";
            this.lblNormalizeBuildVolumesGroundSink.Size = new System.Drawing.Size(95, 17);
            this.lblNormalizeBuildVolumesGroundSink.TabIndex = 8;
            this.lblNormalizeBuildVolumesGroundSink.Tag = "BatchWindow.Function.NormalizeBuildVolumes.GroundSink";
            this.lblNormalizeBuildVolumesGroundSink.Text = "Ground sink";
            // 
            // btnNormalizeBuildVolumes
            // 
            this.btnNormalizeBuildVolumes.Location = new System.Drawing.Point(6, 0);
            this.btnNormalizeBuildVolumes.Name = "btnNormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnNormalizeBuildVolumes.TabIndex = 3;
            this.btnNormalizeBuildVolumes.Tag = "BatchWindow.Function.NormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Text = "Normalize Build Volumes";
            this.btnNormalizeBuildVolumes.UseVisualStyleBackColor = true;
            // 
            // gbElements
            // 
            this.gbElements.Controls.Add(this.btnCheckMissingElements);
            this.gbElements.Controls.Add(this.cbElementSandboxVolumes);
            this.gbElements.Controls.Add(this.cbElementBorderEnablers);
            this.gbElements.Location = new System.Drawing.Point(6, 12);
            this.gbElements.Name = "gbElements";
            this.gbElements.Size = new System.Drawing.Size(168, 72);
            this.gbElements.TabIndex = 12;
            this.gbElements.TabStop = false;
            this.gbElements.Tag = "Workshop.Elements";
            // 
            // btnCheckMissingElements
            // 
            this.btnCheckMissingElements.Location = new System.Drawing.Point(6, 0);
            this.btnCheckMissingElements.Name = "btnCheckMissingElements";
            this.btnCheckMissingElements.Size = new System.Drawing.Size(156, 23);
            this.btnCheckMissingElements.TabIndex = 1;
            this.btnCheckMissingElements.Tag = "BatchWindow.Function.CheckElements";
            this.btnCheckMissingElements.Text = "Check Missing Elements";
            this.btnCheckMissingElements.UseVisualStyleBackColor = true;
            // 
            // cbElementSandboxVolumes
            // 
            this.cbElementSandboxVolumes.Checked = true;
            this.cbElementSandboxVolumes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementSandboxVolumes.Location = new System.Drawing.Point(12, 48);
            this.cbElementSandboxVolumes.Name = "cbElementSandboxVolumes";
            this.cbElementSandboxVolumes.Size = new System.Drawing.Size(141, 17);
            this.cbElementSandboxVolumes.TabIndex = 2;
            this.cbElementSandboxVolumes.Tag = "Workshop.Element.SandboxVolumes";
            this.cbElementSandboxVolumes.Text = "Sandbox Volumes";
            this.cbElementSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // cbElementBorderEnablers
            // 
            this.cbElementBorderEnablers.Checked = true;
            this.cbElementBorderEnablers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementBorderEnablers.Location = new System.Drawing.Point(12, 29);
            this.cbElementBorderEnablers.Name = "cbElementBorderEnablers";
            this.cbElementBorderEnablers.Size = new System.Drawing.Size(141, 17);
            this.cbElementBorderEnablers.TabIndex = 0;
            this.cbElementBorderEnablers.Tag = "SubDivisionBatchWindow.BorderEnablers";
            this.cbElementBorderEnablers.Text = "Border Enablers";
            this.cbElementBorderEnablers.UseVisualStyleBackColor = true;
            // 
            // SubDivisionBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 404);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 426);
            this.Name = "SubDivisionBatch";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "SubDivisionBatchWindow.Title";
            this.WindowPanel.ResumeLayout(false);
            this.gbSubDivisionFunctions.ResumeLayout(false);
            this.gbOptimizeSandboxVolumes.ResumeLayout(false);
            this.gbOptimizeSandboxVolumes.PerformLayout();
            this.gbNormalizeBuildVolumes.ResumeLayout(false);
            this.gbNormalizeBuildVolumesScanTerrain.ResumeLayout(false);
            this.gbNormalizeBuildVolumesScanTerrain.PerformLayout();
            this.gbElements.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
