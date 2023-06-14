using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeRecreationState : PayloadedState<SlimeSaveData>
	{
		private readonly SlimeSaver slimeSaver;
		private readonly SlimeCreationState slimeCreationState;

		private SlimeSaveData recreateSlimeSaveData;

		public SlimeRecreationState(SlimeSaver slimeSaver, SlimeCreationState slimeCreationState)
		{
			this.slimeSaver = slimeSaver;
			this.slimeCreationState = slimeCreationState;
		}

		public override void Enter(SlimeSaveData saveData)
		{
			base.Enter(saveData);
			SetRecreateSlimeId(saveData);
			InitializeCreationState();
			EnterCreationState();
		}

		public override void Exit()
		{
			base.Exit();
			slimeCreationState.Exit();
		}

		private void SetRecreateSlimeId(SlimeSaveData recreateSlimeSaveData) =>
			this.recreateSlimeSaveData = recreateSlimeSaveData;

		private void InitializeCreationState()
		{
			slimeCreationState.OnFinish += Finish;
		}

		private void EnterCreationState()
		{
			slimeCreationState.Enter();
		}


		protected override void FinishInner(FinishStatus finishStatus)
		{
			base.FinishInner(finishStatus);

			if (finishStatus == FinishStatus.Failure)
				return;

			RemoveRecreateSlimeSaveData();
			DeInitializeCreationState();
		}

		private void RemoveRecreateSlimeSaveData()
		{
			slimeSaver.RemoveSlimeSave(recreateSlimeSaveData);
		}

		private void DeInitializeCreationState()
		{
			slimeCreationState.OnFinish -= Finish;
		}
	}
}
