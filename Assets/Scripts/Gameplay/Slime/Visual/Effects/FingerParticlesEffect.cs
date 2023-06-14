using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	public class FingerParticlesEffect : Effect
	{
		[SerializeField]
		private bool keepAliveUntilParticlesAlive = true;

		[SerializeField]
		private ParticleSystem particles = null;

		private new Camera camera;
		private CreationInput input;

		protected override bool IsCanBeDisposed => !keepAliveUntilParticlesAlive
												   || !particles.IsAlive();

		[Inject]
		public void Construct(Camera camera, CreationInput input)
		{
			this.camera = camera;
			this.input = input;
			InitializeInput();
		}

		protected override void DisposeInner(bool immediate)
		{
			Disable();
			DeInitializeInput();
		}

		private void Start()
		{
			Disable();
		}

		private void InitializeInput()
		{
			UpdateEnabled();

			input.OnInteractionStart += Enable;
			input.OnInteractionFinish += Disable;
			input.OnCursorMoving += UpdatePosition;
		}

		private void UpdateEnabled()
		{
			if (input.IsInteract)
			{
				Enable();
			}
			else
			{
				Disable();
			}
		}

		private void DeInitializeInput()
		{
			input.OnInteractionStart -= Enable;
			input.OnInteractionFinish -= Disable;
			input.OnCursorMoving -= UpdatePosition;
		}

		private void Enable()
		{
			UpdatePosition();
			particles.Play();
		}

		private void Disable()
		{
			particles.Stop();
		}

		private void UpdatePosition()
		{
			transform.position = GetWorldPosition();
		}

		private Vector3 GetWorldPosition()
		{
			return camera.ScreenToWorldPoint(GetMousePosition());
		}

		private Vector3 GetMousePosition()
		{
			Vector3 position = input.MousePosition;
			position.z = camera.nearClipPlane;

			return position;
		}
	}
}
