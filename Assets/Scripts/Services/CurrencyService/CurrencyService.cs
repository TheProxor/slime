using System;
using System.Collections.Generic;
using TheProxor.Services.Preference;
using UnityEngine;


namespace TheProxor.Services.Currency
{
	public class CurrencyService : ICurrencyService<CurrencyType>
	{
		private IPreferenceService preferenceService;
		private Dictionary<CurrencyType, SecureCurrency> currencies;
		private Dictionary<CurrencyType, List<float>> transactions;

		public Action<CurrencyType> OnCurrencyValueChange { get; set; }


		public CurrencyService(IPreferenceService preferenceService)
		{
			this.preferenceService = preferenceService;

			Initialize();
		}


		public void Initialize()
		{
			transactions = new Dictionary<CurrencyType, List<float>>();
			currencies = new Dictionary<CurrencyType, SecureCurrency>();

			foreach (var currencyTypeName in Enum.GetNames(typeof(CurrencyType)))
			{
				if (Enum.TryParse(currencyTypeName, out CurrencyType currencyType))
				{
					if (currencyType == CurrencyType.None)
						continue;

					string currencyKey = $"Currency_{currencyType}";
					var currentCurrency = new SecureCurrency(currencyType, 0.0f);
					currentCurrency = preferenceService.LoadValue(currencyKey, currentCurrency);
					currentCurrency.OnCurrencyChanged += (value) => preferenceService.SaveValue(currencyKey, currentCurrency);
					currencies.Add(currencyType, currentCurrency);
					transactions[currencyType] = new List<float>();
				}
			}
		}


		public ICollection<float> GetAllTransactions(CurrencyType currencyType) =>
			transactions[currencyType];


		public float GetLastTransaction(CurrencyType currencyType)
		{
			List<float> transactions = this.transactions[currencyType];

			if (transactions.Count == 0)
				return 0;

			if (transactions.Count == 1)
				return transactions[0];

			return transactions[^1];
		}


		public void AddCurrencyValue(params (CurrencyType, float)[] values)
		{
			foreach (var currency in values)
			{
				if (currency.Item1 == CurrencyType.None)
				{
					Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
					continue;
				}

				currencies[currency.Item1].Value += currency.Item2;
				RegistrTransaction(currency.Item1, currency.Item2);

				OnCurrencyValueChange?.Invoke(currency.Item1);
			}
		}


		public void AddCurrencyValue(params (CurrencyType, float, float)[] values)
		{
			foreach (var currency in values)
			{
				if (currency.Item1 == CurrencyType.None)
				{
					Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
					continue;
				}

				currencies[currency.Item1].Value = Mathf.Clamp(currencies[currency.Item1].Value + currency.Item2,
					int.MinValue,
					currency.Item3);

				RegistrTransaction(currency.Item1, currency.Item2);

				OnCurrencyValueChange?.Invoke(currency.Item1);
			}
		}


		public void SubtractCurrencyValue(params (CurrencyType, float)[] values)
		{
			foreach (var currency in values)
			{
				if (currency.Item1 == CurrencyType.None)
				{
					Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
					continue;
				}

				currencies[currency.Item1].Value -= currency.Item2;
				RegistrTransaction(currency.Item1, currency.Item2);

				OnCurrencyValueChange?.Invoke(currency.Item1);
			}
		}


		public bool TrySubtractCurrencyValue(params (CurrencyType, float)[] values)
		{
			foreach (var currency in values)
			{
				if (currency.Item1 == CurrencyType.None)
				{
					Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
					continue;
				}

				if (!IsEnoughCurrency(currency.Item1, currency.Item2))
					return false;
			}

			foreach (var currency in values)
			{
				currencies[currency.Item1].Value -= currency.Item2;
				RegistrTransaction(currency.Item1, currency.Item2);
				OnCurrencyValueChange?.Invoke(currency.Item1);
			}

			return true;
		}


		public void AddCurrency(params ICurrency<CurrencyType>[] values)
		{
			foreach (var currency in values)
			{
				currencies[currency.CurrencyType].Value += currency.Value;
				RegistrTransaction(currency.CurrencyType, currency.Value);
				OnCurrencyValueChange?.Invoke(currency.CurrencyType);
			}
		}


		public void SubtractCurrency(params ICurrency<CurrencyType>[] values)
		{
			foreach (var currency in values)
			{
				currencies[currency.CurrencyType].Value -= currency.Value;
				RegistrTransaction(currency.CurrencyType, currency.Value);
				OnCurrencyValueChange?.Invoke(currency.CurrencyType);
			}
		}


		public bool TrySubtractCurrency(params ICurrency<CurrencyType>[] values)
		{
			foreach (var currency in values)
			{
				if (!IsEnoughCurrency(currency.CurrencyType, currency.Value))
					return false;
			}

			foreach (var currency in values)
			{
				currencies[currency.CurrencyType].Value -= currency.Value;
				RegistrTransaction(currency.CurrencyType, currency.Value);
				OnCurrencyValueChange?.Invoke(currency.CurrencyType);
			}

			return true;
		}


		public void SetCurrencyValue(params (CurrencyType, float)[] values)
		{
			foreach (var currency in values)
			{
				if (currency.Item1 == CurrencyType.None)
				{
					Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
					continue;
				}

				currencies[currency.Item1].Value = currency.Item2;
				OnCurrencyValueChange?.Invoke(currency.Item1);
			}
		}


		public float GetCurrencyValue(CurrencyType currencyType)
		{
			if (currencyType == CurrencyType.None)
			{
				Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
				return 0.0f;
			}

			return currencies[currencyType].Value;
		}


		public float GetCurrencyValueMax(CurrencyType currencyType)
		{
			if (currencyType == CurrencyType.None)
			{
				Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
				return 0.0f;
			}

			return currencies[currencyType].ValueMax;
		}


		public ICurrency<CurrencyType> GetCurrency(CurrencyType currencyType)
		{
			if (currencyType == CurrencyType.None)
			{
				Debug.LogError($"Attempt to get currency with type: <b>{CurrencyType.None}</b>!");
				return null;
			}

			return currencies[currencyType];
		}


		public bool IsEnoughCurrency(CurrencyType currencyType, float value) =>
			GetCurrencyValue(currencyType) >= value;


		private void RegistrTransaction(CurrencyType currencyType, float value) =>
			transactions[currencyType].Add(value);
	}
}
