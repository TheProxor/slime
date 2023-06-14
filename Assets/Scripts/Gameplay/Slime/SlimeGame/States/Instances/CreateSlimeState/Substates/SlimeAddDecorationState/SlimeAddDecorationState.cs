using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.PanelSystem;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeAddDecorationState : PayloadedState<DecorationStaticData>
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{
				[SerializeField]
				private DecorEvaluationState.Installer.Settings decorEvaluationState;

				public DecorEvaluationState.Installer.Settings DecorEvaluationState =>
					decorEvaluationState;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				InstallSlimeAddDecorationState();
				InstallDecorEvaluationState();
			}

			private void InstallSlimeAddDecorationState()
			{
				Container.Bind<SlimeAddDecorationState>()
						 .AsSingle();
			}

			private void InstallDecorEvaluationState()
			{
				Container.Bind<DecorEvaluationState>()
						 .FromSubContainerResolve()
						 .ByInstaller<DecorEvaluationState.Installer>()
						 .AsSingle();

				Container.BindInstance(settings.DecorEvaluationState)
						 .WhenInjectedInto<DecorEvaluationState.Installer>();
			}
		}

		private readonly SlimeFacade slime;
		private readonly CreationInput input;
		private readonly PanelManager panelManager;
		private readonly DecorEvaluationState decorEvaluationState;
		private readonly StatesSequencer statesSequencer;


		protected SlimeAddDecorationState(SlimeFacade slime,
										  CreationInput input,
										  PanelManager panelManager,
										  DecorEvaluationState decorEvaluationState,
										  StatesSequencer statesSequencer)
		{
			this.slime = slime;
			this.input = input;
			this.panelManager = panelManager;
			this.decorEvaluationState = decorEvaluationState;
			this.statesSequencer = statesSequencer;
		}

		public override void Enter(DecorationStaticData decoration)
		{
			CreateDecorations(decoration);
			InitializeEvaluationState();
			InitializeStatesSequencer();
			StartSequencer();
			ShowProgressPanel();
			EnableInput();
		}

		public override void Exit()
		{
			DeInitializeStatesSequencer();
			HideProgressPanel();
			DisableInput();
			StopSequencer();
		}


		private void CreateDecorations(DecorationStaticData decoration)
		{
			slime.AddDecoration(decoration, AddDecoration);
		}

		private static void AddDecoration(GameObject decoration)
		{
			decoration.SetActive(false);
		}

		private void InitializeEvaluationState()
		{
			int decorationsIndex = slime.DecorationsCount - 1;
			IReadOnlyList<GameObject> decorations = slime.GetDecorations(decorationsIndex);
			decorEvaluationState.Init(decorations);
		}

		private void InitializeStatesSequencer()
		{
			statesSequencer.Init(new List<ProgressState> { decorEvaluationState });
			statesSequencer.OnSequenceEnd += OnSequenceEnd;
		}


		private void StartSequencer()
		{
			statesSequencer.Start();
		}

		private void ShowProgressPanel()
		{
			panelManager.TryShow(PanelType.SlimeCreationProgressPanel);
		}

		private void EnableInput()
		{
			input.IsEnabled = true;
		}

		private void DeInitializeStatesSequencer()
		{
			statesSequencer.OnSequenceEnd -= OnSequenceEnd;
		}

		private void HideProgressPanel()
		{
			panelManager.HideCurrentPanel();
		}

		private void DisableInput()
		{
			input.IsEnabled = false;
		}

		private void StopSequencer()
		{
			statesSequencer.Stop();
		}


		private void OnSequenceEnd()
		{
			DelayedFinish();
		}
	}
}
