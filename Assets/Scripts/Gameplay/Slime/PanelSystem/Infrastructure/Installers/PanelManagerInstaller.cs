using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace TheProxor.PanelSystem.Infrastructure.Installers
{
	[CreateAssetMenu]
	public class PanelManagerInstaller : ScriptableObjectInstaller
	{
		// [SerializeField]
		// private RootCanvas rootCanvasPrefab;

		[SerializeField]
		private List<Panel> panelsPrefabs;


		public override void InstallBindings()
		{
			Container.BindInterfacesTo<PanelManager>()
				.FromSubContainerResolve()
				.ByMethod(InstallPanelManager)
				.AsSingle()
				.NonLazy();
		}


		private void InstallPanelManager(DiContainer container)
		{
			container.Bind<PanelManager>().AsSingle().WithArguments(panelsPrefabs);

			container.BindFactory<Panel, Panel, Panel.Factory>()
				.FromFactory<PrefabFactory<Panel>>();

			// container.Bind<IRootCanvas>().FromComponentInNewPrefab(rootCanvasPrefab).AsSingle();
		}
	}
}
