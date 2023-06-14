using System;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.States
{
	public abstract class ProgressState : State
	{
		public event Action OnUpdateProgress;

		private float progress;

		public float Progress
		{
			get => progress;

			protected set
			{
				progress = Mathf.Clamp01(value);
				OnUpdateProgress?.Invoke();
			}
		}

		public override void Enter()
		{
			progress = 0;
		}
	}
}
