using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	[RequireComponent(typeof(SortingGroup))]
	public class EffectLayerGroup : MonoBehaviour
	{
		[SerializeField]
		private EffectLayer layer = EffectLayer.Default;

		public EffectLayer Layer => layer;

		[Inject]
		public void Construct(EffectDistributor effectDistributor)
		{
			effectDistributor.RegisterGroup(this);
		}

		public void AddEffect(Effect effect)
		{
			SetParentToThis(effect.transform);
		}

		private void SetParentToThis(Transform childTransform)
		{
			childTransform.SetParent(transform, false);
		}
	}
}
