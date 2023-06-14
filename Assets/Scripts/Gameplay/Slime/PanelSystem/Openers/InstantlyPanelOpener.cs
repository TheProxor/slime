namespace TheProxor.PanelSystem.Openers
{
	public class InstantlyPanelOpener : PanelOpener
	{
		protected override void UpdateProcessing()
		{
			foreach (Panel panel in HiddenPanels)
			{
				panel.Hide(true);
			}

			NewPanel.Show(true);
		}
	}
}
