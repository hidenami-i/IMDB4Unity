namespace IMDB4Unity.Editor
{
	/// <summary>
	/// Schema type
	/// Repository and DataMapper
	/// </summary>
	public enum SchemaType
	{
		/// <summary>
		/// Schema type is master data.
		/// </summary>
		Master = 0,
		
		/// <summary>
		/// Schema type is user data.
		/// </summary>
		User = 1,
		
		/// <summary>
		/// Schema type is local data.
		/// </summary>
		Local = 2,
		
		/// <summary>
		/// Schema type is enumeration data. 
		/// </summary>
		Enumeration = 5,
		
		/// <summary>
		/// Schema type is none.
		/// </summary>
		None = 99
	}

	/// <summary>
	/// SchemaType Exxtension class.
	/// </summary>
	public static class SchemaTypeExtensions
	{
		/// <summary>
		/// It is judged whether it is Master.
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static bool IsMaster(this SchemaType schemaType) {
			return schemaType == SchemaType.Master;
		}
		
		/// <summary>
		/// It is judged whether it is User.
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static bool IsUser(this SchemaType schemaType) {
			return schemaType == SchemaType.User;
		}
		
		/// <summary>
		/// It is judged whether it is Local.
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static bool IsLocal(this SchemaType schemaType) {
			return schemaType == SchemaType.Local;
		}
		
		/// <summary>
		/// It is judged whether it is Enumeration.
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static bool IsEnumeration(this SchemaType schemaType) {
			return schemaType == SchemaType.Enumeration;
		}
		
		/// <summary>
		/// It is judged whether it is None.
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static bool IsNone(this SchemaType schemaType) {
			return schemaType == SchemaType.None;
		}
	} 
}