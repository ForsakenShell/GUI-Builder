/*
 * PluginSelector.cs
 *
 * Plugin selector window for GUIBuilder.
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
    /// Description of PluginSelector.
    /// </summary>
    public partial class PluginSelector : Form, GodObject.XmlConfig.IXmlConfiguration
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return null; } }
        public string XmlNodeName { get{ return "PluginSelector"; } }
        
        const string NodeFormat = "{1} [{0}]";
        const int NodeFilenameTail = 5;
        readonly Color NodeDisabledColor = Color.Gray;
        
        bool OverrideCheckedCheck = false;
        List<Setup.LoadOrderItem> LoadOrder = Setup.GetLoadOrder();
        
        public List<string> SelectedPlugins = null;
        public string WorkingFile = null;
        public bool OpenRenderWindowOnLoad { get { return cbOpenRenderWindowOnLoad.Checked; } }
        
        bool onLoadComplete = false;
        
        public PluginSelector()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            
        }
        
        void PluginSelectorLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            
            tvPlugins.Nodes.Clear();
            
            if( !LoadOrder.NullOrEmpty() )
            {
                for( int i = 0; i < LoadOrder.Count; i++ )
                {
                    var nt = string.Format( NodeFormat, i.ToString( "X2" ), LoadOrder[ i ].Filename );
                    var n = new TreeNode( nt );
                    tvPlugins.Nodes.Add( n );
                    if( !LoadOrder[ i ].Masters.NullOrEmpty() )
                    {
                        foreach( var m in LoadOrder[ i ].Masters )
                        {
                            var nnt = string.Format( NodeFormat, LoadOrder.FindIndex( loi => loi.Filename.InsensitiveInvariantMatch( m ) ).ToString( "X2" ), m );
                            var nn = new TreeNode( nnt );
                            nn.ForeColor = NodeDisabledColor;
                            n.Nodes.Add( nn );
                        }
                    }
                }
            }
            
            foreach( var master in GodObject.Master.Files )
            {
                if( master.AlwaysSelect )
                {
                    foreach( TreeNode node in tvPlugins.Nodes )
                    {
                        var filename = CutOffString( node.Text, NodeFilenameTail );
                        if( filename.InsensitiveInvariantMatch( master.Filename ) )
                            node.Checked = true;
                    }
                }
            }
            
            cbWorkingFile.Items.Clear();
            cbWorkingFile.Items.Add( string.Format( " [{0}] ", "DropdownSelectNone".Translate() ) );
            cbWorkingFile.SelectedIndex = 0;
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
        
        void btnLoadClick( object sender, EventArgs e )
        {
            SelectedPlugins = new List<string>();
            foreach( TreeNode node in tvPlugins.Nodes )
                if( node.Checked )
                    SelectedPlugins.Add( CutOffString( node.Text, NodeFilenameTail ) );
            var working = cbWorkingFile.SelectedIndex <= 0 ? null : (string)cbWorkingFile.Items[ cbWorkingFile.SelectedIndex ];
            WorkingFile = working == null ? null : CutOffString( working, NodeFilenameTail );
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        void btnCancelClick( object sender, EventArgs e )
        {
            WorkingFile = null;
            SelectedPlugins = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        void tvPluginsBeforeCheckOrSelect( object sender, TreeViewCancelEventArgs e )
        {
            if( ( !onLoadComplete )||( OverrideCheckedCheck ) ) return;
            if( e.Node.ForeColor == NodeDisabledColor ) e.Cancel = true;
        }
        
        void UpdateRootCheckState( string key, bool state )
        {
            foreach( TreeNode node in tvPlugins.Nodes )
            {
                if( key.InsensitiveInvariantMatch( node.Text ) )
                {
                    node.Checked = state;
                    UpdateChildCheckState( key, state );
                }
            }
        }
        
        void tvPluginsAfterCheck( object sender, TreeViewEventArgs e )
        {
            if( ( !onLoadComplete )||( OverrideCheckedCheck ) ) return;
            OverrideCheckedCheck = true;
            
            try{
            var key = e.Node.Text;
            var state = e.Node.Checked;
            UpdateChildCheckState( key, state );
            if( state )
            {
                if( e.Node.Nodes.Count > 0 )
                    foreach( TreeNode child in e.Node.Nodes )
                        UpdateRootCheckState( child.Text, state );
            }
            else
            {
                foreach( TreeNode node in tvPlugins.Nodes )
                {
                    if( node.Nodes.Count > 0 )
                    {
                        foreach( TreeNode child in node.Nodes )
                        {
                            if( key.InsensitiveInvariantMatch( child.Text ) )
                            {
                                node.Checked = false;
                                UpdateChildCheckState( node.Text, false );
                            }
                        }
                    }
                }
            }
            
            var lastSelectedWorking = cbWorkingFile.SelectedIndex < 1 ? null : (string)cbWorkingFile.Items[ cbWorkingFile.SelectedIndex ];
            cbWorkingFile.Items.Clear();
            cbWorkingFile.Items.Add( string.Format( " [{0}] ", "DropdownSelectNone".Translate() ) );
            foreach( TreeNode node in tvPlugins.Nodes )
            {
                var f = CutOffString( node.Text, NodeFilenameTail );
                if( ( node.Checked )&&( !f.EndsWith( ".esm", StringComparison.InvariantCultureIgnoreCase ) ) )
                    cbWorkingFile.Items.Add( node.Text );
            }
            
            var workingSelected = 0;
            if( lastSelectedWorking == null )
            {
                // No working file was selected, pick the last one to load but keep searching backwards until
                // one selected requiring ATC is found and pick it instead
                for( int i = tvPlugins.Nodes.Count - 1; i >= 0; i-- )
                {
                    var node = tvPlugins.Nodes[ i ];
                    if( node.Checked )
                    {
                        // Pick it if nothing is selected yet
                        if( lastSelectedWorking == null )
                            lastSelectedWorking = node.Text;
                        
                        if( GodObject.Master.Loaded( GodObject.Master.AnnexTheCommonwealth ) )
                        {
                            // Prefer an ATC dependant file
                            var p = LoadOrder.Find( f => f.Filename.InsensitiveInvariantMatch( CutOffString( node.Text, NodeFilenameTail ) ) );
                            if( !p.Masters.NullOrEmpty() )
                            {
                                foreach( var m in p.Masters ) // Gourmetrix (discord) reported an null reference error on this line, wtf?
                                {
                                    if( m.InsensitiveInvariantMatch( GodObject.Master.AnnexTheCommonwealth.Filename ) )
                                    {
                                        lastSelectedWorking = node.Text;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if( lastSelectedWorking != null )
            {
                // Update the working file dropdown with the previously (or automagically) selected plugin
                for( int i = 0; i < cbWorkingFile.Items.Count; i++ )
                {
                    if( lastSelectedWorking.InsensitiveInvariantMatch( (string)cbWorkingFile.Items[ i ] ) )
                    {
                        workingSelected = i;
                        break;
                    }
                }
            }
            cbWorkingFile.SelectedIndex = workingSelected;
            btnLoad.Enabled = workingSelected > 0;
            
            OverrideCheckedCheck = false;
            }
            catch( Exception ex )
            {
                Console.WriteLine( string.Format( "An exception has occured...no REALLY!\n{0}\n{1}\n{2}\n{3}", e.Action.ToString(), e.Node.ToStringNullSafe(), ex.ToString(), ex.StackTrace ) );
            }
        }
        
        void CbWorkingFileSelectedIndexChanged( object sender, EventArgs e )
        {
            if( ( !onLoadComplete )||( OverrideCheckedCheck ) ) return;
            btnLoad.Enabled = cbWorkingFile.SelectedIndex > 0;
        }
        
        void UpdateChildCheckState( string key, bool state )
        {
            foreach( TreeNode node in tvPlugins.Nodes )
                if( node.Nodes.Count > 0 )
                    foreach( TreeNode child in node.Nodes )
                        if( key.InsensitiveInvariantMatch( child.Text ) )
                            child.Checked = state;
        }
        
        string CutOffString( string s, int charsToCutOff )
        {
            var r = s.Substring( 0, s.Length - charsToCutOff );
            //DebugLog.WriteLine( new [] { this.FullTypeName(), "CutOffString()", "\"" + s + "\"", "\"" + r + "\"" } );
            return r;
        }
        
        #region Override (Ignore) Close Button
        
        // Supress the close button on the plugin selector, close with the load/cancel buttons.
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
