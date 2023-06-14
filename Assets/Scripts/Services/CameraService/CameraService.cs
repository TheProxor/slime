using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


namespace TheProxor.Services.Camera
{
	public class CameraService : ICameraService<CinemachineVirtualCamera>
	{
		private readonly CinemachineBrain cinemachineBrain;
		private readonly Dictionary<Type, ICameraTransitionBehaviour<CinemachineVirtualCamera>> cameraTransitionBehaviours;



		public CameraService(/*ICollection<ICameraTransitionBehaviour<CinemachineVirtualCamera>> cameraTransitionBehaviours*/)
		{
			if (UnityEngine.Camera.main is null)
			{
				Debug.LogError($"Camera does not exits!");
				return;
			}

			if (!UnityEngine.Camera.main.TryGetComponent(out cinemachineBrain))
			{
				Debug.LogError($"Camera does not contain {nameof(CinemachineBrain)!}");
				return;
			}

			/*this.cameraTransitionBehaviours = new();
			foreach (var behaviour in cameraTransitionBehaviours)
				this.cameraTransitionBehaviours.Add(behaviour.GetType(), behaviour);*/
		}


		public void SetVirtualCameraActive(CinemachineVirtualCamera state)
		{
			cinemachineBrain.SetCameraOverride(0, cinemachineBrain.ActiveVirtualCamera, state, 1.0f, 1.0f);
		}
	}
}
