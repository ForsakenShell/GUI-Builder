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
    /// Description of WindowBase.
    /// </summary>
    public partial class WindowBase : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {

        bool translateForm = false;

        bool onLoadComplete = false;
        public bool OnLoadComplete {  get{ return onLoadComplete; } }

        [Browsable(true)]
        [Category("Client Events"), Description("Called at the end of WindowBase_OnLoad, set this instead of Form.Load")]
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
            InitializeComponent();

            this.SuspendLayout();

            translateForm       = translate;

            this.Location       = GodObject.XmlConfig.ReadLocation( this );
            this.Size           = GodObject.XmlConfig.ReadSize( this );

            this.ResizeEnd      += new System.EventHandler( this.IXmlConfiguration_OnFormResizeEnd );
            this.Move           += new System.EventHandler( this.IXmlConfiguration_OnFormMove );

            this.FormClosing    += new System.Windows.Forms.FormClosingEventHandler( this.IEnableControlForm_OnFormClosing );

            this.Load           += new System.EventHandler( this.WindowBase_OnLoad );

            this.ResumeLayout( false );
        }


        #region WindowBase_OnFormLoad


        void WindowBase_OnLoad( object sender, EventArgs e )
        {
            SetEnableState( false );

            if( translateForm )
                this.Translate( true );

            ClientLoad?.Invoke( sender, e );

            onLoadComplete = true;
            SetEnableState( true );
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

        [Browsable(true)]
        [Category("Client Events"), Description("This event is fired when SetEnableState is called on the Form.")]
        public event GUIBuilder.Windows.SetEnableStateHandler  OnSetEnableState;

        /// <summary>
        /// Enable or disable this windows main panel.
        /// </summary>
        /// <param name="enabled">Enable state to set</param>
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate () { SetEnableState( enabled ); }, null );
                return;
            }

            // Enable the main panel
            if( WindowPanel != null )
                WindowPanel.Enabled = enabled;

            OnSetEnableState?.Invoke( enabled );
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

        public virtual GodObject.XmlConfig.IXmlConfiguration XmlParent
        {
            get { return null; }
        }

        public virtual string XmlNodeName
        {
            get { return null; }
        }

        #endregion


        #endregion


    }
}
