using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure
{
	public partial class SlimeMetaGame
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings
			{
				[Serializable]
				public class BowlSettings
				{
					[SerializeField]
					private GameObject prefab = null;

					[SerializeField]
					private BowlFacade.Installer.Settings bowlFacade = null;

					public GameObject Prefab => prefab;
					public BowlFacade.Installer.Settings BowlFacade => bowlFacade;
				}

				[SerializeField]
				private SlimeCreationState.Installer.Settings createSlimeState = null;

				[SerializeField]
				private BowlSettings bowl = null;

				public SlimeCreationState.Installer.Settings CreateSlimeState => createSlimeState;
				public BowlSettings Bowl => bowl;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}


			public override void InstallBindings()
			{
				Container.Bind<SlimeMetaGame>()
					.To<SlimeMetaGame>()
					.AsSingle();

				InstallStates();
				InstallBowl(settings.Bowl);
			}


			private void InstallStates()
			{
				InstallStateMachineFactory();
				InstallSlimeSelectionState();
				InstallCreateSlimeState();
				InstallSlimeRecreationState();
				InstallPlaySlimeState();
			}

			private void InstallStateMachineFactory()
			{
				Container.BindFactory<IEnumerable<ExitableState>, States.StateMachine, States.StateMachine.Factory>();
			}


			private void InstallSlimeSelectionState()
			{
				Container.Bind<SlimeSelectionState>()
					.AsSingle();
			}

			private void InstallPlaySlimeState()
			{
				Container.Bind<PlaySlimeState>()
					.AsSingle();
			}

			private void InstallCreateSlimeState()
			{
				Container.Bind<SlimeCreationState>()
					.FromSubContainerResolve()
					.ByInstaller<SlimeCreationState.Installer>()
					.AsSingle();

				Container.BindInstance(settings.CreateSlimeState)
					.WhenInjectedInto<SlimeCreationState.Installer>();
			}

			private void InstallSlimeRecreationState()
			{
				Container.Bind<SlimeRecreationState>()
					.AsSingle();
			}

			private void InstallBowl(Settings.BowlSettings bowlSettings)
			{
				Container.Bind<BowlFacade>()
					.FromSubContainerResolve()
					.ByNewPrefabInstaller<BowlFacade.Installer>(bowlSettings.Prefab)
					.UnderTransformGroup(SlimeMetaGameInstaller.TRANSFORM_GROUP_NAME)
					.AsSingle();

				Container.BindInstance(bowlSettings.BowlFacade)
					.WhenInjectedInto<BowlFacade.Installer>();
			}
		}
	}
}
