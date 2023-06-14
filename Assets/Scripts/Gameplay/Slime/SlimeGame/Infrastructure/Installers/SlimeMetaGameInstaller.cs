using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.LoadSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.Services.Currency;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera;
using TheProxor.SlimeSimulation.InputSystem.MultiTouchService;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers
{
	[CreateAssetMenu(
		fileName = "Slime Meta Game Installer",
		menuName =
			"Take Top/Meta Games/Slime Game/Installers/Slime Meta Game Installer"
	)]
	public class SlimeMetaGameInstaller : ScriptableObjectInstaller
	{
		public const string TRANSFORM_GROUP_NAME = "Slime";

		[Serializable]
		public class StaticDataSettings
		{
			[SerializeField]
			private string basicsPath = "";

			[SerializeField]
			private string subStatesPath = "";

			[SerializeField]
			private string colorsPath = "";

			[SerializeField]
			private string decorationsPath = "";

			public string BasicsPath => basicsPath;
			public string SubStatesPath => subStatesPath;
			public string ColorsPath => colorsPath;
			public string DecorationsPath => decorationsPath;
		}


		[Serializable]
		public class SlimeCameraSettings
		{
			[field: SerializeField] public float SlimeCameraNearClipPlaneOffset { get; private set; } = default;
		}


		[Serializable]
		public class SlimeSettings
		{
			[field: SerializeField] public GameObject SlimeFacadePrefab { get; private set; } = default;
			[field: SerializeField] public Interaction.Settings Interaction { get; private set; } = default;
			[field: SerializeField] public SlimeFacade.Installer.Settings SlimeFacade { get; private set; } = default;
			[field: SerializeField] public Vector3 SlimeSpawnPosition { get; private set; } = default;
			[field: SerializeField] public Vector3 SlimeSpawnOffset { get; private set; } = default;
			[field: SerializeField] public int MaxSlimesCount { get; private set; } = default;
			[field: SerializeField] public Price RecreationCurrency { get; private set; } = default;

			[field: SerializeField] public float SlimeMoveAnimationOffset { get; private set; } = default;
			[field: SerializeField] public float SlimeMoveAnimationDuration { get; private set; } = default;
		}

		[Serializable]
		public class SlimePlayProgressSettings
		{
			[field: SerializeField] public CurrencyType ProgressCurrency { get; private set; } = default;
			[field: SerializeField] public float ProgressCurrencyPerTick { get; set; } = default;
			[field: SerializeField] public float ProgressCurrencyMax { get; private set; } = default;
		}

		[SerializeField] private StaticDataSettings staticData = default;
		[SerializeField] private SlimeCameraSettings slimeCameraSettings = default;
		[SerializeField] private SlimeSettings slime = default;
		[SerializeField] private SlimeMetaGame.Installer.Settings slimeMetaGame = default;


		public override void InstallBindings()
		{
			InstallMetaGame();
			InstallStaticData(staticData);
			InstallSlimeCamera();
			InstallSlimeInfrastructure(slime);
			InstallSlimeCreation(slimeMetaGame.CreateSlimeState);
			InstallEffects();
		}


		private void InstallMetaGame()
		{
			Container.Bind<object>()
				.FromSubContainerResolve()
				.ByInstaller<SlimeMetaGame.Installer>()
				.AsSingle()
				.NonLazy();

			Container.BindInstance(slimeMetaGame)
				.WhenInjectedInto<SlimeMetaGame.Installer>();
		}


		private void InstallStaticData(StaticDataSettings settings)
		{
			Container.Bind<BasisStaticDataProvider>()
				.AsSingle()
				.WithArguments(settings.BasicsPath);

			Container.Bind<SubStateStaticDataProvider>()
				.AsSingle()
				.WithArguments(settings.SubStatesPath);

			Container.Bind<ColorStaticDataProvider>()
				.AsSingle()
				.WithArguments(settings.ColorsPath);

			Container.Bind<DecorStaticDataProvider>()
				.AsSingle()
				.WithArguments(settings.DecorationsPath);

			Container.Bind<SlimeCreationStepStaticDataProvider>()
				.AsSingle()
				.WithArguments(settings.DecorationsPath);
		}


		private void InstallSlimeInfrastructure(SlimeSettings settings)
		{
			Container.Bind<SlimeFacade>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<SlimeFacade.Installer>(settings.SlimeFacadePrefab)
				.UnderTransformGroup(TRANSFORM_GROUP_NAME)
				.AsSingle();

			Container.BindInstance(settings.SlimeFacade)
				.WhenInjectedInto<SlimeFacade.Installer>();

			Container.Bind<IMeshDeformCamera>()
				.To<SlimeDeformCamera>()
				.AsSingle();

			Container.Bind<MeshDeformInput>().AsSingle();

			if (!UnityEngine.Input.touchSupported)
				Container.BindInterfacesTo<MouseTouchService>().AsSingle();
			else
				Container.BindInterfacesTo<MultiTouchService>().AsSingle();

			Container.BindMemoryPool<Interaction, Interaction.Pool>()
				.WithInitialSize(5)
				.WithArguments(settings.Interaction);
		}


		private void InstallSlimeCreation(SlimeCreationState.Installer.Settings settings)
		{
			Container.BindInterfacesAndSelfTo<CreationInput>()
				.AsSingle()
				.WithArguments(settings.CreationInput);

			Container.BindInterfacesTo<ProgressEvaluator>()
				.AsSingle()
				.WithArguments(settings.CreationProgressEvaluator);

			Container.Bind<StatesSequencer>().AsSingle();
			Container.Bind<SlimeBasisCreator>().AsSingle();
			Container.Bind<SlimeSaver>().AsSingle();
			Container.Bind<SlimeLoader>().AsSingle();
		}


		private void InstallSlimeCamera()
		{
			Container.Bind<Camera>()
					 .FromInstance(Camera.main)
					 .AsSingle();

			Container.BindInstance(slimeCameraSettings)
					 .AsSingle();
		}


		private void InstallEffects()
		{
			Container.Bind<Effect.Factory>().AsSingle();
			Container.Bind<EffectDistributor>().AsSingle();
		}
	}
}
