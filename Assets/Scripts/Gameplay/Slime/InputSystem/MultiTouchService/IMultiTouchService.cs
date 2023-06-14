using System;
using Zenject;


namespace TheProxor.SlimeSimulation.InputSystem.MultiTouchService
{
	public interface IMultiTouchService : ITickable
	{
		event Action<ITouch> OnTouchAdded;
		event Action<ITouch> OnTouchRemoved;
	}
}
