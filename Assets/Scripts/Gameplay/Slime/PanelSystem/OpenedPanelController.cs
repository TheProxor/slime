using System.Collections.Generic;

namespace TheProxor.PanelSystem
{
	public class OpenedPanelController
	{
		private readonly List<Panel> allPanels = new List<Panel>();
		private readonly List<Panel> openedPanels = new List<Panel>();

		public IReadOnlyList<Panel> OpenedPanels => openedPanels;

		public void Setup(IEnumerable<Panel> panels)
		{
			allPanels.AddRange(panels);
			foreach (Panel panel in allPanels)
			{
				if (panel.gameObject.activeInHierarchy)
				{
					openedPanels.Add(panel);
				}

				panel.OnBeginShow += BeginShowHandler;
				panel.OnEndHide += EndHideHandler;
			}
		}

		public void Uninstall()
		{
			foreach (Panel panel in allPanels)
			{
				panel.OnBeginShow -= BeginShowHandler;
				panel.OnEndHide -= EndHideHandler;
			}

			allPanels.Clear();
		}

		public bool TryGetLastPanel(out Panel lastPanel)
		{
			int count = openedPanels.Count;
			bool hasPanel = count > 0;
			lastPanel = hasPanel ? openedPanels[count - 1] : null;

			return hasPanel;
		}

		internal void UpdateProcessing()
		{
			for (int i = openedPanels.Count - 1; i >= 0; i--)
			{
				openedPanels[i].Processing();
			}
		}

		private void BeginShowHandler(Panel panel)
		{
			if (!openedPanels.Contains(panel))
			{
				openedPanels.Add(panel);
			}
		}

		private void EndHideHandler(Panel panel)
		{
			openedPanels.Remove(panel);
		}
	}
}
