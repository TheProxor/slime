using System;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera;
using TheProxor.SlimeSimulation.InputSystem;
using UnityEngine;
using Zenject;

namespace TheProxor.SlimeSimulation.DeformationSystemModule.Input
{
	public class Interaction : IDisposable
	{
		public class Pool : MemoryPool<ITouch, Interaction>
		{
			protected override void Reinitialize(ITouch touch, Interaction interaction)
			{
				interaction.InitializeTouch(touch);
			}

			protected override void OnDespawned(Interaction interaction)
			{
				interaction.DeInitializeTouch();
			}

			protected override void OnDestroyed(Interaction interaction)
			{
				interaction.Dispose();
			}
		}

		[Serializable]
		public class Settings
		{
			[SerializeField]
			public float smoothTime = 0.05f;

			[SerializeField]
			public float maxPointerSpeed = 100;

			public float SmoothTime => smoothTime;
			public float MaxPointerSpeed => maxPointerSpeed;
		}

		private readonly IMeshDeformCamera camera;

		private Settings settings;

		private ITouch touch;
		private Vector3 position;
		private Vector3 lastPosition;
		private Vector3 velocity;

		public Vector3 Origin { get; private set; }
		public Vector3 Direction { get; private set; }
		public Vector3 Shift { get; private set; }
		public float ShiftSpeed { get; private set; }
		public float PressIntensity { get; private set; }

		public Interaction(Settings settings, IMeshDeformCamera camera)
		{
			this.settings = settings;
			this.camera = camera;
		}

		~Interaction()
		{
			DeInitializeTouch();
		}

		public void Dispose()
		{
			DeInitializeTouch();
			GC.SuppressFinalize(this);
		}

		private void InitializeTouch(ITouch touch)
		{
			this.touch = touch;
			UpdatePosition(ScreenToWorldPoint(touch.Position));
			UpdateLastPosition();
			UpdateInteraction();
			touch.OnPositionChanged += Update;
		}

		private void UpdatePosition(Vector3 position)
		{
			this.position = position;
		}

		private Vector3 ScreenToWorldPoint(Vector2 position)
		{
			return camera.ScreenToWorldPoint(position);
		}

		private void UpdateLastPosition()
		{
			lastPosition = position;
		}

		private void DeInitializeTouch()
		{
			if (touch == null)
				return;

			touch.OnPositionChanged -= Update;
		}

		private void Update(Vector2 position)
		{
			Vector3 worldPoint = ScreenToWorldPoint(position);
			SmoothUpdatePosition(worldPoint);
			UpdateInteraction();
			UpdateLastPosition();
		}

		private void UpdateInteraction()
		{
			Origin = camera.GetOrigin(position);
			Direction = camera.GetDirection(position);
			Shift = GetPointerDelta();
			ShiftSpeed = GetShiftSpeed();
			PressIntensity = 1;
		}

		private Vector3 GetPointerDelta()
		{
			return position - lastPosition;
		}

		private float GetShiftSpeed()
		{
			return velocity.magnitude;
		}

		private void SmoothUpdatePosition(Vector3 position)
		{
			this.position = Vector3.SmoothDamp(this.position,
											   position,
											   ref velocity,
											   settings.SmoothTime,
											   settings.MaxPointerSpeed);
		}
	}
}
