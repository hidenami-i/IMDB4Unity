

using UnityExtensions;
using UnityExtensions.Editor;
#if UNITY_EDITOR
using System;
using System.Text;
using UnityEngine;

namespace IMDB4Unity.Editor
{
	[Serializable]
	public class TableDefinitionDataEntity
	{
		[SerializeField] private int index = 0;
		[SerializeField] private string logicalName = "";
		[SerializeField] private string physicalName = "";
		[SerializeField] private string dataType = "";
		[SerializeField] private bool unsigned = false;
		[SerializeField] private string defaultValue = "";
		[SerializeField] private string relation = "";

		/// <summary>
		/// Constructor for [T] sheet
		/// </summary>
		/// <param name="logicalName"></param>
		/// <param name="physicalName"></param>
		/// <param name="dataType"></param>
		/// <param name="defaultValue"></param>
		/// <param name="relation"></param>
		public TableDefinitionDataEntity(string logicalName, string physicalName, string dataType, string defaultValue, string relation) {
			this.logicalName = logicalName;
			this.physicalName = physicalName;
			this.dataType = dataType;
			this.defaultValue = defaultValue;
			this.relation = relation;
		}

		/// <summary>
		/// Constructor for enum.
		/// </summary>
		/// <param name="logicalName"></param>
		/// <param name="physicalName"></param>
		/// <param name="value"></param>
		/// <param name="remarks"></param>
		public TableDefinitionDataEntity(string logicalName, string physicalName, int value, string remarks) {
			this.logicalName = logicalName;
			this.physicalName = physicalName;
			this.value = value;
			this.remarks = remarks;
		}

		#region Enumeration

		// Enum
		[SerializeField] private int value = 0;
		[SerializeField] private string remarks = "";

		public int Value => value;
		public string Remarks => remarks;

		public string GenerateEnumValueScript() {

			StringBuilder builder = new StringBuilder();
			builder.SetSummaryComment($"[{logicalName}]", 2);
			builder.AppendLine();
			builder.Indent2().AppendLine($"{physicalName.SnakeToUpperCamel()} = {value},");
			return builder.ToString();
		}

		public string GenerateBooleanScript(string enumPhysicalName) {
			StringBuilder builder = new StringBuilder();
			builder.SetSummaryComment($"Returns true if the {enumPhysicalName} is {physicalName.SnakeToUpperCamel()}.", 2);
			builder.AppendLine();
			builder.Indent2().Append($"public static bool Is{physicalName.SnakeToUpperCamel()}(this {enumPhysicalName.SnakeToUpperCamel()} {enumPhysicalName.SnakeToLowerCamel()})").AppendLine(" {");
			builder.Indent3().AppendLine($"return {enumPhysicalName.SnakeToLowerCamel()} == {enumPhysicalName.SnakeToUpperCamel()}.{physicalName.SnakeToUpperCamel()};");
			builder.Indent2().AppendLine("}");
			return builder.ToString();
		}

		#endregion

		public SchemaType GetSchemaType() {
			if (!relation.IsNullOrEmpty()) {
				if (relation.Contains("[E]")) {
					return SchemaType.Enumeration;
				}

				return SchemaType.Master;
			}

			return SchemaType.None;
		}

		public int Id => index;
		public string LogicalName => logicalName;
		public string PhysicalName => physicalName;
		public DataType DataType => Utility.ConvertToCSharpTypeName(dataType, unsigned);
		public bool Unsigned => unsigned;
		public string DefaultValue => defaultValue;
		public string Relation => relation;

		public string EnumType => relation.Replace("[E]", "").SnakeToUpperCamel();

		public override string ToString() {
			return $"{nameof(index)}: {index}, {nameof(logicalName)}: {logicalName}, {nameof(physicalName)}: {physicalName}, {nameof(DataType)}: {DataType}, {nameof(unsigned)}: {unsigned}, {nameof(defaultValue)}: {defaultValue}, {nameof(relation)}: {relation}";
		}

		#region generate script

		/// <summary>
		/// Get script of serializeField.
		/// </summary>
		public string GenerateSerializeFieldScript {
			get {
				if (GetSchemaType().IsEnumeration()) {
					return $"[SerializeField] private {EnumType} {physicalName.SnakeToLowerCamel()};";
				}

				DataType typeName = DataType.IsDateTime() ? DataType.String : DataType;
				return $"[SerializeField] private {typeName.ToCsharpName()} {physicalName.SnakeToLowerCamel()};";
			}
		}

