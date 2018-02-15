/*
 * HelpAboutForm.cs
 *
 * The form displaying the product version, copyright, etc.
 *
 * User: 1000101
 * Date: 02/12/2017
 * Time: 10:58 AM
 * 
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Border_Builder
{
    /// <summary>
    /// Description of HelpAboutForm.
    /// </summary>
    public partial class HelpAboutForm : Form
    {
        
        public bbMain fMain;
        
        public HelpAboutForm()
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
            fMain.SetEnableState( false );
            lblVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        void HelpAboutFormFormClosed( object sender, FormClosedEventArgs e )
        {
            fMain.SetEnableState( true );
        }
        
        void LlblLicenseLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( llblLicense.Text );
        }
        
        void LlblBorderBuilderLinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            OpenLinkURL( llblBorderBuilder.Text );
        }
        
    }
}
