using System;
using UnityEngine;

namespace TheProxor.SlimeSimulation.InputSystem.MultiTouchService
{
	public abstract class MultiTouchServiceBase : IMultiTouchService
	{
		protected class Touch : ITouch
		{
			public event Action<Vector2> OnPositionChanged;

			private Vector2 position;

			public Vector2 Position
			{
				get => position;
				set
				{
					position = value;
					OnPositionChanged?.Invoke(position);
				}
			}

			public Touch() {}

			public Touch(UnityEngine.Touch touch)
			{
				position = touch.position;
			}
		}

		public event Action<ITouch> OnTouchAdded;
		public event Action<ITouch> OnTouchRemoved;

		protected void AddTouch(Touch touch)
		{
			OnTouchAdded?.Invoke(touch);
		}

		protected void RemoveTouch(Touch touch)
		{
			OnTouchRemoved?.Invoke(touch);
		}


		public abstract void Tick();
	}
}
