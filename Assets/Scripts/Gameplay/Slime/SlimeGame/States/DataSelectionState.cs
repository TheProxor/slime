using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;
using TheProxor.Services.Ads;
using TheProxor.Services.Audio;
using TheProxor.Services.UI;
using PanelType = TheProxor.PanelSystem.PanelType;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public abstract class DataSelectionState<TData, TDataInfo, TDataSelectionPanel> : State
		where TDataSelectionPanel : DataSelectionPanel<TData, TDataInfo>
	{
		public event Action<TData> OnDataSelected;


		private bool isInitialized;
		protected readonly PanelManager panelManager;
		protected readonly ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager;
		protected readonly IWindowsService windowsService;
		protected readonly IAudioService<SoundId> audioService;
		protected readonly IAdsService<AdType> adsService;



		public abstract PanelSystem.PanelType PanelType { get; }
		protected TDataSelectionPanel SelectionPanel { get; private set; }

		protected DataSelectionState(PanelManager panelManager,
									 ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager,
									 IWindowsService windowsService,
									 IAudioService<SoundId> audioService,
									 IAdsService<AdType> adsService)
		{
			this.panelManager = panelManager;
			this.slimeIngredientsManager = slimeIngredientsManager;
			this.windowsService = windowsService;
			this.audioService = audioService;
			this.adsService = adsService;
		}


		public override void Enter()
		{
			InitializeSelectionPanel();
			panelManager.TryShow(PanelType, PanelTransitionType.Override);
		}


		public override void Exit()
		{
			panelManager.HideCurrentPanel();
		}


		protected virtual void InitializeSelectionPanel(TDataSelectionPanel panel)
		{
			SelectionPanel.OnPageChanged += OnPageChanged;
			SelectionPanel.OnSelected += Select;
		}


		protected virtual void Select(TData staticData, TDataInfo slimeDecorationInfo, int direction)
		{
			SelectInner(staticData, slimeDecorationInfo);
			OnDataSelected?.Invoke(staticData);
		}


		protected virtual void SelectInner(TData staticData, TDataInfo dataInfo) {}


		private void InitializeSelectionPanel()
		{
			if (isInitialized)
			{
				return;
			}

			if (!panelManager.TryGetPanel(PanelType, out Panel panel))
			{
				return;
			}

			SelectionPanel = panel as TDataSelectionPanel;

			if (SelectionPanel == null)
			{
				return;
			}

			InitializeSelectionPanel(SelectionPanel);
			isInitialized = true;
		}


		private void OnPageChanged() =>
			audioService.PlayClickSound();
	}
}
