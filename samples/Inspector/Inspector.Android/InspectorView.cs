using Android.Content;
using Android.Text.Method;
using Android.Widget;
using Vulkan;
using Vulkan.Android;

namespace Inspector
{
	public class InspectorView : VulkanView
	{
		TextView textView;
		bool inspectionDone = false;

		public InspectorView (Context context, TextView view) : base (context, CreateInstance ())
		{
			textView = view;
			textView.MovementMethod = new ScrollingMovementMethod();
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

			var inspector = new Common.Inspector { Surface = surface, AppendText = (string s) => textView.Append (s) };
			inspector.Inspect ();

			inspectionDone = true;
		}
	}
}

