using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	public abstract class Effect : MonoBehaviour
	{
		public class Factory : IFactory<UnityEngine.Object, Effect>
		{
			public event Action<Effect> OnNewEffectCreated;

			private readonly DiContainer container;

			public Factory(DiContainer container)
			{
				this.container = container;
			}

			public Effect Create(UnityEngine.Object prototype)
			{
				var effect = container.InstantiatePrefabForComponent<Effect>(prototype);

				OnNewEffectCreated?.Invoke(effect);

				return effect;
			}
		}

		[SerializeField]
		private EffectLayer layer;

		public EffectLayer Layer => layer;
		public bool IsDisposing { get; private set; }
		protected virtual bool IsCanBeDisposed => true;

		public void Dispose(bool immediate)
		{
			DisposeInner(immediate);

			if (immediate || IsCanBeDisposed)
			{
				Dispose();

				return;
			}

			IsDisposing = true;
			StartCoroutine(DisposeOnCanBeDisposed());
		}

		protected virtual void DisposeInner(bool immediate) {}

		protected virtual void Dispose()
		{
			Destroy(gameObject);
		}

		private IEnumerator DisposeOnCanBeDisposed()
		{
			while (!IsCanBeDisposed)
			{
				yield return null;
			}

			Dispose();
		}
	}
}
