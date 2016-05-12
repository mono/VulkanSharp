using System;
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
