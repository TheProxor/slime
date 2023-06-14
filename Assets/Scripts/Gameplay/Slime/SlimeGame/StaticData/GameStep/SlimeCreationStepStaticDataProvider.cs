// using TakeTop.AssetProvider;

using TheProxor.Logic.AssetProvider;
using TheProxor.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class SlimeCreationStepStaticDataProvider :
		StaticDataProvider<SlimeCreationStepType, SlimeCreationStepStaticData>
	{
		public SlimeCreationStepStaticDataProvider(string path, IAssetProvider assetProvider) :
			base(path, assetProvider) {}
	}
}
