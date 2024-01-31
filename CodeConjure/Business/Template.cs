namespace CodeConjure
{
	internal static class Template
	{
		internal enum TemplateType
		{
			Class,
			Root
		}

		internal static string ReadTemplateText(string templateName, TemplateType templateType)
		{
			string dir = getTemplateDir(templateName);

			string filePath = Path.Combine(dir, templateType == TemplateType.Class ? "class" : "root") + ".txt";
			if (!File.Exists(filePath))
			{
				throw new Exception(String.Format("Template '{0}' ('{1}') does not exist.", templateName, filePath));
			}

			return File.ReadAllText(filePath);
		}

		private static string getTemplateDir(string templateName)
		{
			string exeDirectory = Utils.GetExeDirectory();

			string templateDirectory = Path.Combine(exeDirectory, "templates", templateName);
			if (!Directory.Exists(templateDirectory))
			{
				throw new Exception("Template directory '" + templateDirectory + "' does not exist.");
			}

			return templateDirectory;
		}

	}
}
