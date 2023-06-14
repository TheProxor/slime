using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Input;
using UnityEngine;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	[RequireComponent(typeof(CameraLayer))]
	public class SlimeOverlayEffect : Effect
	{
		private const float DISPOSE_VALUE = .0001f;

		[SerializeField]
		private SpriteRenderer sprite = null;

		[SerializeField]
		private bool fadeOutBeforeDestroy = true;

		[SerializeField]
		private float smoothTime = 0;

		[Range(0, 1)]
		[SerializeField]
		public float maxAlpha = 1;

		[SerializeField]
		private AnimationCurve appearCurve = null;

		private CreationInput input;
		private float velocity;
		private float alpha;

		protected override bool IsCanBeDisposed => !fadeOutBeforeDestroy || alpha < DISPOSE_VALUE;

		[Inject]
		public void Construct(CreationInput input)
		{
			this.input = input;
		}

		private void Start()
		{
			SetTiledDrawMode();
			UpdateColor();
		}

		private void SetTiledDrawMode()
		{
			sprite.drawMode = SpriteDrawMode.Tiled;
		}

		private void Update()
		{
			UpdateAlpha();
			UpdateColor();
		}

		private void UpdateAlpha()
		{
			alpha = Mathf.SmoothDamp(alpha, GetValue(), ref velocity, smoothTime);
		}

		private float GetValue()
		{
			if (IsDisposing)
			{
				return 0;
			}

			return input.Value;
		}

		private void UpdateColor()
		{
			Color color = sprite.color;
			color.a = appearCurve.Evaluate(alpha) * maxAlpha;
			sprite.color = color;
		}
	}
}
