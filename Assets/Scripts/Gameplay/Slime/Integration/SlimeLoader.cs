using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;


//TODO: remake
public class SlimeGameLoader
{
	private readonly SlimeFactory factory;

	private SlimeMetaGame slime;


	public SlimeGameLoader(SlimeFactory factory)
	{
		this.factory = factory;
	}


	public void Initialize()
	{
		slime = factory.Create();
		slime.Initialize();
	}


	public void Dispose()
	{
		slime.Dispose();
		slime = null;
		factory.Flush();
	}
}
