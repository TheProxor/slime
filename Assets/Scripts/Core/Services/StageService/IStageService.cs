using System;


namespace TheProxor.Services.Stage
{
	public interface IStageService<TStageType, in TStageBehaviour, in TStageView> : IStateMachine<TStageType, TStageBehaviour>
		where TStageBehaviour : IStageBehaviour
	{
		TCurrentStageView GetStageView<TCurrentStageView>(TStageType stageType) where TCurrentStageView : TStageView;
	}
}
