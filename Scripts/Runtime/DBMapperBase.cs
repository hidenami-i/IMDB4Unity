using DAO4Unity;

namespace IMDB4Unity
{
	public abstract class DBMapperBase<TEntity, KDataMapper> : DataMapperBase<TEntity, KDataMapper>, IDatabase where TEntity : class, new() where KDataMapper : class, IDataMapper<TEntity>, new()
	{
		public abstract string Schema { get; }
	}
}