using System;
using System.Collections.Generic;


namespace TheProxor
{
	public interface ICurrency<TCurrencyType>
	{
		public TCurrencyType CurrencyType { get; }
		public float Value { get; }
	}
}
