using System.Collections.Generic;
using System.Linq;


namespace TheProxor.SlimeSimulation.InputSystem.MultiTouchService
{
	public class MultiTouchService : MultiTouchServiceBase
	{
		private readonly Dictionary<int, Touch> touchByFingerId = new();

		private int[] cachedFingerIds = new int[0];



		public override void Tick()
		{
			UnityEngine.Touch[] touches = UnityEngine.Input.touches;
			var fingerIds = new int[touches.Length];

			for (var i = 0; i < touches.Length; i++)
			{
				UnityEngine.Touch inputTouch = touches[i];
				int fingerId = inputTouch.fingerId;

				if (!TryGetExistingTouchByFingerID(fingerId, out Touch touch))
				{
					AddNewTouch(inputTouch, fingerId);
				}
				else
				{
					UpdateExistingTouchPosition(touch, inputTouch);
				}

				fingerIds[i] = fingerId;
			}

			RemoveNotExistingTouches(fingerIds);
			CacheFingerIds(fingerIds);
		}

		private bool TryGetExistingTouchByFingerID(int fingerId, out Touch touch)
		{
			return touchByFingerId.TryGetValue(fingerId, out touch);
		}

		private void AddNewTouch(UnityEngine.Touch inputTouch, int fingerId)
		{
			var touch = new Touch(inputTouch);
			touchByFingerId.Add(fingerId, touch);
			AddTouch(touch);
		}

		private static void UpdateExistingTouchPosition(Touch touch, UnityEngine.Touch inputTouch)
		{
			touch.Position = inputTouch.position;
		}

		private void RemoveNotExistingTouches(IEnumerable<int> fingerIds)
		{
			foreach (int id in GetRemovedTouchIds(fingerIds))
			{
				RemoveTouch(touchByFingerId[id]);
				touchByFingerId.Remove(id);
			}
		}

		private IEnumerable<int> GetRemovedTouchIds(IEnumerable<int> fingerIds)
		{
			return cachedFingerIds.Except(fingerIds);
		}

		private void CacheFingerIds(int[] fingerIds)
		{
			cachedFingerIds = fingerIds;
		}
	}
}
