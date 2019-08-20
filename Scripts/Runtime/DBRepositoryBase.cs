using DAO4Unity;

namespace IMDB4Unity
{
	public abstract class DBRepositoryBase<TEntity, KRepository> : RepositoryBase<TEntity, KRepository>, IDatabase where TEntity : EntityBase, new() where KRepository : IRepository<TEntity>, new()
	{
		public abstract string Schema { get; }
	}
}