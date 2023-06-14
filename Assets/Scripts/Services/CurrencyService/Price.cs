using System;
using UnityEngine;


namespace TheProxor.Services.Currency
{
	[Serializable]
	public struct Price : ICurrency<CurrencyType>
	{
		public Price(CurrencyType type, float value)
		{
			CurrencyType = type;
			Value = value;
		}



		[field: SerializeField] public CurrencyType CurrencyType { get; private set; }
		[field: SerializeField] public float Value { get; private set; }



		public override string ToString() =>
			$"({CurrencyType}, {Value})";
	}
}
