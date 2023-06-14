using TheProxor.StateMachine;


namespace TheProxor.Services.Stage
{
	public interface IStageBehaviour : IStateBehaviour
	{
		IStageView StageView { get; set; }
	}
}
