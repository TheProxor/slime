using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[Serializable]
	public class BasisRecipe
	{
		[field: SerializeField]
		public List<RecipeStep> Steps { get; private set; } = null;
	}
}
