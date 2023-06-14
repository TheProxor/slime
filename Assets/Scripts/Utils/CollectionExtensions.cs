using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


namespace TheProxor.Utils
{
	public static class CollectionExtensions
	{
		public static void RefillKeys<TKey, TValue>(this IDictionary<TKey, TValue> obj)
		{
			var newDictionary = new Dictionary<TKey, TValue>();
			foreach (TKey key in Enum.GetValues(typeof(TKey)))
			{
				obj.TryGetValue(key, out var value);
				newDictionary.Add(key, value);
			}

			obj.Clear();
			foreach (var pair in newDictionary)
				obj.Add(pair);
		}


		public static void RefillItemsReordered<TEnum>(this IList<TEnum> list)
			where TEnum : Enum
		{
			list.Clear();
			var values = Enum.GetValues(typeof(TEnum));
			foreach (TEnum item in values)
				list.Add(item);
		}


		public static void RefillItems<TEnum>(this IList<TEnum> list)
			where TEnum : Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			foreach (TEnum item in values)
				if (!list.Contains(item))
					list.Add(item);
		}


		public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(this IList<TKey> list)
		{
			var newDictionary = new Dictionary<TKey, TValue>();
			foreach (TKey key in list)
				newDictionary.Add(key, default);

			return newDictionary;
		}


		public static T GetRandomItem<T>(this IList<T> list)
		{
			if (list == null || list.Count == 0)
				return default;
			return list[Random.Range(0, list.Count)];
		}


		public static T GetRandom<T>(this IEnumerable<T> collection) =>
			collection.ElementAt(Random.Range(0, collection.Count()));


		public static bool IsLast<T>(this IList<T> list, T item) =>
			list.IndexOf(item) == list.Count - 1;
	}
}
