using System;
using System.Collections.Generic;
using TheProxor.SlimeSimulation.DeformationSystemModule;
using TheProxor.SlimeSimulation.ViewModule;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace TheProxor.SlimeSimulation.DecorationSystemModule
{
	public partial class SlimeDecorationSystem : IDisposable, ITickable
	{
		public class Factory : PlaceholderFactory<SlimeDecorationGroup, Action<GameObject>,
			SlimeDecorationSystem> {}

		private readonly SlimeMesh slimeMesh;
		private readonly SlimeView view;
		private readonly SlimeDecorationGroup decorationGroup;
		private readonly Action<GameObject> onDecorationCreate;
		private readonly List<GameObject> decorations = new();
		private readonly TickableManager tickableManager;

		private Mesh mesh;
		private NativeArray<TransformData> transformData;
		private TransformAccessArray transformAccessArray;
		private TransformsUpdateJob transformsUpdateJob;
		private JobHandle positionsUpdateJobHandle;

		public IReadOnlyList<GameObject> Decorations => decorations;


		public SlimeDecorationSystem(
			SlimeMesh slimeMesh,
			SlimeView view,
			SlimeDecorationGroup decorationGroup,
			TickableManager tickableManager,
			Action<GameObject> onDecorationCreate
		)
		{
			this.slimeMesh = slimeMesh;
			this.view = view;
			this.decorationGroup = decorationGroup;
			this.onDecorationCreate = onDecorationCreate;
			this.tickableManager = tickableManager;

			Initialize();
			InitializeSlimeMesh();

			tickableManager.Add(this);
		}


		public void Tick()
		{
			transformsUpdateJob.localToWorldMatrix = view.LocalToWorldMatrix;
		}


		public void Dispose()
		{
			tickableManager.Remove(this);

			FinishPositionsUpdateJob();
			DisposeData();
			DeInitializeTransformData();
			DeInitializeSlimeMesh();
			GC.SuppressFinalize(this);
		}


		public static Quaternion GetDecorationRotation(SlimeDecoration decoration)
		{
			float tiltAngle = Random.Range(0, decoration.AngleVariation);
			Vector3 axis = Random.insideUnitCircle.normalized * tiltAngle;
			axis.z = Random.Range(0, 360);

			return Quaternion.Euler(axis);
		}


		private static Vector2 GetRandomNormalizedSquarePoint()
		{
			Vector2 point = new(Random.value, Random.value);

			if (CheckPointNormalized(point))
			{
				point = ReflectPoint(point);
			}

			return point;
		}


		private static bool CheckPointNormalized(Vector2 point)
		{
			return point.x + point.y >= 1f;
		}


		private static Vector2 ReflectPoint(Vector2 point)
		{
			return Vector2.one - new Vector2(point.x, point.y);
		}


		private void InitializeSlimeMesh()
		{
			slimeMesh.OnUpdateVerticesJobScheduled += StartPositionsUpdateJob;
			slimeMesh.OnUpdateVerticesJobFinished += FinishPositionsUpdateJob;
			slimeMesh.OnDeformationInitialized += Initialize;
		}


		private void StartPositionsUpdateJob()
		{
			PreparePositionsUpdateJob();
			InitializeUpdateJobHandle();
		}


		private void FinishPositionsUpdateJob()
		{
			CompletePositionsUpdateJob();
		}


		private void Initialize()
		{
			InitializeMesh();
			InitializeTransformData();
			InitializeUpdateTransformsJob();
		}


		private void InitializeMesh()
		{
			mesh = slimeMesh.Mesh;
		}


		private void InitializeTransformData()
		{
			DeInitializeTransformData();

			var data = new List<TransformData>();
			var transforms = new List<Transform>();

			foreach (SlimeDecoration decoration in decorationGroup.Decorations)
			{
				AddDecoration(decoration, data, transforms);
			}

			transformData = new NativeArray<TransformData>(data.ToArray(), Allocator.Persistent);
			transformAccessArray = new TransformAccessArray(transforms.ToArray());
		}


		private void AddDecoration(
			SlimeDecoration decoration,
			List<TransformData> transformData,
			List<Transform> transforms
		)
		{
			int decorationCount = decoration.Count;

			int[] triangles = mesh.triangles;
			int trianglesCount = triangles.Length / 3;
			IReadOnlyList<GameObject> prefabs = decoration.PrefabVariants;
			int prefabsCount = prefabs.Count;

			for (var i = 0; i < decorationCount; i++)
			{
				int triangleIndex = Random.Range(0, trianglesCount) * 3;

				transformData.Add(
					CreateDecorationTransformData(
						decoration,
						triangleIndex,
						triangles
					)
				);

				GameObject prefab = decoration.PrefabVariants[Random.Range(0, prefabsCount)];
				transforms.Add(CreateDecoration(prefab));
			}
		}


		private TransformData CreateDecorationTransformData(
			SlimeDecoration decoration,
			int triangleIndex,
			int[] triangles
		)
		{
			return new()
			{
				Tris = new Vector3Int(
					triangles[triangleIndex],
					triangles[triangleIndex + 1],
					triangles[triangleIndex + 2]
				),
				TriNormalizedPosition = GetRandomNormalizedSquarePoint(),
				Dipping = decoration.Dipping,
				Rotation = GetDecorationRotation(decoration)
			};
		}


		private Transform CreateDecoration(GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab, view.RendererTransform);
			decorations.Add(gameObject);
			onDecorationCreate?.Invoke(gameObject);

			return gameObject.transform;
		}


		private void DeInitializeTransformData()
		{
			foreach (GameObject decoration in decorations)
			{
				Object.Destroy(decoration.gameObject);
			}

			decorations.Clear();
		}


		private void InitializeUpdateTransformsJob() =>
			transformsUpdateJob = new TransformsUpdateJob(
				slimeMesh.Vertices,
				transformData,
				view.LocalToWorldMatrix
			);


		private void PreparePositionsUpdateJob() =>
			transformsUpdateJob.Normals = GetNormals();


		private NativeArray<Vector3> GetNormals()
		{
			Vector3[] normals = GetMeshNormals();

			return new NativeArray<Vector3>(normals, Allocator.TempJob);
		}


		private Vector3[] GetMeshNormals() =>
			mesh == null ? new Vector3[0] : mesh.normals;


		private void InitializeUpdateJobHandle() =>
			positionsUpdateJobHandle = SchedulePositionsUpdateJob();


		private JobHandle SchedulePositionsUpdateJob() =>
			transformsUpdateJob.Schedule(transformAccessArray, slimeMesh.UpdateVerticesJobHandle);


		private void CompletePositionsUpdateJob() =>
			positionsUpdateJobHandle.Complete();


		private void DeInitializeSlimeMesh()
		{
			slimeMesh.OnUpdateVerticesJobScheduled -= StartPositionsUpdateJob;
			slimeMesh.OnUpdateVerticesJobFinished -= FinishPositionsUpdateJob;
			slimeMesh.OnDeformationInitialized -= Initialize;
		}


		private void DisposeData()
		{
			transformData.Dispose();
			transformAccessArray.Dispose();
		}
	}
}
