using UnityEngine.Audio;
using System;    
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
            
        s.source.Play();
    }
    
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source.isPlaying)
        {
            s.source.Stop();
        }
    }
    
    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }
    
    public void UpdatePitch(string name, float newPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.pitch = newPitch;
        }
    }
    
    public float GetClipLength(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            return s.clip.length;
        }
        else
        {
            return 0f;
        }
    }
    
    void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            CardEffectActions.SetAudioManager(audioManager);
        }
    }

    public void PlayRandomBackgroundMusic()
    {
        StartCoroutine(PlayBackgroundMusicCoroutine());
    }
    
    IEnumerator PlayBackgroundMusicCoroutine()
    {
        string[] backgroundSounds = { "FirstBattleMusic", "SecondBattleMusic" };

        int currentSoundIndex = UnityEngine.Random.Range(0, backgroundSounds.Length);
        while (true)
        {
            string soundToPlay = backgroundSounds[currentSoundIndex];
            
            float fadeInDuration = 5f;
            yield return StartCoroutine(FadeInSound(soundToPlay, fadeInDuration));
            
            float clipLength = GetClipLength(soundToPlay);
            
            float fadeOutDuration = 5f;
            yield return new WaitForSeconds(clipLength - fadeOutDuration);
            
            yield return StartCoroutine(FadeOutSound(soundToPlay, fadeOutDuration));
            
            Stop(soundToPlay);
            
            yield return new WaitForSeconds(1.5f);
            
            currentSoundIndex = (currentSoundIndex + 1) % backgroundSounds.Length;
        }
    }
    
    IEnumerator FadeInSound(string name, float duration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            yield break;
        }

        s.source.volume = 0f;
        s.source.Play();

        float targetVolume = s.volume;
        float startVolume = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        s.source.volume = targetVolume;
    }
    
    IEnumerator FadeOutSound(string name, float duration)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            yield break;
        }

        float startVolume = s.source.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        s.source.volume = 0f;
        s.source.Stop();
    }
    
    public void PlayRandomEnemyDie()
    {
        string[] enemyDieSounds = { "EnemyDie1", "EnemyDie2", "EnemyDie3", "EnemyDie4", "EnemyDie5" };
        string randomSound = enemyDieSounds[UnityEngine.Random.Range(0, enemyDieSounds.Length)];
        Play(randomSound);
    }
    
    public void PlayRandomPowerUp()
    {
        string[] powerUpSounds = { "PowerUp1", "PowerUp2" };
        string randomSound = powerUpSounds[UnityEngine.Random.Range(0, powerUpSounds.Length)];
        Play(randomSound);
    }
    
    public void PlayRandomDebuff()
    {
        string[] debuffSounds = { "Debuff1", "Debuff2" };
        string randomSound = debuffSounds[UnityEngine.Random.Range(0, debuffSounds.Length)];
        Play(randomSound);
    }
    
    public void PlayRandomSoundEffect()
    {
        StartCoroutine(PlayRandomSoundEffectCoroutine());
    }
    
    IEnumerator PlayRandomSoundEffectCoroutine()
    {
        string[] enemyDieSounds = { "Gleam1", "Gleam2", "Gleam3", "Gleam4" };

        while (true)
        {
            float waitTime = UnityEngine.Random.Range(10f, 20f);
            yield return new WaitForSeconds(waitTime);
            
            string randomSound = enemyDieSounds[UnityEngine.Random.Range(0, enemyDieSounds.Length)];
            Play(randomSound);
        }
    }
}
