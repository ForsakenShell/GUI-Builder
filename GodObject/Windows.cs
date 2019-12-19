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
using GUIBuilder.Windows;

using AnnexTheCommonwealth;

using XeLib;
using XeLib.API;


namespace GodObject
{
    
    public static class Windows
    {

        static List<IEnableControlForm>_Forms = null;

        static TWindow                  FindAndReplaceWindow<TWindow>( TWindow newWindow, bool closeOldWindow ) where TWindow : Form, IEnableControlForm
        {
            if( _Forms.NullOrEmpty() )
                _Forms = new List<IEnableControlForm>();
            for( int i = 0; i < _Forms.Count; i++ )
            {
                var oldWindow = _Forms[ i ] as TWindow;
                if( oldWindow != null )
                {
                    if( ( newWindow != null )||( closeOldWindow ) )
                    {
                        if( closeOldWindow )
                            oldWindow.Close();
                        if( newWindow == null )
                            _Forms.RemoveAt( i );
                        else
                            _Forms[ i ] = newWindow;
                        return newWindow;
                    }
                    return oldWindow;
                }
            }
            if( newWindow != null )
                _Forms.Add( newWindow );
            return newWindow;
        }

        public static TWindow           GetWindow<TWindow>( bool showWindow = false ) where TWindow : Form, IEnableControlForm, new()
        {
            //DebugLog.Write( "GodObjects.Windows.GetWindow()" );
            var window = FindAndReplaceWindow<TWindow>( null, false );
            if( window == null )
            {
                window = new TWindow();
                _Forms.Add( window );
                if( showWindow )
                    window.Show();
            }
            else if( showWindow )
                window.BringToFront();
            return window;
        }

        public static void              SetWindow<TWindow>( TWindow newWindow, bool closeOldWindow = true ) where TWindow : Form, IEnableControlForm
        {
            //DebugLog.Write( "GodObjects.Windows.SetWindow()" );
            FindAndReplaceWindow<TWindow>( newWindow, closeOldWindow );
        }

        public static void              CloseAllChildWindows()
        {
            if( _Forms.NullOrEmpty() )
                return;
            for( int i = _Forms.Count - 1; i >= 0; i-- )
            {
                if( !( _Forms[ i ] is GUIBuilder.Windows.Main ) )
                {
                    _Forms[ i ].Close();
                    _Forms.RemoveAt( i );
                }
            }
        }

        public static void              SetEnableState( bool enabled )
        {
            if( _Forms.NullOrEmpty() )
                return;
            for( int i = 0; i < _Forms.Count; i++ )
                _Forms[ i ].SetEnableState( enabled );
        }

        #region TODO:  Move this somewhere more appropriate

        const string                    XmlKey_SDLVideoDriver                   = "SDLVideoDriver";

        static public readonly string SDLVideoDriverDefault = "Default";
        static public readonly string SDLVideoDriverSoftware = "Software";
        static public readonly int SDLVideoDriverDefaultIndex = 0;
        static public readonly string[] SDLVideoDrivers = new [] { SDLVideoDriverDefault, "Direct3D", "OpenGL", "OpenGLES", "OpenGLES2", "Metal", SDLVideoDriverSoftware };

        static int GetDriverIndexFromName( string name )
        {
            if( string.IsNullOrEmpty( name ) ) return SDLVideoDriverDefaultIndex;
            for( int i = 0; i < SDLVideoDrivers.Length; i++ )
                if( name.InsensitiveInvariantMatch( SDLVideoDrivers[ i ] ) ) return i;
            return SDLVideoDriverDefaultIndex;
        }

        static public string SDLVideoDriver
        {
            get
            {
                return GodObject.XmlConfig.ReadValue<string>( XmlConfig.XmlNode_Options, XmlKey_SDLVideoDriver, SDLVideoDriverDefault );
            }
            set
            {
                int i = GetDriverIndexFromName( value );
                GodObject.XmlConfig.WriteValue<string>( XmlConfig.XmlNode_Options, XmlKey_SDLVideoDriver, SDLVideoDrivers[ i ], true );
            }
        }
        static public int SDLVideoDriverIndex
        {
            get
            {
                return GetDriverIndexFromName( SDLVideoDriver );
            }
            set
            {
                SDLVideoDriver = SDLVideoDrivers[
                    ( value < 0 ) || ( value > SDLVideoDrivers.Length )
                    ? SDLVideoDriverDefaultIndex
                    : value
                ];
            }
        }

        #endregion

    }

}

