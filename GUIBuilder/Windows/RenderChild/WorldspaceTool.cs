/*
 * WorldspaceTool.cs
 *
 * Render window worldspace child tool window.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GUIBuilder.Windows.RenderChild
{
    
    /// <summary>
    /// Description of WorldspaceTool.
    /// </summary>
    public partial class WorldspaceTool : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( false ); } }
        public string XmlNodeName { get{ return "Worldspaces"; } }

        bool onLoadComplete = false;
        public bool OnLoadComplete { get { return onLoadComplete; } }

        Size _ExpandedSize;
        
        Engine.Plugin.Forms.Worldspace _SelectedWorldspace = null;

        IEnableControlForm _parent;

        public WorldspaceTool( IEnableControlForm parent )
        {
            _parent = parent;

            InitializeComponent();
            this.SuspendLayout();

            this.lvWorldspaces.OnSetSyncObjectsThreadComplete += OnSyncWorldspacesThreadComplete;

            this.Load       += new System.EventHandler( this.OnClientLoad );
            this.OnSetEnableState += OnClientSetEnableState;

            this.Activated  += new System.EventHandler( this.OnClientActivated );
            this.Deactivate += new System.EventHandler( this.OnClientDeactivate );

            ResetGUIElements();

            this.ResumeLayout();
        }
        
        void OnClientLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.OnFormLoad() :: Start" );
            this.Translate( true );
            
            this.Location   = GodObject.XmlConfig.ReadLocation( this );
            this.Size       = GodObject.XmlConfig.ReadSize( this );
            _ExpandedSize   = this.Size;

            this.Move       += new System.EventHandler( this.OnClientMove );
            this.ResizeEnd  += new System.EventHandler( this.OnClientResizeEnd );

            onLoadComplete = true;
        }
        
        void OverrideSize( Size size )
        {
            this.ResizeEnd -= OnClientResizeEnd;
            this.Size = size;
            this.ResizeEnd += OnClientResizeEnd;
        }
        
        void OnClientActivated( object sender, EventArgs e )
        {
            OverrideSize( _ExpandedSize );
        }
        
        void OnClientDeactivate( object sender, EventArgs e )
        {
            OverrideSize( this.MinimumSize );
        }
        
        void OnClientMove( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteLocation( this );
        }
        
        void OnClientResizeEnd( object sender, EventArgs e )
        {
            if( !OnLoadComplete ) return;
            GodObject.XmlConfig.WriteSize( this );
            _ExpandedSize = this.Size;
        }
        
        public void ResetGUIElements()
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.ResetGUIElements()" );
            // Worldspace controls
            tbWorldspaceFormID.Clear();
            tbWorldspaceEditorID.Clear();
            tbWorldspaceGridBottomX.Clear();
            tbWorldspaceGridBottomY.Clear();
            tbWorldspaceGridTopX.Clear();
            tbWorldspaceGridTopY.Clear();
            tbWorldspaceHeightmapTexture.Clear();
            tbWorldspaceWaterHeightsTexture.Clear();
            tbWorldspaceMapHeightMax.Clear();
            tbWorldspaceMapHeightMin.Clear();
        }
        
        void UpdateGUIElements()
        {
            ResetGUIElements();
            var rw = GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( false );
            
            var worldspace = _SelectedWorldspace;
            DebugLog.WriteLine( string.Format( "worldspace ? {0}", worldspace == null ? "null" : worldspace.ToString() ), true );
            if( worldspace != null )
            {
                var poolEntry = worldspace.PoolEntry;
                var mapData = worldspace.MapData;
                tbWorldspaceFormID.Text = worldspace.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" );
                tbWorldspaceEditorID.Text = worldspace.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
                tbWorldspaceMapHeightMax.Text = poolEntry.MaxHeight.ToString( "n6" );
                tbWorldspaceMapHeightMin.Text = poolEntry.MinHeight.ToString( "n6" );
                var cellNW = mapData.GetCellNW( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                var cellSE = mapData.GetCellSE( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                tbWorldspaceGridTopX.Text = cellNW.X.ToString();
                tbWorldspaceGridTopY.Text = cellNW.Y.ToString();
                tbWorldspaceGridBottomX.Text = cellSE.X.ToString();
                tbWorldspaceGridBottomY.Text = cellSE.Y.ToString();
                tbWorldspaceHeightmapTexture.Text = poolEntry.LandHeights_Texture_File;
                tbWorldspaceWaterHeightsTexture.Text = poolEntry.WaterHeights_Texture_File;
            }
            rw.UpdateSettlementObjectChildWindowContentsForWorldspace( worldspace );
            rw.TryUpdateRenderWindow( true );
        }

        public bool SetEnableState( object sender, bool enable )
        {
            if( this.InvokeRequired )
                return (bool)this.Invoke( (Func<bool>)delegate () { return SetEnableState( sender, enable ); }, null );
            
            bool tryEnable = OnLoadComplete && enable;
            bool enabled = OnSetEnableState != null
                ? OnSetEnableState( sender, tryEnable )
                : tryEnable;
            if( sender != _parent )
                enabled = _parent.SetEnableState( this, enabled );
            pnWindow.Enabled = enabled;
            return enabled;
        }

        public event GUIBuilder.Windows.SetEnableStateHandler  OnSetEnableState;

        /// <summary>
        /// Handle window specific global enable/disable events.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        bool OnClientSetEnableState( object sender, bool enable )
        {
            var enabled =
                enable &&
                !lvWorldspaces.IsSyncObjectsThreadRunning;
            return enabled;
        }
        #region Sync Objects

        void OnSyncWorldspacesThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<Engine.Plugin.Forms.Worldspace> sender )
        {
            SetEnableState( sender, true );
        }

        public List<Engine.Plugin.Forms.Worldspace> SyncObjects
        {
            get { return lvWorldspaces.SyncObjects; }
            set { lvWorldspaces.SyncObjects = value; }
        }
        
        public Engine.Plugin.Forms.Worldspace SelectedWorldspace
        {
            get{ return _SelectedWorldspace; }
        }
        
        void lvWorldspacesItemSelectionChanged( object sender, EventArgs e )
        {
            _SelectedWorldspace = null;
            var lie = e as ListViewItemSelectionChangedEventArgs;
            if( lie != null )
                _SelectedWorldspace = lvWorldspaces.SyncObjectFromListViewItem( lie.Item );
            DebugLog.WriteLine( string.Format( "worldspace ? {0}", this.TypeFullName(), _SelectedWorldspace == null ? "null" : _SelectedWorldspace.ToString() ), true );
            UpdateGUIElements();
        }
        
        #endregion
        
        #region Override (Ignore) Close Button
        
        // Supress the close button on the render windows tool windows, close with the render window.
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

