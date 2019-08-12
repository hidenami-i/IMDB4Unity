

using UnityExtensions;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Google.Apis.Sheets.v4;
using UnityExtensions.Editor;

namespace IMDB4Unity.Editor
{
	internal class TableDefinitionWindow : EditorWindow
	{
		private static readonly TableDefinitionWindow Window = null;

		[MenuItem("Tools/IMDB4Unity/TableDefinitionEditor")]
		static void ShowWindow() {
			if (Window != null) {
				Window.Close();
			}

			GetWindow<TableDefinitionWindow>(nameof(TableDefinitionWindow)).Show();
		}

		private SheetsService sheetsService;
		private SpreadSheetSettingAsset spreadSheetSettingAsset;

		private void OnEnable() {
			maxSize = new Vector2(400, 400);

			if (ExAssetDatabase.FindAllAssetByTypeName<SpreadSheetSettingAsset>().IsNullOrEmpty()) {
				SpreadSheetSettingAsset asset =
					CreateInstance(typeof(SpreadSheetSettingAsset)) as SpreadSheetSettingAsset;
				ExIO.CreateDirectoryNotExist("Assets/App/Editor/");
				AssetDatabase.CreateAsset(
					asset, Path.Combine("Assets/App/Editor/", typeof(SpreadSheetSettingAsset).Name + ".asset"));
				AssetDatabase.SaveAssets();
			}

			Initialize();
		}

		void Initialize() {
			List<SpreadSheetSettingAsset> assetList = ExAssetDatabase.FindAllAssetByTypeName<SpreadSheetSettingAsset>();
			if (assetList.IsNullOrEmpty()) {
				Debug.LogError("not found SpreadSheetSettingAsset.");
				return;
			}

			if (assetList.Count > 1) {
				Debug.LogWarning("There are multiple SpreadSheetSettingAsset.");
			}

			spreadSheetSettingAsset = assetList.First();
		}

		private void OnGUI() {

			EditorGUILayout.ObjectField(nameof(SpreadSheetSettingAsset), spreadSheetSettingAsset,
										typeof(SpreadSheetSettingAsset), false);

			ExEditorGUI.BeginDisabledGroupUnityEditorBusy();
			
			ExEditorGUI.LabelFieldAsBold("Table Definition");

			foreach (SpreadSheetInfo spreadSheetInfo in spreadSheetSettingAsset.SpreadSheetInfoList) {
				EditorGUILayout.BeginHorizontal();

				if (ExEditorGUI.Button($"Generate {spreadSheetInfo.Name} Script")) {
					try {
						spreadSheetSettingAsset.GenerateScriptAsync(spreadSheetInfo);
					}
					catch (Exception e) {
						Debug.LogError(e);
					}
					finally {
						EditorUtility.ClearProgressBar();
					}
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUI.EndDisabledGroup();
			
			ExEditorGUI.Space();
			ExEditorGUI.LabelFieldAsBold("SheetApi Get Credencial.json");
			if (ExEditorGUI.ButtonAsToolBar("--->")) {
				System.Diagnostics.Process.Start(@"https://developers.google.com/sheets/api/quickstart/dotnet");
			}
			
			// https://developers.google.com/drive/api/v3/quickstart/dotnet
			// credencial.json for google drive
		}
	}
}

#endif