using UnityEngine;

namespace TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera
{
	public abstract class SlimeDeformCameraBase : IMeshDeformCamera
	{
		protected Camera Camera { get; }
		protected Transform Transform { get; }
		public Vector3 Normal => Transform.forward;

		public abstract Vector3 ScreenToWorldPoint(Vector2 point);

		public abstract Vector3 GetOrigin(Vector3 position);

		public abstract Vector3 GetDirection(Vector3 position);

		public SlimeDeformCameraBase(Camera camera)
		{
			Camera = camera;
			Transform = camera.transform;
		}
	}
}
