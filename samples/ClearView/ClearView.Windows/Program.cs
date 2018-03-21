using System;
using System.Windows.Forms;
using ClearView.Common;
using VulkanSample.Windows;

namespace ClearView.Windows
{
	static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			var form = new Form ();
			form.Controls.Add (new VulkanSampleControl (new ClearViewSample ()) { Dock = DockStyle.Fill });

			Application.Run (form);
		}
	}
}
