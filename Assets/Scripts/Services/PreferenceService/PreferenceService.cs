using UnityEngine;


namespace TheProxor.Services.Preference
{
	public class PreferenceService : IPreferenceService
	{
		private readonly IPreferenceServiceSerializer serializer;


		public PreferenceService(IPreferenceServiceSerializer serializer)
		{
			this.serializer = serializer;
		}


		public void SaveValue<T>(string key, T value)
		{
			PlayerPrefs.SetString(key, serializer.Serialize(value));
			PlayerPrefs.Save();
		}


		public void SaveValue<T>(T value) =>
			SaveValue(value.GetType().FullName, value);


		public void SaveValue(string key, object value) =>
			SaveValue(key, value);


		public object LoadValue(string key, object defaultValue) =>
			serializer.Deserialize(PlayerPrefs.GetString(key));


		public T LoadValue<T>(string key, T defaultValue)
		{
			T value = serializer.Deserialize<T>(PlayerPrefs.GetString(key));
			return value is not null ? value : defaultValue;
		}


		public T LoadValue<T>(T defaultValue) =>
			LoadValue(defaultValue.GetType().FullName, defaultValue);


		public T LoadValue<T>() where T : new() =>
			LoadValue(typeof(T).FullName, new T());
	}
}
