/*
 * WorkspaceSelector.cs
 *
 * Workspace selector window for GUIBuilder.
 *
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XeLib;
using XeLib.API;


namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of WorkspaceSelector.
    /// </summary>
    public partial class WorkspaceSelector : Form, GodObject.XmlConfig.IXmlConfiguration
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return null; } }
        public string XmlNodeName { get{ return "WorkspaceSelector"; } }
        
        const string NodeFormat = "{1} [{0}]";
        const int NodeFilenameTail = 5;
        readonly Color NodeDisabledColor = Color.Gray;
        
        List<Setup.LoadOrderItem> LoadOrder = Setup.GetLoadOrder();
        
        public string SelectedWorkspace = null;
        
        bool onLoadComplete = false;
        
        public WorkspaceSelector()
        {
            InitializeComponent();
        }
        
        void WorkspaceSelectorLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            
            tvWorkspaces.Nodes.Clear();
            var workspaces = GodObject.Paths.Workspaces;
            
            if( !workspaces.NullOrEmpty() )
            {
                for( int i = 0; i < workspaces.Length; i++ )
                {
                    var wsName = workspaces[ i ];
                    var n = new TreeNode( wsName );
                    tvWorkspaces.Nodes.Add( n );
                    if( !LoadOrder.NullOrEmpty() )
                    {
                        var ws = new Workspace( wsName );
                        if( ws != null )
                        {
                            {
                                var plugins = ws.PluginNames;
                                var pn = new TreeNode( "WorkspaceSelector.PluginsNode".Translate() );
                                pn.ForeColor = NodeDisabledColor;
                                n.Nodes.Add( pn );
                                foreach( var p in plugins )
                                {
                                    var nnt = string.Format( NodeFormat, LoadOrder.FindIndex( loi => loi.Filename.InsensitiveInvariantMatch( p ) ).ToString( "X2" ), p );
                                    var nn = new TreeNode( nnt );
                                    nn.ForeColor = NodeDisabledColor;
                                    //nn.BeforeSelect
                                    pn.Nodes.Add( nn );
                                }
                            }
                            {
                                var workingFile = ws.WorkingFile;
                                if( !string.IsNullOrEmpty( workingFile ) )
                                {
                                    var pn = new TreeNode(
                                        string.Format(
                                            "{0} : {1}",
                                            "WorkspaceSelector.WorkingFileNode".Translate(),
                                            string.Format(
                                                NodeFormat,
                                                LoadOrder.FindIndex( loi => loi.Filename.InsensitiveInvariantMatch( workingFile ) ).ToString( "X2" ),
                                                workingFile ) ) );
                                    pn.ForeColor = NodeDisabledColor;
                                    n.Nodes.Add( pn );
                                }
                            }
                            {
                                var pn = new TreeNode(
                                    string.Format(
                                        "{0} : {1}",
                                        "PluginSelectorWindow.OpenRenderWindow".Translate(),
                                        ws.OpenRenderWindowOnLoad ? "true" : "false" ) );
                                pn.ForeColor = NodeDisabledColor;
                                n.Nodes.Add( pn );
                            }
                            AddFormIdentifier( ws, n, GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderGenerator , "BorderBatchWindow.NodeDetection.WorkshopBorderGenerator"      );
                            AddFormIdentifier( ws, n, GUIBuilder.WorkshopBatch.WSDS_KYWD_BorderLink      , "BorderBatchWindow.NodeDetection.WorkshopMarkerLink"           );
                            AddFormIdentifier( ws, n, GUIBuilder.WorkshopBatch.WSDS_STAT_TerrainFollowing, "BorderBatchWindow.NodeDetection.BorderMarkerTerrainFollowing" );
                            AddFormIdentifier( ws, n, GUIBuilder.WorkshopBatch.WSDS_STAT_ForcedZ         , "BorderBatchWindow.NodeDetection.BorderMarkerForcedZ"          );
                            {
                                var workshopIdentifiers = ws.WorkshopWorkbenches;
                                if( !workshopIdentifiers.NullOrEmpty() )
                                {
                                    var wsnp = new TreeNode( "WorkspaceSelector.WorkshopWorkbenches".Translate() );
                                    wsnp.ForeColor = NodeDisabledColor;
                                    n.Nodes.Add( wsnp );
                                    foreach( var identifier in workshopIdentifiers )
                                        AddFormIdentifier( identifier, wsnp, "WorkspaceSelector.Container" );
                                }
                            }
                        }
                        else
                            DebugLog.WriteError( string.Format( "new Workspace( \"{0}\" ) = null!", wsName ) );
                    }
                }
            }
            
            onLoadComplete = true;
        }

        void AddFormIdentifier( Workspace workspace, TreeNode parentNode, string identifierKey, string translationKey )
        {
            AddFormIdentifier( workspace.GetFormIdentifier( identifierKey, false ), parentNode, translationKey );
        }
        
        void AddFormIdentifier( Workspace.FormIdentifier identifier, TreeNode parentNode, string translationKey )
        {
            if( ( identifier == null )||( parentNode == null ) )
                return;
            
            var identifierNode = new TreeNode( translationKey.Translate() );
            identifierNode.ForeColor = NodeDisabledColor;
            
            var formIDText = string.Format( "{0} : {1}", "Form.FormID".Translate(), identifier.FormID.ToString( "X8" ) );
            var formIDNode = new TreeNode( formIDText );
            formIDNode.ForeColor = NodeDisabledColor;
            
            var filenameText = string.Format( "{0} : {1}", "Form.Filename".Translate(), identifier.Filename );
            var filenameNode = new TreeNode( filenameText );
            filenameNode.ForeColor = NodeDisabledColor;

            identifierNode.Nodes.Add( formIDNode );
            identifierNode.Nodes.Add( filenameNode );
            parentNode.Nodes.Add( identifierNode );
        }

        void OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteLocation( this );
        }
        
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( this );
        }
        
        TreeNode RootNode( TreeNode n )
        {
            return n == null
                ? null
                : n.Parent == null
                ? n
                : RootNode( n.Parent );
        }
        
        void btnCancelClick( object sender, EventArgs e )
        {
            SelectedWorkspace = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        void btnLoadClick( object sender, EventArgs e )
        {
            var selected = RootNode( tvWorkspaces.SelectedNode );
            SelectedWorkspace = selected == null ? null : selected.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        #region Override (Ignore) Close Button
        
        // Supress the close button on the workspace selector, close with the load/cancel buttons.
        // https://stackoverflow.com/questions/13247629/disabling-a-windows-form-closing-button
        const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams mdiCp = base.CreateParams;
                mdiCp.ClassStyle = mdiCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return mdiCp;
            }
        }
       #endregion
        
    }
}
