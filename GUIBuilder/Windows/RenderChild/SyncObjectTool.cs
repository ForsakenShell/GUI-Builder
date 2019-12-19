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
    public partial class SyncObjectTool<TSync> : Form, GodObject.XmlConfig.IXmlConfiguration
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return GodObject.Windows.GetWindow<GUIBuilder.Windows.Render>( false ); } }
        
        string _XmlNodeName = null;
        public string XmlNodeName { get{ return _XmlNodeName; } }
        
        bool onLoadComplete = false;
        Size _ExpandedSize;
        
        Engine.Plugin.Interface.ISyncedGUIList<TSync> _ISyncedList = null;
        Engine.Plugin.Forms.Worldspace _Worldspace;
        
        public SyncObjectTool( string xmlNodeName, string titleTranslationKey, Engine.Plugin.Interface.ISyncedGUIList<TSync> ISyncedList = null, Type syncedEditorFormType = null )
        {
            InitializeComponent();
            _XmlNodeName = xmlNodeName;
            this.Tag = titleTranslationKey;
            _ISyncedList = ISyncedList;
            lvSyncObjects.SyncedEditorFormType = syncedEditorFormType;
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.OnFormLoad() :: Start" );
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            _ExpandedSize = this.Size;
            
            if( _ISyncedList != null )
                _ISyncedList.ObjectDataChanged += OnSyncedListChanged;
            
            onLoadComplete = true;
        }
        
        void OnFormClosing( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindowForm.OnFormClosing() :: Start" );
            onLoadComplete = false;
            
            if( _ISyncedList != null )
                _ISyncedList.ObjectDataChanged -= OnSyncedListChanged;
            
        }
        
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            pnWindow.Enabled = enabled;
        }
        
        #region Common Form Xml Events
        
        void OverrideSize( Size size )
        {
            this.ResizeEnd -= OnFormResizeEnd;
            this.Size = size;
            this.ResizeEnd += OnFormResizeEnd;
        }
        
        void OnActivated( object sender, EventArgs e )
        {
            OverrideSize( _ExpandedSize );
        }
        
        void OnDeactivate( object sender, EventArgs e )
        {
            OverrideSize( this.MinimumSize );
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
            _ExpandedSize = this.Size;
        }
        
        #endregion
        
        #region Sync Objects
        
        void UpdateSyncedList( Engine.Plugin.Forms.Worldspace worldspace )
        {
            DebugLog.WriteLine( string.Format( "{0} :: UpdateSyncedList() :: worldspace ? {1}", this.GetType().ToString(), worldspace == null ? "null" : worldspace.ToString() ) );
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
                DebugLog.WriteLine( string.Format( "{0} :: Worldspace :: worldspace ? {1}", this.GetType().ToString(), value == null ? "null" : value.ToString() ) );
                _Worldspace = value;
                UpdateSyncedList( _Worldspace );
            }
        }
        
        //public List<Engine.Plugin.Form> SyncObjects
        public List<TSync> SyncObjects
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: SyncObjects_Get()", this.GetType().ToString() ) );
                return lvSyncObjects.SyncObjects;
            }
            set
            {
                //DebugLog.Write( string.Format( "\n{0} :: SelectedSyncObjects_Set() :: value ? {1}", this.GetType().ToString(), value == null ? "false" : "true" ) );
                lvSyncObjects.SyncObjects = value;
            }
        }
        
        //public List<Engine.Plugin.Form> SelectedSyncObjects
        public List<TSync> SelectedSyncObjects
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: SelectedSyncObjects_Get()", this.GetType().ToString() ) );
                return lvSyncObjects.GetSelectedSyncObjects();
            }
        }
        
        //public List<Engine.Plugin.Form> VisibleSyncObjects
        public List<TSync> VisibleSyncObjects
        {
            get { return lvSyncObjects.GetVisibleSyncObjects(); }
        }
        
        public bool AnythingSelected
        {
            get
            {
                //DebugLog.Write( string.Format( "\n{0} :: AnythingSelected_Get()", this.GetType().ToString() ) );
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
