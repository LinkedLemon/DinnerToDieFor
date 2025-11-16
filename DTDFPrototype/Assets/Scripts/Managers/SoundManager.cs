using System.Collections.Generic;
using UnityEngine;

public enum MusicType
{
    Menu,
    Game
}

public class SoundManager : MonoBehaviour
{
    //To send a sound to this script put "AudioManager.Instance." and then input the needed function with the enum


    public static SoundManager instance;

    [SerializeField] private AudioSource _sfxSource, _musicSource;

    [SerializeField] private List<AudioClip> m_musicClips = new List<AudioClip>();



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayMusic(MusicType music, float volume)
    {
        instance._musicSource.Stop();
        instance._musicSource.clip = instance.m_musicClips[(int)music];
        instance._musicSource.volume = volume;
        instance._musicSource.Play();

    }

    public void PlayAudioClip(AudioClip audioClip,float volume)
    {
        instance._sfxSource.PlayOneShot(audioClip, volume);
    }
}
