using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace VulkanSharp.Generator
{
	public class Generator
	{
		XElement specTree;
		string specXMLFile;
		string outputPath;
		StreamWriter writer;

		public Generator (string filename, string outputDir)
		{
			specXMLFile = filename;
			outputPath = outputDir;
		}

		public void Run ()
		{
			LoadSpecification ();

			GenerateEnums ();
		}

		void LoadSpecification ()
		{
			specTree = XElement.Load (specXMLFile);

			if (specTree.Name != "registry")
				throw new Exception ("problem parsing the file, top element is not 'registry'");

			Console.WriteLine ("Specification file {0} loaded", specXMLFile);
		}

		static Dictionary<string, string> specialParts = new Dictionary<string, string> {
			{ "API", "Api" },
			{ "EXT", "Ext" },
			{ "KHR", "Khr" },
			{ "1D", "1D" },
			{ "2D", "2D" },
			{ "3D", "3D" },
		};

		string TranslateCName (string name)
		{
			StringWriter sw = new StringWriter ();
			bool first = true;

			foreach (var part in name.Split ('_')) {
				if (first) {
					first = false;
					if (name.StartsWith ("VK", StringComparison.OrdinalIgnoreCase))
						continue;
					else
						Console.WriteLine ("warning: name '{0}' doesn't start with VK prefix", name);
				}

				if (specialParts.ContainsKey (part))
					sw.Write (specialParts [part]);
				else if (part.Length > 0)
					sw.Write (char.ToUpper (part[0]) + part.Substring (1).ToLower ());
			}

			return sw.ToString ();
		}

		void WriteEnumField (XElement e, string csName)
		{
			var valueAttr = e.Attribute ("value");
			string value;
			if (valueAttr == null) {
				int pos = Convert.ToInt32 (e.Attribute ("bitpos").Value);
				value = string.Format ("0x{0:X}", 1 << pos);
			}
			else
				value = valueAttr.Value;

			string fName = TranslateCName (e.Attribute ("name").Value);
			string prefix = csName, suffix = null;
			if (prefix.EndsWith ("FlagBits")) {
				prefix = prefix.Substring (0, prefix.Length - 8);
				suffix = "Bit";
			}

			if (fName.StartsWith (prefix))
				fName = fName.Substring (prefix.Length);

			if (!char.IsLetter (fName [0])) {
				switch (csName) {
				case "ImageType":
					fName = "Image" + fName;
					break;
				case "ImageViewType":
					fName = "View" + fName;
					break;
				case "QueryResultFlagBits":
					fName = "Result" + fName;
					break;
				case "SampleCountFlagBits":
					fName = "Count" + fName;
					break;
				}
			}

			if (suffix != null && fName.EndsWith (suffix))
				fName = fName.Substring (0, fName.Length - suffix.Length);

			writer.WriteLine ("\t\t{0} = {1},", fName, value);
		}

		bool WriteEnum (XElement enumElement)
		{
			string name = enumElement.Attribute ("name").Value;
			string csName;

			if (name.StartsWith ("Vk"))
				csName = name.Substring (2);
			else {
				Console.WriteLine ("warning: enum name '{0}' doesn't start with Vk prefix", name);
				csName = name;
			}

			var values = from el in specTree.Elements ("enums")
					where (string)el.Attribute ("name") == name
				select el;

			if (values.Count () < 1) {
				Console.WriteLine ("warning: not adding empty enum {0}", csName);
				return false;
			}

			var enumsElement = values.First ();
			if (enumsElement.Attribute ("type") != null && enumsElement.Attribute ("type").Value == "bitmask")
				writer.WriteLine ("\t[Flags]");
			writer.WriteLine ("\tenum {0} : int\n\t{{", csName);

			foreach (var e in values.Elements ("enum"))
				WriteEnumField (e, csName);

			writer.WriteLine ("\t}");

			return true;
		}

		void GenerateEnums ()
		{
			writer = File.CreateText (string.Format ("{0}{1}Enums.cs", outputPath, Path.DirectorySeparatorChar));

			writer.WriteLine ("using System;\n");
			writer.WriteLine ("namespace Vulkan\n{");

			var enumTypes = from el in specTree.Elements ("types").Elements ("type")
			               where (string)el.Attribute ("category") == "enum"
			               select el;

			bool written = false;
			foreach (var e in enumTypes) {
				if (written)
					writer.WriteLine ();

				written = WriteEnum (e);
			}

			writer.WriteLine ("}");
			writer.Close ();
		}
	}
}
