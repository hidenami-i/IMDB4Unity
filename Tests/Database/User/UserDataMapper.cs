using System;
using UnityEngine;

namespace IMDB4Unity.Tests
{
	[Serializable]
	internal sealed class UserDataMapper : DataMapperBase<UserEntity, UserDataMapper>, IDatabase
	{
		[SerializeField] private UserEntity user = null;
		
		protected override UserEntity Entity { get => user; set => user = value; }
		public string Schema => "User";
	}
}