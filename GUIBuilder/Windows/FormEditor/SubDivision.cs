/*
 * SubDivision.cs
 *
 * SubDivision editor window.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GUIBuilder.Windows.FormEditor
{
    /// <summary>
    /// Description of SubDivision.
    /// </summary>
    public partial class SubDivision : SyncedFormEditor<AnnexTheCommonwealth.SubDivision>
    {
        
        bool onLoadComplete = false;
        
        protected override string XmlFormNodeName { get { return "SubDivision"; } }
        
        List<Engine.Plugin.Forms.Location> _Locations;
        
        public SubDivision( AnnexTheCommonwealth.SubDivision syncobject ) : base( syncobject )
        {
            InitializeComponent();
        }
        
        void SubDivisionLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            
            cbRequirementsRelationship.Items.Clear();
            cbRequirementsRelationship.Items.AddRange(
                new string [] {
                    "FormEditor.SubDivision.Requirements.AllRelationships".Translate(),
                    "FormEditor.SubDivision.Requirements.AnyRelationship".Translate()
                } );
            cbRequirementsRelationship.SelectedIndex = SyncObject.GetRelationshipsAnyAll( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            cbRequirementsQuest.Items.Clear();
            cbRequirementsQuest.Items.AddRange(
                new string [] {
                    "FormEditor.SubDivision.Requirements.AllQuests".Translate(),
                    "FormEditor.SubDivision.Requirements.AnyQuest".Translate()
                } );
            cbRequirementsQuest.SelectedIndex = SyncObject.GetQuestStagesAnyAll( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            cbRequirementsRelationshipsAndQuests.Items.Clear();
            cbRequirementsRelationshipsAndQuests.Items.AddRange(
                new string [] {
                    "FormEditor.SubDivision.Requirements.RelationshipsAndQuests".Translate(),
                    "FormEditor.SubDivision.Requirements.RelationshipsOrQuests".Translate()
                } );
            cbRequirementsRelationshipsAndQuests.SelectedIndex = SyncObject.GetRelationshipsAndQuests( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            cbLocation.Items.Clear();
            cbLocation.Items.Add( string.Format( " [{0}] ", "DropdownSelectNone".Translate() ) );
            var selectedIndex = 0;
            var cLocations = GodObject.Plugin.Data.Root.GetCollection<Engine.Plugin.Forms.Location>( true, true, true );
            if( cLocations != null )
            {
                
                _Locations = cLocations.ToList<Engine.Plugin.Forms.Location>();
                if( !_Locations.NullOrEmpty() )
                {
                    var formID = SyncObject == null
                        ? Engine.Plugin.Constant.FormID_Invalid
                        : SyncObject.GetMyLocation( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    var count = _Locations.Count;
                    for( int index = 0; index < count; index++ )
                    {
                        var location = _Locations[ index ];
                        var lFormID = location.GetFormID( Engine.Plugin.TargetHandle.Master );
                        var lEditorID = location.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        var id = string.Format( "{0} - \"{1}\"", lFormID.ToString( "X8" ), lEditorID );
                        cbLocation.Items.Add( id );
                        if( lFormID == formID )
                            selectedIndex = 1 + index;
                    }
                }
            }
            cbLocation.SelectedIndex = selectedIndex;
            
            UpdateLocationDisplayFields();
            onLoadComplete  = true;
        }
        
        void cbLocationSelectedIndexChanged( object sender, EventArgs e )
        {
            if( !onLoadComplete ) return;
            UpdateLocationDisplayFields();
        }
        
        void UpdateLocationDisplayFields()
        {
            var index = cbLocation.SelectedIndex;
            var location = index > 0 ? _Locations[ index - 1 ] : null;
            if( location == null )
            {
                pnLocation.Enabled = false;
                tbLocationFormID.Text = "";
                tbLocationEditorID.Text = "";
                tbLocationName.Text = "";
                return;
            }
            tbLocationFormID.Text = location.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" );
            tbLocationEditorID.Text = location.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            tbLocationName.Text = location.GetFullName( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            pnLocation.Enabled = true;
        }
        
    }
    
}
