using TheProxor.StaticData;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[CreateAssetMenu(fileName = "Slime Creation Step Static Data",
					 menuName = "Take Top/Meta Games/Slime Game/Static Data/Slime Creation/Slime Creation Step Static Data")]
	public class SlimeCreationStepStaticData :
		ScriptableObject, IStaticData<SlimeCreationStepType>
	{
		[SerializeField] private SlimeCreationStepType type = default;
		[SerializeField] private string comment = "";

		public SlimeCreationStepType Type => type;
		public string Comment => comment;
	}
}
