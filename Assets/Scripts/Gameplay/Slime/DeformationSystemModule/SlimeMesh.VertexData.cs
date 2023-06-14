using Unity.Mathematics;

namespace TheProxor.SlimeSimulation.DeformationSystemModule
{
	public partial class SlimeMesh
	{
		public struct VertexData
		{
			public readonly float3 OriginalPosition;
			public float3 Velocity;
			public float AccumulatedVelocity;

			public VertexData(float3 position)
			{
				OriginalPosition = position;
				Velocity = float3.zero;
				AccumulatedVelocity = 0;
			}
		}
	}
}
