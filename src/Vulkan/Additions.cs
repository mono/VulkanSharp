using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
	public partial class Instance
	{
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
		}

		public Instance () : this (new InstanceCreateInfo ())
		{
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
