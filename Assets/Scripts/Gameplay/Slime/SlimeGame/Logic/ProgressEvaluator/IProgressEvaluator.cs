using System;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public interface IProgressEvaluator
	{
		public event Action<float> OnEvaluate;
	}
}
