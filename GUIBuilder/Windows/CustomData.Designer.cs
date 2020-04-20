/*
 * Created by SharpDevelop.
 * Date: 03/01/2020
 * Time: 8:45 PM
 */
namespace GUIBuilder.Windows
{
    partial class CustomData
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tcCustomData;
        private System.Windows.Forms.TabPage tpForms;
        private System.Windows.Forms.GroupBox gbWorkshopContainers;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetection;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionStaticMarkers;
        private System.Windows.Forms.Label lblWorkshopBorderMarkerTerrainFollowing;
        private System.Windows.Forms.Label lblWorkshopForcedZStatic;
        private System.Windows.Forms.ComboBox cbWorkshopBorderMarkerTerrainFollowing;
        private System.Windows.Forms.ComboBox cbWorkshopBorderMarkerForcedZ;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionKeywords;
        private System.Windows.Forms.Label lbWorkshopBorderGenerator;
        private System.Windows.Forms.ComboBox cbWorkshopKeywordBorderGenerator;
        private System.Windows.Forms.Label lblWorkshopMarkerLink;
        private System.Windows.Forms.ComboBox cbWorkshopKeywordBorderLink;
        private System.Windows.Forms.CheckBox cbRestrictWorkshopForms;
        private System.Windows.Forms.GroupBox gbWorkshopContainerFilter;
        private GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Container> lvWorkshopContainers;
        private System.Windows.Forms.GroupBox gbWorkshopNodeDetectionLocationRefs;
        private System.Windows.Forms.Label lblWorkshopBordeRefBorderWithBottom;
        private System.Windows.Forms.ComboBox cbWorkshopBordeRefBorderWithBottom;
        private System.Windows.Forms.Button btnApplyWorkshopContainerFilter;
        private System.Windows.Forms.Button btnApplyWorkshopContainerSelection;
        private System.Windows.Forms.ComboBox cbWorkshopContainerFilter;
        private System.Windows.Forms.TabPage tpEditorIDs;
        private System.Windows.Forms.GroupBox gbEditorIDsWorkshop;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopCells;
        private System.Windows.Forms.Label lblEditorIDsWorkshopCells;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopEncounterZone;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopLayer;
        private System.Windows.Forms.Label lblEditorIDsWorkshopEncounterZone;
        private System.Windows.Forms.Label lblEditorIDsWorkshopLayer;
        private System.Windows.Forms.TextBox tbEditorIDsModPrefix;
        private System.Windows.Forms.Label lblEditorIDsModPrefix;
        private System.Windows.Forms.GroupBox gbEditorIDsLegend;
        private System.Windows.Forms.Label lblEditorIDsLegendName;
        private System.Windows.Forms.Label lblEditorIDsLegendType;
        private System.Windows.Forms.Label gbEditorIDsLegendModPrefix;
        private System.Windows.Forms.Label lblEditorIDsLegendIndex;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopWorkbenchRef;
        private System.Windows.Forms.Label lblEditorIDsWorkshopWorkbenchRef;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopCenterMarker;
        private System.Windows.Forms.Label lblEditorIDsWorkshopCenterMarker;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopBuildVolumes;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopSandboxVolume;
        private System.Windows.Forms.Label lblEditorIDsWorkshopBuildVolumes;
        private System.Windows.Forms.Label lblEditorIDsWorkshopSandboxVolume;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopLocation;
        private System.Windows.Forms.Label lblEditorIDsWorkshopLocation;
        private System.Windows.Forms.Label lblEditorIDsLegendCaveat;
        private System.Windows.Forms.TextBox tbEditorIDsHoverOverSample;
        private System.Windows.Forms.TextBox tbEditorIDsWorkshopBorderStatic;
        private System.Windows.Forms.Label lblEditorIDsWorkshopBorderStatic;
        
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
            this.tcCustomData = new System.Windows.Forms.TabControl();
            this.tpEditorIDs = new System.Windows.Forms.TabPage();
            this.tbEditorIDsHoverOverSample = new System.Windows.Forms.TextBox();
            this.gbEditorIDsLegend = new System.Windows.Forms.GroupBox();
            this.lblEditorIDsLegendCaveat = new System.Windows.Forms.Label();
            this.lblEditorIDsLegendIndex = new System.Windows.Forms.Label();
            this.lblEditorIDsLegendName = new System.Windows.Forms.Label();
            this.lblEditorIDsLegendType = new System.Windows.Forms.Label();
            this.gbEditorIDsLegendModPrefix = new System.Windows.Forms.Label();
            this.gbEditorIDsWorkshop = new System.Windows.Forms.GroupBox();
            this.tbEditorIDsWorkshopWorkbenchRef = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopWorkbenchRef = new System.Windows.Forms.Label();
            this.tbEditorIDsWorkshopCenterMarker = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopCenterMarker = new System.Windows.Forms.Label();
            this.tbEditorIDsWorkshopBuildVolumes = new System.Windows.Forms.TextBox();
            this.tbEditorIDsWorkshopSandboxVolume = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopBuildVolumes = new System.Windows.Forms.Label();
            this.lblEditorIDsWorkshopSandboxVolume = new System.Windows.Forms.Label();
            this.tbEditorIDsWorkshopLocation = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopLocation = new System.Windows.Forms.Label();
            this.tbEditorIDsWorkshopCells = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopCells = new System.Windows.Forms.Label();
            this.tbEditorIDsWorkshopEncounterZone = new System.Windows.Forms.TextBox();
            this.tbEditorIDsWorkshopLayer = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopEncounterZone = new System.Windows.Forms.Label();
            this.lblEditorIDsWorkshopLayer = new System.Windows.Forms.Label();
            this.tbEditorIDsModPrefix = new System.Windows.Forms.TextBox();
            this.lblEditorIDsModPrefix = new System.Windows.Forms.Label();
            this.tpForms = new System.Windows.Forms.TabPage();
            this.gbWorkshopContainers = new System.Windows.Forms.GroupBox();
            this.btnApplyWorkshopContainerSelection = new System.Windows.Forms.Button();
            this.lvWorkshopContainers = new GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Container>();
            this.gbWorkshopContainerFilter = new System.Windows.Forms.GroupBox();
            this.cbWorkshopContainerFilter = new System.Windows.Forms.ComboBox();
            this.btnApplyWorkshopContainerFilter = new System.Windows.Forms.Button();
            this.gbWorkshopNodeDetection = new System.Windows.Forms.GroupBox();
            this.gbWorkshopNodeDetectionLocationRefs = new System.Windows.Forms.GroupBox();
            this.lblWorkshopBordeRefBorderWithBottom = new System.Windows.Forms.Label();
            this.cbWorkshopBordeRefBorderWithBottom = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNodeDetectionStaticMarkers = new System.Windows.Forms.GroupBox();
            this.lblWorkshopBorderMarkerTerrainFollowing = new System.Windows.Forms.Label();
            this.lblWorkshopForcedZStatic = new System.Windows.Forms.Label();
            this.cbWorkshopBorderMarkerTerrainFollowing = new System.Windows.Forms.ComboBox();
            this.cbWorkshopBorderMarkerForcedZ = new System.Windows.Forms.ComboBox();
            this.gbWorkshopNodeDetectionKeywords = new System.Windows.Forms.GroupBox();
            this.lbWorkshopBorderGenerator = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderGenerator = new System.Windows.Forms.ComboBox();
            this.lblWorkshopMarkerLink = new System.Windows.Forms.Label();
            this.cbWorkshopKeywordBorderLink = new System.Windows.Forms.ComboBox();
            this.cbRestrictWorkshopForms = new System.Windows.Forms.CheckBox();
            this.tbEditorIDsWorkshopBorderStatic = new System.Windows.Forms.TextBox();
            this.lblEditorIDsWorkshopBorderStatic = new System.Windows.Forms.Label();
            this.WindowPanel.SuspendLayout();
            this.tcCustomData.SuspendLayout();
            this.tpEditorIDs.SuspendLayout();
            this.gbEditorIDsLegend.SuspendLayout();
            this.gbEditorIDsWorkshop.SuspendLayout();
            this.tpForms.SuspendLayout();
            this.gbWorkshopContainers.SuspendLayout();
            this.gbWorkshopContainerFilter.SuspendLayout();
            this.gbWorkshopNodeDetection.SuspendLayout();
            this.gbWorkshopNodeDetectionLocationRefs.SuspendLayout();
            this.gbWorkshopNodeDetectionStaticMarkers.SuspendLayout();
            this.gbWorkshopNodeDetectionKeywords.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add(this.tcCustomData);
            this.WindowPanel.Controls.Add(this.cbRestrictWorkshopForms);
            this.WindowPanel.Size = new System.Drawing.Size(672, 375);
            // 
            // tcCustomData
            // 
            this.tcCustomData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCustomData.Controls.Add(this.tpEditorIDs);
            this.tcCustomData.Controls.Add(this.tpForms);
            this.tcCustomData.Location = new System.Drawing.Point(0, 36);
            this.tcCustomData.Name = "tcCustomData";
            this.tcCustomData.SelectedIndex = 0;
            this.tcCustomData.Size = new System.Drawing.Size(672, 339);
            this.tcCustomData.TabIndex = 0;
            // 
            // tpEditorIDs
            // 
            this.tpEditorIDs.Controls.Add(this.tbEditorIDsHoverOverSample);
            this.tpEditorIDs.Controls.Add(this.gbEditorIDsLegend);
            this.tpEditorIDs.Controls.Add(this.gbEditorIDsWorkshop);
            this.tpEditorIDs.Controls.Add(this.tbEditorIDsModPrefix);
            this.tpEditorIDs.Controls.Add(this.lblEditorIDsModPrefix);
            this.tpEditorIDs.Location = new System.Drawing.Point(4, 22);
            this.tpEditorIDs.Name = "tpEditorIDs";
            this.tpEditorIDs.Size = new System.Drawing.Size(664, 313);
            this.tpEditorIDs.TabIndex = 1;
            this.tpEditorIDs.Tag = "CustomDataWindow.Tab.EditorIDs";
            this.tpEditorIDs.Text = "EditorIDs";
            this.tpEditorIDs.UseVisualStyleBackColor = true;
            // 
            // tbEditorIDsHoverOverSample
            // 
            this.tbEditorIDsHoverOverSample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEditorIDsHoverOverSample.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.tbEditorIDsHoverOverSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbEditorIDsHoverOverSample.Enabled = false;
            this.tbEditorIDsHoverOverSample.Location = new System.Drawing.Point(3, 287);
            this.tbEditorIDsHoverOverSample.Name = "tbEditorIDsHoverOverSample";
            this.tbEditorIDsHoverOverSample.Size = new System.Drawing.Size(658, 20);
            this.tbEditorIDsHoverOverSample.TabIndex = 8;
            this.tbEditorIDsHoverOverSample.Text = "MyMod_Riverwood_WorkshopRef";
            // 
            // gbEditorIDsLegend
            // 
            this.gbEditorIDsLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditorIDsLegend.Controls.Add(this.lblEditorIDsLegendCaveat);
            this.gbEditorIDsLegend.Controls.Add(this.lblEditorIDsLegendIndex);
            this.gbEditorIDsLegend.Controls.Add(this.lblEditorIDsLegendName);
            this.gbEditorIDsLegend.Controls.Add(this.lblEditorIDsLegendType);
            this.gbEditorIDsLegend.Controls.Add(this.gbEditorIDsLegendModPrefix);
            this.gbEditorIDsLegend.Location = new System.Drawing.Point(436, 3);
            this.gbEditorIDsLegend.Name = "gbEditorIDsLegend";
            this.gbEditorIDsLegend.Size = new System.Drawing.Size(228, 278);
            this.gbEditorIDsLegend.TabIndex = 7;
            this.gbEditorIDsLegend.TabStop = false;
            this.gbEditorIDsLegend.Tag = "CustomDataWindow.Tab.EditorIDs.Legend:";
            this.gbEditorIDsLegend.Text = "Token Replacement Keys:";
            // 
            // lblEditorIDsLegendCaveat
            // 
            this.lblEditorIDsLegendCaveat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditorIDsLegendCaveat.Location = new System.Drawing.Point(6, 228);
            this.lblEditorIDsLegendCaveat.Name = "lblEditorIDsLegendCaveat";
            this.lblEditorIDsLegendCaveat.Size = new System.Drawing.Size(216, 44);
            this.lblEditorIDsLegendCaveat.TabIndex = 4;
            this.lblEditorIDsLegendCaveat.Tag = "CustomDataWindow.Tab.EditorIDs.Legend.Caveat";
            this.lblEditorIDsLegendCaveat.Text = "Caveats";
            // 
            // lblEditorIDsLegendIndex
            // 
            this.lblEditorIDsLegendIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditorIDsLegendIndex.Location = new System.Drawing.Point(6, 67);
            this.lblEditorIDsLegendIndex.Name = "lblEditorIDsLegendIndex";
            this.lblEditorIDsLegendIndex.Size = new System.Drawing.Size(216, 17);
            this.lblEditorIDsLegendIndex.TabIndex = 3;
            this.lblEditorIDsLegendIndex.Tag = "CustomDataWindow.Tab.EditorIDs.Legend.Index";
            // 
            // lblEditorIDsLegendName
            // 
            this.lblEditorIDsLegendName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditorIDsLegendName.Location = new System.Drawing.Point(6, 50);
            this.lblEditorIDsLegendName.Name = "lblEditorIDsLegendName";
            this.lblEditorIDsLegendName.Size = new System.Drawing.Size(216, 17);
            this.lblEditorIDsLegendName.TabIndex = 2;
            this.lblEditorIDsLegendName.Tag = "CustomDataWindow.Tab.EditorIDs.Legend.Name";
            // 
            // lblEditorIDsLegendType
            // 
            this.lblEditorIDsLegendType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditorIDsLegendType.Location = new System.Drawing.Point(6, 33);
            this.lblEditorIDsLegendType.Name = "lblEditorIDsLegendType";
            this.lblEditorIDsLegendType.Size = new System.Drawing.Size(216, 17);
            this.lblEditorIDsLegendType.TabIndex = 1;
            this.lblEditorIDsLegendType.Tag = "CustomDataWindow.Tab.EditorIDs.Legend.Type";
            // 
            // gbEditorIDsLegendModPrefix
            // 
            this.gbEditorIDsLegendModPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEditorIDsLegendModPrefix.Location = new System.Drawing.Point(6, 16);
            this.gbEditorIDsLegendModPrefix.Name = "gbEditorIDsLegendModPrefix";
            this.gbEditorIDsLegendModPrefix.Size = new System.Drawing.Size(216, 17);
            this.gbEditorIDsLegendModPrefix.TabIndex = 0;
            this.gbEditorIDsLegendModPrefix.Tag = "CustomDataWindow.Tab.EditorIDs.Legend.ModPrefix";
            // 
            // gbEditorIDsWorkshop
            // 
            this.gbEditorIDsWorkshop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopBorderStatic);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopBorderStatic);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopWorkbenchRef);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopWorkbenchRef);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopCenterMarker);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopCenterMarker);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopBuildVolumes);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopSandboxVolume);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopBuildVolumes);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopSandboxVolume);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopLocation);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopLocation);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopCells);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopCells);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopEncounterZone);
            this.gbEditorIDsWorkshop.Controls.Add(this.tbEditorIDsWorkshopLayer);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopEncounterZone);
            this.gbEditorIDsWorkshop.Controls.Add(this.lblEditorIDsWorkshopLayer);
            this.gbEditorIDsWorkshop.Location = new System.Drawing.Point(0, 30);
            this.gbEditorIDsWorkshop.Name = "gbEditorIDsWorkshop";
            this.gbEditorIDsWorkshop.Size = new System.Drawing.Size(430, 251);
            this.gbEditorIDsWorkshop.TabIndex = 6;
            this.gbEditorIDsWorkshop.TabStop = false;
            this.gbEditorIDsWorkshop.Text = "Workshops";
            // 
            // tbEditorIDsWorkshopWorkbenchRef
            // 
            this.tbEditorIDsWorkshopWorkbenchRef.Location = new System.Drawing.Point(169, 99);
            this.tbEditorIDsWorkshopWorkbenchRef.Name = "tbEditorIDsWorkshopWorkbenchRef";
            this.tbEditorIDsWorkshopWorkbenchRef.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopWorkbenchRef.TabIndex = 17;
            this.tbEditorIDsWorkshopWorkbenchRef.Text = "{mod}_{name}_WorkshopRef";
            // 
            // lblEditorIDsWorkshopWorkbenchRef
            // 
            this.lblEditorIDsWorkshopWorkbenchRef.Location = new System.Drawing.Point(3, 102);
            this.lblEditorIDsWorkshopWorkbenchRef.Name = "lblEditorIDsWorkshopWorkbenchRef";
            this.lblEditorIDsWorkshopWorkbenchRef.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopWorkbenchRef.TabIndex = 16;
            this.lblEditorIDsWorkshopWorkbenchRef.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.WorkbenchRef:";
            this.lblEditorIDsWorkshopWorkbenchRef.Text = "Workshop Ref";
            // 
            // tbEditorIDsWorkshopCenterMarker
            // 
            this.tbEditorIDsWorkshopCenterMarker.Location = new System.Drawing.Point(169, 183);
            this.tbEditorIDsWorkshopCenterMarker.Name = "tbEditorIDsWorkshopCenterMarker";
            this.tbEditorIDsWorkshopCenterMarker.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopCenterMarker.TabIndex = 15;
            this.tbEditorIDsWorkshopCenterMarker.Text = "{mod}_{name}_Center";
            // 
            // lblEditorIDsWorkshopCenterMarker
            // 
            this.lblEditorIDsWorkshopCenterMarker.Location = new System.Drawing.Point(3, 186);
            this.lblEditorIDsWorkshopCenterMarker.Name = "lblEditorIDsWorkshopCenterMarker";
            this.lblEditorIDsWorkshopCenterMarker.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopCenterMarker.TabIndex = 14;
            this.lblEditorIDsWorkshopCenterMarker.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.CenterMarker:";
            this.lblEditorIDsWorkshopCenterMarker.Text = "Center Marker";
            // 
            // tbEditorIDsWorkshopBuildVolumes
            // 
            this.tbEditorIDsWorkshopBuildVolumes.Location = new System.Drawing.Point(169, 141);
            this.tbEditorIDsWorkshopBuildVolumes.Name = "tbEditorIDsWorkshopBuildVolumes";
            this.tbEditorIDsWorkshopBuildVolumes.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopBuildVolumes.TabIndex = 11;
            this.tbEditorIDsWorkshopBuildVolumes.Text = "{mod}_{name}_BuildAreaVolume_{index}";
            // 
            // tbEditorIDsWorkshopSandboxVolume
            // 
            this.tbEditorIDsWorkshopSandboxVolume.Location = new System.Drawing.Point(169, 162);
            this.tbEditorIDsWorkshopSandboxVolume.Name = "tbEditorIDsWorkshopSandboxVolume";
            this.tbEditorIDsWorkshopSandboxVolume.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopSandboxVolume.TabIndex = 13;
            this.tbEditorIDsWorkshopSandboxVolume.Text = "{mod}_{name}_SandboxVolume";
            // 
            // lblEditorIDsWorkshopBuildVolumes
            // 
            this.lblEditorIDsWorkshopBuildVolumes.Location = new System.Drawing.Point(3, 144);
            this.lblEditorIDsWorkshopBuildVolumes.Name = "lblEditorIDsWorkshopBuildVolumes";
            this.lblEditorIDsWorkshopBuildVolumes.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopBuildVolumes.TabIndex = 10;
            this.lblEditorIDsWorkshopBuildVolumes.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.BuildVolumes:";
            this.lblEditorIDsWorkshopBuildVolumes.Text = "Build Volumes";
            // 
            // lblEditorIDsWorkshopSandboxVolume
            // 
            this.lblEditorIDsWorkshopSandboxVolume.Location = new System.Drawing.Point(3, 165);
            this.lblEditorIDsWorkshopSandboxVolume.Name = "lblEditorIDsWorkshopSandboxVolume";
            this.lblEditorIDsWorkshopSandboxVolume.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopSandboxVolume.TabIndex = 12;
            this.lblEditorIDsWorkshopSandboxVolume.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.SandboxVolume:";
            this.lblEditorIDsWorkshopSandboxVolume.Text = "Sandbox Volume";
            // 
            // tbEditorIDsWorkshopLocation
            // 
            this.tbEditorIDsWorkshopLocation.Location = new System.Drawing.Point(169, 15);
            this.tbEditorIDsWorkshopLocation.Name = "tbEditorIDsWorkshopLocation";
            this.tbEditorIDsWorkshopLocation.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopLocation.TabIndex = 9;
            this.tbEditorIDsWorkshopLocation.Text = "{mod}_{name}_Location";
            // 
            // lblEditorIDsWorkshopLocation
            // 
            this.lblEditorIDsWorkshopLocation.Location = new System.Drawing.Point(3, 18);
            this.lblEditorIDsWorkshopLocation.Name = "lblEditorIDsWorkshopLocation";
            this.lblEditorIDsWorkshopLocation.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopLocation.TabIndex = 8;
            this.lblEditorIDsWorkshopLocation.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.Location:";
            this.lblEditorIDsWorkshopLocation.Text = "Location";
            // 
            // tbEditorIDsWorkshopCells
            // 
            this.tbEditorIDsWorkshopCells.Location = new System.Drawing.Point(169, 78);
            this.tbEditorIDsWorkshopCells.Name = "tbEditorIDsWorkshopCells";
            this.tbEditorIDsWorkshopCells.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopCells.TabIndex = 7;
            this.tbEditorIDsWorkshopCells.Text = "{mod}_{name}_{index}";
            // 
            // lblEditorIDsWorkshopCells
            // 
            this.lblEditorIDsWorkshopCells.Location = new System.Drawing.Point(3, 81);
            this.lblEditorIDsWorkshopCells.Name = "lblEditorIDsWorkshopCells";
            this.lblEditorIDsWorkshopCells.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopCells.TabIndex = 6;
            this.lblEditorIDsWorkshopCells.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.Cells:";
            this.lblEditorIDsWorkshopCells.Text = "Cells";
            // 
            // tbEditorIDsWorkshopEncounterZone
            // 
            this.tbEditorIDsWorkshopEncounterZone.Location = new System.Drawing.Point(169, 36);
            this.tbEditorIDsWorkshopEncounterZone.Name = "tbEditorIDsWorkshopEncounterZone";
            this.tbEditorIDsWorkshopEncounterZone.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopEncounterZone.TabIndex = 3;
            this.tbEditorIDsWorkshopEncounterZone.Text = "{mod}_{name}_EncounterZone";
            // 
            // tbEditorIDsWorkshopLayer
            // 
            this.tbEditorIDsWorkshopLayer.Location = new System.Drawing.Point(169, 57);
            this.tbEditorIDsWorkshopLayer.Name = "tbEditorIDsWorkshopLayer";
            this.tbEditorIDsWorkshopLayer.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopLayer.TabIndex = 5;
            this.tbEditorIDsWorkshopLayer.Text = "{mod}_{name}";
            // 
            // lblEditorIDsWorkshopEncounterZone
            // 
            this.lblEditorIDsWorkshopEncounterZone.Location = new System.Drawing.Point(3, 39);
            this.lblEditorIDsWorkshopEncounterZone.Name = "lblEditorIDsWorkshopEncounterZone";
            this.lblEditorIDsWorkshopEncounterZone.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopEncounterZone.TabIndex = 2;
            this.lblEditorIDsWorkshopEncounterZone.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.EncounterZone:";
            this.lblEditorIDsWorkshopEncounterZone.Text = "Encounter Zone";
            // 
            // lblEditorIDsWorkshopLayer
            // 
            this.lblEditorIDsWorkshopLayer.Location = new System.Drawing.Point(3, 60);
            this.lblEditorIDsWorkshopLayer.Name = "lblEditorIDsWorkshopLayer";
            this.lblEditorIDsWorkshopLayer.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopLayer.TabIndex = 4;
            this.lblEditorIDsWorkshopLayer.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.Layer:";
            this.lblEditorIDsWorkshopLayer.Text = "Layer";
            // 
            // tbEditorIDsModPrefix
            // 
            this.tbEditorIDsModPrefix.Location = new System.Drawing.Point(169, 3);
            this.tbEditorIDsModPrefix.Name = "tbEditorIDsModPrefix";
            this.tbEditorIDsModPrefix.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsModPrefix.TabIndex = 1;
            this.tbEditorIDsModPrefix.Text = "MyMod";
            // 
            // lblEditorIDsModPrefix
            // 
            this.lblEditorIDsModPrefix.Location = new System.Drawing.Point(3, 6);
            this.lblEditorIDsModPrefix.Name = "lblEditorIDsModPrefix";
            this.lblEditorIDsModPrefix.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsModPrefix.TabIndex = 0;
            this.lblEditorIDsModPrefix.Tag = "CustomDataWindow.Tab.EditorIDs.ModPrefix:";
            // 
            // tpForms
            // 
            this.tpForms.Controls.Add(this.gbWorkshopContainers);
            this.tpForms.Controls.Add(this.gbWorkshopNodeDetection);
            this.tpForms.Location = new System.Drawing.Point(4, 22);
            this.tpForms.Name = "tpForms";
            this.tpForms.Padding = new System.Windows.Forms.Padding(3);
            this.tpForms.Size = new System.Drawing.Size(664, 313);
            this.tpForms.TabIndex = 0;
            this.tpForms.Tag = "CustomDataWindow.Tab.Forms";
            this.tpForms.Text = "Forms";
            this.tpForms.UseVisualStyleBackColor = true;
            // 
            // gbWorkshopContainers
            // 
            this.gbWorkshopContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkshopContainers.Controls.Add(this.btnApplyWorkshopContainerSelection);
            this.gbWorkshopContainers.Controls.Add(this.lvWorkshopContainers);
            this.gbWorkshopContainers.Controls.Add(this.gbWorkshopContainerFilter);
            this.gbWorkshopContainers.Location = new System.Drawing.Point(336, 3);
            this.gbWorkshopContainers.Name = "gbWorkshopContainers";
            this.gbWorkshopContainers.Size = new System.Drawing.Size(328, 310);
            this.gbWorkshopContainers.TabIndex = 19;
            this.gbWorkshopContainers.TabStop = false;
            this.gbWorkshopContainers.Tag = "CustomDataWindow.Tab.Forms.Workshops";
            this.gbWorkshopContainers.Text = "Workshops";
            // 
            // btnApplyWorkshopContainerSelection
            // 
            this.btnApplyWorkshopContainerSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyWorkshopContainerSelection.Location = new System.Drawing.Point(184, 281);
            this.btnApplyWorkshopContainerSelection.Name = "btnApplyWorkshopContainerSelection";
            this.btnApplyWorkshopContainerSelection.Size = new System.Drawing.Size(136, 23);
            this.btnApplyWorkshopContainerSelection.TabIndex = 13;
            this.btnApplyWorkshopContainerSelection.Tag = "CustomDataWindow.Tab.Forms.Workshops.Filter.Apply";
            this.btnApplyWorkshopContainerSelection.Text = "Update With Selection";
            this.btnApplyWorkshopContainerSelection.UseVisualStyleBackColor = true;
            // 
            // lvWorkshopContainers
            // 
            this.lvWorkshopContainers.AllowHidingItems = false;
            this.lvWorkshopContainers.AllowOverrideColumnSorting = true;
            this.lvWorkshopContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWorkshopContainers.CheckBoxes = true;
            this.lvWorkshopContainers.EditorIDColumn = true;
            this.lvWorkshopContainers.ExtraInfoColumn = false;
            this.lvWorkshopContainers.FilenameColumn = true;
            this.lvWorkshopContainers.FormIDColumn = true;
            this.lvWorkshopContainers.LoadOrderColumn = false;
            this.lvWorkshopContainers.Location = new System.Drawing.Point(6, 61);
            this.lvWorkshopContainers.MultiSelect = true;
            this.lvWorkshopContainers.Name = "lvWorkshopContainers";
            this.lvWorkshopContainers.Size = new System.Drawing.Size(314, 214);
            this.lvWorkshopContainers.SortByColumn = GUIBuilder.Windows.Controls.SyncedSortByColumns.FormID;
            this.lvWorkshopContainers.SortDirection = GUIBuilder.Windows.Controls.SyncedSortDirections.Ascending;
            this.lvWorkshopContainers.SyncedEditorFormType = null;
            this.lvWorkshopContainers.SyncObjects = null;
            this.lvWorkshopContainers.TabIndex = 12;
            this.lvWorkshopContainers.TypeColumn = false;
            // 
            // gbWorkshopContainerFilter
            // 
            this.gbWorkshopContainerFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorkshopContainerFilter.Controls.Add(this.cbWorkshopContainerFilter);
            this.gbWorkshopContainerFilter.Controls.Add(this.btnApplyWorkshopContainerFilter);
            this.gbWorkshopContainerFilter.Location = new System.Drawing.Point(6, 19);
            this.gbWorkshopContainerFilter.Name = "gbWorkshopContainerFilter";
            this.gbWorkshopContainerFilter.Size = new System.Drawing.Size(316, 41);
            this.gbWorkshopContainerFilter.TabIndex = 0;
            this.gbWorkshopContainerFilter.TabStop = false;
            this.gbWorkshopContainerFilter.Tag = "CustomDataWindow.Tab.Forms.Workshops.Filter";
            this.gbWorkshopContainerFilter.Text = "Filter";
            // 
            // cbWorkshopContainerFilter
            // 
            this.cbWorkshopContainerFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWorkshopContainerFilter.FormattingEnabled = true;
            this.cbWorkshopContainerFilter.Location = new System.Drawing.Point(6, 14);
            this.cbWorkshopContainerFilter.Name = "cbWorkshopContainerFilter";
            this.cbWorkshopContainerFilter.Size = new System.Drawing.Size(223, 21);
            this.cbWorkshopContainerFilter.TabIndex = 2;
            // 
            // btnApplyWorkshopContainerFilter
            // 
            this.btnApplyWorkshopContainerFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyWorkshopContainerFilter.Location = new System.Drawing.Point(235, 12);
            this.btnApplyWorkshopContainerFilter.Name = "btnApplyWorkshopContainerFilter";
            this.btnApplyWorkshopContainerFilter.Size = new System.Drawing.Size(75, 23);
            this.btnApplyWorkshopContainerFilter.TabIndex = 1;
            this.btnApplyWorkshopContainerFilter.Tag = "CustomDataWindow.Tab.Forms.Workshops.Filter.Apply";
            this.btnApplyWorkshopContainerFilter.Text = "Apply Filter";
            this.btnApplyWorkshopContainerFilter.UseVisualStyleBackColor = true;
            // 
            // gbWorkshopNodeDetection
            // 
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionLocationRefs);
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionStaticMarkers);
            this.gbWorkshopNodeDetection.Controls.Add(this.gbWorkshopNodeDetectionKeywords);
            this.gbWorkshopNodeDetection.Location = new System.Drawing.Point(0, 3);
            this.gbWorkshopNodeDetection.Name = "gbWorkshopNodeDetection";
            this.gbWorkshopNodeDetection.Size = new System.Drawing.Size(330, 310);
            this.gbWorkshopNodeDetection.TabIndex = 18;
            this.gbWorkshopNodeDetection.TabStop = false;
            this.gbWorkshopNodeDetection.Tag = "BorderBatchWindow.NodeDetection";
            this.gbWorkshopNodeDetection.Text = "Node Detection";
            // 
            // gbWorkshopNodeDetectionLocationRefs
            // 
            this.gbWorkshopNodeDetectionLocationRefs.Controls.Add(this.lblWorkshopBordeRefBorderWithBottom);
            this.gbWorkshopNodeDetectionLocationRefs.Controls.Add(this.cbWorkshopBordeRefBorderWithBottom);
            this.gbWorkshopNodeDetectionLocationRefs.Location = new System.Drawing.Point(6, 243);
            this.gbWorkshopNodeDetectionLocationRefs.Name = "gbWorkshopNodeDetectionLocationRefs";
            this.gbWorkshopNodeDetectionLocationRefs.Size = new System.Drawing.Size(318, 62);
            this.gbWorkshopNodeDetectionLocationRefs.TabIndex = 23;
            this.gbWorkshopNodeDetectionLocationRefs.TabStop = false;
            this.gbWorkshopNodeDetectionLocationRefs.Tag = "BorderBatchWindow.NodeDetection.LocationRefs";
            this.gbWorkshopNodeDetectionLocationRefs.Text = "Location References";
            // 
            // lblWorkshopBordeRefBorderWithBottom
            // 
            this.lblWorkshopBordeRefBorderWithBottom.Location = new System.Drawing.Point(6, 16);
            this.lblWorkshopBordeRefBorderWithBottom.Name = "lblWorkshopBordeRefBorderWithBottom";
            this.lblWorkshopBordeRefBorderWithBottom.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopBordeRefBorderWithBottom.TabIndex = 17;
            this.lblWorkshopBordeRefBorderWithBottom.Tag = "BorderBatchWindow.NodeDetection.WorkshopBorderWithBottom:";
            this.lblWorkshopBordeRefBorderWithBottom.Text = "WorkshopBorderWithBottom:";
            // 
            // cbWorkshopBordeRefBorderWithBottom
            // 
            this.cbWorkshopBordeRefBorderWithBottom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBordeRefBorderWithBottom.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopBordeRefBorderWithBottom.Name = "cbWorkshopBordeRefBorderWithBottom";
            this.cbWorkshopBordeRefBorderWithBottom.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBordeRefBorderWithBottom.TabIndex = 13;
            // 
            // gbWorkshopNodeDetectionStaticMarkers
            // 
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.lblWorkshopBorderMarkerTerrainFollowing);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.lblWorkshopForcedZStatic);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerTerrainFollowing);
            this.gbWorkshopNodeDetectionStaticMarkers.Controls.Add(this.cbWorkshopBorderMarkerForcedZ);
            this.gbWorkshopNodeDetectionStaticMarkers.Location = new System.Drawing.Point(6, 131);
            this.gbWorkshopNodeDetectionStaticMarkers.Name = "gbWorkshopNodeDetectionStaticMarkers";
            this.gbWorkshopNodeDetectionStaticMarkers.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionStaticMarkers.TabIndex = 22;
            this.gbWorkshopNodeDetectionStaticMarkers.TabStop = false;
            this.gbWorkshopNodeDetectionStaticMarkers.Tag = "BorderBatchWindow.NodeDetection.StaticMarkers";
            this.gbWorkshopNodeDetectionStaticMarkers.Text = "StaticMarkers";
            // 
            // lblWorkshopBorderMarkerTerrainFollowing
            // 
            this.lblWorkshopBorderMarkerTerrainFollowing.Location = new System.Drawing.Point(6, 16);
            this.lblWorkshopBorderMarkerTerrainFollowing.Name = "lblWorkshopBorderMarkerTerrainFollowing";
            this.lblWorkshopBorderMarkerTerrainFollowing.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopBorderMarkerTerrainFollowing.TabIndex = 19;
            this.lblWorkshopBorderMarkerTerrainFollowing.Tag = "BorderBatchWindow.NodeDetection.BorderMarkerTerrainFollowing:";
            this.lblWorkshopBorderMarkerTerrainFollowing.Text = "BorderMarkerTerrainFollowing:";
            // 
            // lblWorkshopForcedZStatic
            // 
            this.lblWorkshopForcedZStatic.Location = new System.Drawing.Point(6, 60);
            this.lblWorkshopForcedZStatic.Name = "lblWorkshopForcedZStatic";
            this.lblWorkshopForcedZStatic.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopForcedZStatic.TabIndex = 15;
            this.lblWorkshopForcedZStatic.Tag = "BorderBatchWindow.NodeDetection.BorderMarkerForcedZ:";
            this.lblWorkshopForcedZStatic.Text = "BorderMarkerForcedZ:";
            // 
            // cbWorkshopBorderMarkerTerrainFollowing
            // 
            this.cbWorkshopBorderMarkerTerrainFollowing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBorderMarkerTerrainFollowing.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopBorderMarkerTerrainFollowing.Name = "cbWorkshopBorderMarkerTerrainFollowing";
            this.cbWorkshopBorderMarkerTerrainFollowing.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBorderMarkerTerrainFollowing.TabIndex = 20;
            // 
            // cbWorkshopBorderMarkerForcedZ
            // 
            this.cbWorkshopBorderMarkerForcedZ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopBorderMarkerForcedZ.Location = new System.Drawing.Point(6, 80);
            this.cbWorkshopBorderMarkerForcedZ.Name = "cbWorkshopBorderMarkerForcedZ";
            this.cbWorkshopBorderMarkerForcedZ.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopBorderMarkerForcedZ.TabIndex = 16;
            // 
            // gbWorkshopNodeDetectionKeywords
            // 
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lbWorkshopBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderGenerator);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.lblWorkshopMarkerLink);
            this.gbWorkshopNodeDetectionKeywords.Controls.Add(this.cbWorkshopKeywordBorderLink);
            this.gbWorkshopNodeDetectionKeywords.Location = new System.Drawing.Point(6, 19);
            this.gbWorkshopNodeDetectionKeywords.Name = "gbWorkshopNodeDetectionKeywords";
            this.gbWorkshopNodeDetectionKeywords.Size = new System.Drawing.Size(318, 106);
            this.gbWorkshopNodeDetectionKeywords.TabIndex = 21;
            this.gbWorkshopNodeDetectionKeywords.TabStop = false;
            this.gbWorkshopNodeDetectionKeywords.Tag = "BorderBatchWindow.NodeDetection.Keywords";
            this.gbWorkshopNodeDetectionKeywords.Text = "Keywords";
            // 
            // lbWorkshopBorderGenerator
            // 
            this.lbWorkshopBorderGenerator.Location = new System.Drawing.Point(6, 16);
            this.lbWorkshopBorderGenerator.Name = "lbWorkshopBorderGenerator";
            this.lbWorkshopBorderGenerator.Size = new System.Drawing.Size(306, 17);
            this.lbWorkshopBorderGenerator.TabIndex = 17;
            this.lbWorkshopBorderGenerator.Tag = "BorderBatchWindow.NodeDetection.WorkshopBorderGenerator:";
            this.lbWorkshopBorderGenerator.Text = "WorkshopBorderGenerator:";
            // 
            // cbWorkshopKeywordBorderGenerator
            // 
            this.cbWorkshopKeywordBorderGenerator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopKeywordBorderGenerator.Location = new System.Drawing.Point(6, 36);
            this.cbWorkshopKeywordBorderGenerator.Name = "cbWorkshopKeywordBorderGenerator";
            this.cbWorkshopKeywordBorderGenerator.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopKeywordBorderGenerator.TabIndex = 13;
            // 
            // lblWorkshopMarkerLink
            // 
            this.lblWorkshopMarkerLink.Location = new System.Drawing.Point(6, 60);
            this.lblWorkshopMarkerLink.Name = "lblWorkshopMarkerLink";
            this.lblWorkshopMarkerLink.Size = new System.Drawing.Size(306, 17);
            this.lblWorkshopMarkerLink.TabIndex = 12;
            this.lblWorkshopMarkerLink.Tag = "BorderBatchWindow.NodeDetection.WorkshopMarkerLink:";
            this.lblWorkshopMarkerLink.Text = "WorkshopMarkerLink:";
            // 
            // cbWorkshopKeywordBorderLink
            // 
            this.cbWorkshopKeywordBorderLink.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorkshopKeywordBorderLink.Location = new System.Drawing.Point(6, 80);
            this.cbWorkshopKeywordBorderLink.Name = "cbWorkshopKeywordBorderLink";
            this.cbWorkshopKeywordBorderLink.Size = new System.Drawing.Size(306, 21);
            this.cbWorkshopKeywordBorderLink.TabIndex = 18;
            // 
            // cbRestrictWorkshopForms
            // 
            this.cbRestrictWorkshopForms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRestrictWorkshopForms.Checked = true;
            this.cbRestrictWorkshopForms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRestrictWorkshopForms.Location = new System.Drawing.Point(0, 0);
            this.cbRestrictWorkshopForms.Name = "cbRestrictWorkshopForms";
            this.cbRestrictWorkshopForms.Size = new System.Drawing.Size(672, 30);
            this.cbRestrictWorkshopForms.TabIndex = 14;
            this.cbRestrictWorkshopForms.Text = "Restrict to:\nYo momma";
            this.cbRestrictWorkshopForms.UseVisualStyleBackColor = true;
            // 
            // tbEditorIDsWorkshopBorderStatic
            // 
            this.tbEditorIDsWorkshopBorderStatic.Location = new System.Drawing.Point(168, 120);
            this.tbEditorIDsWorkshopBorderStatic.Name = "tbEditorIDsWorkshopBorderStatic";
            this.tbEditorIDsWorkshopBorderStatic.Size = new System.Drawing.Size(256, 20);
            this.tbEditorIDsWorkshopBorderStatic.TabIndex = 19;
            this.tbEditorIDsWorkshopBorderStatic.Text = "{mod}_{name}_WorkshopBorder";
            // 
            // lblEditorIDsWorkshopBorderStatic
            // 
            this.lblEditorIDsWorkshopBorderStatic.Location = new System.Drawing.Point(3, 123);
            this.lblEditorIDsWorkshopBorderStatic.Name = "lblEditorIDsWorkshopBorderStatic";
            this.lblEditorIDsWorkshopBorderStatic.Size = new System.Drawing.Size(160, 17);
            this.lblEditorIDsWorkshopBorderStatic.TabIndex = 18;
            this.lblEditorIDsWorkshopBorderStatic.Tag = "CustomDataWindow.Tab.EditorIDs.Workshop.BorderStatic:";
            this.lblEditorIDsWorkshopBorderStatic.Text = "Border Static";
            // 
            // CustomData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 375);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(680, 399);
            this.Name = "CustomData";
            this.Tag = "CustomDataWindow.Title";
            this.Text = "CustomData";
            this.TopMost = true;
            this.WindowPanel.ResumeLayout(false);
            this.tcCustomData.ResumeLayout(false);
            this.tpEditorIDs.ResumeLayout(false);
            this.tpEditorIDs.PerformLayout();
            this.gbEditorIDsLegend.ResumeLayout(false);
            this.gbEditorIDsWorkshop.ResumeLayout(false);
            this.gbEditorIDsWorkshop.PerformLayout();
            this.tpForms.ResumeLayout(false);
            this.gbWorkshopContainers.ResumeLayout(false);
            this.gbWorkshopContainerFilter.ResumeLayout(false);
            this.gbWorkshopNodeDetection.ResumeLayout(false);
            this.gbWorkshopNodeDetectionLocationRefs.ResumeLayout(false);
            this.gbWorkshopNodeDetectionStaticMarkers.ResumeLayout(false);
            this.gbWorkshopNodeDetectionKeywords.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
