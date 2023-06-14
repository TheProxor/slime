using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace TheProxor.SlimeSimulation.DecorationSystemModule
{
	public partial class SlimeDecorationSystem
	{
		[BurstCompile]
		private struct TransformsUpdateJob : IJobParallelForTransform
		{
			[ReadOnly]
			private readonly NativeArray<float3> vertices;

			[ReadOnly]
			private readonly NativeArray<TransformData> transformData;

			[ReadOnly]
			[DeallocateOnJobCompletion]
			private NativeArray<Vector3> normals;

			public Matrix4x4 localToWorldMatrix;

			public NativeArray<Vector3> Normals
			{
				set => normals = value;
			}

			public TransformsUpdateJob(NativeArray<float3> vertices,
									   NativeArray<TransformData> transformData,
									   Matrix4x4 localToWorldMatrix)
			{
				this.vertices = vertices;
				this.transformData = transformData;
				this.localToWorldMatrix = localToWorldMatrix;

				normals = default;
			}

			public void Execute(int index, TransformAccess transform)
			{
				TransformData data = transformData[index];

				Vector3Int tris = data.Tris;

				int trisX = tris.x;
				int trisY = tris.y;
				int trisZ = tris.z;

				Vector2 triNormalizedPosition = data.TriNormalizedPosition;


				float normalizedX = triNormalizedPosition.x;
				float normalizedY = triNormalizedPosition.y;

				Vector3 position = vertices[trisX]
								   + (normalizedX * (vertices[trisY] - vertices[trisX]))
								   + (normalizedY * (vertices[trisZ] - vertices[trisX]));

				Vector3 normal = (normals[trisX]
								  + triNormalizedPosition.x * normals[trisY]
								  + triNormalizedPosition.y * normals[trisZ])
								 / 3;

				transform.position = GetPosition(position, normal, data.Dipping);
				transform.rotation = GetRotation(data.Rotation, normal);
			}

			private Vector3 GetPosition(Vector3 vertex, Vector3 normal, float dipping)
			{
				return localToWorldMatrix.MultiplyPoint(vertex - dipping * normal);
			}

			private Quaternion GetRotation(Quaternion rotation, Vector3 normal)
			{
				Vector3 forward = localToWorldMatrix.MultiplyVector(normal);

				Quaternion fromTo = Quaternion.FromToRotation(Vector3.forward, forward);

				return fromTo * rotation;
			}
		}
	}
}
