using System.Collections.Generic;
using TheProxor.Services.UI;
using UnityEngine;
using Zenject;


namespace TheProxor.Installers
{
	public class WindowsServiceInstaller : ServiceInstaller<WindowsService, WindowsService.Settings>
	{
		public override void InstallBindingsInto(DiContainer container)
		{
			IWindowTransitionController[] transitionControllers =
			{
				container.Instantiate<ImmediatelyTransitionController>()
			};

			container.Bind<ICollection<IWindowTransitionController>>()

					 .FromInstance(transitionControllers)
					 .AsSingle();

			container.Bind<IFactory<Object, Transform, Window>>()
					 .To<WindowsFactory>()
					 .AsSingle();

			base.InstallBindingsInto(container);
		}
	}
}
