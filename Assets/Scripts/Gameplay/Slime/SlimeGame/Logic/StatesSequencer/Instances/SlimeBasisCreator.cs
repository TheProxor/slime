using System;
using System.Collections.Generic;
using System.Linq;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeBasisCreator : IStatesSequencer<BasisRecipeStepState>
	{
		public event Action OnStart;
		public event Action OnSequenceEnd;
		public event Action OnStartNextState;

		private readonly StatesSequencer statesSequencer;

		private List<BasisRecipeStepState> slimeCreationSubStates;

		public int SubStatesCount { get; private set; }
		public int CurrentExecutedStateIndex { get; private set; }
		public BasisRecipeStepState ExecutedState { get; private set; }


		public SlimeBasisCreator(StatesSequencer statesSequencer)
		{
			this.statesSequencer = statesSequencer;
			InitializeStatesSequencer();
		}

		public void Init(List<BasisRecipeStepState> slimeCreationSubStates)
		{
			SubStatesCount = slimeCreationSubStates.Count;
			this.slimeCreationSubStates = slimeCreationSubStates;
			statesSequencer.Init(slimeCreationSubStates.Cast<ProgressState>().ToList());
		}

		public void Start()
		{
			statesSequencer.Start();
			OnStart?.Invoke();
		}

		public void Stop()
		{
			statesSequencer.Stop();
		}

		private void InitializeStatesSequencer()
		{
			statesSequencer.OnStartNextState += StartNextState;
			statesSequencer.OnSequenceEnd += EndSequence;
		}

		private void StartNextState()
		{
			CurrentExecutedStateIndex = statesSequencer.CurrentExecutedStateIndex;
			ExecutedState = slimeCreationSubStates[CurrentExecutedStateIndex];
			OnStartNextState?.Invoke();
		}

		private void EndSequence()
		{
			OnSequenceEnd?.Invoke();
		}
	}
}
