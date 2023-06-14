using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public interface IStatesSequencer<TProgressState> where TProgressState : ProgressState
	{
		event Action OnStart;
		event Action OnSequenceEnd;
		event Action OnStartNextState;

		TProgressState ExecutedState { get; }
		int SubStatesCount { get; }
		int CurrentExecutedStateIndex { get; }

		void Init(List<TProgressState> states);

		void Start();

		void Stop();
	}
}
