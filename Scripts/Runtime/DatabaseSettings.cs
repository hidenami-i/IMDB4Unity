namespace IMDB4Unity
{
	public static class DatabaseSettings
	{
		public static class Location
		{
			private static string rootFolderPath;
			private static string name;
			
			/// <summary>
			/// Root folder path for save and load.
			/// </summary>
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
			public static string Name {
				get {
					if (!string.IsNullOrEmpty(name)) {
						return name;
					}
					
					// default
					#if UNITY_EDITOR
					return "DB";
					#else
					return UnityExtensions.Encrypt.MD5ToString("DB");
					#endif
				}
				set { name = value; }
			}
		}
	}
}