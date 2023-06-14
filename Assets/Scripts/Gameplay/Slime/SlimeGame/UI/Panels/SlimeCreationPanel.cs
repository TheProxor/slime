using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.PanelSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeCreationPanel : Panel
	{
		public event Action OnGoBack;
		public event Action OnGoToStartScreen;

		[SerializeField]
		private Button goBackButton = null;

		[SerializeField]
		private Button goToStartScreenButton = null;

		[FormerlySerializedAs("creationStepView"),FormerlySerializedAs("gameStepView"),SerializeField]
		private SlimeCreationStepView slimeCreationStepView;



		public SlimeCreationStepView SlimeCreationStateView => slimeCreationStepView;

		public Button GoBackButton => goBackButton;

		public Button GoToStartScreenButton => goToStartScreenButton;



		public void SetGoBackButtonActive(bool value)
		{
			goBackButton.gameObject.SetActive(value);
		}

		private void Start()
		{
			InitializeGoBackButton();
			InitializeGoToStartScreenButton();
		}

		private void InitializeGoBackButton()
		{
			goBackButton.onClick.AddListener(GoBack);
		}

		private void GoBack()
		{
			OnGoBack?.Invoke();
		}

		private void InitializeGoToStartScreenButton()
		{
			goToStartScreenButton.onClick.AddListener(GoToStartScreen);
		}

		private void GoToStartScreen()
		{
			OnGoToStartScreen?.Invoke();
		}
	}
}
