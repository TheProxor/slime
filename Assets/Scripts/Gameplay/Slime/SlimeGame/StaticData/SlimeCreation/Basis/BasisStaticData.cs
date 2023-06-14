using System.Collections.Generic;
using TheProxor.SlimeSimulation.DeformationSystemModule;
using TheProxor.StaticData;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData
{
	[CreateAssetMenu(fileName = "Basis Static Data",
					 menuName = "Take Top/Meta Games/Slime Game/Static Data/Slime Creation/Basis Static Data")]
	public class BasisStaticData : ScriptableObject, IStaticData<BasisType>
	{
		[SerializeField]
		private BasisType type = default;

		[SerializeField]
		private Sprite icon = null;

		[SerializeField]
		private string label = "";

		[SerializeField]
		private BasisRecipe recipe = null;

		[SerializeField]
		private SlimeMesh.Settings slimeSettings;

		[SerializeField]
		[HideInInspector]
		private Material cachedMaterial = null;

		public BasisType Type => type;
		public Sprite Icon => icon;
		public string Label => label;
		public BasisRecipe Recipe => recipe;
		public SlimeMesh.Settings SlimeSettings => slimeSettings;
		public Material CachedMaterial => cachedMaterial;

		private void OnValidate()
		{
			CacheMaterial();
		}

		private void CacheMaterial()
		{
			cachedMaterial = GetRecipeMaterial();
		}

		private Material GetRecipeMaterial()
		{
			List<RecipeStep> recipeSteps = recipe.Steps;

			for (int i = recipeSteps.Count - 1; i >= 0; i--)
			{
				if (!recipeSteps[i].AnimateMaterial)
				{
					continue;
				}

				return recipeSteps[i].MaterialAnimation.EndValue;
			}

			return null;
		}
	}


	public enum BasisType
	{
		Base = 0,
		Clear = 1,
		Fluffy = 2,
		Cloud = 3,
		Glitter = 4
	}
}
