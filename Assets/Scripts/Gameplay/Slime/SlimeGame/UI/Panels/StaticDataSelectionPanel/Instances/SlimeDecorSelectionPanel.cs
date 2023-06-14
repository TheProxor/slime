using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using UnityEngine;
using UnityEngine.UI;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeDecorSelectionPanel : DataSelectionPanel<DecorationStaticData, SlimeDecorationInfo>
	{
		public event Action OnDecorSkip;

		[SerializeField]
		private Button skipDecorAdditionButton = null;


		protected override Comparison<DataToggle<DecorationStaticData, SlimeDecorationInfo>> DataSortComparison => Compare;


		private int Compare(DataToggle<DecorationStaticData, SlimeDecorationInfo> x, DataToggle<DecorationStaticData, SlimeDecorationInfo> y)
		{
			if (ReferenceEquals(x, y))
				return 0;
			if (ReferenceEquals(null, y))
				return 1;
			if (ReferenceEquals(null, x))
				return -1;

			if (x.DataInfo.isAdsIngredient)
				return -1;

			if (y.DataInfo.isAdsIngredient)
				return 1;

			return y.DataInfo.ReceivingTime.CompareTo(x.DataInfo.ReceivingTime);
		}


		private void Start()
		{
			InitializeSkipDecorAdditionButton();
		}

		private void InitializeSkipDecorAdditionButton()
		{
			skipDecorAdditionButton.onClick.AddListener(SkipDecorAddition);
		}

		private void SkipDecorAddition()
		{
			OnDecorSkip?.Invoke();
		}
	}
}
