using System;
using System.Collections.Generic;
using DG.Tweening;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.LoadSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Ads;
using TheProxor.Services.Audio;
using TheProxor.Services.Currency;
using TheProxor.Services.UI;
using UnityEngine;
using PanelType = TheProxor.PanelSystem.PanelType;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeSelectionState : DataSelectionState<SlimeSaveData, SlimeIngredientInfo, SlimeSelectionPanel>
	{
		public event Action OnNewSlimeCreateSelected;
		public event Action<SlimeSaveData> OnRecreate;

		private readonly CreationInput input;
		private readonly SlimeSaver slimeSaver;
		private readonly SlimeLoader slimeLoader;
		private readonly BowlFacade bowl;
		private readonly SlimeFacade slimeFacade;
		private readonly IAdsService<AdType> adsManager;
		private readonly ICurrencyService<CurrencyType> currencyService;

		private SlimeLimitExceededPanel slimeLimitExceededPanel;
		private List<(SlimeSaveData, SlimeIngredientInfo)> slimeSaveData;
		private bool isSavedSlimesDataExist;
		private Sequence moveSequence;
		private (float leftPoint, float rightPoint) points;

		public SlimeSaveData SelectedSlimeData { get; private set; }

		public SlimeSelectionState(SlimeFacade slimeFacade,
								   CreationInput input,
								   SlimeSaver slimeSaver,
								   SlimeLoader slimeLoader,
								   BowlFacade bowl,
								   PanelManager panelManager,
								   IAdsService<AdType> adsService,
								   IWindowsService windowsService,
								   IAudioService<SoundId> audioService
			) : base(panelManager, null, windowsService, audioService, adsService)
		{
			this.slimeFacade = slimeFacade;
			this.input = input;
			this.slimeSaver = slimeSaver;
			this.slimeLoader = slimeLoader;
			this.bowl = bowl;

			InitializeSaveData();
			LoadLastSlime();
		}

		public override PanelSystem.PanelType PanelType => PanelSystem.PanelType.SlimeSelectionPanel;


		public override void Enter()
		{
			base.Enter();

			/*slimeFacade.SlimeCameraSettings.SlimeIdleCameraSettings
					   .ApplyToCamera(slimeFacade.Camera);*/

			slimeFacade.RemoveDecorations();

			if (isSavedSlimesDataExist)
				InitializeInput();

			ActivateBowl();

			InitializeMoveAnimation();
			ShowMainGameUi();
			ShowCraftTableEnvironment();
			InitializeSaveData();

			SelectionPanel.Initialize(slimeSaveData);
			slimeFacade.SetDeformationEnabled(false);
		}

		public override void Exit()
		{
			base.Exit();
			moveSequence?.Kill(true);

			/*if (stageManager.CurrentView is PlayRoomView playRoomView)
				playRoomView.LightWindow.SetActive(true);*/

			DeInitializeInput();
		}

		public void ReloadSelectedSlime()
		{
			LoadLastSlime();
		}

		protected override void InitializeSelectionPanel(SlimeSelectionPanel panel)
		{
			slimeSaver.OnNewSlimeSaved += AddSlimeData;
			panel.OnSelected += OnSlimeSelected;
			panel.OnNewSlimeCreateSelected += SelectNewSlimeCreation;
			panel.OnRecreateOnAds += RecreateOnAds;
			panel.OnRecreateOnCurrency += RecreateOnCurrency;
			panel.MaxSlimesCount = slimeFacade.SlimeSettings.MaxSlimesCount;
			panel.RecreationCurrency = slimeFacade.SlimeSettings.RecreationCurrency;

			panel.Initialize(slimeSaveData);
			panel.Refresh();

			InitializeLimitExceededPanel();
		}


		private void InitializeMoveAnimation()
		{
			points.leftPoint = slimeFacade.Transform.position.x - slimeFacade.SlimeSettings.SlimeMoveAnimationOffset;
			points.rightPoint = slimeFacade.Transform.position.x + slimeFacade.SlimeSettings.SlimeMoveAnimationOffset;
		}


		private void InitializeLimitExceededPanel()
		{
			panelManager.TryGetPanel(PanelSystem.PanelType.SlimeLimitExceededPanel, out Panel panel);
			slimeLimitExceededPanel = panel as SlimeLimitExceededPanel;
			slimeLimitExceededPanel.OnCloseButton += OnCloseLimitExceededPanel;
		}


		private void OnCloseLimitExceededPanel()
		{
			audioService.PlayOneShot(SoundId.shop_close);
			panelManager.HidePanel(PanelSystem.PanelType.SlimeLimitExceededPanel);
		}


		private void ShowMainGameUi()
		{
			/*if (screenManager.CurrentScreenBehavior is not CraftTableScreen craftTable)
			{
				Debug.LogError("screenManager.CurrentScreenBehavior is not CraftTableScreen");
				return;
			}

			craftTable.ShowElements();*/
		}


		private void ShowCraftTableEnvironment()
		{
			/*if (stageManager.CurrentView is not PlayRoomView playRoomView)
				return;

			playRoomView.PlayRoomCarpet.SetActive(true);
			playRoomView.Stool.SetActive(true);
			playRoomView.PlayRoomCraftTableTop.SetActive(true);
			playRoomView.LightWindow.SetActive(false);*/
		}


		private void InitializeSaveData()
		{
			this.slimeSaveData = new List<(SlimeSaveData, SlimeIngredientInfo)>();

			foreach (var slimeSaveData in slimeSaver.GetAllSlimes())
			{
				this.slimeSaveData.Add((slimeSaveData, null));
			}

			UpdateSlimeDataExistence(slimeSaveData.Count > 0);

			if (!isSavedSlimesDataExist)
			{
				return;
			}

			UpdateSelectedSlimeData(slimeSaveData[^1].Item1);
		}

		private void AddSlimeData(SlimeSaveData slimeSaveData)
		{
			UpdateSlimeDataExistence(true);
			UpdateSelectedSlimeData(slimeSaveData);
		}

		protected override void SelectInner(SlimeSaveData staticData, SlimeIngredientInfo slimeIngredientInfo)
		{
			base.SelectInner(staticData, slimeIngredientInfo);

			if (string.IsNullOrEmpty(staticData.Id))
			{
				slimeFacade.Hide();
				slimeFacade.RemoveDecorations();
				input.IsEnabled = false;
				return;
			}

			input.IsEnabled = true;
			slimeFacade.Show();
			slimeLoader.Load(staticData);
			UpdateSelectedSlimeData(staticData);
		}

		private void UpdateSlimeDataExistence(bool value)
		{
			isSavedSlimesDataExist = value;
		}

		private void UpdateSelectedSlimeData(SlimeSaveData slimeSaveData)
		{
			SelectedSlimeData = slimeSaveData;
		}

		private void LoadLastSlime()
		{
			slimeLoader.Load(SelectedSlimeData);
		}

		private void InitializeInput()
		{
			input.IsEnabled = true;
			input.OnInteractionStart += StartSlimeInteraction;
		}

		private void DeInitializeInput()
		{
			input.IsEnabled = false;
			input.OnInteractionStart -= StartSlimeInteraction;
		}

		private void StartSlimeInteraction()
		{
			Select(SelectedSlimeData, null, 0);
		}

		private void ActivateBowl()
		{
			bowl.SetActive(true);
			bowl.Fill = 1;
		}

		private void SelectNewSlimeCreation()
		{
			if (SelectionPanel.IsNewSlimeCreationAccepted)
			{
				audioService.PlayClickSound();
				OnNewSlimeCreateSelected?.Invoke();
				return;
			}

			moveSequence?.Kill(true);
			audioService.PlayClickSound(false);
			panelManager.TryShow(PanelSystem.PanelType.SlimeLimitExceededPanel);
		}

		private void RecreateOnAds()
		{
			audioService.PlayClickSound();
			adsManager.TryShowAd(AdType.Rewarded,
				result =>
				{
					if (!result)
						return;

					OnRecreate?.Invoke(SelectedSlimeData);
				}/*, adsManager.AdsPlacements.SlimeRecreation*/);
		}


		private void RecreateOnCurrency()
		{
			input.IsEnabled = false;
			audioService.PlayClickSound();

			if (currencyService.TrySubtractCurrency(slimeFacade.SlimeSettings.RecreationCurrency))
				OnRecreateBuy();
		}


		private void OnRecreateBuy()
		{
			OnRecreate?.Invoke(SelectedSlimeData);
		}


		private void OnSlimeSelected(SlimeSaveData slimeSaveData, SlimeIngredientInfo slimeIngredientInfo, int direction)
		{
			if (direction == 0)
			{
				SelectInner(slimeSaveData, slimeIngredientInfo);
				return;
			}

			audioService.PlayClickSound();
			PlayMoveAnimation(direction < 0? points.leftPoint : points.rightPoint,
				direction < 0 ?  points.rightPoint : points.leftPoint, SelectInnerCallback);


			void SelectInnerCallback() =>
				SelectInner(slimeSaveData, slimeIngredientInfo);
		}


		private void PlayMoveAnimation(float outPoint, float inPoint, TweenCallback callback)
		{
			if (moveSequence.IsActive())
				return;

			moveSequence = DOTween.Sequence();

			moveSequence.Append(slimeFacade.Transform.DOMoveX(outPoint,
				slimeFacade.SlimeSettings.SlimeMoveAnimationDuration / 2).SetEase(Ease.InExpo));

			moveSequence.Join(bowl.Transform.DOMoveX(outPoint,
				slimeFacade.SlimeSettings.SlimeMoveAnimationDuration / 2).SetEase(Ease.InExpo));

			moveSequence.Append(slimeFacade.Transform.DOMoveX(inPoint, 0.0f));
			moveSequence.Join(bowl.Transform.DOMoveX(inPoint, 0.0f));
			moveSequence.AppendCallback(callback);

			moveSequence.Append(slimeFacade.Transform.DOMoveX(slimeFacade.SlimeSettings.SlimeSpawnPosition.x,
				slimeFacade.SlimeSettings.SlimeMoveAnimationDuration / 2).SetEase(Ease.OutExpo));

			moveSequence.Join(bowl.Transform.DOMoveX(slimeFacade.SlimeSettings.SlimeSpawnPosition.x,
				slimeFacade.SlimeSettings.SlimeMoveAnimationDuration / 2).SetEase(Ease.OutExpo));
		}
	}
}
