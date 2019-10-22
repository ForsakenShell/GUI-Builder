/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows.RenderChild
{
    
    public partial class WorldspaceTool : Form, GodObject.XmlConfig.IXmlConfiguration
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Worldspace> lvWorldspaces;
        System.Windows.Forms.GroupBox gbWorldspace;
        System.Windows.Forms.TextBox tbWorldspaceEditorID;
        System.Windows.Forms.TextBox tbWorldspaceFormID;
        System.Windows.Forms.Label lWorldspaceFormID;
        System.Windows.Forms.GroupBox gbWorldspaceMapHeightRange;
        System.Windows.Forms.Label lWorldspaceMapHeightMax;
        System.Windows.Forms.Label lWorldspaceMapHeightMin;
        System.Windows.Forms.TextBox tbWorldspaceMapHeightMax;
        System.Windows.Forms.TextBox tbWorldspaceMapHeightMin;
        System.Windows.Forms.GroupBox gbWorldspaceTextures;
        System.Windows.Forms.TextBox tbWorldspaceWaterHeightsTexture;
        System.Windows.Forms.TextBox tbWorldspaceHeightmapTexture;
        System.Windows.Forms.Label lWorldspaceEditorID;
        System.Windows.Forms.GroupBox gbWorldspaceGridRange;
        System.Windows.Forms.TextBox tbWorldspaceGridBottomX;
        System.Windows.Forms.TextBox tbWorldspaceGridBottomY;
        System.Windows.Forms.Label lWorldspaceGridBRComma;
        System.Windows.Forms.TextBox tbWorldspaceGridTopY;
        System.Windows.Forms.TextBox tbWorldspaceGridTopX;
        System.Windows.Forms.Label lWorldspaceGridTLComma;
        System.Windows.Forms.GroupBox gbWorldspaceGridRangeIndicator;
        System.Windows.Forms.Panel pnWindow;
        
        
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
            this.lvWorldspaces = new GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Worldspace>();
            this.gbWorldspace = new System.Windows.Forms.GroupBox();
            this.tbWorldspaceEditorID = new System.Windows.Forms.TextBox();
            this.tbWorldspaceFormID = new System.Windows.Forms.TextBox();
            this.lWorldspaceFormID = new System.Windows.Forms.Label();
            this.gbWorldspaceMapHeightRange = new System.Windows.Forms.GroupBox();
            this.lWorldspaceMapHeightMax = new System.Windows.Forms.Label();
            this.lWorldspaceMapHeightMin = new System.Windows.Forms.Label();
            this.tbWorldspaceMapHeightMax = new System.Windows.Forms.TextBox();
            this.tbWorldspaceMapHeightMin = new System.Windows.Forms.TextBox();
            this.gbWorldspaceTextures = new System.Windows.Forms.GroupBox();
            this.tbWorldspaceWaterHeightsTexture = new System.Windows.Forms.TextBox();
            this.tbWorldspaceHeightmapTexture = new System.Windows.Forms.TextBox();
            this.lWorldspaceEditorID = new System.Windows.Forms.Label();
            this.gbWorldspaceGridRange = new System.Windows.Forms.GroupBox();
            this.tbWorldspaceGridBottomX = new System.Windows.Forms.TextBox();
            this.tbWorldspaceGridBottomY = new System.Windows.Forms.TextBox();
            this.lWorldspaceGridBRComma = new System.Windows.Forms.Label();
            this.tbWorldspaceGridTopY = new System.Windows.Forms.TextBox();
            this.tbWorldspaceGridTopX = new System.Windows.Forms.TextBox();
            this.lWorldspaceGridTLComma = new System.Windows.Forms.Label();
            this.gbWorldspaceGridRangeIndicator = new System.Windows.Forms.GroupBox();
            this.pnWindow = new System.Windows.Forms.Panel();
            this.gbWorldspace.SuspendLayout();
            this.gbWorldspaceMapHeightRange.SuspendLayout();
            this.gbWorldspaceTextures.SuspendLayout();
            this.gbWorldspaceGridRange.SuspendLayout();
            this.pnWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvWorldspaces
            // 
            this.lvWorldspaces.AllowHidingItems = true;
            this.lvWorldspaces.AllowOverrideColumnSorting = true;
            this.lvWorldspaces.CheckBoxes = false;
            this.lvWorldspaces.EditorIDColumn = true;
            this.lvWorldspaces.ExtraInfoColumn = false;
            this.lvWorldspaces.FilenameColumn = false;
            this.lvWorldspaces.FormIDColumn = true;
            this.lvWorldspaces.LoadOrderColumn = false;
            this.lvWorldspaces.Location = new System.Drawing.Point(0, 0);
            this.lvWorldspaces.MultiSelect = false;
            this.lvWorldspaces.Name = "lvWorldspaces";
            this.lvWorldspaces.Size = new System.Drawing.Size(359, 219);
            this.lvWorldspaces.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.EditorID;
            this.lvWorldspaces.SyncedEditorFormType = null;
            this.lvWorldspaces.SyncObjects = null;
            this.lvWorldspaces.TabIndex = 11;
            this.lvWorldspaces.TypeColumn = false;
            this.lvWorldspaces.ItemSelectionChanged += new System.EventHandler(this.lvWorldspacesItemSelectionChanged);
            // 
            // gbWorldspace
            // 
            this.gbWorldspace.Controls.Add(this.tbWorldspaceEditorID);
            this.gbWorldspace.Controls.Add(this.tbWorldspaceFormID);
            this.gbWorldspace.Controls.Add(this.lWorldspaceFormID);
            this.gbWorldspace.Controls.Add(this.gbWorldspaceMapHeightRange);
            this.gbWorldspace.Controls.Add(this.gbWorldspaceTextures);
            this.gbWorldspace.Controls.Add(this.lWorldspaceEditorID);
            this.gbWorldspace.Controls.Add(this.gbWorldspaceGridRange);
            this.gbWorldspace.Location = new System.Drawing.Point(365, 0);
            this.gbWorldspace.Name = "gbWorldspace";
            this.gbWorldspace.Size = new System.Drawing.Size(287, 220);
            this.gbWorldspace.TabIndex = 17;
            this.gbWorldspace.TabStop = false;
            // 
            // tbWorldspaceEditorID
            // 
            this.tbWorldspaceEditorID.Enabled = false;
            this.tbWorldspaceEditorID.Location = new System.Drawing.Point(97, 36);
            this.tbWorldspaceEditorID.Name = "tbWorldspaceEditorID";
            this.tbWorldspaceEditorID.ReadOnly = true;
            this.tbWorldspaceEditorID.Size = new System.Drawing.Size(180, 20);
            this.tbWorldspaceEditorID.TabIndex = 3;
            this.tbWorldspaceEditorID.Text = "Commonwealth";
            // 
            // tbWorldspaceFormID
            // 
            this.tbWorldspaceFormID.Enabled = false;
            this.tbWorldspaceFormID.Location = new System.Drawing.Point(97, 13);
            this.tbWorldspaceFormID.Name = "tbWorldspaceFormID";
            this.tbWorldspaceFormID.ReadOnly = true;
            this.tbWorldspaceFormID.Size = new System.Drawing.Size(180, 20);
            this.tbWorldspaceFormID.TabIndex = 7;
            this.tbWorldspaceFormID.Text = "0000003C";
            // 
            // lWorldspaceFormID
            // 
            this.lWorldspaceFormID.Location = new System.Drawing.Point(6, 16);
            this.lWorldspaceFormID.Name = "lWorldspaceFormID";
            this.lWorldspaceFormID.Size = new System.Drawing.Size(99, 23);
            this.lWorldspaceFormID.TabIndex = 6;
            this.lWorldspaceFormID.Text = "FormID:";
            this.lWorldspaceFormID.Tag = "RenderWindow.Worldspace.FormID";
            // 
            // gbWorldspaceMapHeightRange
            // 
            this.gbWorldspaceMapHeightRange.Controls.Add(this.lWorldspaceMapHeightMax);
            this.gbWorldspaceMapHeightRange.Controls.Add(this.lWorldspaceMapHeightMin);
            this.gbWorldspaceMapHeightRange.Controls.Add(this.tbWorldspaceMapHeightMax);
            this.gbWorldspaceMapHeightRange.Controls.Add(this.tbWorldspaceMapHeightMin);
            this.gbWorldspaceMapHeightRange.Location = new System.Drawing.Point(129, 65);
            this.gbWorldspaceMapHeightRange.Name = "gbWorldspaceMapHeightRange";
            this.gbWorldspaceMapHeightRange.Size = new System.Drawing.Size(148, 73);
            this.gbWorldspaceMapHeightRange.TabIndex = 5;
            this.gbWorldspaceMapHeightRange.TabStop = false;
            this.gbWorldspaceMapHeightRange.Text = "Map Height";
            this.gbWorldspaceMapHeightRange.Tag = "RenderWindow.Worldspace.MapHeight";
            // 
            // lWorldspaceMapHeightMax
            // 
            this.lWorldspaceMapHeightMax.Location = new System.Drawing.Point(9, 48);
            this.lWorldspaceMapHeightMax.Name = "lWorldspaceMapHeightMax";
            this.lWorldspaceMapHeightMax.Size = new System.Drawing.Size(36, 23);
            this.lWorldspaceMapHeightMax.TabIndex = 3;
            this.lWorldspaceMapHeightMax.Text = "Max:";
            this.lWorldspaceMapHeightMax.Tag = "RenderWindow.Worldspace.MapHeight.Max";
            // 
            // lWorldspaceMapHeightMin
            // 
            this.lWorldspaceMapHeightMin.Location = new System.Drawing.Point(9, 22);
            this.lWorldspaceMapHeightMin.Name = "lWorldspaceMapHeightMin";
            this.lWorldspaceMapHeightMin.Size = new System.Drawing.Size(36, 23);
            this.lWorldspaceMapHeightMin.TabIndex = 2;
            this.lWorldspaceMapHeightMin.Text = "Min:";
            this.lWorldspaceMapHeightMin.Tag = "RenderWindow.Worldspace.MapHeight.Min";
            // 
            // tbWorldspaceMapHeightMax
            // 
            this.tbWorldspaceMapHeightMax.Enabled = false;
            this.tbWorldspaceMapHeightMax.Location = new System.Drawing.Point(51, 45);
            this.tbWorldspaceMapHeightMax.Name = "tbWorldspaceMapHeightMax";
            this.tbWorldspaceMapHeightMax.ReadOnly = true;
            this.tbWorldspaceMapHeightMax.Size = new System.Drawing.Size(88, 20);
            this.tbWorldspaceMapHeightMax.TabIndex = 1;
            this.tbWorldspaceMapHeightMax.Text = "44872.000000";
            this.tbWorldspaceMapHeightMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbWorldspaceMapHeightMin
            // 
            this.tbWorldspaceMapHeightMin.Enabled = false;
            this.tbWorldspaceMapHeightMin.Location = new System.Drawing.Point(51, 19);
            this.tbWorldspaceMapHeightMin.Name = "tbWorldspaceMapHeightMin";
            this.tbWorldspaceMapHeightMin.ReadOnly = true;
            this.tbWorldspaceMapHeightMin.Size = new System.Drawing.Size(88, 20);
            this.tbWorldspaceMapHeightMin.TabIndex = 0;
            this.tbWorldspaceMapHeightMin.Text = "-8320.000000";
            this.tbWorldspaceMapHeightMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // gbWorldspaceTextures
            // 
            this.gbWorldspaceTextures.Controls.Add(this.tbWorldspaceWaterHeightsTexture);
            this.gbWorldspaceTextures.Controls.Add(this.tbWorldspaceHeightmapTexture);
            this.gbWorldspaceTextures.Location = new System.Drawing.Point(6, 144);
            this.gbWorldspaceTextures.Name = "gbWorldspaceTextures";
            this.gbWorldspaceTextures.Size = new System.Drawing.Size(271, 73);
            this.gbWorldspaceTextures.TabIndex = 4;
            this.gbWorldspaceTextures.TabStop = false;
            this.gbWorldspaceTextures.Text = "Textures";
            this.gbWorldspaceTextures.Tag = "RenderWindow.Worldspace.Textures";
            // 
            // tbWorldspaceWaterHeightsTexture
            // 
            this.tbWorldspaceWaterHeightsTexture.Enabled = false;
            this.tbWorldspaceWaterHeightsTexture.Location = new System.Drawing.Point(6, 45);
            this.tbWorldspaceWaterHeightsTexture.Name = "tbWorldspaceWaterHeightsTexture";
            this.tbWorldspaceWaterHeightsTexture.ReadOnly = true;
            this.tbWorldspaceWaterHeightsTexture.Size = new System.Drawing.Size(256, 20);
            this.tbWorldspaceWaterHeightsTexture.TabIndex = 1;
            this.tbWorldspaceWaterHeightsTexture.Text = "Commonwealth_WaterHeights.dds";
            // 
            // tbWorldspaceHeightmapTexture
            // 
            this.tbWorldspaceHeightmapTexture.Enabled = false;
            this.tbWorldspaceHeightmapTexture.Location = new System.Drawing.Point(6, 19);
            this.tbWorldspaceHeightmapTexture.Name = "tbWorldspaceHeightmapTexture";
            this.tbWorldspaceHeightmapTexture.ReadOnly = true;
            this.tbWorldspaceHeightmapTexture.Size = new System.Drawing.Size(256, 20);
            this.tbWorldspaceHeightmapTexture.TabIndex = 0;
            this.tbWorldspaceHeightmapTexture.Text = "Commonwealth_LandHeights.dds";
            // 
            // lWorldspaceEditorID
            // 
            this.lWorldspaceEditorID.Location = new System.Drawing.Point(6, 39);
            this.lWorldspaceEditorID.Name = "lWorldspaceEditorID";
            this.lWorldspaceEditorID.Size = new System.Drawing.Size(99, 23);
            this.lWorldspaceEditorID.TabIndex = 2;
            this.lWorldspaceEditorID.Text = "EditorID:";
            this.lWorldspaceEditorID.Tag = "RenderWindow.Worldspace.EditorID";
            // 
            // gbWorldspaceGridRange
            // 
            this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridBottomX);
            this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridBottomY);
            this.gbWorldspaceGridRange.Controls.Add(this.lWorldspaceGridBRComma);
            this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridTopY);
            this.gbWorldspaceGridRange.Controls.Add(this.tbWorldspaceGridTopX);
            this.gbWorldspaceGridRange.Controls.Add(this.lWorldspaceGridTLComma);
            this.gbWorldspaceGridRange.Controls.Add(this.gbWorldspaceGridRangeIndicator);
            this.gbWorldspaceGridRange.Location = new System.Drawing.Point(6, 65);
            this.gbWorldspaceGridRange.Name = "gbWorldspaceGridRange";
            this.gbWorldspaceGridRange.Size = new System.Drawing.Size(117, 73);
            this.gbWorldspaceGridRange.TabIndex = 0;
            this.gbWorldspaceGridRange.TabStop = false;
            this.gbWorldspaceGridRange.Text = "Grid Range";
            this.gbWorldspaceGridRange.Tag = "RenderWindow.Worldspace.GridRange";
            // 
            // tbWorldspaceGridBottomX
            // 
            this.tbWorldspaceGridBottomX.Enabled = false;
            this.tbWorldspaceGridBottomX.Location = new System.Drawing.Point(42, 45);
            this.tbWorldspaceGridBottomX.Name = "tbWorldspaceGridBottomX";
            this.tbWorldspaceGridBottomX.ReadOnly = true;
            this.tbWorldspaceGridBottomX.Size = new System.Drawing.Size(30, 20);
            this.tbWorldspaceGridBottomX.TabIndex = 6;
            this.tbWorldspaceGridBottomX.Text = "38";
            this.tbWorldspaceGridBottomX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorldspaceGridBottomY
            // 
            this.tbWorldspaceGridBottomY.Enabled = false;
            this.tbWorldspaceGridBottomY.Location = new System.Drawing.Point(79, 45);
            this.tbWorldspaceGridBottomY.Name = "tbWorldspaceGridBottomY";
            this.tbWorldspaceGridBottomY.ReadOnly = true;
            this.tbWorldspaceGridBottomY.Size = new System.Drawing.Size(30, 20);
            this.tbWorldspaceGridBottomY.TabIndex = 4;
            this.tbWorldspaceGridBottomY.Text = "-46";
            this.tbWorldspaceGridBottomY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lWorldspaceGridBRComma
            // 
            this.lWorldspaceGridBRComma.Location = new System.Drawing.Point(71, 48);
            this.lWorldspaceGridBRComma.Name = "lWorldspaceGridBRComma";
            this.lWorldspaceGridBRComma.Size = new System.Drawing.Size(10, 23);
            this.lWorldspaceGridBRComma.TabIndex = 5;
            this.lWorldspaceGridBRComma.Text = ",";
            // 
            // tbWorldspaceGridTopY
            // 
            this.tbWorldspaceGridTopY.Enabled = false;
            this.tbWorldspaceGridTopY.Location = new System.Drawing.Point(43, 19);
            this.tbWorldspaceGridTopY.Name = "tbWorldspaceGridTopY";
            this.tbWorldspaceGridTopY.ReadOnly = true;
            this.tbWorldspaceGridTopY.Size = new System.Drawing.Size(30, 20);
            this.tbWorldspaceGridTopY.TabIndex = 1;
            this.tbWorldspaceGridTopY.Text = "36";
            this.tbWorldspaceGridTopY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbWorldspaceGridTopX
            // 
            this.tbWorldspaceGridTopX.Enabled = false;
            this.tbWorldspaceGridTopX.Location = new System.Drawing.Point(6, 19);
            this.tbWorldspaceGridTopX.Name = "tbWorldspaceGridTopX";
            this.tbWorldspaceGridTopX.ReadOnly = true;
            this.tbWorldspaceGridTopX.Size = new System.Drawing.Size(30, 20);
            this.tbWorldspaceGridTopX.TabIndex = 0;
            this.tbWorldspaceGridTopX.Text = "-42";
            this.tbWorldspaceGridTopX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lWorldspaceGridTLComma
            // 
            this.lWorldspaceGridTLComma.Location = new System.Drawing.Point(35, 22);
            this.lWorldspaceGridTLComma.Name = "lWorldspaceGridTLComma";
            this.lWorldspaceGridTLComma.Size = new System.Drawing.Size(10, 23);
            this.lWorldspaceGridTLComma.TabIndex = 2;
            this.lWorldspaceGridTLComma.Text = ",";
            // 
            // gbWorldspaceGridRangeIndicator
            // 
            this.gbWorldspaceGridRangeIndicator.Location = new System.Drawing.Point(18, 22);
            this.gbWorldspaceGridRangeIndicator.Name = "gbWorldspaceGridRangeIndicator";
            this.gbWorldspaceGridRangeIndicator.Size = new System.Drawing.Size(81, 34);
            this.gbWorldspaceGridRangeIndicator.TabIndex = 3;
            this.gbWorldspaceGridRangeIndicator.TabStop = false;
            // 
            // pnWindow
            // 
            this.pnWindow.Controls.Add(this.lvWorldspaces);
            this.pnWindow.Controls.Add(this.gbWorldspace);
            this.pnWindow.Location = new System.Drawing.Point(0, 0);
            this.pnWindow.Name = "pnWindow";
            this.pnWindow.Size = new System.Drawing.Size(653, 219);
            this.pnWindow.TabIndex = 18;
            // 
            // RenderWindowWorldspaceToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 220);
            this.Controls.Add(this.pnWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(659, 242);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(103, 22);
            this.Name = "RenderWindowWorldspaceToolWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "title";
            this.Tag = "RenderWindow.Worldspaces";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.OnActivated);
            this.Deactivate += new System.EventHandler(this.OnDeactivate);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.gbWorldspace.ResumeLayout(false);
            this.gbWorldspace.PerformLayout();
            this.gbWorldspaceMapHeightRange.ResumeLayout(false);
            this.gbWorldspaceMapHeightRange.PerformLayout();
            this.gbWorldspaceTextures.ResumeLayout(false);
            this.gbWorldspaceTextures.PerformLayout();
            this.gbWorldspaceGridRange.ResumeLayout(false);
            this.gbWorldspaceGridRange.PerformLayout();
            this.pnWindow.ResumeLayout(false);
            this.ResumeLayout(false);
            
        }
    }
}
