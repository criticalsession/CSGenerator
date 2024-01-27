using static CSGenerator.Utils;

namespace CSGenerator {
    internal class Parser {
        internal string rootClassName = "";
        internal string templateString = "";
        internal List<Declaration> declarations = new();

        internal List<Declaration> Parse(string filePath) {
            var lines = readFile(filePath)
                .Where(p => !string.IsNullOrEmpty(p) && !p.Trim().StartsWith("//"))
                .ToArray();

            string classDeclaration = lines[0].Replace("[", "").Replace("]", "").Trim();
            if (!classDeclaration.Contains(":")) {
                throw new Exception("No template declared");
            }

            string[] titleDec = classDeclaration.Split(':');
            rootClassName = titleDec[0].Trim();
            templateString = readTemplate(titleDec[1].Trim());

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
