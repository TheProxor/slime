namespace TheProxor.Services.Camera
{
	public interface ICameraTransitionBehaviour<in TCameraState>
	{
		void PlayTransition(TCameraState cameraState);
	}
}
