/*
 * [Insert File Name Here]
 *
 * Insert description here.
 *
 */
namespace GUIBuilder.Windows
{
    partial class Render
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.Panel pnRenderTarget;
        System.Windows.Forms.ToolStrip tsRenderWindow;
        System.Windows.Forms.ToolStripButton tsRenderOverRegion;
        System.Windows.Forms.ToolStripButton tsRenderLandHeight;
        System.Windows.Forms.ToolStripButton tsRenderWaterHeight;
        System.Windows.Forms.ToolStripButton tsRenderCellGrid;
        System.Windows.Forms.ToolStripButton tsRenderBuildVolumes;
        System.Windows.Forms.ToolStripButton tsRenderBorders;
        System.Windows.Forms.ToolStripButton tsRenderEdgeFlags;
        System.Windows.Forms.ToolStripButton tsRenderSettlements;
        System.Windows.Forms.ToolStripButton tsRenderSubDivisions;
        System.Windows.Forms.ToolStripButton tsRenderEdgeFlagLinks;
        System.Windows.Forms.Panel pnWindow;
        System.Windows.Forms.ToolStripButton tsRenderWorkshops;
        System.Windows.Forms.ToolStripButton tsRenderSandboxVolumes;
        System.Windows.Forms.ToolStripTextBox tsMinSettlementObjectsRenderSize;
        System.Windows.Forms.ToolStripButton tsRenderSelectedOnly;
        System.Windows.Forms.ToolStripButton tsRepaintAllObjects;
        System.Windows.Forms.ToolStripLabel tslMouseToCellGrid;
        System.Windows.Forms.ToolStripLabel tslMouseToWorldspace;
        System.Windows.Forms.ToolStripLabel tslEditorSelectionMode;
        
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
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator1;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator2;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator3;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator4;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator5;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator6;
            System.Windows.Forms.ToolStripSeparator tsRenderSeparator7;
            this.tsRenderWindow = new System.Windows.Forms.ToolStrip();
            this.tsRenderOverRegion = new System.Windows.Forms.ToolStripButton();
            this.tsRenderLandHeight = new System.Windows.Forms.ToolStripButton();
            this.tsRenderWaterHeight = new System.Windows.Forms.ToolStripButton();
            this.tsRenderCellGrid = new System.Windows.Forms.ToolStripButton();
            this.tsMinSettlementObjectsRenderSize = new System.Windows.Forms.ToolStripTextBox();
            this.tsRenderSelectedOnly = new System.Windows.Forms.ToolStripButton();
            this.tsRepaintAllObjects = new System.Windows.Forms.ToolStripButton();
            this.tsRenderWorkshops = new System.Windows.Forms.ToolStripButton();
            this.tsRenderSettlements = new System.Windows.Forms.ToolStripButton();
            this.tsRenderSubDivisions = new System.Windows.Forms.ToolStripButton();
            this.tsRenderEdgeFlags = new System.Windows.Forms.ToolStripButton();
            this.tsRenderEdgeFlagLinks = new System.Windows.Forms.ToolStripButton();
            this.tsRenderBuildVolumes = new System.Windows.Forms.ToolStripButton();
            this.tsRenderSandboxVolumes = new System.Windows.Forms.ToolStripButton();
            this.tsRenderBorders = new System.Windows.Forms.ToolStripButton();
            this.tslMouseToCellGrid = new System.Windows.Forms.ToolStripLabel();
            this.tslMouseToWorldspace = new System.Windows.Forms.ToolStripLabel();
            this.tslEditorSelectionMode = new System.Windows.Forms.ToolStripLabel();
            this.pnRenderTarget = new System.Windows.Forms.Panel();
            this.pnWindow = new System.Windows.Forms.Panel();
            tsRenderSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            tsRenderSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsRenderWindow.SuspendLayout();
            this.pnWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsRenderSeparator1
            // 
            tsRenderSeparator1.Name = "tsRenderSeparator1";
            tsRenderSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator2
            // 
            tsRenderSeparator2.Name = "tsRenderSeparator2";
            tsRenderSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator3
            // 
            tsRenderSeparator3.Name = "tsRenderSeparator3";
            tsRenderSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator4
            // 
            tsRenderSeparator4.Name = "tsRenderSeparator4";
            tsRenderSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator5
            // 
            tsRenderSeparator5.Name = "tsRenderSeparator5";
            tsRenderSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator6
            // 
            tsRenderSeparator6.Name = "tsRenderSeparator6";
            tsRenderSeparator6.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderSeparator7
            // 
            tsRenderSeparator7.Name = "tsRenderSeparator7";
            tsRenderSeparator7.Size = new System.Drawing.Size(6, 23);
            // 
            // tsRenderWindow
            // 
            this.tsRenderWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsRenderOverRegion,
            tsRenderSeparator1,
            this.tsRenderLandHeight,
            this.tsRenderWaterHeight,
            tsRenderSeparator2,
            this.tsRenderCellGrid,
            tsRenderSeparator3,
            this.tsMinSettlementObjectsRenderSize,
            this.tsRenderSelectedOnly,
            this.tsRepaintAllObjects,
            tsRenderSeparator4,
            this.tsRenderWorkshops,
            this.tsRenderSettlements,
            this.tsRenderSubDivisions,
            tsRenderSeparator5,
            this.tsRenderBorders,
            tsRenderSeparator7,
            this.tsRenderEdgeFlags,
            this.tsRenderEdgeFlagLinks,
            tsRenderSeparator6,
            this.tsRenderBuildVolumes,
            this.tsRenderSandboxVolumes,
            this.tslMouseToCellGrid,
            this.tslMouseToWorldspace,
            this.tslEditorSelectionMode});
            this.tsRenderWindow.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.tsRenderWindow.Location = new System.Drawing.Point(0, 0);
            this.tsRenderWindow.Name = "tsRenderWindow";
            this.tsRenderWindow.Size = new System.Drawing.Size(771, 23);
            this.tsRenderWindow.TabIndex = 7;
            // 
            // tsRenderOverRegion
            // 
            this.tsRenderOverRegion.CheckOnClick = true;
            this.tsRenderOverRegion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderOverRegion.Image = global::Properties.Resources.tsIconNonPlayableRegion;
            this.tsRenderOverRegion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderOverRegion.Name = "tsRenderOverRegion";
            this.tsRenderOverRegion.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsRenderOverRegion.Size = new System.Drawing.Size(23, 20);
            this.tsRenderOverRegion.ToolTipText = "Non-Playable Regions";
            this.tsRenderOverRegion.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderLandHeight
            // 
            this.tsRenderLandHeight.Checked = true;
            this.tsRenderLandHeight.CheckOnClick = true;
            this.tsRenderLandHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderLandHeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderLandHeight.Image = global::Properties.Resources.tsIconLandHeight;
            this.tsRenderLandHeight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderLandHeight.Name = "tsRenderLandHeight";
            this.tsRenderLandHeight.Size = new System.Drawing.Size(23, 20);
            this.tsRenderLandHeight.ToolTipText = "Land Heightmap";
            this.tsRenderLandHeight.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderWaterHeight
            // 
            this.tsRenderWaterHeight.Checked = true;
            this.tsRenderWaterHeight.CheckOnClick = true;
            this.tsRenderWaterHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderWaterHeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderWaterHeight.Image = global::Properties.Resources.tsIconWaterHeight;
            this.tsRenderWaterHeight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderWaterHeight.Name = "tsRenderWaterHeight";
            this.tsRenderWaterHeight.Size = new System.Drawing.Size(23, 20);
            this.tsRenderWaterHeight.ToolTipText = "Water Heightmap";
            this.tsRenderWaterHeight.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderCellGrid
            // 
            this.tsRenderCellGrid.Checked = true;
            this.tsRenderCellGrid.CheckOnClick = true;
            this.tsRenderCellGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderCellGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderCellGrid.Image = global::Properties.Resources.tsIconCellGrid;
            this.tsRenderCellGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderCellGrid.Name = "tsRenderCellGrid";
            this.tsRenderCellGrid.Size = new System.Drawing.Size(23, 20);
            this.tsRenderCellGrid.ToolTipText = "Cell Grid";
            this.tsRenderCellGrid.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsMinSettlementObjectsRenderSize
            // 
            this.tsMinSettlementObjectsRenderSize.Name = "tsMinSettlementObjectsRenderSize";
            this.tsMinSettlementObjectsRenderSize.Size = new System.Drawing.Size(32, 21);
            this.tsMinSettlementObjectsRenderSize.Text = "4.0";
            this.tsMinSettlementObjectsRenderSize.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tsMinSettlementObjectsRenderSize.ToolTipText = "Minimum scaled size of objects";
            this.tsMinSettlementObjectsRenderSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tsMinSettlementObjectsRenderSizeKeyPress);
            this.tsMinSettlementObjectsRenderSize.TextChanged += new System.EventHandler(this.tsMinSettlementObjectsRenderSizeTextChanged);
            // 
            // tsRenderSelectedOnly
            // 
            this.tsRenderSelectedOnly.CheckOnClick = true;
            this.tsRenderSelectedOnly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderSelectedOnly.Image = global::Properties.Resources.tsIconUnchecked;
            this.tsRenderSelectedOnly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderSelectedOnly.Name = "tsRenderSelectedOnly";
            this.tsRenderSelectedOnly.Size = new System.Drawing.Size(23, 20);
            this.tsRenderSelectedOnly.ToolTipText = "Render selected objects only";
            this.tsRenderSelectedOnly.CheckStateChanged += new System.EventHandler(this.tsRenderSelectedOnlyCheckStateChanged);
            // 
            // tsRepaintAllObjects
            // 
            this.tsRepaintAllObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRepaintAllObjects.Image = global::Properties.Resources.tsIconRepaint;
            this.tsRepaintAllObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRepaintAllObjects.Name = "tsRepaintAllObjects";
            this.tsRepaintAllObjects.Size = new System.Drawing.Size(23, 20);
            this.tsRepaintAllObjects.ToolTipText = "Repaint all objects";
            this.tsRepaintAllObjects.Click += new System.EventHandler(this.tsRepaintAllObjectsClick);
            // 
            // tsRenderWorkshops
            // 
            this.tsRenderWorkshops.Checked = true;
            this.tsRenderWorkshops.CheckOnClick = true;
            this.tsRenderWorkshops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderWorkshops.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderWorkshops.Image = global::Properties.Resources.tsIconWorkshop;
            this.tsRenderWorkshops.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderWorkshops.Name = "tsRenderWorkshops";
            this.tsRenderWorkshops.Size = new System.Drawing.Size(23, 20);
            this.tsRenderWorkshops.ToolTipText = "Workshops";
            this.tsRenderWorkshops.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderSettlements
            // 
            this.tsRenderSettlements.Checked = true;
            this.tsRenderSettlements.CheckOnClick = true;
            this.tsRenderSettlements.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderSettlements.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderSettlements.Image = global::Properties.Resources.tsIconSettlement;
            this.tsRenderSettlements.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderSettlements.Name = "tsRenderSettlements";
            this.tsRenderSettlements.Size = new System.Drawing.Size(23, 20);
            this.tsRenderSettlements.ToolTipText = "Settlements";
            this.tsRenderSettlements.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderSubDivisions
            // 
            this.tsRenderSubDivisions.Checked = true;
            this.tsRenderSubDivisions.CheckOnClick = true;
            this.tsRenderSubDivisions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderSubDivisions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderSubDivisions.Image = global::Properties.Resources.tsIconSubDivision;
            this.tsRenderSubDivisions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderSubDivisions.Name = "tsRenderSubDivisions";
            this.tsRenderSubDivisions.Size = new System.Drawing.Size(23, 20);
            this.tsRenderSubDivisions.ToolTipText = "Sub-Divisions";
            this.tsRenderSubDivisions.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderEdgeFlags
            // 
            this.tsRenderEdgeFlags.Checked = true;
            this.tsRenderEdgeFlags.CheckOnClick = true;
            this.tsRenderEdgeFlags.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderEdgeFlags.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderEdgeFlags.Image = global::Properties.Resources.tsIconEdgeFlag;
            this.tsRenderEdgeFlags.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderEdgeFlags.Name = "tsRenderEdgeFlags";
            this.tsRenderEdgeFlags.Size = new System.Drawing.Size(23, 20);
            this.tsRenderEdgeFlags.ToolTipText = "Sub-Division Edge Flags";
            this.tsRenderEdgeFlags.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderEdgeFlagLinks
            // 
            this.tsRenderEdgeFlagLinks.CheckOnClick = true;
            this.tsRenderEdgeFlagLinks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderEdgeFlagLinks.Image = global::Properties.Resources.tsIconEdgeFlagLink;
            this.tsRenderEdgeFlagLinks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderEdgeFlagLinks.Name = "tsRenderEdgeFlagLinks";
            this.tsRenderEdgeFlagLinks.Size = new System.Drawing.Size(23, 20);
            this.tsRenderEdgeFlagLinks.ToolTipText = "Sub-Division Edge Flag Links";
            this.tsRenderEdgeFlagLinks.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderBuildVolumes
            // 
            this.tsRenderBuildVolumes.CheckOnClick = true;
            this.tsRenderBuildVolumes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderBuildVolumes.Image = global::Properties.Resources.tsIconBuildVolumes;
            this.tsRenderBuildVolumes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderBuildVolumes.Name = "tsRenderBuildVolumes";
            this.tsRenderBuildVolumes.Size = new System.Drawing.Size(23, 20);
            this.tsRenderBuildVolumes.ToolTipText = "Build Volumes";
            this.tsRenderBuildVolumes.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderSandboxVolumes
            // 
            this.tsRenderSandboxVolumes.CheckOnClick = true;
            this.tsRenderSandboxVolumes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderSandboxVolumes.Image = global::Properties.Resources.tsIconSandboxVolumes;
            this.tsRenderSandboxVolumes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderSandboxVolumes.Name = "tsRenderSandboxVolumes";
            this.tsRenderSandboxVolumes.Size = new System.Drawing.Size(23, 20);
            this.tsRenderSandboxVolumes.ToolTipText = "Sandbox Volumes";
            this.tsRenderSandboxVolumes.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tsRenderBorders
            // 
            this.tsRenderBorders.Checked = true;
            this.tsRenderBorders.CheckOnClick = true;
            this.tsRenderBorders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsRenderBorders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRenderBorders.Image = global::Properties.Resources.tsIconBorders;
            this.tsRenderBorders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRenderBorders.Name = "tsRenderBorders";
            this.tsRenderBorders.Size = new System.Drawing.Size(23, 20);
            this.tsRenderBorders.ToolTipText = "Borders";
            this.tsRenderBorders.CheckedChanged += new System.EventHandler(this.RenderStateControlChanged);
            // 
            // tslMouseToCellGrid
            // 
            this.tslMouseToCellGrid.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslMouseToCellGrid.AutoSize = false;
            this.tslMouseToCellGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tslMouseToCellGrid.Name = "tslMouseToCellGrid";
            this.tslMouseToCellGrid.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tslMouseToCellGrid.Size = new System.Drawing.Size(64, 20);
            this.tslMouseToCellGrid.Text = "-99,-99";
            // 
            // tslMouseToWorldspace
            // 
            this.tslMouseToWorldspace.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslMouseToWorldspace.AutoSize = false;
            this.tslMouseToWorldspace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tslMouseToWorldspace.Name = "tslMouseToWorldspace";
            this.tslMouseToWorldspace.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tslMouseToWorldspace.Size = new System.Drawing.Size(112, 20);
            this.tslMouseToWorldspace.Text = "(-123456,-123456)";
            // 
            // tslEditorSelectionMode
            // 
            this.tslEditorSelectionMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslEditorSelectionMode.AutoSize = false;
            this.tslEditorSelectionMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tslEditorSelectionMode.Name = "tslEditorSelectionMode";
            this.tslEditorSelectionMode.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tslEditorSelectionMode.Size = new System.Drawing.Size(31, 20);
            // 
            // pnRenderTarget
            // 
            this.pnRenderTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnRenderTarget.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnRenderTarget.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pnRenderTarget.Location = new System.Drawing.Point(0, 23);
            this.pnRenderTarget.Name = "pnRenderTarget";
            this.pnRenderTarget.Size = new System.Drawing.Size(771, 332);
            this.pnRenderTarget.TabIndex = 8;
            // 
            // pnWindow
            // 
            this.pnWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnWindow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnWindow.Controls.Add(this.tsRenderWindow);
            this.pnWindow.Controls.Add(this.pnRenderTarget);
            this.pnWindow.Location = new System.Drawing.Point(0, 0);
            this.pnWindow.Name = "pnWindow";
            this.pnWindow.Size = new System.Drawing.Size(771, 355);
            this.pnWindow.TabIndex = 17;
            // 
            // Render
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 355);
            this.Controls.Add(this.pnWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(420, 360);
            this.Name = "Render";
            this.ShowInTaskbar = false;
            this.Text = "Render Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResizeEnd += new System.EventHandler(this.OnFormResizeEnd);
            this.Move += new System.EventHandler(this.OnFormMove);
            this.tsRenderWindow.ResumeLayout(false);
            this.tsRenderWindow.PerformLayout();
            this.pnWindow.ResumeLayout(false);
            this.pnWindow.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
