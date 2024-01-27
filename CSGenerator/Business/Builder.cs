using System.Text;

namespace CSGenerator {
    internal class Builder {
        internal string Build(Parser p) {
            if (p.rootClass == null) {
                throw new Exception("Root class structure is null. This shouldn't happen.");
            }

            string t = "// Generated using CSGenerator v1.0: https://github.com/criticalsession/csgenerator" + 
                Environment.NewLine + p.templateString;
            t = t.Replace("{{CLASS_NAME}}", p.rootClass.ClassName);
            t = t.Replace("{{NAMESPACE}}", p.rootNamespace);

            StringBuilder usings = new();
            usings.AppendLine("using System;");
            usings.AppendLine("using System.Collections.Generic;");
            usings.AppendLine("using System.Linq;");
            usings.AppendLine("using System.Text;");
            usings.AppendLine("using System.Threading.Tasks;");
            t = t.Replace("{{USINGS}}", usings.ToString());

            StringBuilder fields = new();
            foreach (var f in p.rootClass.Fields) {
                fields.Append(f.Write());
            }
            t = t.Replace("{{FIELDS}}", fields.ToString());

            StringBuilder methods = new();
            foreach (var m in p.rootClass.Methods) {
                methods.Append(m.Write(p.rootClass.Fields));
            }
            t = t.Replace("{{METHODS}}", methods.ToString());

            StringBuilder subClasses = new();
            t = t.Replace("{{SUB_CLASSES}}", subClasses.ToString());

            if (!Directory.Exists(Utils.GetOutDirectory()))
                Directory.CreateDirectory(Utils.GetOutDirectory());

            string path = Utils.GetOutPath(p.rootClass.ClassName);
            File.WriteAllText(path, t);

            return path;
        }
    }
}
