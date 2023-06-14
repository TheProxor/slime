// using TakeTop.AssetProvider;

using TheProxor.Logic.AssetProvider;
using TheProxor.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class SubStateStaticDataProvider :
		StaticDataProvider<SubStateType, SubStateStaticData>
	{
		public SubStateStaticDataProvider(string path, IAssetProvider assetProvider) :
			base(path, assetProvider) {}
	}
}
