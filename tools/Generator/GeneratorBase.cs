using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VulkanSharp.Generator
{
	public class GeneratorBase
	{
		static Dictionary<string, string> specialParts = new Dictionary<string, string> {
			{ "AMD", "Amd" },
			{ "API", "Api" },
			{ "EXT", "Ext" },
			{ "KHR", "Khr" },
			{ "1D", "1D" },
			{ "2D", "2D" },
			{ "3D", "3D" },
			{ "NV", "Nv" }
		};

		protected string TranslateCName (string name)
		{
			StringWriter sw = new StringWriter ();
			bool first = true;

			foreach (var part in name.Split ('_')) {
				if (first) {
					first = false;
					if (name.StartsWith ("VK", StringComparison.OrdinalIgnoreCase))
						continue;
				}

				if (specialParts.ContainsKey (part))
					sw.Write (specialParts [part]);
				else if (part.Length > 0) {
					var chars = part.ToCharArray ();
					if (chars.All (c => char.IsUpper (c)))
						sw.Write (part [0] + part.Substring (1).ToLower ());
					else {
						string formatted = "";
						bool upIt = true;
						bool wasLower = false;
						bool wasDigit = false;
						foreach (var ch in chars) {
							formatted += upIt ? char.ToUpper (ch) : (wasLower ? ch : char.ToLower (ch));
							upIt = char.IsDigit (ch);
							wasLower = char.IsLower (ch);
							if (wasDigit && char.ToLower (ch) == 'd')
								upIt = true;
							wasDigit = char.IsDigit (ch);
						}
						sw.Write (formatted);
					}
				}
			}

			return sw.ToString ();
		}

		static Dictionary<string, string> basicTypesMap = new Dictionary<string, string> {
			{ "int32_t", "Int32" },
			{ "uint32_t", "UInt32" },
			{ "uint64_t", "UInt64" },
			{ "uint8_t", "byte" },
			{ "size_t", "UIntPtr" },
			{ "xcb_connection_t", "IntPtr" },
			{ "xcb_window_t", "IntPtr" },
			{ "xcb_visualid_t", "Int32" }
		};

		HashSet<string> knownTypes = new HashSet<string> {
			"void",
			"char",
			"float",
		};

		protected Dictionary<string, string> typesTranslation = new Dictionary<string, string> () {
			{ "ANativeWindow", "IntPtr" },
			{ "HWND", "IntPtr" },
			{ "HINSTANCE", "IntPtr" },
			{ "HANDLE", "IntPtr" },
			{ "DWORD", "UInt32" },
			{ "SECURITY_ATTRIBUTES", "SecurityAttributes" },
		};

		protected static Dictionary<string, string> extensions = new Dictionary<string, string> {
			{ "AMD", "Amd" },
			{ "EXT", "Ext" },
			{ "IMG", "Img" },
			{ "KHR", "Khr" },
			{ "NV", "Nv" }
		};

		protected string GetTypeCsName (string name, string typeName = "type")
		{
			if (typesTranslation.ContainsKey (name))
				return typesTranslation [name];

			string csName;

			if (name.StartsWith ("Vk", StringComparison.OrdinalIgnoreCase))
				csName = name.Substring (2);
			else if (name.EndsWith ("_t")) {
				if (!basicTypesMap.ContainsKey (name))
					throw new NotImplementedException (string.Format ("Mapping for the basic type {0} isn't supported", name));

				csName = basicTypesMap [name];
			} else {
				if (typeName == "type" && !knownTypes.Contains (name) && !name.StartsWith ("PFN_", StringComparison.Ordinal))
					Console.WriteLine ("warning: {0} name '{1}' doesn't start with Vk prefix or end with _t suffix", typeName, name);
				csName = name;
			}

			foreach (var ext in extensions)
				if (csName.EndsWith (ext.Key))
					csName = csName.Substring (0, csName.Length - ext.Value.Length) + ext.Value;

			return csName;
		}

		protected HashSet<string> knownBitmaps = new HashSet<string> {
			"VkExternalMemoryHandleTypeFlagBitsNV",
			"VkExternalMemoryFeatureFlagBitsNV",
		};

		protected string GetEnumCsName (string name, bool bitmask)
		{
			string csName = GetTypeCsName (name, "enum");

			if (bitmask || knownBitmaps.Contains (name)) {
				string suffix = null;
				foreach (var ext in extensions)
					if (csName.EndsWith (ext.Value)) {
						suffix = ext.Value + suffix;
						csName = csName.Substring (0, csName.Length - ext.Value.Length);
					}
				if (csName.EndsWith ("FlagBits"))
					csName = csName.Substring (0, csName.Length - 4) + "s";
				if (suffix != null)
					csName += suffix;
			}

			return csName;
		}
	}
}

