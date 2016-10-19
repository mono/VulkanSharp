using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using VulkanSharp.Generator;

namespace VulkanSharp.DocMerge
{
	public class Injector : GeneratorBase
	{
		Dictionary<string, string> descriptions = new Dictionary<string, string> ();
		string vulkanDocsPath = "Vulkan-Docs/doc/specs/vulkan";
		string vulkanSharpDocsPath = "docs/en/Vulkan";

		public Injector ()
		{
			ReadDescriptions ();
		}

		void ReadDescriptions ()
		{
			var reader = new StreamReader (File.OpenRead (vulkanDocsPath + Path.DirectorySeparatorChar + "refDesc.py"));
			var regex = new Regex ("refDesc\\[\\'([^\\']+)\\'[^\\']+\\'([^\\']*)\\'");
			string line;

			while ((line = reader.ReadLine ()) != null) {
				var match = regex.Match (line);
				if (match != null) {
					if (match.Groups.Count != 3)
						continue;
					var name = GetTypeCsName (match.Groups [1].Value);
					var path = GetVulkanSharpDocPath (name);
					if (!File.Exists (path))
						name = GetEnumCsName (match.Groups [1].Value, match.Groups [1].Value.EndsWith ("FlagBits"));
					descriptions [name] = match.Groups [2].Value;
				}
			}
		}

		string GetVulkanSharpDocPath (string baseName)
		{
			return vulkanSharpDocsPath + Path.DirectorySeparatorChar + baseName + ".xml";
		}

		public void UpdateDocXml (string baseName)
		{
			var path = GetVulkanSharpDocPath (baseName);

			XDocument doc;

			try {
				doc = XDocument.Load (path);
			} catch {
				Console.WriteLine ("warning: didn't find documentation for {0}", baseName);
				return;
			}

			var typeElement = doc.Element ("Type");
			if (typeElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain Type element", path);
				return;
			}

			var docsElement = typeElement.Element ("Docs");
			if (docsElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain Docs element", path);
				return;
			}

			var summaryElement = docsElement.Element ("summary");
			if (summaryElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain summary element", path);
				return;
			}

			if (!descriptions.ContainsKey (baseName)) {
				Console.WriteLine ("warning: known descriptions doesn't contain info for {0}", baseName);
				return;
			}
			summaryElement.Value = descriptions [baseName];
			doc.Save (path);
		}

		public void UpdateDocXml ()
		{
			foreach (var name in descriptions.Keys)
				UpdateDocXml (name);
		}
	}
}

