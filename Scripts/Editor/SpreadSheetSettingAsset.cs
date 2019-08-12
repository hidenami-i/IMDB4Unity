

using UnityExtensions;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using UnityEditor;
using UnityEngine;

namespace IMDB4Unity.Editor
{
	[Serializable]
	public class SpreadSheetInfo
	{
		[SerializeField] private string name = "";
		[SerializeField] private string spreadSheetId = "";

		public string Name => name;
		public string SpreadSheetId => spreadSheetId;
	}

	[Serializable]
	public class SheetInfo
	{
		[SerializeField] private string title = "";
		[SerializeField] private string spreadSheetId = "";

		public SheetInfo(Sheet sheet, string spreadSheetId) {
			title = sheet.Properties.Title;
			this.spreadSheetId = spreadSheetId;
		}

		public string Title => title;
		public string SpreadSheetId => spreadSheetId;
	}

	public class SpreadSheetSettingAsset : ScriptableObject
	{
		static readonly string[] Scopes = {SheetsService.Scope.SpreadsheetsReadonly};
		private const string ApplicationName = "IMDB for unity";

		[SerializeField] private TextAsset spreadSheetCreadencialJson = null;

		bool IsNullCredencial => spreadSheetCreadencialJson == null;

		[field: SerializeField]
		public List<SpreadSheetInfo> SpreadSheetInfoList { get; } = new List<SpreadSheetInfo>();

