namespace TheProxor.PanelSystem.Openers
{
	public class ReversePanelOpener : PanelOpener
	{
		protected override void UpdateProcessing()
		{
			NewPanel.Show(false);
			if (NewPanel.State != PanelState.Shown)
			{
				return;
			}

			foreach (Panel panel in HiddenPanels)
			{
				panel.Hide(false);
			}

			IsFinished = true;
		}
	}
}
