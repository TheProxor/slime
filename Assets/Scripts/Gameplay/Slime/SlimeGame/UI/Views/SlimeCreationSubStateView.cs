using I2.Loc;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI.Views
{
	public class SlimeCreationSubStateView : MonoBehaviour
	{
		[SerializeField]
		private Image icon;

		[SerializeField]
		private TextMeshProUGUI comment;

		private SlimeBasisCreator slimeBasisCreator;
		private SubStateStaticDataProvider subStatesDataProvider;

		[Inject]
		public void Construct(SlimeBasisCreator slimeBasisCreator,
							  SubStateStaticDataProvider subStatesDataProvider)
		{
			this.slimeBasisCreator = slimeBasisCreator;
			this.subStatesDataProvider = subStatesDataProvider;

			InitializeSlimeBasisCreationSequencer();
		}

		private void InitializeSlimeBasisCreationSequencer()
		{
			slimeBasisCreator.OnStartNextState += UpdateView;
		}

		private void UpdateView()
		{
			SubStateStaticData stateData = GetCurrentStateStaticData();
			icon.sprite = stateData.Icon;
			comment.text = LocalizationManager.GetTermTranslation(stateData.Comment);
		}

		private SubStateStaticData GetCurrentStateStaticData()
		{
			return GetStaticDataForType(slimeBasisCreator.ExecutedState.Type);
		}

		private SubStateStaticData GetStaticDataForType(SubStateType subStateType)
		{
			return subStatesDataProvider.GetStaticDataForType(subStateType);
		}
	}
}
