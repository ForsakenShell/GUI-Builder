/*
 * Options.cs
 *
 * Main GUIBuilder configuration options.
 *
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of Options.
    /// </summary>
    public partial class Options : Form, GodObject.XmlConfig.IXmlConfiguration
    {
        
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        bool onLoadComplete = false;
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent
        { get { return null; } }
        
        public string XmlKey
        { get { return this.Name; } }
        
        public string        XmlPath                     { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
        
        public Options()
        {
            InitializeComponent();
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            this.Location = GodObject.XmlConfig.ReadPoint( XmlPath, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlPath, XmlSize, this.Size );
            
            lvAlwaysSelectMasters.SyncObjects = GodObject.Master.Files;
            UpdateCSColors();
            
            this.BringToFront();
            onLoadComplete = true;
        }
        
        void OnFormClosed( object sender, FormClosedEventArgs e )
        {
            GodObject.Windows.SetOptionsWindow( null, false );
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
        }
        
        #region Conflict Status
        
        void UpdateCSColors()
        {
            tbCSInvalid.BackColor = Engine.Plugin.ConflictStatus.Invalid.GetConflictStatusBackColor();
            tbCSUneditable.BackColor = Engine.Plugin.ConflictStatus.Uneditable.GetConflictStatusBackColor();
            tbCSNewForm.BackColor = Engine.Plugin.ConflictStatus.NewForm.GetConflictStatusBackColor();
            tbCSNoConflict.BackColor = Engine.Plugin.ConflictStatus.NoConflict.GetConflictStatusBackColor();
            tbCSOverrideInAncestor.BackColor = Engine.Plugin.ConflictStatus.OverrideInAncestor.GetConflictStatusBackColor();
            tbCSOverrideInWorkingFile.BackColor = Engine.Plugin.ConflictStatus.OverrideInWorkingFile.GetConflictStatusBackColor();
            tbCSRequiresOverride.BackColor = Engine.Plugin.ConflictStatus.RequiresOverride.GetConflictStatusBackColor();
        }
        
        #endregion
        
    }
}
