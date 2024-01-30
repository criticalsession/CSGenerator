
namespace CSGenerator
{
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
#if DEBUG
					args = ["./samples/person.txt", "./samples/country.txt"];
#else
                    throw new Exception("At least one file is required.");
#endif
				}

				foreach (string s in args)
				{
					Parser parser = new();

					Console.WriteLine("Generating class for '{0}'", s);
					parser.Parse(s);

					Builder builder = new();
					Builder.Build(parser);
				}

				Console.WriteLine();
				Console.WriteLine("Done.");
			}
			catch (Exception e)
			{
				Console.WriteLine("An error has occurred: " + e.Message);
				Console.WriteLine("Press [Enter] to exit.");
			}

			Console.ReadLine();
		}
	}
}
