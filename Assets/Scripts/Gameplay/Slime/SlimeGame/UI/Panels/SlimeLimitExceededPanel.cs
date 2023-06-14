using System;
using TheProxor.PanelSystem;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeLimitExceededPanel : Panel
	{
		public event Action OnCloseButton;


		[SerializeField] private Button closeButton = default;
		[SerializeField] private Button continueButton = default;


		private void Start()
		{
			closeButton.onClick.AddListener(OnCloseButtonClicked);
			continueButton.onClick.AddListener(OnCloseButtonClicked);
		}


		private void OnCloseButtonClicked()
		{
			OnCloseButton?.Invoke();
		}
	}
}
