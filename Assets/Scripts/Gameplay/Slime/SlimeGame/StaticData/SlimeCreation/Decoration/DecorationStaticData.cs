using TheProxor.SlimeSimulation.DecorationSystemModule;
using TheProxor.StaticData;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[CreateAssetMenu(fileName = "Decor Static Data",
					 menuName = "Take Top/Meta Games/Slime Game/Static Data/Slime Creation/Decor Static Data")]
	public class DecorationStaticData : ScriptableObject, IStaticData<string>
	{
		[SerializeField]
		private string label = "";

		[SerializeField]
		private Sprite icon = null;

		[SerializeField]
		private SlimeDecorationGroup decorationGroup;

		public string Type => name;
		public Sprite Icon => icon;
		public SlimeDecorationGroup DecorationGroup => decorationGroup;
		public string Label => label;
	}
}
