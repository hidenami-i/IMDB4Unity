using System.Collections.Generic;

namespace IMDB4Unity.Tests
{
	public enum CharacterType
	{
		Knight = 1,
		Magician = 2,
		Monster = 3,
	}

	public class CharacterTypeCompare : IEqualityComparer<CharacterType>
	{
		public bool Equals(CharacterType x, CharacterType y) {
			return x == y;
		}

		public int GetHashCode(CharacterType obj) {
			return (int)obj;
		}
	}

	public static partial class CharacterTypeExtensions
	{
		/// <summary>
		/// Returns true if the characterType is Knight.
		/// </summary>
		public static bool IsKnight(this CharacterType characterType) {
			return characterType == CharacterType.Knight;
		}


		/// <summary>
		/// Returns true if the characterType is Magician.
		/// </summary>
		public static bool IsMagician(this CharacterType characterType) {
			return characterType == CharacterType.Magician;
		}


		/// <summary>
		/// Returns true if the characterType is Monster.
		/// </summary>
		public static bool IsMonster(this CharacterType characterType) {
			return characterType == CharacterType.Monster;
		}


	}
}