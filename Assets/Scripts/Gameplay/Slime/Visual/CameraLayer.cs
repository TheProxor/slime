using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	public class CameraLayer : MonoBehaviour
	{
		[SerializeField] private Vector3 offset = default;

		private new Camera camera;



		[Inject]
		public void Construct(Camera camera)
		{
			this.camera = camera;
		}


		private void Start()
		{
			transform.position = camera.ScreenToWorldPoint(GetScreenPosition());
			transform.LookAt(camera.transform);
			transform.position += offset;
		}


		private Vector3 GetScreenPosition()
		{
			return new((float)Screen.width / 2, (float)Screen.height / 2, camera.nearClipPlane);
		}
	}
}
