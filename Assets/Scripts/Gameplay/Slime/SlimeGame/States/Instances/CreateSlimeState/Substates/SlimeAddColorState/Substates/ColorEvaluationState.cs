using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.Services.Audio;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class ColorEvaluationState : EvaluatedProgressState
	{
		[Serializable]
		public new class Settings : EvaluatedProgressState.Settings
		{
			[SerializeField]
			private AnimationCurve animationCurve;

			public AnimationCurve AnimationCurve => animationCurve;
		}

		private readonly SlimeFacade slime;
		private readonly Settings settings;
		private readonly CreationInput creationInput;

		private Guid? loopedSoundGuid;

		private Color from;
		private Color to;


		public ColorEvaluationState(
				IAudioService<SoundId> audioService,
				SlimeFacade slime,
				IProgressEvaluator progressEvaluator,
				CreationInput creationInput,
				Settings settings
			) :
			base(progressEvaluator, audioService, settings)
		{
			this.slime = slime;
			this.settings = settings;
			this.creationInput = creationInput;
		}


		public void Init(Color from, Color to)
		{
			this.from = from;
			this.to = to;
		}


		public override void Exit()
		{
			base.Exit();
			StopSound();
			creationInput.Reset();
		}


		protected override void Evaluate(float evaluation)
		{
			base.Evaluate(evaluation);
			UpdateColor();
			HandleSound(creationInput.Value > 0);
		}


		private void HandleSound(bool isEvaluating)
		{
			if (isEvaluating)
				PlaySound();
			else
				StopSound();
		}


		private void PlaySound()
		{
			if (loopedSoundGuid != null)
				return;

			loopedSoundGuid = AudioService.PlaySoundLoop(SoundId.slime_kneading_activator);
		}


		private void StopSound()
		{
			if (loopedSoundGuid == null)
				return;

			AudioService.Destroy(loopedSoundGuid.Value);
			loopedSoundGuid = null;
		}

		private void UpdateColor()
		{
			float t = settings.AnimationCurve.Evaluate(Progress);
			slime.Color = Color.Lerp(from, to, t);
		}
	}
}
