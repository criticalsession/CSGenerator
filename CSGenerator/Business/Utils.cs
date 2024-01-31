using System.Reflection;

namespace CSGenerator
{
	internal static class Utils
	{
		internal static string[] ReadFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new Exception("Could not find file: " + filePath);
			}

			return File.ReadAllLines(filePath);
		}

		internal static string GetValueBetweenBrackets(string line)
		{
			if (!line.Contains('(') || !line.Contains(')'))
			{
				throw new Exception("No brackets found in line but expected brackets: " + line);
			}

			int first = line.IndexOf('(');
			int last = line.LastIndexOf(')');
			return line.Substring(first + 1, last - first - 1);
		}

		internal static string GetOutDirectory()
		{
			string exePath = Assembly.GetExecutingAssembly().Location;
			string? exeDirectory = Path.GetDirectoryName(exePath);
			if (String.IsNullOrEmpty(exeDirectory))
			{
				throw new Exception("Something went wrong while getting the exe directory.");
			}
			string outPath = Path.Combine(exeDirectory, "out");

			return outPath;
		}

		internal static string GetOutPath(string fileName)
		{
			return Path.Combine(GetOutDirectory(), fileName + ".cs");
		}

		internal static void ValidateFunctionParam(string rawParam)
		{
			foreach (char c in rawParam)
			{
				if (!(c == ':' || c == '_' || c == ' ' || c == '?' || char.IsLetterOrDigit(c)))
				{
					throw new Exception("Function parameter cannot contain brackets: " + rawParam);
				}
			}
		}

		internal static string GetExeDirectory()
		{
			string exePath = Assembly.GetExecutingAssembly().Location;
			string? exeDirectory = Path.GetDirectoryName(exePath);
			if (String.IsNullOrEmpty(exeDirectory))
			{
				throw new Exception("Something went wrong while getting the exe directory.");
			}

			return exeDirectory;
		}
	}
}
