using Newtonsoft.Json;


namespace TheProxor.Services.Preference
{
	public class NewtonsoftPreferenceServiceSerializer : IPreferenceServiceSerializer
	{
		public string Serialize(object obj)
		{
			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			string json = JsonConvert.SerializeObject(obj, settings);
			return json;
		}

		public object Deserialize(string str) =>
			Deserialize<object>(str);

		public T Deserialize<T>(string str)
		{
			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			return JsonConvert.DeserializeObject<T>(str, settings);
		}
	}
}
