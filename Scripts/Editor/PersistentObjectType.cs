namespace IMDB4Unity.Editor
{
	/// <summary>
	/// Data pattern type enumerations.
	/// Repository and DataMapper
	/// </summary>
	public enum PersistentObjectType
	{
		/// <summary>
		/// Repository is Multiple entity
		/// </summary>
		Repository,

		/// <summary>
		/// DataMapper is Single entity
		/// </summary>
		DataMapper
	}

	/// <summary>
	/// Data pattern type extension class.
	/// </summary>
	public static class PersistentObjectTypeExtensions
	{
		/// <summary>
		/// Returns true if it is Repository.
		/// </summary>
		/// <param name="persistentObjectType"></param>
		/// <returns></returns>
		public static bool IsRepository(this PersistentObjectType persistentObjectType) {
			return persistentObjectType == PersistentObjectType.Repository;
		}

		/// <summary>
		/// Returns true if it is DataMapper.
		/// </summary>
		/// <param name="persistentObjectType"></param>
		/// <returns></returns>
		public static bool IsDataMapper(this PersistentObjectType persistentObjectType) {
			return persistentObjectType == PersistentObjectType.DataMapper;
		}
	}
}