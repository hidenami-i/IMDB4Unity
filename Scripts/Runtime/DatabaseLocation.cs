namespace IMDB4Unity
{
	public static class DatabaseLocation
	{
		/// <summary>
		/// Root folder for save and load.
		/// </summary>
		public static string RootFolder {
			get {
				#if UNITY_EDITOR
				return PathCache.DataPath;
				#else
				return PathCache.PersistentDataPath;
				#endif
			}
		}

		public static string DefaultLocation {
			get {
				#if UNITY_EDITOR
				return "SaveData";
				#else
				return UnityExtensions.Encrypt.MD5ToString("SaveData");
				#endif
			}
		}
	}
}