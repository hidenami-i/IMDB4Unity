namespace IMDB4Unity
{
	public interface IDatabase : ISavable
	{
		/// <summary>
		/// Database schema
		/// </summary>
		string Schema { get; }
	}
}