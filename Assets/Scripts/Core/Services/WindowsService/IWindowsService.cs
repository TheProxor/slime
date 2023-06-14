using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheProxor.Services.UI
{
	public interface IWindowsService
	{
		event Action<IWindow> OnShowWindowBegin;
		event Action<IWindow> OnShowWindowEnd;
		event Action<IWindow> OnHideWindowBegin;
		event Action<IWindow> OnHideWindowEnd;



		IReadOnlyCollection<IWindow> OpenedWindows { get; }
		IWindow ActiveWindow { get; }
		Canvas RootCanvas { get; }



		bool TrySwapWindowFromTo(
				IWindow windowFrom,
				IWindow windowTo,
				IWindowTransitionController transitionController
			);


		bool TrySwapWindowFromTo<TWindowFrom, TWindowTo, TTransitionController>(out TWindowTo resultWindow)
			where TWindowFrom : IWindow
			where TWindowTo : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TrySwapWindowFromTo<TWindowFrom, TWindowTo, TTransitionController>()
			where TWindowFrom : IWindow
			where TWindowTo : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TryShowWindow(
				IWindow window,
				IWindowTransitionController transitionController
			);


		bool TryShowWindow<TWindow, TTransitionController>(out TWindow resultWindow)
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TryShowWindow<TWindow, TTransitionController>()
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TryHideWindow(
				IWindow window,
				IWindowTransitionController transitionController
			);


		bool TryHideWindow<TWindow, TTransitionController>(out TWindow resultWindow)
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TryHideWindow<TWindow, TTransitionController>()
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController;


		bool TryGetWindow<TWindow>(out IWindow window)
			where TWindow : IWindow;


		bool TryGetTransitionController<TTransitionController>(out IWindowTransitionController transitionController)
			where TTransitionController : IWindowTransitionController;
	}
}
