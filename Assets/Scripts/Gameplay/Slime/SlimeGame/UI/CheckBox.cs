using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class CheckBox : MonoBehaviour
	{
		public class Pool : MemoryPool<bool, CheckBox>
		{
			protected override void OnCreated(CheckBox checkBox)
			{
				OnDespawned(checkBox);
			}

			protected override void OnSpawned(CheckBox checkBox)
			{
				checkBox.gameObject.SetActive(true);
			}

			protected override void OnDespawned(CheckBox checkBox)
			{
				checkBox.SetStatus(false);
				checkBox.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		private GameObject checkMark = null;

		public RectTransform RectTransform => transform as RectTransform;

		// [Inject]
		// public void Construct([InjectOptional] bool status)
		// {
		// 	SetStatus(status);
		// }

		public void SetStatus(bool status)
		{
			checkMark.SetActive(status);
		}
	}
}
