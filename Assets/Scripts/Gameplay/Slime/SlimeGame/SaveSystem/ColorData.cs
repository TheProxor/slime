using System;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem
{
	[Serializable]
	public struct ColorData
	{
		public float r, g, b, a;

		public ColorData(Color color)
		{
			r = color.r;
			g = color.g;
			b = color.b;
			a = color.a;
		}

		public static implicit operator Color(ColorData data)
		{
			return new(data.r, data.g, data.b, data.a);
		}
	}
}
