using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string path = "./" + p.rootClassName + ".cs";

            File.WriteAllText(path, t);
            return path;
        }
    }
}
