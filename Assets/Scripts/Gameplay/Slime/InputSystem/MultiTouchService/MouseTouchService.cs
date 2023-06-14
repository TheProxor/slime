using UnityEngine;


namespace TheProxor.SlimeSimulation.InputSystem.MultiTouchService
{
	public class MouseTouchService : MultiTouchServiceBase
	{
		private const KeyCode KEY_CODE = KeyCode.Mouse0;

		private readonly Touch touch;

		private bool isPointerDown;


		public MouseTouchService()
		{
			touch = new Touch();
		}


		public override void Tick()
		{
			if (UpdatePointerStatus())
			{
				UpdateTouchPosition();
			}
		}

		private void UpdateTouchPosition()
		{
			touch.Position = UnityEngine.Input.mousePosition;
		}

		private bool UpdatePointerStatus()
		{
			if (isPointerDown)
			{
				if (GetPointerUp())
				{
					OnPointerUp();
				}
			}
			else
			{
				if (GetPointerDown())
				{
					OnPointerDown();
				}
			}

			return isPointerDown;
		}

		private static bool GetPointerDown()
		{
			return UnityEngine.Input.GetKeyDown(KEY_CODE);
		}

		private void OnPointerDown()
		{
			isPointerDown = true;
			UpdateTouchPosition();
			AddTouch(touch);
		}

		private static bool GetPointerUp()
		{
			return UnityEngine.Input.GetKeyUp(KEY_CODE);
		}

		private void OnPointerUp()
		{
			isPointerDown = false;
			RemoveTouch(touch);
		}
	}
}
