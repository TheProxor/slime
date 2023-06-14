using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TheProxor.Database;
using TheProxor.Services.Preference;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;


namespace TheProxor.Services.Audio
{
	public class AudioService : IAudioService<SoundId>, IInitializable, ITickable
	{
		private const float MIN_DECIBEL_LEVEL = -80f;
		private const string SOUND_VOLUME_KEY = "SoundVolume";
		private const string MUSIC_VOLUME_KEY = "MusicVolume";
		private readonly SaveData saveData;

		private readonly Settings settings;
		private readonly IPreferenceService preferenceService;
		private readonly SignalBus signal;
		private readonly Dictionary<Guid, Playback> playbacks = new();
		private readonly SortedList<float, Guid> playbacksExpire = new();
		private readonly Queue<Fade> fadeQueue = new();
		private readonly HashSet<Guid> deleteFadeSet = new();

		private Dictionary<SoundId, DateTime> lastTimePlayed = new();
		private Dictionary<SoundId, float> nextDelay = new();
		private Tween fadeTween;



		private AudioSource audioSource;

		private SoundDatabase DataBase => settings.DataBase;

		public event Action<Guid> OnPlayingDone;


		public bool IsMusicEnabled
		{
			get => saveData.IsMusicEnabled;
			private set
			{
				saveData.IsMusicEnabled = value;
				Save();
				SetMixerMusicVolume(value ? MusicVolume : 0f);
			}
		}
		public bool IsSoundEnabled
		{
			get => saveData.IsSoundEnabled;
			private set
			{
				saveData.IsSoundEnabled = value;
				Save();
				SetMixerSoundVolume(value ? SoundVolume : 0f);
			}
		}
		public float MusicVolume
		{
			get => saveData.MusicVolume;
			private set
			{
				saveData.MusicVolume = value;
				Save();
				SetMixerMusicVolume(value);
			}
		}
		public float SoundVolume
		{
			get => saveData.SoundVolume;
			private set
			{
				saveData.SoundVolume = value;
				Save();
				SetMixerSoundVolume(value);
			}
		}


		private AudioMixer Mixer => settings.Mixer;


		private void Save() =>
			preferenceService.SaveValue(saveData);


		public AudioService(
				Settings settings,
				IPreferenceService preferenceService
			)
		{
			this.settings = settings;
			this.preferenceService = preferenceService;

			saveData = preferenceService.LoadValue(new SaveData());
			SetMixerMusicVolume(IsMusicEnabled ? MusicVolume : 0f);
			SetMixerSoundVolume(IsSoundEnabled ? SoundVolume : 0f);
		}


		void IInitializable.Initialize()
		{
			audioSource = GameObject.Instantiate(settings.PrefabAudioSource);
			audioSource.gameObject.AddComponent<AudioListener>();
		}


		public bool TryGetDataInfo(SoundId id, out SoundDataInfo info)
		{
			if (!DataBase.TryGetDataInfo(id, out info))
			{
				Debug.LogError($"There is no SoundInfo with id \"{id}\"");
				return false;
			}
			return true;
		}


		public bool IsSoundExist(Guid id) =>
			playbacks.TryGetValue(id, out _);


		public void SetMusicEnabled(bool value) =>
			IsMusicEnabled = value;


		public void SetSoundEnabled(bool value) =>
			IsSoundEnabled = value;



		public void PlayOneShot(SoundId id, float fadeOutDuration) =>
			PlaySound(id, fadeOutDuration: fadeOutDuration);


		public void PlayOneShotLimited(SoundId id, float delayBeforeSame)
		{
			if (!lastTimePlayed.ContainsKey(id))
			{
				lastTimePlayed.Add(id, DateTime.MinValue);
				nextDelay.Add(id, 0f);
			}

			if ((DateTime.Now - lastTimePlayed[id]).Milliseconds > nextDelay[id])
			{
				nextDelay[id] = delayBeforeSame * 1000f;
				lastTimePlayed[id] = DateTime.Now;
				PlayOneShot(id);
			}
		}


		public void PlayOneShot(SoundId id)
		{
			if (TryGetDataInfo(id, out var info))
			{
				var clip = info.RandomClip;
				if (clip != null)
					PlayOneShot(clip);
				else
					Debug.LogError($"There is no AudioClip for id \"{id}\"");
			}
		}


		public void PlayOneShot(AudioClip clip) =>
			audioSource.PlayOneShot(clip);


		public Guid? PlaySound(SoundId id, bool useSelfDestruction = true, float fadeOutDuration = 0f)
		{
			if (TryGetDataInfo(id, out var info))
			{
				var clip = info.RandomClip;
				if (clip != null)
					return PlaySound(clip, id.ToString(), useSelfDestruction, fadeOutDuration);
				else
					Debug.LogError($"There is no AudioClip for id \"{id}\"");
			}

			return null;
		}