		/// <summary>
		/// Get script of physicalName getter.
		/// </summary>
		public string GeneratePhysicalNameGetterScript {
			get {
				if (DataType.IsDateTime()) {
					return GenerateDateTimeCacheFieldScript;
				}

				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.SetSummaryComment($"Get {logicalName}.", 2);
				stringBuilder.AppendLine();
				if (GetSchemaType().IsEnumeration()) {
					stringBuilder.Indent2().AppendLine($"public {EnumType} {physicalName.SnakeToUpperCamel()} => {physicalName.SnakeToLowerCamel()};");
				}
				else {
					stringBuilder.Indent2().AppendLine($"public {DataType.ToCsharpName()} {physicalName.SnakeToUpperCamel()} => {physicalName.SnakeToLowerCamel()};");
				}

				return stringBuilder.ToString();
			}
		}

		/// <summary>
		/// generate setter script.
		/// </summary>
		public string GenerateSetterScript {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.SetSummaryComment($"Set {logicalName}.", 2);
				stringBuilder.AppendLine();
				if (GetSchemaType().IsEnumeration()) {
					stringBuilder.Indent2().Append($"public void Set{physicalName.SnakeToUpperCamel()}({EnumType} {physicalName.SnakeToLowerCamel()})");
					stringBuilder.AppendLine("{");
					stringBuilder.Indent3().AppendLine($"this.{physicalName.SnakeToLowerCamel()} = {physicalName.SnakeToLowerCamel()};");
					stringBuilder.Indent2().AppendLine("}");
				}
				else if (DataType.IsDateTime()) {
					stringBuilder.Indent2().Append($"public void Set{physicalName.SnakeToUpperCamel()}(DateTime? {physicalName.SnakeToLowerCamel()})");
					stringBuilder.AppendLine("{");
					stringBuilder.Indent3().AppendLine($"_{physicalName.SnakeToLowerCamel()} = {physicalName.SnakeToLowerCamel()};");
					stringBuilder.Indent3().AppendLine($"this.{physicalName.SnakeToLowerCamel()} = _{physicalName.SnakeToLowerCamel()} == null ? null : {physicalName.SnakeToLowerCamel()}.ToString();");
					stringBuilder.Indent2().AppendLine("}");
				}
				else {
					stringBuilder.Indent2().Append($"public void Set{physicalName.SnakeToUpperCamel()}({DataType.ToCsharpName()} {physicalName.SnakeToLowerCamel()})");
					stringBuilder.AppendLine("{");
					stringBuilder.Indent3().AppendLine($"this.{physicalName.SnakeToLowerCamel()} = {physicalName.SnakeToLowerCamel()};");
					stringBuilder.Indent2().AppendLine("}");
				}

				return stringBuilder.ToString();
			}
		}

		/// <summary>
		/// Get script of constructor argument.
		/// </summary>
		public string GenerateConstructorArgumentScript {
			get {
				if (GetSchemaType().IsEnumeration()) {
					return $"{EnumType} {PhysicalName.SnakeToLowerCamel()}";
				}

				if (DataType.IsDateTime()) {
					return $"{DataType.ToCsharpName()}? {PhysicalName.SnakeToLowerCamel()}";
				}

				return $"{DataType.ToCsharpName()} {PhysicalName.SnakeToLowerCamel()}";
			}
		}

		/// <summary>
		/// Get script of constructor initialize argument.
		/// </summary>
		public string GenerateConstructorInitializeArgumentScript {
			get {
				if (DataType.IsDateTime()) {
					return string.Format("this.{0} = {0}.ToString();", PhysicalName.SnakeToLowerCamel());
				}

				return string.Format("this.{0} = {0};", PhysicalName.SnakeToLowerCamel());
			}
		}

		public string GenerateLogFieldScript {
			get {
				StringBuilder builder = new StringBuilder();
				builder.Append("builder.AppendLine($\"[{");
				builder.Append($"nameof({PhysicalName.SnakeToLowerCamel()})");
				builder.Append("}] {");
				builder.Append($"{PhysicalName.SnakeToLowerCamel()}");
				builder.Append("}\");");
				return builder.ToString();
			}
		}

		public string GenerateFindFunctionScript(string className) {
			StringBuilder builder = new StringBuilder();

			string fieldType = DataType.ToCsharpName();
			if (GetSchemaType().IsEnumeration()) {
				fieldType = EnumType;
			}

			builder.SetSummaryComment($"Gets the {logicalName}.", 2);
			builder.AppendLine();
			builder.Indent2().AppendLine($"/// <param name=\"{physicalName.SnakeToLowerCamel()}\"></param>");
			builder.Indent2().AppendLine($"/// <param name=\"entity\">{className}Entity</param>");
			builder.Indent2().AppendLine("/// <returns>If Entity is not null it returns true.</returns>");

			if (DataType.IsDateTime()) {
				builder.Indent2().Append($"public bool TryFindBy{physicalName.SnakeToUpperCamel()}({fieldType}? {physicalName.SnakeToLowerCamel()}, out {className}Entity entity)");
			}
			else {
				builder.Indent2().Append($"public bool TryFindBy{physicalName.SnakeToUpperCamel()}({fieldType} {physicalName.SnakeToLowerCamel()}, out {className}Entity entity)");
			}

			builder.AppendLine(" {");
			builder.Indent3().AppendLine($"return TryFindBy(x => x.{physicalName.SnakeToUpperCamel()} == {physicalName.SnakeToLowerCamel()}, out entity);");
			builder.Indent2().AppendLine("}");
			return builder.ToString();
		}

