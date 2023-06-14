using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class ProgressEvaluator : IProgressEvaluator, ITickable
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private float smoothTime;

			public float SmoothTime => smoothTime;
		}

		public event Action<float> OnEvaluate;

		private readonly CreationInput input;
		private readonly Settings settings;

		private float evaluation;
		private float velocity;

		public ProgressEvaluator(CreationInput input, Settings settings)
		{
			this.input = input;
			this.settings = settings;
			InitializeInput();
		}

		public void Tick()
		{
			UpdateEvaluation(input.Value);
			Evaluate(evaluation * Time.deltaTime);
		}

		private void InitializeInput()
		{
			input.OnInteractionFinish += ResetEvaluation;
		}

		private void ResetEvaluation()
		{
			velocity = 0;
			evaluation = 0;
		}

		private void UpdateEvaluation(float target)
		{
			evaluation = Mathf.SmoothDamp(evaluation, target, ref velocity, settings.SmoothTime);
		}

		private void Evaluate(float settingsEvaluationSpeed)
		{
			OnEvaluate?.Invoke(settingsEvaluationSpeed);
		}
	}
}
