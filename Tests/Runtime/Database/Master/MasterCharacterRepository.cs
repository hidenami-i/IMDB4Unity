using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMDB4Unity.Tests
{
	[Serializable]
	public sealed partial class MasterCharacterRepository : RepositoryBase<MasterCharacterEntity, MasterCharacterRepository>
	{
		[SerializeField] private List<MasterCharacterEntity> masterCharacter = new List<MasterCharacterEntity>();
		protected override List<MasterCharacterEntity> EntityList => masterCharacter;
		public override string Schema => "Master";

		public bool TryFindById(int id, out MasterCharacterEntity entity) {
			return TryFindBy(x => x.Id == id, out entity);
		}

		public bool TryFindByName(string name, out MasterCharacterEntity entity) {
			return TryFindBy(x => x.Name == name, out entity);
		}

		public bool TryFindByDescription(string description, out MasterCharacterEntity entity) {
			return TryFindBy(x => x.Description == description, out entity);
		}

		public bool TryFindByTypeId(CharacterType typeId, out MasterCharacterEntity entity) {
			return TryFindBy(x => x.TypeId == typeId, out entity);
		}

		public MasterCharacterEntity GetByIdOrDefault(int id, MasterCharacterEntity defaultEntity) {
			return GetByOrDefault(x => x.Id == id, defaultEntity);
		}

		public MasterCharacterEntity GetByNameOrDefault(string name, MasterCharacterEntity defaultEntity) {
			return GetByOrDefault(x => x.Name == name, defaultEntity);
		}

		public MasterCharacterEntity GetByDescriptionOrDefault(string description, MasterCharacterEntity defaultEntity) {
			return GetByOrDefault(x => x.Description == description, defaultEntity);
		}

		public MasterCharacterEntity GetByTypeIdOrDefault(CharacterType typeId, MasterCharacterEntity defaultEntity) {
			return GetByOrDefault(x => x.TypeId == typeId, defaultEntity);
		}

		public List<MasterCharacterEntity> FindAllById(int id) {
			return FindAllBy(x => x.Id == id);
		}

		public List<MasterCharacterEntity> FindAllByName(string name) {
			return FindAllBy(x => x.Name == name);
		}

		public List<MasterCharacterEntity> FindAllByDescription(string description) {
			return FindAllBy(x => x.Description == description);
		}

		public List<MasterCharacterEntity> FindAllByTypeId(CharacterType typeId) {
			return FindAllBy(x => x.TypeId == typeId);
		}
	}
}