using UnityEngine;

namespace TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera
{
	public interface IMeshDeformCamera
	{
		Vector3 ScreenToWorldPoint(Vector2 point);

		Vector3 GetOrigin(Vector3 position);

		Vector3 GetDirection(Vector3 position);

		Vector3 Normal { get; }
	}
}
