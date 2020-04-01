/*
 * SyncedFormEditor.cs
 *
 * Base window class for form editing.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace GUIBuilder.Windows.FormEditor
{
    /// <summary>
    /// Description of SyncedFormEditor.
    /// </summary>
    public partial class SyncedFormEditor<TSync> : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {

        bool translateForm = false;

        bool onLoadComplete = false;
        public bool OnLoadComplete { get { return onLoadComplete; } }

        [Browsable(false)]
        public EventHandler     ClientLoad = null;

        public TSync SyncObject = null;
        
        public SyncedFormEditor( string title, TSync syncObject, bool translate = true )
        {
            INTERNAL_Constructor( title, syncObject, translate );
        }
        
        public SyncedFormEditor( TSync syncObject, bool translate = true )
        {
            INTERNAL_Constructor( null, syncObject, translate );
        }
        
        public SyncedFormEditor( bool translate = true )
        {
            INTERNAL_Constructor( null, null, translate );
        }

        void INTERNAL_Constructor( string title, TSync syncObject, bool translate )
        {
            onLoadComplete = false;

            InitializeComponent();

            this.SuspendLayout();

            translateForm = translate;

            if( !string.IsNullOrEmpty( title ) )
                this.Text = title;

            SyncObject = syncObject;

            this.Load += new System.EventHandler( this.SyncedFormEditor_OnLoad );

            this.btnCancel.Click += new System.EventHandler( this.OnCancelClick );
            this.btnApply.Click += new System.EventHandler( this.OnApplyClick );

            this.ResumeLayout( false );
        }

        #region Common SyncObjectGUIElement Form Controls and Events

        Size _ExpandedSize;
        bool _AllowResizing = false;
        bool _ShrinkOnDeactivate = false;
        
        #region Common Form Xml Events
        
        void SyncedFormEditor_OnLoad( object sender, EventArgs e )
        {
            SetEnableState( sender, false );

            this.Location       = GodObject.XmlConfig.ReadLocation( this );

            if( _AllowResizing )
                this.Size       = GodObject.XmlConfig.ReadSize( this );
            _ExpandedSize       = this.Size;
            MoveButtons();

            if( translateForm )
                this.Translate( true );

            if( SyncObject != null )
            {
                tbFormID.Text   = SyncObject.GetFormID( Engine.Plugin.TargetHandle.Master ).ToString( "X8" );
                tbEditorID.Text = SyncObject.GetEditorID( Engine.Plugin.TargetHandle.LastValid );
                SetFormFields();
            }

            ClientLoad?.Invoke( sender, e );

            this.Move           += new System.EventHandler( this.IXmlConfiguration_OnFormMove );
            
            SetAllowResizing( _AllowResizing );
            SetShrinkOnDeactivate( _ShrinkOnDeactivate );

            onLoadComplete = true;
            SetEnableState( sender, true );
        }

        void OnClientActivated( object sender, EventArgs e )
        {
            OverrideSize( _ExpandedSize );
        }
        
        void OnClientDeactivate( object sender, EventArgs e )
        {
            OverrideSize( this.MinimumSize );
        }
        
        void OnApplyClick( object sender, EventArgs e )
        {
            SyncObject.SupressObjectDataChangedEvents();
            SyncObject.SetEditorID( Engine.Plugin.TargetHandle.Working, tbEditorID.Text );
            ApplyFormChanges();
            SyncObject.ResumeObjectDataChangedEvents( true );
        }
        
        void OnCancelClick( object sender, EventArgs e )
        {
            this.Close();
        }

        #endregion

        #region Interface

        /// <summary>
        /// Client SetEnableState handler - You must be prepared to enable the window when ready - or not!
        /// When used properly, this will give the user windows that will show all the controls in their
        /// default states.  WindowBase.WindowPanel (which all controls should be on) will be disabled which
        /// will block the user access to the controls but they can move the window around and the UI thread
        /// won't be blocked.  This means that long-running threads which are processing can still give
        /// feedback through the Main Window status bar while they prepare the data for the UI.  When those
        /// threads are complete, the UI must be enabled again for the user.  Make sure you handle multi-
        /// threading with the power to destroy with respect!
        /// Return false to force the UI to [continue to] be disabled, return the requested state otherwise.
        /// </summary>
        [Browsable(false)]
        public event GUIBuilder.Windows.SetEnableStateHandler  OnSetEnableState;

        /// <summary>
        /// Enable or disable this windows main panel.
        /// </summary>
        /// <param name="enable">Enable state to set</param>
        public bool SetEnableState( object sender, bool enable )
        {
            if( this.InvokeRequired )
                return (bool)this.Invoke( (Func<bool>)delegate () { return SetEnableState( sender, enable ); }, null );

            bool tryEnable = OnLoadComplete && enable;
            bool enabled = OnSetEnableState != null
                ? OnSetEnableState( sender, tryEnable )
                : tryEnable;

            // Enable the main panel
            if( WindowPanel != null )
                WindowPanel.Enabled = enabled;

            return enabled;
        }

        #endregion


        #region Internal Control setters and getters

        void OverrideSize( Size size )
        {
            if( _AllowResizing )
                this.ResizeEnd -= IXmlConfiguration_OnFormResizeEnd;
            this.Size = size;
            MoveButtons();
            if( _AllowResizing )
                this.ResizeEnd += IXmlConfiguration_OnFormResizeEnd;
        }

        void MoveButtons()
        {
            // Move the buttons to the proper place cuz Windows si derm
            btnCancel.Location = new Point(
                this.Size.Width - 84,
                this.Size.Height - 48 );
            btnApply.Location = new Point(
                btnCancel.Location.X - 81,
                btnCancel.Location.Y );
        }

        void SetAllowResizing( bool value )
        {
            _AllowResizing = value;
            this.FormBorderStyle = value
                ? FormBorderStyle.SizableToolWindow
                : FormBorderStyle.FixedToolWindow;
            if( value )
                this.ResizeEnd += IXmlConfiguration_OnFormResizeEnd;
            else
                this.ResizeEnd -= IXmlConfiguration_OnFormResizeEnd;
        }

        void SetShrinkOnDeactivate( bool value )
        {
            _ShrinkOnDeactivate = value;
            if( value )
            {
                this.Activated += OnClientActivated;
                this.Deactivate += OnClientDeactivate;
            }
            else
            {
                this.Activated -= OnClientActivated;
                this.Deactivate -= OnClientDeactivate;
            }
        }

        #endregion

        #region Common Form Control Properties

        public bool AllowResizing
        {
            get{ return _AllowResizing; }
            set
            {
                SetAllowResizing( value );
            }
        }
        
        public bool ShrinkOnDeactivate
        {
            get{ return _ShrinkOnDeactivate; }
            set
            {
                SetShrinkOnDeactivate( value );
            }
        }
        
        #endregion
        
        #endregion
        
        protected virtual void ApplyFormChanges()
        {}
        
        protected virtual void SetFormFields()
        {}

        #region GodObject.XmlConfig.IXmlConfiguration


        #region Internal

        void IXmlConfiguration_OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete ) return;
            GodObject.XmlConfig.WriteLocation( this );
        }

        void IXmlConfiguration_OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete ) return;
            GodObject.XmlConfig.WriteSize( this );
            _ExpandedSize = this.Size;
            MoveButtons();
        }

        #endregion


        #region Interface

        public const string XmlNode_WindowPrefix = "FormEditorWindow_";

        public virtual GodObject.XmlConfig.IXmlConfiguration XmlParent
        {
            get { return null; }
        }

        public virtual string XmlNodeName
        {
            get
            {
                return XmlNode_WindowPrefix + this.GetType().Name();
            }
        }

        #endregion


        #endregion

    }
}
