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
    public partial class About : Form, GodObject.XmlConfig.IXmlConfiguration, IEnableControlForm
    {
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get{ return null; } }
        public string XmlNodeName { get{ return "AboutWindow"; } }
        
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
            
            this.Location = GodObject.XmlConfig.ReadLocation( this );
            this.Size = GodObject.XmlConfig.ReadSize( this );
            
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            onLoadComplete = true;
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
        }
        
        void OnFormClosed( object sender, FormClosedEventArgs e )
        {
            GodObject.Windows.SetWindow<About>( null, false );
        }

        public void SetEnableState( bool enabled ) { }

        void LlblLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( llblLicense.Text );
        }
        
        void lblAuthorLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( lblAuthor.Text );
        }
        
    }
}
