using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public abstract partial class ProgressAnimator<TValue, TAnimatedValue, TProgressAnimation>
	{
		public class Pool : MemoryPool<TAnimatedValue, ProgressState, TProgressAnimation>
		{
			protected override void Reinitialize(TAnimatedValue animatedValue,
												 ProgressState progressState,
												 TProgressAnimation animation)
			{
				animation.Init(animatedValue, progressState);
			}

			protected override void OnDespawned(TProgressAnimation animation)
			{
				animation.ProgressState = null;
			}
		}
	}
}
