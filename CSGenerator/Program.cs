namespace CSGenerator {
    internal class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                //Console.WriteLine("At least one file required. Press [Enter] to exit.");
                //Console.ReadLine();
                //return;

                args = [
                    @"C:\Users\amant\source\repos\CSGenerator\CSGenerator\bin\Debug\net8.0\person.txt"
                ];
            }

            Parser parser = new();
            foreach (string s in args) {
                Console.WriteLine("Generating class for '{0}'", s);
                List<Declaration> declarations = parser.Parse(s);
                foreach (Declaration d in declarations) {
                    Console.WriteLine(d.ToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
