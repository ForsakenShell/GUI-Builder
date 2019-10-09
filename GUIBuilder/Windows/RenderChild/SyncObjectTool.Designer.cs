/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
//#define DESIGNER_MODE
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows.RenderChild
{
    
    /// <summary>
    /// Description of SyncObjectTool.
    /// </summary>
    public partial class SyncObjectTool<TSync> : Form, GodObject.XmlConfig.IXmlConfiguration
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        
        System.Windows.Forms.Panel pnWindow;
        
        #if DESIGNER_MODE
        GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Form> lvSyncObjects;
        #else
        GUIBuilder.Windows.Controls.SyncedListView<TSync> lvSyncObjects;
        #endif
        
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
            #if DESIGNER_MODE
            this.lvSyncObjects = new GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Form>();
            #else
            this.lvSyncObjects = new GUIBuilder.Windows.Controls.SyncedListView<TSync>();
            #endif
            this.pnWindow = new System.Windows.Forms.Panel();
            this.pnWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSyncObjects
            // 
            this.lvSyncObjects.AllowHidingItems = true;
            this.lvSyncObjects.AllowOverrideColumnSorting = true;
            this.lvSyncObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSyncObjects.CheckBoxes = true;
            this.lvSyncObjects.EditorIDColumn = true;
            this.lvSyncObjects.ExtraInfoColumn = false;
            this.lvSyncObjects.FilenameColumn = false;
            this.lvSyncObjects.FormIDColumn = true;
            this.lvSyncObjects.LoadOrderColumn = false;
            this.lvSyncObjects.Location = new System.Drawing.Point(0, 0);
            this.lvSyncObjects.MultiSelect = true;
            this.lvSyncObjects.Name = "lvSyncObjects";
            this.lvSyncObjects.Size = new System.Drawing.Size(357, 292);
            #if DESIGNER_MODE
            this.lvSyncObjects.SortByColumn = GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Form>.SortByColumns.EditorID;
            this.lvSyncObjects.SortDirection = GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Form>.SortDirections.Ascending;
            #else
            this.lvSyncObjects.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvSyncObjects.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            #endif
            this.lvSyncObjects.SyncedEditorFormType = null;
            this.lvSyncObjects.SyncObjects = null;
            this.lvSyncObjects.TabIndex = 16;
            this.lvSyncObjects.TypeColumn = false;
            // 
            // pnMain
            // 
            this.pnWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnWindow.Controls.Add(this.lvSyncObjects);
            this.pnWindow.Location = new System.Drawing.Point(0, 0);
            this.pnWindow.Name = "pnMain";
            this.pnWindow.Size = new System.Drawing.Size(357, 292);
            this.pnWindow.TabIndex = 17;
            // 
            // RenderWindowSyncObjectToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 291);
            this.Controls.Add(this.pnWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(150, 27);
            this.Name = "RenderWindowSyncObjectToolWindow";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.OnActivated);
            this.Deactivate += new System.EventHandler(this.OnDeactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.pnWindow.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
