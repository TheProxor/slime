using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Audio;
using UnityEngine;
using PanelType = TheProxor.PanelSystem.PanelType;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class PlaySlimeState : State
	{
		private const PanelSystem.PanelType PLAY_SLIME_PANEL = PanelType.PlaySlimePanel;

		private readonly PanelManager panelManager;
		private readonly SlimeFacade slimeFacade;
		private readonly BowlFacade bowlFacade;
		private readonly CreationInput input;
		private readonly SlimeMetaGameInstaller.SlimePlayProgressSettings slimePlayProgressSettings;
		private readonly IAudioService<SoundId> audioService;
		private readonly SlimeSaver slimeSaver;

		private PlaySlimePanel playSlimePanel;
		private Guid? loopedSoundGuid;
		private float interactionTimer;


		public PlaySlimeState(PanelManager panelManager,
							  SlimeFacade slimeFacade,
							  BowlFacade bowlFacade,
							  CreationInput input,
							  SlimeMetaGameInstaller.SlimePlayProgressSettings slimePlayProgressSettings,
							  IAudioService<SoundId> audioService,
							  SlimeSaver slimeSaver)
		{
			this.panelManager = panelManager;
			this.slimeFacade = slimeFacade;
			this.bowlFacade = bowlFacade;
			this.input = input;
			this.slimePlayProgressSettings = slimePlayProgressSettings;
			this.audioService = audioService;
			this.slimeSaver = slimeSaver;
		}

		public override void Enter()
		{
			base.Enter();

			/*slimeFacade.SlimeCameraSettings.SlimeInteractionsCameraSettings
					   .ApplyToCamera(slimeFacade.Camera);*/

			HideMainGameUi();
			HideCraftTableEnvironment();
			DeactivateBowl();
			MakeSlimeInteractable();
			InitializePlaySlimePanel();
			slimeFacade.SetDeformationEnabled(true);

			audioService.PlayOneShot(SoundId.window_screen_open);

			input.IsEnabled = true;
			input.OnCursorMoving += OnInteract;
			input.OnInteractionFinish += OnInteractionFinish;

			interactionTimer = 0.0f;

			RefreshSlimePlayProgress();
		}


		public override void Exit()
		{
			base.Exit();
			DeInitializePlaySlimePanel();
			MakeSlimeNotInteractable();
			HidePlaySlimePanel();

			audioService.PlayOneShot(SoundId.shop_close);

			input.OnCursorMoving -= OnInteract;
			input.OnInteractionFinish -= OnInteractionFinish;
			input.IsEnabled = false;
			input.Reset();
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


		private void MakeSlimeInteractable()
		{
			slimeFacade.SetInteractable(true);
		}


		private void HideCraftTableEnvironment()
		{
			/*if (stageManager.CurrentView is not PlayRoomView playRoomView)
				return;

			playRoomView.PlayRoomCarpet.SetActive(false);
			playRoomView.Stool.SetActive(false);
			playRoomView.PlayRoomCraftTableTop.SetActive(false);
			playRoomView.LightWindow.SetActive(false);*/
		}


		private void InitializePlaySlimePanel()
		{
			playSlimePanel = GetPlaySlimePanel();
			playSlimePanel.OnExit += Finish;
			panelManager.TryShow(PLAY_SLIME_PANEL);
		}


		private PlaySlimePanel GetPlaySlimePanel()
		{
			panelManager.TryGetPanel(PLAY_SLIME_PANEL, out Panel panel);

			return panel as PlaySlimePanel;
		}


		private void DeInitializePlaySlimePanel()
		{
			playSlimePanel.OnExit -= Finish;
		}


		private void MakeSlimeNotInteractable()
		{
			slimeFacade.SetInteractable(false);
		}


		private void HidePlaySlimePanel()
		{
			panelManager.HideCurrentPanel();
		}


		private void DeactivateBowl()
		{
			bowlFacade.Fill = 1;
			bowlFacade.SetActive(false);
		}


		private void OnInteract()
		{
			slimeFacade.SlimePlayProgress += slimePlayProgressSettings.ProgressCurrencyPerTick;
			RefreshSlimePlayProgress();
			SaveSlime();

			if (input.Value > 0)
				BeginInteraction();
			else
				EndInteraction();
		}


		private void OnInteractionFinish() =>
			EndInteraction();


		private void BeginInteraction()
		{
			interactionTimer += Time.deltaTime;

		/*	if (interactionTimer >= miniGameManager.TimeForSlimePlay)
			{
				needsManager.Increase(NeedType.Mood, miniGameManager.MoodForSlimePlay);
				interactionTimer = 0.0f;
			}*/

			if (loopedSoundGuid != null)
				return;

			loopedSoundGuid = audioService.PlaySoundLoop(SoundId.slime_kneading_activator);
		}


		private void EndInteraction()
		{
			if (loopedSoundGuid == null)
				return;

			audioService.Destroy(loopedSoundGuid.Value);
			loopedSoundGuid = null;
		}


		private void SaveSlime()
		{
			slimeSaver.ForceSaveAll();
		}


		private void RefreshSlimePlayProgress() =>
			playSlimePanel.SetProgress(slimeFacade.SlimePlayProgress, slimePlayProgressSettings.ProgressCurrencyMax);
	}
}
