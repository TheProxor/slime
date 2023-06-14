using System.Collections.Generic;
using TheProxor.Services.Stage;
using Zenject;


namespace TheProxor.Installers
{
	public class StageServiceInstaller : ServiceInstaller<StageService, StageService.Settings>
	{
		public override void InstallBindingsInto(DiContainer container)
		{
			base.InstallBindingsInto(container);

			container.Bind<IDictionary<StageType, StageBehaviour>>()
					 .FromMethod(GetStageBehaviours)
					 .AsSingle();

			container.Bind<IFactory<StageView, StageView>>()
					 .To<StageViewFactory>()
					 .AsSingle();



			IDictionary<StageType, StageBehaviour> GetStageBehaviours(InjectContext context) =>
				new Dictionary<StageType, StageBehaviour>()
				{
					{ StageType.CraftTable, context.Container.Instantiate<CraftTableStageBehaviour>() },
				};
		}
	}
}
