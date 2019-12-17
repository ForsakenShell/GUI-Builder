/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class PluginSelector
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        System.Windows.Forms.TreeView tvPlugins;
        System.Windows.Forms.CheckBox cbOpenRenderWindowOnLoad;
        System.Windows.Forms.Button btnLoad;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.ComboBox cbWorkingFile;
        System.Windows.Forms.GroupBox gbWorkingFile;
        
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
            this.tvPlugins = new System.Windows.Forms.TreeView();
            this.cbOpenRenderWindowOnLoad = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbWorkingFile = new System.Windows.Forms.ComboBox();
            this.gbWorkingFile = new System.Windows.Forms.GroupBox();
            this.gbWorkingFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvPlugins
            // 
            this.tvPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvPlugins.CheckBoxes = true;
            this.tvPlugins.Location = new System.Drawing.Point(0, 0);
            this.tvPlugins.Name = "tvPlugins";
            this.tvPlugins.Size = new System.Drawing.Size(272, 317);
            this.tvPlugins.TabIndex = 1;
            this.tvPlugins.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvPluginsBeforeCheckOrSelect);
            this.tvPlugins.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvPluginsAfterCheck);
            this.tvPlugins.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvPluginsBeforeCheckOrSelect);
            // 
            // cbOpenRenderWindowOnLoad
            // 
            this.cbOpenRenderWindowOnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOpenRenderWindowOnLoad.Location = new System.Drawing.Point(0, 369);
            this.cbOpenRenderWindowOnLoad.Name = "cbOpenRenderWindowOnLoad";
            this.cbOpenRenderWindowOnLoad.Size = new System.Drawing.Size(272, 18);
            this.cbOpenRenderWindowOnLoad.TabIndex = 2;
            this.cbOpenRenderWindowOnLoad.Tag = "PluginSelectorWindow.OpenRenderWindow";
            this.cbOpenRenderWindowOnLoad.Text = "Open Render Window On Load";
            this.cbOpenRenderWindowOnLoad.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(116, 391);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Tag = "SelectorWindow.Load";
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoadClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 391);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Tag = "SelectorWindow.Cancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancelClick);
            // 
            // cbWorkingFile
            // 
            this.cbWorkingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWorkingFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkingFile.FormattingEnabled = true;
            this.cbWorkingFile.Location = new System.Drawing.Point(6, 19);
            this.cbWorkingFile.Name = "cbWorkingFile";
            this.cbWorkingFile.Size = new System.Drawing.Size(259, 21);
            this.cbWorkingFile.TabIndex = 5;
            this.cbWorkingFile.SelectedIndexChanged += new System.EventHandler(this.CbWorkingFileSelectedIndexChanged);
            // 
            // gbWorkingFile
            // 
            this.gbWorkingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkingFile.Controls.Add(this.cbWorkingFile);
            this.gbWorkingFile.Location = new System.Drawing.Point(0, 321);
            this.gbWorkingFile.Name = "gbWorkingFile";
            this.gbWorkingFile.Size = new System.Drawing.Size(272, 45);
            this.gbWorkingFile.TabIndex = 6;
            this.gbWorkingFile.TabStop = false;
            this.gbWorkingFile.Tag = "PluginSelectorWindow.WorkingFile";
            this.gbWorkingFile.Text = "Working File";
            // 
            // PluginSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 414);
            this.Controls.Add(this.gbWorkingFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cbOpenRenderWindowOnLoad);
            this.Controls.Add(this.tvPlugins);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(165, 200);
            this.Name = "PluginSelector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Tag = "PluginSelectorWindow.Title";
            this.Text = "title";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PluginSelectorLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.gbWorkingFile.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
