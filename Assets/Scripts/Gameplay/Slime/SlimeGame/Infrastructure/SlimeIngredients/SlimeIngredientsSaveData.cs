using System;
using System.Collections.Generic;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame
{
	[Serializable]
	public class SlimeIngredientsSaveData
	{
		public Dictionary<string, SlimeIngredientInfoSaveData> SlimeIngredientInfos = default;



		[Serializable]
		public class SlimeIngredientInfoSaveData
		{
			public int Count;
			public DateTime ReceivingTime;
		}
	}
}
