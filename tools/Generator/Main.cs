using System;

namespace VulkanSharp.Generator
{
	public class MainClass
	{
		static public int Main ()
		{
			Generator gen = new Generator ("vk.xml", "Vulkan");

			gen.Run ();

			return 0;
		}
	}
}

