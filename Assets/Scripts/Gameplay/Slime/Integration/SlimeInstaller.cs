using TheProxor.MetaGamesSystem.MetaGames.SlimeGame;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using TheProxor.PanelSystem;
using UnityEngine;
using Zenject;


namespace MiniGamesSystem.Games.Towers
{
	[CreateAssetMenu(fileName = "Slime Installer", menuName = "Installers/MiniGames/Slime Installer")]
	public class SlimeInstaller : ScriptableObjectInstaller<SlimeInstaller>
	{
		[SerializeField] private PanelManager.Settings panelManagerSettings = default;
		[SerializeField] private SlimeMetaGameInstaller.StaticDataSettings staticData = default;
		[SerializeField] private SlimeMetaGameInstaller.SlimeCameraSettings slimeCameraSettings = default;
		[SerializeField] private SlimeMetaGameInstaller.SlimeSettings slime = default;
		[SerializeField] private SlimeMetaGame.Installer.Settings slimeMetaGame = default;
		[SerializeField] private SlimeIngredientDataBase slimeIngredientDataBase = default;
		[SerializeField] private SlimeMetaGameInstaller.SlimePlayProgressSettings slimePlayProgressSettings = default;


		public override void InstallBindings()
		{
			Container.BindInstance(panelManagerSettings);

			Container.BindInstance(staticData);
			Container.BindInstance(slime);
			Container.BindInstance(slimeCameraSettings);
			Container.BindInstance(slimeMetaGame);
			Container.BindInstance(slimeIngredientDataBase);
			Container.BindInstance(slimePlayProgressSettings);

			Container.Bind<ISlimeIngredientsManager<SlimeIngredientInfo>>()
				.To<SlimeIngredientsManager>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<SlimeFactory>()
				.AsSingle()
				.NonLazy();
		}
	}
}
