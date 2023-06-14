using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;


namespace TheProxor.Services.UI
{
	public class WindowsService : IWindowsService
	{
		public event Action<IWindow> OnShowWindowBegin;
		public event Action<IWindow> OnShowWindowEnd;

		public event Action<IWindow> OnHideWindowBegin;
		public event Action<IWindow> OnHideWindowEnd;



		private readonly Dictionary<Type, IWindow> windows;
		private readonly Dictionary<Type, IWindowTransitionController> transitionControllers;
		private readonly IFactory<Object, Transform, Window> windowsFactory;
		private readonly Settings settings;
		private readonly List<IWindow> openedWindows;



		public IWindow ActiveWindow
		{
			get
			{
				if (openedWindows == null || openedWindows.Count == 0)
					return null;

				return openedWindows[^1];
			}
		}

		public IReadOnlyCollection<IWindow> OpenedWindows => openedWindows;

		public Canvas RootCanvas { get; private set; }



		public WindowsService(Settings settings,
							  ICollection<IWindowTransitionController> transitionControllers,
							  IFactory<Object, Transform, Window> windowsFactory)
		{
			this.settings = settings;
			this.windowsFactory = windowsFactory;

			windows = new Dictionary<Type, IWindow>();
			openedWindows = new List<IWindow>(settings.Windows.Length);

			this.transitionControllers = new Dictionary<Type, IWindowTransitionController>(transitionControllers.Count);

			foreach (var transitionController in transitionControllers)
				this.transitionControllers.Add(transitionController.GetType(), transitionController);

			RootCanvas = CreateRootCanvas();
			TryCreateEventSystem();
			GenerateCanvasWindows(RootCanvas);
		}


		public virtual bool TrySwapWindowFromTo(
				IWindow windowFrom,
				IWindow windowTo,
				IWindowTransitionController transitionController
			)
		{
			int windowFromIndex = openedWindows.FindIndex(x => x == windowFrom);

			if (windowFromIndex < 0)
			{
				Debug.LogError($"Window with type <b>{windowFrom.GetType()}</b> not yet opened!");
				return false;
			}

			openedWindows[windowFromIndex] = windowTo;

			if (windowFrom.OverrideSortingOrder)
				windowFrom.SetSortingOrder(openedWindows.Count * settings.SortingOffset);

			transitionController.PlayTransitionFromTo(windowFrom,
				windowTo,
				() => OnShowWindowBegin?.Invoke(windowTo),
				() => OnShowWindowEnd?.Invoke(windowTo),
				() => OnHideWindowBegin?.Invoke(windowFrom),
				() => OnHideWindowEnd?.Invoke(windowFrom));

			return true;
		}


		public virtual bool TrySwapWindowFromTo<TWindowFrom, TWindowTo, TTransitionController>(out TWindowTo resultWindow)
			where TWindowFrom : IWindow
			where TWindowTo : IWindow
			where TTransitionController : IWindowTransitionController
		{
			resultWindow = default;

			if (!TryGetWindow<TWindowFrom>(out IWindow windowFrom))
				return false;

			if (!TryGetWindow<TWindowFrom>(out IWindow windowTo))
				return false;

			if (!TryGetTransitionController<TTransitionController>(out IWindowTransitionController transitionController))
				return false;

			resultWindow = (TWindowTo)windowTo;
			return TrySwapWindowFromTo(windowFrom, windowTo, transitionController);
		}


		public virtual bool TrySwapWindowFromTo<TWindowFrom, TWindowTo, TTransitionController>()
			where TWindowFrom : IWindow
			where TWindowTo : IWindow
			where TTransitionController : IWindowTransitionController =>
			TrySwapWindowFromTo<TWindowFrom, TWindowTo, TTransitionController>(out _);


		public virtual bool TryShowWindow(
				IWindow window,
				IWindowTransitionController transitionController
			)
		{
			openedWindows.Add(window);

			if (window.OverrideSortingOrder)
				window.SetSortingOrder(openedWindows.Count * settings.SortingOffset);

			transitionController.PlayTransitionFromTo(null,
				window,
				() => OnShowWindowBegin?.Invoke(window),
				() => OnShowWindowEnd?.Invoke(window));

			return true;
		}


		public virtual bool TryShowWindow<TWindow, TTransitionController>(out TWindow resultWindow)
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController
		{
			resultWindow = default;

			if (!TryGetWindow<TWindow>(out IWindow window))
				return false;

			if (!TryGetTransitionController<TTransitionController>(out IWindowTransitionController transitionController))
				return false;

			resultWindow = (TWindow)window;
			return TryShowWindow(window, transitionController);
		}


