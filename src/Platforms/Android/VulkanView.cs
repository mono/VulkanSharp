using System;
using Android.Content;
using Android.Views;

using Java.Interop;
using System.Runtime.InteropServices;

namespace Vulkan.Android
{
	public class VulkanView : SurfaceView, ISurfaceHolderCallback
	{
		public Instance Instance;
		protected IntPtr aNativeWindow = IntPtr.Zero;

		public VulkanView (Context context) : base (context)
		{
			Holder.AddCallback (this);
			CreateInstance ();
			SetWillNotDraw (false);
		}

		void AcquireNativeWindow (ISurfaceHolder holder)
		{
			if (aNativeWindow != IntPtr.Zero)
				NativeMethods.ANativeWindow_release (aNativeWindow);

			aNativeWindow = NativeMethods.ANativeWindow_fromSurface (JniEnvironment.EnvironmentPointer, Holder.Surface.Handle);
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			AcquireNativeWindow (holder);
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			if (aNativeWindow != IntPtr.Zero)
				NativeMethods.ANativeWindow_release (aNativeWindow);

			aNativeWindow = IntPtr.Zero;
		}

		public void SurfaceChanged (ISurfaceHolder holder, global::Android.Graphics.Format format, int w, int h)
		{
		}

		protected void CreateInstance ()
		{
			var appInfo = new ApplicationInfo ();
			appInfo.ApiVersion = Vulkan.Version.Make (1, 0, 0);

			var info = new InstanceCreateInfo ();
			info.ApplicationInfo = appInfo;
			info.EnabledExtensionCount = 2;
			info.EnabledExtensionNames = new string [] { "VK_KHR_surface", "VK_KHR_android_surface" };
			Instance = new Instance (info);
		}
	}

	internal static class NativeMethods
	{
		const string AndroidRuntimeLibrary = "android";

		[DllImport (AndroidRuntimeLibrary)]
		internal static unsafe extern IntPtr ANativeWindow_fromSurface (IntPtr jniEnv, IntPtr handle);

		[DllImport (AndroidRuntimeLibrary)]
		internal static unsafe extern void ANativeWindow_release (IntPtr window);
	}
}
