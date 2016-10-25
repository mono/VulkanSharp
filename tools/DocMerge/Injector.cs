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
		string vulkanDocsPath = "Vulkan-Docs/doc/specs/vulkan";
		string vulkanSharpDocsPath = "docs/en/Vulkan";

		public Injector ()
		{
			ReadTypesInfo ();
			ReadTxtDocInDirectory (vulkanDocsPath + Path.DirectorySeparatorChar + "chapters");
		}

		struct Reference
		{
			public string name;
			public string description;
			public List<string> content;
		}
		Dictionary<string, Reference> references = new Dictionary<string, Reference> ();
		Reference currentRef;

		void FinishCurrentReference ()
		{
			if (currentRef.content != null && currentRef.name != null) {
				Console.WriteLine ("finished {0} lines {1}", currentRef.name, currentRef.content.Count);
				references [currentRef.name] = currentRef;
				currentRef.content = null;
			}
		}

		void ReadTxtDoc (string path)
		{
			Console.WriteLine ("read {0}", path);
			var reader = new StreamReader (File.OpenRead (path));
			var refBegin = new Regex ("^\\/\\/ refBegin ([^ ]+) (.+)$");
			var refEnd = new Regex ("^\\/\\/ refEnd");
			string line;

			while ((line = reader.ReadLine ()) != null) {
				var match = refBegin.Match (line);
				if (match != null && match.Success) {
					if (match.Groups.Count < 3) {
						Console.WriteLine ("warning: unparsed refBegin: {0}", line);
						continue;
					}
					FinishCurrentReference ();

					currentRef.name = match.Groups [1].Value;
					currentRef.description = match.Groups [2].Value;
					currentRef.content = new List<string> ();
				} else {
					match = refEnd.Match (line);
					if (match != null && match.Success) {
						FinishCurrentReference ();
					} else if (currentRef.content != null)
						currentRef.content.Add (line);
				}
			}
		}

		void ReadTxtDocInDirectory (string directory)
		{
			foreach (var file in Directory.EnumerateFiles (directory, "*.txt"))
				ReadTxtDoc (file);
			foreach (var subDirectory in Directory.EnumerateDirectories (directory))
				ReadTxtDocInDirectory (subDirectory);
		}

		string GetVulkanSharpDocPath (string name)
		{
			return vulkanSharpDocsPath + Path.DirectorySeparatorChar + name + ".xml";
		}

		struct MemberInfo
		{
			public string name;
			public string description;
		}

		Dictionary<string, MemberInfo> ParseReference (Reference reference, TypeKind kind)
		{
			var members = new Dictionary<string, MemberInfo> ();
			Regex memberRegex;
			switch (kind) {
			case TypeKind.Structure:
				memberRegex = new Regex ("^[ ]*\\* pname\\:([^ ]+) (.+)$");
				break;
			case TypeKind.Enum:
				memberRegex = new Regex ("^[ ]*\\*\\*.* ename\\:([^ ]+) (.+)$");
				break;
			default:
				return null;
			}
			MemberInfo info = new MemberInfo ();

			foreach (var line in reference.content) {
				var match = memberRegex.Match (line);
				if (match != null && match.Success && match.Groups.Count > 2) {
					if (info.name != null)
						members [info.name] = info;
					info.name = match.Groups [1].Value;
					info.description = match.Groups [2].Value;
				} else {
					if (line.Trim ().Length < 1) {
						if (info.name != null) {
							members [info.name] = info;
							info.name = null;
						}
					} else if (!string.IsNullOrEmpty (info.name))
						info.description += " " + line.Trim ();
				}
			}

			return members;
		}

		void UpdateSummaryAndRemarks (XElement parentElement, string summary, string remarks)
		{
			var docsElement = parentElement.Element ("Docs");
			if (docsElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain Docs element", parentElement);
				return;
			}

			var summaryElement = docsElement.Element ("summary");
			if (summaryElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain summary element", docsElement);
				return;
			}

			summaryElement.Value = summary;

			var remarksElement = docsElement.Element ("remarks");
			if (remarksElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain summary element", docsElement);
				return;
			}

			remarksElement.Value = remarks;
		}

		void UpdateMembers (XElement typeElement, string typeName, Reference reference)
		{
			var membersElement = typeElement.Element ("Members");
			if (membersElement == null)
				return;

			if (!typesInfo.ContainsKey (typeName)) {
				Console.WriteLine ("warning: do not have info about {0} type", typeName);
				return;
			}

			var members = typesInfo [typeName].members;
			var membersInfo = ParseReference (reference, typesInfo [typeName].kind);
			foreach (var memberElement in membersElement.Elements ("Member")) {
				var memberName = memberElement.Attribute ("MemberName").Value;

				if (string.IsNullOrEmpty (memberName)) {
					Console.WriteLine ("warning: {0} doesn't contain member name", memberElement);
					continue;
				}

				if (!members.ContainsKey (memberName)) {
					Console.WriteLine ("warning: do not have info in members about {0} member", memberName);
					continue;
				}

				var vkName = members [memberName];
				if (!membersInfo.ContainsKey (vkName)) {
					Console.WriteLine ("warning: do not have info in membersInfo about {0} member", vkName);
					continue;
				}

				UpdateSummaryAndRemarks (memberElement, membersInfo [vkName].description, "");
			}
		}

		void UpdateDocXml (string csName, string name)
		{
			var path = GetVulkanSharpDocPath (csName);

			XDocument doc;

			try {
				doc = XDocument.Load (path);
			} catch {
				Console.WriteLine ("warning: didn't find documentation for {0}", csName);
				return;
			}

			var typeElement = doc.Element ("Type");
			if (typeElement == null) {
				Console.WriteLine ("warning: {0} doesn't contain Type element", path);
				return;
			}

			if (!references.ContainsKey (name)) {
				Console.WriteLine ("warning: known references info doesn't contain info for {0}", name);
				return;
			}

			UpdateSummaryAndRemarks (typeElement, references [name].description, "");
			UpdateMembers (typeElement, name, references [name]);

			doc.Save (path);
		}

		public void UpdateDocXml ()
		{
			foreach (var name in typesInfo)
				UpdateDocXml (name.Value.csName, name.Key);
		}

		enum TypeKind
		{
			Enum,
			Structure
		}

		class TypeInfo
		{
			public TypeKind kind;
			public string name;
			public string csName;
			public Dictionary<string, string> members;
		}
		Dictionary<string, TypeInfo> typesInfo;

		void ReadMembersInfo (IEnumerable<XElement> elements, TypeKind kind)
		{
			foreach (var typeElement in elements) {
				var info = new TypeInfo () {
					csName = typeElement.Attribute ("csName").Value,
					name = typeElement.Attribute ("name").Value,
					kind = kind,
				};
				typesInfo [info.name] = info;

				var memberElements = typeElement.Elements ("member");
				if (memberElements == null)
					continue;
				info.members = new Dictionary<string, string> ();
				foreach (var memberElement in memberElements)
					info.members [memberElement.Attribute ("csName").Value] = memberElement.Attribute ("name").Value;
			}
		}

		void ReadTypesInfo ()
		{
			var doc = XDocument.Load ("src/Vulkan/types.xml");

			typesInfo = new Dictionary<string, TypeInfo> ();
			ReadMembersInfo (doc.Element ("types").Elements ("enum"), TypeKind.Enum);
			ReadMembersInfo (doc.Element ("types").Elements ("structure"), TypeKind.Structure);
		}
	}
}

