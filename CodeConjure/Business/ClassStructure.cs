using System.Reflection.Metadata;
using System.Text;
using System.Xml.Serialization;

namespace CodeConjure
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
					.. fields.Where(p => !p.isStatic && p.isPrivate && p.name.Contains('_') && !p.isProperty),
					.. fields.Where(p => !p.isStatic && p.isPrivate && !p.name.Contains('_') && !p.isProperty),
					.. fields.Where(p => !p.isStatic && !p.isPrivate && !p.isProperty),
					.. fields.Where(p => !p.isStatic && p.isProperty),
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
				if (m.isConstructor) m.fieldDetails.name = c.ClassName;
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

		internal class Base
		{
			internal string name;
			internal string comment;
			internal bool isStatic;
			internal bool isPrivate;
			internal List<ExtraData> extras;

			internal Base(Declaration dec)
			{
				name = dec.fieldDetails.name;
				comment = dec.comment;
				isStatic = dec.hasFlag(Declaration.Flag.Static);
				isPrivate = dec.hasFlag(Declaration.Flag.Private);
				extras = [];
			}

			internal Base()
			{
				name = "";
				comment = "";
				isStatic = false;
				isPrivate = false;
				extras = [];
			}
		}

		internal class FieldStructure : Base
		{
			internal string type;
			internal bool isProperty;

			internal FieldStructure(Declaration dec) : base(dec)
			{
				type = dec.fieldDetails.type;
				extras = [];
				ParseExtras(dec.extras);
			}

			internal FieldStructure(FieldDetails fd) : base()
			{
				name = fd.name;
				type = fd.type;
				isProperty = false;
			}

			private void ParseExtras(List<string> dExtras)
			{
				if (dExtras.Count == 0) return;

				if (!(dExtras[0].StartsWith("get ") || dExtras[0].StartsWith("set ")
					|| dExtras[0] == "get" || dExtras[0] == "set"))
				{
					throw new Exception("Property content block should start with 'get' or 'set': " + dExtras[0]);
				}

				isProperty = true;

				List<ExtraData> modifiedExtras = [];
				foreach (string extra in dExtras)
				{
					if (extra.Length > 4)
					{
						if (extra.StartsWith("get "))
						{
							modifiedExtras.Add(new ExtraData("get", ExtraData.DataType.Scaffolding));
							modifiedExtras.Add(new ExtraData(extra[4..], ExtraData.DataType.SingleLine));
							continue;
						}
						else if (extra.StartsWith("set "))
						{
							modifiedExtras.Add(new ExtraData("set", ExtraData.DataType.Scaffolding));
							modifiedExtras.Add(new ExtraData(extra[4..], ExtraData.DataType.SingleLine));
							continue;
						}
					}

					if (extra.Trim().Equals("get"))
					{
						modifiedExtras.Add(new ExtraData("get", ExtraData.DataType.Scaffolding));
					}
					else if (extra.Trim().Equals("set"))
					{
						modifiedExtras.Add(new ExtraData("set", ExtraData.DataType.Scaffolding));
					}
					else
					{
						modifiedExtras.Add(new ExtraData(extra, ExtraData.DataType.MultiLine));
					}
				}

				extras = modifiedExtras;
			}

			internal string Write(IReadOnlyList<FieldStructure> classFields)
			{
				StringBuilder sb = new();

				if (isProperty && !string.IsNullOrEmpty(comment))
				{
					sb.AppendLine("\r\n// " + comment);
				}

				if (isPrivate) sb.Append("private ");
				else sb.Append("public ");

				if (isProperty)
				{
					sb.AppendLine(type + " " + name + " { ");

					bool? getBlock = null;
					foreach (ExtraData extra in extras)
					{
						if (extra.type == ExtraData.DataType.Scaffolding && extra.line == "get")
						{
							if (getBlock.HasValue) sb.AppendLine("}");

							sb.AppendLine("get {");
							getBlock = true;
							continue;
						}

						if (extra.type == ExtraData.DataType.Scaffolding && extra.line == "set")
						{
							if (getBlock.HasValue) sb.AppendLine("}");

							sb.AppendLine("set {");
							getBlock = false;
							continue;
						}

						if (extra.type == ExtraData.DataType.SingleLine)
						{
							if (!getBlock.HasValue)
							{
								throw new Exception("Unexpected extra value found outside of get/set block: " + extra.line);
							}

							if (!getBlock.Value)
							{
								var matchingField = classFields.FirstOrDefault(p => p.name.Equals(extra.line,
									StringComparison.CurrentCultureIgnoreCase) && p.type.Equals(type));

								if (matchingField != null)
								{
									sb.AppendLine(extra.line + " = value;");
								}
								else
								{
									sb.AppendLine(extra.line + ";");
								}
							}
							else
							{
								sb.AppendLine("return " + extra.line + ";");
							}
						}
						else if (extra.type == ExtraData.DataType.MultiLine)
						{
							sb.AppendLine(extra.line);
						}
						else
						{
							throw new Exception("Unexpected data type in extras found: " + extra.type);
						}
					}

					sb.AppendLine("}");
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

				return sb.ToString().Replace(";;", ";");
			}
		}

		internal class MethodStructure : Base
		{
			internal bool isConstructor;
			internal List<FieldStructure> functionParams = [];
			internal string functionReturnType;

			internal MethodStructure(Declaration dec) : base(dec)
			{
				if (dec.functionDetails == null) throw new Exception("Expected function but functionDetails is null.");
				isConstructor = dec.isConstructor;
				functionReturnType =
					String.IsNullOrEmpty(dec.functionDetails.returnType) || dec.functionDetails.returnType.Equals("null", StringComparison.CurrentCultureIgnoreCase)
					? "void"
					: dec.functionDetails.returnType;

				if (dec.functionDetails.parameters != null)
				{
					foreach (var p in dec.functionDetails.parameters)
					{
						FieldStructure s = new(p);
						functionParams.Add(s);
					}
				}

				ParseExtras(dec.extras);
			}

			private void ParseExtras(List<string> dExtras)
			{
				if (dExtras.Count == 0) return;

				List<ExtraData> modifiedExtras = [];
				foreach (string extra in dExtras)
				{
					modifiedExtras.Add(new ExtraData(extra, ExtraData.DataType.MultiLine));
				}

				extras = modifiedExtras;
			}

			internal string Write(IReadOnlyList<FieldStructure> classFields)
			{
				StringBuilder sb = new();

				sb.AppendLine();

				if (!string.IsNullOrEmpty(comment))
				{
					sb.Append(WriteComment());
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
					if (extras != null && extras.Count > 0)
					{
						foreach (string e in extras.Where(p => p.type == ExtraData.DataType.MultiLine).Select(p => p.line))
						{
							sb.AppendLine(e);
						}
					}
					else
					{
						sb.AppendLine("throw new NotImplementedException();");
					}
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

					if (extras != null && extras.Count > 0)
					{
						foreach (string e in extras.Where(p => p.type == ExtraData.DataType.MultiLine).Select(p => p.line))
						{
							sb.AppendLine(e);
						}
					}
				}

				sb.AppendLine("}");

				return sb.ToString();
			}

			internal string WriteComment()
			{
				StringBuilder sb = new();

				sb.AppendLine("/// <summary>");
				sb.AppendLine("/// " + comment);
				sb.AppendLine("/// </summary>");

				foreach (var par in functionParams)
				{
					sb.AppendLine("/// <param name=\"" + par.name + "\"></param>");
				}

				if (functionReturnType != null && functionReturnType != ""
					&& functionReturnType != "void" && functionReturnType != "null")
				{
					sb.AppendLine("/// <returns></returns>");
				}

				return sb.ToString();
			}
		}

		internal class ExtraData
		{
			internal string line;
			internal DataType type;

			internal ExtraData(string line, DataType type)
			{
				this.line = line;
				this.type = type;
			}

			internal enum DataType
			{
				SingleLine,
				Scaffolding,
				MultiLine
			}
		}
	}
}
