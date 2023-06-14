using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.PanelSystem;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeCreationPlayState : State
	{
		private readonly CreationInput input;
		private readonly SlimePlayEvaluationState evaluationState;
		private readonly PanelManager panelManager;
		private readonly StatesSequencer statesSequencer;
		private readonly BowlFacade bowlFacade;
		private readonly SlimeFacade slimeFacade;


		protected SlimeCreationPlayState(CreationInput input,
										 SlimePlayEvaluationState evaluationState,
										 PanelManager panelManager,
										 StatesSequencer statesSequencer,
										 BowlFacade bowlFacade,
										 SlimeFacade slimeFacade)
		{
			this.input = input;
			this.evaluationState = evaluationState;
			this.panelManager = panelManager;
			this.statesSequencer = statesSequencer;
			this.bowlFacade = bowlFacade;
			this.slimeFacade = slimeFacade;
		}


		public override void Enter()
		{
			slimeFacade.SetDeformationEnabled(true);
			DisableBowl();
			EnableInput();
			MakeSlimeInteractable();
			InitializeStatesSequencer();
			StartSequencer();
			ShowProgressPanel();
		}


		public override void Exit()
		{
			EnableBowl();
			DisableInput();
			HideProgressPanel();
			DeInitializeStatesSequencer();
			StopSequencer();
		}


		private void EnableInput()
		{
			input.IsEnabled = true;
		}


		private void EnableBowl() =>
			bowlFacade.SetActive(true);


		private void DisableBowl() =>
			bowlFacade.SetActive(false);


		private void InitializeStatesSequencer()
		{
			statesSequencer.Init(new List<ProgressState> { evaluationState });
			statesSequencer.OnSequenceEnd += DelayedFinish;
		}


		private void StartSequencer()
		{
			statesSequencer.Start();
		}


		private void ShowProgressPanel()
		{
			panelManager.TryShow(PanelType.SlimeCreationProgressPanel);
		}


		private void DisableInput()
		{
			input.IsEnabled = false;
		}


		private void HideProgressPanel()
		{
			panelManager.HideCurrentPanel();
		}


		private void DeInitializeStatesSequencer()
		{
			statesSequencer.OnSequenceEnd -= DelayedFinish;
		}


		private void StopSequencer()
		{
			statesSequencer.Stop();
		}


		private void MakeSlimeInteractable()
		{
			slimeFacade.SetInteractable(true);
		}
	}
}
