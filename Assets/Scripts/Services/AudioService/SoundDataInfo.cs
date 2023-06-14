using System.Collections.Generic;
using TheProxor.Utils;
using UnityEngine;


namespace TheProxor.Services.Audio
{
	public class SoundDataInfo
	{
		[field: SerializeField] public string ClipName { get; private set; } = default;
		[field: SerializeField] public List<AudioClip> Clips { get; private set; } = default;

		public AudioClip RandomClip
		{
			get
			{
				if (Clips == null && Clips.Count == 0)
					return null;
				return Clips.GetRandomItem();
			}
		}
	}
}
