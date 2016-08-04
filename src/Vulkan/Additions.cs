using System;
using System.Runtime.InteropServices;
using Vulkan.Interop;

namespace Vulkan
{
	public partial class Instance
	{
		NativeMethods.vkCreateDebugReportCallbackEXT vkCreateDebugReportCallbackEXT;
		NativeMethods.vkDestroyDebugReportCallbackEXT vkDestroyDebugReportCallbackEXT;
		NativeMethods.vkDebugReportMessageEXT vkDebugReportMessageEXT;

		Delegate GetMethod (string name, Type type)
		{
			var funcPtr = GetProcAddr (name);

			if (funcPtr == IntPtr.Zero)
				return null;

			return Marshal.GetDelegateForFunctionPointer (funcPtr, type);
		}

		void InitializeFunctions ()
		{

			vkCreateDebugReportCallbackEXT = (NativeMethods.vkCreateDebugReportCallbackEXT)GetMethod ("vkCreateDebugReportCallbackEXT", typeof (NativeMethods.vkCreateDebugReportCallbackEXT));
			vkDestroyDebugReportCallbackEXT = (NativeMethods.vkDestroyDebugReportCallbackEXT)GetMethod ("vkDestroyDebugReportCallbackEXT", typeof (NativeMethods.vkDestroyDebugReportCallbackEXT));
			vkDebugReportMessageEXT = (NativeMethods.vkDebugReportMessageEXT)GetMethod ("vkDebugReportMessageEXT", typeof (NativeMethods.vkDebugReportMessageEXT));
		}

		public Instance (InstanceCreateInfo CreateInfo, AllocationCallbacks Allocator = null)
		{
			Result result;

			unsafe {
				fixed (IntPtr* ptrInstance = &m) {
					result = Interop.NativeMethods.vkCreateInstance (CreateInfo.m, Allocator != null ? Allocator.m : null, ptrInstance);
				}
			}

			if (result != Result.Success)
				throw new ResultException (result);

			InitializeFunctions ();
		}

		public Instance () : this (new InstanceCreateInfo ())
		{
		}

		public delegate Bool32 DebugReportCallback (DebugReportFlagsExt flags, DebugReportObjectTypeExt objectType, ulong objectHandle, IntPtr location, int messageCode, IntPtr layerPrefix, IntPtr message, IntPtr userData);

		public void EnableDebug (DebugReportCallback d, DebugReportFlagsExt flags = DebugReportFlagsExt.Debug | DebugReportFlagsExt.Error | DebugReportFlagsExt.Information | DebugReportFlagsExt.PerformanceWarning | DebugReportFlagsExt.Warning)
		{
			var debugCreateInfo = new DebugReportCallbackCreateInfoExt () {
				Flags = flags,
				PfnCallback = Marshal.GetFunctionPointerForDelegate (d)
			};

			CreateDebugReportCallbackEXT (debugCreateInfo);
		}
	}

	unsafe public partial class ShaderModuleCreateInfo
	{
		public byte[] CodeBytes {
			set {
				/* todo free allocated memory when already set */
				if (value == null) {
					m->CodeSize = UIntPtr.Zero;
					m->Code = IntPtr.Zero;
					return;
				}
				m->CodeSize = (UIntPtr)value.Length;
				m->Code = Marshal.AllocHGlobal (value.Length);
				Marshal.Copy (value, 0, m->Code, value.Length);
			}
		}
	}

	public partial class Device
	{
		public ShaderModule CreateShaderModule (byte[] shaderCode, uint flags = 0, AllocationCallbacks allocator = null)
		{
			ShaderModuleCreateInfo createInfo = new ShaderModuleCreateInfo {
				CodeBytes = shaderCode,
				Flags = flags
			};
			return CreateShaderModule (createInfo, allocator);
		}
	}

	unsafe public partial class ClearColorValue
	{
		public ClearColorValue (float[] floatArray) : this ()
		{
			Float32 = floatArray;
		}

		public ClearColorValue (int[] intArray) : this ()
		{
			Int32 = intArray;
		}

		public ClearColorValue (uint[] uintArray) : this ()
		{
			Uint32 = uintArray;
		}
	}
}
