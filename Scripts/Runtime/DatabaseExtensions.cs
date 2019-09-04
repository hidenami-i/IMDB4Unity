using System.Collections.Generic;
using System.IO;
using UnityExtensions;

namespace IMDB4Unity
{
	public static class DatabaseExtensions
	{
		/// <summary>
		/// file path cache.
		/// </summary>
		private static readonly Dictionary<string, string> filePathCache = new Dictionary<string, string>();

		/// <summary>
		/// password cache.
		/// </summary>
		private static readonly Dictionary<string, string> pCache = new Dictionary<string, string>();

		/// <summary>
		/// salt cache.
		/// </summary>
		private static readonly Dictionary<string, string> sCache = new Dictionary<string, string>();

		/// <summary>
		/// Gets the default file path cache.
		/// </summary>
		/// <param name="database"></param>
		/// <returns></returns>
		private static string DefaultFilePath(IDatabase database) {

			#if UNITY_EDITOR
			PathCache.Initialize();
			#endif

			#if UNITY_EDITOR

			if (!filePathCache.TryGetValue(database.KName, out string filePath)) {
				filePath = Path.Combine(DatabaseSettings.Location.RootFolderPath, "../", DatabaseSettings.Location.FolderName, database.Schema, database.KName + ".json");
				filePathCache.Add(database.KName, filePath);
			}

			#else
			if (!filePathCache.TryGetValue(database.KName, out string filePath)) {
				filePath = Path.Combine(DBSettings.Location.RootFolderPath, DBSettings.Location.Name, Encrypt.MD5ToString(database.Schema), Encrypt.MD5ToString(database.KName) + ".bytes");
				filePathCache.Add(database.KName, filePath);
			}

			#endif

			return filePath;
		}

		/// <summary>
		/// Gets the password cache.
		/// </summary>
		/// <param name="database"></param>
		/// <returns></returns>
		private static string Password(IDatabase database) {
			if (!pCache.TryGetValue(database.KName, out string p)) {
				p = PBKDF25.Encrypt(database.KName);
				pCache.Add(database.KName, p);
			}

			return p;
		}

		/// <summary>
		/// Gets the salt cache.
		/// </summary>
		/// <param name="database"></param>
		/// <returns></returns>
		private static string Salt(IDatabase database) {
			if (!sCache.TryGetValue(database.TName, out string s)) {
				s = PBKDF25.Encrypt(database.TName);
				sCache.Add(database.TName, s);
			}

			return s;
		}

		/// <summary>
		/// Default save to local.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="isDeleteContent"></param>
		public static void Save(this IDatabase database, bool shouldDeleteContent = false) {
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Save(database, DefaultFilePath(database), true, false);
			#else
			EncryptSave(database, DefaultFilePath(database), Password(database), Salt(database), shouldDeleteContent);
			#endif
		}

		/// <summary>
		/// Default load from local.
		/// </summary>
		/// <param name="database"></param>
		public static void Load(this IDatabase database) {
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Load(database, DefaultFilePath(database));
			#else
			EncryptLoad(database, DefaultFilePath(database), Password(database), Salt(database));
			#endif
		}

		/// <summary>
		/// Default Delete file contents.
		/// </summary>
		/// <param name="database"></param>
		public static void DeleteContents(this IDatabase database) {
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Save(database, DefaultFilePath(database), true, true);
			#else
			EncryptSave(database, DefaultFilePath(database), "", "", true);
			#endif
		}

		/// <summary>
		/// Save to local.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		/// <param name="isPrettyPrint"></param>
		/// <param name="isDeleteContent"></param>
		public static void Save(this IDatabase database, string filePath, bool isPrettyPrint, bool isDeleteContent) {
			ExIO.WriteAllText(filePath, isDeleteContent ? "" : database.ToJson(isPrettyPrint));
			ExUnityEditor.AssetDataBaseRefresh();
		}

		/// <summary>
		/// Load from local. and initialize database instace.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		public static void Load(this IDatabase database, string filePath) {
			if (string.IsNullOrEmpty(filePath)) {
				ExDebug.LogWarning($"File path is null or empty. Database name is {database.KName}");
				return;
			}

			database.FromJson(ExIO.ReadAllText(filePath));
		}

		/// <summary>
		/// Encrypt Save to local.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		/// <param name="isDeleteContent"></param>
		public static void EncryptSave(this IDatabase database, string filePath, bool isDeleteContent) {
			byte[] contents = Aes128.Encrypt(isDeleteContent ? "" : database.ToJson(false), Password(database), Salt(database));
			ExIO.WriteAllBytes(filePath, contents);
			ExUnityEditor.AssetDataBaseRefresh();
		}

		/// <summary>
		/// Encrypt Save to local.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		/// <param name="password"></param>
		/// <param name="salt"></param>
		/// <param name="isDeleteContent"></param>
		public static void EncryptSave(this IDatabase database, string filePath, string password, string salt, bool isDeleteContent) {
			byte[] contents = Aes128.Encrypt(isDeleteContent ? "" : database.ToJson(false), password, salt);
			ExIO.WriteAllBytes(filePath, contents);
			ExUnityEditor.AssetDataBaseRefresh();
		}

		/// <summary>
		/// Encrypt Load from local. and initialize database instance.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		public static void EncryptLoad(this IDatabase database, string filePath) {
			byte[] contents = ExIO.ReadAllBytes(filePath);
			string jsonData = System.Text.Encoding.UTF8.GetString(Aes128.Decrypt(contents, Password(database), Salt(database)));
			database.FromJson(jsonData);
		}

		/// <summary>
		/// Encrypt Load from local. and initialize database instance.
		/// </summary>
		/// <param name="database"></param>
		/// <param name="filePath"></param>
		/// <param name="isEncrypt"></param>
		/// <param name="password"></param>
		/// <param name="salt"></param>
		public static void EncryptLoad(this IDatabase database, string filePath, string password, string salt) {
			byte[] contents = ExIO.ReadAllBytes(filePath);
			string jsonData = System.Text.Encoding.UTF8.GetString(Aes128.Decrypt(contents, password, salt));
			database.FromJson(jsonData);
		}
	}
}