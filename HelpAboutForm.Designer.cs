/*
 * ${FILE}
 *
 * Insert description here.
 *
 * User: 1000101
 * Date: 02/12/2017
 * Time: 10:58 AM
 * 
 */
namespace Border_Builder
{
    partial class HelpAboutForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCredit;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel llblLicense;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.TextBox tbLicense;
        private System.Windows.Forms.LinkLabel llblBorderBuilder;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpAboutForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.llblLicense = new System.Windows.Forms.LinkLabel();
            this.lblLicense = new System.Windows.Forms.Label();
            this.tbLicense = new System.Windows.Forms.TextBox();
            this.llblBorderBuilder = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Comic Sans MS", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(485, 68);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Border Builder";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCredit
            // 
            this.lblCredit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCredit.Location = new System.Drawing.Point(0, 91);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(485, 23);
            this.lblCredit.TabIndex = 1;
            this.lblCredit.Text = "by 1000101";
            this.lblCredit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Location = new System.Drawing.Point(0, 68);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(485, 23);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "vM.m.r.b";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // llblLicense
            // 
            this.llblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.llblLicense.Location = new System.Drawing.Point(0, 160);
            this.llblLicense.Name = "llblLicense";
            this.llblLicense.Size = new System.Drawing.Size(485, 23);
            this.llblLicense.TabIndex = 4;
            this.llblLicense.TabStop = true;
            this.llblLicense.Text = "http://unlicense.org/";
            this.llblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llblLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlblLicenseLinkClicked);
            // 
            // lblLicense
            // 
            this.lblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicense.Location = new System.Drawing.Point(0, 137);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(485, 23);
            this.lblLicense.TabIndex = 5;
            this.lblLicense.Text = "Released under the Unlicense";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLicense
            // 
            this.tbLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLicense.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbLicense.Location = new System.Drawing.Point(6, 186);
            this.tbLicense.Multiline = true;
            this.tbLicense.Name = "tbLicense";
            this.tbLicense.ReadOnly = true;
            this.tbLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLicense.Size = new System.Drawing.Size(475, 146);
            this.tbLicense.TabIndex = 6;
            this.tbLicense.Text = resources.GetString("tbLicense.Text");
            // 
            // llblBorderBuilder
            // 
            this.llblBorderBuilder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.llblBorderBuilder.Location = new System.Drawing.Point(0, 114);
            this.llblBorderBuilder.Name = "llblBorderBuilder";
            this.llblBorderBuilder.Size = new System.Drawing.Size(485, 23);
            this.llblBorderBuilder.TabIndex = 7;
            this.llblBorderBuilder.TabStop = true;
            this.llblBorderBuilder.Text = "https://www.nexusmods.com/users/106891";
            this.llblBorderBuilder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llblBorderBuilder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlblBorderBuilderLinkClicked);
            // 
            // HelpAboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 338);
            this.Controls.Add(this.llblBorderBuilder);
            this.Controls.Add(this.tbLicense);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.llblLicense);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblCredit);
            this.Controls.Add(this.lblTitle);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpAboutForm";
            this.ShowInTaskbar = false;
            this.Text = "About Border Builder";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HelpAboutFormFormClosed);
            this.Load += new System.EventHandler(this.HelpAboutFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