		public virtual bool TryShowWindow<TWindow, TTransitionController>()
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController =>
			TryShowWindow<TWindow, TTransitionController>(out _);


		public virtual bool TryHideWindow(
				IWindow window,
				IWindowTransitionController transitionController
			)
		{
			int windowIndex = openedWindows.FindIndex(x => x == window);

			if (windowIndex < 0)
			{
				Debug.LogError($"Window with type <b>{window.GetType()}</b> not yet opened!");
				return false;
			}

			openedWindows.RemoveAt(windowIndex);

			if (window.OverrideSortingOrder)
				window.SetSortingOrder(openedWindows.Count * settings.SortingOffset);

			transitionController.PlayTransitionFromTo(window,
				null,
				null,
				null,
				() => OnHideWindowBegin?.Invoke(window),
				() => OnHideWindowEnd?.Invoke(window));

			return true;
		}


		public virtual bool TryHideWindow<TWindow, TTransitionController>(out TWindow resultWindow)
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController
		{
			resultWindow = default;

			if (!TryGetWindow<TWindow>(out IWindow window))
				return false;

			if (!TryGetTransitionController<TTransitionController>(out IWindowTransitionController transitionController))
				return false;

			resultWindow = (TWindow)window;
			return TryHideWindow(window, transitionController);
		}


		public virtual bool TryHideWindow<TWindow, TTransitionController>()
			where TWindow : IWindow
			where TTransitionController : IWindowTransitionController =>
			TryHideWindow<TWindow, TTransitionController>(out _);


		public bool TryGetWindow<TWindow>(out IWindow window)
			where TWindow : IWindow
		{
			if (!windows.TryGetValue(typeof(TWindow), out window))
			{
				Debug.LogError($"Window with type <b>{typeof(TWindow)}</b> does not exists!");
				return false;
			}

			return true;
		}


		public bool TryGetTransitionController<TTransitionController>(out IWindowTransitionController transitionController)
			where TTransitionController : IWindowTransitionController
		{
			if (!transitionControllers.TryGetValue(typeof(TTransitionController), out transitionController))
			{
				Debug.LogError($"Transition Controller with type <b>{typeof(TTransitionController)}</b> does not exists!");
				return false;
			}

			return true;
		}


		private Canvas CreateRootCanvas()
		{
			GameObject rootObject = GameObject.Find(settings.SceneRootName);

			if (rootObject == null)
				rootObject = new GameObject(settings.SceneRootName);

			Canvas canvas = new GameObject($"{nameof(Canvas)}", typeof(Canvas), typeof(CanvasScaler)).GetComponent<Canvas>();
			canvas.transform.parent = rootObject.transform;
			canvas.renderMode = settings.CanvasRenderMode;
			canvas.planeDistance = settings.PlaneDistance;
			canvas.worldCamera = UnityEngine.Camera.main;

			var canvasScaler = canvas.GetComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = settings.CanvasScalerBaseResolution;
			canvasScaler.matchWidthOrHeight = 0.5f;

			return canvas;
		}


		private void TryCreateEventSystem()
		{
			if (Object.FindObjectOfType<EventSystem>() == null)
			{
				var eventSystem = new GameObject($"{nameof(EventSystem)}", typeof(EventSystem), typeof(StandaloneInputModule));
			}
		}


		private void GenerateCanvasWindows(Canvas rootCanvas)
		{
			foreach (var canvasWindowPrefab in settings.Windows)
			{
				Window window = windowsFactory.Create(canvasWindowPrefab.gameObject, rootCanvas.transform);
				windows.Add(window.GetType(), window);
			}
		}


		[Serializable]
		public class Settings
		{
			[Title("General")]
			[field: SerializeField] public string SceneRootName { get; private set; }


			[Title("Windows")]
			[field: SerializeField] public Window[] Windows { get; private set; }


			[Title("Settings")]
			[field: SerializeField] public Vector2Int CanvasScalerBaseResolution { get; private set; } = new(1080, 1920);
			[field: SerializeField] public RenderMode CanvasRenderMode { get; private set; }
			[field: SerializeField] public int SortingOffset { get; private set; } = 100;

			[field: SerializeField, ShowIf(nameof(CanvasRenderMode), RenderMode.ScreenSpaceCamera)]
			public float PlaneDistance { get; private set; } = 0.3f;
		}
	}
}
