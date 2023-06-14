using System;
using UnityEngine;


namespace TheProxor.Services.Audio
{
	public interface IAudioService<TSoundId> where TSoundId : Enum
	{
		event Action<Guid> OnPlayingDone;
		bool IsMusicEnabled { get; }
		bool IsSoundEnabled { get; }
		float MusicVolume { get; }
		float SoundVolume { get; }

		bool TryGetDataInfo(TSoundId id, out SoundDataInfo info);

		bool IsSoundExist(Guid id);

		void SetMusicEnabled(bool value);

		void SetSoundEnabled(bool value);

		void PlayOneShot(TSoundId id, float fadeOutDuration);

		void PlayOneShot(TSoundId id);

		void PlayOneShot(AudioClip clip);

		void PlayOneShotLimited(TSoundId id, float delayBeforeSame);

		Guid? PlaySound(TSoundId id, bool useSelfDestruction = true, float fadeOutDuration = 0f);

		Guid? PlaySound(AudioClip clip, string name = null, bool useSelfDestruction = true, float fadeOutDuration = 0f);

		Guid? PlayFromRecourses(string name);

		Guid? PlaySoundLoop(TSoundId id);

		Guid PlaySoundLoop(AudioClip clip, string name = null);

		Guid PlayMusic(AudioClip clip);

		void Stop(Guid id);

		void Play(Guid id, bool useSelfDestruction = true);

		void Destroy(Guid id);

		void ChangeClip(Guid id, TSoundId TSoundId, bool keepPlaying, bool saveTime);

		void ChangeClip(Guid id, AudioClip clip, bool keepPlaying, bool saveTime);

		float GetPlaybackTime(Guid id);

		void SetPlaybackTime(Guid id, float time);

		float GetPlaybackPitch(Guid id);

		void SetPlaybackPitch(Guid id, float pitch);

		Guid? FadeIn(Guid id, float duration, Action callback = null);

		Guid? FadeOut(Guid id, float duration, Action callback = null);

		Guid? CreateFade(Guid id, float from, float to, float duration, Action callback = null);

		void StopFade(Guid id);

		float GetVolume(Guid id);

		void SetVolume(Guid id, float value);

		void PlayClickSound(bool isAvailable = true);

		void PlayShowScreen();
	}
}
