using System;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	[Serializable]
	public class AnimatedFloat : AnimatedValue<float>
	{
		protected override void OnEvaluate(ref float result, float startValue, float t)
		{
			result = Mathf.Lerp(startValue, EndValue, t);
		}
	}
}
