using Vulkan;

namespace Inspector.Common
{
	public class Inspector
	{
		Instance Instance;

		public delegate void AppendTextDelegate (string text);
		public AppendTextDelegate AppendText { get; set; }

		public SurfaceKhr Surface { get; set; }

		public Inspector ()
		{
			Instance = new Instance ();
		}

		public void Inspect ()
		{
			if (AppendText == null)
				return;

			AppendText ("Vulkan surface created\n\n");

			int i = 0;
			foreach (var device in Instance.EnumeratePhysicalDevices ()) {
				var props = device.GetProperties ();
				AppendText (string.Format ("Physical device #{0}\n", i++));
				AppendText (string.Format ("\tID: {0}\n\tName: {1}\n\tType: {2}\n\tAPI version: {3}\n\tDriver version: {4}\n\tVendor ID: 0x{5:x}\n", props.DeviceId, props.DeviceName, props.DeviceType, Version.ToString (props.ApiVersion), props.DriverVersion, props.VendorId));

				var surfaceCaps = device.GetSurfaceCapabilitiesKHR (Surface);
				AppendText (string.Format ("\n\tSurface capabilities\n"));
				AppendText (string.Format ("\t\tImage count (min - max): {0} - {1}\n\t\tImage extent (min - max): {2}x{3} - {4}x{5}\n\t\tUsage flags: 0x{6:x}\n\t\tSupported transforms: 0x{7:x}\n\t\tSupported composite alpha flags: 0x{8:x}\n",
												surfaceCaps.MinImageCount, surfaceCaps.MaxImageCount,
												surfaceCaps.MinImageExtent.Width, surfaceCaps.MinImageExtent.Height,
												surfaceCaps.MaxImageExtent.Width, surfaceCaps.MaxImageExtent.Height,
												surfaceCaps.SupportedUsageFlags,
												surfaceCaps.SupportedTransforms,
												surfaceCaps.SupportedCompositeAlpha));
				ReportModes (device);
				AppendText (string.Format ("\n\tMemory properties\n"));
				var memProperties = device.GetMemoryProperties ();
				foreach (var memType in memProperties.MemoryTypes)
					AppendText (string.Format ("\t\tType HeapIndex: {0} Flags: 0x{1:X}\n", memType.HeapIndex, memType.PropertyFlags));
				foreach (var memHeap in memProperties.MemoryHeaps)
					AppendText (string.Format ("\t\tHeap Size: {0} Flags: 0x{1:X}\n", (ulong)memHeap.Size, memHeap.Flags));

				var layerProps = Commands.EnumerateInstanceLayerProperties ();
				if (layerProps != null) {
					AppendText ("\nGlobal layer properties\n");
					foreach (var prop in layerProps)
						AppendText (string.Format ("\tLayer {0} description: {1}\n", prop.LayerName, prop.Description));
				}
				else
					AppendText ("\nNo instance layer properties reported\n");

				var extProps = Commands.EnumerateInstanceExtensionProperties ();
				if (extProps != null) {
					AppendText ("\nGlobal extension properties\n");
					foreach (var prop in extProps)
						AppendText (string.Format ("\tExtension {0} version: {1}\n", prop.ExtensionName, prop.SpecVersion));
				}
				else
					AppendText ("\nNo instance extension properties reported\n");
			}
		}
		void ReportModes (PhysicalDevice device)
		{
			var modes = device.GetSurfacePresentModesKHR (Surface);

			if (modes == null)
				return;

			AppendText (string.Format ("\n\tSurface Present modes\n"));
			foreach (var mode in modes)
				AppendText (string.Format ("\t\tMode: {0}\n", mode));
		}
	}
}