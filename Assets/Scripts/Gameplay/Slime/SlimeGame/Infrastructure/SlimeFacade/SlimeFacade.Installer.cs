using System;
using TheProxor.SlimeSimulation.DecorationSystemModule;
using TheProxor.SlimeSimulation.DeformationSystemModule;
using TheProxor.SlimeSimulation.ViewModule;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure
{
	public partial class SlimeFacade
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{

			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				Container.Bind<SlimeFacade>()
						 .AsSingle();

				Container.Bind<SlimeView>()
						 .AsSingle();

				Container.Bind<SlimeMesh>()
						 .AsSingle();

				Container.BindFactory<SlimeDecorationGroup,
					Action<GameObject>,
					SlimeDecorationSystem,
					SlimeDecorationSystem.Factory>();
			}
		}
	}
}
