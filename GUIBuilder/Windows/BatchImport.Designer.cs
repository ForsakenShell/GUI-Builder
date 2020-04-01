/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class BatchImport
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        GUIBuilder.Windows.Controls.SyncedListView<FormImport.ImportBase> lvImportForms;
        System.Windows.Forms.Button btnImportSelected;
        System.Windows.Forms.Button btnClose;
        System.Windows.Forms.SplitContainer scImports;
        System.Windows.Forms.TextBox tbImportMessages;
        
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
            this.scImports = new System.Windows.Forms.SplitContainer();
            this.lvImportForms = new GUIBuilder.Windows.Controls.SyncedListView<GUIBuilder.FormImport.ImportBase>();
            this.tbImportMessages = new System.Windows.Forms.TextBox();
            this.btnImportSelected = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.WindowPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scImports)).BeginInit();
            this.scImports.Panel1.SuspendLayout();
            this.scImports.Panel2.SuspendLayout();
            this.scImports.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.scImports);
            this.WindowPanel.Controls.Add(this.btnImportSelected);
            this.WindowPanel.Controls.Add(this.btnClose);
            this.WindowPanel.Size = new System.Drawing.Size(942, 552);
            // 
            // scImports
            // 
            this.scImports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scImports.Location = new System.Drawing.Point(0, 0);
            this.scImports.Name = "scImports";
            this.scImports.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scImports.Panel1
            // 
            this.scImports.Panel1.Controls.Add(this.lvImportForms);
            this.scImports.Panel1MinSize = 90;
            // 
            // scImports.Panel2
            // 
            this.scImports.Panel2.Controls.Add(this.tbImportMessages);
            this.scImports.Panel2MinSize = 65;
            this.scImports.Size = new System.Drawing.Size(942, 516);
            this.scImports.SplitterDistance = 446;
            this.scImports.TabIndex = 15;
            // 
            // lvImportForms
            // 
            this.lvImportForms.AllowHidingItems = true;
            this.lvImportForms.AllowOverrideColumnSorting = true;
            this.lvImportForms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvImportForms.AutoScroll = true;
            this.lvImportForms.CheckBoxes = true;
            this.lvImportForms.EditorIDColumn = true;
            this.lvImportForms.ExtraInfoColumn = true;
            this.lvImportForms.FilenameColumn = false;
            this.lvImportForms.FormIDColumn = true;
            this.lvImportForms.LoadOrderColumn = false;
            this.lvImportForms.Location = new System.Drawing.Point(3, 3);
            this.lvImportForms.MultiSelect = true;
            this.lvImportForms.Name = "lvImportForms";
            this.lvImportForms.Size = new System.Drawing.Size(936, 440);
            this.lvImportForms.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.Custom;
            this.lvImportForms.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvImportForms.SyncedEditorFormType = null;
            this.lvImportForms.SyncObjects = null;
            this.lvImportForms.TabIndex = 12;
            this.lvImportForms.TypeColumn = true;
            // 
            // tbImportMessages
            // 
            this.tbImportMessages.AcceptsReturn = true;
            this.tbImportMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbImportMessages.HideSelection = false;
            this.tbImportMessages.Location = new System.Drawing.Point(3, 3);
            this.tbImportMessages.Multiline = true;
            this.tbImportMessages.Name = "tbImportMessages";
            this.tbImportMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbImportMessages.Size = new System.Drawing.Size(936, 60);
            this.tbImportMessages.TabIndex = 0;
            // 
            // btnImportSelected
            // 
            this.btnImportSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportSelected.Location = new System.Drawing.Point(779, 522);
            this.btnImportSelected.Name = "btnImportSelected";
            this.btnImportSelected.Size = new System.Drawing.Size(75, 23);
            this.btnImportSelected.TabIndex = 14;
            this.btnImportSelected.Tag = "BatchImportWindow.Import";
            this.btnImportSelected.Text = "Import";
            this.btnImportSelected.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(864, 522);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Tag = "BatchImportWindow.Close";
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // BatchImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 552);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(640, 256);
            this.Name = "BatchImport";
            this.Tag = "BatchImportWindow.Title";
            this.Text = "title";
            this.WindowPanel.ResumeLayout(false);
            this.scImports.Panel1.ResumeLayout(false);
            this.scImports.Panel2.ResumeLayout(false);
            this.scImports.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scImports)).EndInit();
            this.scImports.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
