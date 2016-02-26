using System;
using System.Runtime.InteropServices;

namespace Vulkan.Interop
{
	[StructLayout (LayoutKind.Explicit)]
	internal struct ClearColorValue
	{
		[FieldOffset (0)] internal float Float32;
		[FieldOffset (0)] internal Int32 Int32;
		[FieldOffset (0)] internal UInt32 Uint32;
	}

	[StructLayout (LayoutKind.Explicit)]
	internal struct ClearValue
	{
		[FieldOffset (0)] internal IntPtr Color;
		[FieldOffset (0)] internal IntPtr DepthStencil;
	}
}
