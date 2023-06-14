using System;


namespace TheProxor.Services.UI
{
	public class ImmediatelyTransitionController : IWindowTransitionController
	{
		public void PlayTransitionFromTo(IWindow source,
										 IWindow destination,
										 Action onShowBeginCallback = null,
										 Action onShowEndCallback = null,
										 Action onHideBeginCallback = null,
										 Action onHideEndCallback = null)
		{
			onHideBeginCallback?.Invoke();
			if (source != null)
				source.Hide(true);
			onHideEndCallback?.Invoke();

			onShowBeginCallback?.Invoke();
			if (destination != null)
				destination.Show(true);
			onShowEndCallback?.Invoke();
		}
	}
}
