using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers
{
	public class StatesSequencerProgressViewInstaller : MonoInstaller
	{
		[SerializeField]
		private Transform checkMarksParent;

		[SerializeField]
		private int checkMarkPoolInitialSize = 5;

		[FormerlySerializedAs("checkMarkPrefab"), SerializeField]
		private CheckBox checkBoxPrefab;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<CheckBox, CheckBox.Pool>()
					 .WithInitialSize(checkMarkPoolInitialSize)
					 .FromComponentInNewPrefab(checkBoxPrefab)
					 .UnderTransform(checkMarksParent);
		}
	}
}
