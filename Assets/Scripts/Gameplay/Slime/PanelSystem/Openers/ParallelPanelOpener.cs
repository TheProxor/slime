namespace TheProxor.PanelSystem.Openers
{
	public class ParallelPanelOpener : PanelOpener
	{
		protected override void UpdateProcessing()
		{
			foreach (Panel panel in HiddenPanels)
			{
				panel.Hide(false);
			}

			NewPanel.Show(false);
			IsFinished = true;
		}
	}
}
