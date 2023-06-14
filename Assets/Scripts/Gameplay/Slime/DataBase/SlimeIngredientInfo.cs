using System;
using TheProxor.Services.Currency;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase
{
	public class SlimeIngredientInfo : ScriptableObject, ICloneable
	{
		public string Id;
		public string Description;
		public string LocalizationKey;
		public Price Price;
		public float PriceReal;
		public int DefaultCount;

		[HideInInspector] public int Count;
		[HideInInspector] public bool isAdsIngredient;
		[HideInInspector] public DateTime ReceivingTime;



		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
