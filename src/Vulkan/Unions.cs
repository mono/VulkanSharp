using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
	[StructLayout (LayoutKind.Explicit)]
	public struct ClearColorValue
	{
		[FieldOffset (0)] public float Float32;
		[FieldOffset (0)] public Int32 Int32;
		[FieldOffset (0)] public UInt32 Uint32;
	}

	[StructLayout (LayoutKind.Explicit)]
	public struct ClearValue
	{
		[FieldOffset (0)] public ClearColorValue Color;
		[FieldOffset (0)] public ClearDepthStencilValue DepthStencil;
	}
}
