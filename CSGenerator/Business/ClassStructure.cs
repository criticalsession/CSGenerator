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
                if (m.isConstructor) m.name = ClassName;
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

            internal string Write() {
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
            internal bool isConstructor;
            internal List<FieldStructure> functionParams = new();
            internal string functionReturnType;

            internal MethodStructure(Declaration dec) {
                name = dec.name;
                isStatic = dec.isStatic;
                isPrivate = dec.isPrivate;
                isConstructor = dec.isConstructor;
                functionReturnType = 
                    String.IsNullOrEmpty(dec.functionReturnType) || dec.functionReturnType.ToLower() == "null" 
                    ? "void" 
                    : dec.functionReturnType;

                if (dec.functionParams != null) {
                    foreach (var p in dec.functionParams) {
                        FieldStructure s = new(p);
                        functionParams.Add(s);
                    }
                }
            }

            internal string Write(List<FieldStructure> classFields) {
                StringBuilder sb = new();

                if (isPrivate) sb.Append("private ");
                else sb.Append("public ");

                if (!isConstructor) {
                    if (isStatic) sb.Append("static ");
                    sb.Append(functionReturnType + " ");
                }

                sb.Append(name + "(");
                if (functionParams != null) {
                    sb.Append(String.Join(',', functionParams.Select(x => x.type + " " + x.name)));
                }
                sb.AppendLine(")");
                sb.AppendLine("{");
                if (!isConstructor) {
                    sb.Append("throw new NotImplementedException();");
                } else if (functionParams != null) {
                    foreach (var p in functionParams) {
                        var matching = classFields
                            .FirstOrDefault(x => x.type.Equals(p.type) && x.name.ToLower().Equals(p.name.ToLower()));

                        if (matching != null) {
                            sb.AppendLine(String.Format("this.{0} = {1};", matching.name, p.name));
                        }
                    }
                }

                sb.AppendLine("}\n");

                return sb.ToString();
            }
        }
    }
}
