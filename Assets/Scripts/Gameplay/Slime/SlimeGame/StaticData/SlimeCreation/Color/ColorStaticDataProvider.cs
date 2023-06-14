// using TakeTop.AssetProvider;

using TheProxor.Logic.AssetProvider;
using TheProxor.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class ColorStaticDataProvider : StaticDataProvider<string, ColorStaticData>
	{
		public ColorStaticDataProvider(string path, IAssetProvider assetProvider) :
			base(path, assetProvider) {}
	}
}
