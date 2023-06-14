using UnityEngine;


namespace TheProxor.Services.UI
{
	public class Window : MonoBehaviour, IWindow
	{
		public bool OverrideSortingOrder { get; }

		public virtual void Show(bool immediately)
		{
			throw new System.NotImplementedException();
		}


		public virtual void Hide(bool immediately)
		{
			throw new System.NotImplementedException();
		}


		public virtual void SetSortingOrder(int order)
		{
			throw new System.NotImplementedException();
		}
	}
}
