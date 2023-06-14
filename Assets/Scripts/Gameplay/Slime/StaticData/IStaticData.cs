namespace TheProxor.StaticData
{
	public interface IStaticData<out TType>
	{
		public TType Type { get; }
	}
}
