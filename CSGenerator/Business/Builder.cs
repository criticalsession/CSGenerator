﻿using System.Text;

namespace CSGenerator
{
	internal class Builder
	{
		internal static string Build(Parser p)
		{
			if (p.rootClass == null)
			{
				throw new Exception("Root class structure is null. This shouldn't happen.");
			}

			StringBuilder header = new();
			if (p.settings?.AddGeneratorHeader ?? true)
			{
				if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
				{
					string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
					header.AppendLine("// Generated by " + currentUser);
					header.AppendLine("// On " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
				}
				else
				{
					header.AppendLine("// Generated on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
				}

				header.AppendLine("// Using CSGenerator v1.0 (https://github.com/criticalsession/csgenerator)");
				header.AppendLine();
			}

			header.Append("// Classes in file: ");
			for (int i = 0; i < p.classes.Count; i++)
			{
				header.Append(p.classes[i]);
				if (i < p.classes.Count - 1) header.Append(", ");
			}
			header.AppendLine();

			string mainTemplateString = Template.ReadTemplateText(p.templateName, Template.TemplateType.Root);
			string t = header.ToString() + mainTemplateString;
			t = t.Replace("{{NAMESPACE}}", p.rootNamespace);

			StringBuilder usings = new();
			usings.AppendLine("using System;");
			usings.AppendLine("using System.Collections.Generic;");
			usings.AppendLine("using System.Linq;");
			usings.AppendLine("using System.Text;");
			usings.Append("using System.Threading.Tasks;");
			t = t.Replace("{{USINGS}}", usings.ToString());

			t = t.Replace("{{CLASS}}", BuildClass(p.rootClass, p.templateName, isRoot: true));

			if (!Directory.Exists(Utils.GetOutDirectory(p.settings?.OutDir)))
				Directory.CreateDirectory(Utils.GetOutDirectory(p.settings?.OutDir));

			string path = Utils.GetOutPath(p.settings?.OutDir, p.rootClass.ClassName);
			File.WriteAllText(path, t);

			return path;
		}

		internal static string BuildClass(ClassStructure c, string templateName, bool isRoot = false)
		{
			string t = (isRoot ? "" : Environment.NewLine) + Template.ReadTemplateText(templateName, Template.TemplateType.Class);
			t = t.Replace("{{CLASS_NAME}}", c.ClassName);

			StringBuilder fields = new();
			foreach (var f in c.Fields)
			{
				fields.Append(f.Write(c.Fields));
			}
			t = t.Replace("{{FIELDS}}\r\n", fields.ToString());

			StringBuilder methods = new();
			foreach (var m in c.Methods)
			{
				methods.Append(m.Write(c.Fields));
			}
			t = t.Replace("{{METHODS}}\r\n", methods.ToString());

			StringBuilder subClasses = new();
			foreach (var s in c.subClasses)
			{
				subClasses.AppendLine(BuildClass(s, templateName));
			}
			t = t.Replace("{{SUB_CLASSES}}\r\n", subClasses.ToString());

			return t;
		}
	}
}
