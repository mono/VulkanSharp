using System;

namespace VulkanSharp.DocMerge
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			var injector = new Injector ();
			injector.UpdateDocXml ();
		}
	}
}
