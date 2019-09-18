using System;
using UnityEngine;

namespace IMDB4Unity.Tests
{
	internal class TestIMDB4Unity : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start() {

			DatabaseSettings.Location.FolderName = "AppDatabase";
			DatabaseSettings.Location.RootFolderPath = Application.dataPath;
			
			MasterCharacterEntity knight = new MasterCharacterEntity(1, "Knight", "Knight Desc", CharacterType.Knight, DateTime.Now);
			MasterCharacterEntity magician = new MasterCharacterEntity(1, "Magician", "Magician Desc", CharacterType.Magician, DateTime.Now);
			MasterCharacterEntity monster = new MasterCharacterEntity(1, "Monster", "Monster Desc", CharacterType.Monster, DateTime.Now);
			MasterCharacterRepository.Instance.InsertAll(knight, magician, monster);
			MasterCharacterRepository.Instance.Save();
			MasterCharacterRepository.Instance.Load();
			MasterCharacterRepository.Instance.LogAllEntity();
			
			UserEntity user = new UserEntity(1, "ABCDE");
			UserDataMapper.Instance.Update(user);
			UserDataMapper.Instance.Save();
			UserDataMapper.Instance.Load();
			if (UserDataMapper.Instance.TryGet(out user)) {
				Debug.Log(user.ToString());
			}
		}

		// Update is called once per frame
		void Update() { }
	}
}