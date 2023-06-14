using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.LoadSystem
{
	public class SlimeLoader
	{
		private readonly SlimeFacade slime;
		private readonly BasisStaticDataProvider basisStaticDataProvider;
		private readonly DecorStaticDataProvider decorStaticDataProvider;

		public SlimeLoader(SlimeFacade slime,
						   BasisStaticDataProvider basisStaticDataProvider,
						   DecorStaticDataProvider decorStaticDataProvider)
		{
			this.slime = slime;
			this.basisStaticDataProvider = basisStaticDataProvider;
			this.decorStaticDataProvider = decorStaticDataProvider;
		}

		public void Load(SlimeSaveData saveData)
		{
			slime.LoadFromData(saveData, basisStaticDataProvider, decorStaticDataProvider);
		}
	}
}
