using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public abstract partial class ProgressAnimator<TValue, TAnimatedValue, TProgressAnimation>
		where TAnimatedValue : AnimatedValue<TValue>
		where TProgressAnimation : ProgressAnimator<TValue, TAnimatedValue, TProgressAnimation>
	{
		private TValue referenceValue;
		private ProgressState progressState;

		protected TAnimatedValue AnimatedValue { get; private set; }
		protected TValue ReferenceValue => referenceValue;

		private TValue StartValue { get; set; }
		private ProgressState ProgressState
		{
			set
			{
				if (progressState != null)
				{
					progressState.OnUpdateProgress -= UpdateProgress;
				}

				progressState = value;

				if (progressState == null)
				{
					return;
				}

				progressState.OnUpdateProgress += UpdateProgress;
			}
		}

		private void Init(TAnimatedValue animatedValue, ProgressState progressState)
		{
			AnimatedValue = animatedValue;
			ProgressState = progressState;
			StartValue = GetStartValue();
			referenceValue = GetReferenceValue();
			UpdateProgress();
		}

		protected virtual TValue GetReferenceValue()
		{
			return StartValue;
		}

		protected abstract TValue GetStartValue();

		private void UpdateProgress()
		{
			UpdateValue();
			Animate();
		}

		private void UpdateValue()
		{
			AnimatedValue.Evaluate(ref referenceValue, StartValue, progressState.Progress);
		}

		protected virtual void Animate() {}
	}
}
