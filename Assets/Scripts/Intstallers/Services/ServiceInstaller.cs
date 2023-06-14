using System;
using TheProxor.Services;
using UnityEngine;
using Zenject;


namespace TheProxor.Installers
{
	[Serializable]
	public abstract class ServiceInstaller : Installer<ServiceInstaller>
	{
		public virtual void InstallBindingsInto(DiContainer container) {}


		public override void InstallBindings() =>
			InstallBindingsInto(Container);
	}



	[Serializable]
	public abstract class ServiceInstaller<TService> : ServiceInstaller
	{
		public override void InstallBindingsInto(DiContainer container)
		{
			container.BindInterfacesTo<TService>()
					 .AsSingle()
					 .NonLazy();
		}
	}



	[Serializable]
	public abstract class ServiceInstaller<TService, TServiceSettings> : ServiceInstaller
	{
		[SerializeField] private TServiceSettings settings = default;


		public override void InstallBindingsInto(DiContainer container)
		{
			container.BindInstance(settings)
					 .AsSingle();

			container.BindInterfacesTo<TService>()
					 .AsSingle()
					 .NonLazy();
		}
	}
}
