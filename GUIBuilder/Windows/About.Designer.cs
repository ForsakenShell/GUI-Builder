/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class About
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.Label lblTitle;
        System.Windows.Forms.Label lblCredit;
        System.Windows.Forms.Label lblVersion;
        System.Windows.Forms.LinkLabel linkLicense;
        System.Windows.Forms.Label lblLicense;
        System.Windows.Forms.TextBox tbLicense;
        System.Windows.Forms.LinkLabel linkAuthor;
        
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkLicense = new System.Windows.Forms.LinkLabel();
            this.lblLicense = new System.Windows.Forms.Label();
            this.tbLicense = new System.Windows.Forms.TextBox();
            this.linkAuthor = new System.Windows.Forms.LinkLabel();
            this.WindowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.lblTitle);
            this.WindowPanel.Controls.Add(this.lblCredit);
            this.WindowPanel.Controls.Add(this.linkAuthor);
            this.WindowPanel.Controls.Add(this.lblVersion);
            this.WindowPanel.Controls.Add(this.tbLicense);
            this.WindowPanel.Controls.Add(this.linkLicense);
            this.WindowPanel.Controls.Add(this.lblLicense);
            this.WindowPanel.Size = new System.Drawing.Size(495, 352);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Comic Sans MS", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(495, 68);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "AboutWindow.ComicTitle";
            this.lblTitle.Text = "Comic";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCredit
            // 
            this.lblCredit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCredit.Location = new System.Drawing.Point(0, 91);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(495, 23);
            this.lblCredit.TabIndex = 1;
            this.lblCredit.Tag = "AboutWindow.Author";
            this.lblCredit.Text = "by 1000101";
            this.lblCredit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Location = new System.Drawing.Point(0, 68);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(495, 23);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "vM.m.r.b";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLicense
            // 
            this.linkLicense.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLicense.Location = new System.Drawing.Point(0, 160);
            this.linkLicense.Name = "linkLicense";
            this.linkLicense.Size = new System.Drawing.Size(495, 23);
            this.linkLicense.TabIndex = 4;
            this.linkLicense.TabStop = true;
            this.linkLicense.Text = "http://unlicense.org/";
            this.linkLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLicenseLinkClicked);
            // 
            // lblLicense
            // 
            this.lblLicense.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLicense.Location = new System.Drawing.Point(0, 137);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(495, 23);
            this.lblLicense.TabIndex = 5;
            this.lblLicense.Tag = "AboutWindow.ReleaseLicenseTitle";
            this.lblLicense.Text = "Release license title";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLicense
            // 
            this.tbLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLicense.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbLicense.Location = new System.Drawing.Point(0, 186);
            this.tbLicense.Multiline = true;
            this.tbLicense.Name = "tbLicense";
            this.tbLicense.ReadOnly = true;
            this.tbLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLicense.Size = new System.Drawing.Size(495, 166);
            this.tbLicense.TabIndex = 6;
            this.tbLicense.Tag = "AboutWindow.ReleaseLicenseBody";
            this.tbLicense.Text = "Release license body";
            // 
            // linkAuthor
            // 
            this.linkAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkAuthor.Location = new System.Drawing.Point(0, 114);
            this.linkAuthor.Name = "linkAuthor";
            this.linkAuthor.Size = new System.Drawing.Size(495, 23);
            this.linkAuthor.TabIndex = 7;
            this.linkAuthor.TabStop = true;
            this.linkAuthor.Text = "https://www.nexusmods.com/users/106891";
            this.linkAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAuthorLinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 352);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(503, 560);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(503, 376);
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.Tag = "AboutWindow.Title";
            this.Text = "title";
            this.ClientOnLoad += new System.EventHandler(this.About_OnLoad);
            this.WindowPanel.ResumeLayout(false);
            this.WindowPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
