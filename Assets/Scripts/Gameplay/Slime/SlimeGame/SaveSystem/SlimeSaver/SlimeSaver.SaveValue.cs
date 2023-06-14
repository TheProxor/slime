using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem
{
	public partial class SlimeSaver
	{
		[Serializable]
		private class SaveValue
		{
			public List<SlimeSaveData> saved = new();

			public List<SlimeSaveData> GetSave() => saved;

			public bool HasId(SlimeSaveData saveData, out int index)
			{
				for (int i = saved.Count - 1; i >= 0; i--)
				{
					if (saved[i].Id != saveData.Id)
					{
						continue;
					}

					index = i;

					return true;
				}

				index = -1;

				return false;
			}

			public void AddSave(SlimeSaveData value)
			{
				saved.Add(value);
			}

			public void RemoveSave(SlimeSaveData saveData)
			{
				for (int i = saved.Count - 1; i >= 0; i--)
				{
					if (saved[i].Id != saveData.Id)
					{
						continue;
					}

					saved.RemoveAt(i);

					break;
				}
			}

			public void RemoveSaveAt(int index)
			{
				saved.RemoveAt(index);
			}
		}
	}
}
