using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSceneManager : MonoBehaviour
{
    public AudioSource wipeAS, musicAS;
    public EngineRevving engine;

    public void PlayWipeNoise()
    {
        wipeAS.Play();
    }

    public void PlayMusic()
    {
        musicAS.Play();
        engine.FadeInEngine();
    }

    public void SetMusicVolume(float val)
    {
        musicAS.volume = val;
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
