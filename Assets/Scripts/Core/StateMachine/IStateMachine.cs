using TheProxor.StateMachine;


namespace TheProxor
{
	public interface IStateMachine<TStateType, in TStateBehaviour>
		where TStateBehaviour : IStateBehaviour
	{
		TStateType CurrentState { get; }


		void SwitchState(TStateType stateType);

		TTargetStateBehaviour GetStateBehaviour<TTargetStateBehaviour>(TStateType stateType)
			where TTargetStateBehaviour : TStateBehaviour;

		internal void SwitchState<TState>();

		internal TTargetStateBehaviour GetStateBehaviour<TTargetStateBehaviour>()
			where TTargetStateBehaviour : TStateBehaviour;
	}
}
