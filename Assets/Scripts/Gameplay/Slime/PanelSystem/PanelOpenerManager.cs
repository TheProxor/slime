using System.Collections.Generic;
using TheProxor.PanelSystem.Openers;


namespace TheProxor.PanelSystem
{
	public class PanelOpenerManager
	{
		private readonly Dictionary<PanelTransitionType, PanelOpener> openerDictionary =
			new Dictionary<PanelTransitionType, PanelOpener>()
			{
				{ PanelTransitionType.Instantly, new InstantlyPanelOpener() },
				{ PanelTransitionType.Consistent, new ConsistentPanelOpener() },
				{ PanelTransitionType.Parallel, new ParallelPanelOpener() },
				{ PanelTransitionType.Reverse, new ReversePanelOpener() },
				{ PanelTransitionType.Override, new OverridePanelController() }
			};

		private PanelOpener currentOpener = null;

		public void SetTransition(
				PanelTransitionType transitionType,
				Panel newPanel,
				IReadOnlyList<Panel> currentPanels
			)
		{
			if (!openerDictionary.TryGetValue(transitionType, out PanelOpener panelOpener))
			{
				return;
			}

			currentOpener = panelOpener;
			currentOpener.SetOpenProcess(newPanel, currentPanels);
		}

		public void UpdateProcessing()
		{
			if (currentOpener == null)
			{
				return;
			}

			currentOpener.TryUpdateProcessing();
			if (currentOpener.IsFinished)
			{
				currentOpener = null;
			}
		}
	}
}