		public SheetsService GetSheetService() {
			if (IsNullCredencial) {
				throw new ArgumentNullException("credecial.json is null.");
			}

			string path = AssetDatabase.GetAssetPath(spreadSheetCreadencialJson);
			UserCredential credential;
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			// Create Google Sheets API service.
			return new SheetsService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential, ApplicationName = ApplicationName
			});
		}

		public void UpdateMasterData(SheetInfo sheetInfo) {

			EditorUtility.DisplayProgressBar(sheetInfo.Title, "", 0);
			ValueRange result = GetSheetService().Spreadsheets.Values.Get(sheetInfo.SpreadSheetId, sheetInfo.Title).Execute();
			IList<IList<object>> sheetValues = result.Values;

			string repositoryName = "Master" + sheetInfo.Title.Replace("[D]", "").SnakeToUpperCamel() + "Repository";

			Debug.Log($"repository name : {repositoryName}");

			Type repositoryType = repositoryName.ToType();

			Debug.Log($"repository type : {repositoryType.Name}");

			IList<string> fieldKeys = sheetValues[1].Select(x => x.ToString()).ToList();
			List<Dictionary<string, object>> values = new List<Dictionary<string, object>>();

			for (var index = 3; index < sheetValues.Count; index++) {
				IList<object> row = sheetValues[index];
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				for (var i = 0; i < fieldKeys.Count; i++) {
					string key = fieldKeys[i];
					object value = row.ElementAtOrDefault(i);
					if (string.IsNullOrEmpty(key) || value == null) {
						continue;
					}

					dictionary.Add(key, value);
				}

				values.Add(dictionary);
			}

			try {
				object repositoryInstance = Activator.CreateInstance(repositoryType);
				DatabaseReflection.InsertAll(repositoryType, repositoryInstance, values);

				string json = JsonUtility.ToJson(repositoryInstance, true);
				IDatabase database = repositoryInstance as IDatabase;
				ExIO.WriteAllText(Path.Combine(Application.dataPath, "../", DatabaseLocation.DefaultLocation, database.Schema, database.KName + ".json"), json);
			}
			catch (Exception e) {
				Debug.LogError(e.ToString());
				throw;
			}
			finally {
				ExUnityEditor.AssetDataBaseRefresh();
				EditorUtility.ClearProgressBar();
			}
		}

		public void UpdateAllMasterData(List<SheetInfo> sheetInfoList) {
			foreach (SheetInfo sheetInfo in sheetInfoList) {
				UpdateMasterData(sheetInfo);
			}
		}

		public async void GenerateScriptAsync(SpreadSheetInfo spreadSheetInfo) {

			var result = await GetSheetService().Spreadsheets.Get(spreadSheetInfo.SpreadSheetId).ExecuteAsync();

			var eList = result.Sheets.Where(x => x.Properties.Title.Contains("[E]")).ToList();

			foreach (Sheet sheet in eList) {
				string range = sheet.Properties.Title;
				var resultValues = await GetSheetService().Spreadsheets.Values.Get(spreadSheetInfo.SpreadSheetId, range).ExecuteAsync();
				EditorUtility.DisplayProgressBar(sheet.Properties.Title, "", 0);
				IList<IList<object>> sheetValues = resultValues.Values;
				string logicalName = sheetValues[1][2].ToString();
				string physicalName = sheetValues[2][2].ToString();
				List<TableDefinitionDataEntity> tableDefinitionDataEntities = ToEnumDataEntityList(sheetValues);
				TableDefinitionEntity entity = new TableDefinitionEntity(SchemaType.Enumeration.ToString(), logicalName, physicalName, "", tableDefinitionDataEntities);
				TableDefinitionRepository.Instance.Insert(entity);
			}

			GenerateEnumScript();

			var tList = result.Sheets.Where(x => x.Properties.Title.Contains("[T]")).ToList();

			foreach (Sheet sheet in tList) {
				string range = sheet.Properties.Title;
				var resultValues = await GetSheetService().Spreadsheets.Values.Get(spreadSheetInfo.SpreadSheetId, range).ExecuteAsync();
				EditorUtility.DisplayProgressBar(sheet.Properties.Title, "", 0);
				IList<IList<object>> sheetValues = resultValues.Values;
				string schema = sheetValues[0][2].ToString();
				string logicalName = sheetValues[1][2].ToString();
				string physicalName = sheetValues[2][2].ToString();
				string persistentObjectType = sheetValues[3][2].ToString();
				List<TableDefinitionDataEntity> tableDefinitionDataEntities = ToTableDefinitionDataEntityList(sheetValues);
				TableDefinitionEntity entity = new TableDefinitionEntity(schema, logicalName, physicalName, persistentObjectType, tableDefinitionDataEntities);
				TableDefinitionRepository.Instance.Insert(entity);
			}

			GenerateTableDefinitionScript();

			EditorUtility.ClearProgressBar();
		}

		void GenerateTableDefinitionScript() {
			List<TableDefinitionEntity> list = TableDefinitionRepository.Instance.FindAllBy(x => !x.SchemaType.IsEnumeration());

			for (var index = 0; index < list.Count; index++) {
				TableDefinitionEntity entity = list[index];

				float progress = (float)index / list.Count;
				EditorUtility.DisplayProgressBar("Generate TableDefinition...", entity.PhysicalName, progress);

				string entityScript = entity.GenerateEntityScript();
				string entityFilePath = Path.Combine("Assets/App/Database", entity.SchemaType.ToString(), entity.GetEntityScriptFileName);
				ExIO.WriteAllText(entityFilePath, entityScript);

				string entityServiceScript = entity.GenerateEntityServiceScript();
				string entityServiceFilePath = Path.Combine("Assets/App/Database/", entity.SchemaType.ToString(), entity.GetEntityServiceScriptFileName);
				if (!File.Exists(entityServiceFilePath)) {
					ExIO.WriteAllText(entityServiceFilePath, entityServiceScript);
				}

				string repositoryScript = "";
				string repositoryServiceScript = "";
				string fileName = "";
				string serviceFilePath = "";

				if (entity.PersistentObjectType.IsRepository()) {
					repositoryScript = entity.GenerateRepositoryScript();
					fileName = entity.GetRepositoryScriptFileName;

					repositoryServiceScript = entity.GenerateRepositoryServiceScript();
					serviceFilePath = Path.Combine("Assets/App/Database/", entity.SchemaType.ToString(), entity.GetRepositoryServiceScriptFileName);
				}
				else if (entity.PersistentObjectType.IsDataMapper()) {
					repositoryScript = entity.GenerateDataMapperScript();
					fileName = entity.GetDataMapperScriptFileName;

					repositoryServiceScript = entity.GenerateDataMapperServiceScript();
					serviceFilePath = Path.Combine("Assets/App/Database/", entity.SchemaType.ToString(), entity.GetDataMapperServiceScriptFileName);
				}

				string repositoryFilePath = Path.Combine("Assets/App/Database/", entity.SchemaType.ToString(), fileName);
				ExIO.WriteAllText(repositoryFilePath, repositoryScript);

				if (!File.Exists(serviceFilePath)) {
					ExIO.WriteAllText(serviceFilePath, repositoryServiceScript);
				}

				ExUnityEditor.AssetDataBaseRefresh();

				string[] entityLabels = {entity.SchemaType.ToString(), "Entity", "Database"};
				AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<TextAsset>(entityFilePath), entityLabels);

				string[] repositoryLabels = {entity.SchemaType.ToString(), entity.PersistentObjectType.ToString(), "Database"};
				AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<TextAsset>(repositoryFilePath), repositoryLabels);
			}
		}

		void GenerateEnumScript() {
			var list = TableDefinitionRepository.Instance.FindAllBy(x => x.SchemaType.IsEnumeration());
			for (var index = 0; index < list.Count; index++) {

				TableDefinitionEntity entity = list[index];

				float progress = (float)index / list.Count;
				EditorUtility.DisplayProgressBar("Generate Enumeration...", entity.PhysicalName, progress);

				string script = entity.GenerateEnumScript();
				string enumFilePath = Path.Combine("Assets/App/Database/Enumerations", entity.GetEnumScriptFileName);
				ExIO.WriteAllText(enumFilePath, script);
				ExUnityEditor.AssetDataBaseRefresh();

				string[] labels = {entity.SchemaType.ToString()};
				AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<TextAsset>(enumFilePath), labels);
			}
		}

		List<TableDefinitionDataEntity> ToTableDefinitionDataEntityList(IList<IList<object>> sheetValues) {
			List<TableDefinitionDataEntity> result = new List<TableDefinitionDataEntity>();
			for (int i = 7; i < sheetValues.Count; i++) {
				IList<object> row = sheetValues[i];
				string logicalName = row.ElementAtOrDefault(1)?.ToString();
				string physicalName = row.ElementAtOrDefault(2)?.ToString();
				string dataType = row.ElementAtOrDefault(3)?.ToString();
				string defaultValue = row.ElementAtOrDefault(7)?.ToString();
				string relation = row.ElementAtOrDefault(8)?.ToString();

				// undefined dataType is threw
				if (string.IsNullOrEmpty(dataType)) {
					continue;
				}

				TableDefinitionDataEntity entity = new TableDefinitionDataEntity(logicalName, physicalName, dataType, defaultValue, relation);
				result.Add(entity);
			}

			return result;
		}

		List<TableDefinitionDataEntity> ToEnumDataEntityList(IList<IList<object>> sheetValues) {
			List<TableDefinitionDataEntity> result = new List<TableDefinitionDataEntity>();
			for (int i = 7; i < sheetValues.Count; i++) {
				IList<object> row = sheetValues[i];
				string logicalName = row.ElementAtOrDefault(1)?.ToString();
				string physicalName = row.ElementAtOrDefault(2)?.ToString();
				int value = int.Parse(row.ElementAtOrDefault(3)?.ToString());
				string remarks = row.ElementAtOrDefault(4)?.ToString();

				TableDefinitionDataEntity entity = new TableDefinitionDataEntity(logicalName, physicalName, value, remarks);
				result.Add(entity);
			}

			return result;
		}
	}
}

#endif