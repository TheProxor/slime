using System;
using DG.Tweening;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public abstract class ExitableState
	{
		private Tween delayedCall;

		public event Action OnFinish;
		public event Action<FinishStatus> OnFinishStatus;



		protected virtual float FinishDelay => 0.35f;



		public virtual void Exit()
		{
			delayedCall?.Kill();
		}


		public void FinishWithFailure() =>
			Finish(FinishStatus.Failure);


		protected void DelayedFinish()
		{
			delayedCall = DOVirtual.DelayedCall(FinishDelay, Finish);
		}

		protected void Finish()
		{
			Finish(FinishStatus.Success);
		}

		protected void Finish(FinishStatus finishStatus)
		{
			delayedCall?.Kill();
			FinishInner(finishStatus);
			OnFinish?.Invoke();
			OnFinishStatus?.Invoke(finishStatus);
		}


		protected virtual void FinishInner(FinishStatus finishStatus) {}
	}


	public enum FinishStatus
	{
		Success,
		Failure
	}
}
