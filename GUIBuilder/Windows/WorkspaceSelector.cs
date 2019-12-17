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
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
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
                        }
                        else
                            DebugLog.WriteError( "GUIBuilder.Windows.WorkspaceSelector", "WorkspaceSelectorLoad()", string.Format( "new Workspace( \"{0}\" ) = null!", wsName ) );
                    }
                }
            }
            
            onLoadComplete = true;
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
