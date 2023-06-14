using TheProxor.Services.UI;
using UnityEngine;


namespace TheProxor.UI
{
	public class CraftTableWindow : Window
	{
		[field: SerializeField] public bool OverrideSortingOrder { get; private set; } = default;



		public override void Show(bool immediately)
		{
			gameObject.SetActive(true);
		}


		public override void Hide(bool immediately)
		{
			gameObject.SetActive(false);
		}


		public override void SetSortingOrder(int order)
		{

		}
	}
}
