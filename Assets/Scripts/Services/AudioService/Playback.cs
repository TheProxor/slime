using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace TheProxor.Services.Audio
{
	public class Playback
	{
		public readonly Guid Id = Guid.NewGuid();
		private readonly AudioSource source;
		// private readonly float defaultPitch;
		// private readonly float defaultVolume;
		// private readonly float defaultSpatialBlend;


		public bool IsLoop => source.loop;
		public bool IsExpired => !IsLoop && Time.unscaledTime >= ExpireTime;
		public float ExpireTime { get; private set; }


		public float PlaybackTime
		{
			get => source.time;
			set => source.time = value;
		}

		public float Volume
		{
			get => source.volume;
			set => source.volume = value;
		}

		public float Pitch
		{
			get => source.pitch;
			set => source.pitch = value;
		}

		public float Lenght => source.clip.length;


		public Playback(
				AudioClip clip,
				AudioSource source
			)
		{
			this.source = source;
			// defaultPitch = source.pitch;
			// defaultVolume = source.volume;
			// defaultSpatialBlend = source.spatialBlend;
			source.clip = clip;
			if (clip == null)
				return;

			ExpireTime = Time.unscaledTime + clip.length;
			source.Play();
		}


		public void ChangeClip(AudioClip clip, bool keepPlaying, bool saveTime)
		{
			float time = PlaybackTime;
			source.clip = clip;
			if (saveTime)
				PlaybackTime = time;
			if (keepPlaying)
				source.Play();
		}


		public void Play() =>
			source.Play();


		public void Stop() =>
			source.Stop();


		public void Destroy()
		{
			Object.Destroy(source.gameObject);
		}
	}
}
