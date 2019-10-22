/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows.Controls
{
    partial class SyncedListView<TSync>
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        protected System.Windows.Forms.ListView lvSyncObjects;
        System.Windows.Forms.ContextMenuStrip cmsSyncObjects;
        System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        System.Windows.Forms.ToolStripMenuItem tsmiSelectNone;
        System.Windows.Forms.ToolStripMenuItem tsmiOnlyChangedOrNew;
        System.Windows.Forms.ToolStripSeparator tsmiDividerHideUnchanged;
        System.Windows.Forms.ToolStripMenuItem tsmiHideUnchanged;
        System.Windows.Forms.ToolStripMenuItem tsmiEditObject;
        System.Windows.Forms.ToolStripSeparator tsmiDividerEditObject;
        
        /// <summary>
        /// Disposes resources used by the control.
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
            this.components = new System.ComponentModel.Container();
            this.lvSyncObjects = new System.Windows.Forms.ListView();
            this.cmsSyncObjects = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiEditObject = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerEditObject = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOnlyChangedOrNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerHideUnchanged = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideUnchanged = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSyncObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSyncObjects
            // 
            this.lvSyncObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSyncObjects.ContextMenuStrip = this.cmsSyncObjects;
            this.lvSyncObjects.FullRowSelect = true;
            this.lvSyncObjects.GridLines = true;
            this.lvSyncObjects.HideSelection = false;
            this.lvSyncObjects.LabelWrap = false;
            this.lvSyncObjects.Location = new System.Drawing.Point(0, 0);
            this.lvSyncObjects.MultiSelect = false;
            this.lvSyncObjects.Name = "lvSyncObjects";
            this.lvSyncObjects.Size = new System.Drawing.Size(200, 100);
            this.lvSyncObjects.TabIndex = 16;
            this.lvSyncObjects.UseCompatibleStateImageBehavior = false;
            this.lvSyncObjects.View = System.Windows.Forms.View.Details;
            this.lvSyncObjects.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvSyncObjectsItemChecked);
            this.lvSyncObjects.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvSyncObjectsItemSelectionChanged);
            // 
            // cmsSyncObjects
            // 
            this.cmsSyncObjects.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditObject,
            this.tsmiDividerEditObject,
            this.tsmiSelectAll,
            this.tsmiSelectNone,
            this.tsmiOnlyChangedOrNew,
            this.tsmiDividerHideUnchanged,
            this.tsmiHideUnchanged});
            this.cmsSyncObjects.Name = "cmsImportForms";
            this.cmsSyncObjects.Size = new System.Drawing.Size(180, 126);
            this.cmsSyncObjects.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.cmsSyncObjectsClosed);
            // 
            // tsmiEditObject
            // 
            this.tsmiEditObject.Name = "tsmiEditObject";
            this.tsmiEditObject.Size = new System.Drawing.Size(179, 22);
            this.tsmiEditObject.Tag = "SyncedListView.Edit";
            this.tsmiEditObject.Text = "Edit";
            this.tsmiEditObject.Visible = false;
            this.tsmiEditObject.Click += new System.EventHandler(this.tsmiEditObjectClick);
            // 
            // tsmiDividerEditObject
            // 
            this.tsmiDividerEditObject.Name = "tsmiDividerEditObject";
            this.tsmiDividerEditObject.Size = new System.Drawing.Size(176, 6);
            this.tsmiDividerEditObject.Visible = false;
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(179, 22);
            this.tsmiSelectAll.Tag = "SyncedListView.SelectAll";
            this.tsmiSelectAll.Text = "Select All";
            this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAllClick);
            // 
            // tsmiSelectNone
            // 
            this.tsmiSelectNone.Name = "tsmiSelectNone";
            this.tsmiSelectNone.Size = new System.Drawing.Size(179, 22);
            this.tsmiSelectNone.Tag = "SyncedListView.SelectNone";
            this.tsmiSelectNone.Text = "Select None";
            this.tsmiSelectNone.Click += new System.EventHandler(this.tsmiSelectNoneClick);
            // 
            // tsmiOnlyChangedOrNew
            // 
            this.tsmiOnlyChangedOrNew.Name = "tsmiOnlyChangedOrNew";
            this.tsmiOnlyChangedOrNew.Size = new System.Drawing.Size(179, 22);
            this.tsmiOnlyChangedOrNew.Tag = "SyncedListView.OnlyChangedOrNew";
            this.tsmiOnlyChangedOrNew.Text = "Only Changed or New";
            this.tsmiOnlyChangedOrNew.Click += new System.EventHandler(this.tsmiOnlyChangedOrNewClick);
            // 
            // tsmiDividerHideUnchanged
            // 
            this.tsmiDividerHideUnchanged.Name = "tsmiDividerHideUnchanged";
            this.tsmiDividerHideUnchanged.Size = new System.Drawing.Size(176, 6);
            // 
            // tsmiHideUnchanged
            // 
            this.tsmiHideUnchanged.Name = "tsmiHideUnchanged";
            this.tsmiHideUnchanged.Size = new System.Drawing.Size(179, 22);
            this.tsmiHideUnchanged.Tag = "SyncedListView.HideUnchanged";
            this.tsmiHideUnchanged.Text = "Hide Unchanged";
            this.tsmiHideUnchanged.Click += new System.EventHandler(this.tsmiHideUnchangedClick);
            // 
            // SyncedListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvSyncObjects);
            this.Name = "SyncedListView";
            this.Size = new System.Drawing.Size(200, 100);
            this.Load += new System.EventHandler(this.SyncedListViewLoad);
            this.cmsSyncObjects.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
