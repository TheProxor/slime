using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;


namespace TheProxor.Services.Currency
{
	[Serializable]
	public class SecureCurrency : ICurrency<CurrencyType>
	{
		[JsonProperty] private string securedValue;
		[JsonProperty] private string securedValueMax;
		[JsonProperty] private CurrencyType currencyType;


		public SecureCurrency()
		{
			this.securedValue = string.Empty;
			this.currencyType = CurrencyType.None;
		}


		public SecureCurrency(CurrencyType currencyType)
		{
			this.currencyType = currencyType;
		}


		public SecureCurrency(CurrencyType currencyType, float value)
		{
			this.currencyType = currencyType;

			this.securedValue = string.Empty;
			this.securedValueMax = string.Empty;

			this.ValueMax = value;
			this.Value = value;
		}


		[JsonIgnore]
		public float Value
		{
			get => GetValueFromSecureString(securedValue);
			set
			{
				float normalizedValue = Mathf.Clamp(value, 0, float.MaxValue);

				if (normalizedValue > ValueMax)
					ValueMax = value;

				securedValue = GenerateSecureString(normalizedValue);

				OnCurrencyChanged?.Invoke(value);
			}
		}

		[JsonIgnore]
		public float ValueMax
		{
			get => GetValueFromSecureString(securedValueMax);
			private set => securedValueMax = GenerateSecureString(Mathf.Abs(value));
		}

		[JsonIgnore]
		public CurrencyType CurrencyType =>
			currencyType;


		public event Action<float> OnCurrencyChanged = default;


		protected virtual string GenerateSecureString(float value) =>
			Compress(value.ToString());


		protected virtual float GetValueFromSecureString(string secureString) =>
			float.Parse(Decompress(secureString));


		private string Compress(string uncompressedString)
		{
			byte[] compressedBytes;

			using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
			{
				using (var compressedStream = new MemoryStream())
				{
					using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
					{
						uncompressedStream.CopyTo(compressorStream);
					}

					compressedBytes = compressedStream.ToArray();
				}
			}

			return Convert.ToBase64String(compressedBytes);
		}


		private string Decompress(string compressedString)
		{
			byte[] decompressedBytes;

			var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

			using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
			{
				using (var decompressedStream = new MemoryStream())
				{
					decompressorStream.CopyTo(decompressedStream);

					decompressedBytes = decompressedStream.ToArray();
				}
			}

			return Encoding.UTF8.GetString(decompressedBytes);
		}
	}
}
