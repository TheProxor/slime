using System;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	[Serializable]
	public class AnimatedMaterial : AnimatedValue<Material>
	{
		protected override void OnEvaluate(ref Material result, Material startValue, float t)
		{
			result.Lerp(startValue, EndValue, t);
		}
	}
}
