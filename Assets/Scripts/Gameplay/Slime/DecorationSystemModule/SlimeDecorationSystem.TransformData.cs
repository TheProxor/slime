using UnityEngine;

namespace TheProxor.SlimeSimulation.DecorationSystemModule
{
	public partial class SlimeDecorationSystem
	{
		public struct TransformData
		{
			public Vector3Int Tris;
			public Vector2 TriNormalizedPosition;
			public float Dipping;
			public Quaternion Rotation;
		}
	}
}
