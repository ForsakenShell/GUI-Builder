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
        System.Windows.Forms.Button btnFinalizeElements;
        GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> lvSubDivisions;
        System.Windows.Forms.Button btnNormalizeBuildVolumes;
        System.Windows.Forms.GroupBox gbElements;
        System.Windows.Forms.CheckBox cbElementBorderEnablers;
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
            this.lvSubDivisions = new GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision>();
            this.gbSubDivisionFunctions = new System.Windows.Forms.GroupBox();
            this.gbElements = new System.Windows.Forms.GroupBox();
            this.btnCheckMissingElements = new System.Windows.Forms.Button();
            this.cbElementSandboxVolumes = new System.Windows.Forms.CheckBox();
            this.cbElementBorderEnablers = new System.Windows.Forms.CheckBox();
            this.btnNormalizeBuildVolumes = new System.Windows.Forms.Button();
            this.btnOptimizeSandboxVolumes = new System.Windows.Forms.Button();
            this.btnFinalizeElements = new System.Windows.Forms.Button();
            this.WindowPanel.SuspendLayout();
            this.gbSubDivisionFunctions.SuspendLayout();
            this.gbElements.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnWindow
            // 
            this.WindowPanel.Controls.Add(this.lvSubDivisions);
            this.WindowPanel.Controls.Add(this.gbSubDivisionFunctions);
            this.WindowPanel.Size = new System.Drawing.Size( 514, 404 );
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
            this.lvSubDivisions.SyncedEditorFormType = null;
            this.lvSubDivisions.SyncObjects = null;
            this.lvSubDivisions.TabIndex = 11;
            this.lvSubDivisions.TypeColumn = false;
            // 
            // gbSubDivisionFunctions
            // 
            this.gbSubDivisionFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbSubDivisionFunctions.Controls.Add(this.gbElements);
            this.gbSubDivisionFunctions.Controls.Add(this.btnNormalizeBuildVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.btnOptimizeSandboxVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.btnFinalizeElements);
            this.gbSubDivisionFunctions.Location = new System.Drawing.Point(0, 0);
            this.gbSubDivisionFunctions.Name = "gbSubDivisionFunctions";
            this.gbSubDivisionFunctions.Size = new System.Drawing.Size(180, 402);
            this.gbSubDivisionFunctions.TabIndex = 12;
            this.gbSubDivisionFunctions.TabStop = false;
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
            this.gbElements.Tag = "";
            // 
            // btnCheckMissingElements
            // 
            this.btnCheckMissingElements.Location = new System.Drawing.Point(6, 0);
            this.btnCheckMissingElements.Name = "btnCheckMissingElements";
            this.btnCheckMissingElements.Size = new System.Drawing.Size(156, 23);
            this.btnCheckMissingElements.TabIndex = 1;
            this.btnCheckMissingElements.Tag = "SubDivisionBatchWindow.Function.CheckElements";
            this.btnCheckMissingElements.Text = "Check For Missing Elements";
            this.btnCheckMissingElements.UseVisualStyleBackColor = true;
            this.btnCheckMissingElements.Click += new System.EventHandler(this.btnCheckMissingElementsClick);
            // 
            // cbElementSandboxVolumes
            // 
            this.cbElementSandboxVolumes.Checked = true;
            this.cbElementSandboxVolumes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementSandboxVolumes.Location = new System.Drawing.Point(6, 52);
            this.cbElementSandboxVolumes.Name = "cbElementSandboxVolumes";
            this.cbElementSandboxVolumes.Size = new System.Drawing.Size(153, 17);
            this.cbElementSandboxVolumes.TabIndex = 2;
            this.cbElementSandboxVolumes.Tag = "SubDivisionBatchWindow.SandboxVolumes";
            this.cbElementSandboxVolumes.Text = "Sandbox Volumes";
            this.cbElementSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // cbElementBorderEnablers
            // 
            this.cbElementBorderEnablers.Checked = true;
            this.cbElementBorderEnablers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementBorderEnablers.Location = new System.Drawing.Point(6, 29);
            this.cbElementBorderEnablers.Name = "cbElementBorderEnablers";
            this.cbElementBorderEnablers.Size = new System.Drawing.Size(153, 17);
            this.cbElementBorderEnablers.TabIndex = 0;
            this.cbElementBorderEnablers.Tag = "SubDivisionBatchWindow.BorderEnablers";
            this.cbElementBorderEnablers.Text = "Border Enablers";
            this.cbElementBorderEnablers.UseVisualStyleBackColor = true;
            // 
            // btnNormalizeBuildVolumes
            // 
            this.btnNormalizeBuildVolumes.Location = new System.Drawing.Point(12, 90);
            this.btnNormalizeBuildVolumes.Name = "btnNormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnNormalizeBuildVolumes.TabIndex = 3;
            this.btnNormalizeBuildVolumes.Tag = "SubDivisionBatchWindow.Function.NormalizeBuildVolumes";
            this.btnNormalizeBuildVolumes.Text = "Normalize Build Volumes";
            this.btnNormalizeBuildVolumes.UseVisualStyleBackColor = true;
            this.btnNormalizeBuildVolumes.Click += new System.EventHandler(this.btnNormalizeBuildVolumesClick);
            // 
            // btnOptimizeSandboxVolumes
            // 
            this.btnOptimizeSandboxVolumes.Location = new System.Drawing.Point(12, 119);
            this.btnOptimizeSandboxVolumes.Name = "btnOptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Size = new System.Drawing.Size(156, 23);
            this.btnOptimizeSandboxVolumes.TabIndex = 0;
            this.btnOptimizeSandboxVolumes.Tag = "SubDivisionBatchWindow.Function.OptimizeSandboxVolumes";
            this.btnOptimizeSandboxVolumes.Text = "Optimize Sandbox Volumes";
            this.btnOptimizeSandboxVolumes.UseVisualStyleBackColor = true;
            this.btnOptimizeSandboxVolumes.Click += new System.EventHandler(this.btnOptimizeSandboxVolumesClick);
            // 
            // btnFinalizeElements
            // 
            this.btnFinalizeElements.Enabled = false;
            this.btnFinalizeElements.Location = new System.Drawing.Point(12, 148);
            this.btnFinalizeElements.Name = "btnFinalizeElements";
            this.btnFinalizeElements.Size = new System.Drawing.Size(156, 23);
            this.btnFinalizeElements.TabIndex = 2;
            this.btnFinalizeElements.Tag = "SubDivisionBatchWindow.Function.FinalizeElements";
            this.btnFinalizeElements.Text = "Finalize All Elements";
            this.btnFinalizeElements.UseVisualStyleBackColor = true;
            // 
            // SubDivisionBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 404);
            this.Controls.Add(this.WindowPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 426);
            this.Name = "SubDivisionBatch";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "SubDivisionBatchWindow.Title";
            this.Text = "title";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.gbSubDivisionFunctions.ResumeLayout(false);
            this.gbElements.ResumeLayout(false);
            this.WindowPanel.ResumeLayout( false );
            this.WindowPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
