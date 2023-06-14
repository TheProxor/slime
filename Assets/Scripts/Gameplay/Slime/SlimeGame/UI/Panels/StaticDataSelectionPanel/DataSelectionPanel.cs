using System;
using System.Collections.Generic;
using System.Linq;
using TheProxor.PanelSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public abstract class DataSelectionPanel<TData, TDataInfo> : Panel
	{
		public event Action<TData, TDataInfo, int> OnSelected;
		public event Action OnPageChanged;



		public readonly Dictionary<TData, DataToggle<TData, TDataInfo>> spawnedButtons = new();


		[SerializeField] private Button prevPageBtn = default;
		[SerializeField] private Button nextPageBtn = default;
		private DataToggle<TData, TDataInfo>.Pool dataTogglePool;


		public Button PrevPageBtn => prevPageBtn;
		public Button NextPageBtn => nextPageBtn;
		protected virtual Comparison<DataToggle<TData, TDataInfo>> DataSortComparison => null;



		[Inject]
		public void Construct(DataToggle<TData, TDataInfo>.Pool dataTogglePool)
		{
			this.dataTogglePool = dataTogglePool;
			prevPageBtn?.onClick.AddListener(OnChangePage);
			nextPageBtn?.onClick.AddListener(OnChangePage);
		}


		public virtual void Initialize(IEnumerable<(TData, TDataInfo)> selectableData)
		{
			DeSpawnToggles();

			foreach ((TData, TDataInfo) data in selectableData)
				AddData(data.Item1, data.Item2);

			Sort();
		}


		public virtual void RefreshVisual(bool isSortingRequired = true)
		{
			foreach (var dataToggle in spawnedButtons.Values)
			{
				dataToggle.RefreshVisual();
			}

			if (isSortingRequired)
				Sort();
		}


		protected void Select(TData data, TDataInfo dataInfo, int direction) =>
			OnSelected?.Invoke(data, dataInfo, direction);


		private void AddData(TData data, TDataInfo dataInfo)
		{
			DataToggle<TData, TDataInfo> button = dataTogglePool.Spawn(data, dataInfo);
			spawnedButtons.Add(data, button);
			button.OnToggled += Select;
		}


		private void DeSpawnToggles()
		{
			foreach (DataToggle<TData, TDataInfo> spawnedButton in spawnedButtons.Values)
			{
				dataTogglePool.Despawn(spawnedButton);
			}

			spawnedButtons.Clear();
		}


		private void Sort()
		{
			if (spawnedButtons.Count == 0)
				return;

			var buttonsList = spawnedButtons.Values.ToList();
			buttonsList.Sort(DataSortComparison);

			int offset = buttonsList[0].transform.parent.childCount - spawnedButtons.Count;

			for (int i = 0; i < buttonsList.Count; i++)
				buttonsList[i].transform.SetSiblingIndex(i + offset);
		}


		private void OnChangePage() =>
			OnPageChanged?.Invoke();
	}
}
