using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.PanelSystem;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeAdsIngredientPanel : Panel
	{
		public event Action<SlimeIngredientInfo> OnAdsButton;
		public event Action OnCloseButton;

		[SerializeField] private Button adsButton = default;
		[SerializeField] private Button closeButton = default;
		[SerializeField] private Image previewImage = default;
		[SerializeField] private Image previewJarPaintImage = default;

		private SlimeIngredientInfo slimeIngredientInfo;


		public void SetupSlimeIngredientInfo(SlimeIngredientInfo slimeIngredientInfo) =>
			this.slimeIngredientInfo = slimeIngredientInfo;


		public void SetupIngredientPreview(Sprite previewSprite)
		{
			previewJarPaintImage.gameObject.SetActive(false);
			previewImage.gameObject.SetActive(true);

			previewImage.sprite = previewSprite;
		}


		public void SetupJarPaintIngredientPreview(Color color)
		{
			previewJarPaintImage.gameObject.SetActive(true);
			previewImage.gameObject.SetActive(false);

			previewJarPaintImage.color = color;
		}


		private void Start()
		{
			adsButton.onClick.AddListener(OnAdsButtonClicked);
			closeButton.onClick.AddListener(OnCloseButtonClicked);
		}

		private void OnAdsButtonClicked()
		{
			OnAdsButton?.Invoke(slimeIngredientInfo);
		}


		private void OnCloseButtonClicked()
		{
			OnCloseButton?.Invoke();
		}
	}
}
