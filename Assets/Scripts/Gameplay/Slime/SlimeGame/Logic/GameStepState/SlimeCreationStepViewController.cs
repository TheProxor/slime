using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using TheProxor.PanelSystem;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeCreationStepViewController
	{
		private readonly PanelManager panelManager;

		private SlimeCreationStepView slimeCreationStepView;

		private SlimeCreationStepView SlimeCreationStepView
		{
			get
			{
				if (slimeCreationStepView != null)
				{
					return slimeCreationStepView;
				}

				InitializeGameStepView();

				return slimeCreationStepView;
			}
		}

		public SlimeCreationStepViewController(PanelManager panelManager)
		{
			this.panelManager = panelManager;
		}

		public void AddGameStepState(ISlimeCreationStepProvider state)
		{
			state.OnNewCreationStep += UpdateView;
		}

		private void InitializeGameStepView()
		{
			SlimeCreationPanel slimeCreationPanel = GetSlimeCreationPanel();
			slimeCreationStepView = slimeCreationPanel.SlimeCreationStateView;
		}

		private SlimeCreationPanel GetSlimeCreationPanel()
		{
			panelManager.TryGetPanel(PanelType.SlimeCreationPanel, out Panel panel);

			return panel as SlimeCreationPanel;
		}

		private void UpdateView(ISlimeCreationStepProvider slimeCreationStepProvider, params object[] args)
		{
			SlimeCreationStepView.SetStepType(slimeCreationStepProvider.SlimeCreationStepType, args);
		}
	}
}
