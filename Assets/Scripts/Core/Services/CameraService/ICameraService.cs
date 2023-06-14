namespace TheProxor.Services.Camera
{
	public interface ICameraService<in TVirtualCamera>
	{
		void SetVirtualCameraActive(TVirtualCamera state);
	}
}
