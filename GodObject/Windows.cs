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

        static IEnableControlForm FindWindow( Type type )
        {
            if( _Forms.NullOrEmpty() ) return null;
            for( int i = 0; i < _Forms.Count; i++ )
                if( _Forms[ i ].GetType() == type )
                    return _Forms[ i ];
            return null;
        }

        static TWindow                  FindWindow<TWindow>() where TWindow : Form, IEnableControlForm
        {
            return FindWindow( typeof( TWindow ) ) as TWindow;
        }

        public static TWindow           GetWindow<TWindow>( bool showWindow = false ) where TWindow : Form, IEnableControlForm, new()
        {
            //DebugLog.Write( "GodObjects.Windows.GetWindow()" );
            var window = FindWindow<TWindow>();
            if( window == null )
            {
                window = new TWindow();
                _Forms = _Forms ?? new List<IEnableControlForm>();
                _Forms.Add( window );
                if( showWindow )
                    window.Show();
            }
            else if( showWindow )
                window.BringToFront();
            return window;
        }

        public static void              SetWindow<TWindow>( TWindow newWindow, bool closeOldWindow = false ) where TWindow : Form, IEnableControlForm, new()
        {
            //DebugLog.Write( "GodObjects.Windows.GetWindow()" );
            var window = FindWindow<TWindow>();
            if( window == null )
            {
                if( newWindow == null ) return;
                _Forms = _Forms ?? new List<IEnableControlForm>();
                _Forms.Add( newWindow );
            }
            else
            {
                if( closeOldWindow )
                {
                    window.Close();
                    //window.Dispose();
                }
                _Forms.Remove( window );
                if( newWindow == null )
                {
                    if( _Forms.NullOrEmpty() )
                        _Forms = null;
                    return;
                }
                _Forms.Add( newWindow );
            }
        }

        public static void              ClearWindow<TWindow>( bool closeOldWindow = false ) where TWindow : Form, IEnableControlForm
        {
            var window = FindWindow<TWindow>();
            if( window == null ) return;
            if( closeOldWindow )
            {
                window.Close();
                //window.Dispose();
            }
            if( _Forms != null )
                _Forms.Remove( window );
            if( _Forms.NullOrEmpty() )
                _Forms = null;
        }

        public static void              ClearWindow( Type type, bool closeOldWindow = false )
        {
            var window = FindWindow( type );
            if( window == null ) return;
            if( closeOldWindow )
            {
                window.Close();
                //window.Dispose();
            }
            if( _Forms != null )
                _Forms.Remove( window );
            if( _Forms.NullOrEmpty() )
                _Forms = null;
        }

        public static void              CloseAllChildWindows()
        {
            if( _Forms.NullOrEmpty() ) return;
            var forms = _Forms;
            _Forms = null;  // Temporarily set a null list while closing child windows (prevents closing windows from removing the window in ClearWindow())
            for( int i = forms.Count - 1; i >= 0; i-- )
            {
                var form = forms[ i ];
                if( !( form is GUIBuilder.Windows.Main ) )
                {
                    form.Close();
                    forms.RemoveAt( i );
                }
            }
            _Forms = forms;  // Should now only be the main window
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

