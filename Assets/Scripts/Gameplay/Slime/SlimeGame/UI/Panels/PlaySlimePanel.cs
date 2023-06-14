using System;
using TheProxor.PanelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class PlaySlimePanel : Panel
	{
		public event Action OnExit;

		[SerializeField]
		private Button exitButton = null;
		[Header("Progress bar")]
		[SerializeField] private Image imgProgressBack;
		[SerializeField] private Image imgProgressBar;
		[SerializeField] private TMP_Text txtProgressBar;
		[SerializeField] private RectMask2D progressMask;

		public Button BtnExit => exitButton;


		public void SetProgress(float value, float max)
		{
			var t = value / max;
			progressMask.padding = new Vector4(0f, 0f, (1f - t) * progressMask.canvasRect.width, 0f);
			txtProgressBar.SetText($"{value:F0}/{max:F0}");
		}

		private void Start()
		{
			exitButton.onClick.AddListener(Exit);
		}

		private void Exit()
		{
			OnExit?.Invoke();
		}
	}
}
