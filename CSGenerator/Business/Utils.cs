using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGenerator {
    internal static class Utils {
        internal static string[] readFile(string filePath) {
            return File.ReadAllLines(filePath);
        }

        internal static string getValueBetweenBrackets(string line) {
            if (!line.Contains("(") || !line.Contains(")")) {
                throw new Exception("No brackets found in line: " + line);
            }

            int first = line.IndexOf("(");
            int last = line.LastIndexOf(")");
            return line.Substring(first + 1, last - first - 1);
        }
    }
}
