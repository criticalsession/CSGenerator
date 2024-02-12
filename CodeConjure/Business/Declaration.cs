namespace CodeConjure
{
	internal class Declaration
	{
		internal enum Flag
		{
			Static,
			Private,
			Getter,
			Setter
		}

		internal List<Flag> flags = [];

		internal string comment;

		internal FunctionDetails? functionDetails;
		internal FieldDetails fieldDetails;

		internal List<string> extras;

		internal bool isFunction
		{
			get
			{
				return functionDetails != null;
			}
			set
			{
				if (!value) functionDetails = null;
				else functionDetails = new FunctionDetails();
			}
		}

		internal bool isProperty
		{
			get
			{
				return hasFlag(Flag.Setter) || hasFlag(Flag.Getter);
			}
		}

		internal bool isConstructor
		{
			get
			{
				return (this.functionDetails != null && this.functionDetails.isConstructor);
			}
		}

		internal bool hasFlag(Flag flag)
		{
			return flags.Contains(flag);
		}

		internal void addFlag(Flag flag)
		{
			if (!hasFlag(flag))
			{
				flags.Add(flag);
			}
		}

		internal void removeFlag(Flag flag)
		{
			if (hasFlag(flag))
			{
				flags.Remove(flag);
			}
		}

		internal Declaration()
		{
			comment = "";
			extras = [];
			flags = [];

			isFunction = false;

			fieldDetails = new FieldDetails()
			{
				name = "",
				type = ""
			};
		}

		internal void parseDeclaration(string line)
		{
			if (!line.Contains(':'))
			{
				throw new Exception("Invalid line format: " + line);
			}

			int lastSplit = line.LastIndexOf(':');
			string key = line[..lastSplit].Trim();
			string val = line[(lastSplit + 1)..].Trim();

			if (key.Contains('(') && key.Contains(')'))
			{
				functionDetails = new FunctionDetails();
				
				string functionName = key;
				if (key.StartsWith('('))
				{
					functionDetails.isConstructor = true;
					functionName = "";
				}
				else
				{
					functionName = key[..key.IndexOf('(')];
				}

				string[] rawParams = Utils.GetValueBetweenBrackets(key).Split(',')
					.Select(x => x.Trim()).ToArray();

				foreach (string rawParam in rawParams)
				{
					if (string.IsNullOrEmpty(rawParam))
					{
						continue;
					}

					Utils.ValidateFunctionParam(rawParam);

					Declaration param = new();
					param.parseDeclaration(rawParam);
					functionDetails.parameters.Add(param.fieldDetails);
				}

				key = functionName;
			}

			if (key.StartsWith('&'))
			{
				addFlag(Flag.Static);
				key = key[1..];
			}

			if (!isConstructor && (char.IsLower(key[0]) || key[0] == '_'))
			{
				addFlag(Flag.Private);
				if (isFunction)
				{
					key = key.Replace('_', ' ').Trim();
				}
			}

			if (val.Contains("//"))
			{
				comment = val[val.IndexOf("//")..].Replace("//", "").Trim();
				val = val[..val.IndexOf("//")].Trim();
			}

			fieldDetails = new FieldDetails();
			fieldDetails.name = key;
			if (isFunction && functionDetails != null) functionDetails.returnType = val;
			else fieldDetails.type = val;
		}
	}
}
