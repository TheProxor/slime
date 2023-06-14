using System;
using System.Collections.Generic;
using System.Linq;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.Services.Preference;
using UnityEngine;



namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame
{
	public class SlimeIngredientsManager : ISlimeIngredientsManager<SlimeIngredientInfo>
	{
		private readonly IPreferenceService preferenceService;
		private readonly SlimeIngredientDataBase slimeIngredientDataBase;
		private readonly SlimeIngredientsSaveData slimeIngredientsSaveData;
		private Dictionary<string, SlimeIngredientInfo> slimeIngredientInfos;


		public SlimeIngredientsManager(
				IPreferenceService preferenceService,
				SlimeIngredientDataBase slimeIngredientDataBase
			)
		{
			this.preferenceService = preferenceService;
			this.slimeIngredientDataBase = slimeIngredientDataBase;

			slimeIngredientInfos = new Dictionary<string, SlimeIngredientInfo>();
			slimeIngredientsSaveData = preferenceService.LoadValue<SlimeIngredientsSaveData>();

			slimeIngredientsSaveData.SlimeIngredientInfos ??= new Dictionary<string, SlimeIngredientsSaveData.SlimeIngredientInfoSaveData>();

			foreach (var info in slimeIngredientDataBase.SlimeIngredientInfos)
			{
				SlimeIngredientInfo ingredientInfo = info.Clone() as SlimeIngredientInfo;
				ingredientInfo.Count = ingredientInfo.DefaultCount;
				ingredientInfo.ReceivingTime = ingredientInfo.Count > 0? DateTime.Now : DateTime.MinValue;

				slimeIngredientInfos.Add(ingredientInfo.Id, ingredientInfo);

				if (slimeIngredientsSaveData.SlimeIngredientInfos.TryGetValue(ingredientInfo.Id,
					out SlimeIngredientsSaveData.SlimeIngredientInfoSaveData ingredientInfoSaveData))
				{
					ingredientInfo.Count = ingredientInfoSaveData.Count;
					ingredientInfo.ReceivingTime = ingredientInfoSaveData.ReceivingTime;
					continue;
				}

				ingredientInfoSaveData = new SlimeIngredientsSaveData.SlimeIngredientInfoSaveData
				{
					Count = ingredientInfo.Count,
					ReceivingTime = ingredientInfo.ReceivingTime
				};

				slimeIngredientsSaveData.SlimeIngredientInfos.Add(ingredientInfo.Id, ingredientInfoSaveData);
			}

			RefreshAllIngredientsCollections();
		}


		public ICollection<T> GetIngredientsCollection<T>() where T : SlimeIngredientInfo =>
			slimeIngredientInfos
				.Where(x => x.Value is T)
				.Select(x => x.Value as T)
				.ToList();


		public void ConsumeIngredients<T>(ICollection<T> slimeIngredientInfos) where T : SlimeIngredientInfo
		{
			foreach (var slimeIngredientInfo in slimeIngredientInfos)
			{
				this.slimeIngredientInfos[slimeIngredientInfo.Id].Count
					= Mathf.Clamp(this.slimeIngredientInfos[slimeIngredientInfo.Id].Count - 1, 0, int.MaxValue);

				if (this.slimeIngredientInfos[slimeIngredientInfo.Id].Count == 0)
					this.slimeIngredientInfos[slimeIngredientInfo.Id].ReceivingTime = DateTime.MinValue;
			}

			RefreshAllIngredientsCollections();
		}


		public void ConsumeIngredients(ICollection<string> slimeIngredientIds)
		{
			foreach (var id in slimeIngredientIds)
			{
				this.slimeIngredientInfos[id].Count
					= Mathf.Clamp(this.slimeIngredientInfos[id].Count - 1, 0, int.MaxValue);

				if (this.slimeIngredientInfos[id].Count == 0)
					this.slimeIngredientInfos[id].ReceivingTime = DateTime.MinValue;
			}

			RefreshAllIngredientsCollections();
		}


		public void ConsumeIngredient(string ingredientId, int count)
		{
			this.slimeIngredientInfos[ingredientId].Count
					= Mathf.Clamp(this.slimeIngredientInfos[ingredientId].Count - count, 0, int.MaxValue);

			if (this.slimeIngredientInfos[ingredientId].Count == 0)
				this.slimeIngredientInfos[ingredientId].ReceivingTime = DateTime.MinValue;

			RefreshAllIngredientsCollections();
		}


		/*public bool TryGetInfo(string id, out SlimeIngredientInfo ingredientInfo) =>
			slimeIngredientDataBase.TryGetInfo(id, out ingredientInfo);*/



		public void AddIngredient(string id, int count)
		{
			if (!slimeIngredientInfos.TryGetValue(id, out SlimeIngredientInfo slimeIngredientInfo))
			{
				Debug.LogError($"Ingredient with id {id} does not exists!");
				return;
			}

			slimeIngredientInfo.Count += count;

			if (slimeIngredientInfo.Count > 0)
				slimeIngredientInfo.ReceivingTime = DateTime.Now;

			RefreshAllIngredientsCollections();
		}


		private void RefreshAllIngredientsCollections()
		{
			foreach (var slimeIngredientInfo in slimeIngredientInfos)
			{
				slimeIngredientsSaveData.SlimeIngredientInfos[slimeIngredientInfo.Key].Count = slimeIngredientInfo.Value.Count;
				slimeIngredientsSaveData.SlimeIngredientInfos[slimeIngredientInfo.Key].ReceivingTime = slimeIngredientInfo.Value.ReceivingTime;
			}

			preferenceService.SaveValue(slimeIngredientsSaveData);
		}
	}
}
