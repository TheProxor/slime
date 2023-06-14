using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.SlimeSimulation.DeformationSystemModule;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public partial class BowlFacade
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private SlimeMesh.Settings slimeDeformationSettings;

			[SerializeField]
			private AnimationCurve pushForceByFillCurve;

			public AnimationCurve PushForceByFillCurve => pushForceByFillCurve;
			public SlimeMesh.Settings SlimeDeformationSettings => slimeDeformationSettings;

			[field: SerializeField] public Vector3 SlimeDepthOffset { get; private set; } = default;
		}

		private readonly GameObject gameObject;
		private readonly Transform transform;
		private readonly SlimeFacade slime;
		private readonly SlimeInteractionController slimeInteractionController;
		private readonly Settings settings;

		private float fill;

		public Transform Transform => transform;

		public float Fill
		{
			get => fill;
			set
			{
				fill = value;
				UpdateDeformationSettings();
				UpdateBowlTransform();
				UpdateSlimeTransform();
			}
		}

		public bool IsActive { get; private set; }

		public BowlFacade(Transform transform,
						  SlimeFacade slime,
						  SlimeInteractionController slimeInteractionController,
						  Settings settings,
						  float fill = 1)
		{
			this.transform = transform;
			this.slime = slime;
			this.slimeInteractionController = slimeInteractionController;
			this.settings = settings;
			gameObject = transform.gameObject;
			UpdateTransformDirection();
			SetInteractionControllerEnabled(true);
			Fill = fill;
		}

		public void SetActive(bool value)
		{
			SetActiveGameObject(value);
			SetInteractionControllerEnabled(value);
			IsActive = value;
		}

		private void SetActiveGameObject(bool value)
		{
			gameObject.SetActive(value);
		}

		private void SetInteractionControllerEnabled(bool value)
		{
			slimeInteractionController.IsEnabled = value;
		}

		private void UpdateDeformationSettings()
		{
			SlimeMesh.Settings slimeDeformationSettings = settings.SlimeDeformationSettings;
			slimeDeformationSettings.PushForce = EvaluatePushForce(slimeDeformationSettings);
			slime.DeformationSettings = slimeDeformationSettings;
		}

		private float EvaluatePushForce(SlimeMesh.Settings slimeDeformationSettings)
		{
			float t = settings.PushForceByFillCurve.Evaluate(fill);
			float pushForce = Mathf.Lerp(0, slimeDeformationSettings.PushForce, t);

			return pushForce;
		}

		private void UpdateBowlTransform() =>
			transform.position = slime.SlimeSettings.SlimeSpawnPosition;

		private void UpdateSlimeTransform() =>
			slime.Position = GetFillPosition(fill, slime.Height);

		private void UpdateTransformDirection()
		{
			transform.rotation = slime.Rotation;
		}

		private Vector3 GetFillPosition(float fill, float depth)
		{
			return Vector3.MoveTowards(slime.SlimeSettings.SlimeSpawnPosition - settings.SlimeDepthOffset,
			slime.SlimeSettings.SlimeSpawnPosition,
				fill);
		}
	}
}
