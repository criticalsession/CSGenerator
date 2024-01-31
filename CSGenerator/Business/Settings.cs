namespace CSGenerator
{
	internal class Settings
	{
		internal Dictionary<string, string> settings = [];

		internal bool Verbose
		{
			get
			{
				return !settings.ContainsKey("verbose") || bool.Parse(settings["verbose"]);
			}
		}

		internal string OutDir
		{
			get
			{
				return settings.TryGetValue("outdir", out string? outDir) ? outDir ?? "out" : "out";
			}
		}

		internal string BaseNamespace
		{
			get
			{
				if (settings.TryGetValue("basenamespace", out string? baseNamespace))
				{
					if (!String.IsNullOrEmpty(baseNamespace) && !baseNamespace.EndsWith('.'))
					{
						baseNamespace += '.';
					}
				}

				return baseNamespace ?? "";
			}
		}

		internal void AddSetting(string key, string value)
		{
			key = key.ToLower().Trim();
			value = value.ToLower().Trim();

			settings[key] = value;
		}

		internal void PopulateSettings()
		{
			string[] lines = Utils.ReadFile(Path.Combine(Utils.GetExeDirectory(), "settings.ini"));
			foreach (string line in lines)
			{
				string[] split = line.Split('=');
				if (split.Length != 2)
				{
					throw new Exception("Unexpected setting format: " + line);
				}

				AddSetting(split[0], split[1]);
			}
		}
	}
}
