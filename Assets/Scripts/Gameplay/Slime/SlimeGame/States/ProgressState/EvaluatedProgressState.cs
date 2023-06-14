using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.Services.Audio;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class EvaluatedProgressState : ProgressState
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private float duration = 1;

			public float Duration => duration;
		}

		protected readonly IAudioService<SoundId> AudioService;
		private readonly IProgressEvaluator progressEvaluator;
		private readonly Settings settings;



		public EvaluatedProgressState(IProgressEvaluator progressEvaluator, IAudioService<SoundId> audioService, Settings settings)
		{
			this.progressEvaluator = progressEvaluator;
			this.AudioService = audioService;
			this.settings = settings;
		}


		public override void Enter()
		{
			base.Enter();
			InitializeProgressEvaluator();
		}


		public override void Exit()
		{
			AudioService.PlayOneShot(SoundId.slime_checkpoint_checkmark);
			DeInitializeProgressEvaluator();
		}


		protected virtual void Evaluate(float evaluation)
		{
			Progress += evaluation / settings.Duration;
		}


		private void InitializeProgressEvaluator()
		{
			progressEvaluator.OnEvaluate += Evaluate;
		}


		private void DeInitializeProgressEvaluator()
		{
			progressEvaluator.OnEvaluate -= Evaluate;
		}
	}
}
