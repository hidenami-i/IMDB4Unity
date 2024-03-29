using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

namespace IMDB4Unity
{
	/// <summary>
	/// Objects for persisting multiple Entity.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKRepository"></typeparam>
	public abstract class RepositoryBase<TEntity, KRepository> : IRepository<TEntity> where TEntity : EntityBase, new() where KRepository : IRepository<TEntity>, new()
	{
		static readonly object lockObject = new object();

		/// <summary>
		/// TEntity List.
		/// </summary>
		protected abstract List<TEntity> EntityList { get; }

		/// <summary>
		/// Respository instance cache.
		/// </summary>
		private static KRepository instance;

		/// <summary>
		/// Repository instance.
		/// </summary>
		public static KRepository Instance {
			get {
				if (instance == null) {
					instance = new KRepository();
				}
				
				return instance;
			}
		}

		/// <summary>
		/// Gets KRepository name.
		/// </summary>
		public string KName => typeof(KRepository).Name;

		/// <summary>
		/// Gets Entity name.
		/// </summary>
		public string TName => typeof(TEntity).Name;

		public void FromJson(string json) {
			if (string.IsNullOrEmpty(json)) {
				return;
			}

			instance = JsonUtility.FromJson<KRepository>(json);
		}

		public void AddFromJson(string json) {

			if (EntityList == null) {
				FromJson(json);
				return;
			}

			var repository = JsonUtility.FromJson<KRepository>(json);
			lock (lockObject) {
				EntityList.AddRange(repository.FindAll());
			}
		}

		public string ToJson(bool prettyPrint = false) {
			return JsonUtility.ToJson(Instance, prettyPrint);
		}

		public virtual string GetContentsHash() {
			return PBKDF25.Encrypt(ToJson());
		}

		public int Count() {
			return EntityList.SafeCount();
		}

		public int CountBy(Predicate<TEntity> match) {
			return FindAllBy(match).SafeCount();
		}

		public bool IsNullOrEmpty() {
			return EntityList.IsNullOrEmpty();
		}

		public bool IsNullOrEmptyBy(Predicate<TEntity> match) {
			return FindAllBy(match).IsNullOrEmpty();
		}

		public bool IsNotEmpty() {
			return EntityList.IsNotEmpty();
		}

		public bool IsNotEmptyBy(Predicate<TEntity> match) {
			return FindAllBy(match).IsNotEmpty();
		}

		public virtual void Insert(TEntity entity) {
			if (entity == null) {
				ExDebug.LogError($"{typeof(TEntity).Name} is null.");
				return;
			}

			lock (lockObject) {
				EntityList.Add(entity);
			}
		}

		public virtual void Insert(EntityBase entityBase) {
			if (entityBase == null) {
				ExDebug.LogError($"{typeof(TEntity).Name} is null.");
				return;
			}

			TEntity entity = entityBase as TEntity;
			lock (lockObject) {
				EntityList.Add(entity);
			}
		}

		public void InsertAll(params EntityBase[] entityBases) {
			if (entityBases.IsNullOrEmpty()) {
				return;
			}

			DeleteAll();
			foreach (var entityBase in entityBases) {
				Insert(entityBase);
			}
		}

		public void InsertAll(IEnumerable<Dictionary<string, object>> list) {
			if (list.IsNullOrEmpty()) {
				return;
			}
			
			DeleteAll();

			foreach (var values in list) {
				TEntity entity = new TEntity();
				entity.SetField(values);
				Insert(entity);
			}
		}

		public TEntity GetByOrDefault(Predicate<TEntity> match, TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (TryFindBy(match, out TEntity entity)) {
				result = entity;
			}

			return result;
		}

		public TEntity GetByIndexOrDefault(int index, TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (TryFindByIndex(index, out TEntity entity)) {
				result = entity;
			}

			return result;
		}

		public TEntity GetFirstOrDefault(int index, TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (TryGetFirst(out TEntity entity)) {
				result = entity;
			}

			return result;
		}

		public TEntity GetLastOrDefault(int index, TEntity defaultEntity) {
			TEntity result = defaultEntity;

			if (TryGetLast(out TEntity entity)) {
				result = entity;
			}

			return result;
		}

		public bool TryFindBy(Predicate<TEntity> match, out TEntity entity) {
			entity = EntityList.Find(match);
			return entity != null;
		}

		public bool TryFindByIndex(int index, out TEntity entity) {
			entity = null;

			if (index < 0 || index > Count()) {
				return false;
			}

			entity = EntityList[index];
			return entity != null;
		}

		public bool TryGetFirst(out TEntity entity) {
			entity = null;
			return TryFindByIndex(0, out entity);
		}

		public bool TryGetLast(out TEntity entity) {
			entity = null;
			return TryFindByIndex(Count(), out entity);
		}

		public List<TEntity> FindAll() {
			return EntityList;
		}

		public List<TEntity> FindAllBy(Predicate<TEntity> match) {
			return EntityList.FindAll(match);
		}

		public void DeleteBy(Predicate<TEntity> match) {
			if (TryFindBy(match, out TEntity e)) {
				EntityList.Remove(e);
				return;
			}

			ExDebug.LogWarning("Not found Entity");
		}

		public void DeleteAll() {
			EntityList.SafeClear();
		}

		public void DeleteAllBy(Predicate<TEntity> match) {
			var list = FindAllBy(match);
			foreach (var entity in list) {
				EntityList.Remove(entity);
			}
		}

		public void LogAllEntity() {
			if (IsNullOrEmpty()) {
				ExDebug.LogWarning($"{KName} data is null or empty.");
				return;
			}

			foreach (var entity in FindAll()) {
				ExDebug.Log(entity.ToString());
			}
		}
	}
}