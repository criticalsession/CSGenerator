using System.Text;

namespace CSGenerator
{
	internal class ClassStructure
	{
		internal string ClassName = "";
		private readonly List<FieldStructure> fields = [];
		private readonly List<MethodStructure> methods = [];
		internal List<ClassStructure> subClasses = [];

		internal IReadOnlyList<FieldStructure> Fields
		{
			get
			{
				List<FieldStructure> fList = [
					.. fields.Where(p => !p.isStatic && p.isPrivate && p.name.Contains('_') && !p.IsProperty),
					.. fields.Where(p => !p.isStatic && p.isPrivate && !p.name.Contains('_') && !p.IsProperty),
					.. fields.Where(p => !p.isStatic && !p.isPrivate && !p.IsProperty),
					.. fields.Where(p => !p.isStatic && p.IsProperty),
				];

				return fList;
			}
		}

		internal IReadOnlyList<MethodStructure> Methods
		{
			get
			{
				List<MethodStructure> mList =
				[
					.. methods.Where(p => p.isConstructor),
					.. methods.Where(p => !p.isConstructor && !p.isStatic && p.isPrivate),
					.. methods.Where(p => !p.isConstructor && !p.isStatic && !p.isPrivate),
					.. methods.Where(p => !p.isConstructor && p.isStatic),
				];

				return mList;
			}
		}

		internal ClassStructure() { }

		internal static ClassStructure BuildStructure(string className, Dictionary<string, List<Declaration>> allDecs)
		{
			ClassStructure c = new();

			List<Declaration> decs = allDecs[className];

			c.ClassName = className.Split('.').Last();
			foreach (var f in decs.Where(p => !p.isFunction))
			{
				c.fields.Add(new FieldStructure(f));
			}

			foreach (var m in decs.Where(p => p.isFunction))
			{
				if (m.isConstructor) m.name = c.ClassName;
				c.methods.Add(new MethodStructure(m));
			}

			c.subClasses = [];
			foreach (var subDecs in allDecs.Where(p => p.Key.StartsWith(className + ".")))
			{
				// only create subclasses for declarations one level down from current class
				string def = subDecs.Key.Replace(className + ".", "");
				if (def.Contains('.')) continue;

				c.subClasses.Add(BuildStructure(subDecs.Key, allDecs));
			}

			return c;
		}

		internal class FieldStructure
		{
			internal string name;
			internal string type;
			internal string comment;
			internal bool isStatic;
			internal bool isPrivate;
			internal bool isSetter;
			internal bool isGetter;

			internal bool IsProperty
			{
				get
				{
					return isSetter || isGetter;
				}
			}

			internal FieldStructure(Declaration dec)
			{
				name = dec.name;
				type = dec.type;
				comment = dec.comment;
				isStatic = dec.isStatic;
				isPrivate = dec.isPrivate;
				isGetter = dec.isGetter;
				isSetter = dec.isSetter;
			}

			internal string Write(IReadOnlyList<FieldStructure> classFields)
			{
				StringBuilder sb = new();

				if (IsProperty && !string.IsNullOrEmpty(comment))
				{
					sb.AppendLine("\r\n// " + comment);
				}

				if (isPrivate) sb.Append("private ");
				else sb.Append("public ");

				if (IsProperty)
				{
					var matchingField = classFields.FirstOrDefault(x => x.name.ToLower().Replace("_", "").Equals(type.ToLower()) ||
						x.name.ToLower().Equals(type.ToLower()));
					if (matchingField != null)
					{
						type = matchingField.type;
					}

					sb.AppendLine(type + " " + name + " { ");

					if (isGetter)
					{
						sb.AppendLine("get {");

						if (matchingField != null)
						{
							sb.AppendLine("return " + matchingField.name + ";");
						}
						else
						{
							sb.AppendLine("throw new NotImplementedException();");
						}

						sb.AppendLine("}");
					}

					if (isSetter)
					{
						sb.AppendLine("set {");

						if (matchingField != null)
						{
							sb.AppendLine(matchingField.name + " = value;");
						}
						else
						{
							sb.AppendLine("throw new NotImplementedException();");
						}

						sb.AppendLine("}");
					}

					sb.AppendLine("}");
				}
				else
				{
					if (isStatic) sb.Append("static ");

					sb.Append(type + " ");
					sb.Append(name + ";");

					if (!string.IsNullOrEmpty(comment))
					{
						sb.Append(" // " + comment);
					}

					sb.Append("\r\n");
				}

				return sb.ToString();
			}
		}

		internal class MethodStructure
		{
			internal string name;
			internal string comment;
			internal bool isStatic;
			internal bool isPrivate;
			internal bool isConstructor;
			internal List<FieldStructure> functionParams = [];
			internal string functionReturnType;

			internal MethodStructure(Declaration dec)
			{
				name = dec.name;
				comment = dec.comment;
				isStatic = dec.isStatic;
				isPrivate = dec.isPrivate;
				isConstructor = dec.isConstructor;
				functionReturnType =
					String.IsNullOrEmpty(dec.functionReturnType) || dec.functionReturnType.Equals("null", StringComparison.CurrentCultureIgnoreCase)
					? "void"
					: dec.functionReturnType;

				if (dec.functionParams != null)
				{
					foreach (var p in dec.functionParams)
					{
						FieldStructure s = new(p);
						functionParams.Add(s);
					}
				}
			}

			internal string Write(IReadOnlyList<FieldStructure> classFields)
			{
				StringBuilder sb = new();

				sb.AppendLine();

				if (!string.IsNullOrEmpty(comment))
				{
					sb.AppendLine("// " + comment);
				}

				if (isPrivate) sb.Append("private ");
				else sb.Append("public ");

				if (!isConstructor)
				{
					if (isStatic) sb.Append("static ");
					sb.Append(functionReturnType + " ");
				}

				sb.Append(name + "(");
				if (functionParams != null)
				{
					sb.Append(String.Join(',', functionParams.Select(x => x.type + " " + x.name)));
				}
				sb.AppendLine(")");
				sb.AppendLine("{");

				if (!isConstructor)
				{
					sb.Append("throw new NotImplementedException();");
				}
				else if (functionParams != null)
				{
					foreach (var fParam in functionParams)
					{
						var matching = classFields
							.FirstOrDefault(x => x.type.Equals(fParam.type) &&
								(x.name.ToLower().Equals(fParam.name.ToLower().Replace("_", "")) ||
								x.name.ToLower().Equals(fParam.name.ToLower())));

						if (matching != null)
						{
							sb.AppendLine(String.Format("this.{0} = {1};", matching.name, fParam.name));
						}
					}
				}

				sb.AppendLine("}");

				return sb.ToString();
			}
		}
	}
}
