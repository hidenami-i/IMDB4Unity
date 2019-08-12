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
					
					// can load data fron any scene.
					#if UNITY_EDITOR
					instance.Load();
					#endif
				}

				return instance;
			}
		}

		public string KName => typeof(KDataMapper).Name;

		public string TName => typeof(TEntity).Name;

		public virtual string Schema { get; }

		/// <summary>
		/// Type of data mapper name. 
		/// </summary>
		public static readonly string DataMapperName = typeof(KDataMapper).Name;

		/// <summary>
		/// Type of entity name.
		/// </summary>
		public static readonly string EntityName = typeof(TEntity).Name;

		public virtual void FromJson(string json) {
			instance = JsonUtility.FromJson<KDataMapper>(json);
		}

		public virtual IDatabase Update(TEntity entity) {
			Entity = entity;
			return this;
		}

		public virtual string ToJson(bool prettyPrint = false) {
			return JsonUtility.ToJson(Instance, prettyPrint);
		}

		public virtual string GetContentsHash() {
			return PBKDF25.Encrypt(ToJson());
		}

		public virtual bool TryGet(out TEntity entity) {
			entity = Entity;
			return entity != null;
		}

		public virtual TEntity GetOrDefault(TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (Entity != null) {
				result = Entity;
			}

			return result;
		}

		public virtual IDatabase Delete() {
			Entity = null;
			return this;
		}
	}
}