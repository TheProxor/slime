using System;
using UnityEngine;

namespace TheProxor.SlimeSimulation.InputSystem
{
	public interface ITouch
	{
		event Action<Vector2> OnPositionChanged;
		Vector2 Position { get; }
	}
}
