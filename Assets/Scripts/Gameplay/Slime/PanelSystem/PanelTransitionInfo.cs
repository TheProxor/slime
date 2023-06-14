namespace TheProxor.PanelSystem
{
	public struct PanelTransitionInfo
	{
		public PanelType NextPanel;
		public PanelTransitionType TransitionType;

		public PanelTransitionInfo(PanelType nextPanel, PanelTransitionType transitionType)
		{
			NextPanel = nextPanel;
			TransitionType = transitionType;
		}

		public static PanelTransitionInfo Empty =>
			new PanelTransitionInfo(PanelType.None, PanelTransitionType.Consistent);
	}
}