		public string GenerateGetOrDefaultFunctionScript(string className) {

			StringBuilder builder = new StringBuilder();

			string fieldType = DataType.ToCsharpName();
			if (GetSchemaType().IsEnumeration()) {
				fieldType = EnumType;
			}

			builder.SetSummaryComment($"Gets the {logicalName}. if the name is null or empty. it return default entity.", 2);
			builder.AppendLine();
			builder.Indent2().AppendLine($"/// <param name=\"{physicalName.SnakeToLowerCamel()}\"></param>");
			builder.Indent2().AppendLine($"/// <param name=\"entity\">{className}Entity</param>");
			builder.Indent2().AppendLine("/// <returns>If Entity is not null it returns default entity.</returns>");

			if (DataType.IsDateTime()) {
				builder.Indent2().Append($"public {className}Entity GetBy{physicalName.SnakeToUpperCamel()}OrDefault({fieldType}? {physicalName.SnakeToLowerCamel()}, {className}Entity defaultEntity)");
			}
			else {
				builder.Indent2().Append($"public {className}Entity GetBy{physicalName.SnakeToUpperCamel()}OrDefault({fieldType} {physicalName.SnakeToLowerCamel()}, {className}Entity defaultEntity)");
			}

			builder.AppendLine(" {");
			builder.Indent3().AppendLine($"return GetByOrDefault(x => x.{physicalName.SnakeToUpperCamel()} == {physicalName.SnakeToLowerCamel()}, defaultEntity);");
			builder.Indent2().AppendLine("}");
			return builder.ToString();
		}

		public string GenerateFindAllFunctionScript(string className) {
			StringBuilder builder = new StringBuilder();

			string fieldType = DataType.ToCsharpName();
			if (GetSchemaType().IsEnumeration()) {
				fieldType = EnumType;
			}

			builder.SetSummaryComment($"Get all the {logicalName}.", 2);
			builder.AppendLine();
			builder.Indent2().AppendLine($"/// <param name=\"{physicalName.SnakeToLowerCamel()}\"></param>");
			builder.Indent2().AppendLine($"/// <returns>List<{className}Entity></returns>");

			if (DataType.IsDateTime()) {
				builder.Indent2().Append($"public List<{className}Entity> FindAllBy{physicalName.SnakeToUpperCamel()}({fieldType}? {physicalName.SnakeToLowerCamel()})");
			}
			else {
				builder.Indent2().Append($"public List<{className}Entity> FindAllBy{physicalName.SnakeToUpperCamel()}({fieldType} {physicalName.SnakeToLowerCamel()})");
			}

			builder.AppendLine(" {");
			builder.Indent3().AppendLine($"return FindAllBy(x => x.{physicalName.SnakeToUpperCamel()} == {physicalName.SnakeToLowerCamel()});");
			builder.Indent2().AppendLine("}");
			return builder.ToString();
		}

		/// <summary>
		/// get datetime cache field.
		/// </summary>
		private string GenerateDateTimeCacheFieldScript {
			get {
				StringBuilder builder = new StringBuilder();
				builder.SetSummaryComment($"Get {logicalName}.", 2);
				builder.AppendLine();
				builder.Indent2().AppendLine($"public DateTime? {physicalName.SnakeToUpperCamel()} ");
				builder.Indent2().AppendLine("{");
				builder.Indent3().AppendLine("get");
				builder.Indent3().AppendLine("{");

				builder.Indent4().AppendLine($"if (!_{PhysicalName.SnakeToLowerCamel()}.HasValue) {{");
				builder.Indent5().AppendLine($"if (DateTime.TryParse({PhysicalName.SnakeToLowerCamel()}, out DateTime date)) {{");
				builder.Indent6().AppendLine($"_{PhysicalName.SnakeToLowerCamel()} = date;");
				builder.Indent5().AppendLine("}");
				builder.Indent4().AppendLine("}");
				builder.Indent4().AppendLine($"return _{PhysicalName.SnakeToLowerCamel()};");
				builder.Indent3().AppendLine("}");
				builder.Indent3().AppendLine($"set => _{PhysicalName.SnakeToLowerCamel()} = value;");
				builder.Indent2().AppendLine("}");
				builder.AppendLine();
				builder.Indent2().AppendLine($"private DateTime? _{PhysicalName.SnakeToLowerCamel()};");
				return builder.ToString();
			}
		}

		#endregion
	}
}

#endif