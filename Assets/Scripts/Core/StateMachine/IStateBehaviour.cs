namespace TheProxor.StateMachine
{
	public interface IStateBehaviour
	{
		void Initialize();

		void Deinitialize();

		void OnStateBegin();

		void OnStateSyncUpdate();

		void OnStateEnd();
	}
}
