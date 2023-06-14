using UnityEngine;

namespace TheProxor.Logic.AssetProvider
{
	public class AssetProvider : IAssetProvider
	{
		public T Load<T>(string path) where T : Object
		{
			return Resources.Load<T>(path);
		}
		
		public T[] LoadAll<T>(string path) where T : Object
		{
			return Resources.LoadAll<T>(path);
		}
	}
}
