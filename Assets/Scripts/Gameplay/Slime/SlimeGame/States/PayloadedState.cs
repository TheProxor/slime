namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public abstract class PayloadedState<TPayload> : ExitableState
	{
		public virtual void Enter(TPayload payload) {}
	}
}
