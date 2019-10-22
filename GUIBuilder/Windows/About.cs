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
    public partial class About : Form
    {
        
        const string XmlNode = "AboutWindow";
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        bool onLoadComplete = false;
        
        public About()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        void OpenLinkURL( string url )
        {
            System.Diagnostics.Process.Start( url );
        }
        
        void HelpAboutFormLoad( object sender, EventArgs e )
        {
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadPoint( XmlNode, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlNode, XmlSize, this.Size );
            
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            onLoadComplete = true;
        }
        
        void OnFormMove( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WritePoint( XmlNode, XmlLocation, this.Location, true );
        }
        
        void OnFormResizeEnd( object sender, EventArgs e )
        {
            if( !onLoadComplete )
                return;
            GodObject.XmlConfig.WriteSize( XmlNode, XmlSize, this.Size, true );
        }
        
        void OnFormClosed( object sender, FormClosedEventArgs e )
        {
            GodObject.Windows.SetAboutWindow( null, false );
        }
        
        void LlblLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( llblLicense.Text );
        }
        
        void lblAuthorLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( lblAuthor.Text );
        }
        
        void AboutWindowFormClosing( object sender, FormClosingEventArgs e )
        {
            GodObject.Windows.SetAboutWindow( null, false );
        }
        
        
    }
}
