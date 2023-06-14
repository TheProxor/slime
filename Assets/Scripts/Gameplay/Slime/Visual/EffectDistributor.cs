using System.Collections.Generic;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.VFX
{
	public class EffectDistributor
	{
		private readonly Dictionary<EffectLayer, EffectLayerGroup> groupByLayer = new();

		public void RegisterGroup(EffectLayerGroup @group)
		{
			groupByLayer.Add(@group.Layer, @group);
		}

		public EffectDistributor(Effect.Factory effectsFactory)
		{
			effectsFactory.OnNewEffectCreated += Distribute;
		}

		private void Distribute(Effect effect)
		{
			if (!groupByLayer.TryGetValue(effect.Layer, out EffectLayerGroup layerGroup))
			{
				return;
			}

			layerGroup.AddEffect(effect);
		}
	}
}
