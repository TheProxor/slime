using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public interface ISlimeCreationStepProvider
	{
		event Action<ISlimeCreationStepProvider, object[]> OnNewCreationStep;
		SlimeCreationStepType SlimeCreationStepType { get; }
	}
}
