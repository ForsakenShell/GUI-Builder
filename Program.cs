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

namespace Border_Builder
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new bbMain());
		}
		
	}
}
