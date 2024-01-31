
namespace CodeConjure
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
					args = ["./samples/person.txt"];
#else
                    throw new Exception("At least one file is required.");
#endif
				}

				Settings settings = new();
				settings.PopulateSettings();

				Parser parser;
				Builder builder;
				foreach (string s in args)
				{
					parser = new(settings);

					if (settings.Verbose)
					{
						Console.WriteLine("Generating class for '{0}'", s);
					}

					parser.Parse(s);

					builder = new();
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
