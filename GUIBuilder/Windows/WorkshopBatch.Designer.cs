/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class WorkshopBatch
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbWorkshopFunctions;
        System.Windows.Forms.Button btnOptimizeSandboxVolumes;
        System.Windows.Forms.Button btnCheckMissingElements;
        System.Windows.Forms.Button btnFinalizeElements;
        GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript> lvWorkshops;
        System.Windows.Forms.Button btnNormalizeBuildVolumes;
        System.Windows.Forms.GroupBox gbElements;
        System.Windows.Forms.CheckBox cbElementBorderMarkers;
        System.Windows.Forms.CheckBox cbElementSandboxVolumes;
        
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
            this.lvWorkshops = new GUIBuilder.Windows.Controls.SyncedListView<Fallout4.WorkshopScript>();
            this.gbWorkshopFunctions = new System.Windows.Forms.GroupBox();
            this.gbElements = new System.Windows.Forms.GroupBox();
            this.btnCheckMissingElements = new System.Windows.Forms.Button();
            this.cbElementSandboxVolumes = new System.Windows.Forms.CheckBox();
            this.cbElementBorderMarkers = new System.Windows.Forms.CheckBox();
            this.btnNormalizeBuildVolumes = new System.Windows.Forms.Button();
            this.btnOptimizeSandboxVolumes = new System.Windows.Forms.Button();
            this.btnFinalizeElements = new System.Windows.Forms.Button();
            this.WindowPanel.SuspendLayout();
            this.gbWorkshopFunctions.SuspendLayout();
            this.gbElements.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.lvWorkshops);
            this.WindowPanel.Controls.Add(this.gbWorkshopFunctions);
            this.WindowPanel.Size = new System.Drawing.Size(514, 404);
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
            this.lvWorkshops.Location = new System.Drawing.Point(189, 3);
            this.lvWorkshops.MultiSelect = true;
            this.lvWorkshops.Name = "lvWorkshops";
            this.lvWorkshops.Size = new System.Drawing.Size(323, 399);
            this.lvWorkshops.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvWorkshops.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvWorkshops.SyncedEditorFormType = null;
            this.lvWorkshops.SyncObjects = null;
            this.lvWorkshops.TabIndex = 11;
            this.lvWorkshops.TypeColumn = false;
            // 
            // gbWorkshopFunctions
            // 
            this.gbWorkshopFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbWorkshopFunctions.Controls.Add(this.gbElements);
            this.gbWorkshopFunctions.Controls.Add(this.btnNormalizeBuildVolumes);
            this.gbWorkshopFunctions.Controls.Add(this.btnOptimizeSandboxVolumes);
            this.gbWorkshopFunctions.Controls.Add(this.btnFinalizeElements);
            this.gbWorkshopFunctions.Location = new System.Drawing.Point(0, 0);
            this.gbWorkshopFunctions.Name = "gbWorkshopFunctions";
            this.gbWorkshopFunctions.Size = new System.Drawing.Size(180, 402);
            this.gbWorkshopFunctions.TabIndex = 12;
            this.gbWorkshopFunctions.TabStop = false;
            // 
            // gbElements
            // 
            this.gbElements.Controls.Add(this.btnCheckMissingElements);
            this.gbElements.Controls.Add(this.cbElementSandboxVolumes);
            this.gbElements.Controls.Add(this.cbElementBorderMarkers);
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
            this.cbElementSandboxVolumes.Location = new System.Drawing.Point(6, 52);
            this.cbElementSandboxVolumes.Name = "cbElementSandboxVolumes";
            this.cbElementSandboxVolumes.Size = new System.Drawing.Size(153, 17);
            this.cbElementSandboxVolumes.TabIndex = 2;
            this.cbElementSandboxVolumes.Tag = "Workshop.Element.SandboxVolumes";
            this.cbElementSandboxVolumes.Text = "Sandbox Volumes";
            this.cbElementSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // cbElementBorderMarkers
            // 
            this.cbElementBorderMarkers.Checked = true;
            this.cbElementBorderMarkers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementBorderMarkers.Location = new System.Drawing.Point(6, 29);
            this.cbElementBorderMarkers.Name = "cbElementBorderMarkers";
            this.cbElementBorderMarkers.Size = new System.Drawing.Size(153, 17);
            this.cbElementBorderMarkers.TabIndex = 0;
            this.cbElementBorderMarkers.Tag = "WorkshopBatchWindow.BorderMarkers";
            this.cbElementBorderMarkers.Text = "Border Markers";
            this.cbElementBorderMarkers.UseVisualStyleBackColor = true;
            // 
            // btnNormalizeBuildVolumes
            // 
            this.btnNormalizeBuildVolumes.Location = new System.Drawing.Point(12, 90);
            this.btnNormalizeBuildVolumes.Name = "btnNormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnNormalizeBuildVolumes.TabIndex = 3;
            this.btnNormalizeBuildVolumes.Tag = "BatchWindow.Function.NormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Text = "Normalize Build Volumes";
            this.btnNormalizeBuildVolumes.UseVisualStyleBackColor = true;
            // 
            // btnOptimizeSandboxVolumes
            // 
            this.btnOptimizeSandboxVolumes.Location = new System.Drawing.Point(12, 119);
            this.btnOptimizeSandboxVolumes.Name = "btnOptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnOptimizeSandboxVolumes.TabIndex = 0;
            this.btnOptimizeSandboxVolumes.Tag = "BatchWindow.Function.OptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Text = "Optimize Sandboxes";
            this.btnOptimizeSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // btnFinalizeElements
            // 
            this.btnFinalizeElements.Enabled = false;
            this.btnFinalizeElements.Location = new System.Drawing.Point(12, 148);
            this.btnFinalizeElements.Name = "btnFinalizeElements";
            this.btnFinalizeElements.Size = new System.Drawing.Size(156, 23);
            this.btnFinalizeElements.TabIndex = 2;
            this.btnFinalizeElements.Tag = "BatchWindow.Function.FinalizeElements";
            this.btnFinalizeElements.Text = "Finalize Elements";
            this.btnFinalizeElements.UseVisualStyleBackColor = true;
            // 
            // WorkshopBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 404);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 426);
            this.Name = "WorkshopBatch";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "WorkshopBatchWindow.Title";
            this.WindowPanel.ResumeLayout(false);
            this.gbWorkshopFunctions.ResumeLayout(false);
            this.gbElements.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
