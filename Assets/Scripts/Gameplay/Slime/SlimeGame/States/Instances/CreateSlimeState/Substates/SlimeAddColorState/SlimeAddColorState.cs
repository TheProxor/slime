using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.PanelSystem;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeAddColorState : PayloadedState<Color>
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{
				[SerializeField]
				private ColorEvaluationState.Settings colorEvaluationState;

				public ColorEvaluationState.Settings ColorEvaluationState => colorEvaluationState;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				InstallSlimeAddColorState();
				InstallColorEvaluationState();
			}

			private void InstallSlimeAddColorState()
			{
				Container.Bind<SlimeAddColorState>()
						 .AsSingle();
			}

			private void InstallColorEvaluationState()
			{
				Container.Bind<ColorEvaluationState>()
						 .AsSingle()
						 .WithArguments(settings.ColorEvaluationState);
			}
		}

		private readonly SlimeFacade slime;
		private readonly CreationInput input;
		private readonly ColorEvaluationState colorEvaluationState;
		private readonly PanelManager panelManager;
		private readonly StatesSequencer statesSequencer;


		protected SlimeAddColorState(SlimeFacade slime,
									 CreationInput input,
									 ColorEvaluationState colorEvaluationState,
									 PanelManager panelManager,
									 StatesSequencer statesSequencer)
		{
			this.slime = slime;
			this.input = input;
			this.colorEvaluationState = colorEvaluationState;
			this.panelManager = panelManager;
			this.statesSequencer = statesSequencer;
		}

		public override void Enter(Color color)
		{
			InitializeEvaluationState(color);
			InitializeStatesSequencer();
			StartSequencer();
			ShowProgressPanel();
			EnableInput();
		}

		public override void Exit()
		{
			HideProgressPanel();
			DeInitializeStatesSequencer();
			DisableInput();
			StopSequencer();
		}


		private void InitializeEvaluationState(Color targetColor)
		{
			colorEvaluationState.Init(slime.Color, targetColor);
		}

		private void InitializeStatesSequencer()
		{
			statesSequencer.Init(new List<ProgressState> { colorEvaluationState });
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

		private void HideProgressPanel()
		{
			panelManager.HideCurrentPanel();
		}

		private void DeInitializeStatesSequencer()
		{
			statesSequencer.OnSequenceEnd -= OnSequenceEnd;
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
