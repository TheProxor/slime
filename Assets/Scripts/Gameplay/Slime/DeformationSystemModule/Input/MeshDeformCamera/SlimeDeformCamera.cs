using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using UnityEngine;

namespace TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera
{
	public class SlimeDeformCamera : SlimeDeformCameraBase
	{
		private readonly SlimeMetaGameInstaller.SlimeCameraSettings slimeCameraSettings;


		public SlimeDeformCamera(Camera camera, SlimeMetaGameInstaller.SlimeCameraSettings slimeCameraSettings) : base(camera)
		{
			this.slimeCameraSettings = slimeCameraSettings;
		}


		public override Vector3 ScreenToWorldPoint(Vector2 point)
		{
			return Camera.ScreenToWorldPoint(new Vector3(point.x, point.y, Camera.nearClipPlane + slimeCameraSettings.SlimeCameraNearClipPlaneOffset));
		}


		public override Vector3 GetOrigin(Vector3 position)
		{
			return position;
		}


		public override Vector3 GetDirection(Vector3 position)
		{
			return Transform.forward;
		}
	}
}
