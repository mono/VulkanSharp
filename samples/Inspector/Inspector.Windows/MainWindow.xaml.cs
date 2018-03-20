using System.Windows;
using InspectorWin;
using Vulkan;
using Vulkan.Windows;

namespace Inspector.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow ()
		{
			InitializeComponent ();

			var hWnd = new System.Windows.Interop.WindowInteropHelper (this).EnsureHandle ();
			var hInstance = System.Runtime.InteropServices.Marshal.GetHINSTANCE (typeof (App).Module);
			var instance = new Instance (new InstanceCreateInfo { EnabledExtensionNames = new string [] { "VK_KHR_surface", "VK_KHR_win32_surface" } });
			var surface = instance.CreateWin32SurfaceKHR (new Win32SurfaceCreateInfoKhr { Hwnd = hWnd, Hinstance = hInstance });

			var inspector = new Common.Inspector { AppendText = (string s) => { textBox.Text += s; }, Surface = surface };

			inspector.Inspect ();
		}
	}
}
