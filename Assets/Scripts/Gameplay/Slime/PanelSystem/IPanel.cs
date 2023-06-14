using System;

namespace TheProxor.PanelSystem
{
	public interface IPanel
	{
		event Action<Panel> OnBeginShow;
		event Action<Panel> OnBeginHide;
		event Action<Panel> OnEndShow;
		event Action<Panel> OnEndHide;

		PanelState State { get; }
		PanelType PanelType { get; }
		PanelType SubstitutedPanel { get; }
		PanelTransitionInfo NextPanelInfo { get; }

		void Show(bool instantly);
		void Hide(bool instantly);
		void ResetNext();
		void SetSubstitutedPanel(PanelType value);
		void OpenSubstitutedPanel(PanelTransitionType transitionType);
	}
}
