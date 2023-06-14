using I2.Loc;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeBasisDataToggle : DataToggle<BasisStaticData, SlimeBasisInfo>
	{
		[SerializeField] private Color defaultColor = default;
		[SerializeField] private Color unavailableColor = default;
		[SerializeField] private Image icon = default;
		[SerializeField] private TextMeshProUGUI label = default;
		[SerializeField] private TextMeshProUGUI countLabel = default;
		[SerializeField] private GameObject countVisualRoot = default;
		[SerializeField] private GameObject adsVisualRoot = default;

		private SlimeBasisInfo slimeBasisInfo;


		public override void RefreshVisual()
		{
			if (slimeBasisInfo == null)
				return;

			countVisualRoot.SetActive(!slimeBasisInfo.isAdsIngredient);
			adsVisualRoot.SetActive(slimeBasisInfo.isAdsIngredient);

			isActive = slimeBasisInfo.Count > 0 || slimeBasisInfo.isAdsIngredient;

			icon.color = isActive ? defaultColor : unavailableColor;
			countLabel.text = slimeBasisInfo.Count.ToString();
		}

		protected override void ApplyDataInner(BasisStaticData staticData, SlimeBasisInfo slimeBasisInfo)
		{
			this.slimeBasisInfo = slimeBasisInfo;

			icon.sprite = staticData.Icon;
			label.text = LocalizationManager.GetTermTranslation(slimeBasisInfo.LocalizationKey);

			RefreshVisual();
		}

	}
}
