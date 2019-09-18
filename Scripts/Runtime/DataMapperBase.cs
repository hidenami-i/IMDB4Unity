using UnityEngine;
using UnityExtensions;

namespace IMDB4Unity
{
	/// <summary>
	/// Object for persistent single entity.
	/// DataMapper is Data access object for single entity.
	/// </summary>
	public abstract class DataMapperBase<TEntity, KDataMapper> : IDataMapper<TEntity> where TEntity : class, new() where KDataMapper : class, IDataMapper<TEntity>, new()
	{
		/// <summary>
		/// A single Entity object.
		/// </summary>
		protected abstract TEntity Entity { get; set; }

		/// <summary>
		/// DataMapper instance cahce.
		/// </summary>
		private static KDataMapper instance;

		/// <summary>
		/// KDataMapper Instance.
		/// </summary>
		public static KDataMapper Instance {
			get {
				if (instance == null) {
					instance = new KDataMapper();
				}

				return instance;
			}
		}

		/// <summary>
		/// Gets DataMapper name.
		/// </summary>
		public string KName => typeof(KDataMapper).Name;

		/// <summary>
		/// Gets Entity name.
		/// </summary>
		public string TName => typeof(TEntity).Name;

		public void FromJson(string json) {
			instance = JsonUtility.FromJson<KDataMapper>(json);
		}

		public void Update(TEntity entity) {
			Entity = entity;
		}

		public virtual string ToJson(bool prettyPrint = false) {
			return JsonUtility.ToJson(Instance, prettyPrint);
		}

		public string GetContentsHash() {
			return PBKDF25.Encrypt(ToJson());
		}

		public bool TryGet(out TEntity entity) {
			entity = Entity;
			return entity != null;
		}

		public TEntity GetOrDefault(TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (Entity != null) {
				result = Entity;
			}

			return result;
		}

		public void Delete() {
			Entity = null;
		}
	}
}