

using UnityExtensions;
using UnityExtensions.Editor;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IMDB4Unity.Editor
{
	[Serializable]
	public class TableDefinitionEntity : EntityBase
	{
		[SerializeField] private string schema = "";
		[SerializeField] private string logicalName = "";
		[SerializeField] private string physicalName = "";
		[SerializeField] private string persistentObjectType = "";
		[SerializeField] private List<TableDefinitionDataEntity> data = null;

		public TableDefinitionEntity() { }

		public TableDefinitionEntity(string schema, string logicalName, string physicalName, string persistentObjectType, List<TableDefinitionDataEntity> data) {
			this.schema = schema;
			this.logicalName = logicalName;
			this.physicalName = physicalName;
			this.persistentObjectType = persistentObjectType;
			this.data = data;
		}

		public SchemaType SchemaType => ExEnum.StringToEnum<SchemaType>(schema);
		public string LogicalName => logicalName;
		public string PhysicalName => physicalName;

		public PersistentObjectType PersistentObjectType =>
			ExEnum.StringToEnum<PersistentObjectType>(persistentObjectType);

		public List<TableDefinitionDataEntity> Data => data;

		string GetTableLogicalNameScript {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Indent1().AppendLine("/// <summary>");
				stringBuilder.Indent1().AppendLine("/// This class is auto-generated do not modify.");
				stringBuilder.Indent1().AppendLine($"/// Table logical name is [{logicalName}]");
				stringBuilder.Indent1().AppendLine("/// </summary>");
				return stringBuilder.ToString();
			}
		}

		string GetClassName => SchemaType + PhysicalName.SnakeToUpperCamel();

		#region script generate

		public string GetEntityScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "Entity.cs";

		public string GetEntityServiceScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "EntityService.cs";

		public string GetRepositoryScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "Repository.cs";

		public string GetRepositoryServiceScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "RepositoryService.cs";

		public string GetDataMapperScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "DataMapper.cs";

		public string GetDataMapperServiceScriptFileName =>
			SchemaType.ToString().SnakeToUpperCamel() + PhysicalName.SnakeToUpperCamel() + "DataMapperService.cs";

		public string GetEnumScriptFileName => PhysicalName.SnakeToUpperCamel() + ".cs";

		/// <summary>
		/// Generate Entity Classes.
		/// </summary>
		/// <returns>Entity Script.</returns>
		public string GenerateEntityScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetUsing("System", "System.Text", "IMDB4Unity", "UnityEngine");
			stringBuilder.AppendLine();
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine(GetTableLogicalNameScript);
			stringBuilder.Indent1().AppendLine("[Serializable]");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}Entity : EntityBase");
			stringBuilder.Indent1().AppendLine("{");

			// Field list
			foreach (TableDefinitionDataEntity dataStructureEntity in data) {
				stringBuilder.Indent2().AppendLine(dataStructureEntity.GenerateSerializeFieldScript);
			}

			stringBuilder.AppendLine();

			// Getter list
			foreach (TableDefinitionDataEntity dataStructureEntity in data) {
				stringBuilder.AppendLine(dataStructureEntity.GeneratePhysicalNameGetterScript);
			}

			// Setter list
			foreach (TableDefinitionDataEntity dataStructureEntity in data) {
				stringBuilder.AppendLine(dataStructureEntity.GenerateSetterScript);
			}

			// default constructor
			stringBuilder.Indent2().Append($"public {GetClassName}Entity()").AppendLine(" {}");
			stringBuilder.Indent2().AppendLine($"public {GetClassName}Entity(");

			// Constructor argument list
			string argument = string.Join(",\n", data.Select(x => "\t\t\t" + x.GenerateConstructorArgumentScript));
			stringBuilder.AppendLine(argument);

			stringBuilder.Indent2().AppendLine(") {");

			// Constructor initialize argument list
			foreach (TableDefinitionDataEntity dataStructureEntity in data) {
				stringBuilder.Indent3().AppendLine(dataStructureEntity.GenerateConstructorInitializeArgumentScript);
			}

			stringBuilder.Indent2().AppendLine("}");

			stringBuilder.AppendLine();

			stringBuilder.Indent2().AppendLine("public override string ToString() {");
			stringBuilder.Indent3().AppendLine("StringBuilder builder = new StringBuilder();");
			stringBuilder.Indent3().Append("builder.AppendLine().AppendLine($\"<b>ClassName [{nameof(");
			stringBuilder.Append($"{GetClassName}" + "Entity");
			stringBuilder.Append(")}]</b>\");");
			stringBuilder.AppendLine();

			// log field list
			foreach (TableDefinitionDataEntity dataStructureEntity in data) {
				stringBuilder.Indent3().AppendLine(dataStructureEntity.GenerateLogFieldScript);
			}

			stringBuilder.Indent3().AppendLine("return builder.ToString();");

			stringBuilder.Indent2().AppendLine("}");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GenerateEntityServiceScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}Entity");
			stringBuilder.Indent1().AppendLine("{");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Generate Repository Classes.
		/// </summary>
		/// <returns>Repository Script.</returns>
		public string GenerateRepositoryScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetUsing("System", "System.Collections.Generic", "IMDB4Unity", "UnityEngine");
			stringBuilder.AppendLine();
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine(GetTableLogicalNameScript);
			stringBuilder.Indent1().AppendLine("[Serializable]");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}Repository : RepositoryBase<{GetClassName}Entity, {GetClassName}Repository>");
			stringBuilder.Indent1().AppendLine("{");

			stringBuilder.SetSummaryComment("This field is generated automatically, so it can not be edited.", 2);
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"[SerializeField] private List<{GetClassName}Entity> {GetClassName.SnakeToLowerCamel()} = new List<{GetClassName}Entity>();");

			stringBuilder.AppendLine();

			stringBuilder.SetSummaryComment("This getter is generated automatically, so it can not be edited.", 2);
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"protected override List<{GetClassName}Entity> EntityList => {GetClassName.SnakeToLowerCamel()};");

			stringBuilder.AppendLine();

			stringBuilder.SetSummaryComment("This getter is generated automatically, so it can not be edited.", 2);
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"public override string Schema => \"{schema}\";");

			stringBuilder.AppendLine();

			// find function
			foreach (TableDefinitionDataEntity tableDefinitionEntity in data) {
				stringBuilder.AppendLine(tableDefinitionEntity.GenerateFindFunctionScript(GetClassName));
			}

			// find if null or default function
			foreach (TableDefinitionDataEntity tableDefinitionEntity in data) {
				stringBuilder.AppendLine(tableDefinitionEntity.GenerateGetOrDefaultFunctionScript(GetClassName));
			}

			// find all function
			foreach (TableDefinitionDataEntity tableDefinitionEntity in data) {
				stringBuilder.AppendLine(tableDefinitionEntity.GenerateFindAllFunctionScript(GetClassName));
			}

			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GenerateRepositoryServiceScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}Repository");
			stringBuilder.Indent1().AppendLine("{");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GenerateDataMapperScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetUsing("System", "IMDB4Unity", "UnityEngine");
			stringBuilder.AppendLine();
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine(GetTableLogicalNameScript);
			stringBuilder.Indent1().AppendLine("[Serializable]");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}DataMapper : DataMapperBase<{GetClassName}Entity, {GetClassName}DataMapper>");
			stringBuilder.Indent1().AppendLine("{");

			stringBuilder.SetSummaryComment("This field is generated automatically, so it can not be edited.", 2);
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"[SerializeField] private {GetClassName}Entity {GetClassName.SnakeToLowerCamel()} = null;");

			stringBuilder.AppendLine();

			stringBuilder.SetSummaryComment("This getter is generated automatically, so it can not be edited.", 2);
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"protected override {GetClassName}Entity Entity {{ get => {GetClassName.SnakeToLowerCamel()}; set => {GetClassName.SnakeToLowerCamel()} = value; }}");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GenerateDataMapperServiceScript() {

			StringBuilder stringBuilder = new StringBuilder();

			// Header
			stringBuilder.SetNameSpace("App");

			stringBuilder.AppendLine("{");
			stringBuilder.Indent1().AppendLine($"public sealed partial class {GetClassName}DataMapperService");
			stringBuilder.Indent1().AppendLine("{");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GenerateEnumScript() {

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.SetUsing("System.Collections.Generic");
			stringBuilder.AppendLine();
			stringBuilder.SetNameSpace("App");
			stringBuilder.Indent0().AppendLine("{");
			stringBuilder.SetSummaryComment($"This enum is generated automatically, so it can not be edited.\nTable logical name is {LogicalName}", 1);

			stringBuilder.AppendLine();
			stringBuilder.Indent1().AppendLine($"public enum {PhysicalName.SnakeToUpperCamel()}");
			stringBuilder.Indent1().AppendLine("{");

			foreach (TableDefinitionDataEntity entity in data) {
				stringBuilder.AppendLine(entity.GenerateEnumValueScript());
			}

			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.AppendLine();

			stringBuilder.Indent1().AppendLine($"public class {PhysicalName.SnakeToUpperCamel()}Compare : IEqualityComparer<{PhysicalName.SnakeToUpperCamel()}>");
			stringBuilder.Indent1().AppendLine("{");
			stringBuilder.Indent2().AppendLine($"public bool Equals({PhysicalName.SnakeToUpperCamel()} x, {PhysicalName.SnakeToUpperCamel()} y) " + "{");
			stringBuilder.Indent3().AppendLine("return x == y;");
			stringBuilder.Indent2().AppendLine("}");
			stringBuilder.AppendLine();
			stringBuilder.Indent2().AppendLine($"public int GetHashCode({PhysicalName.SnakeToUpperCamel()} obj) " + "{");
			stringBuilder.Indent3().AppendLine("return (int)obj;");
			stringBuilder.Indent2().AppendLine("}");
			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.AppendLine();

			stringBuilder.Indent1().AppendLine($"public static partial class {PhysicalName.SnakeToUpperCamel()}Extensions");
			stringBuilder.Indent1().AppendLine("{");

			foreach (TableDefinitionDataEntity entity in data) {
				stringBuilder.AppendLine(entity.GenerateBooleanScript(physicalName));
				stringBuilder.AppendLine();
			}

			stringBuilder.Indent1().AppendLine("}");
			stringBuilder.Indent0().AppendLine("}");

			return stringBuilder.ToString();
		}

		#endregion
	}
}

#endif