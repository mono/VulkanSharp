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
		Dictionary<string, HandleInfo> handles = new Dictionary<string,HandleInfo> ();
		Dictionary<string, List<EnumExtensionInfo>> enumExtensions = new Dictionary<string, List<EnumExtensionInfo>> ();

		class HandleInfo
		{
			public string name;
			public string type;
			public List<XElement> commands = new List<XElement> ();
		}

		class EnumExtensionInfo
		{
			public string name;
			public int value;
		}

		public Generator (string filename, string outputDir)
		{
			specXMLFile = filename;
			outputPath = outputDir;
		}

		public void Run ()
		{
			LoadSpecification ();
			Directory.CreateDirectory ("Interop");

			LearnExtensions ();
			GenerateEnums ();
			GenerateBitmasks ();
			LearnHandles ();
			LearnStructsAndUnions ();
			GenerateStructs ();
			GenerateUnions ();
			GenerateCommands ();
			GenerateHandles ();
			GenerateRemainingCommands ();
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

		void WriteEnumField (string name, string value, string csEnumName)
		{
			string fName = TranslateCName (name);
			string prefix = csEnumName, suffix = null;

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
				switch (csEnumName) {
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

			IndentWriteLine ("{0} = {1},", fName, value);
		}

		void WriteEnumField (XElement e, string csEnumName)
		{
			var valueAttr = e.Attribute ("value");
			string value;
			if (valueAttr == null) {
				int pos = Convert.ToInt32 (e.Attribute ("bitpos").Value);
				value = string.Format ("0x{0:X}", 1 << pos);
			}
			else
				value = valueAttr.Value;

			WriteEnumField (e.Attribute ("name").Value, value, csEnumName);
		}

		void WriteEnumExtensions (string csEnumName)
		{
			if (!enumExtensions.ContainsKey (csEnumName))
				return;
			foreach (var info in enumExtensions [csEnumName])
				WriteEnumField (info.name, info.value.ToString (), csEnumName);
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
				IndentWriteLine ("[Flags]");
				if (csName.EndsWith ("FlagBits"))
					csName = csName.Substring (0, csName.Length - 4) + "s";
				if (suffix != null)
					csName += suffix;
			}

			typesTranslation [name] = csName;
			IndentWriteLine ("public enum {0} : int", csName);
			IndentWriteLine ("{");
			IndentLevel++;

			foreach (var e in values.Elements ("enum"))
				WriteEnumField (e, csName);
			WriteEnumExtensions (csName);

			IndentLevel--;
			IndentWriteLine ("}");

			return true;
		}

		void GenerateCodeForElements (IEnumerable<XElement> elements, Func<XElement, bool> generator)
		{
			bool written = false;
			foreach (var e in elements)
			{
				if (written)
					WriteLine ();

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

		void WriteLicensingInformation ()
		{
			WriteLine ("   Licensed under the MIT license.");
			WriteLine ();
			WriteLine ("   Copyright 2016 Xamarin Inc");
			WriteLine ();
			WriteLine ("   This notice may not be removed from any source distribution.");
			WriteLine ("   See LICENSE file for licensing details.");
		}

		void CreateFile (string typeName, bool usingInterop = false, string nspace = "Vulkan", string subDirectory = null)
		{
			string path = subDirectory != null ? string.Format ("{0}{1}{2}", outputPath, Path.DirectorySeparatorChar, subDirectory) : outputPath;
			string filename = string.Format ("{0}{1}{2}.cs", path, Path.DirectorySeparatorChar, typeName);
			writer = File.CreateText (filename);
			IndentLevel = 0;

			WriteLine ("/* Please note that this file is generated by the VulkanSharp's generator. Do not edit directly.");
			WriteLine ();
			WriteLicensingInformation ();
			WriteLine ("*/");
			WriteLine ();
			WriteLine ("using System;");
			if (usingInterop)
				WriteLine ("using System.Runtime.InteropServices;");
			WriteLine ();

			WriteLine ("namespace {0}\n{{", nspace);
			IndentLevel++;
		}

		void FinalizeFile ()
		{
			WriteLine ("}");
			writer.Close ();
		}

		int IndentLevel = 0;
		void WriteIndentation ()
		{
			for (int i = 0; i < IndentLevel; i++)
				writer.Write ('\t');
		}

		void WriteLine ()
		{
			writer.WriteLine ();
		}

		void WriteLine (string str)
		{
			writer.WriteLine (str);
		}

		void Write (string str)
		{
			writer.Write (str);
		}

		void WriteLine (string str, params object[] arg)
		{
			writer.WriteLine (str, arg);
		}

		void Write (string str, params object[] arg)
		{
			writer.Write (str, arg);
		}

		void IndentWriteLine (string str)
		{
			WriteIndentation ();
			WriteLine (str);
		}

		void IndentWriteLine (string str, params object[] arg)
		{
			WriteIndentation ();
			WriteLine (str, arg);
		}

		void IndentWrite (string str)
		{
			WriteIndentation ();
			Write (str);
		}

		void IndentWrite (string str, params object[] arg)
		{
			WriteIndentation ();
			Write (str, arg);
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
				if (structures.Contains (csMemberType) || handles.ContainsKey (csMemberType) || csMemberType == "string")
					csMemberType = "IntPtr";
				IndentWriteLine ("{0}{1} {2}{3} {4};", attr, sec, mod, csMemberType, csMemberName);
			} else {
				if (structures.Contains (csMemberType) || handles.ContainsKey (csMemberType)) {
					IndentWriteLine ("{0} l{1};", csMemberType, csMemberName);
					IndentWriteLine ("{0} {1} {2} {{", sec, csMemberType, csMemberName);
					IndentLevel++;
					IndentWriteLine ("get {{ return l{0}; }}", csMemberName);
					IndentWriteLine ("set {{ l{0} = value; m->{0} = (IntPtr) value.m; }}", csMemberName);
					IndentLevel--;
					IndentWriteLine ("}");
				} else if (csMemberType == "string") {
					IndentWriteLine ("{0} {1} {2} {{", sec, csMemberType, csMemberName);
					IndentLevel++;
					IndentWriteLine ("get {{ return Marshal.PtrToStringAnsi (m->{0}); }}", csMemberName);
					IndentWriteLine ("set {{ m->{0} = Marshal.StringToHGlobalAnsi (value); }}", csMemberName);
					IndentLevel--;
					IndentWriteLine ("}");
				} else {
					IndentWriteLine ("public {0} {1} {{", csMemberType, csMemberName);
					IndentLevel++;
					IndentWriteLine ("get {{ return m->{0}; }}", csMemberName);
					IndentWriteLine ("set {{ m->{0} = value; }}", csMemberName);
					IndentLevel--;
					IndentWriteLine ("}");
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

		bool WriteStructOrUnion (XElement structElement)
		{
			string name = structElement.Attribute ("name").Value;
			if (!typesTranslation.ContainsKey (name))
				return false;

			string csName = typesTranslation [name];

			string mod = "";
			if (isUnion && isInterop)
				IndentWriteLine ("[StructLayout (LayoutKind.Explicit)]");
			if (!isInterop)
				mod = "unsafe ";
			IndentWriteLine ("{0}{1} {2} {3}", mod, isInterop ? "internal" : "public", isInterop ? "struct" : "class", csName);
			IndentWriteLine ("{");
			IndentLevel++;

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

				IndentWriteLine ("internal Interop.{0}* m;\n", csName);
				IndentWriteLine ("public {0} ()", csName);
				IndentWriteLine ("{");
				IndentLevel++;
				IndentWriteLine ("m = (Interop.{0}*) Interop.Structure.Allocate (typeof (Interop.{0}));", csName);
				if (hasSType) {
					IndentWriteLine ("m->SType = StructureType.{0};", csName);
				}
				IndentLevel--;
				IndentWriteLine ("}\n");
			}

			GenerateMembers (structElement, WriteMember);

			IndentLevel--;
			IndentWriteLine ("}");

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

		bool LearnHandle (XElement handleElement)
		{
			string name = handleElement.Element ("name").Value;
			string csName = GetTypeCsName (name, "struct");
			string type = handleElement.Element ("type").Value;

			handles.Add (csName, new HandleInfo { name = csName, type = type });

			return false;
		}

		void LearnHandles ()
		{
			GenerateType ("handle", LearnHandle);
		}

		string GetParamName (string name, bool isPointer)
		{
			if (isPointer && name.StartsWith ("p"))
				name = name.Substring (1);

			return name;
		}

		struct FixedParamInfo
		{
			public bool containsMHandle;
			public string csType;
		}

		Dictionary<string, FixedParamInfo> FindFixedParams (XElement commandElement, bool isForHandle, bool passToNative = false)
		{
			bool first = true;
			var fixedParams = new Dictionary<string, FixedParamInfo> ();

			foreach (var param in commandElement.Elements ("param")) {
				if (first && isForHandle) {
					first = false;
					continue;
				}
				string type = param.Element ("type").Value;
				string csType = GetTypeCsName (type);
				bool isPointer = param.Value.Contains (type + "*");
				if (isPointer && type == "void" && !param.Value.Contains ("**"))
					continue;
				if (!isPointer || structures.Contains (csType))
					continue;
				bool isHandle = handles.ContainsKey (csType);
				bool isStruct = structures.Contains (csType);
				if (isHandle || isStruct || !param.Value.Contains ("const "))
					fixedParams.Add (GetParamName (param.Element ("name").Value, isPointer), new FixedParamInfo () { containsMHandle = isHandle || isStruct, csType = csType });
			}

			return fixedParams;
		}

		Dictionary<string, string> WriteCommandParameters (XElement commandElement, bool isForHandle = false, bool passToNative = false, Dictionary<string, FixedParamInfo> fixedParams = null)
		{
			bool first = true;
			bool previous = false;
			var outParams = new Dictionary<string, string> ();

			foreach (var param in commandElement.Elements ("param")) {
				if (first && isForHandle) {
					if (passToNative) {
						Write ("this.m");
						previous = true;
					}
					first = false;
					continue;
				}

				string type = param.Element ("type").Value;
				string name = param.Element ("name").Value;
				string csType = GetTypeCsName (type);

				var optional = param.Attribute ("optional");
				bool isOptionalParam = (optional != null && optional.Value == "true");
				bool isPointer = param.Value.Contains (type + "*");
				bool isDoublePointer = param.Value.Contains (type + "**");
				bool isConst = false;
				bool isStruct = structures.Contains (csType);
				bool isHandle = handles.ContainsKey (csType);
				if (isPointer) {
					if (!isHandle) {
						switch (csType) {
						case "void":
							csType = "IntPtr";
							break;
						case "char":
							csType = "string";
							isPointer = false;
							break;
						}
					}
					if (param.Value.Contains ("const "))
						isConst = true;
				}
				name = GetParamName (name, isPointer);
				if (!isDoublePointer && isPointer && csType == "IntPtr")
					isPointer = false;

				if (previous)
					Write (", ");
				else
					previous = true;

				bool isOut = isPointer && !isConst;
				if (isOut)
					outParams [name] = csType;

				if (passToNative) {
					bool isFixed = fixedParams != null &&  fixedParams.ContainsKey (name);
					string paramName = isFixed ? "ptr" + name : name;
					bool useHandlePtr = !isFixed && (isStruct || isHandle);

					if (isOptionalParam && isPointer && !isOut)
						Write ("{0} != null ? {0}{1} : null", GetSafeParameterName(paramName), useHandlePtr ? ".m" : "");
					else
						Write ("{0}{1}{2}", (isPointer && !isStruct && !isFixed) ? "&" : "", GetSafeParameterName(paramName), useHandlePtr ? ".m" : "");
				} else
					Write ("{0}{1} {2}", isOut ? "out " : "", csType, keywords.Contains (name) ? "@" + name : name);
			}

			return outParams;
		}

		string GetSafeParameterName(string paramName)
		{
			// if paramName is a reserved name
			return keywords.Contains (paramName) ? "@" + paramName : paramName;
		}

		string GetManagedHandleType (string handleType)
		{
			return handleType == "VK_DEFINE_HANDLE" ? "IntPtr" : "UInt64";
		}

		string GetManagedType (string csType)
		{
			if (structures.Contains (csType))
				return "IntPtr";

			if (handles.ContainsKey (csType))
				return GetManagedHandleType (handles [csType].type);

			switch (csType) {
			case "void":
				return "IntPtr";
			}

			return csType;
		}

		HashSet<string> disabledCommands = new HashSet<string> {
			"vkCreateInstance"
		};

		bool WriteCommand (XElement commandElement, bool prependNewLine, bool isForHandle = false, string handleName = null)
		{
			string function = commandElement.Element ("proto").Element ("name").Value;
			string type = commandElement.Element ("proto").Element ("type").Value;
			string csType = GetTypeCsName (type);

			// todo: extensions support
			if (disabledUnmanagedCommands.Contains (function) || disabledCommands.Contains (function))
				return false;

			if (prependNewLine)
				WriteLine ();

			// todo: function pointers
			if (csType.StartsWith ("PFN_"))
				csType = "IntPtr";

			string csFunction = function;
			if (function.StartsWith ("vk"))
				csFunction = csFunction.Substring (2);

			if (isForHandle) {
				if (csFunction.StartsWith (handleName))
					csFunction = csFunction.Substring (handleName.Length);
				else if (csFunction.StartsWith ("Get" + handleName))
					csFunction = "Get" + csFunction.Substring (handleName.Length + 3);
				else if (csFunction.EndsWith (handleName))
					csFunction = csFunction.Substring (0, csFunction.Length - handleName.Length);
			}

			var fixedParams = FindFixedParams (commandElement, isForHandle);

			IndentWrite ("public {0}{1} {2} (", isForHandle ? "" : "static ", csType, csFunction);
			var outParams = WriteCommandParameters (commandElement, isForHandle);
			WriteLine (")");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("unsafe {");
			IndentLevel++;

			if (fixedParams.Count > 0) {
				int count = 0;
				foreach (var param in fixedParams) {
					if (param.Value.containsMHandle) {
						IndentWriteLine ("{0} = new {1} ();", param.Key, param.Value.csType);
						count++;
					}
				}
				if (count > 0)
					WriteLine ();

				foreach (var param in fixedParams) {
					IndentWriteLine ("fixed ({0}* ptr{1} = &{1}{2}) {{", GetManagedType (param.Value.csType), param.Key, param.Value.containsMHandle ? ".m" : "");
					IndentLevel++;
				}
			}
			if (outParams.Count > 0) {
				foreach (var param in outParams)
					if (!fixedParams.ContainsKey (param.Key))
						IndentWriteLine ("{0} = new {1} ();", param.Key, param.Value);
			}

			IndentWrite ("{0}Interop.NativeMethods.{1} (", csType != "void" ? "return " : "", function);
			WriteCommandParameters (commandElement, isForHandle, true, fixedParams);
			WriteLine (");");

			if (fixedParams.Count > 0) {
				foreach (var param in fixedParams) {
					IndentLevel--;
					IndentWriteLine ("}");
				}
			}

			IndentLevel--;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");

			return true;
		}

		bool WriteHandle (XElement handleElement)
		{
			string csName = GetTypeCsName (handleElement.Element ("name").Value, "handle");
			HandleInfo info = handles [csName];

			IndentWriteLine ("public partial class {0}", csName);
			IndentWriteLine ("{");
			IndentLevel++;

			//// todo: implement marshalling
			switch (info.type) {
			case "VK_DEFINE_NON_DISPATCHABLE_HANDLE":
				IndentWriteLine ("internal UInt64 m;", csName);
				break;
			case "VK_DEFINE_HANDLE":
				IndentWriteLine ("internal IntPtr m;", csName);
				break;
			default:
				throw new Exception ("unknown handle type: " + info.type);
			}

			if (info.commands.Count > 0) {
				WriteLine ();
				bool written = false;
				foreach (var element in info.commands)
					written = WriteCommand (element, written, true, csName);
			}

			IndentLevel--;
			IndentWriteLine ("}");

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

		void WriteUnmanagedCommandParameters (XElement commandElement)
		{
			bool first = true;
			bool previous = false;
			foreach (var param in commandElement.Elements ("param")) {
				string type = param.Element ("type").Value;
				string name = param.Element ("name").Value;
				string csType = GetTypeCsName (type);

				bool isPointer = param.Value.Contains (type + "*");
				if (handles.ContainsKey (csType)) {
					var handle = handles [csType];
					if (first && !isPointer)
						handle.commands.Add (commandElement);
					csType = handle.type == "VK_DEFINE_HANDLE" ? "IntPtr" : "UInt64";
				}
				if (isPointer) {
					switch (csType) {
					case "void":
						csType = "IntPtr";
						break;
					case "char":
						csType = "string";
						isPointer = false;
						break;
					default:
						csType += "*";
						break;
					}
				} else if (first && handles.ContainsKey (csType))
					handles [csType].commands.Add (commandElement);

				name = GetParamName (name, isPointer);

				if (previous)
					Write (", ");
				else
					previous = true;

				if (param.Value.Contains (type + "**"))
					csType += "*";

				Write ("{0} {1}", csType, keywords.Contains (name) ? "@" + name : name);
				first = false;
			}
		}

		HashSet<string> disabledUnmanagedCommands = new HashSet<string> {
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

		bool WriteUnmanagedCommand (XElement commandElement)
		{
			string function = commandElement.Element ("proto").Element ("name").Value;
			string type = commandElement.Element ("proto").Element ("type").Value;
			string csType = GetTypeCsName (type);

			// todo: extensions support
			if (disabledUnmanagedCommands.Contains (function))
				return false;

			// todo: function pointers
			if (csType.StartsWith ("PFN_"))
				csType = "IntPtr";

			IndentWriteLine ("[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Winapi)]");
			IndentWrite ("internal static unsafe extern {0} {1} (", csType, function);
			WriteUnmanagedCommandParameters (commandElement);
			WriteLine (");");

			return true;
		}

		void GenerateCommands ()
		{
			CreateFile ("ImportedCommands", true, "Vulkan.Interop", "Interop");

			IndentWriteLine ("internal static class NativeMethods");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("const string VulkanLibrary = \"vulkan\";\n");

			bool written = false;
			foreach (var command in specTree.Elements ("commands").Elements ("command")) {
				if (written)
					WriteLine ();
				written = WriteUnmanagedCommand (command);
			}

			IndentLevel--;
			IndentWriteLine ("}");

			FinalizeFile ();
		}

		void GenerateRemainingCommands ()
		{
			CreateFile ("Commands");

			IndentWriteLine ("internal static partial class Commands");
			IndentWriteLine ("{");
			IndentLevel++;

			var handlesCommands = new HashSet<string> ();
			foreach (var handle in handles)
				foreach (var command in handle.Value.commands)
					handlesCommands.Add (command.Element ("proto").Element ("name").Value);

			bool written = false;
			foreach (var command in specTree.Elements ("commands").Elements ("command")) {
				if (handlesCommands.Contains (command.Element ("proto").Element ("name").Value))
					continue;

				written = WriteCommand (command, written);
			}

			IndentLevel--;
			IndentWriteLine ("}");

			FinalizeFile ();
		}

		int EnumExtensionValue (XElement element, int number)
		{
			var offsetAttribute = element.Attribute ("offset");
			if (offsetAttribute != null) {
				int direction = 1;
				var dirAttr = element.Attribute ("dir");
				if (dirAttr != null && dirAttr.Value == "-")
					direction = -1;
				int offset = Int32.Parse (offsetAttribute.Value);

				return direction*(1000000000 + (number - 1)*1000 + offset);
			}
			var valueAttribute = element.Attribute ("value");
			if (valueAttribute != null)
				return Int32.Parse (valueAttribute.Value);

			var bitposAttribute = element.Attribute ("bitpos");
			if (bitposAttribute != null)
				return Int32.Parse (bitposAttribute.Value);

			throw new Exception (string.Format ("unexpected extension enum value in: {0}", element));
		}

		void LearnExtension (XElement extensionElement)
		{
			var extensions = from e in extensionElement.Element ("require").Elements ("enum") where e.Attribute ("extends") != null select e;
			int number = Int32.Parse (extensionElement.Attribute ("number").Value);
			foreach (var element in extensions) {
				string enumName = GetTypeCsName (element.Attribute ("extends").Value, "enum");
				if (!enumExtensions.ContainsKey (enumName))
					enumExtensions [enumName] = new List<EnumExtensionInfo> ();

				var info = new EnumExtensionInfo { name = element.Attribute ("name").Value, value = EnumExtensionValue (element, number) };
				enumExtensions [enumName].Add (info);
			}
		}

		void LearnExtensions ()
		{
			var elements = from e in specTree.Elements ("extensions").Elements ("extension") where e.Attribute ("supported").Value != "disabled" select e;

			foreach (var element in elements)
				LearnExtension (element);
		}
	}
}
