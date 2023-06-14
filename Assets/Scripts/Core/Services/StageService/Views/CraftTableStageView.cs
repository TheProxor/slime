using Cinemachine;
using UnityEngine;


namespace TheProxor.Services.Stage
{
	public class CraftTableStageView : StageView
	{
		[field: SerializeField] public CinemachineVirtualCamera FrontVirtualCamera { get; private set; } = default;
		[field: SerializeField] public CinemachineVirtualCamera TopVirtualCamera { get; private set; } = default;
	}
}
