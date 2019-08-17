using Repository4Unity;

namespace IMDB4Unity
{
	public abstract class DatabaseDataMapperBase<TEntity, KDataMapper> : DataMapperBase<TEntity, KDataMapper>, IDatabase where TEntity : class, new() where KDataMapper : class, IDataMapper<TEntity>, new()
	{
		public virtual string Schema { get; }
	}
}