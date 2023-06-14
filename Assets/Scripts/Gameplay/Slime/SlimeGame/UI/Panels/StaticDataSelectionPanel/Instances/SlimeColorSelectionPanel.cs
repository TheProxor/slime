using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.UI
{
	public class SlimeColorSelectionPanel : DataSelectionPanel<ColorStaticData, SlimeColorInfo>
	{
		protected override Comparison<DataToggle<ColorStaticData, SlimeColorInfo>> DataSortComparison => Compare;


		private int Compare(DataToggle<ColorStaticData, SlimeColorInfo> x, DataToggle<ColorStaticData, SlimeColorInfo> y)
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
