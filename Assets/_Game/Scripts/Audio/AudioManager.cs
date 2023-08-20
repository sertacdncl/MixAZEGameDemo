using UnityEngine;

namespace Game
{
	public class AudioManager : Singleton<AudioManager>
	{
		[SerializeField] private AudioSource audioSource;
		public void PlaySound(AudioClip clip, float pitch=1f, float volume=1f)
		{
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(clip,volume);
			// Play sound
		}
	}
}