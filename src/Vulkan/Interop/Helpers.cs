using System;
using System.Runtime.InteropServices;

namespace Vulkan.Interop
{
	internal class Structure
	{
		internal static IntPtr Allocate (Type type)
		{
			int size = Marshal.SizeOf (type);
			IntPtr ptr = Marshal.AllocHGlobal (size);
			unsafe {
				byte* bptr = (byte*) ptr.ToPointer ();
				for (int i = 0; i < size; i++)
					bptr[i] = 0;
			}

			return ptr;
		}
	}
}
