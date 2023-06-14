using System;
using System.Collections.Generic;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class StateMachine
	{
		public class Factory : PlaceholderFactory<IEnumerable<ExitableState>, StateMachine> {}

		private readonly Dictionary<Type, ExitableState> stateByType = new();

		public ExitableState ActiveState { get; private set; }



		public StateMachine(IEnumerable<ExitableState> states)
		{
			foreach (ExitableState state in states)
			{
				stateByType.Add(state.GetType(), state);
			}
		}


		public bool TryEnter<TState>() where TState : State
		{
			return TryEnter(typeof(TState));
		}


		public bool TryEnter(Type type)
		{
			if (ChangeState(type) is not State state)
			{
				return false;
			}

			state.Enter();

			return true;
		}


		public bool TryEnter<TState, TPayload>(TPayload payload)
			where TState : PayloadedState<TPayload>
		{
			if (ChangeState(typeof(TState)) is not PayloadedState<TPayload> state)
			{
				return false;
			}

			state.Enter(payload);

			return true;
		}


		public TState GetState<TState>() where TState : ExitableState
		{
			return GetState(typeof(TState)) as TState;
		}


		public ExitableState GetState(Type type)
		{
			return stateByType.GetValueOrDefault(type);
		}


		private ExitableState ChangeState(Type type)
		{
			ExitActiveState();

			ExitableState state = GetState(type);

			ActiveState = state;

			return state;
		}


		public void ExitActiveState()
		{
			ActiveState?.Exit();
			ActiveState = null;
		}
	}
}
