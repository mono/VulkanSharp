using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace VulkanSharp.Generator
{
	public class Generator : GeneratorBase
	{
		XElement specTree;
		string specXMLFile;
		string outputPath;
		StreamWriter writer;
		bool isUnion;
		bool isInterop;
		bool needsMarshalling;

		Dictionary<string, StructInfo> structures = new Dictionary<string, StructInfo> () { { "SecurityAttributes", new StructInfo { name = "SecurityAttributes", csName = "SECURITY_ATTRIBUTES", needsMarshalling = false } } };
		Dictionary<string, HandleInfo> handles = new Dictionary<string,HandleInfo> ();
		Dictionary<string, EnumInfo> enums = new Dictionary<string, EnumInfo> ();
		Dictionary<string, List<EnumExtensionInfo>> enumExtensions = new Dictionary<string, List<EnumExtensionInfo>> ();
		//HashSet<string> enums = new HashSet<string> ();

		string platform;
		HashSet<string> requiredTypes = null;
		HashSet<string> requiredCommands = null;

		[Flags]
		enum UsingNamespaceFlags
		{
			Interop = 1,
			Collections = 2,
			Vulkan = 4,
			VulkanInterop = 8
		}

		class TypeInfo
		{
			public string name;
			public string csName;
			public Dictionary<string, string> members;
		}

		class StructInfo : TypeInfo
		{
			public XElement element;
			public bool needsMarshalling;
		}

		StructInfo currentStructInfo;

		class EnumInfo : TypeInfo
		{
		}

		EnumInfo currentEnumInfo;

		class HandleInfo
		{
			public string name;
			public string type;
			public List<XElement> commands = new List<XElement> ();
		}

		class EnumExtensionInfo
		{
			public string name;
			public string value;
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
			GenerateExtensions ();

			WriteTypeInformation ();
		}

		void LoadSpecification ()
		{
			specTree = XElement.Load (specXMLFile);

			if (specTree.Name != "registry")
				throw new Exception ("problem parsing the file, top element is not 'registry'");

			Console.WriteLine ("Specification file {0} loaded", specXMLFile);
		}

		void WriteEnumField (string name, string value, string csEnumName)
		{
			string fName = TranslateCName (name);
			string prefix = csEnumName, suffix = null;
			bool isExtensionField = false;
			string extension = null;

			foreach (var ext in extensions) {
				if (prefix.EndsWith (ext.Value)) {
					prefix = prefix.Substring (0, prefix.Length - ext.Value.Length);
					suffix = ext.Value;
				} else if (fName.EndsWith (ext.Value)) {
					isExtensionField = true;
					extension = ext.Value;
				}
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
				case "ImageCreateFlags":
					fName = "Create" + fName;
					break;
				}
			}
			if (suffix != null) {
				if (fName.EndsWith (suffix))
					fName = fName.Substring (0, fName.Length - suffix.Length);
				else if (isExtensionField && fName.EndsWith (suffix + extension))
					fName = fName.Substring (0, fName.Length - suffix.Length - extension.Length) + extension;
			}
			IndentWriteLine ("{0} = {1},", fName, value);

			currentEnumInfo.members [fName] = name;
		}

		string FormatFlagValue (int pos)
		{
			return string.Format ("0x{0:X}", 1 << pos);
		}

		void WriteEnumField (XElement e, string csEnumName)
		{
			var valueAttr = e.Attribute ("value");
			string value;
			if (valueAttr == null)
				value = FormatFlagValue (Convert.ToInt32 (e.Attribute ("bitpos").Value));
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

			currentEnumInfo = new EnumInfo { name = name };
			currentEnumInfo.members = new Dictionary<string, string> ();

			var values = from el in specTree.Elements ("enums")
					where (string)el.Attribute ("name") == name
				select el;

			if (values.Count () < 1) {
				Console.WriteLine ("warning: not adding empty enum {0}", name);
				return false;
			}

			var enumsElement = values.First ();
			var bitmask = enumsElement.Attribute ("type") != null && enumsElement.Attribute ("type").Value == "bitmask";
			if (bitmask)
				IndentWriteLine ("[Flags]");

			string csName = GetEnumCsName (name, bitmask);

			typesTranslation [name] = csName;
			currentEnumInfo.csName = csName;
			enums [csName] = currentEnumInfo;
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

		void CreateFile (string typeName, UsingNamespaceFlags namespaces = 0, string nspace = "Vulkan", string subDirectory = null)
		{
			string path = subDirectory != null ? string.Format ("{0}{1}{2}", outputPath, Path.DirectorySeparatorChar, subDirectory) : outputPath;
			string filename = string.Format ("{0}{1}{2}.cs", path, Path.DirectorySeparatorChar, typeName);
			Directory.CreateDirectory (Path.GetDirectoryName (filename));
			writer = File.CreateText (filename);
			IndentLevel = 0;

			WriteLine ("/* Please note that this file is generated by the VulkanSharp's generator. Do not edit directly.");
			WriteLine ();
			WriteLicensingInformation ();
			WriteLine ("*/");
			WriteLine ();
			WriteLine ("using System;");
			if ((namespaces & UsingNamespaceFlags.Interop) != 0)
				WriteLine ("using System.Runtime.InteropServices;");
			if ((namespaces & UsingNamespaceFlags.Collections) != 0)
				WriteLine ("using System.Collections.Generic;");
			if ((namespaces & UsingNamespaceFlags.Vulkan) != 0)
				WriteLine ("using Vulkan;");
			if ((namespaces & UsingNamespaceFlags.VulkanInterop) != 0)
				WriteLine ("using Vulkan.Interop;");
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

		void WriteMemberCharArray (string csMemberName, string sec)
		{
			IndentWriteLine ("{0} string {1} {{", sec, csMemberName);
			IndentLevel++;
			IndentWriteLine ("get {{ return Marshal.PtrToStringAnsi ((IntPtr)m->{0}); }}", csMemberName);
			IndentWriteLine ("set {{ Interop.Structure.MarshalFixedSizeString (m->{0}, value, 256); }}", csMemberName);
			IndentLevel--;
			IndentWriteLine ("}");
		}

		static Dictionary<string, string> fieldCounterMap = new Dictionary<string, string> {
			{ "Code", "CodeSize" },
			{ "SampleMask", "RasterizationSamples" },
			{ "MemoryTypes", "MemoryTypeCount" },
			{ "MemoryHeaps", "MemoryHeapCount" },
			{ "AcquireSyncs", "AcquireCount" },
			{ "AcquireKeys", "AcquireCount" },
			{ "AcquireTimeoutMilliseconds", "AcquireCount" },
			{ "ReleaseSyncs", "ReleaseCount" },
			{ "ReleaseKeys", "ReleaseCount" },
		};

		void WriteMemberFixedArray (string csMemberType, string csMemberName, XElement memberElement, bool isStruct, bool needsCharCast)
		{
			string counter = fieldCounterMap.ContainsKey (csMemberName) ? fieldCounterMap [csMemberName] : null;
			string len = GetArrayLength (memberElement);
			IndentWriteLine ("public {0}[] {1} {{", csMemberType, csMemberName);
			IndentLevel++;
			IndentWriteLine ("get {");
			IndentLevel++;
			if (counter != null)
				len = string.Format ("m->{0}", counter);
			IndentWriteLine ("var arr = new {0} [{1}];", csMemberType, len);
			IndentWriteLine ("for (int i = 0; i < {0}; i++)", len);
			IndentLevel++;
			if (isStruct) {
				IndentWriteLine ("unsafe");
				IndentWriteLine ("{");
				IndentLevel++;
				IndentWriteLine ("arr [i] = (&m->{0}0) [i];", csMemberName);
				IndentLevel--;
				IndentWriteLine ("}");
			} else
				IndentWriteLine ("arr [i] = {1}m->{0} [i];", csMemberName, needsCharCast ? "(char)" : "");
			IndentLevel--;
			IndentWriteLine ("return arr;");
			IndentLevel--;
			IndentWriteLine ("}");
			WriteLine ();

			IndentWriteLine ("set {");
			IndentLevel++;
			IndentWriteLine ("if (value.Length > {0})", len);
			IndentLevel++;
			IndentWriteLine ("throw new Exception (\"array too long\");");
			IndentLevel--;
			if (counter != null)
				IndentWriteLine ("{0} = (uint)value.Length;", len);
			IndentWriteLine ("for (int i = 0; i < value.Length; i++)");
			IndentLevel++;
			if (isStruct) {
				IndentWriteLine ("unsafe");
				IndentWriteLine ("{");
				IndentLevel++;
				IndentWriteLine ("(&m->{0}0) [i] = value [i];", csMemberName);
				IndentLevel--;
				IndentWriteLine ("}");
			} else
				IndentWriteLine ("m->{0} [i] = {1}value [i];", csMemberName, needsCharCast ? "(byte)" : "");
			IndentLevel--;
			if (counter == null && !isStruct) {
				IndentWriteLine ("for (int i = value.Length; i < {0}; i++)", len);
				IndentLevel++;
				if (isStruct) {
					IndentWriteLine ("unsafe");
					IndentWriteLine ("{");
					IndentLevel++;
					IndentWriteLine ("(&m->{0}0) [i] = 0;", csMemberName);
					IndentLevel--;
					IndentWriteLine ("}");
				} else
					IndentWriteLine ("m->{0} [i] = 0;", csMemberName);
				IndentLevel--;
			}
			IndentLevel--;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");
		}

		void WriteMemberArray (string csMemberType, string csMemberName, string sec, XElement memberElement)
		{
			string countName;

			var lenAttribute = memberElement.Attribute ("len");
			var structNeedsMarshalling = structures.ContainsKey (csMemberType) && structures [csMemberType].needsMarshalling;
			var isHandle = handles.ContainsKey (csMemberType);
			var ptrType = isHandle ? GetHandleType (handles [csMemberType]) : csMemberType;
			if (fieldCounterMap.ContainsKey (csMemberName))
				countName = fieldCounterMap [csMemberName];
			else if (lenAttribute != null) {
				countName = TranslateCName (lenAttribute.Value);
			} else
				throw new Exception (string.Format ("do not know the counter for {0}", csMemberName));
			// fixme: handle size_t sized arrays better
			string zero, cast, len, lenFromValue;
			if (csMemberName == "Code") {
				cast = "(UIntPtr)";
				zero = "UIntPtr.Zero";
				len = string.Format ("((uint)m->{0} >> 2)", countName);
				lenFromValue = string.Format ("(value.Length << 2)");
			} else if (csMemberName == "SampleMask") {
				cast = "";
				zero = "0";
				len = string.Format ("((uint)m->{0} >> 5)", countName);
				lenFromValue = string.Format ("(SampleCountFlags)(value.Length << 5)");
			} else {
				cast = "(uint)";
				zero = "0";
				len = string.Format ("m->{0}", countName);
				lenFromValue = "value.Length";
			}
			IndentWriteLine ("NativeReference ref{0};", csMemberName);
			IndentWriteLine ("{0} {1}[] {2} {{", sec, csMemberType, csMemberName);
			IndentLevel++;
			IndentWriteLine ("get {");
			IndentLevel++;
			IndentWriteLine ("if (m->{0} == {1})", countName, zero);
			IndentLevel++;
			IndentWriteLine ("return null;");
			IndentLevel--;
			IndentWriteLine ("var values = new {0} [{1}];", csMemberType, len);
			IndentWriteLine ("unsafe");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("{0}{1}* ptr = ({0}{1}*)m->{2};", structNeedsMarshalling ? (InteropNamespace + ".") : "", ptrType, csMemberName);
			IndentWriteLine ("for (int i = 0; i < values.Length; i++) {0}", (structNeedsMarshalling || isHandle) ? "{" : "");
			IndentLevel++;
			if (structNeedsMarshalling) {
				IndentWriteLine ("values [i] = new {0} ();", csMemberType);
				IndentWriteLine ("*values [i].m = ptr [i];", csMemberType);
			} else if (isHandle) {
				IndentWriteLine ("values [i] = new {0} ();", csMemberType);
				IndentWriteLine ("values [i].m = ptr [i];", csMemberType);
			} else
				IndentWriteLine ("values [i] = ptr [i];");
			IndentLevel--;
			if (structNeedsMarshalling || isHandle)
				IndentWriteLine ("}");
			IndentLevel --;
			IndentWriteLine ("}");
			IndentWriteLine ("return values;");
			IndentLevel--;
			IndentWriteLine ("}");
			WriteLine ();

			IndentWriteLine ("set {");
			IndentLevel++;
			IndentWriteLine ("if (value == null) {");
			IndentLevel++;
			IndentWriteLine ("m->{0} = {1};", countName, zero);
			IndentWriteLine ("m->{0} = IntPtr.Zero;", csMemberName);
			IndentWriteLine ("return;");
			IndentLevel--;
			IndentWriteLine ("}");
			IndentWriteLine ("m->{0} = {1}{2};", countName, cast, lenFromValue);
			IndentWriteLine ("ref{0} = new NativeReference ((int)(sizeof({1}{2})*value.Length));", csMemberName, structNeedsMarshalling ? (InteropNamespace + ".") : "", ptrType);
			IndentWriteLine ("m->{0} = ref{0}.Handle;", csMemberName);
			IndentWriteLine ("unsafe");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("{0}{1}* ptr = ({0}{1}*)m->{2};", structNeedsMarshalling ? (InteropNamespace + ".") : "", ptrType, csMemberName);
			IndentWriteLine ("for (int i = 0; i < value.Length; i++)");
			IndentLevel++;
			if (structNeedsMarshalling)
				IndentWriteLine ("ptr [i] = *value [i].m;");
			else
				IndentWriteLine ("ptr [i] = value [i]{0};", isHandle ? ".m" : "");
			IndentLevel -= 2;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");
		}

		void WriteMemberStructOrHandle (string csMemberType, string csMemberName, string sec, bool isPointer)
		{
			var isHandle = handles.ContainsKey (csMemberType);
			bool isMarshalled = true;
			if (structures.ContainsKey (csMemberType))
				isMarshalled = structures [csMemberType].needsMarshalling;

			if (isMarshalled) {
				IndentWriteLine ("{0} l{1};", csMemberType, csMemberName);
				initializeMembers.Add (new StructMemberInfo () { csName = csMemberName, csType = csMemberType, isPointer = isPointer, isHandle = isHandle });
			}
			IndentWriteLine ("{0} {1}{2} {3} {{", sec, csMemberType, (isPointer && !needsMarshalling) ? "?" : "", csMemberName);
			IndentLevel++;
			if (isMarshalled) {
				IndentWriteLine ("get {{ return l{0}; }}", csMemberName);
				var castType = isHandle ? GetHandleType (handles [csMemberType]) : "IntPtr";
				if (isPointer || isHandle)
					IndentWriteLine ("set {{ l{0} = value; m->{0} = value != null ? {1}value.m : default{1}; }}", csMemberName, string.Format ("({0})", castType));
				else
					IndentWriteLine ("set {{ l{0} = value; m->{0} = value != null ? *value.m : default({1}.{2}); }}", csMemberName, InteropNamespace, csMemberType);
			} else if (isPointer) {
				var vulkanInteropPrefix = InteropNamespace == "Interop" ? "" : "Vulkan.";
				IndentWriteLine ("get {{ return ({0}){1}Interop.Structure.MarshalPointerToObject (m->{2}, typeof ({0})); }}", csMemberType, vulkanInteropPrefix, csMemberName);
				IndentWriteLine ("set {{ m->{0} = {1}Interop.Structure.MarshalObjectToPointer (m->{0}, value); }}", csMemberName, vulkanInteropPrefix);
			} else {
				IndentWriteLine ("get {{ return m->{0}; }}", csMemberName);
				IndentWriteLine ("set {{ m->{0} = value; }}", csMemberName);
			}
			IndentLevel--;
			IndentWriteLine ("}");
		}

		void WriteMemberString (string csMemberType, string csMemberName, string sec)
		{
			IndentWriteLine ("{0} {1} {2} {{", sec, csMemberType, csMemberName);
			IndentLevel++;
			IndentWriteLine ("get {{ return Marshal.PtrToStringAnsi (m->{0}); }}", csMemberName);
			IndentWriteLine ("set {{ m->{0} = Marshal.StringToHGlobalAnsi (value); }}", csMemberName);
			IndentLevel--;
			IndentWriteLine ("}");
		}

		void WriteMemberStringArray (string csMemberType, string csMemberName, string sec)
		{
			IndentWriteLine ("NativeReference ref{0};", csMemberName);
			IndentWriteLine ("{0} {1} {2} {{", sec, csMemberType, csMemberName);
			IndentLevel++;
			IndentWriteLine ("get {");
			IndentLevel++;
			string countName;
			if (!csMemberName.EndsWith ("Names"))
				throw new Exception (string.Format ("unable to handle member {0} {1}", csMemberType, csMemberName));
			countName = csMemberName.Substring (0, csMemberName.Length - 5) + "Count";
			IndentWriteLine ("if (m->{0} == 0)", countName);
			IndentLevel++;
			IndentWriteLine ("return null;");
			IndentLevel--;
			IndentWriteLine ("var strings = new string [m->{0}];", countName);
			IndentWriteLine ("unsafe");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("void** ptr = (void**)m->{0};", csMemberName);
			IndentWriteLine ("for (int i = 0; i < m->{0}; i++)", countName);
			IndentLevel++;
			IndentWriteLine ("strings [i] = Marshal.PtrToStringAnsi ((IntPtr)ptr [i]);");
			IndentLevel -= 2;
			IndentWriteLine ("}");
			IndentWriteLine ("return strings;");
			IndentLevel--;
			IndentWriteLine ("}");
			WriteLine ();

			IndentWriteLine ("set {");
			IndentLevel++;
			IndentWriteLine ("if (value == null) {");
			IndentLevel++;
			IndentWriteLine ("m->{0} = 0;", countName);
			IndentWriteLine ("m->{0} = IntPtr.Zero;", csMemberName);
			IndentWriteLine ("return;");
			IndentLevel--;
			IndentWriteLine ("}");
			IndentWriteLine ("m->{0} = (uint)value.Length;", countName);
			IndentWriteLine ("ref{0} = new NativeReference ((int)(sizeof(IntPtr)*m->{1}));", csMemberName, countName);
			IndentWriteLine ("m->{0} = ref{0}.Handle;", csMemberName, countName);
			IndentWriteLine ("unsafe");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("void** ptr = (void**)m->{0};", csMemberName);
			IndentWriteLine ("for (int i = 0; i < m->{0}; i++)", countName);
			IndentLevel++;
			IndentWriteLine ("ptr [i] = (void*) Marshal.StringToHGlobalAnsi (value [i]);");
			IndentLevel -= 2;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");
		}

		class StructMemberInfo
		{
			public string csType;
			public string csName;
			public bool isPointer;
			public bool isHandle;
		}
		List<StructMemberInfo> initializeMembers;
		List<string> arrayMembers;

		string InnerValue (XElement element)
		{
			var sb = new StringBuilder ();
			foreach (var node in element.Nodes ().OfType<XText> ())
				sb.Append (node.Value);
			return sb.ToString ();
		}

		bool IsArray (XElement memberElement)
		{
			return InnerValue (memberElement).Contains ('[');
		}

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

			if (!isInterop && needsMarshalling && csMemberType == "StructureType" && name == "sType")
				return false;

			var isArray = false;
			var isFixedArray = false;
			bool isPointer = memberElement.Value.Contains (typeElement.Value + "*");
			bool isDoublePointer = memberElement.Value.Contains (typeElement.Value + "**") || memberElement.Value.Contains (typeElement.Value + "* const*");
			if (isPointer) {
				if (name.StartsWith ("p"))
					name = name.Substring (1);
				if (name.StartsWith ("p"))
					name = name.Substring (1);

				switch (csMemberType) {
				case "void":
					if (!isInterop && name == "Next")
						return false;
					csMemberType = "IntPtr";
					break;
				case "char":
					csMemberType = isDoublePointer ? (isInterop ? "IntPtr" : "string[]") : "string";
					break;
				case "float":
					csMemberType = isInterop ? "IntPtr" : "float";
					isArray = true;
					break;
				case "SampleMask":
				case "UInt32":
					csMemberType = isInterop ? "IntPtr" : "UInt32";
					isArray = true;
					break;
				default:
					var lenAttribute = memberElement.Attribute ("len");
					if (lenAttribute != null || fieldCounterMap.ContainsKey (TranslateCName (name)))
						isArray = true;
					break;
				}
			} else if (IsArray (memberElement)
			           && GetArrayLength (memberElement) != null
			           && !(structures.ContainsKey (csMemberType)
			                && structures [csMemberType].needsMarshalling))
				isFixedArray = true;
			var csMemberName = TranslateCName (name);

			// TODO: fixed arrays of structs
			if (csMemberName.EndsWith ("]")) {
				string array = csMemberName.Substring (csMemberName.IndexOf ('['));
				csMemberName = csMemberName.Substring (0, csMemberName.Length - array.Length);
				// temporarily disable arrays csMemberType += "[]";
			}

			currentStructInfo.members [csMemberName] = nameElement.Value;

			var isCharArray = false;
			if (csMemberType == "char" && InnerValue (memberElement).EndsWith ("]"))
				isCharArray = true;
			string mod = "";
			if (csMemberName.EndsWith ("]"))
				mod = "unsafe fixed ";

			csMemberType = GetFlagBitsName (csMemberType);

			if (csMemberType.StartsWith ("PFN_"))
				csMemberType = "IntPtr";

			if (csMemberType == "SampleMask")
				csMemberType = "UInt32";

			if (csMemberType == "Bool32" && !isInterop && needsMarshalling)
				csMemberType = "bool";

			bool needsCharCast = false;
			if (csMemberType == "char") {
				if (isInterop || !needsMarshalling)
					csMemberType = "byte";
				needsCharCast = true;
			}

			string attr = "";
			string sec = isInterop ? "internal" : "public";
			if (isUnion)
				attr = "[FieldOffset (0)] ";

			bool memberIsStructure = structures.ContainsKey (csMemberType);
			if (isInterop || !needsMarshalling) {
				string member = memberElement.Value;
				string arrayPart = "";
				string fixedPart = "";
				int count = 1;
				if (IsArray (memberElement)
				    && !(memberIsStructure
				         && structures [csMemberType].needsMarshalling)) {
					string len = GetArrayLength (memberElement);
					if (memberIsStructure)
						count = Convert.ToInt32 (len);
					else if (len != null) {
						arrayPart = string.Format ("[{0}]", len);
						fixedPart = "unsafe fixed ";
					}
				}
				if (handles.ContainsKey (csMemberType) && !isPointer) {
					csMemberType = GetHandleType (handles [csMemberType]);
				} else if ((isPointer && !isInterop && memberIsStructure && structures [csMemberType].needsMarshalling) || csMemberType == "string" || isPointer)
					csMemberType = "IntPtr";
				for (int i = 0; i < count; i++)
					IndentWriteLine ("{0}{1} {2}{3}{4} {5}{6}{7};", attr, sec, fixedPart, mod, csMemberType, csMemberName, count > 1 ? i.ToString () : "", arrayPart);
			} else {
				if (isCharArray)
					WriteMemberCharArray (csMemberName, sec);
				else if (isFixedArray)
					WriteMemberFixedArray (csMemberType, csMemberName, memberElement, memberIsStructure, needsCharCast);
				else if (isArray) {
					WriteMemberArray (csMemberType, csMemberName, sec, memberElement);
					arrayMembers.Add (csMemberName);
				} else if (memberIsStructure || handles.ContainsKey (csMemberType))
					WriteMemberStructOrHandle (csMemberType, csMemberName, sec, isPointer);
				else if (csMemberType == "string")
					WriteMemberString (csMemberType, csMemberName, sec);
				else if (csMemberType == "string[]") {
					WriteMemberStringArray (csMemberType, csMemberName, sec);
					arrayMembers.Add (csMemberName);
				} else {
					IndentWriteLine ("public {0} {1} {{", csMemberType, csMemberName);
					IndentLevel++;
					IndentWriteLine ("get {{ return m->{0}; }}", csMemberName);
					IndentWriteLine ("set {{ m->{0} = value; }}", csMemberName);
					IndentLevel--;
					IndentWriteLine ("}");
				}
			}

			return !isInterop && needsMarshalling;
		}

		HashSet<string> disabledStructs = new HashSet<string> {
			"XlibSurfaceCreateInfoKhr",
			"XcbSurfaceCreateInfoKhr",
			"WaylandSurfaceCreateInfoKhr",
			"MirSurfaceCreateInfoKhr",
			"AndroidSurfaceCreateInfoKhr",
			"Win32SurfaceCreateInfoKhr",
			"ImportMemoryWin32HandleInfoNv",
			"ExportMemoryWin32HandleInfoNv",
			"Win32KeyedMutexAcquireReleaseInfoNv",
			"ImportMemoryWin32HandleInfoKhr",
			"ExportMemoryWin32HandleInfoKhr",
			"ImportSemaphoreWin32HandleInfoKhr",
			"ExportSemaphoreWin32HandleInfoKhr",
			"ImportFenceWin32HandleInfoKhr",
			"ExportFenceWin32HandleInfoKhr",
			// TODO: support fixed array of Handles
			"PhysicalDeviceGroupPropertiesKhx",
			// NativeBufferAndroid uses disabled extension
			"NativeBufferAndroid",
		};

		void WriteStructureInitializeMethod (List<StructMemberInfo> members, string csName, bool hasSType)
		{
			WriteLine ();
			IndentWriteLine ("internal void Initialize ()");
			IndentWriteLine ("{");
			IndentLevel++;
			if (hasSType)
				// special case DebugReportLayerFlagsExt, remove once fixed in the spec?
				IndentWriteLine ("m->SType = StructureType.{0};", csName == "DebugReportLayerFlagsExt" ? "DebugReportValidationFlagsExt" : csName);

			foreach (var info in members)
				if (handles.ContainsKey (info.csType) || (structures.ContainsKey (info.csType) && structures [info.csType].needsMarshalling))
					if (!info.isPointer && !info.isHandle)
						IndentWriteLine ("l{0} = new {1} (new NativePointer (native.Reference, (IntPtr)(&m->{0})));", info.csName, info.csType);

			IndentLevel--;
			IndentWriteLine ("}\n");
		}

		bool WriteStructOrUnion (XElement structElement)
		{
			string name = structElement.Attribute ("name").Value;
			if (!typesTranslation.ContainsKey (name) || (requiredTypes != null && !requiredTypes.Contains (name)))
				return false;

			string csName = typesTranslation [name];
			var info = structures [csName];
			needsMarshalling = info.needsMarshalling;

			if (isInterop && !needsMarshalling)
				return false;

			string mod = "";
			if (isUnion && (isInterop || !needsMarshalling))
				IndentWriteLine ("[StructLayout (LayoutKind.Explicit)]");
			if (!isInterop)
				mod = "unsafe ";
			IndentWriteLine ("{0}{1} partial {2} {3}{4}", mod, isInterop ? "internal" : "public", (isInterop || !needsMarshalling) ? "struct" : "class", csName, (!isInterop && info.needsMarshalling) ? " : MarshalledObject" : "");
			IndentWriteLine ("{");
			IndentLevel++;

			initializeMembers = new List<StructMemberInfo> ();
			arrayMembers = new List<string> ();
			currentStructInfo = info;
			currentStructInfo.members = new Dictionary<string, string> ();
			GenerateMembers (structElement, WriteMember);

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

				if (info.needsMarshalling) {
					var needsInitialize = hasSType || initializeMembers.Count > 0;
					IndentWriteLine ("internal {0}.{1}* m {{\n", InteropNamespace, csName);
					IndentLevel++;
					IndentWriteLine ("get {");
					IndentLevel++;
					IndentWriteLine ("return ({0}.{1}*)native.Handle;", InteropNamespace, csName);
					IndentLevel--;
					IndentWriteLine ("}");
					IndentLevel--;
					IndentWriteLine ("}\n");
					IndentWriteLine ("public {0} ()", csName);
					IndentWriteLine ("{");
					IndentLevel++;
					IndentWriteLine ("native = {0}Interop.Structure.Allocate (typeof ({1}.{2}));", InteropNamespace == "Interop" ? "" : "Vulkan.", InteropNamespace, csName);
					if (needsInitialize)
						IndentWriteLine ("Initialize ();");
					IndentLevel--;
					IndentWriteLine ("}");
					WriteLine ();
					IndentWriteLine ("internal {0} (NativePointer pointer)", csName);
					IndentWriteLine ("{");
					IndentLevel++;
					IndentWriteLine ("native = pointer;");
					if (needsInitialize)
						IndentWriteLine ("Initialize ();");
					IndentLevel--;
					IndentWriteLine ("}\n");

					if (arrayMembers.Count () > 0) {
						IndentWriteLine ("override public void Dispose (bool disposing)");
						IndentWriteLine ("{");
						IndentLevel++;
						IndentWriteLine ("base.Dispose (disposing);");
						IndentWriteLine ("if (!disposing)");
						IndentLevel++;
						IndentWriteLine ("return;");
						IndentLevel--;
						foreach (var refName in arrayMembers) {
							IndentWriteLine ("ref{0}.Dispose ();", refName);
							IndentWriteLine ("ref{0} = null;", refName);
						}
						IndentLevel--;
						IndentWriteLine ("}");
					}

					if (needsInitialize)
						WriteStructureInitializeMethod (initializeMembers, csName, hasSType);
				}
			}

			IndentLevel--;
			IndentWriteLine ("}");

			return true;
		}

		string GetAPIConstant (string name)
		{
			var constants = specTree.Elements ("enums").FirstOrDefault (e => e.Attribute ("name").Value == "API Constants");
			if (constants == null)
				return null;
			var field = constants.Elements ("enum").FirstOrDefault (e => e.Attribute ("name").Value == name);
			if (field == null)
				return null;
			return field.Attribute ("value").Value;
		}

		string GetArrayLength (XElement member)
		{
			var enumElement = member.Element ("enum");
			if (enumElement != null)
				return GetAPIConstant (enumElement.Value);
			string len = member.Value.Substring (member.Value.IndexOf ('[') + 1);
			return len.Substring (0, len.IndexOf (']'));
		}

		bool LearnStructureMembers (XElement structElement)
		{
			foreach (var memberElement in structElement.Elements ("member")) {
				string member = memberElement.Value;
				var typeElement = memberElement.Element ("type");
				var csMemberType = GetTypeCsName (typeElement.Value, "member");

				if (member.Contains ("*")
				    || IsArray (memberElement)
				    || (structures.ContainsKey (csMemberType) && structures [csMemberType].needsMarshalling)
				    || handles.ContainsKey (csMemberType))
					return true;
			}

			return false;
		}

		bool LearnStructure (XElement structElement)
		{
			string name = structElement.Attribute ("name").Value;
			string csName = GetTypeCsName (name, "struct");

			if (requiredTypes != null) {
				if (!requiredTypes.Contains (name))
					return false;
			} else if (disabledStructs.Contains (csName))
				return false;

			typesTranslation [name] = csName;
			structures [csName] = new StructInfo () { name = name, csName = csName, needsMarshalling = LearnStructureMembers (structElement), element = structElement };

			return false;
		}

		void CompleteMarshallingInfo ()
		{
			bool changed;
			do {
				changed = false;
				foreach (var pair in structures) {
					var info = pair.Value;
					if (info.element == null)
						continue;
					var structNeedsMarshalling = LearnStructureMembers (info.element);
					if (structNeedsMarshalling != info.needsMarshalling) {
						info.needsMarshalling = structNeedsMarshalling;
						changed = true;
					}
				}
			} while (changed);
		}

		void LearnStructsAndUnions ()
		{
			GenerateType ("struct", LearnStructure);
			GenerateType ("union", LearnStructure);
			CompleteMarshallingInfo ();
		}

		void GenerateStructs ()
		{
			CreateFile ("Structs", UsingNamespaceFlags.Interop, Namespace);
			isUnion = false;
			GenerateType ("struct", WriteStructOrUnion);
			FinalizeFile ();

			CreateFile ("MarshalStructs", 0, "Vulkan." + InteropNamespace, "Interop");
			isUnion = false;
			isInterop = true;
			GenerateType ("struct", WriteStructOrUnion);
			isInterop = false;
			FinalizeFile ();
		}

		void GenerateUnions ()
		{
			CreateFile ("Unions", UsingNamespaceFlags.Interop, Namespace);
			isUnion = true;
			GenerateType ("union", WriteStructOrUnion);
			FinalizeFile ();

			CreateFile ("MarshalUnions", UsingNamespaceFlags.Interop, "Vulkan." + InteropNamespace, "Interop");
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

		string GetParamName (XElement param)
		{
			var name = param.Element ("name").Value;
			if (param.Value.Contains (param.Element ("name").Value + "*") && name.StartsWith ("p"))
				name = name.Substring (1);

			return name;
		}

		class ParamInfo
		{
			public string csName;
			public string csType;
			public string type;
			public string len;
			public bool isOut;
			public bool isStruct;
			public bool isHandle;
			public bool isFixed;
			public bool isPointer;
			public bool isConst;
			public bool needsMarshalling;
			public ParamInfo lenArray;
			public bool isArray;
			public bool isNullable;
			public string constValue;

			public string MarshalSizeSource (Generator generator, bool isInInterop)
			{
				if (generator.IsEnum (csType))
					return "4"; // int enum

				return string.Format ("Marshal.SizeOf (typeof ({0}{1}))",
				                      isInInterop ? "Interop." : "",
				                      isHandle ? generator.GetHandleType (generator.handles [csType]) : csType);
			}
		}

		string GetParamCsType (string type, ref bool isPointer, out bool isHandle)
		{
			string csType = GetTypeCsName (type);
			isHandle = handles.ContainsKey (csType);

			if (!isPointer)
				return csType;

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

			return csType;
		}

		Dictionary<string, ParamInfo> LearnParams (XElement commandElement, bool isInstance, out int fixedCount, out int outCount, bool passToNative = false)
		{
			bool first = true;
			var paramsDict = new Dictionary<string, ParamInfo> ();

			fixedCount = 0;
			outCount = 0;
			foreach (var param in commandElement.Elements ("param")) {
				if (first && isInstance) {
					first = false;
					continue;
				}
				var csName = GetParamName (param);
				var info = new ParamInfo () { csName = csName };
				var lenAttr = param.Attribute ("len");
				if (lenAttr != null)
					info.len = lenAttr.Value;
				string type = param.Element ("type").Value;
				bool isPointer = param.Value.Contains (type + "*");
				info.csType = GetParamCsType (type, ref isPointer, out info.isHandle);
				info.type = type;
				paramsDict.Add (csName, info);
				info.isPointer = isPointer;
				info.isConst = info.isPointer && param.Value.Contains ("const ");
				if (info.isPointer && !info.isConst) {
					info.isOut = true;
					outCount++;
				}
				info.isStruct = structures.ContainsKey (info.csType);
				if (info.isStruct) {
					info.needsMarshalling = (structures [info.csType].needsMarshalling || !isPointer);
				}
				if (isPointer && type == "void" && !param.Value.Contains ("**"))
					continue;
				if (!isPointer || info.isStruct)
					continue;
				if (info.isHandle || !param.Value.Contains ("const ") && !IsEnum (info.csType)) {
					info.isFixed = true;
					fixedCount++;
				}
			}

			return paramsDict;
		}

		string GetDefaultValue (string csType)
		{
			// Check for any known extensions and remove them
			String internalCopy = csType;
			string suffix = null;
			foreach (var ext in extensions)
			{
				if (internalCopy.EndsWith (ext.Value))
				{
					suffix = ext.Value + suffix;
					internalCopy = internalCopy.Substring (0, csType.Length - ext.Value.Length);
				}
			}

			if (internalCopy.EndsWith ("Flags"))
				return string.Format ("({0})0", csType);

			// other known types
			switch (csType) {
			case "IntPtr":
				return "default(IntPtr)";
			case "UInt32":
				return "0";
			}

			return "null";
		}

		string GetDefaultNativeValue (string csType)
		{
			if (csType.EndsWith ("Flags"))
				return string.Format ("({0})0", csType);

			if (handles.ContainsKey (csType))
				csType = GetHandleType (handles [csType]);
			else if (structures.ContainsKey (csType) && structures [csType].needsMarshalling)
				return string.Format ("({0}.{1}*)default(IntPtr)", InteropNamespace, csType);

			return string.Format ("default({0})", csType);
		}

		string GetParamName (string name, bool useArrayParameters)
		{
			if (!useArrayParameters && name.EndsWith ("s"))
				return name.Substring (0, name.Length - 1);
			return name;
		}

		bool WriteCommandParameters (XElement commandElement, bool useArrayParameters, List<ParamInfo> ignoredParameters = null, ParamInfo nullParameter = null, ParamInfo ptrParam = null, bool isInstance = false, bool passToNative = false, Dictionary<string, ParamInfo> paramsDict = null, bool isExtension = false, bool useOptional = false)
		{
			bool first = true;
			bool previous = false;
			string firstOptional = null;
			bool hasArrayParameter = false;

			if (useOptional) {
				foreach (var param in commandElement.Elements ("param")) {
					string name = GetParamName (param);
					if (!paramsDict.ContainsKey (name))
						continue;

					var info = paramsDict [name];

					if (ignoredParameters != null && ignoredParameters.Contains (info))
						continue;

					var optional = param.Attribute ("optional");
					bool isOptionalParam = (optional != null && optional.Value == "true");

					if (!isOptionalParam)
						firstOptional = null;
					else if (firstOptional == null)
						firstOptional = name;
				}
			}
			var optionalPart = false;
			foreach (var param in commandElement.Elements ("param")) {
				string name = GetParamName (param);

				if (first) {
					first = false;
					if (passToNative && (isInstance || isExtension)) {
						Write ("{0}.m", isExtension ? name : "this");
						previous = true;
						continue;
					}
					if (isInstance)
						continue;
					if (isExtension)
						Write ("this ");
				}

				string type = param.Element ("type").Value;
				var info = paramsDict [name];

				if (ignoredParameters != null && ignoredParameters.Contains (info))
					continue;

				if (info.isArray)
					hasArrayParameter = true;

				var optional = param.Attribute ("optional");
				bool isOptionalParam = (optional != null && optional.Value == "true");
				bool isDoublePointer = param.Value.Contains (type + "**");
				if (!isDoublePointer && info.isPointer && info.len != null && info.csType == "IntPtr") {
					info.isPointer = false;
					info.isOut = false;
				}

				if (previous)
					Write (", ");
				else
					previous = true;

				if (passToNative) {
					name = GetParamName (name, useArrayParameters);
					string paramName = info.isFixed ? "ptr" + name : name;
					bool useHandlePtr = !info.isFixed && ((info.isStruct && info.needsMarshalling) || info.isHandle);

					if (info == nullParameter)
						Write ("null");
					else if (useArrayParameters && info.isArray) {
						var arrayType = GetParamArrayType (info);
						Write ("({0}*)array{1}", arrayType, info.csName);
					} else if (info == ptrParam)
						Write ("({0}{1}*)ptr{2}", (info.isStruct && info.needsMarshalling) ? "Interop." : "", info.isHandle ? GetHandleType (handles [info.csType]) : info.csType, GetParamName (info.csName, useArrayParameters));
					else if (isOptionalParam && info.isPointer && !info.isOut)
						Write ("{0} != null ? {0}{1} : null", GetSafeParameterName (paramName), useHandlePtr ? ".m" : "");
					else if (info.lenArray != null) {
						if (useArrayParameters)
							Write ("(uint)len{0}", info.lenArray.csName);
						else
							Write ("({0})({1} != null ? {2} : 0)", info.csType, GetParamName (info.lenArray.csName, useArrayParameters), info.constValue);
					} else {
						var safeParamName = GetSafeParameterName (paramName);
						var needsAddress = (info.isPointer && (!info.isStruct || !info.needsMarshalling) && !(info.isConst && info.csType =="IntPtr") && !info.isFixed);
						var nativeValue = GetDefaultNativeValue (info.csType);
						if (useHandlePtr) {
							if (!needsAddress && nativeValue == "null")
								Write ("{0}?.m", safeParamName);
							else
								Write ("{0} != null ? {1}{0}.m : {2}", safeParamName, needsAddress ? "&" : "", nativeValue);
						} else
							Write ("{0}{1}", (needsAddress && !info.isNullable) ? "&" : "", info.isNullable ? string.Format ("ptr{0}", safeParamName) : safeParamName);
					}
				} else {
					if (firstOptional == name)
						optionalPart = true;
					name = GetParamName (name, useArrayParameters);
					Write ("{0}{1}{2}{3} {4}{5}", info.isOut ? "out " : "", info.csType, info.isNullable ? "?" : "", (useArrayParameters && info.isArray)? "[]" : "", keywords.Contains (name) ? "@" + name : name, (optionalPart && isOptionalParam) ? string.Format (" = {0}", GetDefaultValue (info.csType)) : "");
				}
			}

			return hasArrayParameter;
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
			if (structures.ContainsKey (csType))
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
		HashSet<string> ignoredSimplifiedCommands = new HashSet<string> {
			"CreateGraphicsPipelines",
			"CreateComputePipelines",
			"CreateSharedSwapchainsKHR",
		};
		HashSet<string> notLengthTypes = new HashSet<string> {
			"RROutput",
		};

		bool CommandShouldCreateArray (XElement commandElement, Dictionary<string, ParamInfo> paramsDict, ref ParamInfo intParam, ref ParamInfo dataParam)
		{
			ParamInfo outUInt = null;
			foreach (var param in commandElement.Elements ("param")) {
				string name = GetParamName (param);
				if (!paramsDict.ContainsKey (name))
					continue;

				var info = paramsDict [name];
				if (info.csType == "UInt32" && !notLengthTypes.Contains (info.type))
					outUInt = info;
				else {
					if (outUInt != null && info.isOut && (info.isStruct || info.isHandle || info.isPointer)) {
						intParam = outUInt;
						dataParam = info;

						return true;
					}
					outUInt = null;
				}
			}

			return false;
		}

		void CommandHandleResult (bool hasResult)
		{
			if (hasResult) {
				IndentWriteLine ("if (result != Result.Success)");
				IndentLevel++;
				IndentWriteLine ("throw new ResultException (result);");
				IndentLevel--;
			}
		}

		ParamInfo GetLenParamInfo (string len)
		{
			int index = len.IndexOf ("->");
			if (index < 0)
				index = len.IndexOf ("::");
			if (index > 0)
				len = string.Format ("{0}.{1}", len.Substring (0, index), TranslateCName (len.Substring (index + 2)));

			return new ParamInfo { csName = len };
		}

		string GetParamArrayType (ParamInfo info)
		{

			if (info.isHandle)
				return GetHandleType (handles [info.csType]);
			if (info.isStruct && info.needsMarshalling)
				return string.Format ("{0}.{1}", InteropNamespace, info.csType);

			return info.csType;
		}

		bool WriteCommand (XElement commandElement, bool prependNewLine, bool useArrayParameters, bool isForHandle = false, string handleName = null, bool isExtension = false)
		{
			string function = commandElement.Element ("proto").Element ("name").Value;
			string type = commandElement.Element ("proto").Element ("type").Value;
			string csType = GetTypeCsName (type);
			bool hasArrayParameter = false;

			// todo: extensions support
			if (requiredCommands != null) {
				if (!requiredCommands.Contains (function))
					return false;
			} else if (disabledUnmanagedCommands.Contains (function) || disabledCommands.Contains (function))
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

			if (!useArrayParameters && csFunction.EndsWith ("s"))
				csFunction = csFunction.Substring (0, csFunction.Length - 1);

			int fixedCount, outCount;
			var paramsDict = LearnParams (commandElement, isForHandle && !isExtension, out fixedCount, out outCount);

			var hasResult = csType == "Result";
			if (hasResult)
				csType = "void";

			ParamInfo firstOutParam = null;
			ParamInfo intParam = null;
			string outLen = null;
			ParamInfo dataParam = null;
			var ignoredParameters = new List<ParamInfo> ();
			bool createArray = false;
			bool hasLen = false;
			if (csType == "void") {
				if (outCount == 1) {
					foreach (var param in paramsDict) {
						if (param.Value.isOut) {
							firstOutParam = param.Value;
							switch (firstOutParam.csType) {
							case "Bool32":
							case "IntPtr":
							case "int":
							case "Int32":
							case "UInt32":
							case "Int64":
							case "UInt64":
							case "DeviceSize":
								firstOutParam.isFixed = false;
								break;
							}
							ignoredParameters.Add (param.Value);
							break;
						}
					}
					csType = firstOutParam.csType;
					if (csType != "IntPtr" && firstOutParam.len != null /* && paramsDict.ContainsKey (firstOutParam.len) */) {
						csType += "[]";
						createArray = true;
						hasLen = true;
						intParam = paramsDict.ContainsKey (firstOutParam.len) ? paramsDict [firstOutParam.len] : GetLenParamInfo (firstOutParam.len);
						dataParam = firstOutParam;
						intParam.isFixed = false;
						dataParam.isFixed = false;
					}
				} else if (outCount > 1) {
					createArray = CommandShouldCreateArray (commandElement, paramsDict, ref intParam, ref dataParam);
					if (createArray) {
						ignoredParameters.Add (intParam);
						ignoredParameters.Add (dataParam);
						intParam.isFixed = false;
						dataParam.isFixed = false;
						csType = string.Format ("{0}[]", dataParam.csType);
					}
				}
			}

			if (intParam != null)
				outLen = intParam.csName;

			int arrayParamCount = 0;
			foreach (var param in paramsDict) {
				var info = param.Value;
				if (info.len != null && paramsDict.ContainsKey (info.len) && !info.isOut && info.csType != "IntPtr") {
					var lenParameter = paramsDict [info.len];
					ignoredParameters.Add (lenParameter);
					lenParameter.lenArray = info;
					if (useArrayParameters) {
						info.isArray = true;
						if (lenParameter.csName == outLen)
							outLen = string.Format ("{0}.Length", info.csName);
						info.isFixed = false;
						arrayParamCount++;
					} else {
						lenParameter.constValue = "1";
						if (info.isStruct && !info.needsMarshalling)
							info.isNullable = true;
						else
							switch (info.csType) {
							case "UInt32":
							case "Int32":
							case "UInt64":
							case "Int64":
								info.isNullable = true;
								break;
							}
					}
				}
			}

			if (intParam != null)
				intParam.isOut = false;
			if (dataParam != null)
				dataParam.isOut = false;

			IndentWrite ("public {0}{1} {2} (", (!isExtension && isForHandle) ? "" : "static ", csType, csFunction);
			hasArrayParameter = WriteCommandParameters (commandElement, useArrayParameters, ignoredParameters, null, null, isForHandle && !isExtension, false, paramsDict, isExtension, true);
			WriteLine (")");
			IndentWriteLine ("{");
			IndentLevel++;
			if (hasResult)
				IndentWriteLine ("Result result;");
			if (firstOutParam != null && !hasLen)
				IndentWriteLine ("{0} {1};", csType, firstOutParam.csName);
			IndentWriteLine ("unsafe {");
			IndentLevel++;

			bool isInInterop = false;
			if (createArray) {
				isInInterop = dataParam.isStruct && dataParam.needsMarshalling;
				if (!hasLen) {
					IndentWriteLine ("UInt32 {0};", outLen);
					IndentWrite ("{0}{1}{2}{3} (", hasResult ? "result = " : "", (ignoredParameters.Count == 0 && csType != "void") ? "return " : "", delegateUnmanagedCommands.Contains (function) ? "" : "Interop.NativeMethods.", function);
					WriteCommandParameters (commandElement, useArrayParameters, null, dataParam, null, isForHandle && !isExtension, true, paramsDict, isExtension);
					WriteLine (");");
					CommandHandleResult (hasResult);
				}
				IndentWriteLine ("if ({0} <= 0)", outLen);
				IndentLevel++;
				IndentWriteLine ("return null;");
				IndentLevel--;
				WriteLine ();
				IndentWriteLine ("int size = {0};", dataParam.MarshalSizeSource (this, isInInterop));
				IndentWriteLine ("var ref{0} = new NativeReference ((int)(size * {1}));", dataParam.csName, outLen);
				IndentWriteLine ("var ptr{0} = ref{0}.Handle;", dataParam.csName);
			}

			if (fixedCount > 0) {
				int count = 0;
				foreach (var param in paramsDict) {
					if (param.Value.isFixed && param.Value.isHandle && !param.Value.isConst) {
						IndentWriteLine ("{0} = new {1} ();", param.Key, param.Value.csType);
						count++;
					}
				}
				if (count > 0)
					WriteLine ();

				foreach (var param in paramsDict) {
					if (param.Value.isFixed) {
						IndentWriteLine ("fixed ({0}* ptr{1} = &{1}{2}) {{", GetManagedType (param.Value.csType), GetParamName (param.Key, useArrayParameters), param.Value.isHandle ? ".m" : "");
						IndentLevel++;
					}
				}
			}
			if (outCount > 0)
				foreach (var param in paramsDict) {
					var info = param.Value;
					if (info.isOut && !info.isFixed) // && (ignoredParameters == null || !ignoredParameters.Contains (info)))
						IndentWriteLine ("{0} = new {1} ();", param.Key, info.csType);
				}
			if (arrayParamCount > 0)
				foreach (var param in paramsDict) {
					var info = param.Value;
					if (info.len != null && firstOutParam != info) {
						IndentWriteLine ("var array{0} = {0} == null ? IntPtr.Zero : Marshal.AllocHGlobal ({0}.Length*sizeof ({1}));", info.csName, GetParamArrayType (info));
						IndentWriteLine ("var len{0} = {0} == null ? 0 : {0}.Length;", info.csName);
						IndentWriteLine ("if ({0} != null)", info.csName);
						IndentLevel++;
						IndentWriteLine ("for (int i = 0; i < {0}.Length; i++)", info.csName);
						IndentLevel++;
						IndentWriteLine ("(({0}*)array{1}) [i] = {3}({1} [i]{2});", GetParamArrayType (info), info.csName, ((info.isStruct && info.needsMarshalling) || info.isHandle) ? ".m" : "", (info.isStruct && info.needsMarshalling) ? "*" : "");
						IndentLevel--;
						IndentLevel--;
					}
				}

			foreach (var param in paramsDict) {
				var info = param.Value;
				if (info.isNullable) {
					string name = GetParamName (info.csName, useArrayParameters);
					IndentWriteLine ("{0} val{1} = {1} ?? default({0});", info.csType, name);
					IndentWriteLine ("{0}* ptr{1} = {1} != null ? &val{1} : ({0}*)IntPtr.Zero;", info.csType, name);
				}
			}

			IndentWrite ("{0}{1}{2}{3} (", hasResult ? "result = " : "", (ignoredParameters.Count == 0 && csType != "void") ? "return " : "", delegateUnmanagedCommands.Contains (function) ? "" : string.Format ("{0}.NativeMethods.", InteropNamespace), function);
			WriteCommandParameters (commandElement, useArrayParameters, null, null, dataParam, isForHandle && !isExtension, true, paramsDict, isExtension);
			WriteLine (");");

			if (fixedCount > 0) {
				foreach (var param in paramsDict) {
					if (param.Value.isFixed) {
						IndentLevel--;
						IndentWriteLine ("}");
					}
				}
			}

			if (arrayParamCount > 0)
				foreach (var param in paramsDict) {
					var info = param.Value;
					if (info.len != null && firstOutParam != info)
						IndentWriteLine ("Marshal.FreeHGlobal (array{0});", info.csName);
				}
			CommandHandleResult (hasResult);
			if (firstOutParam != null && !createArray) {
				WriteLine ();
				IndentWriteLine ("return {0};", firstOutParam.csName);
			} else if (createArray) {
				WriteLine ();
				IndentWriteLine ("if ({0} <= 0)", outLen);
				IndentLevel++;
				IndentWriteLine ("return null;");
				IndentLevel--;
				IndentWriteLine ("var arr = new {0} [{1}];", dataParam.csType, outLen);
				IndentWriteLine ("for (int i = 0; i < {0}; i++) {{", outLen);
				IndentLevel++;
				if (isInInterop || !dataParam.isStruct) {
					if (dataParam.isStruct)
						IndentWriteLine ("arr [i] = new {0} (new NativePointer (ref{4}, (IntPtr)({1}(({2}{3}*)ptr{4}) [i])));", dataParam.csType, dataParam.isStruct ? "&" : "", isInInterop ? "Interop." : "", dataParam.isHandle ? GetHandleType (handles [dataParam.csType]) : dataParam.csType, dataParam.csName);
					else {
						IndentWriteLine ("arr [i] = new {0} ();", dataParam.csType);
						IndentWriteLine ("arr [i]{0} = {1}(({2}{3}*)ptr{4}) [i];", (dataParam.isStruct || dataParam.isHandle) ? ".m" : "", dataParam.isStruct ? "&" : "", isInInterop ? "Interop." : "", dataParam.isHandle ? GetHandleType (handles [dataParam.csType]) : dataParam.csType, dataParam.csName);
					}
				} else
					IndentWriteLine ("arr [i] = ((({0}*)ptr{1}) [i]);", dataParam.csType, dataParam.csName);
				IndentLevel--;
				IndentWriteLine ("}");
				WriteLine ();
				IndentWriteLine ("return arr;");
			}

			IndentLevel--;
			IndentWriteLine ("}");
			IndentLevel--;
			IndentWriteLine ("}");

			if (useArrayParameters && hasArrayParameter && !ignoredSimplifiedCommands.Contains (csFunction))
				return WriteCommand (commandElement, true, false, isForHandle, handleName, isExtension);

			return true;
		}

		string GetHandleType (HandleInfo info)
		{
			switch (info.type) {
			case "VK_DEFINE_NON_DISPATCHABLE_HANDLE":
				return "UInt64";
			case "VK_DEFINE_HANDLE":
				return "IntPtr";
			default:
				throw new Exception ("unknown handle type: " + info.type);
			}
		}

		HashSet<string> handlesWithDefaultConstructors = new HashSet<string> {
			"Instance"
		};
		bool WriteHandle (XElement handleElement)
		{
			string csName = GetTypeCsName (handleElement.Element ("name").Value, "handle");
			HandleInfo info = handles [csName];
			bool isRequired = false;

			if (requiredCommands != null) {
				foreach (var commandElement in info.commands)
					if (requiredCommands.Contains (commandElement.Element ("proto").Element ("name").Value)) {
						isRequired = true;
						break;
					}

				if (!isRequired)
					return false;
			}

			var className = string.Format ("{0}{1}", csName, isRequired ? "Extension" : "");
			var marshallingInterface = info.type == "VK_DEFINE_NON_DISPATCHABLE_HANDLE" ? "INonDispatchableHandleMarshalling" : "IMarshalling";
			IndentWriteLine ("public {0} class {1}{2}", isRequired ? "static" : "partial", className, isRequired ? "" : string.Format (" : {0}", marshallingInterface));
			IndentWriteLine ("{");
			IndentLevel++;
			if (!isRequired && !handlesWithDefaultConstructors.Contains (csName))
				IndentWriteLine ("internal {0}() {{}}\n", className);
			//// todo: implement marshalling
			bool written = false;
			if (requiredCommands == null) {
				var handleType = GetHandleType (info);
				IndentWriteLine ("internal {0} m;\n", handleType);
				IndentWriteLine ("{0} {1}.Handle {{", handleType, marshallingInterface);
				IndentLevel++;
				IndentWriteLine ("get {");
				IndentLevel++;
				IndentWriteLine ("return m;");
				IndentLevel--;
				IndentWriteLine ("}");
				IndentLevel--;
				IndentWriteLine ("}");

				written = true;
			}

			if (info.commands.Count > 0) {
				foreach (var element in info.commands) {
					written = WriteCommand (element, written, true, true, csName, isRequired);
				}
			}

			IndentLevel--;
			IndentWriteLine ("}");

			return true;
		}

		void GenerateHandles ()
		{
			CreateFile ("Handles", UsingNamespaceFlags.Interop | UsingNamespaceFlags.Collections, Namespace);
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
				bool isStruct = structures.ContainsKey (csType);
				bool isRequired = requiredTypes != null && requiredTypes.Contains (type);
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

				name = GetParamName (param);

				if (previous)
					Write (", ");
				else
					previous = true;

				if (param.Value.Contains (type + "**"))
					csType += "*";

				Write ("{0}{1} {2}", (isStruct && requiredCommands != null && !isRequired) ? "Vulkan.Interop." : "", csType, keywords.Contains (name) ? "@" + name : name);
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
			"vkGetPhysicalDeviceWin32PresentationSupportKHR",
			"vkCreateXlibSurfaceKHR",
			"vkGetPhysicalDeviceXlibPresentationSupportKHR",
			"vkCreateXcbSurfaceKHR",
			"vkCreateAndroidSurfaceKHR",
			"vkGetMemoryWin32HandleNV",
			"vkImportSemaphoreWin32HandleKHR",
			"vkImportFenceWin32HandleKHR",
			"vkCreateIOSSurfaceMVK",
			// TODO: support fixed array of Handles
			"vkEnumeratePhysicalDeviceGroupsKHX",
		};

		HashSet<string> delegateUnmanagedCommands = new HashSet<string> {
			"vkCreateDebugReportCallbackEXT",
			"vkDestroyDebugReportCallbackEXT",
			"vkDebugReportMessageEXT",
		};

		bool WriteUnmanagedCommand (XElement commandElement)
		{
			string function = commandElement.Element ("proto").Element ("name").Value;
			string type = commandElement.Element ("proto").Element ("type").Value;
			string csType = GetTypeCsName (type);

			// todo: extensions support
			if (requiredCommands != null) {
				if (!requiredCommands.Contains (function))
					return false;
			} else if (disabledUnmanagedCommands.Contains (function))
				return false;

			// todo: function pointers
			if (csType.StartsWith ("PFN_"))
				csType = "IntPtr";

			if (delegateUnmanagedCommands.Contains (function))
				IndentWrite ("internal unsafe delegate {0} {1} (", csType, function);
			else {
				IndentWriteLine ("[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Winapi)]");
				IndentWrite ("internal static unsafe extern {0} {1} (", csType, function);
			}
			WriteUnmanagedCommandParameters (commandElement);
			WriteLine (");");

			return true;
		}

		string InteropNamespace {
			get {
				return string.Format ("{0}Interop", platform == null ? "" : (platform + "."));
			}
		}

		string Namespace {
			get {
				return string.Format ("Vulkan{0}", platform == null ? "" : ("." + platform));
			}
		}

		void GenerateCommands ()
		{
			CreateFile ("ImportedCommands", UsingNamespaceFlags.Interop | (requiredCommands == null ? 0 : UsingNamespaceFlags.VulkanInterop), "Vulkan." + InteropNamespace, "Interop");

			IndentWriteLine ("internal static class NativeMethods");
			IndentWriteLine ("{");
			IndentLevel++;
			IndentWriteLine ("const string VulkanLibrary = \"{0}\";\n", (platform == null || platform == "Windows") ? "vulkan-1" : (platform == "iOS" ? "__Internal" : "vulkan"));

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
			CreateFile ("Commands", UsingNamespaceFlags.Interop | UsingNamespaceFlags.Collections);

			IndentWriteLine ("public static partial class Commands");
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

				written = WriteCommand (command, written, true);
			}

			IndentLevel--;
			IndentWriteLine ("}");

			FinalizeFile ();
		}

		string EnumExtensionValue (XElement element, int number, ref string csEnumName)
		{
			var offsetAttribute = element.Attribute ("offset");
			if (offsetAttribute != null) {
				int direction = 1;
				var dirAttr = element.Attribute ("dir");
				if (dirAttr != null && dirAttr.Value == "-")
					direction = -1;
				int offset = Int32.Parse (offsetAttribute.Value);

				return (direction*(1000000000 + (number - 1)*1000 + offset)).ToString ();
			}
			var valueAttribute = element.Attribute ("value");
			if (valueAttribute != null)
				return valueAttribute.Value;

			var bitposAttribute = element.Attribute ("bitpos");
			if (bitposAttribute != null) {
				csEnumName = GetFlagBitsName (csEnumName);

				return FormatFlagValue (Int32.Parse (bitposAttribute.Value));
			}

			throw new Exception (string.Format ("unexpected extension enum value in: {0}", element));
		}

		void LearnExtension (XElement extensionElement)
		{
			var extensions = from e in extensionElement.Elements ("require").Elements ("enum") where e.Attribute ("extends") != null select e;
			int number = Int32.Parse (extensionElement.Attribute ("number").Value);
			foreach (var element in extensions) {
				string enumName = GetTypeCsName (element.Attribute ("extends").Value, "enum");
				var info = new EnumExtensionInfo { name = element.Attribute ("name").Value, value = EnumExtensionValue (element, number, ref enumName) };
				if (!enumExtensions.ContainsKey (enumName))
					enumExtensions [enumName] = new List<EnumExtensionInfo> ();
				enumExtensions [enumName].Add (info);
			}
		}

		void LearnExtensions ()
		{
			var elements = from e in specTree.Elements ("extensions").Elements ("extension") where e.Attribute ("supported").Value != "disabled" select e;

			foreach (var element in elements)
				LearnExtension (element);
		}

		void PrepareExtensionSets (string[] extensionNames)
		{
			requiredTypes = new HashSet<string> ();
			requiredCommands = new HashSet<string> ();

			foreach (var name in extensionNames)
				PrepareExtensionSets (name);
		}

		void PrepareExtensionSets (string extensionName)
		{
			var elements = from e in specTree.Elements ("extensions").Elements ("extension") where e.Attribute ("name").Value == extensionName select e.Element ("require");

			foreach (var element in elements.Elements ()) {
				switch (element.Name.ToString ()) {
				case "type":
					requiredTypes.Add (element.Attribute ("name").Value);
					break;
				case "command":
					requiredCommands.Add (element.Attribute ("name").Value);
					break;
				}
			}
		}

		void GeneratePlatformExtension (string name, string extensionName)
		{
			GeneratePlatformExtension (name, new string [] { extensionName });
		}

		void GeneratePlatformExtension (string name, string[] extensionNames)
		{
			platform = name;
			var currentPath = outputPath;
			outputPath += string.Format ("{0}..{0}Platforms{0}{1}", Path.DirectorySeparatorChar, name);

			PrepareExtensionSets (extensionNames);

			LearnStructsAndUnions ();
			GenerateStructs ();
			GenerateCommands ();
			GenerateHandles ();

			outputPath = currentPath;
		}

		void GenerateExtensions ()
		{
			GeneratePlatformExtension ("Android", "VK_KHR_android_surface");
			GeneratePlatformExtension ("Linux", new string[] {
				"VK_KHR_xlib_surface",
				"VK_KHR_xcb_surface",
				"VK_KHR_wayland_surface",
				"VK_KHR_mir_surface" } );
			GeneratePlatformExtension ("Windows", new string [] {
				"VK_KHR_win32_surface",
				"VK_NV_external_memory_win32",
				"VK_NV_win32_keyed_mutex",
			} );
			GeneratePlatformExtension("iOS", "VK_MVK_ios_surface");
		}

		void WriteTypes (XmlDocument doc, XmlElement types, string elementName, TypeInfo[] typeInfo)
		{
			foreach (var info in typeInfo) {
				var element = doc.CreateElement (elementName);
				element.SetAttribute ("name", info.name);
				element.SetAttribute ("csName", info.csName);
				types.AppendChild (element);

				if (info.members == null)
					continue;

				foreach (var member in info.members) {
					var memberElement = doc.CreateElement ("member");
					memberElement.SetAttribute ("name", member.Value);
					memberElement.SetAttribute ("csName", member.Key);
					element.AppendChild (memberElement);
				}
			}
		}

		void WriteTypeInformation ()
		{
			var doc = new XmlDocument ();

			XmlElement types = doc.CreateElement ("types");
			doc.AppendChild (types);

			WriteTypes (doc, types, "enum", enums.Values.ToArray ());
			WriteTypes (doc, types, "structure", structures.Values.ToArray ());

			doc.Save (outputPath + Path.DirectorySeparatorChar + "types.xml");
		}

		bool IsEnum (string type)
		{
			return enums.ContainsKey (type);
		}
	}
}
