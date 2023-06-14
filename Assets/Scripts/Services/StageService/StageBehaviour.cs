namespace TheProxor.Services.Stage
{
	public abstract class StageBehaviour : IStageBehaviour
	{
		public IStageView StageView { get; set; }



		public abstract void Initialize();

		public abstract void Deinitialize();

		public abstract void OnStateBegin();

		public abstract void OnStateSyncUpdate();

		public abstract void OnStateEnd();
	}



	public abstract class StageBehaviour<TStageView> : StageBehaviour
		where TStageView : IStageView
	{
		public new TStageView StageView => (TStageView)base.StageView;
	}
}
