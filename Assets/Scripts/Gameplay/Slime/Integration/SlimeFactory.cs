using TheProxor.Logic.AssetProvider;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.LoadSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.PanelSystem;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera;
using TheProxor.SlimeSimulation.InputSystem.MultiTouchService;
using UnityEngine;
using Zenject;


public class SlimeFactory
{
	public const string TRANSFORM_SLIME_ROOT_NAME = "Slime";

	private readonly DiContainer container;
	private readonly SlimeMetaGameInstaller.StaticDataSettings staticData;
	private readonly SlimeMetaGameInstaller.SlimeSettings slimeSettings;
	private readonly SlimeMetaGame.Installer.Settings slimeMetaGame;

	private SlimeMetaGame slime;
	private DiContainer subContainer;


	public SlimeFactory(
		DiContainer container,
		SlimeMetaGameInstaller.StaticDataSettings staticData,
		SlimeMetaGameInstaller.SlimeSettings slimeSettings,
		SlimeMetaGame.Installer.Settings slimeMetaGame
	)
	{
		this.container = container;
		this.staticData = staticData;
		this.slimeSettings = slimeSettings;
		this.slimeMetaGame = slimeMetaGame;
	}


	public SlimeMetaGame Create()
	{
		subContainer = container.CreateSubContainer();

		InstallBindings();
		slime = subContainer.Resolve<SlimeMetaGame>();

		return slime;
	}


	public void Flush()
	{
		slime = null;
		subContainer.UnbindAll();
		subContainer = null;
	}


	private void InstallBindings()
	{
		InstallMetaGame();
		InstallStaticData(staticData);
		InstallSlimeInfrastructure(slimeSettings);
		InstallSlimeCreation(slimeMetaGame.CreateSlimeState);
		InstallEffects();

		subContainer.Bind<IAssetProvider>()
			.To<AssetProvider>()
			.AsSingle();

		subContainer.BindInterfacesAndSelfTo<PanelManager>().AsSingle().NonLazy();

		subContainer.BindFactory<Panel, Panel, Panel.Factory>()
			.FromFactory<PrefabFactory<Panel>>();
	}


	private void InstallMetaGame()
	{
		subContainer.Bind<SlimeMetaGame>()
			.FromSubContainerResolve()
			.ByInstaller<SlimeMetaGame.Installer>()
			.AsSingle()
			.NonLazy();

		subContainer.BindInstance(slimeMetaGame)
			.WhenInjectedInto<SlimeMetaGame.Installer>();
	}


	private void InstallStaticData(SlimeMetaGameInstaller.StaticDataSettings settings)
	{
		subContainer.Bind<BasisStaticDataProvider>()
			.AsSingle()
			.WithArguments(settings.BasicsPath);

		subContainer.Bind<SubStateStaticDataProvider>()
			.AsSingle()
			.WithArguments(settings.SubStatesPath);

		subContainer.Bind<ColorStaticDataProvider>()
			.AsSingle()
			.WithArguments(settings.ColorsPath);

		subContainer.Bind<DecorStaticDataProvider>()
			.AsSingle()
			.WithArguments(settings.DecorationsPath);

		subContainer.Bind<SlimeCreationStepStaticDataProvider>()
			.AsSingle()
			.WithArguments(settings.DecorationsPath);
	}


	private void InstallSlimeInfrastructure(SlimeMetaGameInstaller.SlimeSettings settings)
	{
		subContainer.Bind<SlimeFacade>()
					.FromSubContainerResolve()
					.ByNewPrefabInstaller<SlimeFacade.Installer>(settings.SlimeFacadePrefab)
					.UnderTransformGroup(TRANSFORM_SLIME_ROOT_NAME)
					.AsSingle();

		subContainer.BindInstance(settings.SlimeFacade)
			.WhenInjectedInto<SlimeFacade.Installer>();

		subContainer.Bind<Camera>()
					.FromInstance(Camera.main)
					.AsSingle();

		subContainer.Bind<Vector3>()
					.FromInstance(settings.SlimeSpawnPosition);

		subContainer.Bind<IMeshDeformCamera>()
			.To<SlimeDeformCamera>()
			.AsSingle();

		subContainer.Bind<MeshDeformInput>().AsSingle();

		if (!UnityEngine.Input.touchSupported)
			subContainer.Bind<IMultiTouchService>().To<MouseTouchService>().AsSingle();
		else
			subContainer.Bind<IMultiTouchService>().To<MultiTouchService>().AsSingle();

		subContainer.BindMemoryPool<Interaction, Interaction.Pool>()
			.WithInitialSize(5)
			.WithArguments(settings.Interaction);
	}


	private void InstallSlimeCreation(SlimeCreationState.Installer.Settings settings)
	{
		subContainer.BindInterfacesAndSelfTo<CreationInput>()
			.AsSingle()
			.WithArguments(settings.CreationInput);

		subContainer.BindInterfacesAndSelfTo<ProgressEvaluator>()
			.AsSingle()
			.WithArguments(settings.CreationProgressEvaluator);

		subContainer.Bind<StatesSequencer>().AsSingle();
		subContainer.Bind<SlimeBasisCreator>().AsSingle();
		subContainer.Bind<SlimeSaver>().AsSingle();
		subContainer.Bind<SlimeLoader>().AsSingle();
	}


	private void InstallEffects()
	{
		subContainer.Bind<Effect.Factory>().AsSingle();
		subContainer.Bind<EffectDistributor>().AsSingle();
	}
}
