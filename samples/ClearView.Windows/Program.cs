using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Samples.Common;
using Samples.Windows;
using Vulkan.Windows;

namespace ClearView.Windows
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form();
            form.Controls.Add(new VulkanControlSample(new ClearSample()) {Dock = DockStyle.Fill});

            Application.Run(form);
        }
    }
}
