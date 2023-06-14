using System;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public partial class BowlFacade
	{
		public class Installer : Installer<Settings, Installer>
		{
			[Serializable]
			public class Settings : BowlFacade.Settings {}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				InstallBowlFacade();
				InstallSlimeInteractionController();
			}

			private void InstallBowlFacade()
			{
				Container.Bind<BowlFacade>()
						 .AsSingle()
						 .WithArguments(settings);
			}

			private void InstallSlimeInteractionController()
			{
				Container.Bind<SlimeInteractionController>()
						 .AsSingle();
			}
		}
	}
}
