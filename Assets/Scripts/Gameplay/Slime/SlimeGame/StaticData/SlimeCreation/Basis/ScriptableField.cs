using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	public class ScriptableField : PropertyAttribute
	{
		public Type Type { get; }

		public ScriptableField([NotNull] Type type)
		{
			Type = type;
		}
	}
}
