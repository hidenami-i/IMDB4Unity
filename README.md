# IMDB4Unity
In Memory Database for Unity

# Usage
## Install
Add the following line on manifest.json file.
```json
"com.hidenami-i.imdb4unity": "https://github.com/hidenami-i/IMDB4Unity.git",
"com.hidenami-i.unityextensions": "https://github.com/hidenami-i/UnityExtensions.git#1.0.0",
```

## Basic 
```csharp
MasterCharacterEntity knight = new MasterCharacterEntity(1, "Knight", "Knight Desc", CharacterType.Knight);
MasterCharacterEntity magician = new MasterCharacterEntity(1, "Magician", "Magician Desc", CharacterType.Magician);
MasterCharacterEntity monster = new MasterCharacterEntity(1, "Monster", "Monster Desc", CharacterType.Monster);
MasterCharacterRepository.Instance.InsertAll(knight, magician, monster).Save();
MasterCharacterRepository.Instance.Load();
MasterCharacterRepository.Instance.LogAllEntity();
```

### Classes
```csharp:MasterCharacterEntity.cs
using UnityEngine;
using System;
using System.Text;

namespace IMDB4Unity.Tests
{
	[Serializable]
	public class MasterCharacterEntity : EntityBase
	{
		[SerializeField] private int id;
		[SerializeField] private string name;
		[SerializeField] private string description;
		[SerializeField] private CharacterType typeId;

		// DateTime class can not serialize. So define it as a string.
		[SerializeField] private string createdAt;

		public int Id => id;
		public string Name => name;
		public string Description => description;
		public CharacterType TypeId => typeId;

		// Datetime cache.
		public DateTime? CreatedAt {
			get {
				if (!_createdAt.HasValue) {
					if (DateTime.TryParse(createdAt, out DateTime date)) {
						_createdAt = date;
					}
				}

				return _createdAt;
			}
			set => _createdAt = value;
		}

		private DateTime? _createdAt;

		public MasterCharacterEntity() { }

		public MasterCharacterEntity(int id, string name, string description, CharacterType typeId, DateTime createdAt) {
			this.id = id;
			this.name = name;
			this.description = description;
			this.typeId = typeId;
			this.createdAt = createdAt.ToString();
		}

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.AppendLine().AppendLine($"<b>ClassName [{nameof(MasterCharacterEntity)}]</b>");
			builder.AppendLine($"[{nameof(id)}] {id}");
			builder.AppendLine($"[{nameof(name)}] {name}");
			builder.AppendLine($"[{nameof(description)}] {description}");
			builder.AppendLine($"[{nameof(typeId)}] {typeId}");
			builder.AppendLine($"[{nameof(createdAt)}] {createdAt}");
			return builder.ToString();
		}
	}
}
```

```csharp:MasterCharacterRepository.cs
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
```

## ISerializationCallbackReceiver

```csharp
create partial implemented ISerializationCallbackReceiver class.

void ISerializationCallbackReceiver.OnBeforeSerialize() {
	// from json not called
}

void ISerializationCallbackReceiver.OnAfterDeserialize() {

	name = string.IsNullOrEmpty(name) ? string.Intern(name) : "";
	description = string.IsNullOrEmpty(description) ? string.Intern(description) : "";
}
```
