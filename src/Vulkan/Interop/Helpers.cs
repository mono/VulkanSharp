using System;
using System.Runtime.InteropServices;

namespace Vulkan.Interop
{
	internal class Structure
	{
		internal static Vulkan.NativePointer Allocate (Type type)
		{
			return new NativePointer (new NativeReference (Marshal.SizeOf (type), true));
		}

		unsafe internal static void MarshalFixedSizeString (byte* dst, string src, int size)
		{
			var bytes = System.Text.UTF8Encoding.UTF8.GetBytes (src);
			size = Math.Min (size - 1, bytes.Length);
			int i;
			for (i = 0; i < size; i++)
				dst [i] = bytes[i];
			dst [i] = 0;
		}

		internal static object MarshalPointerToObject (IntPtr ptr, Type type)
		{
			if (ptr == IntPtr.Zero)
				return null;

			return Marshal.PtrToStructure (ptr, type);
		}

		internal static IntPtr MarshalObjectToPointer (IntPtr ptr, object value)
		{
			if (value == null) {
				if (ptr != IntPtr.Zero)
					Marshal.FreeHGlobal (ptr);
				return IntPtr.Zero;
			} else {
				if (ptr == IntPtr.Zero)
					ptr = Marshal.AllocHGlobal (Marshal.SizeOf (value));
				Marshal.StructureToPtr (value, ptr, false);
			}

			return ptr;
		}
	}
}
