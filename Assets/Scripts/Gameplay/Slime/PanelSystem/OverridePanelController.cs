using System.Collections.Generic;

namespace TheProxor.PanelSystem
{
	public class OverridePanelController : PanelOpener
	{
		protected override void SetHiddenPanels(IReadOnlyList<Panel> value,
												List<Panel> hiddenPanelsCollection)
		{
			hiddenPanelsCollection.Clear();
			hiddenPanelsCollection.AddRange(value);
			int firstRemoveIndex = hiddenPanelsCollection.IndexOf(NewPanel) + 1;
			if (firstRemoveIndex > 0)
			{
				hiddenPanelsCollection.RemoveRange(0, firstRemoveIndex);
			}
		}

		protected override void UpdateProcessing()
		{
			foreach (Panel panel in HiddenPanels)
			{
				panel.ResetNext();
			}

			NewPanel.Show(false);
			IsFinished = true;
		}
	}
}
