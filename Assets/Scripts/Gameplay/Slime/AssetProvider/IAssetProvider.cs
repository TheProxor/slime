using UnityEngine;

namespace TheProxor.Logic.AssetProvider
{
	public interface IAssetProvider
	{
		T Load<T>(string path) where T : Object;

		T[] LoadAll<T>(string path) where T : Object;
	}
}
