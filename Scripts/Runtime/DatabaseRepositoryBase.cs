using Repository4Unity;

namespace IMDB4Unity
{
	public abstract class DatabaseRepositoryBase<TEntity, KRepository> : RepositoryBase<TEntity, KRepository>, IDatabase where TEntity : EntityBase, new() where KRepository : IRepository<TEntity>, new()
	{
		public virtual string Schema { get; }
	}
}