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
    public partial class SyncedFormEditor<TSync> : Form, GodObject.XmlConfig.IXmlConfiguration
        where TSync : class, Engine.Plugin.Interface.ISyncedGUIObject
    {
        
        #region Form Xml
        
        const string XmlKeyPrefix = "FormEditorWindow_";
        
        [Browsable( false )]
        public GodObject.XmlConfig.IXmlConfiguration XmlParent
        { get { return null; } }
        
        [Browsable( false )]
        public string XmlKey
        { get { return XmlKeyPrefix + this.Text; } }
        
        [Browsable( false )]
        public string        XmlPath                     { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
        
        #endregion
        
        public TSync SyncObject = null;
        
        public SyncedFormEditor( string title, TSync syncobject )
        {
            InitializeComponent();
            this.Text = title;
            SyncObject = syncobject;
        }
        
        public SyncedFormEditor( TSync syncobject )
        {
            InitializeComponent();
            SyncObject = syncobject;
        }
        
        public SyncedFormEditor()
        {
            InitializeComponent();
        }
        
        #region Common SyncObjectGUIElement Form Controls and Events
        
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        Size _ExpandedSize;
        bool _AllowResizing = false;
        bool _ShrinkOnDeactivate = false;
        bool onLoadComplete = false;
        
        #region Common Form Xml Events
        
        void OnFormLoad( object sender, EventArgs e )
        {
            this.Location = GodObject.XmlConfig.ReadPoint( XmlPath, XmlLocation, this.Location );
            if( _AllowResizing )
                this.Size = GodObject.XmlConfig.ReadSize( XmlPath, XmlSize, this.Size );
            _ExpandedSize = this.Size;
            MoveButtons();
            
            if( SyncObject != null )
            {
                tbFormID.Text = SyncObject.GetFormID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired ).ToString( "X8" );
                tbEditorID.Text = SyncObject.GetEditorID( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                SetFormFields();
            }
            
            onLoadComplete = true;
        }
        
        void OverrideSize( Size size )
        {
            this.ResizeEnd -= OnFormResizeEnd;
            this.Size = size;
            MoveButtons();
            this.ResizeEnd += OnFormResizeEnd;
        }
        
        void MoveButtons()
        {
            // Move the buttons to the proper place cuz Windows si derm
            btnCancel.Location = new Point(
                this.Size.Width - 84,
                this.Size.Height - 48 );
            btnApply.Location = new Point(
                btnCancel.Location .X - 81,
                btnCancel.Location.Y );
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
            GodObject.XmlConfig.WritePoint( XmlPath, XmlLocation, this.Location, true );
        }
        
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( XmlPath, XmlSize, this.Size, true );
            _ExpandedSize = this.Size;
            MoveButtons();
        }
        
        void btnApplyClick( object sender, EventArgs e )
        {
            SyncObject.SupressObjectDataChangedEvents();
            SyncObject.SetEditorID( Engine.Plugin.TargetHandle.Working, tbEditorID.Text );
            ApplyFormChanges();
            SyncObject.ResumeObjectDataChangedEvents( true );
        }
        
        void btnCancelClick( object sender, EventArgs e )
        {
            this.Close();
        }
        
        #endregion
        
        #region Common Form Control Properties
        
        public bool AllowResizing
        {
            get{ return _AllowResizing; }
            set
            {
                _AllowResizing = value;
                this.FormBorderStyle = value
                    ? FormBorderStyle.SizableToolWindow
                    : FormBorderStyle.FixedToolWindow;
                if( value )
                    this.ResizeEnd += OnFormResizeEnd;
                else
                    this.ResizeEnd -= OnFormResizeEnd;
            }
        }
        
        public bool ShrinkOnDeactivate
        {
            get{ return _ShrinkOnDeactivate; }
            set
            {
                _ShrinkOnDeactivate = value;
                if( value )
                {
                    this.Activated += OnActivated;
                    this.Deactivate += OnDeactivate;
                }
                else
                {
                    this.Activated -= OnActivated;
                    this.Deactivate -= OnDeactivate;
                }
            }
        }
        
        #endregion
        
        #endregion
        
        protected virtual void ApplyFormChanges()
        {}
        
        protected virtual void SetFormFields()
        {}
        
    }
}
