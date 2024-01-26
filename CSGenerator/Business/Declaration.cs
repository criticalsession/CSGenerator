using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGenerator {
    internal class Declaration {
        internal string name;
        internal string type;
        internal bool isStatic;
        internal bool isPrivate;
        internal bool isNullable;
        internal int indent;
        private bool _isFunction;
        internal bool isFunction {
            get {
                return _isFunction;
            }
            set {
                _isFunction = value;

                if (value) {
                    functionParams = new List<Declaration>();
                    functionReturnType = "null";
                } else {
                    functionParams = null;
                    functionReturnType = null;
                }
            }
        }

        internal List<Declaration>? functionParams;
        internal string? functionReturnType;

        internal Declaration() {
            name = "";
            type = "";
            indent = 0;
            isStatic = false;
            isPrivate = false;
            isNullable = false;
            _isFunction = false;
        }

        internal bool parseDeclaration(string line) {
            if (!line.Contains(":")) {
                throw new Exception("Invalid line format: " + line);
            }

            bool expectClassDefinition = false;

            int lastSplit = line.LastIndexOf(":");
            string key = line.Substring(0, lastSplit).Trim();
            string val = line.Substring(lastSplit + 1).Trim();

            if (val.Contains("[")) {
                val = val.Replace("[", "").Replace("]", "").Trim();
                expectClassDefinition = true;
            }

            if (key.Contains("(") && key.Contains(")")) {
                this.isFunction = true;
                key = key.Substring(0, key.IndexOf("("));

                string[] rawParams = Utils.getValueBetweenBrackets(line).Split(',')
                    .Select(x => x.Trim()).ToArray();

                foreach (string rawParam in rawParams) {
                    if (string.IsNullOrEmpty(rawParam)) {
                        continue;
                    }

                    if (rawParam.Contains("(")) {
                        throw new Exception("Function parameter cannot contain brackets: " + rawParam);
                    }

                    Declaration param = new();
                    param.parseDeclaration(rawParam);
                    this.functionParams?.Add(param);
                }
            }

            if (key.StartsWith("\t")) {
                for (int i = 0; i < key.Length; i++) {
                    if (key[i] == '\t') {
                        this.indent++;
                    } else {
                        break;
                    }
                }

                key = key.Replace("\t", "");
            }

            if (key.StartsWith("_")) {
                this.isStatic = true;
                key = key.Substring(1);
            }

            if (key.EndsWith("?")) {
                this.isNullable = true;
                key = key.Substring(0, key.Length - 1);
            }

            if (char.IsLower(key[0])) {
                this.isPrivate = true;
            }

            this.name = key;
            if (this.isFunction) {
                this.functionReturnType = val;
            } else {
                this.type = val;
            }

            return expectClassDefinition;
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine(String.Format("Name: {0}, Type: {1}, IsStatic: {2}, IsPrivate: {3}, " +
                "IsNullable: {4}, IsFunction: {5}, FunctionReturnType: {6}, Indent: {7}",
                name, type, isStatic, isPrivate, isNullable, isFunction, functionReturnType, indent));
            if (isFunction) {
                foreach (Declaration d in functionParams!) {
                    sb.Append(" >> " + d.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
