using System;


namespace TheProxor.Services.UI
{
	public interface IWindowTransitionController
	{
		void PlayTransitionFromTo(
				IWindow source,
				IWindow destination,
				Action onShowBeginCallback = null,
				Action onShowEndCallback = null,
				Action onHideBeginCallback = null,
				Action onHideEndCallback = null
			);
	}
}
