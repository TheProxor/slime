using System.Collections.Generic;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame
{
	public interface ISlimeIngredientsManager<TIngredientInfo>
	{
		ICollection<T> GetIngredientsCollection<T>() where T : TIngredientInfo;

		public void AddIngredient(string id, int count);

		void ConsumeIngredients<T>(ICollection<T> slimeIngredientInfos) where T : TIngredientInfo;

		void ConsumeIngredients(ICollection<string> slimeIngredientIds);

		void ConsumeIngredient(string ingredientId, int count);

		//bool TryGetInfo(string id, out TIngredientInfo ingredientInfo);
	}
}
