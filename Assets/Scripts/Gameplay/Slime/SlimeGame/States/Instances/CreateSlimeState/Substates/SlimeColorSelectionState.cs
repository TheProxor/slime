using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
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
	public class SlimeColorSelectionState :
		DataSelectionState<ColorStaticData, SlimeColorInfo, SlimeColorSelectionPanel>
	{
		private readonly ColorStaticDataProvider provider;

		private List<(ColorStaticData, SlimeColorInfo)> data;
		private SlimeAdsIngredientPanel slimeAdsIngredientPanel;


		public override PanelSystem.PanelType PanelType => PanelSystem.PanelType.SlimeColorSelectionPanel;


		public SlimeColorSelectionState(
				ColorStaticDataProvider provider,
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
			SelectionPanel.RefreshVisual();
		}


		public override void Exit()
		{
			base.Exit();
			DeinitializeAdsIngredients();
		}


		protected override void InitializeSelectionPanel(SlimeColorSelectionPanel panel)
		{
			base.InitializeSelectionPanel(panel);

			ICollection<SlimeColorInfo> slimeBasisInfos = slimeIngredientsManager.GetIngredientsCollection<SlimeColorInfo>();

			data = new List<(ColorStaticData, SlimeColorInfo)>();

			foreach (var basisInfo in slimeBasisInfos)
			{
				data.Add((basisInfo.ColorStaticData, basisInfo));
			}

			panel.Initialize(data);
		}


		protected override void Select(ColorStaticData staticData, SlimeColorInfo slimeDecorationInfo, int direction)
		{
			int count = slimeDecorationInfo.Count;
			bool isActive = count > 0 || slimeDecorationInfo.isAdsIngredient;

			if (!isActive)
			{
				audioService.PlayClickSound();
				return;
			}

			if (!slimeDecorationInfo.isAdsIngredient)
			{
				base.Select(staticData, slimeDecorationInfo, direction);
				audioService.PlayOneShot(SoundId.slime_button_colour);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
				return;
			}

			audioService.PlayClickSound();
			panelManager.TryShow(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}


		private void InitializeAdsIngredients()
		{
			List<(ColorStaticData, SlimeColorInfo)> availableForAdsData = new(data.Count);

			foreach (var item in data)
			{
				item.Item2.isAdsIngredient = false;

				if (item.Item2.Count <= 0)
					availableForAdsData.Add(item);
			}

			if (availableForAdsData.Count == 0)
				return;

			int randomIndex = Random.Range(0, availableForAdsData.Count);
			(ColorStaticData, SlimeColorInfo) randomData = availableForAdsData[randomIndex];

			randomData.Item2.isAdsIngredient = true;
			panelManager.TryGetPanel(PanelSystem.PanelType.SlimeAdsIngredientPanel, out Panel panel);

			slimeAdsIngredientPanel = panel as SlimeAdsIngredientPanel;
			slimeAdsIngredientPanel.SetupSlimeIngredientInfo(randomData.Item2);
			slimeAdsIngredientPanel.SetupJarPaintIngredientPreview(randomData.Item1.Color);
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
			adsService.TryShowAd(AdType.Rewarded, result =>
			{
				if (!result)
					return;

				slimeIngredientsManager.AddIngredient(slimeIngredientInfo.Id, 1);
				DeinitializeAdsIngredients();

				SelectionPanel.RefreshVisual(false);

				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);

				audioService.PlayClickSound();

				SlimeColorInfo slimeColorInfo = slimeIngredientInfo as SlimeColorInfo;
				Select(slimeColorInfo.ColorStaticData, slimeColorInfo, 0);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeColorSelectionPanel);
			}/*, adsManager.AdsPlacements.SlimeAdsColor*/);
		}


		private void OnCloseButton()
		{
			audioService.PlayOneShot(SoundId.shop_close);
			panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}
	}
}
