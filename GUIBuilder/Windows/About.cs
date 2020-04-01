/*
 * About.cs
 *
 * The window displaying the product version, copyright, etc.
 *
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    
    /// <summary>
    /// Use GodObject.Windows.GetWindow<About>() to create this Window
    /// </summary>
    public partial class About : WindowBase
    {

        public About() : base( true )
        {
            InitializeComponent();
            this.SuspendLayout();

            this.ClientLoad += new System.EventHandler( this.OnClientLoad );

            this.linkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.OnLicenseLinkClicked );
            this.linkAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.OnAuthorLinkClicked );

            this.ResumeLayout( false );
        }

        void OnClientLoad( object sender, EventArgs e )
        {
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Link Events

        void OpenLinkURL( string url )
        {
            System.Diagnostics.Process.Start( url );
        }

        void OnLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( linkLicense.Text );
        }
        
        void OnAuthorLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( linkAuthor.Text );
        }

        #endregion

    }
}
