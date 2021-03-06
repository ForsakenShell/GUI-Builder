﻿/*
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
        System.Windows.Forms.Label lbSubDivisionNodeLength;
        System.Windows.Forms.Button btnGenerateNodes;
        System.Windows.Forms.TextBox tbSubDivisionNodeLength;
        System.Windows.Forms.Button btnClearNodes;
        System.Windows.Forms.Button btnBuildNIFs;
        
        System.Windows.Forms.TextBox tbSubDivisionNIFGroundOffset;
        System.Windows.Forms.TextBox tbSubDivisionNIFGradientHeight;
        System.Windows.Forms.Label lblSubDivisionNIFGradientHeight;
        System.Windows.Forms.Label lblSubDivisionNIFGroundSink;
        System.Windows.Forms.Label lblSubDivisionNIFGroundOffset;
        System.Windows.Forms.GroupBox gbTargetFolder;
        System.Windows.Forms.TextBox tbTargetFolder;
        System.Windows.Forms.TextBox tbSubDivisionNIFGroundSink;
        System.Windows.Forms.GroupBox gbBorderFunctions;
        System.Windows.Forms.Button btnImportNIFs;
        System.Windows.Forms.TextBox tbWorkshopNIFSampleFilePath;
        System.Windows.Forms.TextBox tbWorkshopNIFMeshSubDirectory;
        System.Windows.Forms.CheckBox cbSubDivisionNIFCreateImportData;
        GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> lvSubDivisions;
        System.Windows.Forms.TabControl tcObjectSelect;
        System.Windows.Forms.TabPage tpWorkshops;
        GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript> lvWorkshops;
        System.Windows.Forms.TabPage tpSubDivisions;
        System.Windows.Forms.Label lbSubDivisionNodeSlopeAllowance;
        System.Windows.Forms.TextBox tbSubDivisionNodeSlopeAllowance;
        System.Windows.Forms.GroupBox gbSubDivisionNIFParameters;
        System.Windows.Forms.TextBox tbSubDivisionNIFMeshSubDirectory;
        System.Windows.Forms.TextBox tbNIFBuilderSubDivisionNIFSampleFilePath;
        System.Windows.Forms.GroupBox gbWorkshopNIFParameters;
        System.Windows.Forms.Label lblSubDivisionNIFMeshSubDirectory;
        System.Windows.Forms.Label lblWorkshopNIFMeshSubDirectory;
        System.Windows.Forms.ComboBox cbSubDivisionPresets;
        System.Windows.Forms.Label lblSubDivisionPresets;
        System.Windows.Forms.TextBox tbWorkshopNIFGroundSink;
        System.Windows.Forms.TextBox tbWorkshopNIFGroundOffset;
        System.Windows.Forms.TextBox tbWorkshopNIFGradientHeight;
        System.Windows.Forms.Label lblWorkshopNIFGradientHeight;
        System.Windows.Forms.Label lblWorkshopNIFGroundSink;
        System.Windows.Forms.Label lblWorkshopNIFGroundOffset;
        System.Windows.Forms.Label lblWorkshopNodeSlopeAllowance;
        System.Windows.Forms.TextBox tbWorkshopNodeSlopeAllowance;
        System.Windows.Forms.Label lblWorkshopNodeLength;
        System.Windows.Forms.TextBox tbWorkshopNodeLength;
        System.Windows.Forms.ComboBox cbWorkshopPresets;
        System.Windows.Forms.Label lblWorkshopPresets;
        System.Windows.Forms.TextBox tbSubDivisionNIFTargetSubDirectory;
        System.Windows.Forms.Label lblSubDivisionNIFTargetSubDirectory;
        System.Windows.Forms.TextBox tbWorkshopNIFTargetSubDirectory;
        System.Windows.Forms.Label lblWorkshopNIFTargetSubDirectory;
        System.Windows.Forms.CheckBox cbWorkshopNIFCreateImportData;
        private System.Windows.Forms.GroupBox gbSubDivisionNodeParameters;
        private System.Windows.Forms.GroupBox gbWorkshopNodeParameters;
        private System.Windows.Forms.TextBox tbSubDivisionNodeAngleAllowance;
        private System.Windows.Forms.Label lbSubDivisionNodeAngleAllowance;
        private System.Windows.Forms.TextBox tbWorkshopNodeAngleAllowance;
        private System.Windows.Forms.Label lblWorkshopNodeAngleAllowance;
        private System.Windows.Forms.CheckBox cbSubDivisionNIFHighPrecisionVertexes;
        private System.Windows.Forms.CheckBox cbWorkshopNIFHighPrecisionVertexes;
        private System.Windows.Forms.CheckBox cbWorkshopNIFUseExistingFilePaths;
        private System.Windows.Forms.CheckBox cbWorkshopNIFUseExistingSTATEditorIDs;
        
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
            this.gbTargetFolder = new System.Windows.Forms.GroupBox();
            this.tbTargetFolder = new System.Windows.Forms.TextBox();
            this.tcObjectSelect = new System.Windows.Forms.TabControl();
            this.tpSubDivisions = new System.Windows.Forms.TabPage();
            this.gbSubDivisionNodeParameters = new System.Windows.Forms.GroupBox();
            this.tbSubDivisionNodeLength = new System.Windows.Forms.TextBox();
            this.tbSubDivisionNodeAngleAllowance = new System.Windows.Forms.TextBox();
            this.lbSubDivisionNodeAngleAllowance = new System.Windows.Forms.Label();
            this.tbSubDivisionNodeSlopeAllowance = new System.Windows.Forms.TextBox();
            this.lbSubDivisionNodeLength = new System.Windows.Forms.Label();
            this.lbSubDivisionNodeSlopeAllowance = new System.Windows.Forms.Label();
            this.gbSubDivisionNIFParameters = new System.Windows.Forms.GroupBox();
            this.tbSubDivisionNIFGroundSink = new System.Windows.Forms.TextBox();
            this.cbSubDivisionNIFCreateImportData = new System.Windows.Forms.CheckBox();
            this.tbSubDivisionNIFTargetSubDirectory = new System.Windows.Forms.TextBox();
            this.lblSubDivisionNIFTargetSubDirectory = new System.Windows.Forms.Label();
            this.tbSubDivisionNIFGroundOffset = new System.Windows.Forms.TextBox();
            this.tbSubDivisionNIFMeshSubDirectory = new System.Windows.Forms.TextBox();
            this.tbNIFBuilderSubDivisionNIFSampleFilePath = new System.Windows.Forms.TextBox();
            this.tbSubDivisionNIFGradientHeight = new System.Windows.Forms.TextBox();
            this.lblSubDivisionNIFGroundOffset = new System.Windows.Forms.Label();
            this.lblSubDivisionNIFMeshSubDirectory = new System.Windows.Forms.Label();
            this.lblSubDivisionNIFGradientHeight = new System.Windows.Forms.Label();
            this.lblSubDivisionNIFGroundSink = new System.Windows.Forms.Label();
            this.cbSubDivisionNIFHighPrecisionVertexes = new System.Windows.Forms.CheckBox();
            this.lvSubDivisions = new GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision>();
            this.cbSubDivisionPresets = new System.Windows.Forms.ComboBox();
            this.lblSubDivisionPresets = new System.Windows.Forms.Label();
            this.tpWorkshops = new System.Windows.Forms.TabPage();
            this.gbWorkshopNodeParameters = new System.Windows.Forms.GroupBox();
            this.tbWorkshopNodeLength = new System.Windows.Forms.TextBox();
            this.tbWorkshopNodeAngleAllowance = new System.Windows.Forms.TextBox();
            this.lblWorkshopNodeAngleAllowance = new System.Windows.Forms.Label();
            this.lblWorkshopNodeLength = new System.Windows.Forms.Label();
            this.tbWorkshopNodeSlopeAllowance = new System.Windows.Forms.TextBox();
            this.lblWorkshopNodeSlopeAllowance = new System.Windows.Forms.Label();
            this.cbWorkshopPresets = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNIFParameters = new System.Windows.Forms.GroupBox();
            this.cbWorkshopNIFHighPrecisionVertexes = new System.Windows.Forms.CheckBox();
            this.cbWorkshopNIFCreateImportData = new System.Windows.Forms.CheckBox();
            this.tbWorkshopNIFGroundSink = new System.Windows.Forms.TextBox();
            this.tbWorkshopNIFGroundOffset = new System.Windows.Forms.TextBox();
            this.tbWorkshopNIFMeshSubDirectory = new System.Windows.Forms.TextBox();
            this.tbWorkshopNIFTargetSubDirectory = new System.Windows.Forms.TextBox();
            this.lblWorkshopNIFTargetSubDirectory = new System.Windows.Forms.Label();
            this.tbWorkshopNIFGradientHeight = new System.Windows.Forms.TextBox();
            this.tbWorkshopNIFSampleFilePath = new System.Windows.Forms.TextBox();
            this.lblWorkshopNIFGradientHeight = new System.Windows.Forms.Label();
            this.lblWorkshopNIFMeshSubDirectory = new System.Windows.Forms.Label();
            this.lblWorkshopNIFGroundOffset = new System.Windows.Forms.Label();
            this.lblWorkshopNIFGroundSink = new System.Windows.Forms.Label();
            this.lvWorkshops = new GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript>();
            this.lblWorkshopPresets = new System.Windows.Forms.Label();
            this.gbBorderFunctions = new System.Windows.Forms.GroupBox();
            this.btnImportNIFs = new System.Windows.Forms.Button();
            this.btnClearNodes = new System.Windows.Forms.Button();
            this.btnGenerateNodes = new System.Windows.Forms.Button();
            this.btnBuildNIFs = new System.Windows.Forms.Button();
            this.cbWorkshopNIFUseExistingSTATEditorIDs = new System.Windows.Forms.CheckBox();
            this.cbWorkshopNIFUseExistingFilePaths = new System.Windows.Forms.CheckBox();
            this.WindowPanel.SuspendLayout();
            this.gbTargetFolder.SuspendLayout();
            this.tcObjectSelect.SuspendLayout();
            this.tpSubDivisions.SuspendLayout();
            this.gbSubDivisionNodeParameters.SuspendLayout();
            this.gbSubDivisionNIFParameters.SuspendLayout();
            this.tpWorkshops.SuspendLayout();
            this.gbWorkshopNodeParameters.SuspendLayout();
            this.gbWorkshopNIFParameters.SuspendLayout();
            this.gbBorderFunctions.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.gbTargetFolder);
            this.WindowPanel.Controls.Add(this.tcObjectSelect);
            this.WindowPanel.Controls.Add(this.gbBorderFunctions);
            this.WindowPanel.Size = new System.Drawing.Size(672, 420);
            // 
            // gbTargetFolder
            // 
            this.gbTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTargetFolder.Controls.Add(this.tbTargetFolder);
            this.gbTargetFolder.Location = new System.Drawing.Point(339, 6);
            this.gbTargetFolder.Name = "gbTargetFolder";
            this.gbTargetFolder.Size = new System.Drawing.Size(330, 44);
            this.gbTargetFolder.TabIndex = 5;
            this.gbTargetFolder.TabStop = false;
            this.gbTargetFolder.Tag = "BorderBatchWindow.TargetDirectory";
            this.gbTargetFolder.Text = "Target Folder";
            // 
            // tbTargetFolder
            // 
            this.tbTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTargetFolder.Location = new System.Drawing.Point(6, 19);
            this.tbTargetFolder.Name = "tbTargetFolder";
            this.tbTargetFolder.ReadOnly = true;
            this.tbTargetFolder.Size = new System.Drawing.Size(318, 20);
            this.tbTargetFolder.TabIndex = 0;
            // 
            // tcObjectSelect
            // 
            this.tcObjectSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcObjectSelect.Controls.Add(this.tpSubDivisions);
            this.tcObjectSelect.Controls.Add(this.tpWorkshops);
            this.tcObjectSelect.Location = new System.Drawing.Point(0, 56);
            this.tcObjectSelect.Name = "tcObjectSelect";
            this.tcObjectSelect.SelectedIndex = 0;
            this.tcObjectSelect.Size = new System.Drawing.Size(672, 364);
            this.tcObjectSelect.TabIndex = 0;
            // 
            // tpSubDivisions
            // 
            this.tpSubDivisions.Controls.Add(this.gbSubDivisionNodeParameters);
            this.tpSubDivisions.Controls.Add(this.gbSubDivisionNIFParameters);
            this.tpSubDivisions.Controls.Add(this.lvSubDivisions);
            this.tpSubDivisions.Controls.Add(this.cbSubDivisionPresets);
            this.tpSubDivisions.Controls.Add(this.lblSubDivisionPresets);
            this.tpSubDivisions.Location = new System.Drawing.Point(4, 22);
            this.tpSubDivisions.Name = "tpSubDivisions";
            this.tpSubDivisions.Padding = new System.Windows.Forms.Padding(3);
            this.tpSubDivisions.Size = new System.Drawing.Size(664, 461);
            this.tpSubDivisions.TabIndex = 1;
            this.tpSubDivisions.Tag = "BorderBatchWindow.Tab.SubDivisions";
            this.tpSubDivisions.Text = "Sub-Divisions";
            this.tpSubDivisions.UseVisualStyleBackColor = true;
            // 
            // gbSubDivisionNodeParameters
            // 
            this.gbSubDivisionNodeParameters.Controls.Add(this.tbSubDivisionNodeLength);
            this.gbSubDivisionNodeParameters.Controls.Add(this.tbSubDivisionNodeAngleAllowance);
            this.gbSubDivisionNodeParameters.Controls.Add(this.lbSubDivisionNodeAngleAllowance);
            this.gbSubDivisionNodeParameters.Controls.Add(this.tbSubDivisionNodeSlopeAllowance);
            this.gbSubDivisionNodeParameters.Controls.Add(this.lbSubDivisionNodeLength);
            this.gbSubDivisionNodeParameters.Controls.Add(this.lbSubDivisionNodeSlopeAllowance);
            this.gbSubDivisionNodeParameters.Location = new System.Drawing.Point(0, 29);
            this.gbSubDivisionNodeParameters.Name = "gbSubDivisionNodeParameters";
            this.gbSubDivisionNodeParameters.Size = new System.Drawing.Size(330, 62);
            this.gbSubDivisionNodeParameters.TabIndex = 12;
            this.gbSubDivisionNodeParameters.TabStop = false;
            this.gbSubDivisionNodeParameters.Tag = "BorderBatchWindow.Node.Parameters";
            this.gbSubDivisionNodeParameters.Text = "Nodes";
            // 
            // tbSubDivisionNodeLength
            // 
            this.tbSubDivisionNodeLength.Location = new System.Drawing.Point(103, 13);
            this.tbSubDivisionNodeLength.Name = "tbSubDivisionNodeLength";
            this.tbSubDivisionNodeLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbSubDivisionNodeLength.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNodeLength.TabIndex = 4;
            this.tbSubDivisionNodeLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSubDivisionNodeAngleAllowance
            // 
            this.tbSubDivisionNodeAngleAllowance.Location = new System.Drawing.Point(264, 13);
            this.tbSubDivisionNodeAngleAllowance.Name = "tbSubDivisionNodeAngleAllowance";
            this.tbSubDivisionNodeAngleAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbSubDivisionNodeAngleAllowance.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNodeAngleAllowance.TabIndex = 9;
            this.tbSubDivisionNodeAngleAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSubDivisionNodeAngleAllowance
            // 
            this.lbSubDivisionNodeAngleAllowance.Location = new System.Drawing.Point(167, 16);
            this.lbSubDivisionNodeAngleAllowance.Name = "lbSubDivisionNodeAngleAllowance";
            this.lbSubDivisionNodeAngleAllowance.Size = new System.Drawing.Size(101, 17);
            this.lbSubDivisionNodeAngleAllowance.TabIndex = 8;
            this.lbSubDivisionNodeAngleAllowance.Tag = "BorderBatchWindow.Node.AngleAllowance:";
            this.lbSubDivisionNodeAngleAllowance.Text = "Angle Allowance:";
            // 
            // tbSubDivisionNodeSlopeAllowance
            // 
            this.tbSubDivisionNodeSlopeAllowance.Location = new System.Drawing.Point(264, 36);
            this.tbSubDivisionNodeSlopeAllowance.Name = "tbSubDivisionNodeSlopeAllowance";
            this.tbSubDivisionNodeSlopeAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbSubDivisionNodeSlopeAllowance.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNodeSlopeAllowance.TabIndex = 7;
            this.tbSubDivisionNodeSlopeAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSubDivisionNodeLength
            // 
            this.lbSubDivisionNodeLength.Location = new System.Drawing.Point(6, 16);
            this.lbSubDivisionNodeLength.Name = "lbSubDivisionNodeLength";
            this.lbSubDivisionNodeLength.Size = new System.Drawing.Size(101, 17);
            this.lbSubDivisionNodeLength.TabIndex = 0;
            this.lbSubDivisionNodeLength.Tag = "BorderBatchWindow.Node.Length:";
            this.lbSubDivisionNodeLength.Text = "Node Length:";
            // 
            // lbSubDivisionNodeSlopeAllowance
            // 
            this.lbSubDivisionNodeSlopeAllowance.Location = new System.Drawing.Point(167, 39);
            this.lbSubDivisionNodeSlopeAllowance.Name = "lbSubDivisionNodeSlopeAllowance";
            this.lbSubDivisionNodeSlopeAllowance.Size = new System.Drawing.Size(101, 17);
            this.lbSubDivisionNodeSlopeAllowance.TabIndex = 6;
            this.lbSubDivisionNodeSlopeAllowance.Tag = "BorderBatchWindow.Node.SlopeAllowance:";
            this.lbSubDivisionNodeSlopeAllowance.Text = "Slope Allowance:";
            // 
            // gbSubDivisionNIFParameters
            // 
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbSubDivisionNIFGroundSink);
            this.gbSubDivisionNIFParameters.Controls.Add(this.cbSubDivisionNIFCreateImportData);
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbSubDivisionNIFTargetSubDirectory);
            this.gbSubDivisionNIFParameters.Controls.Add(this.lblSubDivisionNIFTargetSubDirectory);
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbSubDivisionNIFGroundOffset);
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbSubDivisionNIFMeshSubDirectory);
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbNIFBuilderSubDivisionNIFSampleFilePath);
            this.gbSubDivisionNIFParameters.Controls.Add(this.tbSubDivisionNIFGradientHeight);
            this.gbSubDivisionNIFParameters.Controls.Add(this.lblSubDivisionNIFGroundOffset);
            this.gbSubDivisionNIFParameters.Controls.Add(this.lblSubDivisionNIFMeshSubDirectory);
            this.gbSubDivisionNIFParameters.Controls.Add(this.lblSubDivisionNIFGradientHeight);
            this.gbSubDivisionNIFParameters.Controls.Add(this.lblSubDivisionNIFGroundSink);
            this.gbSubDivisionNIFParameters.Controls.Add(this.cbSubDivisionNIFHighPrecisionVertexes);
            this.gbSubDivisionNIFParameters.Location = new System.Drawing.Point(0, 91);
            this.gbSubDivisionNIFParameters.Name = "gbSubDivisionNIFParameters";
            this.gbSubDivisionNIFParameters.Size = new System.Drawing.Size(330, 205);
            this.gbSubDivisionNIFParameters.TabIndex = 11;
            this.gbSubDivisionNIFParameters.TabStop = false;
            this.gbSubDivisionNIFParameters.Tag = "BorderBatchWindow.NIF.Parameters";
            this.gbSubDivisionNIFParameters.Text = "NIFs";
            // 
            // tbSubDivisionNIFGroundSink
            // 
            this.tbSubDivisionNIFGroundSink.Location = new System.Drawing.Point(103, 110);
            this.tbSubDivisionNIFGroundSink.Name = "tbSubDivisionNIFGroundSink";
            this.tbSubDivisionNIFGroundSink.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNIFGroundSink.TabIndex = 8;
            this.tbSubDivisionNIFGroundSink.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbSubDivisionNIFCreateImportData
            // 
            this.cbSubDivisionNIFCreateImportData.AutoEllipsis = true;
            this.cbSubDivisionNIFCreateImportData.Location = new System.Drawing.Point(6, 16);
            this.cbSubDivisionNIFCreateImportData.Name = "cbSubDivisionNIFCreateImportData";
            this.cbSubDivisionNIFCreateImportData.Size = new System.Drawing.Size(317, 21);
            this.cbSubDivisionNIFCreateImportData.TabIndex = 5;
            this.cbSubDivisionNIFCreateImportData.Tag = "BorderBatchWindow.NIF.CreateImports";
            this.cbSubDivisionNIFCreateImportData.Text = "Create Import Data";
            this.cbSubDivisionNIFCreateImportData.UseVisualStyleBackColor = true;
            // 
            // tbSubDivisionNIFTargetSubDirectory
            // 
            this.tbSubDivisionNIFTargetSubDirectory.Location = new System.Drawing.Point(103, 133);
            this.tbSubDivisionNIFTargetSubDirectory.Name = "tbSubDivisionNIFTargetSubDirectory";
            this.tbSubDivisionNIFTargetSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionNIFTargetSubDirectory.TabIndex = 22;
            // 
            // lblSubDivisionNIFTargetSubDirectory
            // 
            this.lblSubDivisionNIFTargetSubDirectory.Location = new System.Drawing.Point(6, 136);
            this.lblSubDivisionNIFTargetSubDirectory.Name = "lblSubDivisionNIFTargetSubDirectory";
            this.lblSubDivisionNIFTargetSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionNIFTargetSubDirectory.TabIndex = 23;
            this.lblSubDivisionNIFTargetSubDirectory.Tag = "BorderBatchWindow.NIF.TargetSubDirectory:";
            this.lblSubDivisionNIFTargetSubDirectory.Text = "Target Sub-Folder:";
            // 
            // tbSubDivisionNIFGroundOffset
            // 
            this.tbSubDivisionNIFGroundOffset.Location = new System.Drawing.Point(103, 87);
            this.tbSubDivisionNIFGroundOffset.Name = "tbSubDivisionNIFGroundOffset";
            this.tbSubDivisionNIFGroundOffset.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNIFGroundOffset.TabIndex = 7;
            this.tbSubDivisionNIFGroundOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSubDivisionNIFMeshSubDirectory
            // 
            this.tbSubDivisionNIFMeshSubDirectory.Location = new System.Drawing.Point(103, 156);
            this.tbSubDivisionNIFMeshSubDirectory.Name = "tbSubDivisionNIFMeshSubDirectory";
            this.tbSubDivisionNIFMeshSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbSubDivisionNIFMeshSubDirectory.TabIndex = 5;
            // 
            // tbNIFBuilderSubDivisionNIFSampleFilePath
            // 
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.BackColor = System.Drawing.SystemColors.Control;
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.Location = new System.Drawing.Point(6, 179);
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.Name = "tbNIFBuilderSubDivisionNIFSampleFilePath";
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.Size = new System.Drawing.Size(318, 20);
            this.tbNIFBuilderSubDivisionNIFSampleFilePath.TabIndex = 7;
            // 
            // tbSubDivisionNIFGradientHeight
            // 
            this.tbSubDivisionNIFGradientHeight.Location = new System.Drawing.Point(103, 64);
            this.tbSubDivisionNIFGradientHeight.Name = "tbSubDivisionNIFGradientHeight";
            this.tbSubDivisionNIFGradientHeight.Size = new System.Drawing.Size(60, 20);
            this.tbSubDivisionNIFGradientHeight.TabIndex = 6;
            this.tbSubDivisionNIFGradientHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSubDivisionNIFGroundOffset
            // 
            this.lblSubDivisionNIFGroundOffset.Location = new System.Drawing.Point(6, 90);
            this.lblSubDivisionNIFGroundOffset.Name = "lblSubDivisionNIFGroundOffset";
            this.lblSubDivisionNIFGroundOffset.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionNIFGroundOffset.TabIndex = 3;
            this.lblSubDivisionNIFGroundOffset.Tag = "BorderBatchWindow.NIF.GroundOffset:";
            this.lblSubDivisionNIFGroundOffset.Text = "Ground Offset:";
            // 
            // lblSubDivisionNIFMeshSubDirectory
            // 
            this.lblSubDivisionNIFMeshSubDirectory.Location = new System.Drawing.Point(6, 159);
            this.lblSubDivisionNIFMeshSubDirectory.Name = "lblSubDivisionNIFMeshSubDirectory";
            this.lblSubDivisionNIFMeshSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionNIFMeshSubDirectory.TabIndex = 10;
            this.lblSubDivisionNIFMeshSubDirectory.Tag = "BorderBatchWindow.NIF.MeshSubDirectory:";
            this.lblSubDivisionNIFMeshSubDirectory.Text = "Mesh Sub-Folder:";
            // 
            // lblSubDivisionNIFGradientHeight
            // 
            this.lblSubDivisionNIFGradientHeight.Location = new System.Drawing.Point(6, 67);
            this.lblSubDivisionNIFGradientHeight.Name = "lblSubDivisionNIFGradientHeight";
            this.lblSubDivisionNIFGradientHeight.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionNIFGradientHeight.TabIndex = 2;
            this.lblSubDivisionNIFGradientHeight.Tag = "BorderBatchWindow.NIF.GradientHeight:";
            this.lblSubDivisionNIFGradientHeight.Text = "Gradient Height:";
            // 
            // lblSubDivisionNIFGroundSink
            // 
            this.lblSubDivisionNIFGroundSink.Location = new System.Drawing.Point(6, 113);
            this.lblSubDivisionNIFGroundSink.Name = "lblSubDivisionNIFGroundSink";
            this.lblSubDivisionNIFGroundSink.Size = new System.Drawing.Size(101, 17);
            this.lblSubDivisionNIFGroundSink.TabIndex = 1;
            this.lblSubDivisionNIFGroundSink.Tag = "BorderBatchWindow.NIF.GroundSink:";
            this.lblSubDivisionNIFGroundSink.Text = "Ground Sink:";
            // 
            // cbSubDivisionNIFHighPrecisionVertexes
            // 
            this.cbSubDivisionNIFHighPrecisionVertexes.AutoEllipsis = true;
            this.cbSubDivisionNIFHighPrecisionVertexes.Location = new System.Drawing.Point(6, 37);
            this.cbSubDivisionNIFHighPrecisionVertexes.Name = "cbSubDivisionNIFHighPrecisionVertexes";
            this.cbSubDivisionNIFHighPrecisionVertexes.Size = new System.Drawing.Size(317, 21);
            this.cbSubDivisionNIFHighPrecisionVertexes.TabIndex = 29;
            this.cbSubDivisionNIFHighPrecisionVertexes.Tag = "BorderBatchWindow.NIF.UseHighPrecisionFloats";
            this.cbSubDivisionNIFHighPrecisionVertexes.Text = "High Precision Vertexes";
            this.cbSubDivisionNIFHighPrecisionVertexes.UseVisualStyleBackColor = true;
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
            this.lvSubDivisions.Size = new System.Drawing.Size(328, 461);
            this.lvSubDivisions.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvSubDivisions.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvSubDivisions.SyncedEditorFormType = null;
            this.lvSubDivisions.SyncObjects = null;
            this.lvSubDivisions.TabIndex = 10;
            this.lvSubDivisions.TypeColumn = false;
            // 
            // cbSubDivisionPresets
            // 
            this.cbSubDivisionPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubDivisionPresets.Location = new System.Drawing.Point(81, 6);
            this.cbSubDivisionPresets.Name = "cbSubDivisionPresets";
            this.cbSubDivisionPresets.Size = new System.Drawing.Size(249, 21);
            this.cbSubDivisionPresets.TabIndex = 16;
            // 
            // lblSubDivisionPresets
            // 
            this.lblSubDivisionPresets.Location = new System.Drawing.Point(6, 9);
            this.lblSubDivisionPresets.Name = "lblSubDivisionPresets";
            this.lblSubDivisionPresets.Size = new System.Drawing.Size(82, 17);
            this.lblSubDivisionPresets.TabIndex = 15;
            this.lblSubDivisionPresets.Tag = "BorderBatchWindow.Preset:";
            this.lblSubDivisionPresets.Text = "Preset:";
            // 
            // tpWorkshops
            // 
            this.tpWorkshops.Controls.Add(this.gbWorkshopNodeParameters);
            this.tpWorkshops.Controls.Add(this.cbWorkshopPresets);
            this.tpWorkshops.Controls.Add(this.gbWorkshopNIFParameters);
            this.tpWorkshops.Controls.Add(this.lvWorkshops);
            this.tpWorkshops.Controls.Add(this.lblWorkshopPresets);
            this.tpWorkshops.Location = new System.Drawing.Point(4, 22);
            this.tpWorkshops.Name = "tpWorkshops";
            this.tpWorkshops.Padding = new System.Windows.Forms.Padding(3);
            this.tpWorkshops.Size = new System.Drawing.Size(664, 338);
            this.tpWorkshops.TabIndex = 0;
            this.tpWorkshops.Tag = "BorderBatchWindow.Tab.Workshops";
            this.tpWorkshops.Text = "Workshops";
            this.tpWorkshops.UseVisualStyleBackColor = true;
            // 
            // gbWorkshopNodeParameters
            // 
            this.gbWorkshopNodeParameters.Controls.Add(this.tbWorkshopNodeLength);
            this.gbWorkshopNodeParameters.Controls.Add(this.tbWorkshopNodeAngleAllowance);
            this.gbWorkshopNodeParameters.Controls.Add(this.lblWorkshopNodeAngleAllowance);
            this.gbWorkshopNodeParameters.Controls.Add(this.lblWorkshopNodeLength);
            this.gbWorkshopNodeParameters.Controls.Add(this.tbWorkshopNodeSlopeAllowance);
            this.gbWorkshopNodeParameters.Controls.Add(this.lblWorkshopNodeSlopeAllowance);
            this.gbWorkshopNodeParameters.Location = new System.Drawing.Point(0, 29);
            this.gbWorkshopNodeParameters.Name = "gbWorkshopNodeParameters";
            this.gbWorkshopNodeParameters.Size = new System.Drawing.Size(330, 62);
            this.gbWorkshopNodeParameters.TabIndex = 18;
            this.gbWorkshopNodeParameters.TabStop = false;
            this.gbWorkshopNodeParameters.Tag = "BorderBatchWindow.Node.Parameters";
            this.gbWorkshopNodeParameters.Text = "Nodes";
            // 
            // tbWorkshopNodeLength
            // 
            this.tbWorkshopNodeLength.Location = new System.Drawing.Point(103, 13);
            this.tbWorkshopNodeLength.Name = "tbWorkshopNodeLength";
            this.tbWorkshopNodeLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbWorkshopNodeLength.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNodeLength.TabIndex = 16;
            this.tbWorkshopNodeLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorkshopNodeAngleAllowance
            // 
            this.tbWorkshopNodeAngleAllowance.Location = new System.Drawing.Point(264, 13);
            this.tbWorkshopNodeAngleAllowance.Name = "tbWorkshopNodeAngleAllowance";
            this.tbWorkshopNodeAngleAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbWorkshopNodeAngleAllowance.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNodeAngleAllowance.TabIndex = 20;
            this.tbWorkshopNodeAngleAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblWorkshopNodeAngleAllowance
            // 
            this.lblWorkshopNodeAngleAllowance.Location = new System.Drawing.Point(167, 16);
            this.lblWorkshopNodeAngleAllowance.Name = "lblWorkshopNodeAngleAllowance";
            this.lblWorkshopNodeAngleAllowance.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopNodeAngleAllowance.TabIndex = 19;
            this.lblWorkshopNodeAngleAllowance.Tag = "BorderBatchWindow.Node.AngleAllowance:";
            this.lblWorkshopNodeAngleAllowance.Text = "Angle Allowance:";
            // 
            // lblWorkshopNodeLength
            // 
            this.lblWorkshopNodeLength.Location = new System.Drawing.Point(6, 16);
            this.lblWorkshopNodeLength.Name = "lblWorkshopNodeLength";
            this.lblWorkshopNodeLength.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopNodeLength.TabIndex = 15;
            this.lblWorkshopNodeLength.Tag = "BorderBatchWindow.Node.Length:";
            this.lblWorkshopNodeLength.Text = "Node Length:";
            // 
            // tbWorkshopNodeSlopeAllowance
            // 
            this.tbWorkshopNodeSlopeAllowance.Location = new System.Drawing.Point(264, 36);
            this.tbWorkshopNodeSlopeAllowance.Name = "tbWorkshopNodeSlopeAllowance";
            this.tbWorkshopNodeSlopeAllowance.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbWorkshopNodeSlopeAllowance.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNodeSlopeAllowance.TabIndex = 18;
            this.tbWorkshopNodeSlopeAllowance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblWorkshopNodeSlopeAllowance
            // 
            this.lblWorkshopNodeSlopeAllowance.Location = new System.Drawing.Point(167, 39);
            this.lblWorkshopNodeSlopeAllowance.Name = "lblWorkshopNodeSlopeAllowance";
            this.lblWorkshopNodeSlopeAllowance.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopNodeSlopeAllowance.TabIndex = 17;
            this.lblWorkshopNodeSlopeAllowance.Tag = "BorderBatchWindow.Node.SlopeAllowance:";
            this.lblWorkshopNodeSlopeAllowance.Text = "Slope Allowance:";
            // 
            // cbWorkshopPresets
            // 
            this.cbWorkshopPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopPresets.Location = new System.Drawing.Point(81, 6);
            this.cbWorkshopPresets.Name = "cbWorkshopPresets";
            this.cbWorkshopPresets.Size = new System.Drawing.Size(249, 21);
            this.cbWorkshopPresets.TabIndex = 14;
            // 
            // gbWorkshopNIFParameters
            // 
            this.gbWorkshopNIFParameters.Controls.Add(this.cbWorkshopNIFUseExistingFilePaths);
            this.gbWorkshopNIFParameters.Controls.Add(this.cbWorkshopNIFUseExistingSTATEditorIDs);
            this.gbWorkshopNIFParameters.Controls.Add(this.cbWorkshopNIFHighPrecisionVertexes);
            this.gbWorkshopNIFParameters.Controls.Add(this.cbWorkshopNIFCreateImportData);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFGroundSink);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFGroundOffset);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFMeshSubDirectory);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFTargetSubDirectory);
            this.gbWorkshopNIFParameters.Controls.Add(this.lblWorkshopNIFTargetSubDirectory);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFGradientHeight);
            this.gbWorkshopNIFParameters.Controls.Add(this.tbWorkshopNIFSampleFilePath);
            this.gbWorkshopNIFParameters.Controls.Add(this.lblWorkshopNIFGradientHeight);
            this.gbWorkshopNIFParameters.Controls.Add(this.lblWorkshopNIFMeshSubDirectory);
            this.gbWorkshopNIFParameters.Controls.Add(this.lblWorkshopNIFGroundOffset);
            this.gbWorkshopNIFParameters.Controls.Add(this.lblWorkshopNIFGroundSink);
            this.gbWorkshopNIFParameters.Location = new System.Drawing.Point(0, 91);
            this.gbWorkshopNIFParameters.Name = "gbWorkshopNIFParameters";
            this.gbWorkshopNIFParameters.Size = new System.Drawing.Size(330, 247);
            this.gbWorkshopNIFParameters.TabIndex = 10;
            this.gbWorkshopNIFParameters.TabStop = false;
            this.gbWorkshopNIFParameters.Tag = "BorderBatchWindow.NIF.Parameters";
            this.gbWorkshopNIFParameters.Text = "NIFs";
            // 
            // cbWorkshopNIFHighPrecisionVertexes
            // 
            this.cbWorkshopNIFHighPrecisionVertexes.AutoEllipsis = true;
            this.cbWorkshopNIFHighPrecisionVertexes.Location = new System.Drawing.Point(6, 37);
            this.cbWorkshopNIFHighPrecisionVertexes.Name = "cbWorkshopNIFHighPrecisionVertexes";
            this.cbWorkshopNIFHighPrecisionVertexes.Size = new System.Drawing.Size(317, 21);
            this.cbWorkshopNIFHighPrecisionVertexes.TabIndex = 30;
            this.cbWorkshopNIFHighPrecisionVertexes.Tag = "BorderBatchWindow.NIF.UseHighPrecisionFloats";
            this.cbWorkshopNIFHighPrecisionVertexes.Text = "High Precision Vertexes";
            this.cbWorkshopNIFHighPrecisionVertexes.UseVisualStyleBackColor = true;
            // 
            // cbWorkshopNIFCreateImportData
            // 
            this.cbWorkshopNIFCreateImportData.AutoEllipsis = true;
            this.cbWorkshopNIFCreateImportData.Location = new System.Drawing.Point(6, 16);
            this.cbWorkshopNIFCreateImportData.Name = "cbWorkshopNIFCreateImportData";
            this.cbWorkshopNIFCreateImportData.Size = new System.Drawing.Size(317, 21);
            this.cbWorkshopNIFCreateImportData.TabIndex = 18;
            this.cbWorkshopNIFCreateImportData.Tag = "BorderBatchWindow.NIF.CreateImports";
            this.cbWorkshopNIFCreateImportData.Text = "Create Import Data";
            this.cbWorkshopNIFCreateImportData.UseVisualStyleBackColor = true;
            // 
            // tbWorkshopNIFGroundSink
            // 
            this.tbWorkshopNIFGroundSink.Location = new System.Drawing.Point(103, 152);
            this.tbWorkshopNIFGroundSink.Name = "tbWorkshopNIFGroundSink";
            this.tbWorkshopNIFGroundSink.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNIFGroundSink.TabIndex = 24;
            this.tbWorkshopNIFGroundSink.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorkshopNIFGroundOffset
            // 
            this.tbWorkshopNIFGroundOffset.Location = new System.Drawing.Point(103, 129);
            this.tbWorkshopNIFGroundOffset.Name = "tbWorkshopNIFGroundOffset";
            this.tbWorkshopNIFGroundOffset.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNIFGroundOffset.TabIndex = 23;
            this.tbWorkshopNIFGroundOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorkshopNIFMeshSubDirectory
            // 
            this.tbWorkshopNIFMeshSubDirectory.Location = new System.Drawing.Point(103, 198);
            this.tbWorkshopNIFMeshSubDirectory.Name = "tbWorkshopNIFMeshSubDirectory";
            this.tbWorkshopNIFMeshSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopNIFMeshSubDirectory.TabIndex = 2;
            // 
            // tbWorkshopNIFTargetSubDirectory
            // 
            this.tbWorkshopNIFTargetSubDirectory.Location = new System.Drawing.Point(103, 175);
            this.tbWorkshopNIFTargetSubDirectory.Name = "tbWorkshopNIFTargetSubDirectory";
            this.tbWorkshopNIFTargetSubDirectory.Size = new System.Drawing.Size(221, 20);
            this.tbWorkshopNIFTargetSubDirectory.TabIndex = 20;
            // 
            // lblWorkshopNIFTargetSubDirectory
            // 
            this.lblWorkshopNIFTargetSubDirectory.Location = new System.Drawing.Point(6, 178);
            this.lblWorkshopNIFTargetSubDirectory.Name = "lblWorkshopNIFTargetSubDirectory";
            this.lblWorkshopNIFTargetSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopNIFTargetSubDirectory.TabIndex = 21;
            this.lblWorkshopNIFTargetSubDirectory.Tag = "BorderBatchWindow.NIF.TargetSubDirectory:";
            this.lblWorkshopNIFTargetSubDirectory.Text = "Target Sub-Folder:";
            // 
            // tbWorkshopNIFGradientHeight
            // 
            this.tbWorkshopNIFGradientHeight.Location = new System.Drawing.Point(103, 106);
            this.tbWorkshopNIFGradientHeight.Name = "tbWorkshopNIFGradientHeight";
            this.tbWorkshopNIFGradientHeight.Size = new System.Drawing.Size(60, 20);
            this.tbWorkshopNIFGradientHeight.TabIndex = 22;
            this.tbWorkshopNIFGradientHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorkshopNIFSampleFilePath
            // 
            this.tbWorkshopNIFSampleFilePath.BackColor = System.Drawing.SystemColors.Control;
            this.tbWorkshopNIFSampleFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbWorkshopNIFSampleFilePath.Location = new System.Drawing.Point(6, 221);
            this.tbWorkshopNIFSampleFilePath.Name = "tbWorkshopNIFSampleFilePath";
            this.tbWorkshopNIFSampleFilePath.Size = new System.Drawing.Size(318, 20);
            this.tbWorkshopNIFSampleFilePath.TabIndex = 4;
            // 
            // lblWorkshopNIFGradientHeight
            // 
            this.lblWorkshopNIFGradientHeight.Location = new System.Drawing.Point(6, 109);
            this.lblWorkshopNIFGradientHeight.Name = "lblWorkshopNIFGradientHeight";
            this.lblWorkshopNIFGradientHeight.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopNIFGradientHeight.TabIndex = 20;
            this.lblWorkshopNIFGradientHeight.Tag = "BorderBatchWindow.NIF.GradientHeight:";
            this.lblWorkshopNIFGradientHeight.Text = "Gradient Height:";
            // 
            // lblWorkshopNIFMeshSubDirectory
            // 
            this.lblWorkshopNIFMeshSubDirectory.Location = new System.Drawing.Point(6, 201);
            this.lblWorkshopNIFMeshSubDirectory.Name = "lblWorkshopNIFMeshSubDirectory";
            this.lblWorkshopNIFMeshSubDirectory.Size = new System.Drawing.Size(101, 17);
            this.lblWorkshopNIFMeshSubDirectory.TabIndex = 5;
            this.lblWorkshopNIFMeshSubDirectory.Tag = "BorderBatchWindow.NIF.MeshSubDirectory:";
            this.lblWorkshopNIFMeshSubDirectory.Text = "Mesh Sub-Folder:";
            // 
            // lblWorkshopNIFGroundOffset
            // 
            this.lblWorkshopNIFGroundOffset.Location = new System.Drawing.Point(6, 132);
            this.lblWorkshopNIFGroundOffset.Name = "lblWorkshopNIFGroundOffset";
            this.lblWorkshopNIFGroundOffset.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopNIFGroundOffset.TabIndex = 21;
            this.lblWorkshopNIFGroundOffset.Tag = "BorderBatchWindow.NIF.GroundOffset:";
            this.lblWorkshopNIFGroundOffset.Text = "Ground Offset:";
            // 
            // lblWorkshopNIFGroundSink
            // 
            this.lblWorkshopNIFGroundSink.Location = new System.Drawing.Point(6, 155);
            this.lblWorkshopNIFGroundSink.Name = "lblWorkshopNIFGroundSink";
            this.lblWorkshopNIFGroundSink.Size = new System.Drawing.Size(90, 16);
            this.lblWorkshopNIFGroundSink.TabIndex = 19;
            this.lblWorkshopNIFGroundSink.Tag = "BorderBatchWindow.NIF.GroundSink:";
            this.lblWorkshopNIFGroundSink.Text = "Ground Sink:";
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
            this.lvWorkshops.Size = new System.Drawing.Size(328, 338);
            this.lvWorkshops.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvWorkshops.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvWorkshops.SyncedEditorFormType = null;
            this.lvWorkshops.SyncObjects = null;
            this.lvWorkshops.TabIndex = 11;
            this.lvWorkshops.TypeColumn = false;
            // 
            // lblWorkshopPresets
            // 
            this.lblWorkshopPresets.Location = new System.Drawing.Point(6, 9);
            this.lblWorkshopPresets.Name = "lblWorkshopPresets";
            this.lblWorkshopPresets.Size = new System.Drawing.Size(82, 17);
            this.lblWorkshopPresets.TabIndex = 10;
            this.lblWorkshopPresets.Tag = "BorderBatchWindow.Preset:";
            this.lblWorkshopPresets.Text = "Preset:";
            // 
            // gbBorderFunctions
            // 
            this.gbBorderFunctions.Controls.Add(this.btnImportNIFs);
            this.gbBorderFunctions.Controls.Add(this.btnClearNodes);
            this.gbBorderFunctions.Controls.Add(this.btnGenerateNodes);
            this.gbBorderFunctions.Controls.Add(this.btnBuildNIFs);
            this.gbBorderFunctions.Location = new System.Drawing.Point(3, 6);
            this.gbBorderFunctions.Name = "gbBorderFunctions";
            this.gbBorderFunctions.Size = new System.Drawing.Size(330, 44);
            this.gbBorderFunctions.TabIndex = 9;
            this.gbBorderFunctions.TabStop = false;
            this.gbBorderFunctions.Tag = "BatchWindow.Functions";
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
            // 
            // btnClearNodes
            // 
            this.btnClearNodes.Location = new System.Drawing.Point(6, 15);
            this.btnClearNodes.Name = "btnClearNodes";
            this.btnClearNodes.Size = new System.Drawing.Size(75, 23);
            this.btnClearNodes.TabIndex = 0;
            this.btnClearNodes.Tag = "BorderBatchWindow.Function.ClearNodes";
            this.btnClearNodes.Text = "Clear Nodes";
            this.btnClearNodes.UseVisualStyleBackColor = true;
            // 
            // btnGenerateNodes
            // 
            this.btnGenerateNodes.Location = new System.Drawing.Point(87, 15);
            this.btnGenerateNodes.Name = "btnGenerateNodes";
            this.btnGenerateNodes.Size = new System.Drawing.Size(75, 23);
            this.btnGenerateNodes.TabIndex = 1;
            this.btnGenerateNodes.Tag = "BorderBatchWindow.Function.GenerateNodes";
            this.btnGenerateNodes.Text = "Gen. Nodes";
            this.btnGenerateNodes.UseVisualStyleBackColor = true;
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
            // 
            // cbWorkshopNIFUseExistingSTATEditorIDs
            // 
            this.cbWorkshopNIFUseExistingSTATEditorIDs.AutoEllipsis = true;
            this.cbWorkshopNIFUseExistingSTATEditorIDs.Location = new System.Drawing.Point(6, 58);
            this.cbWorkshopNIFUseExistingSTATEditorIDs.Name = "cbWorkshopNIFUseExistingSTATEditorIDs";
            this.cbWorkshopNIFUseExistingSTATEditorIDs.Size = new System.Drawing.Size(317, 21);
            this.cbWorkshopNIFUseExistingSTATEditorIDs.TabIndex = 31;
            this.cbWorkshopNIFUseExistingSTATEditorIDs.Tag = "BorderBatchWindow.NIF.UseExistingSTATEditorIDs";
            this.cbWorkshopNIFUseExistingSTATEditorIDs.Text = "Use Existing Static EditorIDs";
            this.cbWorkshopNIFUseExistingSTATEditorIDs.UseVisualStyleBackColor = true;
            // 
            // cbWorkshopNIFUseExistingFilePaths
            // 
            this.cbWorkshopNIFUseExistingFilePaths.AutoEllipsis = true;
            this.cbWorkshopNIFUseExistingFilePaths.Location = new System.Drawing.Point(6, 79);
            this.cbWorkshopNIFUseExistingFilePaths.Name = "cbWorkshopNIFUseExistingFilePaths";
            this.cbWorkshopNIFUseExistingFilePaths.Size = new System.Drawing.Size(317, 21);
            this.cbWorkshopNIFUseExistingFilePaths.TabIndex = 32;
            this.cbWorkshopNIFUseExistingFilePaths.Tag = "BorderBatchWindow.NIF.UseExistingFilePaths";
            this.cbWorkshopNIFUseExistingFilePaths.Text = "Use Existing Static NIF Filepaths";
            this.cbWorkshopNIFUseExistingFilePaths.UseVisualStyleBackColor = true;
            // 
            // BorderBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 420);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(680, 444);
            this.Name = "BorderBatch";
            this.ShowInTaskbar = false;
            this.Tag = "BorderBatchWindow.Title";
            this.WindowPanel.ResumeLayout(false);
            this.gbTargetFolder.ResumeLayout(false);
            this.gbTargetFolder.PerformLayout();
            this.tcObjectSelect.ResumeLayout(false);
            this.tpSubDivisions.ResumeLayout(false);
            this.gbSubDivisionNodeParameters.ResumeLayout(false);
            this.gbSubDivisionNodeParameters.PerformLayout();
            this.gbSubDivisionNIFParameters.ResumeLayout(false);
            this.gbSubDivisionNIFParameters.PerformLayout();
            this.tpWorkshops.ResumeLayout(false);
            this.gbWorkshopNodeParameters.ResumeLayout(false);
            this.gbWorkshopNodeParameters.PerformLayout();
            this.gbWorkshopNIFParameters.ResumeLayout(false);
            this.gbWorkshopNIFParameters.PerformLayout();
            this.gbBorderFunctions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