		public Guid? PlaySound(AudioClip clip, string name = null, bool useSelfDestruction = true, float fadeOutDuration = 0f)
		{
			var source = GameObject.Instantiate(settings.PrefabAudioSource);
			SetName(source, name, "Sound");

			// source.transform.position = position;

			var playback = new Playback(clip, source);
			AddPlayback(playback, useSelfDestruction);

			if (fadeOutDuration > 0f)
			{
				var time = playback.Lenght - fadeOutDuration;
				if (time >= 0f)
				{
					fadeTween.Kill();
					fadeTween = DOVirtual.DelayedCall(time, () => { FadeOut(playback.Id, fadeOutDuration); });
				}
			}

			return playback.Id;
		}


		public Guid? PlayFromRecourses(string name)
		{
			var path = Path.Combine(settings.SoundsFolderPath, name);
			var clip = Resources.Load<AudioClip>(path);
			if (clip == null)
			{
				Debug.LogError($"Can't load sound \"{path}\"");
				return null;
			}

			return PlaySound(clip);
		}


		public Guid? PlaySoundLoop(SoundId id)
		{
			if (TryGetDataInfo(id, out var info))
			{
				var clip = info.RandomClip;
				if (clip != null)
					return PlaySoundLoop(clip, id.ToString());
				else
					Debug.LogError($"There is no AudioClip for id \"{id}\"");
			}
			return null;
		}


		public Guid PlaySoundLoop(AudioClip clip, string name = null)
		{
			var source = GameObject.Instantiate(settings.PrefabLoopedAudioSource);
			SetName(source, name, "Sound Loop");
			var playback = new Playback(clip, source);
			AddPlayback(playback, false);
			return playback.Id;
		}


		public Guid PlayMusic(AudioClip clip)
		{
			var source = GameObject.Instantiate(settings.PrefabMusicSource);
			SetName(source, null, "Music");
			var playback = new Playback(clip, source);
			AddPlayback(playback, false);
			return playback.Id;
		}


		public void Stop(Guid id)
		{
			if (!TryGetPlayback(id, out var playback))
				return;
			RemoveFromExpiring(id);
			playback.Stop();
		}


		public void Play(Guid id, bool useSelfDestruction = true)
		{
			if (!TryGetPlayback(id, out var playback))
				return;
			if (useSelfDestruction)
				AddToExpiring(playback.ExpireTime, playback);
			playback.Play();
		}


		public void Destroy(Guid id)
		{
			RemoveFromExpiring(id);
			DestroyPlayback(id);
		}


		public void ChangeClip(Guid id, SoundId TSoundId, bool keepPlaying, bool saveTime)
		{
			if (!TryGetPlayback(id, out var playback))
				return;

			if (!TryGetDataInfo(TSoundId, out var info))
				return;

			var clip = info.RandomClip;
			ChangeClip(id, clip, keepPlaying, saveTime);
		}


		public void ChangeClip(Guid id, AudioClip clip, bool keepPlaying, bool saveTime)
		{
			if (TryGetPlayback(id, out var playback))
				playback.ChangeClip(clip, keepPlaying, saveTime);
		}


		public float GetPlaybackTime(Guid id)
		{
			if (TryGetPlayback(id, out var playback))
				return playback.PlaybackTime;

			return 0f;
		}


		public void SetPlaybackTime(Guid id, float time)
		{
			if (TryGetPlayback(id, out var playback))
				playback.PlaybackTime = time;
		}


		public float GetPlaybackPitch(Guid id)
		{
			if (TryGetPlayback(id, out var playback))
				return playback.Pitch;

			return 1f;
		}


		public void SetPlaybackPitch(Guid id, float pitch)
		{
			if (TryGetPlayback(id, out var playback))
				playback.Pitch = pitch;
		}


		public Guid? FadeIn(Guid id, float duration, Action callback = null)
		{
			if (!TryGetPlayback(id, out var playback))
				return null;
			return CreateFade(id, 0f, playback.Volume, duration, callback);
		}


		public Guid? FadeOut(Guid id, float duration, Action callback = null)
		{
			if (!TryGetPlayback(id, out var playback))
				return null;
			return CreateFade(id, playback.Volume, 0f, duration, callback);
		}


		public Guid? CreateFade(Guid id, float from, float to, float duration, Action callback = null)
		{
			if (!TryGetPlayback(id, out var playback))
				return null;
			playback.Volume = from;

			var fade = new Fade
			{
				PlaybackId = id,
				Duration = duration,
				From = from,
				To = to,
				OnComplete = callback,
			};

			fadeQueue.Enqueue(fade);
			return fade.Id;
		}


		public void StopFade(Guid id) =>
			deleteFadeSet.Add(id);


		public float GetVolume(Guid id)
		{
			if (playbacks.TryGetValue(id, out var playback))
				return playback.Volume;
			return 0f;
		}


		public void SetVolume(Guid id, float value)
		{
			if (playbacks.TryGetValue(id, out var playback))
				playback.Volume = value;
		}


