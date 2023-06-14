using System;
using TheProxor.PanelSystem;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeCancelPanel : Panel
	{
		public event Action OnExitButton;
		public event Action OnContinueButton;


		[SerializeField] private Button exitButton = default;
		[SerializeField] private Button continueButton = default;
		[SerializeField] private Button continueCrossButton = default;



		private void Start()
		{
			exitButton.onClick.AddListener(OnExitButtonClicked);
			continueButton.onClick.AddListener(OnContinueButtonClicked);
			continueCrossButton.onClick.AddListener(OnContinueButtonClicked);
		}


		private void OnExitButtonClicked()
		{
			OnExitButton?.Invoke();
		}


		private void OnContinueButtonClicked()
		{
			OnContinueButton?.Invoke();
		}
	}
}
