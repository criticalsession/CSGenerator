using System.Text;

namespace CSGenerator {
    internal class Declaration {
        internal string name;
        internal string type;
        internal bool isStatic;
        internal bool isPrivate;
        internal List<Declaration>? functionParams;
        internal string? functionReturnType;
        internal bool isConstructor;
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
        internal Declaration() {
            name = "";
            type = "";
            isStatic = false;
            isPrivate = false;
            isConstructor = false;
            _isFunction = false;
        }

        internal void parseDeclaration(string line) {
            if (!line.Contains(":")) {
                throw new Exception("Invalid line format: " + line);
            }

            int lastSplit = line.LastIndexOf(":");
            string key = line.Substring(0, lastSplit).Replace(" ", ""); // don't use trim, need \t
            string val = line.Substring(lastSplit + 1).Trim();

            if (key.Contains("(") && key.Contains(")")) {
                this.isFunction = true;
                if (key.StartsWith("(")) {
                    this.isConstructor = true;
                    key = "";
                } else {
                    key = key.Substring(0, key.IndexOf("("));
                }

                string[] rawParams = Utils.GetValueBetweenBrackets(line).Split(',')
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

            if (key.StartsWith("&")) {
                this.isStatic = true;
                key = key.Substring(1);
            }

            if (!this.isConstructor && (char.IsLower(key[0]) || key[0] == '_')) {
                this.isPrivate = true;
                if (this.isFunction) {
                    key = key.Substring(1);
                }
            }

            this.name = key;
            if (this.isFunction) {
                this.functionReturnType = val;
            } else {
                this.type = val;
            }
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine(String.Format("Name: {0}, Type: {1}, IsStatic: {2}, IsPrivate: {3}, " +
                "IsFunction: {4}, FunctionReturnType: {5}",
                name, type, isStatic, isPrivate, isFunction, functionReturnType));
            if (isFunction) {
                foreach (Declaration d in functionParams!) {
                    sb.Append(" >> " + d.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
