/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows.FormEditor
{
    partial class SyncedFormEditor<TSync>
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbUIDs;
        System.Windows.Forms.TextBox tbEditorID;
        System.Windows.Forms.Label lblEditorID;
        System.Windows.Forms.TextBox tbFormID;
        System.Windows.Forms.Label lblFormID;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.Button btnApply;
        
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
            this.gbUIDs = new System.Windows.Forms.GroupBox();
            this.tbEditorID = new System.Windows.Forms.TextBox();
            this.lblEditorID = new System.Windows.Forms.Label();
            this.tbFormID = new System.Windows.Forms.TextBox();
            this.lblFormID = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.gbUIDs.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUIDs
            // 
            this.gbUIDs.Controls.Add(this.tbEditorID);
            this.gbUIDs.Controls.Add(this.lblEditorID);
            this.gbUIDs.Controls.Add(this.tbFormID);
            this.gbUIDs.Controls.Add(this.lblFormID);
            this.gbUIDs.Location = new System.Drawing.Point(3, 3);
            this.gbUIDs.Name = "gbUIDs";
            this.gbUIDs.Size = new System.Drawing.Size(296, 73);
            this.gbUIDs.TabIndex = 0;
            this.gbUIDs.TabStop = false;
            this.gbUIDs.Tag = "FormEditor.UID.Title:";
            this.gbUIDs.Text = "Unique Identifiers:";
            // 
            // tbEditorID
            // 
            this.tbEditorID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEditorID.Location = new System.Drawing.Point(70, 42);
            this.tbEditorID.Name = "tbEditorID";
            this.tbEditorID.Size = new System.Drawing.Size(220, 20);
            this.tbEditorID.TabIndex = 7;
            // 
            // lblEditorID
            // 
            this.lblEditorID.Location = new System.Drawing.Point(6, 45);
            this.lblEditorID.Name = "lblEditorID";
            this.lblEditorID.Size = new System.Drawing.Size(100, 23);
            this.lblEditorID.TabIndex = 6;
            this.lblEditorID.Tag = "Form.EditorID:";
            this.lblEditorID.Text = "EditorID:";
            // 
            // tbFormID
            // 
            this.tbFormID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFormID.BackColor = System.Drawing.SystemColors.Control;
            this.tbFormID.Enabled = false;
            this.tbFormID.Location = new System.Drawing.Point(70, 19);
            this.tbFormID.Name = "tbFormID";
            this.tbFormID.Size = new System.Drawing.Size(220, 20);
            this.tbFormID.TabIndex = 5;
            // 
            // lblFormID
            // 
            this.lblFormID.Location = new System.Drawing.Point(6, 22);
            this.lblFormID.Name = "lblFormID";
            this.lblFormID.Size = new System.Drawing.Size(100, 23);
            this.lblFormID.TabIndex = 4;
            this.lblFormID.Tag = "Form.FormID:";
            this.lblFormID.Text = "FormID:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(224, 80);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Tag = "FormEditor.Cancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancelClick);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(143, 80);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Tag = "FormEditor.Apply";
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApplyClick);
            // 
            // SyncedFormEditor
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(302, 106);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbUIDs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(310, 130);
            this.Name = "SyncedFormEditor";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SyncedFormEditor";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.gbUIDs.ResumeLayout(false);
            this.gbUIDs.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
