using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.RollbackSystem;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public partial class SlimeCreationState
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings : SlimeCreationState.Settings
			{
				[SerializeField]
				private CreationInput.Settings creationInput = null;

				[SerializeField]
				private ProgressEvaluator.Settings creationProgressEvaluator = null;

				[SerializeField]
				private SlimeBasisCreationState.Installer.Settings slimeBasisCreationState = null;

				[SerializeField]
				private SlimeAddColorState.Installer.Settings slimeAddColorState = null;

				[SerializeField]
				private SlimeAddDecorationState.Installer.Settings slimeAddDecorationState = null;

				[SerializeField]
				private SlimeCreationPlayState.Installer.Settings slimeCreationPlayState = null;

				public CreationInput.Settings CreationInput => creationInput;
				public ProgressEvaluator.Settings CreationProgressEvaluator =>
					creationProgressEvaluator;
				public SlimeBasisCreationState.Installer.Settings SlimeBasisCreationState =>
					slimeBasisCreationState;
				public SlimeAddColorState.Installer.Settings SlimeAddColorState =>
					slimeAddColorState;
				public SlimeAddDecorationState.Installer.Settings SlimeAddDecorationState =>
					slimeAddDecorationState;
				public SlimeCreationPlayState.Installer.Settings SlimeCreationPlayState =>
					slimeCreationPlayState;
			}

			private readonly Settings settings;

			public Installer(Settings settings)
			{
				this.settings = settings;
			}

			public override void InstallBindings()
			{
				InstallCreateSlimeState();
				InstallSubStates();
				InstallRollbackSystem();
				InstallView();
			}

			private void InstallCreateSlimeState()
			{
				Container.BindInterfacesAndSelfTo<SlimeCreationState>()
						 .AsSingle();

				Container.BindInstance(settings as SlimeCreationState.Settings)
						 .WhenInjectedInto<SlimeCreationState>();
			}

			private void InstallSubStates()
			{
				InstallSlimeBasisSelectionState();
				InstallSlimeCreationFromBasisState();
				InstallSlimeColorSelectionState();
				InstallSlimeAddColorState();
				InstallSlimeDecorSelectionState();
				InstallAddDecorationState();
				InstallSlimeCreationPlayState();
				InstallSaveSlimeState();
			}

			private void InstallSlimeBasisSelectionState()
			{
				Container.BindInterfacesAndSelfTo<SlimeBasisSelectionState>()
						 .AsSingle();
			}

			private void InstallSlimeCreationFromBasisState()
			{
				Container.Bind<SlimeBasisCreationState>()
						 .FromSubContainerResolve()
						 .ByInstaller<SlimeBasisCreationState.Installer>()
						 .AsSingle();

				Container.BindInstance(settings.SlimeBasisCreationState)
						 .WhenInjectedInto<SlimeBasisCreationState.Installer>();
			}

			private void InstallSlimeColorSelectionState()
			{
				Container.Bind<SlimeColorSelectionState>()
						 .AsSingle();
			}

			private void InstallSlimeAddColorState()
			{
				Container.Bind<SlimeAddColorState>()
						 .FromSubContainerResolve()
						 .ByInstaller<SlimeAddColorState.Installer>()
						 .AsSingle();

				Container.BindInstance(settings.SlimeAddColorState)
						 .WhenInjectedInto<SlimeAddColorState.Installer>();
			}

			private void InstallSlimeDecorSelectionState()
			{
				Container.Bind<SlimeDecorSelectionState>()
						 .AsSingle();
			}

			private void InstallAddDecorationState()
			{
				Container.Bind<SlimeAddDecorationState>()
						 .FromSubContainerResolve()
						 .ByInstaller<SlimeAddDecorationState.Installer>()
						 .AsSingle();

				Container.BindInstance(settings.SlimeAddDecorationState)
						 .WhenInjectedInto<SlimeAddDecorationState.Installer>();
			}

			private void InstallSlimeCreationPlayState()
			{
				Container.Bind<SlimeCreationPlayState>()
						 .FromSubContainerResolve()
						 .ByInstaller<SlimeCreationPlayState.Installer>()
						 .AsSingle();

				Container.BindInstance(settings.SlimeCreationPlayState)
						 .WhenInjectedInto<SlimeCreationPlayState.Installer>();
			}

			private void InstallSaveSlimeState()
			{
				Container.Bind<SaveSlimeState>()
						 .AsSingle();
			}

			private void InstallRollbackSystem()
			{
				Container.Bind<Rollback>()
						 .AsSingle();
			}

			private void InstallView()
			{
				Container.Bind<SlimeCreationStepViewController>()
						 .AsSingle();

				Container.Bind<SlimeCreationStepChangeHandler>()
						 .AsSingle()
						 .NonLazy();
			}
		}
	}
}
