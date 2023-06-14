using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Ads;
using TheProxor.Services.Audio;
using TheProxor.Services.UI;
using UnityEngine;
using PanelType = TheProxor.PanelSystem.PanelType;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeDecorSelectionState
		: DataSelectionState<DecorationStaticData, SlimeDecorationInfo, SlimeDecorSelectionPanel>
	{
		private readonly DecorStaticDataProvider provider;
		private readonly SlimeFacade slimeFacade;

		private List<(DecorationStaticData, SlimeDecorationInfo)> data;
		private SlimeAdsIngredientPanel slimeAdsIngredientPanel;


		public override PanelSystem.PanelType PanelType => PanelSystem.PanelType.SlimeDecorSelectionPanel;


		public SlimeDecorSelectionState(DecorStaticDataProvider provider,
										ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager,
										IWindowsService windowsService,
										IAudioService<SoundId> audioService,
										IAdsService<AdType> adsService,
										PanelManager panelManager
			) : base(panelManager, slimeIngredientsManager, windowsService, audioService, adsService)
		{
			this.provider = provider;
		}


		public override void Enter()
		{
			base.Enter();
			InitializeAdsIngredients();
			RefreshWastedDecorations();
			SelectionPanel.RefreshVisual();
		}


		public override void Exit()
		{
			base.Exit();
			DeinitializeAdsIngredients();
		}


		protected override void InitializeSelectionPanel(SlimeDecorSelectionPanel panel)
		{
			base.InitializeSelectionPanel(panel);

			ICollection<SlimeDecorationInfo> slimeDecorationInfos = slimeIngredientsManager.GetIngredientsCollection<SlimeDecorationInfo>();

			data = new List<(DecorationStaticData, SlimeDecorationInfo)>();

			foreach (var decorationInfo in slimeDecorationInfos)
			{
				if (slimeFacade.DecorationsCount == 0)
					decorationInfo.WastedDecorationsCount = 0;

				data.Add((decorationInfo.DecorationStaticData, decorationInfo));
			}

			panel.Initialize(data);
			panel.OnDecorSkip += OnDecorSkip;
		}


		protected override void Select(DecorationStaticData staticData, SlimeDecorationInfo slimeDecorationInfo, int direction)
		{
			int count = slimeDecorationInfo.Count - slimeDecorationInfo.WastedDecorationsCount;
			bool isActive = count > 0 || slimeDecorationInfo.isAdsIngredient;

			if (!isActive)
			{
				audioService.PlayClickSound();
				return;
			}

			if (!slimeDecorationInfo.isAdsIngredient)
			{
				base.Select(staticData, slimeDecorationInfo, direction);
				slimeDecorationInfo.WastedDecorationsCount++;
				audioService.PlayOneShot(SoundId.slime_adds_list_choose);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
				return;
			}

			audioService.PlayClickSound();
			panelManager.TryShow(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}


		private void RefreshWastedDecorations()
		{
			foreach (var item in data)
			{
				if (slimeFacade.DecorationsCount == 0)
					item.Item2.WastedDecorationsCount = 0;
			}
		}


		private void InitializeAdsIngredients()
		{
			List<(DecorationStaticData, SlimeDecorationInfo)> availableForAdsData = new(data.Count);

			foreach (var item in data)
			{
				item.Item2.isAdsIngredient = false;

				if (item.Item2.Count <= 0)
					availableForAdsData.Add(item);
			}

			if (availableForAdsData.Count == 0)
				return;

			int randomIndex = Random.Range(0, availableForAdsData.Count);
			(DecorationStaticData, SlimeDecorationInfo) randomData = availableForAdsData[randomIndex];

			randomData.Item2.isAdsIngredient = true;
			panelManager.TryGetPanel(PanelSystem.PanelType.SlimeAdsIngredientPanel, out Panel panel);

			slimeAdsIngredientPanel = panel as SlimeAdsIngredientPanel;
			slimeAdsIngredientPanel.SetupSlimeIngredientInfo(randomData.Item2);
			slimeAdsIngredientPanel.SetupIngredientPreview(randomData.Item1.Icon);
			slimeAdsIngredientPanel.OnAdsButton += OnAdsButton;
			slimeAdsIngredientPanel.OnCloseButton += OnCloseButton;
		}


		private void DeinitializeAdsIngredients()
		{
			if (slimeAdsIngredientPanel == null)
				return;

			slimeAdsIngredientPanel.OnAdsButton -= OnAdsButton;
			slimeAdsIngredientPanel.OnCloseButton -= OnCloseButton;

			foreach (var item in data)
				item.Item2.isAdsIngredient = false;
		}


		private void OnAdsButton(SlimeIngredientInfo slimeIngredientInfo)
		{
			var isSecondDecoration = slimeFacade.DecorationsCount > 0;
			/*var placement = slimeFacade.DecorationsCount switch
			{
				0 => adsManager.AdsPlacements.SlimeAdsDecoration,
				> 0 => adsManager.AdsPlacements.SlimeAdsDecoration2,
				_ => adsManager.AdsPlacements.None
			};*/

			adsService.TryShowAd(AdType.Rewarded, OnResult/*, placement*/);


			void OnResult(bool result)
			{
				if (!result)
					return;

				slimeIngredientsManager.AddIngredient(slimeIngredientInfo.Id, 1);
				DeinitializeAdsIngredients();

				SelectionPanel.RefreshVisual(false);

				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);

				audioService.PlayClickSound();

				SlimeDecorationInfo slimeDecorationInfo = slimeIngredientInfo as SlimeDecorationInfo;
				Select(slimeDecorationInfo.DecorationStaticData, slimeDecorationInfo, 0);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeDecorSelectionPanel);
			}
		}


		private void OnCloseButton()
		{
			audioService.PlayOneShot(SoundId.shop_close);
			panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}


		private void OnDecorSkip()
		{
			Finish();
		}
	}
}
