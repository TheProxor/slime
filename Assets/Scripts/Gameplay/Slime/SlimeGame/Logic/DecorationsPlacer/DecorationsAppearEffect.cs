using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Logic.DecorationsPlacer
{
	public class DecorationsAppearEffect
	{
		public virtual void Appear(GameObject decoration)
		{
			decoration.SetActive(true);
		}

		public virtual void Stop()
		{
			
		}
	}
}
