using UnityEngine;
using System;
using System.Text;
using Repository4Unity;

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