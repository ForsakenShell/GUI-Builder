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
        System.Threading.Thread.CurrentThread.Name = "Main";
        DebugLog.OpenIndentLevel();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault( false );
        Application.Run( new GUIBuilder.Windows.Main() );
        DebugLog.CloseIndentLevel();
        DebugLog.Close();
    }
    
}
