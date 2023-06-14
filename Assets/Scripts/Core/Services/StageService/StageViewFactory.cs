using UnityEngine;
using Zenject;


namespace TheProxor.Services.Stage
{
	public class StageViewFactory : IFactory<StageView, StageView>
	{
		public StageView Create(StageView prototype)
		{
			StageView view = Object.Instantiate(prototype.gameObject, Vector3.zero, Quaternion.identity)
								   .GetComponent<StageView>();

			view.gameObject.SetActive(false);

			return view;
		}
	}
}
