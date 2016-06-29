using Android.Widget;
using Vulkan;
using Vulkan.Android;

namespace Inspector
{
	public class InspectorView : VulkanView
	{
		TextView textView;
		bool inspectionDone = false;

		public InspectorView (Android.Content.Context context, TextView view) : base (context, CreateInstance ())
		{
			textView = view;
			textView.MovementMethod = new Android.Text.Method.ScrollingMovementMethod ();
			textView.Append ("Vulkan instance created\n\n");
		}

		protected override void NativeWindowAcquired ()
		{
			if (!inspectionDone)
				InspectVulkan ();
		}

		static Instance CreateInstance ()
		{
			return new Instance (new InstanceCreateInfo () {
				ApplicationInfo = new Vulkan.ApplicationInfo () {
					ApplicationName = "Vulkan Android Inspector",
					ApiVersion = Version.Make (1, 0, 0)
				},
				EnabledExtensionNames = new string [] { "VK_KHR_surface", "VK_KHR_android_surface" }
			});
		}

		void InspectVulkan ()
		{
			var surface = Instance.CreateAndroidSurfaceKHR (new AndroidSurfaceCreateInfoKhr () {
				Window = aNativeWindow
			});

			textView.Append ("Vulkan Android surface created\n\n");

			int i = 0;
			foreach (var device in Instance.EnumeratePhysicalDevices ()) {
				var props = device.GetProperties ();
				textView.Append (string.Format ("Physical device #{0}\n", i++));
				textView.Append (string.Format ("\tID: {0}\n\tName: {1}\n\tType: {2}\n\tAPI version: {3}\n\tDriver version: {4}\n\tVendor ID: 0x{5:x}\n", props.DeviceID, props.DeviceName, props.DeviceType, Version.ToString (props.ApiVersion), props.DriverVersion, props.VendorID));

				var surfaceCaps = device.GetSurfaceCapabilitiesKHR (surface);
				textView.Append (string.Format ("\n\tSurface capabilities\n"));
				textView.Append (string.Format ("\t\tImage count (min - max): {0} - {1}\n\t\tImage extent (min - max): {2}x{3} - {4}x{5}\n\t\tUsage flags: 0x{6:x}\n\t\tSupported transforms: 0x{7:x}\n\t\tSupported composite alpha flags: 0x{8:x}",
				                                surfaceCaps.MinImageCount, surfaceCaps.MaxImageCount,
				                                surfaceCaps.MinImageExtent.Width, surfaceCaps.MinImageExtent.Height,
				                                surfaceCaps.MaxImageExtent.Width, surfaceCaps.MaxImageExtent.Height,
				                                surfaceCaps.SupportedUsageFlags,
				                                surfaceCaps.SupportedTransforms,
				                                surfaceCaps.SupportedCompositeAlpha));

				textView.Append (string.Format ("\n\tMemory properties\n"));
				var memProperties = device.GetMemoryProperties ();
				foreach (var memType in memProperties.MemoryTypes)
					textView.Append (string.Format ("\t\tType HeapIndex: {0} Flags: 0x{1:X}\n", memType.HeapIndex, memType.PropertyFlags));
				foreach (var memHeap in memProperties.MemoryHeaps)
					textView.Append (string.Format ("\t\tHeap Size: {0} Flags: 0x{1:X}\n", (ulong)memHeap.Size, memHeap.Flags));
			}

			inspectionDone = true;
		}
	}
}

