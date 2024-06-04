using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroToBattleChanger : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnIntroFinished;
        audioManager = FindObjectOfType<AudioManager>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Intro.mp4");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Battle");
            audioManager.PlayRandomSoundEffect();
            audioManager.PlayRandomBackgroundMusic();
        }
    }

    private void OnIntroFinished(VideoPlayer player)
    {
        SceneManager.LoadScene("Battle");
        audioManager.PlayRandomSoundEffect();
        audioManager.PlayRandomBackgroundMusic();
    }
}