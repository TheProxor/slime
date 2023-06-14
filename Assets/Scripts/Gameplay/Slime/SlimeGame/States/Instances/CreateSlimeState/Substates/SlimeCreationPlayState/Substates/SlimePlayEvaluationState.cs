using System;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.Services.Audio;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class SlimePlayEvaluationState : EvaluatedProgressState
	{
		[Serializable]
		public new class Settings : EvaluatedProgressState.Settings {}




		private readonly CreationInput creationInput;
		private Guid? loopedSoundGuid;


		public SlimePlayEvaluationState(CreationInput creationInput,
										IAudioService<SoundId> audioService,
										IProgressEvaluator progressEvaluator,
										Settings settings)
			: base(progressEvaluator, audioService, settings)
		{
			this.creationInput = creationInput;
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
	}
}
