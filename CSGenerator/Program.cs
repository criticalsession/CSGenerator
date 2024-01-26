namespace CSGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("At least one file required. Press [Enter] to exit.");
                Console.ReadLine();
                return;
            }

            foreach (string s in args)
            {
                Console.WriteLine("Generating class for '{0}'", s);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
