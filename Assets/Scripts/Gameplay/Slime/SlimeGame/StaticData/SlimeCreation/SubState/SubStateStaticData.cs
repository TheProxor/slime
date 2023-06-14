using TheProxor.StaticData;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[CreateAssetMenu(fileName = "Sub State Static Data",
					 menuName = "Take Top/Meta Games/Slime Game/Static Data/Slime Creation/Sub State Static Data")]
	public class SubStateStaticData :
		ScriptableObject, IStaticData<SubStateType>
	{
		[field: SerializeField]
		public SubStateType Type { get; private set; } = default;

		[field: SerializeField]
		public Sprite Icon { get; private set;} = null;

		[field: SerializeField]
		public string Comment { get; private set;} = "";
	}
}
