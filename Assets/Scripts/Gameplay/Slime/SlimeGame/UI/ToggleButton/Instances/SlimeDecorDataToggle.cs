using I2.Loc;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeDecorDataToggle : DataToggle<DecorationStaticData, SlimeDecorationInfo>
	{
		[SerializeField] private Color defaultColor = default;
		[SerializeField] private Color unavailableColor = default;
		[SerializeField] private TextMeshProUGUI label = null;
		[SerializeField] private Image icon = null;
		[SerializeField] private TextMeshProUGUI countLabel = default;
		[SerializeField] private GameObject countVisualRoot = default;
		[SerializeField] private GameObject adsVisualRoot = default;

		private SlimeDecorationInfo slimeDecorationInfo;


		public override void RefreshVisual()
		{
			if (slimeDecorationInfo == null)
				return;

			countVisualRoot.SetActive(!slimeDecorationInfo.isAdsIngredient);
			adsVisualRoot.SetActive(slimeDecorationInfo.isAdsIngredient);

			int count = slimeDecorationInfo.Count - slimeDecorationInfo.WastedDecorationsCount;
			isActive = count > 0 || slimeDecorationInfo.isAdsIngredient;

			icon.color = isActive ? defaultColor : unavailableColor;
			countLabel.text = count.ToString();
		}

		protected override void ApplyDataInner(DecorationStaticData staticData, SlimeDecorationInfo slimeDecorationInfo)
		{
			this.slimeDecorationInfo = slimeDecorationInfo;

			icon.sprite = staticData.Icon;
			label.text = LocalizationManager.GetTermTranslation(slimeDecorationInfo.LocalizationKey);

			RefreshVisual();
		}
	}
}
