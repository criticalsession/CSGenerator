using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSGenerator
{
	internal static class Utils
	{
		internal static string[] ReadFile(string filePath)
		{
			return File.ReadAllLines(filePath);
		}

		internal static string ReadTemplate(string templateName)
		{
			return File.ReadAllText(getTemplatePath(templateName));
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

		private static string getTemplatePath(string templateName)
		{
			string exePath = Assembly.GetExecutingAssembly().Location;
			string? exeDirectory = Path.GetDirectoryName(exePath);
			if (String.IsNullOrEmpty(exeDirectory))
			{
				throw new Exception("Something went wrong while getting the exe directory.");
			}
			string templatePath = Path.Combine(exeDirectory, "templates", "template_" + templateName + ".txt");

			if (!File.Exists(templatePath))
			{
				throw new Exception("Template '" + templateName + "' ('templates/template_" + templateName + ".txt') does not exist.");
			}

			return templatePath;
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
	}
}
