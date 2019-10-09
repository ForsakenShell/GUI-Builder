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
        System.Windows.Forms.Panel pnWindow;
        System.Windows.Forms.GroupBox gbSubDivisionFunctions;
        System.Windows.Forms.Button btnOptimizeVolumes;
        System.Windows.Forms.Button btnCheckMissingElements;
        System.Windows.Forms.Button btnFinalizeElements;
        GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision> lvSubDivisions;
        System.Windows.Forms.Button btnNormalizeSandboxVolumes;
        System.Windows.Forms.GroupBox gbElements;
        System.Windows.Forms.CheckBox cbElementBorderEnablers;
        System.Windows.Forms.CheckBox cbElementSandboxVolumes;
        System.Windows.Forms.CheckBox cbElementsEdgeFlags;
        System.Windows.Forms.CheckBox cbElementBuildVolumes;
        
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
            this.gbElements = new System.Windows.Forms.GroupBox();
            this.cbElementSandboxVolumes = new System.Windows.Forms.CheckBox();
            this.cbElementsEdgeFlags = new System.Windows.Forms.CheckBox();
            this.cbElementBorderEnablers = new System.Windows.Forms.CheckBox();
            this.lvSubDivisions = new GUIBuilder.Windows.Controls.SyncedListView<AnnexTheCommonwealth.SubDivision>();
            this.gbSubDivisionFunctions = new System.Windows.Forms.GroupBox();
            this.btnNormalizeSandboxVolumes = new System.Windows.Forms.Button();
            this.btnOptimizeVolumes = new System.Windows.Forms.Button();
            this.btnCheckMissingElements = new System.Windows.Forms.Button();
            this.btnFinalizeElements = new System.Windows.Forms.Button();
            this.cbElementBuildVolumes = new System.Windows.Forms.CheckBox();
            this.pnWindow.SuspendLayout();
            this.gbElements.SuspendLayout();
            this.gbSubDivisionFunctions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnWindow
            // 
            this.pnWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnWindow.Controls.Add(this.gbElements);
            this.pnWindow.Controls.Add(this.lvSubDivisions);
            this.pnWindow.Controls.Add(this.gbSubDivisionFunctions);
            this.pnWindow.Location = new System.Drawing.Point(0, 0);
            this.pnWindow.Name = "pnWindow";
            this.pnWindow.Size = new System.Drawing.Size(514, 404);
            this.pnWindow.TabIndex = 0;
            // 
            // gbElements
            // 
            this.gbElements.Controls.Add(this.cbElementBuildVolumes);
            this.gbElements.Controls.Add(this.cbElementSandboxVolumes);
            this.gbElements.Controls.Add(this.cbElementsEdgeFlags);
            this.gbElements.Controls.Add(this.cbElementBorderEnablers);
            this.gbElements.Location = new System.Drawing.Point(3, 143);
            this.gbElements.Name = "gbElements";
            this.gbElements.Size = new System.Drawing.Size(180, 258);
            this.gbElements.TabIndex = 12;
            this.gbElements.TabStop = false;
            this.gbElements.Text = "Elements";
            // 
            // cbElementSandboxVolumes
            // 
            this.cbElementSandboxVolumes.Checked = true;
            this.cbElementSandboxVolumes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementSandboxVolumes.Location = new System.Drawing.Point(6, 65);
            this.cbElementSandboxVolumes.Name = "cbElementSandboxVolumes";
            this.cbElementSandboxVolumes.Size = new System.Drawing.Size(168, 17);
            this.cbElementSandboxVolumes.TabIndex = 2;
            this.cbElementSandboxVolumes.Text = "Sandbox Volumes";
            this.cbElementSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // cbElementsEdgeFlags
            // 
            this.cbElementsEdgeFlags.Checked = true;
            this.cbElementsEdgeFlags.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementsEdgeFlags.Location = new System.Drawing.Point(6, 19);
            this.cbElementsEdgeFlags.Name = "cbElementsEdgeFlags";
            this.cbElementsEdgeFlags.Size = new System.Drawing.Size(168, 17);
            this.cbElementsEdgeFlags.TabIndex = 1;
            this.cbElementsEdgeFlags.Text = "Edge Flags";
            this.cbElementsEdgeFlags.UseVisualStyleBackColor = true;
            // 
            // cbElementBorderEnablers
            // 
            this.cbElementBorderEnablers.Checked = true;
            this.cbElementBorderEnablers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementBorderEnablers.Location = new System.Drawing.Point(6, 42);
            this.cbElementBorderEnablers.Name = "cbElementBorderEnablers";
            this.cbElementBorderEnablers.Size = new System.Drawing.Size(168, 17);
            this.cbElementBorderEnablers.TabIndex = 0;
            this.cbElementBorderEnablers.Text = "Border Enablers";
            this.cbElementBorderEnablers.UseVisualStyleBackColor = true;
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
            this.gbSubDivisionFunctions.Controls.Add(this.btnNormalizeSandboxVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.btnOptimizeVolumes);
            this.gbSubDivisionFunctions.Controls.Add(this.btnCheckMissingElements);
            this.gbSubDivisionFunctions.Controls.Add(this.btnFinalizeElements);
            this.gbSubDivisionFunctions.Location = new System.Drawing.Point(3, 3);
            this.gbSubDivisionFunctions.Name = "gbSubDivisionFunctions";
            this.gbSubDivisionFunctions.Size = new System.Drawing.Size(180, 134);
            this.gbSubDivisionFunctions.TabIndex = 10;
            this.gbSubDivisionFunctions.TabStop = false;
            this.gbSubDivisionFunctions.Text = "Functions";
            // 
            // btnNormalizeSandboxVolumes
            // 
            this.btnNormalizeSandboxVolumes.Location = new System.Drawing.Point(6, 48);
            this.btnNormalizeSandboxVolumes.Name = "btnNormalizeSandboxVolumes";
            this.btnNormalizeSandboxVolumes.Size = new System.Drawing.Size(168, 23);
            this.btnNormalizeSandboxVolumes.TabIndex = 3;
            this.btnNormalizeSandboxVolumes.Text = "Normalize Build Volumes";
            this.btnNormalizeSandboxVolumes.UseVisualStyleBackColor = true;
            // 
            // btnOptimizeVolumes
            // 
            this.btnOptimizeVolumes.Location = new System.Drawing.Point(6, 77);
            this.btnOptimizeVolumes.Name = "btnOptimizeVolumes";
            this.btnOptimizeVolumes.Size = new System.Drawing.Size(168, 23);
            this.btnOptimizeVolumes.TabIndex = 0;
            this.btnOptimizeVolumes.Text = "Optimize Volumes";
            this.btnOptimizeVolumes.UseVisualStyleBackColor = true;
            this.btnOptimizeVolumes.Click += new System.EventHandler(this.btnOptimizeClick);
            // 
            // btnCheckMissingElements
            // 
            this.btnCheckMissingElements.Location = new System.Drawing.Point(6, 19);
            this.btnCheckMissingElements.Name = "btnCheckMissingElements";
            this.btnCheckMissingElements.Size = new System.Drawing.Size(168, 23);
            this.btnCheckMissingElements.TabIndex = 1;
            this.btnCheckMissingElements.Text = "Check For Missing Elements";
            this.btnCheckMissingElements.UseVisualStyleBackColor = true;
            this.btnCheckMissingElements.Click += new System.EventHandler(this.btnCheckMissingElementsClick);
            // 
            // btnFinalizeElements
            // 
            this.btnFinalizeElements.Location = new System.Drawing.Point(6, 106);
            this.btnFinalizeElements.Name = "btnFinalizeElements";
            this.btnFinalizeElements.Size = new System.Drawing.Size(168, 23);
            this.btnFinalizeElements.TabIndex = 2;
            this.btnFinalizeElements.Text = "Finalize All Elements";
            this.btnFinalizeElements.UseVisualStyleBackColor = true;
            // 
            // cbElementBuildVolumes
            // 
            this.cbElementBuildVolumes.Checked = true;
            this.cbElementBuildVolumes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbElementBuildVolumes.Location = new System.Drawing.Point(6, 88);
            this.cbElementBuildVolumes.Name = "cbElementBuildVolumes";
            this.cbElementBuildVolumes.Size = new System.Drawing.Size(168, 17);
            this.cbElementBuildVolumes.TabIndex = 3;
            this.cbElementBuildVolumes.Text = "Build Volumes";
            this.cbElementBuildVolumes.UseVisualStyleBackColor = true;
            // 
            // SubDivisionBatchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 404);
            this.Controls.Add(this.pnWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 426);
            this.Name = "SubDivisionBatchWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Sub-Division Batch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.pnWindow.ResumeLayout(false);
            this.gbElements.ResumeLayout(false);
            this.gbSubDivisionFunctions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
