using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Samples.Common;
using VulkanSample.Windows;
using XLogo.Common;

namespace XLogo.Windows
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
			form.Controls.Add (new VulkanSampleControl (new XLogoSample ()) { Dock = DockStyle.Fill });

			Application.Run (form);
		}
	}
}
