using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[Serializable]
	public class RecipeStep
	{
		#if UNITY_EDITOR
		[SerializeField]
		[UsedImplicitly]
		private string name = "";
		#endif

		[SerializeField]
		private SubStateType stateType = default;

		[SerializeField]
		private bool changeBowlFill = false;

		[SerializeField]
		private AnimatedFloat bowlFillAnimation = null;

		[SerializeField]
		private bool animateMaterial = false;

		[SerializeField]
		private AnimatedMaterial materialAnimation = null;

		[SerializeField]
		private List<Effect> effects = null;

		[SerializeField] private AudioClip stepSound = default;

		public SubStateType StateType => stateType;
		public bool ChangeBowlFill => changeBowlFill;
		public AnimatedFloat BowlFillAnimation => bowlFillAnimation;
		public bool AnimateMaterial => animateMaterial;
		public AnimatedMaterial MaterialAnimation => materialAnimation;
		public List<Effect> Effects => effects;
		public AudioClip StepSound => stepSound;

		public RecipeStep(SubStateType stateType)
		{
			this.stateType = stateType;
		}
	}
}
