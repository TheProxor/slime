using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase
{
	[CreateAssetMenu(fileName = "Database Slime Ingredients", menuName = "Database/Slime Ingredients Database")]
	public class SlimeIngredientDataBase : ScriptableObject
	{
		[SerializeField] private List<SlimeIngredientInfo> slimeIngredientInfos = default;



		public IReadOnlyCollection<SlimeIngredientInfo> SlimeIngredientInfos => slimeIngredientInfos;



		#region Editor Utils
		#if UNITY_EDITOR

		[Button]
		public void FillContent()
		{
			slimeIngredientInfos = new List<SlimeIngredientInfo>();
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(SlimeIngredientInfo)}", null);

			foreach (var guid in guids)
			{
				slimeIngredientInfos.Add(AssetDatabase.LoadAssetAtPath<SlimeIngredientInfo>(AssetDatabase.GUIDToAssetPath(guid)));
			}
		}

		#endif
		#endregion
	}
}
