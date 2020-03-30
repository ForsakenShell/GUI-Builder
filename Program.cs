/*
 * Program.cs
 * 
 * Main entry point for Border Builder.
 * 
 * User: 1000101
 * Date: 24/11/2017
 * Time: 10:55 PM
 * 
 */
using System;
using System.Windows.Forms;


/// <summary>
/// Class with program entry point.
/// </summary>
internal sealed class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    [STAThread]
    static void Main( string[] args )
    {
        WorkerThreadPool.StartMethodBase = System.Reflection.MethodInfo.GetCurrentMethod();
        WorkerThreadPool.SetName( WorkerThreadPool.StartMethodBaseNameFriendly( true ) );

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault( false );

        if( !GodObject.Plugin.Initialize() )
        {
            MessageBox.Show( "Unable to find Fallout 4!\n\nMake sure you have the game installed correctly.", "GUIBuilder Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            return;
        }

        var bbPath = GodObject.Paths.BorderBuilder;
        if( string.IsNullOrEmpty( bbPath ) )
        {
            MessageBox.Show( string.Format( "Unable to find \"{0}\" folder.\n\nMake sure you have the GUIBuilder installed correctly.\n\n\"{0}\" should be placed in either the Fallout 4 folder or where GUIBuilder is installed to.", GUIBuilder.Constant.BorderBuilderPath ), "GUIBuilder Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            return;
        }

        var configFile = GodObject.Paths.GUIBuilderConfigFile;
        if( ( string.IsNullOrEmpty( configFile ) ) || ( !System.IO.File.Exists( configFile ) ) || ( GodObject.XmlConfig.WasReset ) )
        {
            try
            {
                Application.Run( new GUIBuilder.Windows.Options() );
            }
            catch( Exception e )
            {
                DebugLog.WriteError( string.Format( "An unhandled exception occured\n{0}", e.ToStringNullSafe() ) );
                goto localAbort;
            }
        }

        bool mirrorToConsole = GodObject.XmlConfig.ReadValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_MirrorToConsole, false );
        DebugLog.Open( mirrorToConsole );

        try
        {
            Application.Run( new GUIBuilder.Windows.Main() );
        }
        catch( Exception e )
        {
            DebugLog.WriteError( string.Format( "An unhandled exception occured\n{0}", e.ToStringNullSafe() ) );
        }

    localAbort:
        DebugLog.Close();
        if( GodObject.XmlConfig.ReadValue<bool>( GodObject.XmlConfig.XmlNode_Options, GodObject.XmlConfig.XmlKey_ZipLogs, true ) )
            DebugLog.ZipLogs( true );
    }
    
}
