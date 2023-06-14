using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.PanelSystem;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeBasisCreationState : PayloadedState<BasisStaticData>
	{
		private readonly PanelManager panelManager;
		private readonly CreationInput input;
		private readonly SlimeBasisCreator slimeBasisCreator;
		private readonly BasisRecipeStepState.Pool subStatesPool;
		private readonly SlimeFacade slimeFacade;

		private List<BasisRecipeStepState> stepStates;
		private Panel progressPanel;
		private BasisStaticData basisStaticData;



		protected SlimeBasisCreationState(
				BasisRecipeStepState.Pool subStatesPool,
				PanelManager panelManager,
				CreationInput input,
				SlimeBasisCreator slimeBasisCreator,
				SlimeFacade slimeFacade
			)
		{
			this.subStatesPool = subStatesPool;
			this.panelManager = panelManager;
			this.input = input;
			this.slimeBasisCreator = slimeBasisCreator;
			this.slimeFacade = slimeFacade;
		}


		public override void Enter(BasisStaticData basisStaticData)
		{
			base.Enter(basisStaticData);

			this.basisStaticData = basisStaticData;

			ShowPanels();
			InitializeSlimeBasisCreator(basisStaticData.Recipe.Steps);
			StartCreationSequence();
			EnableInput();
		}

		public override void Exit()
		{
			slimeFacade.Basis = basisStaticData;

			base.Exit();
			DeInitializeSlimeBasisCreator();
			HidePanels();
			DisableInput();
		}


		private void StartCreationSequence()
		{
			slimeBasisCreator.Start();
		}

		private void EnableInput()
		{
			input.IsEnabled = true;
		}


		private void ShowPanels()
		{
			panelManager.TryGetPanel(PanelType.SlimeCreationProgressPanel, out progressPanel);
			progressPanel.OnEndShow += ShowBasisCreationPanel;
			progressPanel.OnEndHide += HideBasisCreationPanel;
			panelManager.TryShow(PanelType.SlimeCreationProgressPanel);
		}

		private void ShowBasisCreationPanel(Panel panel)
		{
			panel.OnEndShow -= ShowBasisCreationPanel;
			panelManager.TryShow(PanelType.SlimeBasisCreationPanel, PanelTransitionType.Override);
		}

		private void HideBasisCreationPanel(Panel panel)
		{
			panel.OnEndHide -= HideBasisCreationPanel;
			panelManager.TryGetPanel(PanelType.SlimeBasisCreationPanel, out panel);
			panel.Hide(false);
		}

		private void InitializeSlimeBasisCreator(IEnumerable<RecipeStep> steps)
		{
			stepStates = new List<BasisRecipeStepState>();

			foreach (RecipeStep step in steps)
			{
				stepStates.Add(subStatesPool.Spawn(step));
			}

			slimeBasisCreator.Init(stepStates);
			slimeBasisCreator.OnSequenceEnd += OnSequenceEnd;
			slimeBasisCreator.OnStartNextState += OnStartNextState;
		}

		private void DeInitializeSlimeBasisCreator()
		{
			foreach (BasisRecipeStepState stepState in stepStates)
			{
				subStatesPool.Despawn(stepState);
			}

			slimeBasisCreator.OnSequenceEnd -= OnSequenceEnd;
			slimeBasisCreator.OnStartNextState -= OnStartNextState;
			slimeBasisCreator.Stop();
		}


		private void OnStartNextState()
		{
			ResetInput();
		}

		private void ResetInput()
		{
			input.Reset();
		}

		private void HidePanels()
		{
			progressPanel.Hide(false);
		}

		private void DisableInput()
		{
			input.IsEnabled = false;
		}


		private void OnSequenceEnd()
		{
			DelayedFinish();
		}
	}
}
