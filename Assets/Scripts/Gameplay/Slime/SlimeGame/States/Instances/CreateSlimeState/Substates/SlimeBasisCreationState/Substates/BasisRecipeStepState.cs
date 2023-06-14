using System;
using System.Collections.Generic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.Services.Audio;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public class BasisRecipeStepState : EvaluatedProgressState
	{
		public class Pool : MemoryPool<RecipeStep, BasisRecipeStepState>
		{
			protected override void Reinitialize(
					RecipeStep recipeStep,
					BasisRecipeStepState state
				)
			{
				state.Init(recipeStep);
			}
		}

		[Serializable]
		public new class Settings : EvaluatedProgressState.Settings {}

		private readonly BowlFillAnimator.Pool bowlFillAnimationPool;
		private readonly SlimeMaterialAnimator.Pool slimeMaterialAnimationPool;
		private readonly Effect.Factory effectsFactory;
		private readonly CreationInput creationInput;
		private readonly List<Effect> effects = new();

		private RecipeStep recipeStep;
		private BowlFillAnimator bowlFillAnimator;
		private SlimeMaterialAnimator slimeMaterialAnimator;
		private Guid? loopedSoundGuid;



		public SubStateType Type => recipeStep.StateType;


		public BasisRecipeStepState(
				IAudioService<SoundId> audioService,
				BowlFillAnimator.Pool bowlFillAnimationPool,
				SlimeMaterialAnimator.Pool slimeMaterialAnimationPool,
				Effect.Factory effectsFactory,
				CreationInput creationInput,
				IProgressEvaluator progressEvaluator,
				Settings settings
			) :
			base(progressEvaluator, audioService, settings)
		{
			this.bowlFillAnimationPool = bowlFillAnimationPool;
			this.slimeMaterialAnimationPool = slimeMaterialAnimationPool;
			this.effectsFactory = effectsFactory;
			this.creationInput = creationInput;
		}


		public override void Enter()
		{
			base.Enter();
			ResetInput();
			CreateAnimations();
			CreateEffects();
		}


		public override void Exit()
		{
			base.Exit();
			DisposeAnimations();
			DisposeEffects();
			ResetInput();
			StopSound();
		}


		protected override void Evaluate(float evaluation)
		{
			base.Evaluate(evaluation);
			HandleSound(creationInput.Value > 0);
		}


		private void Init(RecipeStep recipeStep)
		{
			this.recipeStep = recipeStep;
		}


		private void ResetInput()
		{
			creationInput.Reset();
		}


		private void CreateAnimations()
		{
			if (recipeStep.ChangeBowlFill)
			{
				bowlFillAnimator = SpawnBowlFillAnimation();
			}

			if (recipeStep.AnimateMaterial)
			{
				slimeMaterialAnimator = SpawnSlimeMaterialAnimation();
			}
		}


		private void HandleSound(bool isEvaluating)
		{
			if (recipeStep.StepSound == null)
				return;

			if (isEvaluating)
				PlaySound();
			else
				StopSound();
		}


		private void PlaySound()
		{
			if (loopedSoundGuid != null)
				return;

			loopedSoundGuid = AudioService.PlaySoundLoop(recipeStep.StepSound);
		}


		private void StopSound()
		{
			if (loopedSoundGuid == null)
				return;

			AudioService.Destroy(loopedSoundGuid.Value);
			loopedSoundGuid = null;
		}

		private BowlFillAnimator SpawnBowlFillAnimation()
		{
			return bowlFillAnimationPool.Spawn(recipeStep.BowlFillAnimation, this);
		}


		private SlimeMaterialAnimator SpawnSlimeMaterialAnimation()
		{
			return slimeMaterialAnimationPool.Spawn(recipeStep.MaterialAnimation, this);
		}


		private void CreateEffects()
		{
			foreach (Effect effect in recipeStep.Effects)
			{
				effects.Add(effectsFactory.Create(effect));
			}
		}


		private void DisposeAnimations()
		{
			if (recipeStep.ChangeBowlFill)
			{
				bowlFillAnimationPool.Despawn(bowlFillAnimator);
			}

			if (recipeStep.AnimateMaterial)
			{
				slimeMaterialAnimationPool.Despawn(slimeMaterialAnimator);
			}
		}


		private void DisposeEffects()
		{
			foreach (Effect effect in effects)
			{
				effect.Dispose(immediate: false);
			}

			effects.Clear();
		}
	}
}
