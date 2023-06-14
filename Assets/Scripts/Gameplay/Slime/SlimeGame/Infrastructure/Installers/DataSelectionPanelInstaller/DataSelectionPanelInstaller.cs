using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers
{
	public class DataSelectionPanelInstaller<TData, TDataInfo, TToggleButton> : MonoInstaller
		where TToggleButton : DataToggle<TData, TDataInfo>
	{
		[SerializeField]
		private TToggleButton dataTogglePrefab = null;

		[SerializeField]
		private Transform togglesParent = null;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<DataToggle<TData, TDataInfo>, DataToggle<TData, TDataInfo>.Pool>()
					 .FromComponentInNewPrefab(dataTogglePrefab)
					 .UnderTransform(togglesParent)
					 .AsSingle();
		}
	}
}
