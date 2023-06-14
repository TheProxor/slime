using UnityEngine;
using TheProxor.Utils;


namespace TheProxor
{
	public class Database<TDataType, TDataInfo> : ScriptableObject
	{
		[SerializeField] private SerializableDictionary<TDataType, TDataInfo> database = default;



		public bool TryGetDataInfo(TDataType dataKey, out TDataInfo dataInfo)
		{
			if (!database.TryGetValue(dataKey, out dataInfo))
			{
				Debug.LogError($"Database does not contains item with key <b>{dataKey}</b>");
				return false;
			}

			return true;
		}
	}
}
