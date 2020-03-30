/*
 * Created by SharpDevelop.
 * Date: 03/01/2020
 * Time: 8:45 PM
 */
namespace GUIBuilder.Windows
{
    partial class CustomForms
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tcCustomForms;
        private System.Windows.Forms.TabPage tpWorkshop;
        private System.Windows.Forms.GroupBox gbWorkshopContainers;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetection;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionStaticMarkers;
        private System.Windows.Forms.Label lblWorkshopBorderMarkerTerrainFollowing;
        private System.Windows.Forms.Label lblWorkshopForcedZStatic;
        private System.Windows.Forms.ComboBox cbWorkshopBorderMarkerTerrainFollowing;
        private System.Windows.Forms.ComboBox cbWorkshopBorderMarkerForcedZ;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionKeywords;
        private System.Windows.Forms.Label lbWorkshopBorderGenerator;
        private System.Windows.Forms.ComboBox cbWorkshopKeywordBorderGenerator;
        private System.Windows.Forms.Label lblWorkshopMarkerLink;
        private System.Windows.Forms.ComboBox cbWorkshopKeywordBorderLink;
        private System.Windows.Forms.CheckBox cbRestrictWorkshopBorderKeywords;
        private System.Windows.Forms.GroupBox gbWorkshopContainerFilter;
        private GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Container> lvWorkshopContainers;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionLocationRefs;
        private System.Windows.Forms.Label lblWorkshopBordeRefBorderWithBottom;
        private System.Windows.Forms.ComboBox cbWorkshopBordeRefBorderWithBottom;
        private System.Windows.Forms.Button btnApplyWorkshopContainerFilter;
        private System.Windows.Forms.Button btnApplyWorkshopContainerSelection;
        private System.Windows.Forms.ComboBox cbWorkshopContainerFilter;
        
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
            this.tcCustomForms = new System.Windows.Forms.TabControl();
            this.tpWorkshop = new System.Windows.Forms.TabPage();
            this.gbWorkshopContainers = new System.Windows.Forms.GroupBox();
            this.btnApplyWorkshopContainerSelection = new System.Windows.Forms.Button();
            this.lvWorkshopContainers = new GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Container>();
            this.gbWorkshopContainerFilter = new System.Windows.Forms.GroupBox();
            this.btnApplyWorkshopContainerFilter = new System.Windows.Forms.Button();
            this.gbWorkshopNodeDetection = new System.Windows.Forms.GroupBox();
            this.gbWorkshopNodeDetectionLocationRefs = new System.Windows.Forms.GroupBox();
            this.lblWorkshopBordeRefBorderWithBottom = new System.Windows.Forms.Label();
            this.cbWorkshopBordeRefBorderWithBottom = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNodeDetectionStaticMarkers = new System.Windows.Forms.GroupBox();
            this.lblWorkshopBorderMarkerTerrainFollowing = new System.Windows.Forms.Label();
            this.lblWorkshopForcedZStatic = new System.Windows.Forms.Label();
            this.cbWorkshopBorderMarkerTerrainFollowing = new System.Windows.Forms.ComboBox();
            this.cbWorkshopBorderMarkerForcedZ = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNodeDetectionKeywords = new System.Windows.Forms.GroupBox();
            this.lbWorkshopBorderGenerator = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderGenerator = new System.Windows.Forms.ComboBox();
            this.lblWorkshopMarkerLink = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderLink = new System.Windows.Forms.ComboBox();
            this.cbRestrictWorkshopBorderKeywords = new System.Windows.Forms.CheckBox();
            this.cbWorkshopContainerFilter = new System.Windows.Forms.ComboBox();
            this.WindowPanel.SuspendLayout();
            this.tcCustomForms.SuspendLayout();
            this.tpWorkshop.SuspendLayout();
            this.gbWorkshopContainers.SuspendLayout();
            this.gbWorkshopContainerFilter.SuspendLayout();
            this.gbWorkshopNodeDetection.SuspendLayout();
            this.gbWorkshopNodeDetectionLocationRefs.SuspendLayout();
            this.gbWorkshopNodeDetectionStaticMarkers.SuspendLayout();
            this.gbWorkshopNodeDetectionKeywords.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.tcCustomForms);
            this.WindowPanel.Controls.Add(this.cbRestrictWorkshopBorderKeywords);
            this.WindowPanel.Size = new System.Drawing.Size(740, 375);
            // 
            // tcCustomForms
            // 
            this.tcCustomForms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCustomForms.Controls.Add(this.tpWorkshop);
            this.tcCustomForms.Location = new System.Drawing.Point(0, 36);
            this.tcCustomForms.Name = "tcCustomForms";
            this.tcCustomForms.SelectedIndex = 0;
            this.tcCustomForms.Size = new System.Drawing.Size(740, 339);
            this.tcCustomForms.TabIndex = 0;
            // 
            // tpWorkshop
            // 
            this.tpWorkshop.Controls.Add(this.gbWorkshopContainers);
            this.tpWorkshop.Controls.Add(this.gbWorkshopNodeDetection);
            this.tpWorkshop.Location = new System.Drawing.Point(4, 22);
            this.tpWorkshop.Name = "tpWorkshop";
            this.tpWorkshop.Padding = new System.Windows.Forms.Padding(3);
            this.tpWorkshop.Size = new System.Drawing.Size(732, 313);
            this.tpWorkshop.TabIndex = 0;
            this.tpWorkshop.Tag = "CustomFormsWindow.Tab.Workshop";
            this.tpWorkshop.Text = "Workshop";
            this.tpWorkshop.UseVisualStyleBackColor = true;
            // 
            // gbWorkshopContainers
            // 
            this.gbWorkshopContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkshopContainers.Controls.Add(this.btnApplyWorkshopContainerSelection);
            this.gbWorkshopContainers.Controls.Add(this.lvWorkshopContainers);
            this.gbWorkshopContainers.Controls.Add(this.gbWorkshopContainerFilter);
            this.gbWorkshopContainers.Location = new System.Drawing.Point(336, 3);
            this.gbWorkshopContainers.Name = "gbWorkshopContainers";
            this.gbWorkshopContainers.Size = new System.Drawing.Size(396, 310);
            this.gbWorkshopContainers.TabIndex = 19;
            this.gbWorkshopContainers.TabStop = false;
            this.gbWorkshopContainers.Tag = "CustomFormsWindow.Tab.Containers";
            this.gbWorkshopContainers.Text = "Containers";
            // 
            // btnApplyWorkshopContainerSelection
            // 
            this.btnApplyWorkshopContainerSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyWorkshopContainerSelection.Location = new System.Drawing.Point(254, 281);
            this.btnApplyWorkshopContainerSelection.Name = "btnApplyWorkshopContainerSelection";
            this.btnApplyWorkshopContainerSelection.Size = new System.Drawing.Size(136, 23);
            this.btnApplyWorkshopContainerSelection.TabIndex = 13;
            this.btnApplyWorkshopContainerSelection.Tag = "CustomFormsWindow.Tab.Container.Filter.Apply";
            this.btnApplyWorkshopContainerSelection.Text = "Update With Selection";
            this.btnApplyWorkshopContainerSelection.UseVisualStyleBackColor = true;
            this.btnApplyWorkshopContainerSelection.Click += new System.EventHandler(this.btnApplyWorkshopContainerSelectionClick);
            // 
            // lvWorkshops
            // 
            this.lvWorkshopContainers.AllowHidingItems = false;
            this.lvWorkshopContainers.AllowOverrideColumnSorting = true;
            this.lvWorkshopContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWorkshopContainers.CheckBoxes = true;
            this.lvWorkshopContainers.EditorIDColumn = true;
            this.lvWorkshopContainers.ExtraInfoColumn = false;
            this.lvWorkshopContainers.FilenameColumn = true;
            this.lvWorkshopContainers.FormIDColumn = true;
            this.lvWorkshopContainers.LoadOrderColumn = false;
            this.lvWorkshopContainers.Location = new System.Drawing.Point(6, 61);
            this.lvWorkshopContainers.MultiSelect = true;
            this.lvWorkshopContainers.Name = "lvWorkshops";
            this.lvWorkshopContainers.Size = new System.Drawing.Size(384, 214);
            this.lvWorkshopContainers.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.FormID;
            this.lvWorkshopContainers.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvWorkshopContainers.SyncedEditorFormType = null;
            this.lvWorkshopContainers.SyncObjects = null;
            this.lvWorkshopContainers.TabIndex = 12;
            this.lvWorkshopContainers.TypeColumn = false;
            // 
            // gbWorkshopContainerFilter
            // 
            this.gbWorkshopContainerFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkshopContainerFilter.Controls.Add(this.cbWorkshopContainerFilter);
            this.gbWorkshopContainerFilter.Controls.Add(this.btnApplyWorkshopContainerFilter);
            this.gbWorkshopContainerFilter.Location = new System.Drawing.Point(6, 19);
            this.gbWorkshopContainerFilter.Name = "gbWorkshopContainerFilter";
            this.gbWorkshopContainerFilter.Size = new System.Drawing.Size(384, 41);
            this.gbWorkshopContainerFilter.TabIndex = 0;
            this.gbWorkshopContainerFilter.TabStop = false;
            this.gbWorkshopContainerFilter.Tag = "CustomFormsWindow.Tab.Container.Filter";
            this.gbWorkshopContainerFilter.Text = "Filter";
            // 
            // btnApplyWorkshopContainerFilter
            // 
            this.btnApplyWorkshopContainerFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyWorkshopContainerFilter.Location = new System.Drawing.Point(303, 12);
            this.btnApplyWorkshopContainerFilter.Name = "btnApplyWorkshopContainerFilter";
            this.btnApplyWorkshopContainerFilter.Size = new System.Drawing.Size(75, 23);
            this.btnApplyWorkshopContainerFilter.TabIndex = 1;
            this.btnApplyWorkshopContainerFilter.Tag = "CustomFormsWindow.Tab.Container.Filter.Apply";
            this.btnApplyWorkshopContainerFilter.Text = "Apply Filter";
            this.btnApplyWorkshopContainerFilter.UseVisualStyleBackColor = true;
            this.btnApplyWorkshopContainerFilter.Click += new System.EventHandler(this.btnApplyWorkshopContainerFilterClick);
            // 
            // gbWorkshopNodeDetection
            // 
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionLocationRefs);
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionStaticMarkers);
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionKeywords);
            this.gbWorkshopNodeDetection.Location = new System.Drawing.Point(0, 3);
            this.gbWorkshopNodeDetection.Name = "gbWorkshopNodeDetection";
            this.gbWorkshopNodeDetection.Size = new System.Drawing.Size(330, 310);
            this.gbWorkshopNodeDetection.TabIndex = 18;
            this.gbWorkshopNodeDetection.TabStop = false;
            this.gbWorkshopNodeDetection.Tag = "BorderBatchWindow.NodeDetection";
            this.gbWorkshopNodeDetection.Text = "Node Detection";
            // 
            // gbWorkshopNodeDetectionLocationRefs
            // 
            this.gbWorkshopNodeDetectionLocationRefs.Controls.Add(this.lblWorkshopBordeRefBorderWithBottom);
            this.gbWorkshopNodeDetectionLocationRefs.Controls.Add(this.cbWorkshopBordeRefBorderWithBottom);
            this.gbWorkshopNodeDetectionLocationRefs.Location = new System.Drawing.Point(6, 243);
            this.gbWorkshopNodeDetectionLocationRefs.Name = "gbWorkshopNodeDetectionLocationRefs";
            this.gbWorkshopNodeDetectionLocationRefs.Size = new System.Drawing.Size(318, 62);
            this.gbWorkshopNodeDetectionLocationRefs.TabIndex = 23;
            this.gbWorkshopNodeDetectionLocationRefs.TabStop = false;
            this.gbWorkshopNodeDetectionLocationRefs.Tag = "BorderBatchWindow.NodeDetection.LocationRefs";
            this.gbWorkshopNodeDetectionLocationRefs.Text = "Location References";
            // 
            // lblWorkshopBordeRefBorderWithBottom
            // 
            this.lblWorkshopBordeRefBorderWithBottom.Location = new System.Drawing.Point(6, 16);
            this.lblWorkshopBordeRefBorderWithBottom.Name = "lblWorkshopBordeRefBorderWithBottom";
            this.lblWorkshopBordeRefBorderWithBottom.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopBordeRefBorderWithBottom.TabIndex = 17;
            this.lblWorkshopBordeRefBorderWithBottom.Tag = "BorderBatchWindow.NodeDetection.WorkshopBorderWithBottom:";
            this.lblWorkshopBordeRefBorderWithBottom.Text = "WorkshopBorderWithBottom:";
            // 
            // cbWorkshopBordeRefBorderWithBottom
            // 
            this.cbWorkshopBordeRefBorderWithBottom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBordeRefBorderWithBottom.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopBordeRefBorderWithBottom.Name = "cbWorkshopBordeRefBorderWithBottom";
            this.cbWorkshopBordeRefBorderWithBottom.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBordeRefBorderWithBottom.TabIndex = 13;
            // 
            // gbWorkshopNodeDetectionStaticMarkers
            // 
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.lblWorkshopBorderMarkerTerrainFollowing);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.lblWorkshopForcedZStatic);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerTerrainFollowing);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerForcedZ);
            this.gbWorkshopNodeDetectionStaticMarkers.Location = new System.Drawing.Point(6, 131);
            this.gbWorkshopNodeDetectionStaticMarkers.Name = "gbWorkshopNodeDetectionStaticMarkers";
            this.gbWorkshopNodeDetectionStaticMarkers.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionStaticMarkers.TabIndex = 22;
            this.gbWorkshopNodeDetectionStaticMarkers.TabStop = false;
            this.gbWorkshopNodeDetectionStaticMarkers.Tag = "BorderBatchWindow.NodeDetection.StaticMarkers";
            this.gbWorkshopNodeDetectionStaticMarkers.Text = "StaticMarkers";
            // 
            // lblWorkshopBorderMarkerTerrainFollowing
            // 
            this.lblWorkshopBorderMarkerTerrainFollowing.Location = new System.Drawing.Point(6, 16);
            this.lblWorkshopBorderMarkerTerrainFollowing.Name = "lblWorkshopBorderMarkerTerrainFollowing";
            this.lblWorkshopBorderMarkerTerrainFollowing.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopBorderMarkerTerrainFollowing.TabIndex = 19;
            this.lblWorkshopBorderMarkerTerrainFollowing.Tag = "BorderBatchWindow.NodeDetection.BorderMarkerTerrainFollowing:";
            this.lblWorkshopBorderMarkerTerrainFollowing.Text = "BorderMarkerTerrainFollowing:";
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
            // 
            // cbWorkshopBorderMarkerForcedZ
            // 
            this.cbWorkshopBorderMarkerForcedZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBorderMarkerForcedZ.Location = new System.Drawing.Point(6, 80);
            this.cbWorkshopBorderMarkerForcedZ.Name = "cbWorkshopBorderMarkerForcedZ";
            this.cbWorkshopBorderMarkerForcedZ.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBorderMarkerForcedZ.TabIndex = 16;
            // 
            // gbWorkshopNodeDetectionKeywords
            // 
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lbWorkshopBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lblWorkshopMarkerLink);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderLink);
            this.gbWorkshopNodeDetectionKeywords.Location = new System.Drawing.Point(6, 19);
            this.gbWorkshopNodeDetectionKeywords.Name = "gbWorkshopNodeDetectionKeywords";
            this.gbWorkshopNodeDetectionKeywords.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionKeywords.TabIndex = 21;
            this.gbWorkshopNodeDetectionKeywords.TabStop = false;
            this.gbWorkshopNodeDetectionKeywords.Tag = "BorderBatchWindow.NodeDetection.Keywords";
            this.gbWorkshopNodeDetectionKeywords.Text = "Keywords";
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
            // 
            // cbRestrictWorkshopBorderKeywords
            // 
            this.cbRestrictWorkshopBorderKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRestrictWorkshopBorderKeywords.Checked = true;
            this.cbRestrictWorkshopBorderKeywords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRestrictWorkshopBorderKeywords.Location = new System.Drawing.Point(0, 0);
            this.cbRestrictWorkshopBorderKeywords.Name = "cbRestrictWorkshopBorderKeywords";
            this.cbRestrictWorkshopBorderKeywords.Size = new System.Drawing.Size(740, 30);
            this.cbRestrictWorkshopBorderKeywords.TabIndex = 14;
            this.cbRestrictWorkshopBorderKeywords.Text = "Restrict to:\nYo momma";
            this.cbRestrictWorkshopBorderKeywords.UseVisualStyleBackColor = true;
            // 
            // cbWorkshopContainerFilter
            // 
            this.cbWorkshopContainerFilter.FormattingEnabled = true;
            this.cbWorkshopContainerFilter.Location = new System.Drawing.Point(6, 14);
            this.cbWorkshopContainerFilter.Name = "cbWorkshopContainerFilter";
            this.cbWorkshopContainerFilter.Size = new System.Drawing.Size(291, 21);
            this.cbWorkshopContainerFilter.TabIndex = 2;
            // 
            // CustomForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 375);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(748, 399);
            this.Name = "CustomForms";
            this.Tag = "CustomFormsWindow.Title";
            this.Text = "CustomForms";
            this.TopMost = true;
            this.ClientOnLoad += new System.EventHandler(this.CustomForms_OnLoad);
            this.WindowPanel.ResumeLayout(false);
            this.tcCustomForms.ResumeLayout(false);
            this.tpWorkshop.ResumeLayout(false);
            this.gbWorkshopContainers.ResumeLayout(false);
            this.gbWorkshopContainerFilter.ResumeLayout(false);
            this.gbWorkshopNodeDetection.ResumeLayout(false);
            this.gbWorkshopNodeDetectionLocationRefs.ResumeLayout(false);
            this.gbWorkshopNodeDetectionStaticMarkers.ResumeLayout(false);
            this.gbWorkshopNodeDetectionKeywords.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
