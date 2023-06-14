using System;
using System.Collections.Generic;


namespace TheProxor
{
	public interface ICurrencyService<TCurrencyType> where TCurrencyType : Enum
	{
		Action<TCurrencyType> OnCurrencyValueChange { get; set; }



		ICollection<float> GetAllTransactions(TCurrencyType currencyType);

		float GetLastTransaction(TCurrencyType currencyType);

		void AddCurrencyValue(params (TCurrencyType, float)[] values);

		void AddCurrencyValue(params (TCurrencyType, float, float)[] values);

		void SubtractCurrencyValue(params (TCurrencyType, float)[] values);

		void AddCurrency(params ICurrency<TCurrencyType>[] values);

		void SubtractCurrency(params ICurrency<TCurrencyType>[] values);

		bool TrySubtractCurrency(params ICurrency<TCurrencyType>[] values);

		void SetCurrencyValue(params (TCurrencyType, float)[] values);

		bool TrySubtractCurrencyValue(params (TCurrencyType, float)[] values);

		float GetCurrencyValue(TCurrencyType currencyType);

		float GetCurrencyValueMax(TCurrencyType currencyType);

		ICurrency<TCurrencyType> GetCurrency(TCurrencyType currencyType);

		bool IsEnoughCurrency(TCurrencyType currencyType, float value);
	}
}
