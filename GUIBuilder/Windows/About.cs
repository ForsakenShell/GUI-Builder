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
    /// Description of About.
    /// </summary>
    public partial class About : WindowBase
    {


        /// <summary>
        /// Use GodObject.Windows.GetWindow<About>() to create this Window
        /// </summary>
        public About() : base( true )
        {
            InitializeComponent();
        }


        #region GodObject.XmlConfig.IXmlConfiguration


        public override string XmlNodeName { get { return "AboutWindow"; } }


        #endregion


        void OpenLinkURL( string url )
        {
            System.Diagnostics.Process.Start( url );
        }
        
        void About_OnLoad( object sender, EventArgs e )
        {
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        

        void linkLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( linkLicense.Text );
        }
        
        void linkAuthorLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( linkAuthor.Text );
        }
        
    }
}
