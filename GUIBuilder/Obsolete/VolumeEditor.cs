/*
 * VolumeEditor.cs
 *
 * All this code is to edit build volumes, etc.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


using SDL2ThinLayer;
using SDL2;

/*
namespace GUIBuilder
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
        //bool oldFormKeyPreview; // We're setting KeyPreview to true, don't reset it to false erroneously when editor mode is toggled off 
        bool editorMouseOver;
        SelectionMode editorSelectionMode;
        
        RenderTransform transform;
        //Form editorForm;
        //PictureBox pbTarget;
        SDLRenderer sdlRenderer;
        ToolStripStatusLabel editorSelectionModeStatus;
        TextBox editorHotkeyDescriptions;
        
        public VolumeEditor( RenderTransform _transform, ToolStripStatusLabel _editorSelectionModeStatus, TextBox _editorHotkeyDescriptions )
        {
            if( (_transform == null )||( _editorSelectionModeStatus == null )||( _editorHotkeyDescriptions == null ) )
                throw new ArgumentNullException();
            
            editorEnabled = false;
            editorMouseOver = false;
            transform = _transform;
            sdlRenderer = _transform.Renderer;
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
            
            // Dispose of external references
            transform = null;
            editorSelectionModeStatus = null;
            editorHotkeyDescriptions = null;
            
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
            
            //editorForm.KeyPreview = oldFormKeyPreview;
            //editorForm.KeyPress -= this.editorMode_KeyPress;
            //pbTarget.MouseDown -= this.editorMode_MouseDown;
            //pbTarget.MouseUp -= this.editorMode_MouseUp;
            //pbTarget.MouseMove -= this.editorMode_MouseMove;
            //pbTarget.MouseEnter -= this.editorMode_MouseEnter;
            //pbTarget.MouseLeave -= this.editorMode_MouseLeave;
            
            sdlRenderer.KeyUp -= this.editorMode_KeyPress;
            sdlRenderer.MouseButtonDown -= this.editorMode_MouseDown;
            sdlRenderer.MouseButtonUp -= this.editorMode_MouseUp;
            sdlRenderer.MouseMove -= this.editorMode_MouseMove;
            //sdlRenderer.MouseWheel -= this.editorMode_MouseWheel;
            sdlRenderer.MouseEnter -= this.editorMode_MouseEnter;
            sdlRenderer.MouseExit -= this.editorMode_MouseLeave;
        }
        
        public void EnableEditorMode()
        {
            if( editorEnabled )
            {
                DisableEditorMode();
            }
            
            //oldFormKeyPreview = editorForm.KeyPreview;
            //editorForm.KeyPreview = true;
            //editorForm.KeyPress += this.editorMode_KeyPress;
            //pbTarget.MouseDown += this.editorMode_MouseDown;
            //pbTarget.MouseUp += this.editorMode_MouseUp;
            //pbTarget.MouseMove += this.editorMode_MouseMove;
            //pbTarget.MouseEnter += this.editorMode_MouseEnter;
            //pbTarget.MouseLeave += this.editorMode_MouseLeave;
            
            sdlRenderer.KeyUp += this.editorMode_KeyPress;
            sdlRenderer.MouseButtonDown += this.editorMode_MouseDown;
            sdlRenderer.MouseButtonUp += this.editorMode_MouseUp;
            sdlRenderer.MouseMove += this.editorMode_MouseMove;
            //sdlRenderer.MouseWheel += this.editorMode_MouseWheel;
            sdlRenderer.MouseEnter += this.editorMode_MouseEnter;
            sdlRenderer.MouseExit += this.editorMode_MouseLeave;
            
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
        
        public delegate bool HotkeyDelegate( SDLRenderer renderer, SDL.SDL_Event e );
        
        public struct EditorHotkey
        {
            SDL.SDL_Keycode[] _validKeys;
            string _showKey;
            string _description;
            
            HotkeyDelegate _callback;
            
            public EditorHotkey( SDL.SDL_Keycode[] validKeys, string showKey, string description, HotkeyDelegate callback )
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
            
            public bool ValidKeyPressed( SDL.SDL_Keycode test )
            {
                foreach( var key in _validKeys )
                    if( test == key ) return true;
                return false;
            }
            
            public bool TryHandle( SDLRenderer renderer, SDL.SDL_Event e )
            {
                return ValidKeyPressed( e.key.keysym.sym ) && _callback( renderer, e );
            }
        }
        
        public bool HotkeyDispatcher( SDLRenderer renderer, SDL.SDL_Event e )
        {
            foreach( var hk in EditorHotkeys )
                if( hk.TryHandle( renderer, e ) ) return true;
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
            var corners = WeldPoint.FindWeldableCorners( transform.Worldspace, transform.SubDivisions, position, 64f, true, null );
            WeldPoint.FilterWeldPoints( corners, true, false, true, unweldAsNeeded, null );
            editorSelectedVertices = corners;
        }
        
        public WeldPoint ClosestAnchoredCornerNear( Maths.Vector2f position, bool includeAlreadySelectedCorners = false )
        {
            // Find all the corners near the position
            var corners = WeldPoint.FindWeldableCorners( transform.Worldspace, transform.SubDivisions, position, 64f, true, null );
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
        
        void MoveSelectedVerticiesTo( Maths.Vector2f p, bool anchorCorners )
        {
            if( editorSelectedVertices.NullOrEmpty() )
                return;
            
            // Update the mouse selection point
            mouseSelectionPoint = p;
            
            // Find the cloest anchored vertex that isn't in the selected group
            var closestAnchored = ClosestAnchoredCornerNear( mouseSelectionPoint, false );
            
            // Found it, snap the selection point to the anchor point
            if( closestAnchored != null )
                mouseSelectionPoint = closestAnchored.Position;
            
            // Now move the selected corners to the mouse/closest unselected anchored corner
            WeldPoint.WeldCornersTo( mouseSelectionPoint, editorSelectedVertices, true, anchorCorners );
        }
        
        bool ToggleVertexSelection( SDLRenderer renderer, SDL.SDL_Event e )
        {
            // Only allow toggle if not actively selecting anything
            if( mouseSelectionMode ) return false;
            
            EditorSelectionModeToggle( SelectionMode.Vertex );
            return true;
        }
        
        bool ToggleEdgeSelection( SDLRenderer renderer, SDL.SDL_Event e )
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
                    hk.Add( new EditorHotkey( new SDL.SDL_Keycode[] { SDL.SDL_Keycode.SDLK_v }, "V", "Toggle vertex selection", ToggleVertexSelection ) );
                    hk.Add( new EditorHotkey( new SDL.SDL_Keycode[] { SDL.SDL_Keycode.SDLK_e }, "E", "Toggle edge selection", ToggleEdgeSelection ) );
                    _editorHotkeys = hk;
                }
                return _editorHotkeys;
            }
        }
        
        #endregion
        
        #region Editor Mode pbTarget and editorForm events
        
        //void editorMode_KeyPress( object sender, KeyPressEventArgs e )
        void editorMode_KeyPress( SDLRenderer renderer, SDL.SDL_Event e )
        {
            if( !editorMouseOver )
                return;
            HotkeyDispatcher( renderer, e );
        }
        
        //void editorMode_MouseDown( object sender, MouseEventArgs e )
        void editorMode_MouseDown( SDLRenderer renderer, SDL.SDL_Event e )
        {
            if( editorSelectionMode == SelectionMode.None ) return;
            
            mouseSelectionMode = true;
            
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                mouseSelectionPoint = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y ); // May not be valid with SDL for this event
                SelectVerticiesNear( mouseSelectionPoint, true );
            }
            
            //transform.ReRenderCurrentScene();
        }
        
        //void editorMode_MouseUp( object sender, MouseEventArgs e )
        void editorMode_MouseUp( SDLRenderer renderer, SDL.SDL_Event e )
        {
            if( !mouseSelectionMode ) return;
            
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                // Set and anchor the corner[s] when done moving them
                var p = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
                MoveSelectedVerticiesTo( p, true );
            }
            
            mouseSelectionMode = false;
            mouseSelectionPoint = Maths.Vector2f.Zero;
            editorSelectedVertices = null;
        }
        
        //void editorMode_MouseMove( object sender, MouseEventArgs e )
        void editorMode_MouseMove( SDLRenderer renderer, SDL.SDL_Event e )
        {
            if( editorSelectionMode == SelectionMode.Vertex )
            {
                if( mouseSelectionMode )
                {
                    // Set but don't anchor the corner[s] while moving them
                    var p = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
                    MoveSelectedVerticiesTo( p, false );
                }
                else
                {
                    // User isn't actively editing but we need to update the "selection circle"
                    mouseSelectionPoint = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
                }
            }
            
            //transform.ReRenderCurrentScene();
        }
        
        //void editorMode_MouseEnter( object sender, EventArgs e )
        void editorMode_MouseEnter( SDLRenderer renderer, SDL.SDL_Event e )
        {
            editorMouseOver = true;
        }
        
        //void editorMode_MouseLeave( object sender, EventArgs e )
        void editorMode_MouseLeave( SDLRenderer renderer, SDL.SDL_Event e )
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
                    //var pen = new Pen( Color.White );
                    //transform.DrawCircleWorldTransform( pen, mouseSelectionPoint.X, mouseSelectionPoint.Y, 64f );
                    transform.DrawCircleWorldTransform( mouseSelectionPoint.X, mouseSelectionPoint.Y, 64f, Color.White );
                    
                    break;
                    
                case SelectionMode.Edge :
                    break;
            }
            
        }
        
    }
}
*/
