using UnityEngine;


namespace TheProxor.Services.Stage
{
	public abstract class StageView : MonoBehaviour, IStageView
	{
		public void EnableView() =>
			gameObject.SetActive(true);


		public void DisableView() =>
			gameObject.SetActive(false);
	}
}
