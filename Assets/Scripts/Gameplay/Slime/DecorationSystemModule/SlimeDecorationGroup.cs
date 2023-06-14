using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheProxor.SlimeSimulation.DecorationSystemModule
{
	[Serializable]
	public class SlimeDecorationGroup
	{
		[SerializeField]
		private List<SlimeDecoration> decorations = null;

		public IReadOnlyList<SlimeDecoration> Decorations => decorations;
	}
}
