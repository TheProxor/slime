using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.DecorationsPlacer
{
	public class DecorationsPowderAppearEffect : DecorationsAppearEffect, ITickable
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private float moveSpeed;

			[SerializeField]
			private AnimationCurve moveCurve;

			public float MoveSpeed => moveSpeed;
			public AnimationCurve MoveCurve => moveCurve;
		}

		public struct MovingDecorData
		{
			private readonly Transform targetTransform;

			public readonly GameObject Target;
			public readonly GameObject Clone;
			public readonly float Time;

			public float Timer;

			public Vector3 TargetPosition => targetTransform.position;
			public Quaternion TargetRotation => targetTransform.rotation;
			public Transform CloneTransform { get; }
			public Vector3 StartPosition { get; }


			public MovingDecorData(
				GameObject target,
				GameObject clone,
				Vector3 startPosition,
				float time
			)
			{
				Target = target;
				Clone = clone;
				CloneTransform = clone.transform;
				targetTransform = target.transform;
				StartPosition = startPosition;
				Time = time;
				Timer = 0;
			}
		}

		private readonly TickableManager tickableManager;
		private readonly Camera camera;
		private readonly CreationInput input;
		private readonly SlimeFacade slime;
		private readonly Settings settings;
		private readonly List<MovingDecorData> movingDecorData = new();

		private float movementPlaneDistance;
		private Plane movementPlane;


		public DecorationsPowderAppearEffect(
				TickableManager tickableManager,
			Camera camera,
			CreationInput input,
			SlimeFacade slime,
			Settings settings
		)
		{
			this.tickableManager = tickableManager;
			this.camera = camera;
			this.input = input;
			this.slime = slime;
			this.settings = settings;

			InitializeMovementPlane();
			InitializeSceneUpdateService();
		}


		public void Place(GameObject decoration)
		{
			GameObject clone = Object.Instantiate(decoration);
			clone.SetActive(true);
			movingDecorData.Add(CreateMovingDecorData(decoration, clone));
		}


		public override void Stop()
		{
			DeInitializeUpdateService();
			base.Stop();
		}


		// TODO: my code, check it
		private void DeInitializeUpdateService()
		{
			tickableManager.Remove(this);
		}


		private void InitializeMovementPlane()
		{
			Transform cameraTransform = camera.transform;
			Vector3 cameraPosition = cameraTransform.position;
			Vector3 inNormal = cameraTransform.forward;

			movementPlaneDistance = Vector3.Distance(slime.Position, cameraPosition) - slime.Size;

			Vector3 inPoint = cameraPosition + inNormal * movementPlaneDistance;
			movementPlane = new Plane(inNormal, inPoint);
		}


		private void InitializeSceneUpdateService()
		{
			// TODO: create RemoveTickable
			tickableManager.Add(this);
		}


		private MovingDecorData CreateMovingDecorData(GameObject decoration, GameObject clone)
		{
			Vector3 targetPosition = WorldPositionToMovementPlane(decoration.transform.position);
			Vector3 startPosition = GetStartPosition();
			float distance = Vector3.Distance(startPosition, targetPosition);
			float moveTime = distance / settings.MoveSpeed;

			return new MovingDecorData(decoration, clone, startPosition, moveTime);
		}


		private Vector3 GetStartPosition()
		{
			Vector2 mousePosition = input.MousePosition;
			var position = new Vector3(mousePosition.x, mousePosition.y, movementPlaneDistance);

			return camera.ScreenToWorldPoint(position);
		}


		private Vector3 WorldPositionToMovementPlane(Vector3 position)
		{
			return movementPlane.ClosestPointOnPlane(position);
		}


		void ITickable.Tick()
		{
			for (int i = movingDecorData.Count - 1; i >= 0; i--)
			{
				MovingDecorData data = UpdateTransformData(
					movingDecorData[i],
					out bool isPlaced
				);

				if (isPlaced)
				{
					Place(data, i);

					continue;
				}

				movingDecorData[i] = data;
			}
		}


		private void Place(MovingDecorData data, int i)
		{
			base.Appear(data.Target);
			Object.Destroy(data.Clone);
			movingDecorData.RemoveAt(i);
		}


		private MovingDecorData UpdateTransformData(MovingDecorData data, out bool isPlaced)
		{
			Transform transform = data.CloneTransform;

			float timer = data.Timer;
			float time = data.Time;

			float t = settings.MoveCurve.Evaluate(timer / time);

			Vector3 targetPosition = WorldPositionToMovementPlane(data.TargetPosition);
			transform.position = Vector3.Lerp(data.StartPosition, targetPosition, t);
			transform.rotation = data.TargetRotation;

			timer += Time.deltaTime;
			isPlaced = timer >= time;
			data.Timer = timer;

			return data;
		}
	}
}