		private bool TryGetPlayback(Guid id, out Playback playback)
		{
			if (!playbacks.TryGetValue(id, out playback))
			{
				Debug.LogError($"There is no playback with given id!");
				return false;
			}

			return true;
		}


		private void AddPlayback(Playback playback, bool useSelfDestruction = true)
		{
			playbacks.Add(playback.Id, playback);
			if (useSelfDestruction)
				AddToExpiring(playback.ExpireTime, playback);
		}


		private void AddToExpiring(float time, Playback playback)
		{
			if (!playback.IsLoop)
			{
				if (playbacksExpire.ContainsKey(time))
					AddToExpiring(time + 0.0001f, playback);
				else
					playbacksExpire.Add(time, playback.Id);
			}
		}


		private void DestroyPlayback(Guid id)
		{
			if (!playbacks.TryGetValue(id, out var playback))
			{
				Debug.LogError($"There is no playback with given id!");
				return;
			}

			playback.Destroy();
			playbacks.Remove(id);
		}


		private void RemoveFromExpiring(Guid id)
		{
			for (int i = 0; i < playbacksExpire.Values.Count; i++)
			{
				var item = playbacksExpire.Values[i];
				if (item == id)
				{
					playbacksExpire.RemoveAt(i);
					break;
				}
			}
		}


		void ITickable.Tick()
		{
			while (playbacksExpire.Count > 0)
			{
				var id = playbacksExpire.Values[0];
				if (!TryGetPlayback(id, out var playback))
				{
					playbacksExpire.RemoveAt(0);
					continue;
				}

				if (!playback.IsExpired)
					break;

				playbacksExpire.RemoveAt(0);
				DestroyPlayback(id);
				OnPlayingDone?.Invoke(id);
			}

			FadesUpdate();
		}


		private void FadesUpdate()
		{
			for (int i = 0; i < fadeQueue.Count; i++)
			{
				Fade fade = fadeQueue.Dequeue();
				if (deleteFadeSet.Contains(fade.Id))
				{
					deleteFadeSet.Remove(fade.Id);
					continue;
				}

				if (!playbacks.TryGetValue(fade.PlaybackId, out var playback)
					|| playback.IsExpired)
					continue;

				float scaledProgress = fade.Progress * fade.Duration + Time.unscaledDeltaTime;
				float progress = scaledProgress / fade.Duration;
				float volume = Mathf.Lerp(fade.From, fade.To, progress);

				if (progress >= 1f)
				{
					playback.Volume = fade.To;
					fade.OnComplete?.Invoke();
					continue;
				}

				playback.Volume = volume;
				fade.Progress = progress;

				fadeQueue.Enqueue(fade);
			}
		}


		private void SetMixerSoundVolume(float value) =>
			Mixer.SetFloat(SOUND_VOLUME_KEY, VolumeNormalizeToDecibel(value));


		private void SetMixerMusicVolume(float value) =>
			Mixer.SetFloat(MUSIC_VOLUME_KEY, VolumeNormalizeToDecibel(value));


		private void SetName(AudioSource source, string name, string type)
		{
			if (string.IsNullOrEmpty(name))
				name = "AudioSource";

			source.name = $"{name} - {type}";
		}


		public static float VolumeNormalizeToDecibel(float value)
		{
			float dB;
			if (!Mathf.Approximately(value, 0))
			{
				// Формула из экзамплов Unity.
				dB = 20.0f * Mathf.Log10(value);
			}
			else
			{
				dB = MIN_DECIBEL_LEVEL;
			}

			return dB;
		}


		#region Shortcuts

		public void PlayClickSound(bool isAvailable = true)
		{
			var sound = isAvailable ? SoundId.button : SoundId.button_locked;
			PlayOneShot(sound);
		}


		public void PlayShowScreen() =>
			PlayOneShot(SoundId.window_screen_open);

		#endregion



		private class Fade
		{
			public Guid Id = Guid.NewGuid();
			public Guid PlaybackId;
			public float Duration;
			public float Progress = 0f;
			public float From;
			public float To;
			public Action OnComplete;
		}


		[Serializable]
		public class SaveData
		{
			public float MusicVolume = 1f;
			public float SoundVolume = 1f;
			public bool IsMusicEnabled = true;
			public bool IsSoundEnabled = true;
		}


		[Serializable]
		public class Settings
		{
			[field: SerializeField] public SoundDatabase DataBase { get; private set; }
			[field: SerializeField] public AudioMixer Mixer { get; private set; }
			[field: SerializeField] public AudioSource PrefabAudioSource { get; private set; }
			[field: SerializeField] public string SoundsFolderPath { get; private set; }
			[field: SerializeField] public AudioSource PrefabLoopedAudioSource { get; private set; }
			[field: SerializeField] public AudioSource PrefabMusicSource { get; private set; }
		}
	}
}
