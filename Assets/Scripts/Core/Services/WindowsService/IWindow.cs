namespace TheProxor.Services.UI
{
	public interface IWindow
	{
		bool OverrideSortingOrder { get; }


		void Show(bool immediately);
		void Hide(bool immediately);
		void SetSortingOrder(int order);
	}
}
