using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic
{
	public class SlimeMaterialAnimator
		: ProgressAnimator<Material, AnimatedMaterial, SlimeMaterialAnimator>
	{
		private readonly SlimeFacade slime;

		public SlimeMaterialAnimator(SlimeFacade slime)
		{
			this.slime = slime;
		}

		protected override Material GetStartValue()
		{
			var startValue = new Material(slime.Material);
			startValue.CopyPropertiesFromMaterial(slime.Material);

			return startValue;
		}

		protected override Material GetReferenceValue()
		{
			Material material = slime.Material;
			material.CopyPropertiesFromMaterial(AnimatedValue.EndValue);

			return material;
		}
	}
}
