using System;
using System.Collections.Generic;
using TheProxor.Utils;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.Services.Currency;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeSelectionPanel : DataSelectionPanel<SlimeSaveData, SlimeIngredientInfo>
	{
		public event Action OnNewSlimeCreateSelected;
		public event Action OnRecreateOnAds;
		public event Action OnRecreateOnCurrency;

		[SerializeField] private Button buttonSelectPrev = default;
		[SerializeField] private Button buttonSelectNext = default;
		[SerializeField] private Button createNewSlimeButton = null;
		[SerializeField] private Button recreateButtonAds = null;
		[SerializeField] private Button recreateButtonCurrency = null;
		[SerializeField] private TMP_Text slimeCountLabel = default;
		[SerializeField] private string slimeCountTemplate = default;
		[SerializeField] private GameObject slimeRecreatePanel = default;
		[SerializeField] private GameObject slimeActivePlus = default;
		[SerializeField] private GameObject slimeNoneActivePlus = default;

		public Button BtnNext => buttonSelectNext;
		public Button BtnPrev => buttonSelectPrev;

		private List<SlimeSaveData> selectableData;
		private int current = -1;



		public Button CreateNewSlimeButton => createNewSlimeButton;
		public bool IsNewSlimeCreationAccepted => CreatedSlimesCount < MaxSlimesCount;
		public int MaxSlimesCount { get; set; }
		public Price RecreationCurrency { get; set; }
		public int Current
		{
			get => current;
			set
			{
				current = value;

				if (current >= selectableData.Count)
					current = 0;

				if (current < 0)
					current = selectableData.Count - 1;
			}
		}

		public int CreatedSlimesCount => selectableData.Count - 1;


		private void Start()
		{
			InitializeNewSlimeButton();
			InitializeRecreateButton();

			buttonSelectPrev.onClick.AddListener(OnSelectPrevButtonClick);
			buttonSelectNext.onClick.AddListener(OnSelectNextButtonClick);
		}


		public override void Initialize(IEnumerable<(SlimeSaveData, SlimeIngredientInfo)> selectableData)
		{
			this.selectableData = new List<SlimeSaveData>();

			foreach (var data in selectableData)
			{
				if (string.IsNullOrEmpty(data.Item1.Id))
					continue;

				this.selectableData.Add(data.Item1);
			}

			this.selectableData.Add(default);

			if (Current < 0)
				Current = this.selectableData.Count - 2;

			RefreshSlimeCreationButton();
			Refresh();
		}

		public void Refresh()
		{
			slimeCountLabel.text = slimeCountTemplate.SafeStringFormat(CreatedSlimesCount, MaxSlimesCount);
			Select(this.selectableData[Current], null, 0);
		}


		public void TriggerNewSlimeCreateSelected() =>
			OnNewSlimeCreateSelected?.Invoke();


		private void InitializeNewSlimeButton()
		{
			createNewSlimeButton.onClick.AddListener(SelectNewSlimeCreation);
		}


		private void InitializeRecreateButton()
		{
			recreateButtonAds.onClick.AddListener(RecreateForAds);
			recreateButtonCurrency.onClick.AddListener(RecreateForCurrency);
		}


		private void SelectNewSlimeCreation()
		{
			OnNewSlimeCreateSelected?.Invoke();
		}


		private void RecreateForAds()
		{
			OnRecreateOnAds?.Invoke();
		}


		private void RecreateForCurrency()
		{
			OnRecreateOnCurrency?.Invoke();
		}


		private void OnSelectPrevButtonClick()
		{
			Current--;
			Select(selectableData[Current], null, -1);
			RefreshSlimeCreationButton();
		}


		private void OnSelectNextButtonClick()
		{
			Current++;
			Select(selectableData[Current], null, 1);
			RefreshSlimeCreationButton();
		}


		private void RefreshSlimeCreationButton()
		{
			//recreateButtonCurrency.SetPrice(RecreationCurrency);
			slimeRecreatePanel.SetActive(!string.IsNullOrEmpty(selectableData[Current].Id));
			createNewSlimeButton.gameObject.SetActive(string.IsNullOrEmpty(selectableData[Current].Id));
			slimeActivePlus.SetActive(IsNewSlimeCreationAccepted);
			slimeNoneActivePlus.SetActive(!IsNewSlimeCreationAccepted);
		}
	}
}
