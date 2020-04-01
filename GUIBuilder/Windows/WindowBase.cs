/*
 * Created by SharpDevelop.
 * Date: 05/01/2020
 * Time: 5:09 PM
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Base "Client Window" class centralizing common features (translating controls, saving/loading window position, size, global window enable states, etc)
    /// </summary>
    public partial class WindowBase : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {

        bool translateForm = false;

        bool onLoadComplete = false;
        public bool OnLoadComplete {  get{ return onLoadComplete; } }

        [Browsable(false)]
        public EventHandler     ClientLoad = null;

        public WindowBase()
        {
            cTor( false );
        }
        
        public WindowBase( bool translate )
        {
            cTor( translate );
        }
        
        void cTor( bool translate )
        {
            onLoadComplete = false;

            InitializeComponent();

            this.SuspendLayout();

            translateForm       = translate;

            this.Load           += new System.EventHandler( this.WindowBase_OnLoad );
            this.FormClosing    += new System.Windows.Forms.FormClosingEventHandler( this.IEnableControlForm_OnFormClosing );

            this.ResumeLayout( false );
        }


        #region WindowBase_OnLoad


        void WindowBase_OnLoad( object sender, EventArgs e )
        {
            SetEnableState( sender, false );

            this.Location       = GodObject.XmlConfig.ReadLocation( this );
            this.Size           = GodObject.XmlConfig.ReadSize( this );

            if( translateForm )
                this.Translate( true );

            ClientLoad?.Invoke( sender, e );

            // Handle size and location events after OnLoad has finished resizing and moving
            this.ResizeEnd      += new System.EventHandler( this.IXmlConfiguration_OnFormResizeEnd );
            this.Move           += new System.EventHandler( this.IXmlConfiguration_OnFormMove );

            onLoadComplete = true;
            SetEnableState( sender, true );
        }


        #endregion


        #region GUIBuilder.Windows.IEnableControlForm


        #region Internal

        
        void IEnableControlForm_OnFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Windows.ClearWindow( this.GetType() );
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


        #endregion



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
        }

        #endregion


        #region Interface

        public const string XmlNode_WindowSuffix = "Window";

        public virtual GodObject.XmlConfig.IXmlConfiguration XmlParent
        {
            get { return null; }
        }

        public virtual string XmlNodeName
        {
            get
            {
                return this.GetType().Name() + XmlNode_WindowSuffix;
            }
        }

        #endregion


        #endregion


    }
}
