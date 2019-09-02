using System;
using System.Text;
using UnityEngine;

namespace IMDB4Unity.Tests
{
	[Serializable]
	internal sealed class UserEntity : EntityBase
	{
		[SerializeField] private int id = 0;
		[SerializeField] private string name = "";
		
		public int Id => id;
		public string Name => name;

		public UserEntity() { }

		public UserEntity(int id, string name) {
			this.id = id;
			this.name = name;
		}
		
		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.AppendLine().AppendLine($"<b>ClassName [{nameof(UserEntity)}]</b>");
			builder.AppendLine($"[{nameof(id)}] {id}");
			builder.AppendLine($"[{nameof(name)}] {name}");
			return builder.ToString();
		}
	}
}