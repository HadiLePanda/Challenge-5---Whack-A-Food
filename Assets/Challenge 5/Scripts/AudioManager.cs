using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    public static AudioManager singleton;

    private void Awake()
    {
        singleton = this;
    }

    public void PlaySound2DOneShot(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic() => musicSource.Play();
    public void StopMusic() => musicSource.Stop();

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}
