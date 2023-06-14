using System;
using System.Collections.Generic;
using System.Linq;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input;
using TheProxor.SlimeSimulation.ViewModule;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Zenject;


namespace TheProxor.SlimeSimulation.DeformationSystemModule
{
	public partial class SlimeMesh : ITickable, ILateTickable, IDisposable
	{
		[Serializable]
		public struct Settings
		{
			public float ShiftForce;

			public float ShiftSmooth;

			[Range(0.0f, 1.0f)]
			public float ShiftAccumulationFactor;

			public float ShiftAccumulationDecreaseSpeed;

			public float PushForce;

			public float PushRadius;

			public float PushDepthFactor;

			public float PressSpeedFactor;

			public float SpringForce;

			public float Damping;

			public float MaxShiftVelocity;
		}

		public event Action OnUpdateVerticesJobScheduled;
		public event Action OnUpdateVerticesJobFinished;
		public event Action OnDeformationInitialized;

		[Range(1, 128)]
		private readonly int updateVerticesJobBatchSize = 1;
		private readonly MeshDeformInput input;
		private readonly SlimeView view;
		private readonly TickableManager tickableManager;

		private bool enabled = true;
		private Mesh mesh;
		private int verticesCount;
		private NativeArray<float3> vertices;
		private NativeArray<VertexData> verticesData;
		private UpdateVerticesJob updateVerticesJob;
		private JobHandle updateVerticesJobHandle;
		private float timeScale;
		private bool isReset;

		public bool Enabled
		{
			get => enabled;
			set
			{
				if (enabled == value)
					return;

				enabled = value;

				if (enabled)
					InitializeUpdater();
				else
					DeInitializeUpdater();
			}
		}

		public Mesh Mesh
		{
			get => mesh;
			set
			{
				mesh = value;
				PrepareMesh();
				InitializeVertices();
				InitializeVerticesData();
				InitializeUpdateVerticesJob();
			}
		}

		public NativeArray<float3> Vertices => vertices;
		public JobHandle UpdateVerticesJobHandle => updateVerticesJobHandle;
		public bool IsInteractionEnabled { get; set; }
		public Settings DeformationSettings { get; set; }


		public SlimeMesh(
			MeshDeformInput input,
			SlimeView view,
			TickableManager tickableManager
		)
		{
			this.input = input;
			this.view = view;
			this.tickableManager = tickableManager;
			InitializeUpdater();
		}


		public void SetDeformationEnabled(bool isEnabled) =>
			updateVerticesJob.AllowDeformation = isEnabled;


		private void InitializeUpdater()
		{
			try
			{
				tickableManager.Add(this);
				tickableManager.AddLate(this);
			}
			catch (ZenjectException e)
			{
				Debug.LogError(e.Message);
			}
		}


		private void DeInitializeUpdater()
		{
			try
			{
				tickableManager.Remove(this);
				tickableManager.RemoveLate(this);
			}
			catch (ZenjectException e)
			{
				Debug.LogError(e.Message);
			}
		}


		~SlimeMesh()
		{
			FinalizeUpdateVerticesJob();
			DeInitializeVertices();
			DeInitializeVerticesData();
		}


		public void Dispose()
		{
			FinalizeUpdateVerticesJob();
			DeInitializeVertices();
			DeInitializeVerticesData();
		}


		void ITickable.Tick()
		{
			CreateAndRunUpdateVerticesJob();
			OnUpdateVerticesJobScheduled?.Invoke();


			void CreateAndRunUpdateVerticesJob()
			{
				PrepareUpdateVerticesJob();
				InitializeUpdateVerticesJobHandle();
			}

			void PrepareUpdateVerticesJob()
			{
				updateVerticesJob.DeltaTime = Time.deltaTime;
				updateVerticesJob.Settings = DeformationSettings;
				updateVerticesJob.Interactions = GetJobInteractionsArray();
			}

			void InitializeUpdateVerticesJobHandle() =>
				updateVerticesJobHandle = ScheduleUpdateVerticesJob();
		}


		void ILateTickable.LateTick()
		{
			FinalizeUpdateVerticesJob();
			UpdateMesh();
			OnUpdateVerticesJobFinished?.Invoke();
			isReset = false;
		}


		private void PrepareMesh()
		{
			if (Mesh == null)
			{
				return;
			}

			Mesh.MarkDynamic();
		}


		private void InitializeVertices()
		{
			DeInitializeVertices();
			vertices = new NativeArray<float3>(GetMeshVertices(), Allocator.Persistent);
			verticesCount = vertices.Length;
		}


		private void DeInitializeVertices()
		{
			try
			{
				vertices.Dispose();
			}
			catch (ObjectDisposedException)
			{
				// ignored
			}
		}


		private float3[] GetMeshVertices()
		{
			return Mesh == null
				? new float3[0]
				: Mesh.vertices.Select((v) => (float3) v).ToArray();
		}


		private void InitializeVerticesData()
		{
			DeInitializeVerticesData();
			verticesData = new NativeArray<VertexData>(verticesCount, Allocator.Persistent);

			for (var i = 0; i < verticesCount; i++)
			{
				verticesData[i] = new VertexData(vertices[i]);
			}
		}


		private void DeInitializeVerticesData()
		{
			try
			{
				verticesData.Dispose();
			}
			catch (ObjectDisposedException)
			{
				// ignored
			}
		}


		private void InitializeUpdateVerticesJob()
		{
			updateVerticesJob = new UpdateVerticesJob(vertices, verticesData, DeformationSettings);
			OnDeformationInitialized?.Invoke();
		}


		private NativeArray<UpdateVerticesJob.Interaction> GetJobInteractionsArray()
		{
			Vector3 normal = input.Normal;
			IReadOnlyCollection<Interaction> interactions = input.Interactions;

			int length = IsInteractionEnabled ? interactions.Count : 0;

			const Allocator TEMP_JOB = Allocator.TempJob;
			var jobInteractions = new NativeArray<UpdateVerticesJob.Interaction>(length, TEMP_JOB);

			for (var i = 0; i < length; i++)
			{
				Interaction interaction = interactions.ElementAt(i);
				jobInteractions[i] = new UpdateVerticesJob.Interaction(normal, interaction, view);
			}

			return jobInteractions;
		}


		private JobHandle ScheduleUpdateVerticesJob()
		{
			return updateVerticesJob.Schedule(verticesCount, updateVerticesJobBatchSize);
		}


		private void FinalizeUpdateVerticesJob()
		{
			updateVerticesJobHandle.Complete();
		}


		private void UpdateMesh()
		{
			if (Mesh == null)
			{
				return;
			}

			UpdateMeshVertices();
			RecalculateMeshNormals();
		}


		private void UpdateMeshVertices()
		{
			Mesh.SetVertices(vertices);
		}


		private void RecalculateMeshNormals()
		{
			Mesh.RecalculateNormals();
		}
	}
}
