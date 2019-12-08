/*
 * Render.cs
 *
 * Worldspace render window
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.ComponentModel;

using SDL2ThinLayer;
using SDL2;

using Engine;

using Fallout4;
using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;

namespace GUIBuilder.Windows
{
    /// <summary>
    /// Description of Render.
    /// </summary>
    public partial class Render : Form, GodObject.XmlConfig.IXmlConfiguration
    {
        
        const string XmlLocation = "Location";
        const string XmlSize = "Size";
        bool onLoadComplete = false;
        
        public GodObject.XmlConfig.IXmlConfiguration XmlParent { get { return null; } }
        
        public string XmlKey { get { return this.Name; } }
        
        public string XmlPath { get{ return GodObject.XmlConfig.XmlPathTo( this ); } }
        
        // Tool windows for settlement objects
        Windows.RenderChild.WorldspaceTool twWorldspaces = null;
        Windows.RenderChild.SyncObjectTool<Fallout4.WorkshopScript> twWorkshops = null;
        Windows.RenderChild.SyncObjectTool<AnnexTheCommonwealth.Settlement> twSettlements = null;
        Windows.RenderChild.SyncObjectTool<AnnexTheCommonwealth.SubDivision> twSubDivisions = null;
        
        // Main render transform
        RenderChild.RenderTransform transform;
        Maths.Vector2f _mouseWorldPos;
        System.Timers.Timer fpsUpdater;
        
        // Editor
        //VolumeEditor editor;
        
        // Selected import mod
        //ImportMod _selectedImportMod = null;
        
        public Render()
        {
            InitializeComponent();
        }
        
        void OnFormLoad( object sender, EventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.RenderWindow", "OnFormLoad()" } );
            
            //_SetEnableState( false );
            
            this.Translate( true );
            
            this.Location = GodObject.XmlConfig.ReadPoint( XmlPath, XmlLocation, this.Location );
            this.Size = GodObject.XmlConfig.ReadSize( XmlPath, XmlSize, this.Size );
            
            _cancelInitWindow = false;
            
            tslMouseToCellGrid.Text = "";
            tslMouseToWorldspace.Text = "";
            
            twWorldspaces = new Windows.RenderChild.WorldspaceTool();
            twWorkshops = new Windows.RenderChild.SyncObjectTool<WorkshopScript>( "RenderWindow.Workshops", GodObject.Plugin.Data.Workshops );
            AddOwnedForm( twWorldspaces );
            AddOwnedForm( twWorkshops );
            twWorldspaces.Show();
            twWorkshops.Show();
            
            if( GodObject.Master.AnnexTheCommonwealth.Loaded )
            {
                twSettlements = new Windows.RenderChild.SyncObjectTool<Settlement>( "RenderWindow.Settlements", GodObject.Plugin.Data.Settlements );
                twSubDivisions = new Windows.RenderChild.SyncObjectTool<AnnexTheCommonwealth.SubDivision>( "RenderWindow.SubDivisions", GodObject.Plugin.Data.SubDivisions, typeof( FormEditor.SubDivision ) );
                AddOwnedForm( twSettlements );
                AddOwnedForm( twSubDivisions );
                twSettlements.Show();
                twSubDivisions.Show();
                tsRenderSettlements.Visible = true;
                tsRenderSubDivisions.Visible = true;
            }
            else
            {
                tsRenderSettlements.Visible = false;
                tsRenderSubDivisions.Visible = false;
            }
            
            var cWorldspaces = GodObject.Plugin.Data.Root.GetCollection<Engine.Plugin.Forms.Worldspace>( true, true );
            if( cWorldspaces != null )
                twWorldspaces.SyncObjects = cWorldspaces.ToList<Engine.Plugin.Forms.Worldspace>();
            
            //ResetGUIElements();
            List<string> hints = null;
            List<string> hintValues = null;
            int sdlVideoHint = GodObject.Windows.SDLVideoDriverIndex;
            if( GodObject.Windows.SDLVideoDriverIndex != GodObject.Windows.SDLVideoDriverDefaultIndex )
            {
                hints = new List<string>();
                hintValues = new List<string>();
                hints.Add( SDL2.SDL.SDL_HINT_RENDER_DRIVER );
                hintValues.Add( GodObject.Windows.SDLVideoDriver );
            }
            var initParams = new SDL2ThinLayer.SDLRenderer.InitParams( GodObject.Windows.GetMainWindow(), pnRenderTarget, showCursorOverControl: false, sdlHints: hints, sdlHintValues: hintValues );
            WorkerThreadPool.CreateWorker( initParams, THREAD_RENDER_Init, null ).Start();
            
            onLoadComplete = true;
            DebugLog.CloseIndentLevel();
        }
        
        void OnFormClosing( object sender, FormClosingEventArgs e )
        {
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.RenderWindow", "OnFormClosing()" } );
            GodObject.Windows.SetRenderWindow( null, false );
            Shutdown();
            
            if( twWorldspaces != null ) twWorldspaces.Close();
            if( twWorkshops != null ) twWorkshops.Close();
            if( twSettlements != null ) twSettlements.Close();
            if( twSubDivisions != null ) twSubDivisions.Close();
            twWorldspaces = null;
            twWorkshops = null;
            twSettlements = null;
            twSubDivisions = null;
            
            DebugLog.CloseIndentLevel();
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
            if(
                ( transform == null )||
                ( !transform.Renderer.Anchored )
            ) return;
            transform.InvokeSetSDLWindowSize();
        }
        
        bool _cancelInitWindow = false;
        void THREAD_RENDER_Init( object obj )
        {
            var initParams = obj as SDLRenderer.InitParams;
            if( initParams == null )
                throw new ArgumentNullException( "obj does not resolve to InitParams!" );
            
            _UpdatingRenderer = true;
            _ResetViewportOnUpdate = true;
            
            DebugLog.OpenIndentLevel( new [] { "GUIBuilder.RenderWindow", "THREAD_RENDER_Init()" } );
            
            if( CreateTransform( initParams ) )
                UpdateRenderWindowThread(); // Call the thread function directly from this thread
            
            transform.SyncSceneUpdate( false );
            
            DebugLog.CloseIndentLevel();
        }
        
        #region Sync'd list monitoring
        
        public void UpdateSettlementObjectChildWindowContentsForWorldspace( Engine.Plugin.Forms.Worldspace worldspace )
        {
            DebugLog.WriteLine( string.Format( "{0} :: UpdateSettlementObjectChildWindowContentsForWorldspace() :: worldspace ? {1}", this.GetType().ToString(), worldspace == null ? "null" : worldspace.ToString() ) );
            twWorkshops.Worldspace = worldspace;
            if( GodObject.Master.AnnexTheCommonwealth.Loaded )
            {
                twSettlements.Worldspace = worldspace;
                twSubDivisions.Worldspace = worldspace;
            }
        }
        
        #endregion
        
        #region Global Form update
        
        /// <summary>
        /// Long running functions should disable the main form so the user can't spam inputs.  Don't forget to enable the form again after the long-running function is complete so the user can continue to use the program.
        /// </summary>
        /// <param name="enabled">true to enable the form and it's controls, false to disable the form and it's controls.</param>
        public void SetEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetEnableState( enabled ); }, null );
                return;
            }
            pnWindow.Enabled = enabled;
            twWorldspaces.SetEnableState( enabled );
            twWorkshops.SetEnableState( enabled );
            if( GodObject.Master.AnnexTheCommonwealth.Loaded )
            {
                twSettlements.SetEnableState( enabled );
                twSubDivisions.SetEnableState( enabled );
            }
        }
        
        public void SetToolStripEnableState( bool enabled )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetToolStripEnableState( enabled ); }, null );
                return;
            }
            tsRenderWindow.Enabled = enabled;
        }
        
        public void Shutdown()
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { Shutdown(); }, null );
                return;
            }
            //DebugLog.Write( "GUIBuilder.RenderWindow.Shutdown()" );
            DestroyTransform();
        }
        
        void FPSUpdate( object sender, EventArgs e )
        {
            if( transform == null )
                return;
            if( this.InvokeRequired )
            {
                this.BeginInvoke( (Action)delegate() { FPSUpdate( sender, e ); }, null );
                return;
            }
            var str = string.Format(
                "RenderWindow.FPS".Translate(),
                transform.Renderer.ActualFPS,
                transform.Renderer.PotentialFPS,
                transform.Renderer.AverageFrameTimeMS,
                transform.Renderer.AverageFrameTimeTicks );
            this.Text = str;
        }
        
        /*
        public void ResetGUIElements()
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.ResetGUIElements()" );
            // Worldspace controls
            gbSubDivisions.Enabled = false;
            tbWorldspaceFormIDEditorID.Clear();
            tbWorldspaceGridBottomX.Clear();
            tbWorldspaceGridBottomY.Clear();
            tbWorldspaceGridTopX.Clear();
            tbWorldspaceGridTopY.Clear();
            tbWorldspaceHeightmapTexture.Clear();
            tbWorldspaceWaterHeightsTexture.Clear();
            tbWorldspaceMapHeightMax.Clear();
            tbWorldspaceMapHeightMin.Clear();
            cbWorldspace.Text = "";
            
        }
        */
        
        /*
        public bool HasWorldspace( string worldspace )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.HasWorldspace()" );
            return cbWorldspace.Items.Contains( worldspace );
        }
        
        public void SetWorldspaces( List<Engine.Plugin.Forms.Worldspace> worldspaces )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetWorldspaces( worldspaces ); }, null );
                return;
            }
            cbWorldspace.Items.Clear();
            foreach( var w in worldspaces )
                AddWorldspace( w.FormID, w.EditorID );
        }
        
        public void AddWorldspace( uint formid, string editorid )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.AddWorldspace()" );
            cbWorldspace.Items.Add( editorid );
        }
        
        */
        
        /*
        public ImportMod SelectedImportMod()
        {
            return _selectedImportMod;
        }
        */
        
        /*
        public void SetSelectedImportMod( ImportMod mod )
        {
            if( _selectedImportMod == mod )
                return;
            ResetGUIElements();
            ClearSubDivisions();
            AddSubDivisions( mod );
            _selectedImportMod = mod;
            gbRenderSelectedOnly.Enabled = _selectedImportMod != null;
            TryUpdateRenderWindow( true );
        }
        */
        
        /*
        public Engine.Plugin.Forms.Worldspace SelectedWorldspace()
        {
            //DebugLog.Write( "RenderWindow.SelectedWorldspace()\n" );
            int index = cbWorldspace.SelectedIndex;
            if( index < 0 )
                return null;
            string editorid = (string)cbWorldspace.Items[ index ];
            return GodObject.Plugin.Data.Worldspaces.Find( editorid );
        }
        */
        
        #endregion
        
        #region Mouse/Over Rendering
        
        List<IMouseOver> _mouseOverObjects = null;
        List<string> _mouseOverInfo = null;
        
        static void AddMouseOverFrom<T>( ref List<IMouseOver> result, List<T> mouseOverObjects, Maths.Vector2f mouse, float maxDistance ) where T : Engine.Plugin.PapyrusScript
        {
            if( mouseOverObjects.NullOrEmpty() )
                return;
            var moos = new List<IMouseOver>();
            foreach( var moo in mouseOverObjects )
            {
                var refr = moo.Reference;
                if( refr.IsMouseOver( mouse, maxDistance ) )
                    moos.AddOnce( refr );
            }
            if( moos.Count == 0 )
                return;
            if( result == null )
                result = new List<IMouseOver>();
            result.AddOnce( moos );
        }
        
        public List<IMouseOver> GetMouseOverObjects()
        {
            return _mouseOverObjects;
        }
        
        bool UpdateMouseOver( Maths.Vector2f pos, bool toggleSubdivisionSelection )
        {
            if( transform == null )
                return false;
            
            var newMoos = (List<IMouseOver>)null;
            const float maxDistance = 64.0f;
            
            if( tsRenderSettlements.Checked )
                AddMouseOverFrom( ref newMoos, transform.Settlements, pos, maxDistance );
            
            if( tsRenderSubDivisions.Checked )
                AddMouseOverFrom( ref newMoos, transform.SubDivisions, pos, maxDistance );
            
            if( tsRenderWorkshops.Checked )
                AddMouseOverFrom( ref newMoos, transform.Workshops, pos, maxDistance );
            
            if( ( tsRenderBorders.Checked )&&( !transform.SubDivisions.NullOrEmpty() ) )
                foreach( var s in transform.SubDivisions )
                    AddMouseOverFrom( ref newMoos, s.BorderEnablers, pos, maxDistance );
                
            if( tsRenderEdgeFlags.Checked )
            {
                AddMouseOverFrom( ref newMoos, transform.AssociatedEdgeFlags, pos, maxDistance );
                AddMouseOverFrom( ref newMoos, transform.UnassociatedEdgeFlags, pos, maxDistance );
            }
            
            var hasMoos = !_mouseOverObjects.NullOrEmpty();
            if( newMoos.NullOrEmpty() )
            {
                _mouseOverObjects = null;
                _mouseOverInfo = null;
                return hasMoos;
            }
            
            if(
                ( hasMoos )&&
                ( _mouseOverObjects.Count == newMoos.Count )&&
                ( _mouseOverObjects.ContainsAllElementsOf( newMoos ) )
            )
                return false;
            
            var moil = new List<string>();
            foreach( var newMoo in newMoos )
                moil.AddAll( newMoo.MouseOverInfo );
            
            _mouseOverInfo = moil;
            return true;
        }
        
        public void UpdateMousePositionInScene( Maths.Vector2f position )
        {
            if( this.InvokeRequired )
            {
                this.BeginInvoke( (Action)delegate() { UpdateMousePositionInScene( position ); }, null );
                return;
            }
            tslMouseToWorldspace.Text = position.ToString();
            tslMouseToCellGrid.Text = _mouseWorldPos.WorldspaceToCellGrid().ToString();
        }
        
        #endregion
        
        #region Render event handlers (called async in renderer thread)
        
        void RenderWindow_Closed( SDLRenderer renderer )
        {
            //DebugLog.Write( "MainForm.RenderWindow_Closed()" );
            DestroyTransform();
        }
        
        void RenderWindow_MouseUp( SDLRenderer renderer, SDL.SDL_Event e )
        {
            // Editor supercedes us
            /*
            if(
                ( transform == null )|| //<-- Should never happen
                //( transform.ImportMod == null )|| // <-- May happen
                (
                    ( editor != null )&&
                    ( editor.MouseSelectionMode )
                )
            ) return;
            */
            
            DebugLog.OpenIndentLevel( new [] { this.GetType().ToString(), "RenderWindow_MouseUp()" } );
            
            _mouseWorldPos = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
            
            if( UpdateMouseOver( _mouseWorldPos, true ) )
            {
                //if( !_mouseOverInfo.NullOrEmpty() )
                //    foreach( var moil in _mouseOverInfo )
                //        DebugLog.WriteLine( moil );
            }
            
            /*
            List<SubDivision> subdivisions = null;
            List<Volume> volumes = null;
            if( !transform.ImportMod.TryGetVolumesFromPos( _mouseWorldPos, out subdivisions, out volumes, transform.SubDivisions ) )
                return;
           
            ToggleSelectedSubDivisions( subdivisions );
            */
            
            DebugLog.CloseIndentLevel();
        }
        
        void RenderWindow_MouseMove( SDLRenderer renderer, SDL.SDL_Event e )
        {
            _mouseWorldPos = transform.ScreenspaceToWorldspace( e.motion.x, e.motion.y );
            transform.SetMousePos( new Maths.Vector2i( e.motion.x, e.motion.y ) );
            UpdateMousePositionInScene( _mouseWorldPos );
            
            List<SubDivision> parents = null;
            List<Volume> volumes = null;
            
            var rthlParents = transform.HighlightSubDivisions;
            var rthlVolumes = transform.HighlightVolumes;
            
            bool volumesChanged = false;
            
            if( parents != null )
            {
                if(
                    ( rthlParents == null )||
                    (
                        ( !rthlParents.ContainsAllElementsOf( parents ) )||
                        ( !parents.ContainsAllElementsOf( rthlParents ) )
                    )
                )   volumesChanged = true;
            }
            else if( rthlParents != null )
                volumesChanged = true;
            
            if( volumes != null )
            {
                if(
                    ( rthlVolumes == null )||
                    (
                        ( !rthlVolumes.ContainsAllElementsOf( volumes ) )||
                        ( !volumes.ContainsAllElementsOf( rthlVolumes ) )
                    )
                )   volumesChanged = true;
            }
            else if( rthlVolumes != null )
                volumesChanged = true;
            
            if( !volumesChanged ) return;
            
            transform.HighlightSubDivisions = parents;
            transform.HighlightVolumes = volumes;
        }
        
        void RenderWindow_MouseWheel( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var scale = transform.GetScale();
            scale += (float)e.wheel.y * 0.0125f;
            transform.SetScale( scale );
            transform.SetViewCentre( _mouseWorldPos );
        }
        
        void RenderWindow_KeyDown( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var kms = SDL.SDL_GetModState();
            var viewCentre = transform.GetViewCentre();
            var movement = ( kms & SDL.SDL_Keymod.KMOD_SHIFT ) != 0 ? 1024f : 128f;
            
            var code = e.key.keysym.sym;
            if( code == SDL.SDL_Keycode.SDLK_LEFT )     viewCentre.X -= movement;
            if( code == SDL.SDL_Keycode.SDLK_RIGHT )    viewCentre.X += movement;
            if( code == SDL.SDL_Keycode.SDLK_UP )       viewCentre.Y += movement;
            if( code == SDL.SDL_Keycode.SDLK_DOWN )     viewCentre.Y -= movement;
            
            transform.SetViewCentre( viewCentre );
        }
        
        void RenderWindow_KeyUp( SDLRenderer renderer, SDL.SDL_Event e )
        {
            var code = e.key.keysym.sym;
            if( code == SDL.SDL_Keycode.SDLK_F9 )       transform.DumpMouseOverInfo();
        }
        
        #endregion
        
        #region UI to adjust Min Settlement Object Render Size
        
        void tsMinSettlementObjectsRenderSizeKeyPress( object sender, KeyPressEventArgs e )
        {
            if( ( !char.IsControl( e.KeyChar ) )&&( !char.IsDigit( e.KeyChar ) )&&( e.KeyChar != '.' ) )
            {
                e.Handled = true;
            }
        }
        
        void tsMinSettlementObjectsRenderSizeTextChanged( object sender, EventArgs e )
        {
            if( transform == null )
                return;
            transform.MinRenderScaledSettlementObjects = Convert.ToSingle( tsMinSettlementObjectsRenderSize.Text );
        }
        
        void SetMinSettlementObjectRenderSizeText( float value )
        {
            if( this.InvokeRequired )
            {
                this.Invoke( (Action)delegate() { SetMinSettlementObjectRenderSizeText( value ); }, null );
            }
            tsMinSettlementObjectsRenderSize.Text = value.ToString( "0.0" );
        }
        
        #endregion
        
        bool _UpdatingRenderer = false;
        bool _ResetViewportOnUpdate = false;
        public void TryUpdateRenderWindow( bool resetViewport )
        {
            //DebugLog.Write( string.Format( "\n{0} :: TryUpdateRenderWindow() :: resetViewport = {1} :: _UpdatingRenderer = {2}", this.GetType().ToString(), resetViewport, _UpdatingRenderer ) );
            if( ( _UpdatingRenderer )||( transform == null ) )
                return;
            SetToolStripEnableState( false );
            _UpdatingRenderer = true;
            _ResetViewportOnUpdate = resetViewport;
            WorkerThreadPool.CreateWorker( UpdateRenderWindowThread, null ).Start();
        }
        
        bool _suppressControlUpdate = false;
        void RenderStateControlChanged( object sender, EventArgs e )
        {
            if( _suppressControlUpdate )
                return;
            //DebugLog.Write( "GUIBuilder.RenderWindow.RenderStateControlChanged()" );
            TryUpdateRenderWindow( false );
        }
        
        #region Rendering and controls
        
        void GetRenderOptions(
            out bool renderNonPlayable,
            out bool renderCellGrid,
            out bool renderWorldspace,
            out bool renderLand,
            out bool renderWater,
            out bool renderWorkshops,
            out bool renderSettlements,
            out bool renderSubDivisions,
            out bool renderEdgeFlags,
            out bool renderEdgeFlagLinks,
            out bool renderBuildVolumes,
            out bool renderSandboxVolumes,
            out bool renderBorders,
            out bool renderSelectedOnly,
            out bool exportPNG )
        {
            //DebugLog.Write( string.Format( "\n{0} :: GetRenderOptions()", this.GetType().ToString() ) );
            // Set all to false initially
            renderNonPlayable           = false;
            
            renderCellGrid              = false;
            renderWorldspace            = false;
            
            renderLand                  = false;
            renderWater                 = false;
            
            renderWorkshops             = false;
            renderSettlements           = false;
            renderSubDivisions          = false;
            
            renderEdgeFlags             = false;
            renderEdgeFlagLinks         = false;
            
            renderBuildVolumes          = false;
            renderSandboxVolumes        = false;
            
            renderBorders               = false;
            
            renderSelectedOnly          = false;
            
            exportPNG                   = false;//cbExportPNG.Checked;
            
            // Now make sure we actually can render things
            
            var selectedWorldspace      = twWorldspaces.SelectedWorldspace;
            
            renderNonPlayable           = tsRenderOverRegion.Checked;
            renderCellGrid              = tsRenderCellGrid.Checked;
            
            renderWorldspace            = selectedWorldspace != null;
            if( renderWorldspace )
            {
                renderLand              = tsRenderLandHeight.Checked;
                renderWater             = tsRenderWaterHeight.Checked;
                
                renderWorkshops         = tsRenderWorkshops.Checked;
                
                renderSettlements       = tsRenderSettlements.Checked;
                renderSubDivisions      = tsRenderSubDivisions.Checked;
                
                renderEdgeFlags         = tsRenderEdgeFlags.Checked;
                renderEdgeFlagLinks     = tsRenderEdgeFlagLinks.Checked;
                
                renderBuildVolumes      = tsRenderBuildVolumes.Checked;
                renderSandboxVolumes    = tsRenderSandboxVolumes.Checked;
                
                renderBorders           = tsRenderBorders.Checked;
                
                renderSelectedOnly      = tsRenderSelectedOnly.Checked;// ( cbRenderSelectedOnly.CheckState == CheckState.Checked );
            }
        }
        
        // This is called asyncronously in the renderer thread
        void ResetForNewTransform( SDLRenderer renderer )
        {
            //DebugLog.Write( "ResetForNewTransform()" );
            
            GodObject.WorldspaceDataPool.DestroyWorldspaceTextures( false );
        }
        
        void DestroyTransform()
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.DestroyTransform()" );
            
            SetEnableState( false );
            
            _cancelInitWindow = true;
            
            if( fpsUpdater != null )
            {
                fpsUpdater.Stop();
                fpsUpdater.Dispose();
            }
            fpsUpdater = null;
            
            /*
            if( editor != null )
                editor.Dispose();
            editor = null;
            */
            
            if( transform != null )
                transform.Dispose();
            transform = null;
            
            SetEnableState( true );
        }
        
        bool CreateTransform( SDLRenderer.InitParams initParams )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.CreateTransform()" );
            if( _cancelInitWindow )
                return false;
            
            #region These should never happen...
            
            // Dispose of the timer
            if( fpsUpdater != null )
            {
                fpsUpdater.Stop();
                fpsUpdater.Dispose();
                fpsUpdater = null;
            }
            
            // Dispose of the old editor
            /*
            if( editor != null )
            {
                editor.Dispose();
                editor = null;
            }
            */
            
            // Dispose of the old transform
            if( transform != null )
            {
                if( transform.Renderer != null )
                {
                    transform.Renderer.Invoke( ResetForNewTransform );
                }
                transform.Dispose();
                transform = null;
            }
            
            #endregion
            
            //var m = GodObjects.Windows.Main;
            //m.UpdateStatusMessage( "Creating render transform..." );
            //DebugLog.Write( "GUIBuilder.RenderWindow.CreateTransform() :: Creating render transform..." );
            transform = new RenderChild.RenderTransform( true, initParams ); //pnRenderTarget );
            if( transform == null )
            {
                //DebugLog.Write( "GUIBuilder.RenderWindow.CreateTransform() :: transform = null" );
                return false;
            }
            //DebugLog.Write( "GUIBuilder.RenderWindow.CreateTransform() :: ...transform created" );
            
            // Set renderer event handlers
            transform.Renderer.MouseButtonUp += RenderWindow_MouseUp;
            transform.Renderer.MouseMove += RenderWindow_MouseMove;
            transform.Renderer.MouseWheel += RenderWindow_MouseWheel;
            transform.Renderer.KeyDown += RenderWindow_KeyDown;
            transform.Renderer.KeyUp += RenderWindow_KeyUp;
            
            SetMinSettlementObjectRenderSizeText( transform.MinRenderScaledSettlementObjects );
            
            // Create FPS update timer
            fpsUpdater = new System.Timers.Timer();
            fpsUpdater.Interval = 1000;
            fpsUpdater.AutoReset = true;
            fpsUpdater.Elapsed += FPSUpdate;
            
            // Start the timer
            fpsUpdater.Start();
            
            return true;
        }
        
        void UpdateRenderWindowThread()
        {
            //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: Start", this.GetType().ToString(), _ResetViewportOnUpdate ) );
            //var wasEnabled = this.Enabled;
            //this.Enabled = false;
            
            bool renderNonPlayable,
                renderCellGrid,
                renderWorldspace,
                renderLand,
                renderWater,
                renderWorkshops,
                renderSettlements,
                renderSubdivisions,
                renderEdgeFlags,
                renderEdgeFlagLinks,
                renderBuildVolumes,
                renderSandboxVolumes,
                renderBorders,
                renderSelectedOnly,
                exportPNG;
            GetRenderOptions(
                    out renderNonPlayable,
                    out renderCellGrid,
                    out renderWorldspace,
                    out renderLand,
                    out renderWater,
                    out renderWorkshops,
                    out renderSettlements,
                    out renderSubdivisions,
                    out renderEdgeFlags,
                    out renderEdgeFlagLinks,
                    out renderBuildVolumes,
                    out renderSandboxVolumes,
                    out renderBorders,
                    out renderSelectedOnly,
                    out exportPNG );
            
            //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: Get Worldspace objects", this.GetType().ToString(), _ResetViewportOnUpdate ) );
            
            var selectedWorldspace      = !renderWorldspace ? null : twWorldspaces.SelectedWorldspace;
            var poolEntry               = !renderWorldspace ? null : selectedWorldspace.PoolEntry;
            //var selectedImportMod       = (ImportMod)null;//SelectedImportMod();
            var selectedWorkshops       = !renderWorldspace ? null : renderSelectedOnly ? twWorkshops.SelectedSyncObjects : twWorkshops.SyncObjects;
            var selectedSettlements     = ( ( !renderWorldspace )||( !GodObject.Master.AnnexTheCommonwealth.Loaded ) ) ? null : renderSelectedOnly ? twSettlements.SelectedSyncObjects : twSettlements.SyncObjects;
            var selectedSubdivisions    = ( ( !renderWorldspace )||( !GodObject.Master.AnnexTheCommonwealth.Loaded ) ) ? null : renderSelectedOnly ? twSubDivisions.SelectedSyncObjects : twSubDivisions.SyncObjects;
            var unassociatedEdgeFlags   = !renderWorldspace ? null : GodObject.Plugin.Data.EdgeFlags.ByAssociation( GodObject.Plugin.Data.EdgeFlags.FindAllInWorldspace( selectedWorldspace ), GodObject.Plugin.Data.EdgeFlags.Association.Unassociated );
            
            // Get cell range from [whole] map/selected volumes
            Maths.Vector2i cellNW = Maths.Vector2i.Zero;
            Maths.Vector2i cellSE = Maths.Vector2i.Zero;
            
            if( _ResetViewportOnUpdate )
            {
                //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: Calculate viewport", this.GetType().ToString(), _ResetViewportOnUpdate ) );
                
                if( renderSelectedOnly )
                {
                    cellNW = new Maths.Vector2i( int.MaxValue, int.MinValue );
                    cellSE = new Maths.Vector2i( int.MinValue, int.MaxValue );
                    foreach( var subDivision in selectedSubdivisions )
                    {
                        var volNW = subDivision.CellNW;
                        var volSE = subDivision.CellSE;
                        if( volNW.X < cellNW.X ) cellNW.X = volNW.X;
                        if( volNW.Y > cellNW.Y ) cellNW.Y = volNW.Y;
                        if( volSE.X > cellSE.X ) cellSE.X = volSE.X;
                        if( volSE.Y < cellSE.Y ) cellSE.Y = volSE.Y;
                    }
                }
                else if( selectedWorldspace != null )
                {
                    if( renderNonPlayable )
                    {
                        var hmCSize = poolEntry.HeightMapCellSize;
                        cellNW = poolEntry.HeightMapCellOffset;
                        cellSE = new Maths.Vector2i(
                            cellNW.X + ( hmCSize.X - 1 ),
                            cellNW.Y - ( hmCSize.Y - 1 ) );
                    }
                    else
                    {
                        var mapData = selectedWorldspace.MapData;
                        cellNW = mapData.GetCellNW( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                        cellSE = mapData.GetCellSE( Engine.Plugin.TargetHandle.WorkingOrLastFullRequired );
                    }
                }
                else
                {
                    cellNW = new Maths.Vector2i( -2,  2 );
                    cellSE = new Maths.Vector2i(  2, -2 );
                }
            }
            
            var m = GodObject.Windows.GetMainWindow();
            m.PushStatusMessage();
            m.SetCurrentStatusMessage( "RenderWindow.UpdateTransform".Translate() );
            
            transform.SyncSceneUpdate( true );
            
            // Update data references
            //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: Update scene objects", this.GetType().ToString(), _ResetViewportOnUpdate ) );
            transform.Worldspace    = selectedWorldspace;
            transform.Workshops     = selectedWorkshops;
            if( GodObject.Master.AnnexTheCommonwealth.Loaded )
            {
                transform.Settlements   = selectedSettlements;
                transform.SubDivisions  = selectedSubdivisions;
            }
            transform.UnassociatedEdgeFlags = unassociatedEdgeFlags;
            
            //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: transform :: UpdateScene()", this.GetType().ToString(), _ResetViewportOnUpdate ) );
            
            transform.UpdateScene( //renderLand, renderWater, renderCellGrid, renderBuildVolumes, renderBorders, renderEdgeFlags, true );
                    true,
                    renderCellGrid,
                    renderLand,
                    renderWater,
                    renderWorkshops,
                    renderSettlements,
                    renderSubdivisions,
                    renderEdgeFlags,
                    renderEdgeFlagLinks,
                    renderBuildVolumes,
                    renderSandboxVolumes,
                    renderBorders );
            
            #if DEBUG
            
            //transform.debugRenderBuildVolumes = cbRenderBuildVolumes.CheckState == CheckState.Indeterminate;
            //transform.debugRenderBorders = cbRenderBorders.CheckState == CheckState.Indeterminate;
            
            #endif
            
            // Update physical transform (don't recompute until all the initial conditions are set)
            if( _ResetViewportOnUpdate )
            {
                //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: transform :: Update viewport", this.GetType().ToString(), _ResetViewportOnUpdate ) );
                transform.UpdateCellClipper( cellNW, cellSE, false );
                transform.SetScale( transform.CalculateScale( transform.GetClipperCellSize() ), false );
                transform.SetViewCentre( transform.WorldspaceClipperCentre(), false );
            }
            
            // Re-enable editor mode
            //if(
            //    ( editor == null )//&&
            //    ( cbEditModeEnable.Checked )
            //) {
            //    editor = new VolumeEditor( transform, sbiEditorSelectionMode, tbEMHotKeys );
            //    editor.EnableEditorMode();
            //}
            
            // Recompute!
            transform.SyncSceneUpdate( false );
            //this.Enabled = true;
            //DebugLog.Write( string.Format( "\n{0} :: UpdateRenderWindowThread() :: resetViewport = {1} :: Complete", this.GetType().ToString(), _ResetViewportOnUpdate ) );
            
            _ResetViewportOnUpdate = false;
            _UpdatingRenderer = false;
            SetToolStripEnableState( true );
            m.PopStatusMessage();
        }
        
        void tsRepaintAllObjectsClick( object sender, EventArgs e )
        {
            TryUpdateRenderWindow( false );
        }
        
        void tsRenderSelectedOnlyCheckStateChanged( object sender, EventArgs e )
        {
            tsRenderSelectedOnly.Image = tsRenderSelectedOnly.Checked
                ? global::Properties.Resources.tsIconChecked
                : global::Properties.Resources.tsIconUnchecked;
            TryUpdateRenderWindow( false );
        }
       
        #endregion
        
        #region Target Panel events
        /*
        
        bool _setSizeScheduled = false;
        
        // User resized the main form, trigger a delayed size update of the SDL_Window
        
        void PnRenderWindowResize( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.RenderPanel.Resize()" );
            if(
                ( transform == null )||
                ( !transform.Renderer.Anchored )
            ) return;
            if( !_setSizeScheduled )
            {
                _setSizeScheduled = true;
                var timer = new System.Timers.Timer();
                timer.Interval = 2500; // 2.5s
                timer.AutoReset = false;
                timer.Elapsed += ReloadTimerTimeout;
                timer.Start();
            }
        }
        
        void ReloadTimerTimeout( object sender, EventArgs e )
        {
            //DebugLog.Write( "GUIBuilder.RenderWindow.ResizeTimer.Timeout()" );
            _setSizeScheduled = false;
            if(
                ( transform == null )||
                ( !transform.Renderer.Anchored )
            ) return;
            transform.InvokeSetSDLWindowSize();
        }
        
        */
        #endregion
        
        #region Editor Enable/Disable
        
        /*
        void CbEditModeEnableCheckedChanged( object sender, EventArgs e )
        {
            if( transform == null )
            {
                cbEditModeEnable.Checked = false;
                return;
            }
            if( cbEditModeEnable.Checked )
            {
                if( editor == null )
                    editor = new VolumeEditor( transform, sbiEditorSelectionMode, tbEMHotKeys );
                editor.EnableEditorMode();
            }
            else if( editor != null )
            {
                editor.Dispose();
                editor = null;
            }
        }
        */
        
        #endregion
        
    }
}
