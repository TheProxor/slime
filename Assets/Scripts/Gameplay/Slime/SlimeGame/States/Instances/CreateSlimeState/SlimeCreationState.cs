using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.RollbackSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Ads;
using TheProxor.Services.Audio;
using TheProxor.Services.Stage;
using UnityEngine;
using PanelType = TheProxor.PanelSystem.PanelType;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeCreationState : SubStateMachine, ISlimeCreationStepProvider
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private int maxDecorationsCount = 2;

			public int MaxDecorationsCount => maxDecorationsCount;
		}

		private readonly SlimeFacade slimeFacade;
		private readonly BowlFacade bowl;
		private readonly Rollback rollback;
		private readonly Settings settings;
		private readonly StatesSequencer statesSequencer;
		private readonly PanelManager panelManager;
		private readonly IAdsService<AdType> adsService;
		private readonly IStageService<StageType, StageBehaviour, StageView> stageService;
		private readonly IAudioService<SoundId> audioService;
		private readonly CreationInput creationInput;

		private int decorationsCount;
		private SlimeCreationPanel slimeCreationPanel;
		private SlimeCancelPanel slimeCancelPanel;

		public event Action<ISlimeCreationStepProvider, object[]> OnNewCreationStep;
		public SlimeCreationStepType SlimeCreationStepType { get; private set; }

		public SlimeCreationState(PanelManager panelManager,
								  IAdsService<AdType> adsService,
								  IStageService<StageType, StageBehaviour, StageView> stageService,
								  SlimeFacade slimeFacade,
								  BowlFacade bowl,
								  Rollback rollback,
								  StateMachine.Factory stateMachineFactory,
								  SlimeBasisSelectionState basisSelectionState,
								  SlimeBasisCreationState basisCreationState,
								  SlimeColorSelectionState colorSelectionState,
								  SlimeAddColorState slimeAddColorState,
								  SlimeDecorSelectionState decorSelectionState,
								  SlimeAddDecorationState addDecorationState,
								  SlimeCreationPlayState playState,
								  SaveSlimeState saveSlimeState,
								  Settings settings,
								  IAudioService<SoundId> audioService,
								  CreationInput creationInput)
			: base(stateMachineFactory.Create(new ExitableState[]
											  {
												  basisSelectionState,
												  basisCreationState,
												  colorSelectionState,
												  slimeAddColorState,
												  decorSelectionState,
												  addDecorationState,
												  playState,
												  saveSlimeState
											  }))
		{
			this.panelManager = panelManager;
			this.adsService = adsService;
			this.stageService = stageService;
			this.slimeFacade = slimeFacade;
			this.bowl = bowl;
			this.rollback = rollback;
			this.settings = settings;
			this.audioService = audioService;
			this.creationInput = creationInput;

			basisSelectionState.OnDataSelected += GoToSlimeCreationByRecipe;
			basisCreationState.OnFinish += GoToColorSelectionState;
			colorSelectionState.OnDataSelected += GoToAddColorState;

			if (settings.MaxDecorationsCount > 0)
			{
				slimeAddColorState.OnFinish += GoToDecorSelectionState;
			}
			else
			{
				slimeAddColorState.OnFinish += GoToPlaySlimeState;
			}

			decorSelectionState.OnDataSelected += GoToAddDecorationState;
			decorSelectionState.OnFinish += SkipDecorationAdd;
			addDecorationState.OnFinish += OnDecorationStateFinished;
			playState.OnFinish += GoToSaveSlimeState;
			saveSlimeState.OnFinish += FinishSlimeCreation;
		}


		public override void Enter()
		{
			base.Enter();

			HideMainGameUi();
			HideCraftTableEnvironment();
			PrepareSlime();
			ResetDecorationsCount();
			InitializeCreationPanel();
			InitializeCancelPanel();
			InitializeRollback();
			ShowCreationPanel();
			GoToBasisSelectionState();
			UpdateRollbackButtonEnabled();
			slimeFacade.SetDeformationEnabled(true);
		}

		public override void Exit()
		{
			DeInitializeCreationPanel();
			DeInitializeCancelPanel();
			DeInitializeRollback();
			HideCancelPanel();
			HideCreationPanel();

			base.Exit();
		}


		protected override void FinishInner(FinishStatus finishStatus)
		{
			base.FinishInner(finishStatus);
			bowl.SetActive(false);
		}


		private void HideMainGameUi()
		{
			/*if (screenManager.CurrentScreenBehavior is not CraftTableScreen craftTable)
			{
				Debug.LogError("screenManager.CurrentScreenBehavior is not CraftTableScreen");
				return;
			}

			craftTable.HideElements();
			uiManager.HideGuiAll();*/
		}


		private void HideCraftTableEnvironment()
		{
		/*	if (stageManager.CurrentView is not PlayRoomView playRoomView)
				return;

			playRoomView.PlayRoomCarpet.SetActive(false);
			playRoomView.Stool.SetActive(false);
			playRoomView.PlayRoomCraftTableTop.SetActive(false);
			playRoomView.LightWindow.SetActive(false);*/
		}


		private void ResetDecorationsCount()
		{
			decorationsCount = 0;
		}

		private void InitializeCreationPanel()
		{
			panelManager.TryGetPanel(PanelType.SlimeCreationPanel, out Panel panel);
			slimeCreationPanel = panel as SlimeCreationPanel;
			slimeCreationPanel.OnGoBack += Rollback;
			slimeCreationPanel.OnGoToStartScreen += ShowCancelPanel;
		}


		private void InitializeCancelPanel()
		{
			panelManager.TryGetPanel(PanelType.SlimeCancelPanel, out Panel panel);
			slimeCancelPanel = panel as SlimeCancelPanel;
			slimeCancelPanel.OnExitButton += FinishSlimeCreationWithFailure;
			slimeCancelPanel.OnContinueButton += HideCancelPanel;
		}

		private void InitializeRollback()
		{
			rollback.Clear();
			rollback.OnPush += UpdateRollbackButtonEnabled;
		}

		private void Rollback()
		{
			audioService.PlayOneShot(SoundId.shop_close);
			rollback.PerformRollback();
			UpdateRollbackButtonEnabled();
		}

		private void UpdateRollbackButtonEnabled()
		{
			slimeCreationPanel.SetGoBackButtonActive(rollback.StackLength > 0);
		}

		private void ShowCreationPanel()
		{
			panelManager.TryShow(PanelType.SlimeCreationPanel, PanelTransitionType.Override);
		}

		private void GoToBasisSelectionState()
		{
			ResetBowl();
			ResetSlime();
			StateMachine.TryEnter<SlimeBasisSelectionState>();
			SetStepGameType(SlimeCreationStepType.SlimeBasisSelection);
		}

		private void ResetBowl()
		{
			bowl.Fill = 0;
			bowl.SetActive(true);
		}

		private void ResetSlime()
		{
			slimeFacade.RemoveDecorations();
		}

		private void GoToSlimeCreationByRecipe(BasisStaticData basis)
		{
			rollback.PushAction(GoToBasisSelectionState);
			StateMachine.TryEnter<SlimeBasisCreationState, BasisStaticData>(basis);
			SetStepGameType(SlimeCreationStepType.SlimeBasisCreation);
		}

		private void ShowInterstitial()
		{
			adsService.TryShowAd(AdType.Interstitial, null /*, adsService.AdsPlacements.SlimeCreationStage*/);
		}

		private void GoToColorSelectionState()
		{
			ShowInterstitial();

			StateMachine.TryEnter<SlimeColorSelectionState>();
			SetStepGameType(SlimeCreationStepType.SlimeColorSelection);
		}

		private void GoToAddColorState(ColorStaticData colorStaticData)
		{
			Color cachedSlimeColor = slimeFacade.Color;

			rollback.PushAction(() =>
								{
									GoToColorSelectionState();
									slimeFacade.Color = cachedSlimeColor;
								});

			StateMachine.TryEnter<SlimeAddColorState, Color>(colorStaticData.Color);
			SetStepGameType(SlimeCreationStepType.SlimeAddColor);
		}

		private void GoToDecorSelectionState()
		{
			ShowInterstitial();

			StateMachine.TryEnter<SlimeDecorSelectionState>();
			SetStepGameType(SlimeCreationStepType.SlimeDecorSelection, decorationsCount + 1);
		}

		private void GoToAddDecorationState(DecorationStaticData decoration)
		{
			rollback.PushAction(() =>
								{
									slimeFacade.RemoveDecorationGroup(slimeFacade.DecorationsCount - 1);
									GoToDecorSelectionState();
								});

			StateMachine.TryEnter<SlimeAddDecorationState, DecorationStaticData>(decoration);
		}

		private void OnDecorationStateFinished()
		{
			rollback.AddActionToLast(() =>
									 {
										 decorationsCount--;
									 });

			if (++decorationsCount < settings.MaxDecorationsCount)
			{

				GoToDecorSelectionState();

				return;
			}

			for (var i = 0; i < decorationsCount; i++)
			{
				rollback.RemoveLastAction();
			}

			rollback.PushAction(GoToDecorSelectionState);

			SkipDecorationAdd();
		}

		private void SkipDecorationAdd()
		{
			rollback.AddActionToLast(() =>
									 {
										 slimeFacade.RemoveDecorations();
										 ResetDecorationsCount();
									 });

			audioService.PlayClickSound();
			GoToPlaySlimeState();
		}

		private void GoToPlaySlimeState()
		{
			ShowInterstitial();

			StateMachine.TryEnter<SlimeCreationPlayState>();
			SetStepGameType(SlimeCreationStepType.SlimePlay);
		}

		private void GoToSaveSlimeState()
		{
			StateMachine.TryEnter<SaveSlimeState>();
		}

		private void FinishSlimeCreation()
		{
			ShowInterstitial();
			Finish();
		}

		private void FinishSlimeCreationWithFailure()
		{
			HideCancelPanel();
			StateMachine.ActiveState.FinishWithFailure();
			Finish(FinishStatus.Failure);
		}

		private void DeInitializeCreationPanel()
		{
			slimeCreationPanel.OnGoBack -= Rollback;
			slimeCreationPanel.OnGoToStartScreen -= ShowCancelPanel;
		}


		private void DeInitializeCancelPanel()
		{
			slimeCancelPanel.OnExitButton -= FinishSlimeCreationWithFailure;
			slimeCancelPanel.OnContinueButton -= HideCancelPanel;
		}

		private void DeInitializeRollback()
		{
			rollback.OnPush -= UpdateRollbackButtonEnabled;
		}

		private void HideCreationPanel()
		{
			panelManager.HidePanel(PanelType.SlimeCreationPanel, true);
		}

		private void SetStepGameType(SlimeCreationStepType step, params object[] args)
		{
			SlimeCreationStepType = step;
			OnNewCreationStep?.Invoke(this, args);
		}

		private void PrepareSlime()
		{
			stageService.GetStateBehaviour<CraftTableStageBehaviour>(StageType.CraftTable).SetupSlimeInteractionsMode();
			//craftTableStageBehaviour.SetupSlimeInteractionsMode();
			/*slimeFacade.SlimeCameraSettings.SlimeInteractionsCameraSettings
					   .ApplyToCamera(slimeFacade.Camera);*/

			slimeFacade.Show();
		}


		private void ShowCancelPanel()
		{
			creationInput.Reset();
			creationInput.IsEnabled = false;
			slimeFacade.SetInteractable(false);
			panelManager.TryShow(PanelType.SlimeCancelPanel);
		}


		private void HideCancelPanel()
		{
			creationInput.IsEnabled = true;
			slimeFacade.SetInteractable(true);
			panelManager?.HidePanel(PanelType.SlimeCancelPanel, true);
		}
	}
}
