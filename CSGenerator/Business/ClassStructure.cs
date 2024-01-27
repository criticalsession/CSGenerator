using System.Text;

namespace CSGenerator {
    internal class ClassStructure {
        internal string ClassName = "";
        internal List<FieldStructure> Fields = new();
        internal List<MethodStructure> Methods = new();

        internal ClassStructure(string className, List<Declaration> decs) {
            ClassName = className;

            foreach (var f in decs.Where(p => !p.isFunction)) {
                Fields.Add(new FieldStructure(f));
            }

            foreach (var m in decs.Where(p => p.isFunction)) {
                Methods.Add(new MethodStructure(m));
            }
        }

        internal class FieldStructure {
            internal string name;
            internal string type;
            internal bool isStatic;
            internal bool isPrivate;

            internal FieldStructure(Declaration dec) {
                name = dec.name;
                type = dec.type;
                isStatic = dec.isStatic;
                isPrivate = dec.isPrivate;
            }

            public override string ToString() {
                StringBuilder sb = new();

                if (isPrivate) sb.Append("private ");
                else sb.Append("public ");

                if (isStatic) sb.Append("static ");

                sb.Append(type + " ");
                sb.AppendLine(name + ";");

                return sb.ToString();
            }
        }

        internal class MethodStructure {
            internal string name;
            internal bool isStatic;
            internal bool isPrivate;
            internal List<FieldStructure> functionParams = new();
            internal string functionReturnType;

            internal MethodStructure(Declaration dec) {
                name = dec.name;
                isStatic = dec.isStatic;
                isPrivate = dec.isPrivate;
                functionReturnType = String.IsNullOrEmpty(dec.functionReturnType) ? "void" : dec.functionReturnType;

                if (dec.functionParams != null) {
                    foreach (var p in dec.functionParams) {
                        FieldStructure s = new(p);
                        functionParams.Add(s);
                    }
                }
            }

            public override string ToString() {
                StringBuilder sb = new();

                if (isPrivate) sb.Append("private ");
                else sb.Append("public ");

                if (isStatic) sb.Append("static ");

                sb.Append(functionReturnType + " ");

                sb.Append(name + "(");
                if (functionParams != null) {
                    sb.Append(String.Join(',', functionParams.Select(d => d.type + " " + d.name)));
                }
                sb.AppendLine(")");
                sb.AppendLine("{");
                sb.Append("throw new NotImplementedException();");
                sb.AppendLine("}");

                return sb.ToString();
            }
        }
    }
}
