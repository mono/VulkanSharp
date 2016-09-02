using System;

namespace Vulkan.Windows
{
	public struct SecurityAttributes
	{
		public int Length;
		public IntPtr SecurityDescriptor;
		public int InheritHandle;
	}
}
