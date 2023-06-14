using System.Collections.Generic;
using TheProxor.Logic.AssetProvider;


namespace TheProxor.StaticData
{
	public class StaticDataProvider<TType, TStaticData> : IStaticDataProvider<TType, TStaticData>
		where TStaticData : UnityEngine.Object, IStaticData<TType>
	{
		private readonly Dictionary<TType, TStaticData> staticDataByType;

		public IReadOnlyCollection<TStaticData> StaticData { get; }

		public StaticDataProvider(string path, IAssetProvider assetProvider)
		{
			StaticData = assetProvider.LoadAll<TStaticData>(path);

			staticDataByType = new Dictionary<TType, TStaticData>();

			foreach (TStaticData staticData in StaticData)
			{
				staticDataByType.Add(staticData.Type, staticData);
			}
		}

		public TStaticData GetStaticDataForType(TType type) =>
			staticDataByType.TryGetValue(type, out TStaticData staticData)
				? staticData
				: null;
	}
}
