using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace CSGenerator {
    internal class Builder {
        internal string Build(Parser p) {
            if (p.rootClass == null) {
                throw new Exception("Root class structure is null. This shouldn't happen.");
            }

            string mainTemplateString = Utils.ReadTemplate(p.templateName);
            string t = "// Generated using CSGenerator v1.0: https://github.com/criticalsession/csgenerator" +
                Environment.NewLine + mainTemplateString;
            t = t.Replace("{{NAMESPACE}}", p.rootNamespace);

            StringBuilder usings = new();
            usings.AppendLine("using System;");
            usings.AppendLine("using System.Collections.Generic;");
            usings.AppendLine("using System.Linq;");
            usings.AppendLine("using System.Text;");
            usings.AppendLine("using System.Threading.Tasks;");
            t = t.Replace("{{USINGS}}", usings.ToString());

            t = t.Replace("{{CLASS}}", BuildClass(p.rootClass, p.templateName, isRoot: true));

            if (!Directory.Exists(Utils.GetOutDirectory()))
                Directory.CreateDirectory(Utils.GetOutDirectory());

            string path = Utils.GetOutPath(p.rootClass.ClassName);
            File.WriteAllText(path, t);

            return path;
        }

        internal string BuildClass(ClassStructure c, string templateName, bool isRoot = false) {
            string t = (isRoot ? "" : Environment.NewLine) + Utils.ReadTemplate(templateName + "_class");
            t = t.Replace("{{CLASS_NAME}}", c.ClassName);

            StringBuilder fields = new();
            foreach (var f in c.Fields) {
                fields.Append(f.Write(c.Fields));
            }
            t = t.Replace("{{FIELDS}}\r\n", fields.ToString());

            StringBuilder methods = new();
            foreach (var m in c.Methods) {
                methods.Append(m.Write(c.Fields));
            }
            t = t.Replace("{{METHODS}}\r\n", methods.ToString());

            StringBuilder subClasses = new();
            foreach (var s in c.subClasses) {
                subClasses.AppendLine(BuildClass(s, templateName));
            }
            t = t.Replace("{{SUB_CLASSES}}\r\n", subClasses.ToString());

            return t;
        }
    }
}
