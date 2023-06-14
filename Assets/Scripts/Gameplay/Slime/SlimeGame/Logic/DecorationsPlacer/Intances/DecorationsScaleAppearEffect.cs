using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.DecorationsPlacer
{
	public class DecorationsScaleAppearEffect : DecorationsAppearEffect, ITickable
	{
		[Serializable]
		public class Settings
		{
			[SerializeField]
			private float duration = 1;

			[SerializeField]
			private AnimationCurve scaleCurve = default;

			public float Duration => duration;
			public AnimationCurve ScaleCurve => scaleCurve;
		}

		public struct DecorationData
		{
			public readonly Transform Transform;
			public readonly Vector3 LocalScale;

			public float Timer;


			public DecorationData(GameObject gameObject)
			{
				Transform = gameObject.transform;
				LocalScale = Transform.localScale;
				Timer = 0;
			}
		}

		private readonly Settings settings;
		private readonly TickableManager updater;
		private readonly List<DecorationData> decorationsData = new();


		public DecorationsScaleAppearEffect(
			Settings settings,
			TickableManager updater
		)
		{
			this.settings = settings;
			this.updater = updater;
		}


		public override void Appear(GameObject decoration)
		{
			base.Appear(decoration);
			AddUpdatedData(new DecorationData(decoration));
		}


		public override void Stop()
		{
			base.Stop();
			ClearDecorationsData();
			DeInitializeUpdateService();
		}


		private void AddUpdatedData(DecorationData decorationData)
		{
			decorationsData.Add(decorationData);

			if (decorationsData.Count > 1)
			{
				return;
			}

			InitializeSceneUpdateService();
		}


		private void InitializeSceneUpdateService()
		{
			// TODO: create RemoveTickable
			updater.Add(this);
		}


		void ITickable.Tick()
		{
			for (int i = decorationsData.Count - 1; i >= 0; i--)
			{
				DecorationData decorationData = decorationsData[i];
				DecorationData updatedData = UpdateData(decorationData, out bool isDone);

				if (isDone)
				{
					updatedData.Transform.localScale = updatedData.LocalScale;
					RemoveUpdatedData(i);

					continue;
				}

				decorationsData[i] = updatedData;
			}
		}


		private DecorationData UpdateData(DecorationData data, out bool isDone)
		{
			float duration = settings.Duration;
			float timer = data.Timer;

			float t = settings.ScaleCurve.Evaluate(timer / duration);
			data.Transform.localScale = Vector3.LerpUnclamped(Vector3.zero, data.LocalScale, t);

			timer += Time.deltaTime;

			if (timer > duration)
			{
				isDone = true;
			}
			else
			{
				isDone = false;
				data.Timer = timer;
			}

			return data;
		}


		private void RemoveUpdatedData(int i)
		{
			decorationsData.RemoveAt(i);

			if (decorationsData.Count > 0)
			{
				return;
			}

			DeInitializeUpdateService();
		}


		private void ClearDecorationsData()
		{
			decorationsData.Clear();
		}


		private void DeInitializeUpdateService()
		{
			updater.Remove(this);
		}
	}
}
