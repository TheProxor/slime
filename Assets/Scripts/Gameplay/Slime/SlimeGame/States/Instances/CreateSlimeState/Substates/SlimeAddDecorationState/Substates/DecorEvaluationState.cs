using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.DecorationsPlacer;
using TheProxor.Services.Audio;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class DecorEvaluationState : EvaluatedProgressState
	{
		public class Installer : Installer<Installer>
		{
			[Serializable]
			public class Settings : DecorEvaluationState.Settings
			{
				[SerializeField]
				private DecorationsScaleAppearEffect.Settings decorationsScaleAppearEffect;

				public DecorationsScaleAppearEffect.Settings DecorationsScaleAppearEffect =>
					decorationsScaleAppearEffect;
			}

			private readonly Settings settings;


			public Installer(Settings settings)
			{
				this.settings = settings;
			}


			public override void InstallBindings()
			{
				InstallDecorEvaluationState();
				InstallDecorationsAppearEffect();
			}


			private void InstallDecorEvaluationState()
			{
				Container.Bind<DecorEvaluationState>().AsSingle().WithArguments(settings);
			}


			private void InstallDecorationsAppearEffect()
			{
				Container.Bind<DecorationsAppearEffect>()
						 .To<DecorationsScaleAppearEffect>()
						 .AsSingle()
						 .WithArguments(settings.DecorationsScaleAppearEffect);
			}
		}

		[Serializable]
		public new class Settings : EvaluatedProgressState.Settings {}

		private readonly DecorationsAppearEffect decorationsAppearEffect;
		private readonly CreationInput creationInput;

		private IReadOnlyList<GameObject> decorations;
		private int decorationsCount;
		private Guid? loopedSoundGuid;


		public DecorEvaluationState(
				IAudioService<SoundId> audioService,
				DecorationsAppearEffect decorationsAppearEffect,
				IProgressEvaluator progressEvaluator,
				CreationInput creationInput,
				Settings settings
			) :
			base(progressEvaluator, audioService, settings)
		{
			this.decorationsAppearEffect = decorationsAppearEffect;
			this.creationInput = creationInput;
		}


		public void Init(IReadOnlyList<GameObject> decorations)
		{
			this.decorations = decorations;
			decorationsCount = decorations.Count;
		}


		public override void Exit()
		{
			base.Exit();
			decorationsAppearEffect.Stop();
			StopSound();
			creationInput.Reset();
		}


		protected override void Evaluate(float evaluation)
		{
			var fromDecoration = (int)(Progress * decorationsCount);

			base.Evaluate(evaluation);

			var toDecoration = (int)(Progress * decorationsCount);

			for (int i = fromDecoration; i < toDecoration; i++)
			{
				decorationsAppearEffect.Appear(decorations[i]);
			}

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

			loopedSoundGuid = AudioService.PlaySoundLoop(SoundId.slime_kneading_ingredients);
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
