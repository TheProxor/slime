using TheProxor.Services.Preference;
using Zenject;


namespace TheProxor.Installers
{
	public class PreferenceServiceInstaller : ServiceInstaller<PreferenceService>
	{
		public override void InstallBindingsInto(DiContainer container)
		{
			base.InstallBindingsInto(container);

			container.Bind<IPreferenceServiceSerializer>()
					 .To<NewtonsoftPreferenceServiceSerializer>()
					 .AsSingle();
		}
	}
}
