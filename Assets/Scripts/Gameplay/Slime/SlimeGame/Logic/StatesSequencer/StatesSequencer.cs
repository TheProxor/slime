using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class StatesSequencer : IStatesSequencer<ProgressState>
	{
		public event Action OnStart;
		public event Action OnSequenceEnd;
		public event Action OnStartNextState;

		private ProgressState executedState;
		private List<ProgressState> states;

		public int SubStatesCount { get; private set; }
		public int CurrentExecutedStateIndex { get; private set; }
		public ProgressState ExecutedState => executedState;

		public void Init(List<ProgressState> states)
		{
			this.states = states;
			SubStatesCount = states.Count;
		}

		public void Start()
		{
			CurrentExecutedStateIndex = 0;
			StartCurrentState();
			OnStart?.Invoke();
		}

		public void Stop()
		{
			if (executedState == null)
			{
				return;
			}
			
			StopCurrentState();
		}

		private void StartCurrentState()
		{
			if (!TryGetCurrentState(out executedState))
			{
				OnSequenceEnd?.Invoke();

				return;
			}

			executedState.Enter();
			executedState.OnUpdateProgress += UpdateState;
			OnStartNextState?.Invoke();
		}

		private bool TryGetCurrentState(out ProgressState progressState)
		{
			if (CurrentExecutedStateIndex < SubStatesCount)
			{
				progressState = states[CurrentExecutedStateIndex];

				return true;
			}

			progressState = null;

			return false;
		}

		private void UpdateState()
		{
			if (!Mathf.Approximately(executedState.Progress, 1))
			{
				return;
			}

			StartNextState();
		}

		private void StartNextState()
		{
			StopCurrentState();
			CurrentExecutedStateIndex++;
			StartCurrentState();
		}

		private void StopCurrentState()
		{
			executedState.Exit();
			executedState.OnUpdateProgress -= UpdateState;
		}
	}
}
