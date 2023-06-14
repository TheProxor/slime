using I2.Loc;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeColorDataToggle : DataToggle<ColorStaticData, SlimeColorInfo>
	{
		[SerializeField] private Color defaultColor = default;
		[SerializeField] private Color unavailableColor = default;
		[SerializeField] private Image jarPaintIcon = default;
		[SerializeField] private Image jarPaintColorIcon = default;
		[SerializeField] private TextMeshProUGUI label = null;
		[SerializeField] private TextMeshProUGUI countLabel = default;
		[SerializeField] private GameObject countVisualRoot = default;
		[SerializeField] private GameObject adsVisualRoot = default;

		private SlimeColorInfo slimeColorInfo;
		private ColorStaticData slimeColorStaticData;


		public override void RefreshVisual()
		{
			if (slimeColorInfo == null)
				return;

			countVisualRoot.SetActive(!slimeColorInfo.isAdsIngredient);
			adsVisualRoot.SetActive(slimeColorInfo.isAdsIngredient);

			isActive = slimeColorInfo.Count > 0 || slimeColorInfo.isAdsIngredient;

			Color color = isActive ? defaultColor : unavailableColor;

			jarPaintIcon.color = color;
			jarPaintColorIcon.color = slimeColorStaticData.Color * color;

			countLabel.text = slimeColorInfo.Count.ToString();
		}

		protected override void ApplyDataInner(ColorStaticData staticData, SlimeColorInfo slimeColorInfo)
		{
			this.slimeColorStaticData = staticData;
			this.slimeColorInfo = slimeColorInfo;

			label.text = LocalizationManager.GetTermTranslation(slimeColorInfo.LocalizationKey);

			RefreshVisual();
		}
	}
}
