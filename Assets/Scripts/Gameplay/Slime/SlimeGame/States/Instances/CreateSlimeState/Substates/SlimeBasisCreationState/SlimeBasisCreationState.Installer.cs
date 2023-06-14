using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeBasisCreationState
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{
				[SerializeField]
				private BasisRecipeStepState.Settings basisRecipeStepState = null;

				public BasisRecipeStepState.Settings BasisRecipeStepState => basisRecipeStepState;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				Container.Bind<SlimeBasisCreationState>()
						 .AsSingle();

				Container.BindMemoryPool<BasisRecipeStepState, BasisRecipeStepState.Pool>()
						 .WithInitialSize(5);

				Container.BindMemoryPool<SlimeMaterialAnimator, SlimeMaterialAnimator.Pool>()
						 .WithInitialSize(1);

				Container.BindMemoryPool<BowlFillAnimator, BowlFillAnimator.Pool>()
						 .WithInitialSize(1);

				Container.BindInstance(settings.BasisRecipeStepState);
			}
		}
	}
}
