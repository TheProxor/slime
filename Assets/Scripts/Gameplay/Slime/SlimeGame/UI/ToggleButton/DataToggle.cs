using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public abstract class DataToggle<TData, TDataInfo> : MonoBehaviour
	{
		public class Pool : MemoryPool<TData, TDataInfo, DataToggle<TData, TDataInfo>>
		{
			protected override void OnCreated(DataToggle<TData, TDataInfo> button)
			{
				OnDespawned(button);
			}

			protected override void OnSpawned(DataToggle<TData, TDataInfo> button)
			{
				button.gameObject.SetActive(true);
			}

			protected override void OnDespawned(DataToggle<TData, TDataInfo> button)
			{
				button.gameObject.SetActive(true);
			}

			protected override void Reinitialize(TData data, TDataInfo dataInfo, DataToggle<TData, TDataInfo> button)
			{
				button.ApplyData(data, dataInfo);
			}
		}

		public event Action<TData, TDataInfo, int> OnToggled;

		[SerializeField]
		protected Button button = null;

		protected bool isActive;


		public TData Data { get; protected set; }
		public TDataInfo DataInfo { get; protected set; }


		public virtual void RefreshVisual() {}

		protected virtual void ApplyDataInner(TData staticData, TDataInfo dataInfo) {}


		private void Start()
		{
			button.onClick.AddListener(Select);
		}


		private void Select()
		{
			OnToggled?.Invoke(Data, DataInfo, 0);
		}

		private void ApplyData(TData data, TDataInfo dataInfo)
		{
			this.Data = data;
			this.DataInfo = dataInfo;

			ApplyDataInner(data, dataInfo);
		}
	}
}
