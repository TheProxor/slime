using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Ads;
using TheProxor.Services.Audio;
using TheProxor.Services.UI;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimeBasisSelectionState :
		DataSelectionState<BasisStaticData, SlimeBasisInfo, SlimeBasisSelectionPanel>
	{
		private readonly BasisStaticDataProvider provider;


		private List<(BasisStaticData, SlimeBasisInfo)> data;
		private SlimeAdsIngredientPanel slimeAdsIngredientPanel;


		public override PanelSystem.PanelType PanelType => PanelSystem.PanelType.SlimeBasisSelectionPanel;


		public SlimeBasisSelectionState(
				BasisStaticDataProvider provider,
				ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager,
				PanelManager panelManager,
				IAudioService<SoundId> audioService,
				IWindowsService windowsService,
				IAdsService<AdType> adsService)
			: base(panelManager, slimeIngredientsManager, windowsService, audioService, adsService)
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


		protected override void InitializeSelectionPanel(SlimeBasisSelectionPanel panel)
		{
			base.InitializeSelectionPanel(panel);

			ICollection<SlimeBasisInfo> slimeBasisInfos = slimeIngredientsManager.GetIngredientsCollection<SlimeBasisInfo>();

			data = new List<(BasisStaticData, SlimeBasisInfo)>();

			foreach (var basisInfo in slimeBasisInfos)
			{
				data.Add((basisInfo.BasisStaticData, basisInfo));
			}

			panel.Initialize(data);
		}


		protected override void Select(BasisStaticData staticData, SlimeBasisInfo slimeDecorationInfo, int direction)
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
				audioService.PlayOneShot(SoundId.slime_main_list_choose);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
				return;
			}

			audioService.PlayClickSound();
			panelManager.TryShow(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}


		private void InitializeAdsIngredients()
		{
			List<(BasisStaticData, SlimeBasisInfo)> availableForAdsData = new(data.Count);

			foreach (var item in data)
			{
				item.Item2.isAdsIngredient = false;

				if (item.Item2.Count <= 0)
					availableForAdsData.Add(item);
			}

			if (availableForAdsData.Count == 0)
				return;

			int randomIndex = Random.Range(0, availableForAdsData.Count);
			(BasisStaticData, SlimeBasisInfo) randomData = availableForAdsData[randomIndex];

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
			adsService.TryShowAd(AdType.Rewarded, result =>
			{
				if (!result)
					return;

				slimeIngredientsManager.AddIngredient(slimeIngredientInfo.Id, 1);
				DeinitializeAdsIngredients();

				SelectionPanel.RefreshVisual(false);

				panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
				audioService.PlayClickSound();

				SlimeBasisInfo slimeBasisInfo = slimeIngredientInfo as SlimeBasisInfo;
				Select(slimeBasisInfo.BasisStaticData, slimeBasisInfo, 0);
				panelManager.HidePanel(PanelSystem.PanelType.SlimeBasisSelectionPanel);
			}/*, adsService.AdsPlacements.SlimeAdsBasis*/);
		}


		private void OnCloseButton()
		{
			audioService.PlayOneShot(SoundId.shop_close);
			panelManager.HidePanel(PanelSystem.PanelType.SlimeAdsIngredientPanel);
		}
	}
}
