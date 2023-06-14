using TheProxor.Services;
using UnityEngine;
using Zenject;


namespace TheProxor.Installers
{
	[CreateAssetMenu(order = 0, fileName = nameof(ServicesInstaller), menuName = "Installers/" + nameof(ServicesInstaller))]
	public class ServicesInstaller : ScriptableObjectInstaller<ServicesInstaller>
	{
		[SerializeReference] private ServiceInstaller[] servicesInstallers = default;


		public override void InstallBindings()
		{
			foreach (var servicesInstaller in servicesInstallers)
			{
				servicesInstaller.InstallBindingsInto(Container);
			}
		}
	}
}
