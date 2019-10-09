/*
 * Windows.cs
 * 
 * Global access to the various windows forms ("window") in GUIBuilder.
 * 
 * Primarily used for updating the main window status bar but also used to globally [un]block all the windows for long running threads.
 * 
 */
using System;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using Maths;

using Engine;
using GWindow = GUIBuilder.Windows;

using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;


namespace GodObject
{
    
    #region Windows
    
    public static class Windows
    {
        
        #region Main Window
        
        static GWindow.Main _MainWindow;
        
        public static GWindow.Main GetMainWindow( bool showWindow = false )
        {
            if( ( _MainWindow != null )&&( showWindow ) )
                _MainWindow.BringToFront();
            return _MainWindow;
        }
        
        public static void SetMainWindow( GWindow.Main window, bool closeOldWindow = true )
        {
            if( ( closeOldWindow )&&( _MainWindow != null ) )
                _MainWindow.Close();
            _MainWindow = window;
        }
        
        #endregion
        
        #region About Window
        
        static GWindow.About _AboutWindow;
        
        public static GWindow.About GetAboutWindow( bool showWindow = false )
        {
            //DebugLog.Write( "GodObjects.Windows.GetAboutWindow()" );
            if( _AboutWindow == null )
            {
                _AboutWindow = new GWindow.About();
                if( showWindow )
                    _AboutWindow.Show();
            }
            else if( showWindow )
                _AboutWindow.BringToFront();
            return _AboutWindow;
        }
        
        public static void SetAboutWindow( GWindow.About window, bool closeOldWindow = true )
        {
            //DebugLog.Write( "GodObjects.Windows.SetAboutWindow()" );
            if( _AboutWindow == window )
                return;
            if( ( closeOldWindow )&&( _AboutWindow != null ) )
                _AboutWindow.Close();
            _AboutWindow = window;
        }
        
        #endregion
        
        #region Options Window
        
        static GWindow.Options _OptionsWindow;
        
        public static GWindow.Options GetOptionsWindow( bool showWindow = false )
        {
            //DebugLog.Write( "GodObjects.Windows.GetOptionsWindow()" );
            if( _OptionsWindow == null )
            {
                _OptionsWindow = new GWindow.Options();
                if( showWindow )
                    _OptionsWindow.ShowDialog();
            }
            else if( showWindow )
                _OptionsWindow.BringToFront();
            return _OptionsWindow;
        }
        
        public static void SetOptionsWindow( GWindow.Options window, bool closeOldWindow = true )
        {
            //DebugLog.Write( "GodObjects.Windows.SetOptionsWindow()" );
            if( _OptionsWindow == window )
                return;
            if( ( closeOldWindow )&&( _OptionsWindow != null ) )
                _OptionsWindow.Close();
            _OptionsWindow = window;
        }
        
        #endregion
        
        #region Border Batch Window
        
        static GWindow.BorderBatch _BorderBatchWindow;
        
        public static GWindow.BorderBatch GetBorderBatchWindow( bool showWindow = false )
        {
            //DebugLog.Write( "GodObjects.Windows.GetBorderBatchWindow()" );
            if( _BorderBatchWindow == null )
            {
                _BorderBatchWindow = new GWindow.BorderBatch();
                if( showWindow )
                    _BorderBatchWindow.Show();
            }
            else if( showWindow )
                _BorderBatchWindow.BringToFront();
            return _BorderBatchWindow;
        }
        
        public static void SetBorderBatchWindow( GWindow.BorderBatch window, bool closeOldWindow = true )
        {
            //DebugLog.Write( "GodObjects.Windows.SetBorderBatchWindow()" );
            if( _BorderBatchWindow == window )
                return;
            if( ( closeOldWindow )&&( _BorderBatchWindow != null ) )
                _BorderBatchWindow.Close();
            _BorderBatchWindow = window;
        }
        
        #endregion
        
        #region Sub-Division Batch Window
        
        static GWindow.SubDivisionBatch _SubDivisionBatchWindow;
        
        public static GWindow.SubDivisionBatch GetSubDivisionBatchWindow( bool showWindow = false )
        {
            //DebugLog.Write( "GodObjects.Windows.GetSubDivisionBatchWindow()" );
            if( _SubDivisionBatchWindow == null )
            {
                _SubDivisionBatchWindow = new GWindow.SubDivisionBatch();
                if( showWindow )
                    _SubDivisionBatchWindow.Show();
            }
            else if( showWindow )
                _SubDivisionBatchWindow.BringToFront();
            return _SubDivisionBatchWindow;
        }
        
        public static void SetSubDivisionBatchWindow( GWindow.SubDivisionBatch window, bool closeOldWindow = true )
        {
            //DebugLog.Write( "GodObjects.Windows.SetSubDivisionBatchWindow()" );
            if( _SubDivisionBatchWindow == window )
                return;
            if( ( closeOldWindow )&&( _SubDivisionBatchWindow != null ) )
                _SubDivisionBatchWindow.Close();
            _SubDivisionBatchWindow = window;
        }
        
        #endregion
        
        #region Render Window
        
        static GWindow.Render _RenderWindow;
        
        public static GWindow.Render GetRenderWindow( bool showWindow = false )
        {
            if( _MainWindow.InvokeRequired )
            {
                _MainWindow.Invoke( (Action)delegate() { GetRenderWindow( showWindow ); }, null );
                return null;
            }
            //DebugLog.Write( "GodObjects.Windows.GetRenderWindow()" );
            if( _RenderWindow == null )
            {
                _RenderWindow = new GWindow.Render();
                if( showWindow )
                    _RenderWindow.Show();
            }
            else if( showWindow )
                _RenderWindow.BringToFront();
            return _RenderWindow;
        }
        
        public static void SetRenderWindow( GWindow.Render window, bool closeOldWindow = true )
        {
            //DebugLog.Write( "GodObjects.Windows.SetRenderWindow()" );
            if( _RenderWindow == window )
                return;
            if( ( closeOldWindow )&&( _RenderWindow != null ) )
                _RenderWindow.Close();
            _RenderWindow = window;
        }
        
        #endregion
        
        public static void CloseAllChildWindows()
        {
            SetAboutWindow( null, true );
            SetOptionsWindow( null, true );
            SetBorderBatchWindow( null, true );
            SetSubDivisionBatchWindow( null, true );
            SetRenderWindow( null, true );
        }
        
        public static void SetEnableState( bool enabled )
        {
            if( _MainWindow != null )
                _MainWindow.SetEnableState( enabled );
            if( _BorderBatchWindow != null )
                _BorderBatchWindow.SetEnableState( enabled );
            if( _SubDivisionBatchWindow != null )
                _SubDivisionBatchWindow.SetEnableState( enabled );
            if( _RenderWindow != null )
                _RenderWindow.SetEnableState( enabled );
        }
        
    }
    
    #endregion
    
}

