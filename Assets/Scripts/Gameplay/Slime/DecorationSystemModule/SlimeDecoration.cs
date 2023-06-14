using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheProxor.SlimeSimulation.DecorationSystemModule
{
	[Serializable]
	public class SlimeDecoration
	{
		[SerializeField]
		private List<GameObject> prefabVariants = null;

		[SerializeField]
		private int count = 0;

		[SerializeField]
		private float dipping = 0;

		[SerializeField]
		[Range(0f, 180f)]
		private float angleVariation = 0;

		public IReadOnlyList<GameObject> PrefabVariants => prefabVariants;
		public int Count => count;
		public float Dipping => dipping;
		public float AngleVariation => angleVariation;
	}
}
