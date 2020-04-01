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
        private System.Windows.Forms.TextBox tbLocationName;
        private System.Windows.Forms.Label lbLocationName;
        private System.Windows.Forms.GroupBox gbRequirementConditions;
        private System.Windows.Forms.ComboBox cbRequirementsRelationshipsAndQuests;
        private System.Windows.Forms.Label lbRequirementsRelationshipAndQuests;
        private System.Windows.Forms.ComboBox cbRequirementsQuest;
        private System.Windows.Forms.Label lbRequirementsQuests;
        private System.Windows.Forms.ComboBox cbRequirementsRelationship;
        private System.Windows.Forms.Label lbRequirementsRelationships;
        private System.Windows.Forms.TabControl tcRequirements;
        private System.Windows.Forms.TabPage tpLocations;
        private System.Windows.Forms.TabPage tpQuests;
        private System.Windows.Forms.TabPage tpRelationships;
        private System.Windows.Forms.TabPage tpReferences;
        private System.Windows.Forms.TabPage tpEncounterZones;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader LCTNFormID;
        private System.Windows.Forms.ColumnHeader LCTNPlugin;
        private System.Windows.Forms.GroupBox gbLocationRequirements;
        private System.Windows.Forms.Button btnLocationRemove;
        private System.Windows.Forms.Button btnLocationAdd;
        private System.Windows.Forms.ColumnHeader LCTNName;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox cbLocationhideUntilValid;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        
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
            this.tbLocationName = new System.Windows.Forms.TextBox();
            this.lbLocationName = new System.Windows.Forms.Label();
            this.tbLocationEditorID = new System.Windows.Forms.TextBox();
            this.tbLocationFormID = new System.Windows.Forms.TextBox();
            this.lblLocationFormID = new System.Windows.Forms.Label();
            this.lblLocationEditorID = new System.Windows.Forms.Label();
            this.cbLocation = new System.Windows.Forms.ComboBox();
            this.gbRequirementConditions = new System.Windows.Forms.GroupBox();
            this.cbRequirementsQuest = new System.Windows.Forms.ComboBox();
            this.cbRequirementsRelationship = new System.Windows.Forms.ComboBox();
            this.cbRequirementsRelationshipsAndQuests = new System.Windows.Forms.ComboBox();
            this.lbRequirementsQuests = new System.Windows.Forms.Label();
            this.lbRequirementsRelationships = new System.Windows.Forms.Label();
            this.lbRequirementsRelationshipAndQuests = new System.Windows.Forms.Label();
            this.tcRequirements = new System.Windows.Forms.TabControl();
            this.tpLocations = new System.Windows.Forms.TabPage();
            this.gbLocationRequirements = new System.Windows.Forms.GroupBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cbLocationhideUntilValid = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnLocationRemove = new System.Windows.Forms.Button();
            this.btnLocationAdd = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.LCTNName = new System.Windows.Forms.ColumnHeader();
            this.LCTNFormID = new System.Windows.Forms.ColumnHeader();
            this.LCTNPlugin = new System.Windows.Forms.ColumnHeader();
            this.tpEncounterZones = new System.Windows.Forms.TabPage();
            this.tpQuests = new System.Windows.Forms.TabPage();
            this.tpRelationships = new System.Windows.Forms.TabPage();
            this.tpReferences = new System.Windows.Forms.TabPage();
            this.gbLocation.SuspendLayout();
            this.pnLocation.SuspendLayout();
            this.gbRequirementConditions.SuspendLayout();
            this.tcRequirements.SuspendLayout();
            this.tpLocations.SuspendLayout();
            this.gbLocationRequirements.SuspendLayout();
            this.SuspendLayout();
            // 
            // WindowPanel
            // 
            this.WindowPanel.Controls.Add( this.tcRequirements );
            this.WindowPanel.Controls.Add( this.gbRequirementConditions );
            this.WindowPanel.Controls.Add( this.gbLocation );
            this.WindowPanel.Size = new System.Drawing.Size( 616, 314 );
            // 
            // gbLocation
            // 
            this.gbLocation.Controls.Add(this.pnLocation);
            this.gbLocation.Controls.Add(this.cbLocation);
            this.gbLocation.Location = new System.Drawing.Point(3, 82);
            this.gbLocation.Name = "gbLocation";
            this.gbLocation.Size = new System.Drawing.Size(296, 102);
            this.gbLocation.TabIndex = 5;
            this.gbLocation.TabStop = false;
            this.gbLocation.Tag = "FormEditor.SubDivision.Location:";
            this.gbLocation.Text = "Location:";
            // 
            // pnLocation
            // 
            this.pnLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnLocation.Controls.Add(this.tbLocationName);
            this.pnLocation.Controls.Add(this.lbLocationName);
            this.pnLocation.Controls.Add(this.tbLocationEditorID);
            this.pnLocation.Controls.Add(this.tbLocationFormID);
            this.pnLocation.Controls.Add(this.lblLocationFormID);
            this.pnLocation.Controls.Add(this.lblLocationEditorID);
            this.pnLocation.Location = new System.Drawing.Point(6, 27);
            this.pnLocation.Name = "pnLocation";
            this.pnLocation.Size = new System.Drawing.Size(284, 66);
            this.pnLocation.TabIndex = 8;
            // 
            // tbLocationName
            // 
            this.tbLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLocationName.Location = new System.Drawing.Point(64, 44);
            this.tbLocationName.Name = "tbLocationName";
            this.tbLocationName.Size = new System.Drawing.Size(220, 20);
            this.tbLocationName.TabIndex = 9;
            // 
            // lbLocationName
            // 
            this.lbLocationName.Location = new System.Drawing.Point(0, 47);
            this.lbLocationName.Name = "lbLocationName";
            this.lbLocationName.Size = new System.Drawing.Size(100, 23);
            this.lbLocationName.TabIndex = 8;
            this.lbLocationName.Tag = "Location.Name:";
            this.lbLocationName.Text = "Name:";
            // 
            // tbLocationEditorID
            // 
            this.tbLocationEditorID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLocationEditorID.Location = new System.Drawing.Point(64, 22);
            this.tbLocationEditorID.Name = "tbLocationEditorID";
            this.tbLocationEditorID.Size = new System.Drawing.Size(220, 20);
            this.tbLocationEditorID.TabIndex = 7;
            // 
            // tbLocationFormID
            // 
            this.tbLocationFormID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLocationFormID.BackColor = System.Drawing.SystemColors.Control;
            this.tbLocationFormID.Enabled = false;
            this.tbLocationFormID.Location = new System.Drawing.Point(64, 0);
            this.tbLocationFormID.Name = "tbLocationFormID";
            this.tbLocationFormID.Size = new System.Drawing.Size(220, 20);
            this.tbLocationFormID.TabIndex = 5;
            // 
            // lblLocationFormID
            // 
            this.lblLocationFormID.Location = new System.Drawing.Point(0, 3);
            this.lblLocationFormID.Name = "lblLocationFormID";
            this.lblLocationFormID.Size = new System.Drawing.Size(100, 23);
            this.lblLocationFormID.TabIndex = 4;
            this.lblLocationFormID.Tag = "Form.FormID:";
            this.lblLocationFormID.Text = "FormID:";
            // 
            // lblLocationEditorID
            // 
            this.lblLocationEditorID.Location = new System.Drawing.Point(0, 25);
            this.lblLocationEditorID.Name = "lblLocationEditorID";
            this.lblLocationEditorID.Size = new System.Drawing.Size(100, 23);
            this.lblLocationEditorID.TabIndex = 6;
            this.lblLocationEditorID.Tag = "Form.EditorID:";
            this.lblLocationEditorID.Text = "EditorID:";
            // 
            // cbLocation
            // 
            this.cbLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLocation.FormattingEnabled = true;
            this.cbLocation.Location = new System.Drawing.Point(70, 0);
            this.cbLocation.Name = "cbLocation";
            this.cbLocation.Size = new System.Drawing.Size(220, 21);
            this.cbLocation.TabIndex = 3;
            // 
            // gbRequirementConditions
            // 
            this.gbRequirementConditions.Controls.Add(this.cbRequirementsQuest);
            this.gbRequirementConditions.Controls.Add(this.cbRequirementsRelationship);
            this.gbRequirementConditions.Controls.Add(this.cbRequirementsRelationshipsAndQuests);
            this.gbRequirementConditions.Controls.Add(this.lbRequirementsQuests);
            this.gbRequirementConditions.Controls.Add(this.lbRequirementsRelationships);
            this.gbRequirementConditions.Controls.Add(this.lbRequirementsRelationshipAndQuests);
            this.gbRequirementConditions.Location = new System.Drawing.Point(3, 190);
            this.gbRequirementConditions.Name = "gbRequirementConditions";
            this.gbRequirementConditions.Size = new System.Drawing.Size(296, 97);
            this.gbRequirementConditions.TabIndex = 6;
            this.gbRequirementConditions.TabStop = false;
            this.gbRequirementConditions.Tag = "FormEditor.SubDivision.RequirementConditions:";
            this.gbRequirementConditions.Text = "Requirement Conditions:";
            // 
            // cbRequirementsQuest
            // 
            this.cbRequirementsQuest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRequirementsQuest.FormattingEnabled = true;
            this.cbRequirementsQuest.Location = new System.Drawing.Point(124, 42);
            this.cbRequirementsQuest.Name = "cbRequirementsQuest";
            this.cbRequirementsQuest.Size = new System.Drawing.Size(166, 21);
            this.cbRequirementsQuest.TabIndex = 12;
            // 
            // cbRequirementsRelationship
            // 
            this.cbRequirementsRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRequirementsRelationship.FormattingEnabled = true;
            this.cbRequirementsRelationship.Location = new System.Drawing.Point(124, 19);
            this.cbRequirementsRelationship.Name = "cbRequirementsRelationship";
            this.cbRequirementsRelationship.Size = new System.Drawing.Size(166, 21);
            this.cbRequirementsRelationship.TabIndex = 10;
            // 
            // cbRequirementsRelationshipsAndQuests
            // 
            this.cbRequirementsRelationshipsAndQuests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRequirementsRelationshipsAndQuests.FormattingEnabled = true;
            this.cbRequirementsRelationshipsAndQuests.Location = new System.Drawing.Point(124, 65);
            this.cbRequirementsRelationshipsAndQuests.Name = "cbRequirementsRelationshipsAndQuests";
            this.cbRequirementsRelationshipsAndQuests.Size = new System.Drawing.Size(166, 21);
            this.cbRequirementsRelationshipsAndQuests.TabIndex = 8;
            // 
            // lbRequirementsQuests
            // 
            this.lbRequirementsQuests.Location = new System.Drawing.Point(6, 45);
            this.lbRequirementsQuests.Name = "lbRequirementsQuests";
            this.lbRequirementsQuests.Size = new System.Drawing.Size(130, 23);
            this.lbRequirementsQuests.TabIndex = 11;
            this.lbRequirementsQuests.Tag = "FormEditor.SubDivision.RequirementsQuests:";
            this.lbRequirementsQuests.Text = "Quests:";
            // 
            // lbRequirementsRelationships
            // 
            this.lbRequirementsRelationships.Location = new System.Drawing.Point(6, 22);
            this.lbRequirementsRelationships.Name = "lbRequirementsRelationships";
            this.lbRequirementsRelationships.Size = new System.Drawing.Size(130, 23);
            this.lbRequirementsRelationships.TabIndex = 9;
            this.lbRequirementsRelationships.Tag = "FormEditor.SubDivision.RequirementsRelationships:";
            this.lbRequirementsRelationships.Text = "Relationships:";
            // 
            // lbRequirementsRelationshipAndQuests
            // 
            this.lbRequirementsRelationshipAndQuests.Location = new System.Drawing.Point(6, 68);
            this.lbRequirementsRelationshipAndQuests.Name = "lbRequirementsRelationshipAndQuests";
            this.lbRequirementsRelationshipAndQuests.Size = new System.Drawing.Size(130, 23);
            this.lbRequirementsRelationshipAndQuests.TabIndex = 7;
            this.lbRequirementsRelationshipAndQuests.Tag = "FormEditor.SubDivision.RequirementsRelationshipsAndQuests:";
            this.lbRequirementsRelationshipAndQuests.Text = "Relationships && Quests:";
            // 
            // tcRequirements
            // 
            this.tcRequirements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcRequirements.Controls.Add(this.tpLocations);
            this.tcRequirements.Controls.Add(this.tpEncounterZones);
            this.tcRequirements.Controls.Add(this.tpQuests);
            this.tcRequirements.Controls.Add(this.tpRelationships);
            this.tcRequirements.Controls.Add(this.tpReferences);
            this.tcRequirements.Location = new System.Drawing.Point(305, 4);
            this.tcRequirements.Name = "tcRequirements";
            this.tcRequirements.SelectedIndex = 0;
            this.tcRequirements.Size = new System.Drawing.Size(308, 283);
            this.tcRequirements.TabIndex = 0;
            // 
            // tpLocations
            // 
            this.tpLocations.Controls.Add(this.gbLocationRequirements);
            this.tpLocations.Controls.Add(this.btnLocationRemove);
            this.tpLocations.Controls.Add(this.btnLocationAdd);
            this.tpLocations.Controls.Add(this.listView1);
            this.tpLocations.Location = new System.Drawing.Point(4, 22);
            this.tpLocations.Name = "tpLocations";
            this.tpLocations.Padding = new System.Windows.Forms.Padding(3);
            this.tpLocations.Size = new System.Drawing.Size(300, 257);
            this.tpLocations.TabIndex = 0;
            this.tpLocations.Tag = "FormEditor.SubDivision.Tab.Locations";
            this.tpLocations.Text = "Locations";
            this.tpLocations.UseVisualStyleBackColor = true;
            // 
            // gbLocationRequirements
            // 
            this.gbLocationRequirements.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLocationRequirements.Controls.Add(this.checkBox4);
            this.gbLocationRequirements.Controls.Add(this.checkBox3);
            this.gbLocationRequirements.Controls.Add(this.checkBox2);
            this.gbLocationRequirements.Controls.Add(this.checkBox1);
            this.gbLocationRequirements.Controls.Add(this.cbLocationhideUntilValid);
            this.gbLocationRequirements.Controls.Add(this.textBox3);
            this.gbLocationRequirements.Controls.Add(this.textBox2);
            this.gbLocationRequirements.Controls.Add(this.label2);
            this.gbLocationRequirements.Controls.Add(this.label3);
            this.gbLocationRequirements.Controls.Add(this.comboBox1);
            this.gbLocationRequirements.Location = new System.Drawing.Point(0, 56);
            this.gbLocationRequirements.Name = "gbLocationRequirements";
            this.gbLocationRequirements.Size = new System.Drawing.Size(300, 171);
            this.gbLocationRequirements.TabIndex = 3;
            this.gbLocationRequirements.TabStop = false;
            this.gbLocationRequirements.Tag = "FormEditor.SubDivision.Location:";
            this.gbLocationRequirements.Text = "Location:";
            // 
            // checkBox4
            // 
            this.checkBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox4.Location = new System.Drawing.Point(6, 150);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(284, 18);
            this.checkBox4.TabIndex = 22;
            this.checkBox4.Tag = "FormEditor.SubDivision.Tab.Location.OverrideParent";
            this.checkBox4.Text = "Override Encounter Zone && Location Parent";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox3.Location = new System.Drawing.Point(6, 132);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(284, 18);
            this.checkBox3.TabIndex = 21;
            this.checkBox3.Tag = "FormEditor.SubDivision.Tab.Location.Workshop";
            this.checkBox3.Text = "Set \"Workshop\" on Encounter Zone";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.Location = new System.Drawing.Point(6, 114);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(284, 18);
            this.checkBox2.TabIndex = 20;
            this.checkBox2.Tag = "FormEditor.SubDivision.Tab.Location.NeverReset";
            this.checkBox2.Text = "Set \"Never Resets\" on Encounter Zone";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.Location = new System.Drawing.Point(6, 96);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(284, 18);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Tag = "FormEditor.SubDivision.Tab.Location.MustClear";
            this.checkBox1.Text = "Must clear to annex";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // cbLocationhideUntilValid
            // 
            this.cbLocationhideUntilValid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLocationhideUntilValid.Location = new System.Drawing.Point(6, 78);
            this.cbLocationhideUntilValid.Name = "cbLocationhideUntilValid";
            this.cbLocationhideUntilValid.Size = new System.Drawing.Size(284, 18);
            this.cbLocationhideUntilValid.TabIndex = 18;
            this.cbLocationhideUntilValid.Tag = "FormEditor.SubDivision.Tab.Location.Hide";
            this.cbLocationhideUntilValid.Text = "Hide sub-division until conditions met";
            this.cbLocationhideUntilValid.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(70, 27);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(220, 20);
            this.textBox3.TabIndex = 17;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(70, 49);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(220, 20);
            this.textBox2.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 11;
            this.label2.Tag = "Form.FormID:";
            this.label2.Text = "FormID:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 13;
            this.label3.Tag = "Form.EditorID:";
            this.label3.Text = "Plugin:";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(70, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(220, 21);
            this.comboBox1.TabIndex = 10;
            // 
            // btnLocationRemove
            // 
            this.btnLocationRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocationRemove.Location = new System.Drawing.Point(225, 233);
            this.btnLocationRemove.Name = "btnLocationRemove";
            this.btnLocationRemove.Size = new System.Drawing.Size(75, 24);
            this.btnLocationRemove.TabIndex = 2;
            this.btnLocationRemove.Text = "Remove";
            this.btnLocationRemove.UseVisualStyleBackColor = true;
            // 
            // btnLocationAdd
            // 
            this.btnLocationAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocationAdd.Location = new System.Drawing.Point(144, 233);
            this.btnLocationAdd.Name = "btnLocationAdd";
            this.btnLocationAdd.Size = new System.Drawing.Size(75, 24);
            this.btnLocationAdd.TabIndex = 1;
            this.btnLocationAdd.Text = "Add";
            this.btnLocationAdd.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LCTNName,
            this.LCTNFormID,
            this.LCTNPlugin});
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(300, 50);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // LCTNName
            // 
            this.LCTNName.DisplayIndex = 1;
            this.LCTNName.Text = "Name";
            this.LCTNName.Width = 117;
            // 
            // LCTNFormID
            // 
            this.LCTNFormID.DisplayIndex = 0;
            this.LCTNFormID.Text = "FormID";
            this.LCTNFormID.Width = 58;
            // 
            // LCTNPlugin
            // 
            this.LCTNPlugin.Text = "Plugin";
            this.LCTNPlugin.Width = 104;
            // 
            // tpEncounterZones
            // 
            this.tpEncounterZones.Location = new System.Drawing.Point(4, 22);
            this.tpEncounterZones.Name = "tpEncounterZones";
            this.tpEncounterZones.Padding = new System.Windows.Forms.Padding(3);
            this.tpEncounterZones.Size = new System.Drawing.Size(300, 257);
            this.tpEncounterZones.TabIndex = 4;
            this.tpEncounterZones.Tag = "FormEditor.SubDivision.Tab.EncounterZones";
            this.tpEncounterZones.Text = "Encounter Zones";
            this.tpEncounterZones.UseVisualStyleBackColor = true;
            // 
            // tpQuests
            // 
            this.tpQuests.Location = new System.Drawing.Point(4, 22);
            this.tpQuests.Name = "tpQuests";
            this.tpQuests.Padding = new System.Windows.Forms.Padding(3);
            this.tpQuests.Size = new System.Drawing.Size(300, 257);
            this.tpQuests.TabIndex = 1;
            this.tpQuests.Tag = "FormEditor.SubDivision.Tab.Quests";
            this.tpQuests.Text = "Quests";
            this.tpQuests.UseVisualStyleBackColor = true;
            // 
            // tpRelationships
            // 
            this.tpRelationships.Location = new System.Drawing.Point(4, 22);
            this.tpRelationships.Name = "tpRelationships";
            this.tpRelationships.Padding = new System.Windows.Forms.Padding(3);
            this.tpRelationships.Size = new System.Drawing.Size(300, 257);
            this.tpRelationships.TabIndex = 2;
            this.tpRelationships.Tag = "FormEditor.SubDivision.Tab.Relationships";
            this.tpRelationships.Text = "Relationships";
            this.tpRelationships.UseVisualStyleBackColor = true;
            // 
            // tpReferences
            // 
            this.tpReferences.Location = new System.Drawing.Point(4, 22);
            this.tpReferences.Name = "tpReferences";
            this.tpReferences.Padding = new System.Windows.Forms.Padding(3);
            this.tpReferences.Size = new System.Drawing.Size(300, 257);
            this.tpReferences.TabIndex = 3;
            this.tpReferences.Tag = "FormEditor.SubDivision.Tab.References";
            this.tpReferences.Text = "References";
            this.tpReferences.UseVisualStyleBackColor = true;
            // 
            // SubDivision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 314);
            this.MinimumSize = new System.Drawing.Size(624, 338);
            this.Name = "SubDivision";
            this.Tag = "FormEditor.SubDivision.Title";
            this.Text = "Sub-Division";
            this.gbLocation.ResumeLayout(false);
            this.pnLocation.ResumeLayout(false);
            this.pnLocation.PerformLayout();
            this.gbRequirementConditions.ResumeLayout(false);
            this.tcRequirements.ResumeLayout(false);
            this.tpLocations.ResumeLayout(false);
            this.gbLocationRequirements.ResumeLayout(false);
            this.gbLocationRequirements.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
