/*
 * SyncObjectTool.cs
 *
 * Render window object selection child tool window.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GUIBuilder.Windows.RenderChild
{
    
    /// <summary>
    /// Description of SyncObjectTool.
    /// </summary>
    public partial class SyncObjectTool<TSync> : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( false ); } }
        
        string _XmlNodeName = null;
        public string XmlNodeName { get{ return _XmlNodeName; } }
        
        bool onLoadComplete = false;
        public bool OnLoadComplete { get { return onLoadComplete; } }

        Size _ExpandedSize;
        
        Engine.Plugin.Interface.ISyncedGUIList<TSync> _ISyncedList = null;
        Engine.Plugin.Forms.Worldspace _Worldspace;

        IEnableControlForm _parent;

        public SyncObjectTool( IEnableControlForm parent, string xmlNodeName, string titleTranslationKey, Engine.Plugin.Interface.ISyncedGUIList<TSync> ISyncedList = null, Type syncedEditorFormType = null )
        {
            _parent = parent;
            InitializeComponent();
            this.SuspendLayout();

            _XmlNodeName = xmlNodeName;
            this.Tag = titleTranslationKey;
            _ISyncedList = ISyncedList;
            
            lvSyncObjects.SyncedEditorFormType              = syncedEditorFormType;
            lvSyncObjects.OnSetSyncObjectsThreadComplete    += OnSyncObjectsThreadComplete;

            this.Load       += new System.EventHandler( this.OnClientLoad );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.OnClientClosing );
            this.OnSetEnableState += OnClientSetEnableState;

            this.Activated  += new System.EventHandler( this.OnClientActivated );
            this.Deactivate += new System.EventHandler( this.OnClientDeactivate );

            this.ResumeLayout( false );
        }
        
        void OnClientLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.OnFormLoad() :: Start" );
            this.Translate( true );
            
            this.Location   = GodObject.XmlConfig.ReadLocation( this );
            this.Size       = GodObject.XmlConfig.ReadSize( this );
            _ExpandedSize   = this.Size;

            if( _ISyncedList != null )
                _ISyncedList.ObjectDataChanged += OnSyncedListChanged;
            
            this.ResizeEnd  += new System.EventHandler( this.OnClientResizeEnd );
            this.Move       += new System.EventHandler( this.OnClientMove );

            onLoadComplete = true;
        }
        
        void OnClientClosing( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.OnFormClosing() :: Start" );
            onLoadComplete = false;
            
            if( _ISyncedList != null )
                _ISyncedList.ObjectDataChanged -= OnSyncedListChanged;
            
        }
        
        public bool SetEnableState( object sender, bool enable )
        {
            if( this.InvokeRequired )
                return (bool)this.Invoke( (Func<bool>)delegate() { return SetEnableState( sender, enable ); }, null );
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
                !lvSyncObjects.IsSyncObjectsThreadRunning;
            return enabled;
        }

        #region Common Form Xml Events

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
            if( !OnLoadComplete )  return;
            GodObject.XmlConfig.WriteSize( this );
            _ExpandedSize = this.Size;
        }

        #endregion

        #region Sync Objects

        void OnSyncObjectsThreadComplete( GUIBuilder.Windows.Controls.SyncedListView<TSync> sender )
        {
            // Try to enable the Render Window, it will try to enable all child tool windows (such as this one)
            _parent.SetEnableState( this, true );
            //SetEnableState( sender, true );
        }

        void UpdateSyncedList( Engine.Plugin.Forms.Worldspace worldspace )
        {
            DebugLog.WriteLine( string.Format( "worldspace ? {0}", worldspace == null ? "null" : worldspace.ToString() ), true );
            SyncObjects = worldspace == null ? null : _ISyncedList.FindAllInWorldspace( worldspace );
        }
        
        void OnSyncedListChanged( object sender, EventArgs e )
        {
            UpdateSyncedList( _Worldspace );
        }
        
        public Engine.Plugin.Forms.Worldspace Worldspace
        {
            get
            {
                return _Worldspace;
            }
            set
            {
                DebugLog.WriteLine( string.Format( "worldspace ? {0}", this.TypeFullName(), value == null ? "null" : value.ToString() ), true );
                _Worldspace = value;
                UpdateSyncedList( _Worldspace );
            }
        }
        
        public List<TSync> SyncObjects
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: SyncObjects_Get()", this.FullTypeName() ) );
                return lvSyncObjects.SyncObjects;
            }
            set
            {
                //DebugLog.Write( string.Format( "\n{0} :: SelectedSyncObjects_Set() :: value ? {1}", this.FullTypeName(), value == null ? "false" : "true" ) );
                lvSyncObjects.SyncObjects = value;
            }
        }
        
        public List<TSync> SelectedSyncObjects
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: SelectedSyncObjects_Get()", this.FullTypeName() ) );
                return lvSyncObjects.GetSelectedSyncObjects();
            }
        }
        
        public List<TSync> VisibleSyncObjects
        {
            get { return lvSyncObjects.GetVisibleSyncObjects(); }
        }
        
        public bool AnythingSelected
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: AnythingSelected_Get()", this.FullTypeName() ) );
                return lvSyncObjects.AnythingSelected;
            }
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
