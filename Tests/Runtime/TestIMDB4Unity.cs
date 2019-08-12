using System;
using UnityEngine;

namespace IMDB4Unity.Tests
{
	public class TestIMDB4Unity : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start() {
			MasterCharacterEntity knight = new MasterCharacterEntity(1, "Knight", "Knight Desc", CharacterType.Knight, DateTime.Now);
			MasterCharacterEntity magician = new MasterCharacterEntity(1, "Magician", "Magician Desc", CharacterType.Magician, DateTime.Now);
			MasterCharacterEntity monster = new MasterCharacterEntity(1, "Monster", "Monster Desc", CharacterType.Monster, DateTime.Now);
			MasterCharacterRepository.Instance.InsertAll(knight, magician, monster).Save();
			MasterCharacterRepository.Instance.Load();
			MasterCharacterRepository.Instance.LogAllEntity();
		}

		// Update is called once per frame
		void Update() { }
	}
}