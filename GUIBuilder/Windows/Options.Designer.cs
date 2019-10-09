/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class Options
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbAlwaysSelectMasters;
        GUIBuilder.Windows.Controls.SyncedListView<GodObject.Master.File> lvAlwaysSelectMasters;
        System.Windows.Forms.GroupBox gbConflictStatus;
        System.Windows.Forms.TextBox tbCSInvalid;
        System.Windows.Forms.TextBox tbCSNoConflict;
        System.Windows.Forms.TextBox tbCSOverrideInWorkingFile;
        System.Windows.Forms.TextBox tbCSOverrideInAncestor;
        System.Windows.Forms.TextBox tbCSNewForm;
        System.Windows.Forms.TextBox tbCSRequiresOverride;
        System.Windows.Forms.TextBox tbCSUneditable;
        
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
            this.gbAlwaysSelectMasters = new System.Windows.Forms.GroupBox();
            this.lvAlwaysSelectMasters = new GUIBuilder.Windows.Controls.SyncedListView<GodObject.Master.File>();
            this.gbConflictStatus = new System.Windows.Forms.GroupBox();
            this.tbCSUneditable = new System.Windows.Forms.TextBox();
            this.tbCSRequiresOverride = new System.Windows.Forms.TextBox();
            this.tbCSNoConflict = new System.Windows.Forms.TextBox();
            this.tbCSOverrideInWorkingFile = new System.Windows.Forms.TextBox();
            this.tbCSOverrideInAncestor = new System.Windows.Forms.TextBox();
            this.tbCSNewForm = new System.Windows.Forms.TextBox();
            this.tbCSInvalid = new System.Windows.Forms.TextBox();
            this.gbAlwaysSelectMasters.SuspendLayout();
            this.gbConflictStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbAlwaysSelectMasters
            // 
            this.gbAlwaysSelectMasters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAlwaysSelectMasters.Controls.Add(this.lvAlwaysSelectMasters);
            this.gbAlwaysSelectMasters.Location = new System.Drawing.Point(3, 3);
            this.gbAlwaysSelectMasters.Name = "gbAlwaysSelectMasters";
            this.gbAlwaysSelectMasters.Size = new System.Drawing.Size(309, 250);
            this.gbAlwaysSelectMasters.TabIndex = 0;
            this.gbAlwaysSelectMasters.TabStop = false;
            this.gbAlwaysSelectMasters.Text = "Always Select Masters";
            // 
            // lvAlwaysSelectMasters
            // 
            this.lvAlwaysSelectMasters.AllowHidingItems = false;
            this.lvAlwaysSelectMasters.AllowOverrideColumnSorting = false;
            this.lvAlwaysSelectMasters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAlwaysSelectMasters.CheckBoxes = true;
            this.lvAlwaysSelectMasters.EditorIDColumn = false;
            this.lvAlwaysSelectMasters.ExtraInfoColumn = false;
            this.lvAlwaysSelectMasters.FilenameColumn = true;
            this.lvAlwaysSelectMasters.FormIDColumn = false;
            this.lvAlwaysSelectMasters.LoadOrderColumn = true;
            this.lvAlwaysSelectMasters.Location = new System.Drawing.Point(6, 19);
            this.lvAlwaysSelectMasters.MultiSelect = false;
            this.lvAlwaysSelectMasters.Name = "lvAlwaysSelectMasters";
            this.lvAlwaysSelectMasters.Size = new System.Drawing.Size(297, 225);
            this.lvAlwaysSelectMasters.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.LoadOrder;
            this.lvAlwaysSelectMasters.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvAlwaysSelectMasters.SyncedEditorFormType = null;
            this.lvAlwaysSelectMasters.SyncObjects = null;
            this.lvAlwaysSelectMasters.TabIndex = 13;
            this.lvAlwaysSelectMasters.TypeColumn = true;
            // 
            // gbConflictStatus
            // 
            this.gbConflictStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbConflictStatus.Controls.Add(this.tbCSUneditable);
            this.gbConflictStatus.Controls.Add(this.tbCSRequiresOverride);
            this.gbConflictStatus.Controls.Add(this.tbCSNoConflict);
            this.gbConflictStatus.Controls.Add(this.tbCSOverrideInWorkingFile);
            this.gbConflictStatus.Controls.Add(this.tbCSOverrideInAncestor);
            this.gbConflictStatus.Controls.Add(this.tbCSNewForm);
            this.gbConflictStatus.Controls.Add(this.tbCSInvalid);
            this.gbConflictStatus.Location = new System.Drawing.Point(3, 259);
            this.gbConflictStatus.Name = "gbConflictStatus";
            this.gbConflictStatus.Size = new System.Drawing.Size(309, 116);
            this.gbConflictStatus.TabIndex = 1;
            this.gbConflictStatus.TabStop = false;
            this.gbConflictStatus.Text = "Conflict Status";
            // 
            // tbCSUneditable
            // 
            this.tbCSUneditable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSUneditable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSUneditable.Location = new System.Drawing.Point(9, 32);
            this.tbCSUneditable.Name = "tbCSUneditable";
            this.tbCSUneditable.ReadOnly = true;
            this.tbCSUneditable.Size = new System.Drawing.Size(292, 13);
            this.tbCSUneditable.TabIndex = 1;
            this.tbCSUneditable.Text = "Uneditable";
            // 
            // tbCSRequiresOverride
            // 
            this.tbCSRequiresOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSRequiresOverride.BackColor = System.Drawing.SystemColors.Control;
            this.tbCSRequiresOverride.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSRequiresOverride.Location = new System.Drawing.Point(9, 97);
            this.tbCSRequiresOverride.Name = "tbCSRequiresOverride";
            this.tbCSRequiresOverride.ReadOnly = true;
            this.tbCSRequiresOverride.Size = new System.Drawing.Size(292, 13);
            this.tbCSRequiresOverride.TabIndex = 6;
            this.tbCSRequiresOverride.Text = "Requires Override";
            // 
            // tbCSNoConflict
            // 
            this.tbCSNoConflict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSNoConflict.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSNoConflict.Location = new System.Drawing.Point(9, 58);
            this.tbCSNoConflict.Name = "tbCSNoConflict";
            this.tbCSNoConflict.ReadOnly = true;
            this.tbCSNoConflict.Size = new System.Drawing.Size(292, 13);
            this.tbCSNoConflict.TabIndex = 3;
            this.tbCSNoConflict.Text = "No Conflict";
            // 
            // tbCSOverrideInWorkingFile
            // 
            this.tbCSOverrideInWorkingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSOverrideInWorkingFile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSOverrideInWorkingFile.Location = new System.Drawing.Point(9, 84);
            this.tbCSOverrideInWorkingFile.Name = "tbCSOverrideInWorkingFile";
            this.tbCSOverrideInWorkingFile.ReadOnly = true;
            this.tbCSOverrideInWorkingFile.Size = new System.Drawing.Size(292, 13);
            this.tbCSOverrideInWorkingFile.TabIndex = 5;
            this.tbCSOverrideInWorkingFile.Text = "Override in Working File";
            // 
            // tbCSOverrideInAncestor
            // 
            this.tbCSOverrideInAncestor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSOverrideInAncestor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSOverrideInAncestor.Location = new System.Drawing.Point(9, 71);
            this.tbCSOverrideInAncestor.Name = "tbCSOverrideInAncestor";
            this.tbCSOverrideInAncestor.ReadOnly = true;
            this.tbCSOverrideInAncestor.Size = new System.Drawing.Size(292, 13);
            this.tbCSOverrideInAncestor.TabIndex = 4;
            this.tbCSOverrideInAncestor.Text = "Override in Ancestor";
            // 
            // tbCSNewForm
            // 
            this.tbCSNewForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSNewForm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSNewForm.Location = new System.Drawing.Point(9, 45);
            this.tbCSNewForm.Name = "tbCSNewForm";
            this.tbCSNewForm.ReadOnly = true;
            this.tbCSNewForm.Size = new System.Drawing.Size(292, 13);
            this.tbCSNewForm.TabIndex = 2;
            this.tbCSNewForm.Text = "New Form";
            // 
            // tbCSInvalid
            // 
            this.tbCSInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSInvalid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbCSInvalid.Location = new System.Drawing.Point(9, 19);
            this.tbCSInvalid.Name = "tbCSInvalid";
            this.tbCSInvalid.ReadOnly = true;
            this.tbCSInvalid.Size = new System.Drawing.Size(292, 13);
            this.tbCSInvalid.TabIndex = 0;
            this.tbCSInvalid.Text = "Error State";
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 376);
            this.Controls.Add(this.gbConflictStatus);
            this.Controls.Add(this.gbAlwaysSelectMasters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(240, 262);
            this.Name = "Options";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "GUIBuilder Options";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.gbAlwaysSelectMasters.ResumeLayout(false);
            this.gbConflictStatus.ResumeLayout(false);
            this.gbConflictStatus.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
