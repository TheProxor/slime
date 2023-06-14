namespace TheProxor.Services.Preference
{
	public interface IPreferenceServiceSerializer
	{
		public string Serialize(object obj);
		public object Deserialize(string str);
		public T Deserialize<T>(string str);
	}
}
