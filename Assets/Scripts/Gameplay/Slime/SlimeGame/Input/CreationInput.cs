using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input
{
	public class CreationInput : ITickable
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private float minCursorSpeed = 0.25f;

			public float MinCursorSpeed => minCursorSpeed;
		}

		public event Action OnInteractionStart;
		public event Action OnInteractionFinish;
		public event Action OnCursorMoving;

		private const KeyCode KEY_CODE = KeyCode.Mouse0;

		private readonly Settings settings;
		private readonly SlimeFacade slime;
		private readonly Camera camera;
		private readonly InputControls inputControls;

		private bool isEnabled;

		public bool IsEnabled
		{
			get => isEnabled;
			set
			{
				isEnabled = value;

				if (!isEnabled)
				{
					StopInteraction();
				}
			}
		}
		public bool IsInteract { get; private set; }
		public float Value { get; private set; }
		public Vector2 MousePosition { get; private set; }

		public CreationInput(SlimeFacade slime,
							 Camera camera,
							 Settings settings)
		{
			this.slime = slime;
			this.camera = camera;
			this.settings = settings;

			inputControls = new InputControls();
			inputControls.Base.PointerPosition.performed += PointerPositionOnperformed;
		}


		private void PointerPositionOnperformed(InputAction.CallbackContext obj)
		{
			Tick();
		}


		public void Reset()
		{
			StopInteraction();
		}

		public void Tick()
		{
			if (!isEnabled)
			{
				return;
			}

			if (!IsInteract)
			{
				Value = 0;

				if (IsPressOnSlime())
				{
					StartInteraction();
				}
			}
			else
			{
				if (IsPointerUp())
				{
					StopInteraction();
				}

				if (IsMouseMoving())
				{
					OnCursorMoving?.Invoke();
					Value = 1;
				}
				else
				{
					Value = 0;
				}
			}
		}


		private bool IsPressOnSlime()
		{
			return inputControls.Base.OnPointerTap.triggered
				&& IsPositionInSlime(GetMousePosition());
		}


		private bool IsPointerUp() {
			return !inputControls.Base.OnPointerTap.triggered;
		}

		private bool IsPositionInSlime(Vector2 mousePosition)
		{
			Vector3 slimePosition = slime.Position;
			Vector3 cameraPosition = camera.transform.position;

			Vector3 worldPoint = ScreenToWorldPoint(mousePosition, slimePosition, cameraPosition);
			float distance = Vector3.Distance(slimePosition, worldPoint);

			return distance < slime.Radius;
		}

		private Vector3 ScreenToWorldPoint(Vector2 mousePosition,
										   Vector3 slimePosition,
										   Vector3 cameraPosition)
		{
			float planeDistance = (slimePosition - cameraPosition).magnitude;
			var position = new Vector3(mousePosition.x, mousePosition.y, planeDistance);

			return camera.ScreenToWorldPoint(position);
		}

		private void UpdateMousePosition(Vector2 position)
		{
			MousePosition = position;
		}

		private void StartInteraction()
		{
			IsInteract = true;
			UpdateMousePosition(GetMousePosition());
			OnInteractionStart?.Invoke();
		}

		private void StopInteraction()
		{
			Value = 0;
			IsInteract = false;
			OnInteractionFinish?.Invoke();
		}

		private bool IsMouseMoving()
		{
			Vector2 mousePosition = GetMousePosition();
			float speed = GetMouseSpeed(mousePosition);
			UpdateMousePosition(mousePosition);

			return speed > settings.MinCursorSpeed;
		}

		private float GetMouseSpeed(Vector2 mousePosition)
		{
			return (mousePosition - MousePosition).magnitude / Time.deltaTime;
		}

		private  Vector2 GetMousePosition() =>
			inputControls.Base.PointerPosition.ReadValue<Vector2>();
	}
}
