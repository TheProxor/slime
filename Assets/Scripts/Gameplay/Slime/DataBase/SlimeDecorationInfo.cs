using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase
{
	[CreateAssetMenu(fileName = "Slime Decoration", menuName = "DataBase Item/Slime Ingredient/Slime Decoration")]
	public class SlimeDecorationInfo : SlimeIngredientInfo
	{
		public DecorationStaticData DecorationStaticData;
		[HideInInspector] public int WastedDecorationsCount;
	}
}
