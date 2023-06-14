using System.Collections.Generic;

namespace TheProxor.PanelSystem
{
	public abstract class PanelOpener
	{
		private readonly List<Panel> hiddenPanels = new List<Panel>();

		public bool IsFinished { get; protected set; }
		protected Panel NewPanel { get; private set; }

		protected IReadOnlyList<Panel> HiddenPanels
		{
			get => hiddenPanels;
			private set => SetHiddenPanels(value, hiddenPanels);
		}

		protected virtual void SetHiddenPanels(IReadOnlyList<Panel> value,
											   List<Panel> hiddenPanelsCollection)
		{
			hiddenPanelsCollection.Clear();
			hiddenPanelsCollection.AddRange(value);
			hiddenPanelsCollection.Remove(NewPanel);
		}

		public void SetOpenProcess(Panel newPanel, IReadOnlyList<Panel> currentPanels)
		{
			NewPanel = newPanel;
			HiddenPanels = currentPanels;
			IsFinished = false;
		}

		public void TryUpdateProcessing()
		{
			if (IsFinished)
			{
				return;
			}

			UpdateProcessing();
		}

		protected abstract void UpdateProcessing();
	}
}
