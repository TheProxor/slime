using TheProxor.StaticData;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[CreateAssetMenu(fileName = "Color Static Data",
					 menuName = "Take Top/Meta Games/Slime Game/Static Data/Slime Creation/Color Static Data")]
	public class ColorStaticData : ScriptableObject, IStaticData<string>
	{
		[SerializeField]
		private string label = null;

		[SerializeField]
		private Sprite icon = null;

		[SerializeField]
		private Color color = default;

		[SerializeField]
		private bool emission = default;

		public string Label => label;
		public Sprite Icon => icon;
		public Color Color => color;

		public bool Emission => emission;
		public string Type => name;
	}
}
