using System.Collections.Generic;

namespace TheProxor.StaticData
{
	public interface IStaticDataProvider<in TType, out TStaticData>
		where TStaticData : IStaticData<TType>
	{
		IReadOnlyCollection<TStaticData> StaticData { get; }

		TStaticData GetStaticDataForType(TType type);
	}
}
