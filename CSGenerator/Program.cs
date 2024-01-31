
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

				Settings settings = new();
				settings.PopulateSettings();

				foreach (string s in args)
				{
					Parser parser = new(settings);

					if (settings.Verbose)
					{
						Console.WriteLine("Generating class for '{0}'", s);
					}

					parser.Parse(s);

					Builder builder = new();
					Builder.Build(parser);
				}

				if (settings.Verbose)
				{
					Console.WriteLine();
					Console.WriteLine("Done.");

					Console.ReadLine();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("An error has occurred: " + e.Message);
#if DEBUG
				Console.WriteLine(e.StackTrace);
#endif
				Console.WriteLine("Press [Enter] to exit.");

				Console.ReadLine();
			}
		}
	}
}
