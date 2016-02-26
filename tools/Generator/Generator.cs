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
		bool isUnion;
		bool isInterop;

		Dictionary<string, string> typesTranslation = new Dictionary<string, string> ();
		HashSet<string> structures = new HashSet<string> ();
		HashSet<string> handles = new HashSet<string> ();

		public Generator (string filename, string outputDir)
		{
			specXMLFile = filename;
			outputPath = outputDir;
		}

		public void Run ()
		{
			LoadSpecification ();
			Directory.CreateDirectory ("Interop");

			GenerateEnums ();
			GenerateBitmasks ();
			GenerateHandles ();
			LearnStructsAndUnions ();
			GenerateStructs ();
			GenerateUnions ();
			GenerateCommands ();
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

		// TODO: validate this mapping
		static Dictionary<string, string> basicTypesMap = new Dictionary<string, string> {
			{ "int32_t", "Int32" },
			{ "uint32_t", "UInt32" },
			{ "uint64_t", "UInt64" },
			{ "uint8_t", "Byte" },
			{ "size_t", "UIntPtr" },
			{ "xcb_connection_t", "IntPtr" },
			{ "xcb_window_t", "IntPtr" },
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
				{
					if (part.ToCharArray ().All (c => char.IsUpper (c) || char.IsDigit (c)))
						sw.Write (part[0] + part.Substring (1).ToLower ());
					else
						sw.Write (char.ToUpper (part[0]) + part.Substring (1));
				}
			}

			return sw.ToString ();
		}

		string GetTypeCsName (string name, string typeName = "type")
		{
			if (typesTranslation.ContainsKey (name))
				return typesTranslation [name];

			string csName;

			if (name.StartsWith ("Vk"))
				csName = name.Substring (2);
			else if (name.EndsWith ("_t"))
			{
				if (!basicTypesMap.ContainsKey (name))
					throw new NotImplementedException (string.Format ("Mapping for the basic type {0} isn't supported", name));

				csName = basicTypesMap[name];
			}
			else
			{
				Console.WriteLine ("warning: {0} name '{1}' doesn't start with Vk prefix or end with _t suffix", typeName, name);
				csName = name;
			}

			foreach (var ext in extensions)
				if (csName.EndsWith (ext.Key))
					csName = csName.Substring (0, csName.Length - ext.Value.Length) + ext.Value;

			return csName;
		}

		static Dictionary<string, string> extensions = new Dictionary<string, string> {
			{ "EXT", "Ext" },
			{ "KHR", "Khr" }
		};

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

			foreach (var ext in extensions)
				if (prefix.EndsWith (ext.Value)) {
					prefix = prefix.Substring (0, prefix.Length - ext.Value.Length);
					suffix = ext.Value;
				}

			if (prefix.EndsWith ("Flags")) {
				prefix = prefix.Substring (0, prefix.Length - 5);
				suffix = "Bit" + suffix;
			}

			if (fName.StartsWith (prefix, StringComparison.OrdinalIgnoreCase))
				fName = fName.Substring (prefix.Length);

			if (!char.IsLetter (fName [0])) {
				switch (csName) {
				case "ImageType":
					fName = "Image" + fName;
					break;
				case "ImageViewType":
					fName = "View" + fName;
					break;
				case "QueryResultFlags":
					fName = "Result" + fName;
					break;
				case "SampleCountFlags":
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
			string csName = GetTypeCsName (name, "enum");

			var values = from el in specTree.Elements ("enums")
					where (string)el.Attribute ("name") == name
				select el;

			if (values.Count () < 1) {
				Console.WriteLine ("warning: not adding empty enum {0}", csName);
				return false;
			}

			var enumsElement = values.First ();
			if (enumsElement.Attribute ("type") != null && enumsElement.Attribute ("type").Value == "bitmask") {
				string suffix = null;
				foreach (var ext in extensions)
					if (csName.EndsWith (ext.Value)) {
						suffix = ext.Value + suffix;
						csName = csName.Substring (0, csName.Length - ext.Value.Length);
					}
				writer.WriteLine ("\t[Flags]");
				if (csName.EndsWith ("FlagBits"))
					csName = csName.Substring (0, csName.Length - 4) + "s";
				if (suffix != null)
					csName += suffix;
			}

			typesTranslation [name] = csName;
			writer.WriteLine ("\tpublic enum {0} : int\n\t{{", csName);

			foreach (var e in values.Elements ("enum"))
				WriteEnumField (e, csName);

			writer.WriteLine ("\t}");

			return true;
		}

		void GenerateCodeForElements (IEnumerable<XElement> elements, Func<XElement, bool> generator)
		{
			bool written = false;
			foreach (var e in elements)
			{
				if (written)
					writer.WriteLine ();

				written = generator (e);
			}
		}

		void GenerateMembers (XElement parent, Func<XElement, bool> generator)
		{
			GenerateCodeForElements (parent.Elements ("member"), generator);
		}

		void GenerateType (string type, Func<XElement, bool> generator)
		{
			var elements = from el in specTree.Elements ("types").Elements ("type")
						   where (string)el.Attribute ("category") == type
						   select el;

			GenerateCodeForElements (elements, generator);
		}

		void CreateFile (string typeName, bool usingInterop = false, string nspace = "Vulkan", string subDirectory = null)
		{
			string path = subDirectory != null ? string.Format ("{0}{1}{2}", outputPath, Path.DirectorySeparatorChar, subDirectory) : outputPath;
			string filename = string.Format ("{0}{1}{2}.cs", path, Path.DirectorySeparatorChar, typeName);
			writer = File.CreateText (filename);

			writer.WriteLine ("using System;");
			if (usingInterop)
				writer.WriteLine ("using System.Runtime.InteropServices;");
			writer.WriteLine ();

			writer.WriteLine ("namespace {0}\n{{", nspace);
		}

		void FinalizeFile ()
		{
			writer.WriteLine ("}");
			writer.Close ();
		}

		void GenerateEnums ()
		{
			CreateFile ("Enums");
			GenerateType ("enum", WriteEnum);
			FinalizeFile ();
		}

		bool AddBitmask (XElement typeElement)
		{
			if (typeElement.Attribute ("requires") != null)
				return false;

			string name = typeElement.Element ("name").Value;

			typesTranslation [name] = "UInt32";

			return false;
		}

		void GenerateBitmasks ()
		{
			GenerateType ("bitmask", AddBitmask);
		}

		#region Structs generation

		bool WriteMember (XElement memberElement)
		{
			var parentName = memberElement.Parent.Attribute ("name").Value;

			var typeElement = memberElement.Element ("type");
			if (typeElement == null)
			{
				Console.WriteLine ("warning: a member of the struct {0} doesn't have a 'type' node", parentName);
				return false;
			}
			var csMemberType = GetTypeCsName (typeElement.Value, "member");

			var nameElement = memberElement.Element ("name");
			if (nameElement == null)
			{
				Console.WriteLine ("warning: a member of the struct {0} doesn't have a 'name' node", parentName);
				return false;
			}

			string name = nameElement.Value;

			if (!isInterop && csMemberType == "StructureType" && name == "sType")
				return false;

			bool isPointer = memberElement.Value.Contains (typeElement.Value + "*");
			if (isPointer) {
				switch (csMemberType) {
				case "void":
					if (!isInterop && name == "pNext")
						return false;
					csMemberType = "IntPtr";
					break;
				case "char":
					csMemberType = "string";
					break;
				}
				if (name.StartsWith ("p"))
					name = name.Substring (1);
			}
			var csMemberName = TranslateCName (name);

			// TODO: fixed arrays of structs
			if (csMemberName.EndsWith ("]")) {
				string array = csMemberName.Substring (csMemberName.IndexOf ('['));
				csMemberName = csMemberName.Substring (0, csMemberName.Length - array.Length);
				// temporarily disable arrays csMemberType += "[]";
			}

			string mod = "";
			if (csMemberName.EndsWith ("]"))
				mod = "unsafe fixed ";

			if (csMemberType.EndsWith ("Flags"))
				csMemberType = "UInt32";

			if (csMemberType.EndsWith ("FlagBits"))
				csMemberType = csMemberType.Substring (0, csMemberType.Length - 4) + "s";

			if (csMemberType.StartsWith ("PFN_"))
				csMemberType = "IntPtr";

			if (csMemberType == "SampleMask")
				csMemberType = "UInt32";

			if (csMemberType == "Bool32" && !isInterop)
				csMemberType = "bool";

			string attr = "";
			string sec = isInterop ? "internal" : "public";
			if (isUnion)
				attr = "[FieldOffset (0)] ";

			if (isInterop) {
				if (structures.Contains (csMemberType) || handles.Contains (csMemberType) || csMemberType == "string")
					csMemberType = "IntPtr";
				writer.WriteLine ("\t\t{0}{1} {2}{3} {4};", attr, sec, mod, csMemberType, csMemberName);
			} else {
				if (structures.Contains (csMemberType) || handles.Contains (csMemberType)) {
					writer.WriteLine ("\t\t{0} l{1};", csMemberType, csMemberName);
					writer.WriteLine ("\t\t{0} {1} {2} {{", sec, csMemberType, csMemberName);
					writer.WriteLine ("\t\t\tget {{ return l{0}; }}", csMemberName);
					writer.WriteLine ("\t\t\tset {{ l{0} = value; m->{0} = (IntPtr) value.m; }}", csMemberName);
					writer.WriteLine ("\t\t}");
				} else if (csMemberType == "string") {
					writer.WriteLine ("\t\t{0} {1} {2} {{", sec, csMemberType, csMemberName);
					writer.WriteLine ("\t\t\tget {{ return Marshal.PtrToStringAnsi (m->{0}); }}", csMemberName);
					writer.WriteLine ("\t\t\tset {{ m->{0} = Marshal.StringToHGlobalAnsi (value); }}", csMemberName);
					writer.WriteLine ("\t\t}");
				} else {
					writer.WriteLine ("\t\tpublic {0} {1} {{", csMemberType, csMemberName);
					writer.WriteLine ("\t\t\tget {{ return m->{0}; }}", csMemberName);
					writer.WriteLine ("\t\t\tset {{ m->{0} = value; }}", csMemberName);
					writer.WriteLine ("\t\t}");
				}
			}

			return !isInterop;
		}

		HashSet<string> disabledStructs = new HashSet<string> {
			"XlibSurfaceCreateInfoKhr",
			"XcbSurfaceCreateInfoKhr",
			"WaylandSurfaceCreateInfoKhr",
			"MirSurfaceCreateInfoKhr",
			"AndroidSurfaceCreateInfoKhr",
			"Win32SurfaceCreateInfoKhr",
		};

		HashSet<string> disabledStructureTypeEnumValues = new HashSet<string> {
			"DisplayModeCreateInfoKhr",
			"DisplaySurfaceCreateInfoKhr",
			"DisplayPresentInfoKhr",
			"SwapchainCreateInfoKhr",
			"PresentInfoKhr",
			"DebugReportCallbackCreateInfoExt"
		};

		bool WriteStructOrUnion (XElement structElement)
		{
			string name = structElement.Attribute ("name").Value;
			if (!typesTranslation.ContainsKey (name))
				return false;

			string csName = typesTranslation [name];

			string mod = "";
			if (isUnion && isInterop)
				writer.WriteLine ("\t[StructLayout (LayoutKind.Explicit)]");
			if (!isInterop)
				mod = "unsafe ";
			writer.WriteLine ("\t{0}{1} {2} {3}\n\t{{", mod, isInterop ? "internal" : "public", isInterop ? "struct" : "class", csName);

			if (!isInterop) {
				bool hasSType = false;
				var values = from el in structElement.Elements ("member")
						where (string)el.Element ("name") == "sType"
					select el;
				foreach (var el in values) {
					var elType = el.Element ("type");
					if (elType != null && elType.Value == "VkStructureType")
						hasSType = true;
				}

				writer.WriteLine ("\t\tinternal Interop.{0}* m;\n", csName);
				writer.WriteLine ("\t\tpublic {0} ()", csName);
				writer.WriteLine ("\t\t{");
				writer.WriteLine ("\t\t\tm = (Interop.{0}*) Interop.Structure.Allocate (typeof (Interop.{0}));", csName);
				if (hasSType && !disabledStructureTypeEnumValues.Contains (csName) /* todo: handle extexnsions - StructureType should contain extensions values */) {
					writer.WriteLine ("\t\t\tm->SType = StructureType.{0};", csName);
				}
				writer.WriteLine ("\t\t}\n");
			}

			GenerateMembers (structElement, WriteMember);

			writer.WriteLine ("\t}");

			return true;
		}

		bool LearnStructure (XElement structElement)
		{
			string name = structElement.Attribute ("name").Value;
			string csName = GetTypeCsName (name, "struct");

			if (disabledStructs.Contains (csName))
				return false;

			typesTranslation [name] = csName;
			structures.Add (csName);

			return false;
		}

		void LearnStructsAndUnions ()
		{
			GenerateType ("struct", LearnStructure);
			GenerateType ("union", LearnStructure);
		}

		void GenerateStructs ()
		{
			CreateFile ("Structs", true);
			isUnion = false;
			GenerateType ("struct", WriteStructOrUnion);
			FinalizeFile ();

			CreateFile ("MarshalStructs", false, "Vulkan.Interop", "Interop");
			isUnion = false;
			isInterop = true;
			GenerateType ("struct", WriteStructOrUnion);
			isInterop = false;
			FinalizeFile ();
		}

		#endregion

		void GenerateUnions ()
		{
			CreateFile ("Unions");
			isUnion = true;
			GenerateType ("union", WriteStructOrUnion);
			FinalizeFile ();

			CreateFile ("MarshalUnions", true, "Vulkan.Interop", "Interop");
			isInterop = true;
			GenerateType ("union", WriteStructOrUnion);
			isInterop = false;
			isUnion = false;
			FinalizeFile ();
		}

		bool WriteHandle(XElement handleElement)
		{
			string name = handleElement.Element ("name").Value;
			string csName = GetTypeCsName (name, "struct");
			handles.Add (csName);

			writer.WriteLine ("\tpublic class {0}\n\t{{", csName);
			// todo: implement marshalling
			writer.WriteLine ("\t\tinternal IntPtr m;", csName);
			writer.WriteLine ("\t}");

			return true;
		}

		void GenerateHandles ()
		{
			CreateFile ("Handles");
			GenerateType ("handle", WriteHandle);
			FinalizeFile ();
		}

		HashSet<string> keywords = new HashSet<string> {
			"event",
			"object",
		};

		void WriteCommandParameters (XElement commandElement)
		{
			bool previous = false;
			foreach (var param in commandElement.Elements ("param")) {
				string type = param.Element ("type").Value;
				string name = param.Element ("name").Value;
				string csType = GetTypeCsName (type);

				bool isPointer = param.Value.Contains (type + "*");
				bool isConst = false;
				bool isStruct = structures.Contains (csType);
				if (isPointer) {
					if (handles.Contains (csType)) {
						csType = "IntPtr";
					} else {
						switch (csType) {
						case "void":
						case "char":
							csType = "IntPtr";
							break;
						default:
							csType += "*";
							break;
						}
						if (name.StartsWith ("p"))
							name = name.Substring (1);
					}
					if (param.Value.Contains ("const "))
						isConst = true;
				}

				if (previous)
					writer.Write (", ");
				else
					previous = true;
				writer.Write ("{0}{1} {2}", (isPointer && !isConst && !isStruct) ? "out " : "", csType, keywords.Contains (name) ? "@" + name : name);
			}
		}

		HashSet<string> disabledCommands = new HashSet<string> {
			"vkGetPhysicalDeviceXcbPresentationSupportKHR",
			"vkCreateMirSurfaceKHR",
			"vkGetPhysicalDeviceMirPresentationSupportKHR",
			"vkCreateWaylandSurfaceKHR",
			"vkGetPhysicalDeviceWaylandPresentationSupportKHR",
			"vkCreateWin32SurfaceKHR",
			"vkCreateXlibSurfaceKHR",
			"vkGetPhysicalDeviceXlibPresentationSupportKHR",
			"vkCreateXcbSurfaceKHR",
			"vkCreateAndroidSurfaceKHR"
		};

		bool WriteCommand (XElement commandElement)
		{
			string function = commandElement.Element ("proto").Element ("name").Value;
			string type = commandElement.Element ("proto").Element ("type").Value;
			string csType = GetTypeCsName (type);

			// todo: extensions support
			if (disabledCommands.Contains (function))
				return false;

			// todo: function pointers
			if (csType.StartsWith ("PFN_"))
				csType = "IntPtr";

			writer.WriteLine ("\t\t[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]");
			writer.Write ("\t\tinternal static unsafe extern {0} {1} (", csType, function);
			WriteCommandParameters (commandElement);
			writer.WriteLine (");");

			return true;
		}

		void GenerateCommands ()
		{
			CreateFile ("ImportedCommands", true, "Vulkan.Interop", "Interop");

			writer.WriteLine ("\tinternal class Commands\n\t{");
			writer.WriteLine ("\t\tconst string VulkanLibrary = \"vulkan\";\n");

			bool written = false;
			foreach (var command in specTree.Elements ("commands").Elements ("command")) {
				if (written)
					writer.WriteLine ();
				written = WriteCommand (command);
			}

			writer.WriteLine ("\t}");

			FinalizeFile ();
		}
	}
}
