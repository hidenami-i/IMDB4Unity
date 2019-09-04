namespace IMDB4Unity
{
	public static class DatabaseSettings
	{
		public static class Location
		{
			private static string rootFolderPath;
			private static string folderName;
			
			/// <summary>
			/// Root folder path for save and load.
			/// </summary>
			/// <example>
			/// Application.dataPath
			/// </example>
			public static string RootFolderPath {
				get {

					if (!string.IsNullOrEmpty(rootFolderPath)) {
						return rootFolderPath;
					}
					
					// default
					#if UNITY_EDITOR
					return PathCache.DataPath;
					#else
					return PathCache.PersistentDataPath;
					#endif
				}
				set { rootFolderPath = value; }
			}

			/// <summary>
			/// Location folder name.
			/// </summary>
			public static string FolderName {
				get {
					if (!string.IsNullOrEmpty(folderName)) {
						return folderName;
					}
					
					// default
					#if UNITY_EDITOR
					return "Database";
					#else
					return UnityExtensions.Encrypt.MD5ToString("Database");
					#endif
				}
				set { folderName = value; }
			}
		}
	}
}