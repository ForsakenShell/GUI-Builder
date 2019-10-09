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
        
        List<Engine.Plugin.Forms.Location> _Locations;
        
        public SubDivision( AnnexTheCommonwealth.SubDivision syncobject ) : base( syncobject )
        {
            InitializeComponent();
        }
        
        void SubDivisionLoad( object sender, EventArgs e )
        {
            cbLocation.Items.Clear();
            var cLocations = GodObject.Plugin.Data.Root.GetCollection<Engine.Plugin.Forms.Location>( true, true );
            if( cLocations != null )
            {
                
                _Locations = cLocations.ToList<Engine.Plugin.Forms.Location>();
                if( !_Locations.NullOrEmpty() )
                {
                    var formID = SyncObject == null
                        ? Engine.Plugin.Constant.FormID_Invalid
                        : SyncObject.myLocation;
                    var selectedIndex = -1;
                    var count = _Locations.Count;
                    for( int index = 0; index < count; index++ )
                    {
                        var location = _Locations[ index ];
                        cbLocation.Items.Add( location.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) );
                        if( location.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ) == formID )
                            selectedIndex = index;
                    }
                }
            }
        }
        
        void cbLocationSelectedIndexChanged( object sender, EventArgs e )
        {
            var index = cbLocation.SelectedIndex;
            var location = index >= 0 ? _Locations[ index ] : null;
            if( location == null )
            {
                pnLocation.Enabled = false;
                tbLocationFormID.Text = "";
                tbLocationEditorID.Text = "";
                return;
            }
            tbLocationFormID.Text = location.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" );
            tbLocationEditorID.Text = location.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
            
            pnLocation.Enabled = true;
        }
        
    }
    
}
