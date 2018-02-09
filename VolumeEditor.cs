/*
 * VolumeEditor.cs
 *
 * All this code is to edit build volumes, etc.
 *
 * User: 1000101
 * Date: 09/02/2018
 * Time: 12:35 PM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Border_Builder
{
    /// <summary>
    /// Description of VolumeEditor.
    /// </summary>
    public class VolumeEditor : IDisposable
    {
        
        // Editor mode controls
        
        public enum SelectionMode
        {
            None = 0,
            Vertex,
            Edge
        }
        
        bool editorEnabled;
        bool oldFormKeyPreview; // We're setting KeyPreview to true, don't reset it to false erroneously when editor mode is toggled off 
        bool editorMouseOver;
        SelectionMode editorSelectionMode;
        
        RenderTransform transform;
        Form editorForm;
        PictureBox pbTarget;
        ToolStripStatusLabel editorSelectionModeStatus;
        TextBox editorHotkeyDescriptions;
        
        public VolumeEditor( RenderTransform _transform, Form _editorForm, PictureBox _pbTarget, ToolStripStatusLabel _editorSelectionModeStatus, TextBox _editorHotkeyDescriptions )
        {
            if( (_transform == null )||( _editorForm == null )||( _pbTarget == null )||( _editorSelectionModeStatus == null )||( _editorHotkeyDescriptions == null ) )
                throw new ArgumentNullException();
            
            editorEnabled = false;
            editorMouseOver = false;
            transform = _transform;
            editorForm = _editorForm;
            pbTarget = _pbTarget;
            editorSelectionModeStatus = _editorSelectionModeStatus;
            editorHotkeyDescriptions = _editorHotkeyDescriptions;
        }
        
        #region Semi-Public API:  Destructor & IDispose
        
        // Protect against "double-free" errors caused by combinations of explicit disposal[s] and GC disposal
        bool _disposed = false;
        
        ~VolumeEditor()
        {
            Dispose( false );
        }
        
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
        
        protected virtual void Dispose( bool disposing )
        {
            if( _disposed ) return;
            
            // Remove the hooks
            DisableEditorMode();
            
            // This is no longer a valid state
            _disposed = true;
        }
        
        #endregion
        
        #region Editor Mode State (Enable/Disable)
        
        public bool Enabled { get { return editorEnabled; } }
        
        public void DisableEditorMode()
        {
            if( !editorEnabled )
                return;
            
            transform.AttachedEditor = null;
            mouseSelectionMode = false;
            editorSelectedVertices = null;
            editorEnabled = false;
            
            editorSelectionModeStatus.Text = EditorSelectionModeLabel;
            
            editorForm.KeyPreview = oldFormKeyPreview;
            editorForm.KeyPress -= this.editorMode_KeyPress;
            pbTarget.MouseDown -= this.editorMode_MouseDown;
            pbTarget.MouseUp -= this.editorMode_MouseUp;
            pbTarget.MouseMove -= this.editorMode_MouseMove;
            pbTarget.MouseEnter -= this.editorMode_MouseEnter;
            pbTarget.MouseLeave -= this.editorMode_MouseLeave;
            
            editorForm = null;
            editorSelectionModeStatus = null;
        }
        
        public void EnableEditorMode()
        {
            if( editorEnabled )
            {
                DisableEditorMode();
            }
            
            oldFormKeyPreview = editorForm.KeyPreview;
            editorForm.KeyPreview = true;
            editorForm.KeyPress += this.editorMode_KeyPress;
            pbTarget.MouseDown += this.editorMode_MouseDown;
            pbTarget.MouseUp += this.editorMode_MouseUp;
            pbTarget.MouseMove += this.editorMode_MouseMove;
            pbTarget.MouseEnter += this.editorMode_MouseEnter;
            pbTarget.MouseLeave += this.editorMode_MouseLeave;
            
            editorEnabled = true;
            mouseSelectionMode = false;
            editorSelectedVertices = null;
            editorSelectionModeStatus.Text = EditorSelectionModeLabel;
            
            editorHotkeyDescriptions.Clear();
            foreach( var hk in EditorHotkeys )
                editorHotkeyDescriptions.AppendText( hk.FullDescription + "\n" );
            
            transform.AttachedEditor = this;
        }
        
        #endregion
        
        #region Hotkey Dispatcher, Delegate and, Struct
        
        public delegate bool HotkeyDelegate( object sender, KeyPressEventArgs e );
        
        public struct EditorHotkey
        {
            char[] _validKeys;
            string _showKey;
            string _description;
            
            HotkeyDelegate _callback;
            
            public EditorHotkey( char[] validKeys, string showKey, string description, HotkeyDelegate callback )
            {
                this._validKeys = validKeys;
                this._showKey = showKey;
                this._description = description;
                this._callback = callback;
            }
            
            public string FullDescription
            {
                get
                {
                    return string.Format( "{0} - {1}", _showKey, _description );
                }
            }
            
            public bool ValidKeyPressed( char test )
            {
                foreach( var key in _validKeys )
                    if( test == key ) return true;
                return false;
            }
            
            public bool TryHandle( object sender, KeyPressEventArgs e )
            {
                return ValidKeyPressed( e.KeyChar ) && _callback( sender, e );
            }
        }
        
        public bool HotkeyDispatcher( object sender, KeyPressEventArgs e )
        {
            foreach( var hk in EditorHotkeys )
                if( hk.TryHandle( sender, e ) ) return true;
            return false;
        }
        
        #endregion
        
        #region Selection Mode
        
        public SelectionMode EditorSelectionMode { get { return editorSelectionMode; } }
            
        string EditorSelectionModeLabel
        {
            get
            {
                if( editorEnabled )
                {
                    switch( editorSelectionMode )
                    {
                        case SelectionMode.None :
                            return "None";
                        case SelectionMode.Vertex :
                            return "Vertex";
                        case SelectionMode.Edge :
                            return "Edge";
                    }
                }
                return "";
            }
        }
        
        void EditorSelectionModeToggle( SelectionMode mode )
        {
            editorSelectionMode = editorSelectionMode != mode ? mode : SelectionMode.None;
            if( editorSelectionModeStatus != null )
                editorSelectionModeStatus.Text = EditorSelectionModeLabel;
        }
        
        #endregion
        
        #region Hotkey Selection Modes
        
        bool mouseSelectionMode;
        Maths.Vector2f mouseSelectionPoint;
        List<WeldPoint> editorSelectedVertices;
        
        public List<WeldPoint> EditorSelectedVertices { get { return editorSelectedVertices; } }
        public bool MouseSelectionMode { get { return mouseSelectionMode; } }
        
        public void SelectVerticiesNear( Maths.Vector2f position, bool unweldAsNeeded = false )
        {
            var corners = WeldPoint.FindWeldableCorners( transform.Worldspace, transform.RenderVolumes, position, 64f, true, null );
            WeldPoint.FilterWeldPoints( corners, true, false, true, unweldAsNeeded, null );
            editorSelectedVertices = corners;
        }
        
        public WeldPoint ClosestAnchoredCornerNear( Maths.Vector2f position, bool includeAlreadySelectedCorners = false )
        {
            // Find all the corners near the position
            var corners = WeldPoint.FindWeldableCorners( transform.Worldspace, transform.RenderVolumes, position, 64f, true, null );
            WeldPoint.FilterWeldPoints( corners, false, true, true, false, !includeAlreadySelectedCorners ? editorSelectedVertices : null );
            
            // No corners found
            if( corners.NullOrEmpty() ) return null;
            
            // Now search through the remaining anchored corners and return the closest
            int closest = 0;
            float dist = ( position - corners[ closest ].Position ).Length;
            for( int i = 1; i < corners.Count; i++ )
            {
                float temp = ( position - corners[ i ].Position ).Length;
                if( temp < dist )
                {
                    dist = temp;
                    closest = i;
                }
            }
            
            return corners[ closest ];
        }
        
        void MoveSelectedVerticiesToMouse( MouseEventArgs e, bool anchorCorners )
        {
            if( editorSelectedVertices.NullOrEmpty() )
                return;
            
            // Update the mouse selection point
            mouseSelectionPoint = transform.ScreenspaceToWorldspace( e.X, e.Y );
            
            // Find the cloest anchored vertex that isn't in the selected group
            var closestAnchored = ClosestAnchoredCornerNear( mouseSelectionPoint, false );
            
            // Found it, snap the selection point to the anchor point
            if( closestAnchored != null )
                mouseSelectionPoint = closestAnchored.Position;
            
            // Now move the selected corners to the mouse/closest unselected anchored corner
            WeldPoint.WeldCornersTo( mouseSelectionPoint, editorSelectedVertices, true, anchorCorners );
        }
        
        bool ToggleVertexSelection( object sender, KeyPressEventArgs e )
        {
            // Only allow toggle if not actively selecting anything
            if( mouseSelectionMode ) return false;
            
            EditorSelectionModeToggle( SelectionMode.Vertex );
            return true;
        }
        
        bool ToggleEdgeSelection( object sender, KeyPressEventArgs e )
        {
            // Only allow toggle if not actively selecting anything
            if( mouseSelectionMode ) return false;
            
            EditorSelectionModeToggle( SelectionMode.Edge );
            return true;
        }
        
        #endregion
        
        #region EditorHotkeys Property
        
        List<EditorHotkey> _editorHotkeys;
        public List<EditorHotkey> EditorHotkeys
        {
            get
            {
                if( _editorHotkeys == null )
                {
                    var hk = new List<EditorHotkey>();
                    hk.Add( new EditorHotkey( new char[] { 'V', 'v' }, "V", "Toggle vertex selection", ToggleVertexSelection ) );
                    hk.Add( new EditorHotkey( new char[] { 'E', 'e' }, "E", "Toggle edge selection", ToggleEdgeSelection ) );
                    _editorHotkeys = hk;
                }
                return _editorHotkeys;
            }
        }
        
        #endregion
        
        #region Editor Mode pbTarget and editorForm events
        
        void editorMode_KeyPress( object sender, KeyPressEventArgs e )
        {
            if( !editorMouseOver )
                return;
            e.Handled = HotkeyDispatcher( sender, e );
        }
        
        void editorMode_MouseDown( object sender, MouseEventArgs e )
        {
            if( editorSelectionMode == SelectionMode.None ) return;
            
            mouseSelectionMode = true;
            
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                mouseSelectionPoint = transform.ScreenspaceToWorldspace( e.X, e.Y );
                SelectVerticiesNear( mouseSelectionPoint, true );
            }
            
            transform.ReRenderCurrentScene();
        }
        
        void editorMode_MouseUp( object sender, MouseEventArgs e )
        {
            if( !mouseSelectionMode ) return;
            
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                // Set and anchor the corner[s] when done moving them
                MoveSelectedVerticiesToMouse( e, true );
            }
            
            mouseSelectionMode = false;
            mouseSelectionPoint = Maths.Vector2f.Zero;
            editorSelectedVertices = null;
        }
        
        void editorMode_MouseMove( object sender, MouseEventArgs e )
        {
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                if( mouseSelectionMode )
                {
                    // Set but don't anchor the corner[s] while moving them
                    MoveSelectedVerticiesToMouse( e, false );
                }
                else
                {
                    // User isn't actively editing but we need to update the "selection circle"
                    mouseSelectionPoint = transform.ScreenspaceToWorldspace( e.X, e.Y );
                }
            }
            
            transform.ReRenderCurrentScene();
        }
        
        void editorMode_MouseEnter( object sender, EventArgs e )
        {
            editorMouseOver = true;
        }
        
        void editorMode_MouseLeave( object sender, EventArgs e )
        {
            editorMouseOver = false;
        }
        
        #endregion
        
        public void DrawEditor()
        {
            if( !editorEnabled ) return;
            
            switch( editorSelectionMode )
            {
                case SelectionMode.None :
                    break;
                    
                case SelectionMode.Vertex :
                    // Draw the "selection circle"
                    var pen = new Pen( Color.White );
                    transform.DrawCircleWorldTransform( pen, mouseSelectionPoint.X, mouseSelectionPoint.Y, 64f );
                        
                    break;
                    
                case SelectionMode.Edge :
                    break;
            }
            
        }
        
    }
}
