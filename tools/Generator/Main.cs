using System;
using System.IO;

namespace VulkanSharp.Generator
{
	public class MainClass
	{
		static public int Main (string[] args)
		{
			var srcFile = "vk.xml";
			var destDir = "Vulkan";
			if (args.Length > 0)
				srcFile = args [0];
			if (args.Length > 1)
				destDir = args [1];

			Directory.CreateDirectory (destDir);

			Generator gen = new Generator (srcFile, destDir);

			gen.Run ();

			return 0;
		}
	}
}

