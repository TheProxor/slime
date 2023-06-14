using TheProxor.SlimeSimulation.DeformationSystemModule.Input;
using UnityEngine;

namespace TheProxor.SlimeSimulation.ViewModule
{
	public class SlimeView
	{
		private readonly MeshRenderer meshRenderer = default;
		private readonly MeshFilter meshFilter = default;

		public Transform RendererTransform { get; private set; }

		public Vector3 Position
		{
			get => RendererTransform.position;
			set => RendererTransform.position = value;
		}

		public Quaternion Rotation => RendererTransform.rotation;

		public Mesh Mesh
		{
			get => meshFilter.mesh;
			set => meshFilter.mesh = value;
		}

		public Material Material
		{
			get => meshRenderer.material;
			set => meshRenderer.material = value;
		}

		public bool Enabled
		{
			get => meshRenderer.enabled;
			set => meshRenderer.enabled = value;
		}

		public Matrix4x4 LocalToWorldMatrix => RendererTransform.localToWorldMatrix;
		public Matrix4x4 WorldToLocalMatrix => RendererTransform.worldToLocalMatrix;
		public Vector3 Size => Vector3.Scale(Mesh.bounds.size, RendererTransform.localScale);

		public SlimeView(MeshRenderer meshRenderer, MeshFilter meshFilter, MeshDeformInput input, Vector3 position)
		{
			this.meshRenderer = meshRenderer;
			this.meshFilter = meshFilter;
			InitializeRendererTransform();

			Position = position;
		}

		private void InitializeRendererTransform() =>
			RendererTransform = meshRenderer.transform;
	}
}
