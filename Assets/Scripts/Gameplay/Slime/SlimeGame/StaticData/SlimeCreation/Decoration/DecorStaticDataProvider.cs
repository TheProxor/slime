// using TakeTop.AssetProvider;

using TheProxor.Logic.AssetProvider;
using TheProxor.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class DecorStaticDataProvider : StaticDataProvider<string, DecorationStaticData>
	{
		public DecorStaticDataProvider(string path, IAssetProvider assetProvider) :
			base(path, assetProvider) {}
	}
}
