namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public abstract class SubStateMachine : State
	{
		protected StateMachine StateMachine { get; }

		protected SubStateMachine(StateMachine stateMachine)
		{
			StateMachine = stateMachine;
		}

		public override void Exit()
		{
			base.Exit();
			StateMachine.ExitActiveState();
		}
	}
}
