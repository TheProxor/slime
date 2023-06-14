using UnityEngine;
using Zenject;


namespace TheProxor.Services.UI
{
	public class WindowsFactory : IFactory<Object, Transform, Window>
	{
		private readonly DiContainer container;



		public WindowsFactory(DiContainer container)
		{
			this.container = container;
		}



		public Window Create(Object prefab, Transform parent)
		{
			Window window = container.InstantiatePrefab(prefab, parent)
									 .GetComponent<Window>();

			window.gameObject.SetActive(false);

			return window;
		}
	}
}
