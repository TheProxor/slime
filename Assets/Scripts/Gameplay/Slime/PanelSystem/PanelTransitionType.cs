namespace TheProxor.PanelSystem
{
	public enum PanelTransitionType
	{
		/// <summary>
		/// Instantly close the current panels and open a new panel
		/// </summary>
		Instantly = 0,
		/// <summary>
		/// Open a new panel after closing current panels
		/// </summary>
		Consistent = 5,
		/// <summary>
		/// In parallel, start the procedures for opening a new panel and closing the current panel
		/// </summary>
		Parallel = 10,
		/// <summary>
		/// First open a new panel and then close the rest of the panels.
		/// </summary>
		Reverse = 15,
		/// <summary>
		/// Open a new panel without closing other panels
		/// </summary>
		Override = 20,
	}
}
