using System.Collections.Generic;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeCreationStepChangeHandler
	{
		private readonly SlimeCreationStepViewController controller;


		public SlimeCreationStepChangeHandler(
			SlimeCreationStepViewController controller,
			[InjectLocal] IEnumerable<ISlimeCreationStepProvider> providers
		)
		{
			foreach (ISlimeCreationStepProvider state in providers)
			{
				controller.AddGameStepState(state);
			}
		}
	}
}
