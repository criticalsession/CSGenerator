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
    }
}
