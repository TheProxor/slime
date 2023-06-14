using System;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	[Serializable]
	public abstract class AnimatedValue<TValue>
	{
		[SerializeField]
		private AnimationCurve animationCurve = null;

		[SerializeField]
		private TValue endValue = default;

		public TValue EndValue => endValue;

		public void Evaluate(ref TValue result, TValue startValue, float t)
		{
			OnEvaluate(ref result, startValue, animationCurve.Evaluate(t));
		}

		protected abstract void OnEvaluate(ref TValue result, TValue startValue, float t);
	}
}
