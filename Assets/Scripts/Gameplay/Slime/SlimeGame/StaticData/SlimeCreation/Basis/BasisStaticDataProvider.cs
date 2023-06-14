// using TakeTop.AssetProvider;

using TheProxor.Logic.AssetProvider;
using TheProxor.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class BasisStaticDataProvider
		: StaticDataProvider<BasisType, BasisStaticData>
	{
		public BasisStaticDataProvider(string path, IAssetProvider assetProvider) :
			base(path, assetProvider) {}
	}
}
