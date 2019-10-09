/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows.FormEditor
{
    partial class SubDivision
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.GroupBox gbLocation;
        System.Windows.Forms.Panel pnLocation;
        System.Windows.Forms.TextBox tbLocationEditorID;
        System.Windows.Forms.TextBox tbLocationFormID;
        System.Windows.Forms.Label lblLocationFormID;
        System.Windows.Forms.Label lblLocationEditorID;
        System.Windows.Forms.ComboBox cbLocation;
        
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
            this.gbLocation = new System.Windows.Forms.GroupBox();
            this.pnLocation = new System.Windows.Forms.Panel();
            this.tbLocationEditorID = new System.Windows.Forms.TextBox();
            this.tbLocationFormID = new System.Windows.Forms.TextBox();
            this.lblLocationFormID = new System.Windows.Forms.Label();
            this.lblLocationEditorID = new System.Windows.Forms.Label();
            this.cbLocation = new System.Windows.Forms.ComboBox();
            this.gbLocation.SuspendLayout();
            this.pnLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbLocation
            // 
            this.gbLocation.Controls.Add(this.pnLocation);
            this.gbLocation.Controls.Add(this.cbLocation);
            this.gbLocation.Location = new System.Drawing.Point(3, 82);
            this.gbLocation.Name = "gbLocation";
            this.gbLocation.Size = new System.Drawing.Size(298, 80);
            this.gbLocation.TabIndex = 5;
            this.gbLocation.TabStop = false;
            this.gbLocation.Text = "Location:    ";
            // 
            // pnLocation
            // 
            this.pnLocation.Controls.Add(this.tbLocationEditorID);
            this.pnLocation.Controls.Add(this.tbLocationFormID);
            this.pnLocation.Controls.Add(this.lblLocationFormID);
            this.pnLocation.Controls.Add(this.lblLocationEditorID);
            this.pnLocation.Location = new System.Drawing.Point(6, 27);
            this.pnLocation.Name = "pnLocation";
            this.pnLocation.Size = new System.Drawing.Size(286, 51);
            this.pnLocation.TabIndex = 8;
            // 
            // tbLocationEditorID
            // 
            this.tbLocationEditorID.Location = new System.Drawing.Point(65, 22);
            this.tbLocationEditorID.Name = "tbLocationEditorID";
            this.tbLocationEditorID.Size = new System.Drawing.Size(214, 20);
            this.tbLocationEditorID.TabIndex = 7;
            // 
            // tbLocationFormID
            // 
            this.tbLocationFormID.BackColor = System.Drawing.SystemColors.Control;
            this.tbLocationFormID.Enabled = false;
            this.tbLocationFormID.Location = new System.Drawing.Point(65, 0);
            this.tbLocationFormID.Name = "tbLocationFormID";
            this.tbLocationFormID.Size = new System.Drawing.Size(214, 20);
            this.tbLocationFormID.TabIndex = 5;
            // 
            // lblLocationFormID
            // 
            this.lblLocationFormID.Location = new System.Drawing.Point(0, 0);
            this.lblLocationFormID.Name = "lblLocationFormID";
            this.lblLocationFormID.Size = new System.Drawing.Size(100, 23);
            this.lblLocationFormID.TabIndex = 4;
            this.lblLocationFormID.Text = "FormID:";
            // 
            // lblLocationEditorID
            // 
            this.lblLocationEditorID.Location = new System.Drawing.Point(0, 25);
            this.lblLocationEditorID.Name = "lblLocationEditorID";
            this.lblLocationEditorID.Size = new System.Drawing.Size(100, 23);
            this.lblLocationEditorID.TabIndex = 6;
            this.lblLocationEditorID.Text = "EditorID:";
            // 
            // cbLocation
            // 
            this.cbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLocation.FormattingEnabled = true;
            this.cbLocation.Location = new System.Drawing.Point(71, 0);
            this.cbLocation.Name = "cbLocation";
            this.cbLocation.Size = new System.Drawing.Size(214, 21);
            this.cbLocation.TabIndex = 3;
            // 
            // SubDivision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 192);
            this.Controls.Add(this.gbLocation);
            this.Name = "SubDivision";
            this.Text = "Sub-Division";
            this.Load += new System.EventHandler(this.SubDivisionLoad);
            this.Controls.SetChildIndex(this.gbLocation, 0);
            this.gbLocation.ResumeLayout(false);
            this.pnLocation.ResumeLayout(false);
            this.pnLocation.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
