using System.Text;

namespace CSGenerator {
    internal class Builder {
        internal string Build(Parser p) {
            string t = p.templateString;
            t = t.Replace("{{CLASS_NAME}}", p.rootClassName);

            StringBuilder usings = new();
            usings.AppendLine("using System;");
            usings.AppendLine("using System.Collections.Generic;");
            usings.AppendLine("using System.Linq;");
            usings.AppendLine("using System.Text;");
            usings.AppendLine("using System.Threading.Tasks;");
            t = t.Replace("{{USINGS}}", usings.ToString());

            StringBuilder fields = new();
            foreach (var dec in p.declarations.Where(p => !p.isFunction)) {
                if (dec.isPrivate) fields.Append("private ");
                else fields.Append("public ");

                if (dec.isStatic) fields.Append("static ");

                fields.Append(dec.type + " ");
                fields.AppendLine(dec.name + ";");
            }
            t = t.Replace("{{FIELDS}}", fields.ToString());

            StringBuilder methods = new();
            foreach (var dec in p.declarations.Where(p => p.isFunction)) {
                if (dec.isPrivate) methods.Append("private ");
                else methods.Append("public ");

                if (dec.isStatic) methods.Append("static ");

                if (String.IsNullOrEmpty(dec.functionReturnType)) methods.Append("void ");
                else methods.Append(dec.functionReturnType + " ");

                methods.Append(dec.name + "(");
                if (dec.functionParams != null) {
                    methods.Append(String.Join(',', dec.functionParams.Select(d => d.type + " " + d.name)));
                }
                methods.AppendLine(")");
                methods.AppendLine("{");
                methods.Append("throw new NotImplementedException();");
                methods.AppendLine("}");
            }
            t = t.Replace("{{METHODS}}", methods.ToString());

            StringBuilder subClasses = new();
            t = t.Replace("{{SUB_CLASSES}}", subClasses.ToString());

            if (!Directory.Exists("./out/"))
                Directory.CreateDirectory("./out/");

            string path = "./out/" + p.rootClassName + ".cs";

            File.WriteAllText(path, t);
            return path;
        }
    }
}
