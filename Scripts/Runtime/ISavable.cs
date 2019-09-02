namespace IMDB4Unity
{
	public interface ISavable
	{
		/// <summary>
		/// Initialize Database Instance from json.
		/// </summary>
		/// <param name="json"></param>
		void FromJson(string json);
		
		/// <summary>
		/// Databse instance to json data.
		/// </summary>
		/// <param name="prettyPrint"></param>
		/// <returns></returns>
		string ToJson(bool prettyPrint = true);
		
		/// <summary>
		/// Gets the hashCode of the Databse object.
		/// </summary>
		/// <returns></returns>
		string GetContentsHash();
		
		/// <summary>
		/// Instance type Name
		/// </summary>
		string KName { get; }

		/// <summary>
		/// Entity type name
		/// </summary>
		string TName { get; }
	}
}