using I2.Loc;
using TheProxor.Utils;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TMPro;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeCreationStepView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI label;

		private SlimeCreationStepStaticDataProvider staticDataProvider;


		[Inject]
		public void Construct(SlimeCreationStepStaticDataProvider staticDataProvider)
		{
			this.staticDataProvider = staticDataProvider;
		}


		public void SetStepType(SlimeCreationStepType type, params object[] args)
		{
			string localizedText = LocalizationManager.GetTermTranslation(staticDataProvider.GetStaticDataForType(type).Comment);
			label.text = localizedText.SafeStringFormat(args);
		}
	}
}
