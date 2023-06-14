using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeBasisSelectionPanel : DataSelectionPanel<BasisStaticData, SlimeBasisInfo>
	{
		protected override Comparison<DataToggle<BasisStaticData, SlimeBasisInfo>> DataSortComparison => Compare;


		private int Compare(DataToggle<BasisStaticData, SlimeBasisInfo> x, DataToggle<BasisStaticData, SlimeBasisInfo> y)
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
	}
}
