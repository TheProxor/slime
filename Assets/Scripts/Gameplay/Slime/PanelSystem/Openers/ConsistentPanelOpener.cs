namespace TheProxor.PanelSystem.Openers
{
	public class ConsistentPanelOpener : PanelOpener
	{
		protected override void UpdateProcessing()
		{
			bool allHidden = true;
			foreach (Panel panel in HiddenPanels)
			{
				panel.Hide(false);
				if (panel.State != PanelState.Hidden)
				{
					allHidden = false;

					break;
				}
			}

			if (allHidden)
			{
				NewPanel.Show(false);
				IsFinished = true;
			}
		}
	}
}
