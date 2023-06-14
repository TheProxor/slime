using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem
{
	[Serializable]
	public struct SlimeSaveData : IEquatable<SlimeSaveData>
	{
		public string Id;
		public BasisType Basis;
		public ColorData Color;
		public string[] Decorations;
		public float PlayProgress;



		public bool Equals(SlimeSaveData other)
		{
			return this.Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			return obj is SlimeSaveData other && Equals(other);
		}

		public static bool operator ==(SlimeSaveData left, SlimeSaveData right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SlimeSaveData left, SlimeSaveData right)
		{
			return !left.Equals(right);
		}
	}
}
