using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSGenerator.Utils;

namespace CSGenerator {
    internal class Parser {
        public List<Declaration> Parse(string filePath) {
            var lines = readFile(filePath);

            List<Declaration> declarations = new();

            for (int i = 1; i < lines.Length; i++) {
                var line = lines[i];

                Declaration dec = new();
                dec.parseDeclaration(line);

                declarations.Add(dec);
            }

            return declarations;
        }
    }
}
